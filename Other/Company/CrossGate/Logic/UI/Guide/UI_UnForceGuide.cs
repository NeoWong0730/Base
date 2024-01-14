using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    /// <summary> 非强制引导界面 </summary>
    public class UI_UnForceGuide : UIBase
    {
        #region 界面组件
        /// <summary> 画布节点 </summary>
        private RectTransform rt_Canvas;
        /// <summary> 特效字段 </summary>
        public Dictionary<string, GameObject> dict_Effect = new Dictionary<string, GameObject>();
        #endregion
        #region 数据定义
        /// <summary> 非强制引导模版 </summary>
        public class UnForceGuideItem
        {
            #region 数据定义
            /// <summary> 引导编号 </summary>
            public uint Id
            {
                set;
                get;
            }
            /// <summary> 界面编号 </summary>
            public uint uiId;
            /// <summary> 引导任务 </summary>
            public Sys_Guide.GuideTask guideTask
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
            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="guideTask"></param>
            public void SetData(Sys_Guide.GuideTask guideTask)
            {
                this.Id = guideTask.Id;
                this.uiId = guideTask.cSVGuideData.UI_id;
                this.guideTask = guideTask;
                this.waitAnimator = new Sys_Guide.WaitAnimator();
            }
            #endregion
            #region 克隆组件
            /// <summary> 引导特效类 </summary>
            public class GuideEffect
            {
                public uint id;
                public RectTransform effect;
                public GuideEffect(uint id, RectTransform effect)
                {
                    this.id = id;
                    this.effect = effect;
                }
            }
            /// <summary> 特效画布 </summary>
            public Canvas canvas_Effect;
            /// <summary> 特效画布下目标节点点 </summary>
            public RectTransform rt_TargetNode;
            /// <summary> 特效列表 </summary>
            public List<GuideEffect> list_Effect = new List<GuideEffect>();
            /// <summary> 点击位置 </summary>
            public UIPassEvent sp_ClickEvent;
            /// <summary>
            /// 克隆模版
            /// </summary>
            /// <param name="rt_CanvasItem"></param>
            /// <param name="list_EffectItem"></param>
            public void CopyItem(RectTransform rt_CanvasItem, Dictionary<uint, GameObject> dictionary)
            {
                //克隆画布
                GameObject go_CanvasClone = GameObject.Instantiate(rt_CanvasItem.gameObject, rt_CanvasItem.parent);
                go_CanvasClone.name = guideTask.Id.ToString();
                go_CanvasClone.SetActive(true);
                canvas_Effect = go_CanvasClone.GetComponent<Canvas>();
                //克隆画布下主节点
                rt_TargetNode = go_CanvasClone.transform.Find("TargetRange").GetComponent<RectTransform>();
                rt_TargetNode.gameObject.SetActive(false);
                //克隆特效
                foreach (var valuePair in dictionary)
                {
                    GameObject go_EffectClone = GameObject.Instantiate(valuePair.Value.gameObject, rt_TargetNode);
                    go_EffectClone.SetActive(true);
                    RectTransform rt_EffectClone = go_EffectClone.GetComponent<RectTransform>();
                    list_Effect.Add(new GuideEffect(valuePair.Key, rt_EffectClone));
                }

                //Debug.LogErrorFormat("OnClick_CompletedGuide = {0}", Time.time);
                //设置点击事件
                Transform tr_ClickEvent = go_CanvasClone.transform.Find("TargetRange/ClickEvent");
                sp_ClickEvent = tr_ClickEvent.GetComponent<UIPassEvent>();
                sp_ClickEvent.onClick = OnClick_CompletedGuide;
            }
            #endregion
            #region 目标组件
            /// <summary> 目标 </summary>
            public Transform tr_Target;
            /// <summary> 目标画布 </summary>
            public CanvasGroup cg_Target;
            /// <summary>
            /// 设置目标
            /// </summary>
            public void SetTarget()
            {
                var list = Sys_Guide.Instance.FindGameObject(guideTask.cSVGuideData.prefab_type, guideTask.cSVGuideData.prefab_path);
                this.tr_Target = list.Count > 0 ? list[0] : null;
                this.cg_Target = tr_Target?.GetComponent<CanvasGroup>();
                //设置动画目标
                waitAnimator.SetTarget(guideTask.cSVGuideData);
                //设置点击事件目标
                sp_ClickEvent.SetTarget(tr_Target);
                //设置特效目标
                SetEffectState();
                SetEffect();
            }
            #endregion
            #region 功能设置
            /// <summary>
            /// 清理
            /// </summary>
            public void Clear()
            {
                for (int i = 0; i < list_Effect.Count; i++)
                {
                    var item = list_Effect[i];
                    if (null != item.effect)
                        GameObject.Destroy(item.effect.gameObject);
                }
                list_Effect.Clear();

                if (null != canvas_Effect)
                    GameObject.Destroy(canvas_Effect.gameObject);
            }
            /// <summary>
            /// 设置特效状态
            /// </summary>
            public void SetEffectState()
            {
                bool isFinish = guideTask == null ? false : guideTask.triggerGroup.isFinish;
                bool isTarget = tr_Target == null ? false : tr_Target.gameObject.activeInHierarchy;
                bool isAlpha = cg_Target == null ? true : cg_Target.alpha == 1f;
                bool isActive = isFinish && isTarget && isAlpha;

                if (isActive)
                {
                    if (!waitAnimator.IsCompleteAnimator())
                        return;
                    //设置安全区域
                    SetSafeArea();
                    //设置目标区域
                    SetTargetRange();
                }
                if (rt_TargetNode.gameObject.activeInHierarchy != isActive)
                    rt_TargetNode.gameObject.SetActive(isActive);
                //if (rt_TargetNode.gameObject.activeInHierarchy != isActive)
                //{
                   
                //}
            }
            /// <summary>
            /// 设置特效
            /// </summary>
            public void SetEffect()
            {
                //设置特效列表
                uint clickId = 0;
                for (int i = 0; i < list_Effect.Count; i++)
                {
                    GuideEffect guideEffect = list_Effect[i];
                    SetEffectRectTransform(guideEffect.effect.gameObject, guideEffect.id);
                    if (i == 0)
                        clickId = guideEffect.id;
                }
                //设置点击位置
                SetEffectRectTransform(sp_ClickEvent.gameObject, clickId);
            }
            /// <summary>
            /// 设置目标区域
            /// </summary>
            public void SetTargetRange()
            {
                //是否是全屏目标
                bool isFullScreenTarget = null != guideTask ? !System.Convert.ToBoolean(guideTask.cSVGuideData.tippos_type) : true;
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
                    Vector3 v3 = Sys_Guide.Instance.GetTargetPos(rt_TargetNode.parent as RectTransform, tr_Target);
                    rt_TargetNode.position = v3;
                    rt_TargetNode.sizeDelta = Vector3.zero;
                }
            }
            /// <summary>
            /// 设置特效
            /// </summary>
            /// <param name="go"></param>
            /// <param name="Id"></param>
            private void SetEffectRectTransform(GameObject go, uint Id)
            {
                CSVGuideEffect.Data cSVGuideEffectData = CSVGuideEffect.Instance.GetConfData(Id);
                if (null == cSVGuideEffectData || null == go) return;

                RectTransform rectTransform = go.transform as RectTransform;
                if (null == rectTransform) return;

                var effect_anchors = cSVGuideEffectData.effect_anchors;
                if (effect_anchors?.Count >= 4)
                {
                    rectTransform.anchorMin = new Vector2(effect_anchors[0], effect_anchors[1]);
                    rectTransform.anchorMax = new Vector2(effect_anchors[2], effect_anchors[3]);
                }
                if (cSVGuideEffectData.type == 1)
                {
                    var effect_size = cSVGuideEffectData.effect_size;
                    if (effect_size?.Count >= 2)
                    {
                        rectTransform.sizeDelta = new Vector2(effect_size[0], effect_size[1]);
                    }
                }
                var effect_pos = cSVGuideEffectData.effect_pos;
                if (effect_pos?.Count >= 3)
                {
                    rectTransform.anchoredPosition3D = new Vector3(effect_pos[0], effect_pos[1], effect_pos[2]);
                }
                var effect_rotation = cSVGuideEffectData.effect_rotation;
                if (effect_rotation?.Count >= 3)
                {
                    rectTransform.localEulerAngles = new Vector3(effect_rotation[0], effect_rotation[1], effect_rotation[2]);
                }
                var effect_scale = cSVGuideEffectData.effect_scale;
                if (effect_scale?.Count >= 3)
                {
                    rectTransform.localScale = new Vector3(effect_scale[0], effect_scale[1], effect_scale[2]);
                }
                var tipsId = cSVGuideEffectData.tip_content;
                if (tipsId != 0)
                {
                    Text text_Tips = rectTransform.GetComponentInChildren<Text>();
                    if (null != text_Tips)
                        text_Tips.text = LanguageHelper.GetTextContent(tipsId);

                    for (int i = 1; i < 7; ++i)
                    {
                        string str = string.Format("Arrow_{0}", i);
                        Transform trans = rectTransform.Find(str);
                        if (trans != null)
                        {
                            trans.gameObject.SetActive(i == cSVGuideEffectData.arrow_direction);
                        }
                    }
                }
            }
            /// <summary>
            /// 完成非强制引导
            /// </summary>
            /// <param name="go"></param>
            public void OnClick_CompletedGuide(GameObject go)
            {
                //Debug.LogError("OnClick_CompletedGuide");
                if (null != guideTask)
                    guideTask.CompletedGuide();
            }
            /// <summary>
            /// 设置安全区域
            /// </summary>
            public void SetSafeArea()
            {
                UIBase uIBase = UIManager.GetUI((int)guideTask.cSVGuideData.UI_id);
                bool isNeed = true;
                if (null != uIBase)
                {
                    canvas_Effect.sortingOrder = uIBase.nSortingOrder + 9;
                    isNeed = uIBase.transform.GetComponent<SafeAreaController>() != null;
                    UILocalSorting[] layoutControlls = rt_TargetNode.GetComponentsInChildren<UILocalSorting>();
                    for (int i = 0; i < layoutControlls.Length; i++)
                    {
                        layoutControlls[i].SetRootSorting(canvas_Effect.sortingOrder);
                    }
                }

                if (guideTask.cSVGuideData.prefab_type == 1)//场景目标
                    isNeed = false;

                var cpo = canvas_Effect.GetComponent<CanvasPropertyOverrider>();
                if (null != cpo) cpo.isSafeCanvas = isNeed;
                canvas_Effect.GetComponentInParent<SafeAreaController>()?.UpdateSafeArea();
            }
            #endregion
        }
        /// <summary> 引导数据 </summary>
        public List<UnForceGuideItem> list_GuideItem = new List<UnForceGuideItem>();

        private Lib.Core.Timer _timer;
        private float _timerCount;
        private Lib.Core.Timer _UpateTimer;
        #endregion
        #region 系统函数
        protected override void OnInit()
        {

        }
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        //protected override void OnUpdate()
        //{
        //    for (int i = 0, count = list_GuideItem.Count; i < count; i++)
        //    {
        //        var item = list_GuideItem[i];
        //        item.SetEffectState();
        //    }
        //}
        protected override void OnOpen(object arg)
        {

        }
        protected override void OnShow()
        {
            _UpateTimer?.Cancel();
            _UpateTimer = null;
            //_UpateTimer = Lib.Core.Timer.Register(0f, TimeUpdate, null, true);
        }
        protected override void OnHide()
        {
            _UpateTimer?.Cancel();
            _UpateTimer = null;

            _timer?.Cancel();
            _timer = null;
        }
        protected override void OnClose()
        {
            Clear();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }

        private void TimeUpdate()
        {
            for (int i = 0, count = list_GuideItem.Count; i < count; i++)
            {
                var item = list_GuideItem[i];
                item.SetEffectState();
            }
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent()
        {
            rt_Canvas = transform.Find("Canvas") as RectTransform;
            rt_Canvas.gameObject.SetActive(false);

            RectTransform rt_effect = transform.Find("Effect") as RectTransform;
            rt_effect.gameObject.SetActive(false);

            for (int i = 0, count = rt_effect.childCount; i < count; i++)
            {
                Transform item = rt_effect.GetChild(i);
                dict_Effect.Add(item.name.ToString(), item.gameObject);
            }
        }
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="toRegister"></param>
        protected void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Guide.Instance.eventEmitter.Handle<Sys_Guide.GuideTask>(Sys_Guide.EEvents.AddUnForceGuide, AddGuideItem, toRegister);
            Sys_Guide.Instance.eventEmitter.Handle<Sys_Guide.GuideTask>(Sys_Guide.EEvents.RemoveUnForceGuide, RemoveGuideItem, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndEnter, OnUIChange, toRegister);
            //UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginExit, OnUIChange, toRegister);
        }
        #endregion
        #region 数据处理
        /// <summary>
        /// 添加非强制引导
        /// </summary>
        /// <param name="guideTask"></param>
        public void AddGuideItem(Sys_Guide.GuideTask guideTask)
        {
            //Debug.LogErrorFormat("AddGuideItem = {0}", Time.time);
            this.guideTask = guideTask;
            _timerCount = 0f;
            if (_timer == null)
            {
                _timer = Lib.Core.Timer.Register(0.1f, CheckOperationList, null, true);
            }
            else
            {
                _timer.Cancel();
                _timer = Lib.Core.Timer.Register(0.1f, CheckOperationList, null, true);
            }
            CheckOperationList();
        }

        Sys_Guide.GuideTask guideTask;
        private void CheckOperationList()
        {
            _timerCount += 0.1f;
            if (_timerCount > 3f)
            {
                OnStopCheckOperationList();
                return;
            }

            UnForceGuideItem unForceGuideItem = list_GuideItem.Find(x => x.Id == guideTask.Id);
            if (null != unForceGuideItem)
            {
                OnStopCheckOperationList();
                return;
            }

            Sys_Guide.OperationsList operationsList = new Sys_Guide.OperationsList(guideTask);

            Sys_Guide.OperationsList.Operation operation = operationsList.list_Need.Find(x => x.eGuidePhase == Sys_Guide.EGuidePhase.OpenEffect);
            if (!operationsList.IsCreateSuccess || null == operation) return;

            Dictionary<uint, GameObject> dictionary = new Dictionary<uint, GameObject>();
            for (int i = 0; i < operation.Parameters.Count; i++)
            {
                uint EffectId = System.Convert.ToUInt32(operation.Parameters[i]);
                CSVGuideEffect.Data cSVGuideEffectData = CSVGuideEffect.Instance.GetConfData(EffectId);
                if (null == cSVGuideEffectData) continue;
                GameObject go_Effect;
                if (!dict_Effect.TryGetValue(cSVGuideEffectData.effect, out go_Effect)) continue;
                if (!dictionary.ContainsKey(EffectId))
                    dictionary.Add(EffectId, go_Effect);
            }

            unForceGuideItem = new UnForceGuideItem();
            unForceGuideItem.SetData(guideTask);
            unForceGuideItem.CopyItem(rt_Canvas, dictionary);
            unForceGuideItem.SetTarget();
            list_GuideItem.Add(unForceGuideItem);
            if(_UpateTimer == null)
            {
                _UpateTimer = Lib.Core.Timer.Register(0f, TimeUpdate, null, true);
            }
            //Debug.LogErrorFormat("unForceGuideItem = {0}", Time.time);

            OnStopCheckOperationList();
        }

        private void OnStopCheckOperationList()
        {
            _timer?.Cancel();
            _timer = null;
        }

        /// <summary>
        /// 删除非强制引导
        /// </summary>
        /// <param name="guideTask"></param>
        public void RemoveGuideItem(Sys_Guide.GuideTask guideTask)
        {
            var unForceGuideItem = list_GuideItem.Find(x => x.Id == guideTask.Id);
            if (null == unForceGuideItem) return;
            unForceGuideItem.Clear();
            list_GuideItem.Remove(unForceGuideItem);
            if (list_GuideItem.Count == 0)
            {
                _UpateTimer?.Cancel();
                _UpateTimer = null;
            }
        }
        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            _UpateTimer?.Cancel();
            _UpateTimer = null;

            _timer?.Cancel();
            _timer = null;

            for (int i = 0; i < list_GuideItem.Count; i++)
            {
                var item = list_GuideItem[i];
                item.Clear();
            }
            list_GuideItem.Clear();
        }
        #endregion
        #region 界面显示
        #endregion
        #region 响应事件
        /// <summary>
        /// 进入界面后更新目标
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="id"></param>
        private void OnUIChange(uint stack, int id)
        {
            //界面切换时重新寻目标，界面关闭可能会被删除导致目标丢失
            for (int i = 0; i < list_GuideItem.Count; i++)
            {
                var item = list_GuideItem[i];
                if (item.uiId == id)
                {
                    item.SetTarget();
                }
            }
        }
        #endregion
        #region 提供功能
        #endregion
    }
}

