using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private Player _owner;

    public MusicLayer BaseLayer;

    public List<MusicLayer> _musicLayers = new List<MusicLayer>();

    private void Awake()
    {
        CreateSourceForLayer(BaseLayer);
        BaseLayer.Source.volume = 1;
        foreach (var layer in _musicLayers)
        {
            CreateSourceForLayer(layer);
        }
    }

    private void Update()
    {
        float timeScale = _owner.CurrentTimeScale;
        BaseLayer.UpdateSource(timeScale,true); //always on

        bool layerUsed = false;
        int thingCount = PrefabManager.TempBits.childCount; //find out which layer wants to be on.
        for (int i = _musicLayers.Count-1; i >= 0; i--)
        {
            if (_musicLayers[i].EnemyCount < thingCount && !layerUsed)
            {
                _musicLayers[i].UpdateSource(timeScale,true);
                layerUsed = true;
            }
            else
            {
                _musicLayers[i].UpdateSource(timeScale,false);
            }
        }
    }

    private void CreateSourceForLayer(MusicLayer layer)
    {
        if (layer.Source != null) return;
        var source = gameObject.AddComponent<AudioSource>();
        source.clip = layer.track;
        source.spatialBlend = 0.1f;
        source.loop = true;
        source.priority = 50;
        source.volume = 0;
        source.dopplerLevel = 0;
        source.playOnAwake = false;
        layer.Source = source;
        source.Play();
    }

    [Serializable]
    public class MusicLayer
    {
        [HideInInspector]
        public AudioSource Source;

        public AudioClip track;
        public int EnemyCount;

        private float _targetVolume;

        public void UpdateSource(float timeScale, bool on)
        {
            float pitch = Mathf.Clamp(timeScale, 0.6f, 1.4f);
            _targetVolume = on ? 0.5f : 0;
            Source.pitch = pitch;
            Source.volume = Mathf.MoveTowards(Source.volume, _targetVolume, 1*timeScale);
        }
    }
}