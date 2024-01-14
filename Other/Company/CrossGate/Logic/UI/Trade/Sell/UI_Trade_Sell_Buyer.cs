using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;
using Lib.Core;
using System;

namespace Logic
{
    public class UI_Trade_Sell_Buyer
    {
        private class Cell
        {
            private Transform transform;

            private Button m_BtnCell;
            private Image m_ImgIcon;
            private Text m_TextName;
            private Image m_ImgCarrier;
            private Text m_TextCarrier;
            private Text m_TextLevel;

            private Sys_Society.RoleInfo m_RoleInfo;

            public void Init(Transform trans)
            {
                transform = trans;

                m_BtnCell = transform.Find("Image_BG").GetComponent<Button>();
                m_BtnCell.onClick.AddListener(OnClickCell);

                m_ImgIcon = transform.Find("Image_BG/Head").GetComponent<Image>();
                m_TextName = transform.Find("Text_Name").GetComponent<Text>();
                m_ImgCarrier = transform.Find("Image_Prop").GetComponent<Image>();
                m_TextCarrier = transform.Find("Text_Profession").GetComponent<Text>();
                m_TextLevel = transform.Find("Text_Number").GetComponent<Text>();
            }

            private void OnClickCell()
            {
                Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnAssingBuyerNtf, m_RoleInfo);
            }

            public void UpdateInfo(Sys_Society.RoleInfo roleInfo)
            {
                m_RoleInfo = roleInfo;
                CharacterHelper.SetHeadAndFrameData(m_ImgIcon, m_RoleInfo.heroID, m_RoleInfo.iconId, m_RoleInfo.iconFrameId);
                m_TextName.text = m_RoleInfo.roleName;

                uint acrrierIconId = OccupationHelper.GetIconID(m_RoleInfo.occ);
                if (acrrierIconId != 0u)
                {
                    m_ImgCarrier.gameObject.SetActive(true);
                    ImageHelper.SetIcon(m_ImgCarrier, acrrierIconId);
                    m_TextCarrier.text = LanguageHelper.GetTextContent(OccupationHelper.GetTextID(m_RoleInfo.occ, m_RoleInfo.rank));
                }
                else
                {
                    m_ImgCarrier.gameObject.SetActive(false);
                    m_TextCarrier.text = "";
                }

                m_TextLevel.text = LanguageHelper.GetTextContent(2011127, m_RoleInfo.level.ToString());

                ImageHelper.SetImageGray(m_ImgIcon, !m_RoleInfo.isOnLine);
            }
        }

        private Transform transform;

        private Button m_BtnOff;
        private InfinityGridLayoutGroup gridGroup;
        private int visualGridCount;
        private Dictionary<GameObject, Cell> dicCells = new Dictionary<GameObject, Cell>();

        private List<Sys_Society.RoleInfo> roleList = new List<Sys_Society.RoleInfo>();

        public void Init(Transform trans)
        {
            transform = trans;

            m_BtnOff = transform.Find("BG_off").GetComponent<Button>();
            m_BtnOff.onClick.AddListener(() => { Hide(); });

            gridGroup = transform.Find("Background_Root/Scroll_View/List").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 7;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                GameObject go = gridGroup.transform.GetChild(i).gameObject;

                Cell cell = new Cell();
                cell.Init(go.transform);
                dicCells.Add(go, cell);
            }
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);

            UpdateList();
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicCells.ContainsKey(trans.gameObject))
            {
                Cell cell = dicCells[trans.gameObject];
                cell.UpdateInfo(roleList[index]);
            }
        }

        private void UpdateList()
        {
            //roleList.Clear();
            if (Sys_Society.Instance.socialRecentlysInfo != null)
            {
                roleList = Sys_Society.Instance.socialRecentlysInfo.GetAllSortedRecentlysInfos();
            }
            visualGridCount = roleList.Count;
            gridGroup.SetAmount(visualGridCount);
        }
    }
}


