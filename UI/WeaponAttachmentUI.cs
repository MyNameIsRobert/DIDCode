using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponAttachmentUI : MonoBehaviour {

    [SerializeField]
    Button[] weaponButtons;
    
    [SerializeField]
    GameObject weaponHolster;
    [SerializeField]
    PlayerInventory playerInventory;

    Attachment attachmentToAttach;

    Weapon weaponProperties;

    Text hintText;



	// Use this for initialization
	void Start () {
        playerInventory = transform.root.GetComponent<PlayerInventory>();
        weaponHolster = playerInventory.weaponSlot.gameObject;
	}


    // Update is called once per frame
    void Update()
    {
        if (attachmentToAttach != null)
        {
            for (int i = 0; i < weaponButtons.Length; i++)
            {
                if (i + 1 > playerInventory.weaponSlot.childCount)
                {
                    CanvasGroup temp = weaponButtons[i].transform.parent.GetComponent<CanvasGroup>();
                    temp.alpha = 0;
                    temp.interactable = false;
                    temp.blocksRaycasts = false;
                    //weaponButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                }
                else
                {
                    weaponButtons[i].enabled = true;
                    CanvasGroup temp = weaponButtons[i].transform.parent.GetComponent<CanvasGroup>();
                    GameObject weaponObject = weaponButtons[i].transform.parent.gameObject;
                    temp.alpha = 1;
                    temp.interactable = true;
                    temp.blocksRaycasts = true;
                    Image tempImage = weaponButtons[i].transform.GetChild(0).gameObject.GetComponent<Image>();
                    weaponProperties = weaponHolster.transform.GetChild(i).gameObject.GetComponent<Weapon>();
                    tempImage.sprite = weaponProperties.minimapIndicator.GetComponent<SpriteRenderer>().sprite;

                    if (weaponProperties.weaponType == Weapon.WeaponType.Gun)
                    {
                        List<Attachment> attachments = new List<Attachment>();
                        
                        Shoot tempShoot = (Shoot)weaponProperties;
                        for (int j = 0; j < tempShoot.attachments.Length; j++)
                        {
                            if (tempShoot.attachments[j] != null && tempShoot.attachments[j].itemName != "N/A")
                            {
                                attachments.Add(tempShoot.attachments[j]);
                            }
                        }
                        for (int j = 0; j < attachments.Count; j++)
                        {
                            weaponObject.transform.GetChild(2).GetChild(j).GetComponent<Image>().sprite = attachments[j].sprite;
                            weaponObject.transform.GetChild(2).GetChild(j).GetComponent<Image>().enabled = true;
                        }
                        for (int j = attachments.Count; j < 6; j++)
                        {
                            weaponObject.transform.GetChild(2).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                        if (!tempShoot.CanAttachAttachment(attachmentToAttach))
                        {
                            Debug.Log("Already have this type of attachment attached!");
                            weaponButtons[i].interactable = false;
                        }
                        else
                        {
                            weaponButtons[i].interactable = true;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < weaponButtons.Length; i++)
            {
                if (i + 1 > playerInventory.weaponSlot.childCount)
                {
                    CanvasGroup temp = weaponButtons[i].transform.parent.GetComponent<CanvasGroup>();
                    temp.alpha = 0;
                    temp.interactable = false;
                    temp.blocksRaycasts = false;
                    //weaponButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                    
                }
                else
                {
                    CanvasGroup temp = weaponButtons[i].transform.parent.GetComponent<CanvasGroup>();
                    temp.alpha = 1;
                    temp.interactable = true;
                    temp.blocksRaycasts = true;
                    GameObject weaponObject = weaponButtons[i].transform.parent.gameObject;
                    weaponButtons[i].enabled = false;
                    Image tempImage = weaponButtons[i].transform.GetChild(0).gameObject.GetComponent<Image>();
                    weaponProperties = weaponHolster.transform.GetChild(i).gameObject.GetComponent<Weapon>();
                    tempImage.sprite = weaponProperties.minimapIndicator.GetComponent<SpriteRenderer>().sprite;
                    weaponObject.transform.GetChild(1).GetComponent<Text>().text = weaponProperties.gameObject.name;
                    List<Attachment> attachments = new List<Attachment>();
                    if(weaponProperties.weaponType == Weapon.WeaponType.Gun)
                    {
                        Shoot tempShoot = (Shoot)weaponProperties;
                        for(int j = 0; j < tempShoot.attachments.Length; j++)
                        {
                            if(tempShoot.attachments[j] != null && tempShoot.attachments[j].itemName != "N/A")
                            {
                                attachments.Add(tempShoot.attachments[j]);
                            }
                        }
                    }
                    for(int j = 0; j < attachments.Count; j++)
                    {
                        weaponObject.transform.GetChild(2).GetChild(j).GetComponent<Image>().sprite = attachments[j].sprite;
                        weaponObject.transform.GetChild(2).GetChild(j).GetComponent<Image>().enabled = true;
                    }
                    if (attachments.Count > 0)
                        for (int j = attachments.Count; j < 6; j++)
                        {
                            weaponObject.transform.GetChild(2).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                    else
                    {
                        for(int j = 0; j < 6; j++)
                        {
                            weaponObject.transform.GetChild(2).GetChild(j).GetComponent<Image>().enabled = false;
                        }
                    }
                }
            }
        }
    }

    public void SetAttachment(Attachment newAtt)
    {
        
        attachmentToAttach = newAtt;
        //Debug.Log("New attachment: " + attachmentToAttach.itemName);
    }
}
