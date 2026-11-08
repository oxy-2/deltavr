
---
This week was crazy. We had he greatest clutch up in VR history. Having to finish modeling the controllers, making multiple videos, writing documents and other small tasks to polish everything up.

My primary goal this week was modeling, using Joao's modelled controllers as a base, I expanded on them adding triggers, the antenna, smoothing out the design and adding other features to help aid in both assembly of the device and improve usability and strength.

Triggers :
Our primary modeler Joao was busy making a video about the project as a whole, so he was not able to help much. This was the first thing that I got to do. Up till now my skills in Onshape were underwhelming. While making the triggers I went through a few designs and multiple roadblocks, mostly associated with my lack of experience. As I kept working and also communicating with my teammates, I learnt how to use some new tools and also how to better use the tools I had been using since the start. Making complex and curved shapes was quite scary at the start, but once I figured out how to place sketches and use certain tools, it got a lot easier, although I still worked quite slow.

The design for the primary trigger that I settled on and that my teammates approved of is this :

![[primary trigger front.png]]
![[primary trigger back.png]]
![[primary trigger in place.png]]

![[primary trigger hidden stuff.png]]

The triggers are based on the ones found in the Quest 2 headset. The triggers work by having a rod shoved through them around which they swivel, two pen springs in the round holes on each side put pressure on the trigger so that it would return to its original position, the springs are glued in place as most thing on this project but if it works it works. The trigger uses a hall effect sensor to detect how much it has been pressed, behind the trigger in the middle square slot will be glued a hall effect sensor and in front of it on the trigger in a round hole will be glued a magnet, as the trigger is pressed the magnet gets closer and sends out a changing analog signal. The magnet is mounted about 5 mm from the sensor, as this is the optimal working distance.

For the secondary trigger the story is very similar :

![[secondary trigger front.png]]
![[secondary trigger back.png]]
![[secondary trigger in place.png]]

![[secondary trigger hidden stuff.png]]

![[secondary trigger hidden stuf.png]]

Also based on the Quest 2. This trigger has almost the exact same setup, with two round holes on the sides for pen springs and the center ones holding the magnet and hall effect sensor. Only difference is that this trigger does not swivel but instead slides forwards and back, so the button has a elongated hole, visible on the sides, and the shaft does not hold it in place as securely as the first trigger but still sets limits to the max movement that the trigger can make.

All together :

![[both triggers.png]]

Antenna :
To have better aesthetics and also improved functionality, we separated the antenna from the main pcb and have it connected with wires running to it. The antenna now sits just outside the pcb's boundary and is thus less affected by interference. 

The old:

![[old antenna placement.png]]

Originally sitting at the front of the controller and slightly tilted, it was hard to hide and the signal quality would be suboptimal.

The new:

![[new antenna placement.png]]

Now sits almost flush and still outside the negative effects of the pcb, providing better signal quality. It is also placed in a location that the your hand does not cover and pointed toward your head so the signal path is as short and direct as possible.

Alignment notches:
To help with assembly we decided to add a few notches to the outer ring where the top shell and bottom shell meet, this will help with smoother edge lines and structural integrity should also be better in theory.

![[notches bottom.png]]

![[grooves bottom.png]]
![[notches top.png]]

![[grooves top.png]]

Double or nothing:
Lastly, to actually finish this project we need two controllers, but we only have one, so we did a lovely thing called mirroring, and voila you don't need to spend hours painstakingly recreating the other controller.

![[both controllers.png]]

Appart from the pcb which did have to be mirrored by the hand of Rhime the process was very simple and worked first try.

The end:
Controllers now being done, I'm finally free. Although it may sound as if i did not enjoy my time, it was a lot of fun and my skill in onshape has now ascended even higher. I even managed to do things that Rhime was having trouble with such as covering the very front of the controller whitch once held the radio module. after moving it, there was a massive hole left behind and i quickly fixed it, while Rhime was star struck as to how i did it.

![[skill.png]]

---
All lapses in one place (5 lapses) :
https://lapse.hackclub.com/timelapse/3ifnc3oG8h8M
https://lapse.hackclub.com/timelapse/2Jm8s_XXZOSG
https://lapse.hackclub.com/timelapse/guDrli8v9C5Z
https://lapse.hackclub.com/timelapse/dRhNGa5v6G7G


---
:)