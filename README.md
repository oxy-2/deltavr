hi im oxy and im working with 2 of my friends (John/monizjoao and grand/merekelene) now to bring everyone a opensource, pcvr, light weight slam tracked headset with constellation tracked controllers and if you are able to source the parts right it could be possible to build your own under 700 euros.

# deltavr

## summary
so to sum up everything in this project, the pcbs are custom for the headset and for the controllers, we are using high performance imus those being the lsm6dsvs, the pcbs are majority smd, the controllers and headset use the widely available and powerfull NRF52840 "pro micro" boards of which are similar to the nice!nano v2, from communication between the headset and controllers gt-24 nrf24l01 boards are used which allows for antennas externally but work great with the MIFA they have, our optics are a big point of this headset too, being that they are pseudo pancakes, basically two fresnels stacked in a way that we can get close to pancake performance but instead of spending hundreds its about 20 euros in total, the entire headset apart from the linear rods is 3d printable and we are doing ours in asa which we will smooth later to get a very nice smooth finish, the headset uses 5 OV9281 120fps global shutter usb cameras which go to a usb 3.2 gen 2 hub which then goes to a pc, speaking of obviously this headset is pcvr, maybe some day it will be mobile but we dont have anything yet in plans for such a thing for the first versions atleast, the last couple points woud be that the controllers are actually nice, sporting a tmr joystick to ensure they do not drift ever while using a low ammount of battery and also 2 hall effect triggers allowing for not just on of off triggers but a range of values.


## reasoning on protocols used:
firstly the usb cameras, they are expensive, alot more so that mipi when they are the usb version, but we do not have any real way of actually converting camera mipi to one central usb as that would alone be an engineering project, secondly the protocol for the mcus to communicate is Nordic Semi's ESB (Enhanced ShockBurst) protocol which we are using for better battery life with great connectivity.

## what we have done right now
as of writing this (october 1st 2026) we have the PCBs fully completed which i did over the previous summer, we have the full bill of materials and general plan too done and a general idea of the 3d design for the controllers while we are actively modeling the headset rn and have it about 30% done id say, the PCBs do need one made for the left version of the controllers but that is a very simple thing to do and i will be doing it soon, along with a full bom we have all the parts fully sourced and ready for purcahse through aliexpress and alibaba for their competetive pricing of electronic components and availability of them, we are sourcing really just the cameras from alibaba and also the imus we are purchasing in bulk from a seller directly who was found on alibaba at first.

## where can you see our work?
well theres my personal site at oxygenated.uk but i still need to update its information and then theres this github repository and the weekly reports for thirdspace my team and i write which include lapse links of which can be used to view all the work we have done in a time lapse format.


thank you for your time.

\-oxy <3



JohnTarkov: Hi im JohnTarkov Im working on the project too and i updated this so i can get my hacktime functioning for the project to use Lookout to record my project.

