using UnityEngine;

public class CP_ScrolCircleListItem : MonoBehaviour
{
    public System.Action<CP_ScrolCircleListItem, bool, bool> onChange = delegate { };
    private bool _inCircle = false;
    public bool inCircle
    {
        get
        {
            return _inCircle;
        }
        set
        {
            if (_inCircle != value)
            {
                _inCircle = value;
                OnChange(!_inCircle, _inCircle);
            }
        }
    }

    public uint id;
    public int index;

    public RectTransform rectTransform;
    public Transform binder;
    public Vector3 originalPosition;
    public System.Action<CP_ScrolCircleListItem, bool, bool> onCircleStateChanged;

    private void OnChange(bool oldState, bool newState)
    {
        onChange(this, oldState, newState);
        if (newState)
        {
            SetPosition(Vector3.zero);
            SetScale(1.1f);
        }
    }
    public void SetPosition(Vector3 pos)
    {
        rectTransform.anchoredPosition3D = pos;
    }
    public void SetScale(float scale = 1f)
    {
        rectTransform.localScale = new Vector3(scale, scale, scale);
    }
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent, false);
    }
    public void SetOriginalPosition()
    {
        SetPosition(originalPosition);
    }
    public void SetSiblingIndex()
    {
        transform.SetSiblingIndex(index);
    }
    public void SetAnchor(bool flag)
    {
        if(flag)
        {
            rectTransform.anchorMin = new Vector3(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector3(0.5f, 0.5f);
        }
        else // 中心点
        {
            rectTransform.anchorMin = new Vector3(0.5f, 1f);
            rectTransform.anchorMax = new Vector3(0.5f, 1f);
        }
    }
    public void OnCircleStateChanged(bool oldState, bool newState)
    {
        inCircle = newState;
        onCircleStateChanged?.Invoke(this, oldState, newState);
    }
}
