using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCloseCollider : MonoBehaviour {


    public Door doorParent;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        doorParent.SetCloseCollider(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        doorParent.SetCloseCollider(false);
    }
}
