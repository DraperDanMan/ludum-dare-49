using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXEffect : Entity
{
    public bool InPool { get; set; }
    public int ParticleType { get; set; }

    [SerializeField]
    private List<ParticleSystem> _particles = new List<ParticleSystem>();

    public void Init()
    {
        Update();
        foreach (var particle in _particles)
        {
            particle.Play(false);
        }

        StartCoroutine(RepoolWhenDone());
    }

    private void Update()
    {
        foreach (var particle in _particles)
        {
            var main = particle.main;
            main.simulationSpeed = CurrentTimeScale;
        }
    }

    public void Reset()
    {
        _localTimeScale.Clear();
        foreach (var particle in _particles)
        {
            particle.Stop(false);
        }
    }

    private IEnumerator RepoolWhenDone()
    {
        //assumed 1 second long
        for (float t = 0; t < 1; t+=(Time.deltaTime*CurrentTimeScale))
        {
            yield return null;
        }
        PrefabManager.Instance.RepoolVFX(this);
    }
}