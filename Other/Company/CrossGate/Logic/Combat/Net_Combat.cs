using Google.Protobuf.Collections;
using Logic;
using Net;
using Packet;
using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using Table;
using Logic.Core;

public partial class Net_Combat : Logic.Singleton<Net_Combat>
{
    public bool m_RefreshDoTurn;

    public bool m_IsAttackSide = true;

    private RepeatedField<ExcuteTurn> _excuteTurnList;

    private bool _sentOverRoundReq;
    private uint _battleResult;
    public bool m_RoundOver;
    public uint _lastRoleSkillData;
    public uint _lastPetSkillData;
    /// <summary>
    /// =0没有特殊结束,=1逃跑结束战斗,=2服务器发送EndNtf结束战斗,=3撤退结束战斗, =99其他强制战斗结束
    /// </summary>
    public uint m_BattleOverType;

    private float _time;

    private float _fastIntervalTime;
    private float _fastTime;
    private int _nextExcuteIndex;

    public bool m_IsStartResume;
    public bool m_IsReconnect;
    public CmdBattleRoundStartNtf m_ReconnectRoundStartNtf;
    public CmdBattleRoundNtf m_ReconnectRoundNtf;
    public uint m_ReconnectBattleState;

    public bool m_IsExecuteState;   //是否在战斗演示阶段

    public uint m_EnergePoint;
    public uint m_LastRounfEnergePoint;

    public uint m_PosType;     // 0为角色在后排 1为角色在前排

    /// <summary>
    /// 回合开始倒计时
    /// </summary>
    public uint CountDownValue; //倒计时总时间
    private Timer timerCountDown; //倒计时计算定时器
    private uint m_roundCountdown; //倒计时剩余时间
    public uint RoundCountDown
    {
        get
        {
            return m_roundCountdown;
        }
        set
        {
            OnCalRoundCountDown(value);
        }
    }

    public bool _canSendBattleUnitInfoReq;

    private uint _curServerBattleStage;
    /// <summary>
    /// 服务器缓存的战斗阶段
    /// </summary>
    public uint m_CurServerBattleStage
    {
        get
        {
            return _curServerBattleStage;
        }
        set
        {
            DLogManager.Log(ELogType.eCombat, $"服务器阶段：Sys_Fight.curMonsterGroupId:{Sys_Fight.curMonsterGroupId.ToString()}    _curServerBattleStage:{_curServerBattleStage.ToString()}    value:{value.ToString()}");

            if (_curServerBattleStage != value)
            {
                _curServerBattleStage = value;

                if (value > 0u)
                {
                    CSVBattleStage.Data battleStageTb = CSVBattleStage.Instance.GetConfData(Sys_Fight.curMonsterGroupId * 10u + value);
                    if (battleStageTb != null)
                    {
                        CombatManager.Instance.m_BattlePosType = battleStageTb.position_type;
                    }
                }
            }
        }
    }

    private uint _curClientBattleStage;
    /// <summary>
    /// 客户端缓存的战斗阶段
    /// </summary>
    public uint m_CurClientBattleStage
    {
        get
        {
            return _curClientBattleStage;
        }
        set
        {
            DLogManager.Log(ELogType.eCombat, $"客户端阶段：Sys_Fight.curMonsterGroupId:{Sys_Fight.curMonsterGroupId.ToString()}   _curClientBattleStage:{_curClientBattleStage.ToString()}    value:{value.ToString()}");

            if (_curClientBattleStage != value)
            {
                _curClientBattleStage = value;

                if (value > 0u)
                {
                    CSVBattleStage.Data battleStageTb = CSVBattleStage.Instance.GetConfData(Sys_Fight.curMonsterGroupId * 10u + value);
                    if (battleStageTb != null && battleStageTb.refresh_work_id > 0u)
                    {

                        WS_CombatBehaveAIManagerEntity.StartCombatBehaveAI02<WS_CombatSceneAIControllerEntity>(battleStageTb.refresh_work_id);
                    }
                }
            }
        }
    }

    public uint m_CurRound;
    public uint m_MultiBattleStage; //连战阶段
    public uint m_CurStage_Round;
    public float m_ForceDoExcuteTurnTime = 5f;

    public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

    /// <summary> 是否战斗行动中,此标记用于触发器条件判断,在回合开始以及回合结束 </summary>
    public bool isFightOperations = false;

    public bool m_IsWaitDoExcute;

    public bool m_IsWatchBattle;
    public uint m_WatchSide;
    public ulong m_BeWatchRoleId;

    //战斗指令是否ok
    public bool commandIsOk = false;

    //战斗内宠物
    public uint fightPet;
    public List<BattlePet> petsInBattle = new List<BattlePet>();

    //教学观id  id=0为非教学观战斗
    public uint m_TeachingID;

    public enum EEvents : int
    {
        OnRoundNtf,             //回合开始
        OnFirstOperationOver,           //1动结束
        OnSecondOperationOver,          //2动结束
        OnCloseShowOff,   //取消操作界面关闭
        OnCloseShowOn,   //取消操作界面打开
        OnDisableSkillBtn,   //技能按钮禁用
        OnDisableEscapeBtn,   //逃跑按钮禁用
        OnDisableItemBtn,   //物品按钮禁用
        OnUpdateHp,  //更新hp 
        OnUpdateMp, //更新mp
        OnUpdatePet,  //替换宠物

        OnAutoFight, //自动战斗
        OnSetAutoSkill, //设置自动技能
        OnReconnect,   //断线重连
        OnSkillMessageOff,   //技能详情关闭
        OnWaitOthersCommands,   //等待其他玩家指令输入
        OnDoRound,     //执行回合结果
        OnUpdateRound,     //更新回合数（主角死后）

        OnRemoveBattleUI,    //移出战斗ui
        OnBackeBattleUI,     //移回战斗ui
        OnUpdateBossBlood,     //boss血条更新
        OnUpdateBossBuff,          //bossBuff更新

        OnCommandChoose,     //命令选择
        OnCommandOperateStart,
        OnCombatOperateOver,     //命令选择
        OnCommandIsOk,           //战斗指令ok更新
        OnFastCommandChoose,     //快捷指令选择
        OnSkillColdUpdate,        //技能cd更新

        OnFunctionMenuClose,    //功能菜单关闭

        OnEndBattle,    //结束战斗
        OnCallPet,    //召唤宠物
        OnSetPosType,     //角色站位前后排

        OnUIBlackSrceen,  //UI黑屏

        OnBattleOver,   //战斗结束
        OnBattleSettlement,//暗雷结算
        OnSetExcuteTurns,   //战斗演示敏序
        OnOpenAutoSkill,     //打开自动战斗配置

        OnLoadMobsOver,     //加载完所有战斗单位
        OnScoreInfo,     //玩家胜负判定
        OnChangeBattleStage,  //战斗阶段切换
    }

    #region 生命周期
    public void OnAwake()
    {
        EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattle.RoundNtf, OnRoundNtf, CmdBattleRoundNtf.Parser);
        EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattle.RoundStartNtf, OnRoundStartNtf, CmdBattleRoundStartNtf.Parser);
        EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattle.ShowRoundEndNtf, OnShowRoundEndNtf, CmdBattleShowRoundEndNtf.Parser);
        EventDispatcher.Instance.AddEventListener((ushort)CmdBattle.UnitInfoReq, (ushort)CmdBattle.UnitInfoRes, OnBattleUnitInfoRes, CmdBattleUnitInfoRes.Parser);

        EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattle.SetTagNtf, OnSetTagNtf, CmdBattleSetTagNtf.Parser);//指令标记
        EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattle.SetTagRes, OnSetTagRes, CmdBattleSetTagNtf.Parser);//指令标记反馈

        EventDispatcher.Instance.AddEventListener((ushort)CmdBattle.CmdStateReq, (ushort)CmdBattle.CmdStateRes, OnCmdStateRes, CmdBattleCmdStateRes.Parser); //战斗指令撤回
        EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattle.CmdStateNtf, OnCmdStateNtf, CmdBattleCmdStateNtf.Parser);  //战斗指令撤回

        EventDispatcher.Instance.AddEventListener((ushort)CmdBattle.SetPosTypeReq, (ushort)CmdBattle.SetPosTypeRes, OnSetPosTypeRes, CmdBattleSetPosTypeRes.Parser); //战斗指令撤回

#if DEBUG_MODE
        EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattle.AilogNtf, OnCombatAILogNtf, CmdBattleAILogNtf.Parser);
