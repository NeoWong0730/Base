using Cinemachine;
using Lib.Core;
using Logic.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Logic
{
    public struct PlayerMovementInputs
    {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool JumpDown;
    }

    public struct AIMovementInputs
    {
        public Vector3 MoveVector;
        public Vector3 LookVector;
    }

    public class PlayerSelfControlSystem : LvPlaySystemBase
    {
        //@PlayerInputActions Inputs = new PlayerInputActions();

        public static readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents
        {
            OnJoystickDrag,
        }

        public override void OnEnable()
        {
            base.OnEnable();

            mData.mInputDatas.Player.Look.Enable();
            mData.mInputDatas.Player.Scale.Enable();
            mData.mInputDatas.Player.Move.Enable();            
            eventEmitter.Handle<string, Vector2>(EEvents.OnJoystickDrag, OnJoystickDrag, true);
        }

        public override void OnDisable()
        {
            base.OnDisable();

            mData.mInputDatas.Player.Look.Disable();
            mData.mInputDatas.Player.Scale.Disable();
            mData.mInputDatas.Player.Move.Disable();            
            eventEmitter.Handle<string, Vector2>(EEvents.OnJoystickDrag, OnJoystickDrag, false);
        }

        public override void OnUpdate()
        {
            if (mData.mainCharacterBehaviourController == null || mData.mMainCharacter == null)
                return;

            if (mData.mainCharacterBehaviourController.mainCharacterBehaviourComponent == null)
                return;

            if (mData.mainCharacterBehaviourController.mainCharacterBehaviourComponent.currentControllerState == ControllerState.Auto)
                return;

            InputHandle();
        }

        void InputHandle()
        {
            PointClick();
            ActionsInput();
            MovementInput();
        }

        void MovementInput()
        {
            PlayerMovementInputs characterInputs = new PlayerMovementInputs();

            characterInputs.MoveAxisForward = Input.GetAxisRaw("Vertical");
            characterInputs.MoveAxisRight = Input.GetAxisRaw("Horizontal");
            characterInputs.CameraRotation = mData.mCameraData.mainCameraTran.rotation;
            characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);

            mData.mMainActor.mMovementController.SetInputs(ref characterInputs);
        }

        void PointClick()
        {
            if (mData.mMainCharacter.navigationComponent == null)
                return;

            RaycastHit hit;

            if (Physics.Raycast(mData.mCameraData.mainCameraObj.GetComponent<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, (int)ELayerMask.Default))
            {
#if UNITY_ANDROID || UNITY_IPHONE
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#else
                if (!EventSystem.current.IsPointerOverGameObject())
#endif
                {
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        mData.mMainCharacter.SetTargetPosition(hit.point);
                        mData.mMainCharacter.navigationComponent.isNavigate = true;
                    }
                }
            }
        }

        void ActionsInput()
        {
            if (mData.mMainCharacter.animatorComponent == null)
                return;

            if (Keyboard.current.iKey.wasPressedThisFrame)
            {
                mData.mMainCharacter.mAnimationPlayable.PlayAnimation(((SkeletonModelController)mData.mMainCharacter.mAnimationPlayable).animationClipListData.animationClipDatasDict["skill001"].clip);
            }

            if (Keyboard.current.jKey.wasPressedThisFrame)
            {
                mData.mMainCharacter.mAnimationPlayable.LoadOverrideController("animatorOverrideController_100110000.overrideController");
            }

            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                mData.mMainCharacter.mAnimationPlayable.StopLayer(SkeletonModelController.EAnimancerLayer.FullBody);
            }
        }

        void OnJoystickDrag(string name, Vector2 input)
        {
            if (mData.mainCharacterBehaviourController.mainCharacterBehaviourComponent.currentControllerState == ControllerState.Auto)
                return;

            if (name == "Movement")
            {
                mData.mMainCharacter.navigationComponent.isNavigate = false;

                Vector3 joystickInput = new Vector3(input.x, 0, input.y);

                PlayerMovementInputs characterInputs = new PlayerMovementInputs();

                if (joystickInput.magnitude > 0f && joystickInput.magnitude <= 0.5f)
                {
                    characterInputs.MoveAxisForward = joystickInput.normalized.z / 2f;
                    characterInputs.MoveAxisRight = joystickInput.normalized.x / 2f;
                }
                else
                {
                    characterInputs.MoveAxisForward = joystickInput.normalized.z;
                    characterInputs.MoveAxisRight = joystickInput.normalized.x;
                }
                characterInputs.CameraRotation = mData.mCameraData.mainCameraTran.rotation;
                characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);       

                mData.mMainActor.mMovementController.SetInputs(ref characterInputs);
            }
        }
    }
}
