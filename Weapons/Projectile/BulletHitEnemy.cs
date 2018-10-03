using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitEnemy : ProjectileEnemy {

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy Body") || other.CompareTag("Enemy Head") || other.CompareTag("Bullet"))
        {
            Debug.Log(other.tag);
            return;
        }
        else if(!other.CompareTag("Player"))
        {
            Debug.Log(other.tag);
            Destroy(gameObject);
            return;
        }
        if(other.transform.root.GetComponent<HealthSinglePlayer>())
        {
            health = other.transform.root.GetComponent<HealthSinglePlayer>();
            health.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

}
