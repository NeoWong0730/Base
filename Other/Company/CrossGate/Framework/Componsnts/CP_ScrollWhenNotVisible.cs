using UnityEngine;
using UnityEngine.UI;

// serverlist
public class CP_ScrollWhenNotVisible : MonoBehaviour {
    public ScrollRect scrollRect;
    public RectTransform target;

    // 尝试调整 ScrollRect的位置，让target变得可见
    public void TryBecomeVisible(bool isLast, float height) {
        if (isLast) {
            scrollRect.verticalNormalizedPosition = 0f;
        }
        // 如果相交
        else {
            if (GetWorldRect(this.target, out Rect targetRect) && GetWorldRect(this.scrollRect.viewport, out Rect rtRect) && !targetRect.Overlaps(rtRect)) {
                float rate = height / scrollRect.content.rect.height;
                scrollRect.verticalNormalizedPosition -= 3 * rate;
            }
        }
    }

    private static readonly Vector3[] corners = new Vector3[4];
    public static bool GetWorldRect(RectTransform rt, out Rect rect) {
        if (rt != null) {
            rt.GetWorldCorners(corners);
            float x = corners[0].x;
            float y = corners[0].y;
            float w = corners[2].x - corners[0].x;
            float h = corners[1].y - corners[0].y;
            rect = new Rect(x, y, w, h);
            return true;
        }
        else {
            rect = default;
            return false;
        }
    }
}
