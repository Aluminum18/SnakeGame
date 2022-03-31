using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMapping : MonoBehaviour
{
    [SerializeField]
    private Vector3Variable _direction;

    Vector3 buffer = Vector3.zero;
    private void Update()
    {
        buffer.x = Input.GetAxisRaw("Horizontal");
        buffer.z = Input.GetAxisRaw("Vertical");

        _direction.Value = buffer;
    }
}
