using System;
using System.Collections.Generic;
using Lib.Core;
using Logic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

public class LayoutBase {
    public GameObject gameObject { get; protected set; }
    public Transform transform { get; protected set; }

    public void Set(GameObject root) {
        this.gameObject = root;
        this.transform = root.transform;
    }
}

public class UI_FamilyResBattleMain : UIBase, UI_FamilyResBattleMain.Layout.IListener {
    public class Layout : LayoutBase {
        public Button btnSignup;
        public Text btnText;

        public Image icon;
        public Text ruleSesc;
        public Text stage;
        public Text remainTimeDesc;
        public CDText remainTime;
        public Text enemyTitle;
        public Text enemy;

        public Button btnRewardDetail;
        public Button btnDescDetail;

        public Transform rewardParent;
        public Toggle signupToggle;
        public CDText remainSignText;
        public Text signDesc;
        public Button btnRank;

        public void Parse(GameObject root) {
            this.Set(root);

            this.signupToggle = this.transform.Find("Animator/UI_Activity_Message/Animator/View_Content/SignupSetting").GetComponent<Toggle>();
            this.remainSignText = this.transform.Find("Animator/UI_Activity_Message/Animator/View_Content/SignUpDesc/Text").GetComponent<CDText>();
            this.signDesc = this.transform.Find("Animator/UI_Activity_Message/Animator/View_Content/SignUpDesc").GetComponent<Text>();

            this.btnRank = this.transform.Find("Animator/UI_Activity_Message/Animator/ShopItem/Btn_Rank").GetComponent<Button>();
            
            this.btnSignup = this.transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Btn_01").GetComponent<Button>();
            this.btnText = this.transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Btn_01/Text_01").GetComponent<Text>();

            this.icon = this.transform.Find("Animator/UI_Activity_Message/Animator/ShopItem/Image2").GetComponent<Image>();
            this.ruleSesc = this.transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Text_Type/Text").GetComponent<Text>();
            this.stage = this.transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Text_State/Text").GetComponent<Text>();
            this.remainTimeDesc = this.transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Text_Time").GetComponent<Text>();
            this.remainTime = this.transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Text_Time/Text").GetComponent<CDText>();
            this.enemyTitle = this.transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Text_Enemy").GetComponent<Text>();
            this.enemy = this.transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Text_Enemy/Text").GetComponent<Text>();

            this.rewardParent = this.transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Reward_List/Grid");

            this.btnRewardDetail = this.transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Btn_Details").GetComponent<Button>();
            this.btnDescDetail = this.transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Btn_Rule").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener) {
            this.btnRank.onClick.AddListener(listener.OnBtnRankClicked);
            this.btnSignup.onClick.AddListener(listener.OnBtnClicked);
            this.btnRewardDetail.onClick.AddListener(listener.OnBtnRewardDetailClicked);
            this.btnDescDetail.onClick.AddListener(listener.OnBtnDescDetailClicked);
            this.remainTime.onTimeRefresh = listener.OnTimeRefresh;
            this.remainSignText.onTimeRefresh = listener.OnTimeRefresh;
            this.signupToggle.onValueChanged.AddListener(listener.OnSignupStatusChanged);
        }

        public interface IListener {
            void OnBtnClicked();
            void OnBtnRankClicked();
            void OnBtnRewardDetailClicked();
            void OnBtnDescDetailClicked();
            void OnTimeRefresh(Text text, float time, bool isEnd);
            void OnSignupStatusChanged(bool status);
        }
    }

