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
    /// <summary> 家族申请列表 </summary>
    public class UI_Family_Member_ApplyList : UIBase
    {
        #region 界面组件
        /// <summary> 自动接收 </summary>
        private Toggle toggle_AutoReceive;
        /// <summary> 家族人数 </summary>
        private Text text_Member;
        /// <summary> 输入等级 </summary>
        private InputField inputField_Level;
        /// <summary> 无申请节点 </summary>
        private GameObject go_NoApplyNode;
        /// <summary> 规则界面 </summary>
        private GameObject go_RuleView;
        /// <summary> 无限滚动 </summary>
        private ScrollGridVertical scrollGridVertical;
        #endregion
        #region 数据定义
        /// <summary> 申请成员 </summary>
        private List<ApplyMember> list_ApplyMember = new List<ApplyMember>();
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
            RefreshView();
            OnClick_ClearList();
            OnClick_RefreshList();
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
            toggle_AutoReceive = transform.Find("Animator/View_Content/View_Below/Toggle").GetComponent<Toggle>();
            text_Member = transform.Find("Animator/View_Content/Text_Amount/Text").GetComponent<Text>();
            inputField_Level = transform.Find("Animator/View_Content/View_Below/View_Grade/InputField_Number").GetComponent<InputField>();
            go_NoApplyNode = transform.Find("Animator/View_Content/View_None").gameObject;
            go_RuleView = transform.Find("Animator/View_Content/View_rule").gameObject;
            scrollGridVertical = transform.Find("Animator/View_Content/View_Member/ScrollView_Member").GetComponent<ScrollGridVertical>();
            scrollGridVertical.AddCellListener(OnCellUpdateCallback);
            scrollGridVertical.AddCreateCellListener(OnCreateCellCallback);

            transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/View_Content/View_Below/View_Grade/Button").GetComponent<Button>().onClick.AddListener(OnClick_OpenTipsView);
            transform.Find("Animator/View_Content/View_rule/close").GetComponent<Button>().onClick.AddListener(OnClick_CloseTipsView);
            transform.Find("Animator/View_Content/View_Below/Button_Clear").GetComponent<Button>().onClick.AddListener(OnClick_ClearList);
            transform.Find("Animator/View_Content/View_Below/Button_Refresh").GetComponent<Button>().onClick.AddListener(OnClick_RefreshList);

            toggle_AutoReceive.onValueChanged.AddListener(OnChanged_AutoReceive);
            inputField_Level.onValueChanged.AddListener(OnChanged_InputLv);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.GetApplyMember, OnApplyMember, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateApplyMember, OnApplyMember, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateSetApplyInfo, OnUpdateSetApplyInfo, toRegister);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {
            list_ApplyMember.Clear();
            var applyMembers = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildApplyMemberAck.List;
            for (int i = 0, count = applyMembers.Count; i < count; i++)
            {
                var applyMember = applyMembers[i];
                list_ApplyMember.Add(applyMember);
            }
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            text_Member.text = string.Format("{0}/{1}",
                Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetMemberInfoAck.Member.Count,
                Sys_Family.Instance.familyData.familyBuildInfo.membershipCap.ToString());
            inputField_Level.SetTextWithoutNotify(Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.MinAutoLvl.ToString());
            toggle_AutoReceive.SetIsOnWithoutNotify(Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AutoAgree);
        }
        /// <summary>
        /// 设置申请列表
        /// </summary>
        private void SetApplyList()
        {
            scrollGridVertical.SetCellCount(list_ApplyMember.Count);
            go_NoApplyNode.SetActive(list_ApplyMember.Count == 0);
        }
        /// <summary>
        /// 设置申请模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="applyMember"></param>
        private void SetApplyItem(Transform tr, ApplyMember applyMember)
        {
            tr.name = applyMember.RoleId.ToString();
            /// <summary> 玩家名称 </summary>
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            text_Name.text = applyMember.Name.ToStringUtf8();
            /// <summary> 玩家等级 </summary>
            Text text_Lv = tr.Find("Text_Level").GetComponent<Text>();
            text_Lv.text = applyMember.Lvl.ToString();
            /// <summary> 玩家职业 </summary>
            Image image_Job = tr.Find("Image_Prop").GetComponent<Image>();
            Text text_Job = tr.Find("Image_Prop/Text_Profession").GetComponent<Text>();
            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(applyMember.Occ);
            text_Job.text = null == cSVCareerData ? string.Empty : LanguageHelper.GetTextContent(cSVCareerData.name);
            ImageHelper.SetIcon(image_Job, null == cSVCareerData ? 0 : cSVCareerData.logo_icon);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_ApplyList, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 打开提示界面
        /// </summary>
        private void OnClick_OpenTipsView()
        {
            UIManager.HitButton(EUIID.UI_Family_ApplyList, "OnClick_OpenTipsView");
            go_RuleView.SetActive(true);
        }
        /// <summary>
        /// 关闭提示界面
        /// </summary>
        private void OnClick_CloseTipsView()
        {
            UIManager.HitButton(EUIID.UI_Family_ApplyList, "OnClick_CloseTipsView");
            go_RuleView.SetActive(false);
        }
        /// <summary>
        /// 清理列表
        /// </summary>
        private void OnClick_ClearList()
        {
            UIManager.HitButton(EUIID.UI_Family_ApplyList, "OnClick_ClearList");
            list_ApplyMember.Clear();
            SetApplyList();
        }
        /// <summary>
        /// 刷新列表
        /// </summary>
        private void OnClick_RefreshList()
        {
            UIManager.HitButton(EUIID.UI_Family_ApplyList, "OnClick_RefreshList");
            Sys_Family.Instance.SendGuildGetGuildApplyMemberReq();
        }
        /// <summary>
        /// 操作模版
        /// </summary>
        /// <param name="go"></param>
        /// <param name="value"></param>
        public void OnClick_OperationItem(GameObject go, bool value)
        {
            ulong roleId = 0;
            if (!ulong.TryParse(go.name, out roleId)) return;
            UIManager.HitButton(EUIID.UI_Family_ApplyList, "OnClick_OperationItem:" + value.ToString());
            Sys_Family.Instance.SendGuildHandleApplyReq(roleId, value);
        }
        /// <summary>
        /// 输入等级被修改
        /// </summary>
        /// <param name="str"></param>
        private void OnChanged_InputLv(string str)
        {
            bool autoAgree = toggle_AutoReceive.isOn;
            uint minLv = uint.Parse(str);
            Sys_Family.Instance.SendGuildSetApplyInfoReq(autoAgree, minLv);
        }
        /// <summary>
        /// 自动接收被修改
        /// </summary>
        /// <param name="value"></param>
        private void OnChanged_AutoReceive(bool value)
        {
            bool autoAgree = value;
            uint minLv = uint.Parse(inputField_Level.text);
            Sys_Family.Instance.SendGuildSetApplyInfoReq(autoAgree, minLv);
        }
        /// <summary>
        /// 更新滚动子节点
        /// </summary>
        /// <param name="cell"></param>
        private void OnCellUpdateCallback(ScrollGridCell cell)
        {
            var applyMember = list_ApplyMember[cell.index];
            SetApplyItem(cell.gameObject.transform, applyMember);
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
        /// 获取申请列表
        /// </summary>
        private void OnApplyMember()
        {
            SetData();
            SetApplyList();
        }
        /// <summary>
        /// 设置被修改
        /// </summary>
        private void OnUpdateSetApplyInfo()
        {
            RefreshView();
        }
        #endregion
        #region 提供功能
        #endregion
    }
}