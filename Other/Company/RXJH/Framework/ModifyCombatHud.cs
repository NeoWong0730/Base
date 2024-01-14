using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ModifyCombatHud : MonoBehaviour
{

    public static bool UseTestData;
    
    [Header("-----总时长-----")]
    public float mPlayTimer;

    [Space(15)]
    [Header("-----渐显时长-----")]
    public float mShowTimer;

    [Space(15)]
    [Header("-----渐隐时长-----")]
    public float mHideTimer;

    [Space(15)]
    [Header("-----上漂时长-----")]
    public float mUpTimer;

    [Space(15)]
    [Header("-----上漂距离-----")]
    public float mUpDis;

    [Space(15)]
    [Header("-----起始字号比例-----")]
    public float mStartScale;

    [Space(15)]
    [Header("-----最大字号比例-----")]
    public float mMaxScale;

    [Space(15)]
    [Header("-----缩放持续时长-----")]
    public float mStart2MaxScaleTimer;

    [Space(15)]
    [Header("-----放缩持续时长-----")]
    public float mMax2NormalTimer;

    [Space(15)]
    [Header("-----最小字号比例-----")]
    public float mMinScale;

    public bool correct { get; private set; }

    private void Start()
    {
        UseTestData = true;
        correct = false;
    }

    public void Correct()
    {
        correct = true;
    }

    private void OnDestroy()
    {
        UseTestData = false;
    }
}



