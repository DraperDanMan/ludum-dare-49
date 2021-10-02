using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    private Vector3 _rotation = Vector3.zero;
    private Vector3 _rotationForce = Vector3.zero;

    private float SwayOffset = 1.38f;

    public PlayerMovement playerMovement;
    public Camera Cam;

    [SerializeField]
    private float _accelBack = 1f;
    [SerializeField]
    private float _friction = 0.5f;

    private void Update()
    {
        _rotation.x = transform.localEulerAngles.x;

        float val = playerMovement.StrafeSpeed / playerMovement.MaxSpeed;
        _rotation.z = Mathf.Lerp(_rotation.z, SwayOffset * -val, Time.deltaTime * 10);
        //transform.localEulerAngles = rotation;

        _rotationForce *= _friction;
        Transform myTransform = transform;
        myTransform.Rotate(_rotationForce);
        transform.localRotation = Quaternion.Lerp(myTransform.localRotation, Quaternion.Euler(_rotation), Time.deltaTime * _accelBack);
        if (_rotationForce.magnitude < 0.1f)
        {
            _rotationForce = Vector3.zero;
        }
    }

    public void DoPunch(Vector3 pushForce, bool fullReturn = true)
    {
        _rotationForce += pushForce;
    }
}