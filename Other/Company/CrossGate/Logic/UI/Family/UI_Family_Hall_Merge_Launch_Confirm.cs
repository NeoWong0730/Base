using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Packet;

namespace Logic
{
    /// <summary> 家族合并确认 </summary>
    public class UI_Family_Hall_Merge_Launch_Confirm : UIBase
    {
        #region 界面组件
        /// <summary> 菜单选项 </summary>
        private ToggleGroup toggleGroup;
        #endregion
        #region 数据定义
        /// <summary> 合并家族数据 </summary>
        private BriefInfo briefInfo;
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
            briefInfo = (BriefInfo)arg;
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
            toggleGroup = transform.Find("Animator/View_Content/ToggleGroup").GetComponent<ToggleGroup>();

            transform.Find("Animator/View_TipsBg02_Smallest/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/View_Content/Button_Sure").GetComponent<Button>().onClick.AddListener(OnClick_Merge);
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

        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_MergeConfirm, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 确认合并
        /// </summary>
        private void OnClick_Merge()
        {
            var list = toggleGroup.ActiveToggles();
            bool ToMerged = false;
            foreach (var toggle in list)
            {
                if (!toggle.isOn) continue;

                switch (toggle.name)
                {
                    case "Merged":
                        {
                            ToMerged = true;
                        }
                        break;
                    case "Merge":
                        {
                            ToMerged = false;
                        }
                        break;
                }
            }
            UIManager.HitButton(EUIID.UI_Family_MergeConfirm, "OnClick_Merge:" + ToMerged.ToString());
            uint MyMemberCount = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.MemberCount;
            uint MyMemberMax = Sys_Family.Instance.familyData.familyBuildInfo.membershipCap;
            uint OtherMemberCount = briefInfo.MemberCount;
            uint OtherMemberMax = briefInfo.MemberMax;

            System.Action action = () =>
            {
                if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
                ulong myFamilyId = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildId;
                Sys_Family.Instance.SendGuildMergeReq(briefInfo.GuildId, !ToMerged ? myFamilyId : briefInfo.GuildId);
                OnClick_Close();
            };

            if ((ToMerged && MyMemberCount + OtherMemberCount > MyMemberMax)||
                (!ToMerged && MyMemberCount + OtherMemberCount > OtherMemberMax))
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(10670);
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    action.Invoke();
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
            }
            else
            {
                action.Invoke();
            }
        }
        #endregion
        #region 提供功能
        #endregion
    }
}