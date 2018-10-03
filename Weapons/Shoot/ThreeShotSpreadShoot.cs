using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeShotSpreadShoot : Shoot {

    [Header("Three Shot Spread")]
    public float threeShotSpread = 1;
    public bool isProjectile = false;
    public bool vertical = false;
    public override void Update()
    {
        base.Update();

        if(canFireAuto)
        {
            currentMagazine--;
            anim.SetTrigger("Shoot");
            anim.SetTrigger("Fire");

            //Projectile
            if(isProjectile)
            {

            }
            //Raycast/Hitscan
            else
            {
                //Instantiating 3 rays
                Ray[] rays = new Ray[3];
                Vector3 cameraRotationVector = cam.transform.rotation.eulerAngles;
                //Setting each ray to their respective orientation
                rays[1] = new Ray(cam.transform.position, cameraRotationVector);
                Vector3 tempVector = new Vector3(cameraRotationVector.x - threeShotSpread, cameraRotationVector.y, cameraRotationVector.z);
                rays[0] = new Ray(cam.transform.position, tempVector);
                tempVector = new Vector3(cameraRotationVector.x + threeShotSpread, cameraRotationVector.y, cameraRotationVector.z);
                rays[2] = new Ray(cam.transform.position, cameraRotationVector);

                for(int i = 0; i < 3; i++)
                {
                    RaycastHit hit;
                    if(Physics.Raycast(rays[i], out hit))
                    {
                        if(hit.collider.CompareTag("Enemy Body"))
                        {
                            HealthSinglePlayer health = hit.collider.transform.root.GetComponent<HealthSinglePlayer>();
                            health.TakeDamage(damage);
                        }
                        else if(hit.collider.CompareTag("Enemy Head"))
                        {
                            HealthSinglePlayer health = hit.collider.transform.root.GetComponent<HealthSinglePlayer>();
                            health.TakeDamage(damage * critMultiplier);
                            playerProperties.ShowHitmarker(true, damage * critMultiplier);
                        }
                    }
                }
            }
        }
    }
}
