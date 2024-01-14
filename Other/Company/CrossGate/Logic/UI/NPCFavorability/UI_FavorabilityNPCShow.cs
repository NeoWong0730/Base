using System;
using System.Collections.Generic;
using Framework;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_FavorabilityNPCShow_Layout : UILayoutBase {
        public Button btnExit;
        public Button btnCharacterDetail;
        public Button btnVisit;
        public Button btnClue;
        public Button btnLikeDesc;
        public Button btnLikeFood;

        public CP_LRArrowSwitch arrowSwitcher;

        public Button btnFavorablityRewardPreview;

        public Image npcIcon;
        public Image npcType;
        public Text npcCharacter; // 性格描述
        public Text npcName;
        public Text zoneName;
        public Text npcAge;

        public Slider npcMode;
        public Image npcModeIcon;
        public Text npcModeText;
        public Button npcModeBtn;

        public Slider npcHealth;
        public Image npcHealthIcon;
        public Text npcHealthText;

        public Slider npcFavorability;
        public Text npcFavorabilityText;

        public Transform modelParent;

        public Text stageName;

        public Transform likeProto;
        public AssetDependencies assetDependencies;
        public RawImage rawImageLeft;

        public void Parse(GameObject root) {
            this.Init(root);

            this.assetDependencies = this.transform.GetComponent<AssetDependencies>();
            this.rawImageLeft = this.transform.Find("Animator/View_Left/Charapter").GetComponent<RawImage>();

            this.btnExit = this.transform.Find("Animator/View_Title10/Btn_Close").GetComponent<Button>();
            this.btnVisit = this.transform.Find("Animator/View_Right/View_Visit/Button_All").GetComponent<Button>();
            this.btnCharacterDetail = this.transform.Find("Animator/View_Right/Character/Button").GetComponent<Button>();
            this.arrowSwitcher = this.transform.Find("Animator/Arrows").GetComponent<CP_LRArrowSwitch>();
            this.btnFavorablityRewardPreview = this.transform.Find("Animator/View_Right/View_Visit/Image/Image_Get").GetComponent<Button>();
            this.btnClue = this.transform.Find("Animator/View_Right/Image_Health/Button").GetComponent<Button>();
            this.npcModeBtn = this.transform.Find("Animator/View_Right/Image_Mood/Button").GetComponent<Button>();
            this.npcIcon = this.transform.Find("Animator/View_Left/Image_Icon").GetComponent<Image>();
            this.npcType = this.transform.Find("Animator/View_Left/Image_Type").GetComponent<Image>();
            this.npcCharacter = this.transform.Find("Animator/View_Left/Text_Likability").GetComponent<Text>();
            this.npcName = this.transform.Find("Animator/View_Right/Image_Title/Text_Name").GetComponent<Text>();
            this.zoneName = this.transform.Find("Animator/View_Right/Image_Location/Text").GetComponent<Text>();
            this.npcAge = this.transform.Find("Animator/View_Right/Image_Age/Text").GetComponent<Text>();

            this.stageName = this.transform.Find("Animator/View_Right/View_Visit/Text").GetComponent<Text>();

            this.npcMode = this.transform.Find("Animator/View_Right/Image_Mood/Slider_Eg").GetComponent<Slider>();
            this.npcModeIcon = this.transform.Find("Animator/View_Right/Image_Mood/Image_Mood").GetComponent<Image>();
            this.npcModeText = this.transform.Find("Animator/View_Right/Image_Mood/Text_Percent").GetComponent<Text>();

            this.npcHealth = this.transform.Find("Animator/View_Right/Image_Health/Slider_Eg").GetComponent<Slider>();
            this.npcHealthIcon = this.transform.Find("Animator/View_Right/Image_Health/Image_Health").GetComponent<Image>();
            this.npcHealthText = this.transform.Find("Animator/View_Right/Image_Health/Text_Percent").GetComponent<Text>();

            this.npcFavorability = this.transform.Find("Animator/View_Right/View_Visit/Slider_Eg").GetComponent<Slider>();
            this.npcFavorabilityText = this.transform.Find("Animator/View_Right/View_Visit/Text_Percent").GetComponent<Text>();

            this.modelParent = this.transform.Find("Animator/View_Left");

            this.likeProto = this.transform.Find("Animator/View_Right/View_Like/Scroll_View/Viewport/Item");
            
            this.btnLikeDesc = this.transform.Find("Animator/View_Right/View_Like/Button").GetComponent<Button>();
            this.btnLikeFood = this.transform.Find("Animator/View_Right/Food/Button").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener) {
            this.arrowSwitcher.onExec += listener.OnArrowSwicth;

            this.btnClue.onClick.AddListener(listener.OnBtnClueClicked);
            this.npcModeBtn.onClick.AddListener(listener.OnBtnMoodClicked);
            this.btnExit.onClick.AddListener(listener.OnBtnExitClicked);
            this.btnVisit.onClick.AddListener(listener.OnBtnVisitClicked);
            this.btnLikeDesc.onClick.AddListener(listener.OnBtnLikeDescClicked);
            this.btnLikeFood.onClick.AddListener(listener.OnBtnLikeFoodClicked);
            this.btnCharacterDetail.onClick.AddListener(listener.OnBtnCharacterDetailClicked);
            this.btnFavorablityRewardPreview.onClick.AddListener(listener.OnBtnFavorablityRewardPreviewClicked);
        }

        public interface IListener {
            void OnArrowSwicth(int index, uint id);
            void OnBtnExitClicked();
            void OnBtnClueClicked();
            void OnBtnMoodClicked();
            void OnBtnVisitClicked();
            void OnBtnLikeDescClicked();
            void OnBtnLikeFoodClicked();
            void OnBtnCharacterDetailClicked();
            void OnBtnFavorablityRewardPreviewClicked();
        }
    }

    public class UI_FavorabilityNPCShow : UIBase, UI_FavorabilityNPCShow_Layout.IListener {
        public class Gift : UIComponent {
            public new Text name;
            public Image icon;
            public GameObject unlockGo;

            protected override void Loaded() {
                this.name = this.transform.Find("Text_Name").GetComponent<Text>();
                this.icon = this.transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                this.unlockGo = this.transform.Find("Image_Unlock").gameObject;
            }
            public void Refresh(uint npcId, uint giftType) {
                if (Sys_NPCFavorability.Instance.TryGetNpc(npcId, out FavorabilityNPC npc)) {
                    bool unlocked = npc.IsGiftTypeUnlock(giftType);
                    this.unlockGo.SetActive(!unlocked);

                    var csvGift = CSVFavorabilityGiftType.Instance.GetConfData(giftType);
                    if (csvGift != null) {
                        ImageHelper.SetIcon(this.icon, csvGift.icon);
                    }

                    if (unlocked) {
                        this.icon.enabled = true;
                        //this.name.enabled = true;
                        this.unlockGo.SetActive(false);
                        this.icon.gameObject.SetActive(true);

                        //ImageHelper.SetImageGray(this.name, false, true);
                        TextHelper.SetText(this.name, csvGift.name);
                    }
                    else {
                        this.icon.enabled = false;
                        //this.name.enabled = false;
                        this.unlockGo.SetActive(true);
                        this.icon.gameObject.SetActive(false);

                        TextHelper.SetText(this.name, 2010608);
                        //ImageHelper.SetImageGray(this.name, true, true);
                    }
                }
            }
        }
        public UI_FavorabilityNPCShow_Layout Layout = new UI_FavorabilityNPCShow_Layout();
        public COWVd<UI_FavorabilityNPCShow.Gift> gifts = new COWVd<UI_FavorabilityNPCShow.Gift>();

        public List<uint> npcIds = new List<uint>();
        public uint zoneId;
        public uint currentNpcId;
        public int currentNpcIndex;
        public FavorabilityNPC npc;

        public uint lastNpcId;

        public ShowSceneControl showSceneControl;
        public DialogueShowActor dialogueActor = new DialogueShowActor();

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg) {
            this.currentNpcId = Convert.ToUInt32(arg);
            if (Sys_NPCFavorability.Instance.TryGetNpc(this.currentNpcId, out var npc, false)) {
                this.zoneId = npc.zoneId;
            }
        }
        protected override void OnShow() {
            this.npcIds.Clear();
            ZoneFavorabilityNPC.GetIds(this.zoneId, true, this.npcIds);
            this.Layout.arrowSwitcher.SetData(this.npcIds);

            int index = this.npcIds.FindIndex((ele) => {
                return this.currentNpcId == ele;
            });

            this.Layout.arrowSwitcher.SetCurrentIndex(index);
            this.Layout.arrowSwitcher.Exec();
        }
        protected override void OnHide() {
            this.UnloadAll();
            //UIManager.CloseUI(EUIID.UI_FavorabilityDialogue);
        }

        public void UnloadLastModel() {
            this.dialogueActor?.Dispose();
        }
        public void UnloadAll() {
            //this.Layout.rawImageLeft.texture = null;
            this.Layout.rawImageLeft.color = new Color(1f, 1f, 1f, 1f);

            this.dialogueActor?.Dispose();
            this.showSceneControl?.Dispose();

            this.showSceneControl = null;
            this.lastNpcId = 0;
        }
        public void RefreshModel() {
            this.UnloadLastModel();

            if (this.showSceneControl == null) {
                this.showSceneControl = new ShowSceneControl();

                GameObject sceneModel = GameObject.Instantiate<GameObject>(this.Layout.assetDependencies.mCustomDependencies[0] as GameObject);
                sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
                this.showSceneControl.Parse(sceneModel);

                //this.Layout.rawImageLeft.texture = this.showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
            }

            string modelPath = null;
            this.dialogueActor.displayControl = DisplayControl<EHeroModelParts>.Create((int)EHeroModelParts.Count);
            this.dialogueActor.displayControl.eLayerMask = ELayerMask.ModelShow;
            this.dialogueActor.CharID = this.npc.id;
            var csvFavorability = CSVNPCFavorability.Instance.GetConfData(this.npc.id);
            if (csvFavorability != null) {
                this.dialogueActor.LeftOffset = new Vector3(1f * csvFavorability.positionx / 10000f, 1f * csvFavorability.positiony / 10000f, 1f * csvFavorability.positionz / 10000f);
                this.dialogueActor.LeftRotOffset = new Vector3(1f * csvFavorability.rotationx / 10000f, 1f * csvFavorability.rotationy / 10000f, 1f * csvFavorability.rotationz / 10000f);
                this.dialogueActor.LeftScaleOffset = new Vector3(1f * csvFavorability.scale / 10000f, 1f * csvFavorability.scale / 10000f, 1f * csvFavorability.scale / 10000f);
            }

            var csvNpcData = CSVNpc.Instance.GetConfData(this.npc.id);
            if (csvNpcData != null) {
                this.dialogueActor.ActionID = csvNpcData.action_show_id;
                modelPath = csvNpcData.model_show;
            }
            else {
                DebugUtil.LogError($"Can not find npcID : {this.npc.id}");
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
                if (go != null)
                    go.SetActive(false);

                if (this.dialogueActor.heroLoader != null) {
                    this.dialogueActor.heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(evt.actionID, evt.weaponID, null, EStateType.Idle, go);
                }
                else if (this.dialogueActor.displayControl != null) {
                    this.dialogueActor.displayControl.mAnimation.UpdateHoldingAnimations(evt.actionID, evt.weaponID, null, EStateType.Idle, go);
                }

                this.dialogueActor.SetTransformOffset(evt.isLeft);
            }
        }
        public void Refresh(uint id, int index) {
            this.currentNpcId = id;
            this.currentNpcIndex = index;

            bool exist = Sys_NPCFavorability.Instance.TryGetNpc(id, out this.npc, false);
            uint place = this.npc.csvNPCFavorability.Place;
            CSVFavorabilityPlaceReward.Data csvPlace = CSVFavorabilityPlaceReward.Instance.GetConfData(place);
            if (csvPlace != null) {
                TextHelper.SetText(this.Layout.zoneName, csvPlace.PlaceName);
            }
            this.Layout.npcAge.text = this.npc.csvNPCFavorability.Age.ToString();
            CSVNPCOccupation.Data csvNPCOccupation = CSVNPCOccupation.Instance.GetConfData(this.npc.csvNPCFavorability.Occupation);
            if (csvNPCOccupation != null) {
                ImageHelper.SetIcon(this.Layout.npcIcon, csvNPCOccupation.icon);
                ImageHelper.SetIcon(this.Layout.npcType, csvNPCOccupation.nameicon);
            }
            CSVNPCCharacter.Data csvCharacter = CSVNPCCharacter.Instance.GetConfData(this.npc.csvNPCFavorability.Character);
            if (csvCharacter != null) {
                TextHelper.SetText(this.Layout.npcCharacter, csvCharacter.Name);
            }
            CSVNpc.Data csvNPC = CSVNpc.Instance.GetConfData(this.npc.id);
            if (csvNPC != null) {
                TextHelper.SetText(this.Layout.npcName, LanguageHelper.GetNpcTextContent(csvNPC.name));
            }
            CSVNPCMood.Data csvNPCMood = CSVNPCMood.Instance.GetConfData(this.npc.moodId);
            if (csvNPCMood != null) {
                ImageHelper.SetIcon(this.Layout.npcModeIcon, csvNPCMood.Icon);
                TextHelper.SetText(this.Layout.npcModeText, string.Format("{0}/{1}", this.npc.moodValue.ToString(), 100));
                this.Layout.npcMode.value = 1f * this.npc.moodValue / 100;
            }
            CSVNPCDisease.Data csvDisease = CSVNPCDisease.Instance.GetConfData(this.npc.healthId);
            if (csvDisease != null) {
                ImageHelper.SetIcon(this.Layout.npcHealthIcon, csvDisease.Icon);
                TextHelper.SetText(this.Layout.npcHealthText, string.Format("{0}/{1}", this.npc.healthValue.ToString(), 100));
                this.Layout.npcHealth.value = 1f * this.npc.healthValue / 100;
            }
            else {
                // 2是健康
                ImageHelper.SetIcon(this.Layout.npcHealthIcon, CSVNPCHealth.Instance.GetConfData(2).Icon);
                TextHelper.SetText(this.Layout.npcHealthText, string.Format("{0}/{1}", this.npc.healthValue.ToString(), 100));
                this.Layout.npcHealth.value = 1f * this.npc.healthValue / 100;
            }
            CSVFavorabilityStageName.Data csvStageName = CSVFavorabilityStageName.Instance.GetConfData(this.npc.csvNPCFavorabilityStage.Stage);
            if (csvStageName != null) {
                TextHelper.SetText(this.Layout.stageName, csvStageName.name);
            }

            if (this.currentNpcId != this.lastNpcId) {
                this.RefreshModel();
                this.lastNpcId = this.currentNpcId;
            }

            this.gifts.TryBuildOrRefresh(this.Layout.likeProto.gameObject, this.Layout.likeProto.parent, this.npc.likesGiftTypes.Count, (vd, vdIndex) => {
                vd.Refresh(this.currentNpcId, this.npc.likesGiftTypes[vdIndex]);
            });

            bool isNPCReachedMax = this.npc.IsNPCReachedMax;
            // this.Layout.btnVisit.gameObject.SetActive(!isNPCReachedMax);

            this.Layout.npcFavorability.value = this.npc.favorability * 1f / this.npc.csvNPCFavorabilityStage.FavorabilityValueMax;
            if (!isNPCReachedMax) {
                TextHelper.SetText(this.Layout.npcFavorabilityText, string.Format("{0}/{1}", this.npc.favorability.ToString(), this.npc.csvNPCFavorabilityStage.FavorabilityValueMax.ToString()));
            }
            else {
                TextHelper.SetText(this.Layout.npcFavorabilityText, "Max");
            }
        }

        public void OnBtnCharacterDetailClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityNPCCharacterDesc, false, this.npc.id);
        }
        public void OnBtnFavorablityRewardPreviewClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityStageRewardPreview, false, new Tuple<float, float, uint>(965f, -286f, this.npc.id));
        }
        public void OnBtnVisitClicked() {
            // if (this.npc.IsNPCReachedMax) {
            //     Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010612));
            // }
            // else
            {
                UIManager.ClearUntilMain();
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(this.npc.id);
            }
        }

        public void OnBtnLikeDescClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityNpcLikeDesc, false, this.currentNpcId);
        }

        public void OnBtnLikeFoodClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityNpcLikeFood, false, this.currentNpcId);
        }

        public void OnBtnExitClicked() {
            this.lastNpcId = 0;

            this.CloseSelf();
        }
        public void OnArrowSwicth(int index, uint id) {
            this.Refresh(id, index);
        }
        public void OnBtnClueClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityHealth, false, this.npc.id);
        }

        private readonly UIRuleParam par = new UIRuleParam {
            StrContent = LanguageHelper.GetTextContent(2010646),
        };
        public void OnBtnMoodClicked() {
            CSVNPCMood.Data csvNPCMood = CSVNPCMood.Instance.GetConfData(this.npc.moodId);
            if (csvNPCMood != null) {
                string npcName = LanguageHelper.GetNpcTextContent(this.npc.csvNPC.name);
                string content = LanguageHelper.GetTextContent(csvNPCMood.Des, npcName, (csvNPCMood.FavorabilityRatio / 100).ToString());
                Vector3 pos = new Vector3(-168, 3, 0);
                UIManager.OpenUI(EUIID.UI_FavorabilityMoodDesc, false, new Tuple<string, Vector3>(content, pos));
            }
        }

        protected override void ProcessEvents(bool toRegister) {
            Sys_NPCFavorability.Instance.eventEmitter.Handle(Sys_NPCFavorability.EEvents.OnPlayerFavorabilityChanged, this.OnPlayerFavorabilityChanged, toRegister);
            Sys_NPCFavorability.Instance.eventEmitter.Handle<uint, uint>(Sys_NPCFavorability.EEvents.OnGiftUnlock, this.OnGiftUnlock, toRegister);
            Sys_NPCFavorability.Instance.eventEmitter.Handle<uint, uint, uint, uint>(Sys_NPCFavorability.EEvents.OnNpcFavorabilityChanged, this.OnNpcFavorabilityChanged, toRegister);
        }
        private void OnPlayerFavorabilityChanged() {
            if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                this.Refresh(this.currentNpcId, this.currentNpcIndex);
            }
        }
        private void OnGiftUnlock(uint npcId, uint giftType) {
            if (this.gameObject != null && this.gameObject.activeInHierarchy && npcId == this.currentNpcId) {
                this.Refresh(npcId, this.currentNpcIndex);
            }
        }
        private void OnNpcFavorabilityChanged(uint npcId, uint stageId, uint from, uint to) {
            if (this.gameObject != null && this.gameObject.activeInHierarchy && npcId == this.currentNpcId) {
                this.Refresh(npcId, this.currentNpcIndex);
            }
        }
    }
}