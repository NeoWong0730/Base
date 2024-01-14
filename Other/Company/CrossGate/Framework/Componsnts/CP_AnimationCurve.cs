using System;
using System.Collections.Generic;
using UnityEngine;

public class CP_AnimationCurve : MonoBehaviour
{
    public AnimationCurve curve;
    public float fadeTime = 1f;
    public float multiple = 56f;
    [Header("Y确保在0 ~ 1之间")]
    public Action<float, float, float> onChange;
    public List<Vector3> posArray;

    public bool useCurve = true;

    private Vector3 originalPosition;
    private bool set = false;
    private float startTime;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    public CP_AnimationCurve Set(bool set)
    {
        this.set = set;
        this.startTime = Time.time;
        return this;
    }
    public CP_AnimationCurve SetPositionIndex(int index)
    {
        if (0 <= index && index < posArray.Count)
        {
            originalPosition = posArray[index];
            onChange?.Invoke(originalPosition.x, originalPosition.y, originalPosition.z);
        }
        return this;
    }

    private void Update()
    {
        if (set)
        {
            float rate = (Time.time - startTime) / fadeTime;
            if (rate < 1f)
            {
                float curveValue = curve.Evaluate(rate);
                Vector3 pos = new Vector3(originalPosition.x, originalPosition.y + curveValue * multiple, originalPosition.z);
                onChange?.Invoke(pos.x, pos.y, pos.z);
            }
            else
            {
                set = false;
            }
        }
    }
}
