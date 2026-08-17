# DeltaVR — Implementation Plan

Status: **ACTIVE** — architecture decided (nRF52840 + ESB + PC SLAM), KiCad libraries
assembled, PCB design + firmware pending.
Last updated: 2026-08-15

> This document is written for a future contributor/model to pick up the project and
> continue implementation. It records the project context, the current hardware/software
> state, the decisions that were made, the full phased plan, and the technical facts that
> were verified while researching it. It is a living document — update it as work progresses.

---

## 1. Project overview

DeltaVR is an open-source, wired PCVR headset loosely based on **HadesVR** (outside-in
PSMove/PS Eye tracking originally). DeltaVR diverges in four big ways:

1. **Tracking is SLAM** (inside-out, camera-based, computed on the PC) — no PSMoveService,
   no PS Eye cameras.
2. **All MCUs are nRF52840s.** The headset runs a single nRF52840 **aQFN73** that does ESB
   radio + USB HID; the old Pro Micro + 2× NRF24L01 modules + separate Receiver board are
   gone. Controllers use their own nRF52840s.
3. **RF is ESB** (Enhanced ShockBurst, the NRF24L01-compatible protocol built into the
   nRF52 radio) — no external radio ICs anywhere.
4. **Controllers are IR-tracked** by the headset's own cameras (details in §4.6), with
   TMR hall-style sticks (TBA459, PS5-format) instead of carbon pots.

Everything else (SteamVR driver, Madgwick AHRS on the PC, HMDRAWPacket protocol, low-cost
DIY ethos) is inherited from HadesVR.

---

## 2. Repository map

Root: `C:\Users\rhime\Documents\GitHub\deltavr` (git repo, branch `main`)

| Path | Contents |
|---|---|
| `README.md` | Project blurb + initial notes. |
| `docs/IMPLEMENTATION_PLAN.md` | (this file) |
| `kicad/` | KiCad project: `deltavr hmd.kicad_pro/.kicad_sch/.kicad_pcb` + `sym-lib-table`/`fp-lib-table` wired to the libs below. |
| `kicad libaries/` | KiCad libraries (all cloned/present, see §5): `nordic-lib-kicad` (nRF52 symbols), `lsm6dsvtr/` (ST official LSM6DSVTR), `ESP32_MIFA_PCB_ANTENNA/` (MIFA), `thumbstick-breakout/` (PS5 stick), `README_deltavr.md` (full wiring guide). |
| `Hardware/Controllers/Controller boards/` | Old HadesVR ATmega controller PCBs — **reference only**, not used for DeltaVR PCBs. |
| `fusion360/` | Empty. Previously held Fusion 360 CAD. The old Fusion tracking-PCB exports are archived in `C:\Users\rhime\Documents\fusion exports\` (netlist/BOM reference only). |
| `Software/Firmware/` | Old Arduino firmware (`Headset.ino`, `Receiver.ino`, controller variants) — **to be replaced** by an nRF Connect SDK project (§6). Packet format stays compatible. |
| `Software/Driver/src/samples/driver_HadesVR/` | SteamVR/OpenVR driver (MSVC `.vcxproj`, `driver_hadesvr.dll`). **Kept as-is**; only a SLAM position-source plugin gets added. |
| `deltavr.stl` | STL of the headset shell. |
| `LICENSE` | MIT. |

---

## 3. System architecture

```
                 ┌─────────────────────────── PC ───────────────────────────┐
                 │                                                           │
 USB HID ───────►│ driver_HadesVR.dll (HMD + controllers in SteamVR)         │
  64B reports    │   rotation: Madgwick(LSM6DSV accel/gyro)                  │
                 │   position: SLAM UDP pose → V3Kalman.updateMeasCam()      │
                 │   controller pose: IR-tracked pose → V3Kalman             │
                 │                                                           │
 UVC ───────────►│ SLAM process (ORB-SLAM3 stereo, OV9281 pair)              │
 2× OV9281       │   → localhost UDP pose stream (100 Hz)                    │
                 └───────────────────────────────────────────────────────────┘

