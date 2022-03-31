using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ChainedMovingNode : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private Transform _movedTransform;
    [SerializeField]
    private float _distanceWithFront = 0.2f;
    [SerializeField]
    private int _maxHistorySize = 1000;

    [Header("Runtime Reference")]
    [SerializeField]
    private ChainedMovingNode _front;
    [SerializeField]
    private ChainedMovingNode _back;

    [Header("Inspec")]
    [SerializeField]
    private float _displacement = 0f;
    public float Displacement => _displacement;
    [SerializeField]
    private Vector3 _lastPos;

    private LinkedList<TransformHistoryNodeValue> _transformHistory = new LinkedList<TransformHistoryNodeValue>();
    public LinkedList<TransformHistoryNodeValue> TransformHistory => _transformHistory;

    private LinkedList<TransformHistoryNodeValue> _frontMoveHistory;
    private IDisposable _movingStream;

    public void StartMoving()
    {
        _movingStream = Observable.EveryFixedUpdate().Subscribe(_ =>
        {
            if (_front == null)
            {
                float deltaDisplacement = Vector3.Distance(_lastPos, _movedTransform.position);
                if (deltaDisplacement < float.Epsilon)
                {
                    return;
                }

                _displacement += Vector3.Distance(_lastPos, _movedTransform.position);
                _lastPos = _movedTransform.position;
                var currentNode = new TransformHistoryNodeValue(_movedTransform.position, _movedTransform.rotation, _displacement);

                RecordTranformHistory(new LinkedListNode<TransformHistoryNodeValue>(currentNode));
                return;
            }

            MoveByFollowingFront();
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

    public void SetFront(ChainedMovingNode front)
    {
        if (_front == null)
        {
            return;
        }

        _front = front;
        _frontMoveHistory = _front.TransformHistory;

        _transformHistory.Clear();
    }

    public void SetBack(ChainedMovingNode back)
    {
        _back = back;
    }

    private void MoveByFollowingFront()
    {
        if (_front == null || _frontMoveHistory == null)
        {
            return;
        }

        if (_frontMoveHistory.Count == 0)
        {
            return;
        }

        if ((_front.Displacement - _displacement) < _distanceWithFront)
        {
            return;
        }


        LinkedListNode<TransformHistoryNodeValue> nextNodeToMove;
        TransformHistoryNodeValue nextNodeToMoveValue;

        //do
        {
            nextNodeToMove = _frontMoveHistory.Last;
            _frontMoveHistory.RemoveLast();
            nextNodeToMoveValue = nextNodeToMove.Value;
            RecordTranformHistory(nextNodeToMove);
        }
        //while (_front.Displacement - nextNodeToMoveValue.Displacement > _distanceWithFront);
        
        _movedTransform.position = nextNodeToMoveValue.Position;
        _movedTransform.rotation = nextNodeToMoveValue.Rotation;
        _displacement = nextNodeToMoveValue.Displacement;

    }

    private void RecordTranformHistory(LinkedListNode<TransformHistoryNodeValue> node)
    {
        _transformHistory.AddFirst(node);

        if (_maxHistorySize < _transformHistory.Count)
        {
            _transformHistory.RemoveLast();
        }
    }

    private void Start()
    {
        _lastPos = _movedTransform.position;

        SetFront(_front);
        StartMoving();
    }

    private void OnDisable()
    {
        StopMoving();
    }
}

public class TransformHistoryNodeValue
{
    private Vector3 _position;
    public Vector3 Position => _position;
    private Quaternion _rotation;
    public Quaternion Rotation => _rotation;
    private float _displacement;
    public float Displacement => _displacement;

    public TransformHistoryNodeValue(Vector3 position, Quaternion rotation, float displacement)
    {
        _position = position;
        _rotation = rotation;
        _displacement = displacement;
    }
}
