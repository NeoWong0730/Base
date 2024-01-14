using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Logic
{
    public class FindTarget : Action
    {
        private MainCharacterBehaviourComponent mainCharacterBehaviourComponent;
        private NavigationComponent navigationComponent;

        public override void OnAwake()
        {
            mainCharacterBehaviourComponent = transform.GetComponent<MainCharacterBehaviourComponent>();
            navigationComponent = mainCharacterBehaviourComponent.mainCharacter.mTransform.GetComponent<NavigationComponent>();
        }

        public override void OnStart()
        {
            Vector3 targetPos = new Vector3(Random.Range(480, 520), 0, Random.Range(480, 520));
            navigationComponent.targetPoint = targetPos;
        }
    }
}
