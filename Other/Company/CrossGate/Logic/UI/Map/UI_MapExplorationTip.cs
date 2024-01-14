using DG.Tweening;
using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    /// <summary> 地图探索提示 </summary>
    public class UI_MapExplorationTip : UIComponent
    {
        #region 界面组件
        /// <summary> 图标 </summary>
        private Image image_Icon;
        /// <summary> 标题 </summary>
        private Text text_title;
        /// <summary> 画布组 </summary>
        private CanvasGroup cg_Panel;
        /// <summary> 按钮 </summary>
        private Button button_Icon;
        #endregion
        #region 数据
        /// <summary> 当前npc标记 </summary>
        private ENPCMarkType curNPCMarkType;
        /// <summary> 存在时间 </summary>
        private float LimitedTime = 3f;
        /// <summary> 动画时间 </summary>
        private float AnimationTime = 1f;
        /// <summary> 动画序列 </summary>
        public Sequence sequence;
        #endregion
        #region 系统函数
        public UI_MapExplorationTip()
        {
            LimitedTime = uint.Parse(CSVParam.Instance.GetConfData(431).str_value);
        }
        protected override void Loaded()
        {
            base.Loaded();
            OnParseComponent();
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            StopAnimation();
            cg_Panel.alpha = 0;
            cg_Panel.blocksRaycasts = false;
            Sys_Exploration.Instance.eventEmitter.Trigger<Sys_Exploration.ETipType, bool>(Sys_Exploration.EEvents.NoticeViewState, Sys_Exploration.ETipType.IncompleteTip, true);
        }
        public override void Hide()
        {
            base.Hide();
            Sys_Exploration.Instance.eventEmitter.Trigger<Sys_Exploration.ETipType, bool>(Sys_Exploration.EEvents.NoticeViewState, Sys_Exploration.ETipType.IncompleteTip, false);
        }
        protected override void Update()
        {
            base.Update();
        }
        protected override void Refresh()
        {
            base.Refresh();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent()
        {
            image_Icon = transform.Find("Btn_01/Image").GetComponent<Image>();
            text_title = transform.Find("Btn_01/Text").GetComponent<Text>();
            cg_Panel = transform.GetComponent<CanvasGroup>();
            button_Icon = transform.Find("Btn_01").GetComponent<Button>();
            button_Icon.onClick.AddListener(OnClick_OpenMapView);
        }
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Exploration.Instance.eventEmitter.Handle<Sys_Exploration.ETipType, bool>(Sys_Exploration.EEvents.ShowOrHideView, OnShowOrHideView, toRegister);
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            CSVMapExplorationMark.Data cSVMapExplorationMarkData = CSVMapExplorationMark.Instance.GetConfData((uint)curNPCMarkType);
            if (null == cSVMapExplorationMarkData) return;

            ImageHelper.SetIcon(image_Icon, cSVMapExplorationMarkData.main_icon);
            text_title.text = LanguageHelper.GetTextContent(4528, LanguageHelper.GetTextContent(cSVMapExplorationMarkData.List_lan));
        }
        /// <summary>
        /// 播放动画打开界面
        /// </summary>
        public void PlayAnimation_OpenView()
        {
            StopAnimation();
            sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                curNPCMarkType = Sys_Exploration.Instance.curbehaviorOrder.curNPCMarkType;
                RefreshView();
                cg_Panel.blocksRaycasts = true;
            })
            .Append(DOTween.To(() => cg_Panel.alpha, x => cg_Panel.alpha = x, 1, AnimationTime))
            .AppendInterval(LimitedTime)
            .OnComplete(() =>
            {
                Sys_Exploration.Instance.curbehaviorOrder.TryShowOrHideView(false);
            });
            sequence.Play();
        }
        /// <summary>
        /// 播放动画关闭界面
        /// </summary>
        public void PlayAnimation_CloseView()
        {
            StopAnimation();
            sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => cg_Panel.alpha, x => cg_Panel.alpha = x, 0, AnimationTime))
            .AppendInterval(AnimationTime)
            .OnComplete(() =>
            {
                cg_Panel.blocksRaycasts = false;
                curNPCMarkType = ENPCMarkType.None;
                Sys_Exploration.Instance.curbehaviorOrder.CompleteOrder();
            });
            sequence.Play();
        }
        /// <summary>
        /// 暂停动画
        /// </summary>
        public void StopAnimation()
        {
            if (null != sequence)
            {
                //sequence.Pause();
                sequence.Kill();
            }
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 点击打开界面
        /// </summary>
        private void OnClick_OpenMapView()
        {
            UIManager.OpenUI(EUIID.UI_Map, false, new Sys_Map.ResMarkParameter((uint)curNPCMarkType));
            Sys_Exploration.Instance.curbehaviorOrder.CompleteOrder();
        }
        /// <summary>
        /// 显示或隐藏界面
        /// </summary>
        /// <param name="eTipType"></param>
        /// <param name="isShow"></param>
        private void OnShowOrHideView(Sys_Exploration.ETipType eTipType, bool isShow)
        {
            if (eTipType != Sys_Exploration.ETipType.IncompleteTip) return;

            if (isShow)
            {
                PlayAnimation_OpenView();
            }
            else
            {
                PlayAnimation_CloseView();
            }
        }
        #endregion
        #region 提供功能
        #endregion
    }
}

