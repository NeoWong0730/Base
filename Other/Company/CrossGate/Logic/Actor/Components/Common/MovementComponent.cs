using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using Lib.Core;
using Logic.Core;

namespace Logic
{
    public interface IMovementComponent
    {
        MovementComponent GetMovementComponent();
    }

    public class MovementComponent : Logic.Core.Component//, IUpdateCmd
    {
        public enum EMoveState
        {
            Idle,
            MoveBy,
            MoveTo,
            Follow
        }

        public const float defaultAllowDistance = 0.08f;
        public float allowDistance = defaultAllowDistance;

        float _fMoveSpeed;
        public float fMoveSpeed
        {
            get
            {
                return _fMoveSpeed;
            }
            set
            {
                _fMoveSpeed = value;
                if (mNavMeshAgent != null)
                {
                    mNavMeshAgent.speed = value;
                }
            }
        }

        //移动的倍率根据遥感的力度 取值范围 0 - 1
        Vector3 moveVelocity;

        private EMoveState mEMoveState = EMoveState.Idle;
        public EMoveState eMoveState { get { return mEMoveState; } set {

                if (mEMoveState == value)
                    return;

                mEMoveState = value;

                MoveStateChange?.Invoke(mEMoveState);
            } }

        public Vector3 vAimPos = Vector2.zero;
        public SceneActor sceneActor;
        public Transform transform { get; private set; }

        public NavMeshAgent mNavMeshAgent;
        public NavMeshAngentMoveBy mNavMeshAngentMoveBy;
        public StateComponent stateComponent;
        
        Action endMoveTo;
        Action moveToSuccess;
        public Action EndMove;

        public Action<EMoveState> MoveStateChange;
        public Vector3 CurrentLoclPos
        {
            get
            {
                return transform.localPosition;
            }
        }

        public Vector3 CurrentPos
        {
            get
            {
                return transform.position;
            }
        }

        public bool enableflag;
        protected override void OnConstruct()
        {
            enableflag = true;
            sceneActor = actor as SceneActor;
            transform = sceneActor.transform;
            //stateComponent = World.GetComponent<StateComponent>(actor);            
            //stateComponent = ((SceneActor)actor).stateComponent;

            //fMoveSpeed = Sys_Attr.Instance.pkAttrs[101] / 10000f;         
        }

        public void InitNavMeshAgent()
        {
            mNavMeshAgent = sceneActor.gameObject.GetNeedComponent<NavMeshAgent>();
            mNavMeshAgent.speed = fMoveSpeed;
            mNavMeshAgent.angularSpeed = 360f;
            mNavMeshAgent.acceleration = 100;
            mNavMeshAgent.autoBraking = true;
            mNavMeshAgent.radius = 0.4f;
            mNavMeshAgent.height = 1.6f;
            mNavMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            mNavMeshAgent.avoidancePriority = 50;
            mNavMeshAgent.updateRotation = true;

            mNavMeshAgent.enabled = false;

            mNavMeshAngentMoveBy = sceneActor.gameObject.GetNeedComponent<NavMeshAngentMoveBy>();
            mNavMeshAngentMoveBy.enabled = false;
        }

        protected override void OnDispose()
        {
            sceneActor = null;
            transform = null;
            fMoveSpeed = 0f;
            stateComponent = null;            
            allowDistance = defaultAllowDistance;
            moveVelocity = Vector3.zero;
            eMoveState = EMoveState.Idle;
            vAimPos = Vector2.zero;
            mNavMeshAgent = null;
            mNavMeshAngentMoveBy = null;
            endMoveTo = null;
            moveToSuccess = null;
            EndMove = null;
            enableflag = false;

            base.OnDispose();
        }

        public static void GetNavMeshHit(Vector3 pos, out NavMeshHit navMeshHit, float maxDistance = 10f)
        {
            bool result = NavMesh.SamplePosition(pos, out navMeshHit, maxDistance, NavMesh.AllAreas);

            //Debug.LogError("NavMeshHit------ Pos--- " + pos.ToString() + "---- 最大距离 ---" + maxDistance.ToString()
            //    + "----结果 ----" + result.ToString() + "----- 位置 ----- " + navMeshHit.position.ToString() + "----- 是否点击到-----" + navMeshHit.hit.ToString()
            //    + "---- " + navMeshHit.distance.ToString());
        }

