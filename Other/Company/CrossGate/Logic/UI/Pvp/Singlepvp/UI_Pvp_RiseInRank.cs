using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Packet;

namespace Logic
{
    /// <summary>
    /// 晋级奖励
    /// </summary>
    public partial class UI_Pvp_RiseInRank : UIBase, UI_Pvp_RiseInRank_Layout.IListener
    {
        UI_Pvp_RiseInRank_Layout m_Layout = new UI_Pvp_RiseInRank_Layout();

       // private int m_Stage = -1;

        private List<CSVArenaRandAward.Data>  m_RankData = new List<CSVArenaRandAward.Data>();

        private List<ArenaDanInfo> m_DanRewardInfo = new List<ArenaDanInfo>();
        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Pvp.Instance.eventEmitter.Handle(Sys_Pvp.EEvents.DanLvUpAward,OnRankRefresh, toRegister);

            Sys_Pvp.Instance.eventEmitter.Handle<ArenaDanInfo>(Sys_Pvp.EEvents.GetDanLvUpAward, OnGetDanLvUpAward, toRegister);

            Sys_Pvp.Instance.eventEmitter.Handle<List<ArenaDanInfo>>(Sys_Pvp.EEvents.GetAllDanLvUpAward, OnGetAllDanLvUpAward,toRegister);
        }

        protected override void OnOpen(object arg)
        {            
            Sys_Pvp.Instance.Apply_RiseInRank();
        }
        protected override void OnShow()
        {            
            RefreshInfo();
        }

        private void RefreshData()
        {
            m_DanRewardInfo.Clear();

            if (Sys_Pvp.Instance.DanLvUpAwardList == null)
                return;

            m_DanRewardInfo.AddRange(Sys_Pvp.Instance.DanLvUpAwardList);

            m_DanRewardInfo.Sort((x, y) => {

                var xdata = CSVArenaSegmentInformation.Instance.GetConfData((uint)x.DanLv);
                var ydata = CSVArenaSegmentInformation.Instance.GetConfData((uint)y.DanLv);

                if ((x.IsGet && y.IsGet)|| (!x.IsGet && !y.IsGet))
                {
                    return NormalSort(xdata,ydata);
                }

                if (x.IsGet && !y.IsGet)
                    return 1;

                if (!x.IsGet && y.IsGet)
                    return -1;

                return 0;
            });
        }

        private int NormalSort(CSVArenaSegmentInformation.Data data0, CSVArenaSegmentInformation.Data data1)
        {
            if (data0.Rank > data1.Rank)
                return 1;

            if (data0.Rank < data1.Rank)
                return -1;

            if (data0.RankSubordinate > data1.RankSubordinate)
                return 1;

            if (data0.RankSubordinate < data1.RankSubordinate)
                return -1;

            return 0;
        }
        private bool RefreshInfo()
        {
            RefreshData();

            int count = m_DanRewardInfo.Count;

            m_Layout.SetRankSize(count);

            if (count == 0)
                return false;

            for (int i = 0; i < count; i++)
            {
                m_Layout.SetReward(i, CSVDrop.Instance.GetDropItem((uint)m_DanRewardInfo[i].DropId));

                var data = CSVArenaSegmentInformation.Instance.GetConfData((uint)m_DanRewardInfo[i].DanLv);

                m_Layout.SetRankItemName(i,LanguageHelper.GetTextContent(data.RankDisplay));

                bool state = m_DanRewardInfo[i].CanGet && !m_DanRewardInfo[i].IsGet;

                m_Layout.SetGetActive(i, !m_DanRewardInfo[i].IsGet, m_DanRewardInfo[i].CanGet);

                m_Layout.SetHadGetActive(i, m_DanRewardInfo[i].IsGet);

                int stateLaid = /*m_DanRewardInfo[i].IsGet ? 10158 :*/ 10157;

                m_Layout.SetGetStateTex(i,LanguageHelper.GetTextContent((uint)stateLaid));

                m_Layout.SetConfigID(i, m_DanRewardInfo[i].DanLv);
            }


            return true;
        }

