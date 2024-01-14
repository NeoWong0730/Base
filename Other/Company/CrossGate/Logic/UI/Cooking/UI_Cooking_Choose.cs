using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;
using Packet;

namespace Logic
{
    public class UI_Cooking_Choose : UIBase
    {
        private uint m_CookFunId;
        private CSVCookFunction.Data m_CSVCookFunctionData;
        private Button m_CookingAtlas;
        private Button m_Single;
        private Button m_Multiple;
        private Button m_CloseButton;
        private Text m_TextTop;
        private GameObject m_SingleTips;
        private GameObject m_MultipleTips;
        private GameObject m_RedPoint;

        protected override void OnOpen(object arg)
        {
            m_CookFunId = (uint)arg;
            m_CSVCookFunctionData = CSVCookFunction.Instance.GetConfData(m_CookFunId);
        }

        protected override void OnLoaded()
        {
            m_CookingAtlas = transform.Find("Animator/Button").GetComponent<Button>();
            m_Single = transform.Find("Animator/Image_Single").GetComponent<Button>();
            m_Multiple = transform.Find("Animator/Image_Multiple").GetComponent<Button>();
            m_CloseButton = transform.Find("Animator/View_Title/Btn_Close").GetComponent<Button>();
            m_TextTop = transform.Find("Animator/Text_Top").GetComponent<Text>();
            m_SingleTips = transform.Find("Animator/Text_Single").gameObject;
            m_MultipleTips = transform.Find("Animator/Text_Multiple").gameObject;
            m_RedPoint = transform.Find("Animator/Button/Image_Red").gameObject;

            m_Single.onClick.AddListener(OnSingleButtonClicked);
            m_Multiple.onClick.AddListener(OnMultipleButtonClicked);
            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
            m_CookingAtlas.onClick.AddListener(OnCookingAtlasClicked);
        }

        protected override void OnShow()
        {
            Sys_Cooking.Instance.UpdateRewards();
            Sys_Cooking.Instance.UpdateAllCookingSubmitState();
            UpdateUI();
        }

        private void UpdateUI()
        {
            string toolContent = string.Empty;
            for (int i = 0; i < m_CSVCookFunctionData.allow_tool.Count; i++)
            {
                string tool = Sys_Cooking.Instance.GetToolName(m_CSVCookFunctionData.allow_tool[i]) + " ";
                toolContent += tool;
            }
            TextHelper.SetText(m_TextTop, LanguageHelper.GetTextContent(1003070, toolContent));
            bool showSingleTips = m_CSVCookFunctionData.allow_type.Count == 1 && m_CSVCookFunctionData.allow_type[0] == 1;
            m_SingleTips.SetActive(showSingleTips);
            bool showMultipleTips = !m_CSVCookFunctionData.allow_type.Contains(3);
            m_MultipleTips.SetActive(showMultipleTips);
            ButtonHelper.Enable(m_Multiple, !showMultipleTips);
            m_RedPoint.SetActive(Sys_Cooking.Instance.HasSubmitItem(0) || Sys_Cooking.Instance.HasReward());
        }

        private void OnCookingAtlasClicked()
        {
            UIManager.OpenUI(EUIID.UI_Knowledge_Cooking);
        }

        private void OnSingleButtonClicked()
        {
            if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain(Sys_Role.Instance.RoleId) && !Sys_Team.Instance.isPlayerLeave())
            {
                string content = LanguageHelper.GetTextContent(5937);
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }
            Sys_Cooking.Instance.CookPrepareReq(false);
            OpenCookingSingleParm openCookingSingleParm = new OpenCookingSingleParm();
            openCookingSingleParm.cookFunId = m_CookFunId;
            openCookingSingleParm.cookId = 0;
            UIManager.OpenUI(EUIID.UI_Cooking_Single, false, openCookingSingleParm);
        }

        private void OnMultipleButtonClicked()
        {
            if (!Sys_Team.Instance.HaveTeam)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5920));
            }
            else if (!Sys_Team.Instance.isCaptain(Sys_Role.Instance.RoleId))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5920));
                //不是队长
            }
            else if (Sys_Team.Instance.TeamMemsCount < 3)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5920));
                //队伍人员数量不足三人
            }
            else if (!Sys_Cooking.Instance.Free3StageValid())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5931));
            }
            else
            {
                TeamMem teamMem1 = Sys_Team.Instance.getTeamMem(1);
                TeamMem teamMem2 = Sys_Team.Instance.getTeamMem(2);
                if (teamMem1 != null && (teamMem1.IsLeave() || teamMem1.IsOffLine()))
                {
                    //teamMem1暂离
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5903, teamMem1.Name.ToStringUtf8()));
                    return;
                }
                if (teamMem2 != null && (teamMem2.IsLeave() || teamMem2.IsOffLine()))
                {
                    //teamMem2暂离
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5903, teamMem2.Name.ToStringUtf8()));
                    return;
                }
                Sys_Cooking.Instance.CookPrepareReq(true);
            }
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Cooking_Choose);
        }

    }
}


