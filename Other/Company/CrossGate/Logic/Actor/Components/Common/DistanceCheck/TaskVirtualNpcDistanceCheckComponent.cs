using Logic.Core;
using System.Collections.Generic;
using UnityEngine;
using Table;
using Lib.Core;
using Net;
using Packet;
using Framework;
using UnityEngine.AI;

namespace Logic
{
    public class Sys_Escort : SystemModuleBase<Sys_Escort>
    {
        public bool EscortFlag;
        uint curHandlerID;
        uint curHandlerIndex;

        Dictionary<uint, bool> npcEscortFlags = new Dictionary<uint, bool>();

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnSyncConvoyStart,
            OnSyncConvoyEnd,
        }

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.SyncConvoyStart, OnSyncConvoyStart, CmdTeamSyncConvoyStart.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.SyncConvoyEnd, OnSyncConvoyEnd, CmdTeamSyncConvoyEnd.Parser);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnEnterMap, OnEnterMap, true);
        }

        void OnEnterMap()
        {
            if (EscortFlag)
            {
                if (Sys_Team.Instance.HaveTeam)
                {
                    if (Sys_Team.Instance.isCaptain())
                    {
                        CmdTeamSyncConvoyEnd cmdTeamSyncConvoyEnd = new CmdTeamSyncConvoyEnd();
                        cmdTeamSyncConvoyEnd.TaskId = curHandlerID;
                        cmdTeamSyncConvoyEnd.Index = curHandlerIndex;

                        NetClient.Instance.SendMessage((ushort)CmdTeam.SyncConvoyEnd, cmdTeamSyncConvoyEnd);
                    }
                    else
                    {
                        ClearEscortVirtualNpcs();
                    }
                }
                else
                {
                    ClearEscortVirtualNpcs();
                }
            }
            //VirtualShowManager.Instance.ClearVirtualSceneActors();
        }

        public override void OnLogin()
        {
            base.OnLogin();

            ClearEscortVirtualNpcs();
            npcEscortFlags.Clear();
            EscortFlag = false;
            curHandlerID = curHandlerIndex = 0u;
        }

        public override void OnLogout()
        {
            base.OnLogout();

            ClearEscortVirtualNpcs();
            npcEscortFlags.Clear();
            EscortFlag = false;
            curHandlerID = curHandlerIndex = 0u;
        }

        public bool IsNpcEscorting(uint npcInfoID)
        {
            if (npcEscortFlags.ContainsKey(npcInfoID) && npcEscortFlags[npcInfoID])
                return true;
            return false;
        }

        void OnSyncConvoyStart(NetMsg msg)
        {           
            CmdTeamSyncConvoyStart cmdTeamSyncConvoyStart = NetMsgUtil.Deserialize<CmdTeamSyncConvoyStart>(CmdTeamSyncConvoyStart.Parser, msg);
            if (cmdTeamSyncConvoyStart != null)
            {
                StartEscort(cmdTeamSyncConvoyStart.TaskId, cmdTeamSyncConvoyStart.Index);
            }
        }

        public void StartEscort(uint taskID, uint taskGoalIndex)
        {
            EscortFlag = true;
            curHandlerID = taskID;
            curHandlerIndex = taskGoalIndex;

            uint taskGoalID = taskID * 10 + taskGoalIndex + 1;
            CSVTaskGoal.Data cSVTaskGoalData = CSVTaskGoal.Instance.GetConfData(taskGoalID);
            if (cSVTaskGoalData != null)
            {
                if (cSVTaskGoalData.EscortNpc != null)
                {
                    for (int index = 0, len = cSVTaskGoalData.EscortNpc.Count; index < len; index++)
                    {
                        npcEscortFlags[cSVTaskGoalData.EscortNpc[index]] = true;
                        eventEmitter.Trigger<uint>(EEvents.OnSyncConvoyStart, cSVTaskGoalData.EscortNpc[index]);
                    }
                }

                CreateEscortVirtualNpcs(taskID, taskGoalID, (int)taskGoalIndex);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTaskTextContent(CSVTaskLanguage.Instance.GetConfData(cSVTaskGoalData.EscortStartTips)));
            }
            else
            {
                DebugUtil.LogError($"CSVTaskGoal.Data IS NULL ID:{taskGoalID}");
            }

            EffectUtil.Instance.LoadEffect(GameCenter.mainHero.UID, CSVEffect.Instance.GetConfData(3000104135).effects_path, GameCenter.mainHero.fxRoot.transform, EffectUtil.EEffectTag.Escort);
        }

        void OnSyncConvoyEnd(NetMsg msg)
        {         
            CmdTeamSyncConvoyEnd cmdTeamSyncConvoyEnd = NetMsgUtil.Deserialize<CmdTeamSyncConvoyEnd>(CmdTeamSyncConvoyEnd.Parser, msg);
            if (cmdTeamSyncConvoyEnd != null)
            {
                EscortFlag = false;
                uint taskGoalID = cmdTeamSyncConvoyEnd.TaskId * 10 + cmdTeamSyncConvoyEnd.Index + 1;
                CSVTaskGoal.Data cSVTaskGoalData = CSVTaskGoal.Instance.GetConfData(taskGoalID);
                if (cSVTaskGoalData != null)
                {                   
                    if (cSVTaskGoalData.EscortNpc != null)
                    {
                        for (int index = 0, len = cSVTaskGoalData.EscortNpc.Count; index < len; index++)
                        {
                            npcEscortFlags[cSVTaskGoalData.EscortNpc[index]] = false;
                            eventEmitter.Trigger<uint>(EEvents.OnSyncConvoyEnd, cSVTaskGoalData.EscortNpc[index]);
                        }
                    }
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTaskTextContent(CSVTaskLanguage.Instance.GetConfData(cSVTaskGoalData.EscortCompleteTips)));
                }
                else
                {
                    DebugUtil.LogError($"CSVTaskGoal.Data IS NULL ID:{taskGoalID}");
                }
                ClearEscortVirtualNpcs();
            }
        }

        public Dictionary<ulong, VirtualNpc> escortVirtualNpcs = new Dictionary<ulong, VirtualNpc>();

        public void ClearEscortVirtualNpcs()
        {
            EscortFlag = false;
            foreach (var actor in escortVirtualNpcs.Values)
            {
                npcEscortFlags[actor.cSVNpcData.id] = false;
                eventEmitter.Trigger<uint>(EEvents.OnSyncConvoyEnd, actor.cSVNpcData.id);
                //GameCenter.mainWorld.DestroyActor(actor);
                World.CollecActor(actor);
            }
            escortVirtualNpcs.Clear();

            if (GameCenter.mainHero != null)
            {
                EffectUtil.Instance.UnloadEffectByTag(GameCenter.mainHero.UID, EffectUtil.EEffectTag.Escort);
            }
        }

        public void CreateEscortVirtualNpcs(uint taskID, uint taskGoalID, int taskGoalIndex)
        {
            CSVTaskGoal.Data cSVTaskGoalData = CSVTaskGoal.Instance.GetConfData(taskGoalID);
            if (cSVTaskGoalData.EscortNpc != null)
            {
                for (int index = 0, len = cSVTaskGoalData.EscortNpc.Count; index < len; index++)
                {
                    //VirtualNpc taskVirtualNpc = GameCenter.mainWorld.CreateActor<VirtualNpc>((ulong)cSVTaskGoalData.EscortNpc[index] * 100000);
                    ulong uid = (ulong)cSVTaskGoalData.EscortNpc[index] * 100000;
                    VirtualNpc taskVirtualNpc = World.AllocActor<VirtualNpc>(uid);
                    taskVirtualNpc.SetParent(GameCenter.npcRoot.transform);
                    taskVirtualNpc.SetName($"TaskVirtualNPC_{uid.ToString()}");

                    escortVirtualNpcs.Add(taskVirtualNpc.uID, taskVirtualNpc);
                    taskVirtualNpc.cSVNpcData = CSVNpc.Instance.GetConfData(cSVTaskGoalData.EscortNpc[index]);

                    //taskVirtualNpc.stateComponent = World.AddComponent<StateComponent>(taskVirtualNpc);

                    //taskVirtualNpc.transform.position = PosConvertUtil.Svr2Client(cSVTaskGoalData.EscortNpcLocation[index][0], cSVTaskGoalData.EscortNpcLocation[index][1]);
                    taskVirtualNpc.transform.localEulerAngles = new Vector3(0, cSVTaskGoalData.EscortNpcOrientations, 0);
                    //taskVirtualNpc.movementComponent = World.AddComponent<MovementComponent>(taskVirtualNpc);
                    //taskVirtualNpc.movementComponent.TransformToPosImmediately(PosConvertUtil.Svr2Client(cSVTaskGoalData.EscortNpcLocation[index][0], cSVTaskGoalData.EscortNpcLocation[index][1]));
                    NavMeshHit navMeshHit;
                    Vector3 hitPos = PosConvertUtil.Svr2Client(cSVTaskGoalData.EscortNpcLocation[index][0], cSVTaskGoalData.EscortNpcLocation[index][1]);
                    MovementComponent.GetNavMeshHit(hitPos, out navMeshHit);
                    if (navMeshHit.hit)
                        taskVirtualNpc.transform.position = navMeshHit.position;
                    else
                        taskVirtualNpc.transform.position = hitPos;

                    taskVirtualNpc.movementComponent.InitNavMeshAgent();
                    taskVirtualNpc.movementComponent.fMoveSpeed = cSVTaskGoalData.MoveSpeed / 10000f;

                    if (index == 0)
                    {
                        taskVirtualNpc.eVirtualNpcType = VirtualNpc.EVirtualNpcType.Escort;
                        taskVirtualNpc.LoadModel(taskVirtualNpc.cSVNpcData.model, (actor) =>
                        {
                            taskVirtualNpc.AnimationComponent.SetSimpleAnimation(taskVirtualNpc.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                            //taskVirtualNpc.AnimationComponent = World.AddComponent<AnimationComponent>(actor);
                            taskVirtualNpc.modelGameObject.SetActive(false);
                            taskVirtualNpc.AnimationComponent.UpdateHoldingAnimations(taskVirtualNpc.cSVNpcData.action_id, Constants.UMARMEDID, null, EStateType.Idle, taskVirtualNpc.modelGameObject);

                            taskVirtualNpc.escortVirtualNpcTaskGoalDataComponent = new EscortVirtualNpcTaskGoalDataComponent();
                            taskVirtualNpc.escortVirtualNpcTaskGoalDataComponent.taskID = taskID;
                            taskVirtualNpc.escortVirtualNpcTaskGoalDataComponent.taskGoalID = taskGoalID;
                            taskVirtualNpc.escortVirtualNpcTaskGoalDataComponent.taskGoalIndex = taskGoalIndex;

                            taskVirtualNpc.escortVirtualNpcDistanceCheckComponent = new EscortVirtualNpcDistanceCheckComponent();
                            taskVirtualNpc.escortVirtualNpcDistanceCheckComponent.TaskVirtualNpc = taskVirtualNpc;
                            taskVirtualNpc.escortVirtualNpcDistanceCheckComponent.distanceLimit = cSVTaskGoalData.EscortNpcTriggerDis;
                            List<TargetPosWrap> pos = new List<TargetPosWrap>();
                            for (int index2 = 0, len2 = cSVTaskGoalData.EscortLocation.Count; index2 < len2; index2++)
                            {
                                TargetPosWrap targetPosWrap = new TargetPosWrap();
                                targetPosWrap.pos = PosConvertUtil.Svr2Client(cSVTaskGoalData.EscortLocation[index2][0], cSVTaskGoalData.EscortLocation[index2][1]);
                                if (cSVTaskGoalData.EscortLocation[index2].Count >= 3)
                                {
                                    targetPosWrap.bubbleID = cSVTaskGoalData.EscortLocation[index2][2];
                                }
                                pos.Add(targetPosWrap);
                            }
                            Timer.Register(0.5f, () =>
                            {
                                taskVirtualNpc.escortVirtualNpcDistanceCheckComponent.InitTargetPosQueue(pos);
                            });                          
                        });
                    }
                    else
                    {
                        taskVirtualNpc.eVirtualNpcType = VirtualNpc.EVirtualNpcType.Common;
                        taskVirtualNpc.LoadModel(taskVirtualNpc.cSVNpcData.model, (actor) =>
                        {
                            taskVirtualNpc.AnimationComponent.SetSimpleAnimation(taskVirtualNpc.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                            //taskVirtualNpc.AnimationComponent = World.AddComponent<AnimationComponent>(actor);
                            taskVirtualNpc.modelGameObject.SetActive(false);
                            taskVirtualNpc.AnimationComponent.UpdateHoldingAnimations(taskVirtualNpc.cSVNpcData.action_id, Constants.UMARMEDID, null, EStateType.Idle, taskVirtualNpc.modelGameObject);                                                 
                        });

                        FollowComponent followComponent = taskVirtualNpc.followComponent = PoolManager.Fetch<FollowComponent>();
                        followComponent.actor = taskVirtualNpc;
                        followComponent.Construct();

                        followComponent.Target = escortVirtualNpcs[(ulong)cSVTaskGoalData.EscortNpc[0] * 100000];
                        followComponent.Follow = true;
                    }
                }
            }
        }

        public override void Dispose()
        {
            ClearEscortVirtualNpcs();

            base.Dispose();
        }
    }

    //TODO ct 可以都放到GameCenter
    public class VirtualShowManager : Singleton<VirtualShowManager>
    {
        public Dictionary<ulong, VirtualSceneActor> virtualSceneActors = new Dictionary<ulong, VirtualSceneActor>();        

        public void ClearVirtualSceneActors()
        {
            foreach (var actor in virtualSceneActors.Values)
            {
                World.CollecActor(actor);
            }
            virtualSceneActors.Clear();
        }
        public void Remove(ulong uid)
        {
            if (virtualSceneActors.TryGetValue(uid, out VirtualSceneActor actor))
            {
                World.CollecActor(actor);
                virtualSceneActors.Remove(uid);                
            }
        }
        public bool ContainsKey(ulong uid)
        {
            return virtualSceneActors.ContainsKey(uid);
        }
        public bool TryGetValue(ulong uid, out VirtualSceneActor actor)
        {
            return virtualSceneActors.TryGetValue(uid, out actor);
        }
        public bool TryCreateVirtualNpc(ulong uid, CSVNpc.Data npcData, out VirtualNpc virtualNpc)
        {
            if (!virtualSceneActors.ContainsKey(uid))
            {
                virtualNpc = World.AllocActor<VirtualNpc>(uid);

                virtualSceneActors.Add(uid, virtualNpc);                

                virtualNpc.SetParent(GameCenter.npcRoot.transform);
                virtualNpc.SetName($"TaskVirtualNPC_{uid.ToString()}");

                virtualNpc.cSVNpcData = npcData;
                return true;
            }
            else
            {
                virtualNpc = null;
                return false;
            }
        }
        public bool TryCreateVirtualParnter(ulong uid, CSVPartner.Data partnerData, out VirtualParnter virtualParnter)
        {
            if (!virtualSceneActors.ContainsKey(uid))
            {
                virtualParnter = World.AllocActor<VirtualParnter>(uid);

                virtualSceneActors.Add(uid, virtualParnter);                

                virtualParnter.SetParent(GameCenter.npcRoot.transform);
                virtualParnter.SetName($"TaskVirtualParnter_{uid.ToString()}");

                virtualParnter.cSVPartnerData = partnerData;
                return true;
            }
            else
            {
                virtualParnter = null;
                return false;
            }
        }
        public bool TryCreateVirtualMonster(ulong uid, CSVMonster.Data monsterData, out VirtualMonster virtualMonster)
        {
            if (!virtualSceneActors.ContainsKey(uid))
            {
                virtualMonster = World.AllocActor<VirtualMonster>(uid);
                virtualSceneActors.Add(uid, virtualMonster);                

                virtualMonster.SetParent(GameCenter.npcRoot.transform);
                virtualMonster.SetName($"TaskVirtualMonster_{uid.ToString()}");

                virtualMonster.cSVMonsterData = monsterData;
                return true;
            }
            else
            {
                virtualMonster = null;
                return false;
            }
        }
    }

    public class EscortVirtualNpcTaskGoalDataComponent
    {
        public uint taskID
        {
            get;
            set;
        }

        public uint taskGoalID
        {
            get;
            set;
        }

        public int taskGoalIndex
        {
            get;
            set;
        }
    }

    public class TargetPosWrap
    {
        public Vector3 pos;
        public uint bubbleID;
    }

    public class VirtualNpcSystem : LevelSystemBase
    {
        private float lastTriggerTime = 0;
        private float cd = 0.4f;

        /// <summary>
        /// 对应护送/跟随/跟踪任务目标是否激活///
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="taskGoalID"></param>
        /// <returns></returns>
        public static bool IsVirtualNpcActive(uint taskID, uint taskGoalID)
        {
            if (Sys_Escort.Instance.escortVirtualNpcs != null && Sys_Escort.Instance.escortVirtualNpcs.Count > 0)
            {
                foreach (var virtualNpc in Sys_Escort.Instance.escortVirtualNpcs.Values)
                {
                    if (virtualNpc.escortVirtualNpcTaskGoalDataComponent != null && virtualNpc.escortVirtualNpcTaskGoalDataComponent.taskID == taskID && virtualNpc.escortVirtualNpcTaskGoalDataComponent.taskGoalID == taskGoalID)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1600000015));
                        return true;
                    }
                }
            }

            if (Sys_NpcFollow.Instance.npcFollowVirtualNpcs != null && Sys_NpcFollow.Instance.npcFollowVirtualNpcs.Count > 0)
            {
                foreach (var virtualNpc in Sys_NpcFollow.Instance.npcFollowVirtualNpcs.Values)
                {
                    if (virtualNpc.npcFollowVirtualNpcTaskGoalDataComponent != null && virtualNpc.npcFollowVirtualNpcTaskGoalDataComponent.taskID == taskID && virtualNpc.npcFollowVirtualNpcTaskGoalDataComponent.taskGoalID == taskGoalID)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1600000016));
                        return true;
                    }
                }
            }

            if (Sys_Track.Instance.trackVirtualNpcs != null && Sys_Track.Instance.trackVirtualNpcs.Count > 0)
            {
                foreach (var virtualNpc in Sys_Track.Instance.trackVirtualNpcs.Values)
                {
                    if (virtualNpc.trackVirtualNpcTaskGoalDataComponent != null && virtualNpc.trackVirtualNpcTaskGoalDataComponent.taskID == taskID && virtualNpc.trackVirtualNpcTaskGoalDataComponent.taskGoalID == taskGoalID)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1600000017));
                        return true;
                    }
                }
            }

            return false;
        }


        public override void OnUpdate()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
                return;

            if (Time.unscaledTime < lastTriggerTime)
                return;

            lastTriggerTime = Time.unscaledTime + cd;

            foreach (var vNpc in Sys_Escort.Instance.escortVirtualNpcs.Values)
            {
                if (vNpc.eVirtualNpcType == VirtualNpc.EVirtualNpcType.Escort)
                {
                    EscortCheck(vNpc);
                }                
            }

            foreach (var vNpc in Sys_Track.Instance.trackVirtualNpcs.Values)
            {
                if (vNpc.eVirtualNpcType == VirtualNpc.EVirtualNpcType.Track)
                {
                    TrackOutCheck(vNpc);
                    TrackInCheck(vNpc);
                    TrackLogic(vNpc);
                }
            }

            foreach (var vNpc in Sys_NpcFollow.Instance.npcFollowVirtualNpcs.Values)
            {
                if (vNpc.eVirtualNpcType == VirtualNpc.EVirtualNpcType.NpcFollow)
                {
                    NpcFollowLogic(vNpc);
                }
            }
        }

        void TrackOutCheck(VirtualNpc TaskVirtualNpc)
        {
            if (TrackOutCheckResult(TaskVirtualNpc))
            {
                TrackOutTrigger(TaskVirtualNpc);
            }
            else
            {
                TrackOutTriggerExit(TaskVirtualNpc);
            }
        }

        bool TrackOutCheckResult(VirtualNpc TaskVirtualNpc)
        {
            float distance = (GameCenter.mainHero.transform.position - TaskVirtualNpc.transform.position).sqrMagnitude;
            if (TaskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent != null && distance > (TaskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.distanceLimitMax * TaskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.distanceLimitMax))
                return false;

            return true;
        }

        void TrackOutTrigger(VirtualNpc TaskVirtualNpc)
        {
            TaskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.OutFlag = true;
            TaskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.overTimeTimer?.Cancel();
            TaskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.overTimeTimer = null;
        }

        void TrackOutTriggerExit(VirtualNpc TaskVirtualNpc)
        {
            if (TaskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.OutFlag)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1600000018, TaskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.overTime.ToString()));
                TaskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.overTimeTimer?.Cancel();
                TaskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.overTimeTimer = Timer.Register(TaskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.overTime, () =>
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTaskTextContent(CSVTaskLanguage.Instance.GetConfData(CSVTaskGoal.Instance.GetConfData(TaskVirtualNpc.trackVirtualNpcTaskGoalDataComponent.taskGoalID).TracingFailTips)));
                    Sys_Track.Instance.ClearTrackVirtualNpcs();
                });
            }
            TaskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.OutFlag = false;
        }

        void TrackInCheck(VirtualNpc TaskVirtualNpc)
        {
            if (TrackInCheckResult(TaskVirtualNpc))
            {
                TrackInTrigger(TaskVirtualNpc);
            }
            else
            {
                TrackInTriggerExit(TaskVirtualNpc);
            }
        }

        bool TrackInCheckResult(VirtualNpc TaskVirtualNpc)
        {
            float distance = (GameCenter.mainHero.transform.position - TaskVirtualNpc.transform.position).sqrMagnitude;
            if (distance > (TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.distanceLimitMin * TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.distanceLimitMin))
                return false;

            return true;
        }

        void TrackInTrigger(VirtualNpc TaskVirtualNpc)
        {
            if (!TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.triggerFlag && !TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.overTriggerFlag)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ulong, uint>(Sys_HUD.EEvents.OnShowNpcSliderNotice, TaskVirtualNpc.uID, (uint)(TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.triggerTime * 1000));
                TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.triggerTimer?.Cancel();
                TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.triggerTimer = Timer.Register(TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.triggerTime, () =>
                {
                    TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.overTriggerFlag = true;
                    if (TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.punishWay == TrackVirtualNpcDistanceInCheckComponent.EPunishWay.DirectFail)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTaskTextContent(CSVTaskLanguage.Instance.GetConfData(CSVTaskGoal.Instance.GetConfData(TaskVirtualNpc.trackVirtualNpcTaskGoalDataComponent.taskGoalID).TracingFailTips)));
                        Sys_Track.Instance.ClearTrackVirtualNpcs();
                    }
                    else if (TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.punishWay == TrackVirtualNpcDistanceInCheckComponent.EPunishWay.SpeedUp)
                    {
                        TaskVirtualNpc.movementComponent.fMoveSpeed = TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.punishSpeed;
                    }
                    else if (TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.punishWay == TrackVirtualNpcDistanceInCheckComponent.EPunishWay.Fight)
                    {
                        Sys_Track.Instance.ClearTrackVirtualNpcs();

                        CmdTaskTrackEnterBattleReq req = new CmdTaskTrackEnterBattleReq();
                        req.BattleId = TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.punishFightID;
                        req.TaskId = TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.taskID;
                        req.TaskIndex = TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.taskIndex;

                        NetClient.Instance.SendMessage((ushort)CmdTask.EnterBattleReq, req);
                    }
                    
                });
            }
            TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.triggerFlag = true;
        }

        void TrackInTriggerExit(VirtualNpc TaskVirtualNpc)
        {
            Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnHideNpcSliderNotice, TaskVirtualNpc.uID);
            TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.triggerFlag = false;
            TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.triggerTimer?.Cancel();
            TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.triggerTimer = null;
        }

        void TrackLogic(VirtualNpc TaskVirtualNpc)
        {
            if (TaskVirtualNpc.trackVirtualNpcLogicComponent.TargetPosStack == null)
                return;

            //TrackVirtualNpcDistanceInCheckComponent trackVirtualNpcDistanceInCheckComponent = World.GetComponent<TrackVirtualNpcDistanceInCheckComponent>(TaskVirtualNpc);
            TrackVirtualNpcDistanceInCheckComponent trackVirtualNpcDistanceInCheckComponent = TaskVirtualNpc.trackVirtualNpcDistanceInCheckComponent;
            if (trackVirtualNpcDistanceInCheckComponent != null && !trackVirtualNpcDistanceInCheckComponent.overTriggerFlag && trackVirtualNpcDistanceInCheckComponent.triggerFlag)
            {
                TaskVirtualNpc.stateComponent.ChangeState(EStateType.Idle);
                TaskVirtualNpc.movementComponent.Stop();
                if (TaskVirtualNpc.trackVirtualNpcLogicComponent.curTargetPos != null)
                {
                    TaskVirtualNpc.trackVirtualNpcLogicComponent.TargetPosStack.Push(TaskVirtualNpc.trackVirtualNpcLogicComponent.curTargetPos);
                }
                return;
            }

            if (TaskVirtualNpc.trackVirtualNpcLogicComponent.TargetPosStack.Count == 0)
            {
                //TrackVirtualNpcTaskGoalDataComponent trackVirtualNpcTaskGoalDataComponent = World.GetComponent<TrackVirtualNpcTaskGoalDataComponent>(TaskVirtualNpc);
                TrackVirtualNpcTaskGoalDataComponent trackVirtualNpcTaskGoalDataComponent = TaskVirtualNpc.trackVirtualNpcTaskGoalDataComponent;
                if (trackVirtualNpcTaskGoalDataComponent != null)
                {
                    if (Sys_Task.Instance.GetTaskGoalState(trackVirtualNpcTaskGoalDataComponent.taskID, trackVirtualNpcTaskGoalDataComponent.taskGoalIndex) == ETaskGoalState.UnFinish)
                    {
                        Sys_Task.Instance.ReqStepGoalFinish(trackVirtualNpcTaskGoalDataComponent.taskID, (uint)trackVirtualNpcTaskGoalDataComponent.taskGoalIndex);
                    }
                }
            }

            while (TaskVirtualNpc.trackVirtualNpcLogicComponent.TargetPosStack.Count > 0)
            {               
                if (TaskVirtualNpc.trackVirtualNpcLogicComponent.moveFlag)
                    return;

                TaskVirtualNpc.trackVirtualNpcLogicComponent.curTargetPos = TaskVirtualNpc.trackVirtualNpcLogicComponent.TargetPosStack.Peek();
                TaskVirtualNpc.trackVirtualNpcLogicComponent.moveFlag = true;
                if (Vector3.Distance(TaskVirtualNpc.transform.position, TaskVirtualNpc.trackVirtualNpcLogicComponent.curTargetPos.pos) < 0.1f)
                {
                    CreateBubble(TaskVirtualNpc.trackVirtualNpcLogicComponent.curTargetPos.bubbleID, TaskVirtualNpc);

                    TaskVirtualNpc.trackVirtualNpcLogicComponent.TargetPosStack.Pop();
                    TaskVirtualNpc.trackVirtualNpcLogicComponent.moveFlag = false;
                }
                else
                {
                    TaskVirtualNpc.movementComponent.MoveTo(TaskVirtualNpc.trackVirtualNpcLogicComponent.curTargetPos.pos, (v) =>
                    {
                        TaskVirtualNpc.AnimationComponent.Play((uint)EStateType.Run);
                    }, null, () =>
                    {
                        CreateBubble(TaskVirtualNpc.trackVirtualNpcLogicComponent.curTargetPos.bubbleID, TaskVirtualNpc);

                        TaskVirtualNpc.trackVirtualNpcLogicComponent.TargetPosStack.Pop();
                        TaskVirtualNpc.trackVirtualNpcLogicComponent.moveFlag = false;
                    }, 0.01f);
                }
            }         
        }

        void NpcFollowLogic(VirtualNpc TaskVirtualNpc)
        {
            if (TaskVirtualNpc.virtualNPCFollowLogicComponent == null)
                return;

            if (TaskVirtualNpc.virtualNPCFollowLogicComponent.bubbleFlag == true)
            {
                float time = Random.Range(TaskVirtualNpc.virtualNPCFollowLogicComponent.TimerMin, TaskVirtualNpc.virtualNPCFollowLogicComponent.TimerMax);
                int index = Random.Range(0, TaskVirtualNpc.virtualNPCFollowLogicComponent.Bubbles.Count - 1);
                TaskVirtualNpc.virtualNPCFollowLogicComponent.bubbleFlag = false;
                TaskVirtualNpc.virtualNPCFollowLogicComponent.bubbleTimer?.Cancel();
                TaskVirtualNpc.virtualNPCFollowLogicComponent.bubbleTimer = Timer.Register(time, () =>
                {
                    CreateBubble(TaskVirtualNpc.virtualNPCFollowLogicComponent.Bubbles[index], TaskVirtualNpc);
                    TaskVirtualNpc.virtualNPCFollowLogicComponent.bubbleFlag = true;
                });
            }

            if (Vector3.Distance(TaskVirtualNpc.virtualNPCFollowLogicComponent.VirtualNpc.transform.position, GameCenter.mainHero.transform.position) > 20)
            {
                TaskVirtualNpc.virtualNPCFollowLogicComponent.VirtualNpc.movementComponent.TransformToPosImmediately(GameCenter.mainHero.transform.position);
            }

            if (Vector3.Distance(GameCenter.mainHero.transform.position, TaskVirtualNpc.virtualNPCFollowLogicComponent.DestPos) < TaskVirtualNpc.virtualNPCFollowLogicComponent.Distance)
            {
                //NpcFollowVirtualNpcTaskGoalDataComponent npcFollowVirtualNpcTaskGoalDataComponent = World.GetComponent<NpcFollowVirtualNpcTaskGoalDataComponent>(VirtualNpc);
                NpcFollowVirtualNpcTaskGoalDataComponent npcFollowVirtualNpcTaskGoalDataComponent = TaskVirtualNpc.virtualNPCFollowLogicComponent.VirtualNpc.npcFollowVirtualNpcTaskGoalDataComponent;
                if (npcFollowVirtualNpcTaskGoalDataComponent != null)
                {
                    if (Sys_Task.Instance.GetTaskGoalState(npcFollowVirtualNpcTaskGoalDataComponent.taskID, npcFollowVirtualNpcTaskGoalDataComponent.taskGoalIndex) == ETaskGoalState.UnFinish)
                    {
                        Sys_Task.Instance.ReqStepGoalFinish(npcFollowVirtualNpcTaskGoalDataComponent.taskID, (uint)npcFollowVirtualNpcTaskGoalDataComponent.taskGoalIndex);
                    }
                }
            }
        }

        void EscortCheck(VirtualNpc TaskVirtualNpc)
        {
            EffectUtil.Instance.UpdateEffectRotation(GameCenter.mainHero.uID, EffectUtil.EEffectTag.Escort, TaskVirtualNpc.transform.position, new Vector3(0, 180f, 0));

            if (EscortCheckResult(TaskVirtualNpc))
            {
                EscortTrigger(TaskVirtualNpc);
            }
            else
            {
                EscortTriggerExit(TaskVirtualNpc);
            }
        }

        bool EscortCheckResult(VirtualNpc TaskVirtualNpc)
        {
            float distance = (GameCenter.mainHero.transform.position - TaskVirtualNpc.transform.position).sqrMagnitude;
            if (TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent != null && distance > (TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.distanceLimit * TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.distanceLimit))
                return false;

            return true;
        }

        void EscortTrigger(VirtualNpc TaskVirtualNpc)
        {
            if (TaskVirtualNpc == null)
                return;

            if (TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent == null)
                return;

            if (!TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.triggerInFlag)
            {
                EffectUtil.Instance.LoadEffect(TaskVirtualNpc.uID, CSVEffect.Instance.GetConfData(3000104129).effects_path, TaskVirtualNpc.fxRoot.transform, EffectUtil.EEffectTag.EscortIn, 0f, TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.distanceLimit / 3f, TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.distanceLimit / 3f);
                EffectUtil.Instance.UnloadEffectByTag(TaskVirtualNpc.uID, EffectUtil.EEffectTag.EscortOut);
            }
            TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.triggerInFlag = true;

            if (TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.TargetPosQueue == null)
                return;

            if (TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.TargetPosQueue.Count == 0)
            {
                //EscortVirtualNpcTaskGoalDataComponent escortVirtualNpcTaskGoalDataComponent = World.GetComponent<EscortVirtualNpcTaskGoalDataComponent>(TaskVirtualNpc);
                EscortVirtualNpcTaskGoalDataComponent escortVirtualNpcTaskGoalDataComponent = TaskVirtualNpc.escortVirtualNpcTaskGoalDataComponent;
                if (escortVirtualNpcTaskGoalDataComponent != null)
                {
                    if (Sys_Task.Instance.GetTaskGoalState(escortVirtualNpcTaskGoalDataComponent.taskID, escortVirtualNpcTaskGoalDataComponent.taskGoalIndex) == ETaskGoalState.UnFinish)
                    {
                        Sys_Task.Instance.ReqStepGoalFinish(escortVirtualNpcTaskGoalDataComponent.taskID, (uint)escortVirtualNpcTaskGoalDataComponent.taskGoalIndex);
                    }
                }
            }

            while (TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.TargetPosQueue.Count > 0)
            {
                if (TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.moveFlag)
                    return;

                TargetPosWrap curTargetPos = TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.TargetPosQueue.Peek();
                TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.moveFlag = true;
                if (Vector3.Distance(TaskVirtualNpc.transform.position, curTargetPos.pos) < 0.1f)
                {
                    CreateBubble(curTargetPos.bubbleID, TaskVirtualNpc);

                    TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.TargetPosQueue.Dequeue();
                    TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.moveFlag = false;
                }
                else
                {
                    TaskVirtualNpc.movementComponent.MoveTo(curTargetPos.pos, (v) =>
                    {
                        TaskVirtualNpc.AnimationComponent.Play((uint)EStateType.Run);
                    }, null, () =>
                    {
                        CreateBubble(curTargetPos.bubbleID, TaskVirtualNpc);

                        TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.TargetPosQueue.Dequeue();
                        TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.moveFlag = false;
                    }, 0.01f);
                }
            }
        }

        void EscortTriggerExit(VirtualNpc TaskVirtualNpc)
        {
            if (TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.triggerInFlag)
            {
                EffectUtil.Instance.LoadEffect(TaskVirtualNpc.uID, CSVEffect.Instance.GetConfData(3000104130).effects_path, TaskVirtualNpc.fxRoot.transform, EffectUtil.EEffectTag.EscortOut, 0f, TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.distanceLimit / 3f, TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.distanceLimit / 3f);
                EffectUtil.Instance.UnloadEffectByTag(TaskVirtualNpc.uID, EffectUtil.EEffectTag.EscortIn);

            }
            TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.triggerInFlag = false;

            TaskVirtualNpc.movementComponent.Stop(false);
            TaskVirtualNpc.escortVirtualNpcDistanceCheckComponent.moveFlag = false;
        }

        void CreateBubble(uint bubbleId, VirtualNpc TaskVirtualNpc)
        {
            if (bubbleId > 0u)
            {
                CSVBubble.Data cSVBubbleData = CSVBubble.Instance.GetConfData(bubbleId);
                if (cSVBubbleData != null)
                {
                    if (cSVBubbleData.Type == (uint)EBubbleType.PureWord)
                    {
                        TriggerNpcBubbleEvt triggerNpcBubbleEvt = new TriggerNpcBubbleEvt();
                        triggerNpcBubbleEvt.ownerType = 0;
                        triggerNpcBubbleEvt.npcid = TaskVirtualNpc.cSVNpcData.id * 100000ul;
                        triggerNpcBubbleEvt.npcInfoId = TaskVirtualNpc.cSVNpcData.id;
                        triggerNpcBubbleEvt.bubbleid = bubbleId;
                        triggerNpcBubbleEvt.npcobj = TaskVirtualNpc.gameObject;

                        Sys_HUD.Instance.eventEmitter.Trigger<TriggerNpcBubbleEvt>(Sys_HUD.EEvents.OnTriggerNpcBubble, triggerNpcBubbleEvt);
                    }
                    else if (cSVBubbleData.Type == (uint)EBubbleType.EmojiText)
                    {
                        TriggerExpressionBubbleEvt triggerEmotionBubbleEvt = new TriggerExpressionBubbleEvt();
                        triggerEmotionBubbleEvt.id = TaskVirtualNpc.cSVNpcData.id * 100000ul;
                        triggerEmotionBubbleEvt.npcInfoId = TaskVirtualNpc.cSVNpcData.id;
                        triggerEmotionBubbleEvt.ownerType = 0;
                        triggerEmotionBubbleEvt.bubbleId = bubbleId;
                        CSVLanguage.Data cSVLanguageData = CSVLanguage.Instance.GetConfData(cSVBubbleData.BubbleText);
                        if (cSVLanguageData != null)
                        {
                            triggerEmotionBubbleEvt.content = cSVLanguageData.words;
                        }
                        else
                        {
                            DebugUtil.LogError($"CSVLanguage.Data is null id:{cSVBubbleData.BubbleText}");
                        }
                        triggerEmotionBubbleEvt.showTime = cSVBubbleData.BubbleTime;
                        triggerEmotionBubbleEvt.gameObject = TaskVirtualNpc.gameObject;

                        Sys_HUD.Instance.eventEmitter.Trigger<TriggerExpressionBubbleEvt>(Sys_HUD.EEvents.OnTriggerExpressionBubble, triggerEmotionBubbleEvt);
                    }
                    else if (cSVBubbleData.Type == (uint)EBubbleType.Pic)
                    {
                        CreateEmotionEvt createEmotionEvt = new CreateEmotionEvt();
                        createEmotionEvt.actorId = TaskVirtualNpc.cSVNpcData.id * 100000ul;
                        createEmotionEvt.gameObject = TaskVirtualNpc.gameObject;
                        createEmotionEvt.emtionId = cSVBubbleData.MoodId;
                        Sys_HUD.Instance.eventEmitter.Trigger<CreateEmotionEvt>(Sys_HUD.EEvents.OnCreateEmotion, createEmotionEvt);
                    }
                }
#if DEBUG_MODE
                else
                {
                    Lib.Core.DebugUtil.LogError($"CSVBubbleData表中没有Id：{bubbleId.ToString()}");
                }
#endif
            }
        }
    }

    public class EscortVirtualNpcDistanceCheckComponent
    {
        public VirtualNpc TaskVirtualNpc
        {
            get;
            set;
        }

        public float distanceLimit
        {
            get;
            set;
        }

        public Queue<TargetPosWrap> TargetPosQueue;

        public bool moveFlag = false;

        public bool triggerInFlag = false;

        public void InitTargetPosQueue(List<TargetPosWrap> positions)
        {
            if (positions == null || positions.Count == 0)
                return;

            TargetPosQueue = new Queue<TargetPosWrap>();
            for (int index = 0, len = positions.Count; index < len; index++)
            {
                TargetPosWrap targetPosWrap = new TargetPosWrap();
                targetPosWrap.pos = positions[index].pos;
                targetPosWrap.bubbleID = positions[index].bubbleID;
                TargetPosQueue.Enqueue(targetPosWrap);
            }
        }

        public void Dispose()
        {
            TaskVirtualNpc = null;
            moveFlag = false;
            TargetPosQueue?.Clear();
            TargetPosQueue = null;
            triggerInFlag = false;
        }      
    }
}
