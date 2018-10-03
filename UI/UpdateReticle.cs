using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UpdateReticle : MonoBehaviour {

    
    [SerializeField]
    Sprite defaultReticleSprite;

    public float reticleSizeMutliplier = 1;

    float xSize = 30, ySize = 30;
    Image reticleImage;
    WeaponSwitch weaponSwitch;

    private void Start()
    {
        weaponSwitch = transform.root.GetComponent<WeaponSwitch>();
        reticleImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (weaponSwitch.equippedWeapon == null)
        {
            reticleImage.sprite = defaultReticleSprite;
        }

       // reticleImage.rectTransform.sizeDelta = new Vector2(xSize * reticleSizeMutliplier, ySize * reticleSizeMutliplier);
    }
}
