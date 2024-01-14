using System.Collections.Generic;
using Logic.Core;
using Logic;
using Lib.Core;
using Table;
using Packet;
using UnityEngine;
using System;

public class TriggerCondition_UnlockMaps : TriggerSlot
{
    public Dictionary<uint, bool> mapIds = new Dictionary<uint, bool>();
    public int finishedCount;
    public int count { get { return mapIds.Count; } }

    public TriggerCondition_UnlockMaps() { }
    public TriggerCondition_UnlockMaps(List<uint> ids)
    {
        SetArg(ids);
    }
    public void SetArg(List<uint> ids)
    {
        for (int i = 0, length = ids.Count; i < length; ++i) {
            mapIds.Add(ids[i], false);
        }
    }
    protected override void DoChange()
    {
        // 调试代码，强制true，后续删除
        isFinish = true;
    }
    protected override void Listen(bool toListen)
    {

    }
    public override string GetFailReason()
    {
        return string.Format("有地图未解锁");
    }
    public override void PreCheck()
    {
        // 调试代码，强制true，后续删除
        isFinish = true;
    }

    private void OnMapUnlocked(uint mapId)
    {
        if (mapIds.ContainsKey(mapId))
        {
            mapIds[mapId] = true;
            OnChange();
        }
    }
}