This sample demonstrates how to snap a puzzle piece to a puzzle board.
Under the "Puzzle" GameObject, we have the pieces and the target transfroms.
The pieces will be snap to the target transforms when you grab them and release them near the target transforms.
The pieces have "XR Grab Interactable" component. The target transforms have "XR Socket Interactor" component.
The "XR Grab Interactable" component setting is not special. Please check the parameters of the "XR Socket Interactor" component setting. The transforms must have colliders. When the pieces are near the target transforms, the "XR Socket Interactor" component will snap them together.