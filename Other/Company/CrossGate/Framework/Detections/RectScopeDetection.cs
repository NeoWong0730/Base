using UnityEngine;
using System;
using System.Collections.Generic;
public class RectScopeDetection : ScopeDetection
{
    public List<Rect> rectList;
    
    public void Init(List<Rect> rectList)
    {
        this.rectList = rectList;
    }

    public override bool Contains(Transform trans)
    {
        if (null == this.rectList)
            return false;

        Vector2 pos = new Vector2(trans.position.x, trans.position.z); 
        for (int i = 0; i < rectList.Count; ++i)
        {
            if (rectList[i].Contains(pos))
                return true;
        }

        return false;
    }
}

