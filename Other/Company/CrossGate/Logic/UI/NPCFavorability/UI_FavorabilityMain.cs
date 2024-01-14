using System.Collections.Generic;
using Logic.Core;
using Net;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

namespace Logic {
    public abstract class UILayoutBase {
        public GameObject gameObject;
        public Transform transform;

        public void Init(GameObject root) {
            this.gameObject = root;
            this.transform = root.transform;
        }
    }

    public class UI_FavorabilityMain_Layout : UILayoutBase {
        public Button btnExit;

        public GameObject noNpcsGo;
        public GameObject haveNpcsGo;
        
        public InfinityGrid infinity;

        public Text playerFavorability;
        public Text zoneFavorability;
        public GameObject rewardIconGot; // 已经领取
        public GameObject rewardIconReached; // 到达领取条件
        public GameObject rewardIconUnreached; // 未到达领取条件
        public GameObject zoneRewardShowNode;

        public Button btnPlayerFavorabilityInfo;

        public Button btnZoneReward;
        public GameObject favorabilityGo;
        public CP_PopdownList popdownList;

        public void Parse(GameObject root) {
            this.Init(root);

            this.btnPlayerFavorabilityInfo = this.transform.Find("Animator/View_NPC/View_Right/Button_Tips").GetComponent<Button>();
            this.btnExit = this.transform.Find("Animator/View_Title10/Btn_Close").GetComponent<Button>();
            this.btnZoneReward = this.transform.Find("Animator/View_NPC/View_Right/NpcFavorability/Image").GetComponent<Button>();
            this.favorabilityGo = this.transform.Find("Animator/View_NPC/View_Right/NpcFavorability").gameObject;

            this.popdownList = this.transform.Find("Animator/View_NPC/View_Left/PopupList").GetComponent<CP_PopdownList>();

            this.noNpcsGo = this.transform.Find("Animator/View_Empty").gameObject;
            this.haveNpcsGo = this.transform.Find("Animator/View_NPC").gameObject;

            this.playerFavorability = this.transform.Find("Animator/View_NPC/View_Right/PlayerFavorability/Text_Number").GetComponent<Text>();
            this.zoneFavorability = this.transform.Find("Animator/View_NPC/View_Right/NpcFavorability/Text_Number").GetComponent<Text>();
            this.rewardIconGot = this.transform.Find("Animator/View_NPC/View_Right/NpcFavorability/Image/Image_Open").gameObject;
            this.rewardIconReached = this.transform.Find("Animator/View_NPC/View_Right/NpcFavorability/Image/Image_Selected").gameObject;
            this.rewardIconUnreached = this.transform.Find("Animator/View_NPC/View_Right/NpcFavorability/Image/Image_Get").gameObject;
            this.zoneRewardShowNode = this.transform.Find("Animator/View_NPC/View_Right/ZoneAward").gameObject;
        }

        public void RegisterEvents(IListener listener) {
            this.btnExit.onClick.AddListener(listener.OnBtnExitClicked);
            this.btnZoneReward.onClick.AddListener(listener.OnBtnZoneRewardClicked);
            this.btnPlayerFavorabilityInfo.onClick.AddListener(listener.OnBtnPlayerFavorabilityInfoClicked);
        }

        public interface IListener {
            void OnBtnExitClicked();
            void OnBtnZoneRewardClicked();
            void OnBtnPlayerFavorabilityInfoClicked();
        }
    }

    // 好感度主界面
    public class UI_FavorabilityMain : UIBase, UI_FavorabilityMain_Layout.IListener, UI_FavorabilityMain.Tab.IListener {
        public class Tab : UIComponent {
            public GameObject lockGo;
            public GameObject unlockGo;

            public Image npcIcon;
            public Text npcName;
            public Text npcNameLock;
            public Text npcType;
            public Text npcFavorablityStage;
            public Image npcMoodIcon;
            public Image npcHealthIcon;

            public uint npcId;
            public UI_FavorabilityMain ui;

