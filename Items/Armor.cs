using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Item/Armor", fileName = "Armor Name")]
public class Armor : Item {
    public ArmorType armorType;

    [Range(-5, 20)]
    public float damageReductionAmount;

    public AdditionalEffectType additionalEffectType;

    [Range(0,300)]
    public float additionalEffectAmount;

    public AdditionalEffectNumberType additionalEffectNumberType;
        

    public enum ArmorType
    {
        Head,
        Chest,
        Arms,
        Legs,
        Hands,
        Feet
    }

    public enum AdditionalEffectType
    {
        MaxHealth,
        MaxStamina,
        StaminaRegen,
        HealthRegen,
        MaxWeight,
        IncreaseStealth,
        IncreaseAccuracy,
        IncreaseSpeed,
        NoAdditionalEffect
    }

    public enum AdditionalEffectNumberType
    {
        Percentage,
        FlatNumber
    }

    public Armor()
    {
        itemType = Item.ItemType.Armor;
        stackLimit = 1;
        itemWeight = 1;
        additionalEffectType = AdditionalEffectType.NoAdditionalEffect;
    }
}
