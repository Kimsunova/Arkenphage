using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Homing : Bullet
{

    public float rotateSpeed;
    Transform playerTarget;

    protected override void SpecialStartFunction()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 dir = (Vector2)playerTarget.position - rb.position;
        dir.Normalize();
        float rotateAmount = Vector3.Cross(dir, transform.up).z;
        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.velocity = transform.up * speed;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //we'll add damage once we work health into the game
            collision.GetComponent<PlayerController>().HitPlayer();
        }
        //in the case of curving projectiles, we'll want to destroy them whenever they hit any surface
        Destroy(gameObject);
    }
}
