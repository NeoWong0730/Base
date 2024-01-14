using System;
using pbc = global::Google.Protobuf.Collections;
using Logic.Core;
using Packet;
using System.Collections.Generic;
using Framework;
using Lib.Core;
using Net;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_WorldBossRank : UIBase, UI_WorldBossRank.UIRankTypeListParent.IListener {
        public class UIRankTypeListSub : UISelectableElement {
            private CP_Toggle tg;
            private Text textLight;
            private Text textDark;

            public uint id;

            protected override void Loaded() {
                this.tg = transform.GetComponent<CP_Toggle>();
                this.tg?.onValueChanged.AddListener(this.Switch);

                textLight = transform.Find("Image_Select/Text_Light").GetComponent<Text>();
                textDark = transform.Find("Text_Dark").GetComponent<Text>();
            }

            private void Switch(bool arg) {
                if (arg) {
                    this.onSelected?.Invoke((int) this.id, true);
                }
            }

            public override void SetSelected(bool toSelected, bool force) {
                this.tg.SetSelected(toSelected, true);
            }

            public void Refresh(uint subId) {
                this.id = subId;

                var csvBoss = CSVNpc.Instance.GetConfData(subId);
                if (csvBoss != null) {
                    TextHelper.SetText(this.textLight, LanguageHelper.GetNpcTextContent(csvBoss.name));
                    this.textDark.text = this.textLight.text;
                }
                else {
                    // ---
                }
            }
        }

        public class UIRankTypeListParent : UISelectableElement {
            public interface IListener {
                void OnSelectSub(uint id);
            }

            public IListener listener;

            private CP_Toggle tg;
            public Text text;
            private Image arrow;

            private GameObject proto;
            private Transform protoParent;
            private GameObject children;
            public int currentSubId = -1;
            private UIElementCollector<UIRankTypeListSub> vds = new UIElementCollector<UIRankTypeListSub>();

            public uint id;
            private object userData;
            public List<uint> ids = new List<uint>();
            public Action<int, bool, bool> onClicked;

            public override void OnDestroy() {
                this.vds.Clear();
            }

            protected override void Loaded() {
                this.proto = transform.Find("Content_Small/Toggle_Select01").gameObject;
                this.protoParent = transform.Find("Content_Small");
                this.children = transform.Find("Content_Small").gameObject;

                this.tg = transform.GetComponent<CP_Toggle>();
                this.tg.onValueChanged.AddListener(this.Switch);
                this.text = transform.Find("GameObject/Text_Dark").GetComponent<Text>();
                this.arrow = transform.Find("GameObject/Image_Frame").GetComponent<Image>();
            }

            private void Switch(bool arg) {
                this.onClicked?.Invoke((int) this.id, arg, true);
            }

            public override void SetSelected(bool toSelected, bool force) {
                this.tg.SetSelected(toSelected, true);
            }

            public void SetArrow(bool select) {
                float rotateZ = select ? 0f : 90f;
                this.arrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotateZ);
            }

            public virtual void Refresh(uint id) {
                this.id = id;

                var csvChallenge = CSVChallengeLevel.Instance.GetConfData(id);
                this.userData = csvChallenge;
                if (csvChallenge != null) {
                    TextHelper.SetText(this.text, csvChallenge.rule_name);
                }
            }

            protected virtual List<uint> GetIds() {
                var csv = userData as CSVChallengeLevel.Data;
                ids = csv.BOSS_id != null ? new List<uint>(csv.BOSS_id) : ids;
                ids.RemoveAll((itemId) => {
                    var csvBoss = CSVBOSSInformation.Instance.GetConfData(itemId);
                    return !csvBoss.bossIsRanking;
                });
                return this.ids;
            }

            public void ExpandChildren(bool toExpand) {
                this.children.SetActive(toExpand);
                if (toExpand) {
                    this.ids = this.GetIds();
                    this.vds.BuildOrRefresh(this.proto, this.protoParent, this.ids, this.OnRefreshTab);
                    if (this.currentSubId == -1) {
                        // 默认选中!
                        if (this.ids.Count > 0) {
                            this.vds[0].SetSelected(true, true);
                        }
                    }
                    else {
                        int index = this.ids.IndexOf((uint) this.currentSubId);
                        if (index == -1) {
                            this.vds[0].SetSelected(true, true);
                        }
                        else {
                            this.vds[index].SetSelected(true, true);
                        }
                    }
                }
            }

            private void OnRefreshTab(UIRankTypeListSub vd, uint id, int index) {
                vd.SetSelectedAction((innerId, force) => {
                    this.currentSubId = innerId;
                    this.listener?.OnSelectSub((uint) innerId);
                });
                vd.Refresh(id);
            }
        }

        public class RightItem : UIComponent {
            public Text rank;
            public Image rankIcon;
            public Text roleName;
            public Text career;
            public Text roundNum;
            public Text date;
            public Button btnGoto;

            private RankUnitData rankData;
            private CmdRankQueryRes rankRes;

            protected override void Loaded() {
                rank = transform.Find("Text_Number").GetComponent<Text>();
                rankIcon = transform.Find("Image_Rank").GetComponent<Image>();
                roleName = transform.Find("Text01").GetComponent<Text>();
                career = transform.Find("Text02").GetComponent<Text>();
                roundNum = transform.Find("Text03").GetComponent<Text>();
                date = transform.Find("Text04").GetComponent<Text>();

                btnGoto = transform.Find("Btn_Detail").GetComponent<Button>();
                btnGoto.onClick.AddListener(OnBtnGotoClicked);
            }

            private void OnBtnGotoClicked() {
                var worldBossData = this.rankData.WildBossData;
                if (null != worldBossData) {
                    ulong roleId = worldBossData.RoleId;
                    Sys_Role_Info.Instance.OpenRoleInfo(roleId, Sys_Role_Info.EType.FromAvatar);
                }
            }

            public void Refresh(RankUnitData rankData, CmdRankQueryRes rankRes) {
                this.rankData = rankData;
                this.rankRes = rankRes;

                if (rankData.Rank < 4) {
                    this.rankIcon.gameObject.SetActive(true);
                    this.rank.gameObject.SetActive(false);

                    if (rankData.Rank == 1) {
                        ImageHelper.SetIcon(this.rankIcon, 993901, true);
                    }
                    else if (rankData.Rank == 2) {
                        ImageHelper.SetIcon(this.rankIcon, 993902, true);
                    }
                    else if (rankData.Rank == 3) {
                        ImageHelper.SetIcon(this.rankIcon, 993903, true);
                    }
                }
                else {
                    this.rankIcon.gameObject.SetActive(false);
                    this.rank.gameObject.SetActive(true);

                    TextHelper.SetText(this.rank, rankData.Rank.ToString());
                }

                TextHelper.SetText(this.roleName, rankData.WildBossData.Name.ToStringUtf8());
                TextHelper.SetText(this.career, CSVCareer.Instance.GetConfData(rankData.WildBossData.Career).name);
                TextHelper.SetText(this.roundNum, rankData.Score.ToString());
                var dt = Sys_Time.ConvertToLocalTime(rankData.WildBossData.Time);
                TextHelper.SetText(this.date, dt.ToString(GameMain.beijingCulture));
            }
        }

        public class DropDown : UIPopdownItem {
            public virtual void Refresh(uint zoneId, int index) {
                base.Refresh(zoneId, index);

                var csv = CSVBOOSFightPlayMode.Instance.GetConfData(zoneId);
                if (csv != null) {
                    TextHelper.SetText(this.text, csv.playMode);
                    this.optionName = this.text.text;
                }
            }
        }

        private InfinityGrid infinityGrid;

        private GameObject bigProto;
        private Transform bigProtoParent;
        private UIElementCollector<UIRankTypeListParent> bigVds = new UIElementCollector<UIRankTypeListParent>();

        public GameObject unEmptyGo;
        public EmojiText firstKillText;
        public Text firstKillDate;
        public GameObject emptyGo;

        // my
        public GameObject myRankGo;
        public Text myRank;
        public Image myRankIcon;
        public Text myRoleName;
        public Text myCareer;
        public Text myRoundNum;
        public Text myDate;

        protected override void OnLoaded() {
            this.popdownList = this.transform.Find("Animator/View_Left/PopupList").GetComponent<CP_PopdownList>();
            this.infinityGrid = this.transform.Find("Animator/View_Right/UnEmpty/Scroll_List").GetComponent<InfinityGrid>();
            this.infinityGrid.onCreateCell += this.OnCreateCell;
            this.infinityGrid.onCellChange += this.OnCellChange;

            this.bigProto = this.transform.Find("Animator/View_Left/Scroll01/Content/Toggle_Select01").gameObject;
            this.bigProtoParent = this.bigProto.transform.parent;

            this.emptyGo = this.transform.Find("Animator/View_Right/Empty").gameObject;
            this.unEmptyGo = this.transform.Find("Animator/View_Right/UnEmpty").gameObject;

            this.firstKillText = this.transform.Find("Animator/View_Right/UnEmpty/Team/Roles").GetComponent<EmojiText>();
            this.firstKillText.onHrefClick = this.OnHerfClicked;

            this.firstKillDate = this.transform.Find("Animator/View_Right/UnEmpty/Team/Text_Time").GetComponent<Text>();

            // my
            this.myRankGo = this.transform.Find("Animator/View_Right/UnEmpty/MyRank").gameObject;
            this.myRankIcon = this.transform.Find("Animator/View_Right/UnEmpty/MyRank/BG").GetComponent<Image>();
            this.myRank = this.transform.Find("Animator/View_Right/UnEmpty/MyRank/Text_Rank").GetComponent<Text>();
            this.myRoleName = this.transform.Find("Animator/View_Right/UnEmpty/MyRank/Text_Name").GetComponent<Text>();
            this.myCareer = this.transform.Find("Animator/View_Right/UnEmpty/MyRank/Text_Job").GetComponent<Text>();
            this.myRoundNum = this.transform.Find("Animator/View_Right/UnEmpty/MyRank/Text_Amount").GetComponent<Text>();
            this.myDate = this.transform.Find("Animator/View_Right/UnEmpty/MyRank/Text_Time").GetComponent<Text>();
        }

        private void OnCreateCell(InfinityGridCell cell) {
            RightItem entry = new RightItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index) {
            RightItem entry = cell.mUserData as RightItem;
            var rkList = rankList(curKey);
            if (rkList != null) {
                entry.Refresh(rkList.Units[index], rkList);
            }
        }

        private List<uint> dropList = new List<uint>();
        private int currentDropId = -1;

        public CP_PopdownList popdownList;
        public UIElementCollector<DropDown> popdownVds = new UIElementCollector<DropDown>();

        private List<uint> bigIds = new List<uint>();
        private int currentBigId = -1;

        private int currentSmallId = -1;
        private uint currentRankSubType;

        protected override void OnDestroy() {
            this.popdownVds.Clear();
        }

        protected override void OnOpen(object arg) {
            var tp = arg as Tuple<uint, uint>;
            if (arg != null) {
                this.currentDropId = (int) tp.Item1;
                this.currentBigId = (int) tp.Item2;
            }
        }

        protected override void OnOpened() {
            this.dropList = Sys_WorldBoss.Instance.GetSortedActivities(true, false, true);
            this.popdownVds.BuildOrRefresh(this.popdownList.optionProto, this.popdownList.optionParent, this.dropList, this.OnRefreshPopDown);

            int index = dropList.IndexOf((uint) this.currentDropId);
            if (index == -1) {
                if (this.popdownVds.Count > 0) {
                    index = 0;
                    this.currentDropId = (int) this.dropList[index];
                    this.popdownVds[index].SetSelected(true, true);
                }
            }
            else {
                this.popdownVds[index].SetSelected(true, true);
            }
        }

        private void OnRefreshPopDown(DropDown vd, uint id, int index) {
            vd.SetUniqueId((int) id);
            vd.SetSelectedAction((zondId, force) => {
                this.popdownVds.ForEach((e) => { e.SetHighlight(false); });
                vd.SetHighlight(true);

                this.popdownList.Expand(false);
                this.popdownList.SetSelected(vd.optionName);
                // 选中
                int idx = this.dropList.IndexOf((uint) zondId);
                this.popdownList.MoveTo(false, 1f * (idx + 1) / this.dropList.Count);

                this.currentDropId = zondId;
                var csv = CSVBOOSFightPlayMode.Instance.GetConfData((uint) this.currentDropId);
                this.bigIds = new List<uint>(csv.difficulty_id);
                this.bigIds.RemoveAll((itemId) => {
                    var csvChallenge = CSVChallengeLevel.Instance.GetConfData(itemId);
                    return !csvChallenge.challengeLevelIsRanking;
                });

                // id排序按照策划填表

                this.RefreshContent();
            });
            vd.Refresh(id, index);
            vd.SetHighlight(false);
        }

        private void RefreshContent() {
            this.bigVds.BuildOrRefresh(this.bigProto, this.bigProtoParent, this.bigIds, this.OnRefreshTab);

            // 选中
            int index = this.bigIds.IndexOf((uint) this.currentBigId);
            if (index == -1) {
                if (this.bigVds.Count > 0) {
                    index = 0;
                    this.currentBigId = (int) this.bigIds[index];
                    this.bigVds[index].SetSelected(true, true);
                }
            }
            else {
                this.bigVds[index].SetSelected(true, true);
            }
        }

        private void OnRefreshTab(UIRankTypeListParent vd, uint id, int index) {
            vd.listener = this;
            vd.onClicked = ((innerId, selected, force) => {
                vd.SetArrow(selected);
                vd.ExpandChildren(selected);

                if (selected) {
                    this.currentBigId = innerId;
                }

                FrameworkTool.ForceRebuildLayout(this.bigProtoParent.gameObject);
            });

            vd.Refresh(id);
        }

        private uint curKey = 0;
        public void OnSelectSub(uint id) {
            this.currentSmallId = (int) id;

            var csv = CSVBOSSInformation.Instance.GetConfData(id);
            if (csv != null) {
                this.currentRankSubType = csv.bossRankSubType;

                this.curKey = Sys_Rank.SetType((uint) RankType.WildBoss, csv.bossRankSubType, 0);
                
                long curTime = Sys_Time.Instance.GetServerTime();
                if (Sys_WorldBoss.Instance.cds.TryGetValue(this.curKey, out Sys_WorldBoss.Rank rank)) {
                    if (curTime >= rank.cd) {
                        _DoReq();
                    }
                    else {
                        this.RefreshRight(this.curKey);
                    }
                }
                else {
                    _DoReq();
                }

                void _DoReq() {
                    // 请求排行榜
                    CmdRankQueryReq req = new CmdRankQueryReq();
                    req.Type = (uint) RankType.WildBoss;
                    req.Notmain = true;
                    req.SubType = csv.bossRankSubType;
                    NetClient.Instance.SendMessage((ushort) CmdRank.QueryReq, req);
                }
            }
        }

        private void OnHerfClicked(string arg) {
            if (ulong.TryParse(arg, out var roleId)) {
                Sys_Role_Info.Instance.OpenRoleInfo(roleId, Sys_Role_Info.EType.FromAvatar);
            }
        }

        private void RefreshRight(uint key) {
            var rkList = this.rankList(key);
            if (rkList == null || rkList.Units.Count <= 0) {
                this.emptyGo.SetActive(true);
                this.unEmptyGo.SetActive(false);
            }
            else {
                this.emptyGo.SetActive(false);
                this.unEmptyGo.SetActive(true);

                this.infinityGrid.CellCount = rkList.Units.Count;
                this.infinityGrid.ForceRefreshActiveCell();

                this.RefreshFirstKill(key);
                this.RefreshMy(key);
            }
        }

        private void RefreshFirstKill(uint key) {
            string text = "";
            var firstRecord = this.firstkillRecord(key);
            if (firstRecord != null) {
                int length = firstRecord.Roles.Count;
                if (length < 1) {
                    TextHelper.SetText(this.firstKillText, text);
                    TextHelper.SetText(this.firstKillDate, "");
                }
                else if (length == 1) {
                    text = firstRecord.Roles[0].Name.ToStringUtf8();
                    text = LanguageHelper.GetTextContent(4157000023, firstRecord.Roles[0].Id.ToString(), text);
                }
                else {
                    var sb = StringBuilderPool.GetTemporary();
                    sb.Clear();
                    text = firstRecord.Roles[0].Name.ToStringUtf8();
                    sb.AppendFormat(LanguageHelper.GetTextContent(4157000023), firstRecord.Roles[0].Id.ToString(), text);

                    for (int i = 1; i < length; ++i) {
                        string name = firstRecord.Roles[i].Name.ToStringUtf8();
                        sb.Append("、");
                        sb.AppendFormat(LanguageHelper.GetTextContent(4157000023),  firstRecord.Roles[i].Id.ToString(), name);
                    }

                    text = sb.ToString();
                    StringBuilderPool.ReleaseTemporary(sb);
                }

                TextHelper.SetText(this.firstKillText, text);
                var dt = Sys_Time.ConvertToLocalTime(firstRecord.Timestamp);
                TextHelper.SetText(this.firstKillDate, dt.ToString(GameMain.beijingCulture));
            }
            else {
                TextHelper.SetText(this.firstKillText, text);
                TextHelper.SetText(this.firstKillDate, "");
            }
        }

        private void RefreshMy(uint key) {
            var my = mykillRecord(key);
            if (my == null) {
                this.myRankGo.SetActive(false);
                return;
            }
            
            var rkList = this.rankList(key);

            this.myRankGo.SetActive(true);
            int i = 0, length = rkList.Units.Count;
            int rank = -1;
            for (; i < length; ++i) {
                if (rkList.Units[i].WildBossData.RoleId == Sys_Role.Instance.RoleId) {
                    rank = i;
                    break;
                }
            }

            if (rank == -1) {
                TextHelper.SetText(this.myRank, LanguageHelper.GetTextContent(4157000022));
            }
            else {
                if (rank < 3) {
                    this.myRankIcon.gameObject.SetActive(true);
                    this.myRank.gameObject.SetActive(false);

                    if (rank == 0) {
                        ImageHelper.SetIcon(this.myRankIcon, 993901, true);
                    }
                    else if (rank == 1) {
                        ImageHelper.SetIcon(this.myRankIcon, 993902, true);
                    }
                    else if (rank == 2) {
                        ImageHelper.SetIcon(this.myRankIcon, 993903, true);
                    }
                }
                else {
                    this.myRankIcon.gameObject.SetActive(false);
                    this.myRank.gameObject.SetActive(true);

                    TextHelper.SetText(this.myRank, (rank + 1).ToString());
                }
            }

            TextHelper.SetText(this.myRoleName, Sys_Role.Instance.sRoleName);
            TextHelper.SetText(this.myCareer, CSVCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career).name);
            TextHelper.SetText(this.myRoundNum, my.MinRound.ToString());
            var dt = Sys_Time.ConvertToLocalTime(my.Timestamp);
            TextHelper.SetText(this.myDate, dt.ToString(GameMain.beijingCulture));
        }

        #region 事件处理

        protected override void ProcessEvents(bool toRegister) {
            Sys_Rank.Instance.eventEmitter.Handle<CmdRankQueryRes>(Sys_Rank.EEvents.RankQueryRes, this.OnRankQueryRes, toRegister);
        }

        public RankQueryWildBossFirstKill firstkillRecord(uint key){
            if (Sys_WorldBoss.Instance.cds.TryGetValue(key, out Sys_WorldBoss.Rank rank)) {
                return rank.firstkillRecord;
            }

            return null;
        }
        private RankQueryWildBossSelf mykillRecord(uint key){
            if (Sys_WorldBoss.Instance.cds.TryGetValue(key, out Sys_WorldBoss.Rank rank)) {
                return rank.mykillRecord;
            }

            return null;
        }
        private CmdRankQueryRes rankList(uint key){
            if (Sys_WorldBoss.Instance.cds.TryGetValue(key, out Sys_WorldBoss.Rank rank)) {
                return rank.rankList;
            }

            return null;
        }
        
        private void OnRankQueryRes(CmdRankQueryRes res) {
            if (this.currentRankSubType == res.SubType) {
                uint key = Sys_Rank.SetType(res.Type, res.SubType, res.GroupType);
                if (!Sys_WorldBoss.Instance.cds.TryGetValue(key, out Sys_WorldBoss.Rank rank)) {
                    rank = new Sys_WorldBoss.Rank();
                    rank.cd = res.NextReqTime;
                    rank.rankList = res;
                    rank.firstkillRecord = res.Extra.Wildboss.Firstkill;
                    rank.mykillRecord = res.Extra.Wildboss.Self;
                    Sys_WorldBoss.Instance.cds.Add(key, rank);
                }
                else {
                    rank.cd = res.NextReqTime;
                    rank.rankList = res;
                    rank.firstkillRecord = res.Extra.Wildboss.Firstkill;
                    rank.mykillRecord = res.Extra.Wildboss.Self;
                }
                
                this.RefreshRight(key);
            }
        }

        #endregion
    }
}