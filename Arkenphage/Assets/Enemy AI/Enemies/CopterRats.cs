using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopterRats : MonoBehaviour {

    public float speed;

    float timer = 1f;
    bool moving = false;
    Transform playerTarget;
    Vector2 tempTarget = Vector2.zero;

    // Use this for initialization
    void Start () {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if (moving == false)
            {
                moving = true;
                timer = 1f;
                tempTarget = playerTarget.position;
            }
            else
            {
                moving = false;
                timer = 1f;
            }
        }
        else if (moving == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, tempTarget, speed * Time.deltaTime);
        }
    }
}
