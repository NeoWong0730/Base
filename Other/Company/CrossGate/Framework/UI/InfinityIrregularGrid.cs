using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

public struct VirtualEntry
{
    public int fSize;

    public int fStartPos;
    public int fEndPos;
}

public class InfinityIrregularGrid : MonoBehaviour
{
    public ScrollRect _scrollView = null;
    public RectTransform _content = null;
    public GameObject _element = null;

    public bool setCellPosition = true;
    [SerializeField] [HideInInspector] private RectOffset _padding = new RectOffset();
    [SerializeField] [HideInInspector] private Axis _startAxis = Axis.Horizontal;
    [SerializeField] [HideInInspector] private List<VirtualEntry> _virtualEntites = new List<VirtualEntry>();
    [SerializeField] [HideInInspector] private int _spacing = 0;
    [SerializeField] [HideInInspector] private int _fixedAxisSize = 1;
    [SerializeField] [HideInInspector] private int _MinSize = 1;
    [SerializeField] [HideInInspector] private int _MaxSize = 0;

    private int _indexStart = -1;
    private int _indexEnd = -1;
    private int _showCount = 0;    

    private bool isDirty = true;
    private bool isSettingDirty = true;
    private bool isContentSizeDirty = true;
    private bool hasCellChange = true;
    private bool hasCellChangeForce = true;

    private Queue<InfinityGridCell> mCellPool = new Queue<InfinityGridCell>();
    private List<InfinityGridCell> mCells = new List<InfinityGridCell>();

    public Action<InfinityGridCell> onCreateCell;
    public Action<InfinityGridCell, int> onCellChange;

    private float _lockNormalizedPosition;
    private bool _bLocked = false;

    public void SetCapacity(int capacity)
    {
        _virtualEntites.Capacity = capacity;
    }

    public int nIndexEnd
    {
        get
        {
            return _indexEnd;
        }
    }

    public int nIndexStart
    {
        get
        {
            return _indexStart;
        }
    }

    public Vector2 ContentSize
    {
        get
        {
            return Content.sizeDelta;
        }
    }
    public RectTransform Content
    {
        get
        {
            if (_content == null)
            {
                _content = ScrollView.content;
            }
            return _content;
        }
    }
    public ScrollRect ScrollView
    {
        get
        {
            if (null == _scrollView)
            {
                _scrollView = gameObject.GetComponent<ScrollRect>();
            }
            return _scrollView;
        }
    }
    public Vector2 NormalizedPosition
    {
        get
        {
            return ScrollView.normalizedPosition;
        }
        set
        {
            if (ScrollView.normalizedPosition != value)
            {
                ScrollView.normalizedPosition = value;
                isDirty = true;
            }
        }
    }
    public void SetLockNormalizedPosition(bool locked, float pos)
    {
        _bLocked = locked;
        _lockNormalizedPosition = pos;
    }

    public Axis StartAxis
    {
        get
        {
            return _startAxis;
        }
        set
        {
            if (_startAxis != value)
            {
                _startAxis = value;
                isSettingDirty = true;
            }
        }
    }
    public int GetSize(int index)
    {
        if (index < 0 || index >= _virtualEntites.Count)
            return 0;

        VirtualEntry virtualEntry = _virtualEntites[index];
        return virtualEntry.fSize;
    }

    public void SetSize(int index, int size)
    {
        if (index < 0 || index >= _virtualEntites.Count)
            return;

        if(_MaxSize < _MinSize)
        {
            size = Mathf.Max(size, _MinSize);
        }
        else
        {
            size = Mathf.Clamp(size, _MinSize, _MaxSize);
        }
        
        VirtualEntry virtualEntry = _virtualEntites[index];
        if (virtualEntry.fSize != size)
        {
            virtualEntry.fSize = size;
            _virtualEntites[index] = virtualEntry;
            isSettingDirty = true;
        }
    }

    public void Remove(int index, bool rebuildContent = false)
    {
        if (index >= 0 && index < _virtualEntites.Count)
        {
            _virtualEntites.RemoveAt(index);
            if (rebuildContent)
            {
                isSettingDirty = true;
            }
            hasCellChangeForce = true;
            hasCellChange = true;
        }
    }

    public void RemoveTop(bool rebuildContent = false)
    {
        if (_virtualEntites.Count > 0)
        {
            _virtualEntites.RemoveAt(0);
            if (rebuildContent)
            {
                isSettingDirty = true;
            }
            hasCellChangeForce = true;
            hasCellChange = true;
        }
    }

