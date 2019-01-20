using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLoop : EnemyAI{

    public GameObject bullet;
    public Transform firePoint;
    public float timeBetweenShots;
    protected bool shoot = true;
    protected float timeReset;

    void Start()
    {
        timeReset = timeBetweenShots;
    }

    // Update is called once per frame
    void Update () {
        if (shoot == true)
        {
            timeBetweenShots -= Time.deltaTime;
            if (timeBetweenShots < 0)
            {
                FireBullet();
                timeBetweenShots = timeReset;
                print(timeBetweenShots + "  " + timeReset);
            }
        }
	}

    void FireBullet()
    {
        float face = firePoint.eulerAngles.y;
        float objRot = bullet.transform.eulerAngles.z;
        Instantiate(bullet, firePoint.position, Quaternion.Euler(0, face, objRot));
    }
}
