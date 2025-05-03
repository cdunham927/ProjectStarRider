using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringFollowAnimator : MonoBehaviour
{
    /// <summary>
    /// Animates a transform to rotate in the direction of the turning speed of a rigidbody.
    /// </summary>
 
    [Tooltip("The transform whose rotation is being animated.")]
    [SerializeField]
    protected Transform animatedTransform;

    [Tooltip("The rigidbody to get turn speed data from.")]
    [SerializeField]
    protected Rigidbody m_Rigidbody;

    [Tooltip("The coefficient of rotation for the animated transform as a function of the rigidbody turning speed.")]
    [SerializeField]
    protected float strength = 0.1f;



     protected virtual void Reset()
     {
            animatedTransform = transform;
            m_Rigidbody = GetComponentInChildren<Rigidbody>();
     }
   

    protected virtual void FixedUpdate()
    {
            Vector3 localAngularVelocity = m_Rigidbody.transform.InverseTransformDirection(m_Rigidbody.angularVelocity);

            float nextStrength = strength;

            Vector3 targetPos = Vector3.forward + nextStrength * new Vector3(localAngularVelocity.y, -localAngularVelocity.x, 0);

            animatedTransform.localRotation = Quaternion.LookRotation(targetPos);
    }
}

