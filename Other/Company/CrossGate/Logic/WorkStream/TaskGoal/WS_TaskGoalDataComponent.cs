using System.Collections.Generic;
using UnityEngine;

public class WS_TaskGoalDataComponent : BaseComponent<StateMachineEntity>, IAwake
{
    public List<WS_BA_EffectData> m_EffectDataList;
    public int m_LoopNodeMarkCount;
    public ushort m_LoopNodeMarkNodeId;
    public Vector3 cacheStartPos;

    public void Awake()
    {
        if (m_EffectDataList == null)
            m_EffectDataList = new List<WS_BA_EffectData>();
    }

    public override void Dispose()
    {
        for (int i = 0; i < m_EffectDataList.Count; i++)
        {
            var ed = m_EffectDataList[i];
            FxManager.Instance.FreeFx(ed.m_EffectModelId);
            CombatObjectPool.Instance.Push(ed);
        }
        m_EffectDataList.Clear();

        base.Dispose();
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
