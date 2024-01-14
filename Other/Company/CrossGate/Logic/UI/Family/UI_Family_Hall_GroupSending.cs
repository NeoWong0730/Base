using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;

namespace Logic
{
    /// <summary> 家族群发消息 </summary>
    public class UI_Family_Hall_GroupSending : UIBase
    {
        #region 界面组件
        /// <summary> 输入家族群发消息 </summary>
        private InputField inputField_Declaration;
        #endregion
        #region 数据定义
        /// <summary> 描述最大字数 </summary>
        private const int maxLimit_Declaration = 100;
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
            ResetView();
        }
        protected override void OnShow()
        {
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
            inputField_Declaration = transform.Find("Animator/InputField_Describe").GetComponent<InputField>();

            transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/Button_Modify").GetComponent<Button>().onClick.AddListener(OnClick_Send);
            inputField_Declaration.characterLimit = 0;
            inputField_Declaration.onValidateInput = OnValidateInput_Declaration;
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
        
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 重置界面
        /// </summary>
        private void ResetView()
        {
            inputField_Declaration.text = string.Empty;
        }
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
         
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_GroupSending, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 群发
        /// </summary>
        private void OnClick_Send()
        {
            UIManager.HitButton(EUIID.UI_Family_GroupSending, "OnClick_Send");
            if (Sys_WordInput.Instance.HasLimitWord(inputField_Declaration.text))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10023));
                return;
            }

            uint myPosition = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.MyPosition;
            uint BranchId = myPosition / 10000;
            uint Position = myPosition % 10000;
            string strPosition = string.Empty;
            CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData(Position);
            if (null == cSVFamilyPostAuthorityData)
            {
                strPosition = string.Empty;
            }
            else if (BranchId > 0)
            {
                var BranchInfo = Sys_Family.Instance.familyData.CheckBranchInfo(BranchId);
                string BranchName = BranchInfo == null ? string.Empty : BranchInfo.Name.ToStringUtf8();
                strPosition = string.Concat(BranchName, LanguageHelper.GetTextContent(cSVFamilyPostAuthorityData.PostName));
            }
            else
            {
                strPosition = LanguageHelper.GetTextContent(cSVFamilyPostAuthorityData.PostName);
            }

            //if(!SDKManager.GetRealNameStatus())
            //{
            //    Sys_Chat.Instance.PushErrorTip(Sys_Chat.Chat_RealName_Error);
            //    return;
            //}
            Sys_Family.Instance.SendGuildNotifyReq(LanguageHelper.GetTextContent(10637, strPosition, Sys_Role.Instance.sRoleName, inputField_Declaration.text));
            OnClick_Close();
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
            if (TextHelper.GetCharNum(text) + TextHelper.GetCharNum(addedChar.ToString()) > maxLimit_Declaration)
            {
                return '\0';
            }
            return addedChar;
        }
        #endregion
        #region 提供功能
        #endregion
    }
}