using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    public class UI_Family_JoinTips : UIBase
    {
        private Button closeBtn;
        private Button openFamilyListBtn;
        private Button oneKeyApplyBtn;
        protected override void OnLoaded()
        {
            closeBtn = transform.Find("Animator/View_TipsBgNew04/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(CloseBtnClicked);
            openFamilyListBtn = transform.Find("Animator/View_Name/Button_Go").GetComponent<Button>();
            openFamilyListBtn.onClick.AddListener(OpenFamilyListBtnClicked);
            oneKeyApplyBtn = transform.Find("Animator/View_Name/Button_Sure").GetComponent<Button>();
            oneKeyApplyBtn.onClick.AddListener(OneKeyApplyBtnClicked);
        }

        protected override void OnShow()
        {
            
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Family_JoinTips);
        }

        public void OpenFamilyListBtnClicked()
        {
            Sys_Family.Instance.OpenUI_Family();
            CloseBtnClicked();
        }

        public void OneKeyApplyBtnClicked()
        {
            if (!Sys_Family.Instance.CanJoinFamily(true))
            {
                return;
            }
            
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(10029);
            PromptBoxParameter.Instance.SetConfirm(true, () => 
            {
                Sys_Family.Instance.SendGuildOneKeyApplyReq();
                CloseBtnClicked();
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
        }
    }
}