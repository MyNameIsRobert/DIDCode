using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCount : MonoBehaviour {

    TextMesh text;
    public HealthSinglePlayer health;
    public GameObject user;
    public TutorialTriggerDoor door;
    // Use this for initialization
    void Start()
    {
        health = user.GetComponent<HealthSinglePlayer>();
        text = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health.deathCounter < 1)
            return;
        text.text = "Number of Deaths: " + health.deathCounter;

        if (health.deathCounter >= 10)
            door.SetSwitch(true);
    }
}
