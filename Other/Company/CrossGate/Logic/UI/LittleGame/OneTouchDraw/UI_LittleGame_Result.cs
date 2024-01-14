using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_LittleGame_Result : UIBase
    {
        public Transform fail;
        public Transform success;
        //private Button btnCLose;

        private Timer timer;

        private bool result = false;
        private Action onFinish;
        private bool fromOpen = false;

        protected override void OnLoaded()
        {
            fail = transform.Find("Animator/Fail");
            success = transform.Find("Animator/Success");
            //btnCLose = transform.Find("Animator/Image").GetComponent<Button>();
            //btnCLose.onClick.AddListener(OnBtnCloseClicked);
        }
        protected override void OnOpen(object arg)
        {
            Tuple<bool, Action> tp = arg as Tuple<bool, Action>;
            if (tp != null)
            {
                result = tp.Item1;
                onFinish = tp.Item2;
            }
            fromOpen = true;
        }
        protected override void OnHide()
        {
            timer?.Pause();
            fromOpen = false;
        }
        protected override void OnShow()
        {
            if(fromOpen)
            {
                fail.gameObject.SetActive(!result);
                success.gameObject.SetActive(result);

                timer?.Cancel();
                timer = Timer.Register(3f, OnBtnCloseClicked);
            }
            else
            {
                 timer?.Resume();
            }
        }

        private void OnBtnCloseClicked()
        {
            onFinish?.Invoke();
            UIManager.CloseUI(EUIID.UI_LittleGame_Result);
        }
    }
}