using UnityEngine;
using Logic.Core;
using Net;
using Packet;
using System.Collections.Generic;
using Table;
using Lib.Core;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using Framework;

namespace Logic
{
    public partial class Sys_Fight : SystemModuleBase<Sys_Fight>
    {
        private uint levelID;
        public List<BattleUnit> battleUnits = new List<BattleUnit>();

        public Action<CSVBattleType.Data> OnEnterFight;
        public Action OnExitFight;

        public AutoFight AutoFightData = new AutoFight();

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public static uint curMonsterGroupId;

        //RoleId
        public Dictionary<ulong, FightHero> FightHeros = new Dictionary<ulong, FightHero>();

        //UnitId
        public Dictionary<uint, Monster> MonsterDic = new Dictionary<uint, Monster>();
        public Dictionary<uint, FightPet> FightPetDic = new Dictionary<uint, FightPet>();
        public Dictionary<uint, FightHero> FightHeroDic = new Dictionary<uint, FightHero>();

        //mainfightHero Side
        public int mainSide;

        public enum FightState
        {
            None,
            Fight,
        }

        //public FightState fightState { get; private set; } = FightState.None;

        public enum EEvents
        {
            //OnEnterEffect,
            //OnExitEffect, //退出战斗,播放过渡效果后

            StartReConnect,
            Reconnected,
        }

        public uint BattleTypeId;

        public Dictionary<uint, GameObject> cacheMonsterGos = new Dictionary<uint, GameObject>();

        public uint[] cachItemUseCounts = new uint[2] {0, 0}; //玩家战斗内使用道具数量 [0]:1页签 [1]:2页签
        
        public Dictionary<uint, uint> useRound = new Dictionary<uint, uint>();

        public override void Init()
        {
            base.Init();

            //初始化数据
            AutoFightData.AutoState = false;

            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdBattle.StartNtf, OnStartNtf, CmdBattleStartNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdBattle.StartReq, (ushort) CmdBattle.StartRes, OnStartRes, CmdBattleStartRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdBattle.EndReq, (ushort) CmdBattle.EndRes, OnEndRes, CmdBattleEndRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdBattle.AutoFightReq, (ushort) CmdBattle.AutoFightRes, OnAutoFightRes, CmdBattleAutoFightRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdBattle.SetAutoSkillReq, (ushort) CmdBattle.SetAutoSkillRes, OnSetAutoSkillRes, CmdBattleSetAutoSkillRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdBattle.DataNtf, OnDataNtf, CmdBattleDataNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdBattle.ResumeNtf, OnResumeNtf, CmdBattleResumeNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdBattle.CommandNtf, OnCommandNtf, CmdBattleCommandNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdBattle.EnterFailedNtf, OnEnterFailedNtf, CmdBattleEnterFailedNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdBattle.WatchNtf, OnWatchNtf, CmdBattleWatchNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdBattle.ResumeEndNtf, OnResumeEndNtf, CmdBattleResumeEndNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattle.ScoreInfoNtf, OnScoreInfoNtf, CmdBattleScoreInfoNtf.Parser);
        }

        #region Combat_Net

