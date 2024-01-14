using Framework;
using UnityEngine.AI;

namespace Logic
{
    public class VirtualActor : Actor
    {
        public AnimatorComponent animatorComponent;
        public NavigationComponent navigationComponent;

        public void InitVirtualActor()
        {
            base.Init();

            mTransform.name = $"VirtualActor_{uid}";

            animatorComponent = mTransform.GetOrAddComponent<AnimatorComponent>();
            InitAnimatorComponent();
            components[typeof(AnimatorComponent)] = animatorComponent;

            navigationComponent = mTransform.GetOrAddComponent<NavigationComponent>();
            InitNavigationComponent();
            components[typeof(NavigationComponent)] = navigationComponent;
        }

        void InitAnimatorComponent()
        {
        }

        void InitNavigationComponent()
        {
            if (navigationComponent == null)
                return;

            navigationComponent.agent = mTransform.GetComponent<NavMeshAgent>();
            navigationComponent.enabled = true;
        }
    }
}