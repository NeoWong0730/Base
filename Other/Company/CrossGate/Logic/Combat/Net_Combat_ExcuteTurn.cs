using Google.Protobuf.Collections;
using Lib.Core;
using Logic;
using Packet;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

#region ExcuteTurn辅助类
public class NetExcuteTurnInfo
{
    #region 合击使用参数
    public int CombineAttack_ScrIndex;
    public int CombineAttack_WaitCount;
    public List<uint> CombineAttack_SrcUnits;
    #endregion
    
    //对话
    public RepeatedField<TalkInfo> TalkInfos;
    
    //攻击的波生命周期
    public List<AttackBoLifeCycleInfo> AttackBoLifeCycleInfoList;

    //行为数据
    public List<TurnBehaveInfo> TurnBehaveInfoList = new List<TurnBehaveInfo>();

    public Dictionary<uint, Queue<CacheNetInfoForNoMobData>> m_CacheNetInfoForNoMobDataDic;

    public Dictionary<uint, Queue<CacheNetBuffForNoMobData>> m_CacheNetBuffForNoMobDataDic;

    public void Clear()
    {
        CombineAttack_ScrIndex = 0;
        CombineAttack_WaitCount = 0;
        if(CombineAttack_SrcUnits != null)
            CombineAttack_SrcUnits.Clear();

        if (TalkInfos != null)
            TalkInfos = null;

        if (AttackBoLifeCycleInfoList != null)
        {
            int count = AttackBoLifeCycleInfoList.Count;
            if(count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    AttackBoLifeCycleInfoList[i].Push();
                }
                AttackBoLifeCycleInfoList.Clear();
            }
        }
        
        if (TurnBehaveInfoList != null)
        {
            int ttCount = TurnBehaveInfoList.Count;
            if (ttCount > 0)
            {
                for (int ttIndex = 0; ttIndex < ttCount; ttIndex++)
                {
                    TurnBehaveInfoList[ttIndex].Push();
                }
                TurnBehaveInfoList.Clear();
            }
        }

        if (m_CacheNetInfoForNoMobDataDic != null && m_CacheNetInfoForNoMobDataDic.Count > 0)
        {
            foreach (var kv in m_CacheNetInfoForNoMobDataDic)
            {
                Queue<CacheNetInfoForNoMobData> que = kv.Value;
                if (que == null || que.Count < 1)
                    continue;

                while (que.Count > 0)
                {
                    CacheNetInfoForNoMobData cacheNetInfoForNoMobData = que.Dequeue();
                    DLogManager.LogDebugError($"m_CacheNetInfoForNoMobDataDic还有缓存数据没被执行：Unit:{kv.Key.ToString()}   excuteTurnIndex:{cacheNetInfoForNoMobData.m_ExcuteTurnIndex.ToString()}   edIndex:{cacheNetInfoForNoMobData.m_EdIndex.ToString()}   srcUnitId:{cacheNetInfoForNoMobData.m_SrcUnitId.ToString()}   extendType:{cacheNetInfoForNoMobData.m_ExtendType.ToString()}   extendId:{cacheNetInfoForNoMobData.m_ExtendId.ToString()}");
                    cacheNetInfoForNoMobData.Push();
                }
            }
            m_CacheNetInfoForNoMobDataDic.Clear();
        }

        if (m_CacheNetBuffForNoMobDataDic != null && m_CacheNetBuffForNoMobDataDic.Count > 0)
        {
            foreach (var kv in m_CacheNetBuffForNoMobDataDic)
            {
                Queue<CacheNetBuffForNoMobData> que = kv.Value;
                if (que == null || que.Count < 1)
                    continue;

                while (que.Count > 0)
                {
                    CacheNetBuffForNoMobData cacheNetBuffForNoMobData = que.Dequeue();
                    DLogManager.LogDebugError($"m_CacheNetBuffForNoMobDataDic还有缓存数据没被执行：Unit:{kv.Key.ToString()}   buffId:{cacheNetBuffForNoMobData.m_BattleBuffChange.BuffId.ToString()}");
                    cacheNetBuffForNoMobData.Push();
                }
            }
            m_CacheNetBuffForNoMobDataDic.Clear();
        }
    }

    public void CacheToNetInfoForNoMobData(uint unitId, ExcuteTurn excuteTurn, int excuteTurnIndex, ExcuteData ed, int edIndex,
        AttackBoLifeCycleInfo tempAttacBokLifeCycle, SNodeInfo tempTTi,
        uint srcUnitId, uint extendType, uint extendId, CSVActiveSkill.Data skillTb)
    {
        if (m_CacheNetInfoForNoMobDataDic == null)
            m_CacheNetInfoForNoMobDataDic = new Dictionary<uint, Queue<CacheNetInfoForNoMobData>>();

        Queue<CacheNetInfoForNoMobData> cnifnmdQueue = null;
        if (!m_CacheNetInfoForNoMobDataDic.TryGetValue(unitId, out cnifnmdQueue) || cnifnmdQueue == null)
        {
            cnifnmdQueue = new Queue<CacheNetInfoForNoMobData>();
            m_CacheNetInfoForNoMobDataDic[unitId] = cnifnmdQueue;
        }

        CacheNetInfoForNoMobData cnifnmd = BasePoolClass.Get<CacheNetInfoForNoMobData>();
        cnifnmd.Init(unitId, excuteTurn, excuteTurnIndex, ed, edIndex,
            tempAttacBokLifeCycle, tempTTi, srcUnitId, extendType, extendId, skillTb);

        cnifnmdQueue.Enqueue(cnifnmd);

        DLogManager.Log(ELogType.eCombat, $"m_CacheNetInfoForNoMobDataDic--放入队列-----<color=yellow>unitId:{unitId.ToString()}  excuteTurnIndex:{excuteTurnIndex.ToString()}   edIndex:{edIndex.ToString()}    srcUnitId:{srcUnitId.ToString()}    extendType:{extendType.ToString()}   extendId:{extendId.ToString()}</color>");
    }

    public void CacheToNetBuff(BattleBuffChange battleBuffChange, bool isAfterActive)
    {
        if (m_CacheNetBuffForNoMobDataDic == null)
            m_CacheNetBuffForNoMobDataDic = new Dictionary<uint, Queue<CacheNetBuffForNoMobData>>();

        Queue<CacheNetBuffForNoMobData> que = null;
        if (!m_CacheNetBuffForNoMobDataDic.TryGetValue(battleBuffChange.UnitId, out que) || que == null)
        {
            que = new Queue<CacheNetBuffForNoMobData>();
            m_CacheNetBuffForNoMobDataDic[battleBuffChange.UnitId] = que;
        }

        CacheNetBuffForNoMobData cacheNetBuffForNoMobData = BasePoolClass.Get<CacheNetBuffForNoMobData>();
        cacheNetBuffForNoMobData.m_BattleBuffChange = battleBuffChange;
        cacheNetBuffForNoMobData.m_IsAfterActive = isAfterActive;

        que.Enqueue(cacheNetBuffForNoMobData);

        DLogManager.Log(ELogType.eCombat, $"m_CacheNetBuffForNoMobDataDic--放入队列-----<color=yellow>unitId:{battleBuffChange.UnitId.ToString()}  buffId:{battleBuffChange.BuffId.ToString()}</color>");
    }
}

public class AttackBoLifeCycleInfo : BasePoolClass
{
    public OutBoInfo m_OutBoInfo;
    public List<SNodeInfo> SNodeInfoList = new List<SNodeInfo>();

    public bool IsOutBoTempFlag;
    
    public override void Clear()
    {
        if (m_OutBoInfo != null)
        {
            m_OutBoInfo.Push();
            m_OutBoInfo = null;
        }

        if (SNodeInfoList != null)
        {
            int count = SNodeInfoList.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    SNodeInfoList[i].Push();
                }
                SNodeInfoList.Clear();
            }
        }

        IsOutBoTempFlag = false;
    }
}

public class OutBoInfo : BasePoolClass
{
    //波数
    public uint Stage;
    public int Bo;
    public int LeftStartIndex;
    public int LeftEndIndex;
    public int RightStartIndex;
    public int RightEndIndex;

    public int CurDoExcuteDataIndex;
    
    //行为数据
    public List<TurnBehaveInfo> TurnBehaveInfoList = new List<TurnBehaveInfo>();

    public Queue<uint> m_BuffChangeDataBeforeQueue;
    public Queue<uint> m_BuffChangeDataAfterQueue;

    public void SetSNodeInfoListBo()
    {
        if (TurnBehaveInfoList != null)
        {
            int tbiCount = TurnBehaveInfoList.Count;
            if (tbiCount > 0)
            {
                for (int tbiIndex = 0; tbiIndex < tbiCount; tbiIndex++)
                {
                    TurnBehaveInfo turnBehaveInfo = TurnBehaveInfoList[tbiIndex];
                    if (turnBehaveInfo == null)
                        continue;

                    if (turnBehaveInfo.TurnBehaveSkillInfoList != null)
                    {
                        int tbsiCount = turnBehaveInfo.TurnBehaveSkillInfoList.Count;
                        if (tbsiCount > 0)
                        {
                            for (int tbsiIndex = 0; tbsiIndex < tbsiCount; tbsiIndex++)
                            {
                                TurnBehaveSkillInfo turnBehaveSkillInfo = turnBehaveInfo.TurnBehaveSkillInfoList[tbsiIndex];
                                if (turnBehaveSkillInfo == null || turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList == null)
                                    continue;

                                int tbstiCount = turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count;
                                if (tbstiCount > 0)
                                {
                                    for (int tbstiIndex = 0; tbstiIndex < tbstiCount; tbstiIndex++)
                                    {
                                        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[tbstiIndex];
                                        if (turnBehaveSkillTargetInfo == null)
                                            continue;

                                        turnBehaveSkillTargetInfo.Bo = Bo;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public override void Clear()
    {
        Stage = 0u;
        Bo = -1;
        LeftStartIndex = -1;
        LeftEndIndex = -1;
        RightStartIndex = -1;
        RightEndIndex = -1;

        CurDoExcuteDataIndex = -1;
        
        if (TurnBehaveInfoList != null)
        {
            int ttCount = TurnBehaveInfoList.Count;
            if (ttCount > 0)
            {
                for (int ttIndex = 0; ttIndex < ttCount; ttIndex++)
                {
                    TurnBehaveInfoList[ttIndex].Push();
                }
                TurnBehaveInfoList.Clear();
            }
        }

        if (m_BuffChangeDataBeforeQueue != null)
        {
            m_BuffChangeDataBeforeQueue.Clear();
        }

        if (m_BuffChangeDataAfterQueue != null)
            m_BuffChangeDataAfterQueue.Clear();
    }
}

public class SNodeInfo : BasePoolClass
{
    public SNodeInfo ParentSNodeInfo;

    //波数
    public int Bo;

    //一轮的时机Id
    public uint SNodeId;
    //层级
    public int Layer;
    public int LeftStartIndex;
    public int LeftEndIndex;
    public int RightStartIndex;
    public int RightEndIndex;

    public int CurDoExcuteDataIndex;

    public List<SNodeInfo> ChildSNodeInfoList;
    //当前Node包含的下一层级产生的行为数据List
    public List<TurnBehaveInfo> TurnBehaveInfoList;

    #region 主动数据
    public List<ActiveTurnInfo> ActiveTurnInfoList;
    #endregion

    public bool IsAfterActive;
    public Dictionary<int, Queue<uint>> m_BuffChangeDataByBuffTimingDic;

    public override void Clear()
    {
        ParentSNodeInfo = null;

        Bo = -1;

        SNodeId = 0u;
        Layer = 0;
        LeftStartIndex = -1;
        LeftEndIndex = -1;
        RightStartIndex = -1;
        RightEndIndex = -1;

        CurDoExcuteDataIndex = -1;

        if (ChildSNodeInfoList != null)
        {
            int count = ChildSNodeInfoList.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    ChildSNodeInfoList[i].Push();
                }
                ChildSNodeInfoList.Clear();
            }
        }

        if (TurnBehaveInfoList != null)
        {
            int ttCount = TurnBehaveInfoList.Count;
            if (ttCount > 0)
            {
                for (int ttIndex = 0; ttIndex < ttCount; ttIndex++)
                {
                    TurnBehaveInfoList[ttIndex].Push();
                }
                TurnBehaveInfoList.Clear();
            }
        }

        if (ActiveTurnInfoList != null)
        {
            int count = ActiveTurnInfoList.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    ActiveTurnInfoList[i].Push();
                }
                ActiveTurnInfoList.Clear();
            }
        }
        
        IsAfterActive = false;

        if (m_BuffChangeDataByBuffTimingDic != null)
        {
            foreach (var kv in m_BuffChangeDataByBuffTimingDic)
            {
                Queue<uint> bcq = kv.Value;
                bcq.Clear();
                CombatObjectPool.Instance.Push(bcq);
            }
            m_BuffChangeDataByBuffTimingDic.Clear();
        }
    }

    /// <summary>
    /// buffTiming=-1是IsBeforeActive，=0是IsAfterActive
    /// </summary>
    public void SetBuffChangeDataByBuffTiming(uint unitId, int buffTiming)
    {
        if (m_BuffChangeDataByBuffTimingDic == null)
            m_BuffChangeDataByBuffTimingDic = new Dictionary<int, Queue<uint>>();

        if (!m_BuffChangeDataByBuffTimingDic.TryGetValue(buffTiming, out Queue<uint> bcq) || bcq == null)
        {
            bcq = CombatObjectPool.Instance.Get<Queue<uint>>();
            m_BuffChangeDataByBuffTimingDic[buffTiming] = bcq;
        }

        bcq.Enqueue(unitId);
    }
}

public class ActiveTurnInfo : BasePoolClass
{
    public uint SrcUnitId;
    public uint ActiveLogicId;
    public int ExcuteDataIndex;

    public override void Clear()
    {
        SrcUnitId = 0u;
        ActiveLogicId = 0u;
        ExcuteDataIndex = -1;
    }
}

public class TurnBehaveInfo : BasePoolClass
{
    public uint SrcUnitId;

    public List<TurnBehaveSkillInfo> TurnBehaveSkillInfoList;

    public override void Clear()
    {
        SrcUnitId = 0u;

        if (TurnBehaveSkillInfoList != null)
        {
            int count = TurnBehaveSkillInfoList.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    TurnBehaveSkillInfoList[i].Push();
                }
                TurnBehaveSkillInfoList.Clear();
            }
        }
    }

    public TurnBehaveSkillInfo GetTurnBehaveSkillByEdIndex(int excuteDataIndex, out TurnBehaveSkillTargetInfo targetInfo)
    {
        targetInfo = null;

        if (excuteDataIndex < 0 || TurnBehaveSkillInfoList == null)
            return null;

        int tbsiCount = TurnBehaveSkillInfoList.Count;
        if (tbsiCount < 1)
            return null;

        for (int tbsiIndex = 0; tbsiIndex < tbsiCount; tbsiIndex++)
        {
            TurnBehaveSkillInfo tbsi = TurnBehaveSkillInfoList[tbsiIndex];
            if (tbsi == null || tbsi.TurnBehaveSkillTargetInfoList == null)
                continue;

            int tbsTargetCount = tbsi.TurnBehaveSkillTargetInfoList.Count;
            for (int tbsTargetIndex = 0; tbsTargetIndex < tbsTargetCount; tbsTargetIndex++)
            {
                TurnBehaveSkillTargetInfo tbsTarget = tbsi.TurnBehaveSkillTargetInfoList[tbsTargetIndex];
                if (tbsTarget == null)
                    continue;

                if (tbsTarget.ExcuteDataIndex == excuteDataIndex)
                {
                    targetInfo = tbsTarget;
                    return tbsi;
                }
            }
        }

        return null;
    }
}

public class TurnBehaveSkillInfo : BasePoolClass
{
    public uint SkillId;
    public uint ExtendType;
    public uint HitEffect;
    public bool IsConverAttack;
    public int TargetUnitCount;
    public int CallingTargetBattleUnitCount;
    public List<TurnBehaveSkillTargetInfo> TurnBehaveSkillTargetInfoList;
    
    public override void Clear()
    {
        SkillId = 0u;
        ExtendType = 0u;
        HitEffect = 0u;
        IsConverAttack = false;

        TargetUnitCount = 0;
        CallingTargetBattleUnitCount = 0;
        if (TurnBehaveSkillTargetInfoList != null)
        {
            int count = TurnBehaveSkillTargetInfoList.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    TurnBehaveSkillTargetInfoList[i].Push();
                }
                TurnBehaveSkillTargetInfoList.Clear();
            }
        }
    }
    
    public TurnBehaveSkillTargetInfo GetTurnBehaveSkillTargetByTargetUnitId(uint targetUnitId, out int turnBehaveSkillTargetInfoIndex)
    {
        turnBehaveSkillTargetInfoIndex = -1;
        if (targetUnitId == 0u)
            return null;

        if (TurnBehaveSkillTargetInfoList.Count == 0)
        {
#if DEBUG_MODE
            if (TargetUnitCount > 0 || CallingTargetBattleUnitCount > 0)
                DebugUtil.LogError($"GetTargetUnitId    技能{SkillId.ToString()}时TurnBehaveSkillInfo记录的TargetUnitCount:{TargetUnitCount.ToString()}   CallingTargetBattleUnitCount:{CallingTargetBattleUnitCount.ToString()}和实际TargetUnitCount:{TurnBehaveSkillTargetInfoList.Count.ToString()}不一致");
#endif

            return null;
        }
        
        if (TurnBehaveSkillTargetInfoList.Count == 1)
        {
            TurnBehaveSkillTargetInfo tbsti = TurnBehaveSkillTargetInfoList[0];
            if (tbsti.TargetUnitId == targetUnitId ||
                (tbsti.CallingTargetBattleUnit != null && tbsti.CallingTargetBattleUnit.UnitId == targetUnitId))
            {
                turnBehaveSkillTargetInfoIndex = 0;
                return tbsti;
            }

#if DEBUG_MODE
            if (tbsti.TargetUnitId == 0u && tbsti.CallingTargetBattleUnit == null)
                DebugUtil.LogError($"GetTargetUnitId    技能{SkillId.ToString()}时获取TargetUnitId为0u 且 CallingTargetBattleUnit为null");

            if (TargetUnitCount > 1 || CallingTargetBattleUnitCount > 1)
                DebugUtil.LogError($"GetTargetUnitId    技能{SkillId.ToString()}时TurnBehaveSkillInfo记录的TargetUnitCount:{TargetUnitCount.ToString()}   CallingTargetBattleUnitCount:{CallingTargetBattleUnitCount.ToString()}和实际TargetUnitCount:{TurnBehaveSkillTargetInfoList.Count.ToString()}不一致");
            
            DLogManager.Log(ELogType.eCombatBehave, $"未获取到SkillId:{SkillId.ToString()}   TurnBehaveSkillTargetInfoList的TargetUnitId的{targetUnitId.ToString()}");
#endif
        }
        else
        {
            for (int tbstiIndex = 0, tbstiCount = TurnBehaveSkillTargetInfoList.Count; tbstiIndex < tbstiCount; tbstiIndex++)
            {
                TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = TurnBehaveSkillTargetInfoList[tbstiIndex];
                if (turnBehaveSkillTargetInfo.TargetUnitId == targetUnitId ||
                    (turnBehaveSkillTargetInfo.CallingTargetBattleUnit != null && turnBehaveSkillTargetInfo.CallingTargetBattleUnit.UnitId == targetUnitId))
                {
                    turnBehaveSkillTargetInfoIndex = tbstiIndex;
                    return turnBehaveSkillTargetInfo;
                }
            }
        }

        return null;
    }

    public TurnBehaveSkillTargetInfo GetTurnBehaveSkillTargetByIndex(int targetUnitIdIndex, out int turnBehaveSkillTargetInfoIndex)
    {
        turnBehaveSkillTargetInfoIndex = -1;

        if (targetUnitIdIndex < 0)
            return null;

        if (TurnBehaveSkillTargetInfoList.Count == 0)
        {
#if DEBUG_MODE
            if (TargetUnitCount > 0)
                DebugUtil.LogError($"GetTargetUnitId    技能{SkillId.ToString()}时TurnBehaveSkillInfo记录的TargetUnitCount:{TargetUnitCount.ToString()}和实际TargetUnitCount:{TurnBehaveSkillTargetInfoList.Count.ToString()}不一致");
#endif

            return null;
        }

        TurnBehaveSkillTargetInfo targetSkillInfo = null;

        if (targetUnitIdIndex >= TargetUnitCount)
            targetUnitIdIndex = TargetUnitCount - 1;

        if (TurnBehaveSkillTargetInfoList.Count == 1)
        {
            targetSkillInfo = TurnBehaveSkillTargetInfoList[0];

            turnBehaveSkillTargetInfoIndex = 0;

#if DEBUG_MODE
            if (targetSkillInfo.TargetUnitId == 0u && targetSkillInfo.CallingTargetBattleUnit == null)
                DebugUtil.LogError($"GetTargetUnitId    技能{SkillId.ToString()}时获取TargetUnitId为0u 且 CallingTargetBattleUnit为null");

            if (TargetUnitCount > 1)
                DebugUtil.LogError($"GetTargetUnitId    技能{SkillId.ToString()}时TurnBehaveSkillInfo记录的TargetUnitCount:{TargetUnitCount.ToString()}和实际TargetUnitCount:{TurnBehaveSkillTargetInfoList.Count.ToString()}不一致");

            if (targetUnitIdIndex > 0)
                DebugUtil.LogError($"SkillId:{SkillId.ToString()}   TurnBehaveSkillTargetInfoList获取TargetUnitId时下标超了，targetUnitIdIndex:{targetUnitIdIndex.ToString()}");
#endif
        }
        else
        {
            int tbstiTargetUnitIdIndex = 0;
            for (int tbstiIndex = 0, tbstiCount = TurnBehaveSkillTargetInfoList.Count; tbstiIndex < tbstiCount; tbstiIndex++)
            {
                TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = TurnBehaveSkillTargetInfoList[tbstiIndex];
                if (turnBehaveSkillTargetInfo.TargetUnitId > 0u)
                {
                    if (tbstiTargetUnitIdIndex == targetUnitIdIndex)
                    {
                        targetSkillInfo = turnBehaveSkillTargetInfo;

                        turnBehaveSkillTargetInfoIndex = tbstiIndex;
                        break;
                    }

                    ++tbstiTargetUnitIdIndex;
                }
            }
        }

        return targetSkillInfo;
    }
}

public class TurnBehaveSkillTargetInfo : BasePoolClass
{
    public uint Id;

    public int Bo;

    public uint TargetUnitId;
    public BattleUnit CallingTargetBattleUnit;
    public uint SNodeId;
    public int SNodeLayer;
    public uint ExtendId;
    public int ExcuteDataIndex;
    public uint HitEffect;
    public uint ExtraHitEffect;
    public uint BeProtectUnitId;
    public bool IsProtectOver;

    public override void Clear()
    {
        Id = 0u;
        Bo = -1;
        TargetUnitId = 0u;
        CallingTargetBattleUnit = null;
        SNodeId = 0u;
        SNodeLayer = 0;
        ExtendId = 0u;
        ExcuteDataIndex = -1;
        HitEffect = 0u;
        ExtraHitEffect = 0u;
        BeProtectUnitId = 0u;
        IsProtectOver = false;
    }
}

public class NewUnitBehaveInfo : BasePoolClass
{
    public TurnBehaveSkillInfo m_TurnBehaveSkillInfo;
    public BehaveAIControllParam m_BehaveAIControllParam;
    public uint m_newUnitId;

    public override void Clear()
    {
        m_TurnBehaveSkillInfo = null;
        if (m_BehaveAIControllParam != null)
        {
            m_BehaveAIControllParam.Push();
            m_BehaveAIControllParam = null;
        }
        m_newUnitId = 0u;
    }
}

public class CacheNetInfoForNoMobData : BasePoolClass
{
    public uint m_UnitId;
    public ExcuteTurn m_ExcuteTurn;
    public int m_ExcuteTurnIndex;
    public ExcuteData m_Ed;
    public int m_EdIndex;
    public AttackBoLifeCycleInfo m_TempAttacBokLifeCycle;
    public SNodeInfo m_TempTTi;
    public uint m_SrcUnitId;
    public uint m_ExtendType;
    public uint m_ExtendId;
    public CSVActiveSkill.Data m_SkillTb;

    public void Init(uint unitId, ExcuteTurn excuteTurn, int excuteTurnIndex, ExcuteData ed, int edIndex, 
        AttackBoLifeCycleInfo tempAttacBokLifeCycle, SNodeInfo tempTTi, 
        uint srcUnitId, uint extendType, uint extendId, CSVActiveSkill.Data skillTb)
    {
        m_UnitId = unitId;
        m_ExcuteTurn = excuteTurn;
        m_ExcuteTurnIndex = excuteTurnIndex;
        m_Ed = ed;
        m_EdIndex = edIndex;
        m_TempAttacBokLifeCycle = tempAttacBokLifeCycle;
        m_TempTTi = tempTTi;
        m_SrcUnitId = srcUnitId;
        m_ExtendType = extendType;
        m_ExtendId = extendId;
        m_SkillTb = skillTb;
    }

