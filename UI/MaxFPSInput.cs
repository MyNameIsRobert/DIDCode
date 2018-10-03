using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxFPSInput : MonoBehaviour {
    public InputField input;
    public Slider slider;
	// Use this for initialization
	void Start () {
        input.onValueChanged.AddListener(OnValueChange);
        input.onValidateInput += delegate (string input, int charIndex, char addedChar) { return ValidateInput(addedChar); };
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnValueChange(string newValue)
    {
        slider.value = Int32.Parse(newValue);
    }
    char ValidateInput(char value)
    {
        if (!char.IsNumber(value))
        {
            value = '\0';
        }
        return value;
    }
}
