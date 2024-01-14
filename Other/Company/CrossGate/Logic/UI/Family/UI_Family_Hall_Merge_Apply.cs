using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Packet;

namespace Logic
{
    /// <summary> 家族申请合并 </summary>
    public class UI_Family_Hall_Merge_Apply : UIBase
    {
        #region 界面组件
        /// <summary> 无限滚动 </summary>
        private ScrollGridVertical scrollGridVertical;
        #endregion
        #region 数据定义
        /// <summary> 查询列表 </summary>
        private List<BriefInfo> list_Query = new List<BriefInfo>();
        /// <summary> 合并信息 </summary>
        private List<GuildDetailInfo.Types.MergeInfo> list_AllMergeInfos = new List<GuildDetailInfo.Types.MergeInfo>();
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
            SendMergeFamilyList();
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
            scrollGridVertical = transform.Find("Animator/View_Content/View_Member/ScrollView_Member").GetComponent<ScrollGridVertical>();
            scrollGridVertical.AddCellListener(OnCellUpdateCallback);
            scrollGridVertical.AddCreateCellListener(OnCreateCellCallback);

            transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/View_Content/View_Below/Button_Clear").GetComponent<Button>().onClick.AddListener(OnClick_ClearList);
            transform.Find("Animator/View_Content/View_Below/Button_Refresh").GetComponent<Button>().onClick.AddListener(OnClick_RefreshList);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.GetMergeFamilyListRes, OnGetMergeFamilyListRes, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateApplyMergedList, OnGetMergeFamilyListRes, toRegister);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            list_AllMergeInfos.Clear();
            var AllMergeInfos = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllMergeInfos;
            for (int i = 0, count = AllMergeInfos.Count; i < count; i++)
            {
                var info = AllMergeInfos[i];
                list_AllMergeInfos.Add(info);
            }
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            scrollGridVertical.SetCellCount(list_Query.Count);
        }
        /// <summary>
        /// 设置申请模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="briefInfo"></param>
        /// <param name="mergeInfo"></param>
        private void SetApplyItem(Transform tr, BriefInfo briefInfo, GuildDetailInfo.Types.MergeInfo mergeInfo)
        {
            tr.name = briefInfo.GuildId.ToString();
            /// <summary> 编号 </summary>
            Text text_Id = tr.Find("Text_Number").GetComponent<Text>();
            text_Id.text = (briefInfo.GuildId % 100000000UL).ToString();
            /// <summary> 家族名称 </summary>
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            text_Name.text = briefInfo.GuildName.ToStringUtf8();
            /// <summary> 家族等级 </summary>
            Text text_Lv = tr.Find("Text_Level").GetComponent<Text>();
            text_Lv.text = briefInfo.GuildLvl.ToString();
            /// <summary> 目标家族 </summary>
            Text text_Taget = tr.Find("Text_Taget").GetComponent<Text>();
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            text_Taget.text = mergeInfo.DstId == briefInfo.GuildId ?
                briefInfo.GuildName.ToStringUtf8() :
                Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GName.ToStringUtf8();
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_MergeApply, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 清空列表
        /// </summary>
        private void OnClick_ClearList()
        {
            UIManager.HitButton(EUIID.UI_Family_MergeApply, "OnClick_ClearList");
            list_Query.Clear();
            RefreshView();
        }
        /// <summary>
        /// 刷新列表
        /// </summary>
        private void OnClick_RefreshList()
        {
            SendMergeFamilyList();
        }
        /// <summary>
        /// 操作模版
        /// </summary>
        /// <param name="go"></param>
        /// <param name="value"></param>
        private void OnClick_OperationItem(GameObject go, bool value)
        {
            ulong familyId;
            if (!ulong.TryParse(go.name, out familyId)) return;
            int index = list_AllMergeInfos.FindIndex(x => x.OtherId == familyId);
            if (index < 0) return;
            UIManager.HitButton(EUIID.UI_Family_MergeApply, "OnClick_OperationItem:" + value.ToString());
            GuildDetailInfo.Types.MergeInfo mergeInfo = list_AllMergeInfos[index];
            Sys_Family.Instance.SendGuildHandleMergeApplyReq(mergeInfo.OtherId, mergeInfo.DstId, value);
        }
        /// <summary>
        /// 更新滚动子节点
        /// </summary>
        /// <param name="cell"></param>
        private void OnCellUpdateCallback(ScrollGridCell cell)
        {
            BriefInfo briefInfo = list_Query[cell.index];
            var info = list_AllMergeInfos.Find(x => x.OtherId == briefInfo.GuildId || x.DstId == briefInfo.GuildId);
            if (null == info) info = new GuildDetailInfo.Types.MergeInfo();
            SetApplyItem(cell.gameObject.transform, briefInfo, info);
        }
        /// <summary>
        /// 创建模版回调
        /// </summary>
        /// <param name="cell"></param>
        private void OnCreateCellCallback(ScrollGridCell cell)
        {
            GameObject go = cell.gameObject;
            go.transform.Find("Button/Button_Pass").GetComponent<Button>().onClick.AddListener(() => { OnClick_OperationItem(go, true); });
            go.transform.Find("Button/Button_Cross").GetComponent<Button>().onClick.AddListener(() => { OnClick_OperationItem(go, false); });
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
        #endregion
        #region 提供功能
        /// <summary>
        /// 合并家族列表
        /// </summary>
        private void SendMergeFamilyList()
        {
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            ulong myFamilyId = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildId;
            var mergeInfo = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllMergeInfos;
            List<ulong> mergelist = new List<ulong>();
            for (int i = 0, count = mergeInfo.Count; i < count; i++)
            {
                var info = mergeInfo[i];
                mergelist.Add(info.OtherId);
            }
            Sys_Family.Instance.SendGuildFindGuildReq(mergelist);
        }
        #endregion
    }
}