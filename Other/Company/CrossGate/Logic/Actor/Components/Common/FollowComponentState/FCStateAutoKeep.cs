using Logic.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{
  
    public partial class FollowComponent : Logic.Core.Component
    {
        class FllowAutoKeep : FllowStateAction
        {

            private Vector3 m_LastPosition;

            protected Vector3 mTargetPos = Vector3.positiveInfinity;

            protected Vector3 mFollowerAniPos = Vector3.positiveInfinity;

            public override void OnEnter()
            {
                base.OnEnter();

                DoFindPath();
            }
            public override void OnUpdate()
            {
                base.OnUpdate();

                UpdateAutoKeep();
            }
            private void UpdateAutoKeep()
            {
                //float offsetDistance = DistanceToTargeter;


                //var speed = FollowCom.movementComponent.mNavMeshAgent.velocity;

                //var steeringTarget = FollowCom.movementComponent.mNavMeshAgent.steeringTarget;

                //var movetotargetPos = FollowCom.movementComponent.vAimPos;

                //var curpos = FollowCom.movementComponent.CurrentPos;

                //Debug.LogError(
                //    "auto keep state ----- > 速度---" + speed.ToString() +
                //    "------手动的终点---" + movementComponent.vAimPos.ToString() +
                //     "------导航的终点---" + NavAgent.destination.ToString() +
                //    "------导航下一步位置---" + steeringTarget.ToString() +
                //    "----当前位置 ---" + curpos.ToString() +
                //    "---- 是否还有导航路径---" + NavAgent.hasPath.ToString() +
                //    "---- 是否在计算中---" + NavAgent.pathPending.ToString());

                if (IsNavWorking() && Vector3.Distance(Position, NavAgent.steeringTarget) < FollowCom.mAllowDistance)
                {
                    if (DoFindPath() == false)
                        OverFind();
                }

                if (IsNavWorking() == false && DistanceToTargeter > (FollowCom.distance + FollowCom.mAllowDistance))
                {
                    DoFindPath();
                }
                //else if (TageterPosition != Vector3.positiveInfinity && Vector3.Distance(TageterPosition, mFollowerAniPos) > (FollowCom.distance + FollowCom.mAllowDistance))
                //{
                //    DoFindPath();
                //}
                else if (IsNavWorking() == false && FollowCom.targetMovementComponent.eMoveState == MovementComponent.EMoveState.Idle)
                {
                    OverFind();
                }
            }


            private bool DoFindPath()
            {
               // Debug.LogError(" auto keep  find path ---  " + DistanceToTargeter.ToString() + " ------ 保持距离----" + FollowCom.KeepDistance);

                if (DistanceToTargeter <= FollowCom.KeepDistance + FollowCom.mAllowDistance)
                    return false;

                Vector3 targetPos = GetNextTargetPostion();

                m_LastPosition = FollowCom.transform.localPosition;

                float speed = GetSpeedByTargetMoveBy().magnitude;

                //Debug.LogError(" auto keep  find path --- 最大速度---- " + speed.ToString());

                FollowCom.movementComponent.mNavMeshAgent.speed = speed;

                FollowCom.movementComponent.FollowTo(targetPos, 1);

                mTargetPos = targetPos;

                mFollowerAniPos = TageterPosition;

                return true;
            }


            private void OverFind()
            {
                FollowCom.EndMove();

                m_LastPosition = Vector3.positiveInfinity;
            }
            public override void OnExit()
            {
                mTargetPos = Vector3.positiveInfinity;
                mFollowerAniPos = Vector3.positiveInfinity;
            }
        }
    }



}
