using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShiptControlAnimator : ShipControlAnimation
{

    [Tooltip("The rigidbody that provides the velocity values for animating the vehicle.")]
    [SerializeField]
    protected Rigidbody m_Rigidbody;

    [Tooltip("How fast the animation occurs as a response to a change in velocity or angular velocity of the rigidbody.")]
    [SerializeField]
    protected float animationLerpSpeed = 0.5f;
    
    [SerializeField]
    protected bool applyAnimations = true;

    [Header("Roll")]

    [Tooltip("How much the vehicle rolls while turning left or right.")]
    [SerializeField]
    protected float sideRotationToRoll = 20;

    [Tooltip("How much the vehicle rolls while strafing left or right.")]
    [SerializeField]
    protected float sideMovementToRoll = -0.15f;

    [Header("Turn Following")]

    [Tooltip("How much the vehicle rotates up or down when pitching (rotating up or down).")]
    [SerializeField]
    protected float verticalTurnFollowing = 8;

    [Tooltip("How much the vehicle rotates left or right when yawing (rotating sideways).")]
    [SerializeField]
    protected float sideTurnFollowing = 5;

    [Tooltip("Whether this component is active or not.")]
    [SerializeField]
    protected bool activated = true;

    public UnityEvent onUpdated;

    protected Quaternion animationRotation = Quaternion.identity;

    protected virtual void Reset()
    {
        m_Rigidbody = transform.root.GetComponentInChildren<Rigidbody>();
    }


   

    /// <summary>
    /// Remove all animated rotation.
    /// </summary>
    public virtual void ClearAnimation()
    {
        transform.localRotation = Quaternion.identity;
    }


    public override Quaternion GetRotation()
    {
        return animationRotation;
    }

    protected virtual void FixedUpdate()
    {
        if (activated)
        {
            Vector3 localAngularVelocity = m_Rigidbody.transform.InverseTransformDirection(m_Rigidbody.angularVelocity);

            Vector3 localVelocity = m_Rigidbody.transform.InverseTransformDirection(m_Rigidbody.velocity);

            float targetRoll = sideRotationToRoll * -localAngularVelocity.y + sideMovementToRoll * localVelocity.x;

            float targetPitch = verticalTurnFollowing * localAngularVelocity.x;
            float targetYaw = sideTurnFollowing * localAngularVelocity.y;

            animationRotation = Quaternion.Euler(targetPitch, targetYaw, targetRoll);

            if (applyAnimations)
            {
                // Yaw to roll
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(targetPitch, targetYaw, targetRoll), animationLerpSpeed);

            }

            onUpdated.Invoke();

        }
        else
        {
            animationRotation = Quaternion.identity;
        }
    }
}
