using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour {

    [SerializeField]
    public int currentWeapon = 0;

    [Tooltip("Set to one less than actual max. So a max of 3 would be input as 2")]
	public int maxWeapons = 2;

    public int childCount = 0;
	public Transform hands;

    bool canSwitchWeapon;

    public GameObject equippedWeapon;

    bool disabling = false;
    bool enabling = false;
    PlayerInputControls control;

    public struct Weapon1
    {
        public string name;
        public GameObject weapon;

        public GameObject[] attachmentGameObjects;
        public string[] attachmentNames;
        public float[] attachmentXPos, attachmentYPos, attachmentZPos;
        public Amount ammo;
        public float currentAmmoAmount;
        public Weapon1(string name)
        {
            this.name = name;
            weapon = null;
            attachmentGameObjects = null;
            attachmentNames = null;
            attachmentXPos = attachmentYPos = attachmentZPos = null;
            ammo = null;
            currentAmmoAmount = 0;
        }
    }

    public Weapon1[] equippedWeapons;
    void Start(){
        equippedWeapons = new Weapon1[maxWeapons];
        control = GetComponent<PlayerInputControls>();
	}
	void Update(){
        if(Time.timeScale == 0)
        {
            return;
        }
        childCount = hands.childCount;

		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            //UpdateEquippedWeapons();
            Debug.Log("Switching up");

            if(currentWeapon + 1 == maxWeapons)
            {
                Debug.Log("setting to 0");
                currentWeapon = 0;
            }
            else if(hands.childCount > currentWeapon + 1)
            {
                Debug.Log("increasing by one");
                currentWeapon++;
            }
            else
            {
                Debug.Log("Setting to 0");
                currentWeapon = 0;
            }
			//if (currentWeapon + 1 <= maxWeapons -1 ) {
   //             if (hands.GetChild(currentWeapon + 1) != null)
   //                 currentWeapon++;
   //             else if (hands.GetChild(currentWeapon - 1) != null)
   //                 currentWeapon--;
			//} else if(equippedWeapons[0].weapon != null) {
			//	currentWeapon = 0;
			//}
   //         else
   //         {
   //         }


            

        }
        else if ((Input.GetAxis("Mouse ScrollWheel") < 0))
        {
            //UpdateEquippedWeapons();
            Debug.Log("Switching down");
            if (currentWeapon - 1 >= 0)
			{
                if (equippedWeapons[currentWeapon - 1].weapon != null)
                    currentWeapon--;
                else if (equippedWeapons[currentWeapon + 1].weapon != null)
                    currentWeapon++;
			}
			else if(equippedWeapons[maxWeapons - 1].weapon != null)
			{
				currentWeapon = maxWeapons - 1;
			}
            else
            {
            }

            
        }
        else if (Input.GetKey(control.weapon1))
        {
            currentWeapon = 0;
        }
        else if (Input.GetKey(control.weapon2))
        {
            if(hands.childCount >= 2)
            {
                currentWeapon = 1;
            }
        }
        else if (Input.GetKey(control.weapon3))
        {
            if(hands.childCount >= 3)
            {
                currentWeapon = 2;
            }
        }
		if(currentWeapon > maxWeapons - 1)
		{
			currentWeapon = 0;
		}
		if(currentWeapon <= -1)
		{
			currentWeapon = maxWeapons - 1;
		}

        
		SelectWeapon (currentWeapon);
        if (hands.childCount > 0)
            equippedWeapon = hands.GetChild(currentWeapon).transform.gameObject;
        else
            equippedWeapon = null;

	}

    IEnumerator DisableObject(int index)
    {
        disabling = true;
        hands.GetChild(index).transform.gameObject.GetComponent<Animator>().SetTrigger("Reset");
        yield return new WaitForSeconds(.5f);
        disabling = false;
        hands.GetChild(index).transform.gameObject.SetActive(false);
        yield return null;
    }
   
    IEnumerator EnableObject(int index)
    {
        enabling = true;
        yield return new WaitForSeconds(.5f);
        hands.GetChild(index).transform.gameObject.SetActive(true);
        enabling = false;
        yield return null;
    }
	void SelectWeapon(int index){

		for (int i = 0; i < hands.childCount; i++) {
			if (i == index) {
               
                EnableWeapon(i);
                equippedWeapon = hands.GetChild(i).transform.gameObject;
                //UpdateEquippedWeapons();
			}
            else
            {
                 DisableWeapon(i);
			}
		}
	}

    public void SetCanSwitchWeapon(bool canSwitch)
    {
        canSwitchWeapon = canSwitch;
    }
    public void UpdateEquippedWeapons()
    {
//        if (hands.childCount < 1)
//        {
//            for (int i = 0; i < equippedWeapons.Length; i++)
//            {
//                equippedWeapons[i] = new Weapon1();
//            }
//        }
//        else if (hands.childCount > equippedWeapons.Length)
//        {

//        }
//        else if(equippedWeapons.Length > hands.childCount)
//        {
//            for (int i = 0; i < hands.childCount; i++)
//            {
//                GameObject tempWeapon = hands.GetChild(i).transform.gameObject;
//                equippedWeapons[i].name = tempWeapon.name;
//                equippedWeapons[i].weapon = tempWeapon;
//                equippedWeapons[i].weaponProperties = tempWeapon.GetComponent<WeaponProperties>();

//                //equippedWeapons[i].attachmentGameObjects = new GameObject[equippedWeapons[i].weaponProperties.GetAttachements().Length];

//                //equippedWeapons[i].attachmentNames = new string[equippedWeapons[i].weaponProperties.GetAttachements().Length];

//                //equippedWeapons[i].attachmentXPos = new float[equippedWeapons[i].weaponProperties.GetXPosAttachments().Length];
//                //equippedWeapons[i].attachmentYPos = new float[equippedWeapons[i].weaponProperties.GetYPosAttachments().Length];
//                //equippedWeapons[i].attachmentZPos = new float[equippedWeapons[i].weaponProperties.GetZPosAttachments().Length];

////                for (int j = 0; j < equippedWeapons[i].attachmentGameObjects.Length; j++)
////                {
////                    if(equippedWeapons[i].weaponProperties.GetAttachements()[j] != null) { 
////                    equippedWeapons[i].attachmentGameObjects[j] = equippedWeapons[i].weaponProperties.GetAttachements()[j];
////                    equippedWeapons[i].attachmentNames[j] = equippedWeapons[i].weaponProperties.GetAttachements()[j].name;
////                    equippedWeapons[i].attachmentXPos[j] = equippedWeapons[i].weaponProperties.GetXPosAttachments()[j];
////                    equippedWeapons[i].attachmentYPos[j] = equippedWeapons[i].weaponProperties.GetYPosAttachments()[j];
////                    equippedWeapons[i].attachmentZPos[j] = equippedWeapons[i].weaponProperties.GetZPosAttachments()[j];
////}
////                }

     



//            }
//            for(int i = hands.childCount; i < equippedWeapons.Length; i++)
//            {
//                equippedWeapons[i] = new Weapon1("Empty");
//            }
//        }
//        else
//        {
//            for (int i = 0; i < hands.childCount; i++)
//            {
//                GameObject tempWeapon = hands.GetChild(i).transform.gameObject;
//                equippedWeapons[i].name = tempWeapon.name;
//                equippedWeapons[i].weapon = tempWeapon;
//                equippedWeapons[i].weaponProperties = tempWeapon.GetComponent<WeaponProperties>();

//                //equippedWeapons[i].attachmentGameObjects = new GameObject[equippedWeapons[i].weaponProperties.GetAttachements().Length];

//                //equippedWeapons[i].attachmentNames = new string[equippedWeapons[i].weaponProperties.GetAttachements().Length];

//                //equippedWeapons[i].attachmentXPos = new float[equippedWeapons[i].weaponProperties.GetXPosAttachments().Length];
//                //equippedWeapons[i].attachmentYPos = new float[equippedWeapons[i].weaponProperties.GetYPosAttachments().Length];
//                //equippedWeapons[i].attachmentZPos = new float[equippedWeapons[i].weaponProperties.GetZPosAttachments().Length];

//                //for (int j = 0; j < equippedWeapons[i].attachmentGameObjects.Length; j++)
//                //{
//                //    if (equippedWeapons[i].weaponProperties.GetAttachements()[j] != null)
//                //    {
//                //        equippedWeapons[i].attachmentGameObjects[j] = equippedWeapons[i].weaponProperties.GetAttachements()[j];
//                //        equippedWeapons[i].attachmentNames[j] = equippedWeapons[i].weaponProperties.GetAttachements()[j].name;
//                //        equippedWeapons[i].attachmentXPos[j] = equippedWeapons[i].weaponProperties.GetXPosAttachments()[j];
//                //        equippedWeapons[i].attachmentYPos[j] = equippedWeapons[i].weaponProperties.GetYPosAttachments()[j];
//                //        equippedWeapons[i].attachmentZPos[j] = equippedWeapons[i].weaponProperties.GetZPosAttachments()[j];
//                //    }
//                //}

//                equippedWeapons[i].ammo = tempWeapon.GetComponentInChildren<Amount>();
//                equippedWeapons[i].currentAmmoAmount = equippedWeapons[i].ammo.amountOf;


//            }
//        }
    }
    public void SwitchWeaponUp()
    {
        //UpdateEquippedWeapons();
        if(hands.childCount < 1)
        {

        }
        else
        {

            currentWeapon = 0;
            SelectWeapon(currentWeapon);
        }
        

    }
    public void DisableWeapon(int index)
    {
        GameObject weapon = hands.GetChild(index).gameObject;
        Renderer[] renderer = weapon.GetComponentsInChildren<Renderer>(); 
        foreach(Renderer r in renderer)
        {
            r.enabled = false;
        }
        weapon.GetComponent<Animator>().SetTrigger("Reset");
        weapon.GetComponent<Weapon>().DisableWeapon();
    }

    public void DisableWeapon(GameObject weapon)
    {
        Renderer[] renderer = weapon.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderer)
        {
            r.enabled = false;
        }
        weapon.GetComponent<Animator>().SetTrigger("Reset");
        weapon.GetComponent<Weapon>().DisableWeapon();
    }

    public void EnableWeapon(int index)
    {
        GameObject weapon = hands.GetChild(index).gameObject;
        Renderer[] renderer = weapon.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderer)
        {
            r.enabled = true;
        }
        weapon.GetComponent<Weapon>().EnableWeapon();
        //Debug.Log("Enabled weapon: " + weapon.name);
    }

    public void EnableWeapon(GameObject weapon)
    {
        Renderer[] renderer = weapon.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderer)
        {
            r.enabled = true;
        }
        weapon.GetComponent<Weapon>().EnableWeapon();
        //Debug.Log("Disabled weapon: " + weapon.name);
    }

}

