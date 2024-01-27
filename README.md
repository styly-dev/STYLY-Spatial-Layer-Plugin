# STYLY VisionOS Plugin

### What is this plugin?
* You can upload your Unity contents to [STYLY for Spatial Layer](https://spatial-layer.styly.cc/) and can distribute the contents to Vision Pro STYLY app.
* STYLY for Spatial Layer is currently supports only VisionOS, and its code base is different from STYLY-VR or STYLY mobile. The contents are not compatible with STYLY for other devices.
* STYLY for Spatial Layer doesn't have 3D contents creation tools for now.

### Notes
* Please give us your feedback on Issues.  

#### Requirements

* Unity
  * Unity 2022.3.18 or later (Use 2022.3.XX)
  * Modules:
    * visionOS Build Support (experimental)
    * iOS Build Support
  * Render pipelines: URP
  * Color Space: Linear
* Xcode
  * Xcode 15.2 or later
* Apple Silicon Mac (M1/M2/M3)

#### Files

* [STYLY-VisionOS-Plugin](https://drive.google.com/uc?export=download&id=1opDf03H5RrfYrJmeDMfXw3WCMrJ4nZWU)
* [STYLY app for Vision OS Simulator](https://drive.google.com/uc?export=download&id=1GR4Xw14_gMSG_fW7dyXPoTyFde4D6Vwz)

#### Supported features for STYLY

* Mixed Reality mode: Bounded Volumes, Unbounded Volumes
* Standard URP shaders: Lit, Simple Lit, Unlit and some shaders in supported Unity packages
* Visual Scripting
* Timeline, animator, animation, audioclip and other basic features. See [Supported Unity Features and Components](https://docs.unity3d.com/Packages/com.unity.polyspatial.visionos@0.7/manual/SupportedFeatures.html) page for the detail.

#### **NOT** Supported features for STYLY

* Fully Immersive VR, Windowed Apps
* Your C# scripts
* Custom shaders
* Video player
* Visual Effect Graph
* Postprocessing Stack

#### Supported Visual Scripting Unity packages

* [PolySpatial Visual Scripting Nodes](https://openupm.com/packages/com.styly.polyspatial-visualscripting-nodes/)
* [VRM Visual Scripting Nodes](https://openupm.com/packages/com.from2001.vrm-visualscripting-nodes/)
* [glTFast Visual Scripting Nodes](https://openupm.com/packages/com.from2001.gltfast-visualscripting-nodes/)
* [Spectrum Visual Scripting Nodes](https://openupm.com/packages/com.from2001.spectrum-visualscripting-nodes/)
* [WebRequest Visual Scripting Nodes](https://openupm.com/packages/com.styly.webrequest-visualscripting-nodes/)

  You are moe than welcome to recommend new custom Visual Scripting nodes to support.

#### How to setup

* Unity

  * Install Unity via [Unity Hub](https://unity.com/unity-hub)

    * Install Unity [2022.3.18](https://unity.com/ja/releases/editor/whats-new/2022.3.18) or later (Use 2022.3.XX) with modules:
      * visionOS Build Support (experimental)
      * iOS Build Support
  * Create a new project as 3D (URP)
  * Switch platform to VisionOS (Experimental) in Build Settings
  * Add `STYLY-VisionOS-Plugin`

* STYLY-VisionOS-Plugin

  * Option A: Use [OpenUPM](https://openupm.com/packages/com.styly.styly-vision-os-plugin/) (Recommended)
```sh
# Install openupm-cli
npm install -g openupm-cli

# Go to your unity project directory
cd YOUR_UNITY_PROJECT_DIR

# Install package
openupm add com.styly.styly-vision-os-plugin
```

  * Option B: Add git URL  
https://github.com/styly-dev/STYLY-VisionOS-Plugin.git?path=Packages/com.styly.styly-vision-os-plugin

* Xcode

  * Install Xcode 15.2 or later with VisionOS simulator. [Xcodes](https://www.xcodes.app/) is the easiest way to install and manage multiple versions of Xcode.
  * Open Simulator from Xcode menu. Click`Xcode` -`Open Developer Tool` -`Simulator`
  * Open Vision Pro simulator from menu. Click`File` -`Open Simulator` -`VisionOS 1.0` -`Apple Vision Pro`
  * Install STYLY for VisionOS into the simulator
    * Download STYLY-Vision-OS-App.app and drag the app file to the simulator window from Finder.
![Simulator](https://github.com/styly-dev/PolySpatial_VisualScriptingNodes/assets/387880/0e2da2ad-f45c-4452-b71e-9339ade58fd6)

### How to build and upload contents

Unity prefab can be built and uploaded to STYLY. Right click a prefab in a project window and select `STYLY` - `Build Content FIle`

![Right Click](https://github.com/styly-dev/PolySpatial_VisualScriptingNodes/assets/387880/bb5b85d5-4106-4561-aeca-cc8a5297c5cd)

Built file will be created as `yyyyMMddHHmmss.styly` in `_Output` directory in your project.
![Output file](https://github.com/styly-dev/PolySpatial_VisualScriptingNodes/assets/387880/555c708b-787c-48a6-ba93-13c168643f44)

STYLY for VisionOS webpage will be opened in a browser. Login with your STYLY account.
![image10](https://github.com/styly-dev/PolySpatial_VisualScriptingNodes/assets/387880/b82c11f0-706d-434e-9b50-c67b6eca11f9)
![image11](https://github.com/styly-dev/PolySpatial_VisualScriptingNodes/assets/387880/3570cd01-34a0-46a3-b927-f8d087db330b)

Upload your built contents (.styly file) with title, description and display type.
Bounded: Your contents will be displayed in 1m x 1m x 1m size. Contents can be displayed with other apps.
Unbounded: Your contents will be displayed exclusively in a space.
![image12](https://github.com/styly-dev/PolySpatial_VisualScriptingNodes/assets/387880/cb70d668-9970-4d48-8d0d-941654e6aab1)
![image13](https://github.com/styly-dev/PolySpatial_VisualScriptingNodes/assets/387880/9c3e2644-d2de-4bed-bcd4-b5e2eac3f098)

### How to play the contents on STYLY

* Open content page on VisionOS simulator. You can copy and paste your URL of the content from your Mac to the simulator.
* Click `Play on device` on the page.
* Your content will be displayed.

