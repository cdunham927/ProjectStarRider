using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Fire : MonoBehaviour
{
    [Header(" Projectile Settings : ")]
    //bullet 
    public GameObject Player_Bullet;
    
    //bullet  physics and force
    public float ShootForce, UpwardForce;

    //gun stats timbetween shots  , spray etc.
    public float FireRate;
    public bool AllowButtonHold;
    bool isShooting;

    [Header(" Camera References : ")]
    public Camera fpscamera;
    public Transform AttackPoint;




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
        if (AllowButtonHold) isShooting = Input.GetKey(KeyCode.E);
    
    }

    private void Shoot() 
    { 
    
    
    }
}
