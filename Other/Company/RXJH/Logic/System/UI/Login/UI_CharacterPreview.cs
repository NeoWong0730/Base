using System;
using Framework.UI;
using Lib.Core;
using Logic.Core;
using Table;
using TMPro;
using UnityEngine;

namespace Logic.UI {
// 所有职业
    public partial class UI_CharacterPreview : UIBase {
        public UI_CharacterPreview_Layout layout;

        private Timer _roleTimer;

        protected override void OnLoaded() {
            layout = UILayoutBase.GetLayout<UI_CharacterPreview_Layout>(this.transform);
            layout.Init(this.transform);
        }

        protected override void OnDestroy() {
            _roleTimer?.Cancel();
        }

        protected override void OnOpened() {
            // TrySetExpireTimer(ref _roleTimer);
        }

        private void TrySetExpireTimer(ref Timer tmer) {
            layout.cdNode.gameObject.SetActive(false);
            long now = Sys_Time.Instance.GetServerTime();
            long expire = Sys_Login.Instance.actionExpire;
            long diff = expire - now;
            if (diff > 0) {
                void _DoCD() {
                    layout.cdNode.gameObject.SetActive(true);
                    layout.cdText.Begin(diff);
                }

                if (diff <= 300) {
                    _DoCD();
                }
                else {
                    // 大于3分钟则开启后台计时， 否则直接显示倒计时Text
                    layout.cdNode.gameObject.SetActive(false);
                    tmer = Timer.RegisterOrReuse(ref tmer, diff - 300, () => { _DoCD(); });
                }
            }
        }
    }

// UI事件
    public partial class UI_CharacterPreview : UI_CharacterPreview_Layout.IListener {
        public void OnBtnClicked_btnReturn() {
            LevelManager.EnterLevel(typeof(LvLogin));
        }

        public void OnTimeRefresh_cdText(TextMeshProUGUI text, float time, bool isEnd) {
            if (isEnd) {
                TextHelper.SetText(text, "00:00:00");
            }
            else {
                var t = Mathf.Round(time);
                var s = TimeFormater.TimeToString((uint)t, TimeFormater.ETimeFormat.Type_1);
                TextHelper.SetText(text, s);
            }
        }
    }

// 逻辑事件
    public partial class UI_CharacterPreview {
        protected override void ProcessEvents(bool toRegister) {
        }
    }
}