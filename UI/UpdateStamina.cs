using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateStamina : MonoBehaviour {

    GameObject player;
    Slider slider;
    Stamina stamina;
    [SerializeField]
    Image fillImage;
	// Use this for initialization
	void Start () {
        player = transform.root.gameObject;
        fillImage = GetComponent<Image>();
        stamina = player.GetComponent<Stamina>();
	}
	
	// Update is called once per frame
	void Update () {
        float staminaInTermsOfOne = stamina.currentStamina / stamina.maxStamina;
        fillImage.fillAmount = staminaInTermsOfOne;
        if(stamina.currentStamina <= .3f)
        {
            Color c = fillImage.color;
            c.a = 0;
            fillImage.color = c;
        }
        else
        {
            Color c = fillImage.color;
            c.a = 1;
            fillImage.color = c;
        }
	}
}
