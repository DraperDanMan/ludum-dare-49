using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private Player _owner;

    public Transform ShotOrigin;

    private float _nextShot = 1;
    private float _lastShotTime = -1;

    public bool CanShoot => _lastShotTime + _nextShot <= Time.time;

    private Vector3 _gunStartPos;
    private Vector3 _gunRecoil;

    public ParticleSystem FlashEffect;

    [SerializeField] private WeaponData _weaponData;

    public Action<int, Vector3> OnDealDamage;

    // Use this for initialization
    private void Start()
    {
        _nextShot = 1 / (_weaponData.Stage.RPM / 60);
        _gunStartPos = transform.localPosition;
    }

    private void Update()
    {
        transform.localPosition = _gunStartPos + _gunRecoil;
        _gunRecoil = Vector3.Lerp(_gunRecoil, Vector3.zero, Time.deltaTime * 10f);
    }

    public bool Fire(bool firstShot = false)
    {
        if (_lastShotTime + _nextShot > Time.time) return false;
        _lastShotTime = Time.time;
        _owner.Recoil.DoPunch(Vector3.up * Random.Range(-0.05f, 0.06f) * 5f);
        _nextShot = 1 / (_weaponData.Stage.RPM / 60);
        _gunRecoil = Vector3.forward * (Random.Range(-0.2f, -0.05f));
        InternalFire();
        return true;
    }

    private void InternalFire()
    {
        var cam = _owner.Camera.transform;
        var plyForwardSpeed = _owner.Movement.FowardSpeed;
        var rotAngle = Random.Range(0f, 360f);
        var offset = Quaternion.Euler(0f, Random.Range(0f, _weaponData.Spread), 0f);
        var spin = Quaternion.AngleAxis(rotAngle, cam.forward);

        var fireForward = offset * ShotOrigin.forward;
        fireForward = spin * fireForward;

        var bulletPrefab = PrefabManager.Instance.BulletPrefab;
        var bulletRot = Quaternion.LookRotation(fireForward);
        var bulletGO = Instantiate(bulletPrefab, ShotOrigin.position, bulletRot, PrefabManager.ActiveBits);
        var bullet = bulletGO.GetComponent<Bullet>();
        bullet.Init(_weaponData.Stage.InitialSpeed+plyForwardSpeed);

        FlashEffect.Emit(1);
    }

}