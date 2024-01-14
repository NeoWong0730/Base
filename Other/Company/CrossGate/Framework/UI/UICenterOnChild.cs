using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using DG.Tweening;
using Lib.Core;
/// <summary>
/// 滑动元素居中
/// </summary>
public class UICenterOnChild : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    #region 数据定义
    public delegate void OnCenterHandler(GameObject centerChild);
    public OnCenterHandler onCenter; //居中事件
    private Canvas canvas; //画布
    private ScrollRect scrollRect; //卷轴矩阵(设置Pivot(0.5f,0.5f))
    private RectTransform content; //卷轴内容
    private GridLayoutGroup grid; //排序
    private List<Transform> items = new List<Transform>(); //子列表
    private List<float> childrenPos = new List<float>(); //子坐标
    private float targetPagePosition = 0f; //目标页签坐标
    private int currentPage = 0;  //当前页签
    private int pageCount;        //页签总数

    public float smoothTime = 0.5f; //平滑时间
    public AnimationCurve scaleCurve; //动画曲线
    private Vector3 midPos = Vector3.zero; //中点坐标
    private Tweener tweener; //Dotween动画
    #endregion
    #region 系统函数
    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            DebugUtil.LogError("UICenterOnChild：No Canvas");
            return;
        }
        scrollRect = GetComponent<ScrollRect>();
        if (scrollRect == null)
        {
            DebugUtil.LogError("UICenterOnChild：No ScrollRect");
            return;
        }
        content = scrollRect.content;
        if (content == null)
        {
            DebugUtil.LogError("UICenterOnChild：No ScrollRect's Content");
            return;
        }
        grid = content.GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            DebugUtil.LogError("UICenterOnChild：No GridLayoutGroup");
            return;
        }

        scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        scrollRect.inertia = false;
        SetAnimationCurve();
        InitPageArray();
    }
    void Update()
    {
        UpdateScrollView();
    }
    #endregion
    #region 初始化
    /// <summary>
    /// 设置动画
    /// </summary>
    public void SetAnimationCurve()
    {
        Keyframe[] scks = new Keyframe[]
        {
            new Keyframe(0f,1f),
            new Keyframe(0.5f,0.7f),
        };
        scaleCurve = new AnimationCurve(scks);
        scaleCurve.preWrapMode = WrapMode.PingPong;
        scaleCurve.postWrapMode = WrapMode.PingPong;
    }
    /// <summary>
    /// 初始化获取元素总个数
    /// </summary>
    public void InitPageArray()
    {
        items.Clear();
        childrenPos.Clear();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        foreach (Transform item in content)
        {
            if (item.gameObject.activeSelf && !items.Contains(item))
                items.Add(item);
        }
        pageCount = items.Count;
        if (scrollRect.horizontal)
        {
            float childPos = content.rect.width * 0.5f - grid.cellSize.x * 0.5f;
            childrenPos.Add(childPos);
            for (int i = 1; i < pageCount; i++)
            {
                childPos -= grid.cellSize.x + grid.spacing.x;
                childrenPos.Add(childPos);
            }
        }
        else if (scrollRect.vertical)
        {
            float childPos = content.rect.height * 0.5f - grid.cellSize.y * 0.5f;
            childrenPos.Add(childPos);
            for (int i = 0; i < pageCount; i++)
            {
                childPos -= grid.cellSize.y + grid.spacing.y;
                childrenPos.Add(childPos);
            }
        }
    }
    /// <summary>
    /// 更新滚动列表
    /// </summary>
    /// <param name="fValue"></param>
    private void UpdateScrollView()
    {
        midPos = scrollRect.transform.position;
        if (scrollRect.horizontal)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Transform trans = items[i].transform;
                float value = Mathf.Abs(canvas.worldCamera.WorldToViewportPoint(trans.position).x - canvas.worldCamera.WorldToViewportPoint(midPos).x);
                float scaleValue = GetScaleValue(value);
                UpdateItem(trans, scaleValue);
            }
        }
        else if (scrollRect.vertical)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Transform trans = items[i].transform;
                float value = Mathf.Abs(canvas.worldCamera.WorldToViewportPoint(trans.position).y - canvas.worldCamera.WorldToViewportPoint(midPos).y);
                float scaleValue = GetScaleValue(value);
                UpdateItem(items[i], scaleValue);
            }
        }
    }
    /// <summary>
    /// 绘制辅助线
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < items.Count; i++)
        {
            Transform trans = items[i].transform;
            Gizmos.DrawLine(trans.position, midPos);
        }
    }
    /// <summary>
    /// 得到缩放值
    /// </summary>
    /// <param name="sliderValue"></param>
    /// <param name="added"></param>
    /// <returns></returns>
    private float GetScaleValue(float sliderValue)
    {
        float scaleValue = scaleCurve.Evaluate(sliderValue);
        return scaleValue;
    }
    /// <summary>
    /// 更新模版
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="scaleValue"></param>
    private void UpdateItem(Transform tr, float scaleValue)
    {
        Vector3 targetScale = Vector3.zero;
        targetScale.x = targetScale.y = scaleValue;
        tr.localScale = targetScale;
    }
    /// <summary>
    /// 寻找最终点
    /// </summary>
    /// <param name="currentPos"></param>
    /// <returns></returns>
    private float FindClosestPos(float currentPos)
    {
        int childIndex = 0;
        float closest = 0;
        float distance = Mathf.Infinity;
        for (int i = 0; i < childrenPos.Count; i++)
        {
            float p = childrenPos[i];
            float d = Mathf.Abs(p - currentPos);
            if (d < distance)
            {
                distance = d;
                closest = p;
                childIndex = i;
            }
        }
        SetCurrentPageIndex(childIndex, true);
        return closest;
    }
    /// <summary>
    /// 线性动画至目标
    /// </summary>
    /// <param name="targetValue"></param>
    /// <param name="needTween"></param>
    private void LerpTweenToTarget(float targetValue, bool needTween = false)
    {
        Vector3 v = content.anchoredPosition3D;
        if (scrollRect.horizontal)
            v.x = targetValue;
        else if (scrollRect.vertical)
            v.y = targetValue;

        if (!needTween)
        {
            content.anchoredPosition3D = v;
        }
        else
        {
            if (tweener != null)
                tweener.Pause();
            tweener = content.DOLocalMove(v, smoothTime, true);
        }
    }
    #endregion
    #region 响应事件
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (tweener != null)
            tweener.Pause();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (scrollRect.horizontal)
        {
            targetPagePosition = FindClosestPos(content.anchoredPosition3D.x);
        }
        else if (scrollRect.vertical)
        {
            targetPagePosition = FindClosestPos(content.anchoredPosition3D.y);
        }
    }
    #endregion
    #region 功能提供
    /// <summary>
    /// 向左移动一个元素
    /// </summary>
    /// <param name="needTween"></param>
    public void ToLeft(bool needTween = false)
    {
        if (currentPage > 0)
        {
            currentPage = currentPage - 1;
            targetPagePosition = childrenPos[currentPage];
            LerpTweenToTarget(targetPagePosition, needTween);
        }
    }
    /// <summary>
    /// 向右移动一个元素
    /// </summary>
    /// <param name="needTween"></param>
    public void ToRight(bool needTween = false)
    {
        if (currentPage < pageCount - 1)
        {
            currentPage = currentPage + 1;
            targetPagePosition = childrenPos[currentPage];
            LerpTweenToTarget(targetPagePosition, needTween);
        }
    }
    /// <summary>
    /// 设置当前页码
    /// </summary>
    /// <param name="index"></param>
    /// <param name="needTween"></param>
    public void SetCurrentPageIndex(int index, bool needTween = false)
    {
        currentPage = index;
        targetPagePosition = childrenPos[currentPage];
        GameObject centerChild = content.GetChild(currentPage).gameObject;
        if (null != onCenter) onCenter(centerChild);
        LerpTweenToTarget(targetPagePosition, needTween);
    }
    /// <summary>
    /// 获取当前页码
    /// </summary>
    /// <returns></returns>
    public int GetCurrentPageIndex()
    {
        return currentPage;
    }
    /// <summary>
    /// 获取总页数
    /// </summary>
    /// <returns></returns>
    public int GetTotalPages()
    {
        return pageCount;
    }
    #endregion
}