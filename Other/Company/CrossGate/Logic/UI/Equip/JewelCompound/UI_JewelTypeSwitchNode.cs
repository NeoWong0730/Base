using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using System;

namespace Logic
{
    public class UI_JewelTypeSwitchNode : UIParseCommon
    {
        private CP_Toggle toggle;
        private Text jewelTypeName;
        private Image imgLight;

        private EJewelType _jewelType = EJewelType.All;
        private Action<EJewelType> selectAction;


        protected override void Parse()
        {
            toggle = gameObject.GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener(OnClickToggle);

            imgLight = transform.Find("Image").GetComponent<Image>();
            jewelTypeName = transform.Find("Text").GetComponent<Text>();
        }

        private void OnClickToggle(bool isOn)
        {
            if (isOn)
            {
                Sys_Equip.Instance.CurJewelType = _jewelType;
                selectAction?.Invoke(_jewelType);
            }
        }

        public void AddListener(Action<EJewelType> action)
        {
            selectAction = action;
        }

        public void UpdateJewelInfo(EJewelType type)
        {
            _jewelType = type;
            TextHelper.SetText(jewelTypeName, 4161 + (uint)type);

            //toggle.SetSelected(false, true);

            imgLight.gameObject.SetActive(type == Sys_Equip.Instance.CurJewelType);
        }
    }
}


