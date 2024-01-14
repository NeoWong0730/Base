using System;
using System.Collections.Generic;
using Lib.Core;
using Logic;
using Logic.Core;
using Net;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;

// 家族资源战地图
public class UI_FamilyResBattleMap : UIBase, UI_FamilyResBattleMap.Layout.IListener {
    public class Res : UIComponent {
        public Image icon;
        public Text name;
        public CP_SliderLerp slider;
        public Text percent;
        public CDText remainTime;

        protected override void Loaded() {
            this.icon = this.transform.Find("Image_Explore").GetComponent<Image>();
            this.name = this.transform.Find("Text_Tips").GetComponent<Text>();
            this.slider = this.transform.Find("Image_ProcessBG").GetComponent<CP_SliderLerp>();
            this.percent = this.transform.Find("Image_ProcessBG/Text_Num").GetComponent<Text>();
            this.remainTime = this.transform.Find("Text_Time").GetComponent<CDText>();
            this.remainTime.onTimeRefresh = this.OnTimeRefresh;
        }

        public CSVFamilyResBattleResParameter.Data csvRes;
        private bool Perdict(uint id) { return this.csvRes.NpcId == id; }
        public void Refresh(uint resId, Sys_FamilyResBattle.Res info) {
            this.csvRes = CSVFamilyResBattleResParameter.Instance.GetConfData(resId);
            var max = Sys_FamilyResBattle.Instance.allRes[resId].max;

            ImageHelper.SetIcon(this.icon, this.csvRes.IconID, true);
            TextHelper.SetText(this.name, this.csvRes.NameID);

            if (info == null) {
                this.slider.Refresh(0f);
                TextHelper.SetText(this.percent, string.Format("{0}/{1}", "0", max.ToString()));
                this.remainTime.Begin(0f);
            }
            else {
                this.slider.Refresh(1f * info.leftCount / max);
                TextHelper.SetText(this.percent, string.Format("{0}/{1}", info.leftCount.ToString(), max.ToString()));
                long diff = info.nextRefreshTime - Sys_Time.Instance.GetServerTime();
                this.remainTime.Begin(diff);
            }
        }

        private void OnTimeRefresh(Text text, float time, bool isEnd) {
            if (isEnd) {
                TextHelper.SetText(text, "00:00:00");
            }
            else {
                var t = Mathf.Round(time);
                var s = LanguageHelper.TimeToString((uint)t, LanguageHelper.TimeFormat.Type_1);
                TextHelper.SetText(text, s);
            }
        }
    }

    public class Layout : LayoutBase {
        public GameObject tabProto;
        public Transform tabProtoParent;

        public Transform rightNode;

        public void Parse(GameObject root) {
            this.Set(root);

            this.tabProto = this.transform.Find("Animator/Pages/Map/View_Left/Scroll_View_Find/FindItem").gameObject;
            this.tabProtoParent = this.transform.Find("Animator/Pages/Map/View_Left/Scroll_View_Find/TabList");
            this.rightNode = this.transform.Find("Animator/Pages/Map/Image_Map/View_Map01/Map01/View_Map");
        }

        public void RegisterEvents(IListener listener) {
        }

        public interface IListener {
        }
    }

    public Layout layout = new Layout();
    public List<Sys_FamilyResBattle.Res> resInfos = new List<Sys_FamilyResBattle.Res>();
    public COWVd<Res> vds = new COWVd<Res>();
    public UI_FamilyResBattleMapController right;
    public UI_MapStaticMarker staticMarker;

    public List<BattleRoleMiniMapData> holdPlayerInfo = new List<BattleRoleMiniMapData>();
    public UI_MapDynamicMarkerHoldPlayers holdPlayer;

    public List<BattleRoleMiniMapData> unholdPlayerInfo = new List<BattleRoleMiniMapData>();
    public UI_MapDynamicMarkerUnholdPlayers unholdPlayer;

