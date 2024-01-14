using System.Collections.Generic;
using System;
using Lib.Core;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// 行为控制器///
    /// </summary>
    public class ActionCtrl : Singleton<ActionCtrl>
    {
#if DEBUG_MODE
        public int LastExecuteFrame = 0;
        public int LastExecuteTime = 0; //microsecond 微秒
        public string name;
#endif

        /// <summary>
        /// 行为控制状态///
        /// </summary>
        public enum EActionCtrlStatus
        {
            PlayerCtrl,     //玩家控制
            Auto            //自动模式
        }

        /// <summary>
        /// 控制器执行状态///
        /// </summary>
        public enum EActionStatus
        {
            Idle,       //空闲
            Ing,        //执行中
        }

        public EActionCtrlStatus actionCtrlStatus = EActionCtrlStatus.PlayerCtrl;
        public EActionStatus actionStatus = EActionStatus.Idle;

        public Queue<ActionBase> cacheAutoActions = new Queue<ActionBase>();
        public Queue<ActionBase> executeAutoActions = new Queue<ActionBase>();

        public ActionBase currentPlayerCtrlAction;
        public ActionBase currentAutoAction;
        ActionBase lastAutoAction;

        /// <summary>
        /// 控制行为系统是否执行///
        /// </summary>
        public static bool _actionExecuteLockFlag = false;
        public static bool ActionExecuteLockFlag {
            get { return _actionExecuteLockFlag; }
            set {
                _actionExecuteLockFlag = value;
                DebugUtil.LogFormat(ELogType.eTask, "_actionExecuteLockFlag: {0}", _actionExecuteLockFlag);
            }
        }

        //public ActionBase CreateAction(string fullName)
        //{
        //    ActionBase actionBase = PoolManager.Fetch(fullName) as ActionBase;
        //    //ActionBase actionBase = Logic.Core.ObjectPool<ActionBase>.Fetch(fullName);
        //    return actionBase;
        //}

        public ActionBase CreateAction(Type type)
        {
            ActionBase actionBase = PoolManager.Fetch(type) as ActionBase;
            //ActionBase actionBase = Logic.Core.ObjectPool<ActionBase>.Fetch(fullName);
            return actionBase;
        }

        public object ForceCreateAction(string typeName)
        {
            Type type = Type.GetType(typeName);
            object actionBase = Activator.CreateInstance(type);

            return actionBase;
        }

        /// <summary>
        /// 执行手动控制行为///
        /// </summary>
        /// <param name="action"></param>
        public void ExecutePlayerCtrlAction(ActionBase action)
        {
            if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain() && !Sys_Team.Instance.isPlayerLeave())
                return;

            if (ActionExecuteLockFlag || action == null)
                return;

            Reset();

            DebugUtil.Log(ELogType.eActionCtrl, $"ActionCtrl ExecutePlayerCtrlAction {action.GetType().FullName}");

            actionCtrlStatus = EActionCtrlStatus.PlayerCtrl;
            currentPlayerCtrlAction = action;
            ActionCtrl.Instance.actionStatus = ActionCtrl.EActionStatus.Ing;
            currentPlayerCtrlAction.Execute();
        }

        /// <summary>
        /// 添加自动执行行为序列///
        /// </summary>
        /// <param name="actionBases"></param>
        /// <param name="_actionSource"></param>
        public void AddAutoActions(List<ActionBase> actionBases)
        {
            if (actionBases == null || actionBases.Count <= 0)
                return;

            if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain() && !Sys_Team.Instance.isPlayerLeave())
                return;            

            Reset();
            DebugUtil.Log(ELogType.eActionCtrl, "ActionCtrl AddAutoActions");

            actionCtrlStatus = EActionCtrlStatus.Auto;

            foreach (var action in actionBases)
            {
                cacheAutoActions.Enqueue(action);
            }
        }

        /// <summary>
        /// 添加自动执行行为序列///
        /// </summary>
        /// <param name="actionBases"></param>
        /// <param name="_actionSource"></param>
        public void AddAutoAction(ActionBase actionBase)
        {
            if (actionBase == null)
                return;

            if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain() && !Sys_Team.Instance.isPlayerLeave())
                return;            

            Reset();
            DebugUtil.Log(ELogType.eActionCtrl, "ActionCtrl AddAutoAction");

            actionCtrlStatus = EActionCtrlStatus.Auto;

            cacheAutoActions.Enqueue(actionBase);
        }

        /// <summary>
        /// 重置行为控制器///
        /// </summary>
        public void Reset()
        {
            DebugUtil.Log(ELogType.eActionCtrl, "ActionCtrl Reset");
            currentPlayerCtrlAction?.Interrupt();
            currentAutoAction?.Interrupt();

            currentPlayerCtrlAction = null;
            currentAutoAction = null;
            lastAutoAction = null;

            cacheAutoActions.Clear();
            executeAutoActions.Clear();

            actionStatus = EActionStatus.Idle;
            actionCtrlStatus = EActionCtrlStatus.PlayerCtrl;
        }

        public void Update()
        {
#if DEBUG_MODE
            float t = UnityEngine.Time.realtimeSinceStartup;
#endif
            if (actionCtrlStatus == EActionCtrlStatus.PlayerCtrl)
                PlayerCtrlUpdate();
            else
                AutoUpdate();
#if DEBUG_MODE
            LastExecuteTime = (int)((UnityEngine.Time.realtimeSinceStartup - t) * 1000000);
            LastExecuteFrame = UnityEngine.Time.frameCount;
#endif
        }

        void AutoUpdate()
        {
            if (ActionExecuteLockFlag)
                return;

            if (cacheAutoActions.Count > 0)
            {
                if (executeAutoActions.Count > 0)
                {
                    lastAutoAction = executeAutoActions.ToArray()[executeAutoActions.Count - 1];
                    //if (!lastAutoAction.ChangeMap)
                    //{
                    //    executeAutoActions.Enqueue(cacheAutoActions.Dequeue());
                    //}
                }
                else
                {
                    executeAutoActions.Enqueue(cacheAutoActions.Dequeue());
                }
            }

            if (actionStatus != EActionStatus.Ing && executeAutoActions.Count > 0)
            {
                currentAutoAction = executeAutoActions.Dequeue();
                ActionCtrl.Instance.actionStatus = ActionCtrl.EActionStatus.Ing;
                currentAutoAction.AutoExecute();
            }

            if (currentAutoAction != null && actionStatus == EActionStatus.Ing)
            {
                if (currentAutoAction.IsCompleted())
                {
                    currentAutoAction.Completed();
                    currentAutoAction = null;
                    //if (executeAutoActions.Count > 0)
                    //{
                    //    executeAutoActions.Dequeue();
                    //}
                }
            }
        }

        void PlayerCtrlUpdate()
        {
            if (currentPlayerCtrlAction != null && actionStatus == EActionStatus.Ing)
            {
                if (currentPlayerCtrlAction.IsCompleted())
                {
                    currentPlayerCtrlAction.Completed();
                    currentPlayerCtrlAction = null;
                }
            }
        }

        public void InterruptCurrent()
        {
            currentPlayerCtrlAction?.Interrupt();
            lastAutoAction?.Interrupt();
            currentAutoAction?.Interrupt();

            currentPlayerCtrlAction = null;
            lastAutoAction = null;
            currentAutoAction = null;
        }

        public void CreateACollection(uint npcInfoID)
        {
            GameCenter.mPathFindControlSystem.FindNpc(npcInfoID, (pos) =>
            {
                GameCenter.mainHero.OffMount();
                List<ActionBase> actions = new List<ActionBase>();
                Npc targetNpc = null;
                ulong uid = 0;
                GameCenter.FindNearestNpc(npcInfoID, out targetNpc, out uid);
                if (targetNpc != null)
                {
                    PathFindAction pathFindAction = CreateAction(typeof(PathFindAction)) as PathFindAction;
                    if (pathFindAction != null)
                    {
                        pathFindAction.targetPos = targetNpc.transform.position + new Vector3(targetNpc.cSVNpcData.dialogueParameter[0] / 10000f, targetNpc.cSVNpcData.dialogueParameter[1] / 10000f, targetNpc.cSVNpcData.dialogueParameter[2] / 10000f); ;
                        pathFindAction.tolerance = targetNpc.cSVNpcData.InteractiveRange / 10000f;
                        actions.Add(pathFindAction);
                        pathFindAction.Init(null, () =>
                        {
                            List<ActionBase> innerActions = new List<ActionBase>();
                            InteractiveWithNPCAction interactionAction = CreateAction(typeof(InteractiveWithNPCAction)) as InteractiveWithNPCAction;
                            if (interactionAction != null)
                            {
                                interactionAction.npc = targetNpc;
                                innerActions.Add(interactionAction);
                                AddAutoActions(innerActions);
                            }
                        });
                        AddAutoActions(actions);
                    }
                }
            });
        }

        /// <summary>
        /// 寻路到一个NPC旁, 存在同类型多个NPC会找最近的///
        /// </summary>
        /// <param name="npcInfoID">InfoID</param>
        public void MoveToTargetNPC(uint npcInfoID, Action actionCompleted = null, bool interruptHangup = true)
        {
            if (interruptHangup) {
                Sys_Pet.Instance.ForceStop();
            }

            Reset();
            Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;
            GameCenter.mPathFindControlSystem.FindNpc(npcInfoID, (pos) =>
            {
                List<ActionBase> actions = new List<ActionBase>();
                Npc targetNpc = null;
                ulong uid = 0;
                GameCenter.FindNearestNpc(npcInfoID, out targetNpc, out uid);
                if (targetNpc != null)
                {
                    PathFindAction pathFindAction = CreateAction(typeof(PathFindAction)) as PathFindAction;
                    if (pathFindAction != null)
                    {
                        if (targetNpc.transform != null)
                        {
                            pathFindAction.targetPos = targetNpc.transform.position + new Vector3(targetNpc.cSVNpcData.dialogueParameter[0] / 10000f, targetNpc.cSVNpcData.dialogueParameter[1] / 10000f, targetNpc.cSVNpcData.dialogueParameter[2] / 10000f);
                        }
                        else
                        {
                            Vector3 _pos = Vector3.zero;
                            Quaternion eular = Quaternion.identity;
                            Sys_Map.Instance.GetNpcPos(Sys_Map.Instance.CurMapId, targetNpc.cSVNpcData.id, ref _pos, ref eular);
                            pathFindAction.targetPos = _pos + new Vector3(targetNpc.cSVNpcData.dialogueParameter[0] / 10000f, targetNpc.cSVNpcData.dialogueParameter[1] / 10000f, targetNpc.cSVNpcData.dialogueParameter[2] / 10000f);
                        }
                        pathFindAction.tolerance = targetNpc.cSVNpcData.InteractiveRange / 10000f;
                        actions.Add(pathFindAction);
                        Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;
                        pathFindAction.Init(null, actionCompleted);
                        AddAutoActions(actions);
                    }
                }
            });
        }

        public void MoveToTargetNPCAndInteractive(uint npcInfoID, bool forceInteractive = true, bool interruptHangup = true)
        {
            if (interruptHangup) {
                Sys_Pet.Instance.ForceStop();
            }
            
            Reset();
            Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;
            GameCenter.mPathFindControlSystem.FindNpc(npcInfoID, (pos) =>
            {
                List<ActionBase> actions = new List<ActionBase>();
                Npc targetNpc = null;
                ulong uid = 0;
                GameCenter.FindNearestNpc(npcInfoID, out targetNpc, out uid, !forceInteractive);
                if (targetNpc != null)
                {
                    PathFindAction pathFindAction = CreateAction(typeof(PathFindAction)) as PathFindAction;
                    if (pathFindAction != null)
                    {
                        if (targetNpc.transform != null)
                        {
                            pathFindAction.targetPos = targetNpc.transform.position + new Vector3(targetNpc.cSVNpcData.dialogueParameter[0] / 10000f, targetNpc.cSVNpcData.dialogueParameter[1] / 10000f, targetNpc.cSVNpcData.dialogueParameter[2] / 10000f);
                        }
                        else
                        {
                            Vector3 _pos = Vector3.zero;
                            Quaternion eular = Quaternion.identity;
                            Sys_Map.Instance.GetNpcPos(Sys_Map.Instance.CurMapId, targetNpc.cSVNpcData.id, ref _pos, ref eular);
                            pathFindAction.targetPos = _pos + new Vector3(targetNpc.cSVNpcData.dialogueParameter[0] / 10000f, targetNpc.cSVNpcData.dialogueParameter[1] / 10000f, targetNpc.cSVNpcData.dialogueParameter[2] / 10000f);
                        }
                        pathFindAction.tolerance = targetNpc.cSVNpcData.InteractiveRange / 10000f;
                        actions.Add(pathFindAction);
                        Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;
                        pathFindAction.Init(null, () =>
                        {
                            List<ActionBase> innerActions = new List<ActionBase>();
                            InteractiveWithNPCAction interactionAction = CreateAction(typeof(InteractiveWithNPCAction)) as InteractiveWithNPCAction;
                            if (interactionAction != null)
                            {
                                interactionAction.npc = targetNpc;
                                innerActions.Add(interactionAction);
                                AddAutoActions(innerActions);
                            }
                        });
                        AddAutoActions(actions);
                    }
                }
            });
        }

        public void MoveToTargetNPCAndInteractive(Npc targetNpc, bool interruptHangup = true)
        {
            Reset();
            if (targetNpc == null)
                return;

            if (interruptHangup) {
                Sys_Pet.Instance.ForceStop();
            }
            
            List<ActionBase> actions = new List<ActionBase>();

            PathFindAction pathFindAction = CreateAction(typeof(PathFindAction)) as PathFindAction;
            if (pathFindAction != null)
            {
                if (targetNpc.transform != null)
                {
                    pathFindAction.targetPos = targetNpc.transform.position + new Vector3(targetNpc.cSVNpcData.dialogueParameter[0] / 10000f, targetNpc.cSVNpcData.dialogueParameter[1] / 10000f, targetNpc.cSVNpcData.dialogueParameter[2] / 10000f);
                }
                else
                {
                    Vector3 _pos = Vector3.zero;
                    Quaternion eular = Quaternion.identity;
                    Sys_Map.Instance.GetNpcPos(Sys_Map.Instance.CurMapId, targetNpc.cSVNpcData.id, ref _pos, ref eular);
                    pathFindAction.targetPos = _pos + new Vector3(targetNpc.cSVNpcData.dialogueParameter[0] / 10000f, targetNpc.cSVNpcData.dialogueParameter[1] / 10000f, targetNpc.cSVNpcData.dialogueParameter[2] / 10000f);
                }
                pathFindAction.tolerance = targetNpc.cSVNpcData.InteractiveRange / 10000f;
                actions.Add(pathFindAction);
                Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;
                pathFindAction.Init(null, () =>
                {
                    List<ActionBase> innerActions = new List<ActionBase>();
                    InteractiveWithNPCAction interactionAction = CreateAction(typeof(InteractiveWithNPCAction)) as InteractiveWithNPCAction;
                    if (interactionAction != null)
                    {
                        interactionAction.npc = targetNpc;
                        innerActions.Add(interactionAction);
                        AddAutoActions(innerActions);
                    }
                });
                AddAutoActions(actions);
            }
        }
    }

    public class CollectionCtrl : Singleton<CollectionCtrl>
    {
        public bool CanCollection;
        public uint curNpcId;

#if DEBUG_MODE
        public int LastExecuteFrame = 0;
        public int LastExecuteTime = 0; //microsecond 微秒
        public string name;
#endif

        public enum ECollectionCtrlStatus
        {
            Idle,
            Collecting,
        }

        public ECollectionCtrlStatus status = ECollectionCtrlStatus.Idle;

        public void StartCollection(uint npcInfoID)
        {
            Sys_CollectItem.Instance.collectTimer?.Cancel();
            curNpcId = npcInfoID;

            CanCollection = true;
            status = ECollectionCtrlStatus.Collecting;
            ActionCtrl.Instance.actionCtrlStatus = ActionCtrl.EActionCtrlStatus.Auto;
            ActionCtrl.Instance.CreateACollection(curNpcId);
        }

        public void Update()
        {
#if DEBUG_MODE
            float t = UnityEngine.Time.realtimeSinceStartup;
#endif
            if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.PlayerCtrl)
            {
                CanCollection = false;
            }

            if (CanCollection && ActionCtrl.Instance.actionStatus == ActionCtrl.EActionStatus.Idle && status == ECollectionCtrlStatus.Idle)
            {
                status = ECollectionCtrlStatus.Collecting;
                ActionCtrl.Instance.CreateACollection(curNpcId);
            }
#if DEBUG_MODE
            LastExecuteTime = (int)((UnityEngine.Time.realtimeSinceStartup - t) * 1000000);
            LastExecuteFrame = UnityEngine.Time.frameCount;
#endif
        }
    }
}
