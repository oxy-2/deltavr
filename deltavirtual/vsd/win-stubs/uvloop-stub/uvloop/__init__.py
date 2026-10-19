# windows stub. real uvloop does not support windows.
# uvicorn imports this optionally and falls back to asyncio when install() is a no-op.

import asyncio

__version__ = "0.23.0"


class EventLoopPolicy(asyncio.DefaultEventLoopPolicy):
    pass


def install():
    # no-op. keep asyncio's default loop.
    return None


def new_event_loop():
    return asyncio.new_event_loop()
