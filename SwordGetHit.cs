using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordGetHit : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.root.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit By Player");

        }
    }
}
