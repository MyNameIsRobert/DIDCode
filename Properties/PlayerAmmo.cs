using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAmmo : MonoBehaviour {

    AmmoEnum.AmmoType ammoType;


    public int pistolAmmo, arAmmo, sniperAmmo, shotgunAmmo, specialAmmo, grenadeAmmo;
    [SerializeField]
    private bool debugModeInfiniteAmmo = false;

    public void AddAmmo(int amount, AmmoEnum.AmmoType type)
    {
        switch (type)
        {
            case AmmoEnum.AmmoType.Pistol:
                pistolAmmo += amount;
                break;
            case AmmoEnum.AmmoType.AR:
                arAmmo += amount;
                break;
            case AmmoEnum.AmmoType.Sniper_Heavy:
                sniperAmmo += amount;
                break;
            case AmmoEnum.AmmoType.Shotgun:
                shotgunAmmo += amount;
                break;
            case AmmoEnum.AmmoType.Special:
                specialAmmo += amount;
                break;
            case AmmoEnum.AmmoType.Grenade:
                grenadeAmmo += amount;
                break;
        }
    }

    public void RemoveAmmo(int amount, AmmoEnum.AmmoType type)
    {
        switch (type)
        {
            case AmmoEnum.AmmoType.Pistol:
                pistolAmmo -= amount;
                break;
            case AmmoEnum.AmmoType.AR:
                arAmmo -= amount;
                break;
            case AmmoEnum.AmmoType.Sniper_Heavy:
                sniperAmmo -= amount;
                break;
            case AmmoEnum.AmmoType.Shotgun:
                shotgunAmmo -= amount;
                break;
            case AmmoEnum.AmmoType.Special:
                specialAmmo -= amount;
                break;
            case AmmoEnum.AmmoType.Grenade:
                grenadeAmmo -= amount;
                break;
        }
    }

    public int GetAmount(AmmoEnum.AmmoType type)
    {
        int ammoInt = -1;
        switch (type)
        {
            case AmmoEnum.AmmoType.Pistol:
                ammoInt = pistolAmmo;
                break;
            case AmmoEnum.AmmoType.AR:
                ammoInt = arAmmo;
                break;
            case AmmoEnum.AmmoType.Sniper_Heavy:
                ammoInt = sniperAmmo;
                break;
            case AmmoEnum.AmmoType.Shotgun:
                ammoInt = shotgunAmmo;
                break;
            case AmmoEnum.AmmoType.Special:
                ammoInt = specialAmmo;
                break;
            case AmmoEnum.AmmoType.Grenade:
                ammoInt = grenadeAmmo;
                break;
        }
        return ammoInt;
    }

    private void Update()
    {
        if(debugModeInfiniteAmmo)
        {
            for(int i = 0; i < 5; i++)
            {
                if(GetAmount((AmmoEnum.AmmoType)i) < 100)
                {
                    AddAmmo(100, (AmmoEnum.AmmoType)i);
                }
            }
        }
    }

}

namespace AmmoEnum
{
    public enum AmmoType
    {
        Pistol,
        AR,
        Sniper_Heavy,
        Shotgun,
        Special,
        Grenade
    }
}
