using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickGrenade : MonoBehaviour {


    [SerializeField]
    GameObject grenadeObject;
    [SerializeField]
    GameObject grenadeUIParent;
    [SerializeField]
    Transform grenadeSpawn;

    Image[] grenadeImages = new Image[4];

    [SerializeField]
    float throwStrength = 15;

    [SerializeField]
    float rollStrenth = 5;

    float appliedThrowStrength;

    [SerializeField]
    int currentGrenades = 4;
    [SerializeField]
    [Tooltip("In seconds")]
    float grenadeRegenTime = 3f;

    [SerializeField]
    [Tooltip("In Seconds")]
    float grenadeCoolDown = .35f;

    float lastShot = 0;

    bool coroutingRunning = false;

    PlayerProperties properties;

    IEnumerator RegenGrenades()
    {
        coroutingRunning = true;
        StartCoroutine(DisplayRegeningGrenade(grenadeRegenTime));
        yield return new WaitForSeconds(grenadeRegenTime);
        currentGrenades++;
        coroutingRunning = false;
        yield return null;
    }

    IEnumerator DisplayRegeningGrenade(float startingTime)
    {
        float time = 0.01f;
        while(time < startingTime)
        {
            time += Time.deltaTime;
            grenadeImages[currentGrenades].enabled = true;
            grenadeImages[currentGrenades].fillAmount = time/startingTime;
            yield return null;
        }
        yield return null;
    }

	// Use this for initialization
	void Start () {
		if(grenadeUIParent == null)
        {
            grenadeUIParent = transform.Find("SinglePlayer UI/HUD/Grenades").gameObject;
        }

        for(int i = 3; i >= 0; i--)
        {
            grenadeImages[i] = grenadeUIParent.transform.GetChild(i).GetComponent<Image>();

        }

        properties = GetComponent<PlayerProperties>();
	}
	
	// Update is called once per frame
	void Update () {

        #region Determines how many grenades to display in UI
        for (int i = grenadeImages.Length - 1; i >= currentGrenades; i--)
        {
            grenadeImages[i].enabled = false;
        }
        for (int i = currentGrenades -1; i >= 0; i--)
        {
            grenadeImages[i].enabled = true;
            grenadeImages[i].fillAmount = 1;
        }

        #endregion

        appliedThrowStrength = (GetComponent<GunAimUpSinglePlayer>().xRotation > .15f) ? rollStrenth : throwStrength;
        appliedThrowStrength *= properties.playerStrength;
        if (Input.GetButtonDown("Grenade"))
        {
            if (currentGrenades > 0)
            {
                if ( Time.time > lastShot + grenadeCoolDown )
                {
                    lastShot = Time.time;
                    currentGrenades--;

                    #region Throwing Grenade
                    var o = Instantiate(grenadeObject, grenadeSpawn.position, grenadeSpawn.rotation);
                    Rigidbody rigidbody = o.GetComponent<Rigidbody>();
                    rigidbody.AddForce(appliedThrowStrength * grenadeSpawn.forward, ForceMode.Impulse);
                    rigidbody.AddTorque(5 * grenadeSpawn.right, ForceMode.Impulse);
                    o.GetComponent<TimedGrenadeExplosion>().SetPlayerProperties(gameObject.GetComponent<PlayerProperties>());
                    #endregion
                } 
            }
            
        }
        if(currentGrenades < 4 && !coroutingRunning)
        {
            StartCoroutine(RegenGrenades());
        }

    }
}
