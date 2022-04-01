using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SnakeStatsTracker : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _targetLength;

    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _currentSnakeLength;
    [SerializeField]
    private FloatVariable _targetProgress;

    [Header("Unity Events")]
    [SerializeField]
    private UnityEvent _onReachedTargetLength;

    private void CheckCurrentLengthAndTarget(int newLength)
    {
        _targetProgress.Value = (float)newLength / _targetLength.Value;

        if (newLength <= _targetLength.Value)
        {
            return;
        }

        _onReachedTargetLength.Invoke();
    }

    private void OnEnable()
    {
        _currentSnakeLength.OnValueChange += CheckCurrentLengthAndTarget;
    }

    private void OnDisable()
    {
        _currentSnakeLength.OnValueChange -= CheckCurrentLengthAndTarget;
    }
}
