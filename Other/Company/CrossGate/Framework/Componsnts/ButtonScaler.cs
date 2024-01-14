using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Lib.Core;

// 保证初始scale为1
public class ButtonScaler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float scaleTime = 0.2f;
    public float scaleRate = 0.85f;
    public Transform target;

    private Timer timer;
    private float currentScale;

    public void OnPointerDown(PointerEventData eventData)
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
#endif
        OnPress(true);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
#endif
        OnPress(false);
    }

    protected virtual void OnPress(bool press)
    {
        timer?.Cancel();

        if (press)
        {
            currentScale = 1;
            timer = Timer.RegisterOrReuse(ref timer, scaleTime, () => {
                target.localScale = new Vector3(scaleRate, scaleRate, scaleRate);
            }, (progress) => {
                currentScale = Mathf.Lerp(currentScale, scaleRate, progress / scaleTime);
                target.localScale = new Vector3(currentScale, currentScale, currentScale);
            });
        }
        else
        {
            timer = Timer.RegisterOrReuse(ref timer, scaleTime, () => {
                target.localScale = Vector3.one;
            }, (progress) => {
                currentScale = Mathf.Lerp(currentScale, 1, progress / scaleTime);
                target.localScale = new Vector3(currentScale, currentScale, currentScale);
            });
        }
    }
    private void Awake()
    {
        if (target == null)
        {
            target = transform;
        }
    }
    private void OnEnable() { ResetScale(); }
    private void OnDisable() { timer?.Cancel(); }
    protected void ResetScale()
    {
        target.localScale = Vector3.one;
    }
}
