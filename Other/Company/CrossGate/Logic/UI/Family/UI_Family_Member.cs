using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Packet;
using System.Text;

namespace Logic
{
    /// <summary> 家族成员子界面 </summary>
    public class UI_Family_Member : UIComponent
    {
        #region 界面组件
        /// <summary> 在线成员 </summary>
        private Text text_OnlineMember;
        /// <summary> 无限滚动 </summary>
        private ScrollGridVertical scrollGridVertical;
        /// <summary> 排序选项 </summary>
        private Toggle toggle_Sort;
        /// <summary> 菜单选项列表 </summary>
        private List<Toggle> list_Menu = new List<Toggle>();
        /// <summary> 退会按钮 </summary>
        private GameObject go_BtnExit;
        /// <summary> 交接按钮 </summary>
        private GameObject go_BtnRelieve;
        #endregion
        #region 数据定义
        /// <summary> 筛选类型 </summary>
        public enum ESortType
        {
            Level = 0,              //等级
            Job = 1,                //职业
            Status = 2,             //家族职位
            WeekContribution = 3,   //本周贡献
            HistoryContribution = 4,//历史贡献
            Offline = 5,            //离线时间
           // HistoryLive = 6,            //历史家族活跃
        }
        /// <summary> 成员信息 </summary>
        private List<CmdGuildGetMemberInfoAck.Types.MemberInfo> list_memberInfos = new List<CmdGuildGetMemberInfoAck.Types.MemberInfo>();
        /// <summary> 是否第一次打开界面 </summary>
        private bool isFirstOpen = false;
        #endregion
        #region 系统函数
        protected override void Loaded()
        {
            OnParseComponent();
        }
        public override void Reset()
        {
            isFirstOpen = true;
        }
        public override void Show()
        {
            base.Show();
            if (isFirstOpen)
            {
                Sys_Family.Instance.SendGuildGetMemberInfoReq();
                isFirstOpen = false;
            }
            else
            {
                OnUpdateMember();
            }
        }
        public override void Hide()
        {
            base.Hide();
        }
        protected override void Update()
        {
        }
        protected override void Refresh()
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
            text_OnlineMember = transform.Find("Text_Amount/Text").GetComponent<Text>();

            scrollGridVertical = transform.Find("ScrollView_Member").GetComponent<ScrollGridVertical>();
            scrollGridVertical.AddCellListener(OnCellUpdateCallback);
            scrollGridVertical.AddCreateCellListener(OnCreateCellCallback);


            toggle_Sort = transform.Find("View_Title/ArrowList/Toggle").GetComponent<Toggle>();
            toggle_Sort.onValueChanged.AddListener((bool value) => { OnClick_Sort(value); });
            var values = System.Enum.GetValues(typeof(ESortType));
            for (int i = 0, count = values.Length; i < count; i++)
            {
                Toggle toggle = transform.Find(string.Format("View_Title/ArrowList/Menu/Arrow ({0})", i + 1)).GetComponent<Toggle>();
                toggle.onValueChanged.AddListener((bool value) => { OnClick_Menu(toggle, value); });
                list_Menu.Add(toggle);
            }

            go_BtnExit = transform.Find("Button_Exit").gameObject;
            go_BtnRelieve = transform.Find("Button_Relieve").gameObject;

