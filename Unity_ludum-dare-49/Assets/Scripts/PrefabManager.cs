using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class PrefabManager : SingletonBehaviour<PrefabManager>
{
    public static float SquareCameraFarPlanceDistance;
    public static Transform TempBits;
    public static Transform ActiveBits;
    public static Transform InactiveBits;

    public List<FieldSpawn> FieldDescriptions = new List<FieldSpawn>();
    public GameObject BulletPrefab;
    public GameObject EnemyPrefab;
    public GameObject SpawnerPrefab;

    public GameObject AudioCuePrefab;

    private readonly List<Bullet> _allBullets = new List<Bullet>();
    private readonly Stack<Bullet> _poolBullets = new Stack<Bullet>();

    private readonly List<AudioCue> _allAudioCues = new List<AudioCue>();
    private readonly Stack<AudioCue> _poolAudioCues = new Stack<AudioCue>();

    protected override void Initialize()
    {
        ActiveBits = transform;
        InactiveBits = new GameObject("InactiveBits").transform;
        CreateTempBits();
        CreateBullets(200);
        CreateAudios(80);
        SquareCameraFarPlanceDistance = Camera.main.farClipPlane.Squared();
    }

    private void CreateTempBits()
    {
        if (TempBits != null) Destroy(TempBits.gameObject);
        TempBits = new GameObject("TempBits").transform;
    }

    private void CreateBullets(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var bulletGO = Instantiate(BulletPrefab, Vector3.zero, Quaternion.identity, ActiveBits);
            var bullet = bulletGO.GetComponent<Bullet>();
            _allBullets.Add(bullet);
            RepoolBullet(bullet);
        }
    }

    public Bullet UnpoolBullet(Vector3 position, Quaternion rotation)
    {
        if (_poolBullets.Count <= 0) CreateBullets(1);
        var bullet = _poolBullets.Pop();
        bullet.transform.parent = ActiveBits;
        bullet.transform.SetPositionAndRotation(position,rotation);
        bullet.gameObject.SetActive(true);
        bullet.InPool = false;
        return bullet;
    }

    public void RepoolBullet(Bullet bullet)
    {
        bullet.Reset();
        bullet.InPool = true;
        bullet.gameObject.SetActive(false);
        bullet.transform.parent = InactiveBits;
        _poolBullets.Push(bullet);
    }

    public void CreateField(Vector3 position)
    {
        var prefabIdx = FieldDescriptions.GetRandomIndex();
        var fieldSpawn = FieldDescriptions[prefabIdx];
        var field = Instantiate(fieldSpawn.FieldPrefab, TempBits);
        field.transform.position = position;
        field.transform.localScale = Vector3.one * Random.Range(fieldSpawn.MinScale, fieldSpawn.MaxScale);
    }

    private void CreateAudios(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var audioGO = Instantiate(AudioCuePrefab, Vector3.zero, Quaternion.identity, ActiveBits);
            var ac = audioGO.GetComponent<AudioCue>();
            _allAudioCues.Add(ac);
            RepoolAudioCue(ac);
        }
    }

    public AudioCue UnpoolAudioCue()
    {
        if (_poolAudioCues.Count <= 0) CreateAudios(1);
        var ac = _poolAudioCues.Pop();
        ac.transform.parent = ActiveBits;
        ac.gameObject.SetActive(true);
        ac.InPool = false;
        return ac;
    }

    public void RepoolAudioCue(AudioCue ac)
    {
        ac.Reset();
        ac.InPool = true;
        ac.gameObject.SetActive(false);
        ac.transform.parent = InactiveBits;
        _poolAudioCues.Push(ac);
    }

    public void Reset()
    {
        foreach (var bullet in _allBullets)
        {
            if (!bullet.InPool) RepoolBullet(bullet);
        }
        foreach (var cue in _allAudioCues)
        {
            if (!cue.InPool) RepoolAudioCue(cue);
        }
        CreateTempBits();
    }

    protected override void Shutdown()
    {

    }

    [Serializable]
    public class FieldSpawn
    {
        public GameObject FieldPrefab;
        public float MinScale = 1;
        public float MaxScale = 4;
    }
}