using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

public class AudioWindow : DebugWindowBase
{
    public AudioWindow(int id) : base(id) { }
    Vector2 systemInfoPos = Vector2.zero;

    private string sType = "Type";
    private string volume = "volume";

    private string sHideNormal = "筛选";

    private bool bHideNormal = false;

    public override void WindowFunction(int id)
    {
        bHideNormal = UnityEngine.GUILayout.Toggle(bHideNormal, sHideNormal);        

        UnityEngine.GUILayout.BeginHorizontal();
        UnityEngine.GUILayout.Label(sType, UnityEngine.GUILayout.Width(vSize.x * 0.4f));
        UnityEngine.GUILayout.Label(volume, UnityEngine.GUILayout.Width(vSize.x * 0.1f));
        UnityEngine.GUILayout.EndHorizontal();        

        systemInfoPos = GUILayout.BeginScrollView(systemInfoPos);
        AudioManager.Instance.OnGUI(vSize, bHideNormal);
        GUILayout.EndScrollView();
    }
}
