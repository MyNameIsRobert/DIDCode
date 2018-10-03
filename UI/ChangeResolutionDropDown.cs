using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeResolutionDropDown : MonoBehaviour {
    public Toggle fullscreen;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ChangeResolution(int resolution)
    {
        switch (resolution)
        {
            case 0:
                Screen.SetResolution( 640, 480, fullscreen.isOn);
                break;
            case 1:
                Screen.SetResolution( 1024, 600, fullscreen.isOn);
                break;
            case 2:
                Screen.SetResolution( 1024, 768, fullscreen.isOn);
                break;
            case 3:
                Screen.SetResolution( 1152, 720, fullscreen.isOn);
                break;
            case 4:
                Screen.SetResolution( 1152, 860, fullscreen.isOn);
                break;
            case 5:
                Screen.SetResolution( 1280, 720, fullscreen.isOn);
                break;
            case 6:
                Screen.SetResolution( 1440, 900, fullscreen.isOn);
                break;
            case 7:
                Screen.SetResolution( 1440, 1080, fullscreen.isOn);
                break;
            case 8:
                Screen.SetResolution( 1920, 1080, fullscreen.isOn);
                break;
            case 9:
                Screen.SetResolution( 1920, 1440, fullscreen.isOn);
                break;
            case 10:
                Screen.SetResolution( 2304, 1728, fullscreen.isOn);
                break;
            case 11:
                Screen.SetResolution( 2560, 1920, fullscreen.isOn);
                break;
            case 12:
                Screen.SetResolution( 3440, 1440, fullscreen.isOn);
                break;
            case 13:
                Screen.SetResolution( 3840, 2160, fullscreen.isOn);
                break;
            default:
                break;
        }
    }
}
