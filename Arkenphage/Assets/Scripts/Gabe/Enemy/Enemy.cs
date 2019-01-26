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
    protected bool isDead = false;
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
            StartCoroutine(DeathEffect());
            //this.gameObject.SetActive(false);
        }
    }

    private IEnumerator DeathEffect()
    {
        isDead = true;
        myAnimator.SetBool("IsDead", true);
        yield return new WaitForSeconds(deathTime);
        currentState = EnemyState.dead;
        this.gameObject.SetActive(false);
        //uncomment below for adding a death particle effect if desired
        //if (deathEffect != null)
        //{
        //    GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        //    Destroy(effect, 1f);
        //}
    }

    public void Knock(Rigidbody2D myRigidbody, float knockTime, float damage)
    {
        myAnimator.SetBool("Stagger", true);//does this do anything?
        StartCoroutine(KnockCo(myRigidbody, knockTime));
        TakeDamage(damage);
    }

    private IEnumerator KnockCo(Rigidbody2D myRigidbody, float knockTime)
    {
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            currentState = EnemyState.stagger;
            myRigidbody.velocity = Vector2.zero;
            myAnimator.SetBool("Stagger", false);

        }
    }
}