#endif

        EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattle.EndNtf, OnEndNtf, CmdBattleEndNtf.Parser);

        EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattle.CmdSpeedUpNtf, OnCmdSpeedUpNtf, CmdBattleCmdSpeedUpNtf.Parser);

        EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattle.CancelNtf, OnCancelNtf, CmdBattleCancelNtf.Parser);

        _fastIntervalTime = System.Convert.ToSingle(CSVParam.Instance.GetConfData(150u).str_value) * 0.001f;

        m_ForceDoExcuteTurnTime = 5f;

        OnAwakeCombatVideo();
    }

    public void OnEnable()
    {
        m_RoundOver = true;

        _excuteTurnIndex = 0;
        _nextExcuteIndex = -1;
        _sentOverRoundReq = false;
        _battleResult = 0u;
        m_BattleOverType = 0u;
        _time = 0f;
        m_RefreshDoTurn = false;

        _canSendBattleUnitInfoReq = true;
        m_EnergePoint = 0;
        m_LastRounfEnergePoint = 0;

        if (!m_IsReconnect && _excuteTurnList != null && _excuteTurnList.Count > 0)
            _excuteTurnList.Clear();

        OnEnableCombatVideo();
    }

    public void OnUpdate()
    {
        if (m_RoundOver)
            return;

        if (m_IsVideo && m_IsPauseVideo)
            return;

        if (_nextExcuteIndex < 0 && m_RefreshDoTurn)
        {
            m_RefreshDoTurn = false;
            _time = 0f;

            DoExcuteTurn();
            return;
        }

        if (_nextExcuteIndex > -1 && _fastTime + _fastIntervalTime < Time.time)
        {
            DoExcuteTurn();

            _time = 0f;
            m_RefreshDoTurn = false;
            _fastTime = Time.time;

            return;
        }

        if (_time > m_ForceDoExcuteTurnTime)
        {
#if UNITY_EDITOR
            //if (CombatManager.Instance.m_CombatStyleState == 999)
            //    return;

#if ILRUNTIME_MODE
            if(true)
#else
            if (!CreateCombatDataTest.s_ClientType)
#endif
            {
                if (_excuteTurnList != null && _excuteTurnIndex > 0 && _excuteTurnIndex - 1 < _excuteTurnList.Count)
                {
                    ExcuteTurn excuteTurn = _excuteTurnList[_excuteTurnIndex - 1];

                    string netAtInfoStr = $"回合内被强制进行战斗进程 ： {{";

                    for (int i = 0; i < excuteTurn.SrcUnit.Count; i++)
                    {
                        MobEntity me = MobManager.Instance.GetMob(excuteTurn.SrcUnit[i]);
                        if (me == null)
                            continue;

                        netAtInfoStr += $"SkillId : {excuteTurn.SkillId[i].ToString()}  SrcUnitId : {excuteTurn.SrcUnit[i].ToString()}-----ClientNum:{me.m_MobCombatComponent.m_ClientNum.ToString()}  ";
                    }
                    netAtInfoStr += $"}}  {{";

                    for (int i = 0; i < excuteTurn.ExcuteData.Count; i++)
                    {
                        var hc = excuteTurn.ExcuteData[i].HpChange;
                        if (hc != null)
                        {
                            MobEntity me = MobManager.Instance.GetMob(hc.UnitId);
                            if (me != null)
                                netAtInfoStr += $"(hpChangeUnitId : {hc.UnitId.ToString()}--clientNum:{me.m_MobCombatComponent.m_ClientNum.ToString()})  ";
                        }

                        var bc = excuteTurn.ExcuteData[i].BuffChange;
                        if (bc != null)
                        {
                            if (CheckBehaveBuff(excuteTurn, bc))
                            {
                                MobEntity target = MobManager.Instance.GetMob(bc.UnitId);
                                if (target != null)
                                {
                                    var buffTb = CSVBuff.Instance.GetConfData(bc.BuffId);
                                    if (buffTb != null && buffTb.skill_id > 0u)
                                        netAtInfoStr += $"(buffChangeUnitId:{bc.UnitId.ToString()}, clientNum:{target.m_MobCombatComponent.m_ClientNum.ToString()}， buffId:{bc.BuffId.ToString()})   ";
                                }
                            }
                        }

                        var uc = excuteTurn.ExcuteData[i].UnitsChange;
                        if (uc != null)
                        {
                            if (uc.DelUnitId != null && uc.DelUnitId.Count > 0)
                            {
                                for (int ucIndex = 0, ucCount = uc.DelUnitId.Count; ucIndex < ucCount; ucIndex++)
                                {
                                    MobEntity me = MobManager.Instance.GetMob(uc.DelUnitId[ucIndex]);
                                    if (me != null)
                                        netAtInfoStr += $"(DelUnitId : {uc.DelUnitId[ucIndex].ToString()}--clientNum:{me.m_MobCombatComponent.m_ClientNum.ToString()})  ";
                                }
                            }
                            if (uc.DelFailUnitId != null && uc.DelFailUnitId.Count > 0)
                            {
                                for (int ucIndex = 0, ucCount = uc.DelFailUnitId.Count; ucIndex < ucCount; ucIndex++)
                                {
                                    MobEntity me = MobManager.Instance.GetMob(uc.DelFailUnitId[ucIndex]);
                                    if (me != null)
                                        netAtInfoStr += $"(DelFailUnitId : {uc.DelFailUnitId[ucIndex].ToString()}--clientNum:{me.m_MobCombatComponent.m_ClientNum.ToString()})  ";
                                }
                            }
                            if (uc.NewUnits != null && uc.NewUnits.Count > 0)
                            {
                                for (int ucIndex = 0, ucCount = uc.NewUnits.Count; ucIndex < ucCount; ucIndex++)
                                {
                                    BattleUnit bu = uc.NewUnits[ucIndex];
                                    if (bu != null)
                                        netAtInfoStr += $"(添加新成员NewUnits : {bu.UnitId.ToString()}--ServerNum:{bu.Pos.ToString()}--clientNum:{CombatHelp.ServerToClientNum(bu.Pos, CombatManager.Instance.m_IsNotMirrorPos).ToString()})  ";
                                }
                            }
                            if (uc.EscapeUnitId != null && uc.EscapeUnitId.Count > 0)
                            {
                                for (int ucIndex = 0, ucCount = uc.EscapeUnitId.Count; ucIndex < ucCount; ucIndex++)
                                {
                                    MobEntity me = MobManager.Instance.GetMob(uc.EscapeUnitId[ucIndex]);
                                    if (me != null)
                                        netAtInfoStr += $"(EscapeUnitId : {uc.EscapeUnitId[ucIndex].ToString()}--clientNum:{me.m_MobCombatComponent.m_ClientNum.ToString()})  ";
                                }
                            }
                            if (uc.EscapeFailUnitId != null && uc.EscapeFailUnitId.Count > 0)
                            {
                                for (int ucIndex = 0, ucCount = uc.EscapeFailUnitId.Count; ucIndex < ucCount; ucIndex++)
                                {
                                    MobEntity me = MobManager.Instance.GetMob(uc.EscapeFailUnitId[ucIndex]);
                                    if (me != null)
                                        netAtInfoStr += $"(EscapeFailUnitId : {uc.EscapeFailUnitId[ucIndex].ToString()}--clientNum:{me.m_MobCombatComponent.m_ClientNum.ToString()})  ";
                                }
                            }
                            if (uc.UnitPosChange != null && uc.UnitPosChange.Count > 0)
                            {
                                for (int ucIndex = 0, ucCount = uc.UnitPosChange.Count; ucIndex < ucCount; ucIndex++)
                                {
                                    var unitPosChange = uc.UnitPosChange[ucIndex];
                                    if (unitPosChange == null)
                                        continue;

                                    MobEntity me = MobManager.Instance.GetMob(unitPosChange.UnitId);
                                    if (me != null)
                                        netAtInfoStr += $"(unitPosChange.UnitId : {unitPosChange.UnitId.ToString()}--ServerPos:{unitPosChange.Pos.ToString()}--clientNum:{me.m_MobCombatComponent.m_ClientNum.ToString()})  ";
                                }
                            }
                        }

                        var bub = excuteTurn.ExcuteData[i].UnitBase;
                        if (bub != null)
                        {
                            if (bub.UnitId != 0u)
                                netAtInfoStr += $"(基础信息变化UnitId:{bub.UnitId.ToString()}  race:{bub.Race.ToString()}  ShapeShiftId:{bub.ShapeShiftId.ToString()})   ";
                        }
                    }
                    netAtInfoStr += $"}}---------";

                    foreach (var kv in MobManager.Instance.m_MobDic)
                    {
                        if (kv.Value == null)
                            continue;

                        if (kv.Value.m_MobCombatComponent.m_IsStartBehave)
                        {
                            netAtInfoStr += $"ClientNum:{kv.Value.m_MobCombatComponent.m_ClientNum.ToString()}[{(kv.Value.m_Go == null ? null : kv.Value.m_Go.name)}]  IsStartBehave:{kv.Value.m_MobCombatComponent.m_IsStartBehave.ToString()}    behaveCount:{kv.Value.m_MobCombatComponent.GetBehaveCount().ToString()}  m_HpChangeDataQueue.Count:{kv.Value.m_MobCombatComponent.m_HpChangeDataQueue.Count.ToString()}  m_ReadlyBehaveCount:{kv.Value.m_MobCombatComponent.m_ReadlyBehaveCount.ToString()}----";
                        }
                    }

                    netAtInfoStr += $"  新战斗单位正在生成中的数量{m_NewUnitBehaveInfoDic.Count.ToString()}：【";
                    foreach (var kv in m_NewUnitBehaveInfoDic)
                    {
                        NewUnitBehaveInfo newUnitBehaveInfo = kv.Value;
                        if (newUnitBehaveInfo == null || newUnitBehaveInfo.m_newUnitId == 0u)
                            continue;

                        if (MobManager.Instance.GetMob(newUnitBehaveInfo.m_newUnitId) == null)
                        {
                            netAtInfoStr += $"serverNum:{kv.Key.ToString()} newUnitId:{newUnitBehaveInfo.m_newUnitId.ToString()}    ";
                        }
                    }
                    netAtInfoStr += "】";

                    netAtInfoStr += $"---------m_RecordCombatBehaveUnitList : {m_RecordCombatBehaveUnitList.Count}";

                    DebugUtil.LogError(netAtInfoStr);
                }
            }
#endif

#if DEBUG_MODE
            int excuteTIndex = _excuteTurnIndex - 1;
            if (m_NetExcuteTurnInfoDic.TryGetValue(excuteTIndex, out NetExcuteTurnInfo debugNetExcuteTurnInfo) &&
                    debugNetExcuteTurnInfo != null && debugNetExcuteTurnInfo.AttackBoLifeCycleInfoList != null &&
                    debugNetExcuteTurnInfo.AttackBoLifeCycleInfoList.Count > 0)
            {
                DebugModeCheckDoSNodeState(debugNetExcuteTurnInfo.AttackBoLifeCycleInfoList, excuteTIndex < _excuteTurnList.Count ? _excuteTurnList[excuteTIndex] : null);
            }
#endif

            DoExcuteTurn(true);
            _time = 0f;
        }

        _time += Time.deltaTime;
    }

    public void OnDisable()
    {
        _curServerBattleStage = 0;
        _curClientBattleStage = 0;
        m_CurRound = 0;
        m_MultiBattleStage = 0;
        m_CurStage_Round = 0;
        m_TeachingID = 0;
        _excuteTurnList = null;

        DoClearDelayRoundData();

        ClearDoExcuteTurnData();

        OnDisableCombatVideo();
    }

    public void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.RoundNtf, OnRoundNtf);
        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.RoundStartNtf, OnRoundStartNtf);
        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.ShowRoundEndNtf, OnShowRoundEndNtf);
        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.UnitInfoRes, OnBattleUnitInfoRes);
        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.CmdStateRes, OnCmdStateRes);
        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.CmdStateNtf, OnCmdStateNtf);
        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.SetTagRes, OnSetTagRes);
        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.SetTagNtf, OnSetTagNtf);
        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.SetPosTypeRes, OnSetPosTypeRes);
