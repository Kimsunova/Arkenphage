using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VineGolemState
{
    Idle,       //0
    VineAttack, //1
    ThornProj,  //2
    RootGrab,   //3
    NUM_STATES
}

public class Boss_VineGolem : EnemyAI {

    Animator anim;
    int randomAction, lastAction;
    bool needNewAction = true;
    float timer = 2f;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (anim.GetBool("Idle") == true)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SwitchAction();
                timer = 2f;
            }
        }
	}

    void SwitchAction()
    {
        switch (PickRandomAction())
        {
            case 0:
                anim.SetBool("Idle", true);
                Debug.Log("IDLE");
                break;
            case 1:
                anim.SetBool("Vine", true);
                Debug.Log("VINE");
                break;
            case 2:
                anim.SetBool("Thorn", true);
                Debug.Log("THORN");
                break;
            case 3:
                anim.SetBool("Root", true);
                Debug.Log("ROOT");
                break;
            default:
                anim.SetBool("Idle", true);
                Debug.Log("DEFAULT: IDLE");
                break;
        }
    }

    int PickRandomAction()
    {
        randomAction = Random.Range(0, (int)VineGolemState.NUM_STATES);
        if (randomAction != lastAction)
        {
            lastAction = randomAction;
            return randomAction;
        }
        else
            return PickRandomAction();
    }
}
