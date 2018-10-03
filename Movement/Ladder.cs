using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {
	LadderClimb ladder;
	void OnTriggerEnter(Collider other){
		ladder = other.GetComponent<LadderClimb> ();
		ladder.SetTrue ();
	}

	//void onTriggerExit(Collider other){
		//ladder = other.GetComponent<LadderClimb> ();
		//ladder.SetFalse ();

	//}
}
