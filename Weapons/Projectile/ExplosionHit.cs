using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHit : ExplosiveProjectile {

    public GameObject explosion;
    public AudioClip explosionSound;
    bool showHitmarker;

    private void Awake()
    {
        explosiveType = ExplosiveType.OnCollide;
    }
    private void OnCollisionEnter(Collision col)
    {
        if (!col.gameObject.CompareTag("Bullet") && !col.gameObject.transform.root.CompareTag("Player"))
        {
            GameObject explosionClone = Instantiate(explosion, transform.position, transform.rotation);
            explosionClone.SendMessage("PlaySound", explosionSound);
            Destroy(explosionClone, 15f);
            GameObject[] enemies = AreaSearch.FindAllInRadiusWithTag(transform.position, blastRadius, "Enemy");
            GameObject[] players = AreaSearch.FindAllInRadiusWithTag(transform.position, blastRadius/2, "Player");
            showHitmarker = false;
            foreach (GameObject i in enemies)
            {
                if (AreaSearch.CheckInLineOfSight(transform.position, i))
                {
                    health = i.transform.gameObject.GetComponent<HealthSinglePlayer>();
                    health.TakeDamage((int)(damage / (Vector3.Distance(transform.position, i.transform.position)) / 2 ));
                    showHitmarker = true;
                }
            }
            foreach (GameObject i in players)
            {
                if(AreaSearch.CheckInLineOfSight(transform.position, i))
                {
                    health = i.transform.gameObject.GetComponent<HealthSinglePlayer>();
                    health.TakeDamage((damage / 3) / (Vector3.Distance(transform.position, i.transform.position)) / 2 );
                }
            }
            if (showHitmarker)
                playerProperties.ShowHitmarker(false, .2f);
            Destroy(gameObject);
        }

    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
    public void SetBlastRadius(float newRadius)
    {
        blastRadius = newRadius;
    }
    public void SetPlayerProperties(PlayerProperties newPlayerProperties)
    {
        playerProperties = newPlayerProperties;
        Debug.Log(playerProperties);
    }
}
