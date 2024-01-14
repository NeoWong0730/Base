using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScrollGridVertical : MonoBehaviour
{
    public GameObject tempCell;//模板cell，以此为目标，克隆出每个cell。
    private int cellCount;//要显示数据的总数。
    private float cellWidth;
    private float cellHeight;

    private List<System.Action<ScrollGridCell>> onCellUpdateList = new List<System.Action<ScrollGridCell>>();
    private ScrollRect scrollRect;

    private int row;//克隆cell的GameObject数量的行。
    public int col;//克隆cell的GameObject数量的列。

    private System.Action<ScrollGridCell> onCreateCell;
    private List<GameObject> cellList = new List<GameObject>();
    private Dictionary<GameObject, ScrollGridCell> ceils = new Dictionary<GameObject, ScrollGridCell>();


    private Coroutine coroutine = null;

    private void Awake()
    {
        Init_GetView();
    }
    /// <summary>
    /// 初始化(界面动态搭建)
    /// </summary>
    private void Init_CreateView()
    {
        if (tempCell == null)
        {
            Debug.LogError("tempCell不能为空！");
            return;
        }
        this.tempCell.SetActive(false);

        //创建ScrollRect下的viewpoint和content节点。
        this.scrollRect = gameObject.AddComponent<ScrollRect>();
        this.scrollRect.vertical = true;
        this.scrollRect.horizontal = false;
        GameObject viewport = new GameObject("viewport", typeof(RectTransform));
        viewport.transform.parent = transform;
        this.scrollRect.viewport = viewport.GetComponent<RectTransform>();
        GameObject content = new GameObject("content", typeof(RectTransform));
        content.transform.parent = viewport.transform;
        this.scrollRect.content = content.GetComponent<RectTransform>();

        //设置视野viewport的宽高和根节点一致。
        this.scrollRect.viewport.localScale = Vector3.one;
        this.scrollRect.viewport.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        this.scrollRect.viewport.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        this.scrollRect.viewport.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
        this.scrollRect.viewport.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
        this.scrollRect.viewport.anchorMin = Vector2.zero;
        this.scrollRect.viewport.anchorMax = Vector2.one;

        //设置viewpoint的mask。
        this.scrollRect.viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;
        Image image = this.scrollRect.viewport.gameObject.AddComponent<Image>();
        Rect viewRect = this.scrollRect.viewport.rect;
        image.sprite = Sprite.Create(new Texture2D(1, 1), new Rect(Vector2.zero, Vector2.one), Vector2.zero);

        //获取模板cell的宽高。
        Rect tempRect = tempCell.GetComponent<RectTransform>().rect;
        this.cellWidth = tempRect.width;
        this.cellHeight = tempRect.height;

        //设置viewpoint约束范围内的cell的GameObject的行列数。
        //this.col = (int)(this.scrollRect.viewport.rect.width / this.cellWidth);
        //this.col = Mathf.Max(1, this.col);
        this.row = Mathf.CeilToInt(this.scrollRect.viewport.rect.height / this.cellHeight);

        //初始化content。
        this.scrollRect.content.localScale = Vector3.one;
        this.scrollRect.content.offsetMax = new Vector2(0, 0);
        this.scrollRect.content.offsetMin = new Vector2(0, 0);
        this.scrollRect.content.anchorMin = Vector2.zero;
        this.scrollRect.content.anchorMax = Vector2.one;
        this.scrollRect.onValueChanged.AddListener(this.OnValueChange);
        this.CreateCells();

    }
    /// <summary>
    /// 初始化(界面已搭建)
    /// </summary>
    private void Init_GetView()
    {
        this.tempCell.SetActive(false);

        //创建ScrollRect下的viewpoint和content节点。
        this.scrollRect = gameObject.GetComponent<ScrollRect>();
        this.scrollRect.vertical = true;
        this.scrollRect.horizontal = false;

        //设置视野viewport的宽高和根节点一致。
        this.scrollRect.viewport.localScale = Vector3.one;
        this.scrollRect.viewport.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        this.scrollRect.viewport.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        this.scrollRect.viewport.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
        this.scrollRect.viewport.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
        this.scrollRect.viewport.anchorMin = Vector2.zero;
        this.scrollRect.viewport.anchorMax = Vector2.one;

        //获取模板cell的宽高。
        Rect tempRect = tempCell.GetComponent<RectTransform>().rect;
        this.cellWidth = tempRect.width;
        this.cellHeight = tempRect.height;

        //设置viewpoint约束范围内的cell的GameObject的行列数。
        this.row = Mathf.CeilToInt(this.scrollRect.viewport.rect.height / this.cellHeight);

        //初始化content。
        this.scrollRect.content.localScale = Vector3.one;
        this.scrollRect.content.offsetMax = new Vector2(0, 0);
        this.scrollRect.content.offsetMin = new Vector2(0, 0);
        this.scrollRect.content.anchorMin = Vector2.zero;
        this.scrollRect.content.anchorMax = Vector2.one;
        this.scrollRect.content.pivot = new Vector2(0.5f, 1);
        this.scrollRect.onValueChanged.AddListener(this.OnValueChange);
        this.CreateCells();
    }
    /// <summary>
    /// 添加回调事件
    /// </summary>
    /// <param name="call"></param>
    public void AddCellListener(System.Action<ScrollGridCell> call)
    {
        this.onCellUpdateList.Add(call);
        this.RefreshAllCells();
    }
    /// <summary>
    /// 移除回调事件
    /// </summary>
    /// <param name="call"></param>
    public void RemoveCellListener(System.Action<ScrollGridCell> call)
    {
        this.onCellUpdateList.Remove(call);
    }
    /// <summary>
    /// 添加创建模版事件
    /// </summary>
    /// <param name="call"></param>
    public void AddCreateCellListener(System.Action<ScrollGridCell> call)
    {
        onCreateCell = call;
    }
    /// <summary>
    /// 移除创建模版事件
    /// </summary>
    public void RemoveCreateCellListener()
    {
        onCreateCell = null;
    }
    /// <summary>
    /// 设置ScrollGrid要显示的数据数量。
    /// </summary>
    /// <param name="count"></param>
    public void SetCellCount(int count)
    {
        this.cellCount = Mathf.Max(0, count);

        //重新调整content的高度，保证能够包含范围内的cell的anchoredPosition，这样才有机会显示。
        float newContentHeight = this.cellHeight * Mathf.CeilToInt((float)cellCount / this.col);
        float newMinY = -newContentHeight + this.scrollRect.viewport.rect.height;
        float maxY = this.scrollRect.content.offsetMax.y;
        newMinY += maxY;//保持位置
        newMinY = Mathf.Min(maxY, newMinY);//保证不小于viewport的高度。
        this.scrollRect.content.offsetMin = new Vector2(0, newMinY);
        this.CreateCells();
    }
    /// <summary>
    /// 定位
    /// </summary>
    /// <param name="value"></param>
    public void FixedPosition(float value)
    {
        value = 1f - Mathf.Clamp(value, 0f, 1f);
        this.scrollRect.verticalNormalizedPosition = value;
    }

    public void MoveToTopOrBotton(bool isBotton)
    {
        Vector3 targetPos = isBotton ? Vector3.zero : new Vector3 (this.scrollRect.content.sizeDelta.x, this.scrollRect.content.sizeDelta.y, 0);
        MoveTo(targetPos);
    }

    public void MoveTo(Vector3 position, float duration = 0.2f)
    {
        scrollRect.StopMovement();
        if (coroutine != null) { StopCoroutine(coroutine); }
        coroutine = StartCoroutine(MoveToTarget(position, duration));
    }

    protected IEnumerator MoveToTarget(Vector3 to, float duration = 0.2f)
    {
        bool running = true;
        float accumulateTime = 0f;
        // 当前位置
        Vector3 from = scrollRect.content.anchoredPosition;
        while (running)
        {
            yield return new WaitForEndOfFrame();
            accumulateTime += Time.deltaTime;
            Vector3 currentPosition;
            if (accumulateTime >= duration)
            {
                currentPosition = to;
                running = false;
                StopCoroutine(coroutine);
                coroutine = null;
            }
            else
            {
                currentPosition = Vector3.Lerp(from, to, accumulateTime / duration);
            }

            // 设置ScrollView位置
            scrollRect.content.anchoredPosition = currentPosition;
        }
    }


    /// <summary>
    /// 刷新每个cell的数据
    /// </summary>
    public void RefreshAllCells()
    {
        foreach (GameObject cell in this.cellList)
        {
            this.cellUpdate(cell);
        }
    }
    /// <summary>
    /// 刷新一个cell的数据
    /// </summary>
    /// <param name="target"></param>
    public void RefreshOneCell(GameObject target)
    {
        GameObject cell = cellList.Find(x => x == target);
        if(null!= cell)
        {
            this.cellUpdate(cell);
        }
    }
    /// <summary>
    /// 创建每个cell，并且根据行列定它们的位置，最多创建能够在视野范围内看见的个数，加上一行隐藏待进入视野的cell。
    /// </summary>
    private void CreateCells()
    {
        for (int r = 0; r < this.row + 1; r++)
        {
            for (int l = 0; l < this.col; l++)
            {
                int index = r * this.col + l;
                if (index < this.cellCount)
                {
                    if (this.cellList.Count <= index)
                    {
                        GameObject newcell = GameObject.Instantiate<GameObject>(this.tempCell);
                        newcell.SetActive(true);
                        //cell节点锚点强制设为左上角，以此方便算出位置。
                        RectTransform cellRect = newcell.GetComponent<RectTransform>();
                        cellRect.anchorMin = new Vector2(0, 1);
                        cellRect.anchorMax = new Vector2(0, 1);

                        //分别算出每个cell的位置。
                        float x = this.cellWidth / 2 + l * this.cellWidth;
                        float y = -r * this.cellHeight - this.cellHeight / 2;
                        cellRect.SetParent(this.scrollRect.content);
                        cellRect.localScale = Vector3.one;
                        cellRect.anchoredPosition3D = new Vector3(x, y, 0);

                        ScrollGridCell scrollGridCell = new ScrollGridCell();
                        ceils[newcell] = scrollGridCell;
                        scrollGridCell.BindGameObject(newcell);
                        scrollGridCell.SetObjIndex(index);
                        this.cellList.Add(newcell);
                        onCreateCell?.Invoke(scrollGridCell);
                    }
                }
            }
        }
        this.RefreshAllCells();
    }
    /// <summary>
    /// 滚动过程中，重复利用cell
    /// </summary>
    /// <param name="pos"></param>
    private void OnValueChange(Vector2 pos)
    {
        foreach (GameObject cell in this.cellList)
        {
            RectTransform cellRect = cell.GetComponent<RectTransform>();
            float dist = this.scrollRect.content.offsetMax.y + cellRect.anchoredPosition.y;
            float maxTop = this.cellHeight / 2;
            float minBottom = -((this.row + 1) * this.cellHeight) + this.cellHeight / 2;
            if (dist > maxTop)
            {
                float newY = cellRect.anchoredPosition.y - (this.row + 1) * this.cellHeight;
                //保证cell的anchoredPosition只在content的高的范围内活动，下同理
                if (newY > -this.scrollRect.content.rect.height)
                {
                    //重复利用cell，重置位置到视野范围内。
                    cellRect.anchoredPosition = new Vector3(cellRect.anchoredPosition.x, newY);
                    this.cellUpdate(cell);
                }
            }
            else if (dist < minBottom)
            {
                float newY = cellRect.anchoredPosition.y + (this.row + 1) * this.cellHeight;
                if (newY < 0)
                {
                    cellRect.anchoredPosition = new Vector3(cellRect.anchoredPosition.x, newY);
                    this.cellUpdate(cell);
                }
            }
        }
    }
    /// <summary>
    /// 所有的数据的真实行数
    /// </summary>
    private int allRow { get { return Mathf.CeilToInt((float)this.cellCount / this.col); } }
    /// <summary>
    /// cell被刷新时调用，算出cell的位置并调用监听的回调方法（Action）。
    /// </summary>
    /// <param name="cell"></param>
    private void cellUpdate(GameObject cell)
    {
        RectTransform cellRect = cell.GetComponent<RectTransform>();
        int x = Mathf.FloorToInt((cellRect.anchoredPosition.x - this.cellWidth / 2) / this.cellWidth + 0.5f);
        int y = Mathf.FloorToInt(Mathf.Abs((cellRect.anchoredPosition.y + this.cellHeight / 2) / this.cellHeight) + 0.5f);
        int index = y * this.col + x;
        ScrollGridCell scrollGridCell = ceils[cell];
        scrollGridCell.UpdatePos(x, y, index);
        if (index >= cellCount || y >= this.allRow)
        {
            //超出数据范围
            cell.SetActive(false);
        }
        else
        {
            if (cell.activeSelf == false)
            {
                cell.SetActive(true);
            }
            foreach (var call in this.onCellUpdateList)
            {
                call(scrollGridCell);
            }
        }
    }
}
