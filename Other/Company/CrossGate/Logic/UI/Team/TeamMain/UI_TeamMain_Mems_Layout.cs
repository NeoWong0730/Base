using Lib.AssetLoader;
using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Logic.Core;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using Framework;
using System;

namespace Logic
{
    public partial class UI_TeamMain_Mems_Layout
    {
        //队伍成员
        private Transform m_TeamMemsView;
        private Transform m_TeamMemsViewContent;

        private Transform m_ItemTransform;

        public Action<int> ClickItemAction;

        private ClickItemGroup<Item> m_ItemGroup = new ClickItemGroup<Item>();

        private Button m_BtnAddMember;
        private Transform m_TransAddMember;

        public bool isActive { get
            {
                if (m_TeamMemsView == null)
                    return false;

                return m_TeamMemsView.gameObject.activeSelf;
            } }

       
        public void Show()
        {
            m_TeamMemsView.gameObject.SetActive(true);
        }

        public void Hide()
        {
            m_TeamMemsView.gameObject.SetActive(false);
        }

        public void Load(Transform parent)
        {
            m_TeamMemsView = parent.Find("TeamScroll");

            m_TeamMemsViewContent = m_TeamMemsView.Find("Viewport");

            m_ItemTransform = m_TeamMemsViewContent.Find("Item");

            m_ItemTransform.gameObject.SetActive(false);

            m_ItemGroup.SetAddChildListenter(OnAddItem);

            m_ItemGroup.AddChild(m_ItemTransform);

            m_TransAddMember = m_TeamMemsViewContent.Find("ItemAdd");
            m_BtnAddMember = m_TeamMemsViewContent.Find("ItemAdd/Image_Black").GetComponent<Button>();

           
        

        }

        public void SetListener(IListener listener)
        {
            m_BtnAddMember.onClick.AddListener(listener.OnClickAddMember);


        }
        private void OnAddItem( Item item)
        {
            item.callBackAction = OnClickItem;      
        }



        private void OnClickItem(int index)
        {
            ClickItemAction?.Invoke(index);
        }
        public void RegisterEvents(UI_TeamMain_Layout.IListener listener, bool state)
        {
           // ClickItemAction = listener.OnClickMemItem;
        }

        public void SetAddMemberActive(bool active)
        {
            m_TransAddMember.gameObject.SetActive(active);

            if (active)
                m_TransAddMember.SetAsLastSibling();
        }

        public void SetItemSize(int size)
        {
            m_ItemGroup.SetChildSize(size);
        }

        public void GetItemCorners(int index, Vector3[] vectors)
        {
            var item = m_ItemGroup.getAt(index);

            if (item == null)
                return;


            RectTransform rect = item.mTransform as RectTransform;

            rect.GetWorldCorners(vectors);
        }

        public void SetItemSelected(int index, bool b)
        {
            var item = m_ItemGroup.getAt(index);

            if (item == null)
                return;

            item.isSelect = b;
        }

        public Item GetItem(int index)
        {
            var item = m_ItemGroup.getAt(index);

            return item;
        }
    }

    public partial class UI_TeamMain_Mems_Layout
    {
        public interface IListener
        {
            void OnClickItem(int index);

            void OnClickAddMember();
        }
    }

    public partial class UI_TeamMain_Mems_Layout
    {
        public class Item : ClickItem
        {
            public Transform m_transform;
            public Image m_IIcon;
            public Image m_IIconFrame;
            public Image m_IstateStop;//暂离
            public Image m_IstateOffline;
            public Image m_IOccupation;//职业
            public Text m_TextLevel;
            public Text m_TextName;
            public Image m_ISelect;
            public Button m_BtnIcon;

            public Image m_ImHP;
            public Image m_ImMagic;

            private string m_Name;
            public string Name { set { m_Name = value; TextHelper.SetText(m_TextName, m_Name); } }

            private int m_Level;
            public int Level { set { m_Level = value; TextHelper.SetText(m_TextLevel, LanguageHelper.GetTextContent(1000002, m_Level.ToString())); } }

            private bool m_isLeave;
            public bool isLeave { set { m_isLeave = value; UpdataState(2); } }

            private bool m_isOffline;
            public bool isOffline { set { m_isOffline = value; UpdataState(1); } }

            private bool m_isSelect;
            public bool isSelect { set { m_isSelect = value; m_ISelect.gameObject.SetActive(m_isSelect); } }

            private uint m_HeadIcon;

            public uint HeadIcon
            {
                get { return m_HeadIcon; }
                set { if (value == m_HeadIcon) return; m_HeadIcon = value; ImageHelper.SetIcon(m_IIcon, m_HeadIcon); }
            }

            private uint m_HeadIconFrame;
            public uint HeadIconFrame
            {
                get { return m_HeadIconFrame; }
                set
                {
                    if (value == m_HeadIconFrame) return;
                    m_HeadIconFrame = value;
                    if (m_HeadIconFrame == 0)
                    {
                        m_IIconFrame.gameObject.SetActive(false);
                    }
                    else
                    {
                        m_IIconFrame.gameObject.SetActive(true);
                        ImageHelper.SetIcon(m_IIconFrame, m_HeadIconFrame);
                    }
                }
            }

            private uint m_OccupationIcon = 99999;

            public uint OccupationIcon
            {
                get { return m_OccupationIcon; }
                set
                {
                    if (value == m_OccupationIcon)
                        return;
                    m_OccupationIcon = value;

                    if (m_OccupationIcon == 0)
                        m_IOccupation.gameObject.SetActive(false);
                    else
                        m_IOccupation.gameObject.SetActive(true);

                    if (m_OccupationIcon != 0)
                        ImageHelper.SetIcon(m_IOccupation, m_OccupationIcon);
                }
            }

            public int Index { get; set; }

            public Action<int> callBackAction;

            public void OnClick()
            {
                callBackAction?.Invoke(Index);
            }


            private void UpdataState(uint state) // 0 正常 1 离线 2 暂离 
            {
                m_IstateStop.gameObject.SetActive(!m_isOffline && m_isLeave);

                m_IstateOffline.gameObject.SetActive(m_isOffline);
            }

            public override void Load(Transform root)
            {
                base.Load(root);

                m_IIcon = root.Find("Head").GetComponent<Image>();

                m_IIconFrame = root.Find("Head/Image_Before_Frame").GetComponent<Image>();

                m_IstateStop = root.Find("Head/Image_State").GetComponent<Image>();

                m_IstateOffline = root.Find("Head/Image_offline").GetComponent<Image>();

                m_TextLevel = root.Find("Text_Level").GetComponent<Text>();

                m_TextName = root.Find("Text_Name").GetComponent<Text>();

                m_IOccupation = root.Find("Image_Icon").GetComponent<Image>();

                m_ISelect = root.Find("Image_Select").GetComponent<Image>();

                m_BtnIcon = root.Find("Image_Black").GetComponent<Button>();

                m_BtnIcon.onClick.AddListener(OnClick);

                m_ImHP = root.Find("Image_BG/Image_Life/Image_Process").GetComponent<Image>();
                m_ImMagic = root.Find("Image_BG/Image_Magic/Image_Process").GetComponent<Image>();


            }


            public void SetHP(float value)
            {
                m_ImHP.fillAmount = value;
            }

            public void SetMagic(float value)
            {
                m_ImMagic.fillAmount = value;
            }

        public override ClickItem Clone()
            {
                return Clone<Item>(this);
            }

        }
    }
}
