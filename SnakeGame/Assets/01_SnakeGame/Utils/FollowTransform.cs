using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform _follower;
    [SerializeField]
    private bool _followOnEnable;

    [Header("Inpsec")]
    [SerializeField]
    private bool _following;

    private IDisposable _followStream;

    public Transform Target
    {
        get
        {
            return _target;
        }
        set
        {
            _target = value;
        }
    }

    public void StartFollow()
    {
        if (_followStream != null)
        {
            _followStream.Dispose();
        }

        _following = true;

        Vector3 offset = _target.position - _follower.position;
        _followStream = Observable.EveryUpdate().Subscribe(_ =>
        {
            _follower.transform.position = _target.position - offset;
        });
    }

    public void StopFollow()
    {
        _following = false;
        if (_followStream == null)
        {
            return;
        }

        _followStream.Dispose();
    }

    private void OnEnable()
    {
        if (_followOnEnable)
        {
            StartFollow();
        }
    }

    private void OnDisable()
    {
        StopFollow();
        _target = null;
    }
}
