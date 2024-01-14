using System;
using System.Collections.Generic;
using Common;
using Framework.UI;
using Logic.Core;
using Lib.Core;
using Table;
using TMPro;
using UnityEngine;

namespace Logic.UI {
    public partial class UI_ServerList : UIBase {
        public static uint GetServerStatusIcon(enZoneState state) {
            uint iconId = 3;
            switch (state) {
                case enZoneState.Grey:
                    iconId = 4;
                    break;
                case enZoneState.Red:
                    iconId = 1;
                    break;
                case enZoneState.Yellow:
                    iconId = 2;
                    break;
                case enZoneState.Green:
                default:
                    iconId = 3;
                    break;
            }

            return iconId;
        }

        public class ServerItem : UIComponent, UI_ServerList_ServerItem_Layout.IListener {
            public UI_ServerList_ServerItem_Layout layout;
            public Sys_Server.ZoneEntry zone;

            protected override void Loaded() {
                layout = UILayoutBase.GetLayout<UI_ServerList_ServerItem_Layout>(this.transform);
                layout.Init(this.transform);
                layout.BindEvents(this);
            }

            public void Refresh(Sys_Server.ZoneEntry zone) {
                this.zone = zone;
                
                if (zone != null) {
                    ImageHelper.SetIcon(layout.imgServerStatus, GetServerStatusIcon(zone.svrZone.State));
                    TextHelper.SetText(layout.serverName, zone.svrZone.ZoneName);
                    if (zone.roles.Count > 0) {
                        layout.roleNode.gameObject.SetActive(true);

                        TextHelper.SetText(layout.roleNumber, zone.roles.Count.ToString());
                        var role = zone.roles[0];
                        var csvCareer = CSVCareer.Instance.GetConfData(role.CareerId);
                        if (csvCareer != null) {
                            ImageHelper.SetIcon(layout.roleHead, role.Sex == enRoleSex.Male ? csvCareer.maleIcon : csvCareer.femaleIcon);
                        }
                    }
                    else {
                        layout.roleNode.gameObject.SetActive(false);
                    }

                    var flag = (int)(enZoneFunc.New);
                    bool isFlag = (zone.svrZone.ZoneFunc & (1 << flag)) == flag;
                    layout.newFlag.gameObject.SetActive(isFlag);
                }
            }

            public void OnBtnClicked_btnEnter() {
                Sys_Login.Instance.ReqChooseZone(zone.svrZone.ZoneId);
            }
        }

        public class ServerTab : UIToggleSelectable {
            public UI_ServerList_ServerTab_Layout layout;

            protected override void Loaded() {
                layout = UILayoutBase.GetLayout<UI_ServerList_ServerTab_Layout>(this.transform);
                layout.Init(this.transform);

                base.AddToggleListener(layout.toggle, true);
            }

            public void Refresh() {
                if (id == Sys_Server.Instance.lastTab.id) {
                    TextHelper.SetText(layout.nameNormal, Sys_Server.Instance.lastTab.name);
                }
                else if (id == Sys_Server.Instance.recommendTab.id) {
                    TextHelper.SetText(layout.nameNormal, Sys_Server.Instance.recommendTab.name);
                }
                else if (Sys_Server.Instance.allTabs.TryGetValue(id, out var tab)) {
                    TextHelper.SetText(layout.nameNormal, tab.name);
                }
                else {
                    // error
                }

                layout.nameSelected.text = layout.nameNormal.text;
            }
        }

        public UI_ServerList_Layout layout;

        private ServerTab latestTab {
            get { return tabVDs[0]; }
        }

        private ServerTab recommendTab {
            get { return tabVDs[1]; }
        }

        public enum EMode {
            Auto,
            Latest,
            Recommend,
            Normal,
        }

        // 有角色则最近登录，无角色则推荐
        public EMode mode = EMode.Latest;
        private readonly UIElementCollector<ServerTab> tabVDs = new UIElementCollector<ServerTab>();

        private uint currentTabId = 0;
        private List<uint> tabIds = new List<uint>();
        private List<Sys_Server.ZoneEntry> zonesOfTab = new List<Sys_Server.ZoneEntry>();

        public Timer _selectZoneTimer;

        protected override void OnLoaded() {
            layout = UILayoutBase.GetLayout<UI_ServerList_Layout>(this.transform);
            layout.Init(this.transform);
            layout.BindEvents(this);
        }

