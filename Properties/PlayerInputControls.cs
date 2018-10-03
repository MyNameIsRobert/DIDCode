using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputControls : MonoBehaviour {

    #region KeyCodes
    public KeyCode fire;
    public KeyCode run;
    public KeyCode jump;
    public KeyCode crouch;
    public KeyCode quickMelee;
    public KeyCode aim;
    public KeyCode flashlight;
    public KeyCode quickGrenade;
    public KeyCode weapon1;
    public KeyCode weapon2;
    public KeyCode weapon3;
    public KeyCode switchWeaponUp;
    public KeyCode switchWeaponDown;
    public KeyCode attachment1;
    public KeyCode attachment2;
    public KeyCode use;    
    public KeyCode inventory;
    public KeyCode pauseMenu;
    public KeyCode questMenu;
    public KeyCode weaponMenu;
    public KeyCode statsMenu;
    public KeyCode throwWeapon;
    public KeyCode reload;
    public KeyCode[] keys;

    #endregion
    // Use this for initialization
    void Start () {
        keys = new KeyCode[] {fire, run, jump, crouch, quickMelee, aim, flashlight, quickGrenade, weapon1, weapon2, weapon3, switchWeaponUp, switchWeaponDown, attachment1, attachment2,
                                use, inventory, pauseMenu, questMenu, weaponMenu, statsMenu, throwWeapon, reload};
	}
	
	// Update is called once per frame
	void Update () {
        fire = keys[0];
        run = keys[1];
        jump = keys[2];
        crouch = keys[3];
        quickMelee = keys[4];
        aim = keys[5];
        flashlight = keys[6];
        quickGrenade = keys[7];
        weapon1 = keys[8];
        weapon2 = keys[9];
        weapon3 = keys[10];
        switchWeaponUp = keys[11];
        switchWeaponDown = keys[12];
        attachment1 = keys[13];
        attachment2 = keys[14];
        use = keys[15];
        inventory = keys[16];
        pauseMenu = keys[17];
        questMenu = keys[18];
        weaponMenu = keys[19];
        statsMenu = keys[20];
        throwWeapon = keys[21];
        reload = keys[22];

    }

    IEnumerator KeyChange(int index)
    {
        KeyCode tempKey = KeyCode.None;
        int tempCounter = 0;
        while(tempCounter < 500000 && tempKey == KeyCode.None)
        {
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(vKey))
                {
                    tempKey = vKey;
                }
            }
            yield return null;
        }

        if (tempKey != KeyCode.None)
        {
            keys[index] = tempKey;
        }
        else
            Debug.Log("The while loop ran 500,000 times!");
        yield return null;
    }
    public void ChangeKey(int keyVariableIndex)
    {
        StartCoroutine(KeyChange(keyVariableIndex));
    }


}
