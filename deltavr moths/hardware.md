deltavr moths are two little rigid boards joined by a flex middle, butterfly style. antenna on top, touch and battery on the lower wing, everything as flat as we can get it.

## the butterfly fold

- top pcb: antenna plus the radio story, facing up / forward when worn
- flex middle: expanding lw-tpu, wires for touch and battery run through here
- bottom pcb: touch sensing ic, battery side
- both wings face out toward the shell window, flat against each other minus a kapton layer

the nrf52840 pro micro (nice!nano type) ends up as a tiny main board. stock leds on the module get chopped or desoldered. one rgb led on our pcb takes over. enig gold because the boards are meant to be seen.

full build target is a set of 8 finished trackers. sold as sets of 10 if we fold in spares or dongles.

## radio (the thing that actually matters)

for a smolslime class tracker, radio performance is the product. drop packets and the whole thing feels like garbage no matter how nice the imu math is.

- protocol is esb (enhanced shockburst) back to an nrf52840 receiver
- antenna is a mifa (meandered inverted-f) as the sole element at the top of the pcb, facing forward. holds up near plastic and tpu better than a straight wire
- if we reuse a pro micro module, bypass its onboard antenna. zero-ohm jumper / rf switch near the module antenna, reroute to an external pin, then a short fat 50-ohm trace to the mifa
- keep-out under the antenna. no copper, no components, no battery wires on any layer
- solid ground plane on the non-antenna sections so signals bounce away from the body
- 10uF low-esr ceramic right at the nrf vdd/vbus so packet bursts dont sag the rail
- payload stays short. quaternions as 16-bit ints, not float text. a few auto-retries around 250us apart

smolslimes typically emit around 0 to +4dBm. thats the baseline we tune against.

## touch / tap (and why we stopped chasing capacitive)

started wanting a flat capacitive tap under the shell. ttp223 is fine in isolation, but on deltavr moths it fights the antenna and burns the 90mah battery. routing it next to a weak nrf pcb antenna is asking for ghost taps and dead range.

paths that actually fit:

1. **imu hardware tap** (lsmd6dsv class). zero extra parts, zero extra copper. a finger knock vs a leg stomp is a frequency/axis problem, and the chip can filter. slimevr already has double-tap reset logic on top
2. **flat metal dome** (~0.3mm). mechanical, 100% flat, zero rf drama
3. **reed switch + magnet ring**. zero standby current, completely ignores the radio

if we still want a real touch pad on the pcb: shielded touch ic (at42qt1011 class or similar driven shield), 10 to 15mm copper pad, solder mask opened, 2 to 3mm clearance, short trace, far from the antenna. a 30mm keep-out is fantasy on this board. under 5mm means a grounded guard ring and a chip that can actually shield.

stomp-proofing on the imu path: double-tap only, and slimevr server already cross-checks the other trackers so a full-body thud gets rejected.

## board aesthetics (on purpose)

we sell these to reimburse the headset, but also we like pretty parts.

- enig finish
- rgb led front and center
- "designed by oxy2" on the silkscreen or mask if we can sneak it in
- art on the pcb, not just green rectangles
- clear petg window so the gold stays visible
- exposed pcb alone is not enough. sweat and esd will kill it. clear conformal coat (mg chemicals 419d style) keeps the look and adds the safety

## open hardware questions

1. lock the final antenna feed (module bypass vs native mifa only)
2. lock touch: imu tap only, dome, or shielded pad
3. confirm the exact pro micro footprint (nice!nano v2 pinout)

-oxy
