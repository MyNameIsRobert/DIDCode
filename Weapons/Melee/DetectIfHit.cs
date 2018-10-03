using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectIfHit : MonoBehaviour {


    public MeleeAttack attackParent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Enemy Body"))
        {
            attackParent.InitiallyHitEnemy(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Enemy Body"))
        {
            attackParent.StayingInEnemy(other);
        }
    }

    public void SetMeleeAtackParent(MeleeAttack meleeAttack)
    {
        attackParent = meleeAttack;
    }
}
