using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Ammo", fileName = "Ammo")]
public class Ammo : Item {
    public AmmoEnum.AmmoType ammoType;
    private void Awake()
    {
        itemType = ItemType.Ammo;
        if(name != null)
        {
            itemName = name;
        }
    }
}
