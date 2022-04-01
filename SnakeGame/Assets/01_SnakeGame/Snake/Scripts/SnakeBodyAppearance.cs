using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodyAppearance : MonoBehaviour
{
    [SerializeField]
    private Renderer _modelRenderer;

    public void FirstAppear()
    {
        if (_modelRenderer.enabled)
        {
            return;
        }

        _modelRenderer.enabled = true;
    }
}
