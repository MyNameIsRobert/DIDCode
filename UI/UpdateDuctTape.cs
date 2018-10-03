using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateDuctTape : MonoBehaviour {

	public GameObject player;
    PlayerProperties properties;
	Text text;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
        player = transform.root.gameObject;
        properties = player.GetComponent<PlayerProperties>();
	}

	// Update is called once per frame
	void Update () {
        text.text = properties.ductTape.ToString();
	}
}