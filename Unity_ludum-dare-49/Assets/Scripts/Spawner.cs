using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utils;

public class Spawner : Entity, IDamagable
{
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
    private float _moveSpeed = 2f;
    private GameManager.SpawnRef _spawnRef;

    protected override void Awake()
    {
        _visualStartPos = _visual.localPosition;
        _visual.localPosition += Vector3.down * 5;
        _groundSpawnParticle.Play(true);
    }

    private void Start()
    {
        StartCoroutine(AnimateInCo());
    }

    public void SetDestination(GameManager.SpawnRef spawnRef)
    {
        _spawnRef = spawnRef;
        _endDestination = _spawnRef.DestinationLayer.GetSlot(_spawnRef.DestinationSlot,Vector3.zero);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime * CurrentTimeScale;
        if (_timeAdjustedAliveTime >= _nextGroupTime)
        {
            StartCoroutine(SpawnGroupCo());
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
        if (Active)
            _spawnRef.SpawnLayer.SetSlotClear(_spawnRef.SpawnSlot);
        _spawnRef.DestinationLayer.SetSlotClear(_spawnRef.DestinationSlot);
        Destroy(gameObject);
    }

    private void SpawnEnemy()
    {
        var enemyGo = Instantiate(PrefabManager.Instance.EnemyPrefab,
                                _spawnLocation.position, _spawnLocation.rotation, PrefabManager.TempBits);
        var enemy = enemyGo.GetComponent<Enemy>();
        enemy.SpawnAnim();
        enemy.Rigidbody.AddForce(_spawnLocation.forward*EnemyEjectForce);
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
    }

    private IEnumerator AnimateInCo()
    {
        CurrentSpinSpeed = fastSpinSpeed; //maybe even faster?

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
    }
}