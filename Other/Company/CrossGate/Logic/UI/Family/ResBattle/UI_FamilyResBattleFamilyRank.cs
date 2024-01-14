using System.Collections.Generic;
using Logic;
using Logic.Core;
using Net;
using Packet;
using UnityEngine;
using UnityEngine.UI;

// 家族资源战家族排行
public class UI_FamilyResBattleFamilyRank : UIBase, UI_FamilyResBattleFamilyRank.Layout.IListener {
    public class Line : UIComponent {
        public GameObject rankIcon1;
        public GameObject rankIcon2;
        public GameObject rankIcon3;

        public Text rank;

        public Text serverName;
        public Text familyName;
        public Text familyLv;
        public Text familyMember;
        public Text familyScore;
        public Text familyRate;
        public Text familyWIn;

        protected override void Loaded() {
            this.rankIcon1 = this.transform.Find("Rank/Image_Icon").gameObject;
            this.rankIcon2 = this.transform.Find("Rank/Image_Icon1").gameObject;
            this.rankIcon3 = this.transform.Find("Rank/Image_Icon2").gameObject;
            this.rank = this.transform.Find("Rank/Text_Rank").GetComponent<Text>();

            this.familyName = this.transform.Find("Text_Name").GetComponent<Text>();
            this.serverName = this.transform.Find("Text_Server").GetComponent<Text>();
            this.familyLv = this.transform.Find("Text_Level").GetComponent<Text>();
            this.familyScore = this.transform.Find("Text_Point").GetComponent<Text>();
            this.familyMember = this.transform.Find("Text_Member").GetComponent<Text>();
            this.familyRate = this.transform.Find("Text_Rate").GetComponent<Text>();
            this.familyWIn = this.transform.Find("Text_WinningStreak").GetComponent<Text>();
        }

        public void Refresh(RankUnitData info, int index) {
            if (index < 3) {
                rank.gameObject.SetActive(false);
                rankIcon1.SetActive(index == 0);
                rankIcon2.SetActive(index == 1);
                rankIcon3.SetActive(index == 2);
            }
            else {
                this.rank.gameObject.SetActive(true);
                rankIcon1.SetActive(false);
                rankIcon2.SetActive(false);
                rankIcon3.SetActive(false);

                TextHelper.SetText(this.rank, (index + 1).ToString());
            }

            TextHelper.SetText(this.familyName, info.GuildData.GuildName.ToStringUtf8());
            ServerInfo serverInfo = Sys_Login.Instance.FindServerInfoByID(info.GuildData.GamesvrId);
            if (serverInfo == null) {
                TextHelper.SetText(this.serverName, "");
            }
            else {
                TextHelper.SetText(this.serverName, serverInfo.ServerName);
            }

            TextHelper.SetText(this.familyLv, info.GuildData.Level.ToString());
            TextHelper.SetText(this.familyMember, info.GuildData.Battle.MemberCount.ToString());
            TextHelper.SetText(this.familyScore, info.Score.ToString());
            TextHelper.SetText(this.familyRate, $"{info.GuildData.Battle.WinRatio.ToString()}%");
            TextHelper.SetText(this.familyWIn, info.GuildData.Battle.WinStreak.ToString());
        }
    }

    public class Layout : LayoutBase {
        public GameObject unEmpty;
        public InfinityGrid infinity;

        public GameObject empty;

        // my
        public GameObject myRankIcon1;
        public GameObject myRankIcon2;
        public GameObject myRankIcon3;
        public Transform myNode;
        public Text myRank;
        public Text myserverName;
        public Text myfamilyName;
        public Text myfamilyLv;
        public Text myfamilyMember;
        public Text myfamilyScore;
        public Text myfamilyRate;
        public Text myfamilyWIn;

        public void Parse(GameObject root) {
            this.Set(root);

            this.unEmpty = this.transform.Find("Animator/Scroll_Rank").gameObject;
            this.infinity = this.transform.Find("Animator/Scroll_Rank").GetComponent<InfinityGrid>();
            this.empty = this.transform.Find("Animator/View_NoInfo").gameObject;

            // my
            myNode = this.transform.Find("Animator/MyRank/Info");
            this.myRankIcon1 = this.transform.Find("Animator/MyRank/Info/Rank/Image_Icon").gameObject;
            this.myRankIcon2 = this.transform.Find("Animator/MyRank/Info/Rank/Image_Icon1").gameObject;
            this.myRankIcon3 = this.transform.Find("Animator/MyRank/Info/Rank/Image_Icon2").gameObject;
            this.myRank = this.transform.Find("Animator/MyRank/Info/Rank/Text_Rank").GetComponent<Text>();

            this.myfamilyName = this.transform.Find("Animator/MyRank/Info/Text_Name").GetComponent<Text>();
            this.myfamilyScore = this.transform.Find("Animator/MyRank/Info/Text_Point").GetComponent<Text>();
            this.myserverName = this.transform.Find("Animator/MyRank/Info/Text_Server").GetComponent<Text>();
            this.myfamilyLv = this.transform.Find("Animator/MyRank/Info/Text_Level").GetComponent<Text>();
            this.myfamilyMember = this.transform.Find("Animator/MyRank/Info/Text_Member").GetComponent<Text>();
            this.myfamilyRate = this.transform.Find("Animator/MyRank/Info/Text_Rate").GetComponent<Text>();
            this.myfamilyWIn = this.transform.Find("Animator/MyRank/Info/Text_WinningStreak").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener) {
            this.infinity.onCreateCell += listener.OnCreateCell;
            this.infinity.onCellChange += listener.OnCellChange;
        }

