using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;

namespace Logic
{
    public partial class UI_ElementalCrystal : UIBase
    {
        private Button m_CloseButton;
        private Transform m_ParentLeft;
        private Text m_LeftAllotPoint;
        private List<Element> m_ElementsLeft = new List<Element>();
        private int[] m_LeftDatas = new int[4];
        private int m_AllotLeft;
        private List<int> m_LeftValues = new List<int>();
        private List<int> m_LeftNullValues = new List<int>();
        private Transform m_AttrLeft;

        private void RegisterLeft()
        {
            m_AttrLeft = transform.Find("Animator/Page02/Left/Image_BG/Attr");
            m_ParentLeft = transform.Find("Animator/Page02/Left/Elementals");
            m_LeftAllotPoint = transform.Find("Animator/Page02/Left/Image_BG/Text_Point").GetComponent<Text>();
            for (int i = 0; i < m_LeftDatas.Length; i++)
            {
                GameObject gameObject = m_ParentLeft.GetChild(i).gameObject;
                Element element = new Element();
                element.BindGameObject(gameObject);
                element.AddEvent(OnElementAddClicked, OnElementSubClicked);
                element.SetOwner(0);
                element.SetIndex(i);
                m_ElementsLeft.Add(element);
            }
        }

        private void ConstructLeftData()
        {
            uint id = 0;
            uint itemId = 0;
            for (int i = 0; i < m_LeftDatas.Length; i++)
            {
                id = (uint)(i + 1);
                if (id == 1)
                {
                    itemId = 2;
                }
                else if (id == 2)
                {
                    itemId = 3;
                }
                else if (id == 3)
                {
                    itemId = 4;
                }
                else if (id == 4)
                {
                    itemId = 1;
                }
                if (Sys_Attr.Instance.pkAttrs.ContainsKey(itemId))
                {
                    m_LeftDatas[i] = (int)Sys_Attr.Instance.pkAttrs[itemId];
                }
                else
                {
                    m_LeftDatas[i] = 0;
                }
            }
            for (int i = 0; i < m_LeftDatas.Length; i++)
            {
                m_ElementsLeft[i].value = m_LeftDatas[i];
            }
        }

        private void RefreshLeft()
        {
            m_LeftValues.Clear();
            m_LeftNullValues.Clear();
            m_AllotLeft = 10;
            for (int i = 0; i < m_ElementsLeft.Count; i++)
            {
                m_AllotLeft -= m_ElementsLeft[i].value;
                if (m_ElementsLeft[i].value > 0)
                {
                    m_LeftValues.Add(i);
                }
                else
                {
                    m_LeftNullValues.Add(i);
                }
            }
            if (m_LeftValues.Count == 2)
            {
                for (int i = 0; i < m_LeftNullValues.Count; i++)
                {
                    m_ElementsLeft[m_LeftNullValues[i]].ShowOrHideAddButton(false);
                    m_ElementsLeft[m_LeftNullValues[i]].ShowOrHideSubButton(false);
                }

            }
            else if (m_LeftValues.Count == 1)
            {
                int index = m_LeftValues[0];
                int tempIndex = index;
                if (index == 0)
                {
                    tempIndex = 4;
                }
                int lastIndex = (tempIndex - 1) % 4;
                int nextIndex = (tempIndex + 1) % 4;
                for (int i = 0; i < m_ElementsLeft.Count; i++)
                {
                    if (i == index)
                    {
                        continue;
                    }
                    m_ElementsLeft[i].ShowOrHideAddButton(i == lastIndex || i == nextIndex);
                }
            }
            else
            {
                for (int i = 0; i < m_ElementsLeft.Count; i++)
                {
                    m_ElementsLeft[i].ShowOrHideAddButton(true);
                }
            }
            RefreshLeftAllot();
        }

