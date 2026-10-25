Started this week by doing some research on how to install and set up Renode to work on the firmware. This was pretty straight forward although later when I actually used Renode it was confusing as I never even touched a piece of firmware until now. 

I also set up Wakatime for the Jetbrains toolbox as this was going to be needed for later use.
Oxy also introduced me to Verks.ai which was basically a VSD (Visual System Desktop) to visualize the firmware dataflow and simulate it. It took a bit of setting up with terminal but after that it worked nicely.

![[Research paper on renode.png]]*Research paper on Renode*

![[Verks.ai web tool.png]]
*Verks.ai web tool*

*Lapse for setup and research paper: https://lapse.hackclub.com/timelapse/uHxeFh1US1V7*


Then Oxy asked me to fix the controller graph which had to be done through Windows PowerShell.
I'm going to be honest I did not know what I was doing and I just followed Claude's instructions because I genuinely didn't even know where to start.
What's worst is that halfway through I realized I was trying to fix the wrong thing which was just the news I needed.
I ended up fixing the wrong file but I did fix it and it helped me understand a little bit better how Renode worked.

![[Trying to fix the VSD file.png]]
*Trying to fix the VSD file*


*Lapse for Renode debugging: https://lapse.hackclub.com/timelapse/_NvP0If8nRdc*


The next session I attempted to fix the file that had the bug but what ended up happening is that Oxy assigned me to Blender duty to make a render for the headset.

So I started making the scene, watched a quick Youtube tutorial and followed it and it turned out pretty good with some lighting and the shadows.
I imported the .obj file which was tiny so I scaled it up a lot. Then I made some customizations. I made the lens more or less transparent, made the headset a different color and whatnot.
Then I went into PolyHaven to find a good HDRI for the lighting, found a good one and downloaded it. I also created some light entities just to make the lenses glow a bit like the displays were on.
After that I made an infinite backdrop for when I rendered the image for it to look nice in the white void. This proved to be kind of hard to do since it was my first time on blender.

![[Attempt to fix the correct file.png]]
*Attempt to fix the correct file*

![[Importing the headset model.png]]
*Importing the headset model*

![[Finding a good HDRI for the lighting.png]]
*Finding a good HDRI for the lighting*

![[Infinite backdrop.png]]
*Infinite backdrop*


*Lapse for blender and Renode: https://lapse.hackclub.com/timelapse/iN63KZC3bX9w*


Finally, as the last day of the week, today I rendered the scenes I made in blender off camera. And on camera I started modeling the controllers.

The controllers were pretty easy to model all I did was make a circle for the bottom shell. I didn't have to worry about the PCB size because Oxy and Grand already had modeled it so it made my job easier.
After that I made a sketch with a square block to start on the shaft. I rotated the block in such an angle that it'd be comfortable to hold. Then I extruded the circle of the bottom shell and guided it across the path of the shaft which made it better to hold since it was actually a circle.
After that I made a compartment for the radio module that was going to be put in the controller.
The next thing I did was remove a small bit of the controller for the middle finger to hold onto
I noticed the new shaft was kind of chunky which would be uncomfortable and heavy to hold. So I cut it in half and since the controllers will just be mirrored pretty much then its fine.
Of course during all of this I filleted everything so it's as round as possible as to improve comfort.


![[headset front.png]]
*Headset Front*

![[headset back.png]]
*Headset Back*

![[Shell bottom highlighted.png]]
*Shell bottom highlighted*

![[Radio module compartment.png]]*Radio module compartment*

![[Controller prototype model model.png]]
*Controller prototype model model*

![[Middle finger holding dent.png]]
*Middle finger holding dent*


And then I wrote this entry as per usual on Sunday.

*Lapse for Journal writing, controller modeling and etc:
https://lapse.hackclub.com/timelapse/lsz5VshBd6fL*

___
Weekly thoughts: Still confused about the Lapse and Thirdspace desync in time. For example as of right now of me writing this (before uploading the lapse) Hackatime (which is what is connected to Thirdspace) tells me I have 7 hours and 30 minutes give or take. However the same isn't true for the Lapse times. These times say I have 6 hours and 30 minutes which is 1 hour of time difference. This is not good as 1 hour could be make or break for something like Thirdspace. Anyways good news is next week the team is doing a break on DeltaVR and making a little side project to take our minds off of this one for a bit.

P.S. Oxy shipped the project way too early so I have to do a thingamajig and ship it to a different project apparently.

All Lapses:
https://lapse.hackclub.com/timelapse/uHxeFh1US1V7
https://lapse.hackclub.com/timelapse/_NvP0If8nRdc
https://lapse.hackclub.com/timelapse/iN63KZC3bX9w
https://lapse.hackclub.com/timelapse/lsz5VshBd6fL