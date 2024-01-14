using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System.Text;

namespace Logic
{
    /// <summary> 家族分会改名 </summary>
    public class UI_Family_Member_Branch_ModifyName : UIBase
    {
        #region 界面组件
        /// <summary> 分会数量 </summary>
        private Text text_Number;
        /// <summary> 输入家族分会名称 </summary>
        private InputField inputField_Name;
        #endregion
        #region 数据定义
        /// <summary> 分会编号 </summary>
        private uint BranchId;
        /// <summary> 名称最小字数 </summary>
        private const int minLimit_Name = 1;
        /// <summary> 名称最大字数 </summary>
        private const int maxLimit_Name = 4;
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
            BranchId = null == arg ? 0 : System.Convert.ToUInt32(arg);
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
            text_Number = transform.Find("Animator/View_Content/Text_Amount/Text").GetComponent<Text>();
            inputField_Name = transform.Find("Animator/View_Content/InputField_Describe").GetComponent<InputField>();
            transform.Find("Animator/View_TipsBg01_Big/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/View_Content/Button_Sure").GetComponent<Button>().onClick.AddListener(OnClick_Modify);

            inputField_Name.characterLimit = 0;
            inputField_Name.onValidateInput = OnValidateInput_Name;
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
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            int curBranchNum = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.BranchMemberInfo.Count;
            uint maxBranchNum = Sys_Family.Instance.familyData.familyBuildInfo.buildBranchNum;
            text_Number.text = string.Format("{0}/{1}", curBranchNum, maxBranchNum);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_BranchModifyName, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 修改名称
        /// </summary>
        private void OnClick_Modify()
        {
            UIManager.HitButton(EUIID.UI_Family_BranchModifyName, "OnClick_Modify");
            string newName = inputField_Name.text;
            if (TextHelper.GetCharNum(newName) < minLimit_Name)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10665));
                return;
            }
            if (Sys_RoleName.Instance.HasBadNames(inputField_Name.text) || Sys_WordInput.Instance.HasLimitWord(inputField_Name.text))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10023));
                return;
            }
            if (BranchId == 0)//创建分会
            {
                Sys_Family.Instance.SendGuildCreateBranchReq(inputField_Name.text);
                OnClick_Close();
            }
            else//修改分会名称
            {
                if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
                var branchMemberInfos = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.BranchMemberInfo;
                string oldName = string.Empty;

                for (int i = 0, count = branchMemberInfos.Count; i < count; i++)
                {
                    var branchMemberInfo = branchMemberInfos[i];
                    if (branchMemberInfo.Id == BranchId)
                    {
                        oldName = branchMemberInfo.Name.ToStringUtf8();
                        break;
                    }
                }
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(10045, oldName, newName);
                PromptBoxParameter.Instance.SetConfirm(true, () => { Sys_Family.Instance.SendGuildChangeBranchNameReq(BranchId, inputField_Name.text); OnClick_Close(); });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
            }
        }
        /// <summary>
        /// 输入字数
        /// </summary>
        /// <param name="text"></param>
        /// <param name="charIndex"></param>
        /// <param name="addedChar"></param>
        /// <returns></returns>
        private char OnValidateInput_Name(string text, int charIndex, char addedChar)
        {
            if (TextHelper.GetCharNum(text) + TextHelper.GetCharNum(addedChar.ToString()) > maxLimit_Name)
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
