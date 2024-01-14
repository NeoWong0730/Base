using Lib.Core;
using UnityEngine;
using UnityEngine.UI;

public class ScrollTabCollector : TabCollector {
    public ScrollRect scrollRect;

    public bool HorOrVer {
        get {
            bool horOrVer = this.scrollRect.horizontal && !this.scrollRect.vertical;
            if (!this.scrollRect.horizontal && this.scrollRect.vertical) {
                horOrVer = false;
            }
            return horOrVer;
        }
    }

    public void ResetPosition(float normalizedPosition = 0f) {
        bool horOrVer = this.HorOrVer;
        if (horOrVer) {
            this.scrollRect.horizontalNormalizedPosition = normalizedPosition;
        }
        else {
            this.scrollRect.verticalNormalizedPosition = normalizedPosition;
        }
    }

    public override bool SetSelected(uint id) {
        bool result = this.TrySetSelected(id);
        if (result) {
            this.MoveTo(id);
        }
        return result;
    }
    private TabCollector MoveTo(uint id, float duration = 0.3f) {
        int index = this.idList.IndexOf(id);
        this.DoMoveTo(index, duration);
        return this;
    }

    // 当前位置移动到特定位置
    private Timer timer;

    private void DoMoveTo(int index, float duration = 0.3f) {
        bool horOrVer = this.HorOrVer;
        // scroll如果垂直： 从上到下1 ~ 0
        // scroll如果水平： 从左到右0 ~ 1
        // 和anchor以及layotGroup的设置没有关系
        float toRate = 1f * (index + 1) / this.idCount;
        toRate = horOrVer ? toRate : 1 - toRate;
        float fromRate = horOrVer ? this.scrollRect.horizontalNormalizedPosition : this.scrollRect.verticalNormalizedPosition;

        this.timer?.Cancel();
        this.timer = Timer.Register(duration, null, (dt) => {
            float rate = Mathf.Lerp(fromRate, toRate, dt / duration);
            this.ResetPosition(rate);
        });
    }
    private void OnDestroy() {
        timer?.Cancel();
    }
}