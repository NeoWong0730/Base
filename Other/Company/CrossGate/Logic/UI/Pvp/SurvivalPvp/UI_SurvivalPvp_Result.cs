using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Packet;

namespace Logic
{
    public partial class UI_SurvivalPvp_Result : UIBase, UI_SurvivalPvp_Result_Layout.IListener
    {
        private UI_SurvivalPvp_Result_Layout m_Layout = new UI_SurvivalPvp_Result_Layout();

      
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnShow()
        {
            if (Sys_SurvivalPvp.Instance.FightEndInfo == null)
                return;

            int newScore = (int)Sys_SurvivalPvp.Instance.FightEndInfo.Newscore;
            int lastScore = (int)Sys_SurvivalPvp.Instance.FightEndInfo.Oldscore;

            var curScore = newScore - lastScore;

            m_Layout.SetScore(curScore, Sys_SurvivalPvp.Instance.FightEndInfo.Newscore);

            m_Layout.SetSuccess(Sys_SurvivalPvp.Instance.FightEndInfo.Result);

            int itemidscount = Sys_SurvivalPvp.Instance.FightEndInfo.ItemIds.Count;

            m_Layout.m_TransReward.gameObject.SetActive(itemidscount > 0);

            if (itemidscount > 0)
            {
                m_Layout.m_RewardGroup.SetChildSize(itemidscount);

                var info = Sys_SurvivalPvp.Instance.FightEndInfo;

                for (int i = 0; i < itemidscount; i++)
                {
                    var item = m_Layout.m_RewardGroup.getAt(i);
                    if (item != null)
                    {
                        item.SetItem(info.ItemIds[i], info.ItemNums[i]);
                    }
                }

                uint times = Sys_SurvivalPvp.Instance.GetHightRewardTimes();


                m_Layout.m_TexTimes.text = LanguageHelper.GetTextContent(2022443, times.ToString());
            }
           
        }

        public void OnClickClose()
        {
            CloseSelf();
        }
    }

}
