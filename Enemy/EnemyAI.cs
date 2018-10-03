using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour {

	public bool playerDetected;

    public bool willWander;
    public float wanderDistance;
    public float wanderTime;
    
    public float attackDistance = 1f;
    [HideInInspector]
    public float attackTimer = 1f;
    public float attackTimerSet;
    [SerializeField]
    public float attackDamage = 10;

    public float playerAgroRadius = 20f;

    public bool shootingRangeMode = false;

	public NavMeshAgent agent;
    [SerializeField]
    public HealthSinglePlayer selfHealth;
    [HideInInspector]
    public HealthSinglePlayer playerHealth;
    [HideInInspector]
    public Transform playerPostion;
    [HideInInspector]
    public Animator anim;

    GameManager parentManager;

    int dumbCounter;

    List<PlayerDetection> detectionObjects = new List<PlayerDetection>();

    [HideInInspector]
    public float wanderTimer;
    // Use this for initialization
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(selfHealth == null)
            selfHealth = GetComponent<HealthSinglePlayer>();
        anim = GetComponent<Animator>();
        attackTimer = attackTimerSet;
        willWander = true;
        TriggerPlayerDetection();
    }
    public void AddDetectionObject(PlayerDetection detection)
    {
        detectionObjects.Add(detection);
    }
    // Update is called once per frame
    public virtual void Update () {
        if (selfHealth.GetIsDead())
        {
                anim.SetBool("Death", true);
                Destroy(gameObject, .3f);              
        }
        else
        {
            if (playerDetected && playerPostion != null)
            {
                Vector3 toPlayer = playerPostion.transform.position - transform.position;
                bool isWithinDistance = Vector3.Distance(transform.position, playerPostion.transform.position) <= attackDistance;
                bool isTooClose = Vector3.Distance(transform.position, playerPostion.transform.position) <= attackDistance / 3;
                //Debug.Log(Vector3.Distance(transform.position, playerPostion.transform.position));
                //Debug.Log("isWithinDistance is " + isWithinDistance);
                if (isTooClose)
                {
                    Vector3 targetPosition = toPlayer.normalized * -3f;
                    agent.destination = targetPosition;
                    anim.SetBool("Walking", true);
                }
                if (isWithinDistance)
                {

                    agent.isStopped = true;
                    if (attackTimer <= 0 && agent.isStopped)
                    {
                        Attack();   
                    }
                    anim.SetBool("Walking", false);
                }
                else
                {
                    agent.SetDestination(playerPostion.position);
                    anim.SetBool("Walking", true);
                    agent.isStopped = false;
                }
                wanderTimer = -1;
                attackTimer -= Time.deltaTime;
            }
            else if (willWander)
            {
                
                if (wanderTimer <= 0)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, wanderDistance, -1);
                    agent.SetDestination(newPos);
                    wanderTimer = wanderTime;
                }
                if (agent.isStopped)
                    anim.SetBool("Walking", false);
                else
                {
                    anim.SetBool("Walking", true);
                    wanderTimer -= Time.deltaTime;
                }
            }
            else
            {
                anim.SetBool("Walking", false);
            }

            attackTimer -= Time.deltaTime;
        }
        wanderTimer -= Time.deltaTime;
	}

    public void SetPlayerDetected(bool set) {
        playerDetected = set;
    }

    public void SetPlayerPosition(Transform pos) {
        playerPostion = pos;
    }

    void MeleeAttack(float damage, HealthSinglePlayer health) {
        if (attackTimer <= 0f)
        {
            Debug.Log("Attack!");
            health.TakeDamage(damage); 
            attackTimer = attackTimerSet;
            anim.SetTrigger("Attack");
        }
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layerMask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, dist, layerMask);

        return navHit.position;
    }
    public void TriggerPlayerDetection()
    {
        if (!willWander)
        {
            willWander = true;
            return;
        }
        if (shootingRangeMode)
            return;
        GameObject[] closePlayers = AreaSearch.FindAllInRadiusWithTag(transform.position, playerAgroRadius, "Player");
        Debug.Log("Number of close players: " + closePlayers.Length);
        bool[] isInLineOfSight = AreaSearch.CheckInLineOfSight(transform.position, closePlayers);
        for(int i = 0; i < isInLineOfSight.Length; i++)
        {
            if (isInLineOfSight[i])
            {
                SetPlayerDetected(true);
                SetPlayerPosition(closePlayers[i].transform);
                for(int j  = 0; j < detectionObjects.Count; j++)
                {
                    detectionObjects[j].playerDetected = true;
                    detectionObjects[j].playerProperties = closePlayers[i].transform.GetComponent<PlayerProperties>();
                }
                Debug.Log(closePlayers[i] + " is in line of sight");
                break;
            }
            else
            {
                Debug.Log(closePlayers[i] + " is not in line of sight");
            }
        }
    }
    public void TriggerPlayerDetection(Transform player)
    {

    }

    public void SetManager(GameManager manager)
    {
        parentManager = manager;
    }

    public virtual void Attack()
    {
        anim.SetTrigger("Attack");
        anim.SetBool("Walking", false);
        playerHealth = playerPostion.transform.root.gameObject.GetComponent<HealthSinglePlayer>();
        playerHealth.TakeDamage(attackDamage);
        attackTimer = attackTimerSet;
    }
}
