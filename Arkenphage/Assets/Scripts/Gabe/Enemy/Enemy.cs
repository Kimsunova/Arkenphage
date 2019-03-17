using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    idle,
    walk,
    attack,
    stagger,
    dead
}

public class Enemy : MonoBehaviour
{

    public EnemyState currentState;
    public FloatValue maxHealth;
    public float health;
    public string enemyName;
    public int baseAttack;
    public float moveSpeed;
    public GameObject deathEffect;
    public float deathTime;
    [SerializeField] protected Animator myAnimator;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();//is this necessary? is it getting the same one that is gotten in radiusawakenedenemy start()?
    }

    private void Awake()
    {
        health = maxHealth.initialValue;
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            currentState = EnemyState.dead;
            //moveSpeed = .0001f;
            DeathEffect();
            //this.gameObject.SetActive(false);
        }
    }

    private void DeathEffect()
    {
        myAnimator.SetBool("IsDead", true);//this is not always working
        
        Destroy(this.gameObject, 4f);//this isntead of above disable?
        //yield return new WaitForSeconds(0);//should it wait at all? is this just making sure it does stagger first and then death animation?
        //this.gameObject.SetActive(false, 1f);
        //uncomment below for adding a death particle effect if desired
        //if (deathEffect != null)
        //{
        //    GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        //    Destroy(effect, 1f);
        //}
    }

    public void Knock(Rigidbody2D myRigidbody, float knockTime, float damage)
    {
        TakeDamage(damage);//take damage before knockco in case this hit kills target so goes straight to death animation and not stagger first

        if(currentState == EnemyState.dead)
            return;
        
        myAnimator.SetBool("Stagger", true);//does this do anything?
        StartCoroutine(KnockCo(myRigidbody, knockTime));
    }

    private IEnumerator KnockCo(Rigidbody2D myRigidbody, float knockTime)
    {
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            //myRigidbody.velocity = Vector2.zero;
            myAnimator.SetBool("Stagger", false);
            currentState = EnemyState.idle;//set it back to idle after doing knockback animation and time so it can keep walking towards target in radiusawakened enemy

        }
    }
}
