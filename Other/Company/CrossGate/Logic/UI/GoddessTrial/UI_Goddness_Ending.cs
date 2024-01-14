using System;
using System.Collections.Generic;

using UnityEngine;

using Logic.Core;

using Table;
namespace Logic
{
    public class UI_Goddness_Ending_Parma
    {
        public uint InstanceID = 0;
        public uint LeveleID = 0;

        public bool IsPass = false;

        public uint Topic = 0;

        public uint EndId = 0;

        public bool isNextShowAward = true;
    }
   
    public partial class UI_Goddness_Ending :UIBase, UI_Goddness_Ending_Layout.IListener
    {
        UI_Goddness_Ending_Layout m_Layout = new UI_Goddness_Ending_Layout();

        uint m_EndID = 0;

        private UI_Goddness_Ending_Parma m_Parma = null;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnOpen(object arg)
        {
            m_Parma = arg as UI_Goddness_Ending_Parma;

            m_EndID = m_Parma.EndId;
        }
        protected override void OnShow()
        {
           
            Refresh();
        }

        protected override void OnHide()
        {
            CloseSelf();
        }

        protected override void OnClose()
        {            
            m_EndID = 0;
        }

        private void Refresh()
        {
            CSVGoddessEnd.Data endData = null;

            if (m_EndID == 0)
            {
                var data = CSVGoddessEnd.Instance.GetAll();

                foreach (var kvp in data)
                {
                    int resultIndex = kvp.stageId.FindIndex(o => o == Sys_GoddnessTrial.Instance.SelectStageID);

                    if (resultIndex >= 0)
                    {
                        endData = kvp;
                        break;
                    }
                }
            }
            else
            {
                endData = CSVGoddessEnd.Instance.GetConfData(m_EndID);
            }


            if (endData == null)
                return;

            var topicLan = endData.endingName;
            m_Layout.SetTitle(topicLan);
           // m_Layout.SetTitle(LanguageHelper.GetTextContent(2022383,LanguageHelper.GetTextContent(topicLan)));

            m_Layout.Setdiscrib(endData.endingLan);
            m_Layout.SetEndingImage(endData.endingTexture);

          
        }
    }

    public partial class UI_Goddness_Ending : UIBase, UI_Goddness_Ending_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();


            if (m_Parma.isNextShowAward == false)
            {
                return;
            }

            UI_Goddness_Result_Parma parma = new UI_Goddness_Result_Parma();

            parma.InstanceID = m_Parma.InstanceID;
            parma.LeveleID = m_Parma.LeveleID;
            parma.IsPass = m_Parma.IsPass;

            UIManager.OpenUI(EUIID.UI_Goddess_Result,false,parma);
        }
    }
}
