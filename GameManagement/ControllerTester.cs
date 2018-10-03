using UnityEngine;
using System.Collections;

public class ControllerTester : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame

	void ControllerCheck()
	{

		float dhaxis = Input.GetAxis("Horizontal");
		float dvaxis = Input.GetAxis("Vertical");

		bool xbox_a = Input.GetButton("A");
		bool xbox_b = Input.GetButton("B");
		bool xbox_x = Input.GetButton("X");
		bool xbox_y = Input.GetButton("Y");
		       

		Debug.Log("" + 
				xbox_a  +  xbox_b  +  xbox_x  +  xbox_y  +dhaxis  +  dvaxis
				);
	}

	void Update()
	{
		
	

		ControllerCheck();

			
		}
}
