using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public int Health = 100;
    protected int _maxHealth;

    [SerializeField]
    private float moveSpeed = 8;

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
        float timeAdjustedSpeed = moveSpeed * CurrentTimeScale;
        transform.position = transform.right * (Mathf.Sin(Time.time)+5) * timeAdjustedSpeed;
    }
}