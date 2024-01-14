using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_TerroristWeek : UIBase
    {
        #region UI
        private Text m_TextName;
        private Button m_BtnClose;

        private UI_TerroristWeek_Info m_Info;
        private UI_TerroristWeek_Single m_Single;
        private UI_TerroristWeek_Team m_Team;

        private Button m_BtnEnter;
        private Button m_BtnForTeam;

        //private 
        #endregion

        private uint m_InstanceId;
        private CSVInstance.Data m_InstanceData;

        //protected virtual void OnInit() { }
        protected override void OnOpen(object arg)
        {
            m_InstanceId = 0;
            if (arg != null)
            {
                m_InstanceId = (uint)arg;
            }

            m_InstanceData = CSVInstance.Instance.GetConfData(m_InstanceId);
        }

        protected override void OnLoaded()
        {
            m_TextName = transform.Find("Animator/View_Title02/Item Label").GetComponent<Text>();
            m_BtnClose = transform.Find("Animator/View_Title02/Btn_Close").GetComponent<Button>();

            m_Info = AddComponent<UI_TerroristWeek_Info>(transform.Find("Animator/Image_Leftbg"));
            m_Single = AddComponent<UI_TerroristWeek_Single>(transform.Find("Animator/Image_single"));
            m_Team = AddComponent<UI_TerroristWeek_Team>(transform.Find("Animator/Image_team"));

            m_BtnForTeam = transform.Find("Animator/Btn_forteam").GetComponent<Button>();
            m_BtnEnter = transform.Find("Animator/Btn_start").GetComponent<Button>();

            m_BtnClose.onClick.AddListener(() => { UIManager.CloseUI(EUIID.UI_TerroristWeek); });
            m_BtnForTeam.onClick.AddListener(OnClickForTeam);
            m_BtnEnter.onClick.AddListener(OnClickEnterIns);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_TerrorSeries.Instance.eventEmitter.Handle<TerrorSeriesMemItems>(Sys_TerrorSeries.EEvents.OnNtfWeekTaskMemInfo, OnNtfMemInfo, toRegister);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            //组队信息变更
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.NetMsg_InfoNtf, OnTeamMemChange, toRegister);
        }

        private void OnTeamMemChange()
        {
            //重新请求
            Sys_TerrorSeries.Instance.OnWeekTaskReq(m_InstanceId);
        }

        private void OnClickForTeam()
        {
            if (Sys_Team.Instance.IsFastOpen(true))
                Sys_Team.Instance.OpenFastUI(m_InstanceData.TeamID);
        }

        private void OnClickEnterIns()
        {
            if (null == m_InstanceData)
            {
                Debug.LogErrorFormat("副本表里，没有配置副本 id ={0}", m_InstanceId);
                return;
            }

            uint stageId = 0;
            foreach (var data in CSVInstanceDaily.Instance.GetAll())
            {
                if (data.InstanceId == m_InstanceId && data.Layerlevel == 1u)
                {
                    stageId = data.id;
                    break;
                }
            }

            if (stageId != 0)
            {
                Sys_Instance.Instance.InstanceEnterReq(m_InstanceId, stageId);
            }
        }

        private void UpdateInfo()
        {
            if (null == m_InstanceData)
            {
                Debug.LogErrorFormat("副本表里，没有配置副本 id ={0}", m_InstanceId);
                return;
            }

            m_TextName.text = LanguageHelper.GetTextContent(1006044, (m_InstanceData.lv[0]).ToString(), LanguageHelper.GetTextContent(m_InstanceData.Name));

            m_Info.UpdateInfo(m_InstanceData);

            m_Single.Hide();
            m_Team.Hide();

            Sys_TerrorSeries.Instance.OnWeekTaskReq(m_InstanceId);
        }

        private void OnNtfMemInfo(TerrorSeriesMemItems mems)
        {
            bool isSelf = mems.Mems.Count == 1 && mems.Mems[0].RoleId == Sys_Role.Instance.RoleId; //TODO: 判断队伍是否只有自己
            if (isSelf)
            {
                m_Team.Hide();
                m_Single.Show();
                m_Single.UpdateInfo(mems.Mems[0]);
            }
            else
            {
                m_Single.Hide();
                m_Team.Show();
                m_Team.UpdateInfo(mems);
            }
        }
    }
}


