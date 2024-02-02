# STYLY visionOS Plugin

### What is this plugin?
* You can upload your Unity contents to [STYLY for Vision Pro](https://spatial-layer.styly.cc/) and can distribute the contents to the STYLY app for Apple Vision Pro.
* Contents of STYLY for Vision Pro is not compatible with STYLY for other platforms. Its code base is different from STYLY VR or STYLY mobile. 
* STYLY for Vision Pro doesn't have 3D contents creation tools for now.

### Notes
* Please give us your feedback on [Github Issues](https://github.com/styly-dev/STYLY-VisionOS-Plugin/issues).  
* Your contents will be published to the PUBLIC. Don't upload your contents which you want make private. 
* Contents gallery on STYLY app is white list based now. STYLY team may pick up your contents display in the app.    

#### Requirements

* Unity
  * Unity 2022.3.18 or later (Use 2022.3.XX)
  * Modules:
    * visionOS Build Support
  * Render pipelines: URP
  * Color Space: Linear
* Xcode
  * Xcode 15.2 or later
* Apple Silicon Mac (M1/M2/M3)


#### Links
* [STYLY app for Vision OS Simulator download](https://drive.google.com/uc?export=download&id=1GR4Xw14_gMSG_fW7dyXPoTyFde4D6Vwz)
* [Website of STYLY for Vision Pro](https://spatial-layer.styly.cc/)
* [Github repository](https://github.com/styly-dev/STYLY-VisionOS-Plugin)
* [OpenUPM page](https://openupm.com/packages/com.styly.styly-vision-os-plugin/)

#### Supported features for STYLY VisionOS Plugin

* Mixed Reality mode: Bounded Volumes, Unbounded Volumes
* Standard URP shaders: Lit, Simple Lit, Unlit and some shaders in supported Unity packages
* Visual Scripting
* Timeline, animator, animation, audioclip and other basic features. See [Supported Unity Features and Components](https://docs.unity3d.com/Packages/com.unity.polyspatial.visionos@1.0/manual/SupportedFeatures.html) page for the detail.

#### **NOT** Supported features for STYLY VisionOS Plugin

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
      * visionOS Build Support
  * Create a new project as 3D (URP)
  * Switch platform to VisionOS in Build Settings

* STYLY VisionOS Plugin for Unity

  * Option A: Use [OpenUPM](https://openupm.com/packages/com.styly.styly-vision-os-plugin/) (Recommended)
    * Prerequisites : [Node.js 14.18 or above](https://nodejs.org/en/download/)
```sh
# Install openupm-cli
npm install -g openupm-cli

# Go to your unity project directory
cd YOUR_UNITY_PROJECT_DIR

# Install package
openupm add com.styly.styly-vision-os-plugin
```

  * Option B: Add git URL
    * Add the plugin package git URL to `Window` - `Package Manager` - `+` - `Add package from git URL`  
    * Plugin git URL: `https://github.com/styly-dev/STYLY-VisionOS-Plugin.git?path=Packages/com.styly.styly-vision-os-plugin#main`

<img width="552" alt="AddingGitURL" src="https://github.com/styly-dev/STYLY-VisionOS-Plugin/assets/387880/7df0413d-e91c-4210-98f4-eeb6055c303e">


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

Upload your built contents (.styly file) with its title, description and display type.
* Bounded: Your contents will be displayed in 1m x 1m x 1m size. Contents can be displayed with other apps.
* Unbounded: Your contents will be displayed exclusively in a space.

<img width="1033" alt="UploadContents" src="https://github.com/styly-dev/STYLY-VisionOS-Plugin/assets/387880/474b38a9-59c8-4e7d-9b5c-489ab2c59638">

### How to play the contents on STYLY

* Open content page on VisionOS simulator. You can copy and paste your URL of the content from your Mac to the simulator.
* Click `Play on device` on the page.
* Your content will be displayed.

