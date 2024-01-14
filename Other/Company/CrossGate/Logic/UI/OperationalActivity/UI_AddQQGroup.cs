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
    /// <summary> 手机绑定 </summary>
    public class UI_AddQQGroup : UI_OperationalActivityBase
    {

        private Button btnAddGroup;

        protected override void Loaded()
        {
            btnAddGroup = transform.Find("Button").GetComponent<Button>();
            btnAddGroup.onClick.AddListener(OnClickAddGroup);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            //Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdatePhoneBindStatus, OnUpdatePhoneBindStatus, toRegister);
        }

        private void OnClickAddGroup()
        {
            int result = SDKManager.SDKjoinQQGroup();
            //未安装QQ的提示
            if (result == -1)
                Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(500000020).words);
            UIManager.HitButton(EUIID.UI_OperationalActivity, "AddQQGroup", EOperationalActivity.AddQQGroup.ToString());
        }

        private void OnUpdatePhoneBindStatus()
        {
            //m_bindingInfo.UpdateInfo();
        }
    }
}