using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakePropertiesHolder : MonoBehaviour
{
    [SerializeField]
    private ChainedMovingNode _chainedMoving;
    public ChainedMovingNode ChainedMoving => _chainedMoving;
}
