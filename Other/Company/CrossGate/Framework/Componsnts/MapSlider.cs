using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Slider))]
public class MapSlider : MonoBehaviour {
    [Serializable]
    public struct Node {
        public GameObject pageGo;
        public GameObject scaleGo;
    }

    public Slider slider;
    [Range(0.001f, 0.5f)] public float threshold = 0.01f;
    public bool considerMultiSmooth = false;
    [Range(0.1f, 5f)] public float singleSmoothTime = 1f;
    public Vector2 positiveDragScale = new Vector2(1f, 1.2f);
    public Vector2 negetiveDragScale = new Vector2(1f, 0.8f);

    // public Vector2 scale = new Vector2(0.8f, 1f);
    public Node[] ctrls = new Node[0];

    public Action<int, int> onIndexed;
    public Action<bool, int, float> onSliding;

    private int _curIndex = -1;

    public int curIndex {
        get { return this._curIndex; }
        private set {
            if (this._curIndex != value) {
                this.OnIndexChanged(this._curIndex, value);
                this._curIndex = value;
            }
        }
    }

    private float preValue = -1f;

    private void Awake() {
        this.slider.minValue = 0;
        this.slider.maxValue = this.ctrls.Length - 1;
        this.slider.onValueChanged.AddListener(this.OnValueChanged);

        this.preValue = this.slider.value;
    }

    private bool allow = false;
    public float from { get; private set; }
    public float to { get; private set; }
    private float startTime;

    private void Update() {
        if (this.allow) {
            float smoothTime = this.considerMultiSmooth ? Mathf.Abs(this.to - this.from) * this.singleSmoothTime : this.singleSmoothTime;
            float rate = (Time.realtimeSinceStartup - this.startTime) / smoothTime;
            this.slider.value = Mathf.Lerp(this.from, this.to, rate);

            if (rate >= 1f) {
                this.allow = false;
            }
        }
    }

    public void OnTabClicked(int index, bool force = false) {
        if (index < 0 || index >= this.ctrls.Length) {
            return;
        }

        if (force) {
            this.allow = false;
            this.slider.value = index;
        }
        else {
            this.allow = true;
            this.from = this.slider.value;
            this.to = index;
            this.startTime = Time.realtimeSinceStartup;
        }
    }

    private void OnIndexChanged(int preIndex, int curIndex) {
        if (0 <= preIndex && preIndex < this.ctrls.Length) {
            this.ctrls[preIndex].pageGo.SetActive(false);
        }
        if (0 <= curIndex && curIndex < this.ctrls.Length) {
            this.ctrls[curIndex].pageGo.SetActive(true);
            this.onIndexed?.Invoke(preIndex, curIndex);
        }
    }

    private void OnValueChanged(float value) {
        // 正向滑动，还是逆向滑动
        bool positive = value - this.preValue >= 0f;
        this.preValue = value;

        this.curIndex = this.CalcIndex(positive);

        float scale = this.LerpScale(positive);
        if (0 <= this.curIndex && this.curIndex < this.ctrls.Length) {
            this.ctrls[this.curIndex].scaleGo.transform.localScale = new Vector3(scale, scale, scale);
            this.onSliding?.Invoke(positive, this.curIndex, this.slider.value);
        }
    }

    private int CalcIndex(bool positive) {
        float value = this.slider.value;
        int ceil = Mathf.CeilToInt(value);
        bool isEnterCeil = ceil - value < this.threshold;
        int floor = Mathf.FloorToInt(value);
        bool isEnterFloor = value - floor < this.threshold;

        if (positive) {
            return isEnterCeil ? ceil : this.curIndex;
        }
        else {
            return isEnterFloor ? floor : this.curIndex;
        }
    }

    private float LerpScale(bool positive) {
        float value = this.slider.value;
        if (positive) {
            return Mathf.Lerp(this.positiveDragScale.x, this.positiveDragScale.y, value - this.curIndex);
        }
        else {
            // 负方向有两个
            if (value > this.curIndex) {
                return Mathf.Lerp(this.positiveDragScale.y, this.positiveDragScale.x, 1 - (value - this.curIndex));
            }
            else {
                return Mathf.Lerp(this.negetiveDragScale.x, this.negetiveDragScale.y, this.curIndex - value);
            }
        }
    }
}
