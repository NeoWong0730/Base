using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;
using Packet;

namespace Logic
{
    public class UI_TipsAutoClose : UIBase
    {
        private Text m_Text_Tip;
        private Timer m_Timer;
        private uint m_ContentId;
        private int m_Time;

        protected override void OnOpen(object arg)
        {
            Tuple<uint, uint> tuple = arg as Tuple<uint, uint>;
            if (tuple != null)
            {
                m_ContentId = tuple.Item1;
                m_Time = (int)tuple.Item2;
            }
        }

        protected override void OnLoaded()
        {
            m_Text_Tip = transform.Find("Animator/Text_Tip").GetComponent<Text>();
        }

        protected override void OnShow()
        {
            m_Timer?.Cancel();
            m_Timer = Timer.Register(m_Time, OnTimerCompelete, OnAutoUpdateCallback);
            TextHelper.SetText(m_Text_Tip, LanguageHelper.GetTextContent(m_ContentId, m_Time.ToString()));
        }

        private void OnAutoUpdateCallback(float dt)
        {
            int remainTime = 0;
            remainTime = m_Time - (int)dt;
            TextHelper.SetText(m_Text_Tip, LanguageHelper.GetTextContent(m_ContentId, remainTime.ToString()));
        }

        private void OnTimerCompelete()
        {
            UIManager.CloseUI(EUIID.UI_TipsAutoClose);
        }
    }
}


