using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;

namespace Logic
{
    public class UI_PowerSaving : UIBase
    {
        private Button btnClose;

        #region 系统函数 
        protected override void OnLoaded()
        {
            Init();
        }       
        protected override void ProcessEvents(bool toRegister)
        {            
            Sys_PowerSaving.Instance.eventEmitter.Handle(Sys_PowerSaving.EEvents.OnQuitPowerSaving, OnQuitPowerSaving, toRegister);
        }
        #endregion

        private void Init()
        {
            btnClose = transform.Find("Animator/Button_Blank").GetComponent<Button>();
            btnClose.onClick.AddListener(OnCloseClick);
        }

        private void OnCloseClick()
        {
            this.CloseSelf();
        }
        private void OnQuitPowerSaving()
        {
            this.CloseSelf();
        }
    }
}
