# STYLY Spatial Layer Plugin for Vision Pro

![Kujira](https://github.com/styly-dev/STYLY-visionOS-Plugin/assets/387880/f8c5a959-ef49-4ed3-b06b-db367624766f)

### What is this plugin?
* You can build your Unity contents for uplaoding to [STYLY for Vision Pro](https://spatial-layer.styly.cc/) and enjoy it on [our app](https://apps.apple.com/us/app/styly-for-vision-pro/id6475184828).
* This plugin also works a template including [content samples](https://github.com/styly-dev/STYLY-Spatial-Layer-Plugin/wiki/Content-samples) for creations. [Downalod Unity Template Project](https://api.github.com/repos/styly-dev/STYLY-visionOS-Plugin/zipball/) 

### Notes
* Contents of STYLY for Vision Pro is not compatible with [STYLY for other platforms (STYLY VR or STYLY mobile)](https://gallery.styly.cc/). 
* STYLY for Vision Pro doesn't have 3D contents assembly tools for now.
* Please give us your feedback on [Github Issues](https://github.com/styly-dev/STYLY-Spatial-Layer-Plugin/issues).  
* Your contents will be PUBLIC once your uploaded. Don't upload your contents which you want to keep private. 
* The content gallery on the STYLY app is now managed on a whitelist basis. Please apply through [this APPICATION FORM](https://docs.google.com/forms/d/e/1FAIpQLSd6eK_mo6ECXy-fLZP53xkz9e6t-B_au_PSCE0eafIB0u86GQ/viewform) if you want your content to get listed.

#### Requirements

* Unity
  * Unity 6000.0.59f2 or later (Use 6000.0.XX)
  * Modules:
    * visionOS Build Support
  * Render pipelines: URP
  * Color Space: Linear
* Xcode (if you want to test with visionOS simulator)
  * Xcode 16.3 or later
  * Apple Silicon Mac (M1/M2/M3/M4)

#### Links
* [STYLY app for visionOS Simulator download](https://drive.google.com/uc?export=download&id=1GR4Xw14_gMSG_fW7dyXPoTyFde4D6Vwz)
* [Website of STYLY for Vision Pro](https://spatial-layer.styly.cc/)
* [visionOS app release note](https://github.com/styly-dev/STYLY-Spatial-Layer-Plugin/wiki/STYLY-for-Vision-Pro-Release-Notes)
* [Github repository of this plugin](https://github.com/styly-dev/STYLY-Spatial-Layer-Plugin/)
* [OpenUPM page](https://openupm.com/packages/com.styly.styly-spatial-layer-plugin/)

#### Supported features for STYLY visionOS Plugin

* Mixed Reality mode: Bounded Volumes, Unbounded Volumes
* Standard URP materials: Lit, Simple Lit, Unlit (and some spacial materials)
* Visual Scripting
* Timeline, animator, animation, audioclip and other basic features. See [Supported Unity Features and Components](https://docs.unity3d.com/Packages/com.unity.polyspatial.visionos@2.2/manual/SupportedFeatures.html) page for the detail.

#### **NOT** Supported features for STYLY visionOS Plugin

* Fully Immersive VR, Windowed Apps
* Scripting in C#
* Custom shaders / Shader Graph
* Visual Effect Graph

#### Supported Visual Scripting Unity packages

* [VRM Visual Scripting Nodes](https://openupm.com/packages/com.from2001.vrm-visualscripting-nodes/)
* [glTFast Visual Scripting Nodes](https://openupm.com/packages/com.from2001.gltfast-visualscripting-nodes/)
* [Spectrum Visual Scripting Nodes](https://openupm.com/packages/com.from2001.spectrum-visualscripting-nodes/)
* [WebRequest Visual Scripting Nodes](https://openupm.com/packages/com.styly.webrequest-visualscripting-nodes/)
* [STYLY-XR-Rig](https://openupm.com/packages/com.styly.styly-xr-rig/)

You are moe than welcome to recommend new custom Visual Scripting nodes to support.

#### How to setup

##### Unity
* Install Unity 6000.0.59f2 or later (Use 6000.0.XX) via [Unity Hub](https://unity.com/unity-hub) with modules:
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
* Install Xcode 16.3 or later with visionOS simulator. [Xcodes](https://www.xcodes.app/) is the easiest way to install and manage multiple versions of Xcode.
* Open Simulator from Xcode menu. Click`Xcode` -`Open Developer Tool` -`Simulator`
* Open Vision Pro simulator from menu. Click`File` -`Open Simulator` -`visionOS 2.0` -`Apple Vision Pro`

##### Install STYLY for Vision Pro into the visionOS simulator
* Install STYLY for visionOS into the simulator
  * Download [STYLY-Vision-OS-App.app](https://drive.google.com/uc?export=download&id=1GR4Xw14_gMSG_fW7dyXPoTyFde4D6Vwz) and drag the app file and drop to the simulator window.
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

![upload](https://github.com/user-attachments/assets/409abb21-b1f5-49a4-9a7a-c17f2e3bcd5d)

### How to play the contents on STYLY

* Open content page on visionOS simulator. You can copy and paste your URL of the content from your Mac to the simulator.
* Click `Play on device` on the page.
* Your content will be displayed.

### STYLY-XR-Rig (Add-on)

Right click at hierarchy window to install STYLY-XR-Rig, which allows you to develop and test advanced features.
<img width="898" alt="Install STYLY-XR-Rig" src="https://github.com/styly-dev/STYLY-Spatial-Layer-Plugin/assets/387880/4d9e6ec2-da84-4dee-a9c9-4b062a46ba1f">


