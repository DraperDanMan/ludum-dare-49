using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class PrefabManager : SingletonBehaviour<PrefabManager>
{
    public static Transform ActiveBits;

    public GameObject WorldImpactPrefab;
    public GameObject BulletPrefab;
    protected override void Initialize()
    {
        ActiveBits = transform;
    }

    protected override void Shutdown()
    {

    }
}