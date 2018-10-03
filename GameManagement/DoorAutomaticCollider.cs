using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAutomaticCollider : MonoBehaviour {


    [SerializeField]
    Door parentDoor;

    [SerializeField]
    bool enabled = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;
        if (!other.CompareTag("Player"))
            return;
        parentDoor.OpenDoorAutomatically();
    }
    private void OnTriggerExit(Collider other)
    {
        if (!enabled)
            return;
        if (!other.CompareTag("Player"))
            return;
        parentDoor.CloseDoorAutomatically();
    }
}
