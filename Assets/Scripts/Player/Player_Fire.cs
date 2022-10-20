using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Fire : MonoBehaviour
{
    [Header(" Projectile Settings : ")]
    //bullet 
    public GameObject Player_Bullet;
    //bullet  physics and force
    public float ShootForce; 
     //gun stats timbetween shots  , spray etc.
    public float FireRate = .15f;
    public float WeaponRange = 60f;
    public bool isShooting = false;
   
    // upward force for future use may delete
    // public float UpwardForce; 

    [Header(" Camera References : ")]
    public Camera fpscamera;
    public Transform attackPoint;

    [Header(" Sound Effect : ")]
    public AudioSource gunAudio;

    [Header(" Graphics : ")]
    public GameObject muzzleFlash;
    public GameObject Hitvfx;

    private WaitForSeconds shotDuration = new WaitForSeconds(0.7f);
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
    }

    private void MyInput() 
    {
        // holding the key will fire bullets
        if (Input.GetKeyDown(KeyCode.E))
        {
            isShooting = true;
            if (isShooting == true)
            { 
                Shoot(); 
            }
            
        }
    
    }

    private void Shoot() 
    {

        //Find the exact hit position raycast . Creates avector at the center of the camera view port
        Ray ray = fpscamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // a ray throught the middle of your sreen

        // declare a raycast hit , used to stor infor on what the raycast hit
        RaycastHit hit;

        // checks if the raycast is hitting something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(7);

        //Calculate  distance from shoting point to target point : Direction of A-> B = b.positon - A.position
        Vector3 direction = targetPoint - attackPoint.position;

        //Instatiante projectile/ bullets  : stores spawned bullet
        GameObject currentBullet = Instantiate(Player_Bullet, attackPoint.position, Quaternion.identity);

        //Rotate bullet to direction
        currentBullet.transform.forward = direction.normalized;

        //Bullet force calcuclations
        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * ShootForce, ForceMode.Impulse);
        
        //Instantiate muzzle flash
        if( muzzleFlash != null )
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

    }
}
