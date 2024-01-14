using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Light))]
public class SceneLightController : MonoBehaviour
{
    public AnimationCurve mAnimationCurve;
    private Tweener tweener;
    public float intensity;
    public float fMinTime;
    public float fMaxTime;

    //private void Awake()
    //{
    //    //Light light = GetComponent<Light>();
    //    //intensity = light.intensity;
    //}

    private void OnEnable()
    {
        Light light = GetComponent<Light>();
        light.intensity = 0;
        float time = Random.Range(fMinTime, fMaxTime);
        tweener = light.DOIntensity(intensity, time).SetEase(mAnimationCurve).SetLoops(-1, LoopType.Restart);
    }

    private void OnDisable()
    {
        tweener.Kill(true);        
    }
}
