using UnityEngine;

namespace Logic
{
    public class NavigationSystem : LvPlaySystemBase
    {
        public override void OnUpdate()
        {
            foreach (var item in mData.mActorList)
            {
                if (item.HasComponent(typeof(NavigationComponent)))
                {
                    NavigationComponent navigationComponent = item.components[typeof(NavigationComponent)] as NavigationComponent;

                    MoveToPoint(item, navigationComponent);
                }
            }
        }

        void MoveToPoint(Actor actor, NavigationComponent navigationComponent)
        {
            if (!navigationComponent.isNavigate)
            {
                return;
            }

            if (!NearPoint(actor, navigationComponent.targetPoint, actor.mTransform.position))
            {
                if (navigationComponent.agent)
                {
                    if (navigationComponent.agent.isOnNavMesh)
                    {
                        Vector3 dir = navigationComponent.targetPoint - actor.mTransform.position;

                        if (navigationComponent.agent.updatePosition)
                            navigationComponent.agent.updatePosition = false;
                        if (navigationComponent.agent.updateRotation)
                            navigationComponent.agent.updateRotation = false;

                        if (dir.magnitude > 0.1f)
                            navigationComponent.agent.destination = navigationComponent.targetPoint;
                        dir = navigationComponent.agent.desiredVelocity.normalized.normalized * Mathf.Clamp(navigationComponent.agent.desiredVelocity.normalized.magnitude, 0.1f, 1f);
                        navigationComponent.agent.nextPosition = actor.mTransform.position;

                        AIMovementInputs inputs = new AIMovementInputs();
                        inputs.MoveVector = dir.normalized;
                        inputs.LookVector = dir.normalized;
                        actor.mMovementController.SetInputs(ref inputs);
                    }
                    else
                    {

                    }
                }
            }
            else
            {
                navigationComponent.isNavigate = false;
            }
        }

        public static bool NearPoint(Actor actor, Vector3 a, Vector3 b)
        {
            var _a = new Vector3(a.x, actor.mTransform.position.y, a.z);
            var _b = new Vector3(b.x, actor.mTransform.position.y, b.z);
            return Vector3.Distance(_a, _b) <= 0.5f;
        }
    }
}
