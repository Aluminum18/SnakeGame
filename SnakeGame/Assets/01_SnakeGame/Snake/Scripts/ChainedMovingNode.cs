using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

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

    [Header("Unity Events")]
    [SerializeField]
    private UnityEvent _onScaleChanged;

    [Header("Inspec")]
    [SerializeField]
    private float _displacement = 0f;
    public float Displacement => _displacement;
    [SerializeField]
    private Vector3 _lastPos;

    private Vector3 _originScale;

    private LinkedList<TransformHistoryNodeValue> _transformHistory = new LinkedList<TransformHistoryNodeValue>();
    public LinkedList<TransformHistoryNodeValue> TransformHistory => _transformHistory;

    private LinkedList<TransformHistoryNodeValue> _frontMoveHistory;
    private IDisposable _movingStream;

    public void StartMoving()
    {
        _movingStream = Observable.EveryUpdate().Subscribe(_ =>
        {
            if (_front == null)
            {
                RecordNewTransformHistory();
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
        if (front == null)
        {
            return;
        }

        _front = front;

        _transformHistory = front.TransformHistory;
        front.ResetTransformHistory();

        _frontMoveHistory = front.TransformHistory;

        SetInitPos();
    }

    public void SetBack(ChainedMovingNode back)
    {
        _back = back;
    }

    public void ResetTransformHistory()
    {
        _transformHistory = new LinkedList<TransformHistoryNodeValue>();
    }

    private void RecordNewTransformHistory()
    {
        float deltaDisplacement = Vector3.Distance(_lastPos, _movedTransform.position);
        if (deltaDisplacement < float.Epsilon)
        {
            return;
        }

        _displacement += Vector3.Distance(_lastPos, _movedTransform.position);
        _lastPos = _movedTransform.position;
        var currentNode = new TransformHistoryNodeValue(_movedTransform.position, _movedTransform.rotation, _movedTransform.localScale, _displacement);

        AddHistoryNode(new LinkedListNode<TransformHistoryNodeValue>(currentNode));
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

        nextNodeToMove = _frontMoveHistory.Last;
        _frontMoveHistory.RemoveLast();
        nextNodeToMoveValue = nextNodeToMove.Value;
        AddHistoryNode(nextNodeToMove);

        MoveToNode(nextNodeToMoveValue);
    }

    private void MoveToNode(TransformHistoryNodeValue nextNodeToMoveValue)
    {
        _movedTransform.position = nextNodeToMoveValue.Position;
        _movedTransform.rotation = nextNodeToMoveValue.Rotation;
        _movedTransform.localScale = nextNodeToMoveValue.Scale;
        if (nextNodeToMoveValue.Scale != _originScale)
        {
            _onScaleChanged.Invoke();
        }

        _displacement = nextNodeToMoveValue.Displacement;
    }

    private void AddHistoryNode(LinkedListNode<TransformHistoryNodeValue> node)
    {
        _transformHistory.AddFirst(node);

        if (_maxHistorySize < _transformHistory.Count)
        {
            _transformHistory.RemoveLast();
        }
    }

    private void SetInitPos()
    {
        var history = _transformHistory.First;

        while (history != null)
        {
            var historyValue = history.Value;
            if (_front.Displacement - historyValue.Displacement < _distanceWithFront)
            {
                history = history.Next;
                continue;
            }

            MoveToNode(historyValue);
            break;
        }
    }


    private void OnEnable()
    {
        _lastPos = _movedTransform.position;
        _originScale = _movedTransform.localScale;
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
    private Vector3 _scale;
    public Vector3 Scale => _scale;
    private float _displacement;
    public float Displacement => _displacement;

    public TransformHistoryNodeValue(Vector3 position, Quaternion rotation, Vector3 scale, float displacement)
    {
        _position = position;
        _rotation = rotation;
        _scale = scale;
        _displacement = displacement;
    }
}
