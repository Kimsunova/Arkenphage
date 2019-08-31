using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour {

    public string sceneToGoTo;
    public Vector3 moveToPoint;
    GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}

    public void GoToScene()
    {
        SceneManager.LoadScene(sceneToGoTo, LoadSceneMode.Single);
        player.transform.position = moveToPoint;
    }
}
