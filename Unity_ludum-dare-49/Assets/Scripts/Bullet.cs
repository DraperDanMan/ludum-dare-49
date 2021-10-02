using System;
using UnityEngine;


public class Bullet: Entity
{

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
        float timeScale = CurrentTimeScale;
        _timeAdjustedSpeed = _initialSpeed * (timeScale * timeScale ); //bullets should fake slowdown more
        _rigidbody.velocity = transform.forward * _timeAdjustedSpeed;
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}