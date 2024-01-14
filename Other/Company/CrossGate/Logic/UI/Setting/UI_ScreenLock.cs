using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{


    public class UI_ScreenLock : UIBase
    {
        #region 界面组件
        private CP_SliderUnlock lockSlider;
        #endregion

        #region 系统函数
        protected override void OnInit()
        {
        }
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnUpdate()
        {
        }
        protected override void OnOpen(object arg)
        {
        }
        protected override void OnShow()
        {
        }
        protected override void OnHideStart()
        {
        }
        protected override void OnHide()
        {
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }
        #endregion

        #region 初始化
        private void OnParseComponent()
        {
            lockSlider = transform.Find("Animator/Slider").GetComponent<CP_SliderUnlock>();
            lockSlider.onValueChanged.AddListener(OnSliderValueChange);
        }
        #endregion

        #region 响应事件
        private void OnSliderValueChange(float value)
        {
            if (value >= 1)
            {
                Close();
            }
        }
        private void Close()
        {
            UIManager.CloseUI(EUIID.UI_ScreenLock);
        }
        #endregion
    }

}
