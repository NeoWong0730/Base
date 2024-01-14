using DG.Tweening;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Lib.Core;

namespace Logic
{
    /// <summary> 新功能界面 </summary>
    public class UI_Newfunction : UIBase
    {
        #region 界面组件
        /// <summary> 名称 </summary>
        private Text text_Name;
        /// <summary> 图标节点 </summary>
        private RectTransform rt_IconNode;
        /// <summary> 动画脚本 </summary>
        private Animator animator;
        #endregion
        #region 变量
        /// <summary>
        /// 功能开放数据
        /// </summary>
        public Sys_FunctionOpen.FunctionOpenData functionOpenData
        {
            get;
            set;
        }
        /// <summary>
        /// 动画相关数据
        /// </summary>
        private static string kEnterStateName = "Open";
        private static string kExitStateName = "Close";
        private float fEnterTime = 0;
        private float fExitTime = 0;
        private float fMoveTime = 0.5f;
        private float fScale = 0.69f;
        private float fWaitingTime = 0.3f;

        private AsyncOperationHandle<GameObject> requestRef;
        private string sAssetPath = null;

        private Transform _target;
        /// <summary> 动画序列 </summary>
        public Sequence sequence;
        #endregion
        #region 系统函数
        protected override void OnInit()
        {
        }
        protected override void OnLoaded()
        {
            OnParseComponent();
            GetAnimatorData();
        }
        protected override void OnOpen(object arg)
        {
            functionOpenData = (Sys_FunctionOpen.FunctionOpenData)arg;
        }
        protected override void OnShow()
        {
            RequestAsset();
            animator.gameObject.SetActive(false);
            StopMove();
        }
        protected override void OnHide()
        {
        }
        protected override void OnClose()
        {
            if (functionOpenData != null)
                functionOpenData.CompletedFunctionOpen();

            RecoveryAsset();
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
            rt_IconNode = transform.Find("Animator/View_Newfunction/View_FunctionIcon/Image_Bottom/MoveNode") as RectTransform;
            text_Name = transform.Find("Animator/View_Newfunction/Image_Bottom1/Text_Name").GetComponent<Text>();
            animator = transform.Find("Animator").GetComponent<Animator>();
        }
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="toRegister"></param>
        protected void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.StopFunctionOpen, OnStopFunctionOpen, toRegister);
        }
        /// <summary>
        /// 请求资源
        /// </summary>
        private void RequestAsset()
        {
            if (null == functionOpenData || string.IsNullOrEmpty(functionOpenData.cSVFunctionOpenData.Active_Icon))
            {
                OnClick_Close();
            }
            else
            {
                if (!requestRef.IsValid() || sAssetPath != functionOpenData.cSVFunctionOpenData.Active_Icon)
                {
                    RecoveryAsset();
                    sAssetPath = functionOpenData.cSVFunctionOpenData.Active_Icon;
                    AddressablesUtil.InstantiateAsync(ref requestRef, sAssetPath, RequestRef_Completed);
                }
                else
                {
                    RequestRef_Completed(requestRef);
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        private void RecoveryAsset()
        {
            AddressablesUtil.ReleaseInstance(ref requestRef, RequestRef_Completed);
            sAssetPath = null;
        }
        /// <summary>
        /// 完整资源加载
        /// </summary>
        /// <param name="handle"></param>
        private void RequestRef_Completed(AsyncOperationHandle<GameObject> handle)
        {
            if (null == handle.Result) return;

            RefreshView(handle.Result);
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        public void RefreshView(GameObject clone)
        {
            if (null == functionOpenData)
                return;

            animator.gameObject.SetActive(true);
            clone.transform.SetParent(rt_IconNode);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localScale = Vector3.one;

            text_Name.text = LanguageHelper.GetTextContent(functionOpenData.cSVFunctionOpenData.Active_Name);

            if (string.IsNullOrEmpty(functionOpenData.cSVFunctionOpenData.Active_Target))
            {
                _target = null;
            }
            else
            {
                Transform target = UIManager.mRoot.Find(functionOpenData.cSVFunctionOpenData.Active_Target);
                if (null != target && null != target.parent && target.parent.gameObject.activeInHierarchy == false)
                {
                    target = UIManager.mRoot.Find(functionOpenData.cSVFunctionOpenData.Hide_Target);
                }
                _target = target;
            }

            CanvasGroup canvasGroup = animator.GetComponent<CanvasGroup>();
            if (null != canvasGroup)
            {
                //Debug.LogError(functionOpenData.cSVFunctionOpenData.Lock.ToString());
                canvasGroup.interactable = functionOpenData.cSVFunctionOpenData.Lock;
                canvasGroup.blocksRaycasts = functionOpenData.cSVFunctionOpenData.Lock;
                //_target.gameObject.SetActive(true);
                //canvasGroup.alpha = 0;
            }

            PlayAnimator(clone.transform);
        }
        /// <summary>
        /// 播放界面动画
        /// </summary>
        /// <param name="clone"></param>
        /// <param name="_target"></param>
        public void PlayAnimator(Transform clone)
        {
            rt_IconNode.anchoredPosition3D = Vector3.zero;
            rt_IconNode.localScale = Vector3.one;

            try
            {
                if (null != sequence && sequence.IsPlaying())
                {
                    sequence.Pause();
                    sequence.Kill();
                }

                sequence = DOTween.Sequence();
                if (null == _target)
                {
                    sequence.AppendCallback(() => { animator.Play(kEnterStateName, -1, 0); });//播放打开动画
                    sequence.AppendInterval(fEnterTime + fMoveTime);
                    sequence.AppendCallback(() => { animator.Play(kExitStateName, -1, 0); }); //播放关闭动画
                    sequence.AppendCallback(() =>
                    {
                        var childList = rt_IconNode.GetComponentsInChildren<Image>();
                        foreach (Image child in childList)
                            child.DOFade(0f, fExitTime);
                    });
                    sequence.AppendInterval(fExitTime);
                }
                else
                {
                    sequence.AppendCallback(() => { animator.Play(kEnterStateName, -1, 0); });//播放打开动画
                    sequence.AppendInterval(fEnterTime + fMoveTime);
                    sequence.AppendCallback(() => { animator.Play(kExitStateName, -1, 0); }); //播放关闭动画
                    sequence.AppendInterval(fExitTime);
                    sequence.AppendCallback(() =>
                    {
                        CanvasGroup canvasGroup = _target.GetComponent<CanvasGroup>();
                        if (null != canvasGroup)
                        {
                            _target.gameObject.SetActive(true);
                            canvasGroup.alpha = 0;
                        }
                        GridLayoutGroup gridLayoutGroup = _target.GetComponentInParent<GridLayoutGroup>();
                        if (null != gridLayoutGroup)
                        {
                            gridLayoutGroup.CalculateLayoutInputHorizontal();
                            gridLayoutGroup.CalculateLayoutInputVertical();
                            gridLayoutGroup.SetLayoutHorizontal();
                            gridLayoutGroup.SetLayoutVertical();
                        }
                        RectTransform rectTransform = _target.GetComponent<RectTransform>();
                        if (null != rectTransform)
                        {
                            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
                        }
                    }); //播放关闭动画
                    sequence.AppendInterval(0.1f);
                    sequence.AppendCallback(() =>
                    {
                        Vector3 targetPosition = _target.position;
                        rt_IconNode.transform.DOMove(targetPosition, fMoveTime);
                        rt_IconNode.transform.DOScale(new Vector3(fScale, fScale, fScale), fMoveTime);
                    });
                    sequence.AppendInterval(fMoveTime);
                    sequence.AppendCallback(() =>
                    {
                        if (null != clone)
                        {
                            Transform trEffect = clone.transform.Find("Fx_ui_love01");
                            if (null != trEffect)
                            {
                                trEffect.gameObject.SetActive(false);
                                trEffect.gameObject.SetActive(true);
                            }
                        }
                    });
                    sequence.AppendInterval(fWaitingTime);
                }
                sequence.OnComplete(OnTweenComplete);
                sequence.Play();
            }
            catch (System.Exception e)
            {
                OnTweenComplete();
            }
        }

        private void OnTweenComplete()
        {
            OnClick_Close();
            functionOpenData.CompletedFunctionOpen();

            if (null != _target)
            {
                CanvasGroup canvasGroup = _target.GetComponent<CanvasGroup>();
                if (null != canvasGroup)
                    canvasGroup.alpha = 1f;
            }
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            CloseSelf(true);
        }
        /// <summary>
        /// 暂停功能开启
        /// </summary>
        /// <param name="funcData"></param>
        public void OnStopFunctionOpen(Sys_FunctionOpen.FunctionOpenData funcData)
        {
            if (null != functionOpenData && functionOpenData == funcData)
            {
                functionOpenData = null;
                OnClick_Close();
            }
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 得到动画数据
        /// </summary>
        private void GetAnimatorData()
        {
            if (null == animator || animator.runtimeAnimatorController == null)
            {
                fEnterTime = 0;
                fExitTime = 0;
                return;
            }

            AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
            AnimationClip enterClip = null;
            AnimationClip exitClip = null;

            for (int i = 0; i < animationClips.Length; ++i)
            {
                string clipName = animationClips[i].name;
                if (clipName.EndsWith(kEnterStateName, System.StringComparison.Ordinal))
                {
                    enterClip = animationClips[i];
                }
                else if (clipName.EndsWith(kExitStateName, System.StringComparison.Ordinal))
                {
                    exitClip = animationClips[i];
                }

                if (enterClip != null && exitClip != null)
                {
                    break;
                }
            }

            if (enterClip != null && exitClip != null)
            {
                fEnterTime = fEnterTime < enterClip.length ? enterClip.length : fEnterTime;
                fExitTime = fExitTime < exitClip.length ? exitClip.length : fExitTime;
            }
            else
            {
                fEnterTime = 0;
                fExitTime = 0;
            }
        }
        /// <summary>
        /// 角色中断移动
        /// </summary>
        public void StopMove()
        {
            Sys_Task.Instance.StopAutoTask(true);
        }
        #endregion
    }
}