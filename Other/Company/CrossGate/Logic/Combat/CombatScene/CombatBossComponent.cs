using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatBossComponent : AComponent, IAwake
{
    public Dictionary<string, SkinnedMeshRenderer> _meshDic = new Dictionary<string, SkinnedMeshRenderer>();

    public void Awake()
    {
        foreach (var kv in MobManager.Instance.m_MobDic)
        {
            var mob = kv.Value;
            if (mob == null || mob.m_Trans == null || mob.m_MobCombatComponent == null || 
                mob.m_MobCombatComponent.m_BattleUnit == null || mob.m_MobCombatComponent.m_BattleUnit.UnitType != (int)Packet.UnitType.Monster)
                continue;

            SkinnedMeshRenderer[] smrs = mob.m_Trans.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var smr in smrs)
            {
                if (smr == null)
                    continue;

                _meshDic[smr.gameObject.name] = smr;
            }
        }
    }

    public SkinnedMeshRenderer GetMesh(string meshName)
    {
        _meshDic.TryGetValue(meshName, out SkinnedMeshRenderer mesh);

        return mesh;
    }
}
