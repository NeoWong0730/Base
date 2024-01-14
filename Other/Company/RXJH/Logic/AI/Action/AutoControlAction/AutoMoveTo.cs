using BehaviorDesigner.Runtime.Tasks;
using Logic.Core;

namespace Logic
{
    public class AutoMoveTo : Action
    {
        private Actor _actor;
        private Actor actor
        {
            get
            {
                LvPlay lvPlay = LevelManager.mMainLevel as LvPlay;
                if (lvPlay != null)
                {
                    _actor = lvPlay.mLvPlayData.mMainActor;
                }

                return _actor;
            }
        }

        private NavigationComponent _navigationComponent;
        private NavigationComponent navigationComponent
        {
            get
            {
                if (_navigationComponent == null && actor != null)
                {
                    IComponent component;
                    if (actor.components.TryGetValue(typeof(NavigationComponent), out component))
                    {
                        _navigationComponent = component as NavigationComponent;
                    }
                }

                return _navigationComponent;
            }
        }

        public override void OnStart()
        {
            if (navigationComponent != null)
            {
                navigationComponent.isNavigate = true;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (actor == null || navigationComponent == null)
                return TaskStatus.Failure;

            if (NavigationSystem.NearPoint(actor, navigationComponent.targetPoint, actor.mTransform.position))
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }
}