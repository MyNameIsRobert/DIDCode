using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GunAimUpSinglePlayer : MonoBehaviour {
	public float maxRotation = 0.3826835f;
	public float minRotation = -0.3826835f;

	public GameObject gun;
	public float xRotation;
	public float mouseInput;
	public bool reachedMax = false;
	public bool reachedMin = false;
	public float rotationSpeed;

    private PlayerControllerSinglePlayer controller;

	// Use this for initialization
	void Start () {
        controller = gameObject.GetComponent<PlayerControllerSinglePlayer>();
	}

	// Update is called once per frame
	void Update () {
        if (Time.timeScale == 0)
            return;
        rotationSpeed = controller.mouseSensitivity;
		WithinBoundary ();
		mouseInput = Input.GetAxis ("Mouse Y");

		//If the gun is within the Maximum and the minimun rotation, allow it to look up and down freely

		if (!reachedMax && !reachedMin)
			gun.transform.Rotate (mouseInput * rotationSpeed, 0, 0);

		
		//If the gun has reached the maximum, then limit the mouse input to only down

		else if (reachedMax) {
			


			//Sets mouseInput to the value of Mouse Y

			//If mouseInput is down, then let it rotate the gun

			if (mouseInput < 0)
				gun.transform.Rotate (mouseInput * rotationSpeed, 0, 0);
			
		//If the gun has reached the minimum, then limit the mouse input to only up

		} else if (reachedMin) {
			

			//Sets mouseInput to the value of Mouse Y
			mouseInput = Input.GetAxis("Mouse Y");
			//If mouseInput is up, then let it rotate the gun
			if (mouseInput > 0)
				gun.transform.Rotate (mouseInput * rotationSpeed, 0, 0);

		}



	}

	private void WithinBoundary(){
		//Sets the local X rotation of the gun to xRotation

		xRotation = gun.transform.localRotation.x;
		//If xRotation is larger than the Max Rotation, set reachedMax to true, else set it to false
		if (xRotation > maxRotation)
			reachedMax = true; 
		else
			reachedMax = false;
		//Repeats for Min Rotation
		if (xRotation < minRotation)
			reachedMin = true;
		else
			reachedMin = false;
		
	}
}
