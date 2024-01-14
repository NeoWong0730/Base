using Logic.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{

    public partial class FollowComponent
    {
        class FllowMoveTo : FllowStateAction
        {
            Vector3 targetPos = Vector3.zero;

            Vector3 followTragetPos = Vector3.zero;

            NavMeshHit navMeshHit;

            bool mCanGetMoveTo = false;
            public override void OnEnter()
            {
                base.OnEnter();

                MoveTo();
            }

            public override void OnUpdate()
            {
      
                if (!mCanGetMoveTo && Vector3.Distance(FollowCom.transform.localPosition, targetPos) < 0.1)
                {
                    MoveTo();
                }
                if (followTragetPos != FollowCom.targetMovementComponent.vAimPos)
                {
                    MoveTo();
                }

              //  var speed = FollowCom.movementComponent.mNavMeshAgent.velocity;

              //  var steeringTarget = FollowCom.movementComponent.mNavMeshAgent.steeringTarget;

                var movetotargetPos = FollowCom.movementComponent.vAimPos;

               // var curpos = FollowCom.movementComponent.CurrentPos;

                //Debug.LogError(
                //    "move to state ----- > 速度---" + speed.ToString() +
                //    "------手动的终点---" + movementComponent.vAimPos.ToString() +
                //     "------导航的终点---" + NavAgent.destination.ToString() +
                //    "------导航下一步位置---" + steeringTarget.ToString() +
                //    "----当前位置 ---" + curpos.ToString() +
                //    "---- 是否还有导航路径---" + NavAgent.hasPath.ToString() +
                //    "---- 是否在计算中---" + NavAgent.pathPending.ToString());


                //if (NavAgent.enabled && IsNavActive() == false)
                //{
                //    if (Vector3.Distance(NavAgent.destination, movetotargetPos) > movementComponent.allowDistance)
                //    {
                //        NavAgent.SetDestination(movetotargetPos);
                //    }
                //    else
                //    {
                //        FollowCom.EndMove();
                //    }
                //}

                if (IsNavActive()&& Vector3.Distance(NavAgent.destination, movetotargetPos) > movementComponent.allowDistance)
                {
                    NavAgent.SetDestination(movetotargetPos);
                }


                if (IsNavWorking() == false /*|| FollowCom.targetMovementComponent.eMoveState != MovementComponent.EMoveState.MoveTo*/)
                {
                   // var diff = NavAgent.remainingDistance;
                   // var vd = movementComponent.allowDistance;

                    // Debug.LogError("目标距离 -------" + diff.ToString() + "--------- 允许距离 --" + vd.ToString());

                    FollowCom.EndMove();
                    //FollowCom.FollowState = EFllowState.Nothing;
                    return;
                }
            }

            private void MoveTo()
            {
                followTragetPos = FollowCom.targetMovementComponent.vAimPos;

                targetPos = GetTargetPos(FollowCom.targetMovementComponent.vAimPos);

                // MovementComponent.GetNavMeshHit(targetPos, out navMeshHit);

                mCanGetMoveTo = true;

                var speed = FollowCom.targetMovementComponent.mNavMeshAgent.speed;

                FollowCom.movementComponent.mNavMeshAgent.speed = speed;

                //Debug.LogError("move to state ----- > 速度---" + speed.ToString() + 
                //    " ----目标点 --" + targetPos.ToString() + 
                //    "----- 位置 -- " + FollowCom.transform.position.ToString());


                if (FollowCom.movementComponent.FollowTo(targetPos,FollowCom.KeepDistance))
                {
                   var dir = (targetPos - FollowCom.movementComponent.CurrentPos).normalized;

                    FollowCom.movementComponent.mNavMeshAgent.velocity = speed * dir;

                  //  Debug.LogError("move state ---- 导航的目的地 --- " + FollowCom.movementComponent.vAimPos.ToString());

                    return;
                }
                   

               // Debug.LogError("不能到达目的地，所以以目标人物当前点作为目标点");

                targetPos = FollowCom.targetMovementComponent.CurrentLoclPos - (FollowCom.targetMovementComponent.CurrentLoclPos - FollowCom.transform.localPosition).normalized * FollowCom.distance;

                followTragetPos = targetPos;




                mCanGetMoveTo = false;

                if (FollowCom.movementComponent.FollowTo(targetPos) == false)
                {

                    // Debug.LogError("不能到达目的地");
                    FollowCom.EndMove();
                    Controller.FollowState = EFllowState.AutoKeep;
                }


            }
            private Vector3 GetTargetPos(Vector3 followTargetPos)
            {
                Vector3 dir = (FollowCom.targetMovementComponent.CurrentLoclPos - followTargetPos).normalized;

                Vector3 pos = dir * FollowCom.KeepDistance + followTargetPos;

                return pos;
            }

            public override void OnExit()
            {
               // FollowCom.movementComponent.mNavMeshAgent.isStopped = true;
                FollowCom.Stop();

            }
        }
    }



}
