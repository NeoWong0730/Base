using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_FamilyCreatures_SetTime_Layout
    {
        public Transform transform;
        public Button cancleBtn;
        public Button confirmBtn;
        public Button closeBtn;
        public Text hourText;
        public Text minText;
        public UI_Common_Num hourInput;
        public UI_Common_Num minInput;
        public void Init(Transform transform)
        {
            this.transform = transform;
            cancleBtn = transform.Find("Animator/Btn_Cancel").GetComponent<Button>();
            confirmBtn = transform.Find("Animator/Btn_Confirm").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            hourInput = new UI_Common_Num();
            hourInput.Init(transform.Find("Animator/Time/Btn_Hour"), 23);
            minInput = new UI_Common_Num();
            minInput.Init(transform.Find("Animator/Time/Btn_Minute"), 59);
            hourText = transform.Find("Animator/Time/Btn_Hour/Text").GetComponent<Text>();
            minText = transform.Find("Animator/Time/Btn_Minute/Text").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener)
        {
            cancleBtn.onClick.AddListener(listener.OnCancleClicked);
            confirmBtn.onClick.AddListener(listener.OnConfirmClicked);
            closeBtn.onClick.AddListener(listener.OnCloseClicked);
            hourInput.RegChange(listener.OnHourValueChange);
            hourInput.RegEnd(listener.OnHourEditorEnd);
            minInput.RegChange(listener.OnMinValueChange);
            minInput.RegEnd(listener.OnMinEditorEnd);
        }

        public interface IListener
        {
            void OnCancleClicked();
            void OnConfirmClicked();
            void OnCloseClicked();
            void OnHourValueChange(uint num);
            void OnHourEditorEnd(uint num);
            void OnMinValueChange(uint num);
            void OnMinEditorEnd(uint num);
        }
    }
    public class UI_FamilyCreatures_SetTime : UIBase, UI_FamilyCreatures_SetTime_Layout.IListener
    {
        private UI_FamilyCreatures_SetTime_Layout layout = new UI_FamilyCreatures_SetTime_Layout();
        private UI_FamilyCreatures_SetTimeParam param;
        public class UI_FamilyCreatures_SetTimeParam
        {
            public uint currentHour;
            public uint currentMinus;
            public Action<uint> action;
        }

        protected override void OnLoaded()
        {
            layout.Init(gameObject.transform);
            layout.RegisterEvents(this);
        }

        protected override void OnShow()
        {
            layout.hourInput.SetData(param.currentHour, "D2");
            layout.minInput.SetData(param.currentMinus, "D2");
        }

        protected override void OnOpen(object arg1 = null)
        {
            if(null != arg1)
            {
                param = arg1 as UI_FamilyCreatures_SetTimeParam;
            }
            else
            {
                param = new UI_FamilyCreatures_SetTimeParam();
            }
        }

        public void OnCancleClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_SetTime, "OnCancleClicked");
            CloseSelf();
        }

        public void OnCloseClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_SetTime, "OnCloseClicked");
            CloseSelf();
        }

        public void OnConfirmClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_SetTime, "OnConfirmClicked");
            param.action?.Invoke(param.currentHour * 3600 + param.currentMinus * 60);
            CloseSelf();
        }

        public void OnHourValueChange(uint num)
        {

        }

        public void OnHourEditorEnd(uint num)
        {
            uint Input = num;
            param.currentHour = Input;
            layout.hourInput.SetData(Input, "D2");
        }

        private bool CheckActiveIsOpen()
        {
            return false;
        }

        public void OnMinValueChange(uint num)
        {

        }

        public void OnMinEditorEnd(uint num)
        {
            uint Input = num;
            param.currentMinus = Input;
            layout.minInput.SetData(Input, "D2");
        }
    }

}