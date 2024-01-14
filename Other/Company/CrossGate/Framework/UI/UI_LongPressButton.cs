using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UI_LongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
{
    public Vector2 position;
    public bool interactable = true;
    public bool invokeOnce = false;
    public bool handInvoke = false;
    public float interval = 0.5f; //长按的时间
    private bool isPointerDown = false;
    private float recordTime;

    public UnityEvent onClickDown = new UnityEvent();
    public UnityEvent onStartPress = new UnityEvent();
    public UnityEvent onPress = new UnityEvent();
    public UnityEvent onRelease = new UnityEvent();
    public UnityEvent OnPressAcc = new UnityEvent(); //带加速效果的event   
    public UnityEvent onClick = new UnityEvent();    
    
    public class LongPress : UnityEvent<float> { }

    public LongPress onLongPress=new LongPress() ;
    public bool bLongPressed = false;
    public bool hasStartPressProcessed = false;

    //加速效果的长按
    public bool bPressAcc = false;
    private float initSpeed = 0f;
    private float accSpeed = 0f;
    private float accMaxTime = 0f;
    private float accTimer = 0f;

    void Start()
    {
        initSpeed = 1f; //674;
        accSpeed = 1f;
        accMaxTime = 10f;
    }

    void OnDisable()
    {
        isPointerDown = false;
    }

    void Update()
    {
        if ((invokeOnce && handInvoke) || interactable == false) return;
        if (isPointerDown)
        {
            if ((Time.time - recordTime) > interval)
            {
                if (!hasStartPressProcessed)
                {
                    onStartPress?.Invoke();
                    hasStartPressProcessed = true;
                }

                onPress.Invoke();
                handInvoke = true;
                float dt = Time.time - recordTime - interval;
                onLongPress.Invoke(dt);
                bLongPressed = true;

                if (bPressAcc)
                {
                    //if (Time.time - accTimer < 1f) //每秒执行一次
                    //    return;

                    float timetotal = dt < accMaxTime ? dt : accMaxTime;
                    int times = Mathf.FloorToInt(initSpeed * dt + 0.5f * accSpeed * dt * dt);
                    for (int i = 0; i < times; ++i)
                    {
                        OnPressAcc?.Invoke();
                    }
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
#endif
        isPointerDown = true;
        hasStartPressProcessed = false;
        recordTime = Time.time;
        position = eventData.position;
        onClickDown.Invoke();
        bLongPressed = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
#endif
        isPointerDown = false;
        hasStartPressProcessed = false;
        handInvoke = false;
        position = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
#endif
        isPointerDown = false;
        handInvoke = false;
        position = eventData.position;
        onRelease.Invoke();
    }

    public void RemoveAddListener()
    {
        onClickDown.RemoveAllListeners();
        onPress.RemoveAllListeners();
        onStartPress?.RemoveAllListeners();
        onRelease.RemoveAllListeners();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!hasStartPressProcessed)
        {
            onClick.Invoke();
        }
    }
}
