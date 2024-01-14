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
    public class Sys_NpcFollow : SystemModuleBase<Sys_NpcFollow>
    {
        public bool NpcFollowFlag;
        uint curHandlerID;
        uint curHandlerIndex;

        Dictionary<uint, bool> npcFollowFlags = new Dictionary<uint, bool>();

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnSyncNpcFollowStart,
            OnSyncNpcFollowEnd,
        }

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.SyncFollowStart, OnSyncFollowStart, CmdTeamSyncFollowStart.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.SyncFollowEnd, OnSyncFollowEnd, CmdTeamSyncFollowEnd.Parser);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnEnterMap, OnEnterMap, true);
        }

        void OnEnterMap()
        {
            if (NpcFollowFlag)
            {
                if (Sys_Team.Instance.HaveTeam)
                {
                    if (Sys_Team.Instance.isCaptain())
                    {
                        CmdTeamSyncFollowEnd cmdTeamSyncFollowEnd = new CmdTeamSyncFollowEnd();
                        cmdTeamSyncFollowEnd.TaskId = curHandlerID;
                        cmdTeamSyncFollowEnd.Index = curHandlerIndex;

                        NetClient.Instance.SendMessage((ushort)CmdTeam.SyncFollowEnd, cmdTeamSyncFollowEnd);
                    }
                }
                else
                {
                    ClearNpcFollowVirtualNpcs();
                }
            }
            //VirtualShowManager.Instance.ClearVirtualSceneActors();
        }

        public override void OnLogin()
        {
            base.OnLogin();

            ClearNpcFollowVirtualNpcs();
            npcFollowFlags.Clear();
            NpcFollowFlag = false;
            curHandlerID = curHandlerIndex = 0u;
        }

        public override void OnLogout()
        {
            base.OnLogout();

            ClearNpcFollowVirtualNpcs();
            npcFollowFlags.Clear();
            NpcFollowFlag = false;
            curHandlerID = curHandlerIndex = 0u;
        }

        public bool IsNpcFollowing(uint npcInfoID)
        {
            if (npcFollowFlags.ContainsKey(npcInfoID) && npcFollowFlags[npcInfoID])
            {
                return true;
            }
            return false;
        }

        void OnSyncFollowStart(NetMsg msg)
        {
            CmdTeamSyncFollowStart cmdTeamSyncFollowStart = NetMsgUtil.Deserialize<CmdTeamSyncFollowStart>(CmdTeamSyncFollowStart.Parser, msg);
            if (cmdTeamSyncFollowStart != null)
            {
                StartNpcFollow(cmdTeamSyncFollowStart.TaskId, cmdTeamSyncFollowStart.Index);
            }
        }

        public void StartNpcFollow(uint taskID, uint taskGoalIndex)
        {
            NpcFollowFlag = true;
            curHandlerID = taskID;
            curHandlerIndex = taskGoalIndex;

            uint taskGoalID = taskID * 10 + taskGoalIndex + 1;
            CSVTaskGoal.Data cSVTaskGoalData = CSVTaskGoal.Instance.GetConfData(taskGoalID);
            if (cSVTaskGoalData != null)
            {
                if (cSVTaskGoalData.FollowNpc != null)
                {
                    for (int index = 0, len = cSVTaskGoalData.FollowNpc.Count; index < len; index++)
                    {
                        npcFollowFlags[cSVTaskGoalData.FollowNpc[index]] = true;
                        eventEmitter.Trigger<uint>(EEvents.OnSyncNpcFollowStart, cSVTaskGoalData.FollowNpc[index]);
                    }
                }

                CreateNpcFollowVirtualNpcs(taskID, taskGoalID, (int)taskGoalIndex);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTaskTextContent(CSVTaskLanguage.Instance.GetConfData(cSVTaskGoalData.FollowStartTips)));
            }
            else
            {
                DebugUtil.LogError($"CSVTaskGoal.Data IS NULL ID:{taskGoalID}");
            }

        }

        void OnSyncFollowEnd(NetMsg msg)
        {
            CmdTeamSyncFollowEnd cmdTeamSyncFollowEnd = NetMsgUtil.Deserialize<CmdTeamSyncFollowEnd>(CmdTeamSyncFollowEnd.Parser, msg);
            if (cmdTeamSyncFollowEnd != null)
            {
                NpcFollowFlag = false;
                uint taskGoalID = cmdTeamSyncFollowEnd.TaskId * 10 + cmdTeamSyncFollowEnd.Index + 1;
                CSVTaskGoal.Data cSVTaskGoalData = CSVTaskGoal.Instance.GetConfData(taskGoalID);
                if (cSVTaskGoalData != null)
                {
                    if (cSVTaskGoalData.FollowNpc != null)
                    {
                        for (int index = 0, len = cSVTaskGoalData.FollowNpc.Count; index < len; index++)
                        {
                            npcFollowFlags[cSVTaskGoalData.FollowNpc[index]] = false;
                            eventEmitter.Trigger<uint>(EEvents.OnSyncNpcFollowEnd, cSVTaskGoalData.FollowNpc[index]);
                        }
                    }
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTaskTextContent(CSVTaskLanguage.Instance.GetConfData(cSVTaskGoalData.FollowCompleteTips)));
                }
                else
                {
                    DebugUtil.LogError($"CSVTaskGoal.Data IS NULL ID:{taskGoalID}");
                }
                ClearNpcFollowVirtualNpcs();
            }
        }

        public Dictionary<ulong, VirtualNpc> npcFollowVirtualNpcs = new Dictionary<ulong, VirtualNpc>();

        public void ClearNpcFollowVirtualNpcs()
        {
            NpcFollowFlag = false;
            foreach (var actor in npcFollowVirtualNpcs.Values)
            {
                npcFollowFlags[actor.cSVNpcData.id] = false;
                eventEmitter.Trigger<uint>(EEvents.OnSyncNpcFollowEnd, actor.cSVNpcData.id);
                Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnHideNpcArrow, actor.uID);
                //GameCenter.mainWorld.DestroyActor(actor);
                World.CollecActor(actor);
            }
            npcFollowVirtualNpcs.Clear();
        }

        public void CreateNpcFollowVirtualNpcs(uint taskID, uint taskGoalID, int taskGoalIndex)
        {
            CSVTaskGoal.Data cSVTaskGoalData = CSVTaskGoal.Instance.GetConfData(taskGoalID);
            if (cSVTaskGoalData != null && cSVTaskGoalData.FollowNpc != null)
            {
                for (int index = 0, len = cSVTaskGoalData.FollowNpc.Count; index < len; index++)
                {
                    ulong uid = (ulong)cSVTaskGoalData.FollowNpc[index] * 100000 + (ulong)index;
                    VirtualNpc taskVirtualNpc = World.AllocActor<VirtualNpc>(uid);
                    taskVirtualNpc.SetParent(GameCenter.npcRoot.transform);
                    taskVirtualNpc.SetName($"TaskVirtualNPC_{uid.ToString()}");

                    npcFollowVirtualNpcs.Add(taskVirtualNpc.uID, taskVirtualNpc);
                    taskVirtualNpc.cSVNpcData = CSVNpc.Instance.GetConfData(cSVTaskGoalData.FollowNpc[index]);

                    taskVirtualNpc.transform.localEulerAngles = new Vector3(0, cSVTaskGoalData.FollowNpcOrientations, 0);
                    NavMeshHit navMeshHit;
                    Vector3 hitPos = PosConvertUtil.Svr2Client(cSVTaskGoalData.FollowNpcLocation[index][0], cSVTaskGoalData.FollowNpcLocation[index][1]);
                    MovementComponent.GetNavMeshHit(hitPos, out navMeshHit);
                    if (navMeshHit.hit)
                        taskVirtualNpc.transform.position = navMeshHit.position;
                    else
                        taskVirtualNpc.transform.position = hitPos;

                    taskVirtualNpc.movementComponent.InitNavMeshAgent();
                    taskVirtualNpc.movementComponent.fMoveSpeed = cSVTaskGoalData.FollowMoveSpeed / 10000f;

                    if (index == 0)
                    {
                        taskVirtualNpc.eVirtualNpcType = VirtualNpc.EVirtualNpcType.NpcFollow;
                        taskVirtualNpc.LoadModel(taskVirtualNpc.cSVNpcData.model, (actor) =>
                        {
                            taskVirtualNpc.AnimationComponent.SetSimpleAnimation(taskVirtualNpc.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                            taskVirtualNpc.modelGameObject.SetActive(false);
                            taskVirtualNpc.AnimationComponent.UpdateHoldingAnimations(taskVirtualNpc.cSVNpcData.action_id, Constants.UMARMEDID, null, EStateType.Idle, taskVirtualNpc.modelGameObject);

                            taskVirtualNpc.npcFollowVirtualNpcTaskGoalDataComponent = new NpcFollowVirtualNpcTaskGoalDataComponent();
                            NpcFollowVirtualNpcTaskGoalDataComponent npcFollowVirtualNpcTaskGoalDataComponent = taskVirtualNpc.npcFollowVirtualNpcTaskGoalDataComponent;
                            npcFollowVirtualNpcTaskGoalDataComponent.taskID = taskID;
                            npcFollowVirtualNpcTaskGoalDataComponent.taskGoalID = taskGoalID;
                            npcFollowVirtualNpcTaskGoalDataComponent.taskGoalIndex = taskGoalIndex;                            

                            taskVirtualNpc.virtualNPCFollowLogicComponent = new VirtualNPCFollowLogicComponent();
                            taskVirtualNpc.virtualNPCFollowLogicComponent.VirtualNpc = taskVirtualNpc;
                            taskVirtualNpc.virtualNPCFollowLogicComponent.DestPos = new Vector3(cSVTaskGoalData.FollowNpcTargetPlace[0], 0, -cSVTaskGoalData.FollowNpcTargetPlace[1]);
                            taskVirtualNpc.virtualNPCFollowLogicComponent.Distance = cSVTaskGoalData.FollowNpcTargetScope;
                            if (cSVTaskGoalData.FollowNpcBubble != null && cSVTaskGoalData.FollowNpcBubble.Count > 0)
                            {
                                taskVirtualNpc.virtualNPCFollowLogicComponent.Bubbles = cSVTaskGoalData.FollowNpcBubble;
                            }
                            if (cSVTaskGoalData.FollowNpcBubbleInterval != null && cSVTaskGoalData.FollowNpcBubbleInterval.Count > 1)
                            {
                                taskVirtualNpc.virtualNPCFollowLogicComponent.TimerMax = cSVTaskGoalData.FollowNpcBubbleInterval[1];
                                taskVirtualNpc.virtualNPCFollowLogicComponent.TimerMin = cSVTaskGoalData.FollowNpcBubbleInterval[0];
                            }

                            taskVirtualNpc.virtualNpcHUDComponent = new VirtualNpcHUDComponent();
                            taskVirtualNpc.virtualNpcHUDComponent.virtualNpc = taskVirtualNpc;
                            taskVirtualNpc.virtualNpcHUDComponent.OnConstruct();

                            Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnShowNpcArrow, taskVirtualNpc.uID);
                        });
                        
                        FollowComponent followComponent = taskVirtualNpc.followComponent = PoolManager.Fetch<FollowComponent>();
                        followComponent.actor = taskVirtualNpc;
                        followComponent.Construct();

                        followComponent.Target = GameCenter.mainHero;
                        followComponent.Follow = true;
                        followComponent.KeepDistance = 1;
                    }
                    else
                    {
                        taskVirtualNpc.eVirtualNpcType = VirtualNpc.EVirtualNpcType.Common;
                        taskVirtualNpc.LoadModel(taskVirtualNpc.cSVNpcData.model, (actor) =>
                        {
                            taskVirtualNpc.AnimationComponent.SetSimpleAnimation(taskVirtualNpc.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                            taskVirtualNpc.modelGameObject.SetActive(false);
                            taskVirtualNpc.AnimationComponent.UpdateHoldingAnimations(taskVirtualNpc.cSVNpcData.action_id, Constants.UMARMEDID, null, EStateType.Idle, taskVirtualNpc.modelGameObject);
                        });

                        FollowComponent followComponent = taskVirtualNpc.followComponent = PoolManager.Fetch<FollowComponent>();
                        followComponent.actor = taskVirtualNpc;
                        followComponent.Construct();

                        followComponent.Target = npcFollowVirtualNpcs[(ulong)cSVTaskGoalData.FollowNpc[0] * 100000];
                        followComponent.Follow = true;
                    }
                }
            }
        }

        public override void Dispose()
        {
            ClearNpcFollowVirtualNpcs();

            base.Dispose();
        }
    }

    public class VirtualNPCFollowLogicComponent
    {
        public VirtualNpc VirtualNpc
        {
            get;
            set;
        }

        public Vector3 DestPos
        {
            get;
            set;
        }

        public float Distance
        {
            get;
            set;
        }

        public List<uint> Bubbles
        {
            get;
            set;
        }

        public uint TimerMax
        {
            get;
            set;
        }

        public uint TimerMin
        {
            get;
            set;
        }

        public Timer bubbleTimer;
        public bool bubbleFlag = true;

        public float lastTriggerTime = 0;
        public float cd = 1f;

        public void Dispose()
        {
            VirtualNpc = null;
            lastTriggerTime = 0;
            cd = 1f;
            Distance = 0f;
            DestPos = Vector3.zero;
            Bubbles = null;
            TimerMax = 0;
            TimerMin = 0;
            bubbleTimer?.Cancel();
            bubbleTimer = null;
            bubbleFlag = true;
        }
    }

    public class NpcFollowVirtualNpcTaskGoalDataComponent
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
}