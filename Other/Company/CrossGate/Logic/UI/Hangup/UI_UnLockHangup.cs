using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;

namespace Logic
{
    /// <summary> 解锁挂机 </summary>
    public class UI_UnLockHangup : UIBase
    {
        #region 界面组件
        /// <summary> 标题 </summary>
        private Text text_title;
        /// <summary> 动画 </summary>
        private Animator animator;
        #endregion
        #region 数据定义
        /// <summary> 层Id </summary>
        private uint layerId = 0;
        /// <summary> 动画计时器 </summary>
        private Timer aniTimer;
        #endregion
        #region 系统函数
        protected override void OnInit()
        {
        }
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnDestroy()
        {

        }
        protected override void OnOpen(object arg)
        {
            layerId = arg == null ? 0 : System.Convert.ToUInt32(arg);
        }
        protected override void OnOpened()
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
            text_title = transform.Find("Animator/Image_BG/Text").GetComponent<Text>();
            animator = transform.Find("Animator").GetComponent<Animator>();
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {

        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            CSVHangupLayerStage.Data cSVHangupLayerStageData = CSVHangupLayerStage.Instance.GetConfData(layerId);
            if (null == cSVHangupLayerStageData)
                return;
            CSVHangup.Data cSVHangupData = CSVHangup.Instance.GetConfData(cSVHangupLayerStageData.Hangupid);
            if (null == cSVHangupData)
                return;

            text_title.text = LanguageHelper.GetTextContent(2104010, LanguageHelper.GetTextContent(cSVHangupData.HangupName) + LanguageHelper.GetTextContent(cSVHangupLayerStageData.Name));
            OpenAnimator();
        }
        /// <summary>
        /// 打开动画
        /// </summary>
        private void OpenAnimator()
        {
            animator.Play("Open", -1, 0);
            aniTimer?.Cancel();
            // 时间从UIAniamtorControl获取
            aniTimer = Timer.Register(0.1f + 2f, () =>
             {
                 CloseAnimator();
             }, null, false, false);
        }
        /// <summary>
        /// 关闭动画
        /// </summary>
        private void CloseAnimator()
        {
            animator.Play("Close", -1, 0);
            aniTimer?.Cancel();
            aniTimer = Timer.Register(0.1f, () =>
            {
                OnClick_Close();
            }, null, false, false);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            CloseSelf();
        }
        #endregion
        #region 提供功能
        #endregion
    }
}