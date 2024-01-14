using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Lib.Core;
using UnityEngine.Events;

public class ButtonLongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float LongPressTime = 2f;

    public Transform PressingHightLight = null;
    public class ButtonClickedEvent : UnityEvent { }

    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

    private Timer timer;
    public ButtonClickedEvent onClick
    {
        get { return m_OnClick; }
        set { m_OnClick = value; }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        OnPress(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPress(false);
    }

    protected virtual void OnPress(bool press)
    {
        timer?.Cancel();

        if (press)
        {
            timer = Timer.Register(LongPressTime, OnLongPress);
        }

        if (PressingHightLight != null && PressingHightLight.gameObject.activeSelf != press)
            PressingHightLight.gameObject.SetActive(press);
    }

    private void OnLongPress()
    {
        m_OnClick.Invoke();
    }

    private void OnEnable()
    {

        if (PressingHightLight != null && PressingHightLight.gameObject.activeSelf != false)
            PressingHightLight.gameObject.SetActive(false);
    }
    private void OnDisable() {

        timer?.Cancel();

        if (PressingHightLight != null && PressingHightLight.gameObject.activeSelf != false)
            PressingHightLight.gameObject.SetActive(false);
    }


}
