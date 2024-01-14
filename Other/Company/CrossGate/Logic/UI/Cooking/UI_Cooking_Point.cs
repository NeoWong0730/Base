using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;

namespace Logic
{
    public class UI_Cooking_Point : UIBase
    {
        private uint m_Score;
        private Text m_ScoreText;
        private Timer m_Timer;
        private float m_Time;

        protected override void OnOpen(object arg)
        {
            m_Score = (uint)arg;
        }

        protected override void OnLoaded()
        {
            m_ScoreText = transform.Find("Animator/View_Point/Text_Num").GetComponent<Text>();
            m_Time = CSVCookAttr.Instance.GetConfData(10).value/1000f;
        }

        protected override void OnShow()
        {
            m_Timer?.Cancel();
            m_Timer = Timer.Register(m_Time, OnTimerCompelete);
            m_ScoreText.text = m_Score.ToString();
        }

        private void OnTimerCompelete()
        {
            UIManager.CloseUI(EUIID.UI_Cooking_Point);
        }
    }
}


