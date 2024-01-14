using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Packet;

namespace Logic
{
    /// <summary> 家族合并协议 </summary>
    public class UI_Family_Hall_Merge_Agreement : UIBase
    {
        #region 界面组件
        /// <summary> 信息内容 </summary>
        private List<GameObject> list_Info = new List<GameObject>();
        /// <summary> 选项 </summary>
        private List<Toggle> list_SelectToggle = new List<Toggle>();
        /// <summary> 家族提示界面 </summary>
        private GameObject go_TipView;
        #endregion
        #region 数据定义
        /// <summary> 查询列表 </summary>
        private List<BriefInfo> list_Query = new List<BriefInfo>();
        /// <summary> 合并信息 </summary>
        private GuildDetailInfo.Types.MergeInfo mergeInfo;
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
            SetData();
            SendMergeFamilyList();
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
            list_Info.Add(transform.Find("Animator/View_Content/Object_Target").gameObject);
            list_Info.Add(transform.Find("Animator/View_Content/Object_Target1").gameObject);
            list_Info.Add(transform.Find("Animator/View_Content/Object_Merge").gameObject);
            list_SelectToggle.Add(transform.Find("Animator/View_Content/Toggle_Choice/toggle1").GetComponent<Toggle>());
            list_SelectToggle.Add(transform.Find("Animator/View_Content/Toggle_Choice/toggle2").GetComponent<Toggle>());
            go_TipView = transform.Find("Animator/View_Content/View_rule").gameObject;

            transform.Find("Animator/View_Title09/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/View_Content/View_Below/Button_Tips").GetComponent<Button>().onClick.AddListener(OnClick_OpenTipsView);
            transform.Find("Animator/View_Content/View_rule/close").GetComponent<Button>().onClick.AddListener(OnClick_CloseTipsView);
            transform.Find("Animator/View_Content/View_Below/Button_Contact").GetComponent<Button>().onClick.AddListener(OnClick_Contact);
            transform.Find("Animator/View_Content/View_Below/Button_Cancle").GetComponent<Button>().onClick.AddListener(OnClick_MergeCancel);

