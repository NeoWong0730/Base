using System;
using System.Collections.Generic;
using Framework;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    // boss信息展示
    public class UI_BossManualInfo : UIBase {
        public Button btnExit;

        public GiftStatus unlockGift; // 解锁礼包
        public GiftStatus firstKillGift; // 首杀礼包

        public Text skillDesc;
        public Text simpleDesc;

        // 模型挂载
        public RawImage rawImage;

        public Text nameText;
        public Text ageText;
        public Text characterText;
        public Text loveText;
        public Text weakText;

        // 传记开启/未开启
        public CP_TransformContainer transContainer;
        public GiftStatus storyGift; // 传记礼包
        public Text storyDesc;
        public ScrollRect storyRect;

        public class Tab : UISelectableElement {
            public CP_Toggle toggle;
            public Text nameText;
            public Text checkNameText;
            public GameObject redDot;

            protected override void Loaded() {
                this.toggle = this.transform.GetComponent<CP_Toggle>();
                this.nameText = this.transform.Find("Label").GetComponent<Text>();
                this.redDot = this.transform.Find("Image_Red").gameObject;
                this.checkNameText = this.transform.Find("Checkmark/Label").GetComponent<Text>();
                this.toggle.onValueChanged.AddListener(this.Switch);
            }

            public void Refresh(BossManualData bossInfo, int index) {
                bool hasReward = bossInfo.HasCardRewardUngot(index);
                redDot.SetActive(hasReward);
                if (index == 0) {
                    TextHelper.SetText(this.nameText, 4157000005);
                    TextHelper.SetText(this.checkNameText, 4157000005);
                }
                else if (index == 1) {
                    TextHelper.SetText(this.nameText, 4157000006);
                    TextHelper.SetText(this.checkNameText, 4157000006);
                }
                else if (index == 2) {
                    TextHelper.SetText(this.nameText, 4157000007);
                    TextHelper.SetText(this.checkNameText, 4157000007);
                }
            }

            public void Switch(bool arg) {
                if (arg) {
                    this.onSelected?.Invoke(this.id, true);
                }
            }

            public override void SetSelected(bool toSelected, bool force) {
                this.toggle.SetSelected(toSelected, true);
            }
        }

        public GameObject tabProto;
        public UIElementCollector<Tab> tabVds = new UIElementCollector<Tab>();

        public AssetDependencies assetDependencies;
        public RawImage rawImageLeft;
        public ShowSceneControl showSceneControl;
        public DialogueShowActor dialogueActor = new DialogueShowActor();
        public uint lastBossManualId = 0;

        public uint manualId;
        public uint bossId;
        public BossManualData bossInfo;

        public int currentTabId = 0;
        public List<uint> storyIds = new List<uint>();

        protected override void OnLoaded() {
            this.btnExit = this.transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            this.btnExit.onClick.AddListener(this.OnBtnExitClicked);

            this.skillDesc = this.transform.Find("Animator/View_Camp/Intro/Content/Content0/Text_Des").GetComponent<Text>();
            this.simpleDesc = this.transform.Find("Animator/View_Camp/Intro/Content/Content1/Text_Des").GetComponent<Text>();

            this.assetDependencies = this.transform.GetComponent<AssetDependencies>();
            this.rawImageLeft = this.transform.Find("Animator/View_Camp/Boss/RawImage").GetComponent<RawImage>();

            this.nameText = this.transform.Find("Animator/View_Camp/Boss/Image_name/Text").GetComponent<Text>();
            this.ageText = this.transform.Find("Animator/View_Camp/Boss/Rect/Rectlist/Image_BG0/Text_Detail").GetComponent<Text>();
            this.characterText = this.transform.Find("Animator/View_Camp/Boss/Rect/Rectlist/Image_BG1/Text_Detail").GetComponent<Text>();
            this.loveText = this.transform.Find("Animator/View_Camp/Boss/Rect/Rectlist/Image_BG2/Text_Detail").GetComponent<Text>();
            this.weakText = this.transform.Find("Animator/View_Camp/Boss/Rect/Rectlist/Image_BG3/Text_Detail").GetComponent<Text>();

            this.transContainer = this.transform.Find("Animator/View_Camp/Story/Image_bg").GetComponent<CP_TransformContainer>();
            this.storyGift = this.transform.Find("Animator/View_Camp/Story/Image_bg/Image_gifBg/GiftStatus").GetComponent<GiftStatus>();
            this.storyRect = this.transform.Find("Animator/View_Camp/Story/Image_bg/Image_TextMask").GetComponent<ScrollRect>();
            this.storyDesc = this.transform.Find("Animator/View_Camp/Story/Image_bg/Image_TextMask/Text_Story").GetComponent<Text>();

            this.unlockGift = this.transform.Find("Animator/View_Camp/Bottom/Image_gifBg0/GiftStatus").GetComponent<GiftStatus>();
            unlockGift.SetAction((status) => {
                if (status == EGiftStatus.UnGot) {
                    Sys_WorldBoss.Instance.ReqReward(manualId, 1, 0);
                }
                else{
                    var csvManual = CSVBOSSManual.Instance.GetConfData(manualId);
                    if (csvManual != null){
                        var tp = new Tuple<List<ItemIdCount>, bool, Button>(CSVDrop.Instance.GetDropItem(csvManual.BOSSUnlocked_drop), status == EGiftStatus.Got, unlockGift.btn);
                        UIManager.OpenUI(EUIID.UI_WorldBossRewardList, false, tp);
                    }
                }
            });
            this.firstKillGift = this.transform.Find("Animator/View_Camp/Bottom/Image_gifBg1/GiftStatus").GetComponent<GiftStatus>();
            firstKillGift.SetAction((status) => {
                if (status == EGiftStatus.UnGot) {
                    Sys_WorldBoss.Instance.ReqReward(manualId, 2, 0);
                }
                else{
                    var csvManual = CSVBOSSManual.Instance.GetConfData(manualId);
                    if (csvManual != null){
                        var tp = new Tuple<List<ItemIdCount>, bool,Button>(CSVDrop.Instance.GetDropItem(csvManual.BOSSFirstKilled_drop), status == EGiftStatus.Got, firstKillGift.btn);
                        UIManager.OpenUI(EUIID.UI_WorldBossRewardList, false, tp);
                    }
                }
            });

            this.tabProto = this.transform.Find("Animator/View_Camp/Story/Image_bg/Toggles/Toggle").gameObject;
        }

        protected override void OnOpen(object arg) {
            var tp = arg as Tuple<uint, uint>;
            if (tp != null) {
                this.bossId = tp.Item1;
                this.manualId = tp.Item2;

                Sys_WorldBoss.Instance.unlockedBossManuales.TryGetValue(this.manualId, out this.bossInfo);

                // 重置数据
                this.lastBossManualId = 0;
                this.currentTabId = 0;
                this.storyIds.Clear();
                for (int i = 0, length = this.bossInfo.csv.biography.Count; i < length; ++i) {
                    this.storyIds.Add((uint) i + 1);
                }
            }
        }

        protected override void OnClose() {
            this.UnloadModel();
        }

        protected override void OnShow() {
            this.RefreshAll();
        }

        public void RefreshTabContent() {
            bool isUnlocked = this.bossInfo.storyLevel >= this.currentTabId;
            this.transContainer.ShowHideBySetActive(isUnlocked);
            if (isUnlocked) {
                TextHelper.SetText(this.storyDesc, this.bossInfo.csv.biography[this.currentTabId - 1]);
                this.storyGift.Refresh(this.bossInfo.storyRewardStatus[this.currentTabId - 1]);
                this.storyGift.SetAction(this.OnRewardClicked);
            }
        }

        private void OnRewardClicked(EGiftStatus status) {
            int id = this.currentTabId;
            if (status == EGiftStatus.UnGot) {
                Sys_WorldBoss.Instance.ReqReward(this.manualId, 3, id);
            }
            else{
                var csvManual = CSVBOSSManual.Instance.GetConfData(manualId);
                if (csvManual != null)
                {
                    var tp = new Tuple<List<ItemIdCount>, bool, Button>(CSVDrop.Instance.GetDropItem(csvManual.biography_drop[id - 1]), status == EGiftStatus.Got, this.storyGift.btn);
                    UIManager.OpenUI(EUIID.UI_WorldBossRewardList, false, tp);
                }
            }
        }


        public void LoadModel() {
            if (this.lastBossManualId == this.manualId) {
                return;
            }

            this.lastBossManualId = this.manualId;

            this.UnloadModel();

            if (this.showSceneControl == null) {
                this.showSceneControl = new ShowSceneControl();

                GameObject sceneModel = GameObject.Instantiate<GameObject>(this.assetDependencies.mCustomDependencies[0] as GameObject);
                sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
                this.showSceneControl.Parse(sceneModel);
                this.rawImageLeft.texture = this.showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
            }

            string modelPath = null;
            this.dialogueActor.displayControl = DisplayControl<EHeroModelParts>.Create((int) EHeroModelParts.Count);
            this.dialogueActor.displayControl.eLayerMask = ELayerMask.ModelShow;
            this.dialogueActor.CharID = this.bossId;
            var csvManual = CSVBOSSManual.Instance.GetConfData(this.manualId);
            if (csvManual != null) {
                this.dialogueActor.LeftOffset = new Vector3(1f * csvManual.positionx / 10000f, 1f * csvManual.positiony / 10000f, 1f * csvManual.positionz / 10000f);
                this.dialogueActor.LeftRotOffset = new Vector3(1f * csvManual.rotationx / 10000f, 1f * csvManual.rotationy / 10000f, 1f * csvManual.rotationz / 10000f);
                this.dialogueActor.LeftScaleOffset = new Vector3(1f * csvManual.scale / 10000f, 1f * csvManual.scale / 10000f, 1f * csvManual.scale / 10000f);
            }

            var csvNpcData = CSVNpc.Instance.GetConfData(this.bossId);
            if (csvNpcData != null) {
                this.dialogueActor.ActionID = csvNpcData.action_id;
                modelPath = csvNpcData.model;
            }
            else {
                DebugUtil.LogError($"Can not find npcID : {this.manualId}");
            }

            this.dialogueActor.WeaponID = Constants.UMARMEDID;

            this.dialogueActor.IsLeft = true;
            this.dialogueActor.onLoadedPlus = this.OnLoadFinished;

            this.dialogueActor.displayControl.onLoaded += this.dialogueActor.OnLoadedCallBack;
            this.dialogueActor.displayControl.LoadMainModel(EHeroModelParts.Main, modelPath, EHeroModelParts.None, null);

            if (this.showSceneControl != null && this.showSceneControl.mModelPos != null) {
                this.dialogueActor.displayControl.GetPart(EHeroModelParts.Main).SetParent(this.showSceneControl.mModelPos, null);
            }
            else {
                this.dialogueActor.displayControl.GetPart(EHeroModelParts.Main).Dispose();
            }
        }

        private void OnLoadFinished(onLoadedPlusEvt evt) {
            if (evt.index == 0) {
                GameObject go = this.dialogueActor.GetGameObject(0);
                go.SetActive(false);

                if (this.dialogueActor.heroLoader != null) {
                    this.dialogueActor.heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(evt.actionID, evt.weaponID, null, EStateType.Idle, go);
                }
                else if (this.dialogueActor.displayControl != null) {
                    this.dialogueActor.displayControl.mAnimation.UpdateHoldingAnimations(evt.actionID, evt.weaponID, null, EStateType.Idle, go);
                }

                this.dialogueActor.SetTransformOffset(true);
            }
        }

        public void UnloadModel() {
            //this.Layout.rawImageLeft.texture = null;
            this.rawImageLeft.color = new Color(1f, 1f, 1f, 1f);

            this.dialogueActor?.Dispose();
            this.showSceneControl?.Dispose();

            this.showSceneControl = null;
        }

        public void RefreshAll() {
            this.tabVds.BuildOrRefresh<uint>(this.tabProto, this.tabProto.transform.parent, this.storyIds, (vd, id, indexOfVdList) => {
                vd.SetUniqueId((int) id);
                vd.SetSelectedAction((innerId, force) => {
                    this.currentTabId = innerId;
                    this.storyRect.verticalNormalizedPosition = 1f;
                    
                    this.RefreshTabContent();
                });
                vd.Refresh(this.bossInfo, indexOfVdList);
            });

            // 默认选中Tab
            if (this.tabVds.CorrectId(ref this.currentTabId, this.storyIds)) {
                if (this.tabVds.TryGetVdById(this.currentTabId, out var vd)) {
                    vd.SetSelected(true, true);
                }
            }

            this.unlockGift.Refresh(this.bossInfo.unlockRewardStatus);
            this.firstKillGift.Refresh(this.bossInfo.firstSkillRewardStatus);

            TextHelper.SetText(this.skillDesc, this.bossInfo.csv.detailPage_skill);
            TextHelper.SetText(this.simpleDesc, this.bossInfo.csv.detailPage_introduction);

            TextHelper.SetText(this.nameText, this.bossInfo.csv.detailPage_name);
            TextHelper.SetText(this.ageText, this.bossInfo.csv.detailPage_age);
            TextHelper.SetText(this.characterText, this.bossInfo.csv.detailPage_character);
            TextHelper.SetText(this.loveText, this.bossInfo.csv.detailPage_interests);
            TextHelper.SetText(this.weakText, this.bossInfo.csv.detailPage_weakness);

            this.LoadModel();
            Lib.Core.FrameworkTool.ForceRebuildLayout(this.gameObject);
        }

        private void OnBtnExitClicked() {
            this.CloseSelf();
        }

        #region 事件通知

        protected override void ProcessEvents(bool toRegister) {
            Sys_WorldBoss.Instance.eventEmitter.Handle(Sys_WorldBoss.EEvents.OnRewardGot, OnRewardGot, toRegister);
        }

        private void OnRewardGot() {
            if (gameObject.activeInHierarchy) {
                this.RefreshAll();
            }
        }

        #endregion
    }
}