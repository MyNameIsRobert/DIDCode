using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class DisplayInventoryAttachments : MonoBehaviour {

    public GameObject attachmentSlotToSpawn;
    public GameObject weaponSlotToSpawn;

    public Transform inventoryParent;
    public Transform weaponDisplayParent;

    List<Button> attachmentButtons;
    List<Image> attachmentImages;
    List<Button> weaponButtons;
    List<Image> weaponImages;

    WeaponSwitch weaponSwitch;
    PlayerInventory inventory;

    MouseToolTip floatingToolTip;
    UpdateItemTipText toolTipText;
    
    List<Attachment> attachmentsInInventory;
    List<int> attachmentInventoryIndex;

    List<Weapon> weaponsEquipped = new List<Weapon>();
    List<Shoot> gunsEquipped = new List<Shoot>();
    List<bool> canAttach = new List<bool>();

    int numOfAttachmentSlots = 0;
    int numOfWeaponSlots = 0;
	// Use this for initialization
	void Start () {
        weaponSwitch = transform.root.GetComponent<WeaponSwitch>();
        inventory = transform.root.GetComponent<PlayerInventory>();
        attachmentsInInventory = new List<Attachment>();
        attachmentInventoryIndex = new List<int>();
        attachmentButtons = new List<Button>();
        attachmentImages = new List<Image>();
        weaponButtons = new List<Button>();
        weaponImages = new List<Image>();
	}

    // Update is called once per frame
    void Update()
    {
        attachmentInventoryIndex.Clear();
        attachmentsInInventory.Clear();
        weaponsEquipped.Clear();
        //Filling up Inventory Attachments
        for (int i = 0; i < inventory.inventory.Count; i++)
        {
            if (inventory.inventory[i].itemType == Item.ItemType.Attachment)
            {
                attachmentsInInventory.Add((Attachment)inventory.inventory[i]);
                attachmentInventoryIndex.Add(i);
            }
        }
        //Filling up Equipped Weapons/Guns
        for (int i = 0; i < weaponSwitch.childCount; i++)
        {
            try
            {
                Weapon tempWeapon = weaponSwitch.hands.GetChild(i).GetComponent<Weapon>();
                weaponsEquipped.Add(tempWeapon);
                try
                {
                    Shoot tempShoot = (Shoot)tempWeapon;
                    gunsEquipped.Add(tempShoot);
                    canAttach.Add(true);

                }
                catch(System.InvalidCastException)
                {
                    Debug.Log("Weapon " + (i + 1) + " is not a gun!");
                    canAttach.Add(false);
                }
                catch(NullReferenceException)
                {
                    Debug.Log("No weapons!");
                }
            }
            catch(MissingComponentException)
            {
                Debug.Log("Child doesn't have a weapon component!");
            }
        }

        if(numOfAttachmentSlots < attachmentsInInventory.Count)
        {
            AddAttachmentSlot(3);
        }
        if(numOfWeaponSlots < weaponsEquipped.Count)
        {
            AddWeaponSlot();
        }
        if(numOfWeaponSlots > weaponsEquipped.Count)
        {
            RemoveWeaponSlot();
        }
        for(int i = 0; i < attachmentsInInventory.Count; i ++)
        {
            attachmentImages[i].enabled = true;
            attachmentImages[i].sprite = attachmentsInInventory[i].sprite;
            attachmentImages[i].preserveAspect = true;
            attachmentButtons[i].interactable = true;
        }
        for(int i = attachmentsInInventory.Count; i < attachmentImages.Count; i++)
        {
            attachmentImages[i].enabled = false;
            attachmentImages[i].sprite = null;
            attachmentButtons[i].interactable = false;
        }

        for(int i = 0; i < weaponsEquipped.Count; i++)
        {
            weaponImages[i].enabled = true;
            weaponImages[i].preserveAspect = true;
            if (weaponsEquipped[i].minimapIndicator != null)
                weaponImages[i].sprite = weaponsEquipped[i].minimapIndicator.GetComponent<SpriteRenderer>().sprite;
            weaponButtons[i].interactable = true;
        }
	}

    private void RemoveWeaponSlot()
    {
        int tempIndex = weaponButtons.Count - 1;
        GameObject slotToDestroy = weaponButtons[tempIndex].gameObject;
        slotToDestroy.GetComponent<RightClickButton>().rightClick.RemoveAllListeners();
        weaponButtons[tempIndex].onClick.RemoveAllListeners();
        slotToDestroy.GetComponent<EventTrigger>().triggers.Clear();
        Destroy(slotToDestroy);
        weaponButtons.RemoveAt(tempIndex);
        weaponImages.RemoveAt(tempIndex);
    }

    void AddWeaponSlot()
    {
        GameObject tempSpawned = Instantiate(weaponSlotToSpawn);
        tempSpawned.transform.SetParent(weaponDisplayParent.transform);
        tempSpawned.transform.localScale = new Vector3(1, 1);
        numOfWeaponSlots++;
        int tempInt = numOfWeaponSlots - 1;
        weaponImages.Add(tempSpawned.transform.GetChild(0).GetComponent<Image>());
        weaponButtons.Add(tempSpawned.GetComponent<Button>());

        tempSpawned.GetComponent<Button>().onClick.AddListener(() => OnButtonClickWeapon(tempInt));
        RightClickButton rightClick = tempSpawned.GetComponent<RightClickButton>();
        rightClick.rightClick.AddListener(() => OnRightClickWeapon(tempInt));
        EventTrigger trigger = tempSpawned.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { OnPointerExitWeapon(); });
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnPointerEnterWeapon(tempInt); });
        trigger.triggers.Add(entry);
        trigger.triggers.Add(exitEntry);
    }
    void AddAttachmentSlot()
    {
        GameObject tempSpawned = Instantiate(attachmentSlotToSpawn);
        tempSpawned.transform.SetParent(inventoryParent.transform);
        tempSpawned.transform.localScale = new Vector3(1, 1);
        numOfAttachmentSlots++;
        int tempInt = numOfAttachmentSlots - 1;
        attachmentImages.Add(tempSpawned.transform.GetChild(1).GetComponent<Image>());
        //armorImages[tempInt].rectTransform.localPosition = new Vector3(12.8f, -12.8f, 0);
        attachmentButtons.Add(tempSpawned.GetComponent<Button>());

        tempSpawned.GetComponent<Button>().onClick.AddListener(() => OnButtonClickAttachment(tempInt));
        RightClickButton rightClick = tempSpawned.GetComponent<RightClickButton>();
        rightClick.rightClick.AddListener(() => OnRightClickAttachment(tempInt));
        EventTrigger trigger = tempSpawned.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { OnPointerExitAttachment(); });
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnPointerEnterAttachment(tempInt); });
        trigger.triggers.Add(entry);
        trigger.triggers.Add(exitEntry);
    }
    void AddAttachmentSlot(int numOfSlots)
    {
        for (int i = 0; i < numOfSlots; i++)
        {
            GameObject tempSpawned = Instantiate(attachmentSlotToSpawn);
            tempSpawned.transform.SetParent(inventoryParent.transform);
            tempSpawned.transform.localScale = new Vector3(1, 1);
            numOfAttachmentSlots++;
            int tempInt = numOfAttachmentSlots - 1;
            attachmentImages.Add(tempSpawned.transform.GetChild(1).GetComponent<Image>());
            //armorImages[tempInt].rectTransform.localPosition = new Vector3(12.8f, -12.8f, 0);
            attachmentButtons.Add(tempSpawned.GetComponent<Button>());

            tempSpawned.GetComponent<Button>().onClick.AddListener(() => OnButtonClickAttachment(tempInt));
            RightClickButton rightClick = tempSpawned.GetComponent<RightClickButton>();
            rightClick.rightClick.AddListener(() => OnRightClickAttachment(tempInt));
            EventTrigger trigger = tempSpawned.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => { OnPointerExitAttachment(); });
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { OnPointerEnterAttachment(tempInt); });
            trigger.triggers.Add(entry);
            trigger.triggers.Add(exitEntry);
        }
    }

    void OnRightClickAttachment(int buttonNumber)
    {

    }    
    void OnButtonClickAttachment(int buttonNumber)
    {

    }
    void OnPointerEnterAttachment(int buttonNumber)
    {

    }
    void OnPointerExitAttachment()
    {

    }

    void OnButtonClickWeapon(int buttonNumber)
    {

    }
    void OnRightClickWeapon(int buttonNumber)
    { }
    void OnPointerEnterWeapon(int buttonNumber) { }
    void OnPointerExitWeapon() { }

}
