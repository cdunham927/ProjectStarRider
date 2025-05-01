using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControlAnimator : PlayerControlAnimations
{
    /// <summary>
    /// Animates the vehicle relative to its own root transform, according to the velocity and angular velocity of the rigidbody, to give an extra feel of control to the player.
    /// Reffrom palyer Control animations script
    /// </summary>
    /// 

    [Tooltip("The rigidbody that provides the velocity values for animating the vehicle.")]
    [SerializeField]
    protected Rigidbody m_Rigidbody;


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

    protected virtual void Reset()
    {
        m_Rigidbody = transform.root.GetComponentInChildren<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
