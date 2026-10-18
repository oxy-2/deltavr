
This week started with me attempting to install RaspOS on a Raspberry PI. It took me 3 hours and I only got 30 minutes of recording due to the fact that most of it was on the same frame which i wasn't going to upload for obvious reasons.

![[RaspOS Imager verifying the installed files.png]]
*RaspOS Imager verifying the installed files*

*Lapse of the installation of RaspOS:*
https://lapse.hackclub.com/timelapse/NL7N7xp_Ja0K


The next lapse I made was of researching how SLAM works and it taught me a lot on how it works. From my understanding, the software uses the texture on frames to map out feature points on a map. Then the IMUs calculate the inertial values to find the speed, acceleration and that the trackers were on and for how long. This determines the position the trackers are in on a 3D scale and at the same time makes the map of the room with the cameras (kind of like lidar). Now This ISN'T the definitive truth and it's just my understanding of it.

![[Wiki page for SLAM (1).png]]
*Wiki page for SLAM (1)*

![[Wiki page for SLAM (2).png]]
*Wikipage for SLAM (2)*

I wrote my research as bullet points on a TXT file which is in the repository.
I also made yet another attempt with the Raspberry PI which ended in failure 