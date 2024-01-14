using System;
using Common;
using Framework.UI;
using Lib.Core;
using Logic;
using Logic.Core;
using Table;
using TMPro;
using UnityEngine;

namespace Logic.UI {
    // 捏脸
    public partial class UI_MakeFace : UIBase {
        public UI_MakeFace_Layout layout;

        private Timer _roleTimer;
        public uint careerId;
        public enRoleSex sex;
        
        public PosSelector selector;

        protected override void OnLoaded() {
            layout = UILayoutBase.GetLayout<UI_MakeFace_Layout>(this.transform);
            layout.Init(this.transform);
            layout.BindEvents(this);
            
            selector = GameObject.FindObjectOfType<PosSelector>();
        }

        protected override void OnDestroy() {
            _roleTimer?.Cancel();
        }

        protected override void OnOpened() {
            selector.SetStage(PosSelector.EStage.Makeface);
            // TrySetExpireTimer(ref _roleTimer);
        }

        protected override void OnOpen(object arg) {
            var tp = arg as Tuple<uint, enRoleSex>;
            if (tp != null) {
                careerId = tp.Item1;
                sex = tp.Item2;
            }
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
    public partial class UI_MakeFace : UI_MakeFace_Layout.IListener {
        public void OnBtnClicked_btnEnter() {
            Sys_Login.Instance.ReqCreateRole(new ZoneCreateRole() {
                CareerId = careerId,
                Sex = sex,
            });
        }

        public void OnBtnClicked_btnReturn() {
            UIManager.OpenUI(EUIID.UI_CreateCharacter);
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
    public partial class UI_MakeFace {
        protected override void ProcessEvents(bool toRegister) {
        }
    }
}