        private void RefreshSelect()
        {
            int count = Sys_Pvp.Instance.DanLvUpAwardList == null ? 0 : Sys_Pvp.Instance.DanLvUpAwardList.Count;

            int lastHadIndex = -1;
            int lastcanGetIndex = -1;
            CSVArenaSegmentInformation.Data lastcanGetdata = null;

            for (int i = 0; i < count; i++)
            {
                var data = CSVArenaSegmentInformation.Instance.GetConfData((uint)Sys_Pvp.Instance.DanLvUpAwardList[i].DanLv);

                bool state = Sys_Pvp.Instance.DanLvUpAwardList[i].CanGet && !Sys_Pvp.Instance.DanLvUpAwardList[i].IsGet;

                if (state && (lastcanGetdata == null || isLowRankReward(lastcanGetdata, data)))
                {
                    lastcanGetdata = data;

                    lastcanGetIndex = i;
                }

                if (Sys_Pvp.Instance.DanLvUpAwardList[i].CanGet && Sys_Pvp.Instance.DanLvUpAwardList[i].IsGet)
                {
                    lastHadIndex = i;
                }
            }

            if (lastHadIndex >= 0 || lastcanGetIndex >= 0)
            {
                int selectIndex = lastHadIndex >= 0 ? (lastcanGetIndex >= 0 ? lastcanGetIndex : lastHadIndex) : lastcanGetIndex;

                m_Layout.SetSelectRank(selectIndex);
            }
        }
        private bool isLowRankReward(CSVArenaSegmentInformation.Data last, CSVArenaSegmentInformation.Data cur)
        {
            if (cur.Rank > last.Rank)
                return false;

            if (cur.Rank == last.Rank && cur.RankSubordinate >= last.RankSubordinate)
                return false;

            return true;
        }
    }

    /// <summary>
    /// 监听处理
    /// </summary>
    public partial class UI_Pvp_RiseInRank : UIBase, UI_Pvp_RiseInRank_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickOneKeyGet()
        {
            Sys_Pvp.Instance.Apply_GetAllDanUpAward();
        }

        public void OnClickRankItemGet(int index)
        {
            Sys_Pvp.Instance.Apply_GetDanLvUpAward((uint)index);
        }


        private void OnRankRefresh()
        {
             bool result = RefreshInfo();

            int count = Sys_Pvp.Instance.DanLvUpAwardList == null ? 0 : Sys_Pvp.Instance.DanLvUpAwardList.Count;

            if (count == 0 || result)
                return;

            for (int i = 0; i < count; i++)
            {
                //bool state = Sys_Pvp.Instance.DanLvUpAwardList[i].CanGet && !Sys_Pvp.Instance.DanLvUpAwardList[i].IsGet;
                //m_Layout.SetGetActive(i, state);

                m_Layout.SetGetActive(i, !Sys_Pvp.Instance.DanLvUpAwardList[i].IsGet, Sys_Pvp.Instance.DanLvUpAwardList[i].CanGet);
                m_Layout.SetHadGetActive(i, Sys_Pvp.Instance.DanLvUpAwardList[i].IsGet);

                int stateLaid = /*Sys_Pvp.Instance.DanLvUpAwardList[i].IsGet ? 10158 :*/ 10157;

                m_Layout.SetGetStateTex(i, LanguageHelper.GetTextContent((uint)stateLaid));
            }
        }

        private void OnGetDanLvUpAward(ArenaDanInfo value)
        {
            if (value == null)
                return;

           int index = m_Layout.GetRankItemIndex((int)value.DanLv);

            int stateLaid = /*value.IsGet ? 10158 :*/ 10157;


            //bool state = value.CanGet && !value.IsGet;
            //m_Layout.SetGetActive(index, state);


            m_Layout.SetGetActive(index, !value.IsGet, value.CanGet);
            m_Layout.SetHadGetActive(index, value.IsGet);

            m_Layout.SetGetStateTex(index, LanguageHelper.GetTextContent((uint)stateLaid));
        }


        private void OnGetAllDanLvUpAward(List<ArenaDanInfo> value)
        {
            int count = value.Count;

            for (int i = 0; i < count; i++)
            {
                OnGetDanLvUpAward(value[i]);
            }
        }
    }
}
