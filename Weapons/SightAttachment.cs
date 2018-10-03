using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightAttachment : MonoBehaviour {

    Shoot shootParent;
    int animBoolInt;
    Animator shootAnimator;
    public bool showImageForAim;
    bool renderersDisabled = false;
    public Sprite imageToShow;
	// Use this for initialization
	void Start () {
        shootParent = FindShootParent(transform);
        if(shootParent != null && !shootParent.isPickup)
        {
            shootAnimator = shootParent.anim;
            animBoolInt = Animator.StringToHash("SightEquipped");
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(!shootParent.isPickup)
        { //Setting the "SightEquipped" parameter to true in the weapon's animator, in case the weapon has special animations for aiming with a sight equipped
            shootAnimator.SetBool(animBoolInt, true);
            if(shootParent.aiming && showImageForAim && shootParent.isADS)
            {
                shootParent.DisableRenderers();
                renderersDisabled = true;
                shootParent.playerProperties.ToggleScopeReticle(true, imageToShow);
            }
            else if(!shootParent.aiming && renderersDisabled)
            {
                shootParent.EnableRenderers();
                renderersDisabled = false;
                shootParent.playerProperties.ToggleScopeReticle(false, null);
            }
        }
		
	}

    public Shoot FindShootParent(Transform start)
    {
        Shoot tempShoot;
        if(start.GetComponent<Shoot>())
        {
            tempShoot = start.GetComponent<Shoot>();
            return start.GetComponent<Shoot>();
        }
        else
        {
            if (start.parent != null)
                tempShoot = FindShootParent(start.parent);
            else
                return null;
        }
        return tempShoot;
    }
}
