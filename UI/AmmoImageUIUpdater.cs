using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoImageUIUpdater : MonoBehaviour {

    public Sprite pistolImage, arImage, sniperImage, shotgunImage, grenadeImage, specialImage;
    public Sprite noImage, meleeImage;
    Image image;
    WeaponSwitch weaponSwitch;
	// Use this for initialization
	void Start () {
        weaponSwitch = transform.root.GetComponent<WeaponSwitch>();
        image = GetComponent<Image>();
	}

    // Update is called once per frame
    void Update() {
        Shoot tempShoot;
        if (weaponSwitch.equippedWeapon != null && weaponSwitch.equippedWeapon.GetComponent<Weapon>().weaponType != Weapon.WeaponType.Melee)
        {
            tempShoot = weaponSwitch.equippedWeapon.GetComponent<Shoot>();
            switch(tempShoot.ammoType)
            {
                case AmmoEnum.AmmoType.Pistol:
                    image.sprite = pistolImage;
                    break;
                case AmmoEnum.AmmoType.AR:
                    image.sprite = arImage;
                    break;
                case AmmoEnum.AmmoType.Sniper_Heavy:
                    image.sprite = sniperImage;
                    break;
                case AmmoEnum.AmmoType.Shotgun:
                    image.sprite = shotgunImage;
                    break;
                case AmmoEnum.AmmoType.Special:
                    image.sprite = specialImage;
                    break;
                case AmmoEnum.AmmoType.Grenade:
                    image.sprite = grenadeImage;
                    break;
            }
        }
        else
        {
            if(weaponSwitch.equippedWeapon == null)
            {
                image.sprite = noImage;
            }
            else if(weaponSwitch.equippedWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Melee)
            {
                image.sprite = meleeImage;
            }
        }
    }
}
