using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBetweenWayPointsEnemy : MonoBehaviour
{

    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D enemyRigidBody;
    [SerializeField] Transform wayPoint1;
    [SerializeField] Transform wayPoint2;
    Transform currentTarget;

    // Use this for initialization
    void Start()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();
        this.transform.position = wayPoint1.position;
        currentTarget = wayPoint2;
    }

    // Update is called once per frame
    void Update()
    {
        FlipSprite();
        Move();
    }

    private void Move()
    {
        var distanceToTarget = Vector3.Distance(this.transform.position, currentTarget.position);
        if (distanceToTarget < .2)
        {
            currentTarget = (currentTarget.position == wayPoint1.position) ? wayPoint2 : wayPoint1;
        }

        Vector3 temp = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);
        enemyRigidBody.MovePosition(temp);

    }


    private void FlipSprite()
    {
        var num = currentTarget.position == wayPoint1.position ? 1 : -1;
        transform.localScale = new Vector2(Mathf.Sign(num), 1f);
    }
}
