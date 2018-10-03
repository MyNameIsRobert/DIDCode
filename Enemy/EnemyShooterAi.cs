using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//When attack is called, this version of the AI shoots a projectile  or a hitscan at the player
public class EnemyShooterAi : EnemyAI
{
    [Header("Shooter AI")]
    [Tooltip("Higher is Better")]
    [Range(1, 20)]
    public float shootingAccuracy = 1;
    public Transform shootFrom;
    public GameObject objectToShoot;
    public float projectileSpeed = 20;
    public bool projectile;
    bool lookingTowardsPlayer_Coroutine;

    IEnumerator LookTowardsPlayer(Transform playerPos)
    {

        lookingTowardsPlayer_Coroutine = true;
        //Debug.Log(Vector3.Dot(transform.forward, (playerPos.position - transform.position).normalized));

        while (Vector3.Dot(transform.forward, (playerPos.position - transform.position).normalized) < .98f)
        {
            Debug.Log("Rotating towards player");
            Vector3 direction = (playerPos.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2);
            yield return null;
        }
        lookingTowardsPlayer_Coroutine = false;
        yield return null;
    }
    public override void Attack()
    {
        if (!lookingTowardsPlayer_Coroutine)
            StartCoroutine(LookTowardsPlayer(playerPostion));
        anim.SetBool("Walking", false);
        anim.SetBool("Attacking", true);
        Debug.Log("Calling shoot attack");
        Vector3 fireDirection = shootFrom.transform.forward;
        Quaternion fireRotation = Quaternion.LookRotation(fireDirection);
        Quaternion randomRotation = Random.rotation;

        fireRotation = Quaternion.RotateTowards(fireRotation, randomRotation, Random.Range(0f, 1 / shootingAccuracy));

        Ray fireRay = new Ray(shootFrom.position, fireRotation * Vector3.forward);
        Debug.DrawRay(fireRay.origin, fireRay.direction);
        anim.SetTrigger("Attack");
        if (projectile)
        {
            GameObject fired = Instantiate(objectToShoot, shootFrom.position, shootFrom.rotation);
            if (!fired.GetComponent<Rigidbody>())
                fired.AddComponent<Rigidbody>();
            Rigidbody body = fired.GetComponent<Rigidbody>();
            body.AddForce(projectileSpeed * fireRay.direction, ForceMode.Impulse);
            fired.GetComponent<Projectile>().damage = attackDamage;
            fired.GetComponent<ProjectileEnemy>().shooterAi = this;
            Destroy(fired, 10);
        }
        else
        {
            RaycastHit hit;
            if(Physics.Raycast(fireRay, out hit))
            {
                if(hit.transform.root.CompareTag("Player"))
                {
                    Debug.Log("Enemy Shot Player");
                    playerHealth = hit.transform.root.GetComponent<HealthSinglePlayer>();
                    playerHealth.TakeDamage(attackDamage);
                }
            }
        }

        attackTimer = attackTimerSet;
    }
}

