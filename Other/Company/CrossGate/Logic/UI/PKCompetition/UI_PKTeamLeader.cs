using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_PKTeamLeader_Layout
    {
        private Transform transform;
        public Text t_TeamName,t_Tips, t_Member;
        public InfinityGrid _infinityGrid;
        public Button btn_FightSign, btn_DissolveFight, btn_CancelApply, btn_MyTeam, btn_Search;
        public InputField t_input;
        public void Init(Transform transform)
        {
            this.transform = transform;
            t_TeamName= transform.Find("Image_Name/Text").GetComponent<Text>();
            t_Tips = transform.Find("Text").GetComponent<Text>();
            btn_Search = transform.Find("InputField_Describe/Button_Search").GetComponent<Button>();
            t_input = transform.Find("InputField_Describe").GetComponent<InputField>();
            t_Member = transform.Find("Text_Member").GetComponent<Text>();

            _infinityGrid = transform.Find("ScrollView").GetComponent<InfinityGrid>();
            btn_FightSign = transform.Find("Btn_01").GetComponent<Button>();
            btn_DissolveFight = transform.Find("Btn_02").GetComponent<Button>();
            btn_CancelApply = transform.Find("Btn_03").GetComponent<Button>();
            btn_MyTeam = transform.Find("Btn_04").GetComponent<Button>();
            btn_MyTeam.gameObject.SetActive(false);
        }
        public void RegisterEvents(IListener listener)
        {
            _infinityGrid.onCreateCell += listener.OnCreateCell;
            _infinityGrid.onCellChange += listener.OnCellChange;

            btn_Search.onClick.AddListener(listener.OnClick_Search);
            btn_FightSign.onClick.AddListener(listener.OnClick_FightSign);
            btn_DissolveFight.onClick.AddListener(listener.OnClick_FightDissolve);
            btn_CancelApply.onClick.AddListener(listener.OnClick_CancelApply);
        }

        public interface IListener
        {
            void OnCellChange(InfinityGridCell arg1, int arg2);
            void OnCreateCell(InfinityGridCell obj);
            void OnClick_CancelApply();
            void OnClick_FightDissolve();
            void OnClick_FightSign();
            void OnClick_Search();
        }
    }
    public class UI_PKTeamLeader : UIComponent, UI_PKTeamLeader_Layout.IListener
    {
        private UI_PKTeamLeader_Layout layout = new UI_PKTeamLeader_Layout();
        private List<ulong> _searchData = new List<ulong>();
        private List<ulong> _tempList;
        #region 系统函数
        protected override void Loaded()
        {
            base.Loaded();
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        public override void SetData(params object[] arg)
        {
            base.SetData();
        }

        public override void Show()
        {
            base.Show();
            _tempList = Sys_PKCompetition.Instance._memberInfoList;
            UpdateShow();
        }
        public override void Hide()
        {
            base.Hide();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_PKCompetition.Instance.eventEmitter.Handle(Sys_PKCompetition.EEvents.Event_SignSuccess, Event_SignSuccess, toRegister);
            base.ProcessEventsForEnable(toRegister);
        }

        #endregion

        #region Events
        private void Event_SignSuccess()
        {
            UpdateShow();
        }
        #endregion

        private void UpdateShow()
        {

            RefreshInfinityGrid();
            RefreshFightTeamInfo();
        }

        private void RefreshFightTeamInfo()
        {
            layout.t_TeamName.text = LanguageHelper.GetTextContent(2016202, Sys_PKCompetition.Instance.TeamName, Sys_PKCompetition.Instance.JoinTeamID.ToString());
            string str = null;
            int Lv = Sys_PKCompetition.Instance.IsGetLevelLimite();
            if (Lv == 1)
                str = LanguageHelper.GetTextContent(14006);
            else if(Lv==2)
                str = LanguageHelper.GetTextContent(14007);
            else if(Lv==3)
                str = LanguageHelper.GetTextContent(14008);
            layout.t_Tips.text = LanguageHelper.GetTextContent(1001008, Sys_Role.Instance.Role.Level.ToString(), str);
            layout.t_Member.text = LanguageHelper.GetTextContent(1001013, Sys_PKCompetition.Instance.GetMemNum().ToString());
            bool isLeader = Sys_PKCompetition.Instance.IsFightTeamLeader();
            bool isSignSuccess= Sys_PKCompetition.Instance.IsFightTeamSignSuccess();
            if (isSignSuccess)
                UpdateBtnState();
            else if (isLeader)
                UpdateBtnState(true);
            else
                UpdateBtnState(false, true);
        }

        private void UpdateBtnState(bool isLeaderActive=false,bool isMineActive=false)
        {
            layout.btn_FightSign.gameObject.SetActive(isLeaderActive);
            layout.btn_DissolveFight.gameObject.SetActive(isLeaderActive);
            layout.btn_CancelApply.gameObject.SetActive(isMineActive);
        }

        private void RefreshInfinityGrid()
        {
            layout._infinityGrid.CellCount = _tempList.Count;
            layout._infinityGrid.ForceRefreshActiveCell();
        }

        public void OnCreateCell(InfinityGridCell cell)
        {
            TeamMemberCell entry = new TeamMemberCell();
            entry.Init(cell.mRootTransform);
            entry.AddAction(OnClick_Allow, OnClick_Refuse);
            cell.BindUserData(entry);
        }

        public void OnCellChange(InfinityGridCell cell, int index)
        {
            TeamMemberCell entry = cell.mUserData as TeamMemberCell;
            entry.RefreshData(_tempList[index]);
        }

        #region 事件Action
        private void OnClick_Refuse(ulong id)
        {
            Sys_PKCompetition.Instance.Req_HandleApply(id,false);
        }

        private void OnClick_Allow(ulong id)
        {
            Sys_PKCompetition.Instance.Req_HandleApply(id, true);
        }

        public void OnClick_CancelApply()
        {
            //if (Sys_PKCompetition.Instance.GetMemState(Sys_Role.Instance.Role.RoleId) == Sys_PKCompetition.MemberState.Enum_Passed)
            //{
            //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1001004));//  战队队长已通过申请，不能取消
            //    return;
            //}
            Sys_PKCompetition.Instance.Req_CancelApply();
        }

        public void OnClick_FightDissolve()
        {
            Sys_PKCompetition.Instance.OpenSureTip(LanguageHelper.GetTextContent(1001023), ()=> 
            {
                Sys_PKCompetition.Instance.Req_FightDissolve();
            });
        }

        public void OnClick_FightSign()
        {
            if (Sys_PKCompetition.Instance.GetMemNum() < Sys_PKCompetition.Instance.MinMemNum)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1001014));
                return;
            }
            Sys_PKCompetition.Instance.OpenSureTip(LanguageHelper.GetTextContent(1001024), () =>
            {
                Sys_PKCompetition.Instance.Req_SignUp();
            });
        }


        public void OnClick_Search()
        {
            ulong memid = 0;
            _searchData.Clear();
            if (ulong.TryParse(layout.t_input.text, out memid))
            {
                if (Sys_PKCompetition.Instance._memberInfoDic.ContainsKey(memid))
                {
                    _searchData.Add(memid);
                    _tempList = _searchData;
                    RefreshInfinityGrid();
                    return;
                }
            }
            _tempList = Sys_PKCompetition.Instance._memberInfoList;
            RefreshInfinityGrid();
            if (!string.IsNullOrEmpty(layout.t_input.text))
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1001005));//  "请输入正确的成员编号"
        }
        #endregion
    }

    public class TeamMemberCell
    {
        #region 界面组件
        private Text t_Number, t_Level, t_Name, t_Score, t_TeamScore, t_Job;
        public GameObject state1, state2, state3, state4;
        public Button btn_allow, btn_refuse;
        #endregion

        Sys_PKCompetition.MemberInfo _data;
        Action<ulong> allowAction, refuseAction;

        #region 初始化
        internal void Init(RectTransform mRootTransform)
        {
            t_Number = mRootTransform.transform.Find("Text_Number").GetComponent<Text>();
            t_Level = mRootTransform.transform.Find("Text_Level").GetComponent<Text>();
            t_Name = mRootTransform.transform.Find("Text_Name").GetComponent<Text>();
            t_Score = mRootTransform.transform.Find("Text_Point1").GetComponent<Text>();
            t_TeamScore = mRootTransform.transform.Find("Text_Point2").GetComponent<Text>();
            t_Job = mRootTransform.transform.Find("Text_Profession").GetComponent<Text>();
            state1 = mRootTransform.transform.Find("State1").gameObject;
            state2 = mRootTransform.transform.Find("State2").gameObject;
            state3 = mRootTransform.transform.Find("State3").gameObject;
            state4 = mRootTransform.transform.Find("State4").gameObject;
            state1.SetActive(false);
            state4.SetActive(false);
            state2.SetActive(false);
            state3.SetActive(false);
            btn_allow = state1.transform.Find("Btn_01").GetComponent<Button>();
            btn_refuse = state1.transform.Find("Btn_02").GetComponent<Button>();

            btn_allow.onClick.AddListener(OnClick_Allow);
            btn_refuse.onClick.AddListener(OnClick_Refuse);
        }

        internal void RefreshData(ulong memberid)
        {
            Sys_PKCompetition.Instance._memberInfoDic.TryGetValue(memberid, out _data);
            if (_data != null)
            {
                UpdateMemberInfo();
            }
        }
        #endregion


        void UpdateMemberInfo()
        {
            t_Number.text = _data.role.RoleId.ToString();
            t_Level.text = _data.role.Level.ToString();
            t_Name.text = _data.role.RoleName.ToString();
            t_Score.text = _data.role.RolePower.ToString();
            t_TeamScore.text = _data.role.Power.ToString();

            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(_data.role.Job);
            t_Job.text = null == cSVCareerData ? string.Empty : LanguageHelper.GetTextContent(cSVCareerData.name);

            state1.SetActive(false);
            state2.SetActive(false);
            state3.SetActive(false);
            state4.SetActive(false);
            if (Sys_PKCompetition.Instance.IsFightTeamSignSuccess())
            {
                state2.SetActive(true);
                return;
            }
            bool isLeader = Sys_PKCompetition.Instance.IsFightTeamLeader();
            switch (_data.state)
            {
                case Sys_PKCompetition.MemberState.Enum_Checking:
                    state1.SetActive(isLeader);
                    state4.SetActive(!isLeader);
                    break;
                case Sys_PKCompetition.MemberState.Enum_Refused:
                    state3.SetActive(true);
                    break;
                case Sys_PKCompetition.MemberState.Enum_Passed:
                    state2.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        #region Action
        internal void AddAction(Action<ulong> onClick_Allow, Action<ulong> onClick_Refuse)
        {
            this.allowAction = onClick_Allow;
            this.refuseAction = onClick_Refuse;
        }
        private void OnClick_Refuse()
        {
            refuseAction?.Invoke(_data.role.RoleId);
        }

        private void OnClick_Allow()
        {
            allowAction?.Invoke(_data.role.RoleId);
        }
        #endregion

    }
}