# lsm6dsv_i2c_stub.py
#
# Gives the "lsm6dsv" Mocks.DummyI2CSlave (declared in nicenano_deltavr.repl)
# just enough register behavior for a driver to see a valid WHO_AM_I and
# plausible-looking accel/gyro samples. Load per-machine, AFTER
# `machine LoadPlatformDescription`, with:
#   include @peripherals/lsm6dsv_i2c_stub.py
#
# Renode's embedded Python is IronPython (Python 2 syntax).

import math

lsm6dsv = monitor.Machine["sysbus.lsm6dsv"]

_state = {"reg": 0, "t": 0.0}

WHO_AM_I_REG = 0x0F
WHO_AM_I_VAL = 0x70  # LSM6DSV16X datasheet value at time of writing - re-check
                      # against the exact LSM6DSVTR revision you're using

OUTX_L_G = 0x22  # gyro: OUTX_L_G..OUTZ_H_G (6 bytes)
OUTX_L_A = 0x28  # accel: OUTX_L_A..OUTZ_H_A (6 bytes)


def _on_data_received(data):
    # First byte of a write is always the target register address on this
    # part. Further bytes (CTRL1..CTRLn writes) are NOT modeled - add
    # handling here if your firmware's ODR/scale setup needs to "stick"
    # for the values you synthesize below to look right.
    if len(data) > 0:
        _state["reg"] = data[0]


def _on_read_requested(n):
    reg = _state["reg"]
    _state["t"] += 0.02
    resp = bytearray(n)
    for i in range(n):
        addr = reg + i
        if addr == WHO_AM_I_REG:
            resp[i] = WHO_AM_I_VAL
        elif OUTX_L_G <= addr < OUTX_L_G + 6:
            offset = addr - OUTX_L_G
            axis = offset // 2
            val = int(500 * math.sin(_state["t"] + axis))
            resp[i] = (val & 0xFF) if offset % 2 == 0 else ((val >> 8) & 0xFF)
        elif OUTX_L_A <= addr < OUTX_L_A + 6:
            offset = addr - OUTX_L_A
            axis = offset // 2
            # roughly +1g on Z, near-zero with slight wobble on X/Y
            g = 16384 if axis == 2 else int(200 * math.sin(_state["t"] + axis))
            resp[i] = (g & 0xFF) if offset % 2 == 0 else ((g >> 8) & 0xFF)
        else:
            resp[i] = 0
    lsm6dsv.EnqueueResponseBytes(resp)


lsm6dsv.DataReceived += _on_data_received
lsm6dsv.ReadRequested += _on_read_requested

print "lsm6dsv_i2c_stub.py: LSM6DSVTR I2C stub attached"
