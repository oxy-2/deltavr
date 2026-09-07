# deltavr v1.0.0 — PCBs officially done

first fabrication-ready release of the deltavr pcbs. everything here is everything you need to build both boards.

## what's in this release

| asset | what it is |
|---|---|
| `deltavr-hmd-gerbers.zip` | HMD board gerbers + Excellon drill + pick-and-place, JLCPCB layer naming |
| `deltavr-controller-r-gerbers.zip` | right-hand controller board gerbers + drill + PnP, JLCPCB naming |
| `schematics/` | PDF schematics for both boards |
| `bom/` | CSV BOMs, interactive HTML BOMs (open in any browser), + informal parts-sourcing list |
| `renders/` | 3D renders, top/bottom/iso per board |

## fabricating

- fab tested against JLCPCB rules (2-layer, 1.6mm FR-4). upload the 7 gerbers from a zip, extensions are already JLCPCB layer naming so it auto-detects.
- DRC: 0 errors, 0 unconnected nets on both boards (fresh kicad-cli run against the committed designs).
- **left-hand controller**: only the right board is shipped. copy `kicad controller R/`, open the pcb, select everything and mirror about the Y axis — the design is symmetric so it just works.

## sourcing

see `bom/pcb components bill of mats.txt` for the informal full-build parts list (covers HMD + both controllers). everything is Alibaba/AliExpress friendly.

## boards

- **HMD** (48x49mm): nRF52840 Pro Micro (nice_nano) + nRF24L01+ PA/LNA + LSM6DSVTR IMU + debug header exposing every pin + silk art.
- **controller R** (70x70mm round): ginfull TMR thumbstick via ADS1115, 2x SS49E hall triggers, 3x tact, nRF24L01+ radio module for off-board install, IRLZ44N for rumble. art on the silk.

no firmware yet — tracking/display stack is next.

known cosmetic DRC warnings (silk-over-copper on the art, text size flags, isolated islands on B.Cu) are documented and accepted; none affect fab.
