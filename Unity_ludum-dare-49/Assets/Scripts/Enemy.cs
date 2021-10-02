using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Enemy : Entity
{
    public int Health = 100;
    protected int _maxHealth;

    [SerializeField]
    private float moveSpeed = 8;

    [SerializeField]
    private float turnSpeed = 45;

    private float _timeAdjustedSpeed;
    private float _timeAdjustedTurnSpeed;

    [SerializeField] private Transform _visual;

    protected virtual void Awake()
    {
        _maxHealth = Health;
    }

    private void FixedUpdate()
    {
        SetTimeAdjustedSpeed();
    }

    private void SetTimeAdjustedSpeed()
    {
        float deltaTime = Time.fixedDeltaTime;
        var toPlayerDir = transform.position.Direction(Player.Position);
        _timeAdjustedSpeed = moveSpeed * deltaTime * CurrentTimeScale;
        _timeAdjustedTurnSpeed = turnSpeed * deltaTime * CurrentTimeScale;

        _rigidbody.velocity = _visual.forward * _timeAdjustedSpeed;

        var forwardRot = Quaternion.LookRotation(_visual.forward);
        var toPlaterRot = Quaternion.LookRotation(toPlayerDir);
        _visual.rotation = Quaternion.RotateTowards(forwardRot,toPlaterRot,_timeAdjustedTurnSpeed);
    }
}