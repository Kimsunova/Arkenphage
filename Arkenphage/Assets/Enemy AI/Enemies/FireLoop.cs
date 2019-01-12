using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLoop : EnemyAI{

    public GameObject bullet;
    public Transform firePoint;
    public float timeBetweenShots;
    float timeReset;

    private void Start()
    {
        timeReset = timeBetweenShots;
    }

    // Update is called once per frame
    void Update () {
        timeBetweenShots -= Time.deltaTime;
        if (timeBetweenShots < 0)
        {
            FireBullet();
            timeBetweenShots = timeReset;
        }
	}

    void FireBullet()
    {
        float face = firePoint.eulerAngles.y;
        float objRot = bullet.transform.eulerAngles.z;
        Instantiate(bullet, firePoint.position, Quaternion.Euler(0, face, objRot));
    }
}
