using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public abstract class CP_Clippable : UIBehaviour, IClippable
{
    protected RectTransform _rectTransform;
    public RectTransform rectTransform { get { if (_rectTransform == null) { _rectTransform = GetComponent<RectTransform>(); } return _rectTransform; } }

    protected RectMask2D _rectMask2D;
    public RectMask2D rectMask2D { get { if (_rectMask2D == null) { _rectMask2D = GetComponentInParent<RectMask2D>(); } return _rectMask2D; } }

    protected RectTransform _rectMask2DTransform;
    public RectTransform rectMask2DTransform { get { if (_rectMask2DTransform == null) { _rectMask2DTransform = GetComponent<RectTransform>(); } return _rectMask2DTransform; } }

    [SerializeField] protected Vector4 finalClipRect = Vector4.zero;

    protected override void OnEnable()
    {
        base.OnEnable();
        rectMask2D.AddClippable(this);
        UpdateClipParent();
    }
    protected override void OnDisable()
    {
        rectMask2D.RemoveClippable(this);
        UpdateClipParent();
        base.OnDisable();
    }
    protected override void OnTransformParentChanged()
    {
        base.OnTransformParentChanged();
        UpdateClipParent();
    }

    private void UpdateClipParent()
    {
        var newMask2d = MaskUtilities.GetRectMaskForClippable(this);
        if (_rectMask2D != null && (newMask2d != _rectMask2D || !newMask2d.isActiveAndEnabled))
        {
            _rectMask2D.RemoveClippable(this);
        }
        if (newMask2d != null && newMask2d.isActiveAndEnabled)
        {
            newMask2d.AddClippable(this);
        }

        _rectMask2D = newMask2d;
    }

    public void Cull(Rect clipRect, bool validRect) { }
    public void RecalculateClipping() { UpdateClipParent(); }
    public abstract void SetClipRect(Rect value, bool validRect);

    public void SetClipSoftness(Vector2 clipSoftness)
    {        
    }
}