            for (int i = 0; i < list_SelectToggle.Count; i++)
            {
                Toggle toggle = list_SelectToggle[i];
                toggle.onValueChanged.AddListener((value) =>
                {
                    if (value) OnChanged_ChangeDstMerge(toggle);
                });
            }
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.GetMergeFamilyListRes, OnGetMergeFamilyListRes, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMergeTarget, OnUpdateMergeTarget, toRegister);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            mergeInfo = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.InitiativeMerge;
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            if (list_Query.Count >= 2)
            {
                bool isDst = mergeInfo.DstId != mergeInfo.OtherId;
                list_SelectToggle[isDst ? 0 : 1].SetIsOnWithoutNotify(true);

                BriefInfo MyBriefInfo = list_Query[0];
                BriefInfo OtherBriefInfo = list_Query[1];
                BriefInfo NewBriefInfo = new BriefInfo();
                if (isDst)
                {
                    NewBriefInfo.GuildId = MyBriefInfo.GuildId;
                    NewBriefInfo.GuildName = MyBriefInfo.GuildName;
                    NewBriefInfo.GuildLvl = MyBriefInfo.GuildLvl;
                    NewBriefInfo.LeaderName = MyBriefInfo.LeaderName;
                    NewBriefInfo.Notice = MyBriefInfo.Notice;
                    NewBriefInfo.MemberCount = MyBriefInfo.MemberCount + OtherBriefInfo.MemberCount;
                    NewBriefInfo.MemberMax = MyBriefInfo.MemberMax;
                }
                else
                {
                    NewBriefInfo.GuildId = OtherBriefInfo.GuildId;
                    NewBriefInfo.GuildName = OtherBriefInfo.GuildName;
                    NewBriefInfo.GuildLvl = OtherBriefInfo.GuildLvl;
                    NewBriefInfo.LeaderName = OtherBriefInfo.LeaderName;
                    NewBriefInfo.Notice = OtherBriefInfo.Notice;
                    NewBriefInfo.MemberCount = MyBriefInfo.MemberCount + OtherBriefInfo.MemberCount;
                    NewBriefInfo.MemberMax = OtherBriefInfo.MemberMax;
                }

                SetFamilyInfoItem(list_Info[0].transform, MyBriefInfo);
                SetFamilyInfoItem(list_Info[1].transform, OtherBriefInfo);
                SetFamilyInfoItem(list_Info[2].transform, NewBriefInfo);
            }
        }
        /// <summary>
        /// 设置家族信息模版
        /// </summary>
        /// <param name="tr"></param>
        private void SetFamilyInfoItem(Transform tr, BriefInfo briefInfo)
        {
            /// <summary> 家族编号 </summary>
            Text text_Id = tr.Find("Text_ID/Text").GetComponent<Text>();
            text_Id.text = (briefInfo.GuildId % 100000000UL).ToString();
            /// <summary> 家族名称 </summary>
            Text text_Name = tr.Find("Image_Name/Text_Name").GetComponent<Text>();
            text_Name.text = briefInfo.GuildName.ToStringUtf8();
            /// <summary> 家族族长 </summary>
            Text text_Leader = tr.Find("Text_Head/Text").GetComponent<Text>();
            text_Leader.text = briefInfo.LeaderName.ToStringUtf8();
            /// <summary> 家族成员 </summary>
            Text text_Member = tr.Find("Text_Number/Text").GetComponent<Text>();
            text_Member.text = string.Format("{0}/{1}", briefInfo.MemberCount.ToString(), briefInfo.MemberMax.ToString());
            /// <summary> 家族等级 </summary>
            Text text_Level = tr.Find("Text_Status/Text").GetComponent<Text>();
            text_Level.text = briefInfo.GuildLvl.ToString();
            /// <summary> 成员超额提示 </summary>
            GameObject go_Tips = tr.Find("Text_Tips")?.gameObject;
            if (null != go_Tips)
                go_Tips.SetActive(briefInfo.MemberCount > briefInfo.MemberMax);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_MergeAgreement, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 打开提示界面
        /// </summary>
        private void OnClick_OpenTipsView()
        {
            UIManager.HitButton(EUIID.UI_Family_MergeAgreement, "OnClick_OpenTipsView");
            go_TipView.gameObject.SetActive(true);
        }
        /// <summary>
        /// 关闭提示界面
        /// </summary>
        private void OnClick_CloseTipsView()
        {
            UIManager.HitButton(EUIID.UI_Family_MergeAgreement, "OnClick_CloseTipsView");
            go_TipView.gameObject.SetActive(false);
        }
        /// <summary>
        /// 联系对方
        /// </summary>
        private void OnClick_Contact()
        {

        }
        /// <summary>
        /// 合并取消
        /// </summary>
        private void OnClick_MergeCancel()
        {
            UIManager.HitButton(EUIID.UI_Family_MergeAgreement, "OnClick_MergeCancel");
            Sys_Family.Instance.SendGuildCancleMergeReq();
            OnClick_Close();
        }
        /// <summary>
        /// 改变目标
        /// </summary>
        /// <param name="toggle"></param>
        private void OnChanged_ChangeDstMerge(Toggle toggle)
        {
            UIManager.HitButton(EUIID.UI_Family_MergeAgreement, "OnChanged_ChangeDstMerge");
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            int index = list_SelectToggle.IndexOf(toggle);
            ulong myFamilyId = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildId;
            ulong newDstId = index == 0 ? myFamilyId : mergeInfo.OtherId;
            Sys_Family.Instance.SendGuildChangeDstMergeGuildReq(mergeInfo.OtherId, newDstId);
        }
        /// <summary>
        /// 列表获取
        /// </summary>
        private void OnGetMergeFamilyListRes()
        {
            var list = Sys_Family.Instance.familyData.queryFamilyInfo.mergeList;
            list_Query.Clear();
            list_Query.AddRange(list);
            RefreshView();
        }
        /// <summary>
        /// 更新合并目标
        /// </summary>
        private void OnUpdateMergeTarget()
        {
            RefreshView();
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 合并家族列表
        /// </summary>
        private void SendMergeFamilyList()
        {
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            ulong myFamilyId = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildId;
            Sys_Family.Instance.SendGuildFindGuildReq(new List<ulong>() { myFamilyId, mergeInfo.OtherId });
        }
        #endregion
    }
}