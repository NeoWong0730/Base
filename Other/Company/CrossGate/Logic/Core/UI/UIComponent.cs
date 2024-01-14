using UnityEngine;

namespace Logic.Core
{
    public class UIComponent
    {
        public GameObject gameObject { get; private set; }
        public RectTransform rectTransform { get; private set; }
        public Transform transform { get; private set; }
        public bool isProcessEvents { get; private set; } = false;
        public bool enabled
        {
            get
            {
                return gameObject == null ? false : gameObject.activeInHierarchy;
            }
        }

        public UIComponent Init(Transform root)
        {
            this.transform = root;
            this.gameObject = root.gameObject;
            this.rectTransform = transform as RectTransform;
            this.Loaded();
            return this;
        }
        public void SetName(string name)
        {
            if (gameObject != null)
            {
                gameObject.name = name;
            }
        }
        public virtual void OnDestroy()
        {
            if (isProcessEvents)
                ProcessEventsForEnable(false);
            isProcessEvents = false;

            ProcessEventsForAwake(false);
            if (gameObject != null)
            {
                GameObject.Destroy(gameObject);
                transform = null;
                rectTransform = null;
            }
        }
        public void ShowHide(bool toShow)
        {
            if (toShow)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
        public virtual void Show()
        {
            if (!isProcessEvents)
                ProcessEventsForEnable(true);
            isProcessEvents = true;
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }
        public virtual void Hide()
        {
            if (isProcessEvents)
                ProcessEventsForEnable(false);
            isProcessEvents = false;
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
        public virtual void Reset() { }
        public void ExecUpdate()
        {
            if (enabled)
            {
                Update();
            }
        }
        public void OnRefresh()
        {
            Refresh();
        }

        protected virtual void Loaded() { }
        public virtual void SetData(params object[] arg) { }
        protected virtual void Update() { }
        protected virtual void Refresh() { }
        protected virtual void ProcessEventsForEnable(bool toRegister) { }
        protected virtual void ProcessEventsForAwake(bool toRegister) { }

        public TComponent AddComponent<TComponent>(Transform go) where TComponent : UIComponent, new()
        {
            TComponent component = new TComponent();
            component.Init(go);
            return component;
        }
    }

    public class UIElement : UIComponent
    {
        public int id { get; protected set; }

        public UIElement SetUniqueId(int id) { this.id = id; return this; }
    }
}
