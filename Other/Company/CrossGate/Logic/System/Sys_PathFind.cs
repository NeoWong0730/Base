using System.Collections.Generic;
using Lib.Core;
using Logic.Core;

namespace Logic {
    public class Sys_PathFind : SystemModuleBase<Sys_PathFind>, ISystemModuleUpdate {
        public enum EEvents {
            OnPathFind,
        }
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public bool currentState { get; protected set; } = false;

        private EUIID _pathFindId = EUIID.UI_PathFind;
        public EUIID PathFindId {
            get { return this._pathFindId; }
            set {
                if (value != this._pathFindId) {
                    UIManager.CloseUI(this._pathFindId, true);
                    this._pathFindId = value;
                }
            }
        }

        public override void Init() {
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.BeCaptain, this.OnBeCaptain, true);
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.BeMember, this.OnBeMember, true);
        }

        public void OnUpdate() {
            // if (GameCenter.playState != GameCenter.EPlayState.BigWorld)
            //     return;

            // if (Sys_FunctionOpen.Instance.isRunning)
            //     return;

            // NavMeshAgent agent = GameCenter.mainHero?.movementComponent?.mNavMeshAgent;
            // if (agent != null)
            // {
            //     if(UnityEngine.Time.frameCount % 4 == 0)
            //     {
            //         if (!currentState && agent.enabled && agent.hasPath)
            //         {
            //             // 开始寻路
            //             currentState = true;
            //             eventEmitter.Trigger<bool>(EEvents.OnPathFind, true);
            //         }
            //         else if (currentState && (!agent.enabled || !agent.hasPath))
            //         {
            //             // 结束寻路
            //             currentState = false;
            //             eventEmitter.Trigger<bool>(EEvents.OnPathFind, false);
            //         }
            //     }
            // }
            // else
            // {
            //     UIManager.CloseUI(EUIID.UI_PathFind);
            // }
        }
        public override void OnLogout() {
            UIManager.CloseUI(this.PathFindId);
            base.OnLogout();
        }

        public void CloseUI() {
            UIManager.CloseUI(this.PathFindId);
        }

        #region 事件
        private void OnBeCaptain() {
            UIManager.CloseUI(this.PathFindId, true);
        }
        private void OnBeMember() {
            UIManager.CloseUI(this.PathFindId, true);
        }

        #endregion

        public void DoPatrolFind(uint mapId, uint teamId) {
            DebugUtil.LogFormat(ELogType.eTask, "DoPatrolFind: {0} {1}", mapId, teamId);
            List<ActionBase> actions = new List<ActionBase>();
            WaitSecondsAction waitSecondsAction = ActionCtrl.Instance.CreateAction(typeof(WaitSecondsAction)) as WaitSecondsAction;
            if (waitSecondsAction != null) {
                waitSecondsAction.Init(null, () => {
                    DebugUtil.LogFormat(ELogType.eTask, "DoPatrolFind 去地图{0} 杀敌组{1}", mapId, teamId);
                    {
                        Sys_PathFind.Instance.PathFindId = EUIID.UI_Patrol;

                        GameCenter.mPathFindControlSystem?.FindMonsterTeamId(teamId, mapId, (pos) => {
                            DebugUtil.LogFormat(ELogType.eTask, "DoPatrolFind 寻路已到达目标怪物 Over!");
                            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal) {
                                // 如果查找不到目标怪物，则继续寻路
                                this.DoPatrolFind(mapId, teamId);
                            }
                        });
                    }
                });
                actions.Add(waitSecondsAction);
            }
            else {
                DebugUtil.LogFormat(ELogType.eTask, "waitSecondsAction == null");
            }
            ActionCtrl.Instance.AddAutoActions(actions);
        }
    }
}
