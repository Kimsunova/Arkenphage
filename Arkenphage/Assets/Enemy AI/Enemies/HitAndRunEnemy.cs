using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAndRunEnemy : FireLoop {
    public float speed; // get speed for both x- and y-directions

    Renderer render;
    BoxCollider2D shootingRange;
    Rigidbody2D rb;
    Transform playerTarget;
    bool running = false;

	// Use this for initialization
	void Start () {
        timeReset = timeBetweenShots;

        render = GetComponent<Renderer>();
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        shootingRange = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        shoot = false;
    }

    private void FixedUpdate()
    {
        if (render.isVisible && running == false)
            shoot = true;
        else
            shoot = false;

        if (running)
        {
            if (playerTarget.position.x < transform.position.x)
                rb.velocity = new Vector2(speed, rb.velocity.y);
            else if (playerTarget.position.x > transform.position.x)
                rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            running = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            running = false;
            timeBetweenShots = timeReset;
        }
    }
}
