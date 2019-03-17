using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedusaHead : EnemyAI {
    public Vector2 speeds; // get speed for both x- and y-directions
    public float amplitude; // get the amplitude (the height of the wave)

    public bool hover; // check this to make the enemy go back & forth instead of flying in only 1 direction
    public float leftSide, rightSide; // when hovering, the enemy will turn around at the "walls" (x-values)

    int dir = 1; // dir = 1 means enemy will go left, dir = -1 means right
    float startPosition; // the starting position of the enemy

	// Use this for initialization
	void Start () {
        startPosition = transform.position.y;
	}

    // Update is called once per frame
    void Update()
    {
        //moves the enemy in the wavy pattern
        transform.position = new Vector2(transform.position.x + (speeds.x / 10) * dir,
            Mathf.Sin((Time.timeSinceLevelLoad * speeds.y) / 5) * amplitude + startPosition);

        //this is used if the enemy hovers. If it's x-value is beyond the "walls", it will turn around and reverse its direction
        if (hover == true)
        {      
            if (transform.position.x < leftSide)
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
    }
}
