using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGridAdapter : MonoBehaviour {

    Rect tempRect;
    // Use this for initialization
    private void Awake()
    {
        //目前只设置了水平方向大小
        var grid = GetComponentInChildren<GridLayoutGroup>();
        if (grid != null)
        {
            tempRect = GetComponent<RectTransform>().rect;
            grid.cellSize = new Vector2(tempRect.width / grid.constraintCount - grid.spacing.x / grid.constraintCount, grid.cellSize.y);
        }
    }
}
