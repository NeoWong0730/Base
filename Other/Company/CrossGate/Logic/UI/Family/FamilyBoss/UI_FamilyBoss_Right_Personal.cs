using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_FamilyBoss_Right_Personal
    {
        public class Cell
        {
            private Transform transform;

            private Image m_imgRank;
            private Text m_textNumber;
            private Text m_textLv;
            private Text m_textName;
            private Text m_textFamilyName;
            private Text m_textScore;

            public void Init(Transform trans)
            {
                transform = trans;

                m_imgRank = transform.Find("Image_Rank").GetComponent<Image>();
                m_textNumber = transform.Find("Text_Number").GetComponent<Text>();
                m_textLv = transform.Find("Text_Lv").GetComponent<Text>();
                m_textName = transform.Find("Text_Name").GetComponent<Text>();
                m_textFamilyName = transform.Find("Text_Family").GetComponent<Text>();
                m_textScore = transform.Find("Text_Score").GetComponent<Text>();
            }

            public void UpdateInfo(CmdRankRole rankRole)
            {
                m_imgRank.gameObject.SetActive(false);
                m_textNumber.gameObject.SetActive(false);
                if (rankRole.Rank <= 3)
                {
                    m_imgRank.gameObject.SetActive(true);
                    ImageHelper.SetIcon(m_imgRank, 993900 + rankRole.Rank);
                }
                else
                {
                    m_textNumber.gameObject.SetActive(true);
                    m_textNumber.text = rankRole.Rank.ToString();
                }

                //m_textNumber.text = rankRole.Rank.ToString();
                m_textLv.text = rankRole.Level.ToString();
                m_textName.text = rankRole.RoleName.ToStringUtf8();
                m_textFamilyName.text = rankRole.GuildName.ToStringUtf8();
                m_textScore.text = rankRole.Score.ToString();
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;
        //private Lib.Core.CoroutineHandler handler;
        //private Dictionary<GameObject, UI_Mall_ShopItem> dicCells = new Dictionary<GameObject, UI_Mall_ShopItem>();
        //private int visualGridCount;

        private List<CmdRankRole> m_listRoles;

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.Find("Scroll_Rank").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
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
            if (index < m_listRoles.Count)
                entry.UpdateInfo(m_listRoles[index]);
        }

        public void UpdateInfo()
        {
            m_listRoles = Sys_FamilyBoss.Instance.GetRankRoles();

            _infinityGrid.CellCount = m_listRoles.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);
        }
    }
}