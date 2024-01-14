using System;
using Lib.Core;
using Logic;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

// 家族资源战 资源提交结果 给所在队伍中每个成员都弹出
public class UI_FamilyResBattleSubmitResult : UIBase, UI_FamilyResBattleSubmitResult.Layout.IListener {
    public class Layout : LayoutBase {
        public Text incrCount;

        public void Parse(GameObject root) {
            this.Set(root);
            this.incrCount = this.transform.Find("Animator/Image_BG2/Content/Value").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener) {
        }

        public interface IListener {
        }
    }

    public Layout layout = new Layout();
    public Timer timer;

    public long lastCount;
    public long curCount;
    public long incrCount {
        get {
            return this.curCount - this.lastCount;
        }
    }

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }
    protected override void OnDestroy() {
        this.timer?.Cancel();
        this.timer = null;
    }

    public static float LENGTH = 2f;
    protected override void OnOpen(object arg) {
        var tp = arg as Tuple<long, long>;
        this.lastCount = tp.Item1;
        this.curCount = tp.Item2;

        this.curCount = this.curCount - this.lastCount;
        this.lastCount = 0;
    }

    protected override void OnOpened() {
        this.timer?.Cancel();
        this.timer = Timer.RegisterOrReuse(ref this.timer, LENGTH, this.OnTimeEnd, this.OnTiming);
    }

    private void OnTiming(float dt) {
        float rate = dt / LENGTH;
        int cur = Mathf.CeilToInt(this.lastCount + rate * this.incrCount);
        TextHelper.SetText(this.layout.incrCount, cur.ToString());
    }

    private void OnTimeEnd() {
        this.timer?.Cancel();
        this.timer = null;

        TextHelper.SetText(this.layout.incrCount, this.curCount.ToString());

        this.CloseSelf();
    }
}