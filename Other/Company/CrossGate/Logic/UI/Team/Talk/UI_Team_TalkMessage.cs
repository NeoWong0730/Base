using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;

namespace Logic
{
    public class UI_Team_TalkMessage_Parma
    {
        public uint Type = 0;
    }
    public class UI_Team_TalkMessage : UIBase, UI_Team_TalkMessage_Layout.IListener
    {
        UI_Team_TalkMessage_Layout m_Layout = new UI_Team_TalkMessage_Layout();

        UI_Team_TalkMessage_Parma m_Parma = new UI_Team_TalkMessage_Parma();

        string m_context = string.Empty;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnOpen(object arg)
        {
            var value = arg as UI_Team_TalkMessage_Parma;

            if (value != null)
                m_Parma = value;
        }

        protected override void OnShow()
        {
            m_context = Sys_Team.Instance.DefaultTalkString;

            m_Layout.SetDefaultTex(m_context);

        }
        public void OnClickClose()
        {
            UIManager.HitButton(EUIID.UI_Team_TalkMessage, "Close");

            CloseSelf();
        }

        public void OnClickSend()
        {
            if (m_context.Length > 50)
            {
                m_context = m_context.Substring(0, 50);
            }

            Sys_Team.Instance.DefaultTalkString = m_context;

            Sys_Team.Instance.SendTalk(m_Parma.Type, m_context);

            UIManager.HitButton(EUIID.UI_Team_TalkMessage, "Send");

            CloseSelf();
        }

        public void OnInputEnd(string value)
        {
            m_context = value;
        }
    }
}
