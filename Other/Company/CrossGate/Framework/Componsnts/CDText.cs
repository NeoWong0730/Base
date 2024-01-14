using System;
using UnityEngine;
using UnityEngine.UI;

// 倒计时
[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public class CDText : MonoBehaviour {
    public enum ERefreshRate {
        PerFrame,
        PerSecond,
        PerMinute,
    }

    public bool useTimeScale = false;
    public ERefreshRate refreshRate = ERefreshRate.PerSecond;
    [Range(0.01f, 999f)] public float cd = 5f;

#if UNITY_EDITOR
    public double remainTime;
#else
    private double remainTime;
#endif
    private float startTime;
    private bool first;

    private Text _text;

    public Text text {
        get {
            if (this._text == null) {
                this._text = this.GetComponent<Text>();
            }

            return this._text;
        }
    }

    public Action<Text, float, bool> onTimeRefresh;

    private void Awake() {
        this.enabled = false;
    }

    public void Begin(float cd) {
        this.cd = cd;
        this.Begin();
    }

    [ContextMenu("Begin")]
    public void Begin() {
        this.enabled = true;

        this.first = true;
    }

    [ContextMenu("End")]
    private void End() {
        this.enabled = false;
    }

    private void Update() {
        if (this.first) {
            this.startTime = this.useTimeScale ? Time.time : Time.unscaledTime;
            this.remainTime = this.cd;
            this.first = false;

            this._OnTimeRefresh(false);
        }
        else {
            this.remainTime -= (this.useTimeScale ? Time.deltaTime : Time.unscaledDeltaTime);
            bool isEnd = this.remainTime <= 0f;

            if (this.refreshRate == ERefreshRate.PerFrame) {
                this._OnTimeRefresh(false);
            }
            else if (this.refreshRate == ERefreshRate.PerSecond) {
                var cur = this.useTimeScale ? Time.time : Time.unscaledTime;
                if (cur - this.startTime >= 1f) {
                    this.startTime = cur;
                    this._OnTimeRefresh(false);
                }
            }
            else if (this.refreshRate == ERefreshRate.PerMinute) {
                var cur = this.useTimeScale ? Time.time : Time.unscaledTime;
                if (cur - this.startTime >= 1f * 60) {
                    this.startTime = cur;
                    this._OnTimeRefresh(false);
                }
            }

            if (isEnd) {
                this._OnTimeRefresh(true);
                this.End();
            }
        }
    }

    private void _OnTimeRefresh(bool isEnd) {
        // if (isEnd) {
        //     text.text = "0";
        // }
        // else {
        //     text.text = remainTime.ToString();
        // }

        this.onTimeRefresh?.Invoke(this.text, (float)this.remainTime, isEnd);
    }
}
