using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    public class UI_Family_Sure : UIBase
    {
        protected override void OnLoaded()
        {
            Button closeBtn = transform.Find("Animator/View_TipsBg02_Smallest/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(CloseBtnClicked);
            Button sureBtn = transform.Find("Animator/View_Content/Button_Sure").GetComponent<Button>();
            sureBtn.onClick.AddListener(CloseBtnClicked);
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Family_Sure);
        }

    }
}