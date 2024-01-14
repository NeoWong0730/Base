using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lib.Core {
    // Input事件监听器，比如点击事件，Drag事件的监听
    [DisallowMultipleComponent]
    public class EventTrigger : UnityEngine.EventSystems.EventTrigger {
        public delegate void VoidDelegate(GameObject go);

        public delegate void BoolDelegate(GameObject go, bool state);

        public delegate void FloatDelegate(GameObject go, float delta);

        public delegate void VectorDelegate(GameObject go, Vector2 delta);

        public delegate void Vector3Delegate(GameObject go, Vector3 delta);

        public delegate void ObjectDelegate(GameObject go, GameObject obj);

        public VoidDelegate onClick;
        public VoidDelegate onDoubleClick;
        public BoolDelegate onHover;
        public BoolDelegate onPress;
        public FloatDelegate onScroll;
        public VectorDelegate onDrag;
        public VoidDelegate onDragStart;
        public VoidDelegate onDragEnd;
        public ObjectDelegate onDragOver;
        public ObjectDelegate onDragOut;

        public IList<ScrollRect> scrollRects;

        public bool IsDraging { get; private set; } = false;

        //private void Awake() { scrollRect = GetComponentInParent<ScrollRect>(); }

        private void Start() {
            scrollRects = GetComponentsInParent<ScrollRect>();
        }

        public static EventTrigger Get(Component cp) {
            return Get(cp?.gameObject);
        }

        public static EventTrigger Get(GameObject go) {
            EventTrigger trigger = null;
            if (go != null) {
                trigger = go.GetComponent<EventTrigger>();
                if (trigger == null) {
                    trigger = go.AddComponent<EventTrigger>();
                }
            }

            return trigger;
        }

        public static void AddEventListener(GameObject go, EventTriggerType eType, UnityAction<BaseEventData> action) {
            EventTrigger cp = Get(go);
            if (cp != null) {
                cp.AddEventListener(eType, action);
            }
        }

        public void AddEventListener(EventTriggerType eType, UnityAction<BaseEventData> action) {
            if (action != null) {
                Entry entry = new Entry();
                entry.eventID = eType;
                entry.callback.AddListener(action);
                triggers ??= new List<Entry>();
                triggers.Add(entry);
            }
        }

        public void ClearEvents() {
            if (triggers != null) {
                triggers.Clear();
            }
        }

        #region 回调函数

        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            onClick?.Invoke(gameObject);

            if (eventData.clickCount == 2) {
                onDoubleClick?.Invoke(gameObject);
            }

            // 双击原来不是使用eventData.clickTime
            // if (Time.time > 0.35f + eventData.clickTime && onDoubleClick != null) { onDoubleClick(gameObject); }
        }

        public override void OnPointerDown(PointerEventData eventData) {
            base.OnPointerDown(eventData);
            onPress?.Invoke(gameObject, true);
        }

        public override void OnPointerUp(PointerEventData eventData) {
            base.OnPointerUp(eventData);
            onPress?.Invoke(gameObject, false);
        }

        public override void OnPointerEnter(PointerEventData eventData) {
            base.OnPointerEnter(eventData);
            onHover?.Invoke(gameObject, true);

            if (eventData.dragging) {
                onDragOver?.Invoke(gameObject, eventData.pointerDrag);
            }
        }

        public override void OnPointerExit(PointerEventData eventData) {
            base.OnPointerExit(eventData);
            onHover?.Invoke(gameObject, false);

            if (eventData.dragging) {
                onDragOut?.Invoke(gameObject, eventData.pointerDrag);
            }
        }

        // 在ScrollRect中如果使用EventListener，但是不实现ScrollRect的drag系列函数的话，就会导致失去拖拽功能，所以这里实现
        // https://blog.csdn.net/qiangqiang_0420/article/details/51375856
        public override void OnDrag(PointerEventData eventData) {
            IsDraging = true;
            base.OnDrag(eventData);
            onDrag?.Invoke(gameObject, eventData.delta);

            if (scrollRects != null) {
                for (int i = 0, length = scrollRects.Count; i < length; ++i) {
                    scrollRects[i]?.OnDrag(eventData);
                }
            }
        }

        public override void OnBeginDrag(PointerEventData eventData) {
            base.OnDrag(eventData);
            onDragStart?.Invoke(gameObject);

            if (scrollRects != null) {
                for (int i = 0, length = scrollRects.Count; i < length; ++i) {
                    scrollRects[i]?.OnBeginDrag(eventData);
                }
            }
        }

        public override void OnEndDrag(PointerEventData eventData) {
            base.OnDrag(eventData);
            onDragEnd?.Invoke(gameObject);

            if (scrollRects != null) {
                for (int i = 0, length = scrollRects.Count; i < length; ++i) {
                    scrollRects[i]?.OnEndDrag(eventData);
                }
            }

            IsDraging = false;
        }

        public override void OnScroll(PointerEventData eventData) {
            base.OnDrag(eventData);
            onScroll?.Invoke(gameObject, Input.GetAxis("Mouse ScrollWheel"));

            if (scrollRects != null) {
                for (int i = 0, length = scrollRects.Count; i < length; ++i) {
                    scrollRects[i]?.OnScroll(eventData);
                }
            }
        }

        #endregion
    }
}