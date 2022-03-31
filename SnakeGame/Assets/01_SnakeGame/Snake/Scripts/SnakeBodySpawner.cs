using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodySpawner : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private ObjectSpawner _bodyObjectSpawner;
    [SerializeField]
    private ChainedMovingNode _chainedMoving;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onGotFood;

    public void SpawnNewBody(object[] args)
    {
        var newBodyObj = _bodyObjectSpawner.SpawnAndReturnObject();
        newBodyObj.name = "Body";
        var newChainedMoving = newBodyObj.GetComponent<SnakePropertiesHolder>().ChainedMoving;

        newChainedMoving.SetFront(_chainedMoving);
        newChainedMoving.StartMoving();

        _onGotFood.Unsubcribe(SpawnNewBody);
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
