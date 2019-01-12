using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAndSwoopEnemy : MonoBehaviour {

    public float speed; // get speed for both x- and y-directions
    public float edge; // when hovering, the enemy will turn around at the "walls" (x-values)

    int dir = 1; // dir = 1 means enemy will go left, dir = -1 means right
    float startPosition; // the starting position of the enemy
    bool flying = true;
    bool attack = false, swoop = false;
    float timer = 5f;

    public enum AI { Fly, Swoop, Return };
    public AI ai;

    Transform playerTarget;
    Transform cameraTarget;
    Rigidbody2D rb;
    Vector2 tempTarget;
    float currHeight;

    // Use this for initialization
    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        cameraTarget = GameObject.FindGameObjectWithTag("MainCamera").transform;
        rb = GetComponent<Rigidbody2D>();
        currHeight = transform.position.y;

        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height * .8f, 10));
        transform.position = new Vector2(transform.position.x, pos.y);
        edge /= 200f;
    }

    // Update is called once per frame
    void Update () {
        timer -= Time.deltaTime;
        switch (ai)
        {
            case AI.Fly:
                Fly();
                break;

            case AI.Swoop:
                Swoop();
                break;

            case AI.Return:
                Return();
                break;
        }
	}

    void Fly()
    {
        float leftSide = Camera.main.ViewportToWorldPoint(new Vector3(.5f - edge, 0, 0)).x;//playerTarget.position.x - edge;
        float rightSide = Camera.main.ViewportToWorldPoint(new Vector3(.5f + edge, 0, 0)).x;//Screen.width * .6f;//playerTarget.position.x + edge;

        transform.position = new Vector2(transform.position.x + (speed * dir / 50), transform.position.y);
        if (timer <= 0)
        {
            tempTarget = playerTarget.position;
            ai = AI.Swoop;
        }
        else if (transform.position.x < leftSide)
        {
            dir = 1;
            transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        }
        else if (transform.position.x > rightSide)
        {
            dir = -1;
            transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }
    }

    void Swoop()
    {
        transform.position = Vector2.MoveTowards(transform.position, tempTarget, (speed) * Time.deltaTime);
        if(Vector2.Distance(transform.position, tempTarget) < .5f)
        {
            ai = AI.Return;
        }
    }

    void Return()
    {
        tempTarget = Camera.main.ViewportToWorldPoint(new Vector3(.5f + edge, .8f, 0));
        transform.position = Vector2.MoveTowards(transform.position, tempTarget, (speed) * Time.deltaTime);
        if (Vector2.Distance(transform.position, tempTarget) < .5f)
        {
            ai = AI.Fly;
            timer = 5f;
        }
    }

}