        public void MoveTo(Vector3 pos, Action<Vector3> StartMoveTo = null, Action EndMoveTo = null, Action MoveToSuccess = null, float rAllowDistance = defaultAllowDistance)
        {
            if (enableflag == false)
                return;

            if (EMoveState.Follow == eMoveState)
                return;

            moveToSuccess = null;
            mNavMeshAngentMoveBy.enabled = false;
            mNavMeshAgent.enabled = true;

            if (!mNavMeshAgent.isOnNavMesh)
            {
                return;
            }

            allowDistance = rAllowDistance;
            GetNavMeshHit(pos, out NavMeshHit navMeshHit);
            vAimPos = navMeshHit.position;

            if (navMeshHit.hit && Vector3.SqrMagnitude(transform.position - vAimPos) >= allowDistance)
            {
                mNavMeshAgent.SetDestination(vAimPos);

                //DebugUtil.LogError("导航的目的地位置 ------" + vAimPos.ToString());
                eMoveState = EMoveState.MoveTo;
                stateComponent?.ChangeState(EStateType.Run);

                StartMoveTo?.Invoke(vAimPos);
                StartMoveTo = null;
                endMoveTo = EndMoveTo;
                moveToSuccess = MoveToSuccess;
            }
            else
            {
                eMoveState = EMoveState.Idle;
                stateComponent?.ChangeState(EStateType.Idle);

                //if (mNavMeshAgent.enabled && mNavMeshAgent.isOnNavMesh)
                //    mNavMeshAgent.isStopped = true;
                mNavMeshAgent.enabled = false;
                DebugUtil.Log(ELogType.eTask, "MoveTo " + allowDistance.ToString());
            }
        }

        public bool FollowTo(Vector3 pos, float rAllowDistance = defaultAllowDistance)
        {
            if (enableflag == false)
                return false;

            moveToSuccess = null;

            DebugUtil.LogFormat(ELogType.eTask, "MoveTo before {0} enabled: {1}", allowDistance.ToString(), mNavMeshAgent.enabled.ToString());
            if (mNavMeshAgent.enabled == false)
               mNavMeshAgent.enabled = true;

            if (EMoveState.MoveBy == eMoveState || !mNavMeshAgent.isOnNavMesh)
            {
                return false;
            }

            allowDistance = rAllowDistance;
            GetNavMeshHit(pos, out NavMeshHit navMeshHit, allowDistance);
            vAimPos = navMeshHit.position;


            if (navMeshHit.hit)
            {
               bool result =  mNavMeshAgent.SetDestination(vAimPos);

              //  Debug.LogError("导航的结果------" + result.ToString() + "----- 执行的导航点---- " + vAimPos.ToString());
                eMoveState = EMoveState.Follow;
                stateComponent?.ChangeState(EStateType.Run);
            }
            else
            {
                eMoveState = EMoveState.Idle;
                stateComponent?.ChangeState(EStateType.Idle);

                //if (mNavMeshAgent.enabled && mNavMeshAgent.isOnNavMesh)
                    //mNavMeshAgent.isStopped = true;
                mNavMeshAgent.enabled = false;
                DebugUtil.Log(ELogType.eTask, "FollowTo " + pos.ToString());
            }

            return navMeshHit.hit;
        }

        public void FollowToMoveBy(Vector3 speed, bool PetFollow = false, Transform targetTransform = null)
        {
            if (enableflag == false)
                return;
            moveToSuccess = null;
            mNavMeshAgent.enabled = true;

            if (EMoveState.MoveBy == eMoveState || !mNavMeshAgent.isOnNavMesh)
            {
                return;
            }
            moveVelocity = speed;

            eMoveState = EMoveState.Follow;

            if (!PetFollow)
            {
                transform.forward = moveVelocity.normalized;
                mNavMeshAngentMoveBy.Velocity = moveVelocity;
            }
            else
            {

                mNavMeshAgent.SetDestination(targetTransform.position);
                mNavMeshAngentMoveBy.Velocity = moveVelocity / 12;
            }

            mNavMeshAngentMoveBy.enabled = true;
            mNavMeshAgent.enabled = true;

            stateComponent?.ChangeState(EStateType.Run);
        }



