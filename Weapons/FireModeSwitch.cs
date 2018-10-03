using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireModeSwitch : MonoBehaviour {
    [SerializeField]
    Shoot parentOriginalShoot;
    [SerializeField]
    Shoot parentAddedShoot;
    GameObject parentObject;

    float switchRate = .5f, lastSwitch = 0;

	// Use this for initialization
	void Start () {
        FindWeaponParent(transform);
        //Call FindWeaponParent to set the 'parent' Shoot, if it returns false, then stop the rest of Start
        if (parentOriginalShoot != null && !parentOriginalShoot.isPickup)
        {
            parentObject = parentOriginalShoot.gameObject;

            if (!parentObject.GetComponent<BurstShoot>()) //Check to see if the parent gameObject already has a BurstShoot component attached
            {
                BurstShoot tempShoot = parentObject.AddComponent<BurstShoot>(); //Adding a BurstShoot script to the weapon, and setting tempShoot to it
                tempShoot.bulletsPerBurst = 3; //Setting the bullets per burst of the BurstShoot
                tempShoot.burstFireRate = .05f; //Setting the bursetFireRate of the BurstShoot
                bool increasedMag = false;
                while(parentOriginalShoot.magazineSize % 3 != 0) //If the original magazine size is not divisible by three, increase it by one until it is (should run only 3 times, max)
                {
                    parentOriginalShoot.magazineSize++;
                    increasedMag = true;
                }
                if (increasedMag)
                    parentOriginalShoot.Reload(true);
                tempShoot.CopyFrom(parentOriginalShoot); //Copying all the Non-Burst properties into the BurstShoot
                tempShoot.ammo = parentOriginalShoot.ammo; //Giving the BurstShoot the same ammo object as the original Shoot
                parentAddedShoot = tempShoot; //Setting parentAddedShoot. The BurstShoot properties won't change, so we don't need to access them anymore

                
            }
            else //If it does have one attached
            {
                if(parentOriginalShoot.shootType == Shoot.ShootType.BurstShoot) 
                //If the parent gameObject is a BurstShoot Object
                {
                    parentAddedShoot = parentObject.AddComponent<SemiAutoShoot>();
                    parentAddedShoot.CopyFrom(parentOriginalShoot);
                    parentAddedShoot.ammo = parentOriginalShoot.ammo;                    
                }
            }
            parentAddedShoot.enabled = false;
            parentOriginalShoot.enabled = true;
        }
        else
        {
            Debug.Log("Couldn't find parent with shoot script!");
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (parentOriginalShoot.isPickup)
            return;
        parentAddedShoot.weaponEnabled = parentOriginalShoot.weaponEnabled;
        if (parentOriginalShoot.enabled)
        {
            parentAddedShoot.currentMagazine = parentOriginalShoot.currentMagazine;
            parentAddedShoot.reloaded = parentOriginalShoot.reloaded;
        }
        else
        {
            parentOriginalShoot.currentMagazine = parentAddedShoot.currentMagazine;
            parentOriginalShoot.reloaded = parentAddedShoot.reloaded;
        }
        #region Matching all new shoot variables to the original variables
        parentAddedShoot.CopyFrom(parentOriginalShoot, false);
        #endregion
        if (Input.GetButtonDown("Attachment")) //If player has pressed the attachment use button
        {
            if(Time.time > lastSwitch + switchRate) //if the player is within the switchRate
            {
                lastSwitch = Time.time; 
                parentOriginalShoot.enabled = !parentOriginalShoot.enabled; //Switch which shoot is enabled. Only one will be enabled at a time, so they will switch
                parentAddedShoot.enabled = !parentAddedShoot.enabled;
            }
        }

	}

    bool FindWeaponParent(Transform start)
    {
        if(start.parent == null)
        {
            Debug.Log("Could not find a Shoot parent!");
            return false;
        }
        if (start.transform.parent.GetComponent<Shoot>())
        {
            Debug.Log("Found Shoot parent!: " + start.transform.parent);
            parentOriginalShoot = start.transform.parent.GetComponent<Shoot>();
            return true;
        }
        else
        {
            Debug.Log(start.transform.name + "Does not have a shoot component!");
            FindWeaponParent(start.transform.parent);
        }
        return false;
    }
}
