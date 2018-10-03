using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : Projectile {

    public float blastRadius;
    public ExplosiveType explosiveType;

    public enum ExplosiveType
    {
        Timed,
        OnCollide,
        Proximity
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
