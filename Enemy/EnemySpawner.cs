using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class EnemySpawner : MonoBehaviour {

    [SerializeField]
    GameObject[] enemyArray;
    public float spawnDelay;

    public float delayBetweenSpawns;

    [SerializeField]
    float betweenDelayTimer;

    public bool willWander;

    public float walkSpeed;

    public float attackRange;

    public bool destroyOnDeath = true;

    public bool shootingRangeMode = false;

    public bool spawnOnCollider = false;

    bool startedSpawning = false;

    [Tooltip("The tag the collider looks for to trigger the spawning event")]
    public string spawnColliderTagRequired = "Player";

	Vector3 origin;

    BoxCollider spawnCollider;

    GameManager parentManager;


	// Update is called once per frame
	void Start(){
        spawnCollider = GetComponent<BoxCollider>();
        if(!spawnOnCollider)
            StartCoroutine(SpawnEnemies());
	}

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(spawnDelay);

        for (int i = 0; i < enemyArray.Length; i++)
        {
            Vector3 spawnPostion = new Vector3(Random.Range(spawnCollider.bounds.min.x, spawnCollider.bounds.max.x), transform.position.y, Random.Range(spawnCollider.bounds.min.z, spawnCollider.bounds.max.z));
            GameObject tempEnemy = Instantiate(enemyArray[i], spawnPostion, transform.rotation);
            tempEnemy.GetComponent<EnemyAI>().willWander = willWander;
            tempEnemy.GetComponent<EnemyAI>().shootingRangeMode = shootingRangeMode;
            if(attackRange != -1)
                tempEnemy.GetComponent<EnemyAI>().attackDistance = attackRange;
            tempEnemy.GetComponent<HealthSinglePlayer>().destroyOnDeath = destroyOnDeath;
            tempEnemy.GetComponent<NavMeshAgent>().speed = walkSpeed;
            if(parentManager != null)
            parentManager.AddSpawnedEnemy(tempEnemy.GetComponent<EnemyAI>());
            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!startedSpawning)
            if (other.CompareTag(spawnColliderTagRequired))
            {
                StartCoroutine(SpawnEnemies());
                startedSpawning = true;
            }
    }

    public void SetManager(GameManager newManager)
    {
        parentManager = newManager;
    }
}
