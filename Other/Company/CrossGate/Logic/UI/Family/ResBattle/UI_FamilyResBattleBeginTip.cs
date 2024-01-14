using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Logic.Core;
using Lib.Core;

// 家族资源战 进入战场预告
// 只要能进入战场，就弹出
public class UI_FamilyResBattleBeginTip : UIBase, UI_FamilyResBattleBeginTip.Layout.IListener {
    public class Layout : LayoutBase {
        public Text title;
        public Text content;

        public void Parse(GameObject root) {
            Set(root);
        }

        public void RegisterEvents(IListener listener) {
        }

        public interface IListener {
        }
    }

    public Layout layout = new Layout();
    public Timer timer;

    protected override void OnLoaded() {
        layout.Parse(gameObject);
        layout.RegisterEvents(this);
    }

    protected override void OnOpen(object arg) {
    }

    protected override void OnDestroy() {
        timer?.Cancel();
        timer = null;
    }

    public static float LENGTH = 4f;
    protected override void OnOpened() {
        timer?.Cancel();
        timer = Timer.RegisterOrReuse(ref timer, LENGTH, OnTimeEnd);
    }

    private void OnTimeEnd() {
        timer?.Cancel();
        timer = null;

        CloseSelf();
    }
}