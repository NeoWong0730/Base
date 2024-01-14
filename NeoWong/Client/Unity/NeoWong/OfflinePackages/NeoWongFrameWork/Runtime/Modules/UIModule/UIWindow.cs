using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace NWFramework
{
    public abstract class UIWindow : UIBase
    {
        private System.Action<UIWindow> _prepareCallback;

        private bool _isCreate = false;

        private GameObject _panel;

        private Canvas _canvas;

        private Canvas[] _childCanvas;

        private GraphicRaycaster _raycaster;

        private GraphicRaycaster[] _childRaycaster;

        public override UIBaseType BaseType => UIBaseType.Window;

        /// <summary>
        /// 窗口位置组件
        /// </summary>
        public override Transform transform => _panel.transform;

        /// <summary>
        /// 窗口矩阵位置组件
        /// </summary>
        public override RectTransform rectTransform => _panel.transform as RectTransform;

        /// <summary>
        /// 窗口的实例资源对象
        /// </summary>
        public override GameObject gameObject => _panel;

        /// <summary>
        /// 窗口名称
        /// </summary>
        public string WindowName { get; private set; }

        /// <summary>
        /// 窗口层级
        /// </summary>
        public int WindowLayer { get; private set; }

        /// <summary>
        /// 资源定位地址
        /// </summary>
        public string AssetName { get; private set; }

        /// <summary>
        /// 是否为全屏窗口
        /// </summary>
        public bool FullScreen { get; private set; }

        /// <summary>
        /// 是内部资源无需AB加载
        /// </summary>
        public bool FromResources { get; private set; }

        /// <summary>
        /// 是否需要缓存
        /// </summary>
        public bool NeedCache { get; private set; }

        /// <summary>
        /// 自定义数据
        /// </summary>
        public System.Object UserData
        {
            get
            {
                if (userDatas != null && userDatas.Length >= 1)
                {
                    return userDatas[0];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 自定义数据集
        /// </summary>
        public System.Object[] UserDatas => userDatas;

        /// <summary>
        /// 窗口深度值
        /// </summary>
        public int Depth
        {
            get
            {
                if (_canvas != null)
                {
                    return _canvas.sortingOrder;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (_canvas != null)
                {
                    if (_canvas.sortingOrder == value)
                    {
                        return;
                    }

                    //设置父类
                    _canvas.sortingOrder = value;

                    //设置子类
                    int depth = value;
                    for (int i = 0; i < _childCanvas.Length; i++)
                    {
                        var canvas = _childCanvas[i];
                        if (canvas != _canvas)
                        {
                            //注意递增值
                            depth += 5;
                            canvas.sortingOrder = depth;
                        }
                    }

                    //虚函数
                    if (_isCreate)
                    {
                        OnSortDepth(value);
                    }
                }
            }
        }

        /// <summary>
        /// 窗口可见性
        /// </summary>
        public bool Visible
        {
            get
            {
                if (_canvas != null)
                {
                    return _canvas.gameObject.layer == UIModule.WINDOW_SHOW_LAYER;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (_canvas != null)
                {
                    int setLayer = value ? UIModule.WINDOW_SHOW_LAYER : UIModule.WINDOW_HIDE_LAYER;
                    if (_canvas.gameObject.layer == setLayer)
                        return;

                    //显示设置
                    _canvas.gameObject.layer = setLayer;
                    for (int i = 0; i < _childCanvas.Length; i++)
                    {
                        _childCanvas[i].gameObject.layer = setLayer;
                    }

                    //交互设置
                    Interactable = value;

                    //虚函数
                    if (_isCreate)
                    {
                        OnSetVisible(value);
                    }
                }
            }
        }

        /// <summary>
        /// 窗口交互性
        /// </summary>
        private bool Interactable
        {
            get
            {
                if (_raycaster != null)
                {
                    return _raycaster.enabled;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (_raycaster != null)
                {
                    _raycaster.enabled = value;
                    for (int i = 0; i < _childRaycaster.Length; i++)
                    {
                        _childRaycaster[i].enabled = value;
                    }
                }
            }
        }

        /// <summary>
        /// 是否加载完毕
        /// </summary>
        internal bool IsLoadDone => Handle.IsDone;

        public void Init(string name, int layer, bool fullScreen, string assetName, bool fromResources, bool needCache = false)
        {
            WindowName = name;
            WindowLayer = layer;
            FullScreen = fullScreen;
            AssetName = assetName;
            FromResources = fromResources;
            NeedCache = needCache;
        }

        internal void TryInvoke(System.Action<UIWindow> prepareCallback, System.Object[] userDatas)
        {
            base.userDatas = userDatas;
            if (IsPrepare)
            {
                prepareCallback?.Invoke(this);
            }
            else
            {
                _prepareCallback = prepareCallback;
            }
        }

        internal void InternalLoad(string location, System.Action<UIWindow> prepareCallback, System.Object[] userDatas)
        {
            _prepareCallback = prepareCallback;
            this.userDatas = userDatas;
            if (!FromResources)
            {
                Handle = GameModule.Resource.LoadAssetAsyncHandle<GameObject>(location, needCache: NeedCache);
                Handle.Completed += Handle_Completed;
            }
            else
            {
                GameObject panel = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(location), UIModule.UIRootStatic);
                Handle_Completed(panel);
            }
        }

        internal void InternalCreate()
        {
            if (_isCreate == false)
            {
                _isCreate = true;
                ScriptGenerator();
                BindMemberProperty();
                RegisterEvent();
                OnCreate();
            }
        }

        internal void InternalRefresh()
        {
            OnRefresh();
        }

        internal bool InternalUpdate()
        {
            if (!IsPrepare || !Visible)
            {
                return false;
            }

            List<UIWidget> listNextUpdateChild = null;
            if (ListChild != null && ListChild.Count > 0)
            {
                listNextUpdateChild = m_listUpdateChild;
                var updateListValid = m_updateListValid;
                List<UIWidget> listChild = null;
                if (!updateListValid)
                {
                    if (listNextUpdateChild == null)
                    {
                        listNextUpdateChild = new List<UIWidget>();
                        m_listUpdateChild = listNextUpdateChild;
                    }
                    else
                    {
                        listNextUpdateChild.Clear();
                    }

                    listChild = ListChild;
                }
                else
                {
                    listChild = listNextUpdateChild;
                }

                for (int i = 0; i < listChild.Count; i++)
                {
                    var uiWidget = listChild[i];

                    NWProfiler.BeginSample(uiWidget.name);
                    var needValid = uiWidget.InternalUpdate();
                    NWProfiler.EndSample();

                    if (!updateListValid && needValid)
                    {
                        listNextUpdateChild.Add(uiWidget);
                    }
                }

                if (!updateListValid)
                {
                    m_updateListValid = true;
                }
            }

            NWProfiler.BeginSample("OnUpdate");

            bool needUpdate = false;
            if (listNextUpdateChild == null || listNextUpdateChild.Count <= 0)
            {
                HasOverrideUpdate = true;
                OnUpdate();
                needUpdate = HasOverrideUpdate;
            }
            else
            {
                OnUpdate();
                needUpdate = true;
            }

            NWProfiler.EndSample();

            return needUpdate;
        }

        internal void InternalDestroy()
        {
            _isCreate = false;

            RemoveAllUIEvent();

            for (int i = 0; i < ListChild.Count; i++)
            {
                var uiChild = ListChild[i];
                uiChild.OnDestroy();
                uiChild.OnDestroyWidget();
            }

            //注销回调函数
            _prepareCallback = null;
        
            OnDestroy();

            //销毁面板对象
            if (_panel != null)
            {
                UnityEngine.Object.Destroy(_panel);
                _panel = null;
            }
        }

        /// <summary>
        /// 处理资源加载完成回调
        /// </summary>
        /// <param name="handle">资源句柄</param>
        private void Handle_Completed(AssetOperationHandle handle)
        {
            if (handle == null)
            {
                throw new NWFrameworkException("Load uiWindows failed because AssetOperationHandle is null");
            }
            if (handle.AssetObject == null)
            {
                throw new NWFrameworkException("Load uiWindows Failed because AssetObject is null");
            }

            //实例化对象
            var panel = handle.InstantiateSync(UIModule.UIRootStatic);
            if (!NeedCache)
            {
                AssetReference.BindAssetReference(panel, Handle, AssetName);
            }
            Handle_Completed(panel);
        }

        /// <summary>
        /// 处理资源加载完成回调
        /// </summary>
        /// <param name="panel">面板资源实例</param>
        private void Handle_Completed(GameObject panel)
        {
            if (panel == null)
            {
                return;
            }

            _panel = panel;
            _panel.transform.localPosition = Vector3.zero;

            //获取组件
            _canvas = _panel.GetComponent<Canvas>();
            if (_canvas == null)
            {
                throw new NWFrameworkException($"Not found {nameof(Canvas)} in panel {WindowName}");
            }

            _canvas.overrideSorting = true;
            _canvas.sortingOrder = 0;
            _canvas.sortingLayerName = "Default";

            //获取组件
            _raycaster = _panel.GetComponent<GraphicRaycaster>();
            _childCanvas = _panel.GetComponentsInChildren<Canvas>(true);
            _childRaycaster = _panel.GetComponentsInChildren<GraphicRaycaster>(true);

            //通知UI管理器
            IsPrepare = true;
            _prepareCallback?.Invoke(this);
        }

        protected virtual void Close()
        {
            GameModule.UI.CloseWindow(this.GetType());
        }

    }
}