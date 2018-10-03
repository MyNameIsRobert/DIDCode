using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject[] players;
    public GameObject[] playerQuestUI;
    [HideInInspector]
    public PreviousQuestDisplay[] playerPreviousQuestDisplays;
    public EnemySpawner[] enemySpawners;
    [SerializeField]
    List<EnemyAI> enemies;
    List<GameObject> lingeringBulletDecals = new List<GameObject>();
    public int maxNumberOfBulletDecals = 30;
    public float chanceForDecalToLinger_Percent = .5f;
    float actualChanceForDecalToLinger;
    [MinMaxRange(1, 60)]
    public MinMaxRange decalDestroyTime;


    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        enemySpawners = Component.FindObjectsOfType<EnemySpawner>();
        playerQuestUI = new GameObject[players.Length];
        playerPreviousQuestDisplays = new PreviousQuestDisplay[players.Length];
        for(int i = 0; i < playerQuestUI.Length; i++)
        {
            playerQuestUI[i] = players[i].transform.Find("SinglePlayer UI/Quest").gameObject;
            playerPreviousQuestDisplays[i] = players[i].transform.Find("SinglePlayer UI/Previous Quests").GetComponent<PreviousQuestDisplay>();
        }
        foreach(EnemySpawner s in enemySpawners)
        {
            s.SetManager(this);
        }
        actualChanceForDecalToLinger = chanceForDecalToLinger_Percent / 100;
    }

    public void AddSpawnedEnemy(EnemyAI enemyToAdd)
    {
        enemies.Add(enemyToAdd);
        enemyToAdd.SetManager(this);
    }

    public void EnemyDied(EnemyAI deadEnemy)
    {
        enemies.Remove(deadEnemy);
    }
    public void EnemyDied(EnemyAI deadEnemy, PlayerProperties playerThatKilled)
    {
        enemies.Remove(deadEnemy);
    }
    
    IEnumerator FadeDecalThenDestroy(GameObject obj, float fadeDelay)
    {
        yield return new WaitForSeconds(fadeDelay);
        SpriteRenderer decalImage = obj.GetComponentInChildren<SpriteRenderer>();
        while(decalImage.color.a > 0)
        {
            Color tempColor = decalImage.color;
            tempColor.a -= Time.deltaTime;
            decalImage.color = tempColor;
            yield return null;
        }
        Destroy(obj);
        yield return null;
    }
    public void AddDecal(GameObject obj)
    {
        if(Random.value < actualChanceForDecalToLinger)
        {
            Debug.Log("Decal Will Linger!");
            lingeringBulletDecals.Add(obj);
            if (lingeringBulletDecals.Count > maxNumberOfBulletDecals)
                lingeringBulletDecals.RemoveAt(0);
        }
        else
        {
            StartCoroutine(FadeDecalThenDestroy(obj, decalDestroyTime.GetRandomValue()));
            Debug.Log("Decal Will NOT Linger");
        }
    }


    
}




