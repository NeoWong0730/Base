using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cp_HorCoupleScrollRect : MonoBehaviour
{
    public RectTransform leftRectTransform;
    public ScrollRect leftRect;
    public RectTransform rightRectTransform;

    public GridLayoutGroup leftGrid;
    public GridLayoutGroup rightGrid;

    public float totalWidth = 840f;

    // 单个cell的宽度
    public float rightCellWidth = 128f;
    public float rightMaxWidth = 360f;

    //[Range(20f, 850f)]
    //public float testRightWidth = 300f;

    public void SetCouple(float rightWidth)
    {
        // left
        rightWidth = rightWidth > rightMaxWidth ? rightMaxWidth : rightWidth;
        Vector2 sizeDelta = new Vector2(totalWidth - rightWidth, leftRectTransform.sizeDelta.y);
        leftRectTransform.sizeDelta = sizeDelta;

        // 靠近clipArea最右边
        leftRect.horizontalNormalizedPosition = 1f;

        // right
        float diffWidth = rightWidth - rightRectTransform.sizeDelta.x;
        sizeDelta = new Vector2(rightWidth, leftRectTransform.sizeDelta.y);
        rightRectTransform.sizeDelta = sizeDelta;

        Vector3 position = rightRectTransform.anchoredPosition3D;
        position = new Vector3(position.x - diffWidth, position.y, position.z);
        rightRectTransform.anchoredPosition3D = position;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        SetCouple(testRightWidth);
    //    }
    //}
}
