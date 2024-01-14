using Lib.Core;

namespace Logic
{
    public class PlayerAIControlSystem : LvPlaySystemBase
    {
        public static readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents
        {
            EnableBehavior,
            DisableBehavior,
        }

        public override void OnEnable()
        {
            base.OnEnable();

            eventEmitter.Handle(EEvents.EnableBehavior, OnEnableBehavior, true);
            eventEmitter.Handle(EEvents.DisableBehavior, OnDisableBehavior, true);
        }

        public override void OnDisable()
        {
            base.OnDisable();

            eventEmitter.Handle(EEvents.EnableBehavior, OnEnableBehavior, false);
            eventEmitter.Handle(EEvents.DisableBehavior, OnDisableBehavior, false);
        }

        void OnEnableBehavior()
        {
            if (mData.mainCharacterBehaviourController == null || mData.mMainCharacter == null)
                return;

            if (mData.mainCharacterBehaviourController.mainCharacterBehaviourComponent == null)
                return;

            mData.mainCharacterBehaviourController.behaviorTree.EnableBehavior();
        }

        void OnDisableBehavior()
        {
            if (mData.mainCharacterBehaviourController == null || mData.mMainCharacter == null)
                return;

            if (mData.mainCharacterBehaviourController.mainCharacterBehaviourComponent == null)
                return;

            mData.mainCharacterBehaviourController.behaviorTree.DisableBehavior();
        }
    }
}
