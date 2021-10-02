using System;
using UnityEngine;


public class Bullet: Entity
{
    [SerializeField] private Rigidbody _rigidbody;

    private float _initialSpeed;
    private float _timeAdjustedSpeed;

    public void Init(float initialSpeed)
    {
        _initialSpeed = initialSpeed;
        SetTimeAdjustedSpeed();
    }

    private void FixedUpdate()
    {
        SetTimeAdjustedSpeed();
    }

    private void SetTimeAdjustedSpeed()
    {
        _timeAdjustedSpeed = _initialSpeed * CurrentTimeScale;
        _rigidbody.velocity = transform.forward * _timeAdjustedSpeed;
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}