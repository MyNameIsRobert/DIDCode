using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMoreEnemies : MonoBehaviour {

    public GameObject enemyToSpawn;
    float timer;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (timer <= 0)
        {
            timer = 2;
            if (GameObject.FindGameObjectsWithTag("Enemy").Length < 8)
            {
                while (GameObject.FindGameObjectsWithTag("Enemy").Length < 16)
                {
                    GameObject tempObject;
                    tempObject = Instantiate(enemyToSpawn, transform.position, transform.rotation);
                    tempObject.GetComponent<EnemyAI>().willWander = true;
                    tempObject.GetComponent<EnemyAI>().shootingRangeMode = true;
                }
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }
	}
}
