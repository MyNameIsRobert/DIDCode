using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxFPSSlider : MonoBehaviour {
    Slider slider;
    public InputField field;
	// Use this for initialization
	void Start () {
        slider = GetComponent<Slider>();

        slider.onValueChanged.AddListener(ValueChange);
	}
	
	// Update is called once per frame
	void Update () {

	}
    public void ValueChange(float value)
    {
        field.text = value.ToString();
        Application.targetFrameRate = (int)value;
    }
    
}