            protected override void Loaded() {
                this.lockGo = this.transform.Find("list_Lock").gameObject;
                this.unlockGo = this.transform.Find("list_Unlock").gameObject;

                this.npcIcon = this.transform.Find("list_Unlock/Image_Icon").GetComponent<Image>();
                this.npcMoodIcon = this.transform.Find("list_Unlock/Image_Mood").GetComponent<Image>();
                this.npcHealthIcon = this.transform.Find("list_Unlock/Image_Health").GetComponent<Image>();
                this.npcName = this.transform.Find("list_Unlock/Text_Name").GetComponent<Text>();
                this.npcNameLock = this.transform.Find("list_Lock/Text (1)").GetComponent<Text>();
                this.npcType = this.transform.Find("list_Unlock/Text_Type").GetComponent<Text>();
                this.npcFavorablityStage = this.transform.Find("list_Unlock/Text_Likability").GetComponent<Text>();

                Button btn = this.transform.Find("list_Unlock/Image1").GetComponent<Button>();
                btn.onClick.AddListener(this.OnBtnUnlockClicked);
                
                btn = this.transform.Find("list_Lock/Image").GetComponent<Button>();
                btn.onClick.AddListener(this.OnBtnLockClicked);
            }

            public void Refresh(uint npcId, UI_FavorabilityMain ui) {
                this.ui = ui;
                this.npcId = npcId;

                this.lockGo.SetActive(false);
                this.unlockGo.SetActive(false);

                if (Sys_NPCFavorability.Instance.TryGetNpc(npcId, out var npc)) {
                    this.unlockGo.SetActive(true);
                    CSVFavorabilityStageName.Data csvStageName = CSVFavorabilityStageName.Instance.GetConfData(npc.csvNPCFavorabilityStage.Stage);
                    if (csvStageName != null) {
                        TextHelper.SetText(this.npcFavorablityStage, csvStageName.name);
                    }

                    CSVNpc.Data csvNPC = CSVNpc.Instance.GetConfData(npc.id);
                    if (csvNPC != null) {
                        TextHelper.SetText(this.npcName, LanguageHelper.GetNpcTextContent(csvNPC.name));
                    }

                    CSVNPCOccupation.Data csvNPCOccupation = CSVNPCOccupation.Instance.GetConfData(npc.csvNPCFavorability.Occupation);
                    if (csvNPCOccupation != null) {
                        ImageHelper.SetIcon(this.npcIcon, csvNPCOccupation.icon);
                        TextHelper.SetText(this.npcType, csvNPCOccupation.name);
                    }

                    CSVNPCMood.Data csvNPCMood = CSVNPCMood.Instance.GetConfData(npc.moodId);
                    if (csvNPCMood != null) {
                        ImageHelper.SetIcon(this.npcMoodIcon, csvNPCMood.Icon);
                    }

                    CSVNPCDisease.Data csvDisease = CSVNPCDisease.Instance.GetConfData(npc.healthId);
                    if (csvDisease != null) {
                        ImageHelper.SetIcon(this.npcHealthIcon, csvDisease.Icon);
                    }
                    else {
                        ImageHelper.SetIcon(this.npcHealthIcon, CSVNPCHealth.Instance.GetConfData(2).Icon);
                    }
                }
                else {
                    this.lockGo.SetActive(true);
                    CSVNpc.Data csvNPC = CSVNpc.Instance.GetConfData(npcId);
                    if (csvNPC != null) {
                        TextHelper.SetText(this.npcNameLock, LanguageHelper.GetNpcTextContent(csvNPC.name));
                    }
                }
            }

            private void OnBtnUnlockClicked() {
                UIManager.OpenUI(EUIID.UI_FavorabilityNPCShow, false, this.npcId);
            }

            private void OnBtnLockClicked() {
                this.listner?.OnClickedLock(this.npcId);
            }

            public IListener listner;
            public interface IListener {
                void OnClickedLock(uint npcId);
            }
        }

        public class ZoneRewardShow : UIComponent {
            public UI_FavorabilityMain ui;
            public uint zoneId;

            public Transform rewardParent;
            public UI_RewardList rewardList;
            public Text desc;
            public Button btnBg;

            protected override void Loaded() {
                this.rewardParent = this.transform.Find("View_Award/Scroll_View/Viewport").transform;
                this.btnBg = this.transform.Find("Image_Black").GetComponent<Button>();
                this.desc = this.transform.Find("View_Award/Image_Title/Text_Des").GetComponent<Text>();
                this.btnBg.onClick.AddListener(this.OnBtnClicked);
            }

            public void Refresh(UI_FavorabilityMain ui, uint zoneId) {
                this.ui = ui;
                this.zoneId = zoneId;

                if (this.rewardList == null) {
                    this.rewardList = new UI_RewardList(this.rewardParent, EUIID.UI_FavorabilityMain);
                }

                this.rewardList.SetRewardList(ZoneFavorabilityNPC.GetRewards(zoneId));
                bool hasGot = ui.zoneRewardStatus == EZoneRewardStatus.Got;
                this.rewardList.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, hasGot);

