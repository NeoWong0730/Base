using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_CountDown : UIBase
    {
        private Text text;
        private Image image;
        private Timer timer;

        private float countDown = -1f;
        private Vector3 pos;
        private Action onFinish;
        private bool fromOpen = false;

        protected override void OnLoaded()
        {
            image = transform.Find("Animator/Image_BG").GetComponent<Image>();
            text = transform.Find("Animator/Image_BG/Text_Time").GetComponent<Text>();
        }

        protected override void OnOpen(object arg)
        {
            Tuple<float, Vector3, Action> tp = arg as Tuple<float, Vector3, Action>;
            if (tp != null)
            {
                countDown = tp.Item1;
                pos = tp.Item2;
                onFinish = tp.Item3;
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
                image.transform.position = pos;
                if (countDown != -1)
                {
                    timer = Timer.Register(countDown, () =>
                    {
                        onFinish?.Invoke();
                        CloseSelf();                        
                    }, (delta) =>
                    {
                        text.text = ((int)(countDown - delta)+1).ToString();
                    });
                }
                else
                {
                    onFinish?.Invoke();
                    CloseSelf();       
                }
            }
            else
            {
                timer?.Resume();
            }
        }
    }
}