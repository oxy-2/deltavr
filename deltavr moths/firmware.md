firmware on deltavr moths is the nrf52840 and the imu. nothing else really talks.

## targets

- nrf52840 pro micro (nice!nano type) as the brain
- lsmd6dsv class imu. dual int pins exist, body trackers often only wire int1 and merge tap into it via the chip register. we can do better on our own board if we want both
- esb radio link out
- one rgb led (stock module leds are gone)
- touch path depending on what we lock in [[hardware]]: imu tap interrupt, dome on a gpio, or shielded touch ic
- 90mah battery. this is the whole power budget

## stack

- slimevr tracker firmware on nrf (SlimeVR-Tracker-nRF lineage, Chrysalis-style for nrf slimes)
- renode if we want to simulate before boards land
- board overlay / defines.h style pin map for our pcb. this is the piece that makes moths a product instead of a random nice!nano

tap reset is already in slimevr. double-tap, server checks the other trackers, rejects stomp false positives. wire the imu interrupt to the pin the firmware expects and dont invent a new gesture.

## power (the real feature)

90mah lasting around a month of standby is the bar. one pair of batteries on a shelf for a year is the war story that proves the hardware.

that means:

- deepest sleep when idle. no always-on capacitive sampling chip eating microamps
- imu hardware tap or a zero-power switch for wake
- short esb payloads, radio on for as little time as possible
- no fancy led animations unless the tracker is awake and we accept the drain

## radio config

from the moth trackers chat, still right:

- compress quaternions to 16-bit ints
- small auto-retry count, roughly 250us spacing
- keep packets short so the air stays clear when 8 or 10 trackers are shouting at once

## what has to work

1. flash, pair, track. the boring loop, done cleanly
2. double-tap reset that doesnt fire when someone stomps
3. battery life that matches the 90mah story
4. pin map checked against the enig board we actually ordered

-oxy
