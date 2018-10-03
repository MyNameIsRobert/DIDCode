using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateStats : MonoBehaviour {

    Text text;
    public string characterName = "Stats";
    HealthSinglePlayer playerHealth;
    Stamina playerStamina;
    PlayerProperties playerProperties;
    PlayerControllerSinglePlayer playerController;
    Weight weight;
    PlayerInputControls control;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        GameObject player = transform.root.gameObject;
        playerHealth = player.GetComponent<HealthSinglePlayer>();
        playerStamina = player.GetComponent<Stamina>();
        playerProperties = player.GetComponent<PlayerProperties>();
        playerController = player.GetComponent<PlayerControllerSinglePlayer>();
        weight = player.GetComponent<Weight>();
        control = player.GetComponent<PlayerInputControls>();

	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(control.statsMenu))
        {
            if(GetComponent<CanvasGroup>().alpha == 0)
            {
                GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                GetComponent<CanvasGroup>().alpha = 0;
            }
        }
        string tempString;
        float maxHealth = playerHealth.maxHealth, health = playerHealth.currentHealth, maxStamina = playerStamina.maxStamina, stamina = playerStamina.currentStamina, stealthModifier = playerProperties.appliedDetectionModifier, accuracyModifier = playerProperties.accuracyModifier, moveSpeed = playerController.appliedMoveSpeed;

        tempString = characterName + " - \n" + "Current HealthSinglePlayer: " + Mathf.Round(health) + "\nMax HealthSinglePlayer: " + maxHealth + "\nDamage Reduction: " + playerHealth.damageReduction + "\nCurrent Stamina: " + Mathf.Round(stamina) + "\nMax Stamina: " + maxStamina + "\nStealth Modifier: " + stealthModifier +
            "\nAccuracy Modifier: " + accuracyModifier + "\nMove Speed: " + moveSpeed + "\nMax Weight: " + weight.maxWeight + "\nCurrent Weight: " + weight.currentWeight;

        text.text = tempString;
	}
}
