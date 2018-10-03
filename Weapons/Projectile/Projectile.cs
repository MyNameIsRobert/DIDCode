using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

    public float damage;
    public float critHitDamage;
    public HealthSinglePlayer health;
    public PlayerProperties playerProperties;

    public Type projectileType;
    public enum Type
    {
        Explosive,
        Bullet
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    protected virtual void DoDamage()
    {
        health.TakeDamage(damage);
    }
    protected virtual void DoDamage(bool crit)
    {
        if (crit)
            health.TakeDamage(damage * 2);
        else
            health.TakeDamage(damage);
    }
}
