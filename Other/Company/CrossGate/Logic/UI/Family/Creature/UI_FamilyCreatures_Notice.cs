using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    /// <summary> 家族兽公告 </summary>
    public class UI_FamilyCreatures_Notice : UIBase
    {
        #region 界面组件
        /// <summary> 输入家族兽公告 </summary>
        private InputField inputField_Declaration;
        #endregion
        #region 数据定义
        /// <summary> 公告最大字数 </summary>
        private const int maxLimit_Declaration = 100;
        private Button noticeEditorBtn;
        private Text numText;
        private Text buttonName;
        private bool isEditor = false;
        #endregion
        #region 系统函数
        protected override void OnInit()
        {
        }
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnOpen(object arg)
        {
        }
        protected override void OnOpened()
        {
        }
        protected override void OnShow()
        {
            Sys_Family.Instance.GuildPetGetNoticeReq();
            RefreshView();
        }
        protected override void OnHide()
        {

        }
        protected override void OnUpdate()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent()
        {
            inputField_Declaration = transform.Find("Animator/InputField").GetComponent<InputField>();
            numText = transform.Find("Animator/Text_Number").GetComponent<Text>();
            transform.Find("Image_Black").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            noticeEditorBtn = transform.Find("Animator/Btn_Edit").GetComponent<Button>();
            buttonName = transform.Find("Animator/Btn_Edit/Text_01").GetComponent<Text>();
            noticeEditorBtn.onClick.AddListener(OnClick_Modify);
            inputField_Declaration.characterLimit = 0;
            inputField_Declaration.onValidateInput = OnValidateInput_Declaration;
            inputField_Declaration.onValueChanged.AddListener(OnValueChange);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnFamilyPetNotice, RefreshView, toRegister);
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 重置界面
        /// </summary>
        private void ResetView()
        {
            inputField_Declaration.text = LanguageHelper.GetTextContent(2023330u);
        }
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            Sys_Family.Instance.CheckCreatureNoticeRedPoint();
            bool hasEditor = Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.FamilyPetNotice);
            if(hasEditor)
            {
                inputField_Declaration.interactable = isEditor;
                buttonName.text = LanguageHelper.GetTextContent(isEditor ? 2023413u : 2023323u);
            }
            else
            {
                inputField_Declaration.interactable = hasEditor;
            }
            noticeEditorBtn.gameObject.SetActive(hasEditor);
            if (string.IsNullOrEmpty(Sys_Family.Instance.familyNotic))
            {
                var langId = 2023330u;
                if(!hasEditor)
                {
                    langId = 2023332u;
                }
                inputField_Declaration.text = LanguageHelper.GetTextContent(langId);
            }
            else
            {
                inputField_Declaration.text = Sys_Family.Instance.familyNotic;
            }
        }

        private void SetButtonName()
        {
            isEditor = !isEditor;
            inputField_Declaration.interactable = isEditor;
            buttonName.text = LanguageHelper.GetTextContent(isEditor ? 2023413u : 2023323u);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Notice, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 点击编辑
        /// </summary>
        private void OnClick_Modify()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Notice, "OnClick_Modify");
            if (isEditor)
            {
                SetButtonName();
                string strDeclaration = string.IsNullOrEmpty(inputField_Declaration.text) ? LanguageHelper.GetTextContent(10022) : inputField_Declaration.text;
                if (!string.Equals(strDeclaration, Sys_Family.Instance.familyNotic, StringComparison.Ordinal))
                {
                    Sys_Family.Instance.GuildPetChangeNoticeReq(strDeclaration);
                }
            }
            else
            {
                SetButtonName();
            }
        }
        /// <summary>
        /// 输入字数
        /// </summary>
        /// <param name="text"></param>
        /// <param name="charIndex"></param>
        /// <param name="addedChar"></param>
        /// <returns></returns>
        private char OnValidateInput_Declaration(string text, int charIndex, char addedChar)
        {
            int nowNum = TextHelper.GetCharNum(text) + TextHelper.GetCharNum(addedChar.ToString());
            if (nowNum > maxLimit_Declaration)
            {
                return '\0';
            }

            return addedChar;
        }

        private void OnValueChange(string text)
        {
            int english = 0;
            int other = 0;
            int wordLength = 0;
            int index;
            int textLen = text.Length;
            for (index = 0; index < textLen; index++)
            {
                if (text[index] > 256)
                {
                    other++;
                }
                else
                {
                    english++;
                }
            }
            if(other == 0 && english == 1)
            {
                wordLength = 1;
            }
            else
            {
                wordLength = english / 2 + other;
            }
            
            numText.text = LanguageHelper.GetTextContent(2023322, wordLength.ToString(), (maxLimit_Declaration / 2).ToString());
        }
        #endregion
        #region 提供功能
        #endregion
    }
}