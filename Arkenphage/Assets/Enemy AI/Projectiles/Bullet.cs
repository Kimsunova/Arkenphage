using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed, destroyTime, damage;
    protected Rigidbody2D rb;
    [SerializeField] float knockTime;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, destroyTime);
        SpecialStartFunction();
	}

    protected virtual void SpecialStartFunction() { }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //we'll add damage once we work health into the game
            //collision.GetComponent<PlayerController>().HitPlayer();
            collision.GetComponent<Player>().Knock(knockTime, damage);
            Destroy(gameObject);
        }
    }

}
