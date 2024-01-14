using Logic.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{
  
    public partial class FollowComponent : Logic.Core.Component
    {
        class FllowMoveBy : FllowStateAction
        {
            private Vector3 mLastTargetPos = Vector3.zero;

            private Vector3 mLastPos = Vector3.zero;


            private float mOffsetDistance = 0f;
            public override void OnUpdate()
            {
                UpdateFollowMoveBy();
            }

            private void UpdateFollowMoveBy()
            {
                if (!IsCanGotoForward())
                {
                    Controller.FollowState = EFllowState.AutoMoveBy;
                    return;
                }

                mOffsetDistance = Vector3.Distance(FollowCom.transform.localPosition, FollowCom.targetMovementComponent.CurrentLoclPos);

                Vector3 speed0 = GetSpeedByTargetMoveBy();


                mLastPos = FollowCom.movementComponent.CurrentLoclPos;

                if (!FollowCom.PetFollow)
                {
                    FollowCom.movementComponent.FollowToMoveBy(speed0);
                }
                else
                {
                    FollowCom.movementComponent.FollowToMoveBy(speed0, true, FollowCom.targetMovementComponent.sceneActor.transform);
                }
                mLastTargetPos = FollowCom.targetMovementComponent.CurrentLoclPos;

            }

            private Vector3 GetSpeedByTargetMoveBy()
            {

                float speed1 = FollowCom.targetMovementComponent.mNavMeshAngentMoveBy.Velocity.magnitude;

                speed1 = speed1 == 0 ? FollowCom.mDefaultSpeed : speed1;

                float speed = speed1;

                if (mOffsetDistance > 0)
                    speed = (mOffsetDistance / FollowCom.KeepDistance) * speed1;

                speed = Mathf.Clamp(speed, speed1 * 0.1f, speed1 * 1.1f);

                Vector3 dir = DirNoramlToTargeter;
                dir.y = 0;

                Vector3 speed0 = dir * speed;

              //  Debug.LogError("move by speed : " + speed + "   dir " + dir);

                return speed0;
            }

            private bool IsCanGotoForward()
            {
                if (NavAgent.enabled && mLastPos == FollowCom.movementComponent.CurrentLoclPos)
                {
                    UnityEngine.AI.NavMeshHit hit;
                    bool result = FollowCom.movementComponent.mNavMeshAgent.Raycast(FollowCom.targetMovementComponent.CurrentLoclPos, out hit);

                    return !result;

                }
                return true;
            }

            public override void OnExit()
            {
                FollowCom.movementComponent.mNavMeshAngentMoveBy.enabled = false;
            }
        }
    }

 

}
