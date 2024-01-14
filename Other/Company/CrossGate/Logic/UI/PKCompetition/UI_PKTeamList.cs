using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{
    public class UI_PKTeamList_Layout
    {
        public Transform transform;
        public Text t_Time, t_Num;
        public Transform itemParent;
        public InfinityGrid _infinityGrid;
        public Button btn_Search, btn_Create, btn_MyTeam;
        public InputField t_input;

        public void Init(Transform transform)
        {
            this.transform = transform;
            t_Time = transform.Find("Image_Time/Text_time").GetComponent<Text>();
            t_Num = transform.Find("Image_Num/Text_time").GetComponent<Text>();
            btn_Search= transform.Find("InputField_Describe/Button_Search").GetComponent<Button>();
            t_input = transform.Find("InputField_Describe").GetComponent<InputField>();
            btn_MyTeam = transform.Find("Btn_02").GetComponent<Button>();
            btn_Create= transform.Find("Btn_01").GetComponent<Button>();

            _infinityGrid = transform.Find("ScrollView").GetComponent<InfinityGrid>();
            btn_MyTeam.gameObject.SetActive(false);

        }
        public void RegisterEvents(IListener listener)
        {
            _infinityGrid.onCreateCell += listener.OnCreateCell;
            _infinityGrid.onCellChange += listener.OnCellChange;

            btn_Search.onClick.AddListener(listener.OnClick_Search);
            btn_Create.onClick.AddListener(listener.OnClick_CreateTeam);
            btn_MyTeam.onClick.AddListener(listener.OnClick_OpenMyTeam);

            Lib.Core.EventTrigger.Get(_infinityGrid.ScrollView.gameObject).onDrag = listener.onDrag;
        }

        public interface IListener
        {
            void OnCellChange(InfinityGridCell arg1, int arg2);
            void OnCreateCell(InfinityGridCell obj);
            void OnClick_Search();
            void OnClick_OpenMyTeam();
            void OnClick_CreateTeam();
            void onDrag(GameObject go, Vector2 delta);
        }
    }

    public class UI_PKTeamList : UIComponent, UI_PKTeamList_Layout.IListener
    {
        private UI_PKTeamList_Layout layout = new UI_PKTeamList_Layout();

        private List<Sys_PKCompetition.TeamInfo> _tempList;
        private bool isQuerying = false;

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
            Init();
        }
        public override void Hide()
        {
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_PKCompetition.Instance.eventEmitter.Handle(Sys_PKCompetition.EEvents.Event_SearchSuccess, Event_SearchSuccess, toRegister);
            base.ProcessEventsForEnable(toRegister);
        }
        #endregion
        #region Events
        private void Event_SearchSuccess()
        {
            _tempList = Sys_PKCompetition.Instance.SearchData;
            RefreshInfinityGrid();
        }
        #endregion
        #region 界面显示
        private void Init()
        {
            _tempList = Sys_PKCompetition.Instance._teamInfoList;
            RefreshInfinityGrid();
            RefreshPkInfo();
        }
        private void RefreshPkInfo()
        {
            layout.t_Num.text = Sys_PKCompetition.Instance.SignUpSucessCount.ToString();
            layout.t_Time.text = LastTimeShow();
        }

        private string LastTimeShow()
        {
            DateTime _start = DateTime.Now;
            DateTime _end = DateTime.Now;
            Sys_PKCompetition.Instance.ReturnTime(ref _start, ref _end);
            string _str = GetTextContent(_start.Year.ToString(), _start.Month.ToString(), _start.Day.ToString(), FormatString(_start.Hour), FormatString(_start.Minute), _end.Year.ToString(), _end.Month.ToString(), _end.Day.ToString(), FormatString(_end.Hour), FormatString(_end.Minute));
            return _str;
        }

        public static string GetTextContent(params string[] param)
        {
            return string.Format("{0}/{1}/{2} {3}:{4}-{5}/{6}/{7} {8}:{9} ", param);
        }

        private string FormatString(int _time)
        {
            return string.Format("{0:00}", _time);
        }
        #endregion


        private void RefreshInfinityGrid()
        {
            isQuerying = false;
            layout._infinityGrid.CellCount = _tempList.Count;
            layout._infinityGrid.ForceRefreshActiveCell();
        }

        public void OnCreateCell(InfinityGridCell cell)
        {
            TeamRankItem entry = new TeamRankItem();
            entry.Init(cell.mRootTransform);
            entry.AddAction(OnClickApply);
            cell.BindUserData(entry);
        }

        public void OnCellChange(InfinityGridCell cell, int index)
        {
            TeamRankItem entry = cell.mUserData as TeamRankItem;
            entry.RefreshData(_tempList[index]);
        }

        #region OnClick
        private void OnClickApply(uint id)
        {
            if (Sys_PKCompetition.Instance.JoinTeamID != 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1001009));//  "已有申请加入的队伍，不能重复加入"
                return;
            }
            Sys_PKCompetition.Instance.Req_ApplyJoinTeam(id);
        }

        public void OnClick_Search()
        {
            uint teamid = 0;
            if (uint.TryParse(layout.t_input.text, out teamid))
            {
                Sys_PKCompetition.Instance.Req_Search(teamid);
                return;
            }
            _tempList = Sys_PKCompetition.Instance._teamInfoList;
            RefreshInfinityGrid();
            if (!string.IsNullOrEmpty(layout.t_input.text))
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1001006));//  "请输入正确的战队ID"
        }

        public void OnClick_OpenMyTeam()
        {
            //废弃
        }

        public void OnClick_CreateTeam()
        {
            if (Sys_PKCompetition.Instance.JoinTeamID != 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1001010));//  "已有申请加入的队伍，不能创建战队"
                return;
            }
            if (Sys_PKCompetition.Instance.IsGetLevelLimite() == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1001021));//  "等级不足，不能创建战队"
                return;
            }
            UIManager.OpenUI(EUIID.UI_PKCompetitionCreate, false, null, EUIID.UI_PKCompetition);
        }

        public void onDrag(GameObject go, Vector2 delta)
        {
            //if (layout._infinityGrid.NormalizedPosition.y <= 0)
            //{
            //    uint curPage = 0;
            //    Sys_PKCompetition.Instance.Req_TeamList(curPage + 1);
            //}
            if (isQuerying) return;
            int index = Sys_PKCompetition.Instance._teamInfoList.Count / Sys_PKCompetition.Instance.OnePageDatasNum;//页数也是index
            var vertical = layout._infinityGrid.ScrollView.verticalNormalizedPosition;//当前所在比例。越接近0越底下
            if (vertical > 1)
            {
                vertical = 0.99f;
            }
            else if (vertical < 0)
            {
                vertical = 0.01f;
            }
            int currentPage = (int)Math.Floor(index * (1 - vertical)); //所在页数     
            var up = delta.y > 0;
            int page = 0;
            if (Sys_PKCompetition.Instance.MaxPage == 0)
                return;
            if (up)
            {
                if (currentPage >= Sys_PKCompetition.Instance.MaxPage - 1)
                    return;
                if (vertical <= 0.2 * (index - currentPage))
                {
                    if (page == currentPage + 1)
                    {
                        return;
                    }
                    isQuerying = true;
                    page = currentPage + 1;
                    Sys_PKCompetition.Instance.Req_TeamList((uint)page);
                }
            }
            else
            {
                if (currentPage <= 0)
                    return;
                if (vertical >= 0.2 * (index - currentPage))
                {
                    if (page == currentPage - 1)
                    {
                        return;
                    }
                    isQuerying = true;
                    page = currentPage - 1;
                    Sys_PKCompetition.Instance.Req_TeamList((uint)page);
                }
            }

        }
        #endregion
    }

    public class TeamRankItem
    {
        #region 界面组件
        private Text t_Number, t_Level, t_Name, t_Content, t_Head, t_point, t_Member;
        private GameObject go_state1, go_state2;
        private Button btn_Join, btn_Content;
        #endregion
        Sys_PKCompetition.TeamInfo _data;
        Action<uint> joinAction;
        internal void Init(RectTransform mRootTransform)
        {
            t_Number = mRootTransform.transform.Find("Text_Number").GetComponent<Text>();
            t_Level = mRootTransform.transform.Find("Text_Level").GetComponent<Text>();
            t_Name = mRootTransform.transform.Find("Text_Name").GetComponent<Text>();
            t_Content = mRootTransform.transform.Find("Text_Content").GetComponent<Text>();
            btn_Content = t_Content.GetComponent<Button>();
            t_Head = mRootTransform.transform.Find("Text_Head").GetComponent<Text>();
            t_point = mRootTransform.transform.Find("Text_Point").GetComponent<Text>();
            t_Member = mRootTransform.transform.Find("Text_Member").GetComponent<Text>();
            go_state1 = mRootTransform.transform.Find("State1").gameObject;
            go_state2 = mRootTransform.transform.Find("State2").gameObject;
            btn_Join = mRootTransform.transform.Find("State1/Btn").GetComponent<Button>();

            btn_Join.onClick.AddListener(OnClick_Join);
            btn_Content.onClick.AddListener(OnClick_Content);
        }

        private void OnClick_Content()
        {
            UIManager.OpenUI(EUIID.UI_PKCompetition_TeamWords, false, _data.team.TeamAnnounce.ToStringUtf8(), EUIID.UI_PKCompetition);
        }


        private void OnClick_Join()
        {
            if (_data != null && _data.team.MemNum >= Sys_PKCompetition.Instance.MaxMemNum)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1001019));
                return;
            }
            if (Sys_PKCompetition.Instance.IsGetLevelLimite() == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1001020));
                return;
            }
            joinAction?.Invoke(_data.team.TeamId);
        }

        internal void AddAction(Action<uint> onClickApply)
        {
            joinAction = onClickApply;
        }

        internal void RefreshData(Sys_PKCompetition.TeamInfo _data)
        {
            this._data = _data;
            if (_data != null)
            {
                UpdateTeamInfo();
                UpdateState();
            }
        }

        void UpdateTeamInfo()
        {
            t_Number.text = _data.team.TeamId.ToString();
            string str = null;
            int Lv = Sys_PKCompetition.Instance.IsGetLevelLimite();
            if (Lv == 1|| Lv==0)
                str = LanguageHelper.GetTextContent(14006);
            else if (Lv == 2)
                str = LanguageHelper.GetTextContent(14007);
            else if (Lv == 3)
                str = LanguageHelper.GetTextContent(14008);
            t_Level.text = str;//_data.team.TeamAnnounce.ToStringUtf8();
            t_Name.text = _data.team.TeamName.ToStringUtf8();
            t_Content.text = _data.team.TeamAnnounce.ToStringUtf8();
            t_Head.text = _data.team.LeaderName.ToStringUtf8();
            t_point.text = _data.team.Power.ToString();
            t_Member.text = string.Format("{0}/{1}", _data.team.MemNum, Sys_PKCompetition.Instance.MaxMemNum);

        }
        void UpdateState()
        {
            if (_data != null)
            {
                go_state1.SetActive(!_data.team.HasSignUp);
                go_state2.SetActive(_data.team.HasSignUp);
            }
        }
    }

}