#if DEBUG_MODE
        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.AilogNtf, OnCombatAILogNtf);
#endif

        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.EndNtf, OnEndNtf);

        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.CmdSpeedUpNtf, OnCmdSpeedUpNtf);

        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.CancelNtf, OnCancelNtf);

        OnDestroyCombatVideo();
    }
    #endregion

    #region Net
    
    private void OnRoundNtf(NetMsg msg)
    {
        
#if UNITY_EDITOR && !ILRUNTIME_MODE
        if (CreateCombatDataTest.s_ClientType)
            return;
#endif

        CmdBattleRoundNtf ntf = NetMsgUtil.Deserialize<CmdBattleRoundNtf>(CmdBattleRoundNtf.Parser, msg);

        DLogManager.Log(ELogType.eCombat, $"BattleRoundNtf--网络消息--战斗结果:{ntf.BattleResult.ToString()}    BattleId : {ntf.BattleId.ToString()}   CurRound : {ntf.CurRound.ToString()}");

        if (m_IsWaitDoExcute && m_IsReconnect && m_ReconnectBattleState != 0)
            m_ReconnectRoundNtf = ntf;
        else
            DoRound(ntf);
    }

    /// <summary>
    /// 每回合开始发
    /// </summary>
    /// <param name="msg"></param>
    private void OnRoundStartNtf(NetMsg msg)
    {
        CmdBattleRoundStartNtf ntf = NetMsgUtil.Deserialize<CmdBattleRoundStartNtf>(CmdBattleRoundStartNtf.Parser, msg);

        DLogManager.Log(ELogType.eCombat, $"OnRoundStartNtf CurRound:{ntf.CurRound.ToString()} BattleId:{ntf.BattleId.ToString()}");

        if (m_IsStartResume)
        {
            m_ReconnectRoundStartNtf = ntf;
        }
        else
        {
            DoRoundStartNtf(ntf);
        }
        
        Sys_Bag.Instance.eventEmitter.Trigger(Sys_Bag.EEvents.OnBattleRoundStartNtf);
    }

    private void DoRoundStartNtf(CmdBattleRoundStartNtf ntf, bool isClearDelayRoundData = true)
    {
        if (ntf == null)
            return;

        m_IsWaitDoExcute = false;
        m_ReconnectBattleState = 0;

        if (m_IsReconnect)
        {
            ClearTurn();
            if (isClearDelayRoundData)
                DoClearDelayRoundData();
        }
        else
            m_IsWaitDoExcute = false;

        m_CurRound = ntf.CurRound;
        m_MultiBattleStage = ntf.CurStage;
        m_CurStage_Round = ntf.CurstageRound;

        CombatManager.Instance.m_CurRoundStartTime = Time.unscaledTime;
        m_IsExecuteState = false;

        DLogManager.Log(ELogType.eCombat, $"DoRoundStartNtf ---- CurRound:{ntf.CurRound.ToString()} BattleId:{ntf.BattleId.ToString()}");

        if (MobManager.Instance == null)
        {
            DebugUtil.LogError($"MobManager单例为null");
            return;
        }

        if (IsRealCombat() && (GameCenter.mainFightHero == null || GameCenter.mainFightHero.battleUnit == null))
        {
            if (CombatManager.Instance.m_BattleId == ntf.BattleId)
                DebugUtil.LogError($"{(GameCenter.mainFightHero == null ? "GameCenter.mainFightHero为null" : (GameCenter.mainFightHero.battleUnit == null ? "GameCenter.mainFightHero.battleUnit为null" : GameCenter.mainFightHero.battleUnit.ToString()))}");
            else
                DebugUtil.Log(ELogType.eCombat, $"<color=red>{(GameCenter.mainFightHero == null ? "GameCenter.mainFightHero为null" : (GameCenter.mainFightHero.battleUnit == null ? "GameCenter.mainFightHero.battleUnit为null" : GameCenter.mainFightHero.battleUnit.ToString()))}</color>");
            return;
        }

        if (CombatManager.Instance.m_BattleTypeTb != null)
        {
            OnCalRoundCountDown(CombatManager.Instance.m_BattleTypeTb.setup_time);
        }
        else
        {
            DebugUtil.LogError($"CombatManager.Instance.m_BattleTypeTb为空，使得没有执行OnCalRoundCountDown函数");
        }

        if (IsRealCombat())
        {
            //MobEntity mob = MobManager.Instance.GetMob(GameCenter.mainFightHero.battleUnit.UnitId);
            //if (mob == null || mob.m_MobCombatComponent == null)
            //    return;

            foreach (var energe in ntf.Energes)
            {
                m_LastRounfEnergePoint = m_EnergePoint;
                if (energe.UnitId == GameCenter.mainFightHero.battleUnit.UnitId)
                {
                    m_EnergePoint = energe.Energe;
                }
            }
            GameCenter.fightControl.RefreshSkillColdData();  //要放在OnRoundNtf Trigger之前 每回合刷新技能cd
        }
        isFightOperations = true;
        GameCenter.fightControl.isDoRound = false;
        
        if (GameCenter.fightControl == null)
        {
            DLogManager.Log(ELogType.eCombat, $"战斗:GameCenter.fightControl is null!!!");
        }
        else
        {
            if (GameCenter.fightControl.hasCommends == null)
            {
                DLogManager.Log(ELogType.eCombat, $"战斗:GameCenter.fightControl.hasCommends is null!!!");
            }
            else
            {
                GameCenter.fightControl.hasCommends.Clear();
            }
        }

        //Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnClearBattleFlag);
        if (ntf.Turns != null && ntf.Turns.Count > 0)
        {
            SetExcuteTurns(ntf.Turns, !m_IsReconnect, false);

            m_RoundOver = false;
            m_BattleOverType = 0u;
            _excuteTurnIndex = 0;
            _fastTime = Time.time - _fastIntervalTime;
            _sentOverRoundReq = false;

            DoExcuteTurn(false, false);
        }
        else
        {
            ClearTurn();

            CombatManager.Instance.ResetNormalTimeScale();

            ClearNetExcuteTurnInfoDatas();
        }

        eventEmitter.Trigger(EEvents.OnRoundNtf);
    }

    private void OnCalRoundCountDown(uint countdownTime)
    {
        m_roundCountdown = countdownTime;
        CountDownValue = m_roundCountdown;

        timerCountDown?.Cancel();
        timerCountDown = Timer.Register(m_roundCountdown, () =>
        {
            m_roundCountdown = 0;
        }, (time) =>
        {
            m_roundCountdown = (uint)Mathf.CeilToInt(CountDownValue - time);
        }, false, true);
    }

    private void OnShowRoundEndNtf(NetMsg msg)
    {
        CmdBattleShowRoundEndNtf ntf = NetMsgUtil.Deserialize<CmdBattleShowRoundEndNtf>(CmdBattleShowRoundEndNtf.Parser, msg);

        m_RoundOver = true;
    }

    private void OnEndNtf(NetMsg msg)
    {
#if UNITY_EDITOR
#if !ILRUNTIME_MODE
        if (CreateCombatDataTest.s_ClientType)
            return;
#endif
#endif

        CmdBattleEndNtf ntf = NetMsgUtil.Deserialize<CmdBattleEndNtf>(CmdBattleEndNtf.Parser, msg);

        DLogManager.Log(ELogType.eCombat, $"CmdBattleEndNtf---ntf.BattleId:{ntf.BattleId.ToString()}  ntf.BattleResult:{ntf.BattleResult.ToString()}   m_IsAttackSide:{m_IsAttackSide.ToString()}");

        if (CombatManager.Instance.m_CombatStyleState != 999 && ntf.BattleId != CombatManager.Instance.m_BattleId)
        {
            DebugUtil.LogError($"服务器发送CmdBattleEndNtf的ntf.BattleId：{ntf.BattleId.ToString()}和当前的BattleId：{CombatManager.Instance.m_BattleId.ToString()}不相等，不处理退出当前战斗");
            return;
        }
        m_BattleOverType = 2u;
        _battleResult = ntf.BattleResult;
        if (IsRealCombat())
            eventEmitter.Trigger<CmdBattleEndNtf>(EEvents.OnBattleSettlement, ntf);//暗雷结算Sys_Settlement
        DoBattleResult();
        eventEmitter.Trigger<CmdBattleEndNtf>(EEvents.OnEndBattle, ntf);
        PetUnit petUnit = Sys_Pet.Instance.GetFightPet();
        if (petUnit != null)
        {
            Sys_Plan.Instance.eventEmitter.Trigger<ulong, uint, uint>(Sys_Plan.EEvents.OnChangeFightPet, petUnit.Uid, petUnit.PetPointPlanData.CurrentPlanIndex, petUnit.EnhancePlansData.CurrentPlanIndex);
        }
        else
        {
            Sys_Plan.Instance.eventEmitter.Trigger<ulong, uint, uint>(Sys_Plan.EEvents.OnChangeFightPet, 0, 0, 0);
        }
    }

    public void SendCmdBattleCmdSpeedUpReq(float speedTime)
    {
        CmdBattleCmdSpeedUpReq req = new CmdBattleCmdSpeedUpReq();
        req.BattleId = CombatManager.Instance.m_BattleId;
        req.SpeedTime = CombatManager.Instance.SwitchTimeScale2Uint(speedTime);

        DLogManager.Log(ELogType.eCombat, $"SendCmdBattleCmdSpeedUpReq---BattleId:{req.BattleId.ToString()}   speedTime:{speedTime}   req.SpeedTime:{req.SpeedTime.ToString()}");

        NetClient.Instance.SendMessage((ushort)CmdBattle.CmdSpeedUpReq, req);
    }

    private void OnCmdSpeedUpNtf(NetMsg msg)
    {
        CmdBattleCmdSpeedUpNtf ntf = NetMsgUtil.Deserialize<CmdBattleCmdSpeedUpNtf>(CmdBattleCmdSpeedUpNtf.Parser, msg);

        DLogManager.Log(ELogType.eCombat, $"OnCmdSpeedUpNtf---ntf.SpeedTime:{ntf.SpeedTime.ToString()}");

        if (ntf.BattleId != CombatManager.Instance.m_BattleId)
        {
            DebugUtil.LogError($"ntf.BattleId:{ntf.BattleId.ToString()} 和 记录的{CombatManager.Instance.m_BattleId.ToString()}不一样");
        }
        else
            CombatManager.Instance.SetTimeScale(CombatManager.Instance.SwitchUint2TimeScale(ntf.SpeedTime), false, false);
    }

    public void SendBattleUnitInfoReq(uint unitId)
    {
        CmdBattleUnitInfoReq req = new CmdBattleUnitInfoReq();
        req.BattleId = CombatManager.Instance.m_BattleId;
        req.UnitId = unitId;

        DLogManager.Log(ELogType.eCombat, $"SendBattleUnitInfoReq---BattleId:{req.BattleId.ToString()}    UnitId:{req.UnitId.ToString()}");

        NetClient.Instance.SendMessage((ushort)CmdBattle.UnitInfoReq, req);
    }

    private void OnBattleUnitInfoRes(NetMsg msg)
    {
        CmdBattleUnitInfoRes res = NetMsgUtil.Deserialize<CmdBattleUnitInfoRes>(CmdBattleUnitInfoRes.Parser, msg);

        DLogManager.Log(ELogType.eCombat, $"OnBattleUnitInfoRes---UnitId:{res.UnitId.ToString()}    ControlTimes:{res.ControlTimes.ToString()}");

        for (int i = 0; i < res.Buff.Count; i++)
        {
            BattleBuffChange bc = res.Buff[i];
            if (bc == null || bc.FailAdd > 0u)
                continue;

            var buffMob = MobManager.Instance.GetMob(bc.UnitId);
            if (buffMob == null)
                continue;

            buffMob.UpdateBuffComponentByBehave(bc);
        }

        var mobEntity = MobManager.Instance.GetMob(res.UnitId);
        if (mobEntity != null)
        {
            UIManager.OpenUI(EUIID.UI_Buff, true, mobEntity.m_Go);
            _canSendBattleUnitInfoReq = false;

#if UNITY_EDITOR_NO_USE
            var buffCssi = CombatSimulateManager.Instance.GetCombatSimulateStatisticsInfo(mobEntity.m_MobCombatComponent.m_BattleUnit.Pos);
            buffCssi.BeControlledNum = (int)res.ControlTimes;
#endif
        }
    }

    public void BattleCommandReq(uint battle_id, List<BattleCommand> cmds)
    {
        DLogManager.Log(ELogType.eCombat, $"send---{((ushort)CmdBattle.CommandReq).ToString()}");
        DebugUtil.LogFormat(ELogType.eBattleCommand, "send BattleCommandReq:  battle_id is{0}, command.MainCmd is {1},  command.Param1 is {2},command.SrcUnitId is {3},   command.TarPos is {4}", battle_id, cmds[0].MainCmd, cmds[0].Param1, cmds[0].SrcUnitId, cmds[0].TarPos);
        DebugUtil.LogFormat(ELogType.eBattleCommand, "Now battlestate is{0}", GameCenter.fightControl.operationState);

        CmdBattleCommandReq req = new CmdBattleCommandReq();

        if (cmds != null)
        {
            foreach (var cmd in cmds)
            {
                req.Cmd.Add(cmd);
            }
        }
        req.BattleId = battle_id;
        //         byte[] data = NetMsgUtil.Serialize((ushort)CmdBattle.CommandReq, req);
        //         NetClient.Instance.SendData(data);

        NetClient.Instance.SendMessage((ushort)CmdBattle.CommandReq, req);
    }

    public void BattleShowRoundEndReq(uint battle_id, uint self_uid, uint cur_round)
    {
        DLogManager.Log(ELogType.eCombat, $"send---{((ushort)CmdBattle.ShowRoundEndReq).ToString()}---CurRound:{cur_round.ToString()}");

        CmdBattleShowRoundEndReq req = new CmdBattleShowRoundEndReq();
        req.BattleId = battle_id;
        req.SelfUid = self_uid;
        req.CurRound = cur_round;
        //         byte[] data = NetMsgUtil.Serialize((ushort)CmdBattle.ShowRoundEndReq, req);
        //         NetClient.Instance.SendData(data);

        NetClient.Instance.SendMessage((ushort)CmdBattle.ShowRoundEndReq, req);
    }

    private void OnSetTagNtf(NetMsg msg)
    {
        CmdBattleSetTagNtf info = NetMsgUtil.Deserialize<CmdBattleSetTagNtf>(CmdBattleSetTagNtf.Parser, msg);

        int count = info.TagComposeId.Count;

        for (int i = 0; i < count; i++)
        {
            uint tagcompose = info.TagComposeId[i];

            string strvalue = string.Empty;

            uint tagindex = 0;
            uint tagside = 0;

            if (tagcompose > 0)
            {
                uint tag0 = tagcompose / 10000;//标记者uint id

                uint tag1 = (tagcompose - tag0 * 10000) / 100;//tag id

                uint tag2 = (tagcompose - tag0 * 10000 - tag1 * 100) / 10;//队长，被委托指挥

                uint tag3 = tagcompose - tag0 * 10000 - tag1 * 100 - tag2 * 10;//敌友

                strvalue = Sys_Team.Instance.getTeamTagString(tag2, tag1, tag3);


                tagindex = tag1;
                tagside = tag3;

            }

            if (GameCenter.fightControl != null)
                GameCenter.fightControl.SetFightCommand(info.UnitId[i], tagcompose == 0, tagindex, tagside);

            Sys_HUD.Instance.eventEmitter.Trigger<uint, bool, string>(Sys_HUD.EEvents.OnTriggerBattleInstructionFlag, info.UnitId[i], tagcompose > 0, strvalue);
        }
    }

    private void OnSetTagRes(NetMsg msg)
    {

    }

    public void CmdStateReq(uint battle_id, uint unite_id, uint cmdready)
    {
        CmdBattleCmdStateReq req = new CmdBattleCmdStateReq();
        req.BattleId = battle_id;
        req.UniteId = unite_id;
        req.Cmdready = cmdready;
        NetClient.Instance.SendMessage((ushort)CmdBattle.CmdStateReq, req);
    }

    private void OnCmdStateRes(NetMsg msg)
    {
        CmdBattleCmdStateRes res = NetMsgUtil.Deserialize<CmdBattleCmdStateRes>(CmdBattleCmdStateRes.Parser, msg);

    }
  
    private void OnCmdStateNtf(NetMsg msg)
    {
        CmdBattleCmdStateNtf ntf = NetMsgUtil.Deserialize<CmdBattleCmdStateNtf>(CmdBattleCmdStateNtf.Parser, msg);
        if (ntf.BattleId == CombatManager.Instance.m_BattleId)
        {
            if (ntf.Cmdready == 0)
            {
                if (GameCenter.fightControl.operationState == FightControl.EOperationState.OperationOver && Sys_Fight.Instance.HasPet())
                {
                    Sys_HUD.Instance.eventEmitter.Trigger<uint>(Sys_HUD.EEvents.OnUndoBattleOrder, GameCenter.mainFightPet.battleUnit.UnitId);
                }
                else
                {
                    Sys_HUD.Instance.eventEmitter.Trigger<uint>(Sys_HUD.EEvents.OnUndoBattleOrder, ntf.UniteId);
                }
            }
            eventEmitter.Trigger(EEvents.OnCommandIsOk, ntf.UniteId, ntf.Cmdready);
        }
    }

    //角色前排后排  1角色在后排  2角色在前排
    public void SetPosTypeReq(uint posType)
    {
        CmdBattleSetPosTypeReq req = new CmdBattleSetPosTypeReq();
        req.PosType = posType;
        NetClient.Instance.SendMessage((ushort)CmdBattle.SetPosTypeReq, req);
    }

    private void OnSetPosTypeRes(NetMsg msg)
    {
        CmdBattleSetPosTypeRes res = NetMsgUtil.Deserialize<CmdBattleSetPosTypeRes>(CmdBattleSetPosTypeRes.Parser, msg);
        m_PosType = res.PosType;
        eventEmitter.Trigger(EEvents.OnSetPosType);
        if (Net_Combat.Instance.m_PosType == 0)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5010));
        }
        else
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5009));
        }
    }

    /// <summary>
    /// 标记请求
    /// </summary>
    /// <param name="unitID">标记目标</param>
    /// <param name="makeUnitID">标记者</param>
    /// <param name="tagid">标记小标</param>
    /// <param name="type">队长或者 被指挥者</param>
    /// <param name="isfriend">敌友</param>
    /// 
    public void SendSetTag(List<uint> unitID, List<uint> makeUnitID, List<uint> tagid, List<uint> type, List<uint> isfriend)
    {
        CmdBattleSetTagReq info = new CmdBattleSetTagReq();

        info.UnitId.AddRange(unitID);

        int count = makeUnitID.Count;

        for (int i = 0; i < count; i++)
        {
            info.TagComposeId.Add(makeUnitID[i] * 10000 + tagid[i] * 100 + type[i] * 10 + isfriend[i]);
        }


        NetClient.Instance.SendMessage((ushort)CmdBattle.SetTagReq, info);
    }

    /// <summary>
    /// 标记请求
    /// </summary>
    /// <param name="unitID">标记目标</param>
    /// <param name="makeUnitID">标记者</param>
    /// <param name="tagid">标记小标</param>
    /// <param name="type">队长或者 被指挥者</param>
    /// <param name="isfriend">敌友</param>
    public void SendSetTag(uint unitID, uint makeUnitID, uint tagid, uint type, uint isfriend)
    {
        CmdBattleSetTagReq info = new CmdBattleSetTagReq();
        info.UnitId.Add(unitID);
        info.TagComposeId.Add(makeUnitID * 10000 + tagid * 100 + type * 10 + isfriend);

        NetClient.Instance.SendMessage((ushort)CmdBattle.SetTagReq, info);
    }

    /// <summary>
    /// 标记请求清除所有
    /// </summary>
    public void SendSetTagClearAll()
    {
        CmdBattleSetTagReq info = new CmdBattleSetTagReq();

        foreach (var kvp in MobManager.Instance.m_MobDic)
        {
            info.UnitId.Add(kvp.Value.m_MobCombatComponent.m_BattleUnit.UnitId);

            info.TagComposeId.Add(0);
        }
        NetClient.Instance.SendMessage((ushort)CmdBattle.SetTagReq, info);
    }

    /// <summary>
    /// 标记请求清除某一方的
    /// </summary>
    public void SendSetTagClearSide(int side)
    {
        CmdBattleSetTagReq info = new CmdBattleSetTagReq();

        foreach (var kvp in MobManager.Instance.m_MobDic)
        {
            var battunit = kvp.Value.m_MobCombatComponent.m_BattleUnit;

            if (battunit.Side == side)
            {
                info.UnitId.Add(battunit.UnitId);

                info.TagComposeId.Add(0);
            }
        }
        NetClient.Instance.SendMessage((ushort)CmdBattle.SetTagReq, info);
    }

    /// <summary>
    /// 标记请求清除
    /// </summary>
    public void SendSetTagClear(uint unitID)
    {
        CmdBattleSetTagReq info = new CmdBattleSetTagReq();

        info.UnitId.Add(unitID);

        info.TagComposeId.Add(0);

        NetClient.Instance.SendMessage((ushort)CmdBattle.SetTagReq, info);
    }

    public void SendCmdBattleCancelReq()
    {
        CmdBattleCancelReq req = new CmdBattleCancelReq();
        req.BattleId = CombatManager.Instance.m_BattleId;

        DLogManager.Log(ELogType.eCombat, $"SendCmdBattleCancelReq---BattleId:{req.BattleId.ToString()}");

        NetClient.Instance.SendMessage((ushort)CmdBattle.CancelReq, req);
    }

    private void OnCancelNtf(NetMsg msg)
    {
        CmdBattleCancelNtf ntf = NetMsgUtil.Deserialize<CmdBattleCancelNtf>(CmdBattleCancelNtf.Parser, msg);

        if (ntf.BattleId != CombatManager.Instance.m_BattleId)
        {
            DebugUtil.LogError($"OnCancelNtf    ntf.BattleId:{ntf.BattleId.ToString()} 和 记录的{CombatManager.Instance.m_BattleId.ToString()}不一样");
        }
        else
            DLogManager.Log(ELogType.eCombat, $"OnCancelNtf---BattleId:{ntf.BattleId.ToString()}");

        for (int i = 0, count = ntf.UnitIds.Count; i < count; i++)
        {
            uint unitId = ntf.UnitIds[i];
            MobEntity mob = MobManager.Instance.GetMob(unitId);
            if (mob == null)
                continue;

            if (MobManager.Instance.IsPlayer(mob))
                m_BattleOverType = 3;

            mob.m_MobCombatComponent.m_Death = true;
            mob.m_MobCombatComponent.m_DeathType = 1;

            mob.m_MobCombatComponent.m_BattleUnit.CurHp = 0;

            DLogManager.Log(ELogType.eCombat, $"OnCancelNtf-----<color=yellow>{(mob.m_Go == null ? null : mob.m_Go.name)}  unitId:{unitId.ToString()}  clinetNum:{mob.m_MobCombatComponent.m_ClientNum.ToString()}</color>");

            BehaveAIControllParam behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
            behaveAIControllParam.SrcUnitId = unitId;
            behaveAIControllParam.SkillId = 106u;
            if (!mob.m_MobCombatComponent.StartBehave(behaveAIControllParam.SkillId, null, false, behaveAIControllParam))
            {
                mob.m_MobCombatComponent.SetMobDeath();
            }
        }
    }