    public Layout layout = new Layout();
    public UI_RewardList rewardList;
    private Timer timerRefresh;
    private CSVFamilyFight.Data csvActivity;

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }

    protected override void OnDestroy() {
        this.rewardList?.Clear();
        this.timerRefresh?.Cancel();
    }

    protected override void OnShow() {
        this.RefreshAll();
    }

    protected override void OnOpen(object arg) {
        var activityId = Convert.ToUInt32(arg);
        this.csvActivity = CSVFamilyFight.Instance.GetConfData(activityId);
    }

    protected override void OnOpened() {
        if (this.csvActivity != null) {
            ImageHelper.SetIcon(this.layout.icon, this.csvActivity.familyActiveIcon);
        }

        if (this.rewardList == null) {
            this.rewardList = new UI_RewardList(this.layout.rewardParent, EUIID.UI_FamilyResBattleMain);
        }

        this.rewardList.SetRewardList(CSVDrop.Instance.GetDropItem(Sys_FamilyResBattle.Instance.battlewinFamilyDropId));
        this.rewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);

        TextHelper.SetText(this.layout.ruleSesc, 3230000009);
        bool canSign = Sys_FamilyResBattle.Instance.IsSignupMember(Sys_Role.Instance.RoleId);
        if (canSign) {
            this.layout.signupToggle.gameObject.SetActive(true);
            this.layout.signupToggle.isOn = Sys_FamilyResBattle.Instance.chooseSignupSetting;

            this.layout.signDesc.gameObject.SetActive(false);
        }
        else {
            this.layout.signupToggle.gameObject.SetActive(false);

            long diff = Sys_Family.Instance.jointime + Sys_FamilyResBattle.Instance.EnterLimitedPlayerJoinFamilyDays * 86400 - Sys_Time.Instance.GetServerTime();
            if (diff > 0) {
                this.layout.signDesc.gameObject.SetActive(true);
                var hour = Sys_FamilyResBattle.Instance.EnterLimitedPlayerJoinFamilyDays * 24;
                TextHelper.SetText(this.layout.signDesc, 3230000063, hour.ToString());
                this.layout.remainSignText.Begin(diff);
            }
            else {
                this.layout.signDesc.gameObject.SetActive(false);
            }
        }

        this.timerRefresh?.Cancel();
        // 强制10s刷新一次，因为cdtext内部有误差，而且很大
        this.timerRefresh = Timer.RegisterOrReuse(ref this.timerRefresh, 10f, this.RefreshAll, null, true);
    }

    public void RefreshAll() {
        Sys_FamilyResBattle.EStage stage = Sys_FamilyResBattle.Instance.stage;
        var csvFamilyResBattle = CSVFamilyResBattleState.Instance.GetConfData((uint) stage);
        if (csvFamilyResBattle == null) {
            return;
        }

        TextHelper.SetText(this.layout.stage, csvFamilyResBattle.SrateText);
        TextHelper.SetText(this.layout.remainTimeDesc, csvFamilyResBattle.stageCDLanId);
        TextHelper.SetText(this.layout.enemyTitle, csvFamilyResBattle.TitleText);

        void RefreshEnemy() {
            if (Sys_FamilyResBattle.Instance.hasBlue) {
                string s = string.Format("{0}({1})", Sys_FamilyResBattle.Instance.blueFamlilyName, Sys_FamilyResBattle.Instance.blueServerName);
                TextHelper.SetText(this.layout.enemy, s);
            }
            else {
                if (Sys_FamilyResBattle.Instance.hasSigned) {
                    if (Sys_FamilyResBattle.Instance.noTurn) {
                        TextHelper.SetText(this.layout.enemy, 3230000025);
                    }
                    else {
                        TextHelper.SetText(this.layout.enemy, 3230000014);
                    }
                }
                else {
                    TextHelper.SetText(this.layout.enemy, 3230000033);
                }
            }
        }

        bool showBtn = true;
        if (stage == Sys_FamilyResBattle.EStage.Signup) {
            bool hasSigned = Sys_FamilyResBattle.Instance.hasSigned;
            showBtn = (!hasSigned);
            uint lanId = hasSigned ? 3230000011 : 3230000010;
            TextHelper.SetText(this.layout.btnText, lanId);

            TextHelper.SetText(this.layout.enemy, 3230000031);
        }
        else if (stage == Sys_FamilyResBattle.EStage.Match) {
            bool hasSigned = Sys_FamilyResBattle.Instance.hasSigned;
            showBtn = false;
            uint lanId = hasSigned ? 3230000011 : 3230000012;
            TextHelper.SetText(this.layout.btnText, lanId);

            RefreshEnemy();
        }
        else if (stage == Sys_FamilyResBattle.EStage.ReadyBattle || stage == Sys_FamilyResBattle.EStage.Battle) {
            bool hasSigned = Sys_FamilyResBattle.Instance.hasSigned;
            showBtn = (hasSigned && Sys_FamilyResBattle.Instance.hasBlue);

            uint lanId = hasSigned ? 3230000013 : 3230000012;
            TextHelper.SetText(this.layout.btnText, lanId);

            RefreshEnemy();
        }
        else if (stage == Sys_FamilyResBattle.EStage.BattleEnd) {
            TextHelper.SetText(this.layout.btnText, 3230000014);
            TextHelper.SetText(this.layout.enemy, 3230000032);

            showBtn = false;
        }

        ButtonHelper.Enable(this.layout.btnSignup, showBtn && !Sys_FamilyResBattle.Instance.InFamilyBattle);

        long diff = Sys_FamilyResBattle.Instance.endTimeOfStage - Sys_Time.Instance.GetServerTime();
        this.layout.remainTime.Begin(diff);
    }

    public void OnBtnClicked() {
        Sys_FamilyResBattle.EStage stage = Sys_FamilyResBattle.Instance.stage;
        if (stage == Sys_FamilyResBattle.EStage.Signup) {
            UIManager.OpenUI(EUIID.UI_FamilyResBattleSignup);
        }
        else if (Sys_FamilyResBattle.Instance.stage == Sys_FamilyResBattle.EStage.ReadyBattle ||
                 Sys_FamilyResBattle.Instance.stage == Sys_FamilyResBattle.EStage.Battle) {
            // Sys_FamilyResBattle.Instance.ReqEnterBattleField();

            uint npcId = Sys_FamilyResBattle.Instance.EnterNpcId;
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(npcId);

            this.CloseSelf();
            UIManager.CloseUI(EUIID.UI_Family);
        }
    }

    public void OnBtnRankClicked() {
        UIManager.OpenUI(EUIID.UI_FamilyResBattleFamilyRank);
    }

    public void OnBtnRewardDetailClicked() {
        UIManager.OpenUI(EUIID.UI_FamilyResBattleReward);
    }

    public void OnBtnDescDetailClicked() {
        Sys_CommonCourse.Instance.OpenCommonCourse(3);
    }

    public void OnTimeRefresh(Text text, float time, bool isEnd) {
        if (isEnd) {
            TextHelper.SetText(text, "00:00:00");
        }
        else {
            var t = Mathf.Round(time);
            var s = LanguageHelper.TimeToString((uint) t, LanguageHelper.TimeFormat.Type_1);
            TextHelper.SetText(text, s);
        }
    }

    public void OnSignupStatusChanged(bool status) {
        Sys_FamilyResBattle.Instance.ReqChoseSignupSetting(status);
    }

    #region 事件处理

    protected override void ProcessEvents(bool toRegister) {
        Sys_FamilyResBattle.Instance.eventEmitter.Handle<uint, uint, long>(Sys_FamilyResBattle.EEvents.OnStageChanged, this.OnStageChanged, toRegister);
        Sys_FamilyResBattle.Instance.eventEmitter.Handle<bool, bool>(Sys_FamilyResBattle.EEvents.OnSignupChanged, this.OnSignupChanged, toRegister);
        Sys_FamilyResBattle.Instance.eventEmitter.Handle(Sys_FamilyResBattle.EEvents.OnEnter, this.OnEnter, toRegister);
        Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, this.OnTimeNtf, toRegister);
        Sys_FamilyResBattle.Instance.eventEmitter.Handle(Sys_FamilyResBattle.EEvents.OnSignupSettingChanged, this.OnSignupSettingChanged, toRegister);
    }

    private void OnStageChanged(uint oldStage, uint newStage, long remainTime) {
        this.RefreshAll();
    }

    private void OnSignupChanged(bool _, bool __) {
        this.RefreshAll();
    }

    private void OnEnter() {
        this.CloseSelf();
        UIManager.CloseUI(EUIID.UI_Family);
    }

    private void OnTimeNtf(uint oldTime, uint newTime) {
        this.RefreshAll();
    }

    private void OnSignupSettingChanged() {
        layout.signupToggle.isOn = Sys_FamilyResBattle.Instance.chooseSignupSetting;
    }

    #endregion
}