# DeltaVR KiCad Libraries + Wiring Guide

Everything you need to draw the DeltaVR PCBs (HMD + 2 controllers) in KiCad.
Libraries are already cloned into this folder. Point KiCad at them (or use the
`sym-lib-table` / `fp-lib-table` already created next to `kicad/deltavr hmd.kicad_pro`).

Your project is KiCad 9 (`.kicad_pro` has `component_class_settings`). The ST LSM6DSV
library (v6 format), the Nordic lib and everything else load fine in 8/9/10.

---

## 1. What you have, and where it comes from

| Part | Symbol | Footprint | 3D model | Where it lives |
|---|---|---|---|---|
| LSM6DSVTR IMU | `DeltaVR_ST_LSM6DSV:LSM6DSVTR` | `DeltaVR_ST_LSM6DSV` → `LGA14-L_2P59X3P1X0P5_STM` | yes (STEP in `lsm6dsvtr/LGA14-L_2p59x3P1x0P5_STM.step`) | ST's official export, `lsm6dsvtr/KiCADv6/` (you already added this) |
| nRF52840-QIAA (aQFN73, 7x7) | `DeltaVR_Nordic_nRF52:nRF52840-QIXX` | **official KiCad** `Package_DFN_QFN:Nordic_AQFN-73-1EP_7x7mm_P0.5mm` | official KiCad `.step` | symbol: `nordic-lib-kicad/symbols/` |
| nRF52840-QFAA (QFN48, 6x6) | `DeltaVR_Nordic_nRF52:nRF52840-QFXX` | **official KiCad** `Package_DFN_QFN:QFN-48-1EP_6x6mm_P0.4mm_EP4.6x4.6mm_ThermalVias` | official KiCad `.step` | symbol: `nordic-lib-kicad/symbols/` |
| MIFA 2.4 GHz antenna | none (PCB trace, no symbol needed) | `DeltaVR_MIFA_Antenna:ESPRESSIF_ESP32_MIFA_2.4GHz_Right` | none needed (it IS the copper) | `ESP32_MIFA_PCB_ANTENNA/ESP_Antenna.pretty/` |
| TBA459 TMR stick (PS5-style) | `DeltaVR_PS5_Stick:Joystick` | `DeltaVR_PS5_Stick:Joystick` | none | `thumbstick-breakout/kicad/` |
| 6x6x4.3 mm tact switch | official `Switch:SW_Push` | **official** `Button_Switch_THT:SW_PUSH_6mm_H4.3mm` | official | ships with KiCad |
| HT7333 LDO | official `Regulator_Linear:HT7333-A` (if missing, use generic `LD1117`-style or `Regulator_Linear` HT73xx) | **official** `Package_TO_SOT_SMD:SOT-89-3` (or `SOT-23-3` for the -A variant) | official | ships with KiCad |
| 0603 R/C | official `Device:R`, `Device:C` | **official** `Resistor_SMD:R_0603_1608Metric`, `Capacitor_SMD:C_0603_1608Metric` | official | ships with KiCad |
| Battery plug XH2.54-2P (JST XH 2.5 mm) | official `Connector:Conn_01x02_Pin` | **official** `Connector_JST:JST_XH_B2B-XH-A_1x02_P2.50mm_Vertical` | official | ships with KiCad |
| USB-C (headset) | official `Connector:USB_C_Receptacle_HRO_TYPE-C-31-M-12` | **official** `Connector_USB:USB_C_Receptacle_HRO_TYPE-C-31-M-12` | official | ships with KiCad |
| 32 MHz / 32.768 kHz crystals | official `Device:Crystal` | **official** `Crystal:Crystal_SMD_2012-2Pin_2.0x1.2mm` / `Crystal_SMD_3215-2Pin_3.2x1.5mm` | official | ships with KiCad |

**What ships with KiCad (yes, already installed):** everything in the right column
marked "official" — 0603 passives, SOT-89/SOT-23, the 6x6x4.3 tact, JST XH 2-pin,
USB-C, crystals, AND the nRF52840 QFN48/aQFN73 *footprints* (Nordic's own
`Nordic_AQFN-73` is bundled).

**What does NOT ship with KiCad (added in this folder):**
- nRF52840 **symbols** (the bundled `MCU_Nordic` only has a bare aQFN-73 variant; the
  hlord2000 lib has clean QFXX/QIXX symbols with all power pins).
