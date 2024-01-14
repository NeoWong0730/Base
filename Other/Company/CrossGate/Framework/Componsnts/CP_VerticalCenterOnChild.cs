using Lib.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CP_VerticalCenterOnChild : MonoBehaviour
{
    public System.Action onBeginCenter;
    public System.Action onFinished;
    public CP_ScrollRect scrollRect;
    public CP_ScrolCircleList scrollList;
    public float springStrength = 3f;

    public float stopSpeed = 60f;

    public Transform centerOn;

    private CoroutineHandler coroutine;

    public bool isMoving
    {
        get { return Mathf.Abs(scrollRect.velocity.y) > stopSpeed; }
    }

    private void Awake()
    {
        scrollRect.onEndDrag += OnEndDrag;
        scrollRect.onBeginDrag += OnBeginDrag;
    }
    private void OnBeginDrag(PointerEventData eventData)
    {
        TryStopCoroutine();
    }
    private void OnEndDrag(PointerEventData eventData)
    {
        if (enabled)
        {
            TryStopCoroutine();
            coroutine = CoroutineManager.Instance.StartHandler(Wait());
        }
    }
    private void TryStopCoroutine()
    {
        if (coroutine != null)
        {
            CoroutineManager.Instance.Stop(coroutine);
            coroutine = null;
        }
    }

    private IEnumerator Wait()
    {
        // 速度低于stopSpeed，则开始centerOn
        while (isMoving)
        {
            yield return null;
        }

        scrollList.scrollRect.StopMovement();
        Recenter();
        yield return null;
    }

    [ContextMenu("Recenter")]
	private void Recenter()
	{
        onBeginCenter?.Invoke();
    }
}
