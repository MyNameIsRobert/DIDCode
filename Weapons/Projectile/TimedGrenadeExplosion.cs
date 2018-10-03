using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedGrenadeExplosion : ExplosiveProjectile {
    public float explosionTimer = 2f;
    public GameObject explosion;
    bool showHitmarker;
    public AudioClip explosionSound;
    // Use this for initialization

    private void Awake()
    {
        explosiveType = ExplosiveType.Timed;
    }
    IEnumerator TimedExplosion()
    {
        yield return new WaitForSeconds(explosionTimer);

        GameObject explosionClone = Instantiate(explosion, transform.position, Quaternion.LookRotation(transform.up));
        explosionClone.SendMessage("PlaySound", explosionSound);
        Destroy(explosionClone, 2f);

        GameObject[] enemies = AreaSearch.FindAllInRadiusWithTag(transform.position, blastRadius, "Enemy"); 
        
        bool[] lineOfSight = new bool[enemies.Length];
        if (enemies != null) 
            lineOfSight = AreaSearch.CheckInLineOfSight(transform.position, enemies);

        showHitmarker = false;
        int count = 0;
        foreach (GameObject i in enemies)
        {
            if (lineOfSight[count])
            {
                health = i.transform.gameObject.GetComponent<HealthSinglePlayer>();
                health.TakeDamage((int)((damage * 7.5) / Vector3.Distance(transform.position, i.transform.position)));
                showHitmarker = true;
            }
            count++;
        }
        if (showHitmarker)
            playerProperties.ShowHitmarker(false, .2f);

        Destroy(gameObject);
        yield return null;
    }
	void Start () {
        StartCoroutine(TimedExplosion());

	}
	

    
	// Update is called once per frame
	void Update () {
	}



    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
    public void SetExplosionTimer(float newTimer)
    {
        explosionTimer = newTimer;
    }
    public void SetBlastRadius(float newRadius)
    {
        blastRadius = newRadius;
    }
    public void SetPlayerProperties(PlayerProperties newPlayerPropterties)
    {
        playerProperties = newPlayerPropterties;
    }


}