- LSM6DSVTR (you already had this one).
- MIFA antenna footprint.
- PS5 stick module footprint.
- WLCSP nRF52 footprints (only needed if you ever use the CKXX ball package).

---

## 2. Which chip where, and do you still need the HT7333?

| Board | Chip | Power input | HT7333? |
|---|---|---|---|
| **Headset** (always wired) | **nRF52840-QIAA (aQFN73)** — *must* be the aQFN73, it is the only package with USB | USB 5V → HT7333 → 3.3V rail | **Yes, keep it.** 5V→3.3V for the MCU, IMU and everything else. |
| **Controllers** (18650) | nRF52840-QIAA or -QFAA | 18650 (3.0–4.2 V) → HT7333 → 3.3V rail | **Yes, keep it.** QFN48 cannot take 4.2 V directly (max 3.6 V on VDD), so you need the 3.3 V rail. |

Notes:

- **QFN48 vs aQFN73:** the QFN48 (`-QFAA`) has **no USB** and no VDDH — fine for
  controllers, useless for the headset. aQFN73 (`-QIAA`) does USB. If you only want to
  buy one part, use aQFN73 everywhere (7x7 mm, 0.4 mm pitch — same reflow difficulty as
  the QFN48).
- The HT7333 is **not needed for the IMU to work** — the LSM6DSV runs off your 3.3 V
  rail (VDD 1.71–3.6 V). The LDO's only job is making that rail. Headset: from 5 V USB.
  Controller: from the 18650.
- 18650 + LDO caveat: HT7333 drops out at ~3.35 V battery, so you "waste" the
  3.4→3.0 V region (~15–20 % of a 4500 mAh pack). With a 4500 mAh pack and a
  ~40 mA controller draw that's still ~90 h of runtime — not worth a buck converter.
  If you ever want max runtime, run the aQFN73 in high-voltage mode (battery → VDDH,
  2.5–5.5 V, no LDO) — but then all I/O is 1.8 V, so the stick/IMU must run at 1.8 V too.
  Keep it simple: LDO.
- **HT7333 pinout (SOT-89-3):** pin 1 = GND, pin 2 = VIN, pin 3 = VOUT, tab = VOUT.
  Caps: 1 µF in, 1 µF out (0603 ceramic is fine).

---

## 3. nRF52840 hookup (both boards)

