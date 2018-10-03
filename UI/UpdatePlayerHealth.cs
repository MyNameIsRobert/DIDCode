using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePlayerHealth : MonoBehaviour {

    HealthSinglePlayer health;
    Text text;
	// Use this for initialization
	void Start () {
        health = transform.root.gameObject.GetComponent<HealthSinglePlayer>();
        text = GetComponent<Text>(); 
	}

    // Update is called once per frame
    void Update()
    {
        text.text = health.currentHealth + "/" + health.maxHealth;

    }
}
