using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour {

    [Range(15,100)]
    public float maxStamina;

    [SerializeField]
    [Tooltip("How long the game waits to increase the stamina")]
    public float timeToIncrease = 3;
    public float currentStamina;

    [HideInInspector]
    public bool isStaminaEmpty;

    //[SerializeField]
    float staminaIncreaseTimer;

    [SerializeField]
    [Tooltip("How much stamina is gained per second")]
    public float staminaIncreaseAmount = 2;
	// Use this for initialization
	void Start () {
        currentStamina = maxStamina;
	}
	
	// Update is called once per frame
	void Update () {
		if(currentStamina < 0)
        {
            currentStamina = 0;
        }
        else
        {
            isStaminaEmpty = false;
        }
        if(currentStamina <= .3f)
        {
            isStaminaEmpty = true;
        }

        if(staminaIncreaseTimer > 0)
        {
            staminaIncreaseTimer -= Time.deltaTime;
        }
        else
        {
            
            currentStamina += staminaIncreaseAmount * Time.deltaTime;
        }

        if(currentStamina > maxStamina)
        {
            MaxOutStamina();
        }
	}

    public void DecreaseStamina(float amount)
    {
        staminaIncreaseTimer = timeToIncrease;
        currentStamina -= amount;
    }
    public void IncreaseStamina(float amount)
    {
        Debug.Log("Called increase stamina");
        currentStamina += amount;
    }

    public void MaxOutStamina()
    {
        currentStamina = maxStamina;
    }

    public void IncreaseMaxStaminaFlatNumber(float amount)
    {
        maxStamina += amount;
    }

    public void IncreaseMaxStaminaPercentage(float percentage)
    {
        float tempAmountToIncrease = (percentage / 100) * maxStamina;
        maxStamina += tempAmountToIncrease;
    }
    public void IncreaseMaxStaminaPercentage(float percentage, out float amountIncreased)
    {
        float tempAmountToIncrease = (percentage / 100) * maxStamina;
        maxStamina += tempAmountToIncrease;
        amountIncreased = tempAmountToIncrease;
    }
}
