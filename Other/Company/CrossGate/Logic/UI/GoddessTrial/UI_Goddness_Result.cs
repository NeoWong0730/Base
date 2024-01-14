using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
namespace Logic
{
    public class UI_Goddness_Result_Parma
    {
        public uint InstanceID = 0;
        public uint LeveleID = 0;

        public bool IsPass = false;
    }
    public partial class UI_Goddness_Result:UIBase, UI_Goddness_Result_Layout.IListener
    {
        UI_Goddness_Result_Layout m_Layout = new UI_Goddness_Result_Layout();

        private UI_Goddness_Result_Parma m_parma = null;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnOpen(object arg)
        {
            m_parma = arg as UI_Goddness_Result_Parma;
        }
        protected override void OnShow()
        {
           

            OnResresh();
        }


        protected override void OnHide()
        {
            CloseSelf();
        }
        private void OnResresh()
        {

            int count = 0;
            if(Sys_GoddnessTrial.Instance.InstancePassAward != null)
                count = Sys_GoddnessTrial.Instance.InstancePassAward.AwardList.Count;

            m_Layout.SetRewardCount(count);

            for (int i = 0; i < count; i++)
            {
                var value = Sys_GoddnessTrial.Instance.InstancePassAward.AwardList[i];

                m_Layout.SetReward(i, value.Id, value.Count);
            }

            var langeuidInstance = GetStageIndexLangue();

            m_Layout.SetLevelInfo(LanguageHelper.GetTextContent(2022383, LanguageHelper.GetTextContent(langeuidInstance)));


        }


        private uint GetStageIndexLangue()
        {
            var data = Sys_GoddnessTrial.Instance.GetCurSelectTopicChapterIndex(m_parma.InstanceID);//csvc.Instance.GetConfData(m_Parma.LevelID);

            if (data == 0)
                return 0;

            return 2022354u + ((uint)data - 1);
        }
    }

    public partial class UI_Goddness_Result : UIBase, UI_Goddness_Result_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();

            if (Sys_Team.Instance.isCaptain() && Sys_Instance.Instance.IsInInstance)
                Sys_Instance.Instance.InstanceExitReq();
        }
    }
}
