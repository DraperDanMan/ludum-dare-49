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
            ApplyEffect(entity);
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
            RemoveEffect(entity);
        }
        _affectedRigidbodies.Remove(rigid);
    }

    private void ApplyEffect(Entity ent)
    {
        switch (Effect)
        {
            case FieldEffect.TimeSlow:
                ent.LocalTimeScale.Push(this,0);
                break;
            case FieldEffect.TimeSpeed:
                ent.LocalTimeScale.Push(this,2f);
                break;
            case FieldEffect.Gravity:
                break;
            case FieldEffect.UpDraft:
                break;
            case FieldEffect.Damage:
                break;
        }
    }

    private void RemoveEffect(Entity ent)
    {
        switch (Effect)
        {
            case FieldEffect.TimeSlow:
            case FieldEffect.TimeSpeed: //both pop the current time effect whether thats slow or speed
                ent.LocalTimeScale.Pop(this);
                break;
            case FieldEffect.Gravity:
                break;
            case FieldEffect.UpDraft:
                break;
            case FieldEffect.Damage:
                break;
        }
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