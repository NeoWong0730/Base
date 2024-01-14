using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolWindow : DebugWindowBase
{
    public PoolWindow(int id) : base(id) { }
    Vector2 systemInfoPos = Vector2.zero;
    private string sInfo = $"*使用中数量不代表实际内存中存在的数量, 仅仅表示没有调用回收接口的回收数量*";

    private string sType = "Type";
    private string sFetch = "Fetch";
    private string sPool = "Pool";
    private string sPoolSize = "PoolCapacity";
    private string sAllocator = "Allocator";
    private string sCollect = "Collect";

    private string sHideNormal = "筛选";

    private bool bHideNormal = false;

    public override void WindowFunction(int id)
    {
        Color color = GUI.color;
        GUI.color = Color.red;
        GUILayout.Label(sInfo);
        GUI.color = color;

        bHideNormal = UnityEngine.GUILayout.Toggle(bHideNormal, sHideNormal);        

        UnityEngine.GUILayout.BeginHorizontal();
        UnityEngine.GUILayout.Label(sType, UnityEngine.GUILayout.Width(vSize.x * 0.4f));
        UnityEngine.GUILayout.Label(sFetch, UnityEngine.GUILayout.Width(vSize.x * 0.1f));
        UnityEngine.GUILayout.Label(sPool, UnityEngine.GUILayout.Width(vSize.x * 0.1f));
        UnityEngine.GUILayout.Label(sPoolSize, UnityEngine.GUILayout.Width(vSize.x * 0.1f));
        UnityEngine.GUILayout.Label(sAllocator, UnityEngine.GUILayout.Width(vSize.x * 0.1f));
        UnityEngine.GUILayout.Label(sCollect, UnityEngine.GUILayout.Width(vSize.x * 0.1f));
        UnityEngine.GUILayout.EndHorizontal();        

        systemInfoPos = GUILayout.BeginScrollView(systemInfoPos);
        PoolManager.OnGUI_Pool(vSize, bHideNormal);
        GUILayout.EndScrollView();
    }
}
