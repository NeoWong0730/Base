using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic.Core {
    [Serializable]
    public class UIComponent {
        public RectTransform transform { get; private set; }

        public UIComponent Init(Transform root) {
            this.transform = root.transform as RectTransform;
            this.Loaded();
            return this;
        }

        public void ShowHide(bool toShow) {
            if (toShow) {
                Show();
            }
            else {
                Hide();
            }
        }

        public virtual void Show() {
            transform.gameObject.SetActive(true);
        }

        public virtual void Hide() {
            transform.gameObject.SetActive(false);
        }

        protected virtual void Loaded() {
        }

        public void SetName(string name) {
            if (transform != null) {
                transform.name = name;
            }
        }

        public virtual void OnDestroy() {
            if (transform != null) {
                GameObject.Destroy(transform.gameObject);
                transform = null;
            }
        }

        public TComponent AddComponent<TComponent>(Transform go) where TComponent : UIComponent, new() {
            TComponent component = new TComponent();
            component.Init(go);
            return component;
        }
    }

    [Serializable]
    public class UISelectable : UIComponent {
        public uint id { get; protected set; }
        public int index { get; protected set; }
        protected Action<uint /*id*/, int /*index*/, bool> onSelected = null;

        public UISelectable SetId(uint id, int index) {
            this.id = id;
            this.index = index;
            return this;
        }

        public UISelectable SetSelectedAction(Action<uint /*id*/, int /*index*/, bool> onSelected) {
            this.onSelected = onSelected;
            return this;
        }

        public virtual void SetSelected(bool toSelected, bool force) {
        }
    }

    [Serializable]
    public class UIToggleSelectable : UISelectable {
        public ToggleEx toggle { get; protected set; }

        public void AddToggleListener(ToggleEx toggle, bool toListen) {
            this.toggle = toggle;

            if (toggle) {
                if (toListen) {
                    toggle.onValueChanged.AddListener(Switch);
                }
                else {
                    toggle.onValueChanged.RemoveListener(Switch);
                }
            }
        }

        public override void SetSelected(bool toSelected, bool force) {
            if (this.toggle != null) {
                this.toggle.SetSelected(toSelected, force);
            }
        }

        public void Switch(bool arg, bool interaction) {
            if (arg) {
                this.onSelected?.Invoke(id, index, interaction);
            }
        }
    }

    [Serializable]
    public abstract class UILayoutBase
#if UNITY_EDITOR
        : MonoBehaviour
#endif
    {
#if UNITY_EDITOR
        private void Reset() {
            this.hideFlags = HideFlags.DontSaveInBuild;
            if (Application.isPlaying) {
                this.hideFlags |= HideFlags.HideInInspector;
            }

            this.FindByPath(transform, true);
        }
        
        [ContextMenu(nameof(SyncToBinder))]
        private void SyncToBinder() {
            if (!transform.TryGetComponent<UIComponentBinder>(out UIComponentBinder r)) {
                r = transform.gameObject.AddComponent<UIComponentBinder>();
            }

            r.fieldStyle = UIComponentBinder.EFieldStyle.Field;
            r.bindComponents = this.Collect();
        }

        private List<UIComponentBinder.BindComponent> Collect() {
            var fis = this.GetType().GetFields();
            List<UIComponentBinder.BindComponent> array = new List<UIComponentBinder.BindComponent>();
            foreach (var fi in fis) {
                if (!fi.FieldType.IsSubclassOf(typeof(UnityEngine.Component))) {
                    continue;
                }

                var cp = fi.GetValue(this) as UnityEngine.Component;
                var a = new UIComponentBinder.BindComponent() {
                    component = cp,
                    toListen = true,
                    name = fi.Name
                };
                array.Add(a);
            }

            return array;
        }

        protected virtual void FindByPath(Transform transform, bool check = false) {
        }
#endif
        public static TLayout GetLayout<TLayout>(Transform t) where TLayout : UILayoutBase, new() {
#if UNITY_EDITOR
            return t.gameObject.AddComponent<TLayout>();
#endif
            return new TLayout();
        }

        protected UIComponentBinder binder;

        public UILayoutBase Init(Transform transform) {
            binder = transform.GetComponent<UIComponentBinder>();
            if (binder != null) {
                Loaded();
            }

            return this;
        }

        protected virtual void Loaded() {
        }
    }

    [Serializable]
    public class NullLayout : UILayoutBase {
    }

    [Serializable]
    public class VD<TLayout> : UIComponent where TLayout : UILayoutBase, new() {
        public TLayout layout;

        protected override void Loaded() {
            layout = UILayoutBase.GetLayout<TLayout>(this.transform);
            layout.Init(this.transform);
        }
    }
}