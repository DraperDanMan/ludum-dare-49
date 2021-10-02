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

    public void Upgrade()
    {
        _weaponStage = Mathf.Clamp(_weaponStage++, 0, WeaponStages.Count - 1);
    }
}

[Serializable]
public class WeaponStage
{
    public float RPM;
    public float InitialSpeed;
    public int Damage;
}