    public override void Clear()
    {
        m_UnitId = 0u;
        m_ExcuteTurn = null;
        m_ExcuteTurnIndex = -1;
        m_Ed = null;
        m_EdIndex = -1;
        m_TempAttacBokLifeCycle = null;
        m_TempTTi = null;
        m_SrcUnitId = 0u;
        m_ExtendType = 0u;
        m_ExtendId = 0u;
        m_SkillTb = null;
    }
}

public class CacheNetBuffForNoMobData : BasePoolClass
{
    public BattleBuffChange m_BattleBuffChange;
    //是否在行为后的buff数据
    public bool m_IsAfterActive;

    public override void Clear()
    {
        m_BattleBuffChange = null;
        m_IsAfterActive = false;
    }
}

public class CacheDelayBirthNetNewUnitData : BasePoolClass
{
    public BattleUnitChange m_BattleUnitChange;
    public BattleUnit m_BattleUnit;
    public int m_ExcuteTurnIndex;

    public override void Clear()
    {
        m_BattleUnitChange = null;
        m_BattleUnit = null;
        m_ExcuteTurnIndex = -1;
    }
}
#endregion

public partial class Net_Combat
{
    public class RecordTurnInfoTemp
    {
        public uint SrcUnitId;
        public uint BuffSkillId;
        public uint HitEffect;
        public uint ExtraHitEffect;
        public bool IsConverAttack;
        public uint ExtendType;
        public uint ExtendId;
        public BattleUnit NewBattleUnit;
        public bool IsHaveHpMpChange;
        public int ExcuteDataIndex;

        public void Clear()
        {
            SrcUnitId = 0u;
            BuffSkillId = 0u;
            HitEffect = 0u;
            ExtraHitEffect = 0u;
            IsConverAttack = false;
            ExtendType = 0u;
            ExtendId = 0u;
            NewBattleUnit = null;
            IsHaveHpMpChange = false;
            ExcuteDataIndex = -1;
        }
    }

    private int _excuteTurnIndex;

    private int _attackCount;

    private List<MobEntity> _isSetHpMpChangeMobList;

    private RecordTurnInfoTemp _recordTurnInfoTemp = new RecordTurnInfoTemp();

    public Dictionary<int, NetExcuteTurnInfo> m_NetExcuteTurnInfoDic = new Dictionary<int, NetExcuteTurnInfo>();

    public List<uint> m_RecordCombatBehaveUnitList = new List<uint>();

    public Dictionary<int, NewUnitBehaveInfo> m_NewUnitBehaveInfoDic = new Dictionary<int, NewUnitBehaveInfo>();

    private Queue<BattleHpMpChange> _protectHpMpChangeTemp = new Queue<BattleHpMpChange>();

    private List<uint> _tempList = new List<uint>();

    private List<CacheDelayBirthNetNewUnitData> _cacheDelayBirthNetNewUnitDatas = new List<CacheDelayBirthNetNewUnitData>();

#if DEBUG_MODE
    private System.Text.StringBuilder _tempSb = new System.Text.StringBuilder();
    private System.Text.StringBuilder _snodeLogTempSb = new System.Text.StringBuilder();
#endif

    public void ClearDoExcuteTurnData()
    {
        _excuteTurnIndex = -1;

        _attackCount = 0;

        if (_isSetHpMpChangeMobList != null)
            _isSetHpMpChangeMobList.Clear();

        _recordTurnInfoTemp.Clear();

        ClearNetExcuteTurnInfoDatas();

        m_RecordCombatBehaveUnitList.Clear();

        if (m_NewUnitBehaveInfoDic.Count > 0)
        {
            foreach (var kv in m_NewUnitBehaveInfoDic)
            {
                NewUnitBehaveInfo newUnitBehaveInfo = kv.Value;
                if (newUnitBehaveInfo == null)
                    continue;

                newUnitBehaveInfo.Push();
            }
            m_NewUnitBehaveInfoDic.Clear();
        }

        if (_cacheDelayBirthNetNewUnitDatas.Count > 0)
        {
            for (int i = 0, count = _cacheDelayBirthNetNewUnitDatas.Count; i < count; i++)
            {
                CacheDelayBirthNetNewUnitData cacheDelayBirthNetNewUnitData = _cacheDelayBirthNetNewUnitDatas[i];
                if (cacheDelayBirthNetNewUnitData != null)
                    cacheDelayBirthNetNewUnitData.Push();
            }
            _cacheDelayBirthNetNewUnitDatas.Clear();
        }
        
        _tempList.Clear();
    }

    private void ClearNetExcuteTurnInfoDatas()
    {
        foreach (var kv in m_NetExcuteTurnInfoDic)
        {
            var neti = kv.Value;
            if (neti == null)
                continue;

            neti.Clear();
        }
    }

    //服务器发送的ExcuteData只会有一种类型的数据,其他都为null
    //被动中BattleHpMpChange不用Node包裹也能执行
    private void DoExcuteTurn(bool forceExcute = false, bool isCombatRound = true)
    {
        if (m_RoundOver)
        {
            return;
        }

        if (forceExcute)
        {
            m_RecordCombatBehaveUnitList.Clear();
            m_IsWaitDoExcute = false;
        }
        
        _nextExcuteIndex = -1;

        if (CombatManager.Instance.m_BattleTypeTb != null && CombatManager.Instance.m_BattleTypeTb.is_quickfight && m_RecordCombatBehaveUnitList.Count > 0)
            return;

        if (_excuteTurnList == null)
        {
            DebugUtil.LogError($"DoExcuteTurn方法中_excuteTurnList为null");
            return;
        }

        _time = 0f;

        //一次行动要不就是攻击要不就是buff处理
        int excuteCount = 0;
        bool isHaveBehave = false;
        while (_excuteTurnIndex < _excuteTurnList.Count)
        {
            if (excuteCount > 0)
            {
                excuteCount = 0;
                return;
            }

            ExcuteTurn excuteTurn = _excuteTurnList[_excuteTurnIndex];
            _excuteTurnIndex++;
            
            //当服务器只发stage数据时，屏蔽该条数据运行
            if ((excuteTurn.SrcUnit == null || excuteTurn.SrcUnit.Count < 1) &&
                (excuteTurn.SkillId == null || excuteTurn.SkillId.Count < 1) &&
                (excuteTurn.Talks == null || excuteTurn.Talks.Count < 1) &&
                (excuteTurn.ExcuteData == null || excuteTurn.ExcuteData.Count < 1))
            {
                DLogManager.Log(ELogType.eCombat, $"excuteTurnIndex:{_excuteTurnIndex - 1}  <color=yellow>服务器只发stage数据，屏蔽该条数据运行</color>");
                continue;
            }

            _attackCount = 0;

            #region 检测需要执行动作的角色是否处于行动中
            if (IsCheckInterceptBehave(excuteTurn, forceExcute))
                return;
            #endregion

            #region 打印该ExcuteTurn信息
#if DEBUG_MODE
            _tempSb.Clear();

            _tempSb.Append($"显示执行技能Ids----<color=yellow>skillIds: ");
            foreach (var skillId in excuteTurn.SkillId)
            {
                _tempSb.Append($"{skillId.ToString()};");
            }
            _tempSb.Append($"  Stage : {excuteTurn.Stage.ToString()}</color>");

            _tempSb.Append($"   喊话信息----<color=yellow>Talks----");
            for (int i = 0; i < excuteTurn.Talks.Count; i++)
            {
                _tempSb.Append($"【UnitId:{excuteTurn.Talks[i].UnitId.ToString()}  talkId:{excuteTurn.Talks[i].TalkId.ToString()}】    ");
            }
            _tempSb.Append($"</color>");
#endif
            #endregion

            #region 清除数据缓存
            if (_isSetHpMpChangeMobList == null)
                _isSetHpMpChangeMobList = new List<MobEntity>();
            else
                _isSetHpMpChangeMobList.Clear();

            if (!m_NetExcuteTurnInfoDic.TryGetValue(_excuteTurnIndex - 1, out NetExcuteTurnInfo netExcuteTurnInfo) ||
                netExcuteTurnInfo == null)
            {
                netExcuteTurnInfo = new NetExcuteTurnInfo();
                m_NetExcuteTurnInfoDic[_excuteTurnIndex - 1] = netExcuteTurnInfo;
            }
            else
            {
                netExcuteTurnInfo.Clear();

#if DEBUG_MODE
                _tempSb.Append($"   NetExcuteTurnInfo----<color=yellow>对下标{_excuteTurnIndex - 1}的数据进行清除</color>");
#endif
            }

#if DEBUG_MODE
            DLogManager.Log(ELogType.eCombat, $"{_tempSb.ToString()}");
            _tempSb.Clear();

            CheckProtectHpMpChangeData(true);
#endif

            _protectHpMpChangeTemp.Clear();
            #endregion

            #region 准备数据
            //对话数据
            netExcuteTurnInfo.TalkInfos = excuteTurn.Talks;

            CSVActiveSkill.Data skillTb = null;
            if (excuteTurn.SkillId.Count > 0)
                skillTb = CSVActiveSkill.Instance.GetConfData(excuteTurn.SkillId[0]);

            int srcCount = excuteTurn.SrcUnit.Count;
            if (srcCount > 1)
            {
                for (int srcIndex = 0; srcIndex < srcCount; srcIndex++)
                {
                    if (netExcuteTurnInfo.CombineAttack_SrcUnits == null)
                        netExcuteTurnInfo.CombineAttack_SrcUnits = new List<uint>();
                    netExcuteTurnInfo.CombineAttack_SrcUnits.Add(excuteTurn.SrcUnit[srcIndex]);
                }
            }
            
            bool isAttackSkillSuccess = true;

            if (netExcuteTurnInfo.AttackBoLifeCycleInfoList == null)
                netExcuteTurnInfo.AttackBoLifeCycleInfoList = new List<AttackBoLifeCycleInfo>();
            AttackBoLifeCycleInfo tempAttackBoLifeCycle = BasePoolClass.Get<AttackBoLifeCycleInfo>();
            netExcuteTurnInfo.AttackBoLifeCycleInfoList.Add(tempAttackBoLifeCycle);
            
            int layer = 1;
            SNodeInfo tempTTi = null;
            int bo = -1;
            int isBoState = 0;
            tempAttackBoLifeCycle.m_OutBoInfo = BasePoolClass.Get<OutBoInfo>();
            tempAttackBoLifeCycle.m_OutBoInfo.Stage = excuteTurn.Stage;
            tempAttackBoLifeCycle.m_OutBoInfo.LeftStartIndex = 0;
            if (tempAttackBoLifeCycle.SNodeInfoList == null)
                tempAttackBoLifeCycle.SNodeInfoList = new List<SNodeInfo>();
            int excuteTurnCount = excuteTurn.ExcuteData.Count;
            for (int i = 0; i < excuteTurnCount; i++)
            {
                ExcuteData ed = excuteTurn.ExcuteData[i];
                
                GetParseExcuteDataExtendId(ed.ExtendId, out uint srcUnitId, out uint extendType, out uint extendId);

                var sc = ed.StageChange;
                if (sc != null)
                {
                    if (m_CurServerBattleStage != sc.NewStage)
                    {
                        eventEmitter.Trigger<uint>(EEvents.OnChangeBattleStage, sc.NewStage);
                    }
                    m_CurServerBattleStage = sc.NewStage;
                }

                tempAttackBoLifeCycle.m_OutBoInfo.Stage = excuteTurn.Stage;
                if (isBoState == 2)
                {
                    bo = -1;
                }

                if (ed.Bo > 0)
                {
                    if (bo == -1)
                    {
                        DebugUtil.Log(ELogType.eCombat, $"-------Bo【Start】----<color=magenta>Bo:{ed.Bo.ToString()}   edIndex:{i.ToString()}</color>");

                        if (tempTTi != null)
                        {
                            DebugUtil.LogError($"bo内Node开始时tempTTi不为空，edIndex:{i.ToString()} SNodeId:{tempTTi.SNodeId.ToString()}  Layer:{layer.ToString()}   LS:{tempTTi.LeftStartIndex.ToString()}    LE:{tempTTi.LeftEndIndex.ToString()}    RS:{tempTTi.RightStartIndex.ToString()}  RE:{tempTTi.RightEndIndex.ToString()}");
                            tempTTi = null;
                        }

                        if (isBoState == 0)
                            tempAttackBoLifeCycle.m_OutBoInfo.LeftEndIndex = i;
                        else
                        {
                            tempAttackBoLifeCycle.m_OutBoInfo.RightEndIndex = i - 1;
                            tempAttackBoLifeCycle.m_OutBoInfo.SetSNodeInfoListBo();

                            tempAttackBoLifeCycle = BasePoolClass.Get<AttackBoLifeCycleInfo>();
                            netExcuteTurnInfo.AttackBoLifeCycleInfoList.Add(tempAttackBoLifeCycle);
                            if (tempAttackBoLifeCycle.SNodeInfoList == null)
                                tempAttackBoLifeCycle.SNodeInfoList = new List<SNodeInfo>();

                            tempAttackBoLifeCycle.m_OutBoInfo = BasePoolClass.Get<OutBoInfo>();
                            tempAttackBoLifeCycle.m_OutBoInfo.LeftStartIndex = i;
                            tempAttackBoLifeCycle.m_OutBoInfo.LeftEndIndex = i;
                        }

                        bo = ed.Bo;
                        isBoState = 1;

                        tempAttackBoLifeCycle.m_OutBoInfo.Bo = bo;
                    }
                    else
                    {
                        if (bo != ed.Bo)
                            DebugUtil.LogError($"_excuteTurnIndex:{_excuteTurnIndex.ToString()} ExcuteDataIndex:{i.ToString()} 波数情况不对bo:{bo.ToString()}  ed.Bo:{ed.Bo.ToString()}");

                        isBoState = 2;

                        tempAttackBoLifeCycle.m_OutBoInfo.RightStartIndex = i;

                        DebugUtil.Log(ELogType.eCombat, $"-------Bo【End】----<color=magenta>Bo:{ed.Bo.ToString()}   edIndex:{i.ToString()}</color>");
                    }
                }

                #region 处理时机
                tempTTi = SetSNodeInfoExcuteDataIndexRangePart1(tempTTi, i, ed.Node, bo, tempAttackBoLifeCycle.SNodeInfoList, ref layer);
                #endregion

                if (ed.ExtendId == 0)
                {
                    if (ed.BuffChange == null)
                    {
                        #region 服务器AI增删战斗单位处理
                        DoImmediatelyExcuteDataUnitsChange(ed, _excuteTurnIndex - 1, i, false);
                        #endregion

                        continue;
                    }
                    else
                    {
                        extendType = 2u;
                    }
                }

#if DEBUG_MODE
                _tempSb.Clear();
                _tempSb.Append($"ed.ExtendId:[T:{(_excuteTurnIndex - 1).ToString()}@D:{i.ToString()}]---<color=yellow>ExtendId：{ed.ExtendId.ToString()}    srcUnitId:{srcUnitId.ToString()}    extendType:{extendType.ToString()}  extendId:{extendId.ToString()}</color>");
                if (extendType == 1u && extendId > 0u)
                {
                    if (ed.PassiveTri != null)
                    {
                        _tempSb.Append($"   PassiveTri.TriggerId:{ed.PassiveTri.TriggerId.ToString()}");
                    }

                    CSVPassiveSkill.Data passiveSkillTb = CSVPassiveSkill.Instance.GetConfData(extendId);
                    if (passiveSkillTb != null)
                    {
                        _tempSb.Append($"   passiveEffectTrigger:{passiveSkillTb.effect_trigger.ToString()}");
                    }
                }
                DLogManager.Log(ELogType.eCombat, _tempSb.ToString());
                _tempSb.Clear();
#endif

                bool isNeedAddTarget = false;

                if (bo > 0)
                    tempTTi = SetSNodeInfoExcuteDataIndexRangePart2(tempTTi, i, bo, tempAttackBoLifeCycle.SNodeInfoList,
                        ed.ExtendId, srcUnitId, extendType, extendId, true, ref layer);

                #region 处理主动
                if (extendType == 0)
                {
                    if (bo > 0)
                    {
                        if (tempTTi.ActiveTurnInfoList == null)
                            tempTTi.ActiveTurnInfoList = new List<ActiveTurnInfo>();

                        ActiveTurnInfo activeTurnInfo = BasePoolClass.Get<ActiveTurnInfo>();
                        activeTurnInfo.SrcUnitId = srcUnitId;
                        activeTurnInfo.ActiveLogicId = extendId;
                        activeTurnInfo.ExcuteDataIndex = i;

                        tempTTi.ActiveTurnInfoList.Add(activeTurnInfo);

                        DLogManager.Log(ELogType.eCombat, $"主动技能-----<color=yellow>Layer:{tempTTi.Layer.ToString()}层中主动技能ExtendId:{ed.ExtendId.ToString()}  SrcUnitId:{srcUnitId.ToString()}    extendId:{extendId.ToString()}</color>");

                    }

#if DEBUG_MODE
                    if (tempTTi.Layer > 1)
                    {
                        MobEntity activeCheckedMob = MobManager.Instance.GetMob(srcUnitId);
                        if (activeCheckedMob != null && activeCheckedMob.m_MobCombatComponent != null)
                        {
                            activeCheckedMob.m_MobCombatComponent.m_isHaveChildLayerActive = true;
                        }
                    }
#endif

                    isNeedAddTarget = true;

                    tempTTi.IsAfterActive = true;
#if DEBUG_MODE
                    if (ed.BuffChange != null)
                    {
                        var buffTb = CSVBuff.Instance.GetConfData(ed.BuffChange.BuffId);
                        if (buffTb == null)
                            DebugUtil.LogError($"ExcuteTurnIndex:{(_excuteTurnIndex - 1).ToString()}  ExcuteDataIndex:{i.ToString()}  srcUnitId:{srcUnitId.ToString()}    extendType:{extendType.ToString()}  extendId:{extendId.ToString()}   BuffId:{ed.BuffChange.BuffId.ToString()}在buff表中不存在");
                        else if (buffTb.skill_id == 0u)
                            DebugUtil.LogError($"ExcuteTurnIndex:{(_excuteTurnIndex - 1).ToString()}  ExcuteDataIndex:{i.ToString()}  srcUnitId:{srcUnitId.ToString()}    extendType:{extendType.ToString()}  extendId:{extendId.ToString()}   BuffId:{ed.BuffChange.BuffId.ToString()}是作为主动行为却在buff表中skill_id字段没有表现行为");
                    }
#endif
                }
                else if (extendType == 5u)  //反击处理
                {
                    isNeedAddTarget = true;

                    if (tempTTi != null)
                        tempTTi.IsAfterActive = true;
                }
                #endregion
                
                #region 处理ExcuteData数据
                tempAttackBoLifeCycle.IsOutBoTempFlag = bo < 1;
                HandleExcuteData(excuteTurn, _excuteTurnIndex - 1, ed, i, netExcuteTurnInfo, tempAttackBoLifeCycle, tempTTi, srcUnitId, extendType, extendId, isNeedAddTarget,
                    skillTb, 0, ref isHaveBehave, ref isAttackSkillSuccess);
                #endregion
                #endregion
            }

            #region ExcuteData结束时处理下OutBoInfo数据
            if (tempAttackBoLifeCycle.m_OutBoInfo.LeftStartIndex < 0)
                tempAttackBoLifeCycle.m_OutBoInfo.LeftStartIndex = 0;
            if (tempAttackBoLifeCycle.m_OutBoInfo.LeftEndIndex < 0)
                tempAttackBoLifeCycle.m_OutBoInfo.LeftEndIndex = excuteTurnCount;
            else if (tempAttackBoLifeCycle.m_OutBoInfo.LeftEndIndex < tempAttackBoLifeCycle.m_OutBoInfo.LeftStartIndex)
                tempAttackBoLifeCycle.m_OutBoInfo.LeftEndIndex = tempAttackBoLifeCycle.m_OutBoInfo.LeftStartIndex;
            if (tempAttackBoLifeCycle.m_OutBoInfo.RightStartIndex < 0 || tempAttackBoLifeCycle.m_OutBoInfo.RightStartIndex < tempAttackBoLifeCycle.m_OutBoInfo.LeftEndIndex)
                tempAttackBoLifeCycle.m_OutBoInfo.RightStartIndex = excuteTurnCount;
            if (tempAttackBoLifeCycle.m_OutBoInfo.RightEndIndex < tempAttackBoLifeCycle.m_OutBoInfo.RightStartIndex)
            {
                tempAttackBoLifeCycle.m_OutBoInfo.RightEndIndex = excuteTurnCount;
                tempAttackBoLifeCycle.m_OutBoInfo.SetSNodeInfoListBo();
            }
            DebugUtil.Log(ELogType.eCombat, $"ExcuteData结束时bo外数据----<color=magenta>ExcuteTurnIndex:{(_excuteTurnIndex - 1).ToString()}  Stage:{tempAttackBoLifeCycle.m_OutBoInfo.Stage.ToString()}  Bo:{tempAttackBoLifeCycle.m_OutBoInfo.Bo.ToString()}   LeftStartIndex:{tempAttackBoLifeCycle.m_OutBoInfo.LeftStartIndex.ToString()}   LeftEndIndex:{tempAttackBoLifeCycle.m_OutBoInfo.LeftEndIndex.ToString()}   RightStartIndex:{tempAttackBoLifeCycle.m_OutBoInfo.RightStartIndex.ToString()}   RightEndIndex:{tempAttackBoLifeCycle.m_OutBoInfo.RightEndIndex.ToString()}</color>");
            #endregion
            
            //放技能放失败处理
            if (excuteTurn.ExcuteData.Count <= 0 && excuteTurn.SrcUnit != null && excuteTurn.SrcUnit.Count > 0)
            {
                MobEntity tarPosMob = MobManager.Instance.GetMobByServerNum(excuteTurn.TarPos);
                if (tarPosMob != null)
                {
                    SetUnitsChangeTarget(excuteTurn, netExcuteTurnInfo, -1, excuteTurn.SrcUnit[0], 0u, 0u, 
                        tempAttackBoLifeCycle, tempTTi, tarPosMob.m_MobCombatComponent.m_BattleUnit.UnitId, null, 0u, 5, 0, 0);
                }
            }

            #region 服务器数据解析完后对数据再次处理
#if DEBUG_MODE
            CheckProtectHpMpChangeData(false);
#endif
            _protectHpMpChangeTemp.Clear();

            #region 对远程目标数量大于3的进行随机混合和重置m_IsSetHpChange标记
            //被击者进行随机混合
            if (skillTb != null && skillTb.attack_type == 2u)
            {
                for (int i = 1; i < netExcuteTurnInfo.TurnBehaveInfoList.Count; i++)
                {
                    TurnBehaveInfo turnBehaveInfo = netExcuteTurnInfo.TurnBehaveInfoList[i];
                    if (turnBehaveInfo == null || turnBehaveInfo.SrcUnitId == 0u || turnBehaveInfo.TurnBehaveSkillInfoList == null)
                        continue;

                    for (int tbSkillIndex = 0, tbSkillCount = turnBehaveInfo.TurnBehaveSkillInfoList.Count; tbSkillIndex < tbSkillCount; tbSkillIndex++)
                    {
                        TurnBehaveSkillInfo turnBehaveSkillInfo = turnBehaveInfo.TurnBehaveSkillInfoList[tbSkillIndex];
                        if (turnBehaveSkillInfo == null ||
                            turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList == null || turnBehaveSkillInfo.TargetUnitCount < 4)
                            continue;

                        for (int tbiTargetIndex = 0, tbiTargetCount = turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count; tbiTargetIndex < tbiTargetCount; tbiTargetIndex++)
                        {
                            if (tbiTargetIndex == tbiTargetCount - 1)
                                break;

                            int ri = Random.Range(tbiTargetIndex, tbiTargetCount);
                            if (ri == tbiTargetIndex)
                                continue;

                            var tempTbiTargetUnitId = turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[tbiTargetIndex];
                            turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[tbiTargetIndex] = turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[ri];
                            turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[ri] = tempTbiTargetUnitId;
                        }
                    }
                }
            }
            if (_isSetHpMpChangeMobList.Count > 0)
            {
                for (int shmcmIndex = 0, shmcmCount = _isSetHpMpChangeMobList.Count; shmcmIndex < shmcmCount; shmcmIndex++)
                {
                    _isSetHpMpChangeMobList[shmcmIndex].m_MobCombatComponent.m_IsSetHpChange = false;
                }
            }
            #endregion
            #endregion

            #region 开始执行行为
            
            //执行行动前的bo外buff
            if (tempAttackBoLifeCycle.m_OutBoInfo.m_BuffChangeDataBeforeQueue != null)
            {
                while (tempAttackBoLifeCycle.m_OutBoInfo.m_BuffChangeDataBeforeQueue.Count > 0)
                {
                    uint bcUnitId = tempAttackBoLifeCycle.m_OutBoInfo.m_BuffChangeDataBeforeQueue.Dequeue();
                    var buffMob = MobManager.Instance.GetMob(bcUnitId);
                    if (buffMob != null)
                    {
                        buffMob.DoProcessBuffChange(excuteTurn.Stage != 2);
                    }
                }
            }

            PerformBehave(netExcuteTurnInfo.TurnBehaveInfoList, _excuteTurnIndex - 1, isAttackSkillSuccess, true, 0, ref isHaveBehave);
            
            PerformOutRound(excuteTurn.Stage, _excuteTurnIndex - 1, tempAttackBoLifeCycle.m_OutBoInfo, ref isHaveBehave);

            //执行行动后的bo外buff
            if (tempAttackBoLifeCycle.m_OutBoInfo.m_BuffChangeDataAfterQueue != null)
            {
                while (tempAttackBoLifeCycle.m_OutBoInfo.m_BuffChangeDataAfterQueue.Count > 0)
                {
                    uint bcUnitId = tempAttackBoLifeCycle.m_OutBoInfo.m_BuffChangeDataAfterQueue.Dequeue();
                    var buffMob = MobManager.Instance.GetMob(bcUnitId);
                    if (buffMob != null)
                    {
                        buffMob.DoProcessBuffChange();
                    }
                }
            }
            #endregion

            #region 回合结束指令
            if (isHaveBehave)
            {
                excuteCount++;
                _nextExcuteIndex = _excuteTurnIndex;
                _fastTime = Time.time;

                //DebugUtil.LogWarning($"nextExcuteIndex : {_nextExcuteIndex.ToString()}  excuteTurnList : {_excuteTurnList.Count.ToString()}");
            }
            #endregion

            forceExcute = false;
        }

        if (isCombatRound)
        {
            if (!isHaveBehave)
            {
                if (_sentOverRoundReq || (!forceExcute && (MobManager.Instance.IsHaveBehaveMob() || IsExistNeedNewUnitBehaveInfoEnterBattle())))
                    return;

                _sentOverRoundReq = true;
                
                DoBattleResult();
            }
        }
        else
        {
            m_RoundOver = true;
        }
    }

