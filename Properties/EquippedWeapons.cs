//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class EquippedWeapons : MonoBehaviour{

//    public WeaponSwitch weaponSwitch;
//    public static Weapon[] equippedWeapons;
//    public static Weapon currentWeapon;

    

//	// Use this for initialization
//	public void Start () {
//        weaponSwitch = GetComponent<WeaponSwitch>();
//        equippedWeapons = new Weapon[weaponSwitch.maxWeapons];
//        currentWeapon = equippedWeapons[weaponSwitch.currentWeapon];
//	}
	
//	// Update is called once per frame
//	void Update () {
//        currentWeapon = equippedWeapons[weaponSwitch.currentWeapon];
//        UpdateEquippedWeapons();
//    }

//    public void UpdateEquippedWeapons()
//    {

//        #region The player has no weapons equipped
//        if (weaponSwitch.hands.childCount < 1) //The player has no weapons equipped
//        {
//            for(int i = 0; i < equippedWeapons.Length; i++)
//            {
//                equippedWeapons[i] = new Weapon("Empty");
//            }
//        } 
//        #endregion

//        #region If WeaponSwitch's max weapons has increased or decreased since we last updated the weapons. A fail-safe
//        else if (weaponSwitch.maxWeapons != equippedWeapons.Length) //If WeaponSwitch's max weapons has increased or decreased since we last updated the weapons. A fail-safe
//        {
//            Weapon[] tempArray = new Weapon[equippedWeapons.Length]; //Create temp array
//            int count = 0; //counter variable
//            foreach (Weapon tempWeapon in equippedWeapons) //For each Weapon in equippedWeapons
//            {
//                tempArray[count] = tempWeapon; //Fill tempArray with Weapon from equippedWeapons
//                count++; //increase counter
//            }
//            count = 0; //reset counter
//            equippedWeapons = new Weapon[weaponSwitch.maxWeapons]; //create a new Weapon array the size of maxWeapons
//            foreach (Weapon tempWeapon in tempArray) //For each Weapon in tempArray
//            {
//                if (!(count >= equippedWeapons.Length)) //If tempArray is smaller than or equal to equippedWeapons
//                {
//                    equippedWeapons[count] = tempWeapon; //fill equippedWeapons with Weapon from tempArray
//                }
//                count++; //increase counter
//            }

//        }
//        #endregion

//        #region If there are more weapons in the player's hands than the size of equippedWeapons
//        else if (weaponSwitch.hands.childCount > equippedWeapons.Length) //If there are more weapons in the player's hands than the size of equippedWeapons
//        {
//            int count = 0;
//            foreach (Weapon tempWeapon in equippedWeapons) //For each Weapon in tempArray
//            {
//                if (!(count >= equippedWeapons.Length)) //If count is less than or equal to the lenght of equippedWeapons
//                {
//                     //fill equippedWeapons with Weapon from tempArray
//                }
//                else
//                {
//                    Destroy(weaponSwitch.hands.GetChild(count));
//                }
//                count++; //increase counter
//            }
            
//        }
//        #endregion

//        #region There are more slots in equipped weapons than there are weapons in the player's hands
//        else if (equippedWeapons.Length > weaponSwitch.hands.childCount) //There are more slots in equipped weapons than there are weapons in the player's hands
//        {
//            for (int i = 0; i < weaponSwitch.hands.childCount; i++)
//            {
//                GameObject tempWeapon = weaponSwitch.hands.GetChild(i).transform.gameObject;
//                equippedWeapons[i] = new Weapon(tempWeapon.name);
//                equippedWeapons[i].weapon = tempWeapon;
//                equippedWeapons[i].weaponProperties = tempWeapon.GetComponent<WeaponProperties>();

//                //equippedWeapons[i].attachmentGameObjects = new GameObject[equippedWeapons[i].weaponProperties.GetAttachements().Length];

//                //equippedWeapons[i].attachmentNames = new string[equippedWeapons[i].weaponProperties.GetAttachements().Length];

