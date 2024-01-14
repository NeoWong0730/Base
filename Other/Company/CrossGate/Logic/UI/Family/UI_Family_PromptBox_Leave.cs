using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;

namespace Logic
{
    /// <summary> 请离家族对话框 </summary>
    public class UI_Family_PromptBox_Leave : UIBase
    {
        #region 界面组件
        /// <summary> 输入理由 </summary>
        private InputField inputField_Reason;
        #endregion
        #region 数据定义
        /// <summary> 玩家Id </summary>
        private ulong roleId = 0;
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
            roleId = null == arg ? 0 : System.Convert.ToUInt64(arg);
        }
        protected override void OnShow()
        {

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
            inputField_Reason = transform.Find("Animator/InputField_Describe").GetComponent<InputField>();

            transform.Find("Animator/Button_Sure").GetComponent<Button>().onClick.AddListener(OnClick_OK);
            transform.Find("Animator/Button_Cancel").GetComponent<Button>().onClick.AddListener(OnClick_Cancel);
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
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_PromptBox_Leave, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 点击确定
        /// </summary>
        private void OnClick_OK()
        {
            UIManager.HitButton(EUIID.UI_Family_PromptBox_Leave, "OnClick_OK");
            if (string.IsNullOrEmpty(inputField_Reason.text))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10048));
                return;
            }
            if (Sys_WordInput.Instance.HasLimitWord(inputField_Reason.text))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10023));
                return;
            }
            Sys_Family.Instance.SendGuildKickMemberReq(roleId, inputField_Reason.text);
            OnClick_Close();
        }
        /// <summary>
        /// 点击取消
        /// </summary>
        private void OnClick_Cancel()
        {
            UIManager.HitButton(EUIID.UI_Family_PromptBox_Leave, "OnClick_Cancel");
            OnClick_Close();
        }
        #endregion
        #region 提供功能
        #endregion
    }
}