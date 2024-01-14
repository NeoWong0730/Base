using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Terrorist : UIBase
    {
        #region UI
        private Button m_BtnClose;
        private Text m_TextName;
        private Text m_TextDescription;
        private Button m_BtnGoWeek;

        private Button m_BtnTeam;
        private Button m_BtnStart;
        private Button m_BtnGiveUp;

        private UI_Terrorist_LineList m_LineList;
        private UI_Terrorist_LineInfo m_LineInfo;
        //private 
        #endregion
        private uint m_TerrorId;
        private CSVTerrorSeries.Data m_TerrorData;
        private int m_Line;

        //protected virtual void OnInit() { }
        protected override void OnOpen(object arg)
        {
            m_TerrorId = 0;
            m_TerrorData = null;
            if (arg != null)
            {
                m_TerrorId = (uint)arg;
            }

            m_TerrorData = CSVTerrorSeries.Instance.GetConfData(m_TerrorId);
        }
        protected override void OnLoaded()
        {
            m_BtnClose = transform.Find("Animator/View_Title02/Btn_Close").GetComponent<Button>();
            m_BtnClose.onClick.AddListener(() => { UIManager.CloseUI(EUIID.UI_Terrorist); });

            m_TextName = transform.Find("Animator/View_Title02/Item Label").GetComponent<Text>();
            m_TextDescription = transform.Find("Animator/GameObject/Image_explain/Text").GetComponent<Text>();
            m_BtnGoWeek = transform.Find("Animator/GameObject/Button_tips").GetComponent<Button>();

            m_LineList = AddComponent<UI_Terrorist_LineList>(transform.Find("Animator/Image_Leftbg"));
            m_LineInfo = AddComponent<UI_Terrorist_LineInfo>(transform.Find("Animator/Image_Rightbg"));

            m_BtnTeam = transform.Find("Animator/Btn_forteam").GetComponent<Button>();
            m_BtnStart = transform.Find("Animator/Btn_start").GetComponent<Button>();
            m_BtnGiveUp = transform.Find("Animator/Btn_end").GetComponent<Button>();

            m_BtnGoWeek.onClick.AddListener(OnClickGoWeek);
            m_BtnTeam.onClick.AddListener(OnClickTeam);
            m_BtnStart.onClick.AddListener(OnClickStart);
            m_BtnGiveUp.onClick.AddListener(OnClickGiveUp);
        }
        //protected virtual void OnUpdate() { }
        //protected virtual void OnLateUpdate() { }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_TerrorSeries.Instance.eventEmitter.Handle<int>(Sys_TerrorSeries.EEvents.OnSelectLine, OnSelectLine, toRegister);
            Sys_TerrorSeries.Instance.eventEmitter.Handle(Sys_TerrorSeries.EEvents.OnUpdateTaskData, OnUpdateTaskData, toRegister);
        }

        private void OnClickGoWeek()
        {
            UIManager.CloseUI(EUIID.UI_Terrorist);
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(1170921); //策划说写死npcid
            //ActionCtrl.Instance.MoveToTargetNPC();
        }

        private void OnClickTeam()
        {
            if (Sys_Team.Instance.IsFastOpen(true))
                Sys_Team.Instance.OpenFastUI(m_TerrorData.TeamTargetID);
        }

        private void OnClickStart()
        {
            if (null == m_TerrorData)
            {
                Debug.LogError("OnClickStart");
                return;
            }

            if (!Sys_Team.Instance.HaveTeam)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(103907, m_TerrorData.limite_number.ToString()));
                return;
            }

            if (Sys_TerrorSeries.Instance.IsDailyTaskComplete(m_TerrorData.id))
            {
                PromptBoxParameter.Instance.OpenPromptBox(1006056, 0, () => {
                    Sys_TerrorSeries.Instance.OnDailyTaskAccept(m_TerrorData.id, (uint)m_Line);
                    UIManager.CloseUI(EUIID.UI_Terrorist);
                });
            }
            else
            {
                Sys_TerrorSeries.Instance.OnDailyTaskAccept(m_TerrorData.id, (uint)m_Line);
                UIManager.CloseUI(EUIID.UI_Terrorist);
            }
        }

        private void OnClickGiveUp()
        {
            if (null == m_TerrorData)
            {
                Debug.LogError("OnClickGiveUp");
                return;
            }

            PromptBoxParameter.Instance.OpenPromptBox(1006055, 0, () => {
                Sys_TerrorSeries.Instance.OnDailyTaskGiveUpReq(m_TerrorData.id);
            });
        }

        private void UpdateInfo()
        {
            if (null == m_TerrorData)
            {
                Debug.LogErrorFormat("恐怖旅人副本表里，没有配置副本 id ={0}", m_TerrorId);
                return;
            }

            //CSVInstance.Data instance = CSVInstance.Instance.GetConfData(m_TerrorId);
            //m_TextLevel.text = LanguageHelper.GetTextContent(1006044, instance.lv[0], LanguageHelper.GetTextContent(instance.Name));
            m_TextName.text = LanguageHelper.GetTextContent(m_TerrorData.Name);

            m_TextDescription.text = LanguageHelper.GetTextContent(m_TerrorData.instance_des);

            m_LineList.UpdateLineList(m_TerrorData);

            if (Sys_TerrorSeries.Instance.IsDailyTaskLineSelected(m_TerrorData.id))
            {
                m_BtnGiveUp.gameObject.SetActive(true);
                m_BtnStart.gameObject.SetActive(false);
            }
            else
            {
                m_BtnGiveUp.gameObject.SetActive(false);
                m_BtnStart.gameObject.SetActive(true);
            }
        }

        #region Ntf
        private void OnSelectLine(int line)
        {
            m_Line = line;
            m_LineInfo.UpdateInfo(m_TerrorData, m_Line);
        }

        private void OnUpdateTaskData()
        {
            UpdateInfo();
        }
        #endregion
        //protected virtual void OnShowEnd() { }
        //protected virtual void OnHideStart() { }
        //protected override void OnHide()
        //{

        //}
        //protected virtual void OnClose() { }
        //protected virtual void OnDestroy() { }
        //protected virtual void ProcessEvents(bool toRegister) { }
        //protected virtual void ProcessEventsForEnable(bool toRegister) { }
    }
}


