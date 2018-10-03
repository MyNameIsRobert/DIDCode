using UnityEngine;
using System.Collections;


public class HealthSinglePlayer : MonoBehaviour {
	
	public float maxHealth =100f;
	

	public bool destroyOnDeath;
    public bool isDead = false;

    public float damageReduction = 1;
    [Range(50,90)]
    public float maxDamageReduction;

    public int deathCounter;
    public float currentHealth;
    bool isEnemy;
    EnemyAI enemyAi;
	// Use this for initialization
	void Start ()
    {
        currentHealth = maxHealth;
        if (GetComponent<EnemyAI>())
        {
            isEnemy = true;
            enemyAi = GetComponent<EnemyAI>();
        }
			}

	public void TakeDamage(int amount){

        Debug.Log("Taking " + amount + " damge");

        currentHealth -= (float)amount * damageReduction;
			
		if (currentHealth <= 0) {

			if (destroyOnDeath) {
                isDead = true;
			}
			else{
			currentHealth = maxHealth;
             deathCounter++;
			}
		}
        if (currentHealth < 0) {
            currentHealth = 0;
        }
	}
    public void TakeDamage(float amount)
    {
        currentHealth -= (float)amount * damageReduction;

        if (currentHealth <= 0)
        {

            if (destroyOnDeath)
            {
                isDead = true;
            }
            else
            {
                currentHealth = maxHealth;
                deathCounter++;
            }
        }
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        currentHealth = (int)(currentHealth + .5f);
    }
    public void TakeDamage(float amount, bool detectPlayer)
    {
        currentHealth -= amount;
        if(currentHealth <= 0)
        {
            if (destroyOnDeath)
            {
                isDead = true;
            }
            else
            {
                currentHealth = maxHealth;
                deathCounter++;
            }
        }
        currentHealth = currentHealth < 0 ? 0 : currentHealth;
        if(detectPlayer && isEnemy)
        {
            Debug.Log("Triggering Player Detection");
            enemyAi.TriggerPlayerDetection();
        }
    }
    public void DamageOverTime(float amount, float time)
    {

    }
    public void HealNumber(float amount)
    {
        Debug.Log("Healed " + amount + " points");
        currentHealth += amount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
    public void HealPercentage(float percentage)
    {
        float amountToHealBy;
        amountToHealBy = percentage * (maxHealth / 100);
        currentHealth += amountToHealBy;

        Debug.Log("Healed a percentage of: " + percentage + " with a total number healed being: " + amountToHealBy);
        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;
    }
    public float CurrentHealth(){
		return currentHealth;
	}
    public bool GetIsDead(){
        return isDead;
    }
    /// <summary>
    /// Takes number between 1 and 100, converts it to a number between .01 and 1, then decreases damageReducion by that amount
    /// </summary>
    /// <param name="amount">
    /// A percentage between 1 and 100
    /// </param>
    public void ChangeDamageReduction(float amount)
    {
        amount /= 100;
        damageReduction -= amount;
        float tempAmount = 100 - maxDamageReduction;
        tempAmount /= 100;
        if(damageReduction <= tempAmount)
        {
            damageReduction = tempAmount;
        }
    }
	
    public void IncreaseMaxHealthFlatNumber(float amount)
    {
        maxHealth += amount;
    }
    public void IncreaseMaxHealthPercentage(float percentage)
    {
        float tempAmountToIncrease = (percentage / 100) * maxHealth;
        maxHealth += tempAmountToIncrease;
    }
    public void IncreaseMaxHealthPercentage(float percentage, out float amountIncreased)
    {
        float tempAmountToIncrease = (percentage / 100) * maxHealth;
        maxHealth += tempAmountToIncrease;
        amountIncreased = tempAmountToIncrease;
    }

}
