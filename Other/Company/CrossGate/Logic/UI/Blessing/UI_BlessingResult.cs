using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
namespace Logic
{
    public class UI_BlessingResult_Parma
    {
        public uint id = 0;
        public uint result = 0;
        public bool autonext = false;
        
    }
    public class UI_BlessingResult : UIBase, UI_BlessingResult_Layout.IListener
    {
        private UI_BlessingResult_Layout m_Laout = new UI_BlessingResult_Layout();

        private bool fristEnter = false;

        UI_BlessingResult_Parma parma;

        Timer countdownTimer = null;

        uint tipsState = 0;
        protected override void OnLoaded()
        {
            m_Laout.Load(gameObject.transform);

            m_Laout.SetListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
           
        }

        protected override void OnOpen(object arg)
        {
            parma = arg as UI_BlessingResult_Parma;
        }
        protected override void OnShow()
        {
            tipsState = 0;

            if (countdownTimer != null)
                countdownTimer.Cancel();

            countdownTimer = Timer.Register(7, OnTimeOver, OnTimeUpdate);

            m_Laout.TransSuccess.gameObject.SetActive(parma.result == 1);

            m_Laout.TransFail.gameObject.SetActive(parma.result != 1);
        }

        protected override void OnHide()
        {
            
        }


        protected override void OnClose()
        {
            if (countdownTimer != null)
                countdownTimer.Cancel();

            countdownTimer = null;
        }

        public void OnClickClose()
        {
            CloseSelf();
            Sys_Blessing.Instance.SetAuto(parma.id, false);
        }

        private void OnTimeUpdate(float time)
        {
            if (parma.autonext == false)
                return;

            if (time > 2 && tipsState == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13591u));
                tipsState += 1;
            }

            else if (time > 4 && tipsState == 1)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13592u));
                tipsState += 1;
            }
            else if (time > 6 && tipsState == 2)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13593u));
                tipsState += 1;
            }
                

        }
        private void OnTimeOver()
        {
            CloseSelf();

            if (parma.autonext)
            {
                if (parma.result == 1)
                {
                    Sys_Blessing.Instance.SendNetTakeAwardReq(parma.id);
                }

                int state = Sys_Blessing.Instance.GetState(parma.id);

                if (state < 0)
                    return;

                if (state != 1)
                {
                    Sys_Blessing.Instance.RuningAutoID = parma.id;
                    Sys_Blessing.Instance.SendNetStartReq(parma.id);
                }
                else if (state == 1)
                {
                    var data = CSVBless.Instance.GetConfData(parma.id);
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(data.npcID);
                }

            }

        }
    }
}
