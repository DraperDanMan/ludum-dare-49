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
        _timeAdjustedSpeed = _initialSpeed * (timeScale * timeScale ); //bullets should fake slowdown more
        _rigidbody.velocity = transform.forward * _timeAdjustedSpeed;
    }

    private void OnCollisionEnter(Collision other)
    {
        var rigid = other.rigidbody;
        if (rigid != null && other.gameObject.layer != 0)
        {
            var damageable = rigid.GetComponent<IDamagable>();
            if (damageable != null)
            {
                damageable.TakeDamage(_damage);
            }
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