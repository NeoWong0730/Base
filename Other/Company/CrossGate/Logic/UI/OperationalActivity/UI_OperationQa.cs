using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;

namespace Logic
{
    /// <summary> 调查问卷 </summary>
    public class UI_OperationQa : UI_OperationalActivityBase
    {
        private Button m_ClickButton;

        protected override void Loaded()
        {
            //m_ClickButton = transform.Find("Btn_01").GetComponent<Button>();
            //m_ClickButton.onClick.AddListener(OnClicked);
        }
        protected override void InitBeforOnShow()
        {
            m_ClickButton = transform.Find("Btn_01").GetComponent<Button>();
            m_ClickButton.onClick.AddListener(OnClicked);
        }
        public override void Show()
        {
            base.Show();
            Sys_Qa.Instance.hasShowRedPoint = true;
            Sys_Qa.Instance.eventEmitter.Trigger(Sys_Qa.EEvents.OnRefreshQARedPoint);
        }

        private void OnClicked()
        {
            Sys_Qa.Instance.CheckConditionVaild();
            UIManager.HitButton(EUIID.UI_OperationalActivity, "GoQa", EOperationalActivity.Qa.ToString());
        }
    }
}