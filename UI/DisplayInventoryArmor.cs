using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayInventoryArmor : MonoBehaviour {

    public GameObject armorParent;
    public GameObject armorSlotToSpawn;
    List<Armor> armorInInventory;
    List<int> indexOfArmor;
    List<Button> armorButtons;
    List<Image> armorImages;
    PlayerInventory inventory;
    [SerializeField]
    DisplayInventory inventoryDisplay;
    [SerializeField]
    MouseToolTip floatingToolTip;
    [SerializeField]
    UpdateItemTipText toolTipText;
    
    int numOfArmorSlots = 0;
	// Use this for initialization
	void Start () {
        inventory = transform.root.GetComponent<PlayerInventory>();
        armorInInventory = new List<Armor>();
        indexOfArmor = new List<int>();
        armorButtons = new List<Button>();
        armorImages = new List<Image>();
        for(int i = 0; i < 4; i ++)
        {
            AddArmorSlot();
        } 
	}
	
	// Update is called once per frame
	void Update () {
        armorInInventory.Clear();
        indexOfArmor.Clear();
        //Cycling through player inventory, and filling a list with each piece of armor in the inventory, and a parallel list of the index that armor is at
		for(int i = 0; i < inventory.inventory.Count; i++)
        {
            if(inventory.inventory[i].itemType == Item.ItemType.Armor)
            {
                armorInInventory.Add((Armor)inventory.inventory[i]);
                indexOfArmor.Add(i);
            }
        }
        if(armorInInventory.Count > numOfArmorSlots)
        {
            for (int i = 0; i < 3; i++)
                AddArmorSlot();
        }
        for(int i = 0; i < armorInInventory.Count; i++)
        {
            armorImages[i].sprite = armorInInventory[i].sprite;
            armorImages[i].enabled = true;
            armorButtons[i].interactable = true;
        }
        for(int i = armorInInventory.Count; i < armorImages.Count; i++)
        {
            armorImages[i].enabled = false;
            armorButtons[i].interactable = false;
        }
	}
    void AddArmorSlot()
    {
        GameObject tempSpawned = Instantiate(armorSlotToSpawn);
        tempSpawned.transform.SetParent(armorParent.transform);
        tempSpawned.transform.localScale = new Vector3(1,1);
        numOfArmorSlots++;
        int tempInt = numOfArmorSlots - 1;
        armorImages.Add(tempSpawned.transform.GetChild(1).GetComponent<Image>());
        //armorImages[tempInt].rectTransform.localPosition = new Vector3(12.8f, -12.8f, 0);
        armorButtons.Add(tempSpawned.GetComponent<Button>());

        tempSpawned.GetComponent<Button>().onClick.AddListener(() => ButtonClickFunction(tempInt));

        EventTrigger trigger = tempSpawned.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { OnPointerExitDelegate(); });
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnPointerEnterDelegate(tempInt); });
        trigger.triggers.Add(entry);
        trigger.triggers.Add(exitEntry);

        //UnityEvent rightClickTrigger = tempSpawned.GetComponent<RightClickButton>().rightClick;
        //rightClickTrigger.AddListener(() => OnRightClick(tempInt));
    }
    void ButtonClickFunction(int buttonNumber)
    {
        inventoryDisplay.ApplyItemEffect(indexOfArmor[buttonNumber]);
        OnPointerEnterDelegate(buttonNumber);
    }
    void OnPointerEnterDelegate(int buttonNumber)
    {
        if (buttonNumber > armorInInventory.Count)
        {
            floatingToolTip.ResetText();
            toolTipText.ResetToolTipText();
            return;
        }
        string placementString = "";
        switch(armorInInventory[buttonNumber].armorType)
        {
            case Armor.ArmorType.Arms:
                placementString = "Arms";
                break;
            case Armor.ArmorType.Chest:
                placementString = "Chest";
                break;
            case Armor.ArmorType.Feet:
                placementString = "Feet";
                break;
            case Armor.ArmorType.Hands:
                placementString = "Hands";
                break;
            case Armor.ArmorType.Head:
                placementString = "Head";
                break;
            case Armor.ArmorType.Legs:
                placementString = "Legs";
                break;
        }
        string tempString = "<b><color=navy><size=13>" + armorInInventory[buttonNumber].itemName + "</size></color></b>\n\n" + "<b><color=red>Equip</color></b> to <color=green>" + placementString + "</color>";
        floatingToolTip.SetToolTipText(tempString);
        toolTipText.SetToolTipItemIndex(indexOfArmor[buttonNumber]);
    }
    void OnPointerExitDelegate()
    {
        floatingToolTip.ResetText();
        toolTipText.ResetToolTipText();
    }
}
