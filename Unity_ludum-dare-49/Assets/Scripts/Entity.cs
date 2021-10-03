using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Entity : MonoBehaviour
{
    private static float _gameTimeScaleOffset;
    private static int _gameTimeScaleFrame = -1;
    public static float GameTimeScaleOffset
    {
        get
        {
            if (_gameTimeScaleFrame < Time.frameCount)
            {
                float plyTimeScale = Player.PlayerTimeScale.Value;
                _gameTimeScaleOffset = (1 - plyTimeScale);
                _gameTimeScaleFrame = Time.frameCount;
            }

            return _gameTimeScaleOffset;
        }
    }


    public FloatEffectorStack LocalTimeScale => _localTimeScale;
    protected FloatEffectorStack _localTimeScale = new FloatEffectorStack(FloatEffectorStack.RoughAverage, 1);

    public float CurrentTimeScale => _localTimeScale.Value;// + GameTimeScaleOffset;

    public Rigidbody Rigidbody => _rigidbody;

    [SerializeField] protected Rigidbody _rigidbody;

    public bool IsDead { get; protected set; }

}