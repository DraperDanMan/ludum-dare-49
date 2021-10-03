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

    protected virtual void Awake()
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
        var toPlayerDir = transform.position.Direction(Player.Position);
        _timeAdjustedSpeed = moveSpeed * deltaTime * CurrentTimeScale;
        _timeAdjustedTurnSpeed = turnSpeed * deltaTime * CurrentTimeScale;

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
        PrefabManager.Instance.CreateField(transform.position);
        Destroy(gameObject);
    }

    public void SpawnAnim()
    {
        _visual.localScale = Vector3.one * 0.1f;
        StartCoroutine(SpawnAnimCo());
    }

    private IEnumerator SpawnAnimCo()
    {
        while (!_visual.localScale.ApproximatelyEquals(_startScale))
        {
            float timeAdjustedDeltaTime = Time.deltaTime * CurrentTimeScale;
            _visual.localScale = Vector3.SmoothDamp(_visual.localScale,_startScale,
                ref _visualVel,0.15f, 0.5f,timeAdjustedDeltaTime);
            yield return null;
        }
    }

}