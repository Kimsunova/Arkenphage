using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Normal : Bullet {

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = direction * transform.right * speed;
    }

}
