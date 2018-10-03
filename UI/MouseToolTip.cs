using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseToolTip : MonoBehaviour {
    string toolTipText = "";
    public int characterSizingLimit = 50;
    Text text;
    CanvasGroup group;
    // Use this for initialization
    void Start()
    {
        text = GetComponentInChildren<Text>();
        group = GetComponent<CanvasGroup>();

    }

	
	// Update is called once per frame
	void Update () {
        if(toolTipText == "")
        {
            group.alpha = 0;
            return;
        }
        else if(toolTipText.Length > 50)
        {
            text.fontSize = 10;
        }
        else
        {
            text.fontSize = 14;
        }
        group.alpha = 1;
		Vector2 mousePosition = Input.mousePosition;
        mousePosition.x += 85;
        mousePosition.y += 25;

        text.text = toolTipText;
        transform.position = mousePosition;

	}

    public void SetToolTipText(string text)
    {
        toolTipText = text;
    }

    public void SetToolTipTextWeight()
    {
        toolTipText = transform.root.GetComponent<Weight>().currentWeight.ToString() + "/" + transform.root.GetComponent<Weight>().maxWeight.ToString();
    }
    public void ResetText()
    {
        toolTipText = "";
    }
}
