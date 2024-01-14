using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using UnityEngine.UI;
using Lib.Core;
using System;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace Logic
{
    public class UI_Cooking_Main : UIBase
    {
        private Image m_ClickClose;
        private InfinityGrid m_InfinityGrid;
        private Dictionary<GameObject, CookingMainGrid> m_CookingGrids = new Dictionary<GameObject, CookingMainGrid>();
        private List<uint> m_Types = new List<uint>(); //1911-1914

        protected override void OnInit()
        {
            for (uint i = 1911; i < 1915; i++)
            {
                m_Types.Add(i);
            }
        }


        protected override void OnLoaded()
        {
            m_ClickClose = transform.Find("Image_Close").GetComponent<Image>();
            m_InfinityGrid = transform.Find("Animator/Image/Scroll View").GetComponent<InfinityGrid>();
            m_InfinityGrid.onCreateCell += OnCreateCell;
            m_InfinityGrid.onCellChange += OnCellChange;

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(m_ClickClose);
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnClickClose);
        }

        private void OnClickClose(BaseEventData baseEventData)
        {
            UIManager.CloseUI(EUIID.UI_Cooking_Main);
        }


        protected override void OnShow()
        {
            m_InfinityGrid.CellCount = 4;
            m_InfinityGrid.ForceRefreshActiveCell();
        }

        protected override void OnDestroy()
        {
            m_Types.Clear();
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            CookingMainGrid cookingMainGrid = new CookingMainGrid();
            cookingMainGrid.BindGameObject(cell.mRootTransform.gameObject);
            cookingMainGrid.AddClickListener(OnCeilSelected);
            cell.BindUserData(cookingMainGrid);
            m_CookingGrids.Add(cell.mRootTransform.gameObject, cookingMainGrid);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            CookingMainGrid cookingMainGrid = cell.mUserData as CookingMainGrid;
            uint dataIndex = (uint)index + 1911;
            if (Sys_Cooking.Instance.usingCookings.TryGetValue((uint)dataIndex, out Sys_Cooking.AttrCooking attrCooking))
            {
                cookingMainGrid.SetData(dataIndex, attrCooking);
            }
            else
            {
                cookingMainGrid.SetData(dataIndex, null);
            }
        }

        private void OnCeilSelected(CookingMainGrid cookingMainGrid)
        {

        }

        public class CookingMainGrid
        {
            private GameObject m_Go;
            private GameObject m_BG;
            private Text m_Title;
            private Text m_Name;
            private Transform m_AttrParent;
            private Text m_Time;
            private Text m_None;
            private Text m_AttrDesc;
            private Image m_Icon;

            private Sys_Cooking.AttrCooking m_AttrCooking;
            private CSVItem.Data m_CSVItemData;

            public void BindGameObject(GameObject gameObject)
            {
                m_Go = gameObject;

                m_BG= m_Go.transform.Find("Image_BG/Image").gameObject;
                m_Name = m_Go.transform.Find("Image_BG/Text_Name").GetComponent<Text>();
                m_Title = m_Go.transform.Find("Image_Title/Text").GetComponent<Text>();
                m_None = m_Go.transform.Find("Image_BG/Text_Empty").GetComponent<Text>();
                m_Icon = m_Go.transform.Find("Image_BG/Image_Icon").GetComponent<Image>();
                m_AttrParent = m_Go.transform.Find("Image_BG/Buff");
                m_Time = m_Go.transform.Find("Image_BG/Text_Time").GetComponent<Text>();
                m_AttrDesc = m_Go.transform.Find("Image_BG/Text_Atr").GetComponent<Text>();
            }

            public void AddClickListener(Action<CookingMainGrid> action)
            {

            }

            public void SetData(uint type, Sys_Cooking.AttrCooking attrCooking)
            {
                if (attrCooking == null)
                {
                    SetEmpty();
                }
                else
                {
                    m_AttrCooking = attrCooking;
                    m_CSVItemData = CSVItem.Instance.GetConfData(m_AttrCooking.ItemId);
                    UpdateInfo();
                }
                SetTitle(type);
            }

            private void SetTitle(uint type)
            {
                uint lan = 0u;
                if (type == 1911)
                {
                    lan = 1003049u;
                }
                else if (type == 1912)
                {
                    lan = 1003050u;
                }
                else if (type == 1913)
                {
                    lan = 1003048u;
                }
                else if (type == 1914)
                {
                    lan = 1003051u;
                }
                TextHelper.SetText(m_Title, LanguageHelper.GetTextContent(lan));
            }

            private void UpdateInfo()
            {
                if (m_AttrCooking.b_Valid)
                {
                    m_None.gameObject.SetActive(false);
                    m_BG.gameObject.SetActive(true);
                    m_Name.gameObject.SetActive(true);
                    m_Icon.gameObject.SetActive(true);
                    m_Time.gameObject.SetActive(true);
                    m_AttrParent.gameObject.SetActive(true);

                    ImageHelper.SetIcon(m_Icon, m_CSVItemData.icon_id);
                    TextHelper.SetText(m_Time, m_AttrCooking.GetTime());
                    TextHelper.SetText(m_Name, m_CSVItemData.name_id);
                    CSVPassiveSkillInfo.Data cSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(m_CSVItemData.fun_value[0]);
                    if (cSVPassiveSkillInfoData != null)
                    {
                        TextHelper.SetText(m_AttrDesc, cSVPassiveSkillInfoData.desc);

                        //List<List<int>> attr = new List<List<int>>();
                        //if (cSVPassiveSkillInfoData.attr != null)
                        //{
                        //    for (int i = 0; i < cSVPassiveSkillInfoData.attr.Count; i++)
                        //    {
                        //        if (cSVPassiveSkillInfoData.attr[i][0] != 1)
                        //        {
                        //            continue;
                        //        }
                        //        attr.Add(cSVPassiveSkillInfoData.attr[i]);
                        //    }
                        //}
                        //int needCount = attr.Count;
                        //FrameworkTool.CreateChildList(m_AttrParent, needCount);
                        //for (int i = 0; i < m_AttrParent.childCount; i++)
                        //{
                        //    GameObject gameObject = m_AttrParent.GetChild(i).gameObject;
                        //    Text attrName = gameObject.transform.GetComponent<Text>();
                        //    Text attrValue = gameObject.transform.Find("Text_Num").GetComponent<Text>();
                        //    uint attrId = (uint)attr[i][1];
                        //    uint value = (uint)attr[i][2];
                        //    SetAttr(attrId, value, attrName, attrValue);
                        //}
                    }
                }
                else
                {
                    SetEmpty();
                    TextHelper.SetText(m_None, m_AttrCooking.GetTime());
                }

            }

            private void SetEmpty()
            {
                m_Name.gameObject.SetActive(false);
                m_BG.gameObject.SetActive(false);
                m_Icon.gameObject.SetActive(false);
                m_Time.gameObject.SetActive(false);
                m_AttrParent.gameObject.SetActive(false);
                m_None.gameObject.SetActive(true);
            }


            private void SetAttr(uint attr1, uint value1, Text attrName1, Text attrValue1)
            {
                CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(attr1);
                TextHelper.SetText(attrName1, cSVAttrData.name);
                if (cSVAttrData.show_type == 1)
                {
                    attrValue1.text = string.Format("+{0}", value1);
                }
                else
                {
                    attrValue1.text = string.Format("+{0}%", value1 / 100f);
                }
            }
        }
    }
}


