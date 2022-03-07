using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerBase : MonoBehaviour
{
    //We only need idle, alert, attack for now
    public enum enemystates { idle, patrol, alert, attack, retreat, death };
    public enemystates currentState;
    //Stats
    public float maxHp;
    protected float curHp;

    //Time between attacks
    public float timeBetweenAttacks;
    //Range for when the enemy starts attacking
    public float attackRange;
    //Current cooldown time for attacking
    protected float attackCools;
    //Object pool for bullets to shoot
    public ObjectPool bulletPool;
    //Checks if the player is in range
    public bool playerInRange = false;
    //Set the radius for the detection collider
    public SphereCollider detectionCollider;
    public Collider col;
    public bool isRandom;
    public float accx;
    public float accy;

    //Player
    protected PlayerController player;
    protected GameManager cont;
    protected AudioSource src;

    private void Awake()
    {
        src = GetComponent<AudioSource>();
        cont = FindObjectOfType<GameManager>();
        bulletPool = cont.enemyBulPool;
    }

    protected virtual void OnEnable()
    {
        player = FindObjectOfType<PlayerController>();
        detectionCollider.radius = attackRange;
        SetCollider(false);
        ChangeState(enemystates.idle);
        curHp = maxHp;
    }

    protected virtual void Idle() { }
    protected virtual void Patrol() { }
    protected virtual void Alert() { }
    protected virtual void Attack() { }
    protected virtual void Retreat() { }
    protected virtual void Death() { }

    public void ChangeState(enemystates toState)
    {
        currentState = toState;
    }

    public void Damage(float amt = 1)
    {
        curHp -= amt;
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case (enemystates.idle):
                Idle();
                break;
            case (enemystates.patrol):
                Patrol();
                break;
            case (enemystates.alert):
                Alert();
                break;
            case (enemystates.attack):
                Attack();
                break;
            case (enemystates.retreat):
                Retreat();
                break;
            case (enemystates.death):
                Death();
                break;
        }
    }

    public void SetCollider(bool cl = true)
    {
        col.enabled = cl;
    }
}
