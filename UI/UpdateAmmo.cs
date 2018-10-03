using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateAmmo : MonoBehaviour {

	public GameObject player;
    public float currentAmmo;
	public Amount ammoCount;
    WeaponSwitch weaponSwitch;
    PlayerAmmo playerAmmo;
	Text text;
    public Toggle displayAmmoCountToggle;
    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        weaponSwitch = transform.root.GetComponent<WeaponSwitch>();
        playerAmmo = weaponSwitch.GetComponent<PlayerAmmo>();
    }

	
	// Update is called once per frame
	void LateUpdate () {
        Weapon equippedWeapon;
        Shoot tempShoot;
        if (weaponSwitch.equippedWeapon != null)
        {
            equippedWeapon = weaponSwitch.equippedWeapon.GetComponent<Weapon>();
            if(equippedWeapon.weaponType == Weapon.WeaponType.Gun)
            {
                tempShoot = (Shoot)equippedWeapon;
                if (displayAmmoCountToggle.isOn)
                    text.text = (int)tempShoot.currentMagazine + "\n" + playerAmmo.GetAmount(tempShoot.ammoType);
                else
                {
                    int tempInt = (int)tempShoot.currentMagazine;
                    text.text = tempInt.ToString();
                }
            }
            else
            {
                text.text = "\u221E";
            }
        }
        else
        {
            text.text = "";
        }

	}

   public void ChangeAmountObject(Amount newAmmo, float currentMagazine) {
        ammoCount = newAmmo;
        currentAmmo = currentMagazine;

    }
}
