# ads1115_i2c_stub.py
#
# Gives the "ads1115" Mocks.DummyI2CSlave (declared in nicenano_deltavr.repl)
# enough register behavior to hand back a plausible wandering conversion
# value - useful for exercising your hall-sensor / joystick reading code
# on the controller board. Load per-machine after
# `machine LoadPlatformDescription`, with:
#   include @peripherals/ads1115_i2c_stub.py

import math

ads1115 = monitor.Machine["sysbus.ads1115"]

_state = {"ptr": 0x00, "t": 0.0}

CONVERSION = 0x00
CONFIG = 0x01


def _on_data_received(data):
    if len(data) == 0:
        return
    _state["ptr"] = data[0]
    # data[1:] would be a CONFIG register write (mux/gain/mode/rate select) -
    # not modeled. Add handling here if you need reads to differ per input
    # channel (AIN0..AIN3) based on what the firmware last selected.


def _on_read_requested(n):
    _state["t"] += 0.05
    if _state["ptr"] == CONVERSION:
        val = int(16000 * math.sin(_state["t"]))  # signed 16-bit, wanders slowly
        resp = bytearray([(val >> 8) & 0xFF, val & 0xFF])
    elif _state["ptr"] == CONFIG:
        resp = bytearray([0x85, 0x83])  # arbitrary but well-formed CONFIG readback
    else:
        resp = bytearray([0, 0])
    if n > len(resp):
        resp = resp + bytearray(n - len(resp))
    ads1115.EnqueueResponseBytes(resp[:n])


ads1115.DataReceived += _on_data_received
ads1115.ReadRequested += _on_read_requested

print "ads1115_i2c_stub.py: ADS1115 I2C stub attached"
