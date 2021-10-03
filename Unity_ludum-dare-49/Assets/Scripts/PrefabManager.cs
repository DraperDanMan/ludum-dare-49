using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class PrefabManager : SingletonBehaviour<PrefabManager>
{
    public static float SquareCameraFarPlanceDistance;
    public static Transform TempBits;
    public static Transform ActiveBits;
    public static Transform InactiveBits;

    public List<FieldSpawn> FieldDescriptions = new List<FieldSpawn>();
    public GameObject BulletPrefab;
    public GameObject EnemyPrefab;
    public GameObject SpawnerPrefab;

    private readonly List<Bullet> _allBullets = new List<Bullet>();
    private readonly Stack<Bullet> _poolBullets = new Stack<Bullet>();

    protected override void Initialize()
    {
        ActiveBits = transform;
        InactiveBits = new GameObject("InactiveBits").transform;
        CreateTempBits();
        CreateBullets(200);
        SquareCameraFarPlanceDistance = Camera.main.farClipPlane.Squared();
    }

    private void CreateTempBits()
    {
        if (TempBits != null) Destroy(TempBits.gameObject);
        TempBits = new GameObject("TempBits").transform;
    }

    private void CreateBullets(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var bulletGO = Instantiate(BulletPrefab, Vector3.zero, Quaternion.identity, ActiveBits);
            var bullet = bulletGO.GetComponent<Bullet>();
            _allBullets.Add(bullet);
            RepoolBullet(bullet);
        }
    }

    public Bullet UnpoolBullet(Vector3 position, Quaternion rotation)
    {
        if (_poolBullets.Count <= 0) CreateBullets(1);
        var bullet = _poolBullets.Pop();
        bullet.transform.parent = ActiveBits;
        bullet.transform.SetPositionAndRotation(position,rotation);
        bullet.gameObject.SetActive(true);
        bullet.InPool = false;
        return bullet;
    }

    public void RepoolBullet(Bullet bullet)
    {
        bullet.Reset();
        bullet.InPool = true;
        bullet.gameObject.SetActive(false);
        bullet.transform.parent = InactiveBits;
        _poolBullets.Push(bullet);
    }

    public void CreateField(Vector3 position)
    {
        var prefabIdx = FieldDescriptions.GetRandomIndex();
        var fieldSpawn = FieldDescriptions[prefabIdx];
        var field = Instantiate(fieldSpawn.FieldPrefab, ActiveBits);
        field.transform.position = position;
        field.transform.localScale = Vector3.one * Random.Range(fieldSpawn.MinScale, fieldSpawn.MaxScale);
    }

    public void Reset()
    {
        foreach (var bullet in _allBullets)
        {
            if (!bullet.InPool) RepoolBullet(bullet);
        }
        CreateTempBits();
    }

    protected override void Shutdown()
    {

    }

    [Serializable]
    public class FieldSpawn
    {
        public GameObject FieldPrefab;
        public float MinScale = 1;
        public float MaxScale = 4;
    }
}