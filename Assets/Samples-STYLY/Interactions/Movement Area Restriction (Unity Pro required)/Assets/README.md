This sample demonstrates how to restrict area of movement in 1D, 2D or 3D space.
This system consists of two types of GameObjects `Input Manger` and target objects to be moved.
`Input Manager` has `Input Manager` VisualScripting graph and target objects have `Restrict Movement Area` graph.
To apply the restriction to your objects, please just include `Input Manager` (Prefab) in your Prefab and set `Restrict Movement Area` graph to your objects.

`Movement Area Restriction` graph has a parameter called `Area` (Vectror3) as a VisualScripting Object Variable. You can specify the area of movement in 1D, 2D or 3D space.
The value of elements of the vector determines the range of movement in each axis. The range of movement is from the negative value to the positive value of the absolute value you set. Please set 0 for axis you want to restrict.
To restrict movement in X axis in the range [-1, 1], set (1, 0, 0).

Please note that this sample requires Unity Pro license and please add `STYLY-XR-Rig` package to your project.