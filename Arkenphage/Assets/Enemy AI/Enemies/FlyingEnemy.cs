using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : EnemyAI {

    bool following = false;
    public bool goLeft = true;
    public bool testCheck = false;
    public float speed = 5f;

    public float edge;

    Transform playerTarget;
    Rigidbody2D rb;
    float currHeight;

    // Use this for initialization
    void Start () {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        currHeight = transform.position.y;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        print(following);
        FlyOverPlayer();
        /*if (following == true)
        {
            FlyOverPlayer();
        }
        else
            LookForPlayer();*/
	}

    void LookForPlayer()
    {
        if (Physics2D.OverlapCircle(gameObject.transform.position, 10))
            following = true;
    }

    void FlyOverPlayer()
    {
        //use this code for simple back-and-forth stuff
        //rb.velocity = -transform.right * speed;

        //Vector3 leftSide = playerTarget.position + new Vector3(-2.5f, 3f, 0f);
        //Vector3 rightSide = playerTarget.position + new Vector3(2.5f, 3f, 0f);

        float leftSide = playerTarget.position.x - edge;
        float rightSide = playerTarget.position.x + edge;

        if (testCheck == true)
            AttackPlayer();

        rb.AddForce(-transform.right * speed);

        //if (Vector2.Distance(playerTarget.position, transform.position) < speed)
        if (Mathf.Abs(transform.position.x - playerTarget.position.x) < edge)
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);
        //else if (Vector2.Distance(playerTarget.position, transform.position) < (speed * 1.3f))
        else if (Mathf.Abs(transform.position.x - playerTarget.position.x) < (edge * 1.3f))
            rb.AddRelativeForce(-rb.velocity * 1.2f);

        if (transform.position.x < leftSide)
        {
            transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        }
        else if (transform.position.x > rightSide)
        {
            transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }

    }

    //add constant force on both directions

    //Notes:
    //--hovering 3 units above player
    //--hover in between -2.5 to 2.5
    //--hover in location 2s before attacking
    //--lock on  with .5s delay
    //--attack from -2.5 to player back to 2.5

    void AttackPlayer()
    {
        Vector2 tempPlayerLoc = playerTarget.transform.position;
    }

    /*void AttackPlayer()
    {
        Vector2 tempPlayerLoc = playerTarget.transform.position;

        //if (transform.position.y > currHeight)
        //{
        //    transform.position = new Vector2(transform.position.x, currHeight);
        //    testCheck = false;
        //}
        //else
        {
            float place = transform.position.x - tempPlayerLoc.x;
            float height = ((place * place) / currHeight) - Mathf.Abs(tempPlayerLoc.y);// - Mathf.Pow(playerTarget.position.x, 2f);

            transform.position = new Vector2(transform.position.x, height);
        }
    }*/
}