nRF52840 aQFN73 (headset, always wired)
  ├─ ESB PRX, 2 pipes ←── controllers (IMU + buttons + stick @ up to 2 Mbps)
  ├─ USB HID → PC (HMDRAWPacket byte-compatible with HadesVR)
  ├─ LSM6DSVTR I2C 0x6A, INT1=data-ready, INT2=FIFO watermark
  ├─ MIFA antenna, 5V USB → HT7333 → 3.3 V

nRF52840 (controller ×2, 18650 powered)
  ├─ ESB PTX → headset
  ├─ LSM6DSVTR I2C 0x6A/0x6B, INT1/INT2
  ├─ TBA459 TMR stick (X/Y ADC + click) + 6x6x4.3 tact buttons
  ├─ MIFA antenna, 18650 → switch → HT7333 → 3.3 V
  └─ battery voltage divider → ADC (low-bat warning)
```

Key property: **the MCU does no SLAM**. SLAM cameras stream UVC to the PC; the PC runs
ORB-SLAM3 and feeds pose into the existing driver plumbing (`PSMUpdate()` thread pattern →
`V3Kalman.updateMeasCam()`). The nRF52840 only moves IMU/button/stick data and relays
controller ESB packets — same role as HadesVR's MCUs, one chip instead of three.

---

## 4. Hardware design

### 4.1 Chips

| Board | MCU | Why |
|---|---|---|
| Headset | **nRF52840-QIAA (aQFN73, 7x7)** | Only package with **USB**; required for HID to PC. |
| Controllers | nRF52840-QIAA or -QFAA (QFN48, 6x6) | QFN48 fine here (no USB needed); QIAA if one part everywhere. |

Symbols: `DeltaVR_Nordic_nRF52:nRF52840-QIXX / -QFXX` (hlord2000/nordic-lib-kicad).
Footprints: bundled official KiCad (`Package_DFN_QFN:Nordic_AQFN-73-1EP_7x7mm_P0.5mm`,
`QFN-48-1EP_6x6mm_P0.4mm_EP4.6x4.6mm_ThermalVias`).

### 4.2 Power

- Headset: USB 5 V → **HT7333** (SOT-89-3: 1=GND, 2=VIN, 3=VOUT; 1 µF in/out) → 3.3 V rail.
- Controllers: 18650 pack (4500 mAh, XH2.54-2P plug → `JST_XH_B2B-XH-A_1x02_P2.50mm_Vertical`)
  → slide switch → HT7333 → 3.3 V. QFN48 must not see 4.2 V directly (VDD max 3.6 V).
  LDO drops out ~3.35 V battery — acceptable (≈15 % capacity wasted; ~90 h runtime).
- LDO *is* needed (5 V → 3.3 V and 4.2 V → 3.3 V). It is **not** needed "for the IMU" —
  the LSM6DSV runs off the 3.3 V rail (VDD 1.71–3.6 V).
- nRF52 supply: VDD 100 nF + 10 µF; **DCDC** (10 µH DCC→VDD + 10 µF) — big win on battery
  boards; DEC1/3/4/6 100 nF; DECUSB (aQFN73) 1 µF; 32 MHz xtal + 32.768 kHz xtal.
- Battery monitor: 100k/100k divider → SAADC, warn < 3.5 V. Charge via TP4056 module
  (never charge from the MCU).

### 4.3 LSM6DSVTR (headset and both controllers)

Wiring (from `kicad libaries/README_deltavr.md` §4): I2C with 4.7 kΩ pull-ups,
**CS→3V3 (mandatory for I2C)**, **SDO/SA0→GND = 0x6A** (controller #2: SA0→3V3 = 0x6B),
**INT1 = data-ready**, **INT2 = FIFO watermark/overrun** (push-pull, direct to GPIOs),
100 nF + 1 µF on VDD, exposed pad → GND. SDX/SCX/OCS_AUX/SDO_AUX = NC.

> Note: old firmware used I2C address 0x69 (BMI055). New address is 0x6A.

### 4.4 ESB radio + MIFA antenna

- ANT pin → LC match (series 3.9 nH + shunt ~1 pF/1.8–2.2 pF, values from Nordic DK,
  tune with VNA) → short 50 Ω trace → **MIFA feed pad**.
- MIFA footprint `DeltaVR_MIFA_Antenna:ESPRESSIF_ESP32_MIFA_2.4GHz_Right` at a board
  edge/corner; no copper on any layer under the antenna; ground pour ≥ ~30×15 mm behind
  it (the ground plane is the antenna's counterpoise).
- ESB: 1–2 Mbps, ACK, on-air-compatible with NRF24L01. Headset = PRX with 2 pipes
  (left/right). Bidirectional via ACK payloads (calibration/config down, sensor data up).

### 4.5 Controllers

- **TBA459 TMR stick** (PS5-format → footprint `DeltaVR_PS5_Stick:Joystick`):
  pots powered 3'→3V3 / 1'→GND (X) and 3→3V3 / 1→GND (Y), wipers 2'/2 → SAADC
  (internal 0.6 V ref, **gain 1/6** → 0–3.6 V full scale); click a→GPIO pull-up, c→GND;
  4× 1.55 mm mounting holes.
- **Buttons** 6x6x4.3 (`Button_Switch_THT:SW_PUSH_6mm_H4.3mm`): GPIO internal pull-up,
  optional 0.1 µF debounce. ~8 per controller (trigger/grip/A/B/X/Y/menu/system).
- All passives 0603.

### 4.6 Controller tracking (IR, via headset cameras)

Controllers are tracked by the headset's cameras on the PC — **outside-in from the HMD's
own view**. Mechanism options (decide in Phase 4):

- **A (preferred): IR LEDs** on each controller, blobs tracked in the (IR-passive-filtered
  or monochrome) camera frames — HadesVR-style blob math, no PSMoveService.
- B: printed AprilTag/ArUco fiducials (no LEDs needed, works in normal light).

Either way: camera pose per controller → 6-DOF → streamed into the controllers'
`V3Kalman` pose path on the PC (same plumbing as Phase 3). Rotation/input still from the
controller's own LSM6DSV + ESB packets. SLAM for the HMD is scale-correct stereo; the
controller baseline/scale is calibrated once.

---

## 5. KiCad status (DONE — libraries)

- `kicad/` project exists (blank schematic/board, KiCad 9).
- `kicad/sym-lib-table` + `kicad/fp-lib-table` created; the new libs appear in the
  choosers as `DeltaVR_*` entries.
- Libraries present in `kicad libaries/`:
  - `nordic-lib-kicad` — nRF52840 QFXX/QIXX/CKXX symbols (CERN-OHL-P-2.0).
  - `lsm6dsvtr/` — ST official LSM6DSVTR symbol + LGA14-L footprints + STEP.
  - `ESP32_MIFA_PCB_ANTENNA` — Espressif MIFA 2.4 GHz footprint.
  - `thumbstick-breakout` — PS5 stick symbol + footprint (CC-BY-SA-4.0; matches TBA459).
  - `README_deltavr.md` — full wiring reference (power, IMU, stick, MIFA, battery).
- **Not found anywhere:** a public TBA459-specific library. Verify the physical TBA459
  module against the PS5 stick footprint before ordering PCBs.
- Old `ProMicroKiCad-master` lib is obsolete — ignore/delete.

Next: draw headset + controller schematics, then boards (§7 Phase 1).

---

## 6. Firmware (nRF Connect SDK)

Old Arduino firmware is replaced by a single NCS/Zephyr project (one tree, three boards:
`deltavr_hmd`, `deltavr_controller_left`, `deltavr_controller_right`).

- **Radio:** `nrf_esb` driver (NCS/Zephyr). PRX 2 pipes on HMD, PTX on controllers,
  2 Mbps. Controller packets ≈ old PacketID 2 format; HMD relays to USB HID.
- **USB:** TinyUSB or Nordic USB HID stack — 64-byte HID reports, **`HMDRAWPacket`
  (PacketID 3) byte-for-byte identical** to HadesVR so the PC driver needs no changes
  (mag fields stay zero — no magnetometer on the LSM6DSV).
- **IMU:** ST `lsm6dsv` C driver (I2C, ODR ~1 kHz, INT1 data-ready → FIFO reads via
  GPIOTE/PPI, INT2 watermark/overrun). No quaternion fusion on-chip — Madgwick stays on
  the PC.
- **Peripherals:** SAADC sticks (gain 1/6), buttons (GPIO pull-up), battery divider.
- Flash via SWD (P0.18 RESET, P0.20 SWDCLK, P0.21 SWDIO — 4-pin header on every board).

---

## 7. Phased plan

### Phase 0 — KiCad libraries ✅ (done, see §5)

### Phase 1 — PCB design in KiCad
1. Schematics: HMD (nRF52840-QIAA + LSM6DSVTR + MIFA + USB-C + HT7333 + SWD) and
   controller (nRF52840 ± QFAA + LSM6DSVTR + stick + 8× tact + MIFA + XH2.54 + HT7333).
2. ERC clean; document the pin mapping in the repo (was never written down).
3. Layout: MIFA at edge w/ keepout + ground pour, 32 MHz xtal close to chip, decoupling
   per §4.2, sticks/buttons on front, battery plug on rear.
4. DRC clean → Gerbers + drill + BOM + PnP for JLCPCB/PCBWay.
5. Verify TBA459 footprint against the real module (§5).

### Phase 2 — Firmware bring-up
1. NCS project + board files; blink/SWD smoke test.
2. ESB link HMD↔controllers (pipes, ACK, RSSI).
3. LSM6DSV driver (0x6A, INT1/INT2), SAADC sticks, buttons.
4. USB HID with byte-compatible HMDRAWPacket; relay controllers over USB.
5. Power: DCDC on, sleep states, low-bat threshold.

### Phase 3 — SLAM on the PC (stereo, OV9281)
1. Rig two OV9281 (global shutter) + 2.2 mm M12 ~140° as a stereo pair, baseline 6–8 cm.
2. Stereo-fisheye calibration (kalibr or OpenCV `fisheye`), camera→HMD rigid offset.
3. ORB-SLAM3 (Windows port) in stereo mode, ≤60 fps initially.
4. Pose streaming: standalone process → localhost UDP (timestamp, xyz, quaternion,
   confidence, ~100 Hz).
5. Driver: "SLAM" position source mirroring `PSMUpdate()` (dataHandler.cpp:544–612) →
   `HMDKalman.updateMeasCam(...)`; new `SLAM_*` settings keys; PSMoveService path
   compile-time optional, off by default.
6. Tuning: V3Kalman noise, scale check, yaw drift, latency budget < 20 ms.

### Phase 4 — IR controller tracking
1. Decide LED-blob vs fiducial (§4.6); mount markers on controllers.
2. PC-side tracker (OpenCV, from a headset camera stream) → 6-DOF per controller.
3. Feed into controllers' `V3Kalman` pose path (mirror Phase 3 pattern).
4. Latency/occlusion handling, per-controller calibration.

### Phase 5 — Integration, testing, documentation
1. End-to-end: SteamVR sees HMD + both controllers; SLAM position + IMU rotation track.
2. Lighting robustness, motion blur, latency, drift; calibration UI if needed.
3. Update `hardware_setup_guide.md` (nRF flashing, SLAM setup, camera calibration),
   update `README.md`.
4. Record full pin map + packet formats in the repo.

---

## 8. Verified technical facts

- **LSM6DSV**: 6-axis only (no magnetometer), I2C 0x6A/0x6B (SA0), SPI, up to ~2.8 kHz
  ODR, INT1 + INT2 (data-ready, FIFO watermark/overrun, wakeup, FSM/MLC). FastIMU does
  **not** support it → use ST's `lsm6dsv` driver. Madgwick on the PC stays the right
  fusion (no embedded quaternion on the base DSV).
- **nRF52840 packages**: QFN48 (`-QFAA`) has **no USBD** and VDD/VDDH shorted internally
  (normal voltage mode only, 1.7–3.6 V) → headset must be aQFN73 (`-QIAA`, USB + VDDH
  2.5–5.5 V). SWD: RESET=P0.18, SWDCLK=P0.20, SWDIO=P0.21.
- **ESB** on nRF52 = same air protocol as NRF24L01 Enhanced ShockBurst (1/2 Mbps, ACK,
  auto-retransmit). Latency ~1–2 ms/packet at 2 Mbps — fine for VR.
- **SLAM on Windows**: ORB-SLAM3 (`UZ-SLAMLab/ORB_SLAM3`) + Windows port
  (`rexdsp/ORB_SLAM3_Windows`); fisheye model needed for the ~140° lenses; global shutter
  is a hard requirement (OV9281 satisfies it). Do NOT link SLAM into the driver DLL —
  standalone process + UDP.
- **Driver**: existing camera-position plumbing (`PSMUpdate()` → `V3Kalman.updateMeasCam`)
  is directly reusable for SLAM and controller IR tracking.
- **TMR stick**: PS5-format hall/TMR modules are drop-in replacements for the DualSense
  stick; footprint verified from C&K THB001P dimensions; they are **polarized** (VCC/GND
  per axis fixed, see §4.5).

---

## 9. Decisions (locked in)

1. **All MCUs nRF52840**; headset = aQFN73 (USB), controllers = QFN48 or aQFN73.
2. **ESB** replaces NRF24L01 modules; no separate Receiver board (HMD relays over USB).
3. **SLAM on the PC** (stereo OV9281 pair) → UDP pose → existing driver path.
4. **Controllers IR-tracked** by headset cameras (LED blobs or fiducials, §4.6).
5. **KiCad** for all PCBs; library set per §5.
6. **LSM6DSVTR** on every board; INT1 data-ready + INT2 FIFO watermark; I2C 0x6A/0x6B.
7. **HT7333** → 3.3 V rail everywhere (5 V USB / 18650).
8. **MIFA** antenna + big ground plane on every board (chip antenna as fallback).
9. **TBA459** TMR sticks (PS5 footprint) + 6x6x4.3 tacts + 0603 passives.
10. `HMDRAWPacket` (PacketID 3) stays **byte-for-byte identical**; PSMoveService paths
    become compile-time optional, off by default.

---

## 10. Risks / open questions

- **TBA459 specifics unverified** (pinout/polarity assumed PS5-standard; no public
  datasheet found). Measure the physical module before finalizing the footprint/order.
- **ORB-SLAM3 Windows build + stereo-fisheye calibration** is the heaviest lift.
- **USB bandwidth**: 2× OV9281 @ ≤60 fps initial; 4-cam extension needs USB3 budget care.
- **Latency budget**: < 20 ms total (camera → SLAM → UDP → driver + ESB HMD path).
- **Yaw drift**: Madgwick + SLAM heading fusion — the Kalman noise tuning decides.
- **Soldering**: 0.4 mm pitch QFN/aQFN — stencil + hotplate/reflow, not hand iron.
- Controller IR range/occlusion: LEDs must be visible from the headset cameras; consider
  both-sides marker placement.

---

## 11. Handoff notes for the next person/model

- Start with **Phase 1** (KiCad schematics); all wiring reference is in
  `kicad libaries/README_deltavr.md` (§4 table). Keep the LSM6DSV pin map (§4.3) and
  stick map (§4.5) exactly as documented.
- Keep `HMDRAWPacket` identical — the driver depends on it (`dataHandler.h` lines 135–176).
- Do not link SLAM into the driver DLL; use the standalone-process + UDP pattern.
- Reuse `V3Kalman.updateMeasCam` + the `PSMUpdate()` thread pattern for both SLAM (HMD)
  and IR (controllers) instead of inventing new pipelines.
- Firmware lives in a new NCS project under `Software/`; the old Arduino `.ino` files are
  reference only after Phase 2.
- Update this document (status, decisions, progress) as you go.