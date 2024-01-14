using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 左右箭头切换
[DisallowMultipleComponent]
public class LRArrowSwitcher : MonoBehaviour {
    public enum ETravelMode {
        Circle, // 循环
        CircleRightOnly, // 右侧循环
        CircleLeftOnly, // 右侧循环
        NoCircle, // 不循环
    }

    public ETravelMode mode = ETravelMode.Circle;
    public Button leftArrow;
    public Button rightArrow;

    public Action<int, uint> onExec;

    private IList<uint> ids = new List<uint>();

    public int Count {
        get { return ids.Count; }
    }

    public int currentIndex { get; private set; } = 0;

    private void Awake() {
        leftArrow?.onClick.AddListener(OnBtnLeftClicked);
        rightArrow?.onClick.AddListener(OnBtnRightClicked);
    }

    public LRArrowSwitcher SetData(IList<uint> ids) {
        this.ids = ids ?? this.ids;

        bool isActive = Count > 1;
        leftArrow.gameObject.SetActive(isActive);
        rightArrow.gameObject.SetActive(isActive);

        return this;
    }

    public LRArrowSwitcher SetCurrentIndex(int index) {
        this.currentIndex = index;
        return this;
    }

    public void Exec() {
        currentIndex = (currentIndex + Count) % Count;

        if (mode == ETravelMode.NoCircle) {
            leftArrow.gameObject.SetActive(currentIndex != 0);
            rightArrow.gameObject.SetActive(currentIndex != Count - 1);
        }
        else if (mode == ETravelMode.CircleRightOnly) {
            leftArrow.gameObject.SetActive(currentIndex != 0);
        }
        else if (mode == ETravelMode.CircleLeftOnly) {
            rightArrow.gameObject.SetActive(currentIndex != Count - 1);
        }

        onExec?.Invoke(currentIndex, ids[currentIndex]);
    }

    private void OnBtnLeftClicked() {
        --currentIndex;
        Exec();
    }

    private void OnBtnRightClicked() {
        ++currentIndex;
        Exec();
    }
}