using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    
  
    public float Zspeed = 100f;
    public float Xspeed = 100f;
    public float Yspeed = 100f;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(Xspeed, Yspeed, Zspeed) * Time.deltaTime);
    }
}
