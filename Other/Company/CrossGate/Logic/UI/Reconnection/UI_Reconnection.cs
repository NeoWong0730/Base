using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Reconnection : UIBase
    {
        private enum WaitState
        {
            None,
            Wait,
            Fail,
            Disconnect,
        }

        private WaitState curState = WaitState.None;

        private float timerCounter = 0f;

        private Lib.Core.Timer timerFail;
        private int nFailTimeMax = 5;
        private int nFailTimeCount = 0;
        //private float Max_WaitTime = 20f;
        //private float Reconnect_Interval = 4f;
        //private float waitReconnectCounter = 0f;
        //private float Max_FailTime = 5f;

        //wait
        private Transform m_WaitGo;
        private Text m_WaitTip;

        //fail
        private Transform m_WaitFail;
        private Text m_FailTip;
        private Text m_FailContinueText;
        private Text m_FailBack;

        //disconnect
        private Transform m_Disconnect;
        private Text m_DisTip;
        private Text m_DisBack;

        private int pointNum;

        protected override void OnLoaded()
        {            
            m_WaitGo = gameObject.transform.Find("Animator/View_Wait");
            m_WaitTip = m_WaitGo.Find("Text_Tip").GetComponent<Text>();

            m_WaitFail = gameObject.transform.Find("Animator/View_Fail");
            m_FailTip = m_WaitFail.Find("Text_Tip").GetComponent<Text>();
            m_FailTip.text = LanguageHelper.GetTextContent(10013);
            m_WaitFail.gameObject.SetActive(false);

            m_FailContinueText = m_WaitFail.Find("Button_Continue/Text").GetComponent<Text>();
            m_WaitFail.Find("Button_Continue").GetComponent<Button>().onClick.AddListener(OnContinue);
            m_WaitFail.Find("Button_Login").GetComponent<Button>().onClick.AddListener(OnLogout);
            m_FailBack = m_WaitFail.Find("Button_Login/Text").GetComponent<Text>();
            m_FailBack.text = LanguageHelper.GetTextContent(10012);

            m_Disconnect = gameObject.transform.Find("Animator/View_Disconnect");
            m_Disconnect.Find("Button_Login_01").GetComponent<Button>().onClick.AddListener(OnLogout);
            m_DisTip = m_Disconnect.Find("Text_Tip").GetComponent<Text>();
            m_DisTip.text = LanguageHelper.GetTextContent(10014);
            m_DisBack = m_Disconnect.Find("Button_Login_01/Text_01").GetComponent<Text>();
            m_DisBack.text = LanguageHelper.GetTextContent(10012);
        }

        protected override void ProcessEvents(bool toRegister)
        {            
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, toRegister);
        }

        protected override void OnShow()
        {            
            curState = WaitState.Wait;
            timerCounter = 0f;

            timerFail?.Cancel();
            pointNum = 0;
        }

        protected override void OnHide()
        {            
            curState = WaitState.None;
            timerCounter = 0f;

            timerFail?.Cancel();
            timerFail = null;
            pointNum = 0;
        }

        protected override void OnUpdate()
        {
            if (curState == WaitState.Disconnect)
                return;

            WaitState state = Sys_Net.Instance.nReconnectCount > 0 ? (Sys_Net.Instance.nReconnectCount > 3 ? WaitState.Fail : WaitState.Wait) : WaitState.None;
            if (state != curState)
            {
                curState = state;
                if (curState == WaitState.Wait)
                {
                    m_WaitGo.gameObject.SetActive(true);
                    m_WaitFail.gameObject.SetActive(false);
                    m_Disconnect.gameObject.SetActive(false);

                    timerCounter = Sys_Net.Instance.fReconnectTime + Sys_Net.Instance.fReconnect_Interval * (Sys_Net.Instance.nReconnectCount - 1);
                    int pointNum = Mathf.Abs(Mathf.RoundToInt(Mathf.Sin(timerCounter) * 3f));
                    UpdateTextTip(pointNum);
                }
                else if (curState == WaitState.Fail)
                {
                    m_WaitFail.gameObject.SetActive(true);
                    m_WaitGo.gameObject.SetActive(false);
                    m_Disconnect.gameObject.SetActive(false);

                    Sys_Net.Instance.OnPauseReconnect();

                    nFailTimeCount = nFailTimeMax;
                    timerFail?.Cancel();
                    timerFail = Lib.Core.Timer.Register(1f, OnFailTime, null, true);

                    timerCounter = Sys_Net.Instance.fReconnectTime;
                    m_FailContinueText.text = LanguageHelper.GetTextContent(10011, nFailTimeCount.ToString());
                }
            }
            else
            {
                if (curState == WaitState.Wait)
                {
                    timerCounter = Sys_Net.Instance.fReconnectTime + Sys_Net.Instance.fReconnect_Interval * (Sys_Net.Instance.nReconnectCount - 1);
                    int pointNum = Mathf.Abs(Mathf.RoundToInt(Mathf.Sin(timerCounter) * 3f));
                    UpdateTextTip(pointNum);
                }
                //else if (curState == WaitState.Fail)
                //{
                //    //timerCounter = Sys_Net.Instance.fReconnectTime;
                //    //m_FailContinueText.text = LanguageHelper.GetTextContent(10011, (uint)(Max_FailTime - timerCounter));
                //}
            }

            //else if (curState == WaitState.Disconnect)
            //{
            //    m_Disconnect.gameObject.SetActive(true);
            //    m_WaitFail.gameObject.SetActive(false);
            //    m_WaitGo.gameObject.SetActive(false);
            //}
        }

        private void UpdateTextTip(int _pointNum)
        {
            if (pointNum != _pointNum)
            {
                pointNum = _pointNum;

                string tempStr = "";
                for (int i = 0; i < pointNum; ++i)
                {
                    tempStr += ".";
                }
                m_WaitTip.text = string.Format(LanguageHelper.GetTextContent(10010), tempStr);
            }
        }

        private void OnFailTime()
        {
            nFailTimeCount--;
            m_FailContinueText.text = LanguageHelper.GetTextContent(10011, nFailTimeCount.ToString());
            if (nFailTimeCount <= 0)
            {
                OnContinue();
            }
        }

        private void OnContinue()
        {
            timerCounter = 0f;
            timerFail?.Cancel();

            Sys_Net.Instance.StartReconnect();            
        }

        private void OnLogout()
        {
            //if (curState == WaitState.Disconnect)
            //{
            //    Sys_Fight.Instance.ExitFight(true);
            //}

            Sys_Role.Instance.ExitGameReq();
            UIManager.CloseUI(EUIID.UI_Reconnection);
        }

        private void OnReconnectResult(bool result)
        {
            if (result)
            {
                UIManager.CloseUI(EUIID.UI_Reconnection);
            }
            else
            {
                curState = WaitState.Disconnect;

                m_Disconnect.gameObject.SetActive(true);
                m_WaitFail.gameObject.SetActive(false);
                m_WaitGo.gameObject.SetActive(false);
            }
        }
    }
}


