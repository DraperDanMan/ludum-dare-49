using System;
using UnityEngine;


public class Bullet: MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;

    public void Init(float initialSpeed)
    {
        _rigidbody.velocity = transform.forward * initialSpeed;
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}