        void OnEnterFailedNtf(NetMsg msg)
        {
            CmdBattleEnterFailedNtf ntf = NetMsgUtil.Deserialize<CmdBattleEnterFailedNtf>(CmdBattleEnterFailedNtf.Parser, msg);
            if (ntf != null)
            {
                if (ntf.Reason == BattleEnterFailedReason.Item)
                {
                    CSVBattleDrop.Data cSVBattleDropData = CSVBattleDrop.Instance.GetConfData(ntf.MonterTeam);
                    if (cSVBattleDropData != null && cSVBattleDropData.costItem != null)
                    {
                        string str = string.Empty;
                        for (int index = 0, len = cSVBattleDropData.costItem.Count; index < len; index++)
                        {
                            str += LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(cSVBattleDropData.costItem[index][0]).name_id);
                        }

                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3290000001, str));
                    }
                }
            }
        }

        //临时处理
        public void StartFightReq(uint levelId = 1u)
        {
            //CmdBattleStartReq req = new CmdBattleStartReq();
            //req.LevelId = levelId;
            //byte[] data = NetMsgUtil.Serialize((ushort)CmdBattle.StartReq, req);
            //NetClient.Instance.SendData(data);            
            //NetClient.Instance.SendMessage((ushort)CmdBattle.StartReq, req);

            CmdGmReq req = new CmdGmReq();
            //req.Roleid = Sys_Player.Instance.m_RoleId;
            req.Cmd = Google.Protobuf.ByteString.CopyFrom("fight", System.Text.Encoding.UTF8);
            req.Param = Google.Protobuf.ByteString.CopyFrom(levelId.ToString(), System.Text.Encoding.UTF8);

            NetClient.Instance.SendMessage((ushort) CmdGm.Req, req);
        }

        private void OnStartRes(NetMsg msg)
        {
        }

        /// <summary>
        /// 进战斗发一次
        /// </summary>
        /// <param name="msg"></param>
        private void OnStartNtf(NetMsg msg)
        {
            CmdBattleStartNtf ntf = NetMsgUtil.Deserialize<CmdBattleStartNtf>(CmdBattleStartNtf.Parser, msg);
            DoStartNtf(ntf);
        }

        public void DoStartNtf(CmdBattleStartNtf ntf)
        {
            battleUnits.Clear();

            for (int i = 0; i < cachItemUseCounts.Length; i++)
            {
                cachItemUseCounts[i] = 0;
            }
            
            useRound.Clear();

            if (CombatManager.Instance.m_IsFight)
            {
                CombatManager.Instance.OnDisable();
            }

            curMonsterGroupId = ntf.MonsterGroupId;
            levelID = ntf.LevelId;
            CombatManager.Instance.m_BattleId = ntf.BattleId;
            CombatManager.Instance.m_CombatStyleState = 0;
            Net_Combat.Instance.ClearWatchData();
            Net_Combat.Instance.m_IsStartResume = false;
            Net_Combat.Instance.m_IsReconnect = false;
            Net_Combat.Instance.DoClearDelayRoundData();
            Net_Combat.Instance.m_ReconnectBattleState = 0;
            Net_Combat.Instance.m_IsExecuteState = false;
            Net_Combat.Instance.m_TeachingID = ntf.Teachid;
            Net_Combat.Instance.m_CurServerBattleStage = ntf.StageSwitch;
            BattleTypeId = ntf.BattleTypeId;
            CombatManager.Instance.SetBattleTypeData(ntf.BattleTypeId, ntf.LevelId, ntf.Units);
            if (CombatManager.Instance.m_BattleTypeTb != null)
                Net_Combat.Instance.RoundCountDown = CombatManager.Instance.m_BattleTypeTb.setup_time;

            DLogManager.Log(ELogType.eCombat, $"收到OnStartNtf----BattleTypeId:{ntf.BattleTypeId.ToString()}   curMonsterGroupId:{curMonsterGroupId.ToString()}");

            foreach (var data in ntf.Units)
            {
                DLogManager.Log(ELogType.eCombat, $"<color=yellow>CmdBattleStartNtf的UnitId : {data.UnitId.ToString()}   Pos : {data.Pos.ToString()}</color>");

                battleUnits.Add(data);
            }

            for (int i = 0; i < battleUnits.Count; i++)
            {
                if (battleUnits[i].RoleId == Sys_Role.Instance.RoleId)
                {
                    if (battleUnits[i].UseItemTimes.Count == 1)
                    {
                        cachItemUseCounts[0] = battleUnits[i].UseItemTimes[0];
                    }
                    else if (battleUnits[i].UseItemTimes.Count == 2)
                    {
                        cachItemUseCounts[0] = battleUnits[i].UseItemTimes[0];
                        cachItemUseCounts[1] = battleUnits[i].UseItemTimes[1];
                    }
                }
            }

            Net_Combat.Instance.fightPet = ntf.Pet == null ? 0u : ntf.Pet.Fire;
            Net_Combat.Instance.petsInBattle.Clear();
            if (ntf.Pet != null)
            {
                for (int i = 0; i < ntf.Pet.Pets.Count; ++i)
                {
                    Net_Combat.Instance.petsInBattle.Add(ntf.Pet.Pets[i]);
                }
            }
            
            EnterFight(levelID);

            if (GameCenter.mainFightHero != null && GameCenter.mainFightHero.battleUnit != null)
            {
                AutoFightData.AutoState = GameCenter.mainFightHero.battleUnit.AutoFight == 1;
            }
            else if (!Net_Combat.Instance.m_IsVideo)
            {
                Debug.LogError("not found mainfighthero or mainfighthero.battleUnit is null ");
            }
        }

        private Timer _timeDelayResume;
        private Timer _timeRound;

        void OnResumeNtf(NetMsg msg)
        {
            DebugUtil.LogErrorFormat("OnResumeNtf");
            
            CmdBattleResumeNtf ntf = NetMsgUtil.Deserialize<CmdBattleResumeNtf>(CmdBattleResumeNtf.Parser, msg);

            if (CombatManager.Instance.m_IsFight)
                CombatManager.Instance.OnDisable();

            Net_Combat.Instance.m_PosType = ntf.PosType;
            Net_Combat.Instance.m_TeachingID = ntf.Info.Teachid;

            DLogManager.Log(ELogType.eCombat,
                $"收到OnResumeNtf----ExcuteTurnCount:{(ntf.Info.Ntf == null ? null : ntf.Info.Ntf.Turns?.Count.ToString())}   {(ntf.Watchinfo == null ? "没有观看数据" : $"Side:{ntf.Watchinfo.Side.ToString()}   Bewatchid:{ntf.Watchinfo.Bewatchid.ToString()}")}");

            Net_Combat.Instance.ClearWatchData();
            if (ntf.Watchinfo != null && ntf.Watchinfo.Bewatchid > 0ul)
            {
                Net_Combat.Instance.m_WatchSide = ntf.Watchinfo.Side;
                Net_Combat.Instance.m_BeWatchRoleId = ntf.Watchinfo.Bewatchid;
            }

            if (ntf.Pet != null)
            {
                Net_Combat.Instance.fightPet = ntf.Pet.Fire;
                Net_Combat.Instance.petsInBattle.Clear();
                for (int i = 0; i < ntf.Pet.Pets.Count; ++i)
                {
                    Net_Combat.Instance.petsInBattle.Add(ntf.Pet.Pets[i]);
                }
            }

            DoBattleInfo(ntf.Info, () =>
            {
                GameCenter.fightControl.m_HeroSkillColdList.Clear();
                foreach (var data in ntf.Scds)
                {
                    uint time = CSVActiveSkill.Instance.GetConfData((uint) data.SkillId).cold_time;
                    uint coldTime = Net_Combat.Instance.m_CurRound - (uint) data.LastRound;
                    if (time != 0 && coldTime <= time)
                    {
                        GameCenter.fightControl.m_HeroSkillColdList.Add(data);
                    }
                }
            });

            for (int i = 0; i < ntf.Maincds.Count; i++)
            {
                uint mainSkillId = (uint)ntf.Maincds[i].SkillId;
                
                useRound[mainSkillId] = (uint) ntf.Maincds[i].LastRound;
            }
        }

        private void OnWatchNtf(NetMsg msg)
        {
            CmdBattleWatchNtf ntf = NetMsgUtil.Deserialize<CmdBattleWatchNtf>(CmdBattleWatchNtf.Parser, msg);

            DLogManager.Log(ELogType.eCombat, $"收到OnWatchNtf消息： {(ntf.Watchinfo == null ? "没有观看数据" : $"Side:{ntf.Watchinfo.Side.ToString()}   Bewatchid:{ntf.Watchinfo.Bewatchid.ToString()}")}");

            Net_Combat.Instance.ClearWatchData();
            if (ntf.Watchinfo != null && ntf.Watchinfo.Bewatchid > 0ul)
            {
                Net_Combat.Instance.m_WatchSide = ntf.Watchinfo.Side;
                Net_Combat.Instance.m_BeWatchRoleId = ntf.Watchinfo.Bewatchid;
            }

            DoBattleInfo(ntf.Info, null);
        }

        private void OnResumeEndNtf(NetMsg msg)
        {
            DLogManager.Log(ELogType.eCombat, $"收到OnResumeEndNtf消息： m_IsFight:{CombatManager.Instance.m_IsFight.ToString()}");

            if (CombatManager.Instance.m_IsFight)
            {
                DLogManager.Log(ELogType.eCombat, $"<color=red>客户端在战斗中，但服务器已经结束了战斗，通过该消息告知退出战斗</color>");
                Net_Combat.Instance.DoBattleResult(true);
            }
        }

        private void DoBattleInfo(BattleInfo battleInfo, Action action)
        {
            CombatManager.Instance.ResetNormalTimeScale();

            MobManager.Instance.StopMobsCombatState();

            if (battleInfo.BattleId == 0u || battleInfo.Units == null || battleInfo.Units.Count == 0)
            {
                if (CombatManager.Instance.m_IsFight)
                {
                    DLogManager.Log(ELogType.eCombat, $"<color=yellow>还在战斗中重连发送DoBattleInfo退出战斗：{battleInfo.BattleId.ToString()}</color>");
                    Net_Combat.Instance.DoBattleResult(true);
                }

                return;
            }

            Net_Combat.Instance.m_IsWatchBattle = true;
            foreach (var data in battleInfo.Units)
            {
                if (data.RoleId == Sys_Role.Instance.RoleId)
                {
                    Net_Combat.Instance.m_IsWatchBattle = false;
                    break;
                }
            }
            if (!Net_Combat.Instance.m_IsWatchBattle)
            {
                if (Net_Combat.Instance.fightPet == 0)
                {
                    Sys_Plan.Instance.eventEmitter.Trigger<ulong, uint, uint>(Sys_Plan.EEvents.OnChangeFightPet, 0, 0, 0);
                }
                else
                {
                    ClientPet clientPet = Sys_Pet.Instance.GetPetByUId(Net_Combat.Instance.fightPet);
                    Sys_Plan.Instance.eventEmitter.Trigger<ulong, uint, uint>(Sys_Plan.EEvents.OnChangeFightPet, clientPet.petUnit.Uid, clientPet.petUnit.PetPointPlanData.CurrentPlanIndex, clientPet.petUnit.EnhancePlansData.CurrentPlanIndex);
                }
            }

            DLogManager.Log(ELogType.eCombat, $"<color=yellow>DoBattleInfo处在m_IsWatchBattle：{Net_Combat.Instance.m_IsWatchBattle.ToString()}</color>");

            BattleTypeId = battleInfo.BattleTypeId;

            foreach (var battleUnit in battleUnits)
            {
                if (battleUnit.UnitType == (uint) EFightActorType.Monster)
                {
                    //GameCenter.fightWorld.DestroyActor(typeof(Monster), battleUnit.UnitId);
                    RemoveMonsterByUnitId(battleUnit.UnitId);
                }
                else if (battleUnit.UnitType == (uint) EFightActorType.Pet)
                {
                    //GameCenter.fightWorld.DestroyActor(typeof(FightPet), battleUnit.UnitId);
                    RemoveFightPetByUnitId(battleUnit.UnitId);
                }
                else if (battleUnit.UnitType == (uint) EFightActorType.Partner || battleUnit.UnitType == (uint) EFightActorType.Hero)
                {
                    //GameCenter.fightWorld.DestroyActor(typeof(FightHero), battleUnit.UnitId);
                    RemoveFightHeroByUnitId(battleUnit.UnitId);
                }
            }

            battleUnits.Clear();

            for (int i = 0; i < cachItemUseCounts.Length; i++)
            {
                cachItemUseCounts[i] = 0;
            }

            curMonsterGroupId = battleInfo.MonsterGroupId;

            levelID = battleInfo.LevelId;
            CombatManager.Instance.m_BattleId = battleInfo.BattleId;
            CombatManager.Instance.m_CombatStyleState = 0;

            CombatManager.Instance.SetBattleTypeData(battleInfo.BattleTypeId, battleInfo.LevelId, battleInfo.Units);

            Net_Combat.Instance.m_MultiBattleStage = battleInfo.CurStage;
            Net_Combat.Instance.m_CurStage_Round = battleInfo.CurstageRound;
            Net_Combat.Instance.m_CurServerBattleStage = battleInfo.StageSwitch;

            DLogManager.Log(ELogType.eCombat, $"收到DoBattleInfo----BattleTypeId:{battleInfo.BattleTypeId.ToString()}   curMonsterGroupId:{curMonsterGroupId.ToString()}");

            if (Net_Combat.Instance.m_CurRound <= battleInfo.CurRound)
            {
                Net_Combat.Instance.m_CurRound = battleInfo.CurRound;

                if (battleInfo.BattleState == 4)
                {
                    Net_Combat.Instance.m_ReconnectRoundNtf = battleInfo.Ntf;
                }

                Net_Combat.Instance.m_IsWaitDoExcute = true;
            }

            Net_Combat.Instance.m_IsStartResume = true;
            Net_Combat.Instance.m_IsReconnect = true;
            Net_Combat.Instance.m_ReconnectBattleState = battleInfo.BattleState;

            eventEmitter.Trigger(EEvents.StartReConnect);

            _timeRound?.Cancel();
            _timeDelayResume?.Cancel();
            _timeDelayResume = Timer.Register(0.5f, () =>
            {
                eventEmitter.Trigger(EEvents.Reconnected);

                Net_Combat.Instance.m_IsStartResume = false;
                Net_Combat.Instance.m_IsExecuteState = false;
                if (CombatManager.Instance.m_BattleTypeTb != null)
                    Net_Combat.Instance.RoundCountDown = CombatManager.Instance.m_BattleTypeTb.setup_time;

                DLogManager.Log(ELogType.eCombat,
                    "ntf.BattleId: " + battleInfo.BattleId + " ntf.LevelId: " + battleInfo.LevelId + "ntf.Units.Count: " + battleInfo.Units.Count +
                    $"   ntf.BattleState : {battleInfo.BattleState.ToString()}  ntf.CurRound:{battleInfo.CurRound.ToString()}  Net_Combat.Instance.m_CurRound:{Net_Combat.Instance.m_CurRound.ToString()}  ExcuteTurnCount:{battleInfo.Ntf?.Turns?.Count.ToString()}");

                foreach (var data in battleInfo.Units)
                {
                    DLogManager.Log(ELogType.eCombat, $"<color=yellow>Net BattleInfo的UnitId : {data.UnitId.ToString()}   Pos : {data.Pos.ToString()}</color>");

                    battleUnits.Add(data);
                }

                for (int i = 0; i < battleUnits.Count; i++)
                {
                    if (battleUnits[i].RoleId == Sys_Role.Instance.RoleId)
                    {
                        if (battleUnits[i].UseItemTimes.Count == 1)
                        {
                            cachItemUseCounts[0] = battleUnits[i].UseItemTimes[0];
                        }
                        else if (battleUnits[i].UseItemTimes.Count == 2)
                        {
                            cachItemUseCounts[0] = battleUnits[i].UseItemTimes[0];
                            cachItemUseCounts[1] = battleUnits[i].UseItemTimes[1];
                        }
                    }
                }
                
                //fightState = FightState.None;
                bool isEnterSuccess = EnterFight(battleInfo.LevelId);
                //UIManager.OpenUI(EUIID.UI_MainBattle);     

                if (Net_Combat.Instance.IsRealCombat())
                {
                    if (GameCenter.mainFightHero.battleUnit.AutoFight == 1)
                    {
                        AutoFightData.AutoState = true;
                    }
                    else
                    {
                        AutoFightData.AutoState = false;
                    }
                }

                GameCenter.mainHero.movementComponent.enableflag = false;
                if (!isEnterSuccess)
                {
                    MobManager.Instance.ResetMobsState();
                }

                GameCenter.fightControl.isDoRound = false;
                if (battleInfo.BattleState == 4)
                {
                    GameCenter.fightControl.operationState = FightControl.EOperationState.OperationOver;
                    Net_Combat.Instance.m_IsExecuteState = true;
                    GameCenter.fightControl.isDoRound = true;
                }
                else if (battleInfo.BattleState == 3)
                {
                    if (AutoFightData.AutoState)
                    {
                        Net_Combat.Instance.RoundCountDown = battleInfo.ExcuteTime;
                    }
                    else
                    {
                        //if (battleInfo.ExcuteTime > 2)
                        //{
                        //    Net_Combat.Instance.RoundCountDown = battleInfo.ExcuteTime - 2;
                        //}
                        //else
                        {
                            Net_Combat.Instance.RoundCountDown = battleInfo.ExcuteTime;
                        }
                    }

                    CombatManager.Instance.m_CurRoundStartTime = Time.unscaledTime;
                    GameCenter.fightControl.operationState = FightControl.EOperationState.WaitForFirstOperation;
                    Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnReconnect);
                }
                else if (battleInfo.BattleState == 1)
                {
                    GameCenter.fightControl.operationState = FightControl.EOperationState.WaitForFirstOperation;
                }

                if (action != null)
                    action.Invoke();
            }, null, false, false);
        }

        public void OnSimulateStart(Google.Protobuf.Collections.RepeatedField<BattleUnit> units)
        {
            battleUnits.Clear();

            levelID = 14001u;

            BattleTypeId = 999u;

            CombatManager.Instance.m_CombatStyleState = 999;
            Net_Combat.Instance.m_IsStartResume = false;
            Net_Combat.Instance.m_IsReconnect = false;
            Net_Combat.Instance.m_IsExecuteState = false;

            CombatManager.Instance.SetBattleTypeData(BattleTypeId, levelID, units);
            Net_Combat.Instance.RoundCountDown = CombatManager.Instance.m_BattleTypeTb.setup_time;

            foreach (var data in units)
            {
                DLogManager.Log(ELogType.eCombat, $"<color=yellow>OnSimulateStart的UnitId : {data.UnitId.ToString()}   Pos : {data.Pos.ToString()}</color>");

                battleUnits.Add(data);
            }

            EnterFight(levelID);
        }

        private void OnCommandNtf(NetMsg msg)
        {
            CmdBattleCommandNtf ntf = NetMsgUtil.Deserialize<CmdBattleCommandNtf>(CmdBattleCommandNtf.Parser, msg);
            if (ntf.Cmd.Count == 0)
            {
                DebugUtil.LogError("CmdBattleCommandNtf.Cmd.Count=0");
                return;
            }

            if (GameCenter.fightControl == null)
            {
                return;
            }

            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnUpdateTimeSand, ntf.Cmd[0].SrcUnitId, false);
            GameCenter.fightControl.hasCommends.Add(ntf.Cmd[0].SrcUnitId);

            //组队进行指令展示轮播
            if (Sys_Team.Instance.HaveTeam && CombatManager.Instance.m_BattleTypeTb.show_instruct_info && !Sys_Fight.Instance.AutoFightData.AutoState)
            {
                CreateOrderHUDEvt evt = new CreateOrderHUDEvt();
                evt.senderId = ntf.Cmd[0].SrcUnitId;
                foreach (var mob in MobManager.Instance.m_MobDic)
                {
                    if (mob.Value.m_MobCombatComponent.m_BattleUnit.Pos == ntf.Cmd[0].TarPos)
                        evt.receiverId = mob.Key;
                }

                if (ntf.Cmd[0].MainCmd == (uint) FightControl.EOperationType.Pet)
                {
                    if (ntf.Cmd[0].Param1 == 0)
                        evt.iconId = CSVActiveSkillInfo.Instance.GetConfData(103).icon;
                    else
                        evt.iconId = CSVActiveSkillInfo.Instance.GetConfData(104).icon;
                }
                else if (ntf.Cmd[0].MainCmd == (uint) FightControl.EOperationType.UseItem)
                {
                    if (CSVItem.Instance.TryGetValue(ntf.Cmd[0].Param1, out CSVItem.Data cSVItemData))
                    {
                        uint id = cSVItemData.active_skillid;
                        evt.iconId = CSVActiveSkillInfo.Instance.GetConfData(id).icon;
                    }
                }
                else
                {
                    if (CSVActiveSkillInfo.Instance.TryGetValue(ntf.Cmd[0].Param1, out CSVActiveSkillInfo.Data cSVActiveSkillInfoData))
                    {
                        evt.iconId = cSVActiveSkillInfoData.icon;
                    }
                }

                MobEntity senderMob = MobManager.Instance.GetMob(ntf.Cmd[0].SrcUnitId);
                if (senderMob != null && GameCenter.mainFightHero != null && !Net_Combat.Instance.m_IsWatchBattle)
                {
                    bool isSameCamp = CombatHelp.IsSameCamp(senderMob.GetComponent<MobCombatComponent>().m_BattleUnit, GameCenter.mainFightHero.battleUnit);
                    if (isSameCamp)
                    {
                        DebugUtil.LogFormat(ELogType.eBattleCommand, "技能轮播 :SenderID Is " + evt.senderId + "    ReceiverID is " + evt.receiverId + "   IconID is " + evt.iconId);
                        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnAddBattleOrder, evt);
                    }
                }
            }

            DebugUtil.LogFormat(ELogType.eBattleCommand, "技能指令转发 :源对象SrcUnitId Is " + ntf.Cmd[0].SrcUnitId + "目标位置 :TarPos Is " + ntf.Cmd[0].TarPos + "     命令参数:Param1 is: " + ntf.Cmd[0].Param1 + "     操作类型:MainCmd is: " + ntf.Cmd[0].MainCmd);
            Net_Combat.Instance.eventEmitter.Trigger<uint>(Net_Combat.EEvents.OnWaitOthersCommands, ntf.Cmd[0].SrcUnitId);
        }

        public void EndReq()
        {
            Sys_Task.Instance.InterruptCurrentTaskDoing();

            CmdBattleEndReq req = new CmdBattleEndReq();
            req.BattleId = CombatManager.Instance.m_BattleId;
            //             byte[] data = NetMsgUtil.Serialize((ushort)CmdBattle.EndReq, req);
            //             NetClient.Instance.SendData(data);

            NetClient.Instance.SendMessage((ushort) CmdBattle.EndReq, req);
        }

        private void OnEndRes(NetMsg msg)
        {
            ExitFight();
        }

        // Auto Fight
        public void AutoFightReq(bool auto)
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(60101, false))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3900004));
                Net_Combat.Instance.eventEmitter.Trigger<bool>(Net_Combat.EEvents.OnAutoFight, false);
                return;
            }
            CmdBattleAutoFightReq req = new CmdBattleAutoFightReq();
            req.StartAuto = auto ? (uint) 1 : (uint) 0;
            NetClient.Instance.SendMessage((ushort) CmdBattle.AutoFightReq, req);
            DebugUtil.LogFormat(ELogType.eBattleCommand, "send AutoFightReq , auto：" + auto);
        }

        private void OnAutoFightRes(NetMsg msg)
        {
            CmdBattleAutoFightRes res = NetMsgUtil.Deserialize<CmdBattleAutoFightRes>(CmdBattleAutoFightRes.Parser, msg);
            AutoFightData.AutoState = res.AutoState;
            Net_Combat.Instance.eventEmitter.Trigger<bool>(Net_Combat.EEvents.OnAutoFight, AutoFightData.AutoState);
            DebugUtil.LogFormat(ELogType.eBattleCommand, "receive AutoFightRes , auto：" + AutoFightData.AutoState);
        }

        public void SetAutoSkillReq(uint skillId, bool isFirst, bool hero = true, bool isSpecialSecondStage = false)
        {
            CmdBattleSetAutoSkillReq req = new CmdBattleSetAutoSkillReq();
            if (isSpecialSecondStage)
            {
                req.HeroSkillId = skillId;
                req.HeroSkillId2 = 1001;
            }
            else
            {
                if (hero)
                {
                    if (isFirst)
                    {
                        req.HeroSkillId = skillId;
                        if (skillId == 211 && !Sys_Fight.Instance.HasPet() && (GameCenter.fightControl.autoSkillEvt.heroid2 == 211 || GameCenter.fightControl.autoSkillEvt.heroid2 > 1003))
                        {
                            req.HeroSkillId2 = Constants.NEARNORMALATTACKID;
                        }
                    }
                    else
                    {
                        req.HeroSkillId2 = skillId;
                        if (skillId == 211 && !Sys_Fight.Instance.HasPet() && (GameCenter.fightControl.autoSkillEvt.heroid == 211 || GameCenter.fightControl.autoSkillEvt.heroid > 1003))
                        {
                            req.HeroSkillId = Constants.NEARNORMALATTACKID;
                        }
                    }
                }
                else
                {
                    req.PetSkillId = skillId;
                }
            }

            NetClient.Instance.SendMessage((ushort) CmdBattle.SetAutoSkillReq, req);
        }

        private void OnSetAutoSkillRes(NetMsg msg)
        {
            CmdBattleSetAutoSkillRes res = NetMsgUtil.Deserialize<CmdBattleSetAutoSkillRes>(CmdBattleSetAutoSkillRes.Parser, msg);
            AutoBattleSkillEvt skillids = new AutoBattleSkillEvt();
            if (res.HeroSkillId != 0)
            {
                skillids.heroid = res.HeroSkillId;
                GameCenter.fightControl.autoSkillEvt.heroid = skillids.heroid;
            }

            if (res.HeroSkillId2 != 0)
            {
                skillids.heroid2 = res.HeroSkillId2;
                GameCenter.fightControl.autoSkillEvt.heroid2 = skillids.heroid2;
            }

            if (res.PetSkillId != 0)
            {
                skillids.petid = res.PetSkillId;
                GameCenter.fightControl.autoSkillEvt.petid = skillids.petid;
            }

            Net_Combat.Instance.eventEmitter.Trigger<AutoBattleSkillEvt>(Net_Combat.EEvents.OnSetAutoSkill, skillids);
        }

        //登陆时，通知
        private void OnDataNtf(NetMsg msg)
        {
            CmdBattleDataNtf res = NetMsgUtil.Deserialize<CmdBattleDataNtf>(CmdBattleDataNtf.Parser, msg);
            AutoFightData = res.Data.AutoFight;
            Net_Combat.Instance.m_PosType =(uint) res.Data.PosType;
        }

        public void SendCmdBattleWatchReq(ulong beWatchRoleId)
        {
            CmdBattleWatchReq req = new CmdBattleWatchReq();
            req.Roleid = beWatchRoleId;

            NetClient.Instance.SendMessage((ushort) CmdBattle.WatchReq, req);
        }

        public void CmdBattleWatchQuitReq()
        {
            CmdBattleWatchQuitReq req = new CmdBattleWatchQuitReq();
            req.BattleId = CombatManager.Instance.m_BattleId;

            NetClient.Instance.SendMessage((ushort) CmdBattle.WatchQuitReq, req);
        }

        public void CmdBattleScoreInfoReq()
        {
            CmdBattleScoreInfoReq req = new CmdBattleScoreInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdBattle.ScoreInfoReq, req);
        }

        private void OnScoreInfoNtf(NetMsg msg)
        {
            CmdBattleScoreInfoNtf ntf = NetMsgUtil.Deserialize<CmdBattleScoreInfoNtf>(CmdBattleScoreInfoNtf.Parser, msg);
            if (GameCenter.fightControl == null)
            {
                return;
            }
            GameCenter.fightControl.groupScores.Clear();
            for(int i = 0; i < ntf.GroupScore.Count; ++i)
            {
                GameCenter.fightControl.groupScores.Add(ntf.GroupScore[i]);
            }
            Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnScoreInfo);
        }

        #endregion

        public override void OnLogin()
        {
            base.OnLogin();

            CombatManager.Instance.OnAwake();

            //fightState = FightState.None;
        }

        public override void OnLogout()
        {
            base.OnLogout();

            _timeRound?.Cancel();
            _timeDelayResume?.Cancel();
            CombatManager.Instance.OnDestroy();
        }

        public void UpdateFightScene(uint levelID, Action updataOver = null)
        {
            Vector3 fightSceneCenterPos = Vector3.zero;
            CSVBattleScene.Data cSVBattleSceneData = CSVBattleScene.Instance.GetConfData(levelID);
            if (cSVBattleSceneData != null)
            {
                CSVBattleScene.Data fightSceneCenterTb;
                if (Net_Combat.Instance.m_IsVideo)
                {
                    fightSceneCenterTb = CSVBattleScene.Instance.GetConfData(CombatManager.Instance.m_CommonBattleSceneId);
                }
                else
                {
                    fightSceneCenterTb = cSVBattleSceneData;
                }
                fightSceneCenterPos = new Vector3(fightSceneCenterTb.battle_scene_pointx / 10000f, fightSceneCenterTb.battle_scene_pointy / 10000f, fightSceneCenterTb.battle_scene_pointz / 10000f);

                CombatManager.Instance.SetCombatScene(cSVBattleSceneData, fightSceneCenterPos, Net_Combat.Instance.m_IsVideo);
            }
            else
            {
                DebugUtil.LogError($"cSVBattleSceneData is null, id: {levelID}");
            }

            GameCenter.mCameraController.EnterFight(fightSceneCenterPos, cSVBattleSceneData);
            updataOver?.Invoke();
        }

        private bool EnterFight(uint levelID)
        {
            //if (fightState == FightState.Fight)
            //    return false;

            //fightState = FightState.Fight;

            //GameCenter.fightWorld?.Clear();
         
            ClearActor();

            GameMain.Procedure.TriggerEvent(Sys_Fight.Instance, (int) EProcedureEvent.EnterFight);

            Sys_Bag.Instance.eventEmitter.Trigger(Sys_Bag.EEvents.OnResetMainBattleData);
            UpdateFightScene(levelID);

            CombatManager.Instance.OnEnable();

#if UNITY_EDITOR_NO_USE
            if (CombatManager.Instance.m_CombatStyleState == 999)
            {
                CreateSimulateFightHeros();
                CreateSimulateFightPets();
            }
            else
#endif
            {
                CreateFightHeros();
                CreateFightPets();
            }
            CreateMonsters();

            CSVBattleType.Data csvBatleType = CSVBattleType.Instance.GetConfData(BattleTypeId);
            OnEnterFight?.Invoke(csvBatleType);

            return true;
        }

        void CreateFightHeros()
        {
            FightHeros.Clear();

            //赋值mainfighthero的side,
            foreach (var unit in battleUnits)
            {
                if (unit.UnitType == (uint) EFightActorType.Hero && unit.RoleId == Sys_Role.Instance.RoleId)
                {
                    mainSide = unit.Side;
                }
            }

            foreach (var unit in battleUnits)
            {
                if (unit.UnitType == (uint) EFightActorType.Hero && unit.RoleId == Sys_Role.Instance.Role.RoleId)
                {
                    CreateMainFightHero(unit);
                }
            }

            foreach (var unit in battleUnits)
            {
                if ((unit.UnitType == (uint) EFightActorType.Hero && unit.RoleId != Sys_Role.Instance.Role.RoleId) || unit.UnitType == (uint) EFightActorType.Partner)
                {
                    CreateFightHero(unit);
                }
            }
        }

        public void CreateFightHero(BattleUnit battleUnit, bool isCalling = false)
        {
            //变身情况
            CSVTransform.Data transformTb = null;
            if (battleUnit.IsUseShapeShift == 0u && battleUnit.ShapeShiftId > 0u)
            {
                transformTb = CSVTransform.Instance.GetConfData(battleUnit.ShapeShiftId);
                if (transformTb == null)
                    DebugUtil.LogError($"CreateFightHero 变身表中没有数据ShapeShiftId:{battleUnit.ShapeShiftId.ToString()}");

                RemoveFightHeroByUnitId(battleUnit.UnitId);
            }

            //FightHero fightHero = GameCenter.fightWorld.CreateActor<FightHero>((uint)battleUnit.UnitId);
            FightHero fightHero = World.AllocActor<FightHero>((uint) battleUnit.UnitId);
            fightHero.SetName($"FightHero_{battleUnit.Pos.ToString()}");
            fightHero.SetParent(GameCenter.fightActorRoot);
            FightHeroDic.Add(battleUnit.UnitId, fightHero);

            fightHero.battleUnit = battleUnit;
            if (fightHero.battleUnit.WeaponId == 0)
            {
                fightHero.battleUnit.WeaponId = (int) Constants.UMARMEDID;
            }

            fightHero.careerComponent.UpdateCareerType((ECareerType) battleUnit.RoleCareer);
            if (battleUnit.UnitType == (uint) EFightActorType.Partner)
            {
                //fightHero.clickComponent = World.AddComponent<ClickComponent>(fightHero);
                fightHero.clickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                fightHero.clickComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                //fightHero.doubleClickComponent = World.AddComponent<DoubleClickComponent>(fightHero);
                fightHero.doubleClickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                fightHero.doubleClickComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                //fightHero.longpressComponent = World.AddComponent<LongPressComponent>(fightHero);
                fightHero.longpressComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                fightHero.longpressComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                //AnimationComponent animationComponent = World.AddComponent<AnimationComponent>(fightHero);

                //fightHero.weaponComponent = World.AddComponent<WeaponComponent>(fightHero);

                CSVPartner.Data cSVPartnerData = CSVPartner.Instance.GetConfData(battleUnit.UnitInfoId);
                Action<SceneActor> act = (actor) =>
                {
                    fightHero.weaponComponent.UpdateWeapon(cSVPartnerData.weaponID, false);
                    fightHero.animationComponent.SetSimpleAnimation(fightHero.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                    fightHero.animationComponent.UpdateHoldingAnimations(battleUnit.UnitInfoId, cSVPartnerData.weaponID);
                    MobManager.Instance.AddMob(battleUnit, fightHero.animationComponent, fightHero.gameObject, cSVPartnerData.weaponID, isCalling);
                };
                fightHero.LoadModel(cSVPartnerData.model, act);
            }
            else
            {
                //fightHero.clickComponent = World.AddComponent<ClickComponent>(fightHero);
                fightHero.clickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                fightHero.clickComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                //fightHero.doubleClickComponent = World.AddComponent<DoubleClickComponent>(fightHero);
                fightHero.doubleClickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                fightHero.doubleClickComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                //fightHero.longpressComponent = World.AddComponent<LongPressComponent>(fightHero);
                fightHero.longpressComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                fightHero.longpressComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                //AnimationComponent animationComponent = World.AddComponent<AnimationComponent>(fightHero);                

                Action<SceneActor> act = (actor) =>
                {
                    uint actionId = transformTb == null ? battleUnit.UnitInfoId : transformTb.action_id;
                    uint weaponId = transformTb == null ? (uint)battleUnit.WeaponId : transformTb.weapon_id;

                    fightHero.animationComponent.SetSimpleAnimation(fightHero.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                    fightHero.animationComponent.UpdateHoldingAnimations(actionId, weaponId);
                    fightHero.weaponComponent.UpdateWeapon(weaponId, false);
                    MobEntity mobEntity = MobManager.Instance.AddMob(battleUnit, fightHero.animationComponent, fightHero.gameObject, 
                        transformTb == null ? weaponId : (uint)battleUnit.WeaponId, isCalling);
                    if (mobEntity != null && fightHero.heroLoader != null &&
                        fightHero.heroLoader.datas != null && fightHero.heroLoader.datas.Count > 0)
                    {
                        MobCombatComponent mobCombatComponent = mobEntity.GetComponent<MobCombatComponent>();
                        if (mobCombatComponent != null)
                        {
                            if (fightHero.heroLoader.datas.TryGetValue(fightHero.heroLoader.showParts[(int) EHeroModelParts.Main], out List<dressData> dressDataList) &&
                                dressDataList != null && dressDataList.Count > 0)
                                mobCombatComponent.m_DressDataList = dressDataList;
                        }
                    }
                };

                if (transformTb == null)
                    fightHero.LoadModel(act);
                else
                    fightHero.LoadModel(transformTb.model, act);

                fightHero.heroSkillComponent.DoInit();

                FightHeros.Add(battleUnit.RoleId, fightHero);
            }
        }

        /// <summary>
        /// 生成战斗英雄角色///
        /// 战斗英雄变身
        /// </summary>
        /// <param name="battleUnit"></param>
        public void CreateMainFightHero(BattleUnit battleUnit, bool isCalling = false)
        {
            //变身情况
            CSVTransform.Data transformTb = null;
            if (battleUnit.IsUseShapeShift == 0u && battleUnit.ShapeShiftId > 0u)
            {
                transformTb = CSVTransform.Instance.GetConfData(battleUnit.ShapeShiftId);
                if (transformTb == null)
                    DebugUtil.LogError($"CreateMainFightHero 变身表中没有数据ShapeShiftId:{battleUnit.ShapeShiftId.ToString()}");

                if (battleUnit.WeaponId == 0)
                {
                    battleUnit.WeaponId = (int)Constants.UMARMEDID;
                }
            }

            if (GameCenter.mainFightHero != null)
            {
                //GameCenter.fightWorld.DestroyActor(GameCenter.mainFightHero);
                //GameCenter.mainFightHero = null;
                RemoveFightHeroByUnitId((uint) GameCenter.mainFightHero.uID);
                GameCenter.mainFightHero = null;
            }

            SuperHero superHero = null;
            GameObject templateObj = null;
            if (battleUnit.UnitType == (uint) EFightActorType.Hero && battleUnit.UnitInfoId == 1080)
            {
                templateObj = GameCenter.superHero.modelGameObject;
            }

            //FightHero fightHero = GameCenter.fightWorld.CreateActor<FightHero>((uint)battleUnit.UnitId);
            FightHero fightHero = World.AllocActor<FightHero>((uint) battleUnit.UnitId);
            fightHero.SetName($"FightHero_{battleUnit.Pos.ToString()}");
            fightHero.SetParent(GameCenter.fightActorRoot);
            FightHeroDic.Add(battleUnit.UnitId, fightHero);

            fightHero.battleUnit = battleUnit;

            if (battleUnit.UnitType == (uint) EFightActorType.Hero && battleUnit.RoleId == Sys_Role.Instance.Role.RoleId)
            {
                GameCenter.mainFightHero = fightHero;
            }

            if (battleUnit.UnitInfoId != 1080)
            {
                //fightHero.careerComponent = World.AddComponent<CareerComponent>(fightHero);
                fightHero.careerComponent.UpdateCareerType(GameCenter.mainHero.careerComponent.CurCarrerType);

                if (transformTb == null)
                {
                    //fightHero.weaponComponent = World.AddComponent<WeaponComponent>(fightHero);
                    fightHero.weaponComponent.UpdateWeapon(GameCenter.mainHero.weaponComponent.CurWeaponID, false);
                }
                else
                {
                    fightHero.weaponComponent.UpdateWeapon(transformTb.weapon_id, false);
                }
            }

            if (battleUnit.UnitInfoId != 1080)
            {
                //fightHero.skillComponent = World.AddComponent<HeroSkillComponent>(fightHero);

                fightHero.heroSkillComponent.IsSuperHero = false;
                if (battleUnit.UnitType == (uint) EFightActorType.Hero)
                {
                    fightHero.heroSkillComponent.IsPlayer = true;
                }

                fightHero.heroSkillComponent.DoInit();
                fightHero.heroSkillComponent.InitActiveSkill();
            }
            else
            {
                fightHero.heroSkillComponent.IsSuperHero = true;
                fightHero.heroSkillComponent.DoInit();
                //fightHero.skillComponent = World.AddComponent<SuperHeroSkillComponent>(fightHero);
            }

            if (battleUnit.UnitInfoId == 1080)
            {
                if (superHero != null)
                {
                    fightHero.LoadModel(templateObj, (actor) =>
                    {
                        //fightHero.clickComponent = World.AddComponent<ClickComponent>(fightHero);
                        fightHero.clickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                        fightHero.clickComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                        //fightHero.doubleClickComponent = World.AddComponent<DoubleClickComponent>(fightHero);
                        fightHero.doubleClickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                        fightHero.doubleClickComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                        //fightHero.longpressComponent = World.AddComponent<LongPressComponent>(fightHero);
                        fightHero.longpressComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                        fightHero.longpressComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                        //AnimationComponent animationComponent = World.AddComponent<AnimationComponent>(fightHero);
                        fightHero.animationComponent.SetSimpleAnimation(fightHero.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                        fightHero.animationComponent.UpdateHoldingAnimations(battleUnit.UnitInfoId, Constants.UMARMEDID);

                        MobManager.Instance.AddMob(battleUnit, fightHero.animationComponent, fightHero.gameObject, Constants.UMARMEDID, isCalling);
                    });
                }
            }
            else
            {
                Action<SceneActor> act = (actor) =>
                {
                    fightHero.SetLayer(fightHero.transform);

                    //fightHero.clickComponent = World.AddComponent<ClickComponent>(fightHero);
                    fightHero.clickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                    fightHero.clickComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                    //fightHero.doubleClickComponent = World.AddComponent<DoubleClickComponent>(fightHero);
                    fightHero.doubleClickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                    fightHero.doubleClickComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                    //fightHero.longpressComponent = World.AddComponent<LongPressComponent>(fightHero);
                    fightHero.longpressComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                    fightHero.longpressComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                    //AnimationComponent animationComponent = World.AddComponent<AnimationComponent>(fightHero);
                    fightHero.animationComponent.SetSimpleAnimation(fightHero.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                    fightHero.animationComponent.UpdateHoldingAnimations(transformTb == null ? battleUnit.UnitInfoId : transformTb.action_id, fightHero.weaponComponent.CurWeaponID);

                    MobEntity mobEntity = MobManager.Instance.AddMob(battleUnit, fightHero.animationComponent, fightHero.gameObject, 
                        transformTb == null ? fightHero.weaponComponent.CurWeaponID : (uint)battleUnit.WeaponId, isCalling);
                    if (mobEntity != null && fightHero.heroLoader != null &&
                        fightHero.heroLoader.datas != null && fightHero.heroLoader.datas.Count > 0)
                    {
                        MobCombatComponent mobCombatComponent = mobEntity.GetComponent<MobCombatComponent>();
                        if (mobCombatComponent != null)
                        {
                            if (fightHero.heroLoader.datas.TryGetValue(fightHero.heroLoader.showParts[(int) EHeroModelParts.Main], out List<dressData> dressDataList) &&
                                dressDataList != null && dressDataList.Count > 0)
                                mobCombatComponent.m_DressDataList = dressDataList;
                        }
                    }
                };

                if (transformTb == null)
                {
                    fightHero.LoadModel(act);
                }
                else
                {
                    fightHero.LoadModel(transformTb.model, act);
                }

                FightHeros.Add(battleUnit.RoleId, fightHero);
            }
        }

        public void CreateFightPets()
        {
            GameCenter.mainFightPet = null;

            foreach (var unit in battleUnits)
            {
                if (unit.UnitType == (uint) EFightActorType.Pet)
                {
                    CreateFightPet(unit);
                }
            }
        }

        /// <summary>
        /// 生成战斗宠物///
        /// </summary>
        /// <param name="unit"></param>
        public void CreateFightPet(BattleUnit unit, bool isCalling = false)
        {
            CSVPetNew.Data petData = CSVPetNew.Instance.GetConfData(unit.UnitInfoId);

            if (petData == null)
            {
                Debug.LogError($"petData is null id:{unit.UnitInfoId}");
                return;
            }

            //FightPet fightPet = GameCenter.fightWorld.CreateActor<FightPet>(unit.UnitId);

            FightPet fightPet = World.AllocActor<FightPet>((uint) unit.UnitId);
            fightPet.SetName($"FightPet_{unit.Pos.ToString()}");
            fightPet.SetParent(GameCenter.fightActorRoot);
            FightPetDic.Add(unit.UnitId, fightPet);

            fightPet.IsFullPet = unit.FullPet;
            if (unit.PetSuitAppearance != 0)
                fightPet.suitID = CSVPetEquipSuitAppearance.Instance.GetConfData(unit.PetSuitAppearance).show_id;
            else
                fightPet.suitID = 0;

            fightPet.Build = unit.Peteffect;
            fightPet.MagicSoul = unit.PetSoul;

            if (unit.RoleId == Sys_Role.Instance.Role.RoleId)
            {
                GameCenter.mainFightPet = fightPet;
            }

            fightPet.battleUnit = unit;
            fightPet.cSVPetData = petData;

            //fightPet.weaponComponent = World.AddComponent<WeaponComponent>(fightPet);
            fightPet.weaponComponent.UpdateWeapon(Constants.UMARMEDID);

            //fightPet.skillComponent = World.AddComponent<FightPetSkillComponent>(fightPet);
            fightPet.fightPetSkillComponent.DoInit();

            fightPet.LoadModel(petData.model, (System.Action<SceneActor>) ((actor) =>
            {
                //fightPet.clickComponent = World.AddComponent<ClickComponent>(fightPet);
                fightPet.clickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                fightPet.clickComponent.LayMask = ELayerMask.Partner;

                //fightPet.doubleClickComponent = World.AddComponent<DoubleClickComponent>(fightPet);
                fightPet.doubleClickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                fightPet.doubleClickComponent.LayMask = ELayerMask.Partner;

                //fightPet.longpressComponent = World.AddComponent<LongPressComponent>(fightPet);
                fightPet.longpressComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                fightPet.longpressComponent.LayMask = ELayerMask.Partner;

                //AnimationComponent animationComponent = World.AddComponent<AnimationComponent>(fightPet);
                fightPet.animationComponent.SetSimpleAnimation(fightPet.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                fightPet.animationComponent.UpdateHoldingAnimations(petData.action_id);

                MobManager.Instance.AddMob(unit, fightPet.animationComponent, fightPet.gameObject, fightPet.weaponComponent.CurWeaponID, isCalling);
            }));
        }

        static Dictionary<uint, List<BattleUnit>> monsterParts;

        void CreateMonsters()
        {
            cacheMonsterGos.Clear();
            ParseMonsters();

            foreach (BattleUnit unit in battleUnits)
            {
                if (unit.UnitType == (uint) EFightActorType.Monster)
                {
                    CSVMonster.Data monsterData = CSVMonster.Instance.GetConfData(unit.UnitInfoId);

                    if (monsterData == null)
                    {
                        Debug.LogError($"monsterData is null id:{unit.UnitInfoId}");
                        continue;
                    }

                    if (monsterData.body_part == 0)
                    {
                        CreateMonster(unit);
                    }
                }
            }
        }

        void ParseMonsters()
        {
            monsterParts = new Dictionary<uint, List<BattleUnit>>();

            foreach (BattleUnit unit in battleUnits)
            {
                CSVMonster.Data monsterData = CSVMonster.Instance.GetConfData(unit.UnitInfoId);

                if (monsterData != null)
                {
                    if (monsterData.body_part != 0)
                    {
                        if (monsterParts.ContainsKey(monsterData.body_part))
                        {
                            monsterParts[monsterData.body_part].Add(unit);
                        }
                        else
                        {
                            List<BattleUnit> battleUnits = new List<BattleUnit>();
                            battleUnits.Add(unit);
                            monsterParts[monsterData.body_part] = battleUnits;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 生成战斗怪物///
        /// </summary>
        /// <param name="unit"></param>
        public void CreateMonster(BattleUnit unit, bool isCalling = false)
        {
            CSVMonster.Data monsterData = CSVMonster.Instance.GetConfData(unit.UnitInfoId);
            if (monsterData == null)
            {
                DebugUtil.LogError($"CSVMonster表中不存在Id:{unit.UnitInfoId.ToString()}");
                return;
            }

            uint weaponID = 0;
            if (Net_Combat.Instance.m_CurServerBattleStage != 0)
            {
                if (monsterData.weapon_action_id == null)
                {
                    DebugUtil.LogError($"战斗阶段：{Net_Combat.Instance.m_CurServerBattleStage.ToString()}时，CSVMonster表Id:{unit.UnitInfoId.ToString()}中weapon_action_id为空");
                    return;
                }

                int weaponActionIdCount = monsterData.weapon_action_id.Count;
                if (Net_Combat.Instance.m_CurServerBattleStage > weaponActionIdCount)
                {
                    DebugUtil.LogError($"战斗阶段：{Net_Combat.Instance.m_CurServerBattleStage.ToString()}时，但CSVMonster表Id:{unit.UnitInfoId.ToString()}中weapon_action_id数组长度为：{weaponActionIdCount.ToString()}");
                    return;
                }

                weaponID = monsterData.weapon_action_id[(int) Net_Combat.Instance.m_CurServerBattleStage - 1];
            }
            else
            {
                weaponID = monsterData.weapon_id;
            }

            //Monster monster = GameCenter.fightWorld.CreateActor<Monster>(unit.UnitId);

            Monster monster = World.AllocActor<Monster>((uint) unit.UnitId);
            monster.SetName($"Monster_{unit.Pos.ToString()}");
            monster.SetParent(GameCenter.fightActorRoot);
            MonsterDic.Add(unit.UnitId, monster);

            monster.battleUnit = unit;
            monster.cSVMonsterData = monsterData;
            if (unit.PetSuitAppearance != 0)
                monster.suitID = CSVPetEquipSuitAppearance.Instance.GetConfData(unit.PetSuitAppearance).show_id;
            else
                monster.suitID = 0;

            //monster.weaponComponent = World.AddComponent<WeaponComponent>(monster);            
            monster.weaponComponent.UpdateWeapon(weaponID, false);

            //monster.skillComponent = World.AddComponent<MonsterSkillComponent>(monster);
            monster.monsterSkillComponent.DoInit();

            monster.LoadModel(monsterData.model, (actor) =>
            {
                //monster.clickComponent = World.AddComponent<ClickComponent>(monster);
                monster.clickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                monster.clickComponent.LayMask = ELayerMask.Monster;

                //monster.doubleClickComponent = World.AddComponent<DoubleClickComponent>(monster);
                monster.doubleClickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                monster.doubleClickComponent.LayMask = ELayerMask.Monster;

                //monster.longpressComponent = World.AddComponent<LongPressComponent>(monster);
                monster.longpressComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                monster.longpressComponent.LayMask = ELayerMask.Monster;

                //AnimationComponent animationComponent = World.AddComponent<AnimationComponent>(monster);
                monster.animationComponent.SetSimpleAnimation(monster.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                monster.animationComponent.UpdateHoldingAnimations(monster.cSVMonsterData.monster_id, weaponID);

                MobManager.Instance.AddMob(unit, monster.animationComponent, monster.gameObject, weaponID, isCalling);

                if (monsterParts.ContainsKey(unit.UnitInfoId))
                {
                    CreateMonsterParts(monster, monster.animationComponent, monsterParts[unit.UnitInfoId]);
                }
            });
        }

        public void CreateMonsterParts(Monster parent, AnimationComponent parentAnimationComponent, List<BattleUnit> battleUnits)
        {
            foreach (BattleUnit unit in battleUnits)
            {
                CreateMonsterPart(parent, parentAnimationComponent, unit);
            }
        }

        /// <summary>
        /// 生成战斗怪物局部///
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="isCalling"></param>
        void CreateMonsterPart(Monster parent, AnimationComponent parentAnimationComponent, BattleUnit unit, bool isCalling = false)
        {
            CSVMonster.Data monsterData = CSVMonster.Instance.GetConfData(unit.UnitInfoId);
            if (monsterData == null)
            {
                DebugUtil.LogError($"CSVMonster表中不存在Id:{unit.UnitInfoId.ToString()}");
                return;
            }

            uint weaponID = 0;
            if (Net_Combat.Instance.m_CurServerBattleStage != 0)
            {
                if (monsterData.weapon_action_id == null)
                {
                    DebugUtil.LogError($"战斗阶段：{Net_Combat.Instance.m_CurServerBattleStage.ToString()}时，CSVMonster表Id:{unit.UnitInfoId.ToString()}中weapon_action_id为空");
                    return;
                }

                int weaponActionIdCount = monsterData.weapon_action_id.Count;
                if (Net_Combat.Instance.m_CurServerBattleStage > weaponActionIdCount)
                {
                    DebugUtil.LogError($"战斗阶段：{Net_Combat.Instance.m_CurServerBattleStage.ToString()}时，但CSVMonster表Id:{unit.UnitInfoId.ToString()}中weapon_action_id数组长度为：{weaponActionIdCount.ToString()}");
                    return;
                }

                weaponID = monsterData.weapon_action_id[(int) Net_Combat.Instance.m_CurServerBattleStage - 1];
            }
            else
            {
                weaponID = monsterData.weapon_id;
            }

            //MonsterPart monsterPart = GameCenter.fightWorld.CreateActor<MonsterPart>(unit.UnitId);
            MonsterPart monsterPart = World.AllocActor<MonsterPart>(unit.UnitId);

            monsterPart.battleUnit = unit;
            monsterPart.cSVMonsterData = monsterData;

            monsterPart.gameObject = parent.gameObject.FindChildByName(monsterData.model);
            monsterPart.sceneActorWrap = monsterPart.gameObject.GetComponent<SceneActorWrap>();
            if (monsterPart.sceneActorWrap == null)
            {
                monsterPart.sceneActorWrap = monsterPart.gameObject.AddComponent<SceneActorWrap>();
            }

            monsterPart.sceneActorWrap.sceneActor = monsterPart;

            //monsterPart.clickComponent = World.AddComponent<ClickComponent>(monsterPart);
            monsterPart.clickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
            monsterPart.clickComponent.LayMask = ELayerMask.Monster;

            //monsterPart.doubleClickComponent = World.AddComponent<DoubleClickComponent>(monsterPart);
            monsterPart.doubleClickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
            monsterPart.doubleClickComponent.LayMask = ELayerMask.Monster;

            //monsterPart.longpressComponent = World.AddComponent<LongPressComponent>(monsterPart);
            monsterPart.longpressComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
            monsterPart.longpressComponent.LayMask = ELayerMask.Monster;

            MobManager.Instance.AddMob(unit, parentAnimationComponent, monsterPart.gameObject, weaponID, isCalling);
        }

        public void CreateControl()
        {
            if (!CombatManager.Instance.m_IsFight)
            {
                if (GameCenter.fightControl != null)
                {
                    //GameCenter.fightWorld.DestroyActor(GameCenter.fightControl);
                    //GameCenter.fightControl = null;
                    World.CollecActor(ref GameCenter.fightControl);
                }

                //GameCenter.fightControl = GameCenter.fightWorld.CreateActor<FightControl>(9999);
                GameCenter.fightControl = World.AllocActor<FightControl>(9999);
            }
        }

        public void ExitFight(bool force = false)
        {
            GameMain.Procedure.TriggerEvent(this, (int) EProcedureEvent.EnterNormal);

            CombatManager.Instance.OnDisable();
            GameCenter.mainFightHero = null;
            GameCenter.mainFightPet = null;
            GameCenter.fightControl = null;
            //GameCenter.fightWorld?.Clear();
            ClearActor();

            _timeRound?.Cancel();
            if (_timeDelayResume != null && !_timeDelayResume.isDone) //异常情况强制关闭menu的 lockimage 
                eventEmitter.Trigger(EEvents.Reconnected);
            _timeDelayResume?.Cancel();

            //ResetHeros();

            if (!force)
            {
                if (Sys_Instance.Instance.IsInInstance)
                {
                    //UIManager.DestroyUI(EUIID.UI_MainBattle, true);
                    ExitEffectFunc();
                }
                else
                {
                    //UIManager.DestroyUI(EUIID.UI_MainBattle, false);
                    CSVBattleType.Data cSVBattleTypeData = CSVBattleType.Instance.GetConfData(BattleTypeId);
                    if (cSVBattleTypeData == null)
                    {
                        ExitEffectFunc();
                    }
                    else
                    {
                        if (cSVBattleTypeData.exit_battle_effect == 0 || !UIManager.IsVisible(EUIID.UI_MainInterface))
                        {
                            ExitEffectFunc();
                        }
                        else
                        {
                            uint exitEffectType = OptionManager.Instance.GetBoolean((int) OptionManager.EOptionID.UsePcStyleEnterFight) ? cSVBattleTypeData.exit_battle_effectON : cSVBattleTypeData.exit_battle_effectOFF;
                            GameCenter.mCameraController.FadeInOut(exitEffectType, ExitEffectFunc);
                        }
                    }
                }

                DebugUtil.LogFormat(ELogType.eCombat, "UI_MainBattle is Destroy！！！");
                GameCenter.mainHero?.animationComponent?.UpdateAni(0);
                GameCenter.mCameraController.EnterWorld();
            }

            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnActiveAllActorHUD);
            DebugUtil.LogFormat(ELogType.eHUD, "Sys_Fight.OnActiveAllActorHUD");

            //打开遮挡透明效果
            RenderExtensionSetting.bUsageOcclusionTransparent = true;

            //play bgm
            CSVMapInfo.Data data = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
            if (data != null)
            {
                AudioUtil.PlayAudio(data.sound_bgm);
            }

            Sys_Fashion.Instance.OnExitFight();
            FightHeros.Clear();

            //主动发送是否添加好友亲密度
            Sys_Society.Instance.AddIntimacy();
        }

        // 战斗结束的时候需要执行的一些操作
        public readonly List<Action> ExitFightActions = new List<Action>();

        private void ExitEffectFunc()
        {
            try
            {
                for (int i = 0, length = ExitFightActions.Count; i < length; ++i)
                {
                    ExitFightActions[i]?.Invoke();
                }
            }
            finally
            {
                ExitFightActions.Clear();
            }

            OnExitFight?.Invoke();

            //UIManager.OpenUI(EUIID.UI_MainInterface, true);
            //enable npc Click
            GameCenter.EnableNpcClick(true);

            //fightState = FightState.None;

            //发送通知
            ProcedureManager.eventEmitter.Trigger(ProcedureManager.EEvents.OnAfterExitFightEffect);

            Sys_Pet.Instance.OnCheckGetNewPet();
        }

        public bool HasPet()
        {
            if (GameCenter.mainFightPet == null)
            {
                return false;
            }

            return true;
        }

        //战斗中添加新的战斗单元
        public void CallingNewBattleUnit(BattleUnit battleUnit)
        {
            if (battleUnit == null)
                return;

            for (int i = 0, count = battleUnits.Count; i < count; i++)
            {
                var checkBu = battleUnits[i];
                if (checkBu == null)
                    continue;

                if (battleUnit.UnitId == checkBu.UnitId)
                {
                    if (battleUnit.ShapeShiftId > 0u)
                    {
                        DLogManager.Log(ELogType.eCombat, $"<color=red>添加 变身 新单位ShapeShiftId:{battleUnit.ShapeShiftId.ToString()}  IsUseShapeShift:{battleUnit.IsUseShapeShift.ToString()}时已经存在该单位的UnitId：{battleUnit.UnitId.ToString()}，强制用变身新单位BattleUnit数据覆盖</color>");
                        battleUnits.RemoveAt(i);
                        break;
                    }
                    else
                    {
#if DEBUG_MODE
                        if (Net_Combat.Instance.m_IsReconnect)
                            DLogManager.Log(ELogType.eCombat, $"<color=red>添加新单位时已经存在该单位的UnitId：{battleUnit.UnitId.ToString()}</color>");
                        else
                            DebugUtil.LogError($"添加新单位时已经存在该单位的UnitId：{battleUnit.UnitId.ToString()}");
#endif

                        return;
                    }
                }
            }

            DLogManager.Log(ELogType.eCombat, $"<color=yellow>战斗中添加新的战斗单元UnitId : {battleUnit.UnitId.ToString()}   Pos : {battleUnit.Pos.ToString()}   ShapeShiftId:{battleUnit.ShapeShiftId.ToString()}  IsUseShapeShift:{battleUnit.IsUseShapeShift.ToString()}</color>");

            //清理UnitId相同已存在的Mob的状态
            MobManager.Instance.ClearMobByCheckUnitState(battleUnit, CheckMobUnitServerStateEnum.UnitId);

            battleUnits.Add(battleUnit);

            if (battleUnit.UnitType == (uint) EFightActorType.Hero && battleUnit.RoleId == Sys_Role.Instance.Role.RoleId)
            {
                CreateMainFightHero(battleUnit, true);
            }
            else if ((battleUnit.UnitType == (uint) EFightActorType.Hero && battleUnit.RoleId != Sys_Role.Instance.Role.RoleId) || battleUnit.UnitType == (uint) EFightActorType.Partner)
            {
                CreateFightHero(battleUnit, true);
            }
            else if (battleUnit.UnitType == (uint) EFightActorType.Pet)
            {
                CreateFightPet(battleUnit, true);
                if (battleUnit.RoleId == Sys_Role.Instance.Role.RoleId && GameCenter.fightControl != null)
                {
                    GameCenter.fightControl.currentUsePetTime--;
                    if (!GameCenter.fightControl.forbidpetsList.Contains((uint)battleUnit.PetId))
                    {
                        GameCenter.fightControl.forbidpetsList.Add((uint)battleUnit.PetId);
                    }
                    Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnUpdatePet, true, battleUnit.PetId);
                    ClientPet clientPet = Sys_Pet.Instance.GetPetByUId((uint)battleUnit.PetId);
                    if (clientPet != null)
                    {
                        Sys_Plan.Instance.eventEmitter.Trigger<ulong, uint, uint>(Sys_Plan.EEvents.OnChangeFightPet, clientPet.petUnit.Uid, clientPet.petUnit.PetPointPlanData.CurrentPlanIndex, clientPet.petUnit.EnhancePlansData.CurrentPlanIndex);
                    }
                }
            }
            else if (battleUnit.UnitType == (uint) EFightActorType.Monster)
            {
                CreateMonster(battleUnit, true);
            }
        }

        public int GetServerCombatUnitCount()
        {
            return battleUnits.Count;
        }

        public uint GetBossUnitInfoId()
        {
            foreach (var mob in battleUnits)
            {
                if (mob.UnitType == (uint) UnitType.Monster && CSVMonster.Instance.GetConfData(mob.UnitInfoId).body_part != 0)
                {
                    return CSVMonster.Instance.GetConfData(mob.UnitInfoId).body_part;
                }
            }

            return 0;
        }

        public void OnReconnect()
        {
            if (IsFight())
                CombatManager.Instance.OnDisable();
        }

        //判断是否在战斗中
        public bool IsFight()
        {
            if (GameMain.Procedure != null && GameMain.Procedure.CurrentProcedure != null && GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void RemoveFightHeroByUnitId(uint unitId)
        {
            if (FightHeroDic.TryGetValue(unitId, out FightHero fightHero))
            {
                FightHeros.Remove(fightHero.battleUnit.RoleId);

                World.CollecActor(ref fightHero);
                FightHeroDic.Remove(unitId);
            }
        }

        private void RemoveFightPetByUnitId(uint unitId)
        {
            if (FightPetDic.TryGetValue(unitId, out FightPet fightPet))
            {
                World.CollecActor(ref fightPet);
                FightPetDic.Remove(unitId);
            }
        }

        private void RemoveMonsterByUnitId(uint unitId)
        {
            if (MonsterDic.TryGetValue(unitId, out Monster monster))
            {
                World.CollecActor(ref monster);
                MonsterDic.Remove(unitId);
            }
        }

        private void ClearActor()
        {
            foreach (FightHero v in FightHeroDic.Values)
            {
                FightHero fightHero = v;
                World.CollecActor(ref fightHero);
            }

            FightHeroDic.Clear();
            FightHeros.Clear();

            foreach (Monster v in MonsterDic.Values)
            {
                Monster monster = v;
                World.CollecActor(ref monster);
            }

            MonsterDic.Clear();

            foreach (FightPet v in FightPetDic.Values)
            {
                FightPet fightPet = v;
                World.CollecActor(ref fightPet);
            }

            FightPetDic.Clear();
        }
    }
}