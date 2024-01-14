using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LayoutElementWidthSetter : MonoBehaviour {
    public RectTransform[] trs = Array.Empty<RectTransform>();
    public GridLayoutGroup grid;
    public RectTransform totalWidth;

    private int count = 2;

    public void Set(int elementCount) {
        this.count = elementCount;

        var width = totalWidth.rect.width - (elementCount - 1) * grid.spacing.x;
        width /= elementCount;
        
        grid.cellSize = new Vector2(width, 0);

        foreach (var tr in trs) {
            if (tr != null) {
                var sizeData = tr.sizeDelta;
                tr.sizeDelta = new Vector2(width, sizeData.y);
            }
        }
    }

    [ContextMenu(nameof(SetInner))]
    public void SetInner() => this.Set(this.count);
}