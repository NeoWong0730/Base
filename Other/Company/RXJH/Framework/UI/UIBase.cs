using Lib.Core;
using UI.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.UI
{
    public class UIBase
    {
        //private static List<UILocalSorting> tempSortList = new List<UILocalSorting>(32);
        private static List<ILocalSorting> tempSortCanvasList = new List<ILocalSorting>(64);

        protected UIStack mOwnerStack = null;
        public int nID { get; private set; }
        public int nParentID;

        //真正加载完成时机，标记后在StateUpdate处理Onloaded
        private bool isLoadFinish = false;
        public bool bLoaded { get; private set; }
        private bool bOpened = false;
        public GameObject gameObject { get; private set; }
        public RectTransform transform { get; private set; }
        public Canvas canvas { get; private set; }
        protected UIAnimatorControl animatorControl;
        protected UIController[] audioPlayer;
        internal bool bInStack = false;
        //是否根据堆栈隐藏自己
        private bool bStackHide = false;
        //是否设置过隐藏堆栈
        internal bool bRefHideStack = false;

        public string sPrefabName { get; private set; }
        public int nSortingOrder { get; private set; }
        public EUIOption eOptions { get; private set; }

        private AsyncOperationHandle<GameObject> mHandle;
        private EUIState _eState = EUIState.Invalid;
        private EUIState _eAimState = EUIState.Invalid;
        private EUIState _eBefState = EUIState.Invalid;

        protected uint nShowTimePoint;

        private float fStateDuration;
        public float EnterTime
        {
            get
            {
                if (animatorControl)
                {
                    return animatorControl.EnterTime;
                }
                return 0;
            }
        }
        public float ExitTime
        {
            get
            {
                if (animatorControl)
                {
                    return animatorControl.ExitTime;
                }
                return 0;
            }
        }

        private int _offsetFrame = 0;
        private int _intervalFrame = 24;  //默认按照5帧运行 //最大帧率120帧 最小帧率5帧
        private float _unscaledLastUpdateTime = 0f;
        private float _lastUpdateTime = 0f;
        public float deltaTime { get; internal set; }
        public float unscaledDeltaTime { get; internal set; }        

        public EUIState eState
        {
            get
            {
                return _eState;
            }
            private set
            {
                if (_eState != value)
                {
                    _eBefState = _eState;
                    _eState = value;
                    if (isVisibleAndOpen)
                    {
                        _offsetFrame = TimeManager.frameCount;
                    }
                    if (_eState != _eBefState)
                    {
                        DebugUtil.LogFormat(ELogType.eUIState, "{0} {1} -> {2}({3})", sPrefabName, _eBefState.ToString(), _eState.ToString(), _eAimState.ToString());
                    }                    
                }
            }
        }
        public EUIState eExpectState
        {
            get
            {
                if (_eState == EUIState.Show || _eState == EUIState.Showing || _eState == EUIState.WaitShow)
                {
                    return EUIState.Show;
                }
                else if (_eState == EUIState.Hiding || _eState == EUIState.WaitHide)
                {
                    if (_eAimState == EUIState.Hide)
                    {
                        return EUIState.Hide;
                    }
                    else if (_eAimState == EUIState.Close)
                    {
                        return EUIState.Close;
                    }
                    else if (_eAimState == EUIState.Destroy)
                    {
                        return EUIState.Destroy;
                    }
                }
                else
                {
                    return _eState;
                }
                return EUIState.Invalid;
            }

        }
        public bool isVisible
        {
            get
            {
                return bLoaded && gameObject != null && gameObject.activeSelf;
            }
        }
        public bool isVisibleAndOpen
        {
            get
            {
                return _eState == EUIState.Show || _eState == EUIState.Showing;
            }
        }
        public bool isOpen
        {
            get
            {
                return _eState == EUIState.WaitShow || _eState == EUIState.Showing
                    || _eState == EUIState.Show || _eState == EUIState.Hide
                    || ((_eState == EUIState.WaitHide || _eState == EUIState.Hiding) && _eAimState == EUIState.Hide);
            }
        }

        public bool ContainsOptions(EUIOption options)
        {
            return (eOptions & options) == options;
        }

        public void SetIntervalFrame(int intervalFrame)
        {
            _intervalFrame = intervalFrame;
        }

        public void Init(int id, UIConfigData configData, UIStack stack, bool isPreload)
        {
            mOwnerStack = stack;
            nID = id;

            eOptions = configData.options;
            sPrefabName = configData.prefabPath;

            OnInit();

            if (isPreload)
            {
                LoadPrefab();
            }
        }
        public void SetSortingOrder(int order, bool force = false)
        {
            if (nSortingOrder == order && !force)
            {
                return;
            }

            DebugUtil.LogFormat(ELogType.eUIState, "nSortingOrder  {0} -> {1}", nSortingOrder.ToString(), order.ToString());
            nSortingOrder = order;
            if (bLoaded)
            {
                canvas.sortingOrder = nSortingOrder;
                mOwnerStack.eventEmitter.Trigger<uint, int>(UIStack.EUIStackEvent.OnSortOrderChanged, mOwnerStack.stackID, nID);

                //gameObject.GetComponentsInChildren<UILocalSorting>(true, tempSortList);
                //for (int i = 0; i < tempSortList.Count; ++i)
                //{
                //    tempSortList[i].SetRootSorting(nSortingOrder);
                //}
                //tempSortList.Clear();

                gameObject.GetComponentsInChildren<ILocalSorting>(true, tempSortCanvasList);
                for (int i = 0; i < tempSortCanvasList.Count; ++i)
                {
                    tempSortCanvasList[i].SetRootSorting(nSortingOrder);
                    DebugUtil.LogFormat(ELogType.eUIState, "{0} ({1}) nSortingOrder = {2}", tempSortCanvasList[i].GetType().Name, (tempSortCanvasList[i] as MonoBehaviour)?.gameObject?.name, nSortingOrder.ToString());
                }
                tempSortCanvasList.Clear();
            }
        }
        private void MHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {
            gameObject = handle.Result;
            if (gameObject == null)
            {
                DebugUtil.LogErrorFormat("加载UI预制体失败 {0}", nID);
                return;
            }
#if UNITY_EDITOR
            gameObject.name = $"{nID}|{gameObject.name.Remove(gameObject.name.Length - 7, 7)}";
#endif
            transform = gameObject.transform as RectTransform;

            if (gameObject.TryGetComponent<Canvas>(out Canvas tmpCanvas))
            {
                canvas = tmpCanvas;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.pixelPerfect = false;
                canvas.worldCamera = mOwnerStack.mUICamera;

                gameObject.GetOrAddComponent<CanvasScreenMatch>();
                //移植状态更新中处理
                //SetSortingOrder(nSortingOrder, true);
            }

            GraphicRaycasterAdapter graphicRaycaster = gameObject.GetOrAddComponent<GraphicRaycasterAdapter>();
            graphicRaycaster.ignoreReversedGraphics = false;

            transform.anchoredPosition3D = Vector3.zero;
#if UNITY_EDITOR
            ButtonDrawer buttonDrawer = gameObject.GetComponent<ButtonDrawer>();
            if (buttonDrawer == null)
            {
                buttonDrawer = gameObject.AddComponent<ButtonDrawer>();
            }
#endif
            //不再运行时解析
            //ParseAnimation();
            animatorControl = gameObject.GetComponent<UIAnimatorControl>();
            audioPlayer = gameObject.GetComponentsInChildren<Framework.UI.UIController>(true);
            //TODO:预制默认设置为false
            gameObject.SetActive(false);

            isLoadFinish = true;

            //移植状态更新中处理
            //bLoaded = true;
            //OnLoaded();
            //DebugUtil.LogFormat(ELogType.eUIState, "{0} ProcessEvents(true)", sPrefabName);
            //ProcessEvents(true);
        }

        private void PlayEnter()
        {
            if (animatorControl)
            {
                animatorControl.PlayEnter();
            }
        }
        private void PlayExit()
        {
            if (animatorControl)
            {
                animatorControl.PlayExit();
            }
        }
        private void LoadPrefab()
        {
            if (!isLoadFinish && !mHandle.IsValid())
            {
                AddressablesUtil.InstantiateAsync(ref mHandle, sPrefabName, MHandle_Completed, true, mOwnerStack.mRoot);
            }
        }

        internal void Open(float delay = 0, bool stackHide = false, object arg = null)
        {
            if (_eState == EUIState.WaitShow || _eState == EUIState.Showing || _eState == EUIState.Show)
            {
                return;
            }

            if (_eState == EUIState.WaitHide)
            {
                eState = _eBefState;
                return;
            }

            eState = EUIState.WaitShow;
            bOpened = false;
            OnOpen(arg);

            if (eState == EUIState.WaitHide)
                return;

            Show(delay, stackHide);
        }
        public void Show(float delay = 0, bool stackHide = false)
        {
            if (_eState == EUIState.Showing || _eState == EUIState.Show)
            {
                return;
            }

            if (_eState == EUIState.WaitHide)
            {
                eState = _eBefState;
                return;
            }

            bStackHide = stackHide;
            fStateDuration = delay;
            eState = EUIState.WaitShow;

            LoadPrefab();
        }
        internal float Hide(EUIState state, bool immediate)
        {
            //无需判断 不会存在
            //if (state != EUIState.Hide && state != EUIState.Close && state != EUIState.Destroy)
            //{
            //    return 0;
            //}

            if (state <= _eState)
            {
                return 0;
            }

            if (_eState == EUIState.WaitHide || _eState == EUIState.Hiding)
            {
                if (state > _eAimState)
                {
                    _eAimState = state;
                }

                if (_eState == EUIState.WaitHide)
                {
                    fStateDuration = immediate ? 0 : ExitTime;
                }
            }
            else if (_eState == EUIState.Show || _eState == EUIState.Showing)
            {
                _eAimState = state;
                eState = EUIState.WaitHide;
                fStateDuration = immediate ? 0 : ExitTime;
            }
            else if (_eState == EUIState.WaitShow)
            {
                _eAimState = state;
                eState = EUIState.WaitHide;
                fStateDuration = 0;
            }
            else if (_eState == EUIState.Hide)
            {
                _eAimState = state;
                fStateDuration = 0;
            }
            else if (_eState == EUIState.Close)
            {
                _eAimState = state;
                fStateDuration = 0;
            }
            else
            {
                return 0;
            }

            return fStateDuration;
        }
        internal void UpdateState(float dt)
        {
            switch (_eState)
            {
                case EUIState.WaitShow:
                    {
                        if (!bLoaded)
                        {
                            if (isLoadFinish)
                            {
                                bLoaded = true;

                                SetSortingOrder(nSortingOrder, true);
                                _DoLoaded();
                                DebugUtil.LogFormat(ELogType.eUIState, "{0} ProcessEvents(true)", sPrefabName);
                                ProcessEvents(true);
                            }
                        }
                        else
                        {
                            //如果在关闭中的UI重新打开 则先调度玩关闭结束回调
                            if (_eBefState == EUIState.Hiding)
                            {
                                _eBefState = EUIState.Hide;
                                _DoEndExit();
                            }

                            fStateDuration -= dt;
                            if (fStateDuration <= 0 && bLoaded && bStackHide == false)
                            {
                                _DoBeginEnter();
                            }
                            if (_eState == EUIState.Showing && fStateDuration <= 0)
                            {
                                _DoEndEnter();
                            }
                        }
                    }
                    break;
                case EUIState.Showing:
                    {
                        fStateDuration -= dt;

                        if (fStateDuration <= 0)
                        {
                            _DoEndEnter();
                        }
                    }
                    break;
                case EUIState.WaitHide:
                    {
                        //如果在打开中的UI关闭 则先调度玩打开结束回调
                        if (_eBefState == EUIState.Showing)
                        {
                            _eBefState = EUIState.Show;
                            _DoEndEnter();
                        }

                        fStateDuration -= dt;

                        _DoBeginExit();
                        if (_eState == EUIState.Hiding && fStateDuration <= 0)
                        {
                            _DoEndExit();
                        }

                        if (_eState == EUIState.Close || _eState == EUIState.Destroy)
                        {
                            _CloseOrDestroy();
                        }
                    }
                    break;
                case EUIState.Hiding:
                    {
                        fStateDuration -= dt;

                        if (fStateDuration <= 0)
                        {
                            _DoEndExit();
                        }

                        if (_eState == EUIState.Close || _eState == EUIState.Destroy)
                        {
                            _CloseOrDestroy();
                        }
                    }
                    break;
                case EUIState.Hide:
                    {
                        if (_eAimState == EUIState.Close || _eAimState == EUIState.Destroy)
                        {
                            eState = _eAimState;
                            _eAimState = EUIState.Invalid;

                            _CloseOrDestroy();
                        }
                    }
                    break;
                case EUIState.Close:
                    {
                        if (_eAimState == EUIState.Destroy)
                        {
                            eState = _eAimState;
                            _eAimState = EUIState.Invalid;

                            _CloseOrDestroy();
                        }
                    }
                    break;
            }
        }
        internal void Update(float time, float unscaledTime)
        {
            if (isVisibleAndOpen && TimeManager.CanExecute(_offsetFrame, _intervalFrame))
            {
                deltaTime = time - _lastUpdateTime;
                unscaledDeltaTime = unscaledTime - _unscaledLastUpdateTime;

                _lastUpdateTime = time;
                _unscaledLastUpdateTime = unscaledTime;

                OnUpdate();
            }
        }
        internal void LateUpdate(float dt, float usdt)
        {
            if (isVisibleAndOpen)
            {
                OnLateUpdate(dt, usdt);
            }
        }

        protected virtual void _DoLoaded()
        {
            OnLoaded();
        }
        protected virtual void _DoBeginEnter()
        {
            if (!bLoaded)
                return;

            eState = EUIState.Showing;
            fStateDuration = EnterTime;
            _SetActive(true);
            PlayEnter();

            DebugUtil.LogFormat(ELogType.eUIState, "{0} ProcessEventsForEnable(true)", sPrefabName);
            ProcessEventsForEnable(true);

            if (ContainsOptions(EUIOption.eReduceMainCameraQuality))
            {
                mOwnerStack.ReduceMainCameraQuality();
                //CameraManager.ReduceMainCameraQuality();
            }

            if (ContainsOptions(EUIOption.eReduceFrameRate))
            {
                mOwnerStack.ReduceFrameRate();
            }

            if (ContainsOptions(EUIOption.eHideMainCamera))
            {
                mOwnerStack.ReadyHideMainCamera();
            }

            if (!bOpened)
            {
                if (audioPlayer != null)
                {
                    foreach (var item in audioPlayer)
                    {
                        item.PlayOpen();
                    }
                }
                OnOpened();
                bOpened = true;
            }

            _lastUpdateTime = Time.time;
            _unscaledLastUpdateTime = Time.unscaledTime;
            OnShow();

            mOwnerStack.eventEmitter.Trigger<uint, int>(UIStack.EUIStackEvent.BeginEnter, mOwnerStack.stackID, nID);
            mOwnerStack.nEventFlage |= (int)UIStack.EUIStackEvent.BeginEnter;

            nShowTimePoint = TimeManager.GetServerTime();
        }
        protected virtual void _DoEndEnter()
        {
            eState = EUIState.Show;

            if (animatorControl)
            {
                animatorControl.DisableAnimator();
            }

            if (ContainsOptions(EUIOption.eHideMainCamera))
            {
                mOwnerStack.RealHideMainCamera();
                //CameraManager.Hide();
            }

            OnShowEnd();

            mOwnerStack.eventEmitter.Trigger<uint, int>(UIStack.EUIStackEvent.EndEnter, mOwnerStack.stackID, nID);
            mOwnerStack.nEventFlage |= (int)UIStack.EUIStackEvent.EndEnter;
        }
        protected virtual void _DoBeginExit()
        {
            if (isVisible)
            {
                PlayExit();

                if (ContainsOptions(EUIOption.eHideMainCamera))
                {
                    mOwnerStack.CancelRealHideMainCamera();
                    //CameraManager.CancelHide();
                }

                if (ContainsOptions(EUIOption.eReduceMainCameraQuality))
                {
                    mOwnerStack.CancelReduceMainCameraQuality();
                    //CameraManager.CancelReduceMainCameraQuality();
                }

                if (ContainsOptions(EUIOption.eReduceFrameRate))
                {
                    mOwnerStack.CancelReduceFrameRate();
                }

                OnHideStart();
            }

            eState = EUIState.Hiding;

            mOwnerStack.eventEmitter.Trigger<uint, int>(UIStack.EUIStackEvent.BeginExit, mOwnerStack.stackID, nID);
            mOwnerStack.nEventFlage |= (int)UIStack.EUIStackEvent.BeginExit;
        }
        protected virtual void _DoEndExit()
        {
            eState = _eAimState;
            _eAimState = EUIState.Invalid;

            if (bLoaded && gameObject.activeSelf)
            {
                OnHide();
                DebugUtil.LogFormat(ELogType.eUIState, "{0} ProcessEventsForEnable(false)", sPrefabName);
                ProcessEventsForEnable(false);
                _SetActive(false);

                if (ContainsOptions(EUIOption.eHideMainCamera))
                {
                    mOwnerStack.CancelReadyHideMainCamera();
                }
            }

            mOwnerStack.eventEmitter.Trigger<uint, int>(UIStack.EUIStackEvent.EndExit, mOwnerStack.stackID, nID);
            mOwnerStack.nEventFlage |= (int)UIStack.EUIStackEvent.EndExit;
        }

        private void _DoClose()
        {
            if (audioPlayer != null)
            {
                foreach (var item in audioPlayer)
                {
                    item.PlayClose();
                }
            }

            OnClose();
        }
        protected virtual void _CloseOrDestroy()
        {
            if (_eState == EUIState.Close)
            {
                _DoClose();
            }
            else if (_eState == EUIState.Destroy)
            {
                _DoClose();
                DebugUtil.LogFormat(ELogType.eUIState, "{0} ProcessEvents(false)", sPrefabName);
                ProcessEvents(false);
                OnDestroy();

                AddressablesUtil.ReleaseInstance(ref mHandle, MHandle_Completed);

                gameObject = null;
                transform = null;
                canvas = null;
                animatorControl = null;
                mOwnerStack = null;

                bLoaded = false;
                isLoadFinish = false;
            }
        }
        private void _SetActive(bool active)
        {
            if (gameObject == null || active == gameObject.activeSelf)
                return;

            gameObject.SetActive(active);
        }

        public virtual void OnForeQuit() { }

        #region 生命周期        
        public virtual void OnSetData(object arg) { } // 给UI传递数据
        protected virtual void OnInit() { }
        protected virtual void OnOpen(object arg) { }
        protected virtual void OnLoaded() { }
        protected virtual void OnUpdate() { }
        /// <summary>
        /// 处理连续表现上的更新
        /// </summary>
        protected virtual void OnLateUpdate(float dt, float usdt) { }
        /// <summary>
        /// 调用从Open到Show 并非从 Hinding到Show时调用
        /// </summary>
        protected virtual void OnOpened() { }
        protected virtual void OnShow() { }
        protected virtual void OnShowEnd() { }
        protected virtual void OnHideStart() { }
        protected virtual void OnHide() { }
        protected virtual void OnClose() { }
        protected virtual void OnDestroy() { }
        protected virtual void ProcessEvents(bool toRegister) { }
        protected virtual void ProcessEventsForEnable(bool toRegister) { }

        #endregion 生命周期
        //public TComponent AddComponent<TComponent>(Transform go) where TComponent : UIComponent, new()
        //{
        //    TComponent component = new TComponent();
        //    component.Init(go);
        //    return component;
        //}
    }
}
