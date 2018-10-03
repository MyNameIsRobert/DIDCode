using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapIndicator : MonoBehaviour {

    GameObject icon;
    [SerializeField]
    float spawnHeight = 4;
    [SerializeField]
    float rotX, rotY, rotZ;

    GameObject tempIcon;
    Vector3 spawnVector;
    Quaternion spawnQauternion;
	// Use this for initialization
	void Start () {
        icon = GetComponent<Weapon>().minimapIndicator;
        if(rotX != 0 || rotY != 0)
        {
            spawnQauternion = Quaternion.Euler(rotX, rotY, rotZ);
        }
        else
        {
            spawnQauternion = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        }
        spawnVector = new Vector3(transform.position.x, spawnHeight, transform.position.z);
        tempIcon = Instantiate(icon, spawnVector, spawnQauternion);
        
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (tempIcon != null)
        {
            tempIcon.transform.position = new Vector3(transform.position.x, spawnHeight, transform.position.z);
            Quaternion rotation = Quaternion.Euler(rotX, rotY, transform.rotation.z);
            tempIcon.transform.rotation = rotation;
        }
	}
    public void DestroyCurrentIcon()
    {
        Destroy(tempIcon);
    }
}
