// Minimal raw-SPI NRF24L01 driver. Same code for both boards;
// one calls nrf_start_tx(), the other nrf_start_rx().
#include <stdint.h>
#include <stdbool.h>
#include <string.h>

// ---- YOU PROVIDE THESE (SPIM2 transfer, GPIO, delays) ----
// spi_transfer: full duplex, mode 0, MSB first, rx may be NULL.
void spi_transfer(const uint8_t *tx, uint8_t *rx, uint32_t len);
void gpio_write(uint32_t pin, bool level);
bool gpio_read(uint32_t pin);
void delay_us(uint32_t us);
void delay_ms(uint32_t ms);

// nRF52 pin number = port * 32 + pin
#define PIN(port, n) ((port) * 32 + (n))
#define NRF_CE   PIN(1, 7)    // Pro Micro 33 -> module pin 3
#define NRF_CSN  PIN(1, 11)   // Pro Micro 15 -> module pin 4
#define NRF_IRQ  PIN(1, 6)    // Pro Micro 12 -> module pin 8 (optional, active low)

// Commands
#define CMD_R_REG    0x00
#define CMD_W_REG    0x20
#define CMD_R_RX     0x61
#define CMD_W_TX     0xA0
#define CMD_FLUSH_TX 0xE1
#define CMD_FLUSH_RX 0xE2
#define CMD_NOP      0xFF

// Registers
#define REG_CONFIG      0x00
#define REG_EN_AA       0x01
#define REG_EN_RXADDR   0x02
#define REG_SETUP_AW    0x03
#define REG_SETUP_RETR  0x04
#define REG_RF_CH       0x05
#define REG_RF_SETUP    0x06
#define REG_STATUS      0x07
#define REG_RX_ADDR_P0  0x0A
#define REG_TX_ADDR     0x10
#define REG_RX_PW_P0    0x11
#define REG_FIFO_STATUS 0x17

// STATUS bits
#define ST_RX_DR  0x40
#define ST_TX_DS  0x20
#define ST_MAX_RT 0x10

#define PAYLOAD_LEN 32
#define RF_CHANNEL  76
static const uint8_t ADDR[5] = { 'D', 'V', 'R', '0', '1' };   // same on both boards

// One SPI transfer per command, with CSN low for the whole thing.
// (The Renode model ends a command when CSN goes high.)
static uint8_t nrf_cmd(uint8_t cmd, const uint8_t *tx, uint8_t *rx, uint8_t len)
{
    uint8_t txb[1 + PAYLOAD_LEN], rxb[1 + PAYLOAD_LEN];
    txb[0] = cmd;
    if (tx) memcpy(&txb[1], tx, len); else memset(&txb[1], 0, len);

    gpio_write(NRF_CSN, 0);
    spi_transfer(txb, rxb, len + 1);
    gpio_write(NRF_CSN, 1);

    if (rx) memcpy(rx, &rxb[1], len);
    return rxb[0];                      // first byte back is always STATUS
}

static void    write_reg(uint8_t reg, uint8_t v) { nrf_cmd(CMD_W_REG | reg, &v, NULL, 1); }
static uint8_t read_reg(uint8_t reg)             { uint8_t v; nrf_cmd(CMD_R_REG | reg, NULL, &v, 1); return v; }
static void    write_addr(uint8_t reg)           { nrf_cmd(CMD_W_REG | reg, ADDR, NULL, 5); }

static void nrf_common_init(void)
{
    gpio_write(NRF_CSN, 1);
    gpio_write(NRF_CE, 0);
    delay_ms(5);

    write_reg(REG_SETUP_AW, 0x03);          // 5-byte addresses
    write_reg(REG_RF_CH, RF_CHANNEL);
    write_reg(REG_RF_SETUP, 0x06);          // 1 Mbps, 0 dBm
    write_reg(REG_EN_AA, 0x01);             // auto-ack on pipe 0
    write_reg(REG_SETUP_RETR, 0x00);
    write_reg(REG_RX_PW_P0, PAYLOAD_LEN);   // fixed payload length
    nrf_cmd(CMD_FLUSH_TX, NULL, NULL, 0);
    nrf_cmd(CMD_FLUSH_RX, NULL, NULL, 0);
    write_reg(REG_STATUS, ST_RX_DR | ST_TX_DS | ST_MAX_RT);   // clear flags
}

// ---- Receiver ----
void nrf_start_rx(void)
{
    nrf_common_init();
    write_addr(REG_RX_ADDR_P0);
    write_reg(REG_EN_RXADDR, 0x01);         // pipe 0 only
    write_reg(REG_CONFIG, 0x0B);            // EN_CRC | PWR_UP | PRIM_RX
    delay_ms(2);                            // power-up settling
    gpio_write(NRF_CE, 1);                  // CE high = listening
}

// Returns true and fills buf (PAYLOAD_LEN bytes) if a packet was waiting.
bool nrf_receive(uint8_t *buf)
{
    if (read_reg(REG_FIFO_STATUS) & 0x01) return false;   // RX_EMPTY
    nrf_cmd(CMD_R_RX, NULL, buf, PAYLOAD_LEN);
    if (read_reg(REG_FIFO_STATUS) & 0x01)
        write_reg(REG_STATUS, ST_RX_DR);                   // FIFO drained: clear flag
    return true;
}

// ---- Transmitter ----
void nrf_start_tx(void)
{
    nrf_common_init();
    write_addr(REG_TX_ADDR);
    write_addr(REG_RX_ADDR_P0);             // pipe 0 receives the auto-ack
    write_reg(REG_EN_RXADDR, 0x01);
    write_reg(REG_CONFIG, 0x0A);            // EN_CRC | PWR_UP, PRIM_RX = 0
    delay_ms(2);
}

// data must be PAYLOAD_LEN bytes. Returns true if the packet was delivered.
bool nrf_send(const uint8_t *data)
{
    nrf_cmd(CMD_W_TX, data, NULL, PAYLOAD_LEN);
    gpio_write(NRF_CE, 1);
    delay_us(15);                           // CE pulse > 10 us
    gpio_write(NRF_CE, 0);

    uint8_t st = 0;
    for (int i = 0; i < 1000; i++) {        // poll STATUS via NOP
        st = nrf_cmd(CMD_NOP, NULL, NULL, 0);
        if (st & (ST_TX_DS | ST_MAX_RT)) break;
        delay_us(100);
    }
    write_reg(REG_STATUS, ST_TX_DS | ST_MAX_RT);
    if (st & ST_MAX_RT) nrf_cmd(CMD_FLUSH_TX, NULL, NULL, 0);
    return (st & ST_TX_DS) != 0;
}

/* Usage:
 *
 * // controller
 * nrf_start_tx();
 * delay_ms(100);                       // let the receiver start listening first
 * uint8_t msg[PAYLOAD_LEN] = "hello";
 * bool ok = nrf_send(msg);             // print ok on UART
 *
 * // headset
 * nrf_start_rx();
 * uint8_t buf[PAYLOAD_LEN];
 * while (1) { if (nrf_receive(buf)) { /\* print buf on UART *\/ } }
 */