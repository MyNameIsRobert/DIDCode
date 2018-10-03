using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayChestContents : MonoBehaviour {

    [SerializeField]
    public ChestContents chestContents;
    [SerializeField]
    GameObject itemSlot;
    PlayerInventory playerInventory;
    [SerializeField]
    List<Button> itemButtons;
    [SerializeField]
    List<Text> amountOfText;
    [SerializeField]
    List<Image> backgroundImages, itemImage;
    [SerializeField]
    int numOfItemSlots = 0;

    [SerializeField]
    MouseToolTip floatingToolTip;
    [SerializeField]
    UpdateItemTipText toolTipText;

	// Use this for initialization
	void Start () {
        playerInventory = transform.root.gameObject.GetComponent<PlayerInventory>();
        numOfItemSlots = 1;
        Transform tempItemSlot = transform.GetChild(0);
        itemImage.Add(tempItemSlot.GetChild(1).GetComponent<Image>());
        itemButtons.Add(tempItemSlot.GetComponent<Button>());
        amountOfText.Add(tempItemSlot.GetChild(2).GetComponent<Text>());
        tempItemSlot.GetComponent<Button>().onClick.AddListener(() => ButtonClickFunction(0));

        EventTrigger trigger = tempItemSlot.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data, 0); });
        trigger.triggers.Add(entry);
        for (int i = 0; i < 8; i++)
            AddInventorySlot();
    }

    // Update is called once per frame
    void Update()
    {
        if (chestContents != null)
        {
            if (chestContents.contents.Count > numOfItemSlots)
            {
                for (int i = 0; i < 3; i++)
                    AddInventorySlot();
            }
            else
            {
                while (chestContents.contents.Count + 3 <= numOfItemSlots && numOfItemSlots > 9)
                {
                    for(int i = 0; i < 3; i++)
                        RemoveInventorySlot();
                }
            }
            for (int i = 0; i < chestContents.contents.Count; i++)
            {
                if (chestContents.contents[i] != null)
                {
                    itemImage[i].sprite = chestContents.contents[i].sprite;
                    itemImage[i].preserveAspect = true;
                    itemImage[i].gameObject.SetActive(true);
                    itemButtons[i].interactable = true;
                    if(chestContents.itemNumberInStack[i] > 1)
                    {
                        amountOfText[i].text = chestContents.itemNumberInStack[i].ToString();
                    }
                    else
                    {
                        amountOfText[i].text = "";
                    }

                }
                else
                {
                    itemImage[i].gameObject.SetActive(false);
                    itemButtons[i].interactable = false;
                    amountOfText[i].text = "";
                }
            }
            for (int i = chestContents.contents.Count; i < itemImage.Count; i++)
            {
                itemImage[i].gameObject.SetActive(false);
                itemButtons[i].interactable = false;
                amountOfText[i].text = "";
            }
            
        }
    }

    public void SetChestContents(ChestContents newContents)
    {
        chestContents = newContents;
    }

    public void AddItemToInventory(int index)
    {
        if (chestContents.contents[index].itemType == Item.ItemType.Ammo)
        {
            Ammo ammo = (Ammo)chestContents.contents[index];
            playerInventory.GetComponent<PlayerAmmo>().AddAmmo(chestContents.itemNumberInStack[index], ammo.ammoType);
            chestContents.contents.RemoveAt(index);
            chestContents.itemNumberInStack.RemoveAt(index);
        }
        else
        {
            playerInventory.AddItemToInventory(chestContents.contents[index], chestContents, index);
        }

    }
    void AddInventorySlot()
    {
        if (itemImage == null)
        {
            itemImage = new List<Image>(1);
            itemButtons = new List<Button>(1);
            amountOfText = new List<Text>(1);
        }
        GameObject tempSpawned = Instantiate(itemSlot);
        tempSpawned.transform.SetParent(transform);
        tempSpawned.transform.localScale = new Vector3(.5f, .5f);
        numOfItemSlots++;
        int tempInt = numOfItemSlots - 1;
        itemImage.Add(tempSpawned.transform.GetChild(1).GetComponent<Image>());
        itemImage[tempInt].rectTransform.localPosition = new Vector3(12.8f, -12.8f, 0);
        itemButtons.Add(tempSpawned.GetComponent<Button>());
        amountOfText.Add(tempSpawned.transform.GetChild(2).GetComponent<Text>());

        tempSpawned.GetComponent<Button>().onClick.AddListener(() => ButtonClickFunction(tempInt));

        EventTrigger trigger = tempSpawned.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data, tempInt); });
        trigger.triggers.Add(entry);

    }
    void RemoveInventorySlot()
    {
        int tempIndex = itemButtons.Count - 1;
        GameObject slotToDestroy = itemButtons[tempIndex].gameObject;
        itemButtons[tempIndex].onClick.RemoveAllListeners();
        EventTrigger trigger = slotToDestroy.GetComponent<EventTrigger>();
        trigger.triggers.Clear();
        Destroy(slotToDestroy);
        amountOfText.RemoveAt(tempIndex);
        itemImage.RemoveAt(tempIndex);
        itemButtons.RemoveAt(tempIndex);
        numOfItemSlots--;
    }
    void ButtonClickFunction(int buttonNumber)
    {
        AddItemToInventory(buttonNumber);
        OnPointerEnterDelegate(null, buttonNumber);
    }
    void OnPointerEnterDelegate(PointerEventData data, int buttonNumber)
    {
        if (buttonNumber > chestContents.contents.Count - 1)
        {
            floatingToolTip.ResetText();
            toolTipText.ResetToolTipText();
            return;
        }
            
        floatingToolTip.SetToolTipText(chestContents.contents[buttonNumber].itemName);
        toolTipText.SetToolTipFromChest(buttonNumber);
    }
}