        public interface IListener {
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public Layout layout = new Layout();

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }

    protected override void OnOpened() {
    }

    private uint curKey = 0;

    protected override void OnShow() {
        this.curKey = Sys_Rank.SetType((uint) RankType.Guild, (uint) RankTypeGuild.ResFight, 0);
        long curTime = Sys_Time.Instance.GetServerTime();
        if (curTime >= Sys_FamilyResBattle.Instance.cd) {
            _DoReq();
        }
        else {
            this.RefreshAll(this.curKey);
        }
    }

    public void RefreshAll(uint key) {
        var lsCount = Sys_FamilyResBattle.Instance.ranks.Count;
        if (lsCount <= 0) {
            layout.myNode.gameObject.SetActive(false);
            layout.empty.SetActive(true);
            layout.unEmpty.SetActive(false);
        }
        else {
            layout.empty.SetActive(false);
            layout.unEmpty.SetActive(true);

            RefreshMy();
            layout.myNode.gameObject.SetActive(true);
            layout.infinity.CellCount = lsCount;
            layout.infinity.ForceRefreshActiveCell();
        }
    }

    public void RefreshMy() {
        bool __Find(RankUnitData info) {
            var familyId = Sys_Family.Instance.GetFamilyUId();
            return info.GuildData.GuildId == familyId;
        }

        // index为我方家族排名
        int index = Sys_FamilyResBattle.Instance.ranks.FindIndex(__Find);
        if (index == -1) {
            layout.myRank.gameObject.SetActive(true);
            layout.myRankIcon1.SetActive(false);
            layout.myRankIcon2.SetActive(false);
            layout.myRankIcon3.SetActive(false);
            
            TextHelper.SetText(this.layout.myRank, LanguageHelper.GetTextContent(4157000022));
            TextHelper.SetText(this.layout.myfamilyName, "");
            TextHelper.SetText(this.layout.myserverName, "");
            TextHelper.SetText(this.layout.myfamilyLv, "");
            TextHelper.SetText(this.layout.myfamilyMember, "");
            TextHelper.SetText(this.layout.myfamilyScore, "");
            TextHelper.SetText(this.layout.myfamilyRate, "");
            TextHelper.SetText(this.layout.myfamilyWIn, "");
        }
        else {
            if (index <= 3) {
                layout.myRank.gameObject.SetActive(false);
                layout.myRankIcon1.SetActive(index == 0);
                layout.myRankIcon2.SetActive(index == 1);
                layout.myRankIcon3.SetActive(index == 2);
            }
            else {
                layout.myRank.gameObject.SetActive(true);
                layout.myRankIcon1.SetActive(false);
                layout.myRankIcon2.SetActive(false);
                layout.myRankIcon3.SetActive(false);
            }

            var rankData = Sys_FamilyResBattle.Instance.ranks[index].GuildData;
            TextHelper.SetText(this.layout.myfamilyName, rankData.GuildName.ToStringUtf8());
            ServerInfo serverInfo = Sys_Login.Instance.FindServerInfoByID(rankData.GamesvrId);
            if (serverInfo == null) {
                TextHelper.SetText(this.layout.myserverName, "");
            }
            else {
                TextHelper.SetText(this.layout.myserverName, serverInfo.ServerName);
            }

            TextHelper.SetText(this.layout.myfamilyLv, rankData.Level.ToString());
            TextHelper.SetText(this.layout.myfamilyMember, rankData.Battle.MemberCount.ToString());
            TextHelper.SetText(this.layout.myfamilyScore, Sys_FamilyResBattle.Instance.ranks[index].Score.ToString());
            TextHelper.SetText(this.layout.myfamilyRate, $"{rankData.Battle.WinRatio.ToString()}%");
            TextHelper.SetText(this.layout.myfamilyWIn, rankData.Battle.WinStreak.ToString());
        }
    }

    public void OnCreateCell(InfinityGridCell cell) {
        Line entry = new Line();
        entry.Init(cell.mRootTransform);
        cell.BindUserData(entry);
    }

    public void OnCellChange(InfinityGridCell cell, int index) {
        var entry = cell.mUserData as Line;
        entry.Refresh(Sys_FamilyResBattle.Instance.ranks[index], index);
    }

    // 请求排行榜数据
    private void _DoReq() {
        // 请求排行榜
        CmdRankQueryReq req = new CmdRankQueryReq();
        req.Type = (uint) RankType.Guild;
        req.Notmain = true;
        req.SubType = (uint) RankTypeGuild.ResFight;
        NetClient.Instance.SendMessage((ushort) CmdRank.QueryReq, req);
    }

    #region 事件处理

    protected override void ProcessEvents(bool toRegister) {
        Sys_Rank.Instance.eventEmitter.Handle<CmdRankQueryRes>(Sys_Rank.EEvents.RankQueryRes, this.OnRankQueryRes, toRegister);
    }

    private void OnRankQueryRes(CmdRankQueryRes res) {
        uint key = Sys_Rank.SetType(res.Type, res.SubType, res.GroupType);
        if (key != curKey) {
            return;
        }

        if (res.Units != null) {
            Sys_FamilyResBattle.Instance.ranks.Clear();
            Sys_FamilyResBattle.Instance.ranks.AddRange(res.Units);
        }

        Sys_FamilyResBattle.Instance.cd = res.NextReqTime; // res.cd;

        this.RefreshAll(key);
    }

    #endregion
}