using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFlashing : MonoBehaviour {

    Light flashLight;

    [SerializeField]
    float flashFrequency = .3f;

    [SerializeField]
    int turnOffChance = 30;
    float flashTimer;
	// Use this for initialization
	void Start () {
        flashLight = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        if(flashTimer <= 0)
        {
            flashTimer = flashFrequency;
            bool flashOff = Random.Range(0, 100) <= turnOffChance;
            if (!flashOff)
                flashLight.intensity = 1.8f;
            else
                flashLight.intensity = 0f;
        }


        flashTimer -= Time.deltaTime;
	}
}