//                //equippedWeapons[i].attachmentXPos = new float[equippedWeapons[i].weaponProperties.GetXPosAttachments().Length];
//                //equippedWeapons[i].attachmentYPos = new float[equippedWeapons[i].weaponProperties.GetYPosAttachments().Length];
//                //equippedWeapons[i].attachmentZPos = new float[equippedWeapons[i].weaponProperties.GetZPosAttachments().Length];

//                //for (int j = 0; j < equippedWeapons[i].attachmentGameObjects.Length; j++)
//                //{
//                //    equippedWeapons[i].attachmentGameObjects[j] = equippedWeapons[i].weaponProperties.GetAttachements()[j];
//                //    equippedWeapons[i].attachmentNames[j] = equippedWeapons[i].weaponProperties.GetAttachements()[j].name;
//                //    equippedWeapons[i].attachmentXPos[j] = equippedWeapons[i].weaponProperties.GetXPosAttachments()[j];
//                //    equippedWeapons[i].attachmentYPos[j] = equippedWeapons[i].weaponProperties.GetYPosAttachments()[j];
//                //    equippedWeapons[i].attachmentZPos[j] = equippedWeapons[i].weaponProperties.GetZPosAttachments()[j];
//                //}

//                equippedWeapons[i].ammo = tempWeapon.GetComponentInChildren<Amount>();
//                equippedWeapons[i].currentAmmoAmount = equippedWeapons[i].ammo.amountOf;



//            }
//            for (int i = weaponSwitch.hands.childCount; i < equippedWeapons.Length; i++)
//            {
//                equippedWeapons[i] = new Weapon("Empty");
//            }
//        }
//        #endregion

//        #region Number of weapons in players hands equals length of equippedWeapons
//        else //Number of weapons in players hands equals length of equippedWeapons
//        {
//            for (int i = 0; i < equippedWeapons.Length; i++)
//            {
//                GameObject tempWeapon = weaponSwitch.hands.GetChild(i).transform.gameObject;
//                equippedWeapons[i] = new Weapon(tempWeapon.name);
//                equippedWeapons[i].weapon = tempWeapon;
//                equippedWeapons[i].weaponProperties = tempWeapon.GetComponent<WeaponProperties>();

//                //equippedWeapons[i].attachmentGameObjects = new GameObject[equippedWeapons[i].weaponProperties.GetAttachements().Length];

//                //equippedWeapons[i].attachmentNames = new string[equippedWeapons[i].weaponProperties.GetAttachements().Length];

//                //equippedWeapons[i].attachmentXPos = new float[equippedWeapons[i].weaponProperties.GetXPosAttachments().Length];
//                //equippedWeapons[i].attachmentYPos = new float[equippedWeapons[i].weaponProperties.GetYPosAttachments().Length];
//                //equippedWeapons[i].attachmentZPos = new float[equippedWeapons[i].weaponProperties.GetZPosAttachments().Length];

//                //for (int j = 0; j < equippedWeapons[i].attachmentGameObjects.Length; j++)
//                //{
//                //    equippedWeapons[i].attachmentGameObjects[j] = equippedWeapons[i].weaponProperties.GetAttachements()[j];
//                //    equippedWeapons[i].attachmentNames[j] = equippedWeapons[i].weaponProperties.GetAttachements()[j].name;
//                //    equippedWeapons[i].attachmentXPos[j] = equippedWeapons[i].weaponProperties.GetXPosAttachments()[j];
//                //    equippedWeapons[i].attachmentYPos[j] = equippedWeapons[i].weaponProperties.GetYPosAttachments()[j];
//                //    equippedWeapons[i].attachmentZPos[j] = equippedWeapons[i].weaponProperties.GetZPosAttachments()[j];
//                //}

//                equippedWeapons[i].ammo = tempWeapon.GetComponentInChildren<Amount>();
//                equippedWeapons[i].currentAmmoAmount = equippedWeapons[i].ammo.amountOf;


//            }
//        }
//        #endregion


//        for(int i = 0; i < equippedWeapons.Length; i++)
//        {
//            if (equippedWeapons[i] != null)
//                Debug.Log(equippedWeapons[i].weaponName);
//            else
//                Debug.Log("Weapon slot " + (i + 1) + " is null");
//        }
//    }

//}
