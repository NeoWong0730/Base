using UnityEngine;
using Logic.Core;
using System;
using UnityEngine.EventSystems;
using Framework;

namespace Logic
{
    public class UI_Joystick : UIBase
    {
        private JoystickInput m_Joystick;

        protected override void OnLoaded()
        {
            m_Joystick = gameObject.AddComponent<JoystickInput>();
            Sys_Input.Instance.JoyStickInputRegister(m_Joystick);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Input.Instance.bEnableJoystick = toRegister;
            //GameCenter.SetInputEnable(toRegister);
        }
    }
}


