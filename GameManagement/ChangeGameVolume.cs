using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeGameVolume : MonoBehaviour {

    public Slider mainSlider;

    // Update is called once per frame
    public void OnValueChanged() {
        AudioListener.volume = mainSlider.value;
    }
    
}
