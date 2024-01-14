using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_FavorabilityMenu_Layout : UILayoutBase {
        public Button btnExit;
        public GameObject title;
        public Text playerFavorability;

        public Text stageText;
        public Image modeImage;

        public Button btnPlayerFavorabilityInfo;

        public Text npcFavorability;
        public Slider npcSliderFavorability;
        public Button btnNpcFavorability;
        
        public Text modeValue;
        public Slider modeSlider;
        public Button btnNpcModeDesc;

        public Text sendGiftTotalTimes;

        public void Parse(GameObject root) {
            this.Init(root);

            this.title = this.transform.Find("Animator/Title").gameObject;
            this.btnExit = this.transform.Find("Animator/Title/View_Title10/Btn_Close").GetComponent<Button>();
            this.playerFavorability = this.transform.Find("Animator/Title/Image_Number/Text_Number").GetComponent<Text>();
            this.stageText = this.transform.Find("Animator/View_Favorability/Text").GetComponent<Text>();

            this.npcFavorability = this.transform.Find("Animator/View_Favorability/Text_Percent").GetComponent<Text>();
            this.npcSliderFavorability = this.transform.Find("Animator/View_Favorability/Slider_Eg").GetComponent<Slider>();
            this.btnNpcFavorability = this.transform.Find("Animator/View_Favorability/Image/Image_Get").GetComponent<Button>();

            this.btnPlayerFavorabilityInfo = this.transform.Find("Animator/Title/Button_Tips").GetComponent<Button>();
            
            this.modeValue = this.transform.Find("Animator/View_Mood/Text_Percent").GetComponent<Text>();
            this.modeSlider = this.transform.Find("Animator/View_Mood/Slider_Eg").GetComponent<Slider>();
            this.modeImage = this.transform.Find("Animator/View_Mood/Image/Image_Mood").GetComponent<Image>();

            this.btnNpcModeDesc = this.transform.Find("Animator/View_Mood/Button").GetComponent<Button>();
            this.sendGiftTotalTimes = this.transform.Find("Animator/Title/Text_All").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener) {
            this.btnExit.onClick.AddListener(listener.OnBtnExitClicked);
            this.btnNpcFavorability.onClick.AddListener(listener.OnBtnNpcFavorabilityClicked);
            this.btnNpcModeDesc.onClick.AddListener(listener.OnBtnModeDescClicked);
            this.btnPlayerFavorabilityInfo.onClick.AddListener(listener.OnBtnPlayerFavorabilityInfoClicked);
        }

        public interface IListener {
            void OnBtnExitClicked();
            void OnBtnNpcFavorabilityClicked();
            void OnBtnModeDescClicked();
            void OnBtnPlayerFavorabilityInfoClicked();
        }
    }

    // 基类
    public class UIFavorabilityBase : UIComponent {
        public FavorabilityNPC npc;
        public UI_FavorabilityMenu ui;

        public virtual void Refresh(UI_FavorabilityMenu ui, FavorabilityNPC npc) {
            this.npc = npc;
            this.ui = ui;

            this.OnRefresh();
        }

        protected new virtual void OnRefresh() {

        }
    }

    public enum EFavorabilityLetterRewardType {
        Drop = 1,  // 掉落
        Partner = 2,  // 伙伴
        Mall = 3,  // 商城
        // ...  后续拓展
    }

    // normal菜单
    public class UIFavorabilityNormalMenu : UIComponent {
        public FavorabilityNPC npc;
        public UI_FavorabilityMenu ui;

        public Button btnGift;
        public Text btnGiftText;
        public Text btnGiftRemainText;
        public EReason reasonGift;

        public Button btnTalk;
        public Text btnTalkText;
        public Text btnTalkRemainText;
        public EReason reasonTalk;

        public Button btnFete;
        public Text btnFeteText;
        public Text btnFeteRemainText;
        public EReason reasonFete;

        public Button btnMusic;
        public Text btnMusicText;
        public Text btnMusicRemainText;
        public EReason reasonMusic;

        public Button btnDance;
        public Text btnDanceText;
        public Text btnDanceRemainText;
        public EReason reasonDance;

        public enum EReason {
            None,
            UnOpened, // 功能开启
            LessRemainCount, // 剩余次数不足
            NoEnoughPlayerFavorability, // 玩家体力不足
        }

        protected override void Loaded() {
            this.btnGift = this.transform.Find("ButtonGift").GetComponent<Button>();
            this.btnTalk = this.transform.Find("ButtonChat").GetComponent<Button>();
            this.btnFete = this.transform.Find("ButtonFete").GetComponent<Button>();
            this.btnMusic = this.transform.Find("ButtonPlay").GetComponent<Button>();
            this.btnDance = this.transform.Find("ButtonDance").GetComponent<Button>();

            this.btnGiftText = this.btnGift.transform.Find("NormalRoot/Text").GetComponent<Text>();
            this.btnTalkText = this.btnTalk.transform.Find("NormalRoot/Text").GetComponent<Text>();
            this.btnFeteText = this.btnFete.transform.Find("NormalRoot/Text").GetComponent<Text>();
            this.btnMusicText = this.btnMusic.transform.Find("NormalRoot/Text").GetComponent<Text>();
            this.btnDanceText = this.btnDance.transform.Find("NormalRoot/Text").GetComponent<Text>();

            this.btnGiftRemainText = this.btnGiftText.transform.Find("Text_Number").GetComponent<Text>();
            this.btnTalkRemainText = this.btnTalkText.transform.Find("Text_Number").GetComponent<Text>();
            this.btnFeteRemainText = this.btnFeteText.transform.Find("Text_Number").GetComponent<Text>();
            this.btnMusicRemainText = this.btnMusicText.transform.Find("Text_Number").GetComponent<Text>();
            this.btnDanceRemainText = this.btnDanceText.transform.Find("Text_Number").GetComponent<Text>();


            this.btnGift.onClick.AddListener(this.OnBtnGiftClicked);
            this.btnTalk.onClick.AddListener(this.OnBtnTalkClicked);
            this.btnFete.onClick.AddListener(this.OnBtnVisitClicked);
            this.btnMusic.onClick.AddListener(this.OnBtnMusicClicked);
            this.btnDance.onClick.AddListener(this.OnBtnDanceClicked);
        }

        public void Refresh(UI_FavorabilityMenu ui, FavorabilityNPC npc) {
            this.npc = npc;
            this.ui = ui;

            TextHelper.SetText(this.btnGiftText, CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.SendGift).Name);
            TextHelper.SetText(this.btnTalkText, CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.Talk).Name);
            TextHelper.SetText(this.btnFeteText, CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.Fete).Name);
            TextHelper.SetText(this.btnMusicText, CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.Play).Name);
            TextHelper.SetText(this.btnDanceText, CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.Dance).Name);

            this.reasonGift = EReason.None;
            this.reasonTalk = EReason.None;
            this.reasonFete = EReason.None;
            this.reasonMusic = EReason.None;
            this.reasonDance = EReason.None;

            uint remain = npc.RemainActTime(EFavorabilityBahaviourType.SendGift);
            var csvBehaviour = CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.SendGift);
            bool isOpen = Sys_FunctionOpen.Instance.IsOpen(csvBehaviour.FunctionOpenid, false);
            TextHelper.SetText(this.btnGiftRemainText, 2010607, remain.ToString());
            bool enoughPlayerFavorability = Sys_NPCFavorability.Instance.Favorability >= csvBehaviour.CostPoint;
            bool result = remain > 0 && isOpen && enoughPlayerFavorability;
            ImageHelper.SetImageGray(this.btnGift, !result, true);
            if (!isOpen) { this.reasonGift = EReason.UnOpened; }
            else if (remain <= 0) { this.reasonGift = EReason.LessRemainCount; }
            else if (!enoughPlayerFavorability) { this.reasonGift = EReason.NoEnoughPlayerFavorability; }

            remain = npc.RemainActTime(EFavorabilityBahaviourType.Talk);
            csvBehaviour = CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.Talk);
            isOpen = Sys_FunctionOpen.Instance.IsOpen(csvBehaviour.FunctionOpenid, false);
            TextHelper.SetText(this.btnTalkRemainText, 2010607, remain.ToString());
            enoughPlayerFavorability = Sys_NPCFavorability.Instance.Favorability >= csvBehaviour.CostPoint;
            result = remain > 0 && isOpen && enoughPlayerFavorability;
            ImageHelper.SetImageGray(this.btnTalk, !result, true);
            if (!isOpen) { this.reasonTalk = EReason.UnOpened; }
            else if (remain <= 0) { this.reasonTalk = EReason.LessRemainCount; }
            else if (!enoughPlayerFavorability) { this.reasonTalk = EReason.NoEnoughPlayerFavorability; }

            remain = npc.RemainActTime(EFavorabilityBahaviourType.Fete);
            csvBehaviour = CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.Fete);
            isOpen = Sys_FunctionOpen.Instance.IsOpen(csvBehaviour.FunctionOpenid, false);
            TextHelper.SetText(this.btnFeteRemainText, 2010607, remain.ToString());
            enoughPlayerFavorability = Sys_NPCFavorability.Instance.Favorability >= csvBehaviour.CostPoint;
            result = remain > 0 && isOpen && enoughPlayerFavorability;
            ImageHelper.SetImageGray(this.btnFete, !result, true);
            if (!isOpen) { this.reasonFete = EReason.UnOpened; }
            else if (remain <= 0) { this.reasonFete = EReason.LessRemainCount; }
            else if (!enoughPlayerFavorability) { this.reasonFete = EReason.NoEnoughPlayerFavorability; }

            remain = npc.RemainActTime(EFavorabilityBahaviourType.Play);
            csvBehaviour = CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.Play);
            isOpen = Sys_FunctionOpen.Instance.IsOpen(csvBehaviour.FunctionOpenid, false);
            TextHelper.SetText(this.btnMusicRemainText, 2010607, remain.ToString());
            enoughPlayerFavorability = Sys_NPCFavorability.Instance.Favorability >= csvBehaviour.CostPoint;
            result = remain > 0 && isOpen && enoughPlayerFavorability;
            ImageHelper.SetImageGray(this.btnMusic, !result, true);
            if (!isOpen) { this.reasonMusic = EReason.UnOpened; }
            else if (remain <= 0) { this.reasonMusic = EReason.LessRemainCount; }
            else if (!enoughPlayerFavorability) { this.reasonMusic = EReason.NoEnoughPlayerFavorability; }

            remain = npc.RemainActTime(EFavorabilityBahaviourType.Dance);
            csvBehaviour = CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.Dance);
            isOpen = Sys_FunctionOpen.Instance.IsOpen(csvBehaviour.FunctionOpenid, false);
            TextHelper.SetText(this.btnDanceRemainText, 2010607, remain.ToString());
            enoughPlayerFavorability = Sys_NPCFavorability.Instance.Favorability >= csvBehaviour.CostPoint;
            result = remain > 0 && isOpen && enoughPlayerFavorability;
            ImageHelper.SetImageGray(this.btnDance, !result, true);
            if (!isOpen) { this.reasonDance = EReason.UnOpened; }
            else if (remain <= 0) { this.reasonDance = EReason.LessRemainCount; }
            else if (!enoughPlayerFavorability) { this.reasonDance = EReason.NoEnoughPlayerFavorability; }
        }

        // 赠礼
        private void OnBtnGiftClicked() {
            if (this.reasonGift == EReason.UnOpened) {
                Sys_Hint.Instance.PushContent_Normal("功能未开启");
            }
            else if (this.reasonGift == EReason.LessRemainCount) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010610));
            }
            else if (this.reasonGift == EReason.NoEnoughPlayerFavorability) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010611));
            }
            else {
                Sys_Dialogue.Instance.eventEmitter.Trigger<bool>(Sys_Dialogue.EEvents.OnShowContent, false);
                this.ui.showTitle = false;
                this.ui.Layout.title.SetActive(false);
                UIManager.OpenUI(EUIID.UI_FavorabilitySendGift, false, this.npc.id);
            }
        }
        // 交谈
        private void OnBtnTalkClicked() {
            //reasonTalk = EReason.None;

            if (this.reasonTalk == EReason.UnOpened) {
                Sys_Hint.Instance.PushContent_Normal("功能未开启");
            }
            else if (this.reasonTalk == EReason.LessRemainCount) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010610));
            }
            else if (this.reasonTalk == EReason.NoEnoughPlayerFavorability) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010611));
            }
            else {
                uint dialogueId = this.npc.csvNPCFavorabilityStage.TalkDia;
                CSVDialogue.Data csvDialogue = CSVDialogue.Instance.GetConfData(dialogueId);
                List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(csvDialogue);

                ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
                resetDialogueDataEventData.Init(datas, () => {
                    Sys_NPCFavorability.Instance.ReqCmdFavorabilityAddValue((uint)EFavorabilityBahaviourType.Talk, this.npc.id);
                }, csvDialogue);
                Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData, false);
            }
        }
        // 宴请
        private void OnBtnVisitClicked() {
            if (this.reasonFete == EReason.UnOpened) {
                Sys_Hint.Instance.PushContent_Normal("功能未开启");
            }
            else if (this.reasonFete == EReason.LessRemainCount) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010610));
            }
            else if (this.reasonFete == EReason.NoEnoughPlayerFavorability) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010611));
            }
            else {
                Sys_Dialogue.Instance.eventEmitter.Trigger<bool>(Sys_Dialogue.EEvents.OnShowContent, false);
                this.ui.showTitle = false;
                this.ui.Layout.title.SetActive(false);
                UIManager.OpenUI(EUIID.UI_FavorabilityFete, false, this.npc.id);
            }
        }
        // 音乐
        private void OnBtnMusicClicked() {
            if (this.reasonMusic == EReason.UnOpened) {
                Sys_Hint.Instance.PushContent_Normal("功能未开启");
            }
            else if (this.reasonMusic == EReason.LessRemainCount) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010610));
            }
            else if (this.reasonMusic == EReason.NoEnoughPlayerFavorability) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010611));
            }
            else {
                Sys_Dialogue.Instance.eventEmitter.Trigger<bool>(Sys_Dialogue.EEvents.OnShowContent, false);
                this.ui.showTitle = false;
                this.ui.Layout.title.SetActive(false);
                UIManager.OpenUI(EUIID.UI_FavorabilityMusicList, false, this.npc.id);
            }
        }
        // 舞蹈
        private void OnBtnDanceClicked() {
            if (this.reasonDance == EReason.UnOpened) {
                Sys_Hint.Instance.PushContent_Normal("功能未开启");
            }
            else if (this.reasonDance == EReason.LessRemainCount) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010610));
            }
            else if (this.reasonDance == EReason.NoEnoughPlayerFavorability) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010611));
            }
            else {
                Sys_Dialogue.Instance.eventEmitter.Trigger<bool>(Sys_Dialogue.EEvents.OnShowContent, false);
                this.ui.showTitle = false;
                this.ui.Layout.title.SetActive(false);
                UIManager.OpenUI(EUIID.UI_FavorabilityDanceList, false, this.npc.id);
            }
        }
    }

    // 菜单
    public class UIFavorabilityMenuCtrl : UIFavorabilityBase {
        public GameObject letterGo;
        public GameObject wishGo;
        public GameObject normalGo;
        public GameObject clueGo;

        public UIFavorabilityNormalMenu normalMenu;

        private Button btnLetter;
        private Button btnWish;
        private Button btnClue;

        protected override void Loaded() {
            this.letterGo = this.transform.Find("Content_Thanks").gameObject;
            this.wishGo = this.transform.Find("Content_Wish").gameObject;
            this.normalGo = this.transform.Find("Content_Choice").gameObject;
            this.clueGo = this.transform.Find("Content_Heal").gameObject;

            this.btnLetter = this.transform.Find("Content_Thanks/ButtonGift").GetComponent<Button>();
            this.btnWish = this.transform.Find("Content_Wish/ButtonGift").GetComponent<Button>();
            this.btnClue = this.transform.Find("Content_Heal/ButtonGift").GetComponent<Button>();

            this.btnLetter.onClick.AddListener(this.OnBtnLetterClicked);
            this.btnWish.onClick.AddListener(this.OnBtnWishClicked);
            this.btnClue.onClick.AddListener(this.OnBtnClueClicked);
        }

        public override void Refresh(UI_FavorabilityMenu ui, FavorabilityNPC npc) {
            base.Refresh(ui, npc);

            this.letterGo.SetActive(ui.npcType == ENPCFavorabilityType.Letter);
            this.wishGo.SetActive(ui.npcType == ENPCFavorabilityType.WishTask);
            this.normalGo.SetActive(ui.npcType == ENPCFavorabilityType.Normal);
            this.clueGo.SetActive(ui.npcType == ENPCFavorabilityType.SickNess);

            if (ui.npcType == ENPCFavorabilityType.Normal) {
                if (this.normalMenu == null) {
                    this.normalMenu = new UIFavorabilityNormalMenu();
                    this.normalMenu.Init(this.normalGo.transform);
                }
                this.normalMenu.Refresh(ui, npc);
            }
        }

        // 治疗
        private void OnBtnClueClicked() {
            Sys_Dialogue.Instance.eventEmitter.Trigger<bool>(Sys_Dialogue.EEvents.OnShowContent, false);
            this.ui.showTitle = false;
            this.ui.Layout.title.SetActive(false);
            UIManager.OpenUI(EUIID.UI_FavorabilityClue, false, this.npc.id);
        }
        // 领取感谢信
        private void OnBtnLetterClicked() {
            Sys_Dialogue.Instance.eventEmitter.Trigger<bool>(Sys_Dialogue.EEvents.OnShowContent, false);
            this.ui.showTitle = false;
            this.ui.Layout.title.SetActive(false);
            UIManager.OpenUI(EUIID.UI_FavorabilityThanks, false, this.npc.id);
        }
        // 领取心愿任务
        private void OnBtnWishClicked() {
            uint oldTaskId = Sys_NPCFavorability.Instance.GetExistedTask();
            uint newTaskId = this.npc.csvNPCFavorabilityStage.WishTask;
            if (oldTaskId != 0) {
                if (oldTaskId == newTaskId) {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010618, LanguageHelper.GetNpcTextContent(this.npc.csvNPC.name)));
                }
                else {
                    string taskName = "";
                    var csvTask = CSVTask.Instance.GetConfData(oldTaskId);
                    if (csvTask != null) {
                        taskName = LanguageHelper.GetTaskTextContent(csvTask.taskName);
                    }
                    else {
                        DebugUtil.LogErrorFormat("Task id {0} dont exist!", oldTaskId);
                    }

                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2010601, taskName);
                    PromptBoxParameter.Instance.SetConfirm(true, () => {
                        Sys_Task.Instance.ReqForgo(oldTaskId);
                        Sys_NPCFavorability.Instance.ReqReceiveTask(this.npc.id, newTaskId);
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
            }
            else {
                Sys_NPCFavorability.Instance.ReqReceiveTask(this.npc.id, newTaskId);
            }
        }
    }

    public enum ENPCFavorabilityType {
        Letter,   // 感谢书信
        WishTask,  // 心愿任务
        Normal,   // 正常
        SickNess,   // 生病
        Max,
    }

    // 拜访, 几个和NPC交互的入口
    public class UI_FavorabilityMenu : UIBase, UI_FavorabilityMenu_Layout.IListener {
        public UIFavorabilityMenuCtrl menuCtrl;
        public UI_FavorabilityMenu_Layout Layout = new UI_FavorabilityMenu_Layout();

        public uint npcId = 0;
        public uint dialogueId = 0;
        public FavorabilityNPC npc;
        public ENPCFavorabilityType npcType;

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg) {
            this.showTitle = true;
            this.npcId = Convert.ToUInt32(arg);
            Sys_NPCFavorability.Instance.TryGetNpc(this.npcId, out this.npc, false);
        }

        public bool showTitle = true;
        protected override void OnShow() {
            if (this.menuCtrl == null) {
                this.menuCtrl = new UIFavorabilityMenuCtrl();
                this.menuCtrl.Init(this.transform.Find("Animator"));
            }

            this.Layout.title.SetActive(this.showTitle);
            this.RefreshAll();
        }

        public void RefreshAll() {
            Sys_NPCFavorability.Instance.GetNpcStatus(this.npcId, out this.dialogueId, out this.npcType);
            if (this.npcType == ENPCFavorabilityType.Max) {
                this.OnBtnExitClicked();
                return;
            }

            //Debug.LogError("npcType: " + npcType);
            this.menuCtrl.Refresh(this, this.npc);

            uint left = this.npc.favorability;
            uint right = this.npc.csvNPCFavorabilityStage.FavorabilityValueMax;
            if (right <= 0) {
                TextHelper.SetText(this.Layout.npcFavorability, "Max");
                this.Layout.npcSliderFavorability.value = 1f;
            }
            else {
                TextHelper.SetText(this.Layout.npcFavorability, string.Format("{0}/{1}", left.ToString(), right.ToString()));
                this.Layout.npcSliderFavorability.value = 1f * left / right;
            }

            this.Layout.playerFavorability.text = Sys_NPCFavorability.Instance.Favorability.ToString();

            CSVFavorabilityStageName.Data csvStageName = CSVFavorabilityStageName.Instance.GetConfData(this.npc.csvNPCFavorabilityStage.Stage);
            if (csvStageName != null) {
                TextHelper.SetText(this.Layout.stageText, csvStageName.name);
            }

            CSVNPCMood.Data csvNPCMood = CSVNPCMood.Instance.GetConfData(this.npc.moodId);
            if (csvNPCMood != null) {
                ImageHelper.SetIcon(this.Layout.modeImage, csvNPCMood.Icon);
                TextHelper.SetText(this.Layout.modeValue, string.Format("{0}/{1}", this.npc.moodValue.ToString(), 100));
                this.Layout.modeSlider.value = 1f * this.npc.moodValue / 100;
            }

            var usedTotal = Logic.FavorabilityNPC.UsedTotalActTimes(EFavorabilityBahaviourType.SendGift);
            CSVFavorabilityBehavior.Data csvBe = CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.SendGift);
            var total = csvBe != null ? csvBe.DailyTotal : usedTotal;
            var leftTimes = (long) total - (long) usedTotal;
            TextHelper.SetText(this.Layout.sendGiftTotalTimes, 2010660, leftTimes.ToString());
        }

        public void OnBtnExitClicked() {
            UIManager.CloseUI(EUIID.UI_FavorabilityDialogue);
            UIManager.CloseUI(EUIID.UI_FavorabilityMenu);
        }

        private readonly UIRuleParam par = new UIRuleParam {
            StrContent = LanguageHelper.GetTextContent(2010646),
        };
        
        public void OnBtnNpcFavorabilityClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityStageRewardPreview, false, new Tuple<float, float, uint>(704f, -286f, this.npc.id));
        }

        public void OnBtnModeDescClicked() {
            CSVNPCMood.Data csvNPCMood = CSVNPCMood.Instance.GetConfData(this.npc.moodId);
            if (csvNPCMood != null) {
                string npcName = LanguageHelper.GetNpcTextContent(this.npc.csvNPC.name);
                string content = LanguageHelper.GetTextContent(csvNPCMood.Des, npcName, (csvNPCMood.FavorabilityRatio / 100).ToString());
                Vector3 pos = new Vector3(-433, -314, 0);
                UIManager.OpenUI(EUIID.UI_FavorabilityMoodDesc, false, new Tuple<string, Vector3>(content, pos));
            }
        }

        public void OnBtnPlayerFavorabilityInfoClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityTip);
        }

        protected override void ProcessEvents(bool toRegister) {
            Sys_NPCFavorability.Instance.eventEmitter.Handle<uint, uint, uint>(Sys_NPCFavorability.EEvents.OnFavorabilityStageChanged, this.OnFavorabilityStageChanged, toRegister);
            Sys_NPCFavorability.Instance.eventEmitter.Handle(Sys_NPCFavorability.EEvents.OnPlayerFavorabilityChanged, this.OnPlayerFavorabilityChanged, toRegister);
            Sys_NPCFavorability.Instance.eventEmitter.Handle<uint, uint, uint, uint>(Sys_NPCFavorability.EEvents.OnNpcFavorabilityChanged, this.OnNpcFavorabilityChanged, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnReceived, this.OnReceived, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndExit, this.OnUIExitChange, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginEnter, this.OnUIBeginChange, toRegister);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnHeroTel, this.OnHeroTel, toRegister);
            Sys_PathFind.Instance.eventEmitter.Handle<bool>(Sys_PathFind.EEvents.OnPathFind, this.OnPathFind, toRegister);
        }
        private void OnPlayerFavorabilityChanged() {
            this.RefreshAll();
        }
        private void OnNpcFavorabilityChanged(uint npcId, uint stageId, uint from, uint to) {
            if (npcId == this.npc.id) {
                this.RefreshAll();
            }
        }
        private void OnReceived(int menuId, uint taskId, TaskEntry taskEntry) {
            if (this.npc != null && taskId == this.npc.csvNPCFavorabilityStage.WishTask) {
                //this.RefreshAll();
                this.CloseSelf();
                UIManager.CloseUI(EUIID.UI_FavorabilityDialogue);
            }
        }
        private void OnFavorabilityStageChanged(uint npcId, uint oldStageId, uint newStageId) {
            if (this.gameObject != null && this.gameObject.activeInHierarchy && npcId == this.npc.id) {
                this.RefreshAll();
            }
        }
        private void OnUIBeginChange(uint stack, int id) {
            EUIID eId = (EUIID)id;
            if (eId == EUIID.UI_FavorabilityClue || eId == EUIID.UI_FavorabilityMusicList || eId == EUIID.UI_FavorabilityDanceList ||
                eId == EUIID.UI_FavorabilitySendGift || eId == EUIID.UI_FavorabilityThanks || eId == EUIID.UI_Dialogue || eId == EUIID.UI_FavorabilityFete) {
                this.showTitle = false;
                this.Layout.title.SetActive(this.showTitle);
            }
        }
        private void OnUIExitChange(uint stack, int id) {
            EUIID eId = (EUIID)id;
            if (eId == EUIID.UI_FavorabilityClue || eId == EUIID.UI_FavorabilityMusicList || eId == EUIID.UI_FavorabilityDanceList ||
                eId == EUIID.UI_FavorabilitySendGift || eId == EUIID.UI_FavorabilityThanks || eId == EUIID.UI_Dialogue || eId == EUIID.UI_FavorabilityFete) {

                this.showTitle = true;
                this.Layout.title.SetActive(this.showTitle);

                Sys_NPCFavorability.Instance.GetNpcStatus(this.npcId, out this.dialogueId, out this.npcType);
                if (UIManager.IsTop(this.nID)) {
                    if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                        if (this.npcType != ENPCFavorabilityType.Max) {
                            this.menuCtrl.Refresh(this, this.npc);
                            this.RefreshAll();
                            Sys_NPCFavorability.Instance.OpenDialogue(this.npcId, false, true);
                        }
                    }
                }

                if (this.gameObject != null) {
                    if (this.npcType == ENPCFavorabilityType.Max) {
                        this.CloseSelf();
                        UIManager.CloseUI(EUIID.UI_FavorabilityDialogue);
                    }
                }
            }
            
            if (eId == EUIID.UI_Pet_Get) {
                // 获得宠物的时候，立即关闭
                if (this.npcType == ENPCFavorabilityType.Max) 
                {
                    this.CloseSelf();
                    UIManager.CloseUI(EUIID.UI_FavorabilityDialogue);
                }
            }
        }

        private void OnHeroTel() {
            Sys_NPCFavorability.Instance.CloseAllUI();
        }

        private void OnPathFind(bool toBegin) {
            if (toBegin) {
                Sys_NPCFavorability.Instance.CloseAllUI();
            }
        }
    }
}