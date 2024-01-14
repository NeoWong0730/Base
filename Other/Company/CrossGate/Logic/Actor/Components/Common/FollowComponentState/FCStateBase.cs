using Logic.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{
  
    public partial class FollowComponent : Logic.Core.Component
    {
        public enum EFllowState
        {
            None,
            MoveBy,
            AutoKeep,
            AutoMoveBy,
            MoveTo,
            Nothing,
        }

        private FollowStateController mFollowStateController = new FollowStateController();
        public class FollowStateController
        {
            private EFllowState mState = EFllowState.None;

            public FollowComponent FollowCom { get; set; }
            public EFllowState FollowState
            {
                get { return mState; }
                set
                {

                    if (mState == value)
                        return;
                    //Debug.LogError(movementComponent.sceneActor.UID.ToString() + "   change state : from  " + mState + "  to  " + value);
                    ChangeState(value);
                    mState = value;

                }
            }


            private FllowStateAction FollowAction = null;


            private FllowMoveBy mFllowMoveBy = new FllowMoveBy();
            private FllowAutoKeep mFllowAutoKeep = new FllowAutoKeep();
            private FllowAutoMoveBy mFllowAutoMoveBy = new FllowAutoMoveBy();
            private FllowMoveTo mFllowMoveTo = new FllowMoveTo();
            private FllowNothing mFllowNothing = new FllowNothing();
            private void ChangeState(EFllowState eFllowState)
            {

               // Debug.LogError("follow  change state :  " + FollowState + " to ---->" + eFllowState);

                if (FollowAction != null)
                    FollowAction.OnExit();

                switch (eFllowState)
                {
                    case EFllowState.AutoKeep:
                        FollowAction = mFllowAutoKeep;
                        break;
                    case EFllowState.AutoMoveBy:
                        FollowAction = mFllowAutoMoveBy;
                        break;
                    case EFllowState.MoveBy:
                        FollowAction = mFllowMoveBy;
                        break;
                    case EFllowState.MoveTo:
                        FollowAction = mFllowMoveTo;
                        break;
                    case EFllowState.Nothing:
                        FollowAction = mFllowNothing;
                        break;
                    case EFllowState.None:
                        FollowAction = null;
                        break;
                }

                if (FollowAction != null)
                {
                    FollowAction.OnEnter();
                }

            }

            public void OnConstruct()
            {
                mFllowMoveBy.Controller = this;
                mFllowAutoKeep.Controller = this;
                mFllowAutoMoveBy.Controller = this;
                mFllowMoveTo.Controller = this;
                mFllowNothing.Controller = this;


                mFllowMoveBy.FollowCom = FollowCom;
                mFllowAutoKeep.FollowCom = FollowCom;
                mFllowAutoMoveBy.FollowCom = FollowCom;
                mFllowMoveTo.FollowCom = FollowCom;
                mFllowNothing.FollowCom = FollowCom;
            }
            public void OnUpdata()
            {
                if (FollowState == EFllowState.None && FollowCom.GetDistanceToTarget() >= (FollowCom.KeepDistance + FollowCom.mAllowDistance))
                {                   
                    if (FollowCom.targetMovementComponent.eMoveState == MovementComponent.EMoveState.MoveBy)
                        FollowState = EFllowState.MoveBy;
                    else if (FollowCom.targetMovementComponent.eMoveState == MovementComponent.EMoveState.MoveTo)
                        FollowState = EFllowState.MoveTo;
                    else
                        FollowState = EFllowState.AutoKeep;
                }

                if (FollowCom.PetFollow)
                {
                    if (FollowCom.targetMovementComponent.eMoveState == MovementComponent.EMoveState.MoveBy)
                        FollowState = EFllowState.MoveBy;
                    else if (FollowCom.targetMovementComponent.eMoveState == MovementComponent.EMoveState.MoveTo)
                        FollowState = EFllowState.MoveTo;
                    else
                        FollowState = EFllowState.AutoKeep;
                }

                if (FollowAction != null)
                    FollowAction.OnUpdate();
            }


            public void SetState(EFllowState state, bool isImi = false)
            {
                if (isImi)
                {
                    FollowState = state;
                    return;
                }

                if (FollowState == EFllowState.MoveTo)
                    return;

                FollowState = state;
            }
        }


      
    }
    public partial class FollowComponent : Logic.Core.Component
    {
        class FllowStateAction
        {
            public FollowComponent FollowCom { get; set; }

            public FollowStateController Controller { get; set; }

            protected MovementComponent TargeterMoveComponent {

                get {
                    return FollowCom.targetMovementComponent;
                }
            }


            public MovementComponent movementComponent { get {
                    if (FollowCom == null)
                        return null;

                    return FollowCom.movementComponent;
                } }

            public NavMeshAgent NavAgent { get {

                    if (movementComponent == null)
                        return null;

                    return movementComponent.mNavMeshAgent;
                } }


            protected Vector3 TageterPosition {

                get {

                    return TargeterMoveComponent.CurrentPos;
                }
            }

            protected Vector3 Position {

                get {
                    return movementComponent.CurrentPos;
                }
            }


            protected float DistanceToTargeter {
                get {
                    return Vector3.Distance(Position, TageterPosition);
                }
            }

            protected Vector3 DirNoramlToTargeter {

                get {

                    return (TageterPosition - Position).normalized;
                }
            }
            public virtual void OnUpdate() { }
            public virtual void OnEnter() { }

            public virtual void OnExit() { }

            protected bool IsNavActive()
            {
                if (FollowCom == null || FollowCom.movementComponent == null || FollowCom.movementComponent.mNavMeshAgent == null || NavAgent.enabled == false)
                    return false;

                return true;
            }
            protected bool IsNavWorking()
            {
                if (IsNavActive() == false)
                    return false;

                //Debug.LogError("NAVActive 距离导航目标距离 ---- " + FollowCom.movementComponent.mNavMeshAgent.remainingDistance.ToString() +
                //    "----- 允许的的距离差距---" + FollowCom.movementComponent.allowDistance.ToString() +
                //   "------导航正在计算路径-----" + FollowCom.movementComponent.mNavMeshAgent.pathPending.ToString());
                
                if (FollowCom.movementComponent.mNavMeshAgent.remainingDistance < FollowCom.mAllowDistance&&
                    FollowCom.movementComponent.mNavMeshAgent.pathPending == false)
                    return false;

                return true;
            }


            protected Vector3 GetSpeedByTargetMoveBy(float minScale = 0.1f, float maxScale = 1.1f)
            {
                float speed1 = FollowCom.targetMovementComponent.mNavMeshAngentMoveBy.Velocity.magnitude;

                speed1 = speed1 == 0 ? FollowCom.mDefaultSpeed : speed1;

                float speed = speed1;

                float distance = DistanceToTargeter;

                if (distance > 0)
                    speed = (distance / FollowCom.KeepDistance) * speed1;

                speed = Mathf.Clamp(speed, speed1 * minScale, speed1 * maxScale);

                Vector3 dir = DirNoramlToTargeter;
                dir.y = 0;

                Vector3 speed0 = dir * speed;

                return speed0;
            }

            /// <summary>
            /// 根据跟随者的当前位置，且保持距离，获得目标点
            /// </summary>
            /// <returns></returns>
            protected Vector3 GetNextTargetPostion()
            {
               Vector3 postion = TageterPosition - FollowCom.KeepDistance * DirNoramlToTargeter;

                return postion;
            }
        }
    }
   

}
