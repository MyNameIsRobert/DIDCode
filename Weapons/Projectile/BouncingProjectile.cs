using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingProjectile : Projectile {

    public int numberOfBounces = 3;
    int currentBounces = 0;
    public bool canCritHit = false;
    public bool perfectReflect = true;
    public float rayCastWidth = 1;
    Vector3 reflectionAngle = Vector3.zero;
    Rigidbody rigidbody;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Destroy(gameObject, 30);   
    }
    private void Update()
    {
        RaycastHit hit;
        Vector3 tempVector = new Vector3(transform.position.x - rayCastWidth, transform.position.y, transform.position.z);
        Vector3 tempVector2 = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 tempVector3 = new Vector3(transform.position.x + rayCastWidth, transform.position.y, transform.position.z);
        Vector3 movementDirection = transform.InverseTransformDirection(rigidbody.velocity);
        Ray[] rays = { new Ray(tempVector, movementDirection), new Ray(tempVector2, movementDirection), new Ray(tempVector3, movementDirection) };
        foreach(Ray r in rays)
        {
            Debug.DrawRay(r.origin, r.direction);
            if(Physics.Raycast(r, out hit))
            {
                if(!hit.collider.CompareTag("Enemy"))
                {
                    reflectionAngle = Vector3.Reflect(r.direction, hit.normal);
                   // Debug.Log("In angle = " + r.direction + " and out angle = " + reflectionAngle);
                }
            }
        }
    }
    IEnumerator RotateToReflectionAngle()
    {
        while(transform.rotation != Quaternion.Euler(reflectionAngle))
        {
            Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(reflectionAngle), 10 * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Enemy Body") || collision.transform.CompareTag("Enemy Head"))
        {
            Debug.Log("Hit enemy!");
            if(collision.transform.root.GetComponent<HealthSinglePlayer>())
            {
                health = collision.transform.root.GetComponent<HealthSinglePlayer>();
                DoDamage();
                Destroy(gameObject);
            }
            return;
        }
        if (collision.transform.root.CompareTag("Bullet"))
            return;
            
        if(currentBounces > numberOfBounces)
        {
            Destroy(gameObject);
        }
        //ContactPoint cp = collision.contacts[0];
        //Vector3 oldVelocity = rigidbody.velocity;
        //rigidbody.velocity = Vector3.Reflect(oldVelocity, cp.normal);
        //StartCoroutine(RotateToReflectionAngle());
        currentBounces++;
    }

    protected override void DoDamage()
    {
        base.DoDamage();
    }
}
