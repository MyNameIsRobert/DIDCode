using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour {


    [SerializeField]
    Quest[] quests;
    [SerializeField]
    float firstQuestDelayInSeconds = 2;
    int currentQuest = 0;
    bool fadeRoutineRunning = false;
    Coroutine fadeCoroutine;
    [SerializeField]
    List<Quest> previouslyDisplayedQuests;
    
    GameManager gameManager;

    IEnumerator StartQuesting()
    {
        yield return new WaitForSeconds(firstQuestDelayInSeconds);
        ActivateQuest(currentQuest);
        yield return null;
    }
	void Start () {
        gameManager = transform.parent.GetComponent<GameManager>();
        if(quests == null)
        {
            quests = transform.GetComponentsInChildren<Quest>();
        }
        else if(quests.Length < transform.childCount)
        {
            quests = transform.GetComponentsInChildren<Quest>();

        }
        StartCoroutine(StartQuesting());
	}

    // Update is called once per frame
    void Update () {
        if(currentQuest >= quests.Length)
        {
            return;
        }
		if(quests[currentQuest].collider.enabled == false)
        {
            if(quests[currentQuest].lastQuest)
            {
                FinishQuesting();
                return;
            }
            currentQuest++;
            ActivateQuest(currentQuest);
        }
	}
     IEnumerator FadeCanvas(CanvasGroup group, float time)
    {
        fadeRoutineRunning = true;
    
        yield return new WaitForSeconds(time);
        while(group.alpha > 0)
        {
            group.alpha -= Time.deltaTime;
            yield return null;
        }
        fadeRoutineRunning = false;
        yield return null;
    }

    void ActivateQuest(int questNumber)
    {
        Debug.Log("Activate quest called on quest " + questNumber);
        quests[questNumber].collider.enabled = true;
        if(questNumber > 0)
        previouslyDisplayedQuests.Add(quests[questNumber - 1]);
        for(int i = 0; i < gameManager.playerQuestUI.Length; i++)
        {
            Debug.Log("Setting the quest on " + gameManager.playerQuestUI[i].transform.root.name + "s " + gameManager.playerQuestUI[i].name + " object");
            gameManager.playerQuestUI[i].transform.GetChild(0).GetComponent<Text>().text = quests[questNumber].questTitle;
            gameManager.playerQuestUI[i].transform.GetChild(1).GetComponent<Text>().text = quests[questNumber].questDescription;
            gameManager.playerQuestUI[i].GetComponent<CanvasGroup>().alpha = 1;
            gameManager.playerPreviousQuestDisplays[i].MoveToNextQuest(quests[questNumber]);

            if(quests[questNumber].questDescription.Length > 250)
            {
                gameManager.playerQuestUI[i].transform.GetChild(1).GetComponent<Text>().fontSize = 14;
            }
            else
            {
                gameManager.playerQuestUI[i].transform.GetChild(1).GetComponent<Text>().fontSize = 18;
            }
            if(!fadeRoutineRunning)
                fadeCoroutine = StartCoroutine(FadeCanvas(gameManager.playerQuestUI[i].GetComponent<CanvasGroup>(), quests[questNumber].questDisplayLength));
            else
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeCanvas(gameManager.playerQuestUI[i].GetComponent<CanvasGroup>(), quests[questNumber].questDisplayLength));
            }
        }
    }

    void FinishQuesting()
    {

    }
}
