using Lib.Core;
using System.Collections.Generic;
using UnityEngine;

public class WS_CombatBehaveAIDataComponent : BaseComponent<StateMachineEntity>, IAwake
{
    public class MarkData
    {
        public ushort m_NodeId;
        public string MarkFlag;

        public void Push()
        {
            m_NodeId = 0;
            MarkFlag = null;

            CombatObjectPool.Instance.Push(this);
        }
    }

    public List<WS_BA_EffectData> m_EffectDataList;
    public int m_AttackTargetIndex;
    public float m_AttackMoveSpeed;
    public MobEntity m_CurTarget;
    public float m_TimeScale = 1f;
    public uint m_SelectMoveToPointNum;
    public Vector3 m_SelectMoveToPointPos;

    public int m_LoopNodeMarkCount;
    public ushort m_LoopNodeMarkNodeId;
    public int m_LoopWorkBlockType;

    public bool m_IsAdustLocalScale;
    public Vector3 m_OrginLocalScale;
    public Transform m_Trans;

    public int HitEffectIndex;

    public MobEntity Target;
    
    private int _isCanProcessTypeAfterHit;
    public int m_IsCanProcessTypeAfterHit
    {
        get
        {
            int isCPAH = _isCanProcessTypeAfterHit;
            _isCanProcessTypeAfterHit = 0;
            return isCPAH;
        }
        set
        {
            _isCanProcessTypeAfterHit = value;
        }
    }

    private List<MarkData> _markList = new List<MarkData>();

    public Dictionary<int, uint> m_BlockInWorkIdDic = new Dictionary<int, uint>();

#if DEBUG_MODE
    public bool IsDoPollingNodeMarkNode;
#endif

