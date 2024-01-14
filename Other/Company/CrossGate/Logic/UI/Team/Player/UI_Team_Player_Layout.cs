using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using Framework;

namespace Logic
{
    public partial class UI_Team_Player_Layout
    {
        private Transform m_PlayerListTransform;
        private Transform m_PlayerInfoTransform;

        private Transform m_PlayerItemTransform;
        private Transform m_PlayerItemParentTransform;

        private Transform m_FamilyInfoTransform;

        private Button m_BtnClose;
        #region class PlayerItem
        public class PlayerItem
        {
            public Transform transform;

            public Text m_TexName;

            public Image m_IIcon;

            public Image m_IIconFrame;

            public Text m_TexLevel;
            public string Name { set { if (string.IsNullOrEmpty(value) == false) TextHelper.SetText(m_TexName, value); } }
            public uint Icon { set { if (value > 0) ImageHelper.SetIcon(m_IIcon, value); } }
            public uint IconFrame
            {
                set
                {
                    if (value == 0)
                    {
                        m_IIconFrame.gameObject.SetActive(false);
                    }
                    else
                    {
                        m_IIconFrame.gameObject.SetActive(true);
                        ImageHelper.SetIcon(m_IIconFrame, value);
                    }
                }
            }

            private bool m_Active;
            public bool Active { get { return m_Active; } set { m_Active = value; transform.gameObject.SetActive(value); } }

            public int Index { get; set; }

            public Action<int> ClickAction;
            public void OnClick()
            {
                ClickAction?.Invoke(Index);
            }

            public void SetLevel(uint level)
            {
                m_TexLevel.text = "Lv."+level.ToString();
            }
        }
        #endregion

        private List<PlayerItem> m_PlayerItems = new List<PlayerItem>();

        public ClickItemEvent ClickPlayerItem = new ClickItemEvent();


        IListener mlistener;
        public interface IListener
        {
            void ClosePlayerInfo();

            void OnClickClose();

            void OnClickSetStatus();
            // void OnClickCommand(int index);
        }

        public void Loaded(Transform root)
        {
            m_BtnClose = root.Find("close").GetComponent<Button>();
            m_BtnClose.onClick.AddListener(OnClickClose);

            m_PlayerListTransform = root.Find("Team_Teammate");
            m_PlayerItemParentTransform = root.Find("Team_Teammate/Scroll View/Viewport/Grid");
            m_PlayerItemTransform = root.Find("Team_Teammate/Scroll View/Viewport/Grid/Image_Teammate01");

            m_PlayerInfoTransform = root.Find("Team_Player");
            m_FamilyInfoTransform = root.Find("View_Family");

            m_PlayerItemTransform.gameObject.SetActive(false);
            m_FamilyInfoTransform.gameObject.SetActive(false);
            LoadRoleInfo(root);
            LoadFamilyInfo(root);
        }

        public void ProcessEvents(IListener listener)
        {
            //  m_Close.onClick.AddListener(listener.ClosePlayerInfo);

            m_Close1.onClick.AddListener(listener.ClosePlayerInfo);

            m_SetStatus.onClick.AddListener(listener.OnClickSetStatus);

            mlistener = listener;
        }
        private void LoadPlayerItem(Transform transform, PlayerItem item)
        {
            item.transform = transform;

            item.m_IIcon = transform.Find("Head").GetComponent<Image>();

            item.m_IIconFrame = transform.Find("Head/Image_Before_Frame").GetComponent<Image>();

            item.m_TexName = transform.Find("Text").GetComponent<Text>();

            item.m_TexLevel = transform.Find("Image_Icon/Text_Number").GetComponent<Text>();

            Button btn = transform.GetComponent<Button>();
            btn.onClick.AddListener(item.OnClick);

            item.ClickAction = OnClickPlayerItem;
        }

        private void OnClickClose()
        {

            mlistener?.OnClickClose();
        }
        public void SetPlayerSize(int size)
        {
            if (m_PlayerItems.Count < size)
            {
                int offsize = size - m_PlayerItems.Count;
                for (int i = 0; i < offsize; i++)
                {
                    PlayerItem member = ClonePlayerItem();
                    m_PlayerItems.Add(member);
                }
            }

            for (int i = 0; i < m_PlayerItems.Count; i++)
            {
                m_PlayerItems[i].Active = (i < size);

                m_PlayerItems[i].Index = i;

                m_PlayerItems[i].transform.SetSiblingIndex(i + 1);
            }
        }

        private PlayerItem ClonePlayerItem()
        {
            GameObject item = GameObject.Instantiate<GameObject>(m_PlayerItemTransform.gameObject);

            item.transform.SetParent(m_PlayerItemParentTransform, false);

            PlayerItem member = new PlayerItem();

            LoadPlayerItem(item.transform, member);

            return member;
        }

        public PlayerItem getAtPlayerItem(int index)
        {
            if (index >= m_PlayerItems.Count)
                return null;

            return m_PlayerItems[index];
        }
        private void OnClickPlayerItem(int index)
        {
            ClickPlayerItem.Invoke(index);
        }

        public void SetPlayerItemName(int index, string name)
        {
            PlayerItem item = getAtPlayerItem(index);

            item.Name = name;
        }
        public void SetPlayerItemIcon(int index, uint iconID, uint iconFrameID)
        {
            PlayerItem item = getAtPlayerItem(index);

            item.Icon = iconID;
            item.IconFrame = iconFrameID;
        }

        public void SetPlayerItemLevel(int index, uint level)
        {
            PlayerItem item = getAtPlayerItem(index);

            item.SetLevel(level);
        }

        public void SetPlayerListActive(bool b)
        {
            m_PlayerListTransform.gameObject.SetActive(b);
        }

        public void SetFamilyInfoActive(bool b)
        {
            m_FamilyInfoTransform.gameObject.SetActive(b);
        }
    }

}