                if (Sys_NPCFavorability.Instance.GetZoneNpcs(zoneId, out ZoneFavorabilityNPC zoneNpc, false)) {
                    TextHelper.SetText(this.desc, zoneNpc.csv.RewardDes);
                }

                Lib.Core.FrameworkTool.ForceRebuildLayout(this.gameObject);
            }

            private void OnBtnClicked() {
                this.ui.zoneRewardShow.Hide();
            }
        }

        public class PopdownItem : UIPopdownItem {
            public override void Refresh(uint zoneId, int index) {
                base.Refresh(zoneId, index);

                if (zoneId != PopDownAllId) {
                    CSVFavorabilityPlaceReward.Data csv = CSVFavorabilityPlaceReward.Instance.GetConfData(zoneId);
                    if (csv != null) {
                        TextHelper.SetText(this.text, csv.PlaceName);
                        this.optionName = this.text.text;
                    }
                }
                else {
                    string s = LanguageHelper.GetTextContent(4534);
                    TextHelper.SetText(this.text, s);
                    this.optionName = s;
                }
            }
        }

        public static readonly int PopDownAllId = int.MaxValue - 1;
        public UIElementCollector<PopdownItem> popdownVds = new UIElementCollector<PopdownItem>();
        public UI_FavorabilityMain_Layout Layout = new UI_FavorabilityMain_Layout();
        public ZoneRewardShow zoneRewardShow;
        
