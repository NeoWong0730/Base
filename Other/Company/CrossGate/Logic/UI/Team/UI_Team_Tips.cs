using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using UnityEngine;

namespace Logic
{
    class UI_Team_Tips_Param
    {
        public float time { get; set; } = 10f;

        public string title { get; set; } = string.Empty;

        public string message { get; set; } = string.Empty;

        /// <summary>
        /// 0,取消 1，确认
        /// </summary>
        public Action<int> OpReslutAc;

        /// <summary>
        /// 超时的默认操作
        /// </summary>
        public int DefaultOp { get; set; } = 0;
    }
    public partial class UI_Team_Tips:UIBase,UI_Team_Tips_Layout.IListener
    {
        private UI_Team_Tips_Param m_Param = null;

        private UI_Team_Tips_Layout m_Layout = new UI_Team_Tips_Layout();

        private Timer m_Timer;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }
        protected override void OnOpen(object arg)
        {
            m_Param = arg as UI_Team_Tips_Param;

        }

        protected override void OnShow()
        {
            if (m_Timer != null && m_Timer.isPaused)
                m_Timer.Resume();

            OnRefresh();
        }
        protected override void OnClose()
        {
            m_Param = null;

            if (m_Timer != null && m_Timer.isDone == false)
                m_Timer.Cancel();

            m_Timer = null;
        }

        private void OnRefresh()
        {
            string message = m_Param == null ? string.Empty : m_Param.message;

            m_Layout.SetMessage(message);

            if (m_Param != null && m_Param.time > 0 && m_Timer == null)
                m_Timer = Timer.Register(m_Param.time, OnTimeOver, OnTickTime);

            m_Layout.SetBtnSureTex(LanguageHelper.GetTextContent(1000906));

            m_Layout.SetBtnCancleTex(LanguageHelper.GetTextContent(1000905));
        }

        private void OnTickTime(float value)
        {
            float tiem = m_Param.time - value;

            string tex = string.Empty;

            if (m_Param.DefaultOp == 1)
            {
                tex = LanguageHelper.GetTextContent(2002121, ((int)tiem).ToString());
                m_Layout.SetBtnSureTex(tex);
            }
            else if (m_Param.DefaultOp == 0)
            {
                tex = LanguageHelper.GetTextContent(2002122, ((int)tiem).ToString());
                m_Layout.SetBtnCancleTex(tex);
            }
               

          
        }

        private void OnTimeOver()
        {
            if (m_Param.DefaultOp == 1)
            {
                OnClickSure();
            }
            else if (m_Param.DefaultOp == 0)
            {
                OnClickCancle();
            }
        }

        private void OnResutl(int result)
        {
            if (m_Timer != null && m_Timer.isDone == false)
                m_Timer.Cancel();

            if (m_Param != null)
                m_Param.OpReslutAc?.Invoke(result);
        }
    }

    public partial class UI_Team_Tips : UIBase, UI_Team_Tips_Layout.IListener
    {
        public void OnClickCancle()
        {
            CloseSelf();

            OnResutl(0);
        }

        public void OnClickSet()
        {
            UIManager.OpenUI(EUIID.UI_Setting, false, Tuple.Create<ESettingPage, ESetting>(ESettingPage.Settings, ESetting.Game));


           // UIManager.OpenUI(EUIID.UI_Setting);

            if (m_Timer != null && m_Timer.isDone)
                m_Timer.Pause();
        }

        public void OnClickSure()
        {
            CloseSelf();

            OnResutl(1);

            UIManager.OpenUI(EUIID.UI_Team_Apply);
        }


    }
}
