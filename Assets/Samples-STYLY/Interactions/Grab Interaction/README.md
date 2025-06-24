This folder has some samples for intracting by grab by hands.

# Grab Interaction
This includes following objects:

- Grabbable cube which feels gravity.
- Grabbable cube which does not feel gravity.
- Grabbable and scalable cube.
- Reactive cube for grabbing.

All objects can be grabbed by hands directly or by gazing even they are far away.

## Making objects grabbable
1. Add a collider to the object.
2. Attach "XR Grab Interactable" script.

## Making objects scalable
In addition to the step of making objects grabbable, please attach "XR General Grab Transformer" script.
In the setting of "XR General Grab Transformer", please allow scaling. (e.g. Enable "Allow Two Handed Scaling")

## Listening to grab events
1. Attach VisualScripting graph component and set Unity Event listeners.
2. Add Unity Event listener to the object. [XR Grab Interactable] (script component) -> [Interactable Events] -> [Select] -> [Select Entered] and [Select Grabbed].

For details, please check compoents attached to "ReactiveForGrab" object.

# Grab Interaction Hover Effect (Unity Pro required)
This is the same as Grab Interaction but with hover effect. The hover effect in visionOS is expressed by rim effect in white color. To realize hover effect, please add "Vision OS Hover Effect" component to your 3D objects. You should import STYLY-XR-Rig package to your project to use hover effect. To do this, please right click in Object Hierarchy and select "XR" -> "Install STYLY-XR-Rig". Then, you can add "VisionOS Hover Effect" component to your 3D objects.
