using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateHealthSinglePlayer : MonoBehaviour {
    TextMesh text3D;
    Text text2D;
    Slider slider;
    HealthSinglePlayer health;
    GameObject user;
    bool is3D;
    bool isSlider;
	// Use this for initialization
	void Start () {
        user = transform.root.gameObject;
        health = user.GetComponent<HealthSinglePlayer>();
        if (GetComponent<TextMesh>())
        {
            text3D = GetComponent<TextMesh>();
            is3D = true;
        }
        else if (GetComponent<Text>())
        {
            is3D = false;
            text2D = GetComponent<Text>();
         }
        else if (GetComponent<Slider>())
        {
            is3D = false;
            isSlider = true;
            slider = GetComponent<Slider>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (is3D)
            text3D.text = "" + health.currentHealth;
        else if (!isSlider)
            text2D.text = "" + health.currentHealth;
        else
        {
            slider.maxValue = health.maxHealth;
            slider.value = health.currentHealth;
        }
	}
}
