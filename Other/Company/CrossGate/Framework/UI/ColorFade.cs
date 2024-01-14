using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public enum FadeMode
{
    Dark2Light,
    Light2Dark,
}

public class ColorFade : MonoBehaviour
{
    private uint buffId;
    private Image image;
    private float showTime;
    private float hideTime;
    private Color color_0;      //更暗的颜色
    private Color color_1;      //更亮的颜色
    private float intervel;
    private float fadeTime;
    private bool bFade = false;
    private Action<uint> fadeOver;

    private void Start()
    {
        image = GetComponent<Image>();
    }
    

    public void StartFlash(uint _buffId, FadeMode _fadeMode, Color _color_0, Color _color_1, float _interval, float _fadeTime, float _showTime, float _hideTime, Action<uint> _fadeOver = null)
    {
        buffId = _buffId;
        image = GetComponent<Image>();
        color_0 = _color_0;
        color_1 = _color_1;
        intervel = _interval;
        fadeTime = _fadeTime;
        showTime = _showTime;
        hideTime = _hideTime;
        fadeOver = _fadeOver;
        bFade = true;
        switch (_fadeMode)
        {
            case FadeMode.Dark2Light:
                Fade_1();
                break;
            case FadeMode.Light2Dark:
                Fade_0();
                break;
            default:
                break;
        }
    }

    private void Fade_0()
    {
        image.DOColor(color_0, hideTime).onComplete += () =>
        {
            image.DOColor(color_1, showTime).onComplete += () =>
            {
                image.DOColor(color_1, intervel).onComplete += () =>
                {
                    if (bFade)
                    {
                        Fade_0();
                    }
                };
            };
        };
    }

    private void Fade_1()
    {
        image.DOColor(color_1, showTime).onComplete += () =>
        {
            image.DOColor(color_0, hideTime).onComplete += () =>
            {
                image.DOColor(color_0, intervel).onComplete += () =>
                {
                    if (bFade)
                    {
                        Fade_1();
                    }
                };
            };
        };
    }

    private void Update()
    {
        if (!bFade)
            return;
        fadeTime -= Time.deltaTime;
        if (fadeTime <= 0)
        {
            EndFade();
        }
    }

    public void EndFade()
    {
        bFade = false;
        fadeOver?.Invoke(buffId);
        fadeOver = null;
    }
}
