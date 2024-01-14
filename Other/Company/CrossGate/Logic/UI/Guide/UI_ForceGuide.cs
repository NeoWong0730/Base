using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Framework;
using System;
using UnityEngine.EventSystems;

namespace Logic
{
    /// <summary> 引导对话框 </summary>
    public class UI_ForceGuide : UIBase
    {
        #region 界面组件
        /// <summary> 背景遮罩 </summary>
        private Image image_BgMark;
        /// <summary> 镂空背景 </summary>
        private HollowOutBg hollowOutBg;
        /// <summary> 目标节点 </summary>
        private RectTransform rt_TargetNode;
        /// <summary> 对话节点 </summary>
        private RectTransform rt_DialogBoxNode;
        /// <summary> 背景 </summary>
        private Image image_Bg;
        /// <summary> 标题节点 </summary>
        private RectTransform rt_TitleNode;
        /// <summary> 标题 </summary>
        private Text text_Title;
        /// <summary> 半身像节点 </summary>
        private RectTransform rt_PortraitNode;
        /// <summary> 内容 </summary>
        private Text text_Content;
        /// <summary> 手势节点 </summary>
        private RectTransform rt_HandNode;
        /// <summary> 手势 </summary>
        private Image image_Hand;
        /// <summary> 跳过按钮 </summary>
        private Button button_Skip;
        /// <summary> 模型图片 </summary>
        private RawImage Model_rawImage;
        /// <summary> 特效字段 </summary>
        private Dictionary<string, GameObject> dict_Effect = new Dictionary<string, GameObject>();
        /// <summary> 安全区域主脚本 </summary>
        private SafeAreaController safeAreaController;
        /// <summary> 安全区域子脚本 </summary>
        private List<CanvasPropertyOverrider> list_CanvasPropertyOverrider = new List<CanvasPropertyOverrider>();
        /// <summary> 模型显示统一脚本 </summary>
        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        #endregion
        #region 数据定义
        /// <summary> 强制引导任务 </summary>
        public Sys_Guide.GuideTask guideTask
        {
            set;
            get;
        }
        /// <summary>  强制操作列表 </summary>
        public Sys_Guide.OperationsList operationsList
        {
            set;
            get;
        }
        /// <summary>  等待动画 </summary>
        public Sys_Guide.WaitAnimator waitAnimator
        {
            set;
            get;
        }
        /// <summary>  是否自动显示跳过按钮 </summary>
        private bool isAutoSkipBtn = false;
        /// <summary>  计时器 </summary>
        private float timer = 0;
        /// <summary>  角色动作列表 </summary>
        private List<uint> actionlist = new List<uint>();
        /// <summary>  角色动作列表</summary>
        private HashSet<uint> actionHashSet = new HashSet<uint>();
        /// <summary>  更新计时器 </summary>
        private Timer updateTimer;
        /// <summary>  界面是否显示 </summary>
        private bool isShowView = false;
        /// <summary> 检测动画 </summary>
        private bool checkAni = false;
        /// <summary> 动画下标 </summary>
        private int ani_index = 0;
        /// <summary> 是否全屏目标 </summary>
        private bool isFullScreenTarget;
        /// <summary> 目标坐标 </summary>
        private Vector3 vector3Target;
        
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
            guideTask = (Sys_Guide.GuideTask)arg;
        }

        protected override void OnShow()
        {
            SetSafeArea();
            SetData();
            //SetBgMark();
            
            StopMove();
            CanelTimer();
            SetTimer();

            if (guideTask != null)
            {
                if (guideTask.Id == uint.Parse(CSVParam.Instance.GetConfData(553).str_value))//特殊需求，引导中点击自动战斗按钮
                   // Sys_Guide.Instance.eventEmitter.Trigger(Sys_Guide.EEvents.OnClickAutoFightBtn);
                  if(GameCenter.fightControl != null)
                    {
                        GameCenter.fightControl.isGuide = true;
                    }
            }
        }

