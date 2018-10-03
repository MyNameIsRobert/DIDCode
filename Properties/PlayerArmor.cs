using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArmor : MonoBehaviour {
    public Armor[] equippedArmor;
    PlayerInventory playerInventory;
    public DisplayInventory displayInventory;
	// Use this for initialization
	void Start () {
		if(equippedArmor.Length != 6)
        {
            equippedArmor = new Armor[6];
        }
        playerInventory = GetComponent<PlayerInventory>();
        if(displayInventory == null)
        {
            displayInventory = GetComponentInChildren<DisplayInventory>();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Armor EquipArmor(Armor newArmor)
    {
        Armor unequippedArmor = null;
        switch(newArmor.armorType)
        {
        case Armor.ArmorType.Head:
                if (equippedArmor[0] != null)
                {
                    Item tempItem = (Item)equippedArmor[0];
                    playerInventory.AddArmorToInventory(tempItem);
                    unequippedArmor = (Armor)tempItem;
                    equippedArmor[0] = newArmor;

                }
                else
                {
                    equippedArmor[0] = newArmor;
                }
            break;
        case Armor.ArmorType.Chest:
                if (equippedArmor[1] != null)
                {
                    Item tempItem = (Item)equippedArmor[1];
                    playerInventory.AddArmorToInventory(tempItem);
                    unequippedArmor = (Armor)tempItem;
                    equippedArmor[1] = newArmor;
                }
                else
                {
                    equippedArmor[1] = newArmor;
                }
                break;
        case Armor.ArmorType.Arms:
                if (equippedArmor[2] != null)
                {
                    Item tempItem = (Item)equippedArmor[2];
                    playerInventory.AddArmorToInventory(tempItem);
                    unequippedArmor = equippedArmor[2];
                    equippedArmor[2] = newArmor;
                }
                else
                {
                    equippedArmor[2] = newArmor;
                }
                break;
        case Armor.ArmorType.Legs:
                if (equippedArmor[3] != null)
                {
                    Item tempItem = (Item)equippedArmor[3];
                    playerInventory.AddArmorToInventory(tempItem);
                    unequippedArmor = equippedArmor[3];
                    equippedArmor[3] = newArmor;
                }
                else
                {
                    equippedArmor[3] = newArmor;
                }
                break;
        case Armor.ArmorType.Hands:
                if (equippedArmor[4] != null)
                {
                    Item tempItem = (Item)equippedArmor[4];
                    playerInventory.AddArmorToInventory(tempItem);
                    unequippedArmor = equippedArmor[4];
                    equippedArmor[4] = newArmor;
                }
                else
                {
                    equippedArmor[4] = newArmor;
                }
                break;
        case Armor.ArmorType.Feet:
                if (equippedArmor[5] != null)
                {
                    Item tempItem = (Item)equippedArmor[5];
                    playerInventory.AddArmorToInventory(tempItem);
                    unequippedArmor = equippedArmor[5];
                    equippedArmor[5] = newArmor;
                }
                else
                {
                    equippedArmor[5] = newArmor;
                }
                break;            

        }
        return unequippedArmor;
    }

    public void RemoveArmor(int index)
    {
        Armor tempArmor = equippedArmor[index];
        displayInventory.RemoveArmorEffects(tempArmor);
        playerInventory.AddArmorToInventory(tempArmor);
        equippedArmor[index] = null;
    }
}
