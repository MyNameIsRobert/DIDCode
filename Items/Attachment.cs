using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Item/Attachment", fileName = "Attachment Name")]
public class Attachment : Item {

    public AttachmentType attachmentType;

    public GameObject attachmentGameObject;

    [MinMaxRange(5, 500)]
    public MinMaxRange ductTapeNeeded;

    public AttachmentEffect[] attachmentEffects;
    public float[] attachmentEffectAmounts;
    public AttachmentEffectNumberType[] attachmentEffectAmountTypes;

	public enum AttachmentType
    {
        Sights,
        UnderBarrel,
        LeftBarrel,
        Muzzle,
        Stock,
        Magazine
    }


    public enum AttachmentEffect
    {
        MagazineSize,
        Damage,
        Silence,
        AimingAngleDivider,
        DefaultAngle,
        FireRate,
        BulletSpeed,
        FireAmmo,
        IceAmmo,
        ShotGunAccuracy,
        ZoomAmount,
        CritHitMultiplier,
        ReloadSpeed,
        AimSpeed,
        Kickback
        
    }

    public enum AttachmentEffectNumberType
    {
        FlatNumber,
        Percentage
    }
    public Attachment()
    {
        itemType = Item.ItemType.Attachment;
        stackLimit = 1;
        itemWeight = 2;
    }

    public Attachment(string name)
    {
        itemName = name;
    }

}
