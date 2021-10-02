using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public FieldEffect Effect;

    private List<Rigidbody> _affectedRigidbodies = new List<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        var rigid = other.attachedRigidbody;
        var entity = rigid.GetComponent<Entity>();
        if (entity != null)
        {
            entity.LocalTimeScale.Push(this,-0.8f);
        }
        _affectedRigidbodies.Add(rigid);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        var rigid = other.attachedRigidbody;
        var entity = rigid.GetComponent<Entity>();
        if (entity != null)
        {
            entity.LocalTimeScale.Pop(this);
        }
        _affectedRigidbodies.Remove(rigid);
    }



    public enum FieldEffect
    {
        TimeSlow,
        TimeSpeed,
        Gravity,
        UpDraft,
        Damage,
    }
}