using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableFlashlight : MonoBehaviour {
    Light flashLight;
	// Use this for initialization
	void Start () {
        flashLight = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Flashlight")){
            if(flashLight.intensity == 1)
            {
                flashLight.intensity = 0;
            }
            else if(flashLight.intensity == 0)
            {
                flashLight.intensity = 1;
            }
            else
            {
                flashLight.intensity = 0;
            }
        }
	}
}
