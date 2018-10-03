using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerControllerSinglePlayer))]
[RequireComponent(typeof(PlayerProperties))]
public class Crouch : MonoBehaviour {

    Camera cam;
    PlayerProperties playerProperties;
    [SerializeField]
    Transform crouchPosition;

    Vector3 endPostion;
    PlayerInputControls control;
    Vector3 camDefaultPosition;
	// Use this for initialization
	void Start () {
        playerProperties = GetComponent<PlayerProperties>();
        cam = playerProperties.cam;
        camDefaultPosition = cam.transform.localPosition;
        control = GetComponent<PlayerInputControls>();
	}
	
	// Update is called once per frame
	void Update () {
        endPostion = crouchPosition.transform.localPosition;
        Vector3 startPosition = cam.transform.localPosition;

        if (Input.GetKey(control.crouch))
        {

            cam.transform.localPosition = Vector3.Lerp(startPosition, endPostion, .5f);
            playerProperties.SetCrouching(true);
        }
        else
        {

            cam.transform.localPosition = Vector3.Lerp(startPosition, camDefaultPosition, .5f);
            playerProperties.SetCrouching(false);
        }


    }
}