        public void FollowToMove(float speed,float keepdistance,Transform targetTrans, 
            Func<bool> canFollow = null,
            Action onFollow = null,
            Action stopFollow = null)
        {
            if (enableflag == false)
                return;

            moveToSuccess = null;
            mNavMeshAgent.enabled = true;

            if (EMoveState.MoveBy == eMoveState || !mNavMeshAgent.isOnNavMesh)
            {
                return;
            }

            eMoveState = EMoveState.Follow;

            mNavMeshAngentMoveBy.SetTarget(speed, keepdistance, targetTrans, canFollow, onFollow, stopFollow);

            mNavMeshAngentMoveBy.enabled = true;
            mNavMeshAgent.enabled = true;

            stateComponent?.ChangeState(EStateType.Run);
        }
        public void Stop(bool InvokeMoveSuccess = true)
        {
            moveVelocity = Vector3.zero;

            if (mNavMeshAgent != null)
                mNavMeshAgent.enabled = false;
            DebugUtil.Log(ELogType.eTask, "Stop: " + InvokeMoveSuccess.ToString());    

            if (mNavMeshAngentMoveBy != null)
            {
                mNavMeshAngentMoveBy.IsHaveTarget = false;
                mNavMeshAngentMoveBy.Velocity = Vector3.zero;
                mNavMeshAngentMoveBy.enabled = false;
            }
              
            eMoveState = EMoveState.Idle;
            stateComponent?.ChangeState(EStateType.Idle);
            EndMove?.Invoke();
            endMoveTo = null;
            if (InvokeMoveSuccess)
                moveToSuccess?.Invoke();
            moveToSuccess = null;
        }

        public void MoveBy(Vector2 dirNormal, float dirPower)
        {
            if (enableflag == false)
                return;

            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Interactive)
                return;

            ActionCtrl.Instance.actionCtrlStatus = ActionCtrl.EActionCtrlStatus.PlayerCtrl;

            if(eMoveState == EMoveState.MoveTo || eMoveState == EMoveState.Follow)
            {
                Stop();
            }

            if (dirPower == 0)
            {
                moveVelocity = Vector3.zero;
                mNavMeshAngentMoveBy.Velocity = Vector3.zero;
                mNavMeshAngentMoveBy.enabled = false;

                eMoveState = EMoveState.Idle;
                stateComponent?.ChangeState(EStateType.Idle);
                EndMove?.Invoke();
                EndMove = null;
            }
            else
            {
                Vector3 dir = new Vector3(dirNormal.x, 0, dirNormal.y);
                moveVelocity = dir * fMoveSpeed;

                eMoveState = EMoveState.MoveBy;

                transform.forward = dir;
                mNavMeshAngentMoveBy.Velocity = moveVelocity;
                mNavMeshAngentMoveBy.enabled = true;
                mNavMeshAgent.enabled = true;

                stateComponent?.ChangeState(EStateType.Run);
            }

            endMoveTo?.Invoke();
            endMoveTo = null;
            moveToSuccess = null;
        }

        public void TransformToPosImmediately(Vector3 pos)
        {
            mNavMeshAgent.enabled = true;

            NavMeshHit navMeshHit;
            Vector3 hitPos = pos;
            GetNavMeshHit(hitPos, out navMeshHit);
            if (navMeshHit.hit)
                mNavMeshAgent.Warp(navMeshHit.position);
            else
                mNavMeshAgent.Warp(pos);
        }

        private float CaclSpeedByDistance(float distance)
        {
            float speed = fMoveSpeed;

            if (allowDistance > 0)
                speed = (distance / allowDistance) * fMoveSpeed;

            speed = Mathf.Clamp(speed, fMoveSpeed*0.3f, fMoveSpeed*1.3f);

           // Debug.LogError(" follow speed : " + speed + "," + fMoveSpeed + "," + distance + "," + allowDistance);

            return speed;
        }

        public void Update()
        {
            if (stateComponent != null && stateComponent.CurrentState != EStateType.Run)
                return;

            if (eMoveState == EMoveState.MoveTo)
            {
                if (!mNavMeshAgent.pathPending && mNavMeshAgent.remainingDistance< allowDistance)
                {
                    Stop();
                }
            }

            else if (eMoveState == EMoveState.Follow)
            {
                //float distance = Vector3.SqrMagnitude(transform.position - vAimPos);
                //float speed = CaclSpeedByDistance(distance);

                //mNavMeshAgent.speed = speed;

                ////mNavMeshAgent.velocity = mNavMeshAgent.velocity.normalized*speed;
                ////mNavMeshAgent.acceleration = speed * 2;

                //if (speed < 0.01f)
                //{
                //    Stop();
                //}
            }
        }
    }
}
