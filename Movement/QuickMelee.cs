using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickMelee : MonoBehaviour {

    float damage;
    PlayerProperties playerProperties;
    BoxCollider quickMeleeCollider;

    [SerializeField]
    float meleeTime;

    [SerializeField]
    float meleeCoolDown;

    float meleeCoolDownTimer;

    HealthSinglePlayer enemyHealth;

    bool hitEnemy;

    WeaponSwitch weaponSwitch;
    PlayerInputControls control;
    // Use this for initialization
    IEnumerator Melee()
    {
        quickMeleeCollider.enabled = true;
        yield return new WaitForSeconds(meleeTime);
        quickMeleeCollider.enabled = false;
        if(weaponSwitch.equippedWeapon != null)
            weaponSwitch.equippedWeapon.SendMessage("QuickMelee", .5f);
        if (hitEnemy)
        {
            enemyHealth.TakeDamage(playerProperties.playerStrength * damage);
            playerProperties.ShowHitmarker(false);
            enemyHealth = null;
            hitEnemy = false;            
        }
        yield return null;
    }
	void Start () {
        playerProperties = transform.root.gameObject.GetComponent<PlayerProperties>();
        damage = playerProperties.quickMeleeDamage;
        weaponSwitch = transform.root.gameObject.GetComponent<WeaponSwitch>();
        quickMeleeCollider = GetComponent<BoxCollider>();
        control = playerProperties.gameObject.GetComponent<PlayerInputControls>();
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKeyDown(control.quickMelee))
        {
            if (meleeCoolDownTimer <= 0)
            {
                meleeCoolDownTimer = meleeCoolDown;
                StartCoroutine(Melee());
                
            }
        }

        meleeCoolDownTimer -= Time.deltaTime;

	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.transform.root.gameObject.CompareTag("Enemy"))
        {
            enemyHealth = other.transform.root.gameObject.GetComponent<HealthSinglePlayer>();
            hitEnemy = true;
        }
    }

}
