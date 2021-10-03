using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : SingletonBehaviour<GameManager>
{
    public static GameState State => Instance._gameState.CurrentState;
    private readonly StateMachine<GameState> _gameState = new StateMachine<GameState>(true);

    [SerializeField]
    private List<SpawnerLayer> _spawnerLayers = new List<SpawnerLayer>();

    [SerializeField]
    private float _initialDelay = 3f;

    [SerializeField]
    private List<SpawnTimeline> _spawnTimeline = new List<SpawnTimeline>();

    private int _timelineIdx = 0;
    private float GameTime = 0;

    protected override void Initialize()
    {

    }

    protected override void Shutdown()
    {

    }

    private void Start()
    {
        Reset();
    }

    private void Update()
    {
        GameTime += Time.deltaTime;
        if (_timelineIdx < _spawnTimeline.Count && GameTime >= _spawnTimeline[_timelineIdx].Time)
        {
            SpawnSpawner();
            _timelineIdx++;
        }
    }

    public void SpawnSpawner()
    {
        var spRef = FindNewSpawnerDetails();
        if (spRef.SpawnSlot < 0) return;

        var spawnPos = spRef.SpawnLayer.GetSlot(spRef.SpawnSlot, transform.position);

        var enemyGo = Instantiate(PrefabManager.Instance.SpawnerPrefab,
            spawnPos, Quaternion.identity, PrefabManager.TempBits);
        var spawner = enemyGo.GetComponent<Spawner>();
        spawner.SetDestination(spRef);
    }

    public SpawnRef FindNewSpawnerDetails()
    {
        var spawnRef = new SpawnRef();

        var outerLayer = _spawnerLayers.GetLastOrDefault();
        SpawnerLayer targetLayer = null;
        foreach (var layer in _spawnerLayers)
        {
            if (layer.IsFull) continue;
            int idx = Random.Range(0, layer.Slots);
            int attempts = 0;
            while (!layer.IsSlotFree(idx))
            {
                idx = (int)Mathf.Repeat(idx++, layer.Slots);
                attempts++;
                if (attempts >= layer.Slots) continue;
            }

            spawnRef.DestinationLayer = layer;
            spawnRef.DestinationSlot = idx;
        }

        //try find spawn slot that is out of view now
        spawnRef.SpawnLayer = outerLayer;
        spawnRef.SpawnSlot = -1; //no spawn found
        for (int i = 0; i < outerLayer.Slots; i++)
        {
            if (!outerLayer.IsSlotFree(i)) continue;
            spawnRef.SpawnSlot = i;
            var slotPos = outerLayer.GetSlot(i, transform.position);
            if (OutOfView(slotPos))
            {
                break;
            }
        }

        return spawnRef;
    }

    public void Reset()
    {
        PrefabManager.Instance.Reset();
        GameTime = 0;
        _timelineIdx = 0;
    }

    public bool OutOfView(Vector3 worldPosition)
    {
        var screenPoint = Camera.main.WorldToViewportPoint(worldPosition, Camera.MonoOrStereoscopicEye.Mono);
        return screenPoint.z < 0 && screenPoint.x < 0.1f || screenPoint.x > 0.9f && screenPoint.y < 0.1f ||
               screenPoint.y > 0.9f;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var layer in _spawnerLayers)
        {
            layer.EditorDrawLayer(transform.position);
        }
    }

    public enum GameState
    {
        Splash,
        Menu,
        Tutorial,
        Game,
        GameOver,
    }

    [Serializable]
    public class SpawnerLayer
    {
        public bool IsFull => FilledSlots.Count >= Slots;
        public float Radius;
        public int Slots;

        private List<int> FilledSlots = new List<int>();

        public bool IsSlotFree(int idx)
        {
            return !FilledSlots.Contains(idx);
        }

        public void SetSlotFilled(int idx)
        {
            FilledSlots.Add(idx);
        }

        public void SetSlotClear(int idx)
        {
            FilledSlots.Remove(idx);
        }

        public Vector3 GetSlot(int idx, Vector3 pos)
        {
            if (idx >= Slots) return pos;
            var diff = (360 / (float)Slots) * idx;
            var rotDelta = Quaternion.Euler(0,diff,0);
            return pos + (rotDelta * (Vector3.right * Radius));
        }

        public void EditorDrawLayer(Vector3 pos)
        {
#if UNITY_EDITOR
            Handles.DrawWireDisc(pos,Vector3.up, Radius,1);
            for (int i = 0; i < Slots; i++)
            {
                var slot = GetSlot(i, pos);
                Handles.DrawWireDisc(slot,Vector3.up, 0.5f,2);
            }
#endif
        }
    }

    public struct SpawnRef
    {
        public SpawnerLayer SpawnLayer;
        public int SpawnSlot;
        public SpawnerLayer DestinationLayer;
        public int DestinationSlot;
    }

    [Serializable]
    public struct SpawnTimeline
    {
        public float Time;
    }
}