using Logic.Core;
using System;
using UnityEngine;
using Framework;
using Lib.Core;

namespace Logic
{
    public class Sys_Input : SystemModuleBase<Sys_Input>
    {
        private JoystickData mLeftJoystick;
        private JoystickData mRightJoystick;
        private KeyboardInput keyboardInput;

        public Action<Vector2, float> onLeftJoystick;
        public Action<Vector2, float> onRightJoystick;
        public Action<EInputType> onInput;
        public Action<Vector2> onTouchDown;
        public Action<float> onScale;
        public Action<Vector2> onTouchUp;
        public Action<int> onTouchTerrain;
        public Action<Vector2> onTouchLongPress;
        public Action<bool> onClickOrTouch;
        public Action<Vector2> onTouchRightUp; //右键事件
        public Action<Vector3> onTouchLongMove; //左键长按跟随鼠标移动

        public bool bForbidTouch = false;
        public bool bForbidControl = false;

        private bool bJoystickEnable = true;

        private JoystickInput _joystickInput;

        public Action<KeyCode> onKeyDown;

        private Transform screenTouchRoot;

        public bool bIsOpenMainMenu = false;

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents : int
        {
            OnScreenTouchDown,//
            OnCloseMainMenu,//关闭主界面菜单
        }
        public override void Init()
        {
            InitScreenTouchMono();
        }
        public bool bEnableJoystick
        {
            get
            {
                return bJoystickEnable;
            }
            set
            {
                bJoystickEnable = value;

                if (_joystickInput == null)
                    return;

                _joystickInput.JoytickEnable = value;
            }
        }
        public void KeyboardInputRegister(KeyboardInput keyboardInput)
        {
            this.keyboardInput = keyboardInput;
            keyboardInput.SetLeftJoystick = SetLeftJoystick;
            keyboardInput.SetRightJoystick = SetRightJoystick;
            keyboardInput.SendInput = SendInput;
            keyboardInput.SendScale = SendScale;
            keyboardInput.SendHotKey = Sys_SettingHotKey.Instance.OnGUI_HotKey;
        }

        public void KeyboardInputMoveAction(float lh = 0, float lv = 0)
        {
            if (keyboardInput != null)
                keyboardInput.KeyMove(lh, lv);
        }

        public void JoyStickInputRegister(JoystickInput joystickInput)
        {
            _joystickInput = joystickInput;
            _joystickInput.JoytickEnable = bJoystickEnable;

            joystickInput.SetLeftJoystick = SetLeftJoystick;
            joystickInput.SendTouchUp = SendTouchUp;
            joystickInput.SendTouchRightUp = SendTouchRightUp;
            joystickInput.SendMoveFollowMouse = SendTouchLongMoveByMouse;
        }

        public void TouchInputRegister(TouchInput touchInput, bool register)
        {
            if (register)
            {
                touchInput.SendTouchUp = SendTouchUp;
                touchInput.SendTouchLongPress = SendTouchLongPress;
            }
            else
            {
                touchInput.SendTouchUp = null;
                touchInput.SendTouchLongPress = null;
            }
        }

        public void SetLeftJoystick(JoystickData joyData)
        {
            if (joyData.Equals(mLeftJoystick) || bForbidControl)
            {
                return;
            }

            mLeftJoystick = joyData;
            onLeftJoystick?.Invoke(mLeftJoystick.dir, mLeftJoystick.dis);
        }

        public void SetRightJoystick(JoystickData joyData)
        {
            if (joyData.Equals(mRightJoystick) || bForbidControl || !bJoystickEnable)
            {
                return;
            }
            mRightJoystick = joyData;
            onRightJoystick?.Invoke(mRightJoystick.dir, mRightJoystick.dis);
        }

        public void SendInput(KeyCode keyCode)
        {
            if (bForbidControl)
            {
                return;
            }
            if (keyCode == KeyCode.Q)
            {
                onInput?.Invoke(EInputType.SwitchWeapon);
            }
            else if (keyCode == KeyCode.E)
            {
                onInput?.Invoke(EInputType.EnterCar);
            }

            onKeyDown?.Invoke(keyCode);
        }

        public void SendTouchDown(Vector3 pos)
        {
            if (bForbidTouch || bForbidControl)
                return;

            onTouchDown?.Invoke(pos);
            onClickOrTouch?.Invoke(true);
        }

        public void SendTouchUp(Vector3 pos)
        {
            if (bForbidTouch || bForbidControl)
                return;
            if (bIsOpenMainMenu&&UIManager.IsVisibleAndOpen(EUIID.UI_MainMenu))
            {
                eventEmitter.Trigger(EEvents.OnCloseMainMenu);
                return;
            }
            onTouchUp?.Invoke(pos);
            onClickOrTouch?.Invoke(false);
        }

        public void SendTouchRightUp(Vector3 pos)
        {
            if (bForbidTouch || bForbidControl)
                return;

            onTouchRightUp?.Invoke(pos);
            onClickOrTouch?.Invoke(false);
        }

        public void SendTouchLongMoveByMouse(Vector3 pos)
        {
            if (bForbidTouch || bForbidControl)
                return;

            onTouchLongMove?.Invoke(pos);
            onClickOrTouch?.Invoke(false);
        }

        public void SendTouchLongPress(Vector3 pos)
        {
            if (bForbidTouch || bForbidControl)
                return;

            onTouchLongPress?.Invoke(pos);
            onClickOrTouch?.Invoke(false);
        }

        public void SendScale(float scale)
        {
            if (bForbidControl)
            {
                return;
            }
            onScale?.Invoke(scale);
        }

        public void SetEnable(bool enable)
        {
            _joystickInput.enabled = enable;
        }

        /// <summary>
        /// 初始化屏幕点击监听脚本
        /// </summary>
        private void InitScreenTouchMono()
        {
            screenTouchRoot = new GameObject("ScreenTouchRoot").transform;
            if (screenTouchRoot != null)
            {
                GameObject.DontDestroyOnLoad(screenTouchRoot);
                var mono = screenTouchRoot.gameObject.AddComponent<ScreenTouchMonoBehaviour>();
                mono.OnScreenTouchDown = OnScreenTouchDown;
            }
        }

        private void OnScreenTouchDown()
        {
            eventEmitter.Trigger(EEvents.OnScreenTouchDown);
        }

#if GM_PROPAGATE_VERSION && UNITY_STANDALONE_WIN
        public void SetEnableJoystick()
        {
            //仅用于给到港台宣发
            if (_joystickInput == null)
                return;
            _joystickInput.JoytickEnable = false;
        }
#endif
    }
}