using Framework;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{
    public class Character : Actor
    {
        public enum ECharacterNetType
        {
            Local,
            Remote,
        }   

        public ECharacterNetType characterNetType
        {
            get;
            private set;
        }

        public AnimatorComponent animatorComponent;
        public NavigationComponent navigationComponent;

        public void Init(ECharacterNetType netType, ActorDataFromServer actorDataFromServer)
        {
            base.Init();        

            characterNetType = netType;
            mTransform.name = $"Character_{uid}";     

            animatorComponent = mTransform.GetOrAddComponent<AnimatorComponent>();
            InitAnimatorComponent();
            components[typeof(AnimatorComponent)] = animatorComponent;

            navigationComponent = mTransform.GetOrAddComponent<NavigationComponent>();
            InitNavigationComponent();
            components[typeof(NavigationComponent)] = navigationComponent;

            InitSpawnPosition(actorDataFromServer);
            mAnimationPlayable.LoadAnimationClips(100110000);
        }

        void InitSpawnPosition(ActorDataFromServer actorDataFromServer)
        {
            mTransform.position = new Vector3(500f, 0f, 500f);
        }

        void InitAnimatorComponent()
        {
        }

        void InitNavigationComponent()
        {
            navigationComponent.agent = mTransform.GetComponent<NavMeshAgent>();
        }

        #region Function

        public void SetTargetPosition(Vector3 targetPoint)
        {
            if (navigationComponent == null)
                return;

            navigationComponent.targetPoint = targetPoint;
        }

        #endregion
    }
}
