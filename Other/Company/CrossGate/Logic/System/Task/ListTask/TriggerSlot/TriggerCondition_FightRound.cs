using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Logic;

public class TriggerCondition_FightRound : TriggerSlot
{
    public List<uint> keys = new List<uint>();

    public Dictionary<uint, bool> roundIds = new Dictionary<uint, bool>();
    public int count { get { return roundIds.Count; } }


    public TriggerCondition_FightRound() { }

    public TriggerCondition_FightRound(List<uint> ids)
    {
        SetArg(ids);
    }
    public void SetArg(List<uint> ids)
    {
        for (int i = 0, length = ids.Count; i < length; ++i) {
            var id = ids[i];
            keys.Add(id);
            roundIds.Add(id, false);
        }
    }

    public override void PreCheck()
    {
        isFinish = false;
        CheckState();
    }
    protected override void Listen(bool toListen)
    {
        Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnRoundNtf, OnCheckState, toListen);
        Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnDoRound, OnCheckState, toListen);
        ProcedureManager.eventEmitter.Handle(ProcedureManager.EEvents.OnAfterEnterFightEffect, OnCheckState, toListen);
        ProcedureManager.eventEmitter.Handle(ProcedureManager.EEvents.OnAfterExitFightEffect, OnCheckState, toListen);
    }

    public override string GetFailReason()
    {
        return string.Format("未知战斗回合数");
    }

    /// <summary>
    /// 检测状态
    /// </summary>
    public void OnCheckState()
    {
        CheckState();
    }

    public void CheckState()
    {
        uint CurRound = Net_Combat.Instance.m_CurRound;
        bool isFightOperations = Net_Combat.Instance.isFightOperations && Sys_Fight.Instance.IsFight();

        for (int i = 0, length = keys.Count; i < length; ++i) {
            var id = keys[i];
            roundIds[id] = id == CurRound && isFightOperations;
        }
        isFinish = !keys.TrueForAll(x => roundIds[x] == false);
        OnChange();
    }
}