        protected override void OnDestroy() {
            _selectZoneTimer?.Cancel();
            this.tabVDs.ForEach((ele) => { ele.OnDestroy(); });
        }

        protected override void OnOpen(object arg) {
            mode = (EMode)Convert.ToInt32(arg);
        }

        protected override void OnOpened() {
            RefreshAll();
            // TrySetExpireTimer(ref _selectZoneTimer);
        }

        private void TrySetExpireTimer(ref Timer tmer) {
            layout.cdNode.gameObject.SetActive(false);
            // todo 先不处理gm调整时间
            long now = Sys_Time.Instance.GetServerTime();
            long expire = Sys_Login.Instance.selectZoneExpire;
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

        public void RefreshAll() {
            tabIds.Clear();
            ListHelper.CopyKeyTo(Sys_Server.Instance.allTabs, ref tabIds);
            tabIds.Insert(0, Sys_Server.Instance.recommendTab.id); // index == 1是recommendTab
            tabIds.Insert(0, Sys_Server.Instance.lastTab.id); // index == 0是lastTab
            // 服务器tab过多，
            tabVDs.TryBuildOrRefresh(layout.serverTabProto.gameObject, layout.serverTabProtoParent, tabIds.Count,
                _OnTabRefresh,
                _OnTabInit);

            // 如果刷新后，不存在上次选中的tab，则重置
            if (currentTabId != 0 && !this.tabIds.Contains(this.currentTabId)) {
                this.currentTabId = 0;
            }

            if (mode == EMode.Auto) {
                if (this.currentTabId == 0) {
                    mode = Sys_Server.Instance.isNewCharacter ? EMode.Recommend : EMode.Latest;
                }
                else {
                    mode = EMode.Normal;
                }
            }

            if (mode == EMode.Latest) {
                latestTab.SetSelected(true, true);
            }
            else if (mode == EMode.Recommend) {
                recommendTab.SetSelected(true, true);
            }
            else if (mode == EMode.Normal) {
                int index = tabIds.IndexOf(currentTabId);
                if (tabVDs.TryGetVdByIndex(index, out var vd)) {
                    vd.SetSelected(true, true);
                }
                else if (tabVDs.RealCount > 0) {
                    tabVDs[0].SetSelected(true, true);
                }
                else {
                    // error
                }
            }
        }

        private void _OnTabInit(ServerTab vd, int index) {
        }

        private void _OnTabRefresh(ServerTab vd, int index) {
            vd.SetId(tabIds[index], index);
            vd.Refresh();
            vd.SetSelectedAction(_OnTabSelected);
        }

        private void _OnTabSelected(uint tabId, int index, bool force) {
            currentTabId = tabId;

            RefreshServerList(tabId);
        }

        public void RefreshServerList(uint tabId) {
            if (tabId == Sys_Server.Instance.lastTab.id) {
                zonesOfTab = Sys_Server.Instance.lastTab.zones;
            }
            else if (tabId == Sys_Server.Instance.recommendTab.id) {
                zonesOfTab = Sys_Server.Instance.recommendTab.zones;
            }
            else if (Sys_Server.Instance.allTabs.TryGetValue(tabId, out var tab)) {
                zonesOfTab = tab.zones;
            }
            else {
                zonesOfTab.Clear();
            }

            layout.infinityItems.CellCount = zonesOfTab.Count;
            layout.infinityItems.ForceRefreshActiveCell();
        }
    }

    // UI事件
    public partial class UI_ServerList : UI_ServerList_Layout.IListener {
        public void OnBtnClicked_btnClose() {
            UIManager.CloseUI((EUIID)nID);
        }

        public void OnCreateCell_infinityItems(InfinityGridCell cell) {
            ServerItem vd = new ServerItem();
            vd.Init(cell.mRootTransform);
            cell.BindUserData(vd);
        }

        public void OnCellChange_infinityItems(InfinityGridCell cell, int index) {
            ServerItem vd = cell.mUserData as ServerItem;
            var data = zonesOfTab[index];
            vd.Refresh(data);
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
    public partial class UI_ServerList {
        protected override void ProcessEvents(bool toRegister) {
            Sys_Server.Instance.eventEmitter.Handle(Sys_Server.EEvents.OnServerListChanged, _OnServerListChanged, toRegister);
        }

        private void _OnServerListChanged() {
            RefreshAll();
        }
    }
}