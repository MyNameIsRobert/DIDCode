using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoSortInventoryToggle : MonoBehaviour {
    Toggle toggle;
    PlayerInventory inventory;
	// Use this for initialization
	void Start () {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate { ToggleOnValueChanged(toggle); });
        inventory = transform.root.GetComponent<PlayerInventory>();
        inventory.SetAutoSort(toggle.isOn);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ToggleOnValueChanged(Toggle newToggle)
    {
        inventory.SetAutoSort(newToggle.isOn);
    }
}
