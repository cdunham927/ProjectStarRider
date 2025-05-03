using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{

    public bool applyLocal = true;

    //[SerializeField]
    //protected List<CameraView> cameraViews = new List<CameraView>();

    public bool clearAnimationsOnViewChange = true;

    public Transform vehicleHandle;

    public List<PlayerControlAnimations> animations = new List<PlayerControlAnimations>();

    //protected CameraEntity cameraEntity;

    bool activated = false;
    private void FixedUpdate()
    {
        if (!activated) return;

        Quaternion rotation = Quaternion.identity;
        Vector3 position = Vector3.zero;

        for (int i = 0; i < animations.Count; ++i)
        {
            rotation = animations[i].GetRotation() * rotation;
            position += animations[i].GetPosition();
        }

        Apply(position, rotation);

    }

    protected virtual void Apply(Vector3 position, Quaternion rotation)
    {
        if (applyLocal)
        {
            vehicleHandle.localPosition = position;
            vehicleHandle.localRotation = rotation;
        }
        else
        {
            vehicleHandle.position = position;
            vehicleHandle.rotation = rotation;
        }
    }
}
