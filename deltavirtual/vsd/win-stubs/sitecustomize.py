# python 3.12+ no longer auto-creates an event loop in get_event_loop().
# vsd / pipeline-manager still use the old behavior. restore it.

import asyncio

_orig_get_event_loop = asyncio.get_event_loop


def _get_event_loop_compat():
    try:
        return _orig_get_event_loop()
    except RuntimeError:
        loop = asyncio.new_event_loop()
        asyncio.set_event_loop(loop)
        return loop


asyncio.get_event_loop = _get_event_loop_compat
