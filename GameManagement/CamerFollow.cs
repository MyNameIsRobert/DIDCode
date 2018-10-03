using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CamerFollow : MonoBehaviour {

	public GameObject player;       //Public variable to store a reference to the player game object

	public float offsetX;
	public float offsetY;
	public float offsetZ;
	private Vector3 offset;         //Private variable to store the offset distance between the player and camera

	// Use this for initialization
	void Start () 
	{
		//Calculate and store the offset value by getting the distance between the player's position and camera's position.
		offset = new Vector3(offsetX,offsetY,offsetZ);
	}

	// LateUpdate is called after Update each frame
	void LateUpdate () 
	{
//		// Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
//		transform.position = player.transform.position + offset;
//		transform.rotation = player.transform.rotation;
	}
}