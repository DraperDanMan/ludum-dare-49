using System;
using UnityEngine;


public class Bullet: Entity
{
    public bool InPool { get; set; }

    private float _initialSpeed;
    private int _damage;
    private float _timeAdjustedSpeed;



    public void Init(float initialSpeed, int damage)
    {
        _initialSpeed = initialSpeed;
        _damage = damage;
        SetTimeAdjustedSpeed();
    }

    private void FixedUpdate()
    {
        SetTimeAdjustedSpeed();
        DistanceRepoolCheck();
    }

    private void DistanceRepoolCheck()
    {
        if (_rigidbody.position.sqrMagnitude > PrefabManager.SquareCameraFarPlanceDistance)
        {
            PrefabManager.Instance.RepoolBullet(this);
        }
    }

    private void SetTimeAdjustedSpeed()
    {
        float timeScale = CurrentTimeScale;
        _timeAdjustedSpeed = _initialSpeed * (timeScale < 1 ? (timeScale * timeScale ) : timeScale); //bullets should fake slowdown more
        _rigidbody.velocity = transform.forward * _timeAdjustedSpeed;
    }

    private void OnCollisionEnter(Collision other)
    {
        var contact = other.GetContact(0);
        var rigid = other.rigidbody;
        if (rigid != null && other.collider.gameObject.layer != 0)
        {
            var damageable = rigid.GetComponent<IDamagable>();
            if (damageable != null)
            {
                damageable.TakeDamage(_damage);
                var fx = PrefabManager.Instance.UnpoolVFX(false, contact.point, Quaternion.LookRotation(contact.normal));
                fx.Init();
            }
            else
            {
                var fx = PrefabManager.Instance.UnpoolVFX(true, contact.point, Quaternion.LookRotation(contact.normal));
                fx.Init();
            }
        }
        else
        {
            var fx = PrefabManager.Instance.UnpoolVFX(true, contact.point, Quaternion.LookRotation(contact.normal));
            fx.Init();
        }

        PrefabManager.Instance.RepoolBullet(this);
    }

    public void Reset()
    {
        _localTimeScale.Clear();
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.rotation = Quaternion.identity;
        _rigidbody.angularVelocity = Vector3.zero;
    }
}