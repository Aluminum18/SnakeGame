using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodySpawner : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private ObjectSpawner _bodyObjectSpawner;

    [SerializeField]
    private SnakePropertiesHolder _currentSnakeTail;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onGotFood;

    public void SpawnNewBody(object[] args)
    {
        var newBodyObj = _bodyObjectSpawner.SpawnAndReturnObject();
        SnakePropertiesHolder newBodyProperties = newBodyObj.GetComponent<SnakePropertiesHolder>();
        ChainedMovingNode newBodyChainedMoving = newBodyProperties.ChainedMoving;

        newBodyChainedMoving.SetFront(_currentSnakeTail.ChainedMoving);
        newBodyChainedMoving.StartMoving();

        _currentSnakeTail = newBodyProperties;
    }

    private void OnEnable()
    {
        _onGotFood.Subcribe(SpawnNewBody);
    }

    private void OnDisable()
    {
        _onGotFood.Unsubcribe(SpawnNewBody);
    }
}
