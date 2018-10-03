using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class DisplayInventoryConsumables : MonoBehaviour {

    public GameObject consumableParent;
    public GameObject consumableSlotToSpawn;
    List<Consumable> consumablesInInventory;
    List<int> indexOfConsum;
    List<Button> consumButtons;
    List<Image> consumImages;
    List<Text> amountOfText;
    PlayerInventory inventory;
    [SerializeField]
    DisplayInventory inventoryDisplay;
    [SerializeField]
    MouseToolTip floatingToolTip;
    [SerializeField]
    UpdateItemTipText toolTipText;

    int numOfConsumSlots;
    // Use this for initialization
    void Start() {
        inventory = transform.root.GetComponent<PlayerInventory>();
        consumablesInInventory = new List<Consumable>();
        indexOfConsum = new List<int>();
        consumButtons = new List<Button>();
        consumImages = new List<Image>();
        amountOfText = new List<Text>();
        for(int i = 0; i < 6; i++)
        {
            AddConsumableSlot();
        }
    }

    // Update is called once per frame
    void Update() {
        consumablesInInventory.Clear();
        indexOfConsum.Clear();
        for (int i = 0; i < inventory.inventory.Count; i++)
        {
            if (inventory.inventory[i].itemType == Item.ItemType.Consumable)
            {
                consumablesInInventory.Add((Consumable)inventory.inventory[i]);
                indexOfConsum.Add(i);
            }
        }
        if (consumablesInInventory.Count > numOfConsumSlots)
        {
            for (int i = 0; i < 3; i++)
                AddConsumableSlot();
        }
        for (int i = 0; i < consumablesInInventory.Count; i++)
        {
            consumImages[i].sprite = consumablesInInventory[i].sprite;
            consumImages[i].preserveAspect = true;
            consumImages[i].enabled = true;
            consumButtons[i].interactable = true;
            if(inventory.itemNumberInStack[indexOfConsum[i]] > 1)
            {
                amountOfText[i].text = inventory.itemNumberInStack[indexOfConsum[i]].ToString();
            }
            else
            {
                amountOfText[i].text = "";
            }
        }
        for (int i = consumablesInInventory.Count; i < consumImages.Count; i++)
        {
            consumImages[i].enabled = false;
            consumButtons[i].interactable = false;
            amountOfText[i].text = "";
        }
    }
    void AddConsumableSlot()
    {
        GameObject tempSpawned = Instantiate(consumableSlotToSpawn);
        tempSpawned.transform.SetParent(consumableParent.transform);
        tempSpawned.transform.localScale = new Vector3(1, 1);
        numOfConsumSlots++;
        int tempInt = numOfConsumSlots - 1;
        consumImages.Add(tempSpawned.transform.GetChild(1).GetComponent<Image>());
        consumButtons.Add(tempSpawned.GetComponent<Button>());
        amountOfText.Add(tempSpawned.transform.GetChild(2).GetComponent<Text>());
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
    }

    private void OnPointerEnterDelegate(int buttonNumber)
    {
        if (buttonNumber - 1 > consumablesInInventory.Count)
        {
            floatingToolTip.ResetText();
            toolTipText.ResetToolTipText();
            return;
        }
        string tempString = "<b><color=navy><size=13>" + consumablesInInventory[buttonNumber].itemName + "</size></color></b>\n\n" + "<b><color=red>Use</color></b>";
        floatingToolTip.SetToolTipText(tempString);
        toolTipText.SetToolTipItemIndex(indexOfConsum[buttonNumber]);
    }

    void ButtonClickFunction(int buttonNumber)
    {
        inventoryDisplay.ApplyItemEffect(indexOfConsum[buttonNumber]);
        OnPointerEnterDelegate(buttonNumber);
    }
    void OnPointerExitDelegate()
    {
        Debug.Log("Pointer exited");
        floatingToolTip.ResetText();
        toolTipText.ResetToolTipText();
    }
}
