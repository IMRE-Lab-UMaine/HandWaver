# Package Dependencies
### TextMeshPro

# Asset Dependencies

### LeapMotion Orion
[LeapMotion](https://github.com/leapmotion/UnityModules)
Used as an input system, interaction system and gesture system for LeapMotion and OSVR controls.  (Essential to Function)

Free, Open-Source asset.  LeapMotion controller functionality dependent on closed source drivers currently only available on Windows.  OSVR functionality works without the closed source driver.

- [x] Core SDK
- [x] Interaction Engine
- [ ] UI
- [ ] Graphics
- [x] Hands


### LeapMotion App Modules
[LeapMotion](https://github.com/leapmotion/AppExperiments)
Free Asset. Used as a main control interface. (Essential to function).  This asset requires some modification in order for it to compile.  We hope to document this process (or include a copy of  the source) in the future.

### Photon PUN+
[Exit Games](https://assetstore.unity.com/packages/tools/network/photon-pun-classic-12080)

Paid Asset. Used to add multiplayer support. (Essential to function)


# Optional Add-Ons whose integration has been teseted.
Please restrict use of these closed-source assets to (1) a fork of the repository or (2) a local copy of the repository.  No code dependant on these solutions should be included in the main repository.


### BlueprintReality - MixCast SDK
[BluePrintReality](https://mixcast.me/mixcast-download/)

Free Asset. Used to record mixed reality footage. (Non-essential to function)

### TiltBrush Toolkit
[Google](https://github.com/googlevr/tilt-brush-toolkit/releases)

Free Asset. Used to import TiltBrush drawings into the project. (Non-essential to function)

### HTC Unity Plugin
[HTC Github](https://github.com/ViveSoftware/ViveInputUtility-Unity/releases)

Free Asset. Will be used for spacial audio and locomotion. (Non-essential to function)

### Viveport
[HTC Vive](https://developer.viveport.com/documents/sdk/en/download.html)

Free Asset. Will be used for telemetrics. (Non-essential to function)

### Pool Manager
[Path-o-logical Games](http://u3d.as/1Z4)

Paid Asset. Object pooling for GeoObjects and GeoUI. (Essential to function)
We have used this in the past for instancing objects, but arem working to be independent of it.
PoolManager may offer some performance gains, and can be dropped into the application by simply reomving the PoolManager.cs script in the Assets/Scripts folder.

### SteamVR
[Valve](http://u3d.as/cjo)

Free Asset. 