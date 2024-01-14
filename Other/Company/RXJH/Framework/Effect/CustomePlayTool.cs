using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.UI;
public class CustomePlayTool : MonoBehaviour
{
    public List<CustomPlayObj> CustomObj = new List<CustomPlayObj>();
    Sequence seq;
    private void OnEnable()
    {
        seq = DOTween.Sequence();
        for (int i=0;i< CustomObj.Count;i++)
        {
            int index = i;
            seq.InsertCallback(CustomObj[i].show, () =>
            {
                DelayActiveCallBack(index, CustomObj[index].isActive);
            });
        }
    }
    private void Awake()
    {

    }
    private void Update()
    {
        
    }
    private void OnDisable()
    {
        seq.Kill();
    }
    private void OnDestroy()
    {
        seq.Kill();
    }
    private void DelayActiveCallBack(int index,bool isShow)
    {
        CustomObj[index].obj.SetActive(isShow);
    }
}
