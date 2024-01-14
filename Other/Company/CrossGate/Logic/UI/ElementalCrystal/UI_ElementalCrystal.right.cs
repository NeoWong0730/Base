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
        private Transform m_ParentRight;
        private Text m_RightAllotPoint;
        private List<Element> m_ElementsRight = new List<Element>();
        private int[] m_RightDatas = new int[4];
        private int m_AllotRight;
        private List<int> m_RightValues = new List<int>();
        private List<int> m_RightNullValues = new List<int>();
        private Transform m_AttrRight;

        private void RegisterRight()
        {
            m_AttrRight = transform.Find("Animator/Page02/Right/Image_BG/Attr");
            m_ParentRight = transform.Find("Animator/Page02/Right/Elementals");
            m_RightAllotPoint = transform.Find("Animator/Page02/Right/Image_BG/Text_Point").GetComponent<Text>();
            for (int i = 0; i < m_RightDatas.Length; i++)
            {
                GameObject gameObject = m_ParentRight.GetChild(i).gameObject;
                Element element = new Element();
                element.BindGameObject(gameObject);
                element.AddEvent(OnElementAddClicked, OnElementSubClicked);
                element.SetOwner(1);
                element.SetIndex(i);
                m_ElementsRight.Add(element);
            }
        }

        private void ConstructRightData()
        {
            int index1 = UnityEngine.Random.Range(0, 4);
            int index2 = (index1 + 1) % 4;
            int value1 = UnityEngine.Random.Range(1, 9);
            int value2 = 10 - value1;
            m_RightDatas[index1] = value1;
            m_RightDatas[index2] = value2;
            for (int i = 0; i < m_RightDatas.Length; i++)
            {
                if (i == index1 || i == index2)
                {
                    continue;
                }
                m_RightDatas[i] = 0;
            }
            for (int i = 0; i < m_RightDatas.Length; i++)
            {
                m_ElementsRight[i].value = m_RightDatas[i];
            }
        }

        private void RefreshRight()
        {
            m_RightValues.Clear();
            m_RightNullValues.Clear();
            m_AllotRight = 10;
            for (int i = 0; i < m_ElementsRight.Count; i++)
            {
                m_AllotRight -= m_ElementsRight[i].value;
                if (m_ElementsRight[i].value > 0)
                {
                    m_RightValues.Add(i);
                }
                else
                {
                    m_RightNullValues.Add(i);
                }
            }
            if (m_RightValues.Count == 2)
            {
                for (int i = 0; i < m_RightNullValues.Count; i++)
                {
                    m_ElementsRight[m_RightNullValues[i]].ShowOrHideAddButton(false);
                    m_ElementsRight[m_RightNullValues[i]].ShowOrHideSubButton(false);
                }
            }
            else if (m_RightValues.Count == 1)
            {
                int index = m_RightValues[0];
                int tempIndex = index;
                if (index == 0)
                {
                    tempIndex = 4;
                }
                int lastIndex = (tempIndex - 1) % 4;
                int nextIndex = (tempIndex + 1) % 4;
                for (int i = 0; i < m_ElementsRight.Count; i++)
                {
                    if (i == index)
                    {
                        continue;
                    }
                    m_ElementsRight[i].ShowOrHideAddButton(i == lastIndex || i == nextIndex);
                }
            }
            else
            {
                for (int i = 0; i < m_ElementsRight.Count; i++)
                {
                    m_ElementsRight[i].ShowOrHideAddButton(true);
                }
            }
            RefreshRightAllot();
        }

        private void RefreshRightAllot()
        {
            TextHelper.SetText(m_RightAllotPoint, m_AllotRight.ToString());
            if (m_RightValues.Count == 0)
            {
                for (int i = 0; i < m_AttrRight.childCount; i++)
                {
                    m_AttrRight.GetChild(i).gameObject.SetActive(false);
                }
            }
            else if (m_RightValues.Count == 1)
            {
                m_AttrRight.GetChild(0).gameObject.SetActive(true);
                m_AttrRight.GetChild(1).gameObject.SetActive(false);
                Image icon = m_AttrRight.GetChild(0).Find("Image_Icon").GetComponent<Image>();
                Text num = m_AttrRight.GetChild(0).Find("Text").GetComponent<Text>();
                uint attrId = GetAttrId(m_RightValues[0]);
                ImageHelper.SetIcon(icon, CSVAttr.Instance.GetConfData(attrId).attr_icon);
                TextHelper.SetText(num, m_ElementsRight[m_RightValues[0]].value.ToString());
            }
            else
            {
                m_AttrRight.GetChild(0).gameObject.SetActive(true);
                m_AttrRight.GetChild(1).gameObject.SetActive(true);
                for (int i = 0; i < m_AttrRight.childCount; i++)
                {
                    Image icon = m_AttrRight.GetChild(i).Find("Image_Icon").GetComponent<Image>();
                    Text num = m_AttrRight.GetChild(i).Find("Text").GetComponent<Text>();
                    uint attrId = GetAttrId(m_RightValues[i]);
                    ImageHelper.SetIcon(icon, CSVAttr.Instance.GetConfData(attrId).attr_icon);
                    TextHelper.SetText(num, m_ElementsRight[m_RightValues[i]].value.ToString());
                }
            }
        }
    }
}