            transform.Find("Button_Manage").GetComponent<Button>().onClick.AddListener(OnClick_FamilyBranch);
            transform.Find("Button_List").GetComponent<Button>().onClick.AddListener(OnClick_FamilyList);
            transform.Find("Button_Limit").GetComponent<Button>().onClick.AddListener(OnClick_Authority);
            transform.Find("Button_Apply").GetComponent<Button>().onClick.AddListener(OnClick_ApplyList);
            go_BtnExit.GetComponent<Button>().onClick.AddListener(OnClick_LeaveFamily);
            go_BtnRelieve.GetComponent<Button>().onClick.AddListener(OnClick_OutgoingLeader);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMember, OnUpdateMember, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMemberStatus, OnUpdateMember, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.DestroyBranch, OnUpdateMember, toRegister);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {
            list_memberInfos.Clear();
            var members = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetMemberInfoAck.Member;
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
            uint onlineMember = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.OnlineMember;
            uint allMember = Sys_Family.Instance.familyData.familyBuildInfo.membershipCap;
            scrollGridVertical.SetCellCount(list_memberInfos.Count);
            text_OnlineMember.text = string.Format("{0}/{1}", onlineMember, allMember);

            var leader = Sys_Family.Instance.familyData.CheckLeader(0);
            go_BtnExit.SetActive(null != leader && leader.RoleId != Sys_Role.Instance.RoleId);
            go_BtnRelieve.SetActive(null != leader && leader.RoleId == Sys_Role.Instance.RoleId);
        }
        /// <summary>
        /// 设置成员模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="memberInfo"></param>
        private void SetMemberItem(Transform tr, CmdGuildGetMemberInfoAck.Types.MemberInfo memberInfo)
        {
            tr.name = memberInfo.RoleId.ToString();
            /// <summary> 玩家姓名 </summary>
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            text_Name.text = memberInfo.Name.ToStringUtf8();
            /// <summary> 玩家等级 </summary>
            Text text_Lv = tr.Find("Text_Level").GetComponent<Text>();
            text_Lv.text = memberInfo.Lvl.ToString();
            /// <summary> 玩家职业 </summary>
            Image image_Job = tr.Find("Text_Profession/Image_Prop").GetComponent<Image>();
            Text text_Job = tr.Find("Text_Profession").GetComponent<Text>();
            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(memberInfo.Occ);
            text_Job.text = null == cSVCareerData ? string.Empty : LanguageHelper.GetTextContent(cSVCareerData.name);
            ImageHelper.SetIcon(image_Job, null == cSVCareerData ? 0 : cSVCareerData.logo_icon);
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
            /// <summary> 本周贡献 </summary>
            Text text_WeekContribution = tr.Find("Text_Dedicate").GetComponent<Text>();
            text_WeekContribution.text = FamilyLiveValue.GetString(memberInfo.Contribution);// memberInfo.Contribution.ToString();
            /// <summary> 历史贡献 </summary>
            Text text_HistoryContribution = tr.Find("Text_All_Dedicate").GetComponent<Text>();
            text_HistoryContribution.text = FamilyLiveValue.GetString(memberInfo.Totalcontribution);// memberInfo.Totalcontribution.ToString();
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
        /// 分会管理
        /// </summary>
        private void OnClick_FamilyBranch()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_FamilyBranch");
            UIManager.OpenUI(EUIID.UI_Family_Branch);
        }
        /// <summary>
        /// 家族列表
        /// </summary>
        private void OnClick_FamilyList()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_FamilyList");
            UIManager.OpenUI(EUIID.UI_Family_FamilyList);
        }
        /// <summary>
        /// 家族权限
        /// </summary>
        private void OnClick_Authority()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_Authority");
            UIManager.OpenUI(EUIID.UI_Family_Authority);
        }
        /// <summary>
        /// 申请列表
        /// </summary>
        private void OnClick_ApplyList()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_ApplyList");
            UIManager.OpenUI(EUIID.UI_Family_ApplyList);
        }
        /// <summary>
        /// 交接会长
        /// </summary>
        private void OnClick_OutgoingLeader()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_OutgoingLeader");

            if(list_memberInfos.Count == 1)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(11013);
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_Family.Instance.SendGuildQuitReq();
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
            }
            else
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(10041);
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    var viceLeaders = Sys_Family.Instance.familyData.CheckMemberInfos(0,
                    new List<Sys_Family.FamilyData.EFamilyStatus>() { Sys_Family.FamilyData.EFamilyStatus.EViceLeader });
                    if (viceLeaders.Count <= 0)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10040));
                    }
                    else
                    {
                        Sys_Family.Instance.SendGuildOutgoingLeaderReq(viceLeaders[0].RoleId);
                    }
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
            }
        }
        /// <summary>
        /// 退出家族
        /// </summary>
        private void OnClick_LeaveFamily()
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_LeaveFamily");
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(10042);
            PromptBoxParameter.Instance.SetConfirm(true, () => { Sys_Family.Instance.SendGuildQuitReq(); });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
        }
        /// <summary>
        /// 点击成员交互
        /// </summary>
        /// <param name="go"></param>
        private void OnClick_MemberInfo(GameObject go)
        {
            UIManager.HitButton(EUIID.UI_Family, "OnClick_MemberInfo");
            ulong id = 0;
            if (!ulong.TryParse(go.name, out id)) return;
            var memberInfo = list_memberInfos.Find(x => x.RoleId == id);
            if (null == memberInfo || id == Sys_Role.Instance.RoleId) return;

            Sys_Role_Info.Instance.OpenRoleInfo(memberInfo.RoleId, Sys_Role_Info.EType.Family);
        }
        /// <summary>
        /// 更新滚动子节点
        /// </summary>
        /// <param name="cell"></param>
        private void OnCellUpdateCallback(ScrollGridCell cell)
        {
            var memberInfo = list_memberInfos[cell.index];
            SetMemberItem(cell.gameObject.transform, memberInfo);
        }
        /// <summary>
        /// 创建模版回调
        /// </summary>
        /// <param name="cell"></param>
        private void OnCreateCellCallback(ScrollGridCell cell)
        {
            GameObject go = cell.gameObject;
            go.GetComponent<Button>().onClick.AddListener(() => { OnClick_MemberInfo(go); });
        }

        /// <summary>
        /// 点击菜单
        /// </summary>
        /// <param name="toggle"></param>
        private void OnClick_Menu(Toggle toggle, bool value)
        {
            if (value)
            {
                toggle_Sort.transform.position = toggle.transform.position;
                OnClick_Sort(true);
            }
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="value"></param>
        private void OnClick_Sort(bool value)
        {
            OnUpdateMember();
        }
        /// <summary>
        /// 更新成员信息
        /// </summary>
        private void OnUpdateMember()
        {
            SetData();
            Sort();
            RefreshView();
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 排序
        /// </summary>
        private void Sort()
        {
            int index = list_Menu.FindIndex(x => x.isOn == true);
            if (index < 0) return;
            bool isDescendingOrder = toggle_Sort.isOn;
            StringBuilder btnEventStr = StringBuilderPool.GetTemporary();
            btnEventStr.Append("Sort:");
            btnEventStr.Append((index).ToString());
            UIManager.HitButton(EUIID.UI_Family, StringBuilderPool.ReleaseTemporaryAndToString(btnEventStr));
            switch ((ESortType)index)
            {
                case ESortType.Level:
                    {
                        list_memberInfos.Sort((x1, x2) =>
                        {
                            if (isDescendingOrder)
                                return x2.Lvl.CompareTo(x1.Lvl);
                            else
                                return x1.Lvl.CompareTo(x2.Lvl);
                        });
                    }
                    break;
                case ESortType.Job:
                    {
                        list_memberInfos.Sort((x1, x2) =>
                        {
                            if (isDescendingOrder)
                                return x2.Occ.CompareTo(x1.Occ);
                            else
                                return x1.Occ.CompareTo(x2.Occ);
                        });
                    }
                    break;
                case ESortType.Status:
                    {
                        list_memberInfos.Sort((x1, x2) =>
                        {
                            if (isDescendingOrder)
                            {
                                int result = (x2.Position % 10000).CompareTo(x1.Position % 10000);
                                if (result != 0)
                                {
                                    return result;
                                }
                                else
                                {
                                    return (x2.Position / 10000).CompareTo(x1.Position / 10000);
                                }
                            }
                            else
                            {
                                int result = (x1.Position % 10000).CompareTo(x2.Position % 10000);
                                if (result != 0)
                                {
                                    return result;
                                }
                                else
                                {
                                    return (x1.Position / 10000).CompareTo(x2.Position / 10000);
                                }
                            }
                        });
                    }
                    break;
                case ESortType.WeekContribution:
                    {
                        list_memberInfos.Sort((x1, x2) =>
                        {
                            if (isDescendingOrder)
                                return x2.Contribution.CompareTo(x1.Contribution);
                            else
                                return x1.Contribution.CompareTo(x2.Contribution);
                        });
                    }
                    break;
                case ESortType.HistoryContribution:
                    {
                        list_memberInfos.Sort((x1, x2) =>
                        {
                            if (isDescendingOrder)
                                return x2.Totalcontribution.CompareTo(x1.Totalcontribution);
                            else
                                return x1.Totalcontribution.CompareTo(x2.Totalcontribution);
                        });
                    }
                    break;
                case ESortType.Offline:
                    {
                        list_memberInfos.Sort((x1, x2) =>
                        {
                            uint tempX1 = x1.LastOffline;
                            if (tempX1 == 0)
                            {
                                tempX1 = Sys_Time.Instance.GetServerTime();
                            }
                            uint tempX2 = x2.LastOffline;
                            if (x2.LastOffline == 0)
                            {
                                tempX2 = Sys_Time.Instance.GetServerTime();
                            }
                            if (isDescendingOrder)
                                return tempX2.CompareTo(tempX1);
                            else
                                return tempX1.CompareTo(tempX2);
                        });
                    }
                    break;
            }
        }
        #endregion
    }
}