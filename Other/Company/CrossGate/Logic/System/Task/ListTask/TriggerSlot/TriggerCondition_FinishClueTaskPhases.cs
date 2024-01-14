using System.Collections.Generic;
using Logic;

public class TriggerCondition_FinishClueTaskPhases : TriggerSlot
{
    public Dictionary<uint, bool> phases = new Dictionary<uint, bool>();
    public int finishedCount;
    public int count { get { return phases.Count; } }
    public uint taskId;

    public TriggerCondition_FinishClueTaskPhases() { }
    public TriggerCondition_FinishClueTaskPhases(List<uint> ids) 
    {
        SetArg(ids);
    }
    public void SetArg(List<uint> ids)
    {
        for (int i = 0, length = ids.Count; i < length; ++i) {
            phases.Add(ids[i], false);
        }
    }
    protected override void DoChange()
    {
        Check();
    }
    protected override void Listen(bool toListen)
    {
        Sys_ClueTask.Instance.eventEmitter.Handle<ClueTaskPhase>(Sys_ClueTask.EEvents.OnClueTaskPhaseFinished, OnClueTaskPhaseFinished, toListen);

    }
    public override string GetFailReason()
    {
        return string.Format("未完成");
    }

    private Dictionary<uint, bool> dic = new Dictionary<uint, bool>();
    public override void PreCheck()
    {
        isFinish = true;
        finishedCount = 0;
        dic.Clear();
        foreach (var kvp in phases)
        {
            if (Sys_ClueTask.Instance.tasks.TryGetValue(taskId, out ClueTask clueTask) && clueTask.phases.dictionary.TryGetValue(kvp.Key, out ClueTaskPhase clueTaskPhase))
            {
                bool t = clueTaskPhase.isFinish;
                dic.Add(kvp.Key, t);
                isFinish &= t;
                if (isFinish)
                    ++finishedCount;
            }
            // 不存在的任务
            //else
            //{
            //    isFinish = false;
            //}
        }
        foreach (var kvp in dic)
        {
            phases[kvp.Key] = kvp.Value;
        }
    }
    private void Check()
    {
        isFinish = true;
        finishedCount = 0;
        foreach (var kvp in phases)
        {
            isFinish &= kvp.Value;
            if (!isFinish) { break; }
            ++finishedCount;
        }
    }
    private void OnClueTaskPhaseFinished(ClueTaskPhase phase)
    {
        if (phases.ContainsKey(phase.id))
        {
            phases[phase.id] = true;
            OnChange();
        }
    }
}