using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CameraDisable : NetworkBehaviour {
	public Camera thisCamera;
	// Use this for initialization
	void Start () {
		if (!isLocalPlayer)
			thisCamera.enabled = false;
	}

}
