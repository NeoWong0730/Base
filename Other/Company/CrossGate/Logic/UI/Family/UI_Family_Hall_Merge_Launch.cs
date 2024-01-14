using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Packet;

namespace Logic
{
    /// <summary> 家族发起合并 </summary>
    public class UI_Family_Hall_Merge_Launch : UIBase
    {
        #region 界面组件
        /// <summary> 滚动区域 </summary>
        private ScrollRect scrollRect_Family;
        /// <summary> 输入查询家族 </summary>
        private InputField inputField_Query;
        /// <summary> 清理查询内容 </summary>
        private Button button_ClearQuery;
        /// <summary> 无限滚动 </summary>
        private ScrollGridVertical scrollGridVertical;
        /// <summary> 选项组 </summary>
        private ToggleGroup toggleGroup;
        #endregion
        #region 数据定义
        /// <summary> 查询列表 </summary>
        private List<BriefInfo> list_Query = new List<BriefInfo>();
        /// <summary> 是否查询中 </summary>
        private bool isQuerying = false;
        /// <summary> 选中家族下标 </summary>
        private int selectIndex = -1;
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
            Sys_Family.Instance.familyData.queryFamilyInfo.Clear();
            ApplyFamilyList();
            isQuerying = false;
            selectIndex = -1;
            scrollGridVertical.FixedPosition(0F);
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
            scrollRect_Family = transform.Find("Animator/View_Content/View_Merge/ScrollView_Member").GetComponent<ScrollRect>();
            inputField_Query = transform.Find("Animator/View_Content/View_Below/InputField_Describe").GetComponent<InputField>();
            button_ClearQuery = transform.Find("Animator/View_Content/View_Below/InputField_Describe/Button_Delete").GetComponent<Button>();
            toggleGroup = transform.Find("Animator/View_Content/View_Merge/ScrollView_Member/Viewport/Content").GetComponent<ToggleGroup>();
            scrollGridVertical = transform.Find("Animator/View_Content/View_Merge/ScrollView_Member").GetComponent<ScrollGridVertical>();
            scrollGridVertical.AddCellListener(OnCellUpdateCallback);
            scrollGridVertical.AddCreateCellListener(OnCreateCellCallback);

