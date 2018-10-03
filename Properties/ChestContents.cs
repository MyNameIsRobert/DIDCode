using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestContents : MonoBehaviour {
	public List<Item> contents;
    [SerializeField]
    public List<int> itemNumberInStack;
    [SerializeField]
    public bool ammoChest;

    ChestInventoryUIController UI_Controller;

	void Start()
    {
        UI_Controller = transform.GetComponentInChildren<ChestInventoryUIController>();
	}

    public void ShowInventory(PlayerInventory player)
    {
        UI_Controller.ShowUI(player);
    }

    public void RemoveItem(int index)
    {
        contents.RemoveAt(index);
        itemNumberInStack.RemoveAt(index);
    }
    public void AddItem(Item newItem, PlayerInventory playerInventory, int inventoryItemIndex)
    {
        for(int i = 0; i < contents.Count; i++)
        {
            if(contents[i].itemName == newItem.itemName)
            {
                if(itemNumberInStack[i] + playerInventory.itemNumberInStack[i] < contents[i].stackLimit)
                {
                    itemNumberInStack[i] += playerInventory.itemNumberInStack[i];
                    playerInventory.RemoveItem(inventoryItemIndex);
                    return;
                }
            }
        }
    }

    public void AddItemToChest(Item newItem, int numOfItem)
    {
        contents.Add(newItem);
        itemNumberInStack.Add(numOfItem);
    }
}
