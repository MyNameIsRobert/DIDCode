using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerDoor : MonoBehaviour {

    bool switchFromWallToDoor;
    [SerializeField]
    GameObject wall;
    [SerializeField]
    GameObject doorWall;
	
	// Update is called once per frame
	void Update () {
        if (switchFromWallToDoor)
        {
            wall.SetActive(false);
            doorWall.SetActive(true);
        }
        else {
            wall.SetActive(true);
            doorWall.SetActive(false);
        }		
	}
    public void SetSwitch(bool indicator) {
        switchFromWallToDoor = indicator;
    }
}
