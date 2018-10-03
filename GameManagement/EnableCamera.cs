using UnityEngine;
using System.Collections;

public class EnableCamera : MonoBehaviour {
	public GameObject thisCamera;
	// Use this for initialization
	void Update() {
		thisCamera.SetActive(true);
	}
	

}
