# STYLY Spatial Layer Plugin for Vision Pro

![Kujira](https://github.com/styly-dev/STYLY-visionOS-Plugin/assets/387880/f8c5a959-ef49-4ed3-b06b-db367624766f)

### What is this plugin?
* You can upload your Unity contents to [STYLY for Vision Pro](https://apps.apple.com/us/app/styly-for-vision-pro/id6475184828).

### Notes
* Contents of STYLY for Vision Pro is not compatible with STYLY for other platforms. Its code base is different from STYLY VR or STYLY mobile. 
* STYLY for Vision Pro doesn't have 3D contents creation tools for now.
* Please give us your feedback on [Github Issues](https://github.com/styly-dev/STYLY-Spatial-Layer-Plugin/issues).  
* Your contents will be published to the PUBLIC. Don't upload your contents which you want make private. 
* Contents gallery on STYLY app is white list based now. STYLY team may pick up your contents display in the app.    

#### Requirements

* Unity
  * Unity 2022.3.27 or later (Use 2022.3.XX)
  * Modules:
    * visionOS Build Support
  * Render pipelines: URP
  * Color Space: Linear
* Xcode (if you want to test with visionOS simulator)
  * Xcode 15.2 or later
  * Apple Silicon Mac (M1/M2/M3)

#### Links
* [STYLY app for Vision OS Simulator download](https://drive.google.com/uc?export=download&id=1GR4Xw14_gMSG_fW7dyXPoTyFde4D6Vwz)
* [Website of STYLY for Vision Pro](https://spatial-layer.styly.cc/)
* [Github repository](https://github.com/styly-dev/STYLY-Spatial-Layer-Plugin/)
* [OpenUPM page](https://openupm.com/packages/com.styly.styly-spatial-layer-plugin/)

#### Supported features for STYLY visionOS Plugin

* Mixed Reality mode: Bounded Volumes, Unbounded Volumes
* Standard URP shaders: Lit, Simple Lit, Unlit and some shaders in supported Unity packages
* Visual Scripting
* Timeline, animator, animation, audioclip and other basic features. See [Supported Unity Features and Components](https://docs.unity3d.com/Packages/com.unity.polyspatial.visionos@1.2/manual/SupportedFeatures.html) page for the detail.

#### **NOT** Supported features for STYLY visionOS Plugin

* Fully Immersive VR, Windowed Apps
* Your C# scripts
* Custom shaders
* Visual Effect Graph
* Postprocessing Stack

#### Supported Visual Scripting Unity packages

* [VRM Visual Scripting Nodes](https://openupm.com/packages/com.from2001.vrm-visualscripting-nodes/)
* [glTFast Visual Scripting Nodes](https://openupm.com/packages/com.from2001.gltfast-visualscripting-nodes/)
* [Spectrum Visual Scripting Nodes](https://openupm.com/packages/com.from2001.spectrum-visualscripting-nodes/)
* [WebRequest Visual Scripting Nodes](https://openupm.com/packages/com.styly.webrequest-visualscripting-nodes/)
* [STYLY-XR-Rig](https://openupm.com/packages/com.styly.styly-xr-rig/)

  You are moe than welcome to recommend new custom Visual Scripting nodes to support.

#### How to setup

##### Unity

  * Install Unity via [Unity Hub](https://unity.com/unity-hub)

    * Install Unity [2022.3.27](https://unity.com/ja/releases/editor/whats-new/2022.3.27) or later (Use 2022.3.XX) with modules:
      * visionOS Build Support

##### Setup Unity project

  * Option A (Easiest & recommended): Download [Unity Template Project](https://api.github.com/repos/styly-dev/STYLY-visionOS-Plugin/zipball/) including STYLY visionOS Plugin and supported packages

  * Option B (for existing project): Use [OpenUPM](https://openupm.com/packages/com.styly.styly-spatial-layer-plugin/) .
    * Prerequisites : [Node.js 14.18 or above](https://nodejs.org/en/download/)

```sh
    # Install openupm-cli
    npm install -g openupm-cli

    # Go to your unity project directory
    cd YOUR_UNITY_PROJECT_DIR

    # Install STYLY plugin for Unity
    openupm add -f com.styly.styly-spatial-layer-plugin
    # Install supported packages for STYLY
    openupm add -f com.styly.package-collection-spatial-layer
```

##### Xcode

  * Install Xcode 15.2 or later with visionOS simulator. [Xcodes](https://www.xcodes.app/) is the easiest way to install and manage multiple versions of Xcode.
  * Open Simulator from Xcode menu. Click`Xcode` -`Open Developer Tool` -`Simulator`
  * Open Vision Pro simulator from menu. Click`File` -`Open Simulator` -`visionOS 1.0` -`Apple Vision Pro`

##### Install STYLY for Vision Pro into the visionOS simulator
  * Install STYLY for visionOS into the simulator
    * Download [STYLY-Vision-OS-App.app](https://drive.google.com/uc?export=download&id=1GR4Xw14_gMSG_fW7dyXPoTyFde4D6Vwz) and drag the app file to the simulator window from Finder.
![Simulator](https://github.com/styly-dev/PolySpatial_VisualScriptingNodes/assets/387880/0e2da2ad-f45c-4452-b71e-9339ade58fd6)

### How to build and upload contents

Unity prefab can be built and uploaded to STYLY. Right click a prefab in a project window and select `STYLY` - `Build Content FIle`

![Right Click](https://github.com/styly-dev/PolySpatial_VisualScriptingNodes/assets/387880/bb5b85d5-4106-4561-aeca-cc8a5297c5cd)

Built file will be created as `yyyyMMddHHmmss.styly` in `_Output` directory in your project.
![Output file](https://github.com/styly-dev/PolySpatial_VisualScriptingNodes/assets/387880/555c708b-787c-48a6-ba93-13c168643f44)

STYLY for visionOS webpage will be opened in a browser. Login with your STYLY account.

Upload your built contents (.styly file) with its title, description and display type.
* Bounded: Your contents will be displayed in 1m x 1m x 1m size. Contents can be displayed with other apps.
* Unbounded: Your contents will be displayed exclusively in a space.

<img width="1033" alt="UploadContents" src="https://github.com/styly-dev/STYLY-VisionOS-Plugin/assets/387880/474b38a9-59c8-4e7d-9b5c-489ab2c59638">

### How to play the contents on STYLY

* Open content page on visionOS simulator. You can copy and paste your URL of the content from your Mac to the simulator.
* Click `Play on device` on the page.
* Your content will be displayed.

