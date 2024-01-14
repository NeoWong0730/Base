using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;

public class TriggerCondition_CurMap : TriggerSlot
{
    public List<uint> MapIds = new List<uint>();

    public TriggerCondition_CurMap() { }

    public TriggerCondition_CurMap(List<uint> ids)
    {
        SetArg(ids);
    }

    public void SetArg(List<uint> ids)
    {
        MapIds.Clear();
        for (int i = 0, length = ids.Count; i < length; ++i) {
            MapIds.Add(ids[i]);
        }
    }
    protected override void DoChange()
    {
        uint MapID = Sys_Map.Instance.CurMapId;
        isFinish = MapIds.Contains(MapID) || MapIds.Contains(0);
    }
    public override void PreCheck()
    {
        DoChange();
    }
    protected override void Listen(bool toListen)
    {
        Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnEnterMap, OnSwitchMap, toListen);
    }
    public override string GetFailReason()
    {
        return string.Format("不符合地图条件");
    }
    public void OnSwitchMap()
    {
        OnChange();
    }
}
