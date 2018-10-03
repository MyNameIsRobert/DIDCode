using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestInventoryUIController : MonoBehaviour {

    [SerializeField]
    GameObject chest;

    ChestContents chestContents;

    Button[] chestButtons;
    [SerializeField]
    Image[] inventoryItemImages;
    Image[] backgroundImages;

    PlayerInventory userInventory;

    bool isUIDisabled = true;

	// Use this for initialization
	void Start () {
        chestButtons = new Button[transform.childCount];
        inventoryItemImages = new Image[chestButtons.Length];
        backgroundImages = new Image[chestButtons.Length];
        if (chest == null)
            chest = transform.root.gameObject;
        chestContents = chest.GetComponent<ChestContents>();
        for(int i = 0; i < inventoryItemImages.Length; i++)
        {
            chestButtons[i] = transform.GetChild(i).GetComponent<Button>();
            inventoryItemImages[i] = transform.GetChild(i).GetChild(1).GetComponent<Image>();
            backgroundImages[i] = transform.GetChild(i).GetChild(0).GetComponent<Image>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < chestContents.contents.Count; i++)
        {
            if (chestContents.contents[i] != null)
            {
                inventoryItemImages[i].sprite = chestContents.contents[i].sprite;
                inventoryItemImages[i].gameObject.SetActive(true);
                backgroundImages[i].gameObject.SetActive(true);
                chestButtons[i].interactable = true;
            }
            else
            {
                inventoryItemImages[i].gameObject.SetActive(false);
                chestButtons[i].interactable = false;
            }
        }
        for(int i = chestContents.contents.Count; i < inventoryItemImages.Length; i++)
        {
            inventoryItemImages[i].gameObject.SetActive(false);
            backgroundImages[i].gameObject.SetActive(false);
            chestButtons[i].interactable = false;
        }

        GameObject closestPlayer = AreaSearch.FindClosestInRadiusWithTag(transform.position, 6, "Player");
        if(closestPlayer != null)
        {
            ActivateUI();
            Camera playerCam = closestPlayer.GetComponent<PlayerProperties>().cam;
            transform.LookAt(playerCam.transform);
        }
        else
        {
            DisableUI();
        }

        
    }


    public void ShowUI(PlayerInventory player)
    {
        userInventory = player;
    }
    
    public void ActivateUI()
    {
        if (isUIDisabled)
        { 
            for(int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        isUIDisabled = false;
    }

    public void DisableUI()
    {
        if (!isUIDisabled)
        { 
            for(int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        isUIDisabled = true;
    }

}
