using UnityEngine;
using System.Collections;

public class LadderClimb : MonoBehaviour {
	public float speed = 10;
	public bool climb = false;


	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Ladder")) {
			SetTrue ();
			Debug.Log ("Enter Ladder Collider");
		}
	}
	void OnTriggerExit(Collider other){
			if(other.CompareTag("Ladder")){
				SetFalse();
				Debug.Log("Exit Ladder Collider");
			}
		}
	

	void Update(){
		if (climb) {
			if(Input.GetKey(KeyCode.W)){
				transform.Translate (Vector3.up * Time.deltaTime * speed);
			}
			if (Input.GetKey (KeyCode.S)) {
				transform.Translate (Vector3.down * Time.deltaTime * speed);
			}
		}
	
	}
	public void SetTrue(){
		climb = true;
	}
	public void SetFalse(){
		climb = false;
	}

}