            Lib.Core.EventTrigger.Get(scrollRect_Family.gameObject).onDragEnd += OnDragEnd;
            button_ClearQuery.GetComponent<Button>().onClick.AddListener(OnClick_ClearQuery);
            transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/View_Content/View_Below/Button_Look").GetComponent<Button>().onClick.AddListener(OnClick_QueryFamily);
            transform.Find("Animator/View_Content/View_Below/Btn_Refresh").GetComponent<Button>().onClick.AddListener(OnClick_Refresh);
            button_ClearQuery.gameObject.SetActive(false);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateFamilyList, OnUpdateFamilyList, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.GetQueryFamilyListRes, OnGetQueryFamilyListRes, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateApplyFamilyList, OnUpdateApplyFamilyList, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.RoleInfo>(Sys_Society.EEvents.OnGetBriefInfoSuccess, OnGetBriefInfoSuccess, toRegister);
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            scrollGridVertical.SetCellCount(list_Query.Count);
            button_ClearQuery.gameObject.SetActive(isQuerying);
        }
        /// <summary>
        /// 设置家族模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="briefInfo"></param>
        /// <param name="isSelect"></param>
        private void SetFamilyItem(Transform tr, BriefInfo briefInfo, bool isSelect)
        {
            tr.name = briefInfo.GuildId.ToString();
            /// <summary> 单选按钮 </summary>
            Toggle toggle = tr.GetComponent<Toggle>();
            toggle.SetIsOnWithoutNotify(isSelect);
            /// <summary> 编号 </summary>
            Text text_ID = tr.Find("Text_Number").GetComponent<Text>();
            text_ID.text = (briefInfo.GuildId % 100000000UL).ToString();
            /// <summary> 玩家名称 </summary>
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            text_Name.text = briefInfo.GuildName.ToStringUtf8();
            /// <summary> 玩家等级 </summary>
            Text text_Lv = tr.Find("Text_Level").GetComponent<Text>();
            text_Lv.text = briefInfo.GuildLvl.ToString();
            /// <summary> 成员数量 </summary>
            Text text_Member = tr.Find("Text_Amount").GetComponent<Text>();
            text_Member.text = string.Format("{0}/{1}", briefInfo.MemberCount.ToString(), briefInfo.MemberMax.ToString());
            /// <summary> 家族族长 </summary>
            Text text_Patriarch = tr.Find("Text_Head").GetComponent<Text>();
            text_Patriarch.text = briefInfo.LeaderName.ToStringUtf8();
            /// <summary> 聊天按钮 </summary>
            GameObject go_Chat = tr.Find("Button_Chat").gameObject;
            go_Chat.SetActive(isSelect);
            /// <summary> 合并按钮 </summary>
            GameObject go_Merge = tr.Find("Button_Merge").gameObject;
            go_Merge.SetActive(isSelect);
            /// <summary> 图标标记 </summary>
            GameObject go_Icon = tr.Find("Image_Icon").gameObject;
            go_Icon.SetActive(false);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_MergeLaunch, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 滚动拖拽结束事件
        /// </summary>
        /// <param name="vector2"></param>
        private void OnDragEnd(GameObject go)
        {
            if (isQuerying) return;

            if (scrollRect_Family.velocity.y > 1f)
            {
                ApplyFamilyList();
            }
        }
        /// <summary>
        /// 查询家族
        /// </summary>
        private void OnClick_QueryFamily()
        {
            UIManager.HitButton(EUIID.UI_Family_MergeLaunch, "OnClick_QueryFamily");
            Sys_Family.Instance.SendGuildFindGuildReq(inputField_Query.text);
        }
        /// <summary>
        /// 清除查询
        /// </summary>
        private void OnClick_ClearQuery()
        {
            UIManager.HitButton(EUIID.UI_Family_MergeLaunch, "OnClick_ClearQuery");
            isQuerying = false;
            OnUpdateFamilyList();
        }
        /// <summary>
        /// 刷新
        /// </summary>
        private void OnClick_Refresh()
        {
            UIManager.HitButton(EUIID.UI_Family_MergeLaunch, "OnClick_Refresh");
            scrollGridVertical.FixedPosition(0f);
        }
        /// <summary>
        /// 更新滚动子节点
        /// </summary>
        /// <param name="cell"></param>
        private void OnCellUpdateCallback(ScrollGridCell cell)
        {
            BriefInfo briefInfo = list_Query[cell.index];
            SetFamilyItem(cell.gameObject.transform, briefInfo, cell.index == selectIndex);
        }
        /// <summary>
        /// 创建模版回调
        /// </summary>
        /// <param name="cell"></param>
        private void OnCreateCellCallback(ScrollGridCell cell)
        {
            GameObject go = cell.gameObject;
            Toggle toggle = go.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((value) =>
            {
                OnValueChanged_FamilyItem(cell, value);
            });
            go.transform.Find("Button_Chat").GetComponent<Button>().onClick.AddListener(() => { OnClick_Chat(go); });
            go.transform.Find("Button_Merge").GetComponent<Button>().onClick.AddListener(() => { OnClick_Launch(go); });
        }
        /// <summary>
        /// 点击家族模版
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="value"></param>
        private void OnValueChanged_FamilyItem(ScrollGridCell cell, bool value)
        {
            UIManager.HitButton(EUIID.UI_Family_MergeLaunch, "OnValueChanged_FamilyItem");
            if (value)
            {
                selectIndex = cell.index;
            }
            else if (selectIndex == cell.index)
            {
                selectIndex = -1;
            }
            scrollGridVertical.RefreshOneCell(cell.gameObject);
        }
        /// <summary>
        /// 聊天
        /// </summary>
        /// <param name="go"></param>
        public void OnClick_Chat(GameObject go)
        {
            UIManager.HitButton(EUIID.UI_Family_MergeLaunch, "OnClick_Chat");
            ulong id;
            if (!ulong.TryParse(go.name, out id)) return;
            BriefInfo briefInfo = list_Query.Find(x => x.GuildId == id);
            if (null == briefInfo) return;

            if (briefInfo.LeaderId == Sys_Role.Instance.RoleId)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10676));
                return;
            }

            Sys_Society.Instance.ReqGetBriefInfo(briefInfo.LeaderId.ToString());
        }
        /// <summary>
        /// 发起合并
        /// </summary>
        /// <param name="go"></param>
        public void OnClick_Launch(GameObject go)
        {
            UIManager.HitButton(EUIID.UI_Family_MergeLaunch, "OnClick_Launch");
            if (Sys_Family.Instance.familyData.isMerging)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10073));
                return;
            }
            ulong id;
            if (!ulong.TryParse(go.name, out id)) return;
            if (id == Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildGameInfoNtf.GuildId)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10095));
                return;
            }
            BriefInfo briefInfo = list_Query.Find(x => x.GuildId == id);
            UIManager.OpenUI(EUIID.UI_Family_MergeConfirm, false, briefInfo);
        }
        /// <summary>
        /// 更新家族列表
        /// </summary>
        private void OnUpdateFamilyList()
        {
            var list = Sys_Family.Instance.familyData.queryFamilyInfo.familyList;
            list_Query.Clear();
            list_Query.AddRange(list);
            RefreshView();
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        private void OnGetQueryFamilyListRes()
        {
            isQuerying = true;
            var list = Sys_Family.Instance.familyData.queryFamilyInfo.queryList;
            list_Query.Clear();
            list_Query.AddRange(list);
            RefreshView();
        }
        /// <summary>
        /// 更新申请列表
        /// </summary>
        private void OnUpdateApplyFamilyList()
        {
            RefreshView();
        }
        /// <summary>
        /// 获取玩家详细信息回调
        /// </summary>
        /// <param name="roleInfo"></param>
        private void OnGetBriefInfoSuccess(Sys_Society.RoleInfo roleInfo)
        {
            Sys_Society.Instance.OpenPrivateChat(roleInfo);
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 申请家族列表
        /// </summary>
        private void ApplyFamilyList()
        {
            ulong targetId = Sys_Family.Instance.familyData.queryFamilyInfo.GetFamilyListLastId() + 1;
            Sys_Family.Instance.SendGuildGetGuildListReq(targetId);
        }
        #endregion
    }
}