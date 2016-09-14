# SiloSim
Simulates a PLC and a truck scale representing a single asphalt loadout lane with up to five silos. Specifically, it emulates an Allen Bradley Micrologix 1100 PLC loaded with Command Alkon's JWS Apex Loadout program and a Cardinal Scales weigh indicator. 

##I.	Getting Started
Before using SiloSim with JWS Apex, you must first configure the virtual Scale and PLC.

###A.	Scale Setup
From the Apex menu, navigate to **System Administrator > System Setup > Devices/Scales > Scales** and open the **Scales** configuration application. Add a new scale for the simulated scale. Use the default settings on the **General** page, then go to the **Interface** page, and set the **Scale Type** to be **Cardinal Model 738**. Check the box for **Live Interface**. Set the **Interface Method** to **Ethernet TCP/IP**. Make note of the IP address of the machine where SiloSim is installed, and put the IP address into the **Address** field. Then for the **Port**, type the port that SiloSim is using for the scale. The default scale port for SiloSim is **10001**, but this can be reconfigured later.

###B.	PLC Setup
From the Apex menu, navigate to **System Administrator > System Setup > Devices/Scales > Scales** and open the **Scales** configuration application. Add a new scale with the PLC numbering scheme. PLC scales start at **10201** for the first PLC and continue with **10202**, **10203**, etc. Use the default settings on the **General** page, then go to the **Interface** page, and set the **Scale Type** to **None**. Set the **Interface Method** to **Ethernet TCP/IP**. Make note of the IP address of the machine where SiloSim is installed, and put the IP address into the **Address** field. Then for the **Port**, type the port that SiloSim is using for the PLC. The default PLC port for SiloSim is **11001**, but this can be reconfigured later.

###C.	Shortcut Setup
Creating a shortcut to SiloSim allows you to automatically load the port settings and assign a label to SiloSim. This is especially useful if you plan on running more than one SiloSim at a time to simulate multiple loadout lanes. First, navigate to the folder where SiloSim.exe is located. Right-click on the executable and select Send to>Desktop (create shorcut). Then go to the Desktop and right-click on the newly created shortcut and select Properties. In the properties window, in the Shortcut tab, select the text in the Target field. At the end of the Target, you can add the port number for the Scale and the port number for the PLC in addition to the label. The format is as follows:

`C:\JWS\SiloSim\SiloSim.exe <ScalePort> <PLCport> <Label>`

An Example for Lane 2 where the scale uses port 10002 and the PLC uses port 11002 would be:

`C:\JWS\SiloSim\SiloSim.exe 10002 11002 “Lane 2 Sim”`

##II.	Main Controls

###A.	Label
This text box enables you to set the label in the title bar for the simulator

###B.	Stop/Start Simulation
This button starts and stops the simulation server. In order to change the port that the PLC or Scale is listening on, you must stop the simulation first.

###C.	Inventory Controls

####1.	Filling Controls

#####a)	Bin Inventory
Each silo has a text box that shows the current unsynced bin inventory. This number can be set manually by typing in a number and pressing enter.

#####b)	Fill Select
These radio buttons allow you to choose which of the five silos to currently fill. You can only fill one at a time just like in a real plant. Click on Fill Deselect to deselect all silos for filling.

#####c)	Low Bin
Each bin has a checkbox to indicate a low bin level. In a real plant this would come from a sensor, but in the simulator, this is a manual control.

####2.	Loadout Controls

#####a)	Loadout Select
These radio buttons allow you to choose which silo is currently selected for loadout. Only one at a time can be selected for loadout. Click on Loadout Deselect to deselect all silos for loadout.

####3.	Weighbridge Controls

#####a)	Fill Rate (Tons/hr)
This text box allows you to enter the fill rate for filling the silos.

#####b)	Start/Stop Filling
Click this button to start or stop filling the silos. You should see the silo inventory increase as the weighbridge sends pulses to the silo that is selected for filling.

#####c)	Common Pulse/One Per Silo
These radio buttons allow you to choose between two different filling methods. Input number 13 indicates which mode is currently selected. The Common Pulse method will set Input 13 to be always on, the One Per Silo method will set it to be always off. 

In Common Pulse mode, as the weighbridge fills the silos, the Tons Pulse will be visible on Input 14, and the silo is selected with Inputs 15,16,17,18,19.

In One Per Silo mode, as the weighbridge fills the silos, the Tons Pulse will be visible on the individual Input channel for each silo including Inputs 15,16,17,18,19 .

####4.	Scale Controls

#####a)	Increment Weight (+)
This button will increment the weight on the scale by 20 lbs.

#####b)	Decrement Weight (-)
This button will decrement the weight on the scale by 20 lbs.

#####c)	Zero Scale (Z)
This button will set the scale weight to zero.

#####d)	Tare Weight
This text box allows you to set the tare weight of an imaginary truck on the scale. To set the scale to the tare weight in the box, click the TARE button.

#####e)	Gross Weight
This text box allows you to set the gross weight of an imaginary truck on the scale. To set the scale to the gross weight in the box, click the GROSS button.

#####f)	Net Weight
This text box allows you to set the net weight of an imaginary truck on the scale. To set the scale to the net weight in the box, click the NET button.
