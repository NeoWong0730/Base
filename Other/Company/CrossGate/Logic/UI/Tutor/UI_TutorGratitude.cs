using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_TutorGratitude : UIBase
    {
        #region 组件
        Button closeBtn;
        Button selectBtn_1;
        Button selectBtn_2;
        Button selectBtn_3;
        Text title;
        Text textBtn_1;
        Text textBtn_2;
        Text textBtn_3;
        #endregion
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
            SetContent();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {

        }
        #endregion

        private void OnParseComponent()
        {
            closeBtn = transform.Find("Animator/View_TipsBgNew03/Btn_Close").GetComponent<Button>();
            selectBtn_1 = transform.Find("Animator/Content/BtnGroup/Btn_1").GetComponent<Button>();
            selectBtn_2 = transform.Find("Animator/Content/BtnGroup/Btn_2").GetComponent<Button>();
            selectBtn_3 = transform.Find("Animator/Content/BtnGroup/Btn_3").GetComponent<Button>();
            title=transform.Find("Animator/Content/Title/Text_Title").GetComponent<Text>();
            textBtn_1=selectBtn_1.transform.Find("Text").GetComponent<Text>();
            textBtn_2=selectBtn_2.transform.Find("Text").GetComponent<Text>();
            textBtn_3=selectBtn_3.transform.Find("Text").GetComponent<Text>();

            closeBtn.onClick.AddListener(()=> { CloseSelf(); });
            selectBtn_1.onClick.AddListener(()=> { OnSelectClick(8102); });
            selectBtn_2.onClick.AddListener(()=> { OnSelectClick(8103); });
            selectBtn_3.onClick.AddListener(()=> { OnSelectClick(8104); });
        }
        private void SetContent()
        {
            title.text = LanguageHelper.GetTextContent(8101);
            textBtn_1.text = LanguageHelper.GetTextContent(8102);
            textBtn_2.text = LanguageHelper.GetTextContent(8103);
            textBtn_3.text = LanguageHelper.GetTextContent(8104);
        }
        private void OnSelectClick(uint languageId)
        {
            string content = LanguageHelper.GetTextContent(languageId);
            Sys_Chat.Instance.SendContent(Packet.ChatType.Team, content);
            CloseSelf();
        }
    }
}