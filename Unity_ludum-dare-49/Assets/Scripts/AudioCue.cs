using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCue : Entity
{
    public bool InPool { get; set; }

    [SerializeField]
    private bool _manual;
    [SerializeField]
    private AudioSource _source;
    [SerializeField]
    private float _baseVolume = 1;
    [SerializeField]
    private float _basePitch = 1;

    private bool _queuedPlay = false;
    private bool _queuedThisFrame= false;

    private void Start()
    {
        if (_manual)
        {
            UpdateTimeScaledSound();
            _source.Play();
        }
    }

    public void Play(Vector3 pos, AudioClip clip, float volume = 1, float pitch = 1, bool is2D = false)
    {
        transform.position = pos;
        _source.clip = clip;
        _baseVolume = volume;
        _basePitch = pitch;
        UpdateTimeScaledSound();
        _source.spatialize = is2D;
        _queuedThisFrame = true;
        _queuedPlay = true;
    }

    private void Update()
    {
        if (!_manual) CheckCueDeath();
        UpdateTimeScaledSound();

        if (_queuedPlay)
        {
            if (!_queuedThisFrame)
            {
                _source.Play();
                _queuedPlay = false;
            }

            _queuedThisFrame = false;
        }
    }

    private void UpdateTimeScaledSound()
    {
        _source.pitch = Mathf.Clamp(_basePitch * CurrentTimeScale, 0.6f, 1.4f);
        _source.volume = _baseVolume;
    }

    private void CheckCueDeath()
    {
        if (!_source.isPlaying && !_queuedPlay)
        {
            PrefabManager.Instance.RepoolAudioCue(this);
        }
    }

    public void Reset()
    {
        _source.Stop();
        _localTimeScale.Clear();
    }


}