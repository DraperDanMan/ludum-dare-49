using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    public WeaponStage Stage => WeaponStages[_weaponStage];

    public float Spread;
    public List<WeaponStage> WeaponStages;

    private int _weaponStage;

    public void CheckStage(int kills)
    {
        _weaponStage = 0;
        for (int i = WeaponStages.Count-1; i >= 0; i--)
        {
            if (WeaponStages[i].KillRequirement < kills)
            {
                _weaponStage = i;
                break;
            }
        }
    }

    public void Reset()
    {
        _weaponStage = 0;
    }
}

[Serializable]
public class WeaponStage
{
    public float RPM;
    public float InitialSpeed;
    public int Damage;

    public int KillRequirement = 0;
    public Color KillColorMotif;
}