using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenMaterialProperty : MonoBehaviour
{
    [SerializeField]
    private Renderer _renderer;
    [SerializeField]
    private CanvasRenderer _canvasRenderer;
    [SerializeField]
    private LeanTweenType _tweenType;
    [SerializeField]
    private float _duration = 0.5f;

    [Header("Inspec")]
    [SerializeField]
    private string _targetProps;

    public void SetTarget(string propName)
    {
        _targetProps = propName;
    }

    public void SetInitPropValue(float init)
    {
        _renderer.material.SetFloat(_targetProps, init);
    }

    public void SetFloat(float value)
    {
        _renderer.material.SetFloat(_targetProps, value);
    }

    private LTDescr _tweening;
    public void TweenFloat(float to)
    {
        float current = _renderer.material.GetFloat(_targetProps);
        _tweening = LeanTween.value(current, to, _duration).setEase(_tweenType).setOnUpdate(value =>
        {
            _renderer.material.SetFloat(_targetProps, value);
        });
    }

    public void TweenFloatCanvas(float to)
    {
        float current = _canvasRenderer.GetMaterial(0).GetFloat(_targetProps);
        _tweening = LeanTween.value(current, to, _duration).setEase(_tweenType).setOnUpdate(value =>
        {
            _canvasRenderer.GetMaterial(0).SetFloat(_targetProps, value);
        });
    }

    private void OnDisable()
    {
        if (_tweening == null)
        {
            return;
        }

        LeanTween.cancel(_tweening.id);
    }
}
