using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace Framework
{
    public class TouchInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Action<Vector3> SendTouchUp;
        public Action<Vector3> SendTouchLongPress;

        // Start is called before the first frame update

        private PointerEventData eventdata;
        private float recordTime;
        public float interval; //长按的时间

        private bool isPointerDown = false;

        // Update is called once per frame
        void Update()
        {
            if (isPointerDown)
            {
                if ((Time.time - recordTime) > interval)
                {
                    SendTouchLongPress(eventdata.position);
                    isPointerDown = false;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            recordTime = Time.time;
            eventdata = eventData;
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            //mDrag = false;

            //Vector2 templocalPoint;
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, eventData.position, eventData.pressEventCamera, out templocalPoint);
            //SetPos(templocalPoint);
            //evt.state = JoystickState.Start;

            //ActionEvt?.Invoke(evt);

            //RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransBackGround, eventData.position, eventData.pressEventCamera, out mMouseClickPosition);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            //Vector2 templocalPoint;
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, eventData.position, eventData.pressEventCamera, out templocalPoint);
            //evt.state = JoystickState.End;
            //SetPos(Vector2.zero);
            //ActionEvt?.Invoke(evt);
            //tempposition = Vector2.zero;
            //mMouseClickPosition = Vector2.zero;
            SendTouchUp(eventData.position);
        }
    }
}


