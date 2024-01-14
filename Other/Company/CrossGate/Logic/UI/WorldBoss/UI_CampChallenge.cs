using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    // 阵营挑战
    public class UI_CampChallenge : UIBase, UI_CampChallenge.Head.IListener {
        public class Tab : UISelectableElement {
            public Text indexDark;
            public Text nameDark;
            public Text levelUpLimitDark;
            public Image iconDark;

            public Text indexLight;
            public Text nameLight;
            public Text levelUpLimitLight;
            public Image iconLight;

            public CP_Toggle toggle;

            public uint activityId;
            public UI_CampChallenge ui;

            protected override void Loaded() {
                this.toggle = this.transform.GetComponent<CP_Toggle>();

                this.indexDark = this.transform.Find("Image_bg/Text_num").GetComponent<Text>();
                this.nameDark = this.transform.Find("Image_bg/Label").GetComponent<Text>();
                this.levelUpLimitDark = this.transform.Find("Image_bg/Text_lv").GetComponent<Text>();
                this.iconDark = this.transform.Find("Image_bg/Image").GetComponent<Image>();

                this.indexLight = this.transform.Find("Image_selectbg/Text_num").GetComponent<Text>();
                this.nameLight = this.transform.Find("Image_selectbg/Label").GetComponent<Text>();
                this.levelUpLimitLight = this.transform.Find("Image_selectbg/Text_lv").GetComponent<Text>();
                this.iconLight = this.transform.Find("Image_selectbg/Image").GetComponent<Image>();

                this.toggle.onValueChanged.AddListener(this.Switch);
            }

            public bool IsPlayerLevelValid {
                get {
                    bool valid = false;
                    CSVChallengeLevel.Data csvLevel = CSVChallengeLevel.Instance.GetConfData(this.activityId);
                    if (csvLevel != null) {
                        valid = true;
                        valid = csvLevel.challengeLevelLimit[0] <= Sys_Role.Instance.Role.Level;
                        valid &= Sys_Role.Instance.Role.Level <= csvLevel.challengeLevelLimit[1];
                    }

                    return valid;
                }
            }

            public void Refresh(UI_CampChallenge ui, uint activityId) {
                this.ui = ui;
                this.activityId = activityId;

                CSVChallengeLevel.Data csvLevel = CSVChallengeLevel.Instance.GetConfData(activityId);
                if (csvLevel != null) {
                    this.Show();
                    TextHelper.SetText(this.nameDark, csvLevel.rule_name);
                    TextHelper.SetText(this.nameLight, csvLevel.rule_name);

                    TextHelper.SetText(this.indexDark, csvLevel.difficulty_level.ToString());
                    TextHelper.SetText(this.indexLight, csvLevel.difficulty_level.ToString());

                    ImageHelper.SetIcon(this.iconDark, ui.csvCurrentActivity.headIcon_id);
                    ImageHelper.SetIcon(this.iconLight, ui.csvCurrentActivity.headIcon_id);

                    if (this.IsPlayerLevelValid) {
                        string content = LanguageHelper.GetTextContent(4157000001, csvLevel.challengeLevelLimit[0].ToString(), csvLevel.challengeLevelLimit[1].ToString());
                        TextHelper.SetText(this.levelUpLimitDark, content);
                        TextHelper.SetText(this.levelUpLimitLight, content);
                    }
                    else {
                        string content = LanguageHelper.GetTextContent(4157000002, csvLevel.challengeLevelLimit[0].ToString(), csvLevel.challengeLevelLimit[1].ToString());
                        TextHelper.SetText(this.levelUpLimitDark, content);
                        TextHelper.SetText(this.levelUpLimitLight, content);
                    }
                }
                else {
                    this.Hide();
                }
            }

            public void Switch(bool arg) {
                if (arg) {
                    this.onSelected?.Invoke((int) this.activityId, true);
                }
            }

            public override void SetSelected(bool toSelected, bool force) {
                this.toggle.SetSelected(toSelected, true);
            }
        }

        public class Detail : UIComponent {
            public Text pos;
            public Text skill;

            protected override void Loaded() {
                this.pos = this.transform.Find("Image_BG/Title0/Text_Content").GetComponent<Text>();
                this.skill = this.transform.Find("Image_BG/Title1/Text_Content").GetComponent<Text>();
                Button btn = this.transform.Find("Image_BG/Btn_Close").GetComponent<Button>();
                btn.onClick.AddListener(this.OnBtnClicked);

                btn = this.transform.Find("Image")?.GetComponent<Button>();
                btn?.onClick?.AddListener(this.OnBtnClicked);
            }

            public void Refresh(uint bossId) {
                CSVBOSSInformation.Data csvBoss = CSVBOSSInformation.Instance.GetConfData(bossId);
                if (csvBoss != null) {
                    TextHelper.SetText(this.pos, csvBoss.location_description);
                    TextHelper.SetText(this.skill, csvBoss.AI_description);

                    this.refreshListTimer?.Cancel();
                    this.refreshListTimer = Timer.RegisterOrReuse(ref this.refreshListTimer, 0.04f, this.OnTimerCompleted);
                }
            }

            private Timer refreshListTimer;

            private void OnTimerCompleted() {
                FrameworkTool.ForceRebuildLayout(this.gameObject);
            }

            private void OnBtnClicked() {
                this.Hide();
            }

            public override void OnDestroy() {
                this.refreshListTimer?.Cancel();
                base.OnDestroy();
            }
        }

        public Transform campRedDot;
        public GameObject detailGo;
        public Detail detailContent;

        public class Head : UISelectableElement {
            public interface IListener {
                void OnHeadClicked(uint id, bool isUnlock, Vector3 anchor);
            }

            public IListener listener;

            public GameObject unknown;
            public GameObject known;
            public Image remainImg1;
            public Image remainImg2;
            public Text remainText;
            public Image icon;
            public Transform border;

            public CP_Toggle toggle;

            public UI_CampChallenge ui;
            public uint bossId;
            public bool isUnlocked;

            protected override void Loaded() {
                this.unknown = this.transform.Find("unkown").gameObject;
                this.known = this.transform.Find("known").gameObject;

                this.border = this.transform.Find("Image");

                this.toggle = this.transform.GetComponent<CP_Toggle>();
                this.toggle.onValueChanged.AddListener(this.Switch);

                this.remainImg1 = this.transform.Find("Image/ImageBG1").GetComponent<Image>();
                this.remainImg2 = this.transform.Find("Image/ImageBG2").GetComponent<Image>();
                this.remainText = this.transform.Find("Image/Text").GetComponent<Text>();
                this.icon = this.transform.Find("known/Image_boss").GetComponent<Image>();
            }

            public void Refresh(UI_CampChallenge ui, uint bossId) {
                this.ui = ui;
                this.bossId = bossId;
                this.listener = ui;

                CSVBOSSInformation.Data csvBoss = CSVBOSSInformation.Instance.GetConfData(bossId);
                if (csvBoss != null) {
                    this.isUnlocked = Sys_WorldBoss.Instance.unlockedBossManuales.TryGetValue(csvBoss.bossManual_id, out BossManualData bmd);
                    if (this.isUnlocked) {
                        this.known.SetActive(true);
                        this.unknown.SetActive(false);

                        ImageHelper.SetIcon(this.icon, bmd.csv.head_icon);
                    }
                    else {
                        this.known.SetActive(false);
                        this.unknown.SetActive(true);
                    }

                    // 位置
                    if (csvBoss.BOSS_position != null && csvBoss.BOSS_position.Count >= 2) {
                        Vector3 pos = new Vector3(csvBoss.BOSS_position[0], csvBoss.BOSS_position[1]);
                        this.rectTransform.anchoredPosition = pos;

                        int invertX = csvBoss.isFlip_X ? -1 : 1;
                        int invertY = csvBoss.isFlip_Y ? -1 : 1;
                        this.border.localScale = new Vector3(invertX, invertY, 1);

                        this.remainText.transform.localScale = new Vector3(1 / invertX, 1 / invertY, 1);
                        this.remainImg1.color = ui.csvCurrentActivity.themeColor;
                        this.remainImg2.color = ui.csvCurrentActivity.themeColor;
                    }
                }

#if UNITY_EDITOR
                this.gameObject.name = bossId.ToString();
#endif

                this.gameObject.gameObject.SetActive(false);
            }

            public void RefreshBossCount(int bossCount = 0) {
                this.gameObject.gameObject.SetActive(true);
                TextHelper.SetText(this.remainText, 4157000018u, bossCount.ToString());
            }

            public void Switch(bool arg) {
                if (arg) {
                    this.onSelected?.Invoke((int) this.bossId, true);
                    this.listener?.OnHeadClicked(this.bossId, this.isUnlocked, this.transform.position);
                }
            }

            public override void SetSelected(bool toSelected, bool force) {
                this.toggle.SetSelected(toSelected, true);
            }
        }

        public class Map : UIComponent {
            public Transform Area_Label;

            protected override void Loaded() {
                this.Area_Label = this.transform.GetChild(0).Find("Area_Label");
            }

            public void Refresh(uint mapId) {
                this.Area_Label.gameObject.SetActive(false);
            }
        }

        [SerializeField] public Button btnChallenge;
        public Button btnRank;
        public Button btnShop;
        public Text activityName;
        public Text activityTimes;
        public CP_LRArrowSwitch switcher;
        public CP_ToggleRegistry registry;
        public uint selectedBossId = 0;

        public GameObject tabProto;
        public Transform tabProtoParent;
        public UIElementCollector<Tab> tabVds = new UIElementCollector<Tab>();

        public GameObject headProto;
        public UIElementCollector<Head> headVds = new UIElementCollector<Head>();

        public List<Transform> maps = new List<Transform>();
        public Dictionary<uint, Map> idToMap = new Dictionary<uint, Map>();

        public CSVBOOSFightPlayMode.Data csvCurrentActivity;
        public uint currentActivityId;
        public List<uint> activityIds;

        public uint currentDailyId {
            get {
                uint id = 0;
                var csv = CSVBOOSFightPlayMode.Instance.GetConfData(this.currentActivityId);
                if (csv != null) {
                    id = csv.dailyActivites;
                }

                return id;
            }
        }

        public List<uint> challengeIds = new List<uint>();
        public uint currentChallengeId = 0;
        public CSVChallengeLevel.Data csvChallenge;
        public Map map;

        protected override void OnLoaded() {
            Button btn = this.transform.Find("Animator/View_Map/Map/Button_Camp").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnCampRuleClicked);

            this.campRedDot = this.transform.Find("Animator/View_Map/Map/Button_Camp/Image_Red");

            btn = this.transform.Find("Animator/View_Bottom/Button_Team").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnTeamClicked);

            btn = this.transform.Find("Animator/View_Title09/Btn_Close").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnExitClicked);

            btn = this.transform.Find("Animator/View_Map/Map/Button_Rule").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnChallengeRuleClicked);

            this.btnChallenge = this.transform.Find("Animator/View_Bottom/Button_Receive").GetComponent<Button>();
            this.btnChallenge.onClick.AddListener(this.OnBtnChallengeClicked);

            this.btnRank = this.transform.Find("Animator/View_Map/Map/Button_Rank").GetComponent<Button>();
            this.btnRank.onClick.AddListener(this.OnBtnRankClicked);
            
            this.btnShop = this.transform.Find("Animator/View_Map/Map/Button_Shop").GetComponent<Button>();
            this.btnShop.onClick.AddListener(this.OnBtnShopClicked);

            this.switcher = this.transform.Find("Animator/View_Right/Image_Title").GetComponent<CP_LRArrowSwitch>();
            this.switcher.onExec += this.OnSwitch;

            this.tabProto = this.transform.Find("Animator/View_Right/Rect/Rectlist/Proto").gameObject;
            this.tabProtoParent = this.tabProto.transform.parent;

            this.detailGo = this.transform.Find("Animator/View_Map/Map/Scroll View/Viewport/Content/Detail").gameObject;

            this.registry = this.transform.Find("Animator/View_Map/Map/Scroll View/Viewport/Content/Nodes").GetComponent<CP_ToggleRegistry>();
            this.headProto = this.transform.Find("Animator/View_Map/Map/Scroll View/Viewport/Content/Nodes/BossNode").gameObject;
            this.activityName = this.transform.Find("Animator/View_Right/Image_Title/Text").GetComponent<Text>();
            this.activityTimes = this.transform.Find("Animator/View_Map/Map/Text_Tips").GetComponent<Text>();

            Transform mapNode = this.transform.Find("Animator/View_Map/Map/Scroll View/Viewport/Content/Maps");
            this.idToMap.Clear();
            for (int i = 0, length = mapNode.childCount; i < length; ++i) {
                Map mapVd = new Map();
                Transform node = mapNode.GetChild(i);
                mapVd.Init(node);
                mapVd.Hide();
                if (uint.TryParse(node.name, out uint mapId)) {
                    this.idToMap.Add(mapId, mapVd);
                }
            }
        }

        protected override void OnOpen(object arg) {
            this.currentActivityId = Convert.ToUInt32(arg);
            this.activityIds = Sys_WorldBoss.Instance.GetSortedActivities(true, true);

            // 重置数据
            this.currentChallengeId = 0;
            this.selectedBossId = 0;
            this.csvChallenge = null;
        }

        protected override void OnShow() {
            this.switcher.SetData(this.activityIds);

            int index = this.activityIds.FindIndex((ele) => { return this.currentActivityId == ele; });

            this.switcher.SetCurrentIndex(index);
            this.switcher.Exec();
        }

        public void RefreshAll() {
            this.tabVds.BuildOrRefresh<uint>(this.tabProto, this.tabProtoParent, this.challengeIds, (vd, id, indexOfVdList) => {
                vd.SetUniqueId((int) id);
                vd.SetSelectedAction((innerId, force) => {
                    this.currentChallengeId = (uint) innerId;
                    this.csvChallenge = CSVChallengeLevel.Instance.GetConfData(this.currentChallengeId);

                    // 请求boss个数
                    Sys_WorldBoss.Instance.ReqBossCount(this.currentChallengeId, this.csvChallenge.BOSS_id);

                    // 清空选中Head
                    this.selectedBossId = 0;
                    this.registry.SetHighLight(-1);
                    this.detailContent?.Hide();

                    this.RefreshContent();
                    this.RefreshHeads();
                });
                vd.Refresh(this, id);
            });

            // 默认选中Tab
            if (this.tabVds.CorrectId(ref this.currentChallengeId, this.challengeIds)) {
                if (this.tabVds.TryGetVdById((int) this.currentChallengeId, out var vd)) {
                    vd.SetSelected(true, true);
                }
            }
            else {
                Debug.LogError("Cant run here!");
            }
        }

        public void RefreshContent() {
            // 刷新地图
            uint mapId = this.csvChallenge.island_id;
            this.map?.Hide();
            if (this.idToMap.TryGetValue(mapId, out this.map)) {
                this.map.Show();
                this.map.Refresh(mapId);
            }

            if (this.selectedBossId != 0) {
                ImageHelper.SetImageGray(this.btnChallenge, false, true);
            }
            else {
                ImageHelper.SetImageGray(this.btnChallenge, true, true);
            }

            bool canShow = this.csvCurrentActivity.playModeIsRanking && this.csvChallenge.challengeLevelIsRanking;
            this.btnRank.gameObject.SetActive(canShow);

            canShow = this.csvCurrentActivity.isShowShop;
            this.btnShop.gameObject.SetActive(canShow);

            long total = 0;
            long times = Sys_Daily.Instance.GetDailyTimes(this.currentDailyId);
            if (this.csvCurrentActivity != null) {
                if (this.csvCurrentActivity.rewardLimit != 0) {
                    total = this.csvCurrentActivity.rewardLimit;
                    TextHelper.SetText(this.activityTimes, 4157000019u, (total - times).ToString());
                }
                else if (this.csvCurrentActivity.rewardLimitDay != 0) {
                    total = this.csvCurrentActivity.rewardLimitDay;
                    TextHelper.SetText(this.activityTimes, 4157000020u, (total - times).ToString());
                }
            }

            this.campRedDot.gameObject.SetActive(Sys_WorldBoss.Instance.HasRewardUngot);
        }

        public void RefreshHeads() {
            // 地图头像
            this.headVds.BuildOrRefresh<uint>(this.headProto, this.headProto.transform.parent, this.csvChallenge.BOSS_id, (vd, id, indexOfVdList) => {
                vd.SetUniqueId((int) id);
                vd.Refresh(this, id);
            });
        }

        private void OnSwitch(int index, uint data) {
            this.currentActivityId = data;
            //this.activityIds = Sys_WorldBoss.Instance.GetUnlockedActivities(true);

            var csv = this.csvCurrentActivity = CSVBOOSFightPlayMode.Instance.GetConfData(this.currentActivityId);
            TextHelper.SetText(this.activityName, csv.playMode);

            this.challengeIds = csv.difficulty_id;
            int currentIndex;
            this.currentChallengeId = Sys_WorldBoss.Instance.GetMatchChallengeId(this.challengeIds, Sys_Role.Instance.Role.Level, out currentIndex);

            this.RefreshAll();
        }

        private void OnBtnExitClicked() {
            this.CloseSelf();
        }

        private void OnBtnChallengeRuleClicked() {
            UIManager.OpenUI(EUIID.UI_WorldBossChallengeRule, false, this.currentChallengeId);
        }

        private void OnBtnChallengeClicked() {
            var csv = CSVBOSSInformation.Instance.GetConfData(this.selectedBossId);
            if (csv != null) {
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(csv.targetNPC);
                this.CloseSelf();

                Sys_Team.Instance.DoTeamTarget(Sys_Team.DoTeamTargetType.SelectBoss, this.csvCurrentActivity.id);
            }
            else {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4157000021));
            }
        }

        private void OnBtnRankClicked() {
            UIManager.OpenUI(EUIID.UI_WorldBossRank, false, new Tuple<uint, uint>(this.currentActivityId, this.currentChallengeId));
        }

        private void OnBtnShopClicked() {
            var args = new MallPrama() {
                mallId = 1600,
                shopId = 16001,
            };
            UIManager.OpenUI(EUIID.UI_Mall, false, args);
        }

        private void OnBtnCampRuleClicked() {
            UIManager.OpenUI(EUIID.UI_WorldBossCampPreview, false);
        }

        private void OnBtnTeamClicked() {
            if (Sys_Team.Instance.IsFastOpen(true)) {
                Sys_Team.Instance.OpenFastUI(this.csvCurrentActivity.teamPlayModeId);
            }
        }

        public void OnHeadClicked(uint bossId, bool isUnlock, Vector3 anchor) {
            if (this.detailContent == null) {
                this.detailContent = new Detail();
                this.detailContent.Init(this.detailGo.transform);
            }

            this.detailContent.Refresh(bossId);
            this.detailContent.Show();
            this.detailContent.transform.position = anchor;

            this.selectedBossId = bossId;
            ImageHelper.SetImageGray(this.btnChallenge, false, true);
        }

        protected override void OnDestroy() {
            this.detailContent?.OnDestroy();
            this.tabVds.Clear();
            this.headVds.Clear();
        }

        #region 事件通知

        protected override void ProcessEvents(bool toRegister) {
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, this.OnPlayerLevelChanged, toRegister);
            Sys_WorldBoss.Instance.eventEmitter.Handle<CmdWildBossMapInfoRes>(Sys_WorldBoss.EEvents.OnBossCountGot, this.OnBossCountGot, toRegister);
        }

        private void OnPlayerLevelChanged() {
            if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                this.RefreshAll();
            }
        }

        private void OnBossCountGot(CmdWildBossMapInfoRes msg) {
            if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                if (msg.ChallengeId == this.currentChallengeId) {
                    for (int i = 0, length = msg.BossIds.Count; i < length; ++i) {
                        uint bossId = msg.BossIds[i];
                        uint bossCount = msg.BossCounts[i];
                        if (this.headVds.TryGetVdById((int) bossId, out Head vd)) {
                            vd.Show();
                            vd.RefreshBossCount((int) bossCount);
                        }
                    }
                }
            }
        }

        #endregion
    }
}