    private Timer timerRefresh;

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }
    protected override void OnDestroy() {
        this.vds.Clear();

        this.right?.OnDestroy();
        this.staticMarker?.OnDestroy();
        this.holdPlayer?.OnDestroy();
        this.unholdPlayer?.OnDestroy();

        this.timerRefresh?.Cancel();
    }

    public uint mapId;
    protected override void OnOpen(object arg) {
        this.mapId = arg == null ? 1450 : Convert.ToUInt32(arg);

        this.resInfos.Clear();
        foreach (var kvp in Sys_FamilyResBattle.Instance.allRes) {
            this.resInfos.Add(kvp.Value);
        }
    }
    protected override void OnOpened() {
        this.RefreshTimer();

        this.timerRefresh?.Cancel();
        // 强制10s刷新一次，因为cdtext内部有误差，而且很大
        this.timerRefresh = Timer.RegisterOrReuse(ref this.timerRefresh, 10f, this.RefreshTimer, null, true);

        this.RefreshAll();
    }
    public void RefreshTimer() {
        this.vds.TryBuildOrRefresh(this.layout.tabProto, this.layout.tabProtoParent, this.resInfos.Count, this._OnRefresh);
    }
    public void RefreshAll() {
        if (this.right == null) {
            this.right = new UI_FamilyResBattleMapController();
            this.right.Init(this.layout.rightNode);
            this.right.Show();
        }
        this.right.RefreshBaseInfo(this.mapId, true);

        if (this.staticMarker == null) {
            this.staticMarker = new UI_MapStaticMarker();
            this.staticMarker.Init(this.layout.rightNode);
            this.staticMarker.Show();
        }
        this.staticMarker.RefreshBaseInfo(this.mapId, true, this.right.rawImage_BgMap.rectTransform.sizeDelta, this.right.mapSize, false);

        if (this.holdPlayer == null) {
            this.holdPlayer = new UI_MapDynamicMarkerHoldPlayers();
            this.holdPlayer.Init(this.layout.rightNode);
            this.holdPlayer.Show();
        }
        this.holdPlayer.RefreshBaseInfo(this.right.rawImage_BgMap.rectTransform.sizeDelta, this.right.cSVMapInfoData?.ui_pos, this.right.mapSize);
        this.holdPlayer.Refresh(null);

        if (this.unholdPlayer == null) {
            this.unholdPlayer = new UI_MapDynamicMarkerUnholdPlayers();
            this.unholdPlayer.Init(this.layout.rightNode);
            this.unholdPlayer.Show();
        }
        this.unholdPlayer.RefreshBaseInfo(this.right.rawImage_BgMap.rectTransform.sizeDelta, this.right.cSVMapInfoData?.ui_pos, this.right.mapSize);
        this.unholdPlayer.Refresh(null);
    }

    protected override void OnLateUpdate(float dt, float usdt) {
        this.right?.ExecUpdate();
        this.holdPlayer?.OnRefresh();
    }

    protected override void OnShow() {
        Sys_FamilyResBattle.Instance.FocusMiniMapReq(true);
    }
    protected override void OnHide() {
        Sys_FamilyResBattle.Instance.FocusMiniMapReq(false);
    }

    private void _OnRefresh(Res vd, int index) {
        var info = 0 <= index && index < this.resInfos.Count ? this.resInfos[index] : null;
        vd.Refresh(this.resInfos[index].resId, info);
    }

    #region 事件处理
    protected override void ProcessEvents(bool toRegister) {
        if (toRegister) {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuildBattle.MiniMapDataNty, this.OnMiniMapDataNty, CmdGuildBattleMiniMapDataNty.Parser);
        }
        else {
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildBattle.MiniMapDataNty, this.OnMiniMapDataNty);
        }
    }

    private void OnMiniMapDataNty(NetMsg msg) {
        CmdGuildBattleMiniMapDataNty res = NetMsgUtil.Deserialize<CmdGuildBattleMiniMapDataNty>(CmdGuildBattleMiniMapDataNty.Parser, msg);

        this.holdPlayerInfo.Clear();
        this.unholdPlayerInfo.Clear();
        for (int i = 0, length = res.Rolelist.Role.Count; i < length; ++i) {
            var info = res.Rolelist.Role[i];
            if (info.RoleId != Sys_Role.Instance.RoleId) {
                if (info.Extra == null) {
                    this.unholdPlayerInfo.Add(info);
                }
                else {
                    this.holdPlayerInfo.Add(info);
                }
            }
        }
        this.holdPlayer?.Refresh(this.holdPlayerInfo);
        this.unholdPlayer?.Refresh(this.unholdPlayerInfo);
    }
    #endregion
}