    public void Awake()
    {
        if (m_EffectDataList == null)
            m_EffectDataList = new List<WS_BA_EffectData>();
        m_SelectMoveToPointNum = 100u;

        m_IsAdustLocalScale = false;
        m_Trans = ((MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent).m_Trans;
        m_OrginLocalScale = m_Trans.localScale;
    }

    public override void Dispose()
    {
        for (int i = 0, count = m_EffectDataList.Count; i < count; i++)
        {
            var ed = m_EffectDataList[i];
            FxManager.Instance.FreeFx(ed.m_EffectModelId);
            CombatObjectPool.Instance.Push(ed);
        }
        m_EffectDataList.Clear();

        m_AttackTargetIndex = 0;
        m_AttackMoveSpeed = 0f;
        m_CurTarget = null;
        m_TimeScale = 1f;
        m_SelectMoveToPointNum = 100u;

        m_LoopNodeMarkCount = 0;
        m_LoopNodeMarkNodeId = 0;
        m_LoopWorkBlockType = 0;

        if (m_IsAdustLocalScale)
        {
            m_IsAdustLocalScale = false;
            if (m_Trans != null)
            {
                m_Trans.localScale = m_OrginLocalScale;

                DLogManager.Log(ELogType.eCombat, $"{m_Trans.gameObject?.name.ToString()}进行缩放还原:{m_OrginLocalScale.ToString()}");

                m_Trans = null;
            }
        }

        HitEffectIndex = 0;

        Target = null;

        int markCount = _markList.Count;
        if (markCount > 0)
        {
            for (int i = 0; i < markCount; i++)
            {
                _markList[i].Push();
            }
            _markList.Clear();
        }

        _isCanProcessTypeAfterHit = 0;

        if (m_BlockInWorkIdDic.Count > 0)
            m_BlockInWorkIdDic.Clear();

#if DEBUG_MODE
        IsDoPollingNodeMarkNode = false;
#endif

        base.Dispose();
    }

    public void Mark(ushort nodeId, string markFlag)
    {
        if (nodeId == 0)
            return;

        for (int i = 0, markCount = _markList.Count; i < markCount; i++)
        {
            MarkData markData = _markList[i];
            if (markData.m_NodeId == nodeId)
                return;
        }

        MarkData md = CombatObjectPool.Instance.Get<MarkData>();
        md.m_NodeId = nodeId;
        md.MarkFlag = markFlag;

        _markList.Add(md);
    }

    public ushort DequeueMarkNodeId(string markFlag)
    {
        for (int i = 0, markCount = _markList.Count; i < markCount; i++)
        {
            MarkData markData = _markList[i];
            if (markData.MarkFlag == markFlag)
            {
                _markList.RemoveAt(i);
                var nodeId = markData.m_NodeId;
                markData.Push();

                return nodeId;
            }
        }

        return 0;
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

    public TurnBehaveSkillTargetInfo GetTurnBehaveSkillTargetInfo(WS_CombatBehaveAIControllerEntity cbace, MobEntity mob)
    {
        int turnBehaveSkillTargetInfoIndex;
        return GetTurnBehaveSkillTargetInfo(cbace, mob, out turnBehaveSkillTargetInfoIndex);
    }

    public TurnBehaveSkillTargetInfo GetTurnBehaveSkillTargetInfo(WS_CombatBehaveAIControllerEntity cbace, MobEntity mob, out int turnBehaveSkillTargetInfoIndex)
    {
        turnBehaveSkillTargetInfoIndex = -1;
        if (cbace.m_SourcesTurnBehaveSkillInfo == null || cbace.m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList == null ||
            cbace.m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count < 1)
            return null;
        
        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = null;
        if (cbace.m_AttachType == 0)
        {
            if (m_AttackTargetIndex == 0 && cbace.m_BehaveAIControllParam.TurnBehaveSkillTargetInfoIndex > -1 &&
                cbace.m_SourcesTurnBehaveSkillInfo.CallingTargetBattleUnitCount > 0)
            {
                turnBehaveSkillTargetInfoIndex = cbace.m_BehaveAIControllParam.TurnBehaveSkillTargetInfoIndex;
                turnBehaveSkillTargetInfo = cbace.m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[turnBehaveSkillTargetInfoIndex];
            }
            else
            {
                turnBehaveSkillTargetInfo = cbace.m_SourcesTurnBehaveSkillInfo.GetTurnBehaveSkillTargetByIndex(m_AttackTargetIndex, out turnBehaveSkillTargetInfoIndex);
            }
        }
        else if (mob.m_MobCombatComponent.m_BattleUnit != null)
        {
            int tbstiCount = cbace.m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count;

            if (cbace.m_BehaveAIControllParam != null &&
                cbace.m_BehaveAIControllParam.TurnBehaveSkillTargetInfoIndex > -1 &&
                cbace.m_BehaveAIControllParam.TurnBehaveSkillTargetInfoIndex < tbstiCount)
            {
                turnBehaveSkillTargetInfoIndex = cbace.m_BehaveAIControllParam.TurnBehaveSkillTargetInfoIndex;
                turnBehaveSkillTargetInfo = cbace.m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[cbace.m_BehaveAIControllParam.TurnBehaveSkillTargetInfoIndex];
            }
            else
            {
                DebugUtil.LogError($"GetTurnBehaveSkillTargetInfo被击者获取数据不对{mob.m_Go?.name}  m_AttachType:{cbace.m_AttachType.ToString()}  cbace.m_TurnBehaveSkillTargetInfoIndex：{cbace.m_BehaveAIControllParam.TurnBehaveSkillTargetInfoIndex.ToString()}  Count:{tbstiCount.ToString()}");
                turnBehaveSkillTargetInfo = cbace.m_SourcesTurnBehaveSkillInfo.GetTurnBehaveSkillTargetByTargetUnitId(mob.m_MobCombatComponent.m_BattleUnit.UnitId, out turnBehaveSkillTargetInfoIndex);
            }
        }

        return turnBehaveSkillTargetInfo;
    }

    public TurnBehaveSkillTargetInfo GetTurnBehaveSkillTargetInfoByIndex(WS_CombatBehaveAIControllerEntity cbace, int targetUnitIdIndex)
    {
        if (targetUnitIdIndex < 0)
            return null;

        int turnBehaveSkillTargetInfoIndex;
        return GetTurnBehaveSkillTargetInfoByIndex(cbace, targetUnitIdIndex, out turnBehaveSkillTargetInfoIndex);
    }

    public TurnBehaveSkillTargetInfo GetTurnBehaveSkillTargetInfoByIndex(WS_CombatBehaveAIControllerEntity cbace, int targetUnitIdIndex, out int turnBehaveSkillTargetInfoIndex)
    {
        turnBehaveSkillTargetInfoIndex = -1;
        if (cbace.m_SourcesTurnBehaveSkillInfo == null || cbace.m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList == null ||
            cbace.m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count < 1)
            return null;
        
        return cbace.m_SourcesTurnBehaveSkillInfo.GetTurnBehaveSkillTargetByIndex(targetUnitIdIndex, out turnBehaveSkillTargetInfoIndex);
    }
}

public class WS_BA_EffectData
{
    public ulong m_EffectModelId;
    public uint m_EffectTbId;
}