using UnityEngine;
#if UNITY_EDITOR
using NaughtyAttributes;
#endif

// 运动轨迹绘制
// https://blog.csdn.net/moonlightpeng/article/details/83511968
[ExecuteInEditMode]
public class CP_TravelDrawer : MonoBehaviour
{
#if UNITY_EDITOR
    public LineRenderer line;

	private Vector3 moveStart;
	private Vector3 moveNext;
	private int i = 0;

	private void Start()
	{
		moveStart = transform.position;

        line = gameObject.GetComponent<LineRenderer>();
        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
            line.positionCount = 1;
            line.SetPosition(0, transform.position);
            line.widthMultiplier = 0.02f;
        }
    }
	private void Update()
	{
        if (Time.frameCount % 2 == 0)
        {
            moveNext = transform.position;
            if (moveStart != moveNext)
            {
                i++;
                line.positionCount = i;
                line.SetPosition(i - 1, transform.position);
            }
            moveStart = moveNext;
        }
	}

    [ContextMenu("Clear")]
    [Button("Clear")]
    public void Clear()
	{
        line.positionCount = 1;
        i = 0;
        line.SetPosition(0, transform.position);
        moveStart = transform.position;
    }
#endif
}
