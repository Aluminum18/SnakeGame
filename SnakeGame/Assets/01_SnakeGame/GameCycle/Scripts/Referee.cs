using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Referee : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _onMissionSuccess;
    [SerializeField]
    private UnityEvent _onMissionFailed;

    public void MissionSuccess()
    {
        _onMissionSuccess.Invoke();
    }

    public void MissionFailed()
    {
        _onMissionFailed.Invoke();
    }
}
