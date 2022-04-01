using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class RoundTimeCounter : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private FloatVariable _roundTime;
    [SerializeField]
    private FloatVariable _refDeltaTime;

    [Header("Reference - Write")]
    [SerializeField]
    private FloatVariable _remainTime;

    [Header("Unity Event")]
    [SerializeField]
    private UnityEvent _onTimeUp;

    private IDisposable _timerStream;

    public void StartCountTime()
    {
        _remainTime.Value = _roundTime.Value;
        _timerStream = Observable.EveryUpdate().Subscribe(_ =>
        {
            if (_remainTime.Value <= 0f)
            {
                StopCountTime();
                _onTimeUp.Invoke();
                return;
            }

            _remainTime.Value -= _remainTime.Value;
        });
    }

    public void StopCountTime()
    {
        if (_timerStream == null)
        {
            return;
        }

        _timerStream.Dispose();
    }
}
