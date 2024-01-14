using System;
using UnityEngine;
using UnityEngine.UI;

// 左右箭头切换
public class CP_PageSwitcher : MonoBehaviour {
    public enum ETravelMode {
        Circle, // 循环
        CircleRightOnly, // 右侧循环
        CircleLeftOnly, // 右侧循环
        NoCircle, // 不循环
        NoCircleKeepButton, // 不循环
    }

    public ETravelMode mode = ETravelMode.Circle;
    public Button leftArrow;
    public Button rightArrow;

    // 每页多少数据
    [SerializeField, Range(1, 50)] private int countPerPage = 1;

    // index:startIndex:rangeCount
    public Action<int, int, int> onExec;

    public int DataCount { get; private set; }

    public int PageCount {
        get { return Mathf.CeilToInt(1f * this.DataCount / this.countPerPage); }
    }

    public int currentPageIndex { get; private set; } = 0;

    protected void Awake() {
        if (this.leftArrow != null) {
            this.leftArrow.onClick.AddListener(this.OnBtnLeftClicked);
        }

        if (this.rightArrow != null) {
            this.rightArrow.onClick.AddListener(this.OnBtnRightClicked);
        }
    }

    private void OnBtnLeftClicked() {
        if (this.mode == ETravelMode.NoCircleKeepButton) {
            if (this.currentPageIndex <= 0) {
                return;
            }
        }

        --this.currentPageIndex;
        this.Exec();
    }

    private void OnBtnRightClicked() {
        if (this.mode == ETravelMode.NoCircleKeepButton) {
            if (this.currentPageIndex >= this.PageCount - 1) {
                return;
            }
        }

        ++this.currentPageIndex;
        this.Exec();
    }

    public CP_PageSwitcher SetCount(int count) {
        this.DataCount = count;

        if (this.mode != ETravelMode.NoCircleKeepButton) {
            bool isActive = this.PageCount > 1;
            this.leftArrow.gameObject.SetActive(isActive);
            this.rightArrow.gameObject.SetActive(isActive);
        }

        return this;
    }

    public bool SetCurrentIndex(int index) {
        if (0 <= index && index < this.PageCount) {
            this.currentPageIndex = index;
            return true;
        }
        else {
            this.currentPageIndex = -1;
            return false;
        }
    }

    public void Exec(bool sendMessage = true) {
        this.currentPageIndex = (this.currentPageIndex + this.PageCount) % this.PageCount;

        if (this.mode == ETravelMode.NoCircleKeepButton) {
        }
        else if (this.mode == ETravelMode.NoCircle) {
            this.leftArrow.gameObject.SetActive(this.currentPageIndex != 0);
            this.rightArrow.gameObject.SetActive(this.currentPageIndex != this.PageCount - 1);
        }
        else if (this.mode == ETravelMode.CircleRightOnly) {
            this.leftArrow.gameObject.SetActive(this.currentPageIndex != 0);
        }
        else if (this.mode == ETravelMode.CircleLeftOnly) {
            this.rightArrow.gameObject.SetActive(this.currentPageIndex != this.PageCount - 1);
        }

        if (sendMessage) {
            int startIndex = this.currentPageIndex * this.countPerPage;
            int rangeCount = Math.Min(this.countPerPage, this.DataCount - startIndex);
            this.onExec?.Invoke(this.currentPageIndex, startIndex, rangeCount);
        }
    }
}