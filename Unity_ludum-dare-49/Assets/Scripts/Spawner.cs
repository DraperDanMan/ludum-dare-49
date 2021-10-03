using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utils;

public class Spawner : Entity, IDamagable
{
    private static ShaderId CutoffHeight = "Cutoff_Height";

    public int Health = 6;
    protected int _maxHealth;

    public bool Active { get; set; }

    public float idleSpinSpeed = 45;
    public float fastSpinSpeed = 90;

    [SerializeField] private Transform _visual;
    private Vector3 _visualStartPos;
    private Vector3 _visualVel;

    [SerializeField] private Transform _spawnLocation;
    [SerializeField] private ParticleSystem _groundSpawnParticle;

    [Header("Spawn Details")]
    public float TimeBeforeInitialGroup = 8f;
    public int NumberToSpawn;
    public float TimeBetweenEnemies = 0.25f;
    public float EnemyEjectForce = 15f;

    public float TimeBetweenGroups = 20f;
    private float _nextGroupTime = float.MaxValue;

    private float CurrentSpinSpeed { get; set; }
    private float _timeAdjustedTurnSpeed;

    private float _timeAdjustedAliveTime = 0;

    private Vector3 _endDestination;
    private float _moveSpeed = 0.25f;
    private GameManager.SpawnRef _spawnRef;

    [SerializeField] private AudioClip _animInSound;
    [SerializeField] private AudioClip _spawnEnemySound;

    private Coroutine _runningCo;

    [SerializeField] private AudioClip _deathSound;

    [SerializeField]
    private List<Renderer> _renderers = new List<Renderer>();

    private static MaterialPropertyBlock _disolveBlock;

    private float _disolveAmount = 5;

    protected void Awake()
    {
        _disolveBlock ??= new MaterialPropertyBlock();
        _visualStartPos = _visual.localPosition;
        _visual.localPosition += Vector3.down * 5;
        _groundSpawnParticle.Play(true);
    }

    private void Start()
    {
        _runningCo = StartCoroutine(AnimateInCo());
    }

    public void SetDestination(GameManager.SpawnRef spawnRef)
    {
        _spawnRef = spawnRef;
        _endDestination = _spawnRef.DestinationLayer.GetSlot(_spawnRef.DestinationSlot,Vector3.zero);
    }

    private void Update()
    {
        if (IsDead) return;
        float deltaTime = Time.deltaTime * CurrentTimeScale;
        if (_timeAdjustedAliveTime >= _nextGroupTime)
        {
            _runningCo = StartCoroutine(SpawnGroupCo());
        }

        _timeAdjustedAliveTime += deltaTime;
    }
    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        _timeAdjustedTurnSpeed = CurrentSpinSpeed * deltaTime * CurrentTimeScale;
        transform.Rotate(transform.up,_timeAdjustedTurnSpeed);
        if (Active)
        {
            var timeAdjustedSpeed = _moveSpeed * deltaTime * CurrentTimeScale;
            transform.position = Vector3.MoveTowards(transform.position, _endDestination, timeAdjustedSpeed);
            if (transform.position.ApproximatelyEquals(_endDestination, 0.002f))
            {
                Active = false;
                _spawnRef.SpawnLayer.SetSlotClear(_spawnRef.SpawnSlot);
            }
        }
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

        GameManager.Kills++;
        IsDead = true;
        if (Active)
            _spawnRef.SpawnLayer.SetSlotClear(_spawnRef.SpawnSlot);
        _spawnRef.DestinationLayer.SetSlotClear(_spawnRef.DestinationSlot);
        Active = false;
        if (_runningCo != null) StopCoroutine(_runningCo);
        gameObject.SetLayerRecursive(12);
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _runningCo = StartCoroutine(DelayDoDestroy(2.5f));
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
        PrefabManager.Instance.CreateField(transform.position);
        if (_runningCo != null) StopCoroutine(_runningCo);
        Destroy(gameObject);
    }

    private void SpawnEnemy()
    {
        var enemyGo = Instantiate(PrefabManager.Instance.EnemyPrefab,
                                _spawnLocation.position, _spawnLocation.rotation, PrefabManager.TempBits);
        var enemy = enemyGo.GetComponent<Enemy>();
        enemy.SpawnAnim();
        enemy.Rigidbody.AddForce(_spawnLocation.forward*EnemyEjectForce);
        var ac = PrefabManager.Instance.UnpoolAudioCue();
        ac.Play(transform.position, _spawnEnemySound);
    }

    private IEnumerator SpawnGroupCo()
    {
        _nextGroupTime = float.MaxValue; //while we're spawning set out time really far away

        CurrentSpinSpeed = fastSpinSpeed;

        float nextEnemyTime = _timeAdjustedAliveTime + TimeBetweenEnemies;
        int enemiesSpawned = 0;
        while(enemiesSpawned < NumberToSpawn)
        {
            if (_timeAdjustedAliveTime >= nextEnemyTime)
            {
                SpawnEnemy();
                nextEnemyTime = _timeAdjustedAliveTime + TimeBetweenEnemies;
                enemiesSpawned++;
            }

            yield return null;
        }

        _nextGroupTime = _timeAdjustedAliveTime + TimeBetweenGroups;
        CurrentSpinSpeed = idleSpinSpeed;
        _runningCo = null;
    }

    private IEnumerator AnimateInCo()
    {
        CurrentSpinSpeed = fastSpinSpeed; //maybe even faster?

        var ac = PrefabManager.Instance.UnpoolAudioCue();
        ac.Play(transform.position, _animInSound);

        while (!_visual.localPosition.ApproximatelyEquals(_visualStartPos, 0.002f))
        {
            float timeAdjustedDeltaTime = Time.deltaTime * CurrentTimeScale;
            _visual.localPosition = Vector3.SmoothDamp(_visual.localPosition,_visualStartPos,
                        ref _visualVel,0.45f, 8,timeAdjustedDeltaTime);
            yield return null;
        }
        _groundSpawnParticle.Stop(true,ParticleSystemStopBehavior.StopEmitting);
        _nextGroupTime = _timeAdjustedAliveTime + TimeBeforeInitialGroup;
        CurrentSpinSpeed = idleSpinSpeed;
        Active = true;
        _runningCo = null;
    }

    private void OnValidate()
    {
        if (_renderers.Count == 0)
            _renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
    }
}