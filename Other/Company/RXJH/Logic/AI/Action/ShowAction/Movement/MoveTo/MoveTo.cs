using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Logic
{
    public abstract class MoveTo : Action
    {
        protected Vector3 _targetPosition;

        protected Actor _actor;

        private NavigationComponent _navigationComponent;
        private NavigationComponent navigationComponent
        {
            get
            {
                if (_navigationComponent == null && _actor != null)
                {
                    IComponent component;
                    if (_actor.components.TryGetValue(typeof(NavigationComponent), out component))
                    {
                        _navigationComponent = component as NavigationComponent;
                    }
                }

                return _navigationComponent;
            }
        }

        public override void OnStart()
        {
            SetActor();
            SetTargetPosition();
            Move();
        }

        protected abstract void SetActor();

        protected abstract void SetTargetPosition();

        void Move()
        {
            if (navigationComponent != null)
            {
                navigationComponent.targetPoint = _targetPosition;
                navigationComponent.isNavigate = true;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_actor != null && navigationComponent != null)
            {
                if (NavigationSystem.NearPoint(_actor, navigationComponent.targetPoint, _actor.mTransform.position))
                {
                    return TaskStatus.Success;
                }
            }
            else
            {
                SetActor();
                Move();
            }
            return TaskStatus.Running;
        }
    }   
}
