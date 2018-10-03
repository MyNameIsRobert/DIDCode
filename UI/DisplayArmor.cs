using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Written by: Robert
//Takes the equippedArmor Array from PlayerArmor, and display's the sprite in it's respective slot on the UI

public class DisplayArmor : MonoBehaviour {

    #region Variable Declaration
    //Declares an array of buttons, and two parallel arrays of images: background and armor
    //The images are children of the buttons, and the background images are the first child
    [SerializeField]
    Button[] armorButtons = new Button[6];
    [SerializeField]
    Image[] backGroundImage = new Image[6], armorImage = new Image[6];

    //The component attached to the player containing their equipped armor
    [SerializeField]
    PlayerArmor playerArmor;

    #endregion

    #region Start Method
    void Start () {

        //Sets playerArmor to the toplevel transform (The Player Object)
        playerArmor = transform.root.GetComponent<PlayerArmor>();

#region Fills all arrays with their respective type
        //The object displayArmor is attached to has a first spot child not related to the buttons/images, so we skip over it
        for(int i = 0; i < 6; i++)
        {
            armorButtons[i] = transform.GetChild(i).GetComponent<Button>();
            backGroundImage[i] = transform.GetChild(i).GetChild(0).GetComponent<Image>();
            armorImage[i] = transform.GetChild(i).GetChild(1).GetComponent<Image>();
        }
#endregion
    }

    #endregion
    
    #region Update Function
    void Update()
    {
        //Cycles through playerArmor.equippedArmor
        for (int i = 0; i < 6; i++)
        {

            #region If that slot in equippedArmor is empty
            if (playerArmor.equippedArmor[i] == null)
            {
                armorButtons[i].interactable = false;
                armorImage[i].gameObject.SetActive(false);
            }
            #endregion
            #region If that slot contains something
            else
            {
                armorButtons[i].interactable = true;
                armorImage[i].gameObject.SetActive(true);
                armorImage[i].sprite = playerArmor.equippedArmor[i].sprite;
            } 
            #endregion
        }

    } 
    #endregion
}
