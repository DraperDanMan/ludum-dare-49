using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Enemy : Entity, IDamagable
{
    private static ShaderId CutoffHeight = "Cutoff_Height";
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

    [SerializeField] private AudioClip _deathSound;

    [SerializeField]
    private List<Renderer> _renderers = new List<Renderer>();

    private static MaterialPropertyBlock _disolveBlock;

    private float _disolveAmount = 5;

    protected void Awake()
    {
        _disolveBlock ??= new MaterialPropertyBlock();
        _maxHealth = Health;
        _startScale = _visual.localScale;
    }

    private void FixedUpdate()
    {
        if (!IsDead)
        {
            SetTimeAdjustedSpeed();
        }
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
        if (IsDead) return;
        Health -= damage;
        //flash and play sound
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        var ac = PrefabManager.Instance.UnpoolAudioCue();
        ac.Play(transform.position, _deathSound);
        IsDead = true;
        GameManager.Kills++;
        if (_animCo != null) StopCoroutine(_animCo);
        gameObject.SetLayerRecursive(12);
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _animCo = StartCoroutine(DelayDoDestroy(2.5f));
    }

    private void OnCollisionEnter(Collision other)
    {
        if (IsDead) return;
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

    private IEnumerator DelayDoDestroy(float time = 1)
    {
        for (float t = 0; t < 1; t+=(Time.deltaTime * CurrentTimeScale)/time)
        {
            _disolveAmount = Mathf.Lerp(5, -0.5f, t);
            _disolveBlock.SetFloat(CutoffHeight, _disolveAmount);
            foreach (var renderer in _renderers)
            {
                renderer.SetPropertyBlock(_disolveBlock);
            }
            yield return null;
        }

        DoFullDestroy();
    }

    private void DoFullDestroy()
    {
        if (_animCo != null) StopCoroutine(_animCo);
        Destroy(gameObject);
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

    private void OnValidate()
    {
        if (_renderers.Count == 0)
            _renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
    }
}