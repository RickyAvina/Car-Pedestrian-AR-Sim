# Pedestrian Simulation via Hololens and ROS

## Dependencies
* Unity 2018.x
* .Net 4.x
* Visual Studio 2017+

Tested with:
* Unity 2018.2.17f
* Visual Studio 15.9.3
* .Net 4.x equivalent

## Installation

#### Unity (Windows or Mac System)
* Download project.
* Open the project which is the *ACL_Holo_ROS/ACL_Holo_ROS* folder
* In Unity, Open a [scene](https://github.com/RickyAvina/Test_ACL_Holo/wiki/Scenes) under the *Scenes* folder
* In the *PedestrianController* object, change the **serverURL** to your ROS URL.
* Build (Ctr+Shift+B) to a new folder (make sure *Unity C# Projects* debugging is off.
* Once the project is done building, navigate to the *.sln* file, (ex: *BaseProject.sln*)
* Deploy to the Hololens within **Visual Studio** using [these](https://docs.microsoft.com/en-us/windows/mixed-reality/using-visual-studio) instructions.
#### ROS (Linux System)
Any ROS publisher that uses WebSocket (with port 9090) will work. To use the pedestrian simulation engine follow [these](https://github.com/blutjens/pedestrian_simulation) instructions. *Note:* It is useful to assign a static IP to this computer so you do not have to recompile the project everytime the IP changes.

##
Affiliation: MIT ACL
