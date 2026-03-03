using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   public float speed = 5;





    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }



    void movement()
    {
        var x = Input.GetAxis("Vertical") ;
        var z = Input.GetAxis("Horizontal");
        Vector3 move = transform.right * -x + transform.forward * z;
        transform.position += move * Time.deltaTime * speed;
    }
}
