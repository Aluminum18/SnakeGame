using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private FloatVariable _configRoundTime;

    [Header("Config")]
    [SerializeField]
    private FloatVariableToFillImage _timeRemainImage;

    public void SetUpTimeImage()
    {
        _timeRemainImage.MaxValue = _configRoundTime.Value;
    }

    private void OnEnable()
    {
        SetUpTimeImage();
    }
}
