# DeltaVR Renode starter kit

Simulates both boards (headset + controller) from your schematics together,
in one Renode session, running your real compiled firmware ELFs. Verified
against Renode's own docs/examples for syntax; the register-level sensor
behavior is a deliberately simplified starting point, not a datasheet-accurate
model.

## What's here

```
platforms/nicenano_deltavr.repl   # nRF52840 base + LSM6DSVTR/ADS1115/nRF24L01 stubs
peripherals/NRF24L01Stub.cs       # custom SPI peripheral bridging TX/RX between boards
peripherals/lsm6dsv_i2c_stub.py   # WHO_AM_I + synthetic accel/gyro over I2C
peripherals/ads1115_i2c_stub.py   # synthetic ADC conversion register over I2C
scripts/deltavr_two_boards.resc   # boots both machines together
```

## Before running

1. **Install Renode** (nightly recommended, since ad-hoc C# compilation and
   `Mocks.DummyI2CSlave`/`DummySPISlave` are relatively recent additions):
   https://renode.readthedocs.io/en/latest/introduction/installing.html

2. **Fill in the real GPIO pin numbers.** The `.repl` file has placeholder
   pin numbers (CSN/CE/IRQ) marked `TODO`. I intentionally didn't guess these
   from the schematic image — get them from your actual devicetree overlay
   or firmware pinctrl definitions, since a wrong pin number here will look
   like a working sim that silently never talks to the radio. The good news:
   nRF52840 has exactly two GPIO ports (P0 = `gpio0`, pins 0-31; P1 = `gpio1`,
   pins 0-15), matching the P0.xx / P1.xx labels already in your schematic
   directly — no translation needed, just transcription.

3. **Build real firmware ELFs.** Renode emulates the actual Cortex-M4 core,
   so it runs the same ARM ELF you'd flash to hardware (Zephyr build output,
   typically `build/zephyr/zephyr.elf`) — not a native/host build. Point the
   `sysbus LoadELF` lines in the `.resc` at those paths.

4. Confirm the LSM6DSVTR I2C address (`0x6A` vs `0x6B`, set by SDO/SA0) and
   the WHO_AM_I value against your exact part revision's datasheet — I used
   `0x70`, which is correct for the LSM6DSV16X family at time of writing but
   worth double-checking.

## Running

```
renode scripts/deltavr_two_boards.resc
```

Two UART analyzer windows should open (one per board). If your firmware logs
over UART on boot, you should see it immediately, independent of whether the
sensor/radio stubs are wired correctly yet — good first sanity check.

## How the pieces work

- **LSM6DSVTR / ADS1115** use Renode's built-in `Mocks.DummyI2CSlave`, driven
  by small Python scripts that watch for register-address writes and hand
  back either `WHO_AM_I`/config values or synthetic sensor data. Extend the
  `_on_data_received` handlers if your firmware needs writes (ODR, scale,
  MUX select) to actually change what reads come back.

- **nRF24L01** is a custom SPI peripheral (`NRF24L01Stub.cs`) decoding the
  real nRF24 command set (`W_TX_PAYLOAD`, `R_RX_PAYLOAD`, `FLUSH_TX/RX`,
  `R/W_REGISTER`, `NOP`) and a `STATUS`/IRQ model good enough for basic
  link-layer testing. On `CE` going high with data in the TX FIFO, it
  delivers the payload directly to whichever *other* `NRF24L01Stub` instance
  has registered itself (via `NodeId`) — this only works correctly for
  exactly two nodes in one Renode process; it doesn't model channel/address
  filtering, ACKs, or retransmission at all.

## Known limitations / what I'd tighten next

- No auto-ack, no retries, no RF channel modeling on the nRF24 stub — if
  your protocol depends on ACK payloads or `MAX_RT` handling, that needs
  adding to `NRF24L01Stub.cs`.
- Sensor stubs return canned/sine-wave data, not physically consistent
  motion — fine for protocol/parsing tests, not for validating actual
  sensor-fusion math against expected physical behavior.
- `IRQ` polarity is asserted as "true = event pending" in the C# stub; if
  your firmware expects a hardware-inverted active-low line at the GPIO
  level (common with these breakout boards), you may need to flip that in
  `UpdateIrq()`.
- I have not run this end-to-end against your exact firmware — treat it as
  a verified-syntax starting point to iterate from, not a drop-in working
  simulation on the first try.

## Handy commands once it's running

```
(monitor) mach set "headset"
(headset) sysbus.nrf24 NodeId          # confirm registration
(monitor) mach set "controller"
```
