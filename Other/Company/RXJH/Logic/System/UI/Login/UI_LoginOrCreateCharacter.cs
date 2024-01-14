using System;
using Common;
using Framework.UI;
using Lib.Core;
using Logic.Core;
using Table;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Logic.UI {
    public partial class UI_LoginOrCreateCharacter : UIBase {
        public class Head : VD<UI_LoginOrCreateCharacter_Head_Layout> {
            public void Refresh(ZoneRole role, int index) {
                if (role == null) {
                    layout.infoNode.gameObject.SetActive(false);
                }
                else {
                    layout.infoNode.gameObject.SetActive(false);
                    TextHelper.SetText(layout.roleLevel, role.Level.ToString());
                    TextHelper.SetText(layout.roleName, role.NickName);
                    var csvCareer = CSVCareer.Instance.GetConfData(role.CareerId);
                    if (csvCareer != null) {
                        ImageHelper.SetIcon(layout.imgHead, role.Sex == enRoleSex.Female ? csvCareer.femaleIcon : csvCareer.maleIcon);
                        ImageHelper.SetIcon(layout.roleCareer, csvCareer.icon);
                    }
                }

                layout.tg.id = index;
            }
        }

        public UI_LoginOrCreateCharacter_Layout layout;
        public COWVd<Head> heads = new COWVd<Head>();
        public PosSelector selector;

        public Sys_Server.ZoneEntry zone;

        private Timer _roleTimer;

        public int selectedIndex = 0;

        public int roleCount {
            get { return (zone == null ? 0 : zone.roles.Count); }
        }

        public ZoneRole selectedRole {
            get {
                if (0 <= selectedIndex && selectedIndex < roleCount) {
                    var role = zone.roles[selectedIndex];
                    return role;
                }

                return null;
            }
        }

        protected override void OnLoaded() {
            layout = UILayoutBase.GetLayout<UI_LoginOrCreateCharacter_Layout>(this.transform);
            layout.Init(this.transform);
            layout.BindEvents(this);

            selector = GameObject.FindObjectOfType<PosSelector>();
            }

        protected override void OnDestroy() {
            _roleTimer?.Cancel();
            selector = null;
        }

        protected override void OnOpened() {
            selector.SetStage(PosSelector.EStage.Select);
            // TrySetExpireTimer(ref _roleTimer);
            RefreshAll();
        }

        private void RefreshAll() {
            zone = Sys_Server.Instance.GetSelectedZone();
            int max = math.max(roleCount, 7);
            heads.TryBuildOrRefresh(layout.proto.gameObject, layout.protoParent, max, _OnHeadRefresh);

            layout.centerOn.Rebuild();
            // 默认选中
            if (heads.RealCount > 0) {
                layout.tgRegistry.SwitchTo(0);
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

        private void _OnHeadRefresh(Head vd, int index) {
            if (index < roleCount) {
                var role = zone.roles[index];
                vd.Refresh(role, index);
            }
            else {
                vd.Refresh(null, index);
            }
        }

        private void _OnHeadSelected(int index, bool interaction) {
            this.selectedIndex = index;
            layout.centerOn.CenterOn(index, true);
            selector.CtrlCreate(int.MaxValue); // 全部隐藏
            
            if (0 <= index && index < roleCount) {
                // todo 模型加载
                var role = selectedRole;
                PosSelector.CreateCfg st = default;
                if (selector.TryFindCreate(role.CareerId, ref st)) {
                    st.Visible(true);
                    PosSelector.ETimelineType aim;
                    if (!interaction) {
                        aim = role.Sex == (int)enRoleSex.Male ? PosSelector.ETimelineType.Enter_M : PosSelector.ETimelineType.Enter_F;
                    }
                    else {
                        aim = role.Sex == (int)enRoleSex.Male ? PosSelector.ETimelineType.F_M : PosSelector.ETimelineType.M_F;
                    }
                    st.PlayTimeline(aim);
                }
            }
            else {
                // UIManager.OpenUI(EUIID.UI_CreateCharacter);
            }
        }
    }

    // UI事件
    public partial class UI_LoginOrCreateCharacter : UI_LoginOrCreateCharacter_Layout.IListener {
        public void OnBtnClicked_btnEnter() {
            if (selectedRole != null) {
                Sys_Login.Instance.ReqSelectRole(selectedRole.RoleId);
            }
            else {
                // can't reach here
                UIManager.OpenUI(EUIID.UI_CreateCharacter);
            }
        }

        public void OnBtnClicked_btnReturn() {
            Sys_Login.Instance.ExitGame(true);
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

        public void OnCenter_centerOn(int index, Transform t) {
        }

        public void OnTransform_centerOn(bool hOrV, int index, Transform tr, float toMiddle, Vector3 srCenterOnCentent) {
        }

        public void OnToggleChange_tgRegistry(int newIndex, int oldIndex, bool interaction) {
            _OnHeadSelected(newIndex, interaction);
        }
    }

    // 逻辑事件
    public partial class UI_LoginOrCreateCharacter {
        protected override void ProcessEvents(bool toRegister) {
        }
    }
}