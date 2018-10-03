using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChangeMouseSensitivity : MonoBehaviour {

    public Slider mainSlider;
    float value;
    
    private void Update()
    {
        value = mainSlider.value;
        //Debug.Log(mainSlider.value);
        GetComponent<PlayerControllerSinglePlayer>().sensitivity = value;
    }


}