        private void RefreshLeftAllot()
        {
            TextHelper.SetText(m_LeftAllotPoint, m_AllotLeft.ToString());
            if (m_LeftValues.Count == 0)
            {
                for (int i = 0; i < m_AttrLeft.childCount; i++)
                {
                    m_AttrLeft.GetChild(i).gameObject.SetActive(false);
                }
            }
            else if (m_LeftValues.Count == 1)
            {
                m_AttrLeft.GetChild(0).gameObject.SetActive(true);
                m_AttrLeft.GetChild(1).gameObject.SetActive(false);
                Image icon = m_AttrLeft.GetChild(0).Find("Image_Icon").GetComponent<Image>();
                Text num = m_AttrLeft.GetChild(0).Find("Text").GetComponent<Text>();
                uint attrId = GetAttrId(m_LeftValues[0]);
                ImageHelper.SetIcon(icon, CSVAttr.Instance.GetConfData(attrId).attr_icon);
                TextHelper.SetText(num, m_ElementsLeft[m_LeftValues[0]].value.ToString());
            }
            else
            {
                m_AttrLeft.GetChild(0).gameObject.SetActive(true);
                m_AttrLeft.GetChild(1).gameObject.SetActive(true);
                for (int i = 0; i < m_AttrLeft.childCount; i++)
                {
                    Image icon = m_AttrLeft.GetChild(i).Find("Image_Icon").GetComponent<Image>();
                    Text num = m_AttrLeft.GetChild(i).Find("Text").GetComponent<Text>();
                    uint attrId = GetAttrId(m_LeftValues[i]);
                    ImageHelper.SetIcon(icon, CSVAttr.Instance.GetConfData(attrId).attr_icon);
                    TextHelper.SetText(num, m_ElementsLeft[m_LeftValues[i]].value.ToString());
                }
            }
        }

        private uint GetAttrId(int source)
        {
            uint attrId = 0;
            if (source == 0)
            {
                attrId = 2;
            }
            else if (source == 1)
            {
                attrId = 3;
            }
            else if (source == 2)
            {
                attrId = 4;
            }
            else
            {
                attrId = 1;
            }
            return attrId;
        }

        private void OnElementAddClicked(Element element)
        {
            if (element.owner == 0)
            {
                for (int i = 0; i < m_LeftValues.Count; i++)
                {
                    if (m_LeftValues[i] != element.index)
                    {
                        if (m_AllotLeft == 0)
                        {
                            m_ElementsLeft[m_LeftValues[i]].value -= 1;
                        }
                    }
                }
                RefreshLeft();
            }
            else
            {
                for (int i = 0; i < m_RightValues.Count; i++)
                {
                    if (m_RightValues[i] != element.index)
                    {
                        if (m_AllotRight == 0)
                        {
                            m_ElementsRight[m_RightValues[i]].value -= 1;
                        }
                    }
                }
                RefreshRight();
            }
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_ElementalCrystal);
        }

        private void OnElementSubClicked(Element element)
        {
            if (element.owner == 0)
            {
                RefreshLeft();
            }
            else
            {
                RefreshRight();
            }
        }
    }

    public class Element
    {
        private GameObject m_Go;
        private Image m_Process;
        private Button m_AddButton;
        private Button m_SubButton;
        private Action<Element> m_OnAddClicked;
        private Action<Element> m_OnSubClicked;
        public int index;
        private int m_Value = -1;
        public int value
        {
            get { return m_Value; }
            set
            {
                if (m_Value != value)
                {
                    m_Value = value;
                    SetProcess(m_Value);
                }
            }
        }
        public int owner;

        public void BindGameObject(GameObject gameObject)
        {
            m_Go = gameObject;

            m_Process = gameObject.transform.Find("Image_BG/Image").GetComponent<Image>();
            m_AddButton = gameObject.transform.Find("Button_Add").GetComponent<Button>();
            m_SubButton = gameObject.transform.Find("Button_Minus").GetComponent<Button>();
            m_AddButton.onClick.AddListener(OnAddButtonClicked);
            m_SubButton.onClick.AddListener(OnSubButtonClicked);
        }

        public void AddEvent(Action<Element> addClick, Action<Element> subClick)
        {
            m_OnAddClicked = addClick;
            m_OnSubClicked = subClick;
        }

        public void SetOwner(int owner)
        {
            this.owner = owner;
        }

        public void SetIndex(int index)
        {
            this.index = index;
        }

        public void SetProcess(int process)
        {
            m_Process.fillAmount = (float)process / 10f;
            if (process == 0)
            {
                m_SubButton.gameObject.SetActive(false);
                m_AddButton.gameObject.SetActive(true);
            }
            else if (process == 10)
            {
                m_SubButton.gameObject.SetActive(true);
                m_AddButton.gameObject.SetActive(false);
            }
            else
            {
                m_SubButton.gameObject.SetActive(true);
                m_AddButton.gameObject.SetActive(true);
            }
        }

        private void OnSubButtonClicked()
        {
            if (value > 0)
            {
                value -= 1;
                m_OnSubClicked?.Invoke(this);
            }
        }

        private void OnAddButtonClicked()
        {
            if (value < 10)
            {
                value += 1;
                m_OnAddClicked?.Invoke(this);
            }
        }

        public void ShowOrHideAddButton(bool active)
        {
            m_AddButton.gameObject.SetActive(active);
        }

        public void ShowOrHideSubButton(bool active)
        {
            m_SubButton.gameObject.SetActive(active);
        }
    }
}


