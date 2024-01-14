using UnityEngine;

namespace Logic
{
    public class AnimatorComponent : MonoBehaviour, IComponent
    {
        #region Variables

        public bool lockAnimMovement;

        #region AnimatorController

        [ReadOnly]
        public float verticalSpeed;

        [ReadOnly]
        public float horizontalSpeed;

        [ReadOnly]
        public float inputMagnitude;

        [SerializeField]
        float _walkSpeed = 0.5f;
        public float walkSpeed
        {
            get => _walkSpeed;
            set => _walkSpeed = value;
        }

        [SerializeField]
        float _runningSpeed = 1f;
        public float runningSpeed
        {
            get => _runningSpeed;
            set => _runningSpeed = value;
        }

        #endregion

        #endregion
    }

    public static class AnimatorParameters
    {
        public static int InputHorizontal = Animator.StringToHash(Constant_AnimTags.InputHorizontal);
        public static int InputVertical = Animator.StringToHash(Constant_AnimTags.InputVertical);
        public static int InputMagnitude = Animator.StringToHash(Constant_AnimTags.InputMagnitude);
        public static int VerticalVelocity = Animator.StringToHash(Constant_AnimTags.VerticalVelocity);
        public static int IsStrafing = Animator.StringToHash(Constant_AnimTags.IsStrafing);
        public static int IsSliding = Animator.StringToHash(Constant_AnimTags.IsSliding);
        public static int IsGrounded = Animator.StringToHash(Constant_AnimTags.IsGrounded);
        public static int GroundDistance = Animator.StringToHash(Constant_AnimTags.GroundDistance);
        public static int GroundAngle = Animator.StringToHash(Constant_AnimTags.GroundAngle);
    }
}