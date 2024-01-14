using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;
using static Packet.CmdCookPrepareConfirmNtf.Types;
using Packet;

namespace Logic
{
    public class UI_Cooking_Loading : UIBase
    {
        private bool m_IsCaptain;
        private Button m_ButtonCancel;
        private Button m_ButtonConfirm;
        private GameObject m_CaptainWaitTips;
        private Transform m_Parent;
        private Timer m_Timer;
        private int m_RemainTime;
        private Text m_RemainTimeText;

        protected override void OnLoaded()
        {
            m_ButtonCancel = transform.Find("Animator/Btn_02").GetComponent<Button>();
            m_ButtonConfirm = transform.Find("Animator/Btn_01").GetComponent<Button>();
            m_Parent = transform.Find("Animator/Rectlist").gameObject.transform;
            m_CaptainWaitTips = transform.Find("Animator/Bg/Text_Wait").gameObject;
            m_RemainTimeText = m_ButtonCancel.transform.Find("Text").GetComponent<Text>();
            m_ButtonCancel.onClick.AddListener(OnCancelClicked);
            m_ButtonConfirm.onClick.AddListener(OnConfirmClicked);
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Cooking.Instance.eventEmitter.Handle(Sys_Cooking.EEvents.OnUpdateCookPrepareConfirmState, OnUpdateCookPrepareConfirmState, toRegister);
        }

        protected override void OnShow()
        {
            m_IsCaptain = Sys_Team.Instance.isCaptain(Sys_Role.Instance.RoleId);
            m_RemainTime = (int)CSVCookAttr.Instance.GetConfData(7).value;
            m_Timer?.Cancel();
            m_Timer = Timer.Register(m_RemainTime, OnTimerCompleteCallback, OnTimerUpdateCallback);
            Refresh();
        }

        protected override void OnHide()
        {
            m_Timer?.Cancel();
        }


        private void Refresh()
        {
            m_ButtonCancel.gameObject.SetActive(!m_IsCaptain);
            m_ButtonConfirm.gameObject.SetActive(!m_IsCaptain);
            m_CaptainWaitTips.gameObject.SetActive(m_IsCaptain);

            for (int i = 0; i < Sys_Cooking.Instance.cookPrepareMenber.Mems.Count; i++)
            {
                CmdCookPrepareNtf.Types.Member member = Sys_Cooking.Instance.cookPrepareMenber.Mems[i];
                TeamMem teamMem = Sys_Team.Instance.getTeamMem(member.RoleId);

                //fahsionIcon
                uint fashionClothesId = 0;
                for (int j = 0; j < teamMem.FashionList.Count; j++)
                {
                    uint fashionId = teamMem.FashionList[j].FashionId;
                    CSVFashionClothes.Data cSVFashionClothesData = CSVFashionClothes.Instance.GetConfData(fashionId);
                    if (cSVFashionClothesData != null)
                    {
                        fashionClothesId = fashionId;
                        break;
                    }
                }
                if (fashionClothesId != 0)
                {
                    fashionClothesId = fashionClothesId * 10000 + teamMem.HeroId;
                    CSVFashionIcon.Data cSVFashionIconData = CSVFashionIcon.Instance.GetConfData(fashionClothesId);
                    if (cSVFashionIconData != null)
                    {
                        Image imgRole = m_Parent.GetChild(i).Find("Image_player").GetComponent<Image>();
                        ImageHelper.SetIcon(imgRole, null, cSVFashionIconData.Icon_Path, false);
                        imgRole.transform.localScale = new Vector3(cSVFashionIconData.CompeteIcon_scale, cSVFashionIconData.CompeteIcon_scale, 1);
                        (imgRole.transform as RectTransform).anchoredPosition = new Vector2(cSVFashionIconData.CompeteIcon_pos[0], cSVFashionIconData.CompeteIcon_pos[1]);
                    }
                }

                //name
                Text name_Text = m_Parent.GetChild(i).Find("Name").GetComponent<Text>();
                bool isSelf = member.RoleId == Sys_Role.Instance.RoleId;
                uint nameStytle = isSelf ? (uint)76 : (uint)77;
                CSVWordStyle.Data cSVWordStyleData = CSVWordStyle.Instance.GetConfData(nameStytle);
                TextHelper.SetText(name_Text, teamMem.Name.ToStringUtf8(), cSVWordStyleData);

                //statewait
                GameObject stateWait = m_Parent.GetChild(i).Find("Stage").gameObject;
                stateWait.SetActive(member.State == -1);
                GameObject stateImage_Black = m_Parent.GetChild(i).Find("Image_Black").gameObject;
                stateImage_Black.SetActive(member.State == -1);

                //stateconfirm
                GameObject stateConfirm = m_Parent.GetChild(i).Find("Stage (1)").gameObject;
                stateConfirm.SetActive(member.State == 1);
            }
        }

