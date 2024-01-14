using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_PKCompetition : UIBase
    {
        #region 界面组件
        private UI_PKTeamList pkTeamListView;
        private UI_PKTeamLeader pkTeamLeaderView;
        private UI_CurrencyTitle currency;

        private Transform tran_teamlist, tran_teamleader;
        private Button Btn_Close;
        private Text title;
        #endregion


        #region 系统函数
        protected override void OnLoaded()
        {
            tran_teamlist = transform.Find("Animator/Team_List");
            tran_teamleader = transform.Find("Animator/Team_Leader");
            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            Btn_Close = transform.Find("Animator/View_Title02/Btn_Close").GetComponent<Button>();
            pkTeamListView = AddComponent<UI_PKTeamList>(tran_teamlist);
            pkTeamLeaderView = AddComponent<UI_PKTeamLeader>(tran_teamleader);
            Btn_Close.onClick.AddListener(() => CloseSelf());
            title = transform.Find("Animator/Image_Title/Text").GetComponent<Text>();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_PKCompetition.Instance.eventEmitter.Handle(Sys_PKCompetition.EEvents.Event_UpdateView, UpdateView, toRegister);
            Sys_PKCompetition.Instance.eventEmitter.Handle(Sys_PKCompetition.EEvents.Event_TimeOver, CloseView, toRegister);
        }

        protected override void OnOpen(object arg)
        {
            Sys_PKCompetition.Instance.Req_Info();
        }
        protected override void OnShow()
        {
            Init();
            currency.InitUi();
            UpdateView();
        }

        protected override void OnHide()
        {
            pkTeamListView.Hide();
            pkTeamLeaderView.Hide();
        }

        protected override void OnDestroy()
        {
            currency.Dispose();
            Sys_PKCompetition.Instance.ClearData();
        }
        #endregion


        private void Init()
        {
            uint languageId = CSVPKMatch.Instance.GetConfData(Sys_PKCompetition.Instance.MatchID).matchName;                
            title.text = LanguageHelper.GetTextContent(languageId);
        }

        private void UpdateView()
        {
            if (Sys_PKCompetition.Instance.JoinTeamID == 0)
            {
                pkTeamListView.Show();
                pkTeamLeaderView.Hide();
            }
            else
            {
                pkTeamListView.Hide();
                pkTeamLeaderView.Show();
            }
        }

        private void CloseView()
        {
            CloseSelf();
        }

    }

    public class UI_PKCompetition_TeamWords: UIBase
    {
        private Text title;
        private Button btn_close;

        string content;
        protected override void OnLoaded()
        {
            title = transform.Find("Animator/View_Tips/bg/Name").GetComponent<Text>();
            btn_close = transform.Find("Blank").GetComponent<Button>();
            btn_close.onClick.AddListener(() => CloseSelf());
        }

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                content = arg.ToString();
            }
            
        }
        protected override void OnShow()
        {
            if (title != null)
                title.text = content;
        }
    }
}