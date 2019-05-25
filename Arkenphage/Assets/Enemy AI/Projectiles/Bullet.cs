using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    protected int direction = -1;
    public float speed, destroyTime, damage;
    protected Rigidbody2D rb;
    [SerializeField] float knockTime;
    [SerializeField] bool attackPlayer = true; //if "false", the bullet can still damage the player, but can also hurt enemies

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
        //else if (collision.tag == "Enemy" && attackPlayer != true){
        else if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            Parried();
        }
    }

    public void Parried()
    {
        direction *= -1;
        attackPlayer = !attackPlayer;
    }

}
