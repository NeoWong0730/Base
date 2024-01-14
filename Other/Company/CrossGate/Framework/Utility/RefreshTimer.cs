using System;
using Lib.Core;

// 用于客户端定时器，比如每日凌晨定时更新某个系统的一些数据
public class LastRefreshTimer {
    private float _lastRefreshTime;
    private Timer _timer;

    public Func<float> NowFunc { get; private set; }
    public Action OnCompleted { get; private set; }
    public float Duration { get; private set; }

    public float NextRefreshTime {
        get { return this.LastRefreshTime + this.Duration; }
    }

    public virtual float LastRefreshTime {
        get => this._lastRefreshTime;
        set {
            this._lastRefreshTime = value;

            void Completed() {
                this._lastRefreshTime = this.NowFunc.Invoke();
                this.OnCompleted?.Invoke();
            }

            float now = this.NowFunc.Invoke();
            float remain = this.NextRefreshTime - now;

            this._timer?.Cancel();
            this._timer = Timer.Register(remain, () => {
                Completed();

                this._timer?.Cancel();
                this._timer = Timer.Register(this.Duration, Completed, null, true);
            }, null, false);
        }
    }

    public LastRefreshTimer(float lastRefreshTime, float duration, Func<float> nowFunc, Action onCompleted) {
        this.Reset(lastRefreshTime, duration, nowFunc, onCompleted);
    }

    public LastRefreshTimer Reset(float lastRefreshTime, float duration, Func<float> nowFunc, Action onCompleted) {
        this.Duration = duration;
        this.NowFunc = nowFunc;
        this.OnCompleted = onCompleted;

        this._timer?.Cancel();
        this.LastRefreshTime = lastRefreshTime;
        return this;
    }

    public void Cancel() { this._timer?.Cancel(); }
    public void OnServerTimeChanged() {
        float now = this.NowFunc.Invoke();
        while (now > this.NextRefreshTime) {
            this._lastRefreshTime += this.Duration;
        }

        this.LastRefreshTime = this._lastRefreshTime;
    }
}

public class NextRefreshTimer {
    private float _nextRefreshTime;
    private Timer _timer;

    public Func<float> NowFunc { get; private set; }
    public Action OnCompleted { get; private set; }
    public float Duration { get; private set; }

    public float NextRefreshTime {
        get { return this._nextRefreshTime; }
        protected set {
            this._nextRefreshTime = value;

            void Completed() {
                this._nextRefreshTime += this.Duration;
                this.OnCompleted?.Invoke();
            }

            float now = this.NowFunc.Invoke();
            float remain = this.NextRefreshTime - now;
            this._timer?.Cancel();
            this._timer = Timer.Register(remain, () => {
                Completed();

                this._timer?.Cancel();
                this._timer = Timer.Register(this.Duration, Completed, null, true);
            }, null, false);
        }
    }

    public NextRefreshTimer(float nextRefreshTime, float duration, Func<float> nowFunc, Action onCompleted) {
        this.Reset(nextRefreshTime, duration, nowFunc, onCompleted);
    }

    public NextRefreshTimer Reset(float nextRefreshTime, float duration, Func<float> nowFunc, Action onCompleted) {
        this.Duration = duration;
        this.NowFunc = nowFunc;
        this.OnCompleted = onCompleted;

        this._timer?.Cancel();
        this.NextRefreshTime = nextRefreshTime;
        return this;
    }

    public void Cancel() { this._timer?.Cancel(); }
    public void OnServerTimeChanged() {
        float now = this.NowFunc.Invoke();
        while (now > this._nextRefreshTime) {
            this._nextRefreshTime += this.Duration;
        }

        this.NextRefreshTime = this._nextRefreshTime;
    }
}
