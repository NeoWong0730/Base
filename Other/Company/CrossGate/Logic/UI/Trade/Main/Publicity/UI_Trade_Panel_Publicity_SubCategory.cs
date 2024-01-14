using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class UI_Trade_Panel_Publicity_SubCategory
    {
        private class SubCategory_Cell
        {
            private Transform transform;

            private Button m_BtnBg;
            private Image m_IconBg;
            private Image m_Icon;
            private Text m_Name;
            private Text m_Num;

            private uint m_SubCategoryId;

            public void Init(Transform trans)
            {
                transform = trans;

                m_BtnBg = transform.Find("Image_BG").GetComponent<Button>();
                m_IconBg = transform.Find("Icon_BG").GetComponent<Image>();
                m_Icon = transform.Find("Image_Icon").GetComponent<Image>();
                m_Name = transform.Find("Text_name").GetComponent<Text>();
                m_Num = transform.Find("Text_Num").GetComponent<Text>();

                m_BtnBg.onClick.AddListener(OnClickSubCategory);
            }
           
            private void OnClickSubCategory()
            {
                Sys_Trade.Instance.SetPublicitySubCategory(m_SubCategoryId);
            }
            
            public void UpdateInfo(CmdTradeSaleCountRes.Types.Info info)
            {
                m_SubCategoryId = info.Category;
                CSVCommodityCategory.Data data = CSVCommodityCategory.Instance.GetConfData(m_SubCategoryId);
                if (data != null)
                {
                    ImageHelper.SetIcon(m_IconBg, data.iconid);
                    ImageHelper.SetIcon(m_Icon, data.icon);
                    //m_Icon.enabled = true;
                    m_Name.text = LanguageHelper.GetTextContent(data.name);
                    m_Num.text = info.Count.ToString();
                }
                else
                {
                    DebugUtil.LogErrorFormat("CSVCommodityCategory {0} is Error", m_SubCategoryId);
                }
            }
        }

        private Transform transform;

        private Text _textTipCategory;
        
        private InfinityGrid _infinityGrid;

        private Transform _transPage;
        private Transform _transPublicityTip;
        private Button _btnPublicityTip;

        private List<CmdTradeSaleCountRes.Types.Info> listInfos = new List<CmdTradeSaleCountRes.Types.Info>();

        public void Init(Transform trans)
        {
            transform = trans;

            _textTipCategory = transform.Find("Text_Title").GetComponent<Text>();

            _infinityGrid = transform.Find("Rect").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            _transPage = transform.Find("Button_Left");
            _transPage.gameObject.SetActive(false);

            _transPublicityTip = transform.Find("Text_Tip");
            _transPublicityTip.gameObject.SetActive(true);
            _btnPublicityTip = transform.Find("Text_Tip/Image").GetComponent<Button>();
            _btnPublicityTip.onClick.AddListener(OnClickPublicityTip);
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);

            ProcessEvents(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);

            ProcessEvents(false);
        }

        public void OnDestroy()
        {
            ProcessEvents(false);
        }

        public void ProcessEvents(bool register)
        {
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnViewPublicityServerType, OnSelectServer, register);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnViewPublicityListSaleCountNtf, OnListSaleCountNtf, register);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            SubCategory_Cell entry = new SubCategory_Cell();

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            SubCategory_Cell entry = cell.mUserData as SubCategory_Cell;
            entry.UpdateInfo(listInfos[index]);
        }

        private void OnClickPublicityTip()
        {
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2011200) });
        }

        public void UpdateInfo()
        {
            //visualGridCount = 0;
            //gridGroup.SetAmount(visualGridCount);

            Sys_Trade.Instance.OnSaleCountReq(Sys_Trade.Instance.CurPublicityServerType == Sys_Trade.ServerType.Cross,
                     Sys_Trade.Instance.CurPublicityCategory, true);
        }

        private void OnSelectServer()
        {
            //Sys_Trade.Instance.OnSaleCountReq();
        }

        public void OnListSaleCountNtf()
        {
            CSVCommodityList.Data data = CSVCommodityList.Instance.GetConfData(Sys_Trade.Instance.CurPublicityCategory);
            if (data != null)
                _textTipCategory.text = LanguageHelper.GetTextContent(2011000, LanguageHelper.GetTextContent(data.name));

            listInfos.Clear();
            listInfos.AddRange(Sys_Trade.Instance.GetSaleCountList());
            listInfos.Sort((d1, d2) =>
            {
                CSVCommodityCategory.Data data1 = CSVCommodityCategory.Instance.GetConfData(d1.Category);
                CSVCommodityCategory.Data data2 = CSVCommodityCategory.Instance.GetConfData(d2.Category);
                return data1.seqencing.CompareTo(data2.seqencing);
            });

            _infinityGrid.CellCount = listInfos.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);

            if (Sys_Trade.Instance.SearchParam.isSearch && Sys_Trade.Instance.SearchParam.showType == TradeShowType.Publicity)
            {
                bool isContains = false;
                for (int i = 0; i < listInfos.Count; ++i)
                {
                    if (listInfos[i].Category == Sys_Trade.Instance.SearchParam.SubCategory)
                    {
                        isContains = true;
                        break;
                    }
                }

                if (isContains)
                {
                    Sys_Trade.Instance.SetPublicitySubCategory(Sys_Trade.Instance.SearchParam.SubCategory);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011229));
                    Sys_Trade.Instance.SearchParam.Reset();
                }
            }
            else
            {
                if (Sys_Trade.Instance.CurPublicitySubCategory != 0)
                {
                    CSVCommodityCategory.Data temp = CSVCommodityCategory.Instance.GetConfData(Sys_Trade.Instance.CurPublicitySubCategory);
                    if (temp.list != Sys_Trade.Instance.CurPublicityCategory)
                        Sys_Trade.Instance.CurPublicitySubCategory = 0;
                    else
                        Sys_Trade.Instance.SetPublicitySubCategory(Sys_Trade.Instance.CurPublicitySubCategory);
                }
            }
        }
    }
}


