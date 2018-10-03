using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateWeaponName : MonoBehaviour {

    Text currentText;

    [SerializeField]
    GameObject player;

    WeaponSwitch weaponSwitch;

    [SerializeField]
    string weaponName;
	// Use this for initialization
	void Start () {
		if(player == null)
        {
            player = transform.root.gameObject;
        }
        currentText = GetComponent<Text>();
        weaponSwitch = player.GetComponent<WeaponSwitch>();
	}
	
	// Update is called once per frame
	void Update () {
        if (weaponSwitch.hands.childCount > 0 && weaponSwitch.hands.GetChild(weaponSwitch.currentWeapon) != null)
        {
            weaponName = weaponSwitch.hands.GetChild(weaponSwitch.currentWeapon).name;
        }
        else
        {
            weaponName = " ";
        }

        currentText.text = weaponName;
	}
}
