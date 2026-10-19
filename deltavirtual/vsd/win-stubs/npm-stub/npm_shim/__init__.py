# windows shim. python subprocess.run(["npm", ...]) only looks for npm.exe,
# but node ships npm.cmd. this exe forwards to the real one.

import os
import shutil
import subprocess
import sys


def find_real_npm():
    # prefer npm.cmd next to the usual nodejs install, then anything else on PATH
    candidates = []
    for key in ("ProgramFiles", "ProgramFiles(x86)", "LOCALAPPDATA"):
        base = os.environ.get(key)
        if base:
            candidates.append(os.path.join(base, "nodejs", "npm.cmd"))
    which = shutil.which("npm.cmd") or shutil.which("npm")
    if which:
        candidates.append(which)

    for path in candidates:
        if path and os.path.isfile(path) and not path.lower().endswith("npm-shim.exe"):
            return path
    return None


def main():
    real = find_real_npm()
    if not real:
        sys.stderr.write("npm shim: could not find npm.cmd. install node.js.\n")
        return 1

    # when invoked as `npm.exe ...`, sys.argv[1:] is the npm args
    if real.lower().endswith(".cmd") or real.lower().endswith(".bat"):
        completed = subprocess.run(["cmd", "/c", real] + sys.argv[1:])
    else:
        completed = subprocess.run([real] + sys.argv[1:])
    return completed.returncode


if __name__ == "__main__":
    sys.exit(main())
