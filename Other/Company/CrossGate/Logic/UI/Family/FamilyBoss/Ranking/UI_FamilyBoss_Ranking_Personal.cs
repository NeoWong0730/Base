
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_FamilyBoss_Ranking_Personal
    {
        public class Cell
        {
            private Transform transform;

            private Image m_imgRank;
            private Text m_textRank;
            private Image m_imgOcc;
            private Text m_textOcc;
            private Text m_textName;
            private Text m_textScore;
            private Text m_textFamilyName;

            public void Init(Transform trans)
            {
                transform = trans;

                m_imgRank = transform.Find("Rank/Image_Icon").GetComponent<Image>();
                m_textRank = transform.Find("Rank/Text_Rank").GetComponent<Text>();
                m_imgOcc = transform.Find("Image_Prop").GetComponent<Image>();
                m_textOcc = transform.Find("Image_Prop/Text_Profession").GetComponent<Text>();
                m_textName = transform.Find("Text_Name").GetComponent<Text>();
                m_textScore = transform.Find("Text_Score").GetComponent<Text>();
                m_textFamilyName = transform.Find("Text_Family").GetComponent<Text>();
            }

            public void UpdateInfo(CmdRankRole rankRole)
            {
                if (rankRole.Rank <= 3)
                {
                    m_imgRank.gameObject.SetActive(true);
                    ImageHelper.SetIcon(m_imgRank, 993900 + rankRole.Rank);
                    m_textRank.gameObject.SetActive(false);
                }
                else
                {
                    m_imgRank.gameObject.SetActive(false);
                    m_textRank.gameObject.SetActive(true);
                    m_textRank.text = rankRole.Rank.ToString();
                }

                ImageHelper.SetIcon(m_imgOcc, OccupationHelper.GetCareerLogoIcon(rankRole.Occ));
                m_textOcc.text = LanguageHelper.GetTextContent(OccupationHelper.GetTextID(rankRole.Occ));
                m_textName.text = rankRole.RoleName.ToStringUtf8();
                m_textScore.text = rankRole.Score.ToString();
                m_textFamilyName.text = rankRole.GuildName.ToStringUtf8();
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;

        private List<CmdRankRole> m_listRoles;

        private Image m_imgMyRank;
        private Text m_textMyRank;
        private Text m_textUnRank;
        private Image m_imgMyOcc;
        private Text m_textMyOcc;
        private Text m_textMyName;
        private Text m_textMyScore;
        private Text m_textMyFamily;

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.Find("Scroll_Rank").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            m_imgMyRank = transform.Find("MyRank/Image_Icon").GetComponent<Image>();
            m_textMyRank = transform.Find("MyRank/Text_Rank").GetComponent<Text>();
            m_textUnRank = transform.Find("MyRank/Text_unlist").GetComponent<Text>();
            m_imgMyOcc = transform.Find("MyRank/Image_Prop").GetComponent<Image>();
            m_textMyOcc = transform.Find("MyRank/Image_Prop/Text_Profession").GetComponent<Text>();
            m_textMyName = transform.Find("MyRank/Text_Name").GetComponent<Text>();
            m_textMyScore = transform.Find("MyRank/Text_Point").GetComponent<Text>();
            m_textMyFamily = transform.Find("MyRank/Text_Family").GetComponent<Text>();
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
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            Cell entry = cell.mUserData as Cell;
            entry.UpdateInfo(m_listRoles[index]);
        }

        public void UpdateInfo()
        {
            m_listRoles = Sys_FamilyBoss.Instance.GetRankRoles();

            _infinityGrid.CellCount = m_listRoles.Count;

            UpdateMyInfo();
        }

        private void UpdateMyInfo()
        {
            uint myRank = Sys_FamilyBoss.Instance.MyRank;
            m_imgMyRank.gameObject.SetActive(false);
            m_textMyRank.gameObject.SetActive(false);
            m_textUnRank.gameObject.SetActive(false);
            if (myRank != 0)
            {
                if (myRank <= 3)
                {
                    m_imgMyRank.gameObject.SetActive(true);
                    ImageHelper.SetIcon(m_imgMyRank, 993900 + myRank);
                }
                else
                {
                    m_textMyRank.gameObject.SetActive(true);
                    m_textMyRank.text = myRank.ToString();
                }
            }
            else
            {
                m_textUnRank.gameObject.SetActive(true);
            }

            ImageHelper.SetIcon(m_imgMyOcc, OccupationHelper.GetCareerLogoIcon(Sys_Role.Instance.Role.Career));
            m_textMyOcc.text = LanguageHelper.GetTextContent(OccupationHelper.GetTextID(Sys_Role.Instance.Role.Career));
            m_textMyName.text = Sys_Role.Instance.sRoleName;
            m_textMyScore.text = Sys_FamilyBoss.Instance.MyScore.ToString();
            m_textMyFamily.text = Sys_Family.Instance.GetFamilyName();
        }
    }
}