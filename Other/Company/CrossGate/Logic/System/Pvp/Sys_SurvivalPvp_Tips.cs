using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using Net;
using Packet;
using Logic.Core;
using Lib.Core;
using Framework;

namespace Logic
{
    public partial class Sys_SurvivalPvp : SystemModuleBase<Sys_SurvivalPvp>
    {

        private Action m_OnConfirmLeaveAction;

        public void OpenTipsDialog(Action action)
        {
            PromptBoxParameter.Instance.Clear();

            PromptBoxParameter.Instance.SetConfirm(true, OnConfirmLeave, 4913);

            PromptBoxParameter.Instance.SetCancel(true, null, 4914);

            PromptBoxParameter.Instance.SetCountdown(0, PromptBoxParameter.ECountdown.None);

            PromptBoxParameter.Instance.onNoOperator = null;

            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2022439);

            UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);

            m_OnConfirmLeaveAction = action;
        }

        private void OnConfirmLeave()
        {
            //if (isMatching)
            //{
            //    SendCancleMatch();
            //}
            m_OnConfirmLeaveAction?.Invoke();

            m_OnConfirmLeaveAction = null;


        }

        public bool OpenTips(Action action)
        {
            if (isSurvivalPvpMap(Sys_Map.Instance.CurMapId) == false)
                return false;

            OpenTipsDialog(action);

            return true;
        }
    
    }


}
