using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class PrefabManager : SingletonBehaviour<PrefabManager>
{
    public static float SquareCameraFarPlanceDistance;
    public static Transform ActiveBits;
    public static Transform InactiveBits;

    public GameObject WorldImpactPrefab;
    public GameObject BulletPrefab;

    private Stack<Bullet> _poolBullets = new Stack<Bullet>();

    protected override void Initialize()
    {
        ActiveBits = transform;
        InactiveBits = new GameObject("InactiveBits").transform;
        CreateBullets(200);
        SquareCameraFarPlanceDistance = Camera.main.farClipPlane.Squared();
    }

    private void CreateBullets(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var bulletGO = Instantiate(BulletPrefab, Vector3.zero, Quaternion.identity, ActiveBits);
            var bullet = bulletGO.GetComponent<Bullet>();
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
        return bullet;
    }

    public void RepoolBullet(Bullet bullet)
    {
        bullet.Reset();
        bullet.gameObject.SetActive(false);
        bullet.transform.parent = InactiveBits;
        _poolBullets.Push(bullet);
    }

    protected override void Shutdown()
    {

    }
}