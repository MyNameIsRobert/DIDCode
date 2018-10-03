using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviousQuestDisplay : MonoBehaviour {
    [SerializeField]
    Quest currentQuest;
    [SerializeField]
    List<Quest> previousQuests;
    [SerializeField]
    GameObject questPanelToSpawn;
    [SerializeField]
    GameObject contentObject;
    GameObject currentQuestPanel;


    List<Button> questButtons = new List<Button>();
    [SerializeField]
    List<Text> titleTexts = new List<Text>();
    [SerializeField]
    List<Text> descripTexts = new List<Text>();

	// Use this for initialization
	void Start () {
        currentQuestPanel = contentObject.transform.GetChild(1).gameObject;
        questButtons.Add(contentObject.transform.GetChild(3).GetComponent<Button>());
        titleTexts.Add(contentObject.transform.GetChild(3).GetChild(0).GetComponent<Text>());
        descripTexts.Add(contentObject.transform.GetChild(3).GetChild(1).GetComponent<Text>());

	}
	
	// Update is called once per frame
	void Update () {
		if(currentQuest != null)
        {
            currentQuestPanel.transform.GetChild(0).GetComponent<Text>().text = currentQuest.questTitle;
            currentQuestPanel.transform.GetChild(1).GetComponent<Text>().text = currentQuest.questDescription;
            if(currentQuest.questDescription.Length > 100)
            {
                currentQuestPanel.transform.GetChild(1).GetComponent<Text>().fontSize = 14;
            }
            else
            {
                currentQuestPanel.transform.GetChild(1).GetComponent<Text>().fontSize = 18;
            }
        }
        else
        {
            currentQuestPanel.transform.GetChild(0).GetComponent<Text>().text = "No Current Quest!";
            currentQuestPanel.transform.GetChild(1).GetComponent<Text>().text = "There is nothing for you to do";
        }
        if(previousQuests.Count > titleTexts.Count)
        {
            SpawnPreviousQuestPanel();
        }
        if (previousQuests != null)
            for (int i = previousQuests.Count - 1; i >= 0; i--) //The most recently completed quest will be last in the list, so we want to work backwards
            {
                titleTexts[(previousQuests.Count - 1) - i].text = previousQuests[i].questTitle; //We want to use the arrays in reverse order
                descripTexts[(previousQuests.Count - 1) - i].text = previousQuests[i].questDescription;
                if(previousQuests[i].questDescription.Length > 100)
                {
                    descripTexts[(previousQuests.Count - 1) - i].fontSize = 14;
                }
                else
                {
                    descripTexts[(previousQuests.Count - 1) - i].fontSize = 18;
                }
            }
        else
        {
            for (int i = previousQuests.Count - 1; i >= 0; i--) 
            {
                titleTexts[(previousQuests.Count - 1) - i].text = ""; 
                descripTexts[(previousQuests.Count - 1) - 1].text = "";
            }
        }
	}

    public void MoveToNextQuest(Quest newQuest)
    {
        if(currentQuest != null)
            previousQuests.Add(currentQuest);
        currentQuest = newQuest;
    }

    public void SpawnPreviousQuestPanel()
    {
        GameObject tempSpawned = Instantiate(questPanelToSpawn);
        tempSpawned.transform.SetParent(contentObject.transform, false);
        tempSpawned.transform.localScale = new Vector3(1, 1, 1);
        titleTexts.Add(tempSpawned.transform.GetChild(0).GetComponent<Text>());
        descripTexts.Add(tempSpawned.transform.GetChild(1).GetComponent<Text>());
    }
}
