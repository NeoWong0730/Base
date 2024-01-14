using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;

namespace Logic
{
    /// <summary> 家族合并 </summary>
    public class UI_Family_Hall_Merge : UIBase
    {
        #region 界面组件
        /// <summary> 家族状态 </summary>
        private Text text_State;
        /// <summary> 查看 </summary>
        private Button button_Look;
        /// <summary> 提示文本 </summary>
        private Text text_Tips;
        /// <summary> 提示界面 </summary>
        private GameObject go_TipsView;
        //需要处理合并申请红点
        private GameObject redPointG0;
        #endregion
        #region 数据定义
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
            text_State = transform.Find("Animator/View_Content/Image_Title/Text_Des").GetComponent<Text>();
            button_Look = transform.Find("Animator/View_Content/Image_Des/Button_Look").GetComponent<Button>();
            text_Tips = transform.Find("Animator/View_Content/Image_Des/Text_Tips1").GetComponent<Text>();
            transform.Find("Animator/View_Content/Image_Des/Text_Tips2").gameObject.SetActive(false);
            go_TipsView = transform.Find("Animator/View_Content/View_rule").gameObject;
            redPointG0 = transform.Find("Animator/View_Content/Btn_Apply/Image_Dot").gameObject;
            button_Look.onClick.AddListener(OnClick_CheckMergeAgreement);
            transform.Find("Animator/View_Content/Btn_Apply").GetComponent<Button>().onClick.AddListener(OnClick_MergeApply);
            transform.Find("Animator/View_Content/Btn_Merge").GetComponent<Button>().onClick.AddListener(OnClick_MergeLaunch);
            transform.Find("Animator/View_TipsBg04_Smallest/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/View_Content/Image_Title/Button").GetComponent<Button>().onClick.AddListener(OnClick_OpenTipsView);
            transform.Find("Animator/View_Content/View_rule/close").GetComponent<Button>().onClick.AddListener(OnClick_CloseTipsView);

        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMergeStatus, OnUpdateMergeStatus, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateApplyMergedList, OnUpdateMergeStatus, toRegister);
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            uint GuildStatus = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildStatus;
            uint lanId = 0, tipId = 0;
            switch (GuildStatus)
            {
                case 1: { lanId = 10088; tipId = 10627; } break;
                case 2: { lanId = 10089; tipId = 10628; } break;
                case 3: { lanId = 10090; tipId = 10629; } break;
            }
            text_State.text = LanguageHelper.GetTextContent(lanId);
            text_Tips.text = LanguageHelper.GetTextContent(tipId);

            var InitiativeMerge = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.InitiativeMerge;
            bool isMerged = !(InitiativeMerge.DstId == 0 || InitiativeMerge.OtherId == 0);
            if (isMerged)
            {
                button_Look.gameObject.SetActive(true);
            }
            else
            {
                button_Look.gameObject.SetActive(false);
            }
            redPointG0.SetActive(Sys_Family.Instance.HasMergeApply());
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_Merge, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 查看合并协议
        /// </summary>
        private void OnClick_CheckMergeAgreement()
        {
            UIManager.HitButton(EUIID.UI_Family_Merge, "OnClick_CheckMergeAgreement");
            UIManager.OpenUI(EUIID.UI_Family_MergeAgreement);
        }
        /// <summary>
        /// 申请合并
        /// </summary>
        private void OnClick_MergeApply()
        {
            UIManager.HitButton(EUIID.UI_Family_Merge, "OnClick_MergeApply");
            UIManager.OpenUI(EUIID.UI_Family_MergeApply);
        }
        /// <summary>
        /// 发起合并
        /// </summary>
        private void OnClick_MergeLaunch()
        {
            UIManager.HitButton(EUIID.UI_Family_Merge, "OnClick_MergeLaunch");
            UIManager.OpenUI(EUIID.UI_Family_MergeLaunch);
        }
        /// <summary>
        /// 打开提示界面
        /// </summary>
        private void OnClick_OpenTipsView()
        {
            UIManager.HitButton(EUIID.UI_Family_Merge, "OnClick_OpenTipsView");
            go_TipsView.SetActive(true);
        }
        /// <summary>
        /// 关闭提示界面
        /// </summary>
        private void OnClick_CloseTipsView()
        {
            UIManager.HitButton(EUIID.UI_Family_Merge, "OnClick_CloseTipsView");
            go_TipsView.SetActive(false);
        }
        /// <summary>
        /// 更新合并状态
        /// </summary>
        private void OnUpdateMergeStatus()
        {
            RefreshView();
        }
        #endregion
        #region 提供功能
        #endregion
    }
}