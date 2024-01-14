using Logic.Core;
using UnityEngine;

namespace Logic
{
    public class MainCharacterBehaviourComponent : MonoBehaviour
    {
        [HideInInspector]
        private Character _mainCharacter;
        public Character mainCharacter
        {
            get => _mainCharacter;
            set => _mainCharacter = value;
        }

        [SerializeField]
        [ReadOnly]
        private ControllerState _currentControllerState = ControllerState.PlayerSelf;
        public ControllerState currentControllerState
        {
            get => _currentControllerState;
            set => _currentControllerState = value;
        }

        [ContextMenu("ChangePlayerSelf")]
        void ChangePlayerSelf()
        {
            currentControllerState = ControllerState.PlayerSelf;
            PlayerAIControlSystem.eventEmitter.Trigger(PlayerAIControlSystem.EEvents.DisableBehavior);
        }

        [ContextMenu("ChangeAuto")]
        void ChangeAuto()
        {
            currentControllerState = ControllerState.Auto;
            PlayerAIControlSystem.eventEmitter.Trigger(PlayerAIControlSystem.EEvents.EnableBehavior);
        }
    }
}
