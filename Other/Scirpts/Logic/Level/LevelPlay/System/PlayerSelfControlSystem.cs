using Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Logic
{
    public struct PlayerInputsClientCache
    {
        public int tickIndex;
        public PlayerMovementInputs playerMovementInputs;
        public PlayerActionInputs playerActionInputs; 
    }

    public struct PlayerInputsToServer
    {
        public int tickIndex;
        public PlayerMoveInputs moveInputs;
        public PlayerActionInputs actionInputs;
    }

    public struct PlayerMoveInputs
    {
        public Vector3 BaseVelocity;
        public bool JumpDown;
    }

    public struct PlayerActionInputs
    {

    }

    public struct PlayerTickState
    {
        public int tickIndex;
        public KinematicCharacterMotorState motorState;
        public PlayerActionState actionState;
    }

    public struct PlayerActionState
    {

    }

    public class PlayerSelfControlSystem : LvPlaySystemBase
    {
        public enum EEvents
        {
            OnJoystickDrag,
        }

        public static readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        private float timer;
        private int currentTick;
        private float minTimeBetweenTicks = 1f / SERVER_TICK_RATE;
        private const float SERVER_TICK_RATE = 20;
        private const int BUFFER_SIZE = 1024;

        private PlayerTickState[] playerStateBuffer = new PlayerTickState[BUFFER_SIZE];
        private PlayerInputsClientCache[] playerInputsBuffer = new PlayerInputsClientCache[BUFFER_SIZE];
        private PlayerTickState latestServerPlayerState;
        private PlayerTickState lastProcessedPlayerState;

        public override bool CanUpdate()
        {
            return true;
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

        List<PlayerMovementInputs> cacheInputs = new List<PlayerMovementInputs>();

        public override void OnUpdate()
        {
            if (mData.mainCharacterBehaviourController == null || mData.mMainCharacter == null)
                return;

            if (mData.mainCharacterBehaviourController.mainCharacterBehaviourComponent == null)
                return;

            if (mData.mainCharacterBehaviourController.mainCharacterBehaviourComponent.currentControllerState == ControllerState.Auto)
                return;

            Vector2 input = mData.mInputDatas.Player.Move.ReadValue<Vector2>();
            if (input.magnitude > 0)
            {
                mData.mMainCharacter.navigationComponent.isNavigate = false;
            }

            PlayerMovementInputs playerMovementInputs = new PlayerMovementInputs();
            playerMovementInputs.MoveAxisForward = input.y;
            playerMovementInputs.MoveAxisRight = input.x;
            playerMovementInputs.CameraRotation = mData.mCameraData.mainCameraTran.rotation;
            playerMovementInputs.JumpDown = Keyboard.current.spaceKey.isPressed;

            cacheInputs.Add(playerMovementInputs);

            timer += Time.deltaTime;
            while (timer >= minTimeBetweenTicks)
            {
                timer -= minTimeBetweenTicks;
                HandleTick();
                currentTick++;
            }
        }

        void HandleTick()
        {
            PlayerInputsClientCache playerInputsClientCache = new PlayerInputsClientCache();
            PlayerInputsToServer playerInputsToServer = new PlayerInputsToServer();
            playerInputsClientCache.tickIndex = currentTick;
            playerInputsToServer.tickIndex = currentTick;

            MovementInput(ref playerInputsClientCache, ref playerInputsToServer);
            //ActionsInput(ref playerInputsClientCache, ref playerInputsToServer);
            //PointClick();

            int bufferIndex = currentTick % BUFFER_SIZE;
            playerInputsBuffer[bufferIndex] = playerInputsClientCache;
        }

        void MovementInput(ref PlayerInputsClientCache playerInputsClientCache, ref PlayerInputsToServer playerInputsToServer)
        {
            if (cacheInputs.Count <= 0)
                return;

            PlayerMovementInputs playerMovementInputs = new PlayerMovementInputs();
            playerMovementInputs.MoveAxisForward = cacheInputs[cacheInputs.Count - 1].MoveAxisForward;
            playerMovementInputs.MoveAxisRight = cacheInputs[cacheInputs.Count - 1].MoveAxisRight;
            playerMovementInputs.CameraRotation = cacheInputs[cacheInputs.Count - 1].CameraRotation;
            bool jumpDown = false;
            for (int index = 0, len = cacheInputs.Count; index < len; index++)
            {
                if (cacheInputs[index].JumpDown)
                {
                    jumpDown = true;
                    break;
                }
            }
            playerMovementInputs.JumpDown = jumpDown;

            mData.mMainActor.mMovementController.SetInputs(ref playerMovementInputs);

            var cacheState = mData.mMainActor.mMovementController.Motor.GetState();
            playerInputsClientCache.playerMovementInputs = playerMovementInputs;

            mData.mMainActor.mMovementController.Motor.UpdatePhase1(minTimeBetweenTicks);
            mData.mMainActor.mMovementController.Motor.UpdatePhase2(minTimeBetweenTicks, true);

            PlayerMoveInputs playerMoveInputs = new PlayerMoveInputs();
            playerMoveInputs.BaseVelocity = mData.mMainActor.mMovementController.Motor.GetState().BaseVelocity;
            playerMoveInputs.JumpDown = playerMovementInputs.JumpDown;

            playerInputsToServer.moveInputs = playerMoveInputs;

            mData.mMainActor.mMovementController.Motor.ApplyState(cacheState);
            mData.mMainActor.mMovementController.SetInputs(ref playerMovementInputs);

            cacheInputs.Clear();
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

        void ActionsInput(ref PlayerInputsClientCache playerInputsClientCache, ref PlayerInputsToServer playerInputsToServer)
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
