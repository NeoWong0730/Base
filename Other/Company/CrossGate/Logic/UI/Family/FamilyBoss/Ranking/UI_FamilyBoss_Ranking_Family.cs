
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_FamilyBoss_Ranking_Family
    {
        public class Cell
        {
            public class RewardShow
            {
                private Transform transform;
                private Transform transTemplate;

                public void Init(Transform trans)
                {
                    transform = trans;

                    transTemplate = transform.Find("Viewport/PropItem");
                    transTemplate.gameObject.SetActive(false);
                }

                public void UpdateReward(uint dropId)
                {
                    Lib.Core.FrameworkTool.DestroyChildren(transTemplate.parent.gameObject, transTemplate.name);
                    List<ItemIdCount> list = CSVDrop.Instance.GetDropItem(dropId);
                    foreach (var data in list)
                    {
                        CSVDrop.Data dropData = CSVDrop.Instance.GetDropItemData(dropId);

                        GameObject go = GameObject.Instantiate(transTemplate.gameObject);
                        go.transform.SetParent(transTemplate.parent);
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localRotation = Quaternion.identity;
                        go.transform.localScale = transTemplate.localScale;
                        go.gameObject.SetActive(true);

                        PropItem propItem = new PropItem();
                        propItem.BindGameObject(go);

                        PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(data.id, data.count, true, false, false, false, false, true, false, true);
                        itemData.equipPara = dropData.equip_para;
                        propItem.SetData(itemData, EUIID.UI_FamilyBoss_Ranking);
                    }
                }
            }

            private Transform transform;

            private Image m_imgRank;
            private Text m_textRank;
            private Text m_textName;
            private Text m_textNum;
            private Text m_textScore;

            private RewardShow m_rewardShow;

            public void Init(Transform trans)
            {
                transform = trans;

                m_imgRank = transform.Find("Rank/Image_Icon").GetComponent<Image>();
                m_textRank = transform.Find("Rank/Text_Rank").GetComponent<Text>();
                m_textName = transform.Find("Text_Name").GetComponent<Text>();
                m_textNum = transform.Find("Text_Amount").GetComponent<Text>();
                m_textScore = transform.Find("Text_Score").GetComponent<Text>();

                m_rewardShow = new RewardShow();
                m_rewardShow.Init(transform.Find("Scroll_View"));
            }

            public void UpdateInfo(CmdRankGuild rankGuild, int index)
            {
                uint rank = (uint)index + 1;
                if (rank <= 3)
                {
                    m_imgRank.gameObject.SetActive(true);
                    ImageHelper.SetIcon(m_imgRank, 993900 + rank);
                    m_textRank.gameObject.SetActive(false);
                }
                else
                {
                    m_imgRank.gameObject.SetActive(false);
                    m_textRank.gameObject.SetActive(true);
                    m_textRank.text = rank.ToString();
                }

                m_textName.text = rankGuild.GuildName.ToStringUtf8();
                m_textNum.text = rankGuild.AttendNum.ToString();
                m_textScore.text = rankGuild.Score.ToString();

                CalRewardShow(rank);
            }

            private void CalRewardShow(uint rank)
            {
                uint dropId = 0u;
                var dataList = CSVFamilyBossFamilyReward.Instance.GetAll();
                for (int i = dataList.Count - 1; i >= 0; --i)
                {
                    if (rank >= dataList[i].Family_Rank)
                    {
                        dropId = dataList[i].Reward;
                        break;
                    }
                }
                m_rewardShow.UpdateReward(dropId);
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;


        private List<CmdRankGuild> m_listRankGuilds;

        private Image m_imgFamilyRank;
        private Text m_textFamilyRank;
        private Text m_textUnRank;
        private Text m_textFamilyName;
        private Text m_textAttendNum;
        private Text m_textFamilyScore;

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.Find("Scroll_Rank").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            m_imgFamilyRank = transform.Find("MyRank/Image_Icon").GetComponent<Image>();
            m_textFamilyRank = transform.Find("MyRank/Text_Rank").GetComponent<Text>();
            m_textUnRank = transform.Find("MyRank/Text_unlist").GetComponent<Text>();
            m_textFamilyName = transform.Find("MyRank/Text_Name").GetComponent<Text>();
            m_textAttendNum = transform.Find("MyRank/Text_Amount").GetComponent<Text>();
            m_textFamilyScore = transform.Find("MyRank/Text_Score").GetComponent<Text>();
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
            entry.UpdateInfo(m_listRankGuilds[index], index);
        }

        public void UpdateInfo()
        {
            m_listRankGuilds = Sys_FamilyBoss.Instance.GetRankGuilds();
            _infinityGrid.CellCount = m_listRankGuilds.Count;

            UpdateMyInfo();
        }

        private void UpdateMyInfo()
        {
            uint myRank = Sys_FamilyBoss.Instance.MyGuildRank;
            m_imgFamilyRank.gameObject.SetActive(false);
            m_textFamilyRank.gameObject.SetActive(false);
            m_textUnRank.gameObject.SetActive(false);
            if (myRank != 0)
            {
                if (myRank <= 3)
                {
                    m_imgFamilyRank.gameObject.SetActive(true);
                    ImageHelper.SetIcon(m_imgFamilyRank, 993900 + myRank);
                }
                else
                {
                    m_textFamilyRank.text = myRank.ToString();
                }
            }
            else
            {
                m_textUnRank.gameObject.SetActive(true);
            }

            m_textFamilyName.text = Sys_Family.Instance.GetFamilyName();
            m_textAttendNum.text = Sys_FamilyBoss.Instance.MyGuildAttendNum.ToString();
            m_textFamilyScore.text = Sys_FamilyBoss.Instance.MyGuildScore.ToString();
        }
    }
}