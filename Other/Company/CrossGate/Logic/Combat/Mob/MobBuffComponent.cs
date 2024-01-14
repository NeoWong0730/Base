using Lib.Core;
using Logic;
using Packet;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

public class MobBuffComponent : BaseComponent<MobEntity>, IAwake
{
    public class BuffData
    {
        public MobEntity m_MobEntity;
        public CSVBuff.Data m_BuffTb;
        //buff剩余次数(分行动次数和回合次数)
        public uint m_Count;
        //叠加次数
        public uint m_Overlay;
        public ulong m_FxId;
        public ulong m_EffectFxId;
        public uint MaxHp;
        public uint CurHp;
        public uint MaxMp;
        public uint CurMp;
        public uint MaxShield;
        public uint CurShield;
        
        private int _state;
        /// <summary>
        /// ==1生效，==2删除buff生效
        /// </summary>
        public int m_State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value == 1)
                {
                    BuffHuDUpdateEvt buffHuDUpdateEvt = CombatObjectPool.Instance.Get<BuffHuDUpdateEvt>();
                    buffHuDUpdateEvt.id = m_MobEntity.m_MobCombatComponent.m_BattleUnit.UnitId;
                    buffHuDUpdateEvt.side = m_MobEntity.m_MobCombatComponent.m_BattleUnit.Side;
                    buffHuDUpdateEvt.buffid = m_BuffTb.id;
                    buffHuDUpdateEvt.add = true;
                    buffHuDUpdateEvt.count = m_Count;
                    Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateBuffHUD, buffHuDUpdateEvt);
                    CombatObjectPool.Instance.Push(buffHuDUpdateEvt);
                }

                if (_state != value)
                {
                    _state = value;

                    if (_state < 1)
                        return;

                    if (_state == 1)
                    {
                        if (m_BuffTb.effect_parma > 0)
                        {
                            if (m_BuffTb.effect_parma == 1)
                            {
                                m_MobEntity.m_MobCombatComponent.m_AnimationComponent.PauseAll();
                            }
                            ShaderController.Instance.SwitchBuffEffect(m_MobEntity, m_BuffTb.effect_parma);
                        }
                    }
                    else if (_state == 2)
                    {
                        BuffHuDUpdateEvt buffHuDUpdateEvt = CombatObjectPool.Instance.Get<BuffHuDUpdateEvt>();
                        buffHuDUpdateEvt.id = m_MobEntity.m_MobCombatComponent.m_BattleUnit.UnitId;
                        buffHuDUpdateEvt.side = m_MobEntity.m_MobCombatComponent.m_BattleUnit.Side;
                        buffHuDUpdateEvt.buffid = m_BuffTb.id;
                        buffHuDUpdateEvt.add = false;
                        buffHuDUpdateEvt.count = 0u;
                        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateBuffHUD, buffHuDUpdateEvt);
                        CombatObjectPool.Instance.Push(buffHuDUpdateEvt);

                        if (m_BuffTb.effect_parma > 0)
                        {
                            if (!m_MobEntity.m_MobCombatComponent.m_Death && m_BuffTb.effect_parma == 1)
                                m_MobEntity.m_MobCombatComponent.m_AnimationComponent.RemovePauseAll();
                            ShaderController.Instance.RestoreMat(m_MobEntity);
                        }
                    }

                    if (_state == 1)
                    {
                        if (m_EffectFxId == 0ul && m_BuffTb.effect_parma1 > 0u)
                        {
                            uint effectFxType = m_BuffTb.effect_parma1 / 10000000u;
                            uint effectFxInfoId = m_BuffTb.effect_parma1 % 10000000u;
                            if (effectFxType == 1u)
                            {
                                m_EffectFxId = FxManager.Instance.ShowFX(effectFxInfoId, CombatManager.Instance.CombatSceneCenterPos);
                            }
                        }
                        if (m_FxId == 0ul && m_BuffTb.effectProc > 0u)
                        {
                            m_FxId = FxManager.Instance.ShowFX(m_BuffTb.effectProc, m_MobEntity.m_Trans.position, null, m_MobEntity.m_Go, m_MobEntity.m_BindGameObjectDic);
                        }
                    }
                }
            }
        }

        public BuffData Copy()
        {
            BuffData copy = CombatObjectPool.Instance.Get<BuffData>();
            copy.m_BuffTb = m_BuffTb;
            copy.m_Count = m_Count;
            copy.m_Overlay = m_Overlay;

            return copy;
        }

        public static void Push(BuffData buffData, bool isNotDisposer = true)
        {
            if (buffData.m_EffectFxId > 0ul)
                FxManager.Instance.FreeFx(buffData.m_EffectFxId);
            if (buffData.m_FxId > 0ul)
                FxManager.Instance.FreeFx(buffData.m_FxId);
            if (buffData.m_BuffTb.effecDel > 0u && isNotDisposer)
                FxManager.Instance.ShowFX(buffData.m_BuffTb.effecDel, buffData.m_MobEntity.m_Trans.position, null, null, buffData.m_MobEntity.m_BindGameObjectDic, null, -1, true);

            buffData.m_MobEntity = null;
            buffData.m_BuffTb = null;
            buffData.m_Count = 0u;
            buffData.m_Overlay = 0u;
            buffData.m_EffectFxId = 0ul;
            buffData.m_FxId = 0ul;
            buffData._state = -1;
            buffData.MaxHp = 0u;
            buffData.CurHp = 0u;
            buffData.MaxMp = 0u;
            buffData.CurMp = 0u;
            buffData.MaxShield = 0u;
            buffData.CurShield = 0u;

            CombatObjectPool.Instance.Push(buffData);
        }
    }

    public class NeedProcessBuffData
    {
        public BattleBuffChange m_BattleBuffChange;
        public bool m_IsDelayUpdate;

        public void Push()
        {
            m_BattleBuffChange = null;
            m_IsDelayUpdate = false;

            CombatObjectPool.Instance.Push(this);
        }
    }

    public List<BuffData> m_Buffs;

    private Queue<BattleBuffChange> _readyUpdateBuffChangeQueue = new Queue<BattleBuffChange>();

    private Queue<NeedProcessBuffData> _needProcessBuffChangeQueue = new Queue<NeedProcessBuffData>();

    public void Awake()
    {
        if(m_Buffs == null)
            m_Buffs = new List<BuffData>();
    }

