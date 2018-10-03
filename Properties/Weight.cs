using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weight : MonoBehaviour {

    [Range(15,200)]
    public float maxWeight;

    public float currentWeight;

    public float ammoWeight = 0.05f;
    PlayerInventory playerInventory;
    PlayerArmor playerArmor;
    PlayerProperties properties;
    PlayerAmmo playerAmmo;

    [SerializeField]
    GameObject UIElement;

    Image duffleImage;

    float lastWeight = 0;

	// Use this for initialization
	void Start () {
        playerArmor = GetComponent<PlayerArmor>();
        playerInventory = GetComponent<PlayerInventory>();
        properties = GetComponent<PlayerProperties>();
        duffleImage = UIElement.GetComponent<Image>();
        playerAmmo = GetComponent<PlayerAmmo>();
	}
	
	// Update is called once per frame
	void Update () {
        float tempWeight = 0;
		for(int i = 0; i < playerInventory.inventory.Count; i++)
        {
            tempWeight += (playerInventory.inventory[i].itemWeight * (float)playerInventory.itemNumberInStack[i]);
        }
        for(int i = 0; i < playerArmor.equippedArmor.Length; i++)
        {
            if(playerArmor.equippedArmor[i] != null)
                tempWeight += playerArmor.equippedArmor[i].itemWeight;
        }
        for(int i = 0; i < 5; i++)
        {
            tempWeight += playerAmmo.GetAmount((AmmoEnum.AmmoType)i) * ammoWeight;
        }
        currentWeight = tempWeight;

        properties.SetEncumbered(currentWeight > maxWeight);

        duffleImage.fillAmount = (currentWeight / maxWeight);


        lastWeight = currentWeight;
	}
}
