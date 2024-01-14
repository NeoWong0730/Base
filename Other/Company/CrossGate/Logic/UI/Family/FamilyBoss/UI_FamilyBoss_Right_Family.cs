
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_FamilyBoss_Right_Family
    {
        public class Cell
        {
            private Transform transform;

            private Image m_imgRank;
            private Transform m_transRank;
            private Text m_textRank;
            private Text m_textName;
            private Text m_textHead;
            private Text m_textScore;
            private Text m_textNumber;

            public void Init(Transform trans)
            {
                transform = trans;

                m_imgRank = transform.Find("Image_Rank").GetComponent<Image>();
                m_transRank = transform.Find("Text_Rank");
                m_textRank = transform.Find("Text_Rank/Text").GetComponent<Text>();
                m_textName = transform.Find("Text_Name/Text").GetComponent<Text>();
                m_textHead = transform.Find("Text_Head/Text").GetComponent<Text>();
                m_textScore = transform.Find("Text_Score/Text").GetComponent<Text>();
                m_textNumber = transform.Find("Text_Number/Text").GetComponent<Text>();
            }

            public void UpdateInfo(CmdRankGuild rankGuild, int rankIndex)
            {
                int rank = rankIndex + 1;
                if (rank <= 3)
                {
                    m_transRank.gameObject.SetActive(false);
                    m_imgRank.gameObject.SetActive(true);
                    ImageHelper.SetIcon(m_imgRank, 993900u + (uint)rank);
                }
                else
                {
                    m_imgRank.gameObject.SetActive(false);
                    m_transRank.gameObject.SetActive(true);
                    m_textRank.text = rank.ToString();
                }

                m_textName.text = rankGuild.GuildName.ToStringUtf8();
                m_textHead.text = rankGuild.LeaderName.ToStringUtf8();
                m_textScore.text = rankGuild.Score.ToString();
                m_textNumber.text = rankGuild.AttendNum.ToString();
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;
        private List<CmdRankGuild> m_listRankGuilds;
        private Image m_imgRank;
        private Text m_textRankParent;
        private Text m_textRank;
        private Text m_textUnRank;
        private Text m_textName;
        private Text m_textHead;
        private Text m_textScore;
        private Text m_textAttendNum;

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.Find("Scroll_Rank").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            m_imgRank = transform.Find("My/Image_Rank").GetComponent<Image>();
            m_textRankParent = transform.Find("My/Text_Rank").GetComponent<Text>();
            m_textRank = transform.Find("My/Text_Rank/Text").GetComponent<Text>();
            m_textUnRank = transform.Find("My/Text_Rank/Text1").GetComponent<Text>();
            m_textName = transform.Find("My/Text_Name/Text").GetComponent<Text>();
            m_textHead = transform.Find("My/Text_Head/Text").GetComponent<Text>();
            m_textScore = transform.Find("My/Text_Score/Text").GetComponent<Text>();
            m_textAttendNum = transform.Find("My/Text_Number/Text").GetComponent<Text>();
        }

        public void OnShow()
        {
            transform.gameObject.SetActive(true);
        }

        public void OnHide()
        {
            transform.gameObject.SetActive(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            Cell entry = new Cell();

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);

            //dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            Cell entry = cell.mUserData as Cell;
            if (index < m_listRankGuilds.Count)
                entry.UpdateInfo(m_listRankGuilds[index], index);
        }

        public void UpdateInfo()
        {
            m_listRankGuilds = Sys_FamilyBoss.Instance.GetRankGuilds();
            _infinityGrid.CellCount = m_listRankGuilds.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);

            UpdateMyFamilyInfo();
        }

        private void UpdateMyFamilyInfo()
        {
            uint guildRank = Sys_FamilyBoss.Instance.MyGuildRank;
            m_imgRank.gameObject.SetActive(false);
            m_textRankParent.gameObject.SetActive(false);
            if (guildRank != 0)
            {
                if (guildRank <= 3)
                {
                    m_imgRank.gameObject.SetActive(true);
                    ImageHelper.SetIcon(m_imgRank, 993900 + guildRank);
                }
                else
                {
                    m_textRankParent.gameObject.SetActive(true);
                    m_textUnRank.gameObject.SetActive(false);
                    m_textRank.text = guildRank.ToString();
                }
            }
            else
            {
                m_textRankParent.gameObject.SetActive(true);
                m_textUnRank.gameObject.SetActive(true);
                m_textRank.text = "";
            }


            m_textName.text = Sys_Family.Instance.GetFamilyName();
            m_textHead.text = Sys_Family.Instance.GetFamilyHeadName();
            m_textScore.text = Sys_FamilyBoss.Instance.MyGuildScore.ToString();
            m_textAttendNum.text = Sys_FamilyBoss.Instance.MyGuildAttendNum.ToString();
        }
    }
}