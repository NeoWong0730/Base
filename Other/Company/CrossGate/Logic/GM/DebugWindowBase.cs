using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugWindowBase
{
    public int windowID;
    public float fScale;
    public Vector2 vSize;

    public DebugWindowBase(int id)
    {
        windowID = id;
    }

    public virtual void OnAwake() { }
    public virtual void OnDestroy() { }
    public virtual void WindowFunction(int id) { }
}
