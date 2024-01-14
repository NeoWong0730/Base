using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelWindow : DebugWindowBase
{
    public LevelWindow(int id) : base(id) { }

    Vector2 pos = Vector2.zero;

    public override void WindowFunction(int id)
    {
        pos = GUILayout.BeginScrollView(pos);
        LevelManager.OnGUI();
        GUILayout.EndScrollView();
    }
}
