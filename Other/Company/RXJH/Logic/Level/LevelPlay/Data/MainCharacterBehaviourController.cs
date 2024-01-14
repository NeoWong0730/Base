using BehaviorDesigner.Runtime;
using Framework;
using UnityEngine;

namespace Logic
{
    public enum ControllerState
    {
        PlayerSelf,
        Auto,
    }

    public class MainCharacterBehaviourController
    {
        private GameObject gameObject;

        public MainCharacterBehaviourComponent mainCharacterBehaviourComponent;
        public BehaviorTree behaviorTree;

        public void Init(GameObject obj, Character character, ExternalBehaviorTree externalBehaviorTree)
        {
            gameObject = obj;
            gameObject.name = "MainCharacterBehaviourController";

            mainCharacterBehaviourComponent = gameObject.GetOrAddComponent<MainCharacterBehaviourComponent>();
            InitMainCharacterBehaviourComponent(character, externalBehaviorTree);
        }

        void InitMainCharacterBehaviourComponent(Character character, ExternalBehaviorTree externalBehaviorTree)
        {
            mainCharacterBehaviourComponent.mainCharacter = character;
            behaviorTree = gameObject.GetOrAddComponent<BehaviorTree>();
            behaviorTree.StartWhenEnabled = false;
            behaviorTree.ExternalBehavior = externalBehaviorTree;
        }
    }
}
