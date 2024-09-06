This sample demonstrates how to implement hand gesture reactive behaviours. `ThumbUp` and `ThumbDown` GameObjects have `Gesture Tracker` script. `Hand Gesture` GameObject has `XR Hand Tracking Events` script. The `XR Hand Tracking Events` is injected to `Gesture Tracker` script. `Gesture Tracker` accepts a Hand Shape or Hand Pose setting file which can be the gesture detected by the script. You can use Hand Shape and Hand Pose files in `Reusable Assets/Hand Gestures` folder for your cases.

Also, please check the spec of Hand Shape and Hand Pose.

Hand Shape
https://docs.unity3d.com/Packages/com.unity.xr.hands@1.4/manual/gestures/hand-shapes.html

Hand Pose
https://docs.unity3d.com/Packages/com.unity.xr.hands@1.4/manual/gestures/hand-poses.html