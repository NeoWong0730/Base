using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;
using Packet;

namespace Logic
{
    /// <summary> 家族分会成员 </summary>
    public class UI_Family_Member_Branch_Member : UIBase
    {
        #region 界面组件
        /// <summary> 无限滚动 </summary>
        private ScrollGridVertical scrollGridVertical;
        #endregion
        #region 数据定义
        /// <summary> 成员信息 </summary>
        private List<CmdGuildGetMemberInfoAck.Types.MemberInfo> list_memberInfos = new List<CmdGuildGetMemberInfoAck.Types.MemberInfo>();
        /// <summary> 分会编号 </summary>
        private uint BranchId;
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
            OnUpdateList();
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
            scrollGridVertical = transform.Find("Animator/View_Member/ScrollView_Member").GetComponent<ScrollGridVertical>();
            scrollGridVertical.AddCellListener(OnCellUpdateCallback);

            transform.Find("Animator/View_Title11/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/View_Member/Button_Manage").GetComponent<Button>().onClick.AddListener(OnClick_Manage);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMemberStatus, OnUpdateList, toRegister);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {
            list_memberInfos.Clear();
            var members = Sys_Family.Instance.familyData.CheckMemberInfos(BranchId, null);
            for (int i = 0, count = members.Count; i < count; i++)
            {
                var member = members[i];
                list_memberInfos.Add(member);
            }
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            scrollGridVertical.SetCellCount(list_memberInfos.Count);
        }
        /// <summary>
        /// 设置分会成员模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="memberInfo"></param>
        private void SetBranchMemberItem(Transform tr, CmdGuildGetMemberInfoAck.Types.MemberInfo memberInfo)
        {
            /// <summary> 家族职位 </summary>
            Text text_Status = tr.Find("Text_Job").GetComponent<Text>();
            uint BranchId = memberInfo.Position / 10000;
            uint Position = memberInfo.Position % 10000;
            CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData(Position);
            if (null == cSVFamilyPostAuthorityData)
            {
                text_Status.text = string.Empty;
            }
            else if (BranchId > 0)
            {
                var BranchInfo = Sys_Family.Instance.familyData.CheckBranchInfo(BranchId);
                string BranchName = BranchInfo == null ? string.Empty : BranchInfo.Name.ToStringUtf8();
                text_Status.text = string.Concat(BranchName, LanguageHelper.GetTextContent(cSVFamilyPostAuthorityData.PostName));
            }
            else
            {
                text_Status.text = LanguageHelper.GetTextContent(cSVFamilyPostAuthorityData.PostName);
            }
            /// <summary> 玩家姓名 </summary>
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            text_Name.text = memberInfo.Name.ToStringUtf8();
            /// <summary> 玩家等级 </summary>
            Text text_Lv = tr.Find("Text_Level").GetComponent<Text>();
            text_Lv.text = memberInfo.Lvl.ToString();
            /// <summary> 玩家职业 </summary>
            Image image_Job = tr.Find("Image_Prop").GetComponent<Image>();
            Text text_Job = tr.Find("Image_Prop/Text_Profession").GetComponent<Text>();
            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(memberInfo.Occ);
            text_Job.text = null == cSVCareerData ? string.Empty : LanguageHelper.GetTextContent(cSVCareerData.name);
            ImageHelper.SetIcon(image_Job, null == cSVCareerData ? 0 : cSVCareerData.logo_icon);
            /// <summary> 本周贡献 </summary>
            Text text_WeekContribution = tr.Find("Text_Dedicate").GetComponent<Text>();
            text_WeekContribution.text = FamilyLiveValue.GetString(memberInfo.Contribution);//memberInfo.Contribution.ToString();
            /// <summary> 历史贡献 </summary>
            Text text_HistoryContribution = tr.Find("Text_All_Dedicate").GetComponent<Text>();
            text_HistoryContribution.text = FamilyLiveValue.GetString(memberInfo.Totalcontribution); //memberInfo.Totalcontribution.ToString();
            /// <summary> 离线时间 </summary>
            Text text_Offline = tr.Find("Text_Time").GetComponent<Text>();
            uint time = 0;
            if (memberInfo.LastOffline != 0)
            {
                uint severTime = Sys_Time.Instance.GetServerTime();
                if (severTime < memberInfo.LastOffline)
                {
                    time = 1;
                }
                else
                {
                    time = severTime - memberInfo.LastOffline;
                }
            }
            text_Offline.text = LanguageHelper.TimeToString(time, LanguageHelper.TimeFormat.Type_3);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_BranchMember, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 打开管理界面
        /// </summary>
        private void OnClick_Manage()
        {
            UIManager.HitButton(EUIID.UI_Family_BranchMember, "OnClick_Manage");
            if (!Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.MergeBranch))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10072));
                return;
            }

            UIManager.OpenUI(EUIID.UI_Family_BranchMemberManage, false, BranchId);
        }
        /// <summary>
        /// 更新滚动子节点
        /// </summary>
        /// <param name="cell"></param>
        private void OnCellUpdateCallback(ScrollGridCell cell)
        {
            var memberInfo = list_memberInfos[cell.index];
            SetBranchMemberItem(cell.gameObject.transform, memberInfo);
        }
        /// <summary>
        /// 刷新列表
        /// </summary>
        private void OnUpdateList()
        {
            SetData();
            RefreshView();
        }
        #endregion
        #region 提供功能
        #endregion
    }
}
