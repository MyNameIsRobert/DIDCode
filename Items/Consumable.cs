using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable", fileName = "Consumable Name")]
public class Consumable : Item {

    public Type consumableType;
    [Range(0, 500)]
    public float amountToAffectBy;
    public NumberType amountNumberType;
    [Range(-1,420)]
    [Tooltip("How Long the Effect Lasts, in Seconds")]
    public float effectTime;
    public enum Type
    {
        HealthPotion,
        StaminaPotion,
        SpeedPotion,
        StealthPotion,
        AccuracyPotion,
        JumpPotion,
        MaxWeight,
        Strength,
        ReloadSpeed,
        FirePower
    }
    
    public enum NumberType
    {
        Percentage,
        FlatNumber
    }

    public void CopyTo(out Consumable newConsumable)
    {
        Consumable tempConsumable = new Consumable();
        tempConsumable.consumableType = consumableType;
        tempConsumable.amountToAffectBy = amountToAffectBy;
        tempConsumable.effectTime = effectTime;
        tempConsumable.amountNumberType = amountNumberType;
        tempConsumable.itemName = itemName;
        tempConsumable.itemDescription = itemDescription;
        tempConsumable.itemType = itemType;
        tempConsumable.itemWeight = itemWeight;
        tempConsumable.sprite = sprite;
        tempConsumable.stackLimit = stackLimit;
        newConsumable = tempConsumable;
    }

}