        public uint currentZoneId = 0;
        public EZoneRewardStatus zoneRewardStatus;
        public List<uint> npcIds = new List<uint>();

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);

            this.Layout.infinity = this.transform.Find("Animator/View_NPC/View_Left/Scroll").GetComponent<InfinityGrid>();
            this.Layout.infinity.onCreateCell += this.OnCreateCell;
            this.Layout.infinity.onCellChange += this.OnCellChange;
            
            this.Layout.haveNpcsGo.SetActive(true);
        }
        
        private void OnCreateCell(InfinityGridCell cell) {
            Tab entry = new Tab();
            entry.listner = this;
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index) {
            Tab entry = cell.mUserData as Tab;
            entry.Refresh(this.npcIds[index], this);
        }

        public void OnBtnExitClicked() {
            this.CloseSelf();
        }

        private readonly UIRuleParam par = new UIRuleParam {
            StrContent = LanguageHelper.GetTextContent(2010646),
        };

        public void OnBtnPlayerFavorabilityInfoClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityTip);
        }

        public void OnBtnZoneRewardClicked() {
            if (this.zoneRewardShow == null) {
                this.zoneRewardShow = new ZoneRewardShow();
                this.zoneRewardShow.Init(this.Layout.zoneRewardShowNode.transform);
            }

            this.zoneRewardShow.Refresh(this, this.currentZoneId);
            this.zoneRewardShow.Show();
            Lib.Core.FrameworkTool.ForceRebuildLayout(this.gameObject);

            if (this.zoneRewardStatus == EZoneRewardStatus.Reached) {
                Sys_NPCFavorability.Instance.ReqCmdFavorabilityGetZoneReward(this.currentZoneId);
            }
        }

        protected override void OnOpen(object arg) {
            if (this.bLoaded) {
                this.Layout.haveNpcsGo.SetActive(true);
            }
            
            // 重置数据
            this.currentZoneId = Sys_NPCFavorability.Instance.selectedZoneId;
        }

        protected override void OnShow() {
            this.RefreshAll();
        }

        private List<uint> zones = new List<uint>();
        public void RefreshAll() {
            bool haveAnyNpc = Sys_NPCFavorability.Instance.unluckNpcCount > 0;
            this.Layout.haveNpcsGo.SetActive(haveAnyNpc);
            this.Layout.noNpcsGo.SetActive(!haveAnyNpc);

            this.zoneRewardShow?.Hide();
            if (!haveAnyNpc) {
            }
            else {
                // 设置当前下拉菜单中的默认选中项
                var keys = Sys_NPCFavorability.Instance.zoneNpcs.Keys;
                this.zones = new List<uint>(keys);
                this.zones.Sort((left, right) => {
                    var csvL = CSVFavorabilityPlaceReward.Instance.GetConfData(left);
                    var csvR = CSVFavorabilityPlaceReward.Instance.GetConfData(right);
                    if (csvL != null && csvR != null) {
                        return (int)((long)csvL.SortID - (long)csvR.SortID);
                    }
                    else {
                        return 0;
                    }
                });
                this.zones.Insert(0, (uint)PopDownAllId);
                this.popdownVds.BuildOrRefresh(this.Layout.popdownList.optionProto, this.Layout.popdownList.optionParent, this.zones, this.OnRefreshPopDown);

                int index = this.zones.IndexOf(this.currentZoneId);
                if (index == -1) {
                    if (this.popdownVds.Count > 0) {
                        index = 0;
                        this.currentZoneId = this.zones[index];
                        this.popdownVds[index].SetSelected(true, true);
                    }
                }
                else {
                    this.popdownVds[index].SetSelected(true, true);
                }

                this.RefreshPlayerFavorability();
            }

            Lib.Core.FrameworkTool.ForceRebuildLayout(this.gameObject);
        }

        private void OnRefreshPopDown(PopdownItem vd, uint id, int index) {
            vd.SetUniqueId((int) id);
            vd.SetSelectedAction((zondId, force) => {
                this.popdownVds.ForEach((e) => { e.SetHighlight(false); });
                vd.SetHighlight(true);

                this.Layout.popdownList.Expand(false);
                this.Layout.popdownList.SetSelected(vd.optionName);

                // if (this.currentZoneId != currentZoneId)
                {
                    Sys_NPCFavorability.Instance.selectedZoneId = this.currentZoneId = (uint) zondId;
                    Sys_NPCFavorability.Instance.ReqSelectTargetZone(this.currentZoneId);
                    this.Layout.favorabilityGo.SetActive(zondId != PopDownAllId);
                    
                    this.npcIds.Clear();
                    if (zondId == PopDownAllId) {
                        for (int i = 0, length = zones.Count; i < length; ++i) {
                            var oneZone = zones[i];
                            ZoneFavorabilityNPC.GetIds(oneZone, false, this.npcIds);
                        }
                    }
                    else {
                        ZoneFavorabilityNPC.GetIds(this.currentZoneId, false, this.npcIds);
                    }
                    
                    // 排序：解锁的在前
                    this.npcIds.Sort((leftId, rigtId) => {
                        bool leftUnlock = Sys_NPCFavorability.Instance.TryGetNpc(leftId, out FavorabilityNPC leftNpc);
                        bool rightUnlock = Sys_NPCFavorability.Instance.TryGetNpc(rigtId, out FavorabilityNPC rightNpc);
                        bool ll = (leftUnlock && !rightUnlock);
                        bool rr = (!leftUnlock && rightUnlock);

                        if (leftUnlock && rightUnlock) {
                            return (int) ((long) leftNpc.csvNPCFavorability.sortid - (long) rightNpc.csvNPCFavorability.sortid);
                        }
                        else {
                            if (ll || rr) {
                                if (ll) {
                                    return -1;
                                }
                                else {
                                    return 1;
                                }
                            }
                            else {
                                return (int) (leftId - rigtId);
                            }
                        }
                    });

                    this.RefreshContent();
                }
            });
            vd.Refresh(id, index);
            vd.SetHighlight(false);
        }

        public void RefreshContent() {
            // 先下拉菜单设置currentZoneId
            this.RefreshNpcFavorability();
            this.RefreshNpcs();
        }

        public void RefreshPlayerFavorability() {
            this.Layout.playerFavorability.text = Sys_NPCFavorability.Instance.Favorability.ToString();
        }

        public enum EZoneRewardStatus {
            Got,
            Reached,
            Unreached,
        }

        public void RefreshNpcFavorability() {
            this.Layout.rewardIconGot.SetActive(false);
            this.Layout.rewardIconReached.SetActive(false);
            this.Layout.rewardIconUnreached.SetActive(false);

            int zoneLeft = 0;
            int zoneRight = ZoneFavorabilityNPC.GetTotalCount(this.currentZoneId);
            Sys_NPCFavorability.Instance.GetZoneNpcs(this.currentZoneId, out ZoneFavorabilityNPC zoneNpcs);
            if (zoneNpcs == null || zoneNpcs.Count <= 0) {
                // 该区域没有npc解锁过
                zoneLeft = 0;
                this.Layout.rewardIconUnreached.SetActive(true);
                this.zoneRewardStatus = EZoneRewardStatus.Unreached;
            }
            else {
                zoneLeft = zoneNpcs.ReachedMaxCount;
                if (zoneRight != 0 && zoneLeft >= zoneRight) {
                    if (zoneNpcs.gotRewads) {
                        this.Layout.rewardIconGot.SetActive(true);
                        this.zoneRewardStatus = EZoneRewardStatus.Got;
                    }
                    else {
                        this.Layout.rewardIconReached.SetActive(true);
                        this.zoneRewardStatus = EZoneRewardStatus.Reached;
                    }
                }
                else {
                    this.Layout.rewardIconUnreached.SetActive(true);
                    this.zoneRewardStatus = EZoneRewardStatus.Unreached;
                }
            }

            this.Layout.zoneFavorability.text = string.Format("{0}/{1}", zoneLeft, zoneRight);
        }

        public void RefreshNpcs() {
            // if (Sys_NPCFavorability.Instance.GetZoneNpcs(this.currentZoneId, out var zone)) {
            //     this.Layout.infinity.SetAmount(this.npcIds.Count);
            // }
            // else {
            //     this.Layout.infinity.SetAmount(0);
            // }
            
            this.Layout.infinity.CellCount = this.npcIds.Count;
            this.Layout.infinity.ForceRefreshActiveCell();
        }
        
        #region 事件

        protected override void ProcessEvents(bool toRegister) {
            Sys_NPCFavorability.Instance.eventEmitter.Handle(Sys_NPCFavorability.EEvents.OnPlayerFavorabilityChanged, this.OnPlayerFavorabilityChanged, toRegister);
            Sys_NPCFavorability.Instance.eventEmitter.Handle<uint, uint, uint, uint>(Sys_NPCFavorability.EEvents.OnNpcFavorabilityChanged, this.OnNpcFavorabilityChanged, toRegister);
            Sys_NPCFavorability.Instance.eventEmitter.Handle<uint, uint>(Sys_NPCFavorability.EEvents.OnNPCUnlock, this.OnNPCUnlock, toRegister);
            Sys_NPCFavorability.Instance.eventEmitter.Handle<uint>(Sys_NPCFavorability.EEvents.OnZoneUnlock, this.OnZoneUnlock, toRegister);
            Sys_NPCFavorability.Instance.eventEmitter.Handle<uint>(Sys_NPCFavorability.EEvents.OnZoneRewardGot, this.OnZoneRewardGot, toRegister);
            Sys_NPCFavorability.Instance.eventEmitter.Handle(Sys_NPCFavorability.EEvents.OnAllInfoReceived, this.OnAllInfoReceived, toRegister);

            if (toRegister) {
                EventDispatcher.Instance.AddEventListener((ushort)CmdFavorability.FirstUnlockNpcReq, (ushort)CmdFavorability.FirstUnlockNpcRes, this.OnFirstUnlockNpc, CmdFavorabilityFirstUnlockNpcRes.Parser);
            }
            else {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdFavorability.FirstUnlockNpcRes, this.OnFirstUnlockNpc);
            }
        }

        private void OnPlayerFavorabilityChanged() {
            if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                this.RefreshAll();
            }
        }

        private void OnNpcFavorabilityChanged(uint npcId, uint stageId, uint from, uint to) {
            if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                this.RefreshAll();
            }
        }

        private void OnNPCUnlock(uint zoneId, uint npcId) {
            if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                this.RefreshAll();
            }
        }

        private void OnZoneUnlock(uint zoneId) {
            if (this.gameObject != null && this.gameObject.activeInHierarchy && this.currentZoneId == zoneId) {
                this.RefreshAll();
            }
        }

        private void OnZoneRewardGot(uint zoneId) {
            if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                this.RefreshAll();
                this.zoneRewardShow?.Show();
            }
        }

        private void OnAllInfoReceived() {
            if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                this.RefreshAll();
            }
        }

        private uint currentNpcId = 0;
        private void ReqFirstUnlock(uint npcId) {
            this.currentNpcId = npcId;
            this.FirstUnlockNpcReq(npcId);
        }

        public void FirstUnlockNpcReq(uint npcId) {
            CmdFavorabilityFirstUnlockNpcReq req = new CmdFavorabilityFirstUnlockNpcReq {NpcId = npcId};
            NetClient.Instance.SendMessage((ushort)CmdFavorability.FirstUnlockNpcReq, req);
        }

        private void OnFirstUnlockNpc(NetMsg msg) {
            CmdFavorabilityFirstUnlockNpcRes res = NetMsgUtil.Deserialize<CmdFavorabilityFirstUnlockNpcRes>(CmdFavorabilityFirstUnlockNpcRes.Parser, msg);
            if (res.NpcId == this.currentNpcId) {
                if (res.FirstRoleId != 0) {
                    UIManager.OpenUI(EUIID.UI_FavorabilityFirstUnlock, false, res);
                }
                else {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010647));
                }
            }
        }

        #endregion

        public void OnClickedLock(uint npcId) {
            this.ReqFirstUnlock(npcId);
        }
    }
}