#if DEBUG_MODE
    public int m_DisposeCause;
    private System.Text.StringBuilder _cacheSb = new System.Text.StringBuilder();
#endif
    public override void Dispose()
    {
        if (m_Buffs.Count > 0)
        {
            for (int i = 0; i < m_Buffs.Count; i++)
            {
                var buff = m_Buffs[i];
                if (buff == null)
                    continue;
                
                BuffData.Push(buff, false);
            }
            m_Buffs.Clear();
        }

        if (_readyUpdateBuffChangeQueue.Count > 0)
        {
#if DEBUG_MODE
            _cacheSb.Clear();
            _cacheSb.Append($"{(m_CurUseEntity?.m_Go == null ? null : m_CurUseEntity?.m_Go.name)}还有{_readyUpdateBuffChangeQueue.Count.ToString()}个buff未更新,具体为：");
            while (_readyUpdateBuffChangeQueue.Count > 0)
            {
                BattleBuffChange bbc = _readyUpdateBuffChangeQueue.Dequeue();
                _cacheSb.Append($"[UnitId:{bbc.UnitId.ToString()}  BuffId:{bbc.BuffId.ToString()}  OddNum:{bbc.OddNum.ToString()}]  ");
            }

            if (m_DisposeCause == 1)
                DLogManager.Log(ELogType.eCombat, $"<color=red>因相同UnitId或相同位置清除数据：{_cacheSb.ToString()}</color>");
            else
                DebugUtil.LogError(_cacheSb.ToString());
#endif

            _readyUpdateBuffChangeQueue.Clear();
        }
        
        if (_needProcessBuffChangeQueue.Count > 0)
        {
#if DEBUG_MODE
            _cacheSb.Clear();
            _cacheSb.Append($"{(m_CurUseEntity?.m_Go == null ? null : m_CurUseEntity?.m_Go.name)}还有{_needProcessBuffChangeQueue.Count.ToString()}个buff未执行,具体为：");
#endif
            while (_needProcessBuffChangeQueue.Count > 0)
            {
                NeedProcessBuffData needProcessBuffData = _needProcessBuffChangeQueue.Dequeue();
                if (needProcessBuffData == null)
                    continue;

#if DEBUG_MODE
                _cacheSb.Append($"[UnitId:{needProcessBuffData.m_BattleBuffChange.UnitId.ToString()}  BuffId:{needProcessBuffData.m_BattleBuffChange.BuffId.ToString()}  OddNum:{needProcessBuffData.m_BattleBuffChange.OddNum.ToString()}  IsDelayUpdate:{needProcessBuffData.m_IsDelayUpdate.ToString()}]  ");
#endif

                needProcessBuffData.Push();
            }

#if DEBUG_MODE
            if (m_DisposeCause == 1)
                DLogManager.Log(ELogType.eCombat, $"<color=red>因相同UnitId或相同位置清除数据：{_cacheSb.ToString()}</color>");
            else
                DebugUtil.LogError(_cacheSb.ToString());
#endif
        }

#if DEBUG_MODE
        m_DisposeCause = 0;
#endif

        base.Dispose();
    }

    public void CacheProcessBuffChange(BattleBuffChange bc, bool isDelayUpdate)
    {
        NeedProcessBuffData needProcessBuffData = CombatObjectPool.Instance.Get<NeedProcessBuffData>();
        needProcessBuffData.m_BattleBuffChange = bc;
        needProcessBuffData.m_IsDelayUpdate = isDelayUpdate;

        _needProcessBuffChangeQueue.Enqueue(needProcessBuffData);
    }
    
    public void DoProcessBuffChange(bool isForceUpdate)
    {
        if (_needProcessBuffChangeQueue.Count > 0)
        {
            NeedProcessBuffData needProcessBuffData = _needProcessBuffChangeQueue.Dequeue();
            BattleBuffChange bc = needProcessBuffData.m_BattleBuffChange;

            bool needDelayUpdate = !isForceUpdate &&
                    ((m_CurUseEntity.m_MobCombatComponent != null && m_CurUseEntity.m_MobCombatComponent.m_IsStartBehave) || 
                        needProcessBuffData.m_IsDelayUpdate);

            needProcessBuffData.Push();

            DLogManager.Log(ELogType.eCombat, $"buff---{(m_CurUseEntity.m_Go == null ? null : m_CurUseEntity.m_Go.name)}-----执行缓存buff DoProcessBuffChange处理----<color=yellow>needDelayUpdate:{needDelayUpdate}   UnitId : {bc.UnitId.ToString()}  BuffId : {bc.BuffId.ToString()}  Num : {bc.OddNum.ToString()}   bc.CurHp:{bc.CurHp.ToString()}  bc.MaxHp:{bc.MaxHp.ToString()}  bc.CurMp:{bc.CurMp.ToString()}  bc.MaxMp:{bc.MaxMp.ToString()}</color>");

            UpdateBuffChange(bc, needDelayUpdate);
        }
    }

    public void UpdateBuffChangeByBehave(BattleBuffChange bc)
    {
        bool needDelayUpdate = m_CurUseEntity.m_MobCombatComponent != null &&
                                m_CurUseEntity.m_MobCombatComponent.m_IsStartBehave;

        DLogManager.Log(ELogType.eCombat, $"buff---{(m_CurUseEntity.m_Go == null ? null : m_CurUseEntity.m_Go.name)}-----执行buff UpdateBuffChangeByBehave处理----<color=yellow>needDelayUpdate:{needDelayUpdate}   UnitId : {bc.UnitId.ToString()}  BuffId : {bc.BuffId.ToString()}  Num : {bc.OddNum.ToString()}   bc.CurHp:{bc.CurHp.ToString()}  bc.MaxHp:{bc.MaxHp.ToString()}  bc.CurMp:{bc.CurMp.ToString()}  bc.MaxMp:{bc.MaxMp.ToString()}</color>");

        UpdateBuffChange(bc, needDelayUpdate);
    }

    public void UpdateBuffChange(BattleBuffChange bc, bool isDelayUpdate)
    {
        if (isDelayUpdate)
        {
            _readyUpdateBuffChangeQueue.Enqueue(bc);
        }
        else
        {
            UpdateReadyBuffData();

            UpdateBuff(bc.BuffId, bc.OddNum, bc.Overlay, bc.MaxHp, bc.CurHp, bc.MaxMp, bc.CurMp, bc.MaxShield, bc.CurShield);
        }
    }

    public void UpdateBuff(uint buffId, uint count, uint overlay,
        uint maxHp, uint curHp, uint maxMp, uint curMp, uint maxShield, uint curShield)
    {
        if (buffId == 0u)
            return;
        
        var unit = m_CurUseEntity.m_MobCombatComponent.m_BattleUnit;
        if (maxHp > 0u)
        {
            unit.MaxHp = maxHp;
        }
        if (curHp > 0u)
        {
            unit.CurHp = (int)curHp;
        }
        if (maxMp > 0u)
        {
            unit.MaxMp = maxMp;
        }
        if (curMp > 0u)
        {
            unit.CurMp = (int)curMp;
        }

        if (maxHp > 0u || curHp > 0)
            Net_Combat.Instance.UpdateHp(unit, unit.CurHp, unit.MaxHp);

        if (maxMp > 0 || curMp > 0u)
            Net_Combat.Instance.UpdateMp(unit, unit.CurMp, unit.MaxMp);

        if (maxShield > 0 || curShield > 0)
            Net_Combat.Instance.UpdateShield(unit, unit.CurShield, unit.MaxShield);

        BuffData buffData = null;
        for (int i = 0, buffsCount = m_Buffs.Count; i < buffsCount; i++)
        {
            var buff = m_Buffs[i];
            if (buff == null)
                continue;

            if (buff.m_BuffTb.id == buffId)
            {
                buffData = buff;
                break;
            }
        }
        
        if (count == 0u)
        {
            if (buffData != null)
            {
                DLogManager.Log(ELogType.eCombat, $"buff内部---{(m_CurUseEntity.m_Go == null ? null : m_CurUseEntity.m_Go.name)}---移除Buff---<color=yellow>ClientNum:{m_CurUseEntity.m_MobCombatComponent.m_ClientNum.ToString()} buffId : {buffId.ToString()}</color>");

                buffData.m_State = 2;
                BuffData.Push(buffData);

                for (int buffsIndex = 0, buffsCount = m_Buffs.Count; buffsIndex < buffsCount; buffsIndex++)
                {
                    if (m_Buffs[buffsIndex] == buffData)
                    {
                        m_Buffs.RemoveAt(buffsIndex);
                        break;
                    }
                }
            }
            
            return;
        }
        
        if (buffData != null)
        {
            DLogManager.Log(ELogType.eCombat, $"buff内部---{(m_CurUseEntity.m_Go == null ? null : m_CurUseEntity.m_Go.name)}---在原来的基础上添加Buff---<color=yellow>ClientNum:{m_CurUseEntity.m_MobCombatComponent.m_ClientNum.ToString()} buffId : {buffId.ToString()}  OldCount : {buffData.m_Count.ToString()}   NewCount : {count.ToString()}   oldOverlay:{buffData.m_Overlay.ToString()}    newOverlay:{overlay.ToString()}</color>");

            if (count > buffData.m_Count &&
                buffData.m_BuffTb != null && buffData.m_BuffTb.effectAdd > 0u)
            {
                FxManager.Instance.ShowFX(buffData.m_BuffTb.effectAdd, m_CurUseEntity.m_Trans.position, null, null, m_CurUseEntity.m_BindGameObjectDic, null, -1, true);
            }
            buffData.m_Count = count;
            buffData.m_Overlay = overlay;
            buffData.m_State = 1;
            return;
        }

        var buffTb = CSVBuff.Instance.GetConfData(buffId);
        if (buffTb == null)
        {
            DebugUtil.LogError($"CSVBuff表中没有BuffId:{buffId.ToString()}");
            return;
        }

        DLogManager.Log(ELogType.eCombat, $"buff内部---{(m_CurUseEntity.m_Go == null ? null : m_CurUseEntity.m_Go.name)}---新添加Buff----<color=yellow>ClientNum:{m_CurUseEntity.m_MobCombatComponent.m_ClientNum.ToString()} buffId : {buffId.ToString()}  CreateCount : {count.ToString()}   Overlay:{overlay.ToString()}</color>");

        if (buffTb.effectAdd > 0u)
            FxManager.Instance.ShowFX(buffTb.effectAdd, m_CurUseEntity.m_Trans.position, null, null, m_CurUseEntity.m_BindGameObjectDic, null, -1, true);

        buffData = CombatObjectPool.Instance.Get<BuffData>();
        buffData.m_Overlay = overlay;
        buffData.m_MobEntity = m_CurUseEntity;
        buffData.m_BuffTb = buffTb;
        buffData.m_Count = count;
        buffData.m_State = 1;
        
        m_Buffs.Add(buffData);
    }
    
    public void UpdateReadyBuffData()
    {
        if (_readyUpdateBuffChangeQueue.Count > 0)
        {
            while (_readyUpdateBuffChangeQueue.Count > 0)
            {
                BattleBuffChange bc = _readyUpdateBuffChangeQueue.Dequeue();
                if (bc == null)
                    continue;

                UpdateBuff(bc.BuffId, bc.OddNum, bc.Overlay, bc.MaxHp, bc.CurHp, bc.MaxMp, bc.CurMp, bc.MaxShield, bc.CurShield);
            }
        }
    }
    
    public uint GetBuffMaxLevelPriority()
    {
        uint buffState = 0u;
        if (m_Buffs != null)
        {
            int buffCount = m_Buffs.Count;
            if (buffCount > 0)
            {
                uint maxLvPriority = 0u;
                for (int i = 0; i < buffCount; i++)
                {
                    var buff = m_Buffs[i];
                    if (buff == null)
                        continue;

                    if (buff.m_BuffTb.buff_state > 0u)
                    {
                        uint mlp = CombatHelp.BuffStateToLevelPriority(buff.m_BuffTb.buff_state);
                        if (mlp > maxLvPriority)
                        {
                            maxLvPriority = mlp;
                            buffState = buff.m_BuffTb.buff_state;
                        }
                    }
                }
            }
        }
        
        return buffState;
    }

    public void ClearAllBuffs()
    {
        if (m_Buffs.Count > 0)
        {
            for (int i = m_Buffs.Count - 1; i > -1; --i)
            {
                var buff = m_Buffs[i];
                if (buff == null)
                    continue;

                UpdateBuff(buff.m_BuffTb.id, 0u, 0u, buff.MaxHp, buff.CurHp, buff.MaxMp, buff.CurMp, buff.MaxShield, buff.CurShield);
            }
            m_Buffs.Clear();
        }
    }

    public bool HaveBuff(uint buffid)
    {
        foreach(var buff in m_Buffs)
        {
            if(buff.m_BuffTb.id == buffid)
            {
                return true;
            }
        }
        return false;
    }

    public void CheckCacheBuff()
    {
        if (_readyUpdateBuffChangeQueue.Count > 0)
        {
#if DEBUG_MODE
            _cacheSb.Clear();
            _cacheSb.Append($"回合结束   CacheBuff--readyUpdate   {(m_CurUseEntity?.m_Go == null ? null : m_CurUseEntity?.m_Go.name)}还有{_readyUpdateBuffChangeQueue.Count}个未更新,具体为：");
            
            BattleBuffChange[] bbcList = _readyUpdateBuffChangeQueue.ToArray();
            foreach (var bbc in bbcList)
            {
                _cacheSb.Append($"[UnitId:{bbc.UnitId.ToString()}  BuffId:{bbc.BuffId.ToString()}  OddNum:{bbc.OddNum.ToString()}]  ");
            }
            bbcList = null;

            DebugUtil.LogError(_cacheSb.ToString());
#endif

            UpdateReadyBuffData();
        }

        if (_needProcessBuffChangeQueue.Count > 0)
        {
#if DEBUG_MODE
            _cacheSb.Clear();
            _cacheSb.Append($"回合结束   CacheBuff--needProcess   {(m_CurUseEntity?.m_Go == null ? null : m_CurUseEntity?.m_Go.name)}还有{_needProcessBuffChangeQueue.Count}个未执行,具体为：");
#endif

            while (_needProcessBuffChangeQueue.Count > 0)
            {
                NeedProcessBuffData needProcessBuffData = _needProcessBuffChangeQueue.Dequeue();
                if (needProcessBuffData == null)
                    continue;

                BattleBuffChange bc = needProcessBuffData.m_BattleBuffChange;

#if DEBUG_MODE
                _cacheSb.Append($"[UnitId:{bc.UnitId.ToString()}  BuffId:{bc.BuffId.ToString()}  OddNum:{bc.OddNum.ToString()}  IsDelayUpdate:{needProcessBuffData.m_IsDelayUpdate.ToString()}]  ");
#endif

                needProcessBuffData.Push();

                UpdateBuffChange(bc, false);
            }

#if DEBUG_MODE
            DebugUtil.LogError(_cacheSb.ToString());
#endif
        }
    }

    public bool CheckExistBuffEffective(int buffEffective)
    {
        foreach (var buff in m_Buffs)
        {
            if (buff.m_BuffTb.buff_effective == buffEffective)
            {
                return true;
            }
        }
        return false;
    }
}