Power / decoupling (from Nordic's reference layouts):

- VDD: 1× 100 nF + 1× 10 µF (0603/0805) as close as possible.
- **DCDC (recommended, saves ~40 % current on battery boards):** 10 µH inductor DCC→VDD,
  10 µF on VDD. Firmware enables DCDC. (If you skip it, tie DCC straight to VDD.)
- DEC1, DEC3, DEC4, DEC6: 100 nF each to GND (QFN48/QIAA; the aQFN73 also has DECUSB,
  see below).
- DECUSB (aQFN73 only): 1 µF to GND.
- RESET (P0.18): 10 kΩ pull-up to VDD + 100 nF to GND.
- SWD: 4-pin header (SWDIO=P0.21, SWDCLK=P0.20, RESET, GND) — cheap insurance, add it.
- NFC1/NFC2: leave unconnected (or use as GPIOs with the NFC peripheral disabled).
- 32 MHz crystal (XC1/XC2), e.g. 2012 SMD: 2× ~12 pF load caps (check crystal datasheet).
- 32.768 kHz crystal (LF): 2× ~6.8 pF. Not strictly required for ESB (LFCLK can use the
  internal RC), but include it — the reference designs all do, and it makes sleeps sane.
- USB (headset aQFN73 only): D+/D− through 27 Ω series resistors, ESD diodes, VBUS pin
  to 5 V. Optional VBUS detect: 100k/100k divider to a GPIO.

Antenna (ESB radio):

```
ANT pin ──┬── L1 (3.9 nH) ──┬── 50 Ω trace ──► MIFA feed pad (pad 1)
          │                 │
          C1 (1 pF)        C2 (1.8–2.2 pF)   (values from Nordic DK; tune with VNA)
          │                 │
         GND               GND
```

- Keep the trace from ANT to the antenna feed short and straight; pour ground on both
  sides of it (coplanar, ~0.3–0.4 mm gap).
- MIFA rules (this is where "large groundplane" comes from): the MIFA footprint goes at
  a board **edge/corner**, the antenna zone must have **no copper on ANY layer** beneath
  it (the footprint's keepout does this — the copper meander is on F.Cu), and the ground
  pour should extend at least ~30×15 mm *behind* the antenna. The nRF52840's ground
  plane is the antenna's counterpoise — bigger = better range.
- Pad 2 of the footprint is the shorting stub — it auto-connects to your ground pour
  (zone_connect is set), keep that.
- If you'd rather have a discrete part: skip the MIFA and use a Johanson 2450AT18B100
  chip antenna (footprint `RF_Antenna:Johanson_2450AT18B100` is in the bundled libs) —
  same 50 Ω feed, smaller ground plane OK. But MIFA + ground plane is free and
  performance is as good, so: MIFA.

---

## 4. LSM6DSVTR wiring (headset — INT1 + INT2)

```
         LSM6DSVTR (LGA14-L)
  pin 1  SDO/SA0 ──► GND            (SA0=0 → I2C address 0x6A)
  pin 2  SDX      ──► NC            (SPI only; float)
  pin 3  SCX      ──► NC            (SPI only; float)
  pin 4  INT1     ──► MCU GPIO      (data-ready; push-pull, no pull needed)
  pin 5  VDD_IO   ──► 3V3           (decouple 100 nF)
  pin 6  GND      ──► GND
  pin 7  GND      ──► GND
  pin 8  VDD      ──► 3V3           (decouple 100 nF + 1 µF, right next to the pin)
  pin 9  INT2     ──► MCU GPIO      (FIFO watermark/overrun; push-pull)
  pin 10 OCS_AUX  ──► NC
  pin 11 SDO_AUX  ──► NC
  pin 12 CS       ──► 3V3           (must be HIGH for I2C!)
  pin 13 SCL      ──► I2C SCL (4.7 kΩ pull-up to 3V3)
  pin 14 SDA      ──► I2C SDA (4.7 kΩ pull-up to 3V3)
  EP    exposed pad ─► GND
```

- I2C pull-ups: 4.7 kΩ is fine at 400 kHz (drop to 2.2 kΩ if you push 1 MHz).
- INT1/INT2 default to push-pull — route short traces straight to nRF52840 GPIOs,
  no external components.
- Firmware intent (from your plan): INT1 = data-ready triggers the FIFO read, INT2 =
  FIFO watermark / overrun. On the nRF52840 use the GPIOTE + PPI to timestamp INT1
  against the ESB packets if you ever go inertial-SLAM.
- Address: SA0=0 → **0x6A** (your old firmware hardcoded 0x69 for the BMI055 — update).
- Only if you ever switch the IMU to SPI: SDX=MISO, SCX=SCLK, CS=pin 12, SDO/SA0=then
  floats.

---

## 5. TBA459 TMR stick wiring (controllers)

The TBA459 is a PS5-format TMR stick, so it is pin-compatible with the stock DualSense
module — same footprint as `DeltaVR_PS5_Stick:Joystick` (14 pads, from the C&K THB001P
dimension sheet which matches PS5 modules exactly).

Verified net mapping from the thumbstick-breakout design (this is the *polarity* those
sticks need — PS5 TMR sticks output in a fixed direction):

```
  X axis (top row):  3 ──► 3V3      Y axis (right row): 3' ──► 3V3
                     2 ──► X_OUT ──► MCU ADC (SAADC)     2' ──► Y_OUT ──► MCU ADC
                     1 ──► GND                            1' ──► GND
  Stick click:       a (SEL+) ──► MCU GPIO (input, internal pull-up)
                     c (SEL-) ──► GND
                     b, d: NC (second contact pair; may parallel them with a/c)
  4× corner holes:   mount, 1.55 mm drill (tie to GND pour or leave NPTH)
```

- Power both pots from 3.3 V (the TMR bridge is ratiometric, ~0.15–3.15 V out at
  3.3 V supply). **Do not** power from the nRF's internal rails.
- nRF52840 SAADC config for these sticks: internal 0.6 V reference, **gain 1/6** →
  full-scale 0–3.6 V, differential not needed (use single-ended, e.g. P0.02/AIN0,
  P0.03/AIN1). With gain 1/4 you'd clip the top of the range.
- If X or Y comes out inverted in firmware, swap VCC/GND on that axis's pot.
- Skip the old carbon-pot "center tap" bias tricks — TMR sticks are direct 3.3 V rails.

---

## 6. Buttons, battery, and remaining controller bits

- **6x6x4.3 tact buttons** (`Button_Switch_THT:SW_PUSH_6mm_H4.3mm`): one side to GND,
  other side to a GPIO with the nRF's internal pull-up (20 kΩ — fine; add a 0.1 µF
  cap in parallel if you want hardware debounce). Trigger/grip/A/B/XY/menu/system:
  budget ~8 per controller.
- **Battery (18650 pack, XH2.54-2P):**
  `JST_XH_B2B-XH-A_1x02_P2.50mm_Vertical`. Wire: BAT+ → slide power switch → HT7333
  VIN (1 µF to GND); BAT− → GND. HT7333 OUT (1 µF) → 3.3 V rail.
  - Add a **battery voltage monitor**: 100k/100k divider BAT+ → ADC input, so firmware
    can warn below ~3.5 V (LDO drops out ~3.35 V — give yourself margin).
  - Charging: the pack is 18650 — charge with a TP4056 module (USB-C in, XH2.54 out) or
    a TP4056 + USB-C on the board. Do not charge from the nRF52840 directly.
  - Add a diode (or the TP4056's protection) so USB 5V can't backfeed the battery if
    you ever add USB to a controller.
- **IMU on the controllers too:** same LSM6DSVTR circuit as §4 (I2C + INT1/INT2). Two
  IMUs share the same I2C bus fine — second one gets SA0=1 → 0x6B.

---

## 7. ESB (Enhanced ShockBurst) on nRF52840 — yes, it works

- The nRF52840 has ESB **built into the radio** (Nordic SDK / Zephyr `nrf_esb` driver,
  or the new `esb` from the nRF Connect SDK). Same air protocol as the old NRF24L01+
  modules HadesVR used — 1 or 2 Mbps, with ACK, so the existing
  packet/pipe architecture ports over almost 1:1.
- One radio, **two RX pipes** on the headset = both controllers (e.g. pipe addresses
  0xE1E2E3E4E5 / 0xE1E2E3E4E6, TX on the controllers matching one each).
- The **separate Receiver board disappears**: the headset's nRF52840 receives both
  controllers over ESB and relays everything to the PC over USB HID — replacing the
  Pro Micro **and** both NRF24L01 modules with one chip.
- 2 Mbps ESB is ~1–2 ms on-air per packet — fine for VR (HadesVR did it over 1 Mbps
  NRF24 with no problems).

---

## 8. "Can the nRF52840 pretend to be the VR headset?" — Yes

- **aQFN73 nRF52840 + USB HID is exactly what the HadesVR driver already expects**: the
  driver's `HIDTransport` reads 64-byte reports; the HMD firmware sends the same
  `HMDRAWPacket` (PacketID 3) — keep the struct byte-identical and the PC side needs
  zero changes (that's also in your plan).
- The nRF52840 USB stack (TinyUSB / Nordic's USB HID examples) enumerates as a normal
  HID device on Windows — no drivers needed.
- Tracking: the nRF52840 is NOT doing SLAM. It feeds rotation (LSM6DSV → Madgwick on
  the PC, exactly like today) + the SLAM cameras stream UVC over USB straight to the
  PC; the SLAM process publishes pose over localhost UDP into the driver's existing
  camera-position path (`PSMUpdate()` → `V3Kalman.updateMeasCam()`). The MCU only needs
  to send IMU data + relay controller ESB packets. Half-software tracking, as you said —
  the MCU side is unchanged from HadesVR's role, just one chip instead of three.

---

## 9. Remaining gotchas

- **Pro Micro / ATmega stuff is gone** — the SparkFun Pro Micro lib (`ProMicroKiCad`)
  in this folder is no longer needed; ignore it (or delete it).
- `deltavr hmd.kicad_sch` / `.kicad_pcb` are blank templates — the tables above are
  what make the libs appear in the symbol/footprint choosers. Open Schematic Editor,
  "Add Symbol", filter by `DeltaVR_` and you'll see them.
- KiCad 9 note: after adding the tables, restart KiCad / rescan libraries if the new
  libs don't show immediately.
- The Nordic lib also ships WLCSP (BGA) footprints + STEP models — only needed for the
  CKXX part, not used here.
- License notes: nordic-lib-kicad is CERN-OHL-P-2.0, thumbstick-breakout is CC-BY-SA-4.0,
  ESP32 MIFA is from Espressif's public reference design, ST LSM6DSV lib is ST's export.
  All fine for a personal/hobby repo.