    public void RemoveTopRange(int count, bool rebuildContent = false)
    {
        count = Mathf.Min(_virtualEntites.Count, count);
        _virtualEntites.RemoveRange(0, count);
        if (rebuildContent)
        {
            isSettingDirty = true;
        }
        hasCellChangeForce = true;
        hasCellChange = true;
    }

    public void Add(int size)
    {
        int startPos = 0;
             
        if(_virtualEntites.Count > 0)
        {
            startPos = _virtualEntites[_virtualEntites.Count - 1].fEndPos + _spacing;
        }
        else
        {
            if (StartAxis == Axis.Horizontal)
            {
                startPos = _padding.top;
            }
            else
            {
                startPos = _padding.left;
            }
        }

        VirtualEntry virtualEntry = new VirtualEntry();

        if (_MaxSize < _MinSize)
        {
            size = Mathf.Max(size, _MinSize);
        }
        else
        {
            size = Mathf.Clamp(size, _MinSize, _MaxSize);
        }

        virtualEntry.fStartPos = startPos;
        virtualEntry.fSize = size;
        virtualEntry.fEndPos = size + startPos;

        _virtualEntites.Add(virtualEntry);
        isContentSizeDirty = true;
        isDirty = true;
    }

    public int Spacing
    {
        get
        {
            return _spacing;
        }
        set
        {
            if (_spacing != value)
            {
                _spacing = value;
                isSettingDirty = true;
            }
        }
    }
    public int CellCount
    {
        get
        {
            return _virtualEntites.Count;
        }
        set
        {
            if (_virtualEntites.Count == value)
                return;
            
            if (_virtualEntites.Count < value)
            {
                if (_virtualEntites.Capacity < value)
                {
                    _virtualEntites.Capacity = value;
                }

                for (int i = _virtualEntites.Count; i < _virtualEntites.Capacity; ++i)
                {
                    _virtualEntites.Add(new VirtualEntry() { fSize = _MinSize });
                }
            }
            else
            {
                _virtualEntites.RemoveRange(value, _virtualEntites.Count - value);
            }

            isSettingDirty = true;
        }
    }
    public int Left
    {
        get
        {
            return _padding.left;
        }
        set
        {
            if (_padding.left != value)
            {
                _padding.left = value;
                isSettingDirty = true;
            }
        }
    }
    public int Right
    {
        get
        {
            return _padding.right;
        }
        set
        {
            if (_padding.right != value)
            {
                _padding.right = value;
                isSettingDirty = true;
            }
        }
    }
    public int Top
    {
        get
        {
            return _padding.top;
        }
        set
        {
            if (_padding.top != value)
            {
                _padding.top = value;
                isSettingDirty = true;
            }
        }
    }
    public int Bottom
    {
        get
        {
            return _padding.bottom;
        }
        set
        {
            if (_padding.bottom != value)
            {
                _padding.bottom = value;
                isSettingDirty = true;
            }
        }
    }

    public int MinSize
    {
        get
        {
            return _MinSize;
        }
        set
        {
            if (value < 1)
            {
                _MinSize = 1;
            }
            else
            {
                _MinSize = value;
            }
        }
    }

    public int MaxSize
    {
        get
        {
            return _MaxSize;
        }
        set
        {
            if (value < 0)
            {
                _MaxSize = 0;
            }
            else
            {
                _MaxSize = value;
            }
        }
    }
    
    public void ApplySetting()
    {
        isSettingDirty = false;

        if (null == ScrollView)
        {
            Debug.LogError("UIGrid ApplySetting ScrollRect is null");
            return;
        }
        
        int endPos = 0;
        if (StartAxis == Axis.Horizontal)
        {
            endPos = _padding.top;
        }
        else
        {
            endPos = _padding.left;
        }

        VirtualEntry tmp;
        int cellCount = _virtualEntites.Count;
        for (int i = 0; i < cellCount; ++i)
        {
            tmp = _virtualEntites[i];

            tmp.fStartPos = endPos;
            tmp.fEndPos = endPos = endPos + tmp.fSize;

            endPos += Spacing;

            _virtualEntites[i] = tmp;
        }
        endPos -= Spacing;

        isContentSizeDirty = true;        
    }

    public void ApplyContentSize()
    {
        isContentSizeDirty = false;

        float realSize = 0;
        if (_virtualEntites.Count > 0)
        {
            realSize = _virtualEntites[_virtualEntites.Count - 1].fEndPos;
        }        
        
        Vector2 size;
        if (StartAxis == Axis.Horizontal)
        {            
            size.x = _padding.left + _fixedAxisSize + _padding.right;
            size.y = realSize + _padding.bottom;
        }
        else
        {            
            size.x = realSize + _padding.right;
            size.y = _padding.top + _fixedAxisSize + _padding.bottom;
        }

        if(Content.sizeDelta != size)
        {
            Content.sizeDelta = size;
            isDirty = true;
        }                
    }

