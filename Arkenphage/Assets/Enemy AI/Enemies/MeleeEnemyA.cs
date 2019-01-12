using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyA : EnemyAI {

    public float speed; // get speed for both x- and y-directions

    Vector2 origPos;
    int dir = 1; // dir = 1 means enemy will go left, dir = -1 means right
    Transform playerTarget;
    Rigidbody2D rb;
    bool chasing = false;

    // Use this for initialization
    void Start () {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        origPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {

	}

    private void FixedUpdate()
    {
        if (chasing)
        {
            if (playerTarget.position.x > transform.position.x)
                rb.velocity = new Vector2(speed, rb.velocity.y);
            else if (playerTarget.position.x < transform.position.x)
                rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
    }

    IEnumerator StopChaseOnHit()
    {
        chasing = false;
        yield return new WaitForSeconds(1.5f);
        chasing = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().HitPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            chasing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            chasing = false;
        }
    }
}
