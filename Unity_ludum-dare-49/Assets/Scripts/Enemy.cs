using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Enemy : Entity, IDamagable
{
    public int Health = 6;
    protected int _maxHealth;

    [SerializeField]
    private float moveSpeed = 8;

    [SerializeField]
    private float turnSpeed = 45;

    private float _timeAdjustedSpeed;
    private float _timeAdjustedTurnSpeed;

    [SerializeField] private Transform _visual;
    private Vector3 _startScale;
    private Vector3 _visualVel;

    private Coroutine _animCo;

    protected void Awake()
    {
        _maxHealth = Health;
        _startScale = _visual.localScale;
    }

    private void FixedUpdate()
    {
        SetTimeAdjustedSpeed();
    }

    private void SetTimeAdjustedSpeed()
    {
        float deltaTime = Time.fixedDeltaTime;

        bool outOfView = OutOfView();
        var tweakTurnSpeed = outOfView ? turnSpeed * 1.25f : turnSpeed;
        var tweakMoveSpeed = outOfView ? moveSpeed * 1.10f : moveSpeed;

        var toPlayerDir = transform.position.Direction(Player.Position);
        _timeAdjustedSpeed = tweakMoveSpeed * deltaTime * CurrentTimeScale;
        _timeAdjustedTurnSpeed = tweakTurnSpeed * deltaTime * CurrentTimeScale;

        _rigidbody.velocity = _visual.forward * _timeAdjustedSpeed;

        var forwardRot = Quaternion.LookRotation(_visual.forward);
        var toPlaterRot = Quaternion.LookRotation(toPlayerDir);
        _visual.rotation = Quaternion.RotateTowards(forwardRot,toPlaterRot,_timeAdjustedTurnSpeed);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        //flash and play sound
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Kills++;
        if (_animCo != null) StopCoroutine(_animCo);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        var rigid = other.rigidbody;
        if (rigid != null && other.gameObject.layer == 6)
        {
            var damagable = rigid.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(1);
            }
        }
    }

    public void SpawnAnim()
    {
        _visual.localScale = Vector3.one * 0.1f;
        _animCo = StartCoroutine(SpawnAnimCo());
    }

    private IEnumerator SpawnAnimCo()
    {
        while (!_visual.localScale.ApproximatelyEquals(_startScale, 0.002f))
        {
            float timeAdjustedDeltaTime = Time.deltaTime * CurrentTimeScale;
            _visual.localScale = Vector3.SmoothDamp(_visual.localScale,_startScale,
                ref _visualVel,0.15f, 5f,timeAdjustedDeltaTime);
            yield return null;
        }

        _animCo = null;
    }

    public bool OutOfView()
    {
        var screenPoint = Player.Cam.WorldToViewportPoint(transform.position, Camera.MonoOrStereoscopicEye.Mono);
        return screenPoint.z < 0 && screenPoint.x < 0.1f || screenPoint.x > 0.9f && screenPoint.y < 0.1f ||
               screenPoint.y > 0.9f;
    }

}