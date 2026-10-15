buyers of deltavr moths should not need a toolchain. cash cost of the software side is near zero, the time cost is not.

## what buyers actually run

- slimevr server on the pc. this is the whole reason moths exist. open source, already works with smolslime class hardware
- a receiver / dongle. one dongle does not magically do a full set forever, see the butterfly pricing chat. budget dongles into the set story or buyers will hit a wall at 10 trackers
- firmware flash tools. currently more diy than we want. drag and drop or one script before anything ships

nrf52840 pro micro modules flash over uf2 for the most part. we already have SlimeNRF_Tracker_SPI_StackedSmol.uf2 floating around as a reference build.

## host side we care about

- pairing and calibration in slimevr (mounting, yaw reset, body proportions)
- double-tap reset. slimevr server already has the multi-tracker safety filter so stomps dont false trigger. we design around that, we dont reinvent it
- battery reporting if we want it. nice to have, not a blocker

## what we maintain

- board definition / pin map for the deltavr moths pcb so someone can flash it without reading our minds
- a short install note: get slimevr, flash this, pair, strap on, calibrate
- maybe a configurator pass later. there is already SmolSlimeConfiguratorV9 in the wild, worth seeing how far that gets people

## software cost reality

| item | cash | time |
| --- | --- | --- |
| slimevr server | $0 | we ride on community software, we just need to not break against it |
| flashing story | $0 | medium. this is the #1 support ticket if we skip it |
| install doc | $0 | low-medium and it is part of the product |
| board pinmap / firmware fork | $0 | medium. needs to match the pcb we actually ordered |

## before selling

1. flash and pair from a clean pc in under 30 minutes
2. written install path that matches the real buttons and ports
3. know what happens when slimevr updates. even if the answer is "we patch the fork", we need to know thats the plan

-oxy
