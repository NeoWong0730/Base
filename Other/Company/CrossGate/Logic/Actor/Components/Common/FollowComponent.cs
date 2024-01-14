using Logic.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{
    public partial class FollowComponent : Logic.Core.Component
    {
        public bool PetFollow
        {
            get;
            set;
        } = false;

        public bool NeedFollow;
        private float distance = 0.5f;
        public float KeepDistance { get { return distance; } set { distance = value; } }

        private SceneActor m_Target = null;

        private float mAllowDistance = 0.01f;
        public SceneActor Target
        {
            get { return m_Target; }
            set { SetTarget(value); }
        }

        private MovementComponent _targetMovementComponent = null;
        public MovementComponent targetMovementComponent
        {
            get
            {
                if (_targetMovementComponent == null)
                {
                    IMovementComponent sceneActor = Target as IMovementComponent;
                    if (sceneActor != null)
                    {
                        _targetMovementComponent = sceneActor.GetMovementComponent();
                        _targetMovementComponent.EndMove += OnTragetEndMove;

                        //Debug.LogError("target id : " + _targetMovementComponent.transform.name.ToString() + " id :" + transform.gameObject.name.ToString());
                    }
                }
                return _targetMovementComponent;
            }
        }

        public Transform transform { get; private set; }

        public MovementComponent movementComponent { get; private set; }

        private bool m_bFollow = true;
        public bool Follow { get { return m_bFollow; } set { m_bFollow = value; } }

        private float mDefaultSpeed = 0;
        private float mDefaultAcceleration = 0;

        public int Index = -1;
        protected override void OnConstruct()
        {
            base.OnConstruct();
            SceneActor sceneActor = actor as SceneActor;

            m_bFollow = false;
            m_Target = null;
            _targetMovementComponent = null;

            if (actor != null)
            {
                transform = sceneActor.transform;
                //movementComponent = sceneActor.movementComponent;
                IMovementComponent movementInterface = sceneActor as IMovementComponent;
                movementComponent = movementInterface.GetMovementComponent();
                mDefaultSpeed = movementComponent.fMoveSpeed;
                //mDefaultAcceleration = movementComponent.mNavMeshAgent.acceleration;

                mFollowStateController.FollowCom = this;

                mFollowStateController.OnConstruct();
            }

        }

        protected override void OnDispose()
        {
            movementComponent = null;
            transform = null;
            if (_targetMovementComponent != null)
            {
                _targetMovementComponent.EndMove -= OnTragetEndMove;
            }
            _targetMovementComponent = null;


            base.OnDispose();
        }



        struct runrecord
        {
            public Vector3 position;
            public Vector3 targetposition;

            public Vector3 dir;
            public float TimePoint;
        }

        Queue<runrecord> speedlist = new Queue<runrecord>();

        bool isNaving = false;
        Vector3 StartNavPosition = Vector3.zero;

        private void RemenberRecord(Vector3 offset)
        {
            var curtime = Time.realtimeSinceStartup;

            runrecord record;
            record.position = movementComponent.CurrentPos;
            record.targetposition = targetMovementComponent.CurrentPos;
            record.dir = offset.normalized;
            record.TimePoint = Time.realtimeSinceStartup;
            speedlist.Enqueue(record);

        }

        private void RemoveVaileRecord()
        {
            if (speedlist.Count == 0)
                return;

            var curtime = Time.realtimeSinceStartup;
            var fristvalue = speedlist.Peek();
            if (curtime - fristvalue.TimePoint > 0.5f)
            {
                while (speedlist.Count > 0)
                {
                    fristvalue = speedlist.Peek();

                    if (curtime - fristvalue.TimePoint > 0.5f)
                    {
                        speedlist.Dequeue();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }


        private bool CheckBlocked()
        {
            var curtime = Time.realtimeSinceStartup;

            var fristvalue = speedlist.Peek();

            if (curtime - fristvalue.TimePoint > 0.5f)
            {

                int count = speedlist.Count;

                float total = 0;
                foreach (var item in speedlist)
                {
                    total += (item.position - movementComponent.CurrentPos).magnitude;
                }

                if (total < 0.01f)
                {
                    return true;
                }
            }

            return false;
        }
        public void Update()
        {
            if (!m_bFollow || targetMovementComponent == null)
            {
                return;
            }

            if (movementComponent != null && movementComponent.mNavMeshAgent != null)
                mDefaultAcceleration = movementComponent.mNavMeshAgent.acceleration;

            Vector3 offset = targetMovementComponent.CurrentPos - movementComponent.CurrentPos;

            if (offset.magnitude > KeepDistance)
            {
                RemenberRecord(offset);

                //被阻挡了，走不到目标点,然后开启寻路
                if (!isNaving && CheckBlocked())
                {
                    EndMove();

                    isNaving = true;

                    StartNavPosition = targetMovementComponent.CurrentPos;

                    movementComponent.FollowTo(targetMovementComponent.CurrentPos);
                }

                RemoveVaileRecord();

                if (!isNaving)
                {
                    movementComponent.FollowToMove(targetMovementComponent.fMoveSpeed, KeepDistance, targetMovementComponent.transform);
                }

                if (isNaving)
                {
                    var changeoffset = targetMovementComponent.CurrentPos - StartNavPosition;

                    UnityEngine.AI.NavMeshHit hit;
                    bool result = NavMesh.Raycast(movementComponent.CurrentPos, targetMovementComponent.CurrentPos, out hit, NavMesh.AllAreas);

                    var hitmagnitude = Vector3.Magnitude(movementComponent.CurrentPos - hit.position);

                    if (result == false || result && hit.mask != 0)
                    {
                        EndMove();
                        return;
                    }

                   // Debug.LogError(Index.ToString() + " ++++ ray cast  " + result.ToString() + hitmagnitude.ToString() + " mask " + hit.mask);


                    //寻路过程中目标移动了一定距离重新导航
                    if (changeoffset.magnitude > KeepDistance)
                    {
                        StartNavPosition = targetMovementComponent.CurrentPos;

                        movementComponent.FollowTo(targetMovementComponent.CurrentPos);
                    }
                }


            }
            else if (offset.magnitude <= KeepDistance && movementComponent.eMoveState == MovementComponent.EMoveState.Follow &&
                targetMovementComponent.eMoveState == MovementComponent.EMoveState.Idle)
            {
                EndMove();
                speedlist.Clear();
            }

        }



        private float GetDistanceToTarget()
        {
            if (targetMovementComponent == null)
                return 0;

            float offsetDistance = Vector3.Distance(transform.localPosition, targetMovementComponent.CurrentLoclPos);

            return offsetDistance;
        }

        private void OnTragetEndMove()
        {

        }
        void EndMove()
        {
            movementComponent.Stop();

            NeedFollow = false;

           

            isNaving = false;

          //  Debug.LogError(Index.ToString() + " ------- end move");
        }

        public void Stop()
        {
            movementComponent.Stop();

            NeedFollow = false;
        }

        private void SetTarget(SceneActor target)
        {
            if (target != null && target == m_Target)
                return;

            m_Target = target;

            if (target != null)
            {
                var hero = target.gameObject;
                var ownhero = transform;

               // Debug.LogError("target id : " + hero.name.ToString() + " id :" + ownhero.gameObject.name.ToString());
            }
            if (_targetMovementComponent != null)
                _targetMovementComponent.EndMove -= OnTragetEndMove;

            if (m_Target == null)
            {
                _targetMovementComponent = null;

                EndMove();
                return;
            }

            IMovementComponent movementInterface = m_Target as IMovementComponent;
            if (movementInterface != null)
            {
                _targetMovementComponent = movementInterface.GetMovementComponent();
                _targetMovementComponent.EndMove += OnTragetEndMove;
            }
        }


        private void OnSetTarget()
        {
            if (targetMovementComponent != null)
            {

                targetMovementComponent.mNavMeshAngentMoveBy.Index = Index;
            }

            Follow = true;
              
        }

        private void OnRemoveTarget()
        {

            Follow = false;
        }

        public void SetTarget(SceneActor target, float keepDistance)
        {
            if (target == m_Target)
                return;

            if (target == null)
                OnRemoveTarget();

            SetTarget(target);

            if (target != null)
                OnSetTarget();

            EndMove();

            KeepDistance = keepDistance;

        }


    }





}
