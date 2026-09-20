
---
Setting up laptop :
I started this week by setting up my laptop which I had not yet been able to do up till now. This was so that I could work both at home and also on the move. This is probably one of the most important things I did as otherwise I most likely would not have had enough time to reach this weeks quota. 

The setup consisted of me installing apps that I will most definitely be using like vscode, renode, github desktop, lookout, hakatime.

![[laptop setup downloading.png]]

I will continue setup next week to include additional libraries needed for Renode.

https://lapse.hackclub.com/timelapse/IjtBbvyHWhr8
---
Learning Renode :
In the last week of the setup phase, I spent some time looking around for a emulator software to test my firmware on, as I am not in possession of the physical hardware that I will be programing. I found two very good options but of the two I ended up choosing Renode because of its wider support for peripherals and many likely uses outside of just this one event. 

![[emulation software.png]]

After finding Renode, I quickly and suddenly came to the realization that Renode is made up entirely by terminals and is not at all similar to what I had in mind, that being Tinkercad or Wowki.

![[emulation software showcase.png]]
(Example of how Renode looks)
After going through the 5 different stages of grief, I finally started trying to figure out how to do all the different things that are necessary to simulate a project. This took a very long amount of time as finding easy to understand and beginner friendly tutorials was difficult. 

with the help of Claude opus 5 I made a basic test project witch included a compiling script, run script and the general hierarchy of files that are necessary for building the virtual controller and also uploading the firmware to it.

![[emulation software test run.png]]
(Test program runing)
https://lapse.hackclub.com/timelapse/3nuWk_uo0MtO
https://lapse.hackclub.com/timelapse/apcl5atXomEH
https://lapse.hackclub.com/timelapse/UcAXlp--94xe
---
Understanding the PCB :
Rhime7274 who invited me to join him in this event had already been doing a lot of work before I joined the team so a significant amount of time was spent looking around the github page looking through schematics, lists of components and other sometimes unrelated files.

The main problem I ran into was that the only schematic showing all the gpio port connections to peripherals was very messy and hard to read so I spent two lapses working on putting them in a more readable format.

From this :
![[pcb.png]]

To this :
![[readable gpio pin conections.png]]

For me this seems like a easier to read format as I can find the connections between components and their pins in a quick glance. I predict this will come in handy when writing the firmware. I will need to update it a bit more but until I have spoken to Rhime7274 about the workings of each segment in depth, it will have to do.

https://lapse.hackclub.com/timelapse/7rRauqIhLzpM
https://lapse.hackclub.com/timelapse/YyGTl8KKhVPs
---
Github and Renode :
Rhime7274 came up with the idea to put my Renode app in the github repo so that all of us would be able to test the firmware virtually. This is my first time using gihub in this way, ever, so at the start trying to communicating where anything on my computer is located and why was frustrating even for me. We spent about half an hour finagling with my files trying all sorts of things but inevitably gave up.

This was all caused by my ignorance and lack of thinking of the future. I had put the Renode app files in a higher directory because otherwise it wouldn't compile, and it was causing new problems in different ways.

The fix wasnt all that hard, I moved the Renode app files to the gihub repo, merged, pushed, and lastly rewrote the compile script and also the launch script. I also had to tweak the .gitignore witch was a frightening experience for me but in the end I got it done.

https://lapse.hackclub.com/timelapse/XWEwpXMi7DrT 
https://lapse.hackclub.com/timelapse/eEOnstobjyAC
---
All lapses in one place :
Setting up laptop :
https://lapse.hackclub.com/timelapse/IjtBbvyHWhr8
learning Renode :
https://lapse.hackclub.com/timelapse/3nuWk_uo0MtO
https://lapse.hackclub.com/timelapse/apcl5atXomEH
https://lapse.hackclub.com/timelapse/UcAXlp--94xe
Understanding the PCB :
https://lapse.hackclub.com/timelapse/7rRauqIhLzpM
https://lapse.hackclub.com/timelapse/YyGTl8KKhVPs
Github and Renode :
https://lapse.hackclub.com/timelapse/XWEwpXMi7DrT 
https://lapse.hackclub.com/timelapse/eEOnstobjyAC
---
The end :)