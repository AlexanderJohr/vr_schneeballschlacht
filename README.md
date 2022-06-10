# VR_Schneeballschlacht / VR_Snowball War

Connect both computers via directly LAN cable.

The host computer:
Set the static IP to 192.168.0.1 and the subnet mask to 255.255.255.0.
Start the game and press: "Play and host".
The host adress should display "127.0.0.1".


The client computer:
Set the static ip to 192.168.0.2 and the subnet mask to 255.255.255.0.
Start the game and press: "Join".
The correct IP Adress 192.168.0.1 should already be typed in.


For development:
Never check the option "Web Sockets" in the Network Manager. It causes the connection via static IP to not work.
