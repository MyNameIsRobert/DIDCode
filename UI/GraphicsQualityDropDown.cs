using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsQualityDropDown : MonoBehaviour {

    Dropdown dropdown;

	// Use this for initialization
	void Start () {
        dropdown = GetComponent<Dropdown>();
        dropdown.value = 5;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ChangeQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level,true);
        Debug.Log("Quality level is now: " + QualitySettings.GetQualityLevel());
    }
}
