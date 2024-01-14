using UnityEngine;

namespace Logic
{
    public class AnimatorSystem : LvPlaySystemBase
    {
        public override void OnFixedUpdate()
        {
            foreach (var item in mData.mActorList)
            {
                if (item.HasComponent(typeof(AnimatorComponent)))
                {
                    UpdateAnimator(item);
                }
            }
        }

        void UpdateAnimator(Actor actor)
        {
            if (actor.mAnimationPlayable == null)
                return;

            IComponent component;
            AnimatorComponent animatorComponent = null;

            if (actor.components.TryGetValue(typeof(AnimatorComponent), out component))
                animatorComponent = component as AnimatorComponent;

            SetAnimatorMoveSpeed(actor, animatorComponent);
            UpdateAnimatorParameters(actor, animatorComponent);
        }

        void SetAnimatorMoveSpeed(Actor actor, AnimatorComponent animatorComponent)
        {
            if (animatorComponent.lockAnimMovement)
                return;
        
            animatorComponent.verticalSpeed = actor.mMovementController.moveInputVector.z;
            animatorComponent.horizontalSpeed = actor.mMovementController.moveInputVector.x;
            var newInput = new Vector2(animatorComponent.verticalSpeed, animatorComponent.horizontalSpeed);
            animatorComponent.inputMagnitude = Mathf.Clamp(newInput.magnitude, 0, actor.mMovementController.MaxStableMoveSpeed);
        }

        void UpdateAnimatorParameters(Actor actor, AnimatorComponent animatorComponent)
        {        
            actor.mAnimationPlayable.SetFloat(AnimatorParameters.InputVertical, animatorComponent.verticalSpeed, 0.2f, Time.fixedDeltaTime);
            actor.mAnimationPlayable.SetFloat(AnimatorParameters.InputHorizontal, 0, 0.2f, Time.fixedDeltaTime);
            actor.mAnimationPlayable.SetFloat(AnimatorParameters.VerticalVelocity, actor.mMovementController.Motor.Velocity.y);       
            actor.mAnimationPlayable.SetBool(AnimatorParameters.IsGrounded, actor.mMovementController.Motor.GroundingStatus.IsStableOnGround);
            actor.mAnimationPlayable.SetFloat(AnimatorParameters.InputMagnitude, animatorComponent.inputMagnitude, 0.2f, Time.fixedDeltaTime);
        }
    }
}