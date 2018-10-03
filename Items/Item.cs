using UnityEngine;

[CreateAssetMenu(menuName = "Item", fileName = "Item Name")]
public class Item : ScriptableObject {
    public string itemName = "New Item";
    [TextArea]
    public string itemDescription = "Item Description";
    public ItemType itemType;
    public Sprite sprite;
    [Range(1,100)]
    public int stackLimit;
    public float itemWeight;

    public enum ItemType
    {
        Consumable,
        Armor,
        Attachment,
        Ammo
    }




}