    public void ApplyLayout()
    {
        isDirty = false;
        #region 计算新的开头结尾
        int newStartIndex = -1;
        int newEndIndex = -1;

        float startPos = 0f;
        float endPos = 0f;
        int _paddingStart = 0;

        //因为移除元素导致的头部偏移量
        if (StartAxis == Axis.Horizontal)
        {
            _paddingStart = _padding.top;
            startPos = Content.anchoredPosition.y;// - _padding.top;
            endPos = startPos + ScrollView.viewport.rect.height;
        }
        else
        {
            _paddingStart = _padding.left;
            startPos = -(Content.anchoredPosition.x);// - _padding.left);
            endPos = startPos + ScrollView.viewport.rect.width;
        }

        bool startRecode = false;
        _showCount = 0;        

        if (true)
        {
            for (int i = _virtualEntites.Count - 1; i >= 0; --i)
            {
                VirtualEntry entry = _virtualEntites[i];
                if (entry.fStartPos >= endPos)
                {
                    continue;
                }

                if (entry.fEndPos <= startPos)
                {
                    //因为从大到小的遍历 所以直接就可以结束了
                    break;
                }

                if (!startRecode)
                {
                    startRecode = true;
                    newEndIndex = i;
                }

                newStartIndex = i;

                ++_showCount;
            }
        }
        else
        {
            int cellCount = _virtualEntites.Count;
            for (int i = 0; i < cellCount; ++i)
            {
                VirtualEntry entry = _virtualEntites[i];
                if (entry.fEndPos <= startPos)
                {
                    continue;
                }

                if (entry.fStartPos >= endPos)
                {
                    //因为从小到大的遍历 所以直接就可以结束了
                    break;
                }

                if (!startRecode)
                {
                    startRecode = true;
                    newStartIndex = i;
                }

                newEndIndex = i;

                ++_showCount;
            }
        }
        #endregion

        int head = newStartIndex - _indexStart;
        int tail = _indexEnd - newEndIndex;

        _indexStart = newStartIndex;
        _indexEnd = newEndIndex;

        #region 去除多余的开头结尾
        if (head > 0)
        {
            int removeCount = Mathf.Min(head, mCells.Count);
            if (removeCount > 0)
            {
                for (int i = 0; i < removeCount; ++i)
                {
                    Collect(mCells[i]);
                }
                mCells.RemoveRange(0, removeCount);
            }
        }

        if (tail > 0)
        {
            int removeCount = Mathf.Min(tail, mCells.Count);
            if (removeCount > 0)
            {
                for (int i = mCells.Count - 1; i >= mCells.Count - removeCount; --i)
                {
                    Collect(mCells[i]);
                }
                mCells.RemoveRange(mCells.Count - removeCount, removeCount);
            }
        }
        #endregion

        #region 添加不够的开头结尾
        if (head < 0)
        {
            int addCount = Mathf.Min(-head, _showCount);
            for (int i = 0; i < addCount; ++i)
            {
                mCells.Insert(0, Alloc());
            }
        }

        if (tail < 0)
        {
            int addCount = Mathf.Min(-tail, _showCount);
            for (int i = 0; i < addCount; ++i)
            {
                mCells.Add(Alloc());
            }
        }
        #endregion

        #region 刷新列表Index
        InfinityGridCell cell = null;
        for (int i = 0; i < mCells.Count; ++i)
        {
            cell = mCells[i];
            int oldIndex = cell.nIndex;
            cell.SetIndex(i + _indexStart);

            VirtualEntry entry = _virtualEntites[i + _indexStart];

            if (_startAxis == Axis.Horizontal)
            {
                cell.mRootTransform.localPosition = new Vector3(_padding.left, -(entry.fStartPos + _padding.top), 0);
                cell.mRootTransform.sizeDelta = new Vector2(cell.mRootTransform.sizeDelta.x, entry.fSize);
            }
            else
            {
                cell.mRootTransform.localPosition = new Vector3(entry.fStartPos + _padding.left, -_padding.top, 0);
                cell.mRootTransform.sizeDelta = new Vector2(entry.fSize, cell.mRootTransform.sizeDelta.y);
            }

            if (cell.nIndex != oldIndex)
            {
                hasCellChange = true;
            }
        }
        #endregion               

        if ((_indexStart == 0 || _showCount == 0) && _virtualEntites.Count > 0 && _virtualEntites[0].fStartPos > _paddingStart)
        {
            isSettingDirty = true;
            _bLocked = true;
            _lockNormalizedPosition = 1;
        }
    }
    public void Clear()
    {
        CellCount = 0;

        for (int i = 0; i < mCells.Count; ++i)
        {
            InfinityGridCell cell = mCells[i];
            if (cell != null && cell.mRootTransform != null)
            {
                cell.BindUserData(null);
                DestroyImmediate(cell.mRootTransform.gameObject);
            }
        }

        mCells.Clear();

        while (mCellPool.Count > 0)
        {
            InfinityGridCell cell = mCellPool.Dequeue();
            if (cell != null && cell.mRootTransform != null)
            {
                cell.BindUserData(null);
                DestroyImmediate(cell.mRootTransform.gameObject);
            }
        }

        _indexStart = -1;
        _indexEnd = -1;
    }
    private InfinityGridCell Alloc()
    {
        InfinityGridCell cell = null;
        if (mCellPool.Count > 0)
        {
            cell = mCellPool.Dequeue();
        }
        else
        {
            GameObject go = GameObject.Instantiate(_element);

            cell = new InfinityGridCell();
            cell.SetIndex(-1);
            cell.BindGameObject(go);

            cell.mRootTransform.SetParent(Content, false);
            cell.mRootTransform.localScale = Vector3.one;
            cell.mRootTransform.pivot = new Vector2(0, 1);

            onCreateCell?.Invoke(cell);
        }

        cell.mRootTransform.anchoredPosition3D = Vector3.zero;
        cell.mRootTransform.gameObject.SetActive(true);
        return cell;
    }
    private void Collect(InfinityGridCell uIGridElement)
    {
        mCellPool.Enqueue(uIGridElement);
        uIGridElement.SetIndex(-1);
        uIGridElement.mRootTransform.gameObject.SetActive(false);
    }
    private void OnValueChanged(Vector2 v)
    {        
        isDirty = true;
        if (Content != ScrollView.content)
        {
            Content.anchoredPosition = ScrollView.content.anchoredPosition;
        }
    }    
    private void Awake()
    {
        if (null != ScrollView)
        {
            _scrollView.onValueChanged.AddListener(OnValueChanged);
            Content.anchorMin = new Vector2(0f, 1f);//new Vector2(0.5f, 1f);
            Content.anchorMax = new Vector2(0f, 1f);//new Vector2(0.5f, 1f);
            Content.pivot = new Vector2(0f, 1f);//new Vector2(0.5f, 1f);
        }
    }
    public void Update()
    {
        if (isSettingDirty)
        {
            ApplySetting();
        }

        if(isContentSizeDirty)
        {
            ApplyContentSize();
        }

        if (_bLocked)
        {
            if (StartAxis == Axis.Horizontal)
            {
                NormalizedPosition = new Vector2(ScrollView.normalizedPosition.x, _lockNormalizedPosition);
            }
            else
            {
                NormalizedPosition = new Vector2(_lockNormalizedPosition, ScrollView.normalizedPosition.y);
            }
        }

        if (isDirty)
        {
            ApplyLayout();
        }

        if (hasCellChange && onCellChange != null)
        {
            InfinityGridCell cell;
            for (int i = 0; i < mCells.Count; ++i)
            {
                cell = mCells[i];
                if (cell.bDirty || hasCellChangeForce)
                {
                    cell.SetDirty(false);
                    onCellChange(cell, cell.nIndex);
                }
            }
            hasCellChange = false;
        }
    }

    public void ForceRefreshActiveCell()
    {
        InfinityGridCell cell;
        for (int i = 0; i < mCells.Count; ++i)
        {
            cell = mCells[i];
            cell.SetDirty(true);
        }
        hasCellChange = true;
    }
    public InfinityGridCell GetItemByIndex(int index)
    {
        if (_indexStart < 0 || _indexEnd < 0)
        {
            return null;
        }

        if (index < _indexStart || index > _indexEnd)
        {
            return null;
        }

        for (int i = 0; i < mCells.Count; ++i)
        {
            if (mCells[i].nIndex == index)
                return mCells[i];
        }
        return null;
    }

    public bool IsGreaterViewport()
    {
        if (StartAxis == Axis.Horizontal)
        {
            return Content.sizeDelta.y > ScrollView.viewport.rect.height;
        }
        else
        {
            return Content.sizeDelta.x > ScrollView.viewport.rect.width;
        }
    }

    public IReadOnlyList<InfinityGridCell> GetCells()
    {
        return mCells;
    }    
}
