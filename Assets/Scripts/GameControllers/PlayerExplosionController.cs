using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosionController : MonoBehaviour
{
    public int damage = 2;
    public bool hasDamaged = false;
    public float disableTime;
    public AnimationClip explosionClip;
    public AudioClip explodeSound;
    public float vol;

    public LayerMask enemyMask;
    public float castSize;

    private void OnEnable()
    {
        hasDamaged = false;
        if (explosionClip != null) disableTime = explosionClip.length;
        Invoke("Disable", disableTime);
    }

    private void Update()
    {
        Collider[] hitColliders;
        hitColliders = Physics.OverlapSphere(transform.position, castSize, enemyMask);
        foreach (Collider hitcol in hitColliders)
        {
            hitcol.GetComponent<EnemyControllerBase>().Damage(damage);
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    void Disable()
    {
        if (MusicController.instance != null) MusicController.instance.audioSourceArray[6].PlayOneShot(explodeSound, vol);
        gameObject.SetActive(false);
    }
}
