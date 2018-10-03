using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour {

    public string questTitle;
    [TextArea]
    public string questDescription;
    public QuestType questType;
    [Range(10,500)]
    public int ductTapeReward;
    [SerializeField]
    public float questDisplayLength = 5;

    [SerializeField]
    Item itemToPickup;
    [SerializeField]
    GameObject weaponToPickup;
    [SerializeField]
    Key keytoPickup;
    
    [SerializeField]
    bool isFirstQuest = false;
    public bool lastQuest = false;
    public Collider collider;



    public enum QuestType
    {
        GoTo,
        Activate,
        PickupItem,
        PickupWeapon,
        PickupKey
    }
	// Use this for initialization
	void Start () {
        if (isFirstQuest)
            collider.enabled = true;
        else
            collider.enabled = false;
        
        collider.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (questType != QuestType.GoTo)
                return;
            CompleteQuest(other.transform.root.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        GameObject player = other.transform.root.gameObject;
        WeaponSwitch weaponSwitch = player.GetComponent<WeaponSwitch>();
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        switch (questType)
        {
            case QuestType.PickupWeapon:
                for(int i = 0; i < weaponSwitch.hands.childCount; i++)
                {
                    if(weaponToPickup.name == weaponSwitch.hands.GetChild(i).name)
                    {
                        CompleteQuest(player);
                        break;
                    }
                }
                break;
            case QuestType.PickupKey:
                if (inventory.keyNumbers.Contains(keytoPickup.keyNumber))
                {
                    CompleteQuest(player);
                }
                break;
            case QuestType.PickupItem:
                if (inventory.inventory.Contains((Item)itemToPickup))
                {
                    CompleteQuest(player);
                }
                break;                
                
        }
      


    }
    void CompleteQuest(GameObject player)
    {
        collider.enabled = false;
        player.transform.Find("Duct Tape").GetComponent<Amount>().amountOf += ductTapeReward;
        return;
    }
}
