using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateItemTipText : MonoBehaviour {

    Text text;
    GameObject player;
    PlayerInventory inventory;
    [SerializeField]
    ChestContents chest;
    [SerializeField]
    DisplayChestContents chestDisplay;
	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        if (player == null)
            player = transform.root.gameObject;
        inventory = player.GetComponent<PlayerInventory>();
        if (chestDisplay == null)
            chestDisplay = transform.parent.parent.GetComponentInChildren<DisplayChestContents>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ResetToolTipText()
    {
        text.text = "";
    }
    public void SetToolTipItemIndex(int index)
    {
        text.text = inventory.inventory[index].itemName + " - " + inventory.inventory[index].itemDescription + "\n" + inventory.itemNumberInStack[index] + " in stack"; 
    }
    public void SetToolTipFromChest(int index)
    {
        chest = chestDisplay.chestContents;
        text.text = chest.contents[index].itemName + " - " + chest.contents[index].itemDescription + "\n" + chest.itemNumberInStack[index] + " in stack";
    }
}
