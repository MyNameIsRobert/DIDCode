
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour {

    public bool playerDetected;

    [SerializeField]
    public bool shootingRangeMode;


    [SerializeField]
    float memoryTime;

    HealthSinglePlayer selfHealth;

    public PlayerProperties playerProperties;

    float currentHealth;

    Transform playerPosition;
    float detectionTimer;
    [SerializeField]
    Transform eyes;
	public EnemyAI ai;

    [SerializeField]
    [Tooltip("The chance to detect the player, in percentage")]
    float chanceToDetect = 50f;

    float appliedChanceToDetect;

    [SerializeField]
    float chanceToDetectTimerSet = .3f;

    float randomDetectionTimer = .3f;
    private void Start()
    {
        selfHealth = transform.root.GetComponent<HealthSinglePlayer>();
        currentHealth = selfHealth.CurrentHealth();
        shootingRangeMode = ai.shootingRangeMode;
        ai.AddDetectionObject(this);
    }

    void OnTriggerStay(Collider other){
        if (shootingRangeMode)
        {
            return;
        }
        if (randomDetectionTimer <= 0)
        {
            randomDetectionTimer = chanceToDetectTimerSet;
            if (other.CompareTag("Player"))
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.root.position, other.transform.position - transform.root.position);
                Ray eyesRay = new Ray(eyes.position, other.transform.position - eyes.position);
                Debug.DrawRay(ray.origin, ray.direction);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.transform.root.gameObject.CompareTag("Player"))
                    {
                        playerProperties = hit.collider.transform.root.gameObject.GetComponent<PlayerProperties>();
                        appliedChanceToDetect = chanceToDetect * playerProperties.appliedDetectionModifier;
                        if(appliedChanceToDetect > 90)
                        {
                            appliedChanceToDetect = 90;
                        }
                        float detectionChance = Random.Range(0, 100);
                        //Debug.Log(detectionChance);
                        if(detectionChance > 100 - appliedChanceToDetect) {
                            playerDetected = true;
                            playerPosition = other.transform;
                        }
                    }
                    else if (Physics.Raycast(eyesRay, out hit))
                    {
                        if (hit.collider.transform.root.gameObject.CompareTag("Player"))
                        {
                            playerProperties = hit.collider.transform.root.gameObject.GetComponent<PlayerProperties>();
                            appliedChanceToDetect = chanceToDetect * playerProperties.appliedDetectionModifier;
                            if (appliedChanceToDetect > 90)
                            {
                                appliedChanceToDetect = 90;
                            }
                            float detectionChance = Random.Range(0, 100);
                            //Debug.Log(detectionChance);
                            if (detectionChance > 100 - appliedChanceToDetect)
                            {
                                playerDetected = true;
                                playerPosition = other.transform;
                            }
                        }
                    }
                    else
                    {
                        playerDetected = false;
                    }
                }
            }
        }
        randomDetectionTimer -= Time.deltaTime;   
	}

	void OnTriggerExit(Collider other){
		if (other.CompareTag ("Player")) {
            playerDetected = false;
		}
	}

    private void Update()
    {
        if (shootingRangeMode)
        {
            return;
        }
        if (playerDetected)
        {
            detectionTimer = memoryTime;
            ai.SetPlayerPosition(playerPosition);
            ai.SetPlayerDetected(true);
        }
        else if(detectionTimer >= 0)
        {
            ai.SetPlayerPosition(playerPosition);
            ai.SetPlayerDetected(true);
        }
        else
        {
            ai.SetPlayerDetected(false);
        }


        currentHealth = selfHealth.CurrentHealth();
        detectionTimer -= Time.deltaTime;
    }
}
