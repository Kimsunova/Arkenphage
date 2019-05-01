using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Normal : Bullet {

    private int direction = -1;

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = direction * transform.right * speed;
    }

    public void Parried()
    {
        direction *= -1;
    }
}
