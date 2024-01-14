using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobEnterSceneComponent : BaseComponent<MobEntity>
{
    public override void Dispose()
    {
        GetComponent<MobCombatComponent>()?.StopBehave();

        base.Dispose();
    }

    public void StartEnterScene(bool isCalling)
    {
        uint skillId = 0u;
        int blockType = 0;
        if (isCalling)
            skillId = 201u;
        else if (CombatManager.Instance.m_IsRunEnterScene)
            skillId = 1u;
        else
            return;

        int serverNum = -1;
        NewUnitBehaveInfo newUnitBehaveInfo = null;
        if (m_CurUseEntity.m_MobCombatComponent != null && m_CurUseEntity.m_MobCombatComponent.m_BattleUnit != null)
        {
            serverNum = m_CurUseEntity.m_MobCombatComponent.m_BattleUnit.Pos;
            Net_Combat.Instance.GetNewUnitBehaveInfoByServerNum(serverNum, out newUnitBehaveInfo);
        }

        TurnBehaveSkillInfo turnBehaveSkillInfo = null;
        BehaveAIControllParam behaveAIControllParam;
        if (newUnitBehaveInfo != null)
        {
            turnBehaveSkillInfo = newUnitBehaveInfo.m_TurnBehaveSkillInfo;
            behaveAIControllParam = BehaveAIControllParam.DeepClone(newUnitBehaveInfo.m_BehaveAIControllParam);
            behaveAIControllParam.IsNotUpdateEveryTypeBehaveCount = true;

            Net_Combat.Instance.RemoveNewUnitBehaveInfoByServerNum(serverNum, newUnitBehaveInfo);

            if (behaveAIControllParam != null && behaveAIControllParam.ExcuteTurnIndex > -1)
            {
                NetExcuteTurnInfo netExcuteTurnInfo;
                if (Net_Combat.Instance.m_NetExcuteTurnInfoDic.TryGetValue(behaveAIControllParam.ExcuteTurnIndex, out netExcuteTurnInfo) &&
                    netExcuteTurnInfo != null)
                {
                    if (m_CurUseEntity.m_MobCombatComponent != null && m_CurUseEntity.m_MobCombatComponent.m_BattleUnit != null)
                    {
                        DoCacheNetInfo(netExcuteTurnInfo, behaveAIControllParam.ExcuteTurnIndex, false);

                        DoCacheNetBuff(netExcuteTurnInfo);
                    }
                }

                uint turnStage = Net_Combat.Instance.GetExcuteTurnStage(behaveAIControllParam.ExcuteTurnIndex);
                blockType = turnStage != 4u ? 6001 : 6002;
            }
        }
        else
        {
            behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();

            if (!isCalling)
            {
                foreach (var kv in Net_Combat.Instance.m_NetExcuteTurnInfoDic)
                {
                    int excuteTurnIndex = kv.Key;
                    NetExcuteTurnInfo netExcuteTurnInfo = kv.Value;

                    DoCacheNetInfo(netExcuteTurnInfo, excuteTurnIndex, false);

                    DoCacheNetBuff(netExcuteTurnInfo);
                }
            }
        }

        behaveAIControllParam.IsIdleInControlOver = true;

        GetComponent<MobCombatComponent>()?.StartBehave(skillId, turnBehaveSkillInfo, false, behaveAIControllParam, 0, blockType);
    }

    private void DoCacheNetInfo(NetExcuteTurnInfo netExcuteTurnInfo, int excuteTurnIndex, bool isPerformBehave)
    {
        if (netExcuteTurnInfo == null)
            return;

        if (netExcuteTurnInfo.m_CacheNetInfoForNoMobDataDic == null)
            return;

        Queue<CacheNetInfoForNoMobData> queue;
        if (netExcuteTurnInfo.m_CacheNetInfoForNoMobDataDic.TryGetValue(m_CurUseEntity.m_MobCombatComponent.m_BattleUnit.UnitId, out queue) &&
            queue != null)
        {
            if (queue.Count > 0)
            {
                while (queue.Count > 0)
                {
                    CacheNetInfoForNoMobData cacheNetInfoForNoMobData = queue.Dequeue();
                    if (cacheNetInfoForNoMobData == null)
                        continue;

                    DLogManager.Log(ELogType.eCombat, $"m_CacheNetInfoForNoMobDataDic--从队列取出-----<color=yellow>unitId:{cacheNetInfoForNoMobData.m_UnitId.ToString()}  excuteTurnIndex:{cacheNetInfoForNoMobData.m_ExcuteTurnIndex.ToString()}   edIndex:{cacheNetInfoForNoMobData.m_EdIndex.ToString()}    srcUnitId:{cacheNetInfoForNoMobData.m_SrcUnitId.ToString()}    extendType:{cacheNetInfoForNoMobData.m_ExtendType.ToString()}   extendId:{cacheNetInfoForNoMobData.m_ExtendId.ToString()}</color>");

                    bool isHaveBehave = false;
                    bool isAttackSkillSuccess = false;
                    Net_Combat.Instance.HandleExcuteData(cacheNetInfoForNoMobData.m_ExcuteTurn, cacheNetInfoForNoMobData.m_ExcuteTurnIndex,
                        cacheNetInfoForNoMobData.m_Ed, cacheNetInfoForNoMobData.m_EdIndex, netExcuteTurnInfo, cacheNetInfoForNoMobData.m_TempAttacBokLifeCycle,
                        cacheNetInfoForNoMobData.m_TempTTi, cacheNetInfoForNoMobData.m_SrcUnitId, cacheNetInfoForNoMobData.m_ExtendType, cacheNetInfoForNoMobData.m_ExtendId,
                        true, cacheNetInfoForNoMobData.m_SkillTb, 0, ref isHaveBehave, ref isAttackSkillSuccess);

                    cacheNetInfoForNoMobData.Push();
                }

                if (isPerformBehave)
                {
                    bool isHaveBehave = true;
                    Net_Combat.Instance.PerformBehave(netExcuteTurnInfo.TurnBehaveInfoList, excuteTurnIndex, true, true, 0, ref isHaveBehave);
                }
            }
        }
    }

    private void DoCacheNetBuff(NetExcuteTurnInfo netExcuteTurnInfo)
    {
        if (netExcuteTurnInfo == null)
            return;

        if (netExcuteTurnInfo.m_CacheNetBuffForNoMobDataDic == null)
            return;

        Queue<CacheNetBuffForNoMobData> que;
        if (netExcuteTurnInfo.m_CacheNetBuffForNoMobDataDic.TryGetValue(m_CurUseEntity.m_MobCombatComponent.m_BattleUnit.UnitId, out que) &&
            que != null)
        {
            while (que.Count > 0)
            {
                CacheNetBuffForNoMobData cacheNetBuffForNoMobData = que.Dequeue();
                if (cacheNetBuffForNoMobData == null)
                    continue;

                DLogManager.Log(ELogType.eCombat, $"m_CacheNetBuffForNoMobDataDic--从队列取出-----<color=yellow>unitId:{cacheNetBuffForNoMobData.m_BattleBuffChange.UnitId.ToString()}  buffId:{cacheNetBuffForNoMobData.m_BattleBuffChange.BuffId.ToString()}</color>");

                m_CurUseEntity.UpdateBuffComponent(cacheNetBuffForNoMobData.m_BattleBuffChange, cacheNetBuffForNoMobData.m_IsAfterActive);
            }
        }
    }
}
