using System.Collections.Generic;
using UnityEngine;

public class WS_UIModelShowDataComponent : BaseComponent<StateMachineEntity>
{
    public class MarkData
    {
        public WorkNodeData m_WorkNodeData;
        public string MarkFlag;

        public void Push()
        {
            m_WorkNodeData = null;
            MarkFlag = null;

            CombatObjectPool.Instance.Push(this);
        }
    }
    
    private List<MarkData> _markList = new List<MarkData>();

    public List<WS_BA_EffectData> m_EffectDataList = new List<WS_BA_EffectData>();

    public Transform m_BoneTrans;
    
    public override void Dispose()
    {
        int markCount = _markList.Count;
        if (markCount > 0)
        {
            for (int i = 0; i < markCount; i++)
            {
                _markList[i].Push();
            }
            _markList.Clear();
        }

        for (int i = 0, count = m_EffectDataList.Count; i < count; i++)
        {
            var ed = m_EffectDataList[i];
            FxManager.Instance.FreeFx(ed.m_EffectModelId);
            CombatObjectPool.Instance.Push(ed);
        }
        m_EffectDataList.Clear();

        m_BoneTrans = null;

        base.Dispose();
    }

    public void Mark(WorkNodeData workNodeData, string markFlag)
    {
        if (workNodeData == null)
            return;

        for (int i = 0, markCount = _markList.Count; i < markCount; i++)
        {
            MarkData markData = _markList[i];
            if (markData.m_WorkNodeData == workNodeData)
                return;
        }

        MarkData md = CombatObjectPool.Instance.Get<MarkData>();
        md.m_WorkNodeData = workNodeData;
        md.MarkFlag = markFlag;

        _markList.Add(md);
    }

    public WorkNodeData DequeueMarkNodeId(string markFlag)
    {
        for (int i = 0, markCount = _markList.Count; i < markCount; i++)
        {
            MarkData markData = _markList[i];
            if (markData.MarkFlag == markFlag)
            {
                _markList.RemoveAt(i);
                var workNodeData = markData.m_WorkNodeData;
                markData.Push();

                return workNodeData;
            }
        }

        return null;
    }

    public void AddEffect(ulong effectModelId, uint effectTbId)
    {
        if (effectModelId == 0ul || effectTbId == 0u)
            return;

        foreach (var ed in m_EffectDataList)
        {
            if (ed.m_EffectModelId == effectModelId)
                return;
        }

        WS_BA_EffectData bN_EffectData = CombatObjectPool.Instance.Get<WS_BA_EffectData>();
        bN_EffectData.m_EffectModelId = effectModelId;
        bN_EffectData.m_EffectTbId = effectTbId;

        m_EffectDataList.Add(bN_EffectData);
    }
}