using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{

    public float thrust;
    public float knockTime;
    public float damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //***changed this around to not use tags and so many repeated getcomponent calls

        var player = other.GetComponent<Player>();
        var otherIsPlayer = player != null;
        var thisIsPlayer = this.GetComponentInParent<Player>() != null;

        var enemy = other.GetComponent<Enemy>();
        var otherIsEnemy = enemy != null;
        var thisIsEnemy = this.GetComponentInParent<Enemy>() != null;

        //for breakable objects later
        //var breakable = other.GetComponent<Breakable>();
        //var targetHasBreakable = breakable != null;

        //if (targetHasBreakable && thisIsPlayer)
        //{
        //    breakable.Smash();
        //}

        if (otherIsPlayer)
        {
            if (player.currentState == PlayerState.dead)
                return;
        }
        if (otherIsEnemy)
        {
            if (enemy.currentState == EnemyState.dead)
                return;

        }
        if (thisIsPlayer)
        {
            if (this.GetComponentInParent<Player>().currentState == PlayerState.dead)
                return;
        }
        if (thisIsEnemy)
        {
            if (this.GetComponentInParent<Enemy>().currentState == EnemyState.dead)
                return;
        }


        

        //player and enemy tirgger
        if ((otherIsPlayer && thisIsEnemy) || (thisIsPlayer && otherIsEnemy))
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if (hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);

                if (otherIsEnemy)// was also  && other.isTrigger but not needed because other will be collider of enemey
                {
                    if (enemy.currentState != EnemyState.stagger)//this means can't hit enemy again while they are staggered?
                    {
                        enemy.currentState = EnemyState.stagger;
                    }
                    enemy.Knock(hit, knockTime, damage);//so enemy can be hurt even if staggered
                }

                if (otherIsPlayer)
                {
                    if (player.currentState != PlayerState.stagger)
                    {
                        player.currentState = PlayerState.stagger;
                    }
                    player.Knock(knockTime, damage);//so player can be hurt if staggered
                }
            }
        }

        if (otherIsEnemy && thisIsEnemy)
        {
            Rigidbody2D otherRigidBody = other.GetComponent<Rigidbody2D>();
            if (otherRigidBody != null)
            {
                Vector2 difference = otherRigidBody.transform.position - transform.position;
                difference = difference.normalized;
                otherRigidBody.AddForce(difference, ForceMode2D.Force);


                //below for making lower enemy closer for topdown 2d
                //SpriteRenderer thisSprite = this.GetComponent<SpriteRenderer>();
                //SpriteRenderer otherSprite = other.GetComponent<SpriteRenderer>();


                //if (enemy.transform.position.y > this.transform.position.y)
                //{
                //    thisSprite.sortingOrder = (otherSprite.sortingOrder + 1);
                //}
                //else
                //{
                //    thisSprite.sortingOrder = (otherSprite.sortingOrder - 1);
                //}

                //above loop will get messed up if there is lots of overlapping and the orders end up all over the place?
            }
        }
    }


}
