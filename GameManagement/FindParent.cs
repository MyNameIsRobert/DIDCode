using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindParent : MonoBehaviour {

    public static int callAmount = 0;
	public static GameObject FindParentWithTag(string tagToFind, Transform currentTransform)
    {
        callAmount++;
        GameObject tempObject;
        if (currentTransform.parent == null)
            return null;
        if(currentTransform.parent.CompareTag(tagToFind))
        {
            tempObject = currentTransform.transform.parent.gameObject;
            return tempObject;
        }
        else 
        {
            tempObject = FindParentWithTag(tagToFind, currentTransform.parent);
            return tempObject;
        }

    }

    public static Transform FindParentTransformWithTag(string tagToFind, Transform currentTransform)
    {
        callAmount++;
        Transform tempObject;
        if(currentTransform.parent == null)
        {
            return null;
        }
        //if (currentTransform.parent.CompareTag(tagToFind))
        //{
        //    tempObject = currentTransform.transform.parent;
        //    return tempObject;
        //}
        //else
        //{
        //    tempObject = FindParentTransformWithTag(tagToFind, currentTransform);
        //    return tempObject;

        //}
        tempObject = (currentTransform.parent.CompareTag(tagToFind)) ? currentTransform.parent : FindParentTransformWithTag(tagToFind, currentTransform.parent);

        return tempObject;

    }

    public static GameObject FindParentWithComponent(System.Type comp, Transform curr)
    {
        callAmount++;
        GameObject tempObject;
        if (curr.parent == null)
            return null;
        if(curr.parent.GetComponent(comp))
        {
            tempObject = curr.parent.gameObject;
            return tempObject;
        }
        else
        {
            tempObject = FindParentWithComponent(comp, curr.parent);
            return tempObject;
        }

    }

    public static Transform FindParentTransformWithComponent(System.Type comp, Transform curr)
    {
        callAmount++;
        Transform tempObject;
        if (curr.parent == null)
            return null;
        if (curr.parent.GetComponent(comp))
        {
            tempObject = curr.parent;
            return tempObject;
        }
        else
        {
            tempObject = FindParentTransformWithComponent(comp, curr.parent);
            return tempObject;
        }

    }
}
