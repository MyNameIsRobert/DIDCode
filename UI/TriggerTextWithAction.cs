using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerTextWithAction : MonoBehaviour {

    [TextArea(3, 10)]
    public string textToDisplay = "";
    [Tooltip("Enter either: Collider Enter, Collider Exit, or Button Press")]
    public string whatTrigger = "";
    [Tooltip("Only enter if the trigger is a button press")]
    public string axisName;

    [Tooltip("How long to display the text for, in seconds")]
    public float displayLength;
    public bool delayDisplayUntilOtherTextIsDone = false;
    public GameObject[] otherText;

    bool colEnter = false;
    bool colExit = false;
    bool butPress = false;
    bool show = false;
    // Use this for initialization
    void Start () {
        if (string.Compare(whatTrigger, "Collider Enter", true) != 0 && string.Compare(whatTrigger, "Collider Exit", true) != 0 && string.Compare(whatTrigger, "Button Press", true) != 0)
        {
            Debug.Log("You didn't enter a correct trigger!");
            gameObject.SetActive(false);
        }
        else if (string.Compare(whatTrigger, "Collider Enter", true) == 0)
        {
            colEnter = true;
        }
        else if (string.Compare(whatTrigger, "Collider Exit", true) == 0)
        {
            colExit = true;
        }
        else
            butPress = true;
        GetComponent<Text>().enabled = false;
        GetComponent<Text>().text = textToDisplay;

    }
	
	// Update is called once per frame
	void Update () {

        if (delayDisplayUntilOtherTextIsDone) {
            for (int i = 0; i < otherText.Length; i++)
                if (otherText[i].activeInHierarchy)
                    return;
        }
        if (butPress) {
            if (Input.GetAxis(axisName) != 0) {
                show = true;
            }
        }
        if (show) {
            displayLength -= Time.deltaTime;
            GetComponent<Text>().enabled = true;
        }

        if (displayLength <= 0) {
            GetComponent<Text>().enabled = false;
            show = false;
        }
	}
}
