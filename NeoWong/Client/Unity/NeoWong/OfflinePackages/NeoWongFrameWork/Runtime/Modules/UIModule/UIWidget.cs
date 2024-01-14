using System.Collections.Generic;
using UnityEngine;

namespace NWFramework
{
    public abstract class UIWidget : UIBase, IUIBehaviour
    {
        /// <summary>
        /// 窗口组件的实例资源对象
        /// </summary>
        public override GameObject gameObject { get; protected set; }

        /// <summary>
        /// 窗口组件矩阵位置组件
        /// </summary>
        public override RectTransform rectTransform { get; protected set; }

        /// <summary>
        /// 窗口位置组件
        /// </summary>
        public override Transform transform { get; protected set; }

        /// <summary>
        /// 窗口组件名称。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string name { get; private set; } = nameof(UIWidget);

        /// <summary>
        /// UI类型。
        /// </summary>
        public override UIBaseType BaseType => UIBaseType.Widget;

        /// <summary>
        /// 所属的窗口
        /// </summary>
        public UIWindow OnwnerWindow
        {
            get
            {
                var parentUI = base.parent;
                while (parentUI != null)
                {
                    if (parentUI.BaseType == UIBaseType.Window)
                    {
                        return parentUI as UIWindow;
                    }

                    parentUI = parentUI.Parent;
                }

                return null;
            }
        }

        /// <summary>
        /// 窗口可见性
        /// </summary>
        public bool Visible
        {
            get => gameObject.activeSelf;

            set
            {
                gameObject.SetActive(value);
                OnSetVisible(value);
            }
        }

        internal bool InternalUpdate()
        {
            if (!IsPrepare)
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

        /// <summary>
        /// 创建窗口内嵌的界面
        /// </summary>
        /// <param name="parentUI">父节点UI</param>
        /// <param name="widgetRoot">组件根节点</param>
        /// <param name="visible">是否可见</param>
        /// <returns>是否创建成功</returns>
        public bool Create(UIBase parentUI, GameObject widgetRoot, bool visible = true)
        {
            return CreateImp(parentUI, widgetRoot, false, visible);
        }

        /// <summary>
        /// 根据资源名创建
        /// </summary>
        /// <param name="resPath">资源路径</param>
        /// <param name="parentUI">父节点UI</param>
        /// <param name="parentTrans">父节点Transform</param>
        /// <param name="visible">是否可见</param>
        /// <returns>是否创建成功</returns>
        public bool CreateByPath(string resPath, UIBase parentUI, Transform parentTrans = null, bool visible = true)
        {
            GameObject goInst = GameModule.Resource.LoadAsset<GameObject>(resPath, parentTrans);
            if (goInst == null)
            {
                return false;
            }

            if (!Create(parentUI, goInst, visible))
            {
                return false;
            }

            goInst.transform.localScale = Vector3.one;
            goInst.transform.localPosition = Vector3.zero;
            
            return true;
        }

        /// <summary>
        /// 根据prefab或者模版来创建新的 widget
        /// </summary>
        /// <remarks>存在父物体得资源故不需要异步加载。</remarks>
        /// <param name="parentUI">父物体UI</param>
        /// <param name="goPrefab">实例化预制体</param>
        /// <param name="parentTrans">实例化父节点</param>
        /// <param name="visible">是否可见</param>
        /// <returns>是否创建成功</returns>
        public bool CreateByPrefab(UIBase parentUI, GameObject goPrefab, Transform parentTrans = null, bool visible = true)
        {
            if (parentTrans == null)
            {
                parentTrans = parentUI.rectTransform;
            }

            return CreateImp(parentUI, UnityEngine.Object.Instantiate(goPrefab, parentTrans), true, visible);
        }

        private bool CreateImp(UIBase parentUI, GameObject widgetRoot, bool bindGo, bool visible = true)
        {
            if (!CreateBase(widgetRoot, bindGo))
            {
                return false;
            }

            ResetChildCanvas(parentUI);
            parent = parentUI;
            Parent.ListChild.Add(this);
            ScriptGenerator();
            BindMemberProperty();
            RegisterEvent();
            OnCreate();
            OnRefresh();
            IsPrepare = true;

            if (!visible)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }
            }

            return true;
        }

        private bool CreateBase(GameObject go, bool bindGo)
        {
            if (go == null)
            {
                return false;
            }

            transform = go.GetComponent<Transform>();
            rectTransform = transform as RectTransform;
            gameObject = go;
            Log.Assert(rectTransform != null, $"{go.name} ui base element need to be RectTransform");
            return true;
        }

        private void ResetChildCanvas(UIBase parentUI)
        {
            if (parentUI == null || Parent.gameObject == null)
            {
                return;
            }

            Canvas parentCanvas = parentUI.gameObject.GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                return;
            }

            if (gameObject != null)
            {
                var listCanvas = gameObject.GetComponentsInChildren<Canvas>(true);
                for (int index = 0; index < listCanvas.Length; index++)
                {
                    var childCanvas = listCanvas[index];
                    childCanvas.sortingOrder = parentCanvas.sortingOrder + childCanvas.sortingOrder % UIModule.WINDOW_DEEP;
                }
            }
        }

        /// <summary>
        /// 组件被销毁调用
        /// <remarks>请勿手动调用！</remarks>
        /// </summary>
        internal void OnDestroyWidget()
        {
            RemoveAllUIEvent();

            foreach (var uiChild in ListChild)
            {
                uiChild.OnDestroy();
                uiChild.OnDestroyWidget();
            }

            if (gameObject != null)
            {
                UnityEngine.Object.Destroy(gameObject);
            }
        }

        /// <summary>
        /// 主动销毁组件
        /// </summary>
        public void Destroy()
        {
            if (parent != null)
            {
                parent.ListChild.Remove(this);
                OnDestroy();
                OnDestroyWidget();
            }
        }
    }
}