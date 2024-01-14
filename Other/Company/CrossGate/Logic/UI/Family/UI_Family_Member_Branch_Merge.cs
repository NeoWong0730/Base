using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;
using Packet;
using static Packet.GuildDetailInfo.Types;

namespace Logic
{
    /// <summary> 家族分会合并 </summary>
    public class UI_Family_Member_Branch_Merge : UIBase
    {
        #region 界面组件
        /// <summary> 无分会节点 </summary>
        private GameObject go_NoneView;
        /// <summary> 无限滚动 </summary>
        private ScrollGridVertical scrollGridVertical;
        /// <summary> 选项组 </summary>
        private ToggleGroup toggleGroup;
        /// <summary> 选项列表 </summary>
        private List<Toggle> toggles = new List<Toggle>();
        #endregion
        #region 数据定义
        /// <summary> 当前的分会 </summary>
        private uint curBranchId;
        /// <summary> 分会列表 </summary>
        private List<GuildDetailInfo.Types.BranchInfo> list_BranchInfos = new List<GuildDetailInfo.Types.BranchInfo>();
        /// <summary> 选中家族下标 </summary>
        private int selectIndex = 0;
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
            curBranchId = System.Convert.ToUInt32(arg);
        }
        protected override void OnOpened()
        {
            selectIndex = 0;
            scrollGridVertical.FixedPosition(0F);
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
            go_NoneView = transform.Find("Animator/View_None").gameObject;
            toggleGroup = transform.Find("Animator/View_Merge/ScrollView_Member/Viewport/Content").GetComponent<ToggleGroup>();
            scrollGridVertical = transform.Find("Animator/View_Merge/ScrollView_Member").gameObject.GetNeedComponent<ScrollGridVertical>();
            scrollGridVertical.AddCellListener(OnCellUpdateCallback);
            scrollGridVertical.AddCreateCellListener(OnCreateCellCallback);

            transform.Find("View_TipsBg01_Square/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/Btn_01").GetComponent<Button>().onClick.AddListener(OnClick_OK);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {

        }
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            list_BranchInfos.Clear();
            var branchMemberInfos = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.BranchMemberInfo;
            for (int i = 0, count = branchMemberInfos.Count; i < count; i++)
            {
                var branchMemberInfo = branchMemberInfos[i];
                if (branchMemberInfo.Id != curBranchId)
                {
                    list_BranchInfos.Add(branchMemberInfo);
                }
            }
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            go_NoneView.SetActive(list_BranchInfos.Count == 0);
            scrollGridVertical.SetCellCount(list_BranchInfos.Count);
        }
        /// <summary>
        /// 设置分会模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="branchInfo"></param>
        /// <param name="isSelect"></param>
        private void SetBranchItem(Transform tr, BranchInfo branchInfo, bool isSelect)
        {
            tr.name = branchInfo.Id.ToString();
            /// <summary> 选项脚本 </summary>
            Toggle toggle = tr.GetComponent<Toggle>();
            toggle.SetIsOnWithoutNotify(isSelect);
            /// <summary> 分会会长 </summary>
            Text text_Leader = tr.Find("Text_Head").GetComponent<Text>();
            var leader = Sys_Family.Instance.familyData.CheckLeader(branchInfo.Id);
            text_Leader.text = null == leader ? "--" : leader.Name.ToStringUtf8();
            /// <summary> 分会名称 </summary>
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            text_Name.text = branchInfo.Name.ToStringUtf8();
            /// <summary> 分会人数 </summary>
            Text text_Number = tr.Find("Text_Number").GetComponent<Text>();
            text_Number.text = string.Format("{0}/{1}", Sys_Family.Instance.familyData.GetMemberCount(branchInfo.Id), Sys_Family.Instance.familyData.familyBuildInfo.branchMemberNum);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_BranchMerge, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 确定
        /// </summary>
        private void OnClick_OK()
        {
            UIManager.HitButton(EUIID.UI_Family_BranchMerge, "OnClick_OK");
            uint id = GetSelectBranchId();
            if (id == 0) return;
            Sys_Family.Instance.SendGuildBranchMergeReq(curBranchId, id);
            CloseSelf();
        }
        /// <summary>
        /// 更新滚动子节点
        /// </summary>
        /// <param name="cell"></param>
        private void OnCellUpdateCallback(ScrollGridCell cell)
        {
            var branchInfo = list_BranchInfos[cell.index];
            SetBranchItem(cell.gameObject.transform, branchInfo, cell.index == selectIndex);
        }
        /// <summary>
        /// 创建模版回调
        /// </summary>
        /// <param name="cell"></param>
        private void OnCreateCellCallback(ScrollGridCell cell)
        {
            GameObject go = cell.gameObject;
            Toggle toggle = go.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((bool value) => { OnClick_BranchItem(cell, value); });
            toggles.Add(toggle);
        }
        /// <summary>
        /// 点击家族模版
        /// </summary>
        /// <param name="toggle"></param>
        private void OnClick_BranchItem(ScrollGridCell cell, bool value)
        {
            UIManager.HitButton(EUIID.UI_Family_BranchMerge, "OnClick_BranchItem");
            if (value)
            {
                selectIndex = cell.index;
            }
            else if (selectIndex == cell.index)
            {
                selectIndex = -1;
            }
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 当前单项框
        /// </summary>
        /// <returns></returns>
        private Toggle GetSelectToggle()
        {
            Toggle toggle_Item = null;
            var ActiveToggles = toggleGroup.ActiveToggles();
            foreach (var toggle in ActiveToggles)
            {
                if (toggle.isOn)
                {
                    toggle_Item = toggle;
                    break;
                }
            }
            return toggle_Item;
        }
        /// <summary>
        /// 获取分会ID
        /// </summary>
        /// <returns></returns>
        private uint GetSelectBranchId()
        {
            Toggle toggle = GetSelectToggle();
            uint id = 0;
            if (null != toggle)
            {
                uint.TryParse(toggle.name, out id);
            }
            return id;
        }
        #endregion
    }
}
