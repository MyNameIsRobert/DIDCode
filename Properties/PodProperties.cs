using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodProperties: MonoBehaviour {

    [SerializeField]
    GameObject topDoorObject;
    [SerializeField]
    GameObject topWallObject;

    [SerializeField]
    GameObject leftDoorObject;
    [SerializeField]
    GameObject leftWallObject;

    [SerializeField]
    GameObject rightDoorObject;
    [SerializeField]
    GameObject rightWallObject;

    [SerializeField]
    GameObject bottomDoorObject;
    [SerializeField]
    GameObject bottomWallObject;
    [SerializeField]
    bool hasBottomDoor;


    public void ActivateTopDoor()
    {
        topDoorObject.SetActive(true);
        topWallObject.SetActive(false);
    }
    public void ActivateLeftDoor()
    {
        leftDoorObject.SetActive(true);
        leftWallObject.SetActive(false);
    }
    public void ActivateRightDoor()
    {
        rightDoorObject.SetActive(true);
        rightWallObject.SetActive(false);
    }
    public void ActivateBottomDoor()
    {
        bottomDoorObject.SetActive(true);
        bottomWallObject.SetActive(false);
    }

    private void Start()
    {
        if (hasBottomDoor)
            ActivateBottomDoor();
    }



}
	
	

