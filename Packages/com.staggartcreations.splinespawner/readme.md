# Spline Spawner
*by Staggart Creations*

## Project Requirements
- Unity 2022.3.23f1 or newer (older versions contain a bug related to prefab variants)

*Unity 2021.3 is not compatible, since it does not support the Collections 2.5.1+ package*

**Requires the following packages**
- Splines 2.8.1+
- Mathematics 1.2.6+
- Burst 1.8+
- Collections 2.5.1+

## New editor options
- Spline Spawner component
- Context menu on Mesh Filter component to spawn it on a spline
- Context menu on Spline Container component to add a Spline Spawner
- New menu option: GameObject/3D Object/Spline Spawner

## Runtime usage

There is no editor-only code in place, so runtime usage is possible. That's not to say that a complete spline UI and control system is in place.
You can call the Respawn() function on a Spliner Spawner component from any external script.