This folder has some samples for intracting with objects by hands.

# Object Interaction
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

# Object Interaction Advanced
Explanation to be added.