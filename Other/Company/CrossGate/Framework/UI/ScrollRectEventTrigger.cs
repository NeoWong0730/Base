using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ScrollRect + EventTrigger
/// 自行拓展消息
/// </summary>
public class ScrollRectEventTrigger : ScrollRect
{
    public EventTrigger.TriggerEvent onBeginDrag { get; set; } = new EventTrigger.TriggerEvent();
    public EventTrigger.TriggerEvent onEndDrag { get; set; } = new EventTrigger.TriggerEvent();

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        onBeginDrag?.Invoke(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        onEndDrag?.Invoke(eventData);
    }
}