#if DEBUG_MODE
    private void OnCombatAILogNtf(NetMsg msg)
    {
        CmdBattleAILogNtf ntf = NetMsgUtil.Deserialize<CmdBattleAILogNtf>(CmdBattleAILogNtf.Parser, msg);

        DebugUtil.Log(ELogType.CombatAILog, $"CmdBattleAILogNtf--------{ntf.Msg.ToStringUtf8()}");
    }
#endif
    #endregion

    #region Logic
    public Dictionary<uint, int> m_TempAttackCountDic = new Dictionary<uint, int>();
    public void SetExcuteTurns(RepeatedField<ExcuteTurn> excuteTurnList, bool isNeedClearTurnData, bool isDoRound)
    {
        if (isNeedClearTurnData)
            ClearTurn();

        _excuteTurnList = excuteTurnList;

        if (isDoRound)
        {
            ClearAttackCountDic();
            GameCenter.fightControl.sequenceRound = m_CurRound;
        }
        if (_excuteTurnList != null)
        {
            for (int etIndex = 0, etCount = _excuteTurnList.Count; etIndex < etCount; etIndex++)
            {
                ExcuteTurn excuteTurn = _excuteTurnList[etIndex];
                if (excuteTurn == null || excuteTurn.SrcUnit == null)
                    continue;
                
                for (int srcIndex = 0, srcCount = excuteTurn.SrcUnit.Count; srcIndex < srcCount; srcIndex++)
                {
                    uint srcId = excuteTurn.SrcUnit[srcIndex];

                    for (int i = 0, _count = Sys_Fight.Instance.battleUnits.Count; i < _count; i++)
                    {
                        BattleUnit unit = Sys_Fight.Instance.battleUnits[i];

                        if (srcId == unit.UnitId && Sys_Role.Instance.Role.RoleId == unit.RoleId) 
                        {
                            var ic = excuteTurn.ItemChange;
                            
                            if (ic != null)
                            {
                                for (int j = 0; j < ic.Count; j++) 
                                {
                                    if (ic[j]!=null)
                                    {
                                        uint itemId = ic[j].Itemid;

                                        Sys_Bag.Instance.eventEmitter.Trigger<uint>(Sys_Bag.EEvents.OnUseItemSuccessInBattle, itemId);
                                        
                                        CSVItem.Data data = CSVItem.Instance.GetConfData(itemId);

                                        if (data != null) 
                                        {
                                            CSVActiveSkill.Data activeSkill = CSVActiveSkill.Instance.GetConfData(data.active_skillid);

                                            if (activeSkill != null)
                                            {
                                                if (activeSkill.type_cold_time > 0) 
                                                {
                                                    Sys_Fight.Instance.useRound[activeSkill.main_skill_id] = m_CurRound;    
                                                }
                                            }
                                        }
                                        DebugUtil.LogFormat(ELogType.eBag, "id:{0}", ic[j].Itemid.ToString());    
                                    }
                                }
                            }
                        }
                    }
                    
                    MobEntity mob = MobManager.Instance.GetMob(srcId);
                    if (mob == null || mob.m_MobCombatComponent == null || mob.m_MobCombatComponent.m_BattleUnit == null)
                        continue;

                    if (m_TempAttackCountDic.TryGetValue(mob.m_MobCombatComponent.m_BattleUnit.UnitId, out int attackCount))
                        ++attackCount;
                    else
                        attackCount = 1;
                    m_TempAttackCountDic[mob.m_MobCombatComponent.m_BattleUnit.UnitId] = attackCount;

                }
            }
            if (isDoRound)
            {
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnSetExcuteTurns);
            }
        }
    }

    public void ClearAttackCountDic()
    {
        m_TempAttackCountDic.Clear();
    }

    public void DoDelayRoundData(bool isClearTurn = true)
    {
        DoRoundStartNtf(m_ReconnectRoundStartNtf, false);
        m_ReconnectRoundStartNtf = null;

        DoRound(m_ReconnectRoundNtf, false);
        m_ReconnectRoundNtf = null;
    }

    public void DoClearDelayRoundData()
    {
        m_ReconnectRoundStartNtf = null;
        m_ReconnectRoundNtf = null;
    }

    public void DoRound(CmdBattleRoundNtf ntf, bool isClearTurn = true)
    {
        if (ntf == null)
            return;

        if (ntf.BattleId != CombatManager.Instance.m_BattleId)
            return;

        //#if DEBUG_MODE && !ILRUNTIME_MODE && UNITY_EDITOR
        //        DLogManager.Log(ELogType.eCombat, $"DoRound----服务器消息记录：{LitJson.JsonMapper.ToJson(ntf)}");
        //#endif

        DLogManager.Log(ELogType.eCombat, $"DoRound----战斗结果:{ntf.BattleResult.ToString()}    BattleId : {ntf.BattleId.ToString()}   CurRound : {ntf.CurRound.ToString()}  excuteTurnCount:{ntf.Turns?.Count.ToString()}   ntf.BattleResult:{ntf.BattleResult.ToString()}   m_IsAttackSide:{m_IsAttackSide.ToString()}");

        SetExcuteTurns(ntf.Turns, isClearTurn, true);

        if (m_IsVideo)
        {
            CacheMobRoundStatesInVideo(ntf.CurRound);
        }

        if (!m_IsReconnect)
            m_IsWaitDoExcute = false;

        //clear countdown
        m_roundCountdown = 0;
        timerCountDown?.Cancel();

        CombatManager.Instance.SwitchOldTimeScale();
        m_RoundOver = false;

        _battleResult = ntf.BattleResult;
        m_BattleOverType = 0u;
        _excuteTurnIndex = 0;
        _fastTime = Time.time - _fastIntervalTime;
        _sentOverRoundReq = false;
        isFightOperations = false;

        if (GameCenter.fightControl != null)
            GameCenter.fightControl.operationState = FightControl.EOperationState.OperationOver;

        ClearNetExcuteTurnInfoDatas();
        DoExcuteTurn();
        GameCenter.fightControl.isDoRound = true;
        eventEmitter.Trigger(EEvents.OnDoRound);
        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnClearBattleOrder);
        foreach (var v in MobManager.Instance.m_MobDic)
        {
            UpdateArrowEvt evt = new UpdateArrowEvt();
            evt.id = v.Value.m_MobCombatComponent.m_BattleUnit.UnitId;
            evt.active = false;
            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateArrow, evt);
            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateTimeSand, v.Key, false);
        }
    }

    /// <summary>
    /// 返回0=正常，1=战斗新单位未被创建
    /// </summary>
    public int ClearTurn()
    {
        int clearTurnState = 0;

        m_RoundOver = true;

        MobManager.Instance.ResetAllMobState();

        if (_excuteTurnList != null && _excuteTurnList.Count > 0 && _excuteTurnIndex > -1 && _excuteTurnIndex < _excuteTurnList.Count)
        {
            BulletManager.Instance.Dispose();

            for (int i = 0, excuteTurnCount = _excuteTurnList.Count; i < excuteTurnCount; i++)
            {
                ExcuteTurn excuteTurn = _excuteTurnList[i];
                for (int j = 0, excuteDataCount = excuteTurn.ExcuteData.Count; j < excuteDataCount; j++)
                {
                    ExcuteData excuteData = excuteTurn.ExcuteData[j];
                    if (excuteData == null)
                        continue;

                    ClearTurnByExcuteTurn(excuteData, i, j);
                }
            }

            if (m_IsVideo)
                _excuteTurnList = null;
            else
                _excuteTurnList.Clear();
        }

        MobManager.Instance.ResetMobsState(true);

        if (_cacheDelayBirthNetNewUnitDatas.Count > 0)
        {
            for (int i = 0; i < _cacheDelayBirthNetNewUnitDatas.Count; i++)
            {
                CacheDelayBirthNetNewUnitData cacheDelayBirthNetNewUnitData = _cacheDelayBirthNetNewUnitDatas[i];
                if (cacheDelayBirthNetNewUnitData != null)
                {
                    DebugUtil.LogError($"_cacheDelayBirthNetNewUnitDatas还有延迟新建战斗单位未执行：UnitId:{cacheDelayBirthNetNewUnitData.m_BattleUnit.UnitId.ToString()}  ShapeShiftId:{cacheDelayBirthNetNewUnitData.m_BattleUnit.ShapeShiftId.ToString()}  isUseShapeShift:{cacheDelayBirthNetNewUnitData.m_BattleUnit.IsUseShapeShift.ToString()}  ExcuteTurnIndex:{cacheDelayBirthNetNewUnitData.m_ExcuteTurnIndex}");
                    CreateNewUnit(cacheDelayBirthNetNewUnitData.m_BattleUnitChange, cacheDelayBirthNetNewUnitData.m_BattleUnit, cacheDelayBirthNetNewUnitData.m_ExcuteTurnIndex);
                    
                    clearTurnState = 1;
                }
            }
            _cacheDelayBirthNetNewUnitDatas.Clear();
        }

        return clearTurnState;
    }

    private void ClearTurnByExcuteTurn(ExcuteData excuteData, int excuteTurnIndex, int excuteDataIndex)
    {
        if (excuteData == null)
            return;

        var hc = excuteData.HpChange;
        if (hc != null)
        {
            MobEntity target = MobManager.Instance.GetMob(hc.UnitId);
            if (target != null)
            {
                DLogManager.Log(ELogType.eCombat, $"ClearTurn()hc--<color=yellow>{(target.m_Go == null ? null : target.m_Go.name)}   UnitId:{hc.UnitId.ToString()}  excuteTurnIndex:{excuteTurnIndex.ToString()}   excuteDataIndex:{excuteDataIndex.ToString()}   CurHp:{hc.CurHp.ToString()}    CurMp:{hc.CurMp.ToString()}</color>");

                if (hc.ChangeType == 1 || hc.ChangeType == 2)
                {
                    target.m_MobCombatComponent.m_BattleUnit.CurMp = hc.CurMp;
                }
                if ((hc.ChangeType == 0 || hc.ChangeType == 2) && hc.HitEffect != 1)
                {
                    target.m_MobCombatComponent.m_BattleUnit.CurHp = hc.CurHp;
                    //target.m_MobCombatComponent.ResetMobState(true, 4);

                    UpdateHp(target.m_MobCombatComponent.m_BattleUnit,
                    target.m_MobCombatComponent.m_BattleUnit.CurHp, target.m_MobCombatComponent.m_BattleUnit.MaxHp);

                    UpdateMp(target.m_MobCombatComponent.m_BattleUnit,
                    target.m_MobCombatComponent.m_BattleUnit.CurMp, target.m_MobCombatComponent.m_BattleUnit.MaxMp);

                    UpdateShield(target.m_MobCombatComponent.m_BattleUnit,
                    target.m_MobCombatComponent.m_BattleUnit.CurShield, target.m_MobCombatComponent.m_BattleUnit.MaxShield);

                    UpdateGas(target.m_MobCombatComponent.m_BattleUnit,
                    target.m_MobCombatComponent.m_BattleUnit.CurGas, target.m_MobCombatComponent.m_BattleUnit.MaxGas);
                }
            }
        }

        //buff处理
        var bc = excuteData.BuffChange;
        if (bc != null)
        {
            var buffMob = MobManager.Instance.GetMob(bc.UnitId);
            if (buffMob != null)
            {
                if (excuteTurnIndex >= _excuteTurnIndex)
                    buffMob.CacheProcessBuffChange(bc);

                buffMob.DoProcessBuffChange(true);
            }
        }

        DoImmediatelyExcuteDataUnitsChange(excuteData, excuteTurnIndex, excuteDataIndex);

        var bub = excuteData.UnitBase;
        if (bub != null)
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

        var sc = excuteData.StageChange;
        if (sc != null)
        {
            m_CurServerBattleStage = sc.NewStage;
        }
    }
    
    private void DoImmediatelyExcuteDataUnitsChange(ExcuteData excuteData, int excuteTurnIndex, int excuteDataIndex, bool isForceForFlashNewUnit = true)
    {
        var uc = excuteData.UnitsChange;
        if (uc != null)
        {
            if (uc.DelUnitId != null && uc.DelUnitId.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.DelUnitId.Count; ucIndex < ucCount; ucIndex++)
                {
                    MobEntity target = MobManager.Instance.GetMob(uc.DelUnitId[ucIndex]);
                    if (target != null)
                    {
                        DLogManager.Log(ELogType.eCombat, $"DoImmediatelyExcuteDataUnitsChange()----<color=yellow>excuteTurnIndex:{excuteTurnIndex.ToString()}   excuteDataIndex:{excuteDataIndex.ToString()}   DelUnitId   UnitId : {uc.DelUnitId[ucIndex].ToString()}</color>");
                        
                        target.m_MobCombatComponent.m_BeDelUnit = true;
                        target.m_MobCombatComponent.m_BattleUnit.CurHp = 0;

                        OnAddOrDelUnit(true, target.m_MobCombatComponent.m_BattleUnit, false);

                        target.m_MobCombatComponent.ResetMobState(true, 0);
                    }
                }
            }
            if (uc.DelFailUnitId != null && uc.DelFailUnitId.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.DelFailUnitId.Count; ucIndex < ucCount; ucIndex++)
                {
                    MobEntity target = MobManager.Instance.GetMob(uc.DelFailUnitId[ucIndex]);
                    if (target != null)
                    {
                        DLogManager.Log(ELogType.eCombat, $"DoImmediatelyExcuteDataUnitsChange()----<color=yellow>excuteTurnIndex:{excuteTurnIndex.ToString()}   excuteDataIndex:{excuteDataIndex.ToString()}   DelFailUnitId   UnitId : {uc.DelFailUnitId[ucIndex].ToString()}</color>");

                        OnAddOrDelUnit(false, target.m_MobCombatComponent.m_BattleUnit, false);

                        target.m_MobCombatComponent.ResetTrans();
                    }
                }
            }
            if (uc.NewUnits != null && uc.NewUnits.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.NewUnits.Count; ucIndex < ucCount; ucIndex++)
                {
                    var battleUnit = uc.NewUnits[ucIndex];
                    if (battleUnit == null)
                        continue;

                    bool isImmediately = true;
                    if (uc.ReplaceType == 3 && !isForceForFlashNewUnit)
                    {
                        var oldMob = MobManager.Instance.GetMobByServerNum(battleUnit.Pos);
                        if (oldMob != null)
                            isImmediately = false;
                    }

                    if (isImmediately)
                    {
                        DLogManager.Log(ELogType.eCombat, $"DoImmediatelyExcuteDataUnitsChange()----<color=yellow>excuteTurnIndex:{excuteTurnIndex.ToString()}   excuteDataIndex:{excuteDataIndex.ToString()}   NewUnits   UnitId : {battleUnit.UnitId.ToString()}   ServerNum:{battleUnit.Pos.ToString()}  uc.ReplaceType:{uc.ReplaceType.ToString()}</color>");

                        CreateNewUnit(uc, battleUnit, excuteTurnIndex);
                    }
                    else
                    {
                        DLogManager.Log(ELogType.eCombat, $"该位置有战斗单位，等待触发新战斗单位----<color=yellow>excuteTurnIndex:{excuteTurnIndex.ToString()}   excuteDataIndex:{excuteDataIndex.ToString()}   NewUnits   UnitId : {battleUnit.UnitId.ToString()}   ServerNum:{battleUnit.Pos.ToString()}  uc.ReplaceType:{uc.ReplaceType.ToString()}</color>");

                        CacheDelayBirthNetNewUnitData cacheDelayBirthNetNewUnitData = BasePoolClass.Get<CacheDelayBirthNetNewUnitData>();
                        cacheDelayBirthNetNewUnitData.m_BattleUnitChange = uc;
                        cacheDelayBirthNetNewUnitData.m_BattleUnit = battleUnit;
                        cacheDelayBirthNetNewUnitData.m_ExcuteTurnIndex = excuteTurnIndex;

                        _cacheDelayBirthNetNewUnitDatas.Add(cacheDelayBirthNetNewUnitData);
                    }
                }
            }
            if (uc.EscapeUnitId != null && uc.EscapeUnitId.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.EscapeUnitId.Count; ucIndex < ucCount; ucIndex++)
                {
                    MobEntity target = MobManager.Instance.GetMob(uc.EscapeUnitId[ucIndex]);
                    if (target != null)
                    {
                        DLogManager.Log(ELogType.eCombat, $"DoImmediatelyExcuteDataUnitsChange()----<color=yellow>excuteTurnIndex:{excuteTurnIndex.ToString()}   excuteDataIndex:{excuteDataIndex.ToString()}   EscapeUnitId   UnitId : {uc.EscapeUnitId[ucIndex].ToString()}</color>");
                        
                        target.m_MobCombatComponent.m_IsRunAway = true;
                        target.m_MobCombatComponent.m_BattleUnit.CurHp = 0;

                        OnAddOrDelUnit(true, target.m_MobCombatComponent.m_BattleUnit, false);

                        target.m_MobCombatComponent.ResetMobState(true, 2);
                        
                        if (MobManager.Instance.IsPlayer(target.m_MobCombatComponent.m_BattleUnit))
                        {
                            m_BattleOverType = 1u;
                        }
                    }
                }
            }
            if (uc.EscapeFailUnitId != null && uc.EscapeFailUnitId.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.EscapeFailUnitId.Count; ucIndex < ucCount; ucIndex++)
                {
                    MobEntity target = MobManager.Instance.GetMob(uc.EscapeFailUnitId[ucIndex]);
                    if (target != null)
                    {
                        DLogManager.Log(ELogType.eCombat, $"DoImmediatelyExcuteDataUnitsChange()----<color=yellow>excuteTurnIndex:{excuteTurnIndex.ToString()}   excuteDataIndex:{excuteDataIndex.ToString()}   EscapeFailUnitId   UnitId : {uc.EscapeFailUnitId[ucIndex].ToString()}</color>");

                        target.m_MobCombatComponent.ResetTrans();
                    }
                }
            }
            if (uc.UnitPosChange != null && uc.UnitPosChange.Count > 0)
            {
                for (int ucIndex = 0, ucCount = uc.UnitPosChange.Count; ucIndex < ucCount; ucIndex++)
                {
                    var unitPosChange = uc.UnitPosChange[ucIndex];
                    if (unitPosChange == null)
                        continue;

                    MobEntity target = MobManager.Instance.GetMob(unitPosChange.UnitId);
                    if (target != null)
                    {
                        DLogManager.Log(ELogType.eCombat, $"DoImmediatelyExcuteDataUnitsChange()----<color=yellow>excuteTurnIndex:{excuteTurnIndex.ToString()}   excuteDataIndex:{excuteDataIndex.ToString()}   UnitPosChangeId : {unitPosChange.UnitId.ToString()}--ServerPos:{unitPosChange.Pos.ToString()}--clientNum:{target.m_MobCombatComponent.m_ClientNum.ToString()})</color>");
                        target.m_MobCombatComponent.ChangeUnitServerPos((int)unitPosChange.Pos, true);
                    }
                }
            }
        }
    }

    public void DoBattleResult(bool isForceOver = false)
    {
        if (isForceOver)
            m_BattleOverType = 99u;

        int clearTurnState = ClearTurn();

        m_IsNeedPlayNextVideo = false;

        if (m_BattleOverType != 2)
        {
            MobEntity mobEntity = MobManager.Instance.GetPlayerMob();
            if (mobEntity != null)
                BattleShowRoundEndReq(CombatManager.Instance.m_BattleId, mobEntity.m_MobCombatComponent.m_BattleUnit.UnitId, m_CurRound);
            else if (!m_IsVideo)
            {
                DebugUtil.LogError($"DoBattleResult 获取PlayerMob为null  m_BattleOverType:{m_BattleOverType.ToString()}");

                if (clearTurnState == 1)
                {
                    m_RoundOver = false;
                    _sentOverRoundReq = false;
                }
            }

            if (m_IsVideo && !m_IsPauseVideo)
            {
                m_IsNeedPlayNextVideo = true;
            }
        }

        if (m_BattleOverType != 0u)
        {
            DLogManager.Log(ELogType.eCombat, $"退出场景: m_BattleOverType--{m_BattleOverType.ToString()}");

            m_IsNeedPlayNextVideo = false;

            //战斗结束，退出场景前的处理放该函数处理
            DoOtherThingsBeforeExitBattle();

            uint endWorkId = (!CombatManager.Instance.m_IsFight || CombatManager.Instance.m_BattleTypeTb == null) ? 0 : CombatManager.Instance.m_BattleTypeTb.battle_end_workid;

            Sys_Fight.Instance.ExitFight();

            if (endWorkId > 0u)
            {
                WS_CombatBehaveAIManagerEntity.StartCombatBehaveAI02<WS_CombatSceneAIControllerEntity>(endWorkId);
            }

            eventEmitter.Trigger<bool>(EEvents.OnBattleOver, true);
        }

        CombatManager.Instance.ResetNormalTimeScale();

        if (m_IsNeedPlayNextVideo)
        {
            m_IsNeedPlayNextVideo = false;
            PlayVideoNextRound();
        }
    }

    //战斗结束，退出场景前的处理
    private void DoOtherThingsBeforeExitBattle()
    {

        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnClearBattleFlag);


    }

    public void CheckCancelBattle()
    {
        if (m_BattleOverType != 3u)
            return;

        if (MobManager.Instance.IsHaveBehaveMob())
            return;

        CombatSceneEntity.Instance.GetNeedComponent_Repeat<CombatTimerComponent>().Init(0.5f, () =>
        {
            DoBattleResult();
        });
    }

    public void UpdateHp(BattleUnit unit, int curHp, uint maxHp)
    {
        float ratio = curHp > 0 ? (float)curHp / maxHp : 0f;

        HpValueChangedEvt hvce = CombatObjectPool.Instance.Get<HpValueChangedEvt>();
        hvce.id = unit.UnitId;
        hvce.ratio = ratio;
        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateHp, hvce);
        CombatObjectPool.Instance.Push(hvce);

        if (unit.RoleId == Sys_Role.Instance.Role.RoleId)
        {
            if ((UnitType)unit.UnitType == UnitType.Hero)
            {
                HpValueUpdateEvt rhp = CombatObjectPool.Instance.Get<HpValueUpdateEvt>();
                rhp.id = 0u;
                rhp.ratio = ratio;
                eventEmitter.Trigger<HpValueUpdateEvt>(EEvents.OnUpdateHp, rhp);
                CombatObjectPool.Instance.Push(rhp);
            }
            else if ((UnitType)unit.UnitType == UnitType.Pet)
            {
                HpValueUpdateEvt rhp = CombatObjectPool.Instance.Get<HpValueUpdateEvt>();
                rhp.id = 1u;
                rhp.ratio = ratio;
                eventEmitter.Trigger<HpValueUpdateEvt>(EEvents.OnUpdateHp, rhp);
                CombatObjectPool.Instance.Push(rhp);
            }
        }

        if ((UnitType)unit.UnitType == UnitType.Monster && CombatManager.Instance.m_BattleTypeTb.show_UI_hp)
        {
            eventEmitter.Trigger(EEvents.OnUpdateBossBlood, unit, curHp, maxHp);
        }
    }

    public void UpdateMp(BattleUnit unit, int curMp, uint maxMp)
    {
        float ratio = curMp > 0 ? (float)curMp / maxMp : 0f;

        MpValueChangedEvt mvce = CombatObjectPool.Instance.Get<MpValueChangedEvt>();
        mvce.id = unit.UnitId;
        mvce.ratio = ratio;
        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateMp, mvce);
        CombatObjectPool.Instance.Push(mvce);

        if (unit.RoleId == Sys_Role.Instance.Role.RoleId)
        {
            if ((UnitType)unit.UnitType == UnitType.Hero)
            {
                MpValueUpdateEvt rmp = CombatObjectPool.Instance.Get<MpValueUpdateEvt>();
                rmp.id = 0u;
                rmp.ratio = ratio;
                eventEmitter.Trigger<MpValueUpdateEvt>(EEvents.OnUpdateMp, rmp);
                CombatObjectPool.Instance.Push(rmp);
            }
            else if ((UnitType)unit.UnitType == UnitType.Pet)
            {
                MpValueUpdateEvt rmp = CombatObjectPool.Instance.Get<MpValueUpdateEvt>();
                rmp.id = 1u;
                rmp.ratio = ratio;
                eventEmitter.Trigger<MpValueUpdateEvt>(EEvents.OnUpdateMp, rmp);
                CombatObjectPool.Instance.Push(rmp);
            }
        }
    }

    public void UpdateShield(BattleUnit unit, int curShield, uint maxShield)
    {
        float ratio = curShield > 0 ? (float)curShield / maxShield : 0f;

        ShieldValueChangedEvt svce = CombatObjectPool.Instance.Get<ShieldValueChangedEvt>();
        svce.id = unit.UnitId;
        svce.ratio = ratio;
        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateShield, svce);
        CombatObjectPool.Instance.Push(svce);
    }

    public void UpdateGas(BattleUnit unit, int curGas, int maxGas)
    {
        float ratio = curGas > 0 ? (float)curGas / maxGas : 0f;

        EnergyValueChangedEvt svce = CombatObjectPool.Instance.Get<EnergyValueChangedEvt>();
        svce.id = unit.UnitId;
        svce.ratio = ratio;
        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateEnergy, svce);
        CombatObjectPool.Instance.Push(svce);
    }

    public void UpdateBuff(MobEntity mobEntity, BattleUnit unit)
    {
        if (mobEntity == null || unit == null)
            return;

        for (int i = 0; i < unit.BuffList.Count; i++)
        {
            var bc = unit.BuffList[i];
            if (bc != null)
            {
                mobEntity.UpdateBuffComponentByBehave(bc);
            }
        }
    }

    public ExcuteTurn GetNetExcuteTurnData(int excuteIndex)
    {
        if (_excuteTurnList == null)
            return null;

        if (excuteIndex >= _excuteTurnList.Count || excuteIndex < 0)
        {
            DebugUtil.LogError($"Net Datat ExcuteTurn-----excuteIndex:{excuteIndex.ToString()}下标超标了");
            return null;
        }

        return _excuteTurnList[excuteIndex];
    }

    public void CreateBloodHub(BattleUnit unit, int clientNum, GameObject go)
    {
        DLogManager.Log(ELogType.eCombat, $"CreateBloodHub----<color=yellow>UnitId:{unit.UnitId.ToString()}  clientNum:{clientNum.ToString()}   {go?.name}</color>");

        CreateBloodEvt createBloodEvt = CombatObjectPool.Instance.Get<CreateBloodEvt>();
        createBloodEvt.id = unit.UnitId;
        createBloodEvt.ClientNum = clientNum;
        createBloodEvt.gameObject = go;
        createBloodEvt.side = unit.Side;

        if (unit.UnitType != (int)UnitType.Zero)
        {
            createBloodEvt.attributeData = new AttributeData();
            createBloodEvt.attributeData.fightUnitType = unit.UnitType;
            createBloodEvt.attributeData.UnitLevel = unit.Level;
            if (unit.UnitType == (int)UnitType.Hero)
            {
                for (int i = 0; i < unit.EleAttr.Count; i++)
                {
                    var eAttr = unit.EleAttr[i];

                    createBloodEvt.attributeData.playerAttr[eAttr.AttrId] = eAttr.Value;
                }
                //if (!Sys_Team.Instance.HaveTeam)
                //{
                //    Hero hero = GameCenter.mainWorld.GetActor(Hero.Type,unit.RoleId) as Hero;
                //    if (hero != null)
                //    {
                //        createBloodEvt.attributeData.playerName = World.GetComponent<HeroBaseComponent>(hero).Name;
                //    }
                //    else
                //    {
                //        DebugUtil.LogWarningFormat("场景中不存在此玩家{0}", unit.RoleId);
                //    }
                //}
                //else
                //{
                //    TeamMem teamMem = Sys_Team.Instance.getTeamMem(unit.RoleId);
                //    if (teamMem != null)
                //    {
                //        createBloodEvt.attributeData.playerName = teamMem.Name.ToStringUtf8();
                //    }
                //}
                if (unit.ServerUnitType == 8)//怪物模拟真人(服务器给的类型是hero，但其实需要读怪物表)
                {
                    createBloodEvt.attributeData.notPlayerAttr = (uint)unit.RoleId;
                }
                else
                {
                    createBloodEvt.attributeData.playerName = unit.RoleName.ToStringUtf8();
                    createBloodEvt.attributeData.ShapeShiftId = unit.ShapeShiftId;
                    createBloodEvt.attributeData.RoleCareer = unit.RoleCareer;
                }
            }
            else
            {
                createBloodEvt.attributeData.notPlayerAttr = unit.UnitInfoId;
                createBloodEvt.attributeData.petName = unit.PetName.ToStringUtf8();

                if (CombatManager.Instance.m_BattleTypeTb.show_hp_tie > 0)
                {
                    Transform bindTrans = MobManager.Instance.GetPosByMobBind(unit.UnitId, CombatManager.Instance.m_BattleTypeTb.show_hp_tie);
                    if (bindTrans != null)
                    {
                        createBloodEvt.gameObject = bindTrans.gameObject;
                    }
                }
            }
        }

        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnCreateBlood, createBloodEvt);
        createBloodEvt.attributeData = null;
        CombatObjectPool.Instance.Push(createBloodEvt);
    }

    public bool NeedDoExcuteTurn(uint unitId)
    {
        if (_excuteTurnList == null || _excuteTurnList.Count == 0)
            return false;

        for (int i = 0, count = _excuteTurnList.Count; i < count; i++)
        {
            var excuteTurn = _excuteTurnList[i];
            if (excuteTurn == null || excuteTurn.ExcuteData == null || excuteTurn.ExcuteData.Count == 0)
                continue;

            for (int dataIndex = 0, dataCount = excuteTurn.ExcuteData.Count; dataIndex < dataCount; dataIndex++)
            {
                BattleHpMpChange hc = excuteTurn.ExcuteData[dataIndex].HpChange;
                if (hc == null)
                    continue;

                if (hc.UnitId == unitId)
                    return true;
            }
        }

        return false;
    }

    public bool IsCallingNewBattleUnit(uint battleUnitId)
    {
        if (_excuteTurnList == null)
        {
            DebugUtil.LogError($"IsCallingNewBattleUnit方法中_excuteTurnList为null");
            return false;
        }

        if (_excuteTurnIndex > _excuteTurnList.Count)
        {
            DebugUtil.LogError($"IsCallingNewBattleUnit()时，_excuteTurnList:{_excuteTurnList.Count.ToString()}的下标{_excuteTurnIndex.ToString()}超了");
            return false;
        }

        for (int i = 0; i < _excuteTurnIndex - 1; i++)
        {
            ExcuteTurn excuteTurn = _excuteTurnList[i];
            for (int dataIndex = 0, dataCount = excuteTurn.ExcuteData.Count; dataIndex < dataCount; dataIndex++)
            {
                ExcuteData ed = excuteTurn.ExcuteData[dataIndex];
                var uc = ed.UnitsChange;
                if (uc != null && uc.NewUnits != null && uc.NewUnits.Count > 0)
                {
                    for (int newUnitIndex = 0; newUnitIndex < uc.NewUnits.Count; newUnitIndex++)
                    {
                        if (uc.NewUnits[newUnitIndex].UnitId == battleUnitId)
                            return true;
                    }
                }
            }
        }

        return false;
    }

    public void OnAddOrDelUnit(bool isSuccess, BattleUnit battleUnit, bool isAdd)
    {
        if (isSuccess)
        {
            if (battleUnit != null)
            {
                if (battleUnit.UnitType == (uint)UnitType.Pet && battleUnit.RoleId == Sys_Role.Instance.Role.RoleId)
                {
                    if (!isAdd && GameCenter.mainFightPet != null)
                    {
                        if (GameCenter.mainFightPet.battleUnit == null)
                        {
                            DebugUtil.LogError($"OnAddOrDelUnit方法   isSuccess:{isSuccess.ToString()}   battleUnit.UnitId:{battleUnit.UnitId.ToString()} isAdd:{isAdd.ToString()}    GameCenter.mainFightPet.battleUnit为null");
                        }
                        else if (GameCenter.mainFightPet.battleUnit.PetId == battleUnit.PetId)
                        {
                            GameCenter.mainFightPet = null;
                            Sys_Plan.Instance.eventEmitter.Trigger<ulong, uint, uint>(Sys_Plan.EEvents.OnChangeFightPet, 0, 0, 0);
                        }
                        if (!GameCenter.fightControl.forbidpetsList.Contains((uint)battleUnit.PetId))
                        {
                            GameCenter.fightControl.forbidpetsList.Add((uint)battleUnit.PetId);
                        }
                    }
                    eventEmitter.Trigger(EEvents.OnUpdatePet, isAdd, battleUnit.PetId);
                }
            }
        }
    }

    public void SetForceDoExcuteTurnTime(float time)
    {
        _time = 0f;
        m_ForceDoExcuteTurnTime = time;
    }

    public int GetExcuteTurnIndex(ExcuteTurn excuteTurn)
    {
        if (_excuteTurnList == null || _excuteTurnList.Count <= 0)
            return -1;

        for (int i = 0, count = _excuteTurnList.Count; i < count; i++)
        {
            if (_excuteTurnList[i] == excuteTurn)
                return i;
        }

        return -1;
    }

    public uint GetExcuteTurnStage(int excuteTurnIndex)
    {
        if (_excuteTurnList == null || _excuteTurnList.Count <= 0 ||
            excuteTurnIndex < 0 || excuteTurnIndex >= _excuteTurnList.Count)
            return 0u;

        ExcuteTurn excuteTurn = _excuteTurnList[excuteTurnIndex];
        if (excuteTurn == null)
            return 0u;

        return excuteTurn.Stage;
    }

    /// <summary>
    /// 0=平局，1=胜利，=2失败
    /// </summary>
    public int GetBattleOverResult()
    {
        if (_battleResult == 0 || _battleResult == 3)
            return 0;
        else if ((m_IsAttackSide && _battleResult == 1) ||
            (!m_IsAttackSide && _battleResult == 2))
            return 1;
        else
            return 2;
    }

    public void ClearWatchData()
    {
        m_IsWatchBattle = false;
        m_WatchSide = 0u;
        m_BeWatchRoleId = 0ul;
    }

    /// <summary>
    /// 真实的实时战斗，不是观战也不是录像
    /// </summary>
    public bool IsRealCombat()
    {
        return !m_IsWatchBattle && !m_IsVideo;
    }
    #endregion
}