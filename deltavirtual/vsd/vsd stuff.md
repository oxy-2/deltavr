# vsd (visual system designer)

this is antmicro's visual-system-designer-app, vendored in visual-system-designer-app/. upstream is apache-2.0.

## what you need to install to run it


- python 3.10+ (python.org, and add it to PATH)
- node + npm (nodejs.org, LTS works)
- git with git bash (git-scm.com). vsd init literally calls bash
- renode (see ../UserManual/install.md)

vsd init will install zephyr and zephyr sdk so dont worry abt those

# running vsd

run the bat file i made: start-vsd1.bat
first run takes a while (venv, pip deps, zephyr clone, sdk, builds the web ui) after that it's quick
to open vsd its hosted at http://localhost:9000
you can kill it with ctrl+c

## using it

1. you load a graph: use "load graph file" in the ui, pick something from graphs/ or start from scratch and save into graphs/ when you're done

2. build compiles zephyr for the board you drew

3. run simulation boots it in renode


there's a test graph in graphs/stm32-led-thermometer.json you can load it to see if its working

## if it breaks

- "python not found" install python and tick add to PATH
- "node not found" install node lts
- "bash not found" reinstall git for windows, make sure git bash is included
- "pip install failed" need network on first run. it pulls deps from github
- "vsd init failed" the log above the error is the real one. usually network, bash, or npm
- renode missing ../UserManual/install.md

the launcher patches vsd-env.yml after init so it uses your windows renode install, vsd init itself  tho downloads a linux renode portable which is useless here but its like, jus ignore it bruh, if you uninstall sum you might fuck everything up.

## updating the vendored vsd source

it's a plain copy of the antmicro repo (no .git inside). to update: clone the upstream repo somewhere else, copy its files over visual-system-designer-app/, delete the .git folder if you cloned one. commit.

