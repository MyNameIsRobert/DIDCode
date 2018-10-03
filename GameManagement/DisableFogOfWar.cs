using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableFogOfWar : MonoBehaviour {
    [SerializeField]
    GameObject fogOfWar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            fogOfWar.SetActive(false);
        }
    }
}
