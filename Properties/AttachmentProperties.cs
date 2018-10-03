using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentProperties : MonoBehaviour {
    public AttachmentType attachmentType;
    [System.Serializable]
    public class AttachmentType : System.Object
    {
        public bool sight;
        public bool magazine;
        public bool leftSide;
        public bool rightSide;
        public bool underBarrel;
        public bool misc;
    }

    [Tooltip("Should be formatted like: \"IncreaseDamage\", where you replace Damage with a weapon property")]
    public string[] attachmentModifier;
    public float[] modifierAmount;
    // Use this for initialization
    void Start() {
        if (attachmentType.sight)
        {
            attachmentType.magazine = false;
            attachmentType.leftSide = false;
            attachmentType.rightSide = false;
            attachmentType.underBarrel = false;
            attachmentType.misc = false;
        }
        else if (attachmentType.magazine)
        {
            attachmentType.sight = false;
            attachmentType.leftSide = false;
            attachmentType.rightSide = false;
            attachmentType.underBarrel = false;
            attachmentType.misc = false;
        }
        else if (attachmentType.leftSide)
        {
            attachmentType.sight = false;
            attachmentType.magazine = false;
            attachmentType.rightSide = false;
            attachmentType.underBarrel = false;
            attachmentType.misc = false;
        }
        else if (attachmentType.rightSide)
        {
            attachmentType.underBarrel = false;
            attachmentType.misc = false;
        }
        else if (attachmentType.underBarrel)
        {
            attachmentType.misc = false;
        }
        else if (attachmentType.misc)
        {

        }
        else
        {
            attachmentType.sight = true;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public int GetAttachmentType()
    {
        if (attachmentType.sight)
            return 0;
        else if (attachmentType.magazine)
            return 1;
        else if (attachmentType.leftSide)
            return 2;
        else if (attachmentType.rightSide)
            return 3;
        else if (attachmentType.underBarrel)
            return 4;
        else
            return 5;
    }
}