    private SNodeInfo SetSNodeInfoExcuteDataIndexRangePart1(SNodeInfo tempTTi, int excuteDataIndex, uint node, int bo, List<SNodeInfo> snodeInfoList, 
        ref int layer)
    {
        if (node > 0u)
        {
            if (layer == 1 && tempTTi == null)
            {
#if DEBUG_MODE
                _snodeLogTempSb.Clear();
                _snodeLogTempSb.Append($"SNodeInfo数据设置-----<color=yellow>");
#endif

                tempTTi = BasePoolClass.Get<SNodeInfo>();
                snodeInfoList.Add(tempTTi);

                tempTTi.Bo = bo;
                tempTTi.Layer = layer;

                DebugUtil.Log(ELogType.eCombat, $"-------Node【Start】----<color=magenta>node:{node.ToString()}   layer:{layer.ToString()}   edIndex:{excuteDataIndex.ToString()}   bo:{bo.ToString()}</color>");
            }

            if (tempTTi.SNodeId == 0u)
            {
                tempTTi.SNodeId = node;
                if (tempTTi.LeftStartIndex < 0)
                    tempTTi.LeftStartIndex = excuteDataIndex;
                tempTTi.LeftEndIndex = excuteDataIndex;

#if DEBUG_MODE
                if (tempTTi.ParentSNodeInfo == null)
                {
                    int preTtiIndex = snodeInfoList.Count - 2;
                    if (preTtiIndex > -1)
                    {
                        SNodeInfo preTti = snodeInfoList[preTtiIndex];
                        if (preTti != null)
                        {
                            if (preTti.RightEndIndex > tempTTi.LeftStartIndex)
                                DebugUtil.LogError($"前SNodeId:{preTti.SNodeId.ToString()}的RightEndIndex:{preTti.RightEndIndex.ToString()} > 后SNodeId:{tempTTi.SNodeId.ToString()}的LeftStartIndex:{tempTTi.LeftStartIndex.ToString()}");
                        }
                    }
                }
                else if (tempTTi.ParentSNodeInfo.ChildSNodeInfoList != null)
                {
                    int preTtiIndex = tempTTi.ParentSNodeInfo.ChildSNodeInfoList.Count - 2;
                    if (preTtiIndex > -1)
                    {
                        SNodeInfo preTti = tempTTi.ParentSNodeInfo.ChildSNodeInfoList[preTtiIndex];
                        if (preTti != null)
                        {
                            if (preTti.RightEndIndex > tempTTi.LeftStartIndex)
                                DebugUtil.LogError($"前SNodeId:{preTti.SNodeId.ToString()}的RightEndIndex:{preTti.RightEndIndex.ToString()} > 后SNodeId:{tempTTi.SNodeId.ToString()}的LeftStartIndex:{tempTTi.LeftStartIndex.ToString()}");
                        }
                    }
                }

                _snodeLogTempSb.Append($"   SNodeId:{tempTTi.SNodeId.ToString()}[D:{excuteDataIndex.ToString()}]   [T:{(_excuteTurnIndex - 1).ToString()}@D:{excuteDataIndex.ToString()}]LeftStartIndex[LI:{tempTTi.SNodeId.ToString()}&{tempTTi.Layer.ToString()}]:{tempTTi.LeftStartIndex.ToString()}");
#endif
            }
            else if (tempTTi.SNodeId == node)
            {
                if (tempTTi.RightStartIndex < 0)
                    tempTTi.RightStartIndex = excuteDataIndex;
                tempTTi.RightEndIndex = excuteDataIndex;

#if DEBUG_MODE
                _snodeLogTempSb.Append($"   [T:{(_excuteTurnIndex - 1).ToString()}@D:{excuteDataIndex.ToString()}]RightEndIndex[LI:{tempTTi.SNodeId.ToString()}&La:{tempTTi.Layer.ToString()}]:{tempTTi.RightEndIndex.ToString()}");

                if (tempTTi.RightStartIndex > tempTTi.RightEndIndex)
                    DebugUtil.LogError($"{_excuteTurnIndex - 1}处理时机时数据有问题RightStartIndex:{tempTTi.RightStartIndex.ToString()}  RightEndIndex:{tempTTi.RightEndIndex.ToString()}");
#endif

                if (tempTTi.ParentSNodeInfo != null)
                {
#if DEBUG_MODE
                    if (layer < 2)
                        DebugUtil.LogError($"{_excuteTurnIndex - 1}处理时机时,当前Layer：{layer.ToString()},但却有父时机信息Layer:{tempTTi.ParentSNodeInfo.Layer.ToString()} LeftStartIndex:{tempTTi.ParentSNodeInfo.LeftStartIndex.ToString()}");
#endif

                    //当前时机回到父时机
                    tempTTi = tempTTi.ParentSNodeInfo;
                    tempTTi.RightStartIndex = excuteDataIndex + 1;

#if DEBUG_MODE
                    _snodeLogTempSb.Append($"   [T:{(_excuteTurnIndex - 1).ToString()}@D:{excuteDataIndex.ToString()}]RightStartIndex[LI:{tempTTi.SNodeId.ToString()}&La:{tempTTi.Layer.ToString()}]:{tempTTi.RightStartIndex.ToString()}");
#endif
                }
#if DEBUG_MODE
                else if (layer > 1)
                {
                    DebugUtil.LogError($"{_excuteTurnIndex - 1}处理时机时,当前Layer：{layer.ToString()},但却没有父时机信息Layer:{tempTTi.Layer.ToString()} LeftStartIndex:{tempTTi.LeftStartIndex.ToString()}");
                }
#endif

                if (layer > 1)
                {
                    DebugUtil.Log(ELogType.eCombat, $"-------Node【End】----<color=magenta>node:{node.ToString()}   layer:{layer.ToString()}   edIndex:{excuteDataIndex.ToString()}   bo:{bo.ToString()}</color>");
                    --layer;
                }
                else
                {
                    tempTTi = null;

#if DEBUG_MODE
                    DLogManager.Log(ELogType.eCombat, $"{_snodeLogTempSb.ToString()}</color>");

                    DebugUtil.Log(ELogType.eCombat, $"-------Node【End】----<color=magenta>node:{node.ToString()}   layer:{layer.ToString()}   edIndex:{excuteDataIndex.ToString()}   bo:{bo.ToString()}</color>");
#endif
                }
            }
            else if (tempTTi.SNodeId != node)
            {
                #region 处理当前时机
                if (tempTTi.LeftEndIndex <= tempTTi.LeftStartIndex)
                {
                    tempTTi.LeftEndIndex = excuteDataIndex - 1;

#if DEBUG_MODE
                    _snodeLogTempSb.Append($"   [T:{(_excuteTurnIndex - 1).ToString()}@D:{excuteDataIndex.ToString()}]LeftEndIndex[LI:{tempTTi.SNodeId.ToString()}&La:{tempTTi.Layer.ToString()}]:{tempTTi.LeftEndIndex.ToString()}");
#endif
                }

#if DEBUG_MODE
                if (tempTTi.LeftStartIndex > tempTTi.LeftEndIndex)
                    DebugUtil.LogError($"{_excuteTurnIndex - 1}处理时机时数据有问题LeftStartIndex:{tempTTi.LeftStartIndex}  LeftEndIndex:{tempTTi.LeftEndIndex}");
#endif
                #endregion

                #region 遇到子时机,进行处理
                if (tempTTi.ChildSNodeInfoList == null)
                    tempTTi.ChildSNodeInfoList = new List<SNodeInfo>();

                ++layer;

                SNodeInfo childTti = BasePoolClass.Get<SNodeInfo>();
                childTti.ParentSNodeInfo = tempTTi;
                childTti.Bo = bo;
                childTti.Layer = layer;
                childTti.SNodeId = node;
                childTti.LeftStartIndex = excuteDataIndex;
                childTti.LeftEndIndex = excuteDataIndex;

                tempTTi.ChildSNodeInfoList.Add(childTti);

#if DEBUG_MODE
                _snodeLogTempSb.Append($"   子[T:{(_excuteTurnIndex - 1).ToString()}@D:{excuteDataIndex.ToString()}]LeftStartIndex[LI:{childTti.SNodeId.ToString()}&La:{childTti.Layer.ToString()}]:{childTti.LeftStartIndex.ToString()}");

                DebugUtil.Log(ELogType.eCombat, $"-------Node【Start】----<color=magenta>node:{node.ToString()}   layer:{layer.ToString()}   edIndex:{excuteDataIndex.ToString()}   bo:{bo.ToString()}</color>");
#endif
                #endregion

                //把子时机置为当前时机
                tempTTi = childTti;
            }
        }

        return tempTTi;
    }

    private SNodeInfo SetSNodeInfoExcuteDataIndexRangePart2(SNodeInfo tempTTi, int excuteDataIndex, int bo, List<SNodeInfo> snodeInfoList,
        ulong edExtendId, uint srcUnitId, uint extendType, uint extendId, bool isAddTempTTi, ref int layer)
    {
        if (tempTTi == null)
        {
            if (layer == 1)
            {
                if (snodeInfoList.Count > 0)
                {
                    SNodeInfo tti = snodeInfoList[snodeInfoList.Count - 1];
                    if (tti != null)
                    {
                        DLogManager.Log(ELogType.eCombat, $"时机【外结束 】[T:{(_excuteTurnIndex - 1).ToString()}@D:{excuteDataIndex.ToString()}]---<color=yellow>SNodeId:{tti.SNodeId.ToString()}   旧RightEndIndex:{tti.RightEndIndex.ToString()}   新RightEndIndex:{excuteDataIndex.ToString()}    ExtendId：{edExtendId.ToString()}    srcUnitIdTest:{srcUnitId.ToString()}    extendTypeTest:{extendType.ToString()}  extendIdTest:{extendId.ToString()}</color>");

                        if (tti.RightEndIndex < excuteDataIndex)
                            tti.RightEndIndex = excuteDataIndex;
                    }
                }
                else if (isAddTempTTi)
                {
#if DEBUG_MODE
                    _snodeLogTempSb.Clear();
                    _snodeLogTempSb.Append($"SNodeInfo数据设置-----<color=yellow>");
#endif

                    tempTTi = BasePoolClass.Get<SNodeInfo>();
                    snodeInfoList.Add(tempTTi);

                    tempTTi.Bo = bo;
                    tempTTi.Layer = layer;
                    tempTTi.LeftStartIndex = excuteDataIndex;

                    DLogManager.Log(ELogType.eCombat, $"时机【外开始】[T:{(_excuteTurnIndex - 1).ToString()}@D:{excuteDataIndex.ToString()}]---<color=yellow>ExtendId：{edExtendId.ToString()}    srcUnitIdTest:{srcUnitId.ToString()}    extendTypeTest:{extendType.ToString()}  extendIdTest:{extendId.ToString()}</color>");
                }
            }
#if DEBUG_MODE
            else
            {
                DebugUtil.LogError($"[T:{(_excuteTurnIndex - 1).ToString()}&D:{excuteDataIndex.ToString()}]数据下标   ed.ExtendId:{edExtendId.ToString()}时tempTTi未null   layer：{layer.ToString()}");
            }
#endif
        }

        return tempTTi;
    }

    /// <summary>
    /// excuteDataExtendId的前16位 extendType=0 主动效果 1 被动  2 Buff 3技能行动前 4 光环 5反击；
    /// 中16位 ownId = 触发者unit_id；
    /// 后32   extendId = 被动触发ID
    /// </summary>
    public void GetParseExcuteDataExtendId(ulong excuteDataExtendId, out uint ownId, out uint extendType, out uint extendId)
    {
        if (excuteDataExtendId == 0ul)
        {
            ownId = 0u;
            extendType = 0u;
            extendId = 0u;
            return;
        }

        ulong extendId_64_48_16 = excuteDataExtendId >> 48;
        extendType = (uint)extendId_64_48_16;

        ulong extendId_48_0_48 = excuteDataExtendId - (extendId_64_48_16 << 48);
        ulong extendId_48_32_16 = extendId_48_0_48 >> 32;
        ownId = (uint)extendId_48_32_16;

        extendId = (uint)(extendId_48_0_48 - (extendId_48_32_16 << 32));
    }

    private bool IsCheckInterceptBehave(ExcuteTurn excuteTurn, bool forceExcute)
    {
        for (int i = 0, edCheckCount = excuteTurn.ExcuteData.Count; i < edCheckCount; i++)
        {
            ExcuteData excuteData = excuteTurn.ExcuteData[i];

            if (!CheckHpMpState(excuteTurn, excuteData, forceExcute, 1) ||
                !CheckBuffState(excuteTurn, excuteData, forceExcute, 1) ||
                !CheckUnitsState(excuteData, forceExcute, 1)
                )
                return true;
        }

        if (excuteTurn.ExcuteData.Count <= 0 && excuteTurn.SrcUnit != null && excuteTurn.SrcUnit.Count > 0)
        {
            MobEntity mob = MobManager.Instance.GetMobByServerNum(excuteTurn.TarPos);
            if (mob != null)
            {
                if (InterceptBehave(mob.m_MobCombatComponent.m_BattleUnit.UnitId, forceExcute) < 0)
                    return true;
            }
        }

        for (int i = 0; i < excuteTurn.SrcUnit.Count; i++)
        {
            if (InterceptBehave(excuteTurn.SrcUnit[i], forceExcute) < 0)
                return true;
        }
        
        return false;
    }

    public bool IsExistInExcuteTurns(RepeatedField<ExcuteTurn> excuteTurnList, System.Func<uint, bool> func)
    {
        if (excuteTurnList == null || excuteTurnList.Count < 1)
            return false;

        for (int etIndex = 0, etCount = excuteTurnList.Count; etIndex < etCount; etIndex++)
        {
            ExcuteTurn excuteTurn = excuteTurnList[etIndex];
            if (excuteTurn == null)
                continue;

            for (int i = 0, edCheckCount = excuteTurn.ExcuteData.Count; i < edCheckCount; i++)
            {
                ExcuteData excuteData = excuteTurn.ExcuteData[i];

                if (!CheckHpMpState(excuteTurn, excuteData, false, 2, func) 
                    //!CheckBuffState(excuteTurn, excuteData, false, 2, func) ||
                    //!CheckUnitsState(excuteData, false, 2, func)
                    )
                    return true;
            }

            //if (excuteTurn.ExcuteData.Count <= 0 && excuteTurn.SrcUnit != null && excuteTurn.SrcUnit.Count > 0)
            //{
            //    MobEntity mob = MobManager.Instance.GetMobByServerNum(excuteTurn.TarPos);
            //    if (mob != null && func != null)
            //    {
            //        if (func(mob.m_MobCombatComponent.m_BattleUnit.UnitId))
            //            return true;
            //    }
            //}

            //for (int i = 0; i < excuteTurn.SrcUnit.Count; i++)
            //{
            //    if (func != null && func(excuteTurn.SrcUnit[i]))
            //        return true;
            //}
        }

        return false;
    }

    /// <summary>
    /// type=1检测行为，=2是否存在
    /// </summary>
    private bool CheckHpMpState(ExcuteTurn excuteTurn, ExcuteData excuteData, bool forceExcute, int type, System.Func<uint, bool> func = null)
    {
        var hc = excuteData.HpChange;
        if (hc != null)
        {
            if (type == 1)
            {
                if (InterceptBehave(hc.UnitId, forceExcute) < 0)
                    return false;

                GetParseExcuteDataExtendId(excuteData.ExtendId, out uint srcUnitId, out uint extendType, out uint extendId);

                if (excuteTurn.SrcUnit.Count > 0 && excuteTurn.SrcUnit[0] == srcUnitId && hc.HpChange < 0)
                    _attackCount++;
            }
            else if (type == 2 && func != null)
            {
                if (func(hc.UnitId))
                    return false;
            }
        }

        return true;
    }

    private bool CheckBuffState(ExcuteTurn excuteTurn, ExcuteData excuteData, bool forceExcute, int type, System.Func<uint, bool> func = null)
    {
        var bc = excuteData.BuffChange;
        if (bc != null)
        {
            if (type == 1)
            {
                if (CheckBehaveBuff(excuteTurn, bc))
                {
                    if (InterceptBehave(bc.UnitId, forceExcute) < 0)
                        return false;
                }
            }
            else if (type == 2 && func != null)
            {
                if (func(bc.UnitId))
                    return false;
            }
        }

        return true;
    }

