using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaSearch : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static GameObject FindClosestInRadiusWithTag(Vector3 origin, float radius, string tag)
    {
        Collider[] hitColliders = Physics.OverlapSphere(origin, radius);
        float minDistance = 10000;
        int closestIndex = 0;
        for(int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag(tag))
            {
                float tempDistance = Vector3.Distance(origin, hitColliders[i].transform.position);
                if (tempDistance < minDistance)
                {
                    minDistance = tempDistance;
                    closestIndex = i;
                }
            }
        }
        if(minDistance == 10000)
        {
            return null;
        }
        return hitColliders[closestIndex].gameObject;
    }

    public static GameObject[] FindAllInRadiusWithTag(Vector3 origin, float radius, string tag)
    {
        Collider[] hitColliders = Physics.OverlapSphere(origin, radius);
        List<GameObject> collidersGameObjects = new List<GameObject>(); 

        for(int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.transform.root.CompareTag(tag))
            {
                collidersGameObjects.Add(hitColliders[i].gameObject.transform.root.gameObject);
                //Debug.Log(collidersGameObjects[i].name + " has the tag " + tag);
            }
        }
        GameObject[] tempArray = new GameObject[collidersGameObjects.Count];
        for(int i = 0; i < tempArray.Length; i++)
        {
            tempArray[i] = collidersGameObjects[i];
        }
        tempArray = TrimObjectArray(tempArray);
        return tempArray;
        
    }

    public static bool[] CheckInLineOfSight(Vector3 origin, GameObject[] objects)
    {
        bool[] isInLineOfSight = new bool[objects.Length];
        for(int i = 0; i < objects.Length; i++)
        {
            Ray ray = new Ray(origin, objects[i].transform.position - origin);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if (hit.transform.root != objects[i].transform.root)
                {
                    isInLineOfSight[i] = false;
                    Debug.Log(hit.transform.root.name);
                }
                else
                {
                    isInLineOfSight[i] = true;
                }
            }
        }

        return isInLineOfSight;
    }

    public static bool CheckInLineOfSight(Vector3 origin, GameObject gameObject)
    {
        Ray ray = new Ray(origin, gameObject.transform.position - origin);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.root != gameObject.transform.root)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
            return false;
    }

    static GameObject[] TrimObjectArray(GameObject[] array)
    {
        List<GameObject> objects = new List<GameObject>();
        
        foreach(GameObject g in array)
        {
            bool isSame = false;
            for(int i = 0; i < objects.Count; i++)
            {
                isSame = g.GetInstanceID() == objects[i].GetInstanceID();
                if (isSame)
                    break;
            }
            if (!isSame)
            {
                objects.Add(g); 
            }

        }

        GameObject[] tempArray = new GameObject[objects.Count];
        for (int i = 0; i < tempArray.Length; i++)
        {
            tempArray[i] = objects[i];
        }
        return tempArray;
    }
}
