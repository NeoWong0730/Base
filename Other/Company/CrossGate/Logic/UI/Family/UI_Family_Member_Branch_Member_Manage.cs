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
    /// <summary> 家族分会成员管理 </summary>
    public class UI_Family_Member_Branch_Member_Manage : UIBase
    {
        #region 界面组件
        /// <summary> 分会名称 </summary>
        private Text text_BranchName;
        /// <summary> 家族人数 </summary>
        private Text text_Member;
        /// <summary> 家族列表 </summary>
        private ScrollGridVertical scrollGridVertical_Family;
        /// <summary> 分会列表 </summary>
        private ScrollGridVertical scrollGridVertical_Branch;
        #endregion
        #region 数据定义
        /// <summary> 分会编号 </summary>
        private uint BranchId;
        /// <summary> 家族列表 </summary>
        private List<CmdGuildGetMemberInfoAck.Types.MemberInfo> list_Family = new List<CmdGuildGetMemberInfoAck.Types.MemberInfo>();
        /// <summary> 分会列表 </summary>
        private List<CmdGuildGetMemberInfoAck.Types.MemberInfo> list_Branch = new List<CmdGuildGetMemberInfoAck.Types.MemberInfo>();
        /// <summary> 移动至家族列表 </summary>
        private List<ulong> list_MoveToFamily = new List<ulong>();
        /// <summary> 移动至分会列表 </summary>
        private List<ulong> list_MoveToBranch = new List<ulong>();
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
            SetData();
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
            text_BranchName = transform.Find("Animator/View_Right/Image1/Text").GetComponent<Text>();
            text_Member = transform.Find("Animator/View_Right/View_Amount/Text_Amount/Text").GetComponent<Text>();
            scrollGridVertical_Family = transform.Find("Animator/View_Left/ScrollView_Member").GetComponent<ScrollGridVertical>();
            scrollGridVertical_Branch = transform.Find("Animator/View_Right/ScrollView_Member").GetComponent<ScrollGridVertical>();
            scrollGridVertical_Family.AddCellListener(OnUpdateChildrenCallback_Family);
            scrollGridVertical_Branch.AddCellListener(OnUpdateChildrenCallback_Branch);
            transform.Find("Animator/View_Title09/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            if (toRegister)
            {
                ScrollOrDragControl.onCopyDataCallback = OnCopyItemData;
                ScrollOrDragControl.onChangeDataCallback = OnChangeMember;
            }
            else
            {
                ScrollOrDragControl.onCopyDataCallback = null;
                ScrollOrDragControl.onChangeDataCallback = null;
            }
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {
            list_Family.Clear();
            list_Branch.Clear();
            list_MoveToFamily.Clear();
            list_MoveToBranch.Clear();

            var familys = Sys_Family.Instance.familyData.CheckMemberInfos(0,
                new List<Sys_Family.FamilyData.EFamilyStatus>()
                {
                    Sys_Family.FamilyData.EFamilyStatus.EMember
                });
            var branchs = Sys_Family.Instance.familyData.CheckMemberInfos(BranchId,
                new List<Sys_Family.FamilyData.EFamilyStatus>()
                {
                    Sys_Family.FamilyData.EFamilyStatus.EBrachMember,
                    Sys_Family.FamilyData.EFamilyStatus.EBranchLeader
                });

            for (int i = 0, count = familys.Count; i < count; i++)
            {
                var member = familys[i];
                list_Family.Add(member);
            }
            for (int i = 0, count = branchs.Count; i < count; i++)
            {
                var member = branchs[i];
                list_Branch.Add(member);
            }
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            var branchInfo = Sys_Family.Instance.familyData.CheckBranchInfo(BranchId);
            text_BranchName.text = branchInfo == null ? string.Empty : branchInfo.Name.ToStringUtf8();
            scrollGridVertical_Family.SetCellCount(list_Family.Count);
            scrollGridVertical_Branch.SetCellCount(list_Branch.Count);
            text_Member.text = string.Format("{0}/{1}", list_Branch.Count, Sys_Family.Instance.familyData.familyBuildInfo.branchMemberNum);
        }
        /// <summary>
        /// 设置成员模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="index"></param>
        private void SetMemberItem(Transform tr, CmdGuildGetMemberInfoAck.Types.MemberInfo memberInfo)
        {
            tr.name = memberInfo.RoleId.ToString();
            /// <summary> 成员名称 </summary>
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            text_Name.text = memberInfo.Name.ToStringUtf8();
            /// <summary> 成员等级 </summary>
            Text text_Lv = tr.Find("Text_Level").GetComponent<Text>();
            text_Lv.text = memberInfo.Lvl.ToString();
            /// <summary> 成员职业 </summary>
            Image image_Job = tr.Find("Image_Prop").GetComponent<Image>();
            Text text_Job = tr.Find("Image_Prop/Text_Profession").GetComponent<Text>();
            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(memberInfo.Occ);
            text_Job.text = null == cSVCareerData ? string.Empty : LanguageHelper.GetTextContent(cSVCareerData.name);
            ImageHelper.SetIcon(image_Job, null == cSVCareerData ? 0 : cSVCareerData.logo_icon);
            /// <summary> 标记 </summary>
            GameObject go_New = tr.Find("Image_Lable").gameObject;
            bool isShow = list_MoveToFamily.Contains(memberInfo.RoleId) || list_MoveToBranch.Contains(memberInfo.RoleId);
            go_New.SetActive(isShow);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_BranchMemberManage, "OnClick_Close");
            CloseSelf();

            var familys = Sys_Family.Instance.familyData.CheckMemberInfos(0,
               new List<Sys_Family.FamilyData.EFamilyStatus>()
               {
                    Sys_Family.FamilyData.EFamilyStatus.EMember
               });
            var branchs = Sys_Family.Instance.familyData.CheckMemberInfos(BranchId,
                new List<Sys_Family.FamilyData.EFamilyStatus>()
                {
                    Sys_Family.FamilyData.EFamilyStatus.EBrachMember,
                    Sys_Family.FamilyData.EFamilyStatus.EBranchLeader
                });

            for (int i = 0, count = familys.Count; i < count; i++)
            {
                var member = familys[i];
                if(list_MoveToFamily.Contains(member.RoleId))
                {
                    list_MoveToFamily.Remove(member.RoleId);
                }
            }
            for (int i = 0, count = branchs.Count; i < count; i++)
            {
                var member = branchs[i];
                if (list_MoveToBranch.Contains(member.RoleId))
                {
                    list_MoveToBranch.Remove(member.RoleId);
                }
            }

            Sys_Family.Instance.SendGuildBranchMoveReq(BranchId, list_MoveToFamily, list_MoveToBranch);
        }
        /// <summary>
        /// 更新滚动子节点
        /// </summary>
        /// <param name="cell"></param>
        private void OnUpdateChildrenCallback_Family(ScrollGridCell cell)
        {
            var memberInfo = list_Family[cell.index];
            SetMemberItem(cell.gameObject.transform, list_Family[cell.index]);
        }
        /// <summary>
        /// 更新滚动子节点
        /// </summary>
        /// <param name="cell"></param>
        private void OnUpdateChildrenCallback_Branch(ScrollGridCell cell)
        {
            var memberInfo = list_Branch[cell.index];
            SetMemberItem(cell.gameObject.transform, list_Branch[cell.index]);
        }
        /// <summary>
        /// 交互数据
        /// </summary>
        private void OnChangeMember(Transform tr, Transform copy)
        {
            switch (copy.name)
            {
                case "Slot_Left":
                    {
                        if (!Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.MergeBranch))
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10072));
                            return;
                        }
                        ulong RoleId = 0;
                        ulong.TryParse(tr.name, out RoleId);
                        var memberInfo = list_Branch.Find(x => x.RoleId == RoleId);
                        if (null == memberInfo || RoleId == Sys_Role.Instance.RoleId)
                            return;

                        list_Family.Add(memberInfo);
                        list_Branch.Remove(memberInfo);

                        if (!list_MoveToFamily.Contains(memberInfo.RoleId))
                        {
                            list_MoveToFamily.Add(memberInfo.RoleId);
                        }
                        if (list_MoveToBranch.Contains(memberInfo.RoleId))
                        {
                            list_MoveToBranch.Remove(memberInfo.RoleId);
                        }
                        RefreshView();
                    }
                    break;
                case "Slot_Right":
                    {
                        if (!Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.MergeBranch))
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10072));
                            return;
                        }
                        if (list_Branch.Count + 1 > Sys_Family.Instance.familyData.familyBuildInfo.branchMemberNum)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10061));
                            return;
                        }
                        ulong RoleId = 0;
                        ulong.TryParse(tr.name, out RoleId);
                        var memberInfo = list_Family.Find(x => x.RoleId == RoleId);
                        if (null == memberInfo || RoleId == Sys_Role.Instance.RoleId)
                            return;

                        list_Branch.Add(memberInfo);
                        list_Family.Remove(memberInfo);

                        if (!list_MoveToBranch.Contains(memberInfo.RoleId))
                        {
                            list_MoveToBranch.Add(memberInfo.RoleId);
                        }
                        if (list_MoveToFamily.Contains(memberInfo.RoleId))
                        {
                            list_MoveToFamily.Remove(memberInfo.RoleId);
                        }
                        RefreshView();
                    }
                    break;
            }
        }
        /// <summary>
        /// 克隆模版数据
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="copy"></param>
        private void OnCopyItemData(Transform tr, Transform copy)
        {
            ulong id = 0;
            ulong.TryParse(tr.name, out id);
            CmdGuildGetMemberInfoAck.Types.MemberInfo memberInfo = null;
            memberInfo = list_Family.Find(x => x.RoleId == id);
            if (null == memberInfo)
            {
                memberInfo = list_Branch.Find(x => x.RoleId == id);
            }
            SetMemberItem(copy, memberInfo);
        }
        #endregion
        #region 提供功能
        #endregion
    }
}
