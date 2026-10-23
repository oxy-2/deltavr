# DeltaVR

![[Pasted image 20261023214732.png]]

![[Pasted image 20261023214838.png]]


DeltaVR is an open-source, lightweight PCVR headset built  to be a step up from the hadesvr headset while being more oriented towards technical users due to the smd components, it uses almost all custom hardware aside from the microcontroller, cameras, lenses, screens and drivers, and like linear rods ig, other than that everything is made by us with customs pcbs, and a almost entirely 3d printable design aside from again the ⌀5mm x 160mm stainless steel linear rods, the overarching goal of deltavr was an advanced opensource VR headset with large ipd accomidation, with inside out SLAM tracking and constellation tracked controllers with actually analog triggers on our custom controllers, all under 500 if you have the tools (we got all the way down to 390 actually!)

---
## Table of Contents

- [Features](##features)
    
- [Hardware Overview](##hardware-overview)
    
    - [Headset](###headset)
        
    - [Controllers](###controllers)
        
    - [Optics](###optics)
        
    - [Communication](###communication)
        
- [Design Decisions](##design-decisions)
    
- [Design Previews](##design-previews)
    
- [Firmware](##firmware)
    
- [Assembly](##assembly)
    
- [Bill of Materials](##bill-of-materials)
    
- [Development Status](##development-status)
    
- [Known Issues](##known-issues)
    
- [Project Links](##project-links)
    
- [Credits](##credits)
    
- [License](##license)

---

## Features

- 2x OV9281 global shutter cameras at 120 fps, combined by a usb hub with the hmd pcb all into one 3.2 gen 2 cable to your pc
- custom hmd pcb, mostly smd (hence the technical users bit)
- pseudo pancake optics with 2x fresnels per eye giving better dimensions for just 20 euros for all 4 of the lenses
- a big ipd range, accommodating for all possible human ipd ranges that aren't legit 1 in a trillion

```
headset
- 5x ov9281 global shutter cams, 120fps, into one usb 3.2 gen 2 hub, so inside out slam goes to the pc over a single cable
- custom hmd pcb, mostly smd (thats the "for technical users" part vs hadesvr)
- pseudo pancake optics: two stacked fresnels, pancake-ish for ~20 eur instead of hundreds
- big ipd range so it actually fits different heads
- printed in asa, everything except the rods, so you can reprint a part instead of waiting on shipping
- pcvr first, mobile only if we ever get there
controllers
- custom pcb per hand, same nrf52840 pro micro as the headset
- lsm6dsv imus
- tmr joystick, no drift and low battery draw
- 2 hall effect triggers with real analog range, not just on or off
- constellation tracked, no base stations
comms
- nrf24l01 (gt-24) on nordic esb, better battery life and range than bt or wifi
- antenna can go external over u.fl, but the stock mifa is fine
cost
- under 500 if you own the tools, we got it to 390
```


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

