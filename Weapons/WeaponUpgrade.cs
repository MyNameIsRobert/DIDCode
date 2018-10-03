using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgrade : MonoBehaviour
{

    public string methodToCall;
    public float newAmount;
    public float ductTapeToUse;
    [SerializeField]
    Amount ductTape;

    #region MethodCallTriggers
    bool callFireRate;
    bool callMagazineSize;
    bool callDamage;
    bool callZoomAmount;
    bool callAmmoLimit;
    bool callDamageFallOff;
    bool callCritHit;
    bool callBlastRadius;
    bool callPellets;
    #endregion

    private void Start()
    {
        callFireRate = methodToCall.Equals("IncreaseFireRate");
        callMagazineSize = methodToCall.Equals("IncreaseMaxMagazineSize");
        callDamage = methodToCall.Equals("IncreaseDamage");
        callZoomAmount = methodToCall.Equals("IncreaseZoomAmount");
        callAmmoLimit = methodToCall.Equals("IncreaseAmmoLimit");
        callDamageFallOff = methodToCall.Equals("IncreaseDamageFallOff");
        callCritHit = methodToCall.Equals("IncreaseCritHitMultiplier");
        callBlastRadius = methodToCall.Equals("IncreaseBlastRadius");
        callPellets = methodToCall.Equals("IncreaseNumOfPellets");



    }
}


//    public void ButtonWeaponUpgrade()
//    {
//        ductTape = gameObject.transform.root.Find("Duct Tape").GetComponent<Amount>();
//        if (ductTape.amountOf >= ductTapeToUse)
//        {
//            WeaponSwitch weaponSwitch = gameObject.transform.root.GetComponent<WeaponSwitch>();
            
//            WeaponProperties weaponProperties = weaponSwitch.equippedWeapon.GetComponent<WeaponProperties>();
//            if (callFireRate)
//            {
//                if(weaponProperties.IncreaseFireRate(newAmount))
//                    ductTape.amountOf -= ductTapeToUse;
//            }
//            else if (callMagazineSize)
//            {
//                if(weaponProperties.IncreaseMaxMagazineSize(newAmount))
//                    ductTape.amountOf -= ductTapeToUse;
//            }
//            else if (callDamage)
//            {
//                if(weaponProperties.IncreaseDamage(newAmount))
//                    ductTape.amountOf -= ductTapeToUse;
//            }
//            else if (callZoomAmount)
//            {
//                if(weaponProperties.IncreaseZoomAmount(newAmount))
//                    ductTape.amountOf -= ductTapeToUse;
//            }
//            else if (callAmmoLimit)
//            {
//                if(weaponProperties.IncreaseAmmoLimit(newAmount))
//                    ductTape.amountOf -= ductTapeToUse;
//            }
//            else if (callCritHit)
//            {
//                if(weaponProperties.IncreaseCritHitMultiplier(newAmount))
//                    ductTape.amountOf -= ductTapeToUse;
//            }
//            else if (callBlastRadius)
//            {
//                if(weaponProperties.IncreaseBlastRadius(newAmount))
//                    ductTape.amountOf -= ductTapeToUse;
//            }
//            else if (callDamageFallOff)
//            {
//                if(weaponProperties.IncreaseDamageFallOff(newAmount))
//                    ductTape.amountOf -= ductTapeToUse;
//            }
//            else if (callPellets)
//            {
//                if(weaponProperties.IncreaseNumOfPellets(newAmount))
//                    ductTape.amountOf -= ductTapeToUse;
//            }
//            else
//            {
//                Debug.Log("Not a method that can be called!");
//            }
                
//        }
//        else
//        {
//            Debug.Log("Not Enough Duct Tape!");
//        }
//    }
//}
