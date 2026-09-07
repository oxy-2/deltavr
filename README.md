# deltavr

me and a friend build a open source thin vr headset from scratch with stacked fresnel lenses to act as pseudo pancake lenses, high performance lsm6d family imus, inside out SLAM tracking for the hmd unit via 4 ov9281 cameras and ORB-SLAM3 along with constellation and imu tracked controllers using fully rechargeable and replaceable easy to source 3.7v LiPo batteries.



so the headset is made up hardware wise of a couple components, a driver board for the displays, the displays obviously, the imu, the four cameras, the usb hub for the cameras, the hmd board which has a nrf24l01+ and a nrf52840, the controllers and hmd communicate over Nordic's enhanced shockburst (ESB) protocol.



as of now the pcbs for the headset and controllers are officially done, we dont have firmware yet but you can take a look at everything, in the kicad folder youll find the project file for the hmd pcb and the controller pcb is in kicad controller R (the R is for right hand, its the only one shipped. for the left hand just copy the project and mirror it in pcbnew: edit -> flip board view... actually just select everything and mirror about the Y axis, its designed symmetric so it just works). everything needed to fabricate is in the release: gerbers, drill files, BOMs, schematics and renders. you see some more stuff and project stats on my website page :D


thanks for reading this

\-oxy <3



JohnTarkov: Hi im JohnTarkov Im working on the project too and i updated this so i can get my hacktime functioning for the project to use Lookout to record my project.

