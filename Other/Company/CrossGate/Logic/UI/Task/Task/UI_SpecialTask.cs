using Logic.Core;
using UnityEngine;
using UnityEngine.UI;
using Table;
using DG.Tweening;

namespace Logic
{
    /// <summary> 特殊任务 </summary>
    public class UI_SpecialTask : UIComponent
    {
        #region 界面组件
        /// <summary> 界面1 </summary>
        private Transform tr_View1;
        /// <summary> 类型 </summary>
        private Text text_Type1;
        /// <summary> 标题 </summary>
        private Text text_Title1;
        /// <summary> 内容 </summary>
        private Text text_Content1;
        /// <summary> 界面2 </summary>
        private Transform tr_View2;
        /// <summary> 标题 </summary>
        private Text text_Title2;
        /// <summary> 内容 </summary>
        private Text text_Content2;
        /// <summary> 图标 </summary>
        private Image image_Icon;
        /// <summary> 边框 </summary>
        private Image image_Frame;
        /// <summary> 任务数量 </summary>
        private Text text_TaskNum;
        /// <summary> 进度条 </summary>
        private Slider slider_Progress;
        /// <summary> 文字进度 </summary>
        private Text text_Progress;
        /// <summary> 领奖特效 </summary>
        private GameObject go_Effect_ReceiveAwards;
        /// <summary> 画布组 </summary>
        private CanvasGroup cg_Panel;
        /// <summary> 打开界面特效 </summary>
        private GameObject go_EffectOpenView;
        #endregion
        #region 数据
        /// <summary> 特殊任务数据 </summary>
        private Sys_Exploration.ExplorationTipData explorationTipData
        {
            get;
            set;
        }
        /// <summary> 存在时间 </summary>
        private float LimitedTime = 0;
        /// <summary> 动画时间 </summary>
        private float AnimationTime = 1f;
        /// <summary> 动画序列 </summary>
        public Sequence sequence;
        #endregion
        #region 系统函数        
        protected override void Loaded()
        {
            base.Loaded();
            OnParseComponent();
            CSVParam.Data csv = CSVParam.Instance.GetConfData(271);
            LimitedTime = csv == null ? 5f : System.Convert.ToSingle(csv.str_value) / 1000f;
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
            go_Effect_ReceiveAwards.SetActive(false);
            go_EffectOpenView.SetActive(false);
            Sys_Exploration.Instance.eventEmitter.Trigger<Sys_Exploration.ETipType, bool>(Sys_Exploration.EEvents.NoticeViewState, Sys_Exploration.ETipType.ActivatitonTip, true);
        }
        public override void Hide()
        {
            base.Hide();
            Sys_Exploration.Instance.eventEmitter.Trigger<Sys_Exploration.ETipType, bool>(Sys_Exploration.EEvents.NoticeViewState, Sys_Exploration.ETipType.ActivatitonTip, false);
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
            tr_View1 = transform.Find("View01");
            text_Type1 = transform.Find("View01/Text_Type").GetComponent<Text>();
            text_Title1 = transform.Find("View01/Text_Type/Text_Name01").GetComponent<Text>();
            text_Content1 = transform.Find("View01/Text_Name02").GetComponent<Text>();
            tr_View2 = transform.Find("View02");
            text_Title2 = transform.Find("View02/Text_Name01").GetComponent<Text>();
            text_Content2 = transform.Find("View02/Text_Name02").GetComponent<Text>();
            image_Icon = transform.Find("View_Tiaozhan/Image").GetComponent<Image>();
            image_Frame = transform.Find("View_Tiaozhan/Image_bg").GetComponent<Image>();
            text_TaskNum = transform.Find("Text_Num").GetComponent<Text>();
            slider_Progress = transform.Find("Image_ProcessBG").GetComponent<Slider>();
            text_Progress = transform.Find("Image_ProcessBG/Text_Num").GetComponent<Text>();
            go_Effect_ReceiveAwards = transform.Find("Btn_01/UI_Fx").gameObject;
            cg_Panel = transform.GetComponent<CanvasGroup>();
            go_EffectOpenView = transform.Find("View_Tiaozhan/Image/Fx_ui_love02").gameObject;
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
        public void RefreshView()
        {
            if (null == explorationTipData) return;

            tr_View1.gameObject.SetActive(true);
            tr_View2.gameObject.SetActive(false);
            go_Effect_ReceiveAwards.SetActive(false);

            text_Type1.text = LanguageHelper.GetTextContent(explorationTipData.cSVMapExplorationMarkData.type_lan);

            switch (explorationTipData.markType)
            {
                case 5://爱心任务
                case 6://挑战任务
                {
                    text_Title1.text = "";
                    CSVNpc.Data csvNpc = CSVNpc.Instance.GetConfData(explorationTipData.npcId);
                    if (csvNpc != null) {
                        var taskId = Sys_Npc.Instance.GetTaskId(csvNpc);
                        var csvTask = CSVTask.Instance.GetConfData(taskId);
                        text_Title1.text = csvTask == null ? "" : LanguageHelper.GetTaskTextContent(csvTask.taskName);
                    }
                }
                    break;
                default:
                    text_Title1.text = LanguageHelper.GetNpcTextContent(explorationTipData.cSVNpcData.name);
                    break;
            }
            text_Content1.text = LanguageHelper.GetTextContent(4507, LanguageHelper.GetTextContent(explorationTipData.cSVMapExplorationRewardData.title_lan));
            ImageHelper.SetIcon(image_Icon, explorationTipData.cSVMapExplorationMarkData.main_icon);
            image_Icon.SetNativeSize();
            text_TaskNum.text = string.Format("{0}/{1}", explorationTipData.subCurNum, explorationTipData.subTargetNum);
            slider_Progress.value = explorationTipData.totalTargetNum == 0 ? 0 : (float)explorationTipData.totalCurNum / (float)explorationTipData.totalTargetNum;
            text_Progress.text = string.Format("{0}%", System.Math.Round((float)explorationTipData.totalCurNum / (float)explorationTipData.totalTargetNum * 100, 1));
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
                explorationTipData = Sys_Exploration.Instance.curbehaviorOrder.curExplorationTipData;
                RefreshView();
                cg_Panel.blocksRaycasts = true;
                DOTween.To(() => cg_Panel.alpha, x => cg_Panel.alpha = x, 1, AnimationTime);
            })//播放打开动画
            .AppendInterval(AnimationTime / 3f)
            .AppendCallback(() => { go_EffectOpenView.SetActive(false); go_EffectOpenView.SetActive(true); })
            .AppendInterval(AnimationTime * 2 / 3f)
            .AppendInterval(LimitedTime)
            .OnComplete(() =>
            {
                if (Sys_Exploration.Instance.curbehaviorOrder.eTipType == Sys_Exploration.ETipType.ActivatitonTip)
                {
                    Sys_Exploration.Instance.curbehaviorOrder.TryShowOrHideView(false);
                }
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
            sequence.AppendCallback(() =>
            {
                DOTween.To(() => cg_Panel.alpha, x => cg_Panel.alpha = x, 0, AnimationTime);
            })
            .AppendInterval(AnimationTime)
            .OnComplete(() =>
            {
                go_Effect_ReceiveAwards.SetActive(false);
                go_EffectOpenView.SetActive(false);
                explorationTipData = null;
                cg_Panel.blocksRaycasts = false;
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
        /// 显示或隐藏界面
        /// </summary>
        /// <param name="eTipType"></param>
        /// <param name="isShow"></param>
        private void OnShowOrHideView(Sys_Exploration.ETipType eTipType, bool isShow)
        {
            if (!(eTipType == Sys_Exploration.ETipType.ActivatitonTip)) return;

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

