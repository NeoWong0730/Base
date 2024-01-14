using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
namespace Logic
{ 
    public partial class UI_PvpSingle_RankReward : UIBase, UI_PvpSingle_RankReward_Layout.IListener
    {
        private UI_PvpSingle_RankReward_Layout m_Layout = new UI_PvpSingle_RankReward_Layout();

        private int m_Stage = -1;
        List<CSVArenaRankingAward.Data>  m_RankData = new List<CSVArenaRankingAward.Data>(15);

        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnShow()
        {            
            RefreshRank();
        }

        private void RefreshRank()
        {
            RefreshRankData();

            int count = m_RankData.Count;

            m_Layout.SetRewardSize(count);

            for (int i = 0; i < count; i++)
            {
                var dataItem = m_RankData[i];
                var items = CSVDrop.Instance.GetDropItem(dataItem.Reward);

                m_Layout.SetReward(i, items);

                if (i > 2)
                {
                    if (dataItem.Ranking != null && dataItem.Ranking.Count > 0)
                    {
                        m_Layout.SetRewardTitle(i, LanguageHelper.GetTextContent(10142, dataItem.Ranking[0].ToString(), dataItem.Ranking[1].ToString()));
                    }
                    else if (dataItem.RankingPer != null && dataItem.RankingPer.Count > 0)
                    {
                        uint value = dataItem.RankingPer[1] / 100;

                        string strtex = value < 100 ? LanguageHelper.GetTextContent(10143, value.ToString()) : LanguageHelper.GetTextContent(10150);

                        m_Layout.SetRewardTitle(i, strtex);
                    }
                }
            }

        }


        private void RefreshRankData()
        {
            int stage = Sys_Pvp.Instance.GetLevelStage();

            if (stage == m_Stage)
                return;

            m_Stage = stage;

            var datas = CSVArenaRankingAward.Instance.GetAll(); 

            m_RankData.Clear();

            foreach (var kvp in datas)
            {
                if(kvp.Division == stage)
                {
                    m_RankData.Add(kvp);
                }
            }

        }
    }

    public partial class UI_PvpSingle_RankReward : UIBase, UI_PvpSingle_RankReward_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }
    }

}
