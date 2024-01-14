using UnityEditor;
using UnityEngine;

public class DragAreaEditor
{
    public Vector2 ScrollPos;
    public float Width;
    public bool IsReset;
    public float StartX;
    public float StartY;
    public float StartMousePosX;

    private Rect _rect = new Rect();
    private bool _isScroll;

    public DragAreaEditor(float x, float y, float initWidth)
    {
        StartX = x;
        StartY = y;
        Width = initWidth;
    }

    public void BeginVerticalDragArea(bool isScroll = true)
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(Width), GUILayout.Height(Screen.height));
        _isScroll = isScroll;
        if (_isScroll)
            ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);
    }

    public void EndVerticalDragArea(System.Action action)
    {
        if (_isScroll)
            EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        _rect.x = StartX + Width - 5f;
        _rect.y = StartY;
        _rect.width = 10f;
        _rect.height = Screen.height;
        EditorGUIUtility.AddCursorRect(_rect, MouseCursor.ResizeHorizontal);

        if (Event.current.type == EventType.MouseDown && _rect.Contains(Event.current.mousePosition))
        {
            IsReset = true;
            StartMousePosX = Event.current.mousePosition.x;
        }

        if (IsReset)
        {
            Width = Mathf.Clamp(Width + Event.current.mousePosition.x - StartMousePosX, 100f, Screen.width - 100f);
            StartMousePosX = Event.current.mousePosition.x;
            action?.Invoke();
        }

        if (Event.current.type == EventType.MouseUp)
        {
            IsReset = false;
        }
    }
}
