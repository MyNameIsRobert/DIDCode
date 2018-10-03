using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour {
    

    public List<Item> inventory;
    [SerializeField]
    public List<int> itemNumberInStack;
    [SerializeField]
    public List<int> keyNumbers = new List<int> {999};
    PlayerInputControls control;
	Amount currentAmmo;
	ChestContents chest;
	public Camera cam;
	public GameObject useText;
    WeaponSwitch weaponSwitch;
    public Transform weaponSlot;
    [SerializeField]
    DisplayChestContents chestDisplay;
    [SerializeField]
    DisplayInventory inventoryDisplay;

    PauseGame pause;

    
    int weaponNumber;

    
    bool isCoroutineRunning;

    [HideInInspector]
    public GameObject attachmentToAttach;

    void MoveToLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
            MoveToLayer(child, layer);
    }

    IEnumerator RemoveAttachments()
    {
        isCoroutineRunning = true;
        gameObject.GetComponent<PauseGame>().ShowAttachmentMenu();

        yield return null;
    }
    public struct Weapon1
    {
        public string name;
        public GameObject weapon;

        public GameObject[] attachmentGameObjects;
        public string[] attachmentNames;
        public float[] attachmentXPos, attachmentYPos, attachmentZPos;
        public Amount ammo;
        public float currentAmmoAmount;
    }

    public Weapon1[] equippedWeapons;
    bool canAutoSort;

    void Start(){
        weaponSwitch = GetComponent<WeaponSwitch>();
        equippedWeapons = new Weapon1[weaponSwitch.maxWeapons];
        pause = GetComponent<PauseGame>();
        control = GetComponent<PlayerInputControls>();
        if(chestDisplay == null)
            chestDisplay = transform.GetComponentInChildren<DisplayChestContents>();
	}
	void Update(){
        
        if ((Input.GetKey(control.pauseMenu) || Input.GetKey(control.inventory)) && isCoroutineRunning)
        {
            gameObject.GetComponent<PauseGame>().HideAttachmentMenu();
            StopCoroutine("AttachWeapon");
            attachmentToAttach = null;
            isCoroutineRunning = false;

        }
        Ray ray = new Ray (cam.transform.position, cam.transform.forward);
		RaycastHit hit;
		Debug.DrawRay (ray.origin, ray.direction);
		if (Physics.Raycast (ray, out hit, 5)) {

            if (hit.collider.CompareTag("Chest") || hit.collider.gameObject.GetComponent<ChestContents>())
            {
                useText.SetActive(true);
                chest = hit.collider.GetComponent<ChestContents>();
                useText.GetComponent<Text>().text = chest.ammoChest ? "Pick up Ammo" : "Open Chest";
                if (Input.GetKeyDown(control.use)) {

                    if (chest.ammoChest)
                    {
                        currentAmmo = weaponSwitch.equippedWeapon.GetComponent<Weapon>().ammo.GetComponent<Amount>();
                        currentAmmo.amountOf += currentAmmo.limit;
                    }
                    else
                    {
                        pause.ShowChestAndInventory();
                        chestDisplay.SetChestContents(chest);
                        inventoryDisplay.SetOpenChest(chest);
                    }

                                        

                }
            }
            else if (hit.collider.gameObject.CompareTag("Weapon")) {
                useText.SetActive(true);
                useText.GetComponent<Text>().text = "Pick up " + hit.collider.gameObject.GetComponent<PickUpProperties>().actualGunPrefab.name;

                string weaponName = hit.collider.gameObject.GetComponent<PickUpProperties>().actualGunPrefab.name;

                if (weaponSlot.childCount >= weaponSwitch.maxWeapons)
                {
                    useText.GetComponent<Text>().text = "Replace current weapon with " + weaponName;
                }
                UpdateEquippedWeapons();
                if (Input.GetKeyDown(control.use))
                {
                    if (weaponSlot.childCount >= weaponSwitch.maxWeapons)
                    {                        
                        GameObject tempWeapon = Instantiate(hit.collider.gameObject.GetComponent<PickUpProperties>().actualGunPrefab, weaponSlot.position, weaponSlot.rotation, weaponSlot);
                        tempWeapon.name = weaponName;

                        equippedWeapons[weaponSwitch.currentWeapon].name = hit.collider.gameObject.GetComponent<PickUpProperties>().actualGunPrefab.name;
                        equippedWeapons[weaponSwitch.currentWeapon].weapon = tempWeapon;

                        Weapon weaponValues = tempWeapon.GetComponent<Weapon>();
                        weaponValues.CopyFrom(hit.collider.gameObject.GetComponent<Weapon>());

                        if (weaponValues.weaponType == Weapon.WeaponType.Gun)
                        {
                            Shoot tempShoot = (Shoot)weaponValues;
                            tempShoot.CopyFrom(hit.collider.gameObject.GetComponent<Shoot>());
                            switch (tempShoot.shootType)
                            {
                                case Shoot.ShootType.GunShoot:
                                    GunShoot gunShoot = (GunShoot)tempShoot;
                                    gunShoot.CopyFrom(hit.collider.gameObject.GetComponent<GunShoot>());
                                    break;
                                case Shoot.ShootType.ShotgunShoot:
                                    ShotgunShoot shotgun = (ShotgunShoot)tempShoot;
                                    shotgun.CopyFrom(hit.collider.gameObject.GetComponent<ShotgunShoot>());
                                    break;
                                case Shoot.ShootType.SemiAutoShoot:
                                    break;
                                case Shoot.ShootType.ChargeShoot:
                                    ChargeShoot chargeShoot = (ChargeShoot)tempShoot;
                                    chargeShoot.CopyFrom(hit.collider.gameObject.GetComponent<ChargeShoot>());
                                    break;
                                case Shoot.ShootType.ProjectileShoot:
                                    break;
                            }
                        }
                        else
                        {
                            MeleeAttack tempMelee = (MeleeAttack)weaponValues;
                            tempMelee.CopyFrom(hit.collider.GetComponent<MeleeAttack>());
                        }
                        tempWeapon.transform.SetSiblingIndex(weaponSwitch.currentWeapon);

                        GameObject weaponToDestroy = weaponSlot.GetChild(weaponSwitch.currentWeapon + 1).gameObject;
                        hit.collider.gameObject.GetComponent<MiniMapIndicator>().DestroyCurrentIcon();
                        ThrowWeapon(weaponToDestroy);
                        Destroy(weaponToDestroy);
                        UpdateEquippedWeapons();
                        Destroy(hit.collider.gameObject);


                    }
                    else
                    {



                        GameObject weaponPickedUp = hit.collider.gameObject;
                        GameObject tempWeapon = Instantiate(weaponPickedUp.GetComponent<PickUpProperties>().actualGunPrefab, weaponSlot.position, weaponSlot.rotation, weaponSlot);
                        tempWeapon.name = weaponName;
                        UpdateEquippedWeapons();
                        Weapon weaponValues = tempWeapon.GetComponent<Weapon>();
                        weaponValues.CopyFrom(weaponPickedUp.GetComponent<Weapon>());
                        weaponValues.isPickup = false;
                        if (weaponValues.weaponType == Weapon.WeaponType.Gun)
                        {
                            Shoot tempShoot = (Shoot)weaponValues;
                            tempShoot.CopyFrom(hit.collider.gameObject.GetComponent<Shoot>());
                            switch (tempShoot.shootType)
                            {
                                case Shoot.ShootType.GunShoot:
                                    GunShoot gunShoot = (GunShoot)tempShoot;
                                    gunShoot.CopyFrom(hit.collider.gameObject.GetComponent<GunShoot>());
                                    break;
                                case Shoot.ShootType.ShotgunShoot:
                                    ShotgunShoot shotgun = (ShotgunShoot)tempShoot;
                                    shotgun.CopyFrom(hit.collider.gameObject.GetComponent<ShotgunShoot>());
                                    break;
                                case Shoot.ShootType.SemiAutoShoot:

                                    break;
                                case Shoot.ShootType.ChargeShoot:
                                    ChargeShoot chargeShoot = (ChargeShoot)tempShoot;
                                    chargeShoot.CopyFrom(hit.collider.gameObject.GetComponent<ChargeShoot>());
                                    break;
                                case Shoot.ShootType.ProjectileShoot:
                                    break;
                            }
                        }
                        else
                        {
                            MeleeAttack tempMelee = (MeleeAttack)weaponValues;
                            tempMelee.CopyFrom(hit.collider.GetComponent<MeleeAttack>());
                        }

                        weaponSwitch.DisableWeapon(tempWeapon);
                        hit.collider.gameObject.GetComponent<MiniMapIndicator>().DestroyCurrentIcon();
                        Destroy(hit.collider.gameObject);

                    }
                }
            }
            else if (hit.collider.gameObject.CompareTag("Door"))
            {
                Door tempDoor = hit.collider.gameObject.GetComponent<Door>();
                useText.SetActive(true);
                Text text = useText.GetComponent<Text>();
                if (tempDoor.GetIsLocked())
                {
                    bool hasKey = false;
                    int keyLoc = 0;
                    for(int i = 0; i < keyNumbers.Count; i++)
                    {
                        hasKey = tempDoor.IsCorrectKey(keyNumbers[i]);
                        if (hasKey)
                            keyLoc = i;
                    }
                    text.text = (hasKey) ? "Unlock door" : "Door requires key!";

                    if (Input.GetKeyDown(control.use) && hasKey)
                    {
                        tempDoor.UnlockDoor(keyNumbers[keyLoc]);
                        keyNumbers.RemoveAt(keyLoc);
                    }
                }
                else if((tempDoor.doorType == Door.DoorType.Use || tempDoor.doorType == Door.DoorType.RequiresKeyAfter || tempDoor.doorType == Door.DoorType.RequiresKeyInit) && !tempDoor.isOpen)
                {
                    text.text = "Open door";
                    if (Input.GetKeyDown(control.use))
                    {
                        tempDoor.OpenDoorWithUse();
                    }
                }
                else if(tempDoor.isOpen && tempDoor.doorType == Door.DoorType.Use)
                {
                    text.text = "Close door";
                    if (Input.GetKeyDown(control.use))
                    {
                        tempDoor.CloseDoorWithUse();
                    }
                }
                else
                {
                    useText.SetActive(false);
                }
                
            }
            else if (hit.collider.gameObject.CompareTag("Key"))
            {
                useText.SetActive(true);
                Text text = useText.GetComponent<Text>();
                text.text = "Pickup key";

                if (Input.GetKeyDown(control.use))
                {
                    try
                    {
                        Key key = hit.collider.gameObject.GetComponent<Key>();
                        keyNumbers.Add(key.keyNumber);
                        Destroy(hit.collider.gameObject);
                    }
                    catch(MissingComponentException e)
                    {
                        Debug.LogException(e);
                    }
                    
                }
            }
            else {
                useText.SetActive(false);
            }

		} else {
			useText.SetActive (false);
		}

        if (Input.GetKeyDown(control.throwWeapon))
        {
            if (weaponSlot.GetChild(weaponSwitch.currentWeapon).gameObject != null)
            {
                ThrowWeapon(weaponSlot.GetChild(weaponSwitch.currentWeapon).gameObject);
                weaponSwitch.SwitchWeaponUp();
            }
            UpdateEquippedWeapons();
        }

}

    void ThrowWeapon(GameObject weaponToThrow)
    {
        GameObject tempWeapon = Instantiate(weaponToThrow.GetComponent<PickUpProperties>().actualGunPrefab, weaponSlot.position, weaponSlot.rotation);
        tempWeapon.GetComponent<Rigidbody>().AddForce(4 * weaponSlot.transform.forward, ForceMode.Impulse);
        tempWeapon.GetComponent<Rigidbody>().AddTorque(2 * weaponSlot.transform.up, ForceMode.Impulse);
        Weapon weaponValues = tempWeapon.GetComponent<Weapon>();
        weaponValues.CopyFrom(weaponToThrow.GetComponent<Weapon>());
        weaponValues.isPickup = true;
        if(weaponValues.weaponType == Weapon.WeaponType.Gun)
        {
            Shoot tempShoot = (Shoot)weaponValues;
            tempShoot.CopyFrom(weaponToThrow.GetComponent<Shoot>());
            switch (tempShoot.shootType)
            {
                case Shoot.ShootType.GunShoot:
                    GunShoot tempGunShoot = (GunShoot)(tempShoot);
                    tempGunShoot.CopyFrom(weaponToThrow.GetComponent<GunShoot>());
                    break;
                case Shoot.ShootType.ShotgunShoot:
                    ShotgunShoot tempShotgun = (ShotgunShoot)(tempShoot);
                    tempShotgun.CopyFrom(weaponToThrow.GetComponent<ShotgunShoot>());
                    break;
                case Shoot.ShootType.SemiAutoShoot:
                    break;
                case Shoot.ShootType.ChargeShoot:
                    ChargeShoot chargeShoot = (ChargeShoot)tempShoot;
                    chargeShoot.CopyFrom(weaponToThrow.GetComponent<ChargeShoot>());
                    break;
                case Shoot.ShootType.ProjectileShoot:
                    break;
            }
        }
        else
        {
            MeleeAttack tempMelee = (MeleeAttack)weaponValues;
            tempMelee.CopyFrom(weaponToThrow.GetComponent<MeleeAttack>());
        }
        
        
        Destroy(weaponToThrow);
        UpdateEquippedWeapons();

    }
    //void UpdateWeaponProperties(GameObject weaponToUpdate, GameObject oldWeaponProperties)
    //{
    //    weaponToUpdate.GetComponent<WeaponProperties>().UpdateAllVariables(oldWeaponProperties.GetComponent<WeaponProperties>());
    //}
    //void UpdateWeaponProperties(GameObject weaponToUpdate, WeaponProperties oldWeaponProperties)
    //{
    //    WeaponProperties newProperties = weaponToUpdate.GetComponent<WeaponProperties>();
    //    newProperties.UpdateAllVariables(oldWeaponProperties);
        
        
    //}

    public void IncreaseMaxWeaponsByOne()
    {
        weaponSwitch.maxWeapons += 1;
        int equippedWeaponsLength = equippedWeapons.Length;
        Weapon1[] tempArray = new Weapon1[equippedWeaponsLength];
        for(int i = 0; i < equippedWeaponsLength; i++)
        {
            tempArray[i] = equippedWeapons[i];
        }
        equippedWeapons = new Weapon1[equippedWeaponsLength + 1];
        for(int i = 0; i < equippedWeaponsLength; i++)
        {
            equippedWeapons[i] = tempArray[i];
        }
        equippedWeapons[equippedWeaponsLength + 1] = new Weapon1();

    }

    void UpdateEquippedWeapons()
    {
        weaponSwitch.UpdateEquippedWeapons();
        if (weaponSlot.childCount < 1)
        {
            for(int i = 0; i < equippedWeapons.Length; i++)
            {
                equippedWeapons[i] = new Weapon1();
            }
        }
        else
        {
            for (int i = 0; i < weaponSwitch.equippedWeapons.Length; i++)
            {
                equippedWeapons[i].name = weaponSwitch.equippedWeapons[i].name;
                equippedWeapons[i].weapon = weaponSwitch.equippedWeapons[i].weapon;
                equippedWeapons[i].ammo = weaponSwitch.equippedWeapons[i].ammo;
            }
        }
    }

    /// <summary>
    /// Returns true if the player already has the weapon equipped
    /// </summary>
    /// <param name="weaponName"></param>
    /// <returns></returns>
    bool CheckIfWeaponInInventory(string weaponName, out int weaponPosition)
    {
        for(int i = 0; i < equippedWeapons.Length; i++)
        {
            if(equippedWeapons[i].name == weaponName)
            {
                weaponPosition = i;
                return true;
            }
        }
        weaponPosition = -1;
        return false;
    }
    bool CheckIfWeaponInInventory(string weaponName)
    {
        for (int i = 0; i < equippedWeapons.Length; i++)
        {
            if (equippedWeapons[i].name == weaponName)
            {

                return true;
            }
        }
        return false;
    }
    public void ChangeWeaponNumber(int newNumber)
    {
        weaponNumber = newNumber;
    }

    public void AddItemToInventory(Item newItem, ChestContents chest, int itemIndex)
    {
        bool breakLoop = false;
        bool sucessfullyAddedItem = false;
       for(int i = 0; i < inventory.Count; i++)
        { 
            if (inventory[i] == null)
            {
                switch (newItem.itemType)
                {
                    case Item.ItemType.Consumable:
                        Consumable inventoryConsumable = Instantiate<Consumable>((Consumable)(newItem));
                        Consumable tempConsumable = (Consumable)newItem;
                        tempConsumable.CopyTo( out inventoryConsumable);
                        inventory[i] = tempConsumable;
                        breakLoop = true;
                        sucessfullyAddedItem = true;
                        chest.contents[itemIndex] = null;
                        break;
                    case Item.ItemType.Armor:
                        break;
                    default:
                        break;
                }

            }
            else if(inventory[i].itemName == newItem.itemName)
            {
                if (itemNumberInStack[i] <= inventory[i].stackLimit - chest.itemNumberInStack[itemIndex])
                {
                    itemNumberInStack[i] += chest.itemNumberInStack[itemIndex];
                    breakLoop = true;
                    sucessfullyAddedItem = true;
                }
                else
                {
                    while (itemNumberInStack[i] < inventory[i].stackLimit)
                    {
                        itemNumberInStack[i]++;
                        chest.itemNumberInStack[itemIndex]--;
                        breakLoop = true;
                    }
                }
            }
            if (breakLoop)
                break;
        }
        if (!sucessfullyAddedItem)
        {
            inventory.Add(newItem);
            itemNumberInStack.Add(chest.itemNumberInStack[itemIndex]);
            chest.RemoveItem(itemIndex);
        }
        AutoSortInventory();
    }
    public void AddArmorToInventory(Item armor)
    {
        inventory.Add(armor);
        itemNumberInStack.Add(1);
        AutoSortInventory();
    }
    public void RemoveItem(int index)
    {
        inventory.RemoveAt(index);
        itemNumberInStack.RemoveAt(index);
    }
    public void AddAttachmentToInventory(Item attachment)
    {
        inventory.Add(attachment);
        itemNumberInStack.Add(1);
    }

    public void AutoSortInventory()
    {
        if (!canAutoSort)
        {
            return;
        }
        List<Consumable> tempConsumables = new List<Consumable>();
        List<int> tempConsumableAmount = new List<int>();
        List<Armor> tempArmor = new List<Armor>();
        List<int> tempArmorAmount = new List<int>();
        List<Attachment> tempAttachments = new List<Attachment>();
        List<int> tempAttachmentAmount = new List<int>();

        for(int i = 0; i < 3; i++)
        {
            if (inventory.Contains(inventoryDisplay.topThreeConsumables[i]))
            {
                int index = inventory.IndexOf(inventoryDisplay.topThreeConsumables[i]);
                tempConsumables.Add((Consumable)inventory[index]);
                tempConsumableAmount.Add(itemNumberInStack[index]);
                inventory.RemoveAt(index);
                itemNumberInStack.RemoveAt(index);
            }
        }
        for (int i = 0; i < inventory.Count; i++)
        {
            switch (inventory[i].itemType)
            {
                case Item.ItemType.Consumable:
                    tempConsumables.Add((Consumable)inventory[i]);
                    tempConsumableAmount.Add(itemNumberInStack[i]);
                    break;
                case Item.ItemType.Armor:
                    tempArmor.Add((Armor)inventory[i]);
                    tempArmorAmount.Add(itemNumberInStack[i]);
                    break;
                case Item.ItemType.Attachment:
                    tempAttachments.Add((Attachment)inventory[i]);
                    tempAttachmentAmount.Add(itemNumberInStack[i]);
                    break;
            }
        }
        List<Item> tempInventory = new List<Item>();
        List<int> tempItemNumInStack = new List<int>();

        for(int i = 0; i < tempConsumables.Count; i++)
        {
            tempInventory.Add(tempConsumables[i]);
            tempItemNumInStack.Add(tempConsumableAmount[i]);
        }
        for (int i = 0; i < tempArmor.Count; i++)
        {
            tempInventory.Add(tempArmor[i]);
            tempItemNumInStack.Add(tempArmorAmount[i]);
        }
        for (int i = 0; i < tempAttachments.Count; i++)
        {
            tempInventory.Add(tempAttachments[i]);
            tempItemNumInStack.Add(tempAttachmentAmount[i]);
        }
        inventory = tempInventory;
        itemNumberInStack = tempItemNumInStack;

    }

    public void SetAutoSort(bool set)
    {
        canAutoSort = set;
    } 
}