        private void OnUpdateCookPrepareConfirmState()
        {
            //有人拒绝
            for (int i = 0; i < Sys_Cooking.Instance.cookingMember.Mems.Count; i++)
            {
                CmdCookPrepareConfirmNtf.Types.Member member = Sys_Cooking.Instance.cookingMember.Mems[i];
                if (member.State == 0)
                {
                    TeamMem teamMem = Sys_Team.Instance.getTeamMem(member.RoleId);
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5902, teamMem.Name.ToStringUtf8()));
                    UIManager.CloseUI(EUIID.UI_Cooking_Loading);
                    return;
                }
            }
            //全部确定
            bool allMemberConfirm = true;
            for (int i = 0; i < Sys_Cooking.Instance.cookingMember.Mems.Count; i++)
            {
                CmdCookPrepareConfirmNtf.Types.Member member = Sys_Cooking.Instance.cookingMember.Mems[i];
                if (member.State == -1)
                {
                    allMemberConfirm = false;
                }
            }
            if (allMemberConfirm)
            {
                UIManager.CloseUI(EUIID.UI_Cooking_Loading);
                UIManager.OpenUI(EUIID.UI_Cooking_Multiple);
                return;
            }
            //等待
            for (int i = 0; i < Sys_Cooking.Instance.cookingMember.Mems.Count; i++)
            {
                CmdCookPrepareConfirmNtf.Types.Member member = Sys_Cooking.Instance.cookingMember.Mems[i];
                TeamMem teamMem = Sys_Team.Instance.getTeamMem(member.RoleId);
                //statewait
                GameObject stateWait = m_Parent.GetChild(i).Find("Stage").gameObject;
                stateWait.SetActive(member.State == -1);
                GameObject stateImage_Black = m_Parent.GetChild(i).Find("Image_Black").gameObject;
                stateImage_Black.SetActive(member.State == -1);

                //stateconfirm
                GameObject stateConfirm = m_Parent.GetChild(i).Find("Stage (1)").gameObject;
                stateConfirm.SetActive(member.State == 1);
            }
        }

        private void OnTimerCompleteCallback()
        {
            if (m_IsCaptain)
                return;
            UIManager.CloseUI(EUIID.UI_Cooking_Loading);
            Sys_Cooking.Instance.PrepareConfirmOpReq(0);
        }

        private void OnTimerUpdateCallback(float dt)
        {
            int remainTime = m_RemainTime - (int)dt;
            TextHelper.SetText(m_RemainTimeText, LanguageHelper.GetTextContent(1003037, remainTime.ToString()));
        }

        private void OnCancelClicked()
        {
            UIManager.CloseUI(EUIID.UI_Cooking_Loading);
            Sys_Cooking.Instance.PrepareConfirmOpReq(0);
        }

        private void OnConfirmClicked()
        {
            Sys_Cooking.Instance.PrepareConfirmOpReq(1);
        }
    }
}