        protected override void OnHide()
        {
            _UnloadModel();
            CanelTimer();
            // OnExcuteGuideCompleted();
        }
        protected override void OnClose()
        {
            isShowView = false;
            ClearGuideTask();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        protected override void OnUpdate()
        {

        }
        /// <summary>
        /// OnUpdate 走系统分帧处理，引导会出现严重延迟，使用Timer替代
        /// </summary>
        private void OnUpdateData()
        {
            if (null == guideTask || null == guideTask.cSVGuideData || null == waitAnimator) return;

            if (!isShowView)
            {
                if (waitAnimator.IsCompleteAnimator())
                {
                    operationsList?.Exectue();
                    isShowView = true;
                }
            }
            if (isShowView && !isFullScreenTarget)
            {
                SynchronousTargetPosition();
            }

            if (isAutoSkipBtn)
            {
                timer += Time.deltaTime;//  deltaTime;
                if (timer >= guideTask.cSVGuideData.auto_time)
                {
                    isAutoSkipBtn = false;
                    SetSkipBtn(true);
                }
            }
            if (guideTask.guideState != Sys_Guide.EGuideState.Executing)
            {
                guideTask = null;
                CloseView();
            }
            else if (Sys_FunctionOpen.Instance.isRunning)
            {
                guideTask.SetGuideState(Sys_Guide.EGuideState.Waiting);
                guideTask = null;
                CloseView();
            }
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent()
        {
            image_BgMark = transform.Find("MaskRoot/Image_Black").GetComponent<Image>();
            hollowOutBg = image_BgMark.gameObject.GetComponent<HollowOutBg>();
            rt_TargetNode = transform.Find("Animator/TargetRange") as RectTransform;
            rt_DialogBoxNode = transform.Find("Animator/TargetRange/Guide01") as RectTransform;
            image_Bg = transform.Find("Animator/TargetRange/Guide01/Image_Bg01").GetComponent<Image>();
            rt_TitleNode = transform.Find("Animator/TargetRange/Guide01/Image_Bg01/Title") as RectTransform;
            text_Title = transform.Find("Animator/TargetRange/Guide01/Image_Bg01/Title/Text_Title02").GetComponent<Text>();
            rt_PortraitNode = transform.Find("Animator/TargetRange/Guide01/Image_Bg01/NpcRoot") as RectTransform;
            text_Content = transform.Find("Animator/TargetRange/Guide01/Image_Bg01/Text").GetComponent<Text>();

            rt_HandNode = transform.Find("Animator/TargetRange/Guide02") as RectTransform;
            image_Hand = transform.Find("Animator/TargetRange/Guide02/Image_Hand").GetComponent<Image>();
            button_Skip = transform.Find("Animator/SkipButton").GetComponent<Button>();

            safeAreaController = transform.GetComponent<SafeAreaController>();
            list_CanvasPropertyOverrider.Add(transform.Find("Animator").GetComponent<CanvasPropertyOverrider>());
            list_CanvasPropertyOverrider.Add(transform.Find("UI_Fx").GetComponent<CanvasPropertyOverrider>());

            assetDependencies = transform.GetComponent<AssetDependencies>();
            Model_rawImage = transform.Find("Animator/TargetRange/Guide01/Image_Bg01/Charapter").GetComponent<RawImage>();

            Transform node = transform.Find("UI_Fx");
            for (int i = 0, count = node.childCount; i < count; i++)
            {
                Transform item = node.GetChild(i);
                item.gameObject.SetActive(false);
                dict_Effect.Add(item.name.ToString(), item.gameObject);
            }

            rt_DialogBoxNode.gameObject.SetActive(false);
            rt_HandNode.gameObject.SetActive(false);

            hollowOutBg.action_ClickTarget = OnClick_CompletedGuide;
            hollowOutBg.action_ClickBg = OnClick_Tips;

            button_Skip.onClick.AddListener(OnClick_Skip);
        }
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="toRegister"></param>
        protected void OnProcessEventsForEnable(bool toRegister)
        {
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndExit, EndExit, toRegister);
        }

        private void EndExit(uint stack, int id)
        {
            EUIID eId = (EUIID)id;
            if (guideTask != null && guideTask.cSVGuideData != null)
            {
                if (guideTask.cSVGuideData.UI_id == (uint) eId)
                {
                    CloseView();
                }
            }
        }
        
        /// <summary>
        /// 设置安全区域
        /// </summary>
        public void SetSafeArea()
        {
            UIBase uIBase = UIManager.GetUI((int)guideTask.cSVGuideData.UI_id);
            bool isNeed = true;
            if (null != uIBase)
                isNeed = uIBase.transform.GetComponent<SafeAreaController>() != null;

            if (guideTask.cSVGuideData.prefab_type == 1)//场景目标
                isNeed = false;

            for (int i = 0, count = list_CanvasPropertyOverrider.Count; i < count; i++)
            {
                var item = list_CanvasPropertyOverrider[i];
                item.isSafeCanvas = isNeed;
            }

            safeAreaController.UpdateSafeArea();
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        public void SetData()
        {
            //Debug.LogErrorFormat("guideId = {0}", this.guideTask.Id);
            OnStopCheckOperationsList();
            _timerCount = 0f;
            _timer = Timer.Register(0.1f, CheckOperationsList, null, true);
            //CheckOperationsList();
        }

        private Timer _timer;
        private float _timerCount;
        private void CheckOperationsList()
        {
            _timerCount += 0.1f;
            if (_timerCount > 2f)
            {
                //_timer?.Cancel();
                //_timer = null;
                OnStopCheckOperationsList();
                OnClick_Skip(); //如果2s没找到，强制完成引导
                return;
            }

            Sys_Guide.OperationsList operationsList = new Sys_Guide.OperationsList(guideTask);
            if (!operationsList.IsCreateSuccess)
            {
                //guideTask.SkipGuide();
                //CloseView();
                return;
            }
            this.isAutoSkipBtn = guideTask.IsAutoSkipBtn;
            this.timer = 0;
            this.operationsList = operationsList;
            this.operationsList.SetAction(OperationFunction);
            this.waitAnimator = new Sys_Guide.WaitAnimator();
            this.waitAnimator.SetTarget(guideTask.cSVGuideData);
            this.isShowView = false;

            OnStopCheckOperationsList();

            SetDefaultView();
            SetBgMark();
        }

        private void OnStopCheckOperationsList()
        {
            _timer?.Cancel();
            _timer = null;
        }

        /// <summary>
        /// 清理引导任务
        /// </summary>
        public void ClearGuideTask()
        {
            _timer?.Cancel();
            _timer = null;

            if (null != guideTask)
            {
                guideTask = null;
            }
            if (null != operationsList)
            {
                operationsList.CancelAllOperations();
                operationsList.Clear();
                operationsList = null;
            }
            if (null != hollowOutBg)
            {
                hollowOutBg.ClearTargets();
            }
        }
        /// <summary>
        /// 设置定时器
        /// </summary>
        public void SetTimer()
        {
            updateTimer = Timer.Register(0f, OnUpdateData, null, true);
        }
        /// <summary>
        /// 取消定时器
        /// </summary>
        public void CanelTimer()
        {
            updateTimer?.Cancel();
            updateTimer = null;
        }
        #endregion
        #region 外部脚本逻辑
        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            sceneModel.transform.localPosition = new Vector3(2000, 0, 0);

            showSceneControl.Parse(sceneModel);
            Model_rawImage.gameObject.SetActive(true);
            //设置RenderTexture纹理到RawImage
            Model_rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
        }
        private void _LoadShowModel(CSVGuideTip.Data cSVGuideTipData)
        {
            CSVGuideModel.Data cSVGuideModelData = CSVGuideModel.Instance.GetConfData(cSVGuideTipData.Model);
            if (null == cSVGuideModelData) return;
            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
            }
            petDisplay.onLoaded = (int obj) =>
            {
                if (obj == 0)
                {
                    ani_index = 0;
                    actionlist.Clear();
                    actionHashSet.Clear();
                    
                    if (null == cSVGuideTipData.Motion || cSVGuideTipData.Motion.Count == 0)
                    {
                        actionlist.Add((uint)EStateType.Idle);
                        actionHashSet.Add((uint)EStateType.Idle);
                    }
                    else
                    {
                        actionlist.AddRange(cSVGuideTipData.Motion);
                        for (int i = 0; i < cSVGuideTipData.Motion.Count; ++i)
                        {
                            actionHashSet.Add(cSVGuideTipData.Motion[i]);
                        }
                    }

                    petDisplay.mAnimation.UpdateHoldingAnimations(cSVGuideModelData.action_id, cSVGuideModelData.weapon_id, actionHashSet, (EStateType)actionlist[0], null,
                        () =>
                        {
                            PlayAnimator();
                        });
                }
            };

            string _modelPath = cSVGuideModelData.model;
            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            Vector3 localRotation = cSVGuideTipData.Rotation.Count >= 3 ? new Vector3(cSVGuideTipData.Rotation[0], cSVGuideTipData.Rotation[1], cSVGuideTipData.Rotation[2]) : Vector3.zero;
            Vector3 localScale = cSVGuideTipData.Scale.Count >= 3 ? new Vector3(cSVGuideTipData.Scale[0], cSVGuideTipData.Scale[1], cSVGuideTipData.Scale[2]) : Vector3.one;
            Vector3 localPosition = cSVGuideTipData.Position.Count >= 3 ? new Vector3(cSVGuideTipData.Position[0], cSVGuideTipData.Position[1], cSVGuideTipData.Position[2]) : Vector3.zero;
            showSceneControl.mModelPos.transform.localEulerAngles = localRotation;
            showSceneControl.mModelPos.transform.localScale = localScale;
            showSceneControl.mModelPos.transform.localPosition = localPosition;
        }
        public void _UnloadModel()
        {
            _UnloadShowContent();
        }
        private void _UnloadShowContent()
        {
            Model_rawImage.gameObject.SetActive(false);
            Model_rawImage.texture = null;
            //petDisplay?.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl?.Dispose();
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 设置背景遮罩
        /// </summary>
        public void SetBgMark()
        {
            Sys_Guide.OperationsList.Operation operation = operationsList?.list_Need.Find(x => x.eGuidePhase == Sys_Guide.EGuidePhase.OpenBackgroundMask);
            if (null == operation)
                return;

            operationsList.action(operation);
            SetBgAlpha();
        }
        /// <summary>
        /// 设置背景透明值
        /// </summary>
        public void SetBgAlpha()
        {
            var color = image_BgMark.color;
            if (guideTask != null && guideTask.cSVGuideData != null && guideTask.cSVGuideData.effect_type == 1)
            {
                image_BgMark.color = new Color(color.r, color.g, color.b, 1f);
            }
            else
            {
                image_BgMark.color = new Color(color.r, color.g, color.b, 0f);
            }
        }
        /// <summary>
        /// 设置默认界面
        /// </summary>
        public void SetDefaultView()
        {
            rt_DialogBoxNode.gameObject.SetActive(false);
            rt_HandNode.gameObject.SetActive(false);

            List<string> list_EffectKeys = new List<string>(dict_Effect.Keys);
            for (int i = 0, count = list_EffectKeys.Count; i < count; i++)
            {
                string id = list_EffectKeys[i];
                var effect = dict_Effect[id];
                effect.gameObject.SetActive(false);
            }
            SetSkipBtn(!isAutoSkipBtn);
        }
        /// <summary>
        /// 设置目标范围
        /// </summary>
        public void SetTargetRange()
        {
            //是否是全屏目标
            isFullScreenTarget = null != guideTask ? !System.Convert.ToBoolean(guideTask.cSVGuideData.tippos_type) : true;
            if (isFullScreenTarget)
            {
                rt_TargetNode.anchorMin = Vector2.zero;
                rt_TargetNode.anchorMax = Vector2.one;
                rt_TargetNode.offsetMin = Vector2.zero;
                rt_TargetNode.offsetMax = Vector2.zero;
            }
            else
            {
                rt_TargetNode.anchorMin = new Vector2(0.5f, 0.5f);
                rt_TargetNode.anchorMax = new Vector2(0.5f, 0.5f);
                rt_TargetNode.sizeDelta = Vector3.zero;
                SynchronousTargetPosition();
            }
        }
        /// <summary>
        /// 同步目标坐标
        /// </summary>
        public void SynchronousTargetPosition()
        {
            Transform tr_Target = GetTarget();
            if (null == tr_Target || vector3Target == tr_Target.position) return;

            vector3Target = tr_Target.position;
            Vector3 v3 = Sys_Guide.Instance.GetTargetPos(rt_TargetNode.parent as RectTransform, tr_Target);
            rt_TargetNode.position = v3;
        }
        /// <summary>
        /// 设置对话
        /// </summary>
        /// <param name="Id"></param>
        public void SetDialogBox(uint Id)
        {
            CSVGuideTip.Data cSVGuideTipData = CSVGuideTip.Instance.GetConfData(Id);
            if (null == cSVGuideTipData)
            {
                rt_DialogBoxNode.gameObject.SetActive(false);
                return;
            }
            rt_DialogBoxNode.gameObject.SetActive(true);
            //反转
            bool isReverse = System.Convert.ToBoolean(cSVGuideTipData.tip_scale);
            Vector3 localScale = new Vector3(isReverse ? -1f : 1f, 1f, 1f);
            image_Bg.rectTransform.localScale = localScale;
            text_Content.rectTransform.localScale = localScale;
            rt_TitleNode.localScale = localScale;
            //适配位置
            var tip_anchors = cSVGuideTipData.tip_anchors;
            if (tip_anchors?.Count >= 4)
            {
                rt_DialogBoxNode.anchorMin = new Vector2(tip_anchors[0], tip_anchors[1]);
                rt_DialogBoxNode.anchorMax = new Vector2(tip_anchors[2], tip_anchors[3]);
            }
            //提示框坐标设置
            var tip_pos = cSVGuideTipData.tip_pos;
            if (tip_pos?.Count >= 3)
            {
                rt_DialogBoxNode.anchoredPosition3D = new Vector3(tip_pos[0], tip_pos[1], tip_pos[2]);
            }
            //提示框边框大小
            var tip_size = cSVGuideTipData.tip_size;
            if (tip_size?.Count >= 2)
            {
                image_Bg.rectTransform.sizeDelta = new Vector2(tip_size[0], tip_size[1]);
            }
            //内容
            text_Content.text = LanguageHelper.GetTextContent(cSVGuideTipData.tip_content);
            //标题
            text_Title.text = cSVGuideTipData.image_name == 0 ? string.Empty : LanguageHelper.GetTextContent(cSVGuideTipData.image_name);
            //半身像
            if (cSVGuideTipData.Model != 0)
            {
                _UnloadModel();
                _LoadShowScene();
                _LoadShowModel(cSVGuideTipData);
            }
        }
        /// <summary>
        /// 设置手势
        /// </summary>
        /// <param name="Id"></param>
        public void SetHand(uint Id)
        {
            CSVGuideArrow.Data cSVGuideArrowData = CSVGuideArrow.Instance.GetConfData(Id);
            if (null == cSVGuideArrowData)
            {
                rt_HandNode.gameObject.SetActive(false);
                return;
            }
            rt_HandNode.gameObject.SetActive(true);

            var arrow_anchors = cSVGuideArrowData.arrow_anchors;
            if (arrow_anchors?.Count >= 4)
            {
                rt_HandNode.anchorMin = new Vector2(arrow_anchors[0], arrow_anchors[1]);
                rt_HandNode.anchorMax = new Vector2(arrow_anchors[2], arrow_anchors[3]);
            }

            //提示框坐标设置
            var arrow_pos = cSVGuideArrowData.arrow_pos;
            if (arrow_pos?.Count >= 3)
            {
                rt_HandNode.anchoredPosition3D = new Vector3(arrow_pos[0], arrow_pos[1], arrow_pos[2]);
            }
            var arrow_rotation = cSVGuideArrowData.arrow_rotation;
            if (arrow_rotation?.Count >= 3)
            {
                rt_HandNode.localEulerAngles = new Vector3(arrow_rotation[0], arrow_rotation[1], arrow_rotation[2]);
            }
            var arrow_scale = cSVGuideArrowData.arrow_scale;
            if (arrow_scale?.Count >= 3)
            {
                rt_HandNode.localScale = new Vector3(arrow_scale[0], arrow_scale[1], arrow_scale[2]);
            }
        }
        /// <summary>
        /// 设置特效状态
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="state"></param>
        public void SetEffectState(uint Id, bool state)
        {
            CSVGuideEffect.Data cSVGuideEffectData = CSVGuideEffect.Instance.GetConfData(Id);
            if (null == cSVGuideEffectData) return;

            GameObject go_Effect = null;
            dict_Effect.TryGetValue(cSVGuideEffectData.effect, out go_Effect);
            if (null == go_Effect) return;

            RectTransform rt_Effect = go_Effect.transform as RectTransform;
            if (null == rt_Effect) return;

            go_Effect.SetActive(state);

            if (cSVGuideEffectData.type == 1)
            {
                var effect_size = cSVGuideEffectData.effect_size;
                if (effect_size?.Count >= 2)
                {
                    rt_Effect.sizeDelta = new Vector2(effect_size[0], effect_size[1]);
                }
            }
            var effect_anchors = cSVGuideEffectData.effect_anchors;
            if (effect_anchors?.Count >= 4)
            {
                rt_Effect.anchorMin = new Vector2(effect_anchors[0], effect_anchors[1]);
                rt_Effect.anchorMax = new Vector2(effect_anchors[2], effect_anchors[3]);
            }

            var effect_pos = cSVGuideEffectData.effect_pos;
            if (string.IsNullOrEmpty(cSVGuideEffectData.prefab_path))
            {
                if (effect_pos?.Count >= 3)
                {
                    rt_Effect.anchoredPosition3D = new Vector3(effect_pos[0], effect_pos[1], effect_pos[2]);
                }
            }
            else
            {
                Transform targetNode = UIManager.mRoot.Find(cSVGuideEffectData.prefab_path);
                if (targetNode != null)
                {
                    rt_Effect.pivot = new Vector2(cSVGuideEffectData.effect_Pivot[0], cSVGuideEffectData.effect_Pivot[1]);
                    rt_Effect.position = targetNode.position;
                    if (hollowOutBg != null)
                        hollowOutBg.SetMaskPivot(rt_Effect.pivot);
                }
            }
           
            var effect_rotation = cSVGuideEffectData.effect_rotation;
            if (effect_rotation?.Count >= 3)
            {
                rt_Effect.localEulerAngles = new Vector3(effect_rotation[0], effect_rotation[1], effect_rotation[2]);
            }
            var effect_scale = cSVGuideEffectData.effect_scale;
            if (effect_scale?.Count >= 3)
            {
                rt_Effect.localScale = new Vector3(effect_scale[0], effect_scale[1], effect_scale[2]);
            }
        }
        /// <summary>
        /// 设置跳过按钮
        /// </summary>
        /// <param name="isShow"></param>
        public void SetSkipBtn(bool isShow)
        {
            button_Skip?.gameObject.SetActive(isShow);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 点击完成引导事件
        /// </summary>
        public void OnClick_CompletedGuide()
        {
            OnExcuteGuideCompleted();
            CloseView();
        }

        private void OnExcuteGuideCompleted()
        {
            if (null != guideTask)
            {

                guideTask.CompletedGuide();
                guideTask = null;
            }
        }

        /// <summary>
        /// 点击提示
        /// </summary>
        public void OnClick_Tips(PointerEventData eventData)
        {
            if (null != guideTask && null != guideTask.cSVGuideData)
            {
                switch (guideTask.cSVGuideData.finish_type)
                {
                    case 0: //目标点击
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1004027));
                        }
                        break;
                    case 1: //全屏点击
                        {
                            if (null != hollowOutBg)
                            {
                                hollowOutBg.OnClick_Target(eventData); //目标事件完成
                            }
                            else
                            {
                                OnClick_CompletedGuide();
                            }
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// 点击跳过
        /// </summary>
        public void OnClick_Skip()
        {
            CloseView();
            if (null != guideTask)
            {
                guideTask.SkipGuide();
                guideTask = null;
            }
        }
        #endregion
        #region 提供功能
        public void CloseView()
        {
            _timer?.Cancel();
            _timer = null;
            UIManager.CloseUI(EUIID.UI_ForceGuide, true, false);
        }
        /// <summary>
        /// 操作功能
        /// </summary>
        /// <param name="parameters"></param>
        public void OperationFunction(Sys_Guide.OperationsList.Operation operation)
        {
            switch (operation.eGuidePhase)
            {
                case Sys_Guide.EGuidePhase.OpenBackgroundMask:
                    {
                        image_BgMark.gameObject.SetActive(true);
                        hollowOutBg.ClearTargets();
                    }
                    break;
                case Sys_Guide.EGuidePhase.CloseBackgroundMask:
                    {
                        image_BgMark.gameObject.SetActive(false);
                        hollowOutBg.ClearTargets();
                    }
                    break;
                case Sys_Guide.EGuidePhase.OpenDialog:
                    {
                        uint Id = System.Convert.ToUInt32(operation.Parameters[0]);
                        SetDialogBox(Id);
                    }
                    break;
                case Sys_Guide.EGuidePhase.CloseDialog:
                    {
                        rt_DialogBoxNode.gameObject.SetActive(false);
                    }
                    break;
                case Sys_Guide.EGuidePhase.OpenEffect:
                    {
                        uint Id = System.Convert.ToUInt32(operation.Parameters[0]);
                        SetEffectState(Id, true);
                    }
                    break;
                case Sys_Guide.EGuidePhase.CloseEffect:
                    {
                        uint Id = System.Convert.ToUInt32(operation.Parameters[0]);
                        SetEffectState(Id, false);
                    }
                    break;
                case Sys_Guide.EGuidePhase.OpenGuideIcon:
                    {
                        uint Id = System.Convert.ToUInt32(operation.Parameters[0]);
                        SetHand(Id);
                    }
                    break;
                case Sys_Guide.EGuidePhase.CloseGuideIcon:
                    {
                        rt_HandNode.gameObject.SetActive(false);
                    }
                    break;
                case Sys_Guide.EGuidePhase.SetCompletedOption:
                    {
                        Vector2 vector2 = new Vector2(System.Convert.ToSingle(operation.Parameters[0]), System.Convert.ToSingle(operation.Parameters[1]));
                        if (operationsList.list_Target.Count <= 0)
                        {
                            DebugUtil.Log(ELogType.eGuide, "引导缺少目标导致无法继续，现在自动强制完成，请查找原因。");
                            OnClick_Skip();
                        }
                        else
                        {
                            hollowOutBg.SetTargets(operationsList.list_Target, vector2);
                            SetTargetRange();
                        }
                    }
                    break;
                case Sys_Guide.EGuidePhase.CannelCompletedOption:
                    {
                        hollowOutBg.ClearTargets();
                    }
                    break;
                case Sys_Guide.EGuidePhase.WaitTime:
                    {
                        float time = System.Convert.ToSingle(operation.Parameters[0]);
                        Timer.Register(time, () =>
                        {
                            operation.isSkip = true;
                        }, (progress) => { });
                    }
                    break;
                case Sys_Guide.EGuidePhase.OpenView:
                    {
                        int Id = System.Convert.ToInt32(operation.Parameters[0]);
                        UIManager.OpenUI(Id);
                    }
                    break;
            }
        }
        /// <summary>
        /// 得到目标
        /// </summary>
        /// <returns></returns>
        public Transform GetTarget()
        {
            if (null == operationsList || null == operationsList.list_Target || operationsList.list_Target.Count <= 0)
                return null;

            return operationsList.list_Target[0];
        }
        /// <summary>
        /// 角色中断移动
        /// </summary>
        public void StopMove()
        {
            Sys_Task.Instance.StopAutoTask(true);
        }
        /// <summary>
        /// 播放动画
        /// </summary>
        private void PlayAnimator()
        {
            if (null == actionlist || null == petDisplay || null == petDisplay.mAnimation)
                return;

            if (actionlist.Count > ani_index)
            {
                petDisplay.mAnimation.CrossFade(actionlist[ani_index], Constants.CORSSFADETIME, () =>
                {
                    ani_index += 1;
                    PlayAnimator();
                });
            }
            else
            {
                int lastIndex = actionlist.Count - 1;
                if (lastIndex >= 0)
                {
                    petDisplay.mAnimation.CrossFade(actionlist[lastIndex], Constants.CORSSFADETIME);
                }
            }
        }
        #endregion
    }
}
