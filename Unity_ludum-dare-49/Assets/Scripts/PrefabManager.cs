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

    public GameObject NormalHitPrefab;
    public GameObject CritHitPrefab;

    private readonly List<Bullet> _allBullets = new List<Bullet>();
    private readonly Stack<Bullet> _poolBullets = new Stack<Bullet>();

    private readonly List<AudioCue> _allAudioCues = new List<AudioCue>();
    private readonly Stack<AudioCue> _poolAudioCues = new Stack<AudioCue>();

    private readonly List<VFXEffect> _allNormalHits = new List<VFXEffect>();
    private readonly Stack<VFXEffect> _poolNormalHits = new Stack<VFXEffect>();

    private readonly List<VFXEffect> _allCritHits = new List<VFXEffect>();
    private readonly Stack<VFXEffect> _poolCritHits = new Stack<VFXEffect>();

    protected override void Initialize()
    {
        ActiveBits = transform;
        InactiveBits = new GameObject("InactiveBits").transform;
        CreateTempBits();
        CreateBullets(200);
        CreateAudios(80);
        CreateParticle(true,30);
        CreateParticle(false,30);
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

    private void CreateParticle(bool normal, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var partGO = Instantiate(normal ? NormalHitPrefab : CritHitPrefab, Vector3.zero, Quaternion.identity, ActiveBits);
            var part = partGO.GetComponent<VFXEffect>();
            part.ParticleType = normal ? 0 : 1;
            if (part.ParticleType == 0)
            {
                _allNormalHits.Add(part);
            }
            else
            {
                _allCritHits.Add(part);
            }
            RepoolVFX(part);
        }
    }

    public VFXEffect UnpoolVFX(bool normal, Vector3 position, Quaternion rotation)
    {
        VFXEffect effect;
        if (normal)
        {
            if (_poolNormalHits.Count <= 0) CreateParticle(true, 1);
            effect = _poolNormalHits.Pop();
        }
        else
        {
            if (_poolCritHits.Count <= 0) CreateParticle(false, 1);
            effect = _poolCritHits.Pop();
        }

        effect.transform.parent = ActiveBits;
        effect.transform.SetPositionAndRotation(position,rotation);
        effect.gameObject.SetActive(true);
        effect.InPool = false;
        return effect;
    }

    public void RepoolVFX(VFXEffect fx)
    {
        fx.Reset();
        fx.InPool = true;
        fx.gameObject.SetActive(false);
        fx.transform.parent = InactiveBits;
        if (fx.ParticleType == 0)
        {
            _poolNormalHits.Push(fx);
        }
        else
        {
            _poolCritHits.Push(fx);
        }

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

        foreach (var fx in _allNormalHits)
        {
            if (!fx.InPool) RepoolVFX(fx);
        }
        foreach (var fx in _allCritHits)
        {
            if (!fx.InPool) RepoolVFX(fx);
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