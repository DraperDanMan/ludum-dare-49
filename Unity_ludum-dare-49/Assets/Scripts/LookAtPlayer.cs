using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] private Entity _owner;
    [SerializeField] private float _inRadius = 5;

    [SerializeField] private Transform _eyeTransform;

    [SerializeField] private float _timeBetweenRandomLooks = 3;

    private float _nextRandomLook;
    private Vector3 _randomLookTarget;
    void Update()
    {
        float scaleDelta = Time.deltaTime * _owner.CurrentTimeScale;

        HandleRandomLook();

        var dirToPlayer = transform.position.Direction(Player.Position);
        _eyeTransform.up = Vector3.Lerp(_eyeTransform.up,
            dirToPlayer.magnitude < _inRadius ? dirToPlayer.normalized : _randomLookTarget.normalized,
            scaleDelta * 5f);
    }

    private void HandleRandomLook()
    {
        if (Time.time > _nextRandomLook)
        {
            _nextRandomLook = Time.time + _timeBetweenRandomLooks;
            _randomLookTarget = (transform.forward*-1) + Random.insideUnitSphere * 3;
        }
    }
}