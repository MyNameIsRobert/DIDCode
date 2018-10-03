using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class MeleeAttack : Weapon
{


    #region Variable Declaration
    [SerializeField]
    float colliderUpTime = .25f;

    [SerializeField]
    public float stayingDamagePerSecond = 25;

    PlayerInputControls control;
    [SerializeField]
    new Collider collider;

    bool initalHit = false;

    public bool canCleave = false;

    HealthSinglePlayer health;

    [SerializeField]
    DetectIfHit detectIfHit;


    bool isAttacking = false;

    public bool swungLeft = false;
    #endregion

    IEnumerator Attack()
    {
        isAttacking = true;
        collider.enabled = true;
        anim.SetTrigger("Swing");
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(colliderUpTime);
        collider.enabled = false;
        initalHit = false;
        isAttacking = false;
        yield return null;
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        if (isPickup)
            return;
        if (collider == null)
            collider = GetComponent<Collider>();

        if(detectIfHit == null)
        {
            detectIfHit = transform.GetComponentInChildren<DetectIfHit>();
        }
        detectIfHit.SetMeleeAtackParent(this);
        control = player.GetComponent<PlayerInputControls>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isPickup)
        {
            return;
        }
        updateAmmo.ChangeAmountObject(ammo.GetComponent<Amount>(), 0);
        if (Input.GetKey(control.fire) && !isAttacking)
        {
            StartCoroutine(Attack());
        }

        if (Input.GetKey(control.fire))
        {
            anim.SetBool("Attacking", true);
        }
        else
        {
            anim.SetBool("Attacking", false);
            
        }

        anim.SetBool("SwungLeft", swungLeft);



    }

    public void InitiallyHitEnemy(Collider other)
    {
        health = other.transform.root.GetComponent<HealthSinglePlayer>();
        health.TakeDamage(damage);
        playerProperties.ShowHitmarker(false, (int)damage);
    }

    public void StayingInEnemy(Collider other)
    {
        health = other.transform.root.GetComponent<HealthSinglePlayer>();
        health.TakeDamage(stayingDamagePerSecond * Time.deltaTime);
        playerProperties.ShowHitmarker(false, false);
    }

    public void CopyFrom(MeleeAttack attack)
    {
        colliderUpTime = attack.colliderUpTime;
        stayingDamagePerSecond = attack.stayingDamagePerSecond;
        canCleave = attack.canCleave;
    }
}
