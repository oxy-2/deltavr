// Minimal raw-SPI LSM6DSV driver (polling, no FIFO, no interrupts).
// Uses the same spi_transfer / gpio / delay helpers as nrf24_raw.c.
#include <stdint.h>
#include <stdbool.h>
#include <string.h>

// ---- YOU PROVIDE THESE (same as for the radio driver) ----
// spi_transfer: full duplex, mode 0, MSB first.
void spi_transfer(const uint8_t *tx, uint8_t *rx, uint32_t len);
void gpio_write(uint32_t pin, bool level);
void delay_us(uint32_t us);
void delay_ms(uint32_t ms);

// nRF52 pin number = port * 32 + pin
#define PIN(port, n) ((port) * 32 + (n))
#define IMU_CS   PIN(0, 2)    // Pro Micro 18 -> LSM6DSV pin 12
#define IMU_INT1 PIN(1, 1)    // not used by this driver
#define IMU_INT2 PIN(1, 2)    // not used by this driver

// Registers
#define IMU_CTRL1      0x10   // accel ODR + mode
#define IMU_CTRL2      0x11   // gyro ODR + mode
#define IMU_CTRL3      0x12   // BDU, IF_INC, SW_RESET
#define IMU_CTRL6      0x15   // gyro full scale
#define IMU_CTRL8      0x17   // accel full scale
#define IMU_WHO_AM_I   0x0F
#define IMU_STATUS     0x1E
#define IMU_OUT_TEMP_L 0x20
#define IMU_OUTX_L_G   0x22   // 6 bytes gyro, then 6 bytes accel at 0x28

#define IMU_ID 0x70

typedef struct {
    int32_t accel_mg[3];      // milli-g
    int32_t gyro_mdps[3];     // milli-degrees per second
    int32_t temp_mc;          // milli-degrees C
} imu_sample_t;

// One SPI transfer per access, CS low for the whole thing.
// Bit 7 of the first byte: 1 = read, 0 = write. Address auto-increments.
static void imu_read(uint8_t reg, uint8_t *out, uint8_t len)
{
    uint8_t txb[1 + 12] = { 0 }, rxb[1 + 12];
    txb[0] = 0x80 | reg;
    gpio_write(IMU_CS, 0);
    spi_transfer(txb, rxb, len + 1);
    gpio_write(IMU_CS, 1);
    memcpy(out, &rxb[1], len);
}

static uint8_t imu_read_reg(uint8_t reg)
{
    uint8_t v;
    imu_read(reg, &v, 1);
    return v;
}

static void imu_write_reg(uint8_t reg, uint8_t value)
{
    uint8_t txb[2] = { reg & 0x7F, value };
    gpio_write(IMU_CS, 0);
    spi_transfer(txb, NULL, 2);
    gpio_write(IMU_CS, 1);
}

// Returns true if the chip answered with the expected WHO_AM_I.
bool imu_init(void)
{
    gpio_write(IMU_CS, 1);
    delay_ms(20);                               // power-on settling

    if (imu_read_reg(IMU_WHO_AM_I) != IMU_ID) {
        return false;
    }

    imu_write_reg(IMU_CTRL3, 0x01);             // SW_RESET
    for (int i = 0; i < 100 && (imu_read_reg(IMU_CTRL3) & 0x01); i++) {
        delay_ms(1);
    }

    imu_write_reg(IMU_CTRL3, 0x44);             // BDU = 1, IF_INC = 1
    imu_write_reg(IMU_CTRL8, 0x01);             // accel +-4 g   (0.122 mg/LSB)
    imu_write_reg(IMU_CTRL6, 0x04);             // gyro  +-2000 dps (70 mdps/LSB)
    imu_write_reg(IMU_CTRL1, 0x06);             // accel 120 Hz, high-performance
    imu_write_reg(IMU_CTRL2, 0x06);             // gyro  120 Hz, high-performance
    delay_ms(20);                               // let the first samples settle
    return true;
}

static int16_t le16(const uint8_t *p)
{
    return (int16_t)((uint16_t)p[0] | ((uint16_t)p[1] << 8));
}

// Returns true if a fresh sample was read (accel and gyro data-ready set).
bool imu_read_sample(imu_sample_t *s)
{
    if ((imu_read_reg(IMU_STATUS) & 0x03) != 0x03) {
        return false;
    }

    uint8_t d[12];
    imu_read(IMU_OUTX_L_G, d, 12);              // gyro X,Y,Z then accel X,Y,Z
    for (int i = 0; i < 3; i++) {
        s->gyro_mdps[i] = (int32_t)le16(&d[2 * i]) * 70;
        s->accel_mg[i]  = (int32_t)le16(&d[6 + 2 * i]) * 122 / 1000;
    }

    uint8_t t[2];
    imu_read(IMU_OUT_TEMP_L, t, 2);
    s->temp_mc = 25000 + ((int32_t)le16(t) * 1000) / 256;   // 256 LSB/degC, 0 at 25 C
    return true;
}

/* Usage:
 *
 * if (!imu_init()) { // print "IMU not found" on the UART }
 * imu_sample_t s;
 * while (1) {
 *     if (imu_read_sample(&s)) {
 *         // print s.accel_mg[0..2], s.gyro_mdps[0..2], s.temp_mc on the UART
 *     }
 *     delay_ms(10);
 * }
 */