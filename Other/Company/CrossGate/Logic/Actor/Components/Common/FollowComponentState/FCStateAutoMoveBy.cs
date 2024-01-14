using Logic.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{

    public partial class FollowComponent : Logic.Core.Component
    {
        class FllowAutoMoveBy : FllowAutoKeep
        {
            private Vector3 mLastPos;
            public override void OnUpdate()
            {
                mLastPos = FollowCom.movementComponent.CurrentLoclPos;

                if (IsCanGotoForward())
                {
                    Controller.FollowState = EFllowState.MoveBy;
                    return;
                }

                base.OnUpdate();


            }

            private bool IsCanGotoForward()
            {
                //if (mLastPos = FollowCom.movementComponent.CurrentLoclPos)
                {
                    
                    UnityEngine.AI.NavMeshHit hit;

                    bool result = NavMesh.Raycast(Position, TageterPosition, out hit, NavMesh.AllAreas);
                    // FollowCom.movementComponent.mNavMeshAgent.Raycast(FollowCom.targetMovementComponent.CurrentLoclPos, out hit);

                    return !result;

                }
                //return true;
            }

            public override void OnExit()
            {
                base.OnExit();
            }
        }
    }


}
