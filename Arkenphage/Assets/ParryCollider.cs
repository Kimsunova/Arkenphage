using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryCollider : MonoBehaviour {

    public float thrust;
    public float knockTime;
    public float damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        var otherIsPlayer = player != null;
        var thisIsPlayer = this.GetComponentInParent<Player>() != null;

        var enemy = other.GetComponent<Enemy>();
        var otherIsEnemy = enemy != null;
        var thisIsEnemy = this.GetComponentInParent<Enemy>() != null;

        var bullet = other.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Parried();
        }
    }
}
