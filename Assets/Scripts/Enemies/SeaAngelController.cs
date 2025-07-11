using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaAngelController : EnemyControllerBase 
{
    [Header("Private Variables: ")]
    //private Vector3 startPoint;
    //private const float radius = 1f;
    public GameObject seaAngelMesh;

    [Header("Audio Clips: ")]
    public AudioClip ShotSounds;
    private AudioSource AS;

    protected override void Awake()
    {
        AS = GetComponent<AudioSource>();
    }

    protected override void OnEnable()
    {
        seaAngelMesh.SetActive(false);
        seaAngelMesh.SetActive(true);
        base.OnEnable();
    }

    protected override void Alert()
    {
        //If the cooldown is at 0 then we can attack
        if (attackCools <= 0) Attack();
    }

    GameObject bul;

    protected override void Attack()
    {
        PlaySound();
        //if (bulletPool == null) bulletPool = cont.seaAngelBulPool;

        float angleStep = 360f / bulletShot;
        //float angle = 0f;
        
        //Get pooled bullet
        for (int i = 0; i < bulletShot; i++)
        {
            //if (bulletPool == null) 
                bul = cont.seaAngelBulPool.GetPooledObject();
            /*if (bul != null) //base bullet controller
            {
                //Put it where the enemy position is
                bul.transform.position = transform.position;
                //Aim it at the player
                //bul.transform.rotation = transform.rotation;
                //Activate it at the enemy position
                bul.SetActive(true);
                bul.transform.LookAt(player.transform);
                bul.transform.Rotate(Random.Range(-accx, accx), Random.Range(-accy, accy), 0);
                if (isRandom == true)
                {
                    bul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                }
                bul.GetComponent<EnemyBullet>().Push();
            }*/

            if( bul != null) 
            {
                bul.transform.position = transform.position;
                bul.SetActive(true);
                bul.transform.LookAt(player.transform);
                bul.transform.Rotate(Random.Range(-accx, accx), Random.Range(-accy, accy), 0);
                if (isRandom == true)
                {
                    bul.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                }
                
                bul.GetComponent<EnemyBullet>().Push();
            } 
        }
        //Reset attack cooldown
        attackCools = timeBetweenAttacks;

        ChangeState(enemystates.alert);
    }

    protected override void Idle()
    {
        //If the player is close enough
        if (playerInRange && player != null)
        {
            ChangeState(enemystates.alert);
        }
    }

    protected override void Patrol()
    {

    }

    protected override void Retreat()
    {

    }

    protected override void Update()
    {
        
        //If the player is close enough
        if (playerInRange && player != null)
        {
            ChangeState(enemystates.alert);
        }
        else ChangeState(enemystates.idle);

        //If the cooldown is greater than 0 we decrement it every frame
        if (attackCools > 0) attackCools -= Time.deltaTime;

        base.Update();
    }

    public void PlaySound()
    {
        AS.PlayOneShot(ShotSounds);
    }

}
