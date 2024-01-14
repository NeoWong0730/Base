using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_TaskOrTeamMain : UIComponent {
        public Transform parent;

        // 对应ui_menu的Btn_Task
        public Transform openBtn;
        public Button closeBtn;

        public CP_Toggle taskToggle;
        public CP_Toggle teamToggle;
        public CP_Toggle instanceToggle;
        public CP_Toggle familyToggle;

        public UI_TaskMain taskMain;
        public UI_TeamMain teamMain;
        public UI_MagicOpen magicOpen;
        public UI_InstanceMain instanceMain;
        public UIFamilyResBattleMain familyMain;

        public UI_FamilyConstructOpen familyConstruct; // 家族建设特殊弹窗
        public UI_FamilyPartyMenuView familyPartyView; //家族酒会小面板


        public Transform transTeamNormal;
        public Transform transTeamNormalMatching;

        public Transform transTeamSelect;
        public Transform transTeamSelectMatching;

        public Text m_TexTeamNormalNum;
        public Text m_TexTeamSelectNum;

        public GameObject matchingFxGo;

        public Text m_TexInstanceNameNormal;
        public Text m_TexInstanceNameSelect;
        protected override void Loaded() {
            this.taskMain = new UI_TaskMain();
            this.taskMain.Init(this.transform);
            this.teamMain = new UI_TeamMain();
            this.teamMain.Init(this.transform);
            this.magicOpen = new UI_MagicOpen();
            this.magicOpen.Init(this.transform.Find("Object_Function"));
            this.instanceMain = new UI_InstanceMain();
            this.instanceMain.Init(this.transform);
            this.familyMain = new UIFamilyResBattleMain();
            this.familyMain.Init(this.transform);

            familyConstruct = AddComponent<UI_FamilyConstructOpen>(this.transform.Find("View_FamilyConstruct"));
            familyPartyView = AddComponent<UI_FamilyPartyMenuView>(this.transform.Find("View_FamilyParty"));
            this.parent = this.transform.parent.parent;
            this.openBtn = this.parent.Find("Button_Open");
            this.closeBtn = this.transform.Find("Button_Close").GetComponent<Button>();
            this.taskToggle = this.transform.Find("Toggle_Task").GetComponent<CP_Toggle>();
            this.teamToggle = this.transform.Find("Toggle_Team").GetComponent<CP_Toggle>();
            this.instanceToggle = this.transform.Find("Toggle_Copy").GetComponent<CP_Toggle>();
            this.familyToggle = this.transform.Find("Toggle_FamilyResBattle").GetComponent<CP_Toggle>();

            this.closeBtn.onClick.AddListener(this.OnBtnCloseClicked);
            this.taskToggle.onValueChanged.AddListener(this.SwitchToTask);
            this.teamToggle.onValueChanged.AddListener(this.SwitchToTeam);
            this.instanceToggle.onValueChanged.AddListener(this.SwitchToInstance);
            this.familyToggle.onValueChanged.AddListener(this.SwitchToFamily);

            this.ProcessEventsForAwake(true);


            transTeamNormal = teamToggle.transform.Find("Normal/Normal");
            transTeamNormalMatching = teamToggle.transform.Find("Normal/Matching");
            m_TexTeamNormalNum = transTeamNormal.Find("Text_Num").GetComponent<Text>();

            transTeamSelect = teamToggle.transform.Find("Select/Normal");
            transTeamSelectMatching = teamToggle.transform.Find("Select/Matching");
            m_TexTeamSelectNum = transTeamSelect.Find("Text_Num").GetComponent<Text>();

            matchingFxGo = this.transform.Find("Toggle_Team/Fx_ui_team_huanrao").gameObject;

            m_TexInstanceNameNormal = this.transform.Find("Toggle_Copy/Normal/Text").GetComponent<Text>();
            m_TexInstanceNameSelect = this.transform.Find("Toggle_Copy/Select/Text").GetComponent<Text>();
        }

        protected override void ProcessEventsForEnable(bool toRegister) {
            Sys_Team.Instance.eventEmitter.Handle<bool>(Sys_Team.EEvents.MatchState, OnTeamMatchingState, toRegister);

            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.NetMsg_InfoNtf, UpdateTeamMemCount, toRegister);

            Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemLeaveNtf, OnTeamMemerLeave, toRegister);
        }

        protected override void ProcessEventsForAwake(bool toRegister) {
            Sys_FuncPreview.Instance.eventEmitter.Handle<uint>(Sys_FuncPreview.EEvents.OnDel, (delId) => {
                this.CheckMagicState();
            }, toRegister);
            
#if DEBUG_MODE
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, (fd) => {
                if (fd.id == 51101) {
                    this.CheckMagicState();
                }
            }, toRegister);
            Sys_FunctionOpen.Instance.eventEmitter.Handle(Sys_FunctionOpen.EEvents.UnLockAllFuntion, this.CheckMagicState, toRegister);
