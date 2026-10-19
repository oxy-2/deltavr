so lets kick things off so this is for you grand and joão.

so we have renode, renode is our simulator it fully simulates our everything, technically our cameras could be simulated but for slam software we will just buy the headset later on, for now we just need to have a way to time the controller leds to a given rate and match a given heartbeat and adjust for yada yada but maybe that's better for software cuz we are writing it in c++ and our pcs are fast lol.

anyways so renode

# what is renode?

so renode is a application we are using to simulate everything, we basically have it and it manages stuff like zephyr rtos which is a real time os simulator yk, anyways so like it gives us the ability to write all the configurations, wiring and such and instead of a physical prototype we have a digital simulation that's very accurate, also we can link it up with external things, like if we buy the cameras first only we can make the slam stuff and start on it with just that cuz we can link the slam output data into renode and it will do real time simulation of the other stuff and we can write out the base software, but for real world tests, nothing can really just cover all possible edge cases we might get ourselves irl and stuff like the ir leds and its timing over wireless to a headset mcu and then that thru usb etc, but regardless for now renode is fucking amazing.


# what is vsd?

vsd is a thing from antmicro for renode, it's full name is visual system designer, it basically lets us draw out connections for the parts and such like unreal engine and its way to make games without coding, its nice and having a gui makes me not go insane personally, also we have "renode --ui" which is a way to run renode but with a nice looking windowed gui, in general renode has a lot of gui stuff which i was pleasantly surprised by, anyways vsd can run on verks.ai like here https://designer.verks.ai/vsd but u can also run it locally, which is prolly better but the site isn't bad and has google sso but wont work with your personal gmail, since i have a domain for my site, grand and joão you guys can just have a custom domain email ill make you lol.


# hackclub spaces?

as we are under 19 and have a hc acc we can use hc spaces it gives us cloud hosted website accessible instances of Blender, FreeCAD, VSCode and KiCAD which is lowk tuff ash, but yeah so make sure to sign up for that if your on the go often then well you can now have a persistent vscode server with any plugin you want as well like actual plugins, which is fucking awesome, and ofc kicad, freecad and blender.


# is firmware and software actually gonna be hell?

nah we was larping yo.

software is really easy and is just putting the firmware stuff all together and making a gui maybe or sum, maybe not rs and just c++ im not sure, c++ is good but c++ is like 