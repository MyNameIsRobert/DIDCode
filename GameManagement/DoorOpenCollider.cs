using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenCollider : MonoBehaviour {

    [SerializeField]
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
        doorParent.SetOpenCollider(true);
       
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        doorParent.SetOpenCollider(false);
        
    }
}
