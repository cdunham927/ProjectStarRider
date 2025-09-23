1.0.0
Note: Updating from the preview version is not supported, delete it before importing.
All component settings and configurations will be lost due to data structure changes.

Added:
- Masking functionality, other splines with a SplineSpawnerMask can define masking layers. Spline Spawners can filter by them.
- UI, drag & drop box for adding new prefabs
- Snap to Colliders modifier, added "Direction" option. Making it possible to snap down from the Spline curve.
- Height Filter modifier, restricts spawning within the configured height range.
- On Curve distribution, spacing can now be set between a minimum and maximum value.
- Scale, functionality to scale over a curve
- On Knot distribution, options to select which knots to spawn on (eg. first & last, specific range, etc).

Changed:
- Inspector UI has been overhauled and polished
- Distribution settings seed can now be randomized
- Distribution modes now have performance/accuracy preference options
- Prefabs array is now a List, for easier insertion and deletion of objects
- Performance of spawning inside a Spline's area has improved 400-1800%

Fixed:
- Duplicating a Spline Spawner component would inadvertently link the original and copy's modifier stack.
- Modifier stack not being saved to prefabs or supporting prefab overrides.
- Modifier stack not supporting undo/redo operations

== Preview version ==
1.0.2

Added:
- Scale: option to invert distance-based scaling

1.0.1

Changed:
- Randomization can now also be applied using alternating values
- Rotation modifier now has a min/max rotation field

1.0.0
Initial testing release