using Framework;
using Logic.Core;
using System;
using Table;
namespace Logic
{
    public class UI_Underground_Rank : UIBase, UI_Underground_Rank_Layout.IListener
    {
        private UI_Underground_Rank_Layout m_Layout = new UI_Underground_Rank_Layout();

        private uint m_SelectInstance = 0;

        private int m_CurTypeIndex = 0;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnOpen(object arg)
        {
            Sys_Instance_UGA.Instance.SendRankInfo(Sys_Instance_UGA.Instance.CurInstance);
        }
        protected override void OnShow()
        {
            m_SelectInstance = Sys_Instance_UGA.Instance.CurInstance;

            m_CurTypeIndex = Sys_Instance_UGA.Instance.GetInstanceIndex(m_SelectInstance);

            m_Layout.m_TogListChoice[m_CurTypeIndex].SetSelected(true, true);

            OnRefresh();

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Instance_UGA.Instance.eventEmitter.Handle(Sys_Instance_UGA.EEvents.RankUpdate, OnRefresh, toRegister);
        }

        private void OnRefresh()
        {
           int count = Sys_Instance_UGA.Instance.RankInfo == null ? 0 : Sys_Instance_UGA.Instance.RankInfo.Units.Count;

            m_Layout.m_CardInfinity.CellCount = count;
            m_Layout.m_CardInfinity.ForceRefreshActiveCell();
            m_Layout.m_CardInfinity.MoveToIndex(0);


            RefreshPlayerRank();

            m_Layout.m_TransNoRank.gameObject.SetActive(count == 0);
        }
        public void OnClickClose()
        {
            CloseSelf();
        }

        private void RefreshPlayerRank()
        {
            var result = Sys_Instance_UGA.Instance.RankInfo == null ? null : Sys_Instance_UGA.Instance.RankInfo.Units.Find(o => o.UnderGroundData.RoleId == Sys_Role.Instance.RoleId);

            m_Layout.m_PlayerRank.m_TexName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();

            if (result == null)
            {
                m_Layout.m_PlayerRank.m_ImgRank.gameObject.SetActive(false);
                m_Layout.m_PlayerRank.m_TexNoRank.gameObject.SetActive(true);
                m_Layout.m_PlayerRank.m_TexRank.text = string.Empty;
                m_Layout.m_PlayerRank.m_TexTime.text = string.Empty;
                m_Layout.m_PlayerRank.m_TexType.text = LanguageHelper.GetTextContent(14059u);
            }
            else
            {
                var stageid = result.Score;
                var dailyData = CSVInstanceDaily.Instance.GetConfData(stageid);

                m_Layout.m_PlayerRank.m_ImgRank.gameObject.SetActive(true);
                m_Layout.m_PlayerRank.m_TexNoRank.gameObject.SetActive(false);

                m_Layout.m_PlayerRank.m_TexRank.text = result.Rank.ToString();

                m_Layout.m_PlayerRank.m_TexType.text = LanguageHelper.GetTextContent(dailyData.Name);

                DateTime time = TimeManager.GetDateTime(result.UnderGroundData.Time);
                m_Layout.m_PlayerRank.m_TexTime.text = LanguageHelper.GetTextContent(11903u, time.Month.ToString(), time.Day.ToString(), time.Hour.ToString(), time.Minute.ToString());

                m_Layout.m_PlayerRank.m_ImgRank.gameObject.SetActive(result.Rank < 4);

                if (result.Rank < 4)
                {
                    ImageHelper.SetIcon(m_Layout.m_PlayerRank.m_ImgRank, 993900u + (uint)result.Rank);
                    m_Layout.m_PlayerRank.m_TexRank.text = string.Empty;
                }
                

            }
        }
        public void OnInfinityChange(InfinityGridCell cell, int index)
        {
            var rankdata = Sys_Instance_UGA.Instance.RankInfo.Units[index].UnderGroundData;

            var stageid = Sys_Instance_UGA.Instance.RankInfo.Units[index].Score;

            var uiRankItem = cell.mUserData as UI_Underground_Rank_Layout.RankInfoCard;

            uiRankItem.m_TexName.text = rankdata.Name.ToStringUtf8();
            uiRankItem.m_TexRank.text = (index + 1).ToString();

            var dailyData = CSVInstanceDaily.Instance.GetConfData(stageid);
            uiRankItem.m_TexType.text = dailyData == null ? string.Empty : LanguageHelper.GetTextContent(dailyData.Name);

            DateTime time = TimeManager.GetDateTime(rankdata.Time);

            uiRankItem.m_TexTime.text = LanguageHelper.GetTextContent(11903u, time.Month.ToString(), time.Day.ToString(), time.Hour.ToString(), time.Minute.ToString());

            uiRankItem.m_ImgRank.gameObject.SetActive(index < 3);

            if (index < 3)
            {
                ImageHelper.SetIcon(uiRankItem.m_ImgRank, 993901u + (uint)index);
                uiRankItem.m_TexRank.text = string.Empty;
            }


        }

        public void OnInfinityCreate(InfinityGridCell cell)
        {
            UI_Underground_Rank_Layout.RankInfoCard info = new UI_Underground_Rank_Layout.RankInfoCard();

            info.Load(cell.mRootTransform);

            cell.BindUserData(info);
        }

        public void OnToggleChange(int curid, int lastid)
        {
           uint id = Sys_Instance_UGA.Instance.GetInstanceID(curid);

            if (id == m_SelectInstance)
                return;

            Sys_Instance_UGA.Instance.SendRankInfo(id);

            m_SelectInstance = id;
        }
    }
}