
---
Naming gpio's :
A grand whoping 1 hour of this week was spent finishing a task that i started last week, that is the naming and sorting of all gpio connections between the pro micro nrf board we are using and all the peripherals.

![[Gpio naming showcase.png]]

My group member liked this comprehensive assortment of connections and links between peripherals so much that he asked that i add it to our github repository, so now it is apart of the project files instead of being a little list of notes that only i use for making the firmware in the future.

https://lapse.hackclub.com/timelapse/yI_2YMaBX_g9
https://lapse.hackclub.com/timelapse/dq2dJCWjqWjy
---
Looking for components :
After last weeks failure to get a logical handle on the workings of renode the hellish emulation software, i decided to look for only the necessary components to make a real life testing device, it would have two controllers with all their peripherals and id be able to use arduino ide most likely to program them by writing c in a comprehensible environment and quite quickly get the firmware working, with a little bit of debuging, but it would at least be better than renod because it would actually let me test. Is it reasnable to say i curently dont like renode? anyway.

I looked for components on ali express and found a some good options but when i finally thought i could check out the price had doubled because of eu import tax, so i started thinking and remembered about alibaba, witch in my experience is usually cheaper by a significant margin.

![[cheeper components than on aliexpress.png]]

Later i spoke to my team and double checked what items i had found and i was informed that we have switched out a radio module from the old one witch had a range of 1km to a much more reasonable module 1.1km, because that is not excessive at all for a vr headset and its controllers witch will not ever be more than 2 meters away.

![[new radio module.png]]

After talking about the prices and noticing that shipping for me from china costs close to zero while for my teammates it costs an arm and a leg, we had the idea that maybe i can order the components from china and send them to them as that would still be cheaper. while we were talking about that i was tasked with checking the price for a type of camera and its lenses as the headset will utilize cameras as well. for my teammates the camera and lenses combo costs close to 300 euro while for me it was only 100 euro so it is very likely that i will be a sort of logistician for our team. 

![[camera and lenses.png]]

https://lapse.hackclub.com/timelapse/tYNXB_Xn2c5I
https://lapse.hackclub.com/timelapse/TuPlermgICmS
https://lapse.hackclub.com/timelapse/mzj91E8dXs07
https://lapse.hackclub.com/timelapse/WBEe1AnuQdcK
---
Learning to emulate with Renode : 
Same as last week i am fighting with this software and trying to figure out how it works. I used ai a fair bit to try and setup the environment in witch i will work in, as the documentation or any information found about renode is not understandable unless you already know how it all works. 

I dont know if this software really is as complex as it seems as i have gotten it to work, but understanding the filesystem if i can call it that is another thing.

![[file system showcase.png]]

I do believe that i have made a certain breakthrough as i figured out the three primary file types that you need to have to describe and run a simulation.

The first being a ".repl" file that contains the system description like what cpu is being used, memory size and type, peripherals and also communication protocols.

![[small chunk of repl file.png]]

The second being a ".elf" file that is simply the compiled c code that i write as firmware.

![[uncompiled c code that will become an elf file.png]]

The third and last being a ".resc" file that contains launch commands, creates the virtual machine, loads the .repl and .elf, opens windows for logging outputs and lastly starts simulation.

![[resc file.png]]

https://lapse.hackclub.com/timelapse/IxfJfZzC3tDP
https://lapse.hackclub.com/timelapse/awH_BEz8IXAa
https://lapse.hackclub.com/timelapse/fhpDfgZU8DIh
https://lapse.hackclub.com/timelapse/aShfHadrIlBc
---
All lapses in one place (11 lapses) :
Naming gpio's :
https://lapse.hackclub.com/timelapse/yI_2YMaBX_g9
https://lapse.hackclub.com/timelapse/dq2dJCWjqWjy
Looking for components :
https://lapse.hackclub.com/timelapse/tYNXB_Xn2c5I
https://lapse.hackclub.com/timelapse/TuPlermgICmS
https://lapse.hackclub.com/timelapse/mzj91E8dXs07
https://lapse.hackclub.com/timelapse/WBEe1AnuQdcK
Learning to emulate with Renode :
https://lapse.hackclub.com/timelapse/IxfJfZzC3tDP
https://lapse.hackclub.com/timelapse/awH_BEz8IXAa
https://lapse.hackclub.com/timelapse/fhpDfgZU8DIh
https://lapse.hackclub.com/timelapse/aShfHadrIlBc
Making the devlog :
https://lapse.hackclub.com/timelapse/nyf27BeVddA7
---
The end :)