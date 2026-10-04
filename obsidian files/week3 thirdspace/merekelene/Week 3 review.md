
---
Double checking :
Spent a while talking with my teammate to make sure in what way certain peripherals are connected to make sure that the emulation is correct.

![[nrf board gpios.png]]

https://lapse.hackclub.com/timelapse/R5sW3ClU1utM
---
Components, Renode, Instruction manual :
In attempt to try and avoid the EU import tax, I looked at some regional electronics components sellers in hope that maybe they end up being cheaper, they were not, except for one item which if I would order would end up being more expensive because of shipping.

![[digikey.png|337]]
![[robotshop.png]]
![[mouser.png|340]]

So I'm going to save up some money and order from the cheapest option so far witch is alibaba.

Next I worked on actually understanding what I'm doing in renode, as previously i had no idea what was actually happening. I started by seting up a working run script to easily run the simulation.

![[launch file 1.png]]

It makes a machine, loads the micro controller itself onto it, loads the compiled code, opens a debug window and lastly starts the simulation.

Next I updated it to load two machines at once, nothing really different just two bocks of code instead of one.

![[launch file 2.png]]

After this great feat I decided to get on writing a simple renode user manual for my team so that they could also use the simulation to test firmware. I started by writing the installation manual.

![[install manual.png]]

Then i wrote the startup guide, so that my teammates know how to launch the simulation.

![[run manual.png]]

Lastly I started writing a compilation of useful commands although the software itself provides a list, so I might remove it.

https://lapse.hackclub.com/timelapse/yrCdkR12VN5a
---
Coding :
Most of the work this week was recorded with hakatime. I programmed the nrf24l01 module and also the lsm6dsv but testing them was unsuccessful as the simulation refuses to launch, so I will need to track down the cause of this, next week.

![[error.png]]

https://lapse.hackclub.com/timelapse/Oa-JVPVgzEIj
---
All lapses in one place (4 lapses) :
Double checking :
https://lapse.hackclub.com/timelapse/R5sW3ClU1utM
Components, Renode, Instruction manual :
https://lapse.hackclub.com/timelapse/yrCdkR12VN5a
Coding :
https://lapse.hackclub.com/timelapse/Oa-JVPVgzEIj
Making the devlog :
https://lapse.hackclub.com/timelapse/VGnJ9vysu7tV
---
:)