    private bool CheckUnitsState(ExcuteData excuteData, bool forceExcute, int type, System.Func<uint, bool> func = null)
    {
        var uc = excuteData.UnitsChange;
        if (uc != null)
        {
            //删除单位成功
            if (uc.DelUnitId != null && uc.DelUnitId.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.DelUnitId.Count; ucIndex < ucCount; ucIndex++)
                {
                    if (type == 1)
                    {
                        if (InterceptBehave(uc.DelUnitId[ucIndex], forceExcute) < 0)
                            return false;
                    }
                    else if (type == 2 && func != null)
                    {
                        if (func(uc.DelUnitId[ucIndex]))
                            return false;
                    }
                }
            }
            //删除单位失败
            if (uc.DelFailUnitId != null && uc.DelFailUnitId.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.DelFailUnitId.Count; ucIndex < ucCount; ucIndex++)
                {
                    if (type == 1)
                    {
                        if (InterceptBehave(uc.DelFailUnitId[ucIndex], forceExcute) < 0)
                            return false;
                    }
                    else if (type == 2 && func != null)
                    {
                        if (func(uc.DelFailUnitId[ucIndex]))
                            return false;
                    }
                }
            }
            //增加新的单位
            if (uc.NewUnits != null && uc.NewUnits.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.NewUnits.Count; ucIndex < ucCount; ucIndex++)
                {
                    var battleUnit = uc.NewUnits[ucIndex];
                    if (battleUnit == null)
                        continue;

                    //_callingTargetBattleUnitTempList.Add(battleUnit);

                    if (type == 1)
                    {
                        MobEntity posMob = MobManager.Instance.GetMobByServerNum(battleUnit.Pos);
                        if (posMob != null && InterceptMobBehave(posMob, forceExcute) < 0)
                            return false;
                    }
                    else if (type == 2)
                    {
                        if (func != null && func(battleUnit.UnitId))
                            return false;
                    }
                }
            }
            //逃跑成功
            if (uc.EscapeUnitId != null && uc.EscapeUnitId.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.EscapeUnitId.Count; ucIndex < ucCount; ucIndex++)
                {
                    if (type == 1)
                    {
                        if (InterceptBehave(uc.EscapeUnitId[ucIndex], forceExcute) < 0)
                            return false;
                    }
                    else if (type == 2 && func != null)
                    {
                        if (func(uc.EscapeUnitId[ucIndex]))
                            return false;
                    }
                }
            }
            //逃跑失败
            if (uc.EscapeFailUnitId != null && uc.EscapeFailUnitId.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.EscapeFailUnitId.Count; ucIndex < ucCount; ucIndex++)
                {
                    if (type == 1)
                    {
                        if (InterceptBehave(uc.EscapeFailUnitId[ucIndex], forceExcute) < 0)
                            return false;
                    }
                    else if (type == 2 && func != null)
                    {
                        if (func(uc.EscapeFailUnitId[ucIndex]))
                            return false;
                    }
                }
            }
            //换位
            if (uc.UnitPosChange != null && uc.UnitPosChange.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.UnitPosChange.Count; ucIndex < ucCount; ucIndex++)
                {
                    if (type == 1)
                    {
                        var unitPosChange = uc.UnitPosChange[ucIndex];
                        if (unitPosChange == null)
                            continue;

                        if (InterceptBehave(unitPosChange.UnitId, forceExcute) < 0)
                            return false;
                    }
                    else if (type == 2 && func != null)
                    {
                        if (func(uc.UnitPosChange[ucIndex].UnitId))
                            return false;
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// "handleType"=0处理可以生成CombatHpChangeData的ExcuteData，=1处理被动
    /// </summary>
    /// <param name="handleType">=0处理layer=1主动技能，=1处理波数内的被动, =2处理波数外的被动</param>
    public void HandleExcuteData(ExcuteTurn excuteTurn, int excuteTurnIndex, ExcuteData ed, int edIndex, NetExcuteTurnInfo netExcuteTurnInfo,
        AttackBoLifeCycleInfo tempAttacBokLifeCycle, SNodeInfo tempTTi, uint srcUnitId, uint extendType, uint extendId, 
        bool isNeedAddTarget, CSVActiveSkill.Data skillTb, int handleType, ref bool isHaveBehave, ref bool isAttackSkillSuccess)
    {
        var hc = ed.HpChange;
        if (hc != null)
        {
            if (extendType == 3 && hc.HitEffect == 14)
            {
                DLogManager.Log(ELogType.eCombat, $"屏蔽数据------<color=yellow>HpChange屏蔽掉extendType:{extendType.ToString()}   hc.HitEffect:{hc.HitEffect.ToString()}的数据</color>");
                return;
            }

            if (hc.HitEffect == 6)
            {
                if (handleType == 0 && hc.ProtectTar > 0u)
                {
                    DLogManager.Log(ELogType.eCombat, $"护卫数据------<color=yellow>srcUnitId:{srcUnitId.ToString()}    hc.UnitId:{hc.UnitId.ToString()}   extendType:{extendType.ToString()}  extendId:{extendId.ToString()}   hc.HitEffect:{hc.HitEffect.ToString()}   hc.ProtectTar:{hc.ProtectTar.ToString()}的数据</color>");

                    _protectHpMpChangeTemp.Enqueue(hc);
                }
                return;
            }
            
#if DEBUG_MODE
            _tempSb.Clear();
            _tempSb.Append($"HpChange[{edIndex.ToString()}]-----<color=magenta>HpChange的hc.HitEffect:{hc.HitEffect.ToString()}   extendId:{extendId.ToString()}</color>");
#endif

            uint hitEffect_32_16 = 0u;
            if (hc.HitEffect > 255u)
            {
                hitEffect_32_16 = hc.HitEffect >> 16;
                hc.HitEffect = hc.HitEffect - (hitEffect_32_16 << 16);
#if DEBUG_MODE
                _tempSb.Append($"<color=magenta>    解析后hitEffect_32_16:{hitEffect_32_16.ToString()}   hc.HitEffect:{hc.HitEffect.ToString()}</color>");
#endif
            }

            bool isHaveHpMpChange = false;

            uint buffSkillId = 0u;
            uint buffEffectType = 0u;
            if (handleType == 0 && extendType == 2 && extendId > 0)
            {
                var buffTb = CSVBuff.Instance.GetConfData(extendId);
                if (buffTb != null && buffTb.hp_skill_id > 0u)
                {
                    buffSkillId = buffTb.hp_skill_id;
                    isNeedAddTarget = true;
                    buffEffectType = buffTb.effect_buff;
                }
                else
                    DebugUtil.LogError($"buff引起的Hpchange----buffId:{extendId.ToString()}   buffTb.hp_skill_id:{(buffTb == null ? "0u" : buffTb.hp_skill_id.ToString())}   extendType:{extendType.ToString()}没有buff表没有行为数据</color>");
            }

            if (hc.HitEffect < 6 || hc.HitEffect == 7 ||
                hc.HitEffect == 9 || hc.HitEffect == 10 || hc.HitEffect == 12 || hc.HitEffect == 14 ||
                hc.HitEffect == 15 || hc.HitEffect == 16)
            {
#if DEBUG_MODE
                _tempSb.Append($"   <color=yellow>HpChange的UnitId : {hc.UnitId.ToString()}</color>");
#endif

                MobEntity target = MobManager.Instance.GetMob(hc.UnitId);
                if (target != null)
                {
                    if (target.m_MobCombatComponent.m_Death && hc.HpChange < 0)
                    {
#if DEBUG_MODE
                        if (m_IsReconnect)
                            _tempSb.Append($"   <color=red>ClientNum:{target.m_MobCombatComponent.m_ClientNum.ToString()}已经Death:{target.m_MobCombatComponent.m_Death.ToString()},但收到：HpChange:{hc.HpChange.ToString()}");
                        else if (handleType == 0)
                            DebugUtil.LogError($"ClientNum:{target.m_MobCombatComponent.m_ClientNum.ToString()}已经Death:{target.m_MobCombatComponent.m_Death.ToString()},但收到：HpChange:{hc.HpChange.ToString()}");
#endif
                        if (handleType == 0 && !m_IsReconnect)
                            return;
                    }

                    isHaveHpMpChange = true;

                    if (handleType == 0 && extendType == 2 && extendId > 0)
                    {
                        if (buffSkillId == 0u)
                        {
                            isHaveHpMpChange = false;
                            if (hc.ChangeType == 0 || hc.ChangeType == 2)
                            {
                                if (hc.HitEffect != 1 && hc.HitEffect != 3)
                                {
                                    target.m_MobCombatComponent.m_BattleUnit.CurHp = hc.CurHp;
                                    target.m_MobCombatComponent.m_Death = hc.CurHp <= 0;
                                }
                            }

                            if (hc.ChangeType == 1 || hc.ChangeType == 2)
                            {
                                target.m_MobCombatComponent.m_BattleUnit.CurMp = hc.CurMp;
                            }

                            DebugUtil.LogError($"buff引起的Hpchange----buffId:{extendId.ToString()} {(target.m_Go == null ? null : target.m_Go.name)} Client:{target.m_MobCombatComponent.m_ClientNum.ToString()}buff表没有行为数据</color>");
                        }
                    }
                    else if (handleType != 0)
                        isHaveHpMpChange = false;

                    if (isHaveHpMpChange)
                    {
                        DoCombatHpChangeDataState(target);

                        bool isConverAttack = false;
                        if (excuteTurn.SrcUnit.Count > 1)
                        {
                            for (int srcIndex = 0, srcCount = excuteTurn.SrcUnit.Count; srcIndex < srcCount; srcIndex++)
                            {
                                if (excuteTurn.SrcUnit[srcIndex] == srcUnitId)
                                {
                                    isConverAttack = true;

                                    break;
                                }
                            }
                        }

                        var hpChangeData = CombatObjectPool.Instance.Get<CombatHpChangeData>();
                        hpChangeData.m_ChangeType = hc.ChangeType;
                        if (hc.ChangeType == 0 || hc.ChangeType == 2 || hc.ChangeType == 4)
                        {
                            hpChangeData.m_Revive = target.m_MobCombatComponent.m_Death && (hc.HpChange > 0 || hc.CurHp > 0);
                            if (hc.HitEffect != 1 && hc.HitEffect != 3)
                            {
                                target.m_MobCombatComponent.m_BattleUnit.CurHp = hpChangeData.m_CurHp = hc.CurHp;
                                target.m_MobCombatComponent.m_Death = hpChangeData.m_Death = hc.CurHp <= 0;
                            }
                            else
                            {
                                hpChangeData.m_CurHp = target.m_MobCombatComponent.m_BattleUnit.CurHp;
                                hpChangeData.m_Death = target.m_MobCombatComponent.m_Death;
                            }

                            hpChangeData.m_CurMp = target.m_MobCombatComponent.m_BattleUnit.CurMp;
                            hpChangeData.m_HpChange = hc.HpChange;
                            hpChangeData.m_damageAddon = hc.DamageAddon;
                            hpChangeData.m_ConverAttackCount = 0;
                            AnimType animType = AnimType.e_None;
                            if ((hc.HitEffect == 0 || hc.HitEffect == 9 || hc.HitEffect == 10) && hc.HpChange >= 0)
                                animType |= AnimType.e_AddHp;
                            else
                            {
                                if (isConverAttack)
                                {
                                    animType |= AnimType.e_ConverAttack;
                                    hpChangeData.m_ConverAttackCount = excuteTurn.SrcUnit.Count;
                                }

                                if (_attackCount > 1)
                                {
                                    animType |= AnimType.e_Combo;
                                }

                                if (hc.HitEffect == 0)
                                    animType |= AnimType.e_Normal;
                                else if (hc.HitEffect == 2)
                                    animType |= AnimType.e_Crit;
                                else if (hc.HitEffect == 1)
                                {
                                    if (skillTb != null && skillTb.skill_type == 1)
                                        animType |= AnimType.e_Error;
                                    else
                                        animType |= AnimType.e_Miss;
                                }
                                else if (hc.HitEffect == 4 || hc.HitEffect == 7)
                                {
                                    animType |= AnimType.e_PassiveDamage;
                                }
                                else if (hc.HitEffect == 14)
                                {
                                    animType |= AnimType.e_Invalid;
                                }
                                else
                                    animType |= AnimType.e_Normal;
                            }

                            hpChangeData.m_AnimType = animType;

                            target.m_MobCombatComponent.SetCombatUnitState(target.m_MobCombatComponent.m_Death ? 4 : -1);
                        }

                        if (hc.ChangeType == 1 || hc.ChangeType == 2)
                        {
                            target.m_MobCombatComponent.m_BattleUnit.CurMp = hpChangeData.m_CurMp = hc.CurMp;

                            if (buffEffectType == 8u)
                                hpChangeData.m_AnimType |= AnimType.e_Drunk;
                            else if (hc.MpChange > 0)
                                hpChangeData.m_AnimType |= AnimType.e_AddMp;
                            else
                                hpChangeData.m_AnimType |= AnimType.e_DeductMp;

                            hpChangeData.m_CurHp = target.m_MobCombatComponent.m_BattleUnit.CurHp;
                            hpChangeData.m_MpChange = hc.MpChange;
                        }

                        if (hc.ChangeType == 3 || hc.ChangeType == 4)
                        {
                            target.m_MobCombatComponent.m_BattleUnit.CurShield = hpChangeData.m_CurShield = hc.CurShield;

                            hpChangeData.m_AnimType |= AnimType.e_Normal;

                            hpChangeData.m_CurHp = target.m_MobCombatComponent.m_BattleUnit.CurHp;
                            hpChangeData.m_ShieldChange = hc.ShieldChange;
                        }

                        if (hc.ChangeType == 5)
                        {
                            target.m_MobCombatComponent.m_BattleUnit.CurGas = hpChangeData.m_CurGas = hc.CurGas;

                            hpChangeData.m_AnimType |= AnimType.e_Normal;

                            hpChangeData.m_CurHp = target.m_MobCombatComponent.m_BattleUnit.CurHp;
                            hpChangeData.m_GasChange = hc.GasChange;
                        }

                        hpChangeData.m_ExtendId = extendId;
                        hpChangeData.m_ExtendType = extendType;
                        hpChangeData.m_BuffSkillId = buffSkillId;
                        hpChangeData.m_ExcuteDataIndex = edIndex;
                        hpChangeData.m_ExtraHitEffect = hitEffect_32_16;
                        hpChangeData.m_SrcUnitId = srcUnitId;

                        target.m_MobCombatComponent.m_HpChangeDataQueue.Enqueue(hpChangeData);

#if DEBUG_MODE
                        _tempSb.Append($"   被击信息----<color=yellow>被击位子{target.m_MobCombatComponent.m_ClientNum.ToString()}状态</color><color=red>{hpChangeData.m_AnimType.ToString()}；</color><color=yellow>BattleId:{CombatManager.Instance.m_BattleId.ToString()}   CurRound:{m_CurRound.ToString()}   HitEffect:{hc.HitEffect.ToString()}   Uid:{target.m_MobCombatComponent.m_BattleUnit.UnitId.ToString()}  ServerPos:{target.m_MobCombatComponent.m_BattleUnit.Pos.ToString()}   HpChange:{hc.HpChange.ToString()}  DamageAddon:{hc.DamageAddon}  CurHp:{hc.CurHp.ToString()}  MaxHp:{target.m_MobCombatComponent.m_BattleUnit.MaxHp.ToString()}  MpChange:{hc.MpChange.ToString()}   CurMp:{hc.CurMp.ToString()}   MaxMp:{target.m_MobCombatComponent.m_BattleUnit.MaxMp.ToString()}  ChangeType:{hc.ChangeType.ToString()}   Death:{target.m_MobCombatComponent.m_Death.ToString()}   m_Revive:{hpChangeData.m_Revive.ToString()}    buffSkillId:{buffSkillId.ToString()}</color>");
#endif

                        if (isNeedAddTarget)
                        {
                            _recordTurnInfoTemp.Clear();
                            _recordTurnInfoTemp.SrcUnitId = srcUnitId;
                            _recordTurnInfoTemp.BuffSkillId = buffSkillId;
                            _recordTurnInfoTemp.HitEffect = hc.HitEffect;
                            _recordTurnInfoTemp.ExtraHitEffect = hitEffect_32_16;
                            _recordTurnInfoTemp.IsConverAttack = isConverAttack;
                            _recordTurnInfoTemp.ExtendType = extendType;
                            _recordTurnInfoTemp.ExtendId = extendId;
                            _recordTurnInfoTemp.IsHaveHpMpChange = true;
                            _recordTurnInfoTemp.ExcuteDataIndex = edIndex;

                            SetTurnBehaveInfos(excuteTurn, netExcuteTurnInfo, tempAttacBokLifeCycle, tempTTi, hc.UnitId, _recordTurnInfoTemp, handleType);
                        }
                    }

                    //if (buffSkillId > 0u)
                    //{
                    //    //注---如果该target有一群伤害，且该buff伤害在中间执行，就会有飘字顺序不正确
                    //    BehaveAIControllParam behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
                    //    behaveAIControllParam.SrcUnitId = hc.UnitId;
                    //    behaveAIControllParam.SkillId = buffSkillId;
                    //    behaveAIControllParam.ExcuteTurnIndex = excuteTurnIndex;
                    //    if (target.m_MobCombatComponent.StartBehave(buffSkillId, null, false, behaveAIControllParam))
                    //    {
                    //        isHaveBehave = true;
                    //    }
                    //}

                    #region 模拟器需要的记录
#if UNITY_EDITOR_NO_USE
                    var beHitCssi = CombatSimulateManager.Instance.GetCombatSimulateStatisticsInfo(target.m_MobCombatComponent.m_BattleUnit.Pos);
                    beHitCssi.BeDmg += (long)((hc.HpChange < 0 ? hc.HpChange : 0) + hc.DamageAddon);
                    for (int simulateIndex = 0; simulateIndex < excuteTurn.SrcUnit.Count; simulateIndex++)
                    {
                        MobEntity attack = MobManager.Instance.GetMob(excuteTurn.SrcUnit[simulateIndex]);
                        if (attack == null)
                            continue;

                        var hitCssi = CombatSimulateManager.Instance.GetCombatSimulateStatisticsInfo(attack.m_MobCombatComponent.m_BattleUnit.Pos);
                        hitCssi.Dmg += (long)((hc.HpChange < 0 ? hc.HpChange : 0) + hc.DamageAddon);
                        hitCssi.KillNum += target.m_MobCombatComponent.m_Death ? 1 : 0;
                        hitCssi.AddHp += (long)(hc.HpChange > 0 ? hc.HpChange : 0);
                    }
#endif
                    #endregion
                }
                else if (handleType == 0)
                {
                    netExcuteTurnInfo.CacheToNetInfoForNoMobData(hc.UnitId, excuteTurn, excuteTurnIndex, ed, edIndex,
                            tempAttacBokLifeCycle, tempTTi, srcUnitId, extendType, extendId, skillTb);
                }
            }
            else if (hc.HitEffect == (uint)HitEffectType.Mana || hc.HitEffect == (uint)HitEffectType.Energe)
            {
                isAttackSkillSuccess = false;

                if (handleType == 0 && extendType == 3)
                {
                    _recordTurnInfoTemp.Clear();
                    _recordTurnInfoTemp.SrcUnitId = srcUnitId;
                    _recordTurnInfoTemp.BuffSkillId = buffSkillId;
                    _recordTurnInfoTemp.HitEffect = hc.HitEffect;
                    _recordTurnInfoTemp.ExtraHitEffect = hitEffect_32_16;
                    _recordTurnInfoTemp.ExtendType = extendType;
                    _recordTurnInfoTemp.ExtendId = extendId;
                    _recordTurnInfoTemp.IsHaveHpMpChange = isHaveHpMpChange;
                    _recordTurnInfoTemp.ExcuteDataIndex = edIndex;

                    SetTurnBehaveInfos(excuteTurn, netExcuteTurnInfo, tempAttacBokLifeCycle, tempTTi, hc.UnitId, _recordTurnInfoTemp, handleType);
                }
#if DEBUG_MODE
                _tempSb.Append($"   Mana|Energe不足----<color=magenta>HpChange的UnitId : {hc.UnitId.ToString()}     HitEffect:{hc.HitEffect}</color>");
#endif
            }

            if (handleType != 0 && isNeedAddTarget)
            {
                _recordTurnInfoTemp.Clear();
                _recordTurnInfoTemp.SrcUnitId = srcUnitId;
                _recordTurnInfoTemp.BuffSkillId = buffSkillId;
                _recordTurnInfoTemp.HitEffect = hc.HitEffect;
                _recordTurnInfoTemp.ExtraHitEffect = hitEffect_32_16;
                _recordTurnInfoTemp.ExtendType = extendType;
                _recordTurnInfoTemp.ExtendId = extendId;
                _recordTurnInfoTemp.IsHaveHpMpChange = isHaveHpMpChange;
                _recordTurnInfoTemp.ExcuteDataIndex = edIndex;

                SetTurnBehaveInfos(excuteTurn, netExcuteTurnInfo, tempAttacBokLifeCycle, tempTTi, hc.UnitId, _recordTurnInfoTemp, handleType);
            }

#if DEBUG_MODE
            DLogManager.Log(ELogType.eCombat, $"{_tempSb.ToString()}");
#endif
        }

        var bc = ed.BuffChange;
        if (bc != null && handleType == 0)
        {
            var buffTb = CSVBuff.Instance.GetConfData(bc.BuffId);

            bool isNeedCacheToNetBuff = false;
            bool isAfterActive = false;
            if (bc.FailAdd == 0u)
            {
                if (tempTTi == null || tempAttacBokLifeCycle.IsOutBoTempFlag)
                {
                    DLogManager.Log(ELogType.eCombat, $"buff待触发记录-----在bo外OutBoInfo的数据   bo:{tempAttacBokLifeCycle.m_OutBoInfo.Bo.ToString()}   Stage:{tempAttacBokLifeCycle.m_OutBoInfo.Stage.ToString()}----<color=yellow>UnitId : {bc.UnitId.ToString()}  BuffId : {bc.BuffId.ToString()}  Num : {bc.OddNum.ToString()}   bc.CurHp:{bc.CurHp.ToString()}  bc.MaxHp:{bc.MaxHp.ToString()}  bc.CurMp:{bc.CurMp.ToString()}  bc.MaxMp:{bc.MaxMp.ToString()}</color>");

                    if (tempAttacBokLifeCycle.m_OutBoInfo.Bo < 1)
                    {
                        if (tempAttacBokLifeCycle.m_OutBoInfo.m_BuffChangeDataBeforeQueue == null)
                            tempAttacBokLifeCycle.m_OutBoInfo.m_BuffChangeDataBeforeQueue = new Queue<uint>();

                        tempAttacBokLifeCycle.m_OutBoInfo.m_BuffChangeDataBeforeQueue.Enqueue(bc.UnitId);
                    }
                    else
                    {
                        if (tempAttacBokLifeCycle.m_OutBoInfo.m_BuffChangeDataAfterQueue == null)
                            tempAttacBokLifeCycle.m_OutBoInfo.m_BuffChangeDataAfterQueue = new Queue<uint>();

                        tempAttacBokLifeCycle.m_OutBoInfo.m_BuffChangeDataAfterQueue.Enqueue(bc.UnitId);
                    }
                }
                else
                {
                    if (buffTb != null && buffTb.clear_trigger > 0u)
                    {
                        tempTTi.SetBuffChangeDataByBuffTiming(bc.UnitId, (int)buffTb.clear_trigger);
                    }
                    else if (tempTTi.IsAfterActive)
                    {
                        tempTTi.SetBuffChangeDataByBuffTiming(bc.UnitId, 0);
                    }
                    else
                    {
                        tempTTi.SetBuffChangeDataByBuffTiming(bc.UnitId, -1);
                    }

                    DLogManager.Log(ELogType.eCombat, $"buff待触发记录-----在SNodeInfo的数据SNodeId:{tempTTi.SNodeId.ToString()}   Layer:{tempTTi.Layer.ToString()}  bo:{tempTTi.Bo.ToString()}   Stage:{tempAttacBokLifeCycle.m_OutBoInfo.Stage.ToString()}----<color=yellow>tempTTi.IsAfterActive:{tempTTi.IsAfterActive.ToString()}  UnitId : {bc.UnitId.ToString()}  BuffId : {bc.BuffId.ToString()}  Num : {bc.OddNum.ToString()}   bc.CurHp:{bc.CurHp.ToString()}  bc.MaxHp:{bc.MaxHp.ToString()}  bc.CurMp:{bc.CurMp.ToString()}  bc.MaxMp:{bc.MaxMp.ToString()}</color>");

                    isAfterActive = tempTTi.IsAfterActive;
                }

                var buffMob = MobManager.Instance.GetMob(bc.UnitId);
                if (buffMob != null)
                {
                    DLogManager.Log(ELogType.eCombat, $"buff---{(buffMob.m_Go == null ? null : buffMob.m_Go.name)}-----缓存buff CacheProcessBuffChange处理----<color=yellow>isAfterActive:{isAfterActive}   UnitId : {bc.UnitId.ToString()}  BuffId : {bc.BuffId.ToString()}  Num : {bc.OddNum.ToString()}   bc.CurHp:{bc.CurHp.ToString()}  bc.MaxHp:{bc.MaxHp.ToString()}  bc.CurMp:{bc.CurMp.ToString()}  bc.MaxMp:{bc.MaxMp.ToString()}</color>");

                    buffMob.CacheProcessBuffChange(bc);
                }
                else
                {
                    isNeedCacheToNetBuff = true;
                }
            }

            if (CheckBehaveBuff(excuteTurn, bc))
            {
                if (buffTb != null && buffTb.skill_id > 0u)
                {
                    MobEntity target = MobManager.Instance.GetMob(bc.UnitId);
                    if (target != null)
                    {
                        if (!IsExistTurnBehaveTarget(netExcuteTurnInfo.TurnBehaveInfoList, bc.UnitId))
                        {
                            DoCombatHpChangeDataState(target);

                            var bcChangeData = CombatObjectPool.Instance.Get<CombatHpChangeData>();
                            bcChangeData.m_CurHp = target.m_MobCombatComponent.m_BattleUnit.CurHp;
                            bcChangeData.m_CurMp = target.m_MobCombatComponent.m_BattleUnit.CurMp;
                            bcChangeData.m_Death = target.m_MobCombatComponent.m_Death;
                            bcChangeData.m_ConverAttackCount = 0;
                            bcChangeData.m_AnimType = AnimType.e_Normal;

                            bcChangeData.m_ExtendId = extendId;
                            bcChangeData.m_ExtendType = extendType;
                            bcChangeData.m_BuffSkillId = buffTb.skill_id;
                            bcChangeData.m_ExcuteDataIndex = edIndex;
                            bcChangeData.m_SrcUnitId = srcUnitId;

                            target.m_MobCombatComponent.m_HpChangeDataQueue.Enqueue(bcChangeData);

                            DLogManager.Log(ELogType.eCombat, $"有行为buff[{edIndex.ToString()}]生成HpChangeData加入Queue----<color=yellow>extendType:{extendType.ToString()}  目标UnitId:{bc.UnitId.ToString()}, 被击位子{target.m_MobCombatComponent.m_ClientNum.ToString()}， buffId:{bc.BuffId.ToString()}</color>");

                            if (extendType == 0)
                            {
                                if (tempTTi != null)
                                    tempTTi.IsAfterActive = true;

                                _recordTurnInfoTemp.Clear();
                                _recordTurnInfoTemp.SrcUnitId = srcUnitId;
                                _recordTurnInfoTemp.ExtendType = extendType;
                                _recordTurnInfoTemp.ExtendId = extendId;
                                _recordTurnInfoTemp.IsHaveHpMpChange = true;
                                _recordTurnInfoTemp.ExcuteDataIndex = edIndex;

                                SetTurnBehaveInfos(excuteTurn, netExcuteTurnInfo, tempAttacBokLifeCycle, tempTTi, bc.UnitId, _recordTurnInfoTemp, handleType);
                            }
                            else
                            {
                                //注---如果该target有一群伤害，且该buff伤害在中间执行，就会有飘字顺序不正确
                                BehaveAIControllParam behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
                                behaveAIControllParam.SrcUnitId = bc.UnitId;
                                behaveAIControllParam.SkillId = buffTb.skill_id;
                                behaveAIControllParam.ExcuteTurnIndex = excuteTurnIndex;
                                if (target.m_MobCombatComponent.StartBehave(buffTb.skill_id, null, false, behaveAIControllParam))
                                {
                                    isHaveBehave = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        isNeedCacheToNetBuff = false;

                        netExcuteTurnInfo.CacheToNetInfoForNoMobData(bc.UnitId, excuteTurn, excuteTurnIndex, ed, edIndex,
                            tempAttacBokLifeCycle, tempTTi, srcUnitId, extendType, extendId, skillTb);
                    }
                }
            }

            if (isNeedCacheToNetBuff)
            {
                DLogManager.Log(ELogType.eCombat, $"buff---还没生成Mob-----CacheToNetBuff处理----<color=yellow>isAfterActive:{isAfterActive}   UnitId : {bc.UnitId.ToString()}  BuffId : {bc.BuffId.ToString()}  Num : {bc.OddNum.ToString()}   bc.CurHp:{bc.CurHp.ToString()}  bc.MaxHp:{bc.MaxHp.ToString()}  bc.CurMp:{bc.CurMp.ToString()}  bc.MaxMp:{bc.MaxMp.ToString()}</color>");

                netExcuteTurnInfo.CacheToNetBuff(bc, isAfterActive);
            }
        }

        var uc = ed.UnitsChange;
        if (uc != null)
        {
            if (uc.DelUnitId != null && uc.DelUnitId.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.DelUnitId.Count; ucIndex < ucCount; ucIndex++)
                {
                    SetUnitsChangeTarget(excuteTurn, netExcuteTurnInfo, edIndex, srcUnitId, extendType, extendId, tempAttacBokLifeCycle, 
                        tempTTi, uc.DelUnitId[ucIndex], null, 0u, 0, uc.ReplaceType, handleType);
                }
            }
            if (uc.DelFailUnitId != null && uc.DelFailUnitId.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.DelFailUnitId.Count; ucIndex < ucCount; ucIndex++)
                {
                    SetUnitsChangeTarget(excuteTurn, netExcuteTurnInfo, edIndex, srcUnitId, extendType, extendId, tempAttacBokLifeCycle, 
                        tempTTi, uc.DelFailUnitId[ucIndex], null, 0u, 1, uc.ReplaceType, handleType);
                }
            }
            if (uc.NewUnits != null && uc.NewUnits.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.NewUnits.Count; ucIndex < ucCount; ucIndex++)
                {
                    var battleUnit = uc.NewUnits[ucIndex];
                    if (battleUnit == null)
                        continue;

                    if (handleType != 1)
                    {
                        CacheDelayBirthNetNewUnitData cacheDelayBirthNetNewUnitData = BasePoolClass.Get<CacheDelayBirthNetNewUnitData>();
                        cacheDelayBirthNetNewUnitData.m_BattleUnitChange = uc;
                        cacheDelayBirthNetNewUnitData.m_BattleUnit = battleUnit;
                        cacheDelayBirthNetNewUnitData.m_ExcuteTurnIndex = excuteTurnIndex;
                        _cacheDelayBirthNetNewUnitDatas.Add(cacheDelayBirthNetNewUnitData);
                    }
                    
                    SetUnitsChangeTarget(excuteTurn, netExcuteTurnInfo, edIndex, srcUnitId, extendType, extendId, tempAttacBokLifeCycle, 
                        tempTTi, 0u, battleUnit, 0u, 6, uc.ReplaceType, handleType);
                }
            }
            if (uc.EscapeUnitId != null && uc.EscapeUnitId.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.EscapeUnitId.Count; ucIndex < ucCount; ucIndex++)
                {
                    SetUnitsChangeTarget(excuteTurn, netExcuteTurnInfo, edIndex, srcUnitId, extendType, extendId, tempAttacBokLifeCycle, 
                        tempTTi, uc.EscapeUnitId[ucIndex], null, 0u, 2, uc.ReplaceType, handleType);
                }
            }
            if (uc.EscapeFailUnitId != null && uc.EscapeFailUnitId.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.EscapeFailUnitId.Count; ucIndex < ucCount; ucIndex++)
                {
                    SetUnitsChangeTarget(excuteTurn, netExcuteTurnInfo, edIndex, srcUnitId, extendType, extendId, tempAttacBokLifeCycle, 
                        tempTTi, uc.EscapeFailUnitId[ucIndex], null, 0u, 3, uc.ReplaceType, handleType);
                }
            }
            if (uc.UnitPosChange != null && uc.UnitPosChange.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.UnitPosChange.Count; ucIndex < ucCount; ucIndex++)
                {
                    var unitPosChange = uc.UnitPosChange[ucIndex];
                    if (unitPosChange == null)
                        continue;

                    SetUnitsChangeTarget(excuteTurn, netExcuteTurnInfo, edIndex, srcUnitId, extendType, extendId, tempAttacBokLifeCycle, 
                        tempTTi, unitPosChange.UnitId, null, unitPosChange.Pos, 7, uc.ReplaceType, handleType);
                }
            }
        }

        var bub = ed.UnitBase;
        if (bub != null && handleType == 0)
        {
            if (bub.UnitId != 0u)
            {
                var bubMob = MobManager.Instance.GetMob(bub.UnitId);
                if (bubMob != null && bubMob.m_MobCombatComponent != null &&
                    bubMob.m_MobCombatComponent.m_BattleUnit != null)
                {
                    bubMob.m_MobCombatComponent.m_BattleUnit.Race = bub.Race;
                    bubMob.m_MobCombatComponent.m_BattleUnit.ShapeShiftId = bub.ShapeShiftId;

                    ShapeShiftChangedEvt svce = CombatObjectPool.Instance.Get<ShapeShiftChangedEvt>();
                    svce.id = bub.UnitId;
                    svce.ShapeShiftId = bub.ShapeShiftId;
                    Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateShapeShift, svce);
                    CombatObjectPool.Instance.Push(svce);
                }
            }
        }

        //extendType == 6u是服务器发送的没有ExcuteData数据但要有表现行为的类型
        if (extendType == 6u && handleType == 0)
        {
            MobEntity attack = MobManager.Instance.GetMob(srcUnitId);
            if (attack != null)
            {
                DebugUtil.Log(ELogType.eCombat, $"直接执行行为---【没有ExcuteData数据但要有表现行为】---<color=yellow>extendType:{extendType.ToString()}   srcUnitId:{srcUnitId.ToString()}  excuteTurnIndex:{excuteTurnIndex.ToString()}  extendId:{extendId.ToString()}</color>");

                uint skillId = extendId > 0u ? extendId / 10u : 0u;

                BehaveAIControllParam behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
                behaveAIControllParam.SrcUnitId = srcUnitId;
                behaveAIControllParam.SkillId = skillId;
                behaveAIControllParam.ExcuteTurnIndex = excuteTurnIndex;
                attack.m_MobCombatComponent.StartBehave(skillId, null, false, behaveAIControllParam);
            }
        }
    }

    public void PerformBehave(List<TurnBehaveInfo> turnBehaveInfoList, int excuteTurnIndex, bool isAttackSkillSuccess, 
        bool isSetRoundBattleCount, int startSkillExtendType, ref bool isHaveBehave)
    {
        if (turnBehaveInfoList == null)
            return;

        for (int i = 0, count = turnBehaveInfoList.Count; i < count; i++)
        {
            TurnBehaveInfo turnBehaveInfo = turnBehaveInfoList[i];
            if (turnBehaveInfo == null || turnBehaveInfo.SrcUnitId == 0u || turnBehaveInfo.TurnBehaveSkillInfoList == null)
                continue;

            MobEntity attack = MobManager.Instance.GetMob(turnBehaveInfo.SrcUnitId);
            if (attack == null)
                continue;

            for (int tbSkillIndex = 0, tbSkillCount = turnBehaveInfo.TurnBehaveSkillInfoList.Count; tbSkillIndex < tbSkillCount; tbSkillIndex++)
            {
                TurnBehaveSkillInfo turnBehaveSkillInfo = turnBehaveInfo.TurnBehaveSkillInfoList[tbSkillIndex];

                PerformBehave(turnBehaveSkillInfo, attack, excuteTurnIndex, isAttackSkillSuccess, 
                        isSetRoundBattleCount, startSkillExtendType, ref isHaveBehave);
            }
        }
    }

    private bool PerformBehave(TurnBehaveSkillInfo turnBehaveSkillInfo, MobEntity attack,
        int excuteTurnIndex, bool isAttackSkillSuccess, bool isSetRoundBattleCount, 
        int startSkillExtendType, ref bool isHaveBehave, StartControllerStyleEnum startControllerStyleEnum = StartControllerStyleEnum.None)
    {
        if (turnBehaveSkillInfo == null || turnBehaveSkillInfo.SkillId == 0u ||
            turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList == null ||
            attack == null)
            return false;

        if (isAttackSkillSuccess)
        {
            InformIsAttackSkillSuccess(attack.m_MobCombatComponent.m_BattleUnit, turnBehaveSkillInfo.SkillId);
        }

        if (startSkillExtendType > -1 && turnBehaveSkillInfo.ExtendType != (uint)startSkillExtendType)
            return false;

        CSVActiveSkill.Data activeSkillTb = CSVActiveSkill.Instance.GetConfData(turnBehaveSkillInfo.SkillId);
        if (activeSkillTb == null)
        {
            DebugUtil.LogError($"进行StartBehave时使用的技能Id：{turnBehaveSkillInfo.SkillId.ToString()}在表CSVActiveSkill中没有数据");
            return false;
        }

        if (activeSkillTb.have_behavior == 0u && turnBehaveSkillInfo.TargetUnitCount == 0)
        {
            DebugUtil.Log(ELogType.eCombat, $"<color=red>在技能执行的目标不是场上现有战斗单位的情况下，主动技能表的字段have_behavior值为{activeSkillTb.have_behavior.ToString()}，不会去执行表现</color>");
            return false;
        }

#if DEBUG_MODE
        _tempSb.Clear();
        _tempSb.Append($"攻击者位置：{attack.m_MobCombatComponent.m_ClientNum.ToString()}[{attack.m_MobCombatComponent.m_BattleUnit.UnitId}]-----目标位置：");
        foreach (var targetInfo in turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList)
        {
            if (targetInfo.TargetUnitId == 0u && targetInfo.CallingTargetBattleUnit == null)
                continue;

            uint targetUnitId = targetInfo.TargetUnitId > 0u ? targetInfo.TargetUnitId : targetInfo.CallingTargetBattleUnit.UnitId;

            MobEntity t = MobManager.Instance.GetMob(targetUnitId);
            if (t != null)
                _tempSb.Append($"{t.m_MobCombatComponent.m_ClientNum.ToString()}[{t.m_MobCombatComponent.m_BattleUnit.UnitId}]，");
            else
                _tempSb.Append($"UnitId:{targetUnitId}，");
        }
        DLogManager.Log(ELogType.eCombat, $"行为描述----<color=yellow>skillId:{turnBehaveSkillInfo.SkillId.ToString()}   have_behavior:{activeSkillTb.have_behavior.ToString()} {_tempSb.ToString()}</color>");
        _tempSb.Clear();
#endif
        if (isSetRoundBattleCount)
            attack.m_MobCombatComponent.SetRoundBattleCount(m_CurRound);

        #region 处理行为前的buff处理
        DoBuffChangeData(excuteTurnIndex, turnBehaveSkillInfo, -1);
        #endregion

        BehaveAIControllParam behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
        behaveAIControllParam.SrcUnitId = attack.m_MobCombatComponent.m_BattleUnit.UnitId;
        behaveAIControllParam.SkillId = turnBehaveSkillInfo.SkillId;
        behaveAIControllParam.ExcuteTurnIndex = excuteTurnIndex;
        behaveAIControllParam.m_StartControllerStyleEnum = startControllerStyleEnum;
        if (attack.m_MobCombatComponent.StartBehave(turnBehaveSkillInfo.SkillId, turnBehaveSkillInfo,
                turnBehaveSkillInfo.IsConverAttack, behaveAIControllParam))
        {
            isHaveBehave = true;
            return true;
        }

        return false;
    }

    private void PerformOutRound(uint stage, int excuteTurnIndex, OutBoInfo outBoInfo, ref bool isHaveBehave)
    {
        if (stage == 1u || stage == 2u || stage == 3u)
        {
            WS_CombatSceneAIControllerEntity sController = null;
            if (stage == 1u)
            {
                sController = WS_CombatBehaveAIManagerEntity.CreateCombatAI<WS_CombatSceneAIControllerEntity>(1301u, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null,
                () =>
                {
                    m_RefreshDoTurn = true;
                });
            }
            else if (stage == 2u)
            {
                sController = WS_CombatBehaveAIManagerEntity.CreateCombatAI<WS_CombatSceneAIControllerEntity>(1302u, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null,
                () =>
                {
                    m_RefreshDoTurn = true;
                });
            }
            else if (stage == 3u)
            {
                sController = WS_CombatBehaveAIManagerEntity.CreateCombatAI<WS_CombatSceneAIControllerEntity>(1303u, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null,
                () =>
                {
                    m_RefreshDoTurn = true;
                });
            }
            if (sController != null && sController.StartSceneController(excuteTurnIndex, outBoInfo))
                isHaveBehave = true;
        }
    }

    //检测有Mob时是否在行为中，
    //没有Mob的话就检测当前回合执行到_excuteTurnList序列_excuteTurnIndex之前的所有ExcuteTurn数据中是否有unitId的生成战斗单位
    private short InterceptBehave(uint unitId, bool forceExcute)
    {
        MobEntity target = MobManager.Instance.GetMob(unitId);
        if (target != null)
        {
            return InterceptMobBehave(target, forceExcute);
        }
        else if (IsCallingNewBattleUnit(unitId))
        {
            if (!forceExcute)
            {
                DebugUtil.Log(ELogType.eCombatBehave, $"正在召唤加载中----<color=green>UnitId：{unitId.ToString()}</color>");

                _excuteTurnIndex--;
                return -2;
            }

            DebugUtil.LogError($"强制回合的时候UnitId：{unitId.ToString()}还没有召唤出来");

            return 2;
        }

        return 0;
    }

    private short InterceptMobBehave(MobEntity target, bool forceExcute)
    {
        if (forceExcute)
            target.m_MobCombatComponent.StopBehave(true);
        else if (target.m_MobCombatComponent.m_IsStartBehave || target.m_MobCombatComponent.m_HpChangeDataQueue.Count > 0 ||
            target.m_MobCombatComponent.m_ReadlyBehaveCount > 0)
        {
            DebugUtil.Log(ELogType.eCombatBehave, $"正在行动中----<color=green>ClientNum: {target.m_MobCombatComponent.m_ClientNum.ToString()}--IsStartBehave:{target.m_MobCombatComponent.m_IsStartBehave.ToString()}--BehaveCount:{target.m_MobCombatComponent.GetBehaveCount().ToString()}    m_HpChangeDataQueue.Count:{target.m_MobCombatComponent.m_HpChangeDataQueue.Count.ToString()}    m_ReadlyBehaveCount:{target.m_MobCombatComponent.m_ReadlyBehaveCount.ToString()}</color>");

            _excuteTurnIndex--;
            return -1;
        }

        return 1;
    }

    //处理目标CombatHpChangeDataQueue的状态
    private void DoCombatHpChangeDataState(MobEntity target)
    {
        if (!target.m_MobCombatComponent.m_IsSetHpChange)
        {
            target.m_MobCombatComponent.ClearCombatHpChangeDataQueue(false);
            target.m_MobCombatComponent.m_IsSetHpChange = true;
        }
        _isSetHpMpChangeMobList.Add(target);
    }

    /// <summary>
    /// =0 DelUnitId，=1 DelFailUnitId，=2 EscapeUnitId， =3 EscapeFailUnitId, =5 BeUsedSkillFail，=6 NewUnits, =7 UnitPosChange
    /// </summary>
    /// <param name="setType">=0 DelUnitId，=1 DelFailUnitId，=2 EscapeUnitId， =3 EscapeFailUnitId, =5 BeUsedSkillFail，=6 NewUnits, =7 UnitPosChange</param>
    /// <param name="replaceType">0 默认删除表现 1 附身  2 击飞  3 闪现 4 逃跑 5 技能击飞效果 6 不需要表现</param>
    private void SetUnitsChangeTarget(ExcuteTurn excuteTurn, NetExcuteTurnInfo netExcuteTurnInfo, int edIndex, uint srcUnitId, uint extendType, uint extendId, 
        AttackBoLifeCycleInfo tempAttacBokLifeCycle, SNodeInfo tempTTi, uint unitId, BattleUnit newBattleUnit, uint unitChangeServerPos, int setType, uint replaceType, int handleType)
    {
#if DEBUG_MODE
        string SetUnitsChangeTargetLog = $"有{(setType == 0 ? "DelUnitId" : (setType == 1 ? "DelFailUnitId" : (setType == 2 ? "EscapeUnitId" : (setType == 3 ? "EscapeFailUnitId" : (setType == 6 ? "NewUnits" : (setType == 5 ? "BeUsedSkillFail" : (setType == 7 ? "UnitPosChange" : "")))))))}行为----";
#endif

        if (setType == 6)
        {
#if DEBUG_MODE
            DLogManager.Log(ELogType.eCombat, $"{SetUnitsChangeTargetLog}<color=yellow>目标UnitId:{unitId.ToString()}   newBattleUnit:{(newBattleUnit == null ? string.Empty : $"{newBattleUnit.UnitId.ToString()}|pos:{newBattleUnit.Pos.ToString()}|ShapeShiftId:{newBattleUnit.ShapeShiftId.ToString()}|IsUseShapeShift:{newBattleUnit.IsUseShapeShift.ToString()}")}   replaceType:{replaceType.ToString()}</color>");
#endif

            _recordTurnInfoTemp.Clear();
            _recordTurnInfoTemp.SrcUnitId = srcUnitId;
            _recordTurnInfoTemp.ExtendType = extendType;
            _recordTurnInfoTemp.ExtendId = extendId;
            _recordTurnInfoTemp.NewBattleUnit = newBattleUnit;
            _recordTurnInfoTemp.IsHaveHpMpChange = false;
            _recordTurnInfoTemp.ExcuteDataIndex = edIndex;

            SetTurnBehaveInfos(excuteTurn, netExcuteTurnInfo, tempAttacBokLifeCycle, tempTTi, unitId, _recordTurnInfoTemp, handleType);
        }
        else
        {
            MobEntity target = MobManager.Instance.GetMob(unitId);
            if (target != null)
            {
#if DEBUG_MODE
                DLogManager.Log(ELogType.eCombat, $"{SetUnitsChangeTargetLog}<color=yellow>目标UnitId:{unitId.ToString()}, 位置{target.m_MobCombatComponent.m_ClientNum.ToString()}   replaceType:{replaceType.ToString()}</color>");
#endif
                
                if (setType == 0 && (replaceType == 2 || replaceType == 6))
                {
                    if (replaceType == 2)
                    {
                        target.m_MobCombatComponent.AddCombatUnitState(CombatUnitState.Death | CombatUnitState.DelSuccess | CombatUnitState.BeHitFlyDeath);
                        target.m_MobCombatComponent.m_BeHitToFlyState = true;
                    }
                    else
                    {
                        target.m_MobCombatComponent.m_BeDelUnit = true;
                        target.m_MobCombatComponent.AddCombatUnitState(CombatUnitState.Death | CombatUnitState.DelSuccess);
                        target.m_MobCombatComponent.m_BeHitToFlyState = false;
                    }
                    
                    target.m_MobCombatComponent.m_Death = true;

                    OnAddOrDelUnit(true, target.m_MobCombatComponent.m_BattleUnit, false);
                    return;
                }

                if (replaceType == 4)
                {
                    if (target.m_MobCombatComponent != null)
                        target.m_MobCombatComponent.m_IsRunAway = true;
                }

                if (setType == 2)
                {
                    if (target.m_MobCombatComponent != null)
                        target.m_MobCombatComponent.m_IsRunAway = true;

                    if (MobManager.Instance.IsPlayer(target.m_MobCombatComponent.m_BattleUnit))
                        m_BattleOverType = 1u;
                }

                _recordTurnInfoTemp.Clear();
                _recordTurnInfoTemp.SrcUnitId = srcUnitId;
                _recordTurnInfoTemp.ExtendType = extendType;
                _recordTurnInfoTemp.ExtendId = extendId;
                _recordTurnInfoTemp.IsHaveHpMpChange = true;
                _recordTurnInfoTemp.ExcuteDataIndex = edIndex;

                if (handleType == 0)
                {
                    DoCombatHpChangeDataState(target);

                    var ucChangeData = CombatObjectPool.Instance.Get<CombatHpChangeData>();
                    ucChangeData.m_CurHp = target.m_MobCombatComponent.m_BattleUnit.CurHp;
                    ucChangeData.m_CurMp = target.m_MobCombatComponent.m_BattleUnit.CurMp;
                    if (setType == 0)
                    {
                        ucChangeData.m_CurHp = target.m_MobCombatComponent.m_BattleUnit.CurHp = 0;
                        ucChangeData.m_Death = target.m_MobCombatComponent.m_Death = true;
                        if (replaceType == 5u)
                        {
                            target.m_MobCombatComponent.m_BeHitToFlyState = true;
                            OnAddOrDelUnit(true, target.m_MobCombatComponent.m_BattleUnit, false);
                        }
                        else
                        {
                            target.m_MobCombatComponent.m_BeDelUnit = true;
                        }
                    }
                    else
                    {
                        ucChangeData.m_Death = target.m_MobCombatComponent.m_Death;
                    }
                    ucChangeData.m_ConverAttackCount = 0;
                    ucChangeData.m_AnimType = AnimType.e_Normal;
                    
                    ucChangeData.m_ExtendId = extendId;
                    ucChangeData.m_ExtendType = extendType;
                    ucChangeData.m_ExcuteDataIndex = edIndex;
                    ucChangeData.m_SrcUnitId = srcUnitId;
                    if (setType == 7)
                        ucChangeData.m_UnitChangePos = (int)unitChangeServerPos;

                    target.m_MobCombatComponent.m_HpChangeDataQueue.Enqueue(ucChangeData);

                    target.m_MobCombatComponent.SetCombatUnitState(setType);
                }

                SetTurnBehaveInfos(excuteTurn, netExcuteTurnInfo, tempAttacBokLifeCycle, tempTTi, unitId, _recordTurnInfoTemp, handleType);
            }
            else if (handleType == 0)
            {
#if DEBUG_MODE
                DLogManager.Log(ELogType.eCombat, $"{SetUnitsChangeTargetLog}<color=yellow>目标UnitId:{unitId.ToString()}   newBattleUnit:{(newBattleUnit == null ? string.Empty : newBattleUnit.UnitId.ToString())}   replaceType:{replaceType.ToString()}</color>");
#endif

                netExcuteTurnInfo.CacheToNetInfoForNoMobData(unitId, excuteTurn, GetExcuteTurnIndex(excuteTurn), excuteTurn.ExcuteData[edIndex], edIndex,
                    tempAttacBokLifeCycle, tempTTi, srcUnitId, extendType, extendId, null);
            }
        }
    }

    private void SetTurnBehaveInfos(ExcuteTurn excuteTurn, NetExcuteTurnInfo netExcuteTurnInfo, AttackBoLifeCycleInfo tempAttacBokLifeCycle,
        SNodeInfo tempTTi, uint targetUnitId, RecordTurnInfoTemp recordTurnInfoTemp, int handleType)
    {
        if (tempAttacBokLifeCycle == null)
            return;

        if (handleType == 0)
        {
            MobEntity srcMob = MobManager.Instance.GetMob(recordTurnInfoTemp.SrcUnitId);
            if (srcMob != null && srcMob.m_MobCombatComponent != null)
                srcMob.m_MobCombatComponent.UpdateReadlyBehaveCount(1);
        }

        //处理主动技能 或 反击数据
        if (recordTurnInfoTemp.ExtendType == 0u ||
            recordTurnInfoTemp.ExtendType == 5u)    
        {
            if (tempTTi != null && handleType == 0)
            {
                //逻辑效果Id/10 = 技能id
                uint skillId = recordTurnInfoTemp.ExtendId > 0u ? recordTurnInfoTemp.ExtendId / 10u : 0u;

                //排除被动引起的主动技能
                if (tempTTi.Layer == 1)
                {
                    if (tempAttacBokLifeCycle.IsOutBoTempFlag)
                    {
                        if (tempAttacBokLifeCycle.m_OutBoInfo != null)
                            AddTurnBehaveInfo(tempAttacBokLifeCycle.m_OutBoInfo.TurnBehaveInfoList, tempTTi.Bo, recordTurnInfoTemp.ExcuteDataIndex,
                                recordTurnInfoTemp.SrcUnitId, targetUnitId, tempTTi.SNodeId, tempTTi.Layer,
                                skillId, recordTurnInfoTemp.HitEffect, recordTurnInfoTemp.ExtraHitEffect, recordTurnInfoTemp.IsConverAttack, recordTurnInfoTemp.NewBattleUnit,
                                recordTurnInfoTemp.ExtendType, recordTurnInfoTemp.ExtendId, true);
#if DEBUG_MODE
                        else
                        {
                            DebugUtil.LogError($"tempAttacBokLifeCycle.m_OutBoInfo获取数据null，执行第{tempTTi.Layer.ToString()}层 ExtendId:{recordTurnInfoTemp.ExtendId.ToString()} ExtendType:{recordTurnInfoTemp.ExtendType.ToString()}的SrcUnitId为0");
                        }
#endif
                    }
                    else
                    {
                        for (int srcIndex = 0, srcCount = excuteTurn.SrcUnit.Count; srcIndex < srcCount; srcIndex++)
                        {
                            if (skillId == 0u)
                                skillId = excuteTurn.SkillId[srcIndex];

                            AddTurnBehaveInfo(netExcuteTurnInfo.TurnBehaveInfoList, tempTTi.Bo, recordTurnInfoTemp.ExcuteDataIndex,
                                excuteTurn.SrcUnit[srcIndex], targetUnitId, tempTTi.SNodeId, tempTTi.Layer,
                                skillId, recordTurnInfoTemp.HitEffect, recordTurnInfoTemp.ExtraHitEffect, recordTurnInfoTemp.IsConverAttack, recordTurnInfoTemp.NewBattleUnit,
                                recordTurnInfoTemp.ExtendType, recordTurnInfoTemp.ExtendId);
                        }
                    }
                }
                else if (tempTTi.Layer > 1)
                {
                    if (recordTurnInfoTemp.SrcUnitId > 0u)
                    {
                        if (tempTTi.ParentSNodeInfo != null)
                        {
                            if (tempTTi.ParentSNodeInfo.TurnBehaveInfoList == null)
                                tempTTi.ParentSNodeInfo.TurnBehaveInfoList = new List<TurnBehaveInfo>();

                            AddTurnBehaveInfo(tempTTi.ParentSNodeInfo.TurnBehaveInfoList, tempTTi.ParentSNodeInfo.Bo, recordTurnInfoTemp.ExcuteDataIndex,
                                recordTurnInfoTemp.SrcUnitId, targetUnitId, tempTTi.SNodeId, tempTTi.Layer,
                                skillId, recordTurnInfoTemp.HitEffect, recordTurnInfoTemp.ExtraHitEffect, recordTurnInfoTemp.IsConverAttack, recordTurnInfoTemp.NewBattleUnit,
                                recordTurnInfoTemp.ExtendType, recordTurnInfoTemp.ExtendId);
                        }
                    }
#if DEBUG_MODE
                    else
                    {
                        DebugUtil.LogError($"执行第{tempTTi.Layer.ToString()}层 ExtendId:{recordTurnInfoTemp.ExtendId.ToString()} ExtendType:{recordTurnInfoTemp.ExtendType.ToString()}的SrcUnitId为0");
                    }
#endif
                }
            }
        }
        else if (recordTurnInfoTemp.ExtendType == 1)    //处理被动
        {
            if (handleType != 0 && recordTurnInfoTemp.ExtendId > 0)
            {
                CSVPassiveSkill.Data pssiveSkillTb = CSVPassiveSkill.Instance.GetConfData(recordTurnInfoTemp.ExtendId);
                if (pssiveSkillTb != null)
                {
                    if (pssiveSkillTb.behavior_work_id > 0u)
                    {
                        if (recordTurnInfoTemp.SrcUnitId > 0u)
                        {
                            MobEntity attack = MobManager.Instance.GetMob(recordTurnInfoTemp.SrcUnitId);
                            if (attack != null)
                            {
                                int tempTtiBo;
                                uint temTtiSNodeId;
                                int temTtiSNodeLayer;
                                List<TurnBehaveInfo> passiveTurnBehaveInfoList = GetTurnBehaveInfoListBySNodeInfo(tempAttacBokLifeCycle, tempTTi,
                                    out tempTtiBo, out temTtiSNodeId, out temTtiSNodeLayer);

                                BehaveAIControllParam behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
                                behaveAIControllParam.SrcUnitId = recordTurnInfoTemp.SrcUnitId;
                                behaveAIControllParam.SkillId = recordTurnInfoTemp.ExtendId;
                                behaveAIControllParam.TargetUnitId = targetUnitId > 0u ? targetUnitId : (recordTurnInfoTemp.NewBattleUnit == null ? 0u : recordTurnInfoTemp.NewBattleUnit.UnitId);
                                behaveAIControllParam.ExcuteTurnIndex = GetExcuteTurnIndex(excuteTurn);

                                TurnBehaveSkillInfo passiveTurnBehaveSkillInfo = AddTurnBehaveInfo(passiveTurnBehaveInfoList, tempTtiBo, 
                                                                        recordTurnInfoTemp.ExcuteDataIndex,
                                                                        recordTurnInfoTemp.SrcUnitId, targetUnitId,
                                                                        temTtiSNodeId, temTtiSNodeLayer,
                                                                        recordTurnInfoTemp.ExtendId, recordTurnInfoTemp.HitEffect, recordTurnInfoTemp.ExtraHitEffect, 
                                                                        recordTurnInfoTemp.IsConverAttack, recordTurnInfoTemp.NewBattleUnit,
                                                                        recordTurnInfoTemp.ExtendType, recordTurnInfoTemp.ExtendId, true);
                                
                                attack.m_MobCombatComponent.DoBehave(pssiveSkillTb.behavior_work_id, null, 0, passiveTurnBehaveSkillInfo, behaveAIControllParam);
                            }
                        }
#if DEBUG_MODE
                        else
                        {
                            DebugUtil.LogError($"执行第{tempTTi?.Layer.ToString()}层 ExtendId:{recordTurnInfoTemp.ExtendId.ToString()} ExtendType:{recordTurnInfoTemp.ExtendType.ToString()}的SrcUnitId为0");
                        }
#endif
                    }
#if DEBUG_MODE
                    else
                    {
                        DLogManager.Log(ELogType.eCombat, $"被动------<color=red>被动Id：{recordTurnInfoTemp.ExtendId.ToString()}表中behavior_work_id为0</color>");
                    }
#endif
                }
#if DEBUG_MODE
                else
                {
                    DebugUtil.LogError($"CSVPassiveSkillData被动表中不存在被动Id：{recordTurnInfoTemp.ExtendId.ToString()}");
                }
#endif
            }
        }
        else if (recordTurnInfoTemp.ExtendType == 2 && recordTurnInfoTemp.BuffSkillId > 0u && handleType == 0)     //buff行为处理
        {
            if (recordTurnInfoTemp.SrcUnitId > 0u)
            {
                int tempTtiBo;
                uint temTtiSNodeId;
                int temTtiSNodeLayer;
                List<TurnBehaveInfo> buffTurnBehaveInfoList = GetTurnBehaveInfoListBySNodeInfo(tempAttacBokLifeCycle, tempTTi,
                    out tempTtiBo, out temTtiSNodeId, out temTtiSNodeLayer);
                
                if (tempTTi == null || tempAttacBokLifeCycle.IsOutBoTempFlag)
                {
                    MobEntity attack = MobManager.Instance.GetMob(recordTurnInfoTemp.SrcUnitId);
                    if (attack != null)
                    {
                        BehaveAIControllParam behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
                        behaveAIControllParam.SrcUnitId = recordTurnInfoTemp.SrcUnitId;
                        behaveAIControllParam.SkillId = recordTurnInfoTemp.BuffSkillId;
                        behaveAIControllParam.TargetUnitId = targetUnitId > 0u ? targetUnitId : (recordTurnInfoTemp.NewBattleUnit == null ? 0u : recordTurnInfoTemp.NewBattleUnit.UnitId);
                        behaveAIControllParam.ExcuteTurnIndex = GetExcuteTurnIndex(excuteTurn);

                        var buffTurnBehaveSkillInfo = AddTurnBehaveInfo(buffTurnBehaveInfoList, tempTtiBo,
                                                                recordTurnInfoTemp.ExcuteDataIndex,
                                                                recordTurnInfoTemp.SrcUnitId, targetUnitId,
                                                                temTtiSNodeId, temTtiSNodeLayer,
                                                                recordTurnInfoTemp.BuffSkillId, recordTurnInfoTemp.HitEffect, recordTurnInfoTemp.ExtraHitEffect, 
                                                                false, recordTurnInfoTemp.NewBattleUnit,
                                                                recordTurnInfoTemp.ExtendType, recordTurnInfoTemp.ExtendId, true);

                        attack.m_MobCombatComponent.StartBehave(recordTurnInfoTemp.BuffSkillId, buffTurnBehaveSkillInfo, false, behaveAIControllParam);
                    }
                }
                else
                {
                    AddTurnBehaveInfo(buffTurnBehaveInfoList, tempTtiBo, recordTurnInfoTemp.ExcuteDataIndex,
                                        recordTurnInfoTemp.SrcUnitId, targetUnitId,
                                        temTtiSNodeId, temTtiSNodeLayer,
                                        recordTurnInfoTemp.BuffSkillId, recordTurnInfoTemp.HitEffect, recordTurnInfoTemp.ExtraHitEffect,
                                        false, recordTurnInfoTemp.NewBattleUnit,
                                        recordTurnInfoTemp.ExtendType, recordTurnInfoTemp.ExtendId, true);
                }
            }
#if DEBUG_MODE
            else
            {
                DebugUtil.LogError($"执行第{tempTTi?.Layer.ToString()}层 ExtendId:{recordTurnInfoTemp.ExtendId.ToString()} ExtendType:{recordTurnInfoTemp.ExtendType.ToString()}的SrcUnitId为0");
            }
#endif
        }
        else if (recordTurnInfoTemp.ExtendType == 3 && handleType == 0)     //技能行动前
        {
            if (recordTurnInfoTemp.HitEffect == (uint)HitEffectType.Mana || recordTurnInfoTemp.HitEffect == (uint)HitEffectType.Energe)
            {
                if (tempTTi == null || tempAttacBokLifeCycle.IsOutBoTempFlag)
                {
                    MobEntity attack = MobManager.Instance.GetMob(recordTurnInfoTemp.SrcUnitId);
                    if (attack != null)
                    {
                        uint skillId = recordTurnInfoTemp.ExtendId > 0u ? recordTurnInfoTemp.ExtendId / 10u : 0u;

                        BehaveAIControllParam behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
                        behaveAIControllParam.SrcUnitId = recordTurnInfoTemp.SrcUnitId;
                        behaveAIControllParam.SkillId = skillId;
                        behaveAIControllParam.TargetUnitId = targetUnitId > 0u ? targetUnitId : (recordTurnInfoTemp.NewBattleUnit == null ? 0u : recordTurnInfoTemp.NewBattleUnit.UnitId);
                        behaveAIControllParam.ExcuteTurnIndex = GetExcuteTurnIndex(excuteTurn);
                        behaveAIControllParam.HitEffect = (int)recordTurnInfoTemp.HitEffect;
                        
                        attack.m_MobCombatComponent.StartBehave(skillId, null, false, behaveAIControllParam);
                    }
                }
            }
        }

        recordTurnInfoTemp.Clear();
    }

    private List<TurnBehaveInfo> GetTurnBehaveInfoListBySNodeInfo(AttackBoLifeCycleInfo tempAttacBokLifeCycle, SNodeInfo tempTTi, 
        out int bo, out uint snodeId, out int snodeLayer)
    {
        List<TurnBehaveInfo> turnBehaveInfo = null;
        if (tempTTi != null)
        {
            if (tempTTi.Layer > 1 && tempTTi.ParentSNodeInfo != null)
            {
                if (tempTTi.ParentSNodeInfo.TurnBehaveInfoList == null)
                    tempTTi.ParentSNodeInfo.TurnBehaveInfoList = new List<TurnBehaveInfo>();
                turnBehaveInfo = tempTTi.ParentSNodeInfo.TurnBehaveInfoList;
            }
            else
            {
                if (tempTTi.TurnBehaveInfoList == null)
                    tempTTi.TurnBehaveInfoList = new List<TurnBehaveInfo>();
                turnBehaveInfo = tempTTi.TurnBehaveInfoList;
            }

            bo = tempTTi.Bo;
            snodeId = tempTTi.SNodeId;
            snodeLayer = tempTTi.Layer;
        }
        else
        {
            if (tempAttacBokLifeCycle.m_OutBoInfo.TurnBehaveInfoList == null)
                tempAttacBokLifeCycle.m_OutBoInfo.TurnBehaveInfoList = new List<TurnBehaveInfo>();
            turnBehaveInfo = tempAttacBokLifeCycle.m_OutBoInfo.TurnBehaveInfoList;

            bo = tempAttacBokLifeCycle.m_OutBoInfo.Bo;
            snodeId = uint.MaxValue;
            snodeLayer = 1;
        }
        
        return turnBehaveInfo;
    }

    public TurnBehaveSkillInfo AddTurnBehaveInfo(List<TurnBehaveInfo> list, int bo, int excuteDataIndex, uint srcUnitId, 
        uint targetUnitId, uint snodeId, int snodeLayer, uint skillId, uint hitEffect, uint extraHitEffect, 
        bool isConverAttack, BattleUnit newBattleUnit, uint extendType, uint extendId, bool isNotMergeSkill = false)
    {
        if (list == null)
            return null;

        TurnBehaveInfo turnBehaveInfo = null;
        int tbiCount = list.Count;
        if (tbiCount > 0)
        {
            for (int i = 0; i < tbiCount; i++)
            {
                TurnBehaveInfo checkTbi = list[i];
                if (checkTbi.SrcUnitId == srcUnitId)
                {
                    turnBehaveInfo = checkTbi;

                    break;
                }
            }
        }
        
        if (turnBehaveInfo == null)
        {
            turnBehaveInfo = BasePoolClass.Get<TurnBehaveInfo>();
            turnBehaveInfo.SrcUnitId = srcUnitId;

            list.Add(turnBehaveInfo);
        }

        TurnBehaveSkillInfo turnBehaveSkillInfo = null;

        if (turnBehaveInfo.TurnBehaveSkillInfoList == null)
        {
            turnBehaveInfo.TurnBehaveSkillInfoList = new List<TurnBehaveSkillInfo>();
        }
        else
        {
            if (snodeLayer == 1 && !isNotMergeSkill)
            {
                int skillCount = turnBehaveInfo.TurnBehaveSkillInfoList.Count;
                if (skillCount > 0)
                {
                    for (int skillIndex = 0; skillIndex < skillCount; skillIndex++)
                    {
                        TurnBehaveSkillInfo tbsi = turnBehaveInfo.TurnBehaveSkillInfoList[skillIndex];
                        if (tbsi == null)
                            continue;

                        if (tbsi.SkillId == skillId)
                        {
                            turnBehaveSkillInfo = tbsi;
                            break;
                        }
                    }
                }
            }
        }

        if (turnBehaveSkillInfo == null)
        {
            turnBehaveSkillInfo = BasePoolClass.Get<TurnBehaveSkillInfo>();
            turnBehaveSkillInfo.SkillId = skillId;
            turnBehaveSkillInfo.HitEffect = hitEffect;
            turnBehaveSkillInfo.IsConverAttack = isConverAttack;

            turnBehaveInfo.TurnBehaveSkillInfoList.Add(turnBehaveSkillInfo);
        }
        
        SetTurnBehaveSkillInfo(turnBehaveSkillInfo, bo, excuteDataIndex, srcUnitId, targetUnitId, 
            snodeId, snodeLayer, skillId, newBattleUnit, extendType, extendId, hitEffect, extraHitEffect);

        return turnBehaveSkillInfo;
    }
    
    private void SetTurnBehaveSkillInfo(TurnBehaveSkillInfo turnBehaveSkillInfo, int bo, int excuteDataIndex, uint srcUnitId, 
        uint targetUnitId, uint snodeId, int snodeLayer, uint skillId, BattleUnit newBattleUnit, 
        uint extendType, uint extendId, uint hitEffect, uint extraHitEffect)
    {
#if DEBUG_MODE
        if (turnBehaveSkillInfo.ExtendType > 0u && turnBehaveSkillInfo.ExtendType != extendType)
            DebugUtil.LogError($"skillId:{skillId.ToString()}   turnBehaveSkillInfo.ExtendType:{turnBehaveSkillInfo.ExtendType.ToString()}  extendType:{extendType.ToString()}数据不对");
#endif
        turnBehaveSkillInfo.ExtendType = extendType;

        if (turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList == null)
            turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList = new List<TurnBehaveSkillTargetInfo>();

        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = BasePoolClass.Get<TurnBehaveSkillTargetInfo>();
        turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Add(turnBehaveSkillTargetInfo);

        turnBehaveSkillTargetInfo.Id = ++CombatHelp.m_UnitStartId;
        turnBehaveSkillTargetInfo.Bo = bo;

        if (targetUnitId > 0u)
        {
            turnBehaveSkillTargetInfo.TargetUnitId = targetUnitId;
            ++turnBehaveSkillInfo.TargetUnitCount;
        }

        if (newBattleUnit != null)
        {
            turnBehaveSkillTargetInfo.CallingTargetBattleUnit = newBattleUnit;
            ++turnBehaveSkillInfo.CallingTargetBattleUnitCount;
        }

#if DEBUG_MODE
        if (targetUnitId == 0u && newBattleUnit == null)
            DebugUtil.LogError($"AddTurnBehaveInfo记录srcUnitId：{srcUnitId.ToString()}   skillId:{skillId.ToString()}   数据为null");

        if (snodeId == 0u)
            DebugUtil.LogError($"AddTurnBehaveInfo记录srcUnitId：{srcUnitId.ToString()}   skillId:{skillId.ToString()}   snodeId为{snodeId.ToString()}");
#endif

        turnBehaveSkillTargetInfo.SNodeId = snodeId;
        turnBehaveSkillTargetInfo.SNodeLayer = snodeLayer;
        turnBehaveSkillTargetInfo.ExtendId = extendId;
        turnBehaveSkillTargetInfo.ExcuteDataIndex = excuteDataIndex;
        turnBehaveSkillTargetInfo.HitEffect = hitEffect;
        turnBehaveSkillTargetInfo.ExtraHitEffect = extraHitEffect;
        turnBehaveSkillTargetInfo.BeProtectUnitId = GetBeProtectUnitIdByTemp(extraHitEffect, srcUnitId, targetUnitId);
    }

    private bool IsExistTurnBehaveTarget(List<TurnBehaveInfo> list, uint targetUnitId)
    {
        int turnBehaveCount = list.Count;
        if (turnBehaveCount > 0)
        {
            for (int turnBehaveIndex = 0; turnBehaveIndex < turnBehaveCount; turnBehaveIndex++)
            {
                TurnBehaveInfo tbi = list[turnBehaveIndex];
                if (tbi.TurnBehaveSkillInfoList == null)
                    continue;

                int tbsiCount = tbi.TurnBehaveSkillInfoList.Count;
                if (tbsiCount > 0)
                {
                    for (int tbsiIndex = 0; tbsiIndex < tbsiCount; tbsiIndex++)
                    {
                        TurnBehaveSkillInfo turnBehaveSkillInfo = tbi.TurnBehaveSkillInfoList[tbsiIndex];
                        if (turnBehaveSkillInfo == null || turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList == null)
                            continue;

                        int targetCount = turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count;
                        if (targetCount > 0)
                        {
                            for (int targetIndex = 0; targetIndex < targetCount; targetIndex++)
                            {
                                TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[targetIndex];
                                if ((turnBehaveSkillTargetInfo.TargetUnitId > 0u && turnBehaveSkillTargetInfo.TargetUnitId == targetUnitId) ||
                                    (turnBehaveSkillTargetInfo.CallingTargetBattleUnit != null && turnBehaveSkillTargetInfo.CallingTargetBattleUnit.UnitId == targetUnitId))
                                    return true;
                            }
                        }
                    }
                }
            }
        }
        
        return false;
    }

    private bool CheckBehaveBuff(ExcuteTurn excuteTurn, BattleBuffChange bc)
    {
        if ((excuteTurn.SrcUnit.Count > 0 && excuteTurn.SkillId.Count > 0) ||
            excuteTurn.Stage == 1 || excuteTurn.Stage == 2 || excuteTurn.Stage == 3)
        {
            if (bc.FailAdd == 0u && bc.OddNum == 0u)
                return false;

            return true;
        }

        return false;
    }

    public void InformIsAttackSkillSuccess(BattleUnit bu, uint skillId)
    {
        if (Sys_Role.Instance.Role == null)
            return;

        if (bu.RoleId == Sys_Role.Instance.Role.RoleId)
        {
            if ((UnitType)bu.UnitType == UnitType.Hero)
            {
                eventEmitter.Trigger(EEvents.OnSkillColdUpdate, skillId, true);
            }
            else if ((UnitType)bu.UnitType == UnitType.Pet)
            {
                eventEmitter.Trigger(EEvents.OnSkillColdUpdate, skillId, false);
            }
        }
    }
    
    public NetExcuteTurnInfo GetNetExcuteTurnInfo(int excuteIndex)
    {
        if (!m_NetExcuteTurnInfoDic.TryGetValue(excuteIndex, out NetExcuteTurnInfo netExcuteTurnInfo))
        {
            DebugUtil.LogError($"GetNetExcuteTurnInfo-----excuteIndex:{excuteIndex.ToString()}下标不存在数据");
            return null;
        }

        return netExcuteTurnInfo;
    }

    public void DoCombineAttackWaitCount(int excuteIndex)
    {
        if (!m_NetExcuteTurnInfoDic.TryGetValue(excuteIndex, out NetExcuteTurnInfo netExcuteTurnInfo))
        {
            DebugUtil.LogError($"DoCombineAttackWaitCount-----excuteIndex:{excuteIndex.ToString()}下标不存在数据");
            return;
        }

        netExcuteTurnInfo.CombineAttack_WaitCount++;

        if (netExcuteTurnInfo.CombineAttack_WaitCount >= netExcuteTurnInfo.CombineAttack_SrcUnits.Count)
        {
            netExcuteTurnInfo.CombineAttack_WaitCount = -1;
            CombatManager.Instance.m_EventEmitter.Trigger(CombatManager.EEvents.WaitAttack_CombineAttack, netExcuteTurnInfo.CombineAttack_SrcUnits[0]);
            return;
        }
    }

    public RepeatedField<TalkInfo> GetTalkInfoId(int excuteIndex)
    {
        if (!m_NetExcuteTurnInfoDic.TryGetValue(excuteIndex, out NetExcuteTurnInfo netExcuteTurnInfo))
        {
            DebugUtil.LogError($"GetNetExcuteTurnInfo-----excuteIndex:{excuteIndex.ToString()}下标不存在数据");
            return null;
        }

        return netExcuteTurnInfo.TalkInfos;
    }

    private Queue<CombatHpChangeData> _tempHcQueue = new Queue<CombatHpChangeData>();
    public void DoTriggerBuffBehave(MobEntity mob, int excuteTurnIndex, TurnBehaveSkillInfo triggerSkillInfo, int triggerSkillTargetInfoIndex)
    {
        if (mob != null && mob.m_MobCombatComponent != null && excuteTurnIndex > -1 && 
            mob.m_MobCombatComponent.m_HpChangeDataQueue != null && mob.m_MobCombatComponent.m_HpChangeDataQueue.Count > 0)
        {
#if DEBUG_MODE
            if (_tempHcQueue.Count > 0)
                DebugUtil.LogError($"DoTriggerBuffBehave时_tempHcQueue.Count:{_tempHcQueue.Count.ToString()}");
#endif
            _tempHcQueue.Clear();
            CombatHpChangeData combatHpChangeData = null;
            while (mob.m_MobCombatComponent.m_HpChangeDataQueue.Count > 0)
            {
                CombatHpChangeData chcd = mob.m_MobCombatComponent.m_HpChangeDataQueue.Dequeue();
                if (combatHpChangeData == null && !chcd.m_IsBeUse)
                    combatHpChangeData = chcd;
                _tempHcQueue.Enqueue(chcd);
            }
            CombatHelp.Swap(ref mob.m_MobCombatComponent.m_HpChangeDataQueue, ref _tempHcQueue);

            if (combatHpChangeData != null && combatHpChangeData.m_ExtendType == 2u && combatHpChangeData.m_BuffSkillId > 0u
                && combatHpChangeData.m_ExcuteDataIndex > -1)
            {
                if (_excuteTurnList == null)
                    return;

                if (excuteTurnIndex >= _excuteTurnList.Count)
                    return;

                ExcuteTurn excuteTurn = _excuteTurnList[excuteTurnIndex];
                if (excuteTurn == null || excuteTurn.ExcuteData == null)
                    return;

                int excuteDataCount = excuteTurn.ExcuteData.Count;
                if (excuteDataCount <= 0)
                    return;

                NetExcuteTurnInfo netExcuteTurnInfo = GetNetExcuteTurnInfo(excuteTurnIndex);
                if (netExcuteTurnInfo == null || netExcuteTurnInfo.AttackBoLifeCycleInfoList == null)
                    return;

                uint srcUnitId = 0u;
                TurnBehaveSkillTargetInfo selectTargetInfo;
                TurnBehaveSkillInfo turnBehaveSkillInfo = GetTurnBehaveSkillInfoByEdIndex(netExcuteTurnInfo.TurnBehaveInfoList,
                                                                combatHpChangeData.m_ExcuteDataIndex, ref srcUnitId, out selectTargetInfo);

                if (turnBehaveSkillInfo == null)
                {
                    int ablciCount = netExcuteTurnInfo.AttackBoLifeCycleInfoList.Count;
                    if (ablciCount > 0)
                    {
                        for (int ablciIndex = 0; ablciIndex < ablciCount; ablciIndex++)
                        {
                            AttackBoLifeCycleInfo ablci = netExcuteTurnInfo.AttackBoLifeCycleInfoList[ablciIndex];
                            if (ablci == null)
                                continue;

                            turnBehaveSkillInfo = GetTurnBehaveSkillInfoInSNodeInfo(ablci.SNodeInfoList, combatHpChangeData.m_ExcuteDataIndex, 
                                1, ref srcUnitId, out selectTargetInfo);

                            if (turnBehaveSkillInfo != null)
                                break;

                            if (ablci.m_OutBoInfo != null)
                            {
                                turnBehaveSkillInfo = GetTurnBehaveSkillInfoByEdIndex(ablci.m_OutBoInfo.TurnBehaveInfoList,
                                                                combatHpChangeData.m_ExcuteDataIndex, ref srcUnitId, out selectTargetInfo);

                                if (turnBehaveSkillInfo != null)
                                    break;
                            }
                        }
                    }
                }
                
                if (turnBehaveSkillInfo != null)
                {
                    if (triggerSkillInfo != null && triggerSkillInfo.TurnBehaveSkillTargetInfoList != null &&
                        triggerSkillTargetInfoIndex > -1 && selectTargetInfo != null && selectTargetInfo.SNodeId > 0u)
                    {
                        bool isExistSNodeId = false;
                        for (int tbstiIndex = 0, tbstiCount = triggerSkillInfo.TurnBehaveSkillTargetInfoList.Count; tbstiIndex < tbstiCount; tbstiIndex++)
                        {
                            TurnBehaveSkillTargetInfo triggerSkillTargetInfo = triggerSkillInfo.TurnBehaveSkillTargetInfoList[tbstiIndex];
                            if (triggerSkillTargetInfo.SNodeId == selectTargetInfo.SNodeId)
                            {
                                isExistSNodeId = true;
                                break;
                            }
                        }
                        if (!isExistSNodeId)
                            return;
                    }

                    if (turnBehaveSkillInfo.SkillId != combatHpChangeData.m_BuffSkillId)
                    {
                        DebugUtil.LogError($"turnBehaveSkillInfo.SkillId:{turnBehaveSkillInfo.SkillId.ToString()} != combatHpChangeData.m_BuffSkillId:{combatHpChangeData.m_BuffSkillId.ToString()}");
                        return;
                    }
                    
                    MobEntity behaveMob = null;
                    if (srcUnitId != mob.m_MobCombatComponent.m_BattleUnit.UnitId)
                        behaveMob = MobManager.Instance.GetMob(srcUnitId);
                    else
                        behaveMob = mob;

                    if (behaveMob == null)
                        return;

                    combatHpChangeData.m_IsBeUse = true;

                    BehaveAIControllParam behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
                    behaveAIControllParam.SrcUnitId = srcUnitId;
                    behaveAIControllParam.SkillId = combatHpChangeData.m_BuffSkillId;
                    behaveAIControllParam.ExcuteTurnIndex = excuteTurnIndex;
                    behaveAIControllParam.TargetUnitId = mob.m_MobCombatComponent.m_BattleUnit.UnitId;
                    behaveMob.m_MobCombatComponent.StartBehave(combatHpChangeData.m_BuffSkillId, turnBehaveSkillInfo,
                            turnBehaveSkillInfo.IsConverAttack, behaveAIControllParam);
                }
            }
        }
    }

    private TurnBehaveSkillInfo GetTurnBehaveSkillInfoInSNodeInfo(List<SNodeInfo> snodeInfoList, int edIndex, 
        int layer, ref uint srcUnitId, out TurnBehaveSkillTargetInfo targetInfo)
    {
        targetInfo = null;

        if (snodeInfoList == null)
            return null;

        int ttiCount = snodeInfoList.Count;
        if (ttiCount > 0)
        {
            for (int ttiIndex = 0; ttiIndex < ttiCount; ttiIndex++)
            {
                SNodeInfo tti = snodeInfoList[ttiIndex];
                if (tti == null || tti.TurnBehaveInfoList == null)
                    continue;

                if (tti.Layer != layer)
                {
                    DebugUtil.LogError($"SNodeInfo的Layer:{tti.Layer.ToString()} != {layer.ToString()}  SNodeId:{tti.SNodeId.ToString()}");
                    continue;
                }
                
                TurnBehaveSkillInfo turnBehaveSkillInfo = GetTurnBehaveSkillInfoByEdIndex(tti.TurnBehaveInfoList, edIndex, 
                    ref srcUnitId, out targetInfo);
                if (turnBehaveSkillInfo != null)
                    return turnBehaveSkillInfo;
                
                GetTurnBehaveSkillInfoInSNodeInfo(tti.ChildSNodeInfoList, edIndex, layer + 1, ref srcUnitId, out targetInfo);
            }
        }

        return null;
    }

    private TurnBehaveSkillInfo GetTurnBehaveSkillInfoByEdIndex(List<TurnBehaveInfo> turnBehaveInfoList, int edIndex, 
        ref uint srcUnitId, out TurnBehaveSkillTargetInfo targetInfo)
    {
        targetInfo = null;

        if (turnBehaveInfoList == null)
            return null;

        int tbiCount = turnBehaveInfoList.Count;
        if (tbiCount > 0)
        {
            for (int tbiIndex = 0; tbiIndex < tbiCount; tbiIndex++)
            {
                TurnBehaveInfo turnBehaveInfo = turnBehaveInfoList[tbiIndex];
                if (turnBehaveInfo == null)
                    continue;

                TurnBehaveSkillTargetInfo tbsti;
                TurnBehaveSkillInfo tbsi = turnBehaveInfo.GetTurnBehaveSkillByEdIndex(edIndex, out tbsti);
                if (tbsi != null)
                {
                    srcUnitId = turnBehaveInfo.SrcUnitId;
                    targetInfo = tbsti;
                    return tbsi;
                }
            }
        }

        return null;
    }

    public void DoPassive(int excuteTurnIndex, TurnBehaveSkillInfo turnBehaveSkillInfo, int turnBehaveSkillTargetInfoIndex, uint effectTrigger)
    {
        if (excuteTurnIndex > -1 &&
            turnBehaveSkillInfo != null &&
            turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList != null &&
            turnBehaveSkillTargetInfoIndex > -1)
        {
            TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[turnBehaveSkillTargetInfoIndex];
            if (turnBehaveSkillTargetInfo != null)
                DoPassive(excuteTurnIndex, turnBehaveSkillTargetInfo.SNodeId, turnBehaveSkillTargetInfo.SNodeLayer, effectTrigger);
        }
    }

    public void DoPassive(int excuteTurnIndex, uint snodeId, int snodeLayer, uint effectTrigger)
    {
        DLogManager.Log(ELogType.eCombatBehave, $"DoPassive----<color=yellow>excuteTurnIndex:{excuteTurnIndex.ToString()}   snodeId:{snodeId.ToString()}  snodeLayer:{snodeLayer.ToString()}   effectTrigger:{effectTrigger.ToString()}</color>");

        if (_excuteTurnList == null)
            return;

        if (excuteTurnIndex >= _excuteTurnList.Count)
            return;

        ExcuteTurn excuteTurn = _excuteTurnList[excuteTurnIndex];
        if (excuteTurn == null || excuteTurn.ExcuteData == null)
            return;

        int excuteDataCount = excuteTurn.ExcuteData.Count;
        if (excuteDataCount <= 0)
            return;

        NetExcuteTurnInfo netExcuteTurnInfo = GetNetExcuteTurnInfo(excuteTurnIndex);
        if (netExcuteTurnInfo == null)
            return;

        if (netExcuteTurnInfo.AttackBoLifeCycleInfoList != null)
        {
            SNodeInfo snodeInfo = GetSNodeInfo(netExcuteTurnInfo.AttackBoLifeCycleInfoList, snodeId, snodeLayer, 1, out AttackBoLifeCycleInfo attackBoLifeCycleInfo);
            if (snodeInfo == null)
            {
                if (snodeId != uint.MaxValue || snodeLayer > 1)
                    DebugUtil.LogError($"excuteTurnIndex:{excuteTurnIndex.ToString()}获取时机snodeId：{snodeId.ToString()}  snodeLayer:{snodeLayer.ToString()}数据失败");
                return;
            }

#if DEBUG_MODE
            if (snodeInfo.LeftStartIndex > snodeInfo.LeftEndIndex ||
                snodeInfo.LeftEndIndex > snodeInfo.RightStartIndex ||
                snodeInfo.RightStartIndex > snodeInfo.RightEndIndex)
                DebugUtil.LogError($"SNodeInfo时机Id:{snodeInfo.SNodeId.ToString()}数据不对, LeftStartIndex:{snodeInfo.LeftStartIndex.ToString()}  LeftEndIndex:{snodeInfo.LeftEndIndex.ToString()}  RightStartIndex:{snodeInfo.RightStartIndex.ToString()}  RightEndIndex:{snodeInfo.RightEndIndex.ToString()}");
#endif

            #region 查询出被动触发的时机在哪个位置
            int needDoExcuteDataEndIndex = -1;

            if (snodeInfo.CurDoExcuteDataIndex < 0)
                snodeInfo.CurDoExcuteDataIndex = snodeInfo.LeftStartIndex;
            else
                CheckTimingExcuteDataIndex(snodeInfo, ref snodeInfo.CurDoExcuteDataIndex);

            int timingStartIndex = snodeInfo.CurDoExcuteDataIndex;
            bool triggerUnitStart = true;
            while (timingStartIndex <= snodeInfo.RightEndIndex)
            {
                if (timingStartIndex >= excuteDataCount)
                    break;

                GetNeedDoExcuteDataEndIndex(excuteTurn, timingStartIndex, effectTrigger, true, ref needDoExcuteDataEndIndex, ref triggerUnitStart);

                //if (GetNeedDoExcuteDataEndIndex(excuteTurn, timingStartIndex, effectTrigger, true, ref needDoExcuteDataEndIndex, ref triggerUnitStart))
                //{
                //    DLogManager.Log(ELogType.eCombatBehave, $"时机查找----<color=yellow>在timingTurnInfo时机Id：{timingTurnInfo.TimingLifeId.ToString()}找到被动时机effect_trigger：{effectTrigger.ToString()}  timingTurnInfo.CurDoExcuteDataIndex:{timingTurnInfo.CurDoExcuteDataIndex.ToString()}   needDoExcuteDataEndIndex:{needDoExcuteDataEndIndex.ToString()}  timingStartIndex:{timingStartIndex.ToString()}</color>");
                //    break;
                //}

                ++timingStartIndex;
                CheckTimingExcuteDataIndex(snodeInfo, ref timingStartIndex);
            }
#if DEBUG_MODE
            if (!triggerUnitStart)
                DebugUtil.LogError($"被动PassiveTri数据不对----snodeId:{snodeId.ToString()}   snodeLayer:{snodeLayer.ToString()}  excuteTurnIndex:{excuteTurnIndex.ToString()}  effectTrigger:{effectTrigger.ToString()}  snodeInfo.SNodeId:{snodeInfo.SNodeId.ToString()}    LeftStartIndex:{snodeInfo.LeftStartIndex.ToString()}  LeftEndIndex:{snodeInfo.LeftEndIndex.ToString()}  RightStartIndex:{snodeInfo.RightStartIndex.ToString()}  RightEndIndex:{snodeInfo.RightEndIndex.ToString()}");
#endif

            if (needDoExcuteDataEndIndex < 0)
            {
                DLogManager.Log(ELogType.eCombatBehave, $"时机查找----<color=red>在snodeInfo时机Id：{snodeInfo.SNodeId.ToString()}没有找到被动时机effect_trigger：{effectTrigger.ToString()}</color>");
                return;
            }
            #endregion

            #region 从上次的位置开始-到-上面查询被动触发时机的位置依次执行ExcuteData数据,如果其中包含需要触发的子主动技能需要全部执行表现
            
            timingStartIndex = snodeInfo.CurDoExcuteDataIndex;
            while (timingStartIndex <= needDoExcuteDataEndIndex)
            {
                if (timingStartIndex >= excuteDataCount)
                    break;

                ExcuteData ed = excuteTurn.ExcuteData[timingStartIndex];
                if (ed != null)
                {
                    GetParseExcuteDataExtendId(ed.ExtendId, out uint srcUnitId, out uint extendType, out uint extendId);
                    DLogManager.Log(ELogType.eCombatBehave, $"DoPassive[{excuteTurnIndex.ToString()}]---<color=yellow>edIndex:{timingStartIndex.ToString()}    ExtendId：{ed.ExtendId.ToString()}    srcUnitId:{srcUnitId.ToString()}    extendType:{extendType.ToString()}  extendId:{extendId.ToString()}</color>");
                    
                    bool isHave = false;
                    bool isAttackSkillSuccess = false;
                    HandleExcuteData(excuteTurn, excuteTurnIndex, ed, timingStartIndex, netExcuteTurnInfo, attackBoLifeCycleInfo, snodeInfo, srcUnitId, extendType, extendId, true, null, 1,
                        ref isHave, ref isAttackSkillSuccess);
                    
                    if (extendType == 1u && ed.PassiveTri != null)
                    {
                        bool isExistTri = false;
                        for (int tempTriIndex = 0, tempTriCount = _tempList.Count; tempTriIndex < tempTriCount; tempTriIndex++)
                        {
                            if (_tempList[tempTriIndex] == ed.PassiveTri.TriggerId)
                            {
                                isExistTri = true;
                                break;
                            }
                        }
                        if (!isExistTri)
                        {
                            if (ed.PassiveTri.IsPassive)
                                CombatManager.Instance.ShowPassiveName(ed.PassiveTri.SrcUnitId, extendId);

                            _tempList.Add(ed.PassiveTri.TriggerId);
                        }

                        if (timingStartIndex + 1 < excuteDataCount)
                        {
                            ExcuteData nextEd = excuteTurn.ExcuteData[timingStartIndex + 1];
                            if (nextEd.Node > 0)
                            {
                                if (snodeInfo.RightEndIndex > timingStartIndex + 1)
                                {
                                    PerformTriggerEffectForActive(excuteTurnIndex, snodeInfo.TurnBehaveInfoList, nextEd.Node);

                                    SNodeInfo nextNodeTti = GetSNodeInfoBySNodeId(snodeInfo.ChildSNodeInfoList, nextEd.Node);
                                    if (nextNodeTti == null)
                                    {
                                        timingStartIndex = snodeInfo.RightEndIndex;
                                        DebugUtil.LogError($"DoPassive[{excuteTurnIndex.ToString()}]---edIndex:{(timingStartIndex + 1).ToString()}没有获取到SNodeId:{nextEd.Node.ToString()}数据");
                                    }
                                    else
                                    {
                                        timingStartIndex = nextNodeTti.RightEndIndex + 1;
                                        CheckTimingExcuteDataIndex(snodeInfo, ref timingStartIndex);
                                        if (timingStartIndex >= snodeInfo.RightStartIndex)
                                            timingStartIndex = snodeInfo.RightEndIndex;
                                    }

                                    CheckTimingExcuteDataIndex(snodeInfo, ref timingStartIndex);

#if DEBUG_MODE
                                    DLogManager.Log(ELogType.eCombatBehave, $"执行了PerformTriggerEffectForActive---<color=yellow>timingStartIndex直接跳转到{timingStartIndex.ToString()}</color>");
#endif

                                    continue;
                                }
                            }
                        }
                    }
                }
                
                ++timingStartIndex;
                CheckTimingExcuteDataIndex(snodeInfo, ref timingStartIndex);
            }

            snodeInfo.CurDoExcuteDataIndex = timingStartIndex;

            _tempList.Clear();
            #endregion
        }
    }

    public void DoTriggerTimingEffect(int excuteTurnIndex, uint stage, int bo, uint effectTrigger)
    {
        if (bo < 1 && stage == 0u)
        {
            DebugUtil.LogError($"DoTriggerTimingEffect需要触发的bo：{bo.ToString()}   stage:{stage.ToString()}不对");
            return;
        }

        if (_excuteTurnList == null)
            return;

        if (excuteTurnIndex >= _excuteTurnList.Count)
            return;

        ExcuteTurn excuteTurn = _excuteTurnList[excuteTurnIndex];
        if (excuteTurn == null || excuteTurn.ExcuteData == null)
            return;

        int excuteDataCount = excuteTurn.ExcuteData.Count;
        if (excuteDataCount <= 0)
            return;

        NetExcuteTurnInfo netExcuteTurnInfo = GetNetExcuteTurnInfo(excuteTurnIndex);
        if (netExcuteTurnInfo == null)
            return;

        OutBoInfo outBoInfo = GetOutBoInfo(netExcuteTurnInfo.AttackBoLifeCycleInfoList, stage, bo, out AttackBoLifeCycleInfo attackBoLifeCycleInfo);
        if (outBoInfo == null)
        {
            DebugUtil.Log(ELogType.eCombatBehave, $"未获取到波外数据----<color=red>excuteTurnIndex:{excuteTurnIndex.ToString()}获取bo：{bo.ToString()}  effectTrigger:{effectTrigger.ToString()}</color>");
            return;
        }
        
        #region 查询出被动触发的时机在哪个位置
        int needDoExcuteDataEndIndex = -1;
        
        CheckBoExcuteDataIndex(outBoInfo, ref outBoInfo.CurDoExcuteDataIndex);

        int startIndex = outBoInfo.CurDoExcuteDataIndex;
        bool triggerUnitStart = true;
        while (startIndex <= outBoInfo.RightEndIndex)
        {
            if (startIndex >= excuteDataCount)
                break;

            GetNeedDoExcuteDataEndIndex(excuteTurn, startIndex, effectTrigger, false, ref needDoExcuteDataEndIndex, ref triggerUnitStart);

            //if (GetNeedDoExcuteDataEndIndex(excuteTurn, startIndex, effectTrigger, false, ref needDoExcuteDataEndIndex, ref triggerUnitStart))
            //{
            //    DLogManager.Log(ELogType.eCombatBehave, $"波数数据外的查找----<color=yellow>在outBoInfo波数boId：{outBoInfo.Bo.ToString()}找到被动时机effect_trigger：{effectTrigger.ToString()}   outBoInfo.CurDoExcuteDataIndex:{outBoInfo.CurDoExcuteDataIndex.ToString()}   needDoExcuteDataEndIndex:{needDoExcuteDataEndIndex.ToString()}  StartIndex:{startIndex.ToString()}</color>");
            //    break;
            //}

            ++startIndex;
            CheckBoExcuteDataIndex(outBoInfo, ref startIndex);
        }
#if DEBUG_MODE
        if (!triggerUnitStart)
            DebugUtil.LogError($"被动PassiveTri数据不对----stage:{stage.ToString()}   bo:{bo.ToString()}  excuteTurnIndex:{excuteTurnIndex.ToString()}  effectTrigger:{effectTrigger.ToString()}");
#endif

        if (needDoExcuteDataEndIndex < 0)
        {
            DLogManager.Log(ELogType.eCombatBehave, $"波数数据外的查找----<color=red>在outBoInfo波数boId：{outBoInfo.Bo.ToString()}没有找到被动时机effect_trigger：{effectTrigger.ToString()}</color>");
            return;
        }
        #endregion

        #region 从上次的位置开始-到-上面查询被动触发时机的位置依次执行ExcuteData数据
        startIndex = outBoInfo.CurDoExcuteDataIndex;
        while (startIndex <= needDoExcuteDataEndIndex)
        {
            if (startIndex >= excuteDataCount)
                break;

            ExcuteData ed = excuteTurn.ExcuteData[startIndex];
            if (ed != null)
            {
                GetParseExcuteDataExtendId(ed.ExtendId, out uint srcUnitId, out uint extendType, out uint extendId);
                DLogManager.Log(ELogType.eCombatBehave, $"DoTriggerTimingEffect----DoPassive[{excuteTurnIndex.ToString()}]---<color=yellow>edIndex:{startIndex.ToString()}  ExtendId：{ed.ExtendId.ToString()}    srcUnitId:{srcUnitId.ToString()}    extendType:{extendType.ToString()}  extendId:{extendId.ToString()}</color>");
                
                bool isHave = false;
                bool isAttackSkillSuccess = false;
                HandleExcuteData(excuteTurn, excuteTurnIndex, ed, startIndex, netExcuteTurnInfo, attackBoLifeCycleInfo, null, srcUnitId, extendType, extendId, 
                        true, null, 2, ref isHave, ref isAttackSkillSuccess);
                
                if (extendType == 1u && ed.PassiveTri != null)
                {
                    bool isExistTri = false;
                    for (int tempTriIndex = 0, tempTriCount = _tempList.Count; tempTriIndex < tempTriCount; tempTriIndex++)
                    {
                        if (_tempList[tempTriIndex] == ed.PassiveTri.TriggerId)
                        {
                            isExistTri = true;
                            break;
                        }
                    }
                    if (!isExistTri)
                    {
                        if (ed.PassiveTri.IsPassive)
                            CombatManager.Instance.ShowPassiveName(ed.PassiveTri.SrcUnitId, extendId);

                        _tempList.Add(ed.PassiveTri.TriggerId);
                    }
                    
                    if (startIndex + 1 < excuteDataCount)
                    {
                        ExcuteData nextEd = excuteTurn.ExcuteData[startIndex + 1];
                        if (nextEd.Node > 0)
                        {
                            SNodeInfo snodeInfo = GetSNodeInfoBySNodeId(attackBoLifeCycleInfo.SNodeInfoList, nextEd.Node);
                            if (snodeInfo != null)
                            {
                                if (snodeInfo.RightEndIndex > startIndex + 1)
                                {
                                    PerformTriggerEffectForActive(excuteTurnIndex, attackBoLifeCycleInfo.m_OutBoInfo.TurnBehaveInfoList, snodeInfo.SNodeId);

                                    startIndex = snodeInfo.RightEndIndex;
                                    CheckBoExcuteDataIndex(outBoInfo, ref startIndex);
                                    continue;
                                }
#if DEBUG_MODE
                                else
                                {
                                    DebugUtil.LogError($"DoTriggerTimingEffect执行被动的主动技能时snodeInfo时机Id:{snodeInfo.SNodeId.ToString()}数据不对, LeftStartIndex:{snodeInfo.LeftStartIndex.ToString()}  LeftEndIndex:{snodeInfo.LeftEndIndex.ToString()}  RightStartIndex:{snodeInfo.RightStartIndex.ToString()}  RightEndIndex:{snodeInfo.RightEndIndex.ToString()}    timingStartIndex:{(startIndex + 1).ToString()}");
                                }
#endif
                            }
                        }
                    }
                }
            }

            ++startIndex;
            CheckBoExcuteDataIndex(outBoInfo, ref startIndex);
        }

        outBoInfo.CurDoExcuteDataIndex = startIndex;

        _tempList.Clear();
        #endregion
    }

    /// <summary>
    /// 获取当前Node是否包含反击行为
    /// </summary>
    public bool DoFightBackTurnBehaveInfo(int excuteTurnIndex, uint snodeId, int snodeLayer, uint fightBackUnitId, 
        uint beFightBackUnitId, bool isDoBehave = true)
    {
        if (_excuteTurnList == null)
            return false;

        if (excuteTurnIndex >= _excuteTurnList.Count)
            return false;

        ExcuteTurn excuteTurn = _excuteTurnList[excuteTurnIndex];
        if (excuteTurn == null || excuteTurn.ExcuteData == null)
            return false;

        int excuteDataCount = excuteTurn.ExcuteData.Count;
        if (excuteDataCount <= 0)
            return false;

        NetExcuteTurnInfo netExcuteTurnInfo = GetNetExcuteTurnInfo(excuteTurnIndex);
        if (netExcuteTurnInfo == null)
            return false;

        if (netExcuteTurnInfo.AttackBoLifeCycleInfoList != null)
        {
            SNodeInfo snodeInfo = GetSNodeInfo(netExcuteTurnInfo.AttackBoLifeCycleInfoList, snodeId, snodeLayer, 1, out AttackBoLifeCycleInfo attackBoLifeCycleInfo);
            if (snodeInfo != null && snodeInfo.TurnBehaveInfoList != null && snodeInfo.TurnBehaveInfoList.Count > 0)
            {
                for (int tbiIndex = 0, tbiCount = snodeInfo.TurnBehaveInfoList.Count; tbiIndex < tbiCount; tbiIndex++)
                {
                    TurnBehaveInfo turnBehaveInfo = snodeInfo.TurnBehaveInfoList[tbiIndex];
                    if (turnBehaveInfo == null || turnBehaveInfo.TurnBehaveSkillInfoList == null ||
                        turnBehaveInfo.TurnBehaveSkillInfoList.Count < 1)
                        continue;

                    MobEntity fightBackMob = MobManager.Instance.GetMob(turnBehaveInfo.SrcUnitId);
                    if (fightBackMob == null)
                        continue;

                    for (int tbsiIndex = 0, tbsiCount = turnBehaveInfo.TurnBehaveSkillInfoList.Count; tbsiIndex < tbsiCount; tbsiIndex++)
                    {
                        TurnBehaveSkillInfo turnBehaveSkillInfo = turnBehaveInfo.TurnBehaveSkillInfoList[tbsiIndex];
                        if (turnBehaveSkillInfo != null && turnBehaveSkillInfo.ExtendType == 5u)
                        {
                            DLogManager.Log(ELogType.eCombatBehave, $"FightBack----{(fightBackMob.m_Go == null ? null : fightBackMob.m_Go.name)}----<color=yellow>excuteTurnIndex:{excuteTurnIndex.ToString()}   snodeId:{snodeId.ToString()}  snodeLayer:{snodeLayer.ToString()}获取到下层级node包含反击行为数据</color>");

#if DEBUG_MODE
                            if (turnBehaveInfo.SrcUnitId != fightBackUnitId)
                            {
                                DebugUtil.LogError($"{(fightBackMob.m_Go == null ? null : fightBackMob.m_Go.name)}----excuteTurnIndex:{excuteTurnIndex.ToString()}   snodeId:{snodeId.ToString()}  snodeLayer:{snodeLayer.ToString()}反击数据中turnBehaveInfo.SrcUnitId:{turnBehaveInfo.SrcUnitId.ToString()}和反击者{fightBackUnitId.ToString()}不一致");
                            }
                            else 
                            {
                                if (turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList == null ||
                                    turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count < 1)
                                {
                                    DebugUtil.LogError($"{(fightBackMob.m_Go == null ? null : fightBackMob.m_Go.name)}----excuteTurnIndex:{excuteTurnIndex.ToString()}   snodeId:{snodeId.ToString()}  snodeLayer:{snodeLayer.ToString()}反击数据fightBackUnitId:{fightBackUnitId.ToString()}  beFightBackUnitId:{beFightBackUnitId.ToString()}中TurnBehaveSkillTargetInfoList为空");
                                }
                                else
                                {
                                    for (int tbstiIndex = 0, tbstiCount = turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count; tbstiIndex < tbstiCount; tbstiIndex++)
                                    {
                                        var tbsti = turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[tbstiIndex];
                                        if (tbsti == null || tbsti.TargetUnitId != beFightBackUnitId)
                                        {
                                            DebugUtil.LogError($"{(fightBackMob.m_Go == null ? null : fightBackMob.m_Go.name)}----excuteTurnIndex:{excuteTurnIndex.ToString()}   snodeId:{snodeId.ToString()}  snodeLayer:{snodeLayer.ToString()}反击数据fightBackUnitId:{fightBackUnitId.ToString()}  beFightBackUnitId:{beFightBackUnitId.ToString()}中目标为tbsti.TargetUnitId:{tbsti?.TargetUnitId.ToString()}");
                                        }
                                    }
                                }
                            }
#endif

                            if (isDoBehave)
                            {
                                bool isChildHaveBehave = false;
                                return PerformBehave(turnBehaveSkillInfo, fightBackMob, excuteTurnIndex,
                                    false, false, 5, ref isChildHaveBehave, StartControllerStyleEnum.Insert_MainQueue);
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    private void GetNeedDoExcuteDataEndIndex(ExcuteTurn excuteTurn, int timingStartIndex, uint effectTrigger, bool isTriggerPassive, 
        ref int needDoExcuteDataEndIndex, ref bool triggerUnitStart)
    {
        ExcuteData ed = excuteTurn.ExcuteData[timingStartIndex];
        if (ed != null)
        {
            GetParseExcuteDataExtendId(ed.ExtendId, out uint srcUnitId, out uint extendType, out uint extendId);

            if (extendType == 1 && extendId > 0u)
            {
                CSVPassiveSkill.Data pssiveSkillTb = CSVPassiveSkill.Instance.GetConfData(extendId);
                if (pssiveSkillTb != null)
                {
                    uint tbClientTrigger = CombatHelp.EffectTriggerTypeServerToClient(pssiveSkillTb.effect_trigger);
                    uint checkClientTrigger = CombatHelp.EffectTriggerTypeServerToClient(effectTrigger);
                    if ((isTriggerPassive && (tbClientTrigger == checkClientTrigger || tbClientTrigger >= CombatHelp.EffectTriggerTypeServerToClient(3u))) || !isTriggerPassive)
                    {
                        if (tbClientTrigger < checkClientTrigger)
                            needDoExcuteDataEndIndex = timingStartIndex;
                        else if (tbClientTrigger == checkClientTrigger)
                        {
                            if (ed.PassiveTri != null)
                            {
                                needDoExcuteDataEndIndex = timingStartIndex;

                                if (triggerUnitStart)
                                    triggerUnitStart = false;
                                else
                                {
                                    DLogManager.Log(ELogType.eCombatBehave, $"GetNeedDoExcuteDataEndIndex----<color=yellow>tbClientTrigger:{tbClientTrigger.ToString()}   checkClientTrigger:{checkClientTrigger.ToString()}   needDoExcuteDataEndIndex:{needDoExcuteDataEndIndex.ToString()}</color>");

                                    triggerUnitStart = true;
                                }
                            }
                        }
                        //else
                        //    return true;
                    }
                }
            }
        }
    }

    private void PerformTriggerEffectForActive(int excuteTurnIndex, List<TurnBehaveInfo> turnBehaveInfoList, uint snodeId)
    {
        if (turnBehaveInfoList == null)
            return;

        int tbiCount = turnBehaveInfoList.Count;
        if (tbiCount > 0)
        {
            for (int tbiIndex = 0; tbiIndex < tbiCount; tbiIndex++)
            {
                TurnBehaveInfo turnBehaveInfo = turnBehaveInfoList[tbiIndex];
                if (turnBehaveInfo == null || turnBehaveInfo.TurnBehaveSkillInfoList == null)
                    continue;

                int tbsiCount = turnBehaveInfo.TurnBehaveSkillInfoList.Count;
                if (tbsiCount < 1)
                    continue;

                MobEntity tti_tbi_attackMob = MobManager.Instance.GetMob(turnBehaveInfo.SrcUnitId);

                for (int tbsiIndex = 0; tbsiIndex < tbsiCount; tbsiIndex++)
                {
                    TurnBehaveSkillInfo turnBehaveSkillInfo = turnBehaveInfo.TurnBehaveSkillInfoList[tbsiIndex];
                    if (turnBehaveSkillInfo == null || turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList == null ||
                        turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count < 1 && turnBehaveSkillInfo.ExtendType != 0)
                        continue;

                    if (turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[0].SNodeId == snodeId)
                    {
                        bool isChildHaveBehave = false;
                        PerformBehave(turnBehaveSkillInfo, tti_tbi_attackMob, excuteTurnIndex,
                            true, false, 0, ref isChildHaveBehave);

#if DEBUG_MODE
                        if (tti_tbi_attackMob != null && tti_tbi_attackMob.m_MobCombatComponent != null)
                            tti_tbi_attackMob.m_MobCombatComponent.m_isHaveChildLayerActive = false;
#endif

                        continue;
                    }
                }
            }
        }
    }

    private void CheckTimingExcuteDataIndex(SNodeInfo snodeInfo, ref int checkExcuteDataIndex)
    {
        if (snodeInfo.ChildSNodeInfoList != null)
        {
            int childSNodeInfoCount = snodeInfo.ChildSNodeInfoList.Count;
            if (childSNodeInfoCount > 0)
            {
                for (int childSNodeInfoIndex = 0; childSNodeInfoIndex < childSNodeInfoCount; childSNodeInfoIndex++)
                {
                    SNodeInfo child = snodeInfo.ChildSNodeInfoList[childSNodeInfoIndex];
                    if (checkExcuteDataIndex < child.LeftStartIndex)
                        return;
                    else if (checkExcuteDataIndex >= child.LeftStartIndex && checkExcuteDataIndex <= child.RightEndIndex)
                    {
                        checkExcuteDataIndex = child.RightEndIndex + 1;
                        return;
                    }
                }
            }
        }
    }

    private void CheckBoExcuteDataIndex(OutBoInfo outBoInfo, ref int checkExcuteDataIndex)
    {
        if (checkExcuteDataIndex < outBoInfo.LeftStartIndex)
            checkExcuteDataIndex = outBoInfo.LeftStartIndex;
        else if (checkExcuteDataIndex > outBoInfo.LeftEndIndex && checkExcuteDataIndex < outBoInfo.RightStartIndex)
            checkExcuteDataIndex = outBoInfo.RightStartIndex;
    }

    public SNodeInfo GetSNodeInfo(List<AttackBoLifeCycleInfo> attackBoLifeCycleInfoList, uint snodeId, int snodeLayer, int layerIndex, out AttackBoLifeCycleInfo attackBoLifeCycleInfo)
    {
        attackBoLifeCycleInfo = null;

        if (attackBoLifeCycleInfoList == null || attackBoLifeCycleInfoList.Count <= 0)
            return null;

        for (int ablciIndex = 0, ablciCount = attackBoLifeCycleInfoList.Count; ablciIndex < ablciCount; ablciIndex++)
        {
            AttackBoLifeCycleInfo checkAttackBoLifeCycleInfo = attackBoLifeCycleInfoList[ablciIndex];
            if (checkAttackBoLifeCycleInfo == null || checkAttackBoLifeCycleInfo.SNodeInfoList == null ||
                checkAttackBoLifeCycleInfo.SNodeInfoList.Count <= 0)
                continue;

            SNodeInfo snodeInfo = GetSNodeInfo(checkAttackBoLifeCycleInfo.SNodeInfoList, snodeId, snodeLayer, layerIndex);
            if (snodeInfo != null)
            {
                attackBoLifeCycleInfo = checkAttackBoLifeCycleInfo;
                return snodeInfo;
            }
        }

        return null;
    }

    private SNodeInfo GetSNodeInfo(List<SNodeInfo> snodeInfoList, uint snodeId, int snodeLayer, int layerIndex)
    {
        if (snodeInfoList == null)
            return null;

        for (int i = 0, count = snodeInfoList.Count; i < count; i++)
        {
            SNodeInfo snodeInfo = snodeInfoList[i];
            if (snodeInfo == null)
                continue;

            if (snodeInfo.Layer != layerIndex)
            {
                DebugUtil.LogError($"SNodeInfo时机Id:{snodeInfo.SNodeId.ToString()}在{snodeInfo.Layer.ToString()}层，实际应该处在{layerIndex.ToString()}");
                continue;
            }

            if (snodeInfo.Layer == snodeLayer)
            {
                if (snodeInfo.SNodeId == snodeId)
                    return snodeInfo;
            }
            else if (snodeInfo.Layer < snodeLayer)
            {
                if (snodeInfo.ChildSNodeInfoList == null)
                    continue;

                var childSNodeInfo = GetSNodeInfo(snodeInfo.ChildSNodeInfoList, snodeId, snodeLayer, layerIndex + 1);
                if (childSNodeInfo != null)
                    return childSNodeInfo;
            }
            else
            {
                return null;
            }
        }

        return null;
    }

    private SNodeInfo GetSNodeInfoBySNodeId(List<SNodeInfo> snodeInfoList, uint snodeId)
    {
        if (snodeInfoList == null)
            return null;

        for (int i = 0, count = snodeInfoList.Count; i < count; i++)
        {
            SNodeInfo snodeInfo = snodeInfoList[i];
            if (snodeInfo == null)
                continue;

            if (snodeInfo.SNodeId == snodeId)
                return snodeInfo;
        }

        return null;
    }

    private SNodeInfo GetSNodeInfoByDeepSNodeId(List<SNodeInfo> snodeInfoList, uint snodeId)
    {
        if (snodeInfoList == null)
            return null;

        for (int i = 0, count = snodeInfoList.Count; i < count; i++)
        {
            SNodeInfo snodeInfo = snodeInfoList[i];
            if (snodeInfo == null)
                continue;

            if (snodeInfo.SNodeId == snodeId)
                return snodeInfo;

            SNodeInfo childTti = GetSNodeInfoByDeepSNodeId(snodeInfo.ChildSNodeInfoList, snodeId);
            if (childTti != null)
                return childTti;
        }

        return null;
    }

    private OutBoInfo GetOutBoInfo(List<AttackBoLifeCycleInfo> attackBoLifeCycleInfoList, uint stage, int bo, out AttackBoLifeCycleInfo attackBoLifeCycleInfo)
    {
        attackBoLifeCycleInfo = null;

        if (attackBoLifeCycleInfoList == null || attackBoLifeCycleInfoList.Count <= 0 || (bo < 1 && stage == 0u))
            return null;

        for (int ablciIndex = 0, ablciCount = attackBoLifeCycleInfoList.Count; ablciIndex < ablciCount; ablciIndex++)
        {
            AttackBoLifeCycleInfo checkAttackBoLifeCycleInfo = attackBoLifeCycleInfoList[ablciIndex];
            if (checkAttackBoLifeCycleInfo == null || checkAttackBoLifeCycleInfo.m_OutBoInfo == null)
                continue;

            if (bo > 0)
            {
                if (checkAttackBoLifeCycleInfo.m_OutBoInfo.Bo == bo)
                {
                    attackBoLifeCycleInfo = checkAttackBoLifeCycleInfo;
                    return checkAttackBoLifeCycleInfo.m_OutBoInfo;
                }
            }
            else if (checkAttackBoLifeCycleInfo.m_OutBoInfo.Stage == stage)
            {
                attackBoLifeCycleInfo = checkAttackBoLifeCycleInfo;
                return checkAttackBoLifeCycleInfo.m_OutBoInfo;
            }
        }

        return null;
    }

    public void AddNewUnitBehaveInfoByServerNum(int serverNum, NewUnitBehaveInfo newUnit)
    {
        if (m_NewUnitBehaveInfoDic.TryGetValue(serverNum, out NewUnitBehaveInfo old) && old != null)
            old.Push();

        m_NewUnitBehaveInfoDic[serverNum] = newUnit;
    }

    public void GetNewUnitBehaveInfoByServerNum(int serverNum, out NewUnitBehaveInfo newUnitBehaveInfo)
    {
        m_NewUnitBehaveInfoDic.TryGetValue(serverNum, out newUnitBehaveInfo);
    }

    public void RemoveNewUnitBehaveInfoByServerNum(int serverNum, NewUnitBehaveInfo newUnit)
    {
        m_NewUnitBehaveInfoDic.Remove(serverNum);
        newUnit.Push();
    }

    public bool IsExistNeedNewUnitBehaveInfoEnterBattle()
    {
        foreach (var kv in m_NewUnitBehaveInfoDic)
        {
            NewUnitBehaveInfo newUnitBehaveInfo = kv.Value;
            if (newUnitBehaveInfo == null || newUnitBehaveInfo.m_newUnitId == 0u)
                continue;

            if (MobManager.Instance.GetMob(newUnitBehaveInfo.m_newUnitId) == null)
            {
                DebugUtil.Log(ELogType.eCombat, $"IsExistNeedNewUnitBehaveInfoEnterBattle----<color=yellow>m_NewUnitBehaveInfoDic中还有newUnitId:{newUnitBehaveInfo.m_newUnitId.ToString()}还在生成中...</color>");
                return true;
            }
        }

        return false;
    }

    public void CreateNewUnit(BattleUnitChange uc, BattleUnit battleUnit, int excuteTurnIndex)
    {
        for (int i = 0, count = _cacheDelayBirthNetNewUnitDatas.Count; i < count; i++)
        {
            CacheDelayBirthNetNewUnitData cacheDelayBirthNetNewUnitData = _cacheDelayBirthNetNewUnitDatas[i];
            if (cacheDelayBirthNetNewUnitData.m_BattleUnit == battleUnit)
            {
                _cacheDelayBirthNetNewUnitDatas.RemoveAt(i);

                cacheDelayBirthNetNewUnitData.Push();
                break;
            }
        }

        OnAddOrDelUnit(true, battleUnit, true);

        if (Sys_Fight.Instance != null)
        {
            NewUnitBehaveInfo newUnitBehaveInfo = BasePoolClass.Get<NewUnitBehaveInfo>();
            newUnitBehaveInfo.m_BehaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
            newUnitBehaveInfo.m_BehaveAIControllParam.ExcuteTurnIndex = excuteTurnIndex;
            newUnitBehaveInfo.m_BehaveAIControllParam.m_BattleReplaceType = uc.ReplaceType;
            newUnitBehaveInfo.m_newUnitId = battleUnit.UnitId;
            AddNewUnitBehaveInfoByServerNum(battleUnit.Pos, newUnitBehaveInfo);
            
            Sys_Fight.Instance.CallingNewBattleUnit(battleUnit);
        }
    }

    public void CreateNewUnit(TurnBehaveSkillInfo turnBehaveSkillInfo, 
        BehaveAIControllParam behaveAIControllParam, uint replaceType, BattleUnit targetBattleUnit)
    {
        for (int i = 0, count = _cacheDelayBirthNetNewUnitDatas.Count; i < count; i++)
        {
            CacheDelayBirthNetNewUnitData cacheDelayNetFlashNewUnitData = _cacheDelayBirthNetNewUnitDatas[i];
            if (cacheDelayNetFlashNewUnitData.m_BattleUnit == targetBattleUnit)
            {
                _cacheDelayBirthNetNewUnitDatas.RemoveAt(i);

                cacheDelayNetFlashNewUnitData.Push();
                break;
            }
        }

        OnAddOrDelUnit(true, targetBattleUnit, true);

        NewUnitBehaveInfo newUnitBehaveInfo = BasePoolClass.Get<NewUnitBehaveInfo>();
        newUnitBehaveInfo.m_TurnBehaveSkillInfo = turnBehaveSkillInfo;
        newUnitBehaveInfo.m_BehaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
        if (behaveAIControllParam != null)
        {
            newUnitBehaveInfo.m_BehaveAIControllParam.SrcUnitId = behaveAIControllParam.SrcUnitId;
            newUnitBehaveInfo.m_BehaveAIControllParam.SkillId = behaveAIControllParam.SkillId;
            newUnitBehaveInfo.m_BehaveAIControllParam.TurnBehaveSkillTargetInfoIndex = behaveAIControllParam.TurnBehaveSkillTargetInfoIndex;
            newUnitBehaveInfo.m_BehaveAIControllParam.ExcuteTurnIndex = behaveAIControllParam.ExcuteTurnIndex;
        }
        newUnitBehaveInfo.m_BehaveAIControllParam.TargetUnitId = targetBattleUnit.UnitId;
        newUnitBehaveInfo.m_BehaveAIControllParam.m_BattleReplaceType = replaceType;
        newUnitBehaveInfo.m_newUnitId = targetBattleUnit.UnitId;
        AddNewUnitBehaveInfoByServerNum(targetBattleUnit.Pos, newUnitBehaveInfo);

        Sys_Fight.Instance.CallingNewBattleUnit(targetBattleUnit);
    }

    public bool IsExistNewBattleUnit(uint unitId)
    {
        for (int i = 0, count = _cacheDelayBirthNetNewUnitDatas.Count; i < count; i++)
        {
            CacheDelayBirthNetNewUnitData cacheDelayNetFlashNewUnitData = _cacheDelayBirthNetNewUnitDatas[i];
            if (cacheDelayNetFlashNewUnitData.m_BattleUnit.UnitId == unitId)
                return true;
        }

        for (int buIndex = 0, buCount = Sys_Fight.Instance.battleUnits.Count; buIndex < buCount; buIndex++)
        {
            BattleUnit bu = Sys_Fight.Instance.battleUnits[buIndex];
            if (bu == null)
                continue;

            if (bu.UnitId == unitId)
                return true;
        }

        return false;
    }

    private uint GetBeProtectUnitIdByTemp(uint extraHitEffect, uint srcUnitId, uint targetUnitId)
    {
        if (extraHitEffect != (uint)HitEffectType.Protect)
            return 0u;
        else
        {
            if (_protectHpMpChangeTemp.Count > 0)
            {
                BattleHpMpChange hc = _protectHpMpChangeTemp.Dequeue();
                if (hc.UnitId == targetUnitId)
                {
                    return (uint)hc.ProtectTar;
                }
#if DEBUG_MODE
                else
                    DebugUtil.LogError($"extraHitEffect:{extraHitEffect.ToString()}获取护卫信息数据不对：srcUnitId:{srcUnitId.ToString()}  targetUnitId:{targetUnitId.ToString()}  hc.UnitId:{hc.UnitId.ToString()}");
#endif
            }
#if DEBUG_MODE
            else
            {
                DebugUtil.LogError($"extraHitEffect:{extraHitEffect.ToString()}获取护卫信息却没有数据");
            }
#endif
        }

        return 0u;
    }

    public void DoBuffChangeData(int excuteTurnIndex, TurnBehaveSkillInfo turnBehaveSkillInfo, int buffTiming)
    {
        NetExcuteTurnInfo netExcuteTurnInfo = GetNetExcuteTurnInfo(excuteTurnIndex);
        if (netExcuteTurnInfo != null)
        {
            for (int tbsiIndex = 0, tbsiCount = turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count; tbsiIndex < tbsiCount; tbsiIndex++)
            {
                TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[tbsiIndex];

                DoBuffChangeData(excuteTurnIndex, netExcuteTurnInfo, turnBehaveSkillTargetInfo, buffTiming);
            }
        }
    }

    public void DoBuffChangeData(int excuteTurnIndex, TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo, int buffTiming)
    {
        NetExcuteTurnInfo netExcuteTurnInfo = GetNetExcuteTurnInfo(excuteTurnIndex);
        DoBuffChangeData(excuteTurnIndex, netExcuteTurnInfo, turnBehaveSkillTargetInfo, buffTiming);
    }

    private void DoBuffChangeData(int excuteTurnIndex, NetExcuteTurnInfo netExcuteTurnInfo, TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo, int buffTiming)
    {
        if (netExcuteTurnInfo == null)
            return;

        SNodeInfo snodeInfo = GetSNodeInfo(netExcuteTurnInfo.AttackBoLifeCycleInfoList, turnBehaveSkillTargetInfo.SNodeId,
                    turnBehaveSkillTargetInfo.SNodeLayer, 1, out AttackBoLifeCycleInfo attackBoLifeCycleInfo);

        if (snodeInfo == null)
        {
            DebugUtil.Log(ELogType.eCombatBehave, $"<color=red>buff----DoBuffChangeData---excuteTurnIndex:{excuteTurnIndex}  buffTiming:{buffTiming.ToString()}   SNodeId:{turnBehaveSkillTargetInfo.SNodeId}  SNodeLayer:{turnBehaveSkillTargetInfo.SNodeLayer}</color>");
            return;
        }

        if (snodeInfo.m_BuffChangeDataByBuffTimingDic == null)
            return;

        if (snodeInfo.m_BuffChangeDataByBuffTimingDic.TryGetValue(buffTiming, out Queue<uint> bcq) && bcq != null)
        {
            while (bcq.Count > 0)
            {
                uint bcUnitId = bcq.Dequeue();
                var buffMob = MobManager.Instance.GetMob(bcUnitId);
                if (buffMob != null)
                {
                    buffMob.DoProcessBuffChange(buffTiming > 0);
                }
            }

            CombatObjectPool.Instance.Push(bcq);

            snodeInfo.m_BuffChangeDataByBuffTimingDic.Remove(buffTiming);
        }
    }

#if DEBUG_MODE
    private void DebugModeCheckDoSNodeState(List<AttackBoLifeCycleInfo> attackBoLifeCycleInfoList, ExcuteTurn excuteTurn)
    {
        if (attackBoLifeCycleInfoList == null)
            return;

        for (int i = 0, count = attackBoLifeCycleInfoList.Count; i < count; i++)
        {
            AttackBoLifeCycleInfo checkAttackBoLifeCycleInfo = attackBoLifeCycleInfoList[i];
            if (checkAttackBoLifeCycleInfo == null || checkAttackBoLifeCycleInfo.SNodeInfoList == null ||
                checkAttackBoLifeCycleInfo.SNodeInfoList.Count <= 0)
                continue;

            DebugModeCheckDoSNodeState(checkAttackBoLifeCycleInfo.SNodeInfoList, excuteTurn);
        }
    }

    private void DebugModeCheckDoSNodeState(List<SNodeInfo> snodeInfoList, ExcuteTurn excuteTurn)
    {
        if (snodeInfoList == null)
            return;

        for (int i = 0, count = snodeInfoList.Count; i < count; i++)
        {
            SNodeInfo snodeInfo = snodeInfoList[i];
            if (snodeInfo == null)
                continue;

            if (snodeInfo.CurDoExcuteDataIndex < snodeInfo.RightEndIndex)
            {
                _tempSb.Clear();

                if (excuteTurn != null)
                {
                    _tempSb.Append($"   ExtendID:【");
                    for (int edIndex = snodeInfo.CurDoExcuteDataIndex < 0 ? 0 : snodeInfo.CurDoExcuteDataIndex; edIndex < snodeInfo.RightEndIndex; edIndex++)
                    {
                        if (edIndex < excuteTurn.ExcuteData.Count)
                        {
                            ExcuteData excuteData = excuteTurn.ExcuteData[edIndex];
                            if (excuteData.ExtendId > 0u)
                            {
                                GetParseExcuteDataExtendId(excuteData.ExtendId, out uint srcUnitId, out uint extendType, out uint extendId);
                                _tempSb.Append($"edIndex:{edIndex.ToString()}   ExtendId:{excuteData.ExtendId.ToString()}   srcUnitId:{srcUnitId.ToString()}   extendType:{extendType.ToString()}   extendId:{extendId.ToString()}---");
                            }
                        }
                    }
                    _tempSb.Append($"】");
                }

                _tempSb.Append($"   【未执行到时机的结尾CurDoEd:{snodeInfo.CurDoExcuteDataIndex.ToString()}   RightEndIndex:{snodeInfo.RightEndIndex.ToString()}   ");

                if (snodeInfo.ActiveTurnInfoList != null)
                {
                    for (int activeIndex = 0, activeCount = snodeInfo.ActiveTurnInfoList.Count; activeIndex < activeCount; activeIndex++)
                    {
                        ActiveTurnInfo activeTurnInfo = snodeInfo.ActiveTurnInfoList[activeIndex];
                        if (activeTurnInfo == null)
                            continue;

                        MobEntity checkMob = MobManager.Instance.GetMob(activeTurnInfo.SrcUnitId);
                        if(checkMob != null)
                            _tempSb.Append($"{(checkMob.m_Go == null ? null : checkMob.m_Go.name)}---");
                        _tempSb.Append($"SrcUnitId:{activeTurnInfo.SrcUnitId.ToString()}--ActiveLogic:{activeTurnInfo.ActiveLogicId.ToString()}  ");
                    }
                }

                _tempSb.Append($"】");

                DebugUtil.LogError($"{_tempSb.ToString()};   可能是没有配置{"被动执行时机"}节点导致的");

                _tempSb.Clear();
            }
            else
            {
                DebugModeCheckDoSNodeState(snodeInfo.ChildSNodeInfoList, excuteTurn);
            }
        }
    }

    private void CheckProtectHpMpChangeData(bool isBefore)
    {
        if (_protectHpMpChangeTemp.Count > 0)
        {
            _tempSb.Clear();
            _tempSb.Append($"excuteTurnIndex:{(isBefore ? (_excuteTurnIndex - 2) : (_excuteTurnIndex - 1))}还有护卫的数据没有处理");
            while (_protectHpMpChangeTemp.Count > 0)
            {
                BattleHpMpChange battleHpMpChange = _protectHpMpChangeTemp.Dequeue();
                _tempSb.Append($"----targetUnitId:{battleHpMpChange.UnitId.ToString()}   ProtectTar:{battleHpMpChange.ProtectTar.ToString()}");
            }
            DebugUtil.LogError(_tempSb.ToString());
            _tempSb.Clear();
        }
    }
#endif
}
