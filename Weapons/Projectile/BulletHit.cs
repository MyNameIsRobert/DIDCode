using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHit : Projectile {

    [SerializeField]
    bool destroyOnCollision = false;
    [SerializeField]
    GameObject hitExplosion;
    [SerializeField]
    Rigidbody rBody;

    [SerializeField]
    bool canHeadshot = true;
    
    private void Awake()
    {
        projectileType = Type.Bullet;
    }

    private void OnCollisionEnter(Collision col)
    {
        bool hitHead = false;
        if (!col.gameObject.CompareTag("Bullet") && !col.gameObject.transform.root.CompareTag("Player"))
        {
            if (col.transform.root.GetComponent<HealthSinglePlayer>())
            {
                hitHead = col.transform.CompareTag("Enemy Head");
                health = col.gameObject.transform.root.gameObject.GetComponent<HealthSinglePlayer>();
                Debug.Log("Collided with " + col.transform.name + " and hitHead is " + hitHead);

                if (hitHead && canHeadshot)
                {
                    
                    health.TakeDamage(damage * critHitDamage);
                    playerProperties.ShowHitmarker(true, (int)damage * critHitDamage);
                }
                else
                {
                    health.TakeDamage(damage);
                    playerProperties.ShowHitmarker(false, (int)damage);
                }

                if (hitExplosion != null)
                {
                    GameObject tempExplosion = Instantiate(hitExplosion, transform.position, transform.rotation);
                    Destroy(tempExplosion, 10f);
                    
                }
                    playerProperties.ShowHitmarker(false, (int)damage);
                    Destroy(gameObject);
            }
            else if (col.transform.root.gameObject.GetComponent<HealthSinglePlayer>())
            {
                hitHead = col.transform.CompareTag("Enemy Head");
                health = col.transform.root.gameObject.GetComponent<HealthSinglePlayer>();
                Debug.Log("Collided with " + col.transform.name + " and hitHead is " + hitHead);
                if (hitHead && canHeadshot)
                {

                    health.TakeDamage(damage * critHitDamage);
                    playerProperties.ShowHitmarker(true, (int)damage * critHitDamage);
                }
                else
                {
                    health.TakeDamage(damage);
                    playerProperties.ShowHitmarker(false, (int)damage);
                }
                if (hitExplosion != null)
                {
                    GameObject tempExplosion = Instantiate(hitExplosion, transform.position, transform.rotation);
                    Destroy(tempExplosion, 10f);
                    
                }
                
                Destroy(gameObject);
            }
            if (!destroyOnCollision)
            {
                rBody.isKinematic = true;
                Destroy(gameObject, 5f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        bool hitHead = false;
        if (!col.gameObject.CompareTag("Bullet") && !col.gameObject.transform.root.CompareTag("Player") && !col.gameObject.layer.Equals("Ignore Raycast"))
        {
            if (col.transform.root.GetComponent<HealthSinglePlayer>())
            {
                hitHead = col.transform.CompareTag("Enemy Head");
                health = col.gameObject.transform.root.gameObject.GetComponent<HealthSinglePlayer>();
                Debug.Log("Triggered by " + col.transform.name + " and hitHead is " + hitHead);
                if (hitHead && canHeadshot)
                {

                    health.TakeDamage(damage * critHitDamage);
                    playerProperties.ShowHitmarker(true, (int)damage * critHitDamage);
                }
                else
                {
                    health.TakeDamage(damage);
                    playerProperties.ShowHitmarker(false, (int)damage);
                }
                if (hitExplosion != null)
                {
                    GameObject tempExplosion = Instantiate(hitExplosion, transform.position, transform.rotation);
                    Destroy(tempExplosion, 10f);

                }
                
                Destroy(gameObject);
            }
            else if (col.transform.root.gameObject.GetComponent<HealthSinglePlayer>())
            {
                hitHead = col.transform.CompareTag("Enemy Head");
                health = col.transform.root.gameObject.GetComponent<HealthSinglePlayer>();
                Debug.Log("Triggered by " + col.transform.name + " and hitHead is " + hitHead);
                if (hitHead && canHeadshot)
                {

                    health.TakeDamage(damage * critHitDamage);
                    playerProperties.ShowHitmarker(true, (int)damage * critHitDamage);
                }
                else
                {
                    health.TakeDamage(damage);
                    playerProperties.ShowHitmarker(false, (int)damage);
                }
                if (hitExplosion != null)
                {
                    GameObject tempExplosion = Instantiate(hitExplosion, transform.position, transform.rotation);
                    Destroy(tempExplosion, 10f);

                }
                
                Destroy(gameObject);
            }
            Destroy(gameObject, 5f);
        }
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
    public void SetBlastRadius(float newRadius)
    {
    }
    public void SetPlayerProperties(PlayerProperties newPlayerProperties)
    {
        playerProperties = newPlayerProperties;
    }
}
