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
    public class Sys_Track : SystemModuleBase<Sys_Track>
    {
        public bool TrackFlag;
        uint curHandlerID;
        uint curHandlerIndex;

        Dictionary<uint, bool> npcTrackFlags = new Dictionary<uint, bool>();

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnSyncTrackStart,
            OnSyncTrackEnd,
        }

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.SyncTrackStart, OnSyncTrackStart, CmdTeamSyncTrackStart.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.SyncTrackEnd, OnSyncTrackEnd, CmdTeamSyncTrackEnd.Parser);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnEnterMap, OnEnterMap, true);
        }

        void OnEnterMap()
        {
            if (TrackFlag)
            {
                if (Sys_Team.Instance.HaveTeam)
                {
                    if (Sys_Team.Instance.isCaptain())
                    {
                        CmdTeamSyncTrackEnd cmdTeamSyncTrackEnd = new CmdTeamSyncTrackEnd();
                        cmdTeamSyncTrackEnd.TaskId = curHandlerID;
                        cmdTeamSyncTrackEnd.Index = curHandlerIndex;

                        NetClient.Instance.SendMessage((ushort)CmdTeam.SyncTrackEnd, cmdTeamSyncTrackEnd);
                    }
                }
                else
                {
                    ClearTrackVirtualNpcs();
                }
            }
            //VirtualShowManager.Instance.ClearVirtualSceneActors();
        }

        public override void OnLogin()
        {
            base.OnLogin();

            ClearTrackVirtualNpcs();
            npcTrackFlags.Clear();
            TrackFlag = false;
            curHandlerID = curHandlerIndex = 0u;
        }

        public override void OnLogout()
        {
            base.OnLogout();

            ClearTrackVirtualNpcs();
            npcTrackFlags.Clear();
            TrackFlag = false;
            curHandlerID = curHandlerIndex = 0u;
        }

        public bool IsNpcTracking(uint npcInfoID)
        {
            if (npcTrackFlags.ContainsKey(npcInfoID) && npcTrackFlags[npcInfoID])
                return true;
            return false;
        }

        void OnSyncTrackStart(NetMsg msg)
        {
            CmdTeamSyncTrackStart cmdTeamSyncTrackStart = NetMsgUtil.Deserialize<CmdTeamSyncTrackStart>(CmdTeamSyncTrackStart.Parser, msg);
            if (cmdTeamSyncTrackStart != null)
            {
                StartTrack(cmdTeamSyncTrackStart.TaskId, cmdTeamSyncTrackStart.Index);
            }
        }

        public void StartTrack(uint taskID, uint taskGoalIndex)
        {
            TrackFlag = true;
            curHandlerID = taskID;
            curHandlerIndex = taskGoalIndex;

            uint taskGoalID = taskID * 10 + taskGoalIndex + 1;
            CSVTaskGoal.Data cSVTaskGoalData = CSVTaskGoal.Instance.GetConfData(taskGoalID);
            if (cSVTaskGoalData != null)
            {
                if (cSVTaskGoalData.TracingNpc != null)
                {
                    for (int index = 0, len = cSVTaskGoalData.TracingNpc.Count; index < len; index++)
                    {
                        npcTrackFlags[cSVTaskGoalData.TracingNpc[index]] = true;
                        eventEmitter.Trigger<uint>(EEvents.OnSyncTrackStart, cSVTaskGoalData.TracingNpc[index]);
                    }
                }

                CreateTrackVirtualNpcs(taskID, taskGoalID, (int)taskGoalIndex);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTaskTextContent(CSVTaskLanguage.Instance.GetConfData(cSVTaskGoalData.TracingStartTips)));
            }
            else
            {
                DebugUtil.LogError($"CSVTaskGoal.Data IS NULL ID:{taskGoalID}");
            }
        }

        void OnSyncTrackEnd(NetMsg msg)
        {
            CmdTeamSyncTrackEnd cmdTeamSyncTrackEnd = NetMsgUtil.Deserialize<CmdTeamSyncTrackEnd>(CmdTeamSyncTrackEnd.Parser, msg);
            if (cmdTeamSyncTrackEnd != null)
            {
                TrackFlag = false;
                uint taskGoalID = cmdTeamSyncTrackEnd.TaskId * 10 + cmdTeamSyncTrackEnd.Index + 1;
                CSVTaskGoal.Data cSVTaskGoalData = CSVTaskGoal.Instance.GetConfData(taskGoalID);
                if (cSVTaskGoalData != null)
                {
                    if (cSVTaskGoalData.TracingNpc != null)
                    {
                        for (int index = 0, len = cSVTaskGoalData.TracingNpc.Count; index < len; index++)
                        {
                            npcTrackFlags[cSVTaskGoalData.TracingNpc[index]] = false;
                            eventEmitter.Trigger<uint>(EEvents.OnSyncTrackEnd, cSVTaskGoalData.TracingNpc[index]);
                        }
                    }
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTaskTextContent(CSVTaskLanguage.Instance.GetConfData(cSVTaskGoalData.TracingCompleteTips)));
                }
                else
                {
                    DebugUtil.LogError($"CSVTaskGoal.Data IS NULL ID:{taskGoalID}");
                }
                ClearTrackVirtualNpcs();
            }
        }

        public Dictionary<ulong, VirtualNpc> trackVirtualNpcs = new Dictionary<ulong, VirtualNpc>();

        public void ClearTrackVirtualNpcs()
        {
            TrackFlag = false;
            foreach (var actor in trackVirtualNpcs.Values)
            {
                npcTrackFlags[actor.cSVNpcData.id] = false;
                eventEmitter.Trigger<uint>(EEvents.OnSyncTrackEnd, actor.cSVNpcData.id);
                //GameCenter.mainWorld.DestroyActor(actor);
                World.CollecActor(actor);
            }
            trackVirtualNpcs.Clear();
        }

        public void CreateTrackVirtualNpcs(uint taskID, uint taskGoalID, int taskGoalIndex)
        {
            CSVTaskGoal.Data cSVTaskGoalData = CSVTaskGoal.Instance.GetConfData(taskGoalID);
            if (cSVTaskGoalData.TracingNpc != null)
            {
                for (int index = 0, len = cSVTaskGoalData.TracingNpc.Count; index < len; index++)
                {
                    //VirtualNpc taskVirtualNpc = GameCenter.mainWorld.CreateActor<VirtualNpc>((ulong)cSVTaskGoalData.TracingNpc[index] * 100000);
                    ulong uid = (ulong)cSVTaskGoalData.TracingNpc[index] * 100000;
                    VirtualNpc taskVirtualNpc = World.AllocActor<VirtualNpc>(uid);
                    taskVirtualNpc.SetParent(GameCenter.npcRoot.transform);
                    taskVirtualNpc.SetName($"TaskVirtualNPC_{uid.ToString()}");

                    trackVirtualNpcs.Add(taskVirtualNpc.uID, taskVirtualNpc);
                    taskVirtualNpc.cSVNpcData = CSVNpc.Instance.GetConfData(cSVTaskGoalData.TracingNpc[index]);

                    //taskVirtualNpc.stateComponent = World.AddComponent<StateComponent>(taskVirtualNpc);

                    //taskVirtualNpc.transform.position = PosConvertUtil.Svr2Client(cSVTaskGoalData.TracingNpcLocation[index][0], cSVTaskGoalData.TracingNpcLocation[index][1]);
                    taskVirtualNpc.transform.localEulerAngles = new Vector3(0, cSVTaskGoalData.TracingNpcOrientations, 0);
                    //taskVirtualNpc.movementComponent = World.AddComponent<MovementComponent>(taskVirtualNpc);
                    //taskVirtualNpc.movementComponent.TransformToPosImmediately(PosConvertUtil.Svr2Client(cSVTaskGoalData.TracingNpcLocation[index][0], cSVTaskGoalData.TracingNpcLocation[index][1]));
                    NavMeshHit navMeshHit;
                    Vector3 hitPos = PosConvertUtil.Svr2Client(cSVTaskGoalData.TracingNpcLocation[index][0], cSVTaskGoalData.TracingNpcLocation[index][1]);
                    MovementComponent.GetNavMeshHit(hitPos, out navMeshHit);
                    if (navMeshHit.hit)
                        taskVirtualNpc.transform.position = navMeshHit.position;
                    else
                        taskVirtualNpc.transform.position = hitPos;

                    taskVirtualNpc.movementComponent.InitNavMeshAgent();
                    taskVirtualNpc.movementComponent.fMoveSpeed = cSVTaskGoalData.TracingMoveSpeed / 10000f;

                    if (index == 0)
                    {
                        taskVirtualNpc.eVirtualNpcType = VirtualNpc.EVirtualNpcType.Track;
                        taskVirtualNpc.LoadModel(taskVirtualNpc.cSVNpcData.model, (actor) =>
                        {
                            taskVirtualNpc.AnimationComponent.SetSimpleAnimation(taskVirtualNpc.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                            //taskVirtualNpc.AnimationComponent = World.AddComponent<AnimationComponent>(actor);
                            taskVirtualNpc.modelGameObject.SetActive(false);
                            taskVirtualNpc.AnimationComponent.UpdateHoldingAnimations(taskVirtualNpc.cSVNpcData.action_id, Constants.UMARMEDID, null, EStateType.Idle, taskVirtualNpc.modelGameObject);

                            taskVirtualNpc.trackVirtualNpcTaskGoalDataComponent = new TrackVirtualNpcTaskGoalDataComponent();
                            taskVirtualNpc.trackVirtualNpcTaskGoalDataComponent.taskID = taskID;
                            taskVirtualNpc.trackVirtualNpcTaskGoalDataComponent.taskGoalID = taskGoalID;
                            taskVirtualNpc.trackVirtualNpcTaskGoalDataComponent.taskGoalIndex = taskGoalIndex;

                            taskVirtualNpc.trackVirtualNpcLogicComponent = new TrackVirtualNpcLogicComponent();
                            taskVirtualNpc.trackVirtualNpcLogicComponent.TaskVirtualNpc = taskVirtualNpc;
                            taskVirtualNpc.trackVirtualNpcLogicComponent.distanceIn = cSVTaskGoalData.TracingAlertScope / 10000f;
                            taskVirtualNpc.trackVirtualNpcLogicComponent.distanceOut = cSVTaskGoalData.TracingNpcTriggerDis / 10000f;
                            taskVirtualNpc.trackVirtualNpcLogicComponent.failDistance = cSVTaskGoalData.TracingFailDistance / 10000f;
                            List<TargetPosWrap> pos = new List<TargetPosWrap>();
                            for (int index2 = 0, len2 = cSVTaskGoalData.TracingLocation.Count; index2 < len2; index2++)
                            {
                                TargetPosWrap targetPosWrap = new TargetPosWrap();
                                targetPosWrap.pos = PosConvertUtil.Svr2Client(cSVTaskGoalData.TracingLocation[index2][0], cSVTaskGoalData.TracingLocation[index2][1]);
                                if (cSVTaskGoalData.TracingLocation[index2].Count >= 3)
                                {
                                    targetPosWrap.bubbleID = cSVTaskGoalData.TracingLocation[index2][2];
                                }
                                pos.Add(targetPosWrap);
                            }
                            Timer.Register(0.5f, () =>
                            {
                                taskVirtualNpc.trackVirtualNpcLogicComponent.InitTargetPosStack(pos);
                                taskVirtualNpc.trackVirtualNpcLogicComponent.LoadEffect();
                            });

                            taskVirtualNpc.virtualNpcHUDComponent = new VirtualNpcHUDComponent();
                            taskVirtualNpc.virtualNpcHUDComponent.virtualNpc = taskVirtualNpc;
                            taskVirtualNpc.virtualNpcHUDComponent.OnConstruct();

                            taskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent = new TrackVirtualNpcDistanceOutCheckComponent();
                            taskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.TaskVirtualNpc = taskVirtualNpc;
                            taskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.distanceLimitMax = cSVTaskGoalData.TracingNpcTriggerDis / 10000f;
                            taskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.overTime = cSVTaskGoalData.FailDistanceTime / 1000f;
                            float distanceFaill = Vector3.Distance(taskVirtualNpc.transform.position, GameCenter.mainHero.transform.position);
                            if (distanceFaill > taskVirtualNpc.trackVirtualNpcLogicComponent.failDistance)
                            {
                                taskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.OutFlag = true;
                            }
                            else
                            {
                                taskVirtualNpc.trackVirtualNpcDistanceOutCheckComponent.OutFlag = false;
                            }

                            taskVirtualNpc.trackVirtualNpcDistanceInCheckComponent = new TrackVirtualNpcDistanceInCheckComponent();
                            taskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.TaskVirtualNpc = taskVirtualNpc;
                            taskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.distanceLimitMin = cSVTaskGoalData.TracingAlertScope /10000f;
                            taskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.triggerTime = cSVTaskGoalData.TracingAlertTime / 1000f;
                            taskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.punishWay = (TrackVirtualNpcDistanceInCheckComponent.EPunishWay)cSVTaskGoalData.TracingPunishWay;
                            taskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.punishSpeed = cSVTaskGoalData.PunishNpcMoveSpeed / 10000f;
                            taskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.punishFightID = cSVTaskGoalData.PunishFight;
                            taskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.taskID = curHandlerID;
                            taskVirtualNpc.trackVirtualNpcDistanceInCheckComponent.taskIndex = curHandlerIndex;
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

                        followComponent.Target = trackVirtualNpcs[(ulong)cSVTaskGoalData.TracingNpc[0] * 100000];
                        followComponent.Follow = true;
                    }
                }
            }
        }

        public override void Dispose()
        {
            ClearTrackVirtualNpcs();

            base.Dispose();
        }
    }

    public class TrackVirtualNpcTaskGoalDataComponent
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

    public class TrackVirtualNpcLogicComponent
    {
        public VirtualNpc TaskVirtualNpc
        {
            get;
            set;
        }

        public float distanceOut
        {
            get;
            set;
        }

        public float distanceIn
        {
            get;
            set;
        }

        public float failDistance
        {
            get;
            set;
        }

        public Stack<TargetPosWrap> TargetPosStack;

        public bool moveFlag = false;

        public void InitTargetPosStack(List<TargetPosWrap> positions)
        {
            if (positions == null || positions.Count == 0)
                return;

            TargetPosStack = new Stack<TargetPosWrap>();
            for (int index = positions.Count - 1; index >= 0; index--)
            {
                TargetPosWrap targetPosWrap = new TargetPosWrap();
                targetPosWrap.pos = positions[index].pos;
                targetPosWrap.bubbleID = positions[index].bubbleID;
                TargetPosStack.Push(targetPosWrap);
            }
        }

        public void Dispose()
        {
            TaskVirtualNpc = null;
            moveFlag = false;
            TargetPosStack?.Clear();
            TargetPosStack = null;
            distanceOut = 0;
            distanceIn = 0;
            curTargetPos = null;
        }

        public void LoadEffect()
        {
            EffectUtil.Instance.LoadEffect(TaskVirtualNpc.uID, CSVEffect.Instance.GetConfData(3000104129).effects_path, TaskVirtualNpc.fxRoot.transform, EffectUtil.EEffectTag.TrackOut, 0f, distanceOut / 3f, distanceOut / 3f);
            EffectUtil.Instance.LoadEffect(TaskVirtualNpc.uID, CSVEffect.Instance.GetConfData(3000104134).effects_path, TaskVirtualNpc.fxRoot.transform, EffectUtil.EEffectTag.TrackIn, 0f, distanceIn / 3f, distanceIn / 3f);
        }

        public TargetPosWrap curTargetPos = null;
    }

    /// <summary>
    /// 失败检测///
    /// </summary>
    public class TrackVirtualNpcDistanceOutCheckComponent
    {
        public VirtualNpc TaskVirtualNpc
        {
            get;
            set;
        }

        public float distanceLimitMax
        {
            get;
            set;
        }

        public float overTime
        {
            get;
            set;
        }

        public Timer overTimeTimer;

        public bool OutFlag = false;

        public void Dispose()
        {
            TaskVirtualNpc = null;
            overTimeTimer?.Cancel();
            overTimeTimer = null;
            OutFlag = false;
        }
    }

    /// <summary>
    /// 警觉///
    /// </summary>
    public class TrackVirtualNpcDistanceInCheckComponent
    {
        public enum EPunishWay
        {
            None,
            DirectFail,
            SpeedUp,
            Fight,
        }

        public VirtualNpc TaskVirtualNpc
        {
            get;
            set;
        }

        public float distanceLimitMin
        {
            get;
            set;
        }

        public float triggerTime
        {
            get;
            set;
        }

        public EPunishWay punishWay
        {
            get;
            set;
        }

        public float punishSpeed
        {
            get;
            set;
        }

        public uint punishFightID
        {
            get;
            set;
        }

        public uint taskID
        {
            get;
            set;
        }

        public uint taskIndex
        {
            get;
            set;
        }

        public Timer triggerTimer;

        public bool triggerFlag;

        public bool overTriggerFlag;

        public void Dispose()
        {
            TaskVirtualNpc = null;
            triggerTimer?.Cancel();
            triggerTimer = null;
            triggerFlag = false;
            overTriggerFlag = false;
            punishWay = EPunishWay.None;
            punishSpeed = 0f;
            punishFightID = 0;
        }
    }
}