#endif
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, this.CheckMagicState, toRegister);
            
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnLoadOK, FamilyRefeshState, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, FamilyRefeshState, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.GuildCurrencyChange, FamilyRefeshState, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.GuildConstructLvChange, FamilyRefeshState, toRegister);
        }

        public void SwitchToTask(bool arg) {
            if (arg) {
                Sys_Task.currentTabType = Sys_Task.ETabType.Task;
                this.teamMain.Hide();
            }

            if (!Sys_Task.toggle) {
                if (arg) {
                    this.taskMain.RefreshPages();
                    Sys_Task.toggle = true;
                }
            }
            else {
                if (arg) {
                    if (Sys_Task.Instance.receivedTasks.Count > 0) {
                        UIManager.OpenUI(EUIID.UI_TaskList);
                    }
                }
            }
        }

        public void SwitchToTeam(bool arg) {
            var lastTabType = Sys_Task.currentTabType;

            if (arg)
                Sys_Task.currentTabType = Sys_Task.ETabType.Team;

            if (!Sys_Task.toggle) {
                if (arg) {
                    this.teamMain.Show();
                    Sys_Task.toggle = true;
                }
                else {
                    this.teamMain.Hide();
                }
            }
            else {
                if (arg) {
                    this.teamMain.ToAgain();
                }
            }

            UpdateTeamState();
        }

        public void SwitchToFamily(bool arg) {
            if (arg) {
                Sys_Task.currentTabType = Sys_Task.ETabType.FamilyRes;
                this.teamMain.Hide();
            }

            if (!Sys_Task.toggle) {
                if (arg) {
                    this.familyMain.RefreshView();
                    Sys_Task.toggle = true;
                }
            }
            else {
                if (arg) {
                    Sys_CommonCourse.Instance.OpenCommonCourse(3, 301, 30103);
                }
            }
        }

        public void SwitchToInstance(bool arg) {
            if (arg) {
                Sys_Task.currentTabType = Sys_Task.ETabType.Dungon;
                this.teamMain.Hide();
            }

            if (!Sys_Task.toggle) {
                if (arg) {
                    this.instanceMain.RefreshView();
                    Sys_Task.toggle = true;
                }
            }
            else {
                if (arg) {
                    var mode = this.GetMode();
                    if (mode == EMode.SingleInstance)
                        UIManager.OpenUI(EUIID.UI_Onedungeons);
                    else if (mode == EMode.MultiInstance)
                        UIManager.OpenUI(EUIID.UI_Multi_Info, false, Sys_Instance.Instance.curInstance.InstanceId);
                    else if (mode == EMode.HundreadPeolle)
                        UIManager.OpenUI(EUIID.UI_HundredPeopleArea);
                }
            }
        }

        private UI_Menu ui;

        public void SetUI(UI_Menu ui) {
            this.ui = ui;
        }

        private void OnBtnCloseClicked() {
            if (Sys_HundredPeopleArea.Instance.IsInstance) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1006203));
                return;
            }

            ui.isTaskExpand = false;
            this.openBtn.gameObject.SetActive(true);
            this.Hide();
            teamMain.HideWith();
        }

        public override void Show() {
            base.Show();

            UpdateTeamMemCount();

            UpdateTeamState();
        }

        public void OnHide() {
            Sys_Task.toggle = false;

            teamMain.Hide();
            familyPartyView.Hide();
        }

        public override void OnDestroy() {
            this.taskMain.OnDestroy();
            this.teamMain.OnDestroy();
            this.instanceMain.OnDestroy();
            this.familyMain.OnDestroy();

            Sys_Task.toggle = false;
            familyPartyView?.OnDestroy();
            base.OnDestroy();
        }

        /// <summary>
        /// 设置模式
        /// </summary>
        public void SetMode() {
            if (Sys_Task.currentTabType == Sys_Task.ETabType.Team) {
                if (!Sys_Task.toggle) {
                    this.teamToggle.SetSelected(true, false); //Sys_Team.Instance.IsOpen() && Sys_Team.Instance.HaveTeam
                    this.teamMain.DoRefresh();
                    Sys_Task.toggle = true;
                }
                else {
                    this.teamMain.Show();
                }

                if (Sys_Instance.Instance.IsInInstance) {
                    this.taskToggle.gameObject.SetActive(false);
                    this.familyToggle.gameObject.SetActive(false);
                    this.instanceToggle.gameObject.SetActive(true);
                    RefreshInstanceName();
                    instanceToggle.SetSelected(true, true);
                }
                else if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                    this.taskToggle.gameObject.SetActive(false);
                    this.instanceToggle.gameObject.SetActive(false);
                    this.familyToggle.gameObject.SetActive(true);
                }
                else {
                    this.familyToggle.gameObject.SetActive(false);
                    this.instanceToggle.gameObject.SetActive(false);
                    this.taskToggle.gameObject.SetActive(true);
                }
            }
            else {
                this.teamMain.Hide();

                if (Sys_Instance.Instance.IsInInstance) {
                    // 副本模式，影藏任务
                    //this.taskToggle.SetSelected(false, true);
                    this.taskToggle.gameObject.SetActive(false);

                    //this.familyToggle.SetSelected(false, true);
                    this.familyToggle.gameObject.SetActive(false);

                    this.instanceToggle.gameObject.SetActive(true);

                    RefreshInstanceName();

                    if (!Sys_Task.toggle) {
                        this.instanceToggle.SetSelected(true, true);
                    }
                    else {
                        this.instanceMain.RefreshView();
                    }
                }
                else if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                    //this.taskToggle.SetSelected(false, true);
                    this.taskToggle.gameObject.SetActive(false);

                    //this.instanceToggle.SetSelected(false, true);
                    this.instanceToggle.gameObject.SetActive(false);

                    this.familyToggle.gameObject.SetActive(true);
                    if (!Sys_Task.toggle) {
                        this.familyToggle.SetSelected(true, true);
                    }
                    else {
                        this.familyMain.RefreshView();
                    }
                }
                else {
                    // 任务模式，影藏副本
                    //this.instanceToggle.SetSelected(false, true);
                    this.instanceToggle.gameObject.SetActive(false);

                    //this.familyToggle.SetSelected(false, true);
                    this.familyToggle.gameObject.SetActive(false);

                    this.taskToggle.gameObject.SetActive(true);
                    if (!Sys_Task.toggle) {
                        this.taskToggle.SetSelected(true, true);
                    }
                    else {
                        this.taskMain.RefreshPages();
                    }
                }
            }
        }

        /// <summary>
        /// 得到模式
        /// </summary>
        /// <returns></returns>
        public EMode GetMode() {
            EMode eMode = EMode.Normal;
            // 如果组队模式
            if (Sys_Task.currentTabType == Sys_Task.ETabType.Team) {
                eMode = EMode.Team;
            }
            else {
                // 非组队模式：normal/dungon
                if (Sys_Instance.Instance.IsInInstance) {
                    CSVInstance.Data cSVInstanceData = CSVInstance.Instance.GetConfData(Sys_Instance.Instance.curInstance.InstanceId);
                    if (null != cSVInstanceData) {
                        switch ((EInstanceType) cSVInstanceData.Type) {
                            case EInstanceType.SingleDaily:
                                eMode = EMode.SingleInstance;
                                break;
                            case EInstanceType.MultiDaily:
                                eMode = EMode.MultiInstance;
                                break;
                            case EInstanceType.TerroristWeek:
                                eMode = EMode.TerroristInstance;
                                break;
                            case EInstanceType.HundreadPeolle:
                                eMode = EMode.HundreadPeolle;
                                break;
                        }
                    }
                }
                else if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                    eMode = EMode.FamilyRes;
                }
            }

            return eMode;
        }

        /// <summary>
        /// 界面模式
        /// </summary>
        public enum EMode {
            Normal, //正常模式
            Team, // 组队模式
            FamilyRes, // 家族资源战
            SingleInstance, //单人副本模式
            MultiInstance, //多人副本模式
            TerroristInstance, //恐怖旅团副本模式
            HundreadPeolle, // 百人道场
        }

        /// <summary>
        /// 其他挂件刷新处
        /// </summary>
        public void FunctionState() {
            CheckMagicState();
            FamilyInitState();
            FamilyPartyInitState();
        }

        private void FamilyRefeshState() {
            if (!Sys_FunctionOpen.Instance.IsOpen(30501, false))
                return;
            if (null != this.familyConstruct) {
                familyConstruct.SetSys();
            }
        }

        private void FamilyRefeshState(uint id, long value) {
            if (id != (uint) ECurrencyType.FamilyStamina) {
                return;
            }

            if (!Sys_FunctionOpen.Instance.IsOpen(30501, false))
                return;
            if (null != this.familyConstruct) {
                familyConstruct.SetSys();
            }
        }

        private void FamilyInitState() {
            if (!Sys_FunctionOpen.Instance.IsOpen(30501, false))
                return;
            if (null != this.familyConstruct) {
                familyConstruct.Init();
            }
        }

        private void FamilyPartyInitState() {
            if (null != this.familyPartyView) {
                familyPartyView.Init();
            }
        }

        private void CheckMagicState() {
            if (!Sys_FunctionOpen.Instance.IsOpen(51101, false))
                return;
            if (null != this.magicOpen) {
                magicOpen.SetSys();
            }
        }

        private void OnTeamMatchingState(bool ismatch) {
            UpdateTeamState();
        }

        public void UpdateTeamState() {
            if (!teamToggle.IsOn) {
                transTeamNormal.gameObject.SetActive(!Sys_Team.Instance.isMatching);
                transTeamNormalMatching.gameObject.SetActive(Sys_Team.Instance.isMatching);
                if (matchingFxGo.activeInHierarchy != Sys_Team.Instance.isMatching)
                    matchingFxGo.SetActive(Sys_Team.Instance.isMatching);
            }
            else {
                transTeamSelect.gameObject.SetActive(!Sys_Team.Instance.isMatching);
                transTeamSelectMatching.gameObject.SetActive(Sys_Team.Instance.isMatching);
                if (matchingFxGo.activeInHierarchy != Sys_Team.Instance.isMatching)
                    matchingFxGo.SetActive(Sys_Team.Instance.isMatching);
            }
        }

        private void UpdateTeamMemCount() {
            var curCount = Sys_Team.Instance.TeamMemsCount;
            var maxCount = Sys_Team.Instance.TeamMemberCountMax;

            string tex = curCount == 0 ? string.Empty : (curCount.ToString() + "/" + maxCount.ToString());

            m_TexTeamNormalNum.text = tex;

            m_TexTeamSelectNum.text = tex;
        }

        private void OnTeamMemerLeave(ulong value) {
            UpdateTeamMemCount();
        }

        private void RefreshInstanceName()
        {
            uint InstanceId = Sys_Instance.Instance.curInstance.InstanceId;
            var instanceiddata = CSVInstance.Instance.GetConfData(InstanceId);
            if (instanceiddata != null && instanceiddata.Type == 7)
            {
                TextHelper.SetText(m_TexInstanceNameNormal, 14031u);
                TextHelper.SetText(m_TexInstanceNameSelect, 14031u);
            }
            else
            {
                TextHelper.SetText(m_TexInstanceNameNormal, 14057u);
                TextHelper.SetText(m_TexInstanceNameSelect, 14057u);
            }
        }
    }
}