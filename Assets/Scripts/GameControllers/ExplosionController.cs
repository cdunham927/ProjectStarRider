using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public int damage = 2;
    public bool hasDamaged = false;
    public float disableTime;
    public AnimationClip explosionClip;
    public AudioClip explodeSound;
    public float vol;

    private void OnEnable()
    {
        hasDamaged = false;
        if (explosionClip != null) disableTime = explosionClip.length;
        Invoke("Disable", disableTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasDamaged)
        {
            other.GetComponent<Player_Stats>().Damage(damage);
            hasDamaged = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !hasDamaged)
        {
            other.GetComponent<Player_Stats>().Damage(damage);
            hasDamaged = true;
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    void Disable()
    {
        if (MusicController.instance != null) MusicController.instance.audioSourceArray[1].PlayOneShot(explodeSound, vol);
        gameObject.SetActive(false);
    }
}
