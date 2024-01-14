using System;
using Lib.Core;
using Logic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

// 家族资源战 重生
public class UI_FamilyResBattleReborn : UIBase, UI_FamilyResBattleReborn.Layout.IListener {
    public class Layout : LayoutBase {
        public Text remainTime;

        public void Parse(GameObject root) {
            this.Set(root);

            this.remainTime = this.transform.Find("Animator/Text_Dis/RemainTime").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener) {
        }

        public interface IListener {
        }
    }

    public Layout layout = new Layout();
    public Timer timer;

    public long cdTime;

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }

    protected override void OnDestroy() {
        this.timer?.Cancel();
        this.timer = null;

        Sys_Input.Instance.bEnableJoystick = true;
        Sys_Input.Instance.bForbidControl = false;
        Sys_Input.Instance.bForbidTouch = false;
    }

    protected override void OnOpen(object arg) {
        long expiretime = (long) Convert.ToUInt32(arg);
        this.cdTime = expiretime - Sys_Time.Instance.GetServerTime();
        if (this.cdTime <= 0) {
            this.cdTime = 3;
        }
    }

    protected override void OnOpened() {
        this.timer?.Cancel();
        this.timer = Timer.RegisterOrReuse(ref this.timer, this.cdTime, this.OnTimeEnd, this.OnTimeing);

        Sys_Input.Instance.bEnableJoystick = false;
        Sys_Input.Instance.bForbidControl = true;
        Sys_Input.Instance.bForbidTouch = true;
    }

    private void OnTimeing(float dt) {
        Sys_Input.Instance.bEnableJoystick = false;
        Sys_Input.Instance.bForbidControl = true;
        Sys_Input.Instance.bForbidTouch = true;

        float remain = this.cdTime - dt;
        if (remain <= 0) {
            this.layout.remainTime.text = "00: 00";
        }
        else {
            var t = Mathf.Round(remain);
            this.layout.remainTime.text = LanguageHelper.TimeToString((uint) t, LanguageHelper.TimeFormat.Type_1);
        }
    }

    private void OnTimeEnd() {
        this.timer?.Cancel();
        this.timer = null;

        this.layout.remainTime.text = "00: 00";
        this.CloseSelf();
    }
}