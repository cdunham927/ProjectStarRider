using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPickup : Pickup
{
    public float pickupDistance = 10f;
    float distance;

    Player_Stats stats;
    public int amountToTake;
    public float spd;
    bool dmg = false;
    public bool moves = true;
    //explosion effect
    public ParticleSystem BombBlast;
    //timer for bomb explosion
    public float timer;
    public Sprite bomb;
    public Sprite explo;

    //public AudioClip clip;

    private void OnEnable()
    {
        stats = FindObjectOfType<Player_Stats>();
        dmg = false;
        //xRot = Random.Range(0, rotSpd);
        //yRot = Random.Range(0, rotSpd);
        //zRot = Random.Range(0, rotSpd);
    }

    private void Update()
    {
        distance = Vector3.Distance(transform.position, stats.transform.position);

        //transform.rotation = Random.rotation;
        //transform.Rotate(xRot, yRot, zRot);

        if (distance <= pickupDistance && stats.Curr_hp > stats.Max_hp && moves)
        {
            transform.position = Vector3.MoveTowards(transform.position, stats.transform.position, spd * Time.deltaTime);
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
            if (this.gameObject.GetComponent<SpriteRenderer>().sprite == bomb)
            {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = explo;
            this.gameObject.tag = "explosion";
            timer = 0;
            return;
            }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && stats.Curr_hp > stats.Max_hp) GetPickup();
        Debug.Log("trigger");
        if (this.tag == "explosion")
        {
            Destroy(this);
        }
    }

    public override void GetPickup()
    {
        if (!dmg)
        {
            //MusicController.instance.src.PlayOneShot(clip);
            dmg = true;
            stats.Damage(amountToTake);
            Invoke("Disable", 0.001f);
        }
    }
    /*
  private void OnDisable()
  {
      CancelInvoke();
  }

  void Disable()
  {
      gameObject.SetActive(false);
  }*/
}
