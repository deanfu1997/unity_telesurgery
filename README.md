# unity_telesurgery
This is the most updated repo for the IMU telesurgery project, as of 03/29/2022. 

## Installation

1. Clone the repo
   ```sh
   git clone https://github.com/deanfu1997/unity_telesurgery.git
   ```
2. Open an example scene
   ```
   .\Assets/Scenes/Ball Joint.unity
   ```
3. Enjoy!
## Dependencies
* Unity 
* Matlab (for calibration)

## File to open
* `Assets/Scenes/Ball Joint.unity` if you want to run the IROS script
* `Assets/Scenes/PSM_Control.unity` if you want to run the hand-held device script

## Scripts description
* **`balljoint.cs`**

>This is the main script that is attached in the Scene **Ball Joint**, in the start() function, you can find the initialization code for the IMUs, which were copy and pasted from the OpenZen (LP-Research IMU unity library) example code. In the update() function, you will find the bulk of commented codes for calculating the ring pose, generating transformation matrices, etc. 

* **`PSM_Main.cs`**

> This is the main sciprt that's attached to the Scene **PSM_Control**. Has very similar structure as `balljoint.cs` The main difference is this script uses `MyButton.cs` helper function.

* **`Counter.cs`**
>A public class that stores static variables for different scripts to access.

* **`end_trigger.cs`**
>The trigger for collider of the stop buttton.


* **`MyButton.cs`**
>A public class for button variables. 

* **`praser.cs`**
>Praser function that prases received UDP inputs.

* **`startTrigger.cs`**
>The trigger for collider of the start buttton.

* **`Trigger.cs`**
>The collision detection script for the wires and obstacles.

* **`UdpConnection.cs, UDPReceive.cs, UDPSend.cs`**
>A ll UDP functions that ensures communication between the unity program and HoloLens.

* All other scripts are either not used or helper functions/classes, which you can figure out using the comments in the individual scripts

---
## Matlab folder
> This folder contains all the Matlab functions and some plots generated for calibration of the link length of the user. 
---
### KeyCodes Description (keys to press when running the unity application)

**V**: Toggle visibility of the ring/human arm

**C**: Switch between tasks (straight wire, S-shaped wire, piece-wise wire)

**I**: Let the IMU input teleoperate the ring

**U**: Let UDP teleoperate the ring (while communicating with MTM)

**N**: Generate a new data file (when doing user study)

**S**: Send the current ring position and orientation to **HoloLens**

**R**: Record transformation matrices for calibration purposes

**A**: Start a new calibration trial

**J**: Mock json string to debug UDP serilization

## Plugins Used

* [OpenZen](https://bitbucket.org/lpresearch/openzen/src/master/) IMU plugin that runs in unity.
* [Uduino](https://marcteyssier.com/uduino/) Arduino plugin that runs in unity.
* [NonConvexMeshCollider](https://assetstore.unity.com/packages/tools/physics/non-convex-mesh-collider-84867) Mesh of a non-convex shape for collision detection of the ring


