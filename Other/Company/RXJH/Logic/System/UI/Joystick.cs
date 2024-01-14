using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using System;

namespace Logic
{
    public class Joystick : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IDragHandler,
        IEndDragHandler
    {
        [Header("General Settings")]
        [HelpBox(" Skill name which pass to skill handler")]
        public string inputName;

        [HelpBox("The default draggable area of joystick / Skill button is the image width of the outer circle.  This is used to adjust the default area ")]
        public float draggableRadiusModifier = 0;

        [HelpBox("Set only if you want the result relative to the target object (e.g.The camera)")]
        public Transform relativeTransform;

        [Header("Events Settings")]
        [HelpBox("CallBack when Drag Begin - Function(string inputName)")]
        public Action<string> beginDragEvent;

        public Sprite analogImage;
        public Sprite analogAreaImage;

        private Image joystickImage; 
        private RectTransform rect;
        private Vector2 parentPosition;
        private Vector3 relativeForward;
        private float maxDisplacement = 0;
        private bool IsEndDrag = false;
        private Vector2 draggingTarget;

        [ReadOnly]
        public Vector2 result;

        void Start()
        {
            joystickImage = transform.Find("Joystick").GetComponent<Image>();
            if (analogImage != null && joystickImage != null)
            {
                joystickImage.sprite = analogImage;
            }
            if (analogAreaImage != null && GetComponent<Image>() != null)
            {
                GetComponent<Image>().sprite = analogAreaImage;
            }

            rect = gameObject.GetComponent<RectTransform>();
            maxDisplacement = (rect.rect.width / 2) + draggableRadiusModifier;

            relativeTransform = UIManager.mUICamera.transform;
        }

        public virtual void OnPointerDown(PointerEventData eventArgs)
        {
            parentPosition = new Vector2(transform.position.x, transform.position.y);
            if (beginDragEvent != null)
            {
                beginDragEvent.Invoke(inputName);
            }
            if (relativeTransform != null)
            {
                relativeForward = relativeTransform.forward;
            }
            OnDrag(eventArgs);
        }

        public virtual void OnDrag(PointerEventData eventArgs)
        {
            Vector2 relativePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventArgs.position, eventArgs.pressEventCamera, out relativePosition);
            Vector2 targetMarkerImagePosition;

            if (relativePosition.magnitude > maxDisplacement)
            {
                relativePosition = relativePosition.normalized * maxDisplacement;
            }
            targetMarkerImagePosition = relativePosition;

            relativePosition = relativePosition.GetRelativeTransformedPosition(relativeForward);
            draggingTarget = GetFinalResult(relativePosition);

            if (joystickImage != null)
            {
                joystickImage.transform.localPosition = targetMarkerImagePosition;
            }
            ReturnResult(relativePosition);
        }

        public virtual void OnEndDrag(PointerEventData eventArgs)
        {
            if (joystickImage != null)
            {
                joystickImage.transform.position = transform.position;
            }
            ReturnResult(Vector2.zero);
        }

        public virtual void OnPointerUp(PointerEventData eventArgs)
        {
            OnEndDrag(eventArgs);
        }

        void ReturnResult(Vector2 relativePosition)
        {
            PlayerSelfControlSystem.eventEmitter.Trigger<string, Vector2>(PlayerSelfControlSystem.EEvents.OnJoystickDrag, inputName, GetFinalResult(relativePosition));
            IsEndDrag = true;
        }

        Vector2 GetFinalResult(Vector2 relativePosition)
        {
            result = relativePosition / maxDisplacement;
            return result;
        }
    }
}
