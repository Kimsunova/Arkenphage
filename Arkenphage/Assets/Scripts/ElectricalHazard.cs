using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalHazard : MonoBehaviour {


    private ParticleSystem electricity;
    private BoxCollider2D thisCollider;
    [SerializeField] private float interval = 5f;
    private bool isActive = true;
    private float currentTimeElapsed = 0f;

    [SerializeField] [Range(0f,15f)] float electricityLifeTime;
    [SerializeField] [Range(0f,30f)] float boxColliderSize;

	// Use this for initialization
	void Start () {
        electricity = GetComponentInChildren<ParticleSystem>();
        thisCollider = GetComponent<BoxCollider2D>();
    }
	
	// Update is called once per frame
	void Update () {


        var main = electricity.main;
        main.startLifetime = electricityLifeTime;

        thisCollider.size = new Vector2(boxColliderSize, thisCollider.size.y);
        thisCollider.offset = new Vector2(boxColliderSize / 2, thisCollider.offset.y);


        if (currentTimeElapsed < interval)
        {
            currentTimeElapsed += Time.deltaTime;
        }
        else
        {
            isActive = !isActive;
            currentTimeElapsed = 0;
            if (isActive)
            {
                electricity.Play();
            }
            else
            {
                electricity.Stop();
                electricity.Clear();
            }
        }
        //electricity.emission.enabled = isActive;
        thisCollider.enabled = isActive;
		
	}
}
