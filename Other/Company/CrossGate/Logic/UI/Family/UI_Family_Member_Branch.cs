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
    /// <summary> 家族分会管理 </summary>
    public class UI_Family_Member_Branch : UIBase
    {
        #region 界面组件
        /// <summary> 设置界面 </summary>
        private GameObject go_SettingView;
        /// <summary> 设置移动节点 </summary>
        private GameObject go_SettingNode;
        /// <summary> 分会数量 </summary>
        private Text text_Number;
        /// <summary> 无分会节点 </summary>
        private GameObject go_NoneView;
        /// <summary> 无限滚动 </summary>
        private ScrollGridVertical scrollGridVertical;
        /// <summary> 当前选中的分会模版 </summary>
        private GameObject curSelectBranchItem { get; set; }
        #endregion
        #region 数据定义
        /// <summary> 分会信息 </summary>
        private List<GuildDetailInfo.Types.BranchInfo> list_BranchInfos = new List<GuildDetailInfo.Types.BranchInfo>();
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
            go_NoneView = transform.Find("Animator/View_Content/View_None").gameObject;
            go_SettingView = transform.Find("Animator/View_Setting").gameObject;
            go_SettingNode = transform.Find("Animator/View_Setting/ZoneSetting/View_Content").gameObject;
            text_Number = transform.Find("Animator/View_Content/View_Member/Text_Amount/Text").GetComponent<Text>();

            scrollGridVertical = transform.Find("Animator/View_Content/View_Member/ScrollView_Member").GetComponent<ScrollGridVertical>();
            scrollGridVertical.AddCellListener(OnCellUpdateCallback);
            scrollGridVertical.AddCreateCellListener(OnCreateCellCallback);

            transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/View_Content/View_Member/Button_Create").GetComponent<Button>().onClick.AddListener(OnClick_CreateBranch);

            transform.Find("Animator/View_Setting/ZoneSetting/View_Content/Button_Rename").GetComponent<Button>().onClick.AddListener(OnClick_BranchModifyName);
            transform.Find("Animator/View_Setting/ZoneSetting/View_Content/Button_Move").GetComponent<Button>().onClick.AddListener(OnClick_MoveBranch);
            transform.Find("Animator/View_Setting/ZoneSetting/View_Content/Button_dismiss").GetComponent<Button>().onClick.AddListener(OnClick_DismissBranch);
            transform.Find("Animator/View_Setting/Close").GetComponent<Button>().onClick.AddListener(OnClick_CloseSetting);
            go_SettingView.SetActive(false);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.CreateBranch, OnUpdateList, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.DestroyBranch, OnUpdateList, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateBranchName, OnUpdateList, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMemberStatus, OnUpdateList, toRegister);
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
                list_BranchInfos.Add(branchMemberInfo);
            }
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            int curBranchNum = list_BranchInfos.Count;
            uint maxBranchNum = Sys_Family.Instance.familyData.familyBuildInfo.buildBranchNum;
            text_Number.text = string.Format("{0}/{1}", curBranchNum, maxBranchNum);
            scrollGridVertical.SetCellCount(curBranchNum);
            go_NoneView.SetActive(curBranchNum == 0);
        }
        /// <summary>
        /// 设置分会模版
        /// </summary>
        /// <param name="tr"></param>
        private void SetBranchItem(Transform tr, GuildDetailInfo.Types.BranchInfo branchInfo)
        {
            tr.name = branchInfo.Id.ToString();
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
            UIManager.HitButton(EUIID.UI_Family_Branch, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 创建分会
        /// </summary>
        private void OnClick_CreateBranch()
        {
            UIManager.HitButton(EUIID.UI_Family_Branch, "OnClick_CreateBranch");
            UIManager.OpenUI(EUIID.UI_Family_BranchModifyName, false, null);
        }
        /// <summary>
        /// 进入分会
        /// </summary>
        /// <param name="tr"></param>
        private void OnClick_EnterBranch(Transform tr)
        {
            uint id = 0;
            uint.TryParse(tr.name, out id);
            UIManager.HitButton(EUIID.UI_Family_Branch, "OnClick_EnterBranch");
            UIManager.OpenUI(EUIID.UI_Family_BranchMember, false, id);
        }
        /// <summary>
        /// 设置分会
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="position"></param>
        private void OnClick_SetBranch(Transform tr, Vector3 position)
        {
            UIManager.HitButton(EUIID.UI_Family_Branch, "OnClick_SetBranch");
            curSelectBranchItem = tr.gameObject;
            go_SettingView.SetActive(true);
            go_SettingNode.transform.position = position;
        }
        /// <summary>
        /// 关闭设置界面
        /// </summary>
        private void OnClick_CloseSetting()
        {
            UIManager.HitButton(EUIID.UI_Family_Branch, "OnClick_CloseSetting");
            curSelectBranchItem = null;
            go_SettingView.SetActive(false);
        }
        /// <summary>
        /// 分会改名
        /// </summary>
        private void OnClick_BranchModifyName()
        {
            uint BranchId = 0;
            uint.TryParse(curSelectBranchItem?.name, out BranchId);
            UIManager.OpenUI(EUIID.UI_Family_BranchModifyName, false, BranchId);
            UIManager.HitButton(EUIID.UI_Family_Branch, "OnClick_BranchModifyName");
            OnClick_CloseSetting();
        }
        /// <summary>
        /// 移动分会
        /// </summary>
        private void OnClick_MoveBranch()
        {
            uint BranchId = 0;
            uint.TryParse(curSelectBranchItem?.name, out BranchId);
            UIManager.OpenUI(EUIID.UI_Family_BranchMerge, false, BranchId);
            UIManager.HitButton(EUIID.UI_Family_Branch, "OnClick_MoveBranch");
            OnClick_CloseSetting();
        }
        /// <summary>
        /// 解散分会
        /// </summary>
        private void OnClick_DismissBranch()
        {
            UIManager.HitButton(EUIID.UI_Family_Branch, "OnClick_DismissBranch");
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(10056);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                uint BranchId = 0;
                uint.TryParse(curSelectBranchItem?.name, out BranchId);
                Sys_Family.Instance.SendGuildDestroyBranchReq(BranchId);
                OnClick_CloseSetting();
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
        }
        /// <summary>
        /// 更新滚动子节点
        /// </summary>
        /// <param name="cell"></param>
        private void OnCellUpdateCallback(ScrollGridCell cell)
        {
            var branchInfo = list_BranchInfos[cell.index];
            SetBranchItem(cell.gameObject.transform, branchInfo);
        }
        /// <summary>
        /// 创建模版回调
        /// </summary>
        /// <param name="cell"></param>
        private void OnCreateCellCallback(ScrollGridCell cell)
        {
            GameObject go = cell.gameObject;
            Button button_Enter = go.transform.Find("Button_Enter").GetComponent<Button>();
            Button button_Set = go.transform.Find("Button_Set").GetComponent<Button>();
            button_Enter.onClick.AddListener(() => { OnClick_EnterBranch(go.transform); });
            button_Set.onClick.AddListener(() => { OnClick_SetBranch(go.transform, button_Set.transform.position); });
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