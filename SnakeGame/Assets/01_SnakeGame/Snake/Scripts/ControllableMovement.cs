using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ControllableMovement : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private Vector3Variable _direction;
    [SerializeField]
    private FloatVariable _refDeltaTime;

    [Header("Config")]
    [SerializeField]
    private Transform _movedTransform;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _rotateSpeed;

    [Header("Inspec")]
    [SerializeField]
    private bool _isRotating = false;

    private IDisposable _movingStream;
    private IDisposable _rotatingStream;

    public void StartRotating(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            return;
        }

        if (Mathf.Abs(direction.x - direction.z) < float.Epsilon)
        {
            return;
        }

        if (1f - Mathf.Abs(Vector3.Dot(direction, _movedTransform.forward)) < float.Epsilon)
        {
            return;
        }

        if (_isRotating)
        {
            return;
        }

        Quaternion rotateTo = Quaternion.LookRotation(direction);
        _rotatingStream = Observable.EveryUpdate().Subscribe(_ =>
        {
            _isRotating = true;
            if (Quaternion.Angle(rotateTo, _movedTransform.rotation) < 5f)
            {
                StopRotating();
                _movedTransform.rotation = rotateTo;
                _isRotating = false;
                return;
            }
            _movedTransform.rotation = Quaternion.RotateTowards(_movedTransform.rotation, rotateTo, _rotateSpeed * _refDeltaTime.Value);
        });
    }

    public void StopRotating()
    {
        if (_rotatingStream == null)
        {
            return;
        }

        _rotatingStream.Dispose();
    }

    public void StartMoving()
    {
        _movingStream = Observable.EveryUpdate().Subscribe(_ =>
        {
            _movedTransform.position += _movedTransform.forward * _speed * _refDeltaTime.Value;
        });
    }

    public void StopMoving()
    {
        if (_movingStream == null)
        {
            return;
        }

        _movingStream.Dispose();
    }

    private void OnEnable()
    {
        StartMoving();
        _direction.OnValueChange += StartRotating;
    }

    private void OnDisable()
    {
        StopMoving();
        StopRotating();
        _direction.OnValueChange -= StartRotating;
    }
}
