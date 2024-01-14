using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework
{
    public class JoystickInput : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        private Vector2 tempposition;
        private RectTransform Thumb, Bound;
        public Action<JoystickData> ActionEvt;

        private float radius;
        private RectTransform rectTrans;

        private RectTransform rectTransBackGround;

        public JoystickData evt = new JoystickData();

        private Vector2 mMouseClickPosition = Vector2.zero;

        public Action<Vector3> SendTouchUp;
        public Action<JoystickData> SetLeftJoystick;
        public Action<Vector3> SendTouchRightUp; //右键事件
        public Action<Vector3> SendMoveFollowMouse;//跟随鼠标移动事件

        private bool isMoveByMouse = false;
        private bool mDrag = false;

        [SerializeField] private bool mJoytickEnable = true;

        public float mInterval = 2; //长按的时间
        private float mClickTime;//点击时间
        private int mPointerID = -1;
        public bool JoytickEnable
        {
            get { return mJoytickEnable; }
            set { mJoytickEnable = value; SetJoystick(value); }
        }
        private void SetJoystick(bool b)
        {
            if (!b)
            {
                HideJoy();
            }
        }
        protected virtual void Awake()
        {
            rectTrans = transform.Find("Joy") as RectTransform;
            if (Bound == null)
            {
                Bound = transform.Find("Joy/Bound") as RectTransform;
            }

            if (Thumb == null)
            {
                Thumb = transform.Find("Joy/Thumb") as RectTransform;
            }

            if (Bound != null)
            {
                radius = Bound.sizeDelta.x / 2 - Thumb.sizeDelta.x / 4;
            }

            rectTransBackGround = transform as RectTransform;
        }

        private void OnEnable()
        {
            if (Thumb != null)
            {
                ActionEvt += JoystickEvt;
            }

            mPointerID = -1;
        }

        private void Start()
        {
            HideJoy();
        }
        private void OnDisable()
        {
            evt.state = JoystickState.End;
            JoystickEvt(evt);
            if (Thumb != null)
            {
                ActionEvt -= JoystickEvt;
            }
        }

        protected virtual void JoystickEvt(JoystickData obj)
        {
            switch (obj.state)
            {
                case JoystickState.Start:
                    Thumb.localPosition = obj.dir * radius;
                    break;
                case JoystickState.Move:
                    Thumb.localPosition = obj.dir * radius;
                    break;
                case JoystickState.End:
                    Thumb.localPosition = Vector2.zero;
                    HideJoy();
                    break;
            }
        }

        private void ShowJoy()
        {
            if (rectTrans == null || rectTrans.gameObject.activeSelf || !mJoytickEnable)
                return;

            rectTrans.gameObject.SetActive(true);

            SetJoyPosition(mMouseClickPosition);
        }


        private void HideJoy()
        {
            if (rectTrans != null)
                rectTrans.gameObject.SetActive(false);

            SetPos(Vector2.zero);
        }

        private void SetJoyPosition(Vector3 pos)
        {
            rectTrans.anchoredPosition = (Vector2)pos + rectTransBackGround.sizeDelta / 2;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            mPointerID = eventData.pointerId;
            mClickTime = Time.time;
            isMoveByMouse = false;
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            mDrag = false;

            Vector2 templocalPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, eventData.position, eventData.pressEventCamera, out templocalPoint);
            SetPos(templocalPoint);


            evt.state = JoystickState.Start;

            ActionEvt?.Invoke(evt);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransBackGround, eventData.position, eventData.pressEventCamera, out mMouseClickPosition);
        }

        public void OnDrag(PointerEventData eventData)
        {
            isMoveByMouse = false;
            if (eventData.button != PointerEventData.InputButton.Left || eventData.pointerId != mPointerID)
                return;

            mDrag = true;

            evt.state = JoystickState.Move;


            Vector2 templocalPoint;

            if (mJoytickEnable)
            {
                ShowJoy();
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, eventData.position, eventData.pressEventCamera, out templocalPoint);

            ActionEvt?.Invoke(evt);

            SetPos(templocalPoint);

            if (Thumb != null)
            {
                float degree = VectorAngle(Vector2.up, evt.dir);
                Thumb.rotation = Quaternion.Euler(0f, 0f, degree);
            }
        }

        float VectorAngle(Vector2 from, Vector2 to)
        {
            float angle;
            Vector3 cross = Vector3.Cross(from, to);
            angle = Vector2.Angle(from, to);
            return cross.z < 0 ? -angle : angle;
        }

        public void FixedUpdate()
        {
            if (evt.state == JoystickState.Move)
            {
                ActionEvt?.Invoke(evt);
            }
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (mClickTime != 0 && evt.state == JoystickState.Start && Time.time - mClickTime >= mInterval)
            {
                isMoveByMouse = true;
            }
            if (isMoveByMouse)
                SendMoveFollowMouse(Input.mousePosition);
#endif

        }

        public void OnPointerUp(PointerEventData eventData)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (eventData.pointerId != mPointerID)
                return;
#else
            if (eventData.pointerId != mPointerID || eventData.button != PointerEventData.InputButton.Left)
                return;
#endif

            Vector2 templocalPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, eventData.position, eventData.pressEventCamera, out templocalPoint);
            evt.state = JoystickState.End;
            SetPos(Vector2.zero);


            ActionEvt?.Invoke(evt);
            tempposition = Vector2.zero;
            mMouseClickPosition = Vector2.zero;

            if (!mDrag && !isMoveByMouse)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                    SendTouchUp(eventData.position);
                else if (eventData.button == PointerEventData.InputButton.Right)
                    SendTouchRightUp(eventData.position);
            }
            mDrag = false;

            mPointerID = -1;
        }

        public void SetPos(Vector2 pos)
        {
            if (pos.magnitude > radius)
            {
                tempposition = pos.normalized * radius;
            }
            else
            {
                tempposition = pos;
            }
            evt.dir = evt.state == JoystickState.Move ? tempposition / radius : Vector2.zero;
            evt.dis = evt.dir.sqrMagnitude;

            SetLeftJoystick?.Invoke(evt);
        }

        private void OnMouseDrag()
        {

        }
    }
}


