using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System.Text;
using System;
using System.Linq;

namespace Logic
{
    public class UI_Awaken_Addition_Ceil
    {
        private Text attrName;
        private Text attrNum;
        public void BindGameObject(GameObject gameObject)
        {
            attrName = gameObject.transform.Find("Text").GetComponent<Text>();
            attrNum = gameObject.transform.Find("Text_Num").GetComponent<Text>();
        }

        public void SetAttrData(int _type,AttrInfo _info)
        {
            CSVAttr.Data aData = CSVAttr.Instance.GetConfData(_info.attrId);
            attrName.text = LanguageHelper.GetTextContent(aData.name);
            if (aData.show_type == 1)
            {
                attrNum.text = "+" + _info.attrNum;
            }
            else
            {
                attrNum.text= "+" +(_info.attrNum/100) + "%";
            }

        }
    }
    public class UI_Awaken_Addition : UIBase
    {
        public Button btn_Close;
        public CP_ToggleRegistry tg_MessageTable;
        //public GameObject go_additionItem;
        public InfinityGrid messageGrid;
        public int additionType;
        private ImprintAttrEntry aEntry;
        private List<AttrInfo> iAList;
        private List<UI_Awaken_Addition_Ceil> ceilEntry = new List<UI_Awaken_Addition_Ceil>();

        protected override void OnLoaded()
        {
            btn_Close = transform.Find("Animator/View_TipsBgNew05/Btn_Close").GetComponent<Button>();
            messageGrid= transform.Find("Animator/View_Content/Scroll_Rank").GetComponent<InfinityGrid>();
            tg_MessageTable= transform.Find("Animator/View_Content/Toggles").GetComponent<CP_ToggleRegistry>();
            messageGrid.CellCount = 20;
            tg_MessageTable.onToggleChange += OnToggleChange;
            messageGrid.onCreateCell += OnCreateCell;
            messageGrid.onCellChange += OnCellChange;
            btn_Close.onClick.AddListener(OnBtnCloseClicked);
            
        }

        protected override void OnShow()
        {            
            RefreshPanel(0);
            tg_MessageTable.SwitchTo(0);
        }        
        protected override void OnDestroy()
        {            
            ceilEntry.Clear();
        }

            private void OnToggleChange(int current, int old)
        {
            if (additionType == current)
                return;

            additionType = current;
            RefreshPanel(additionType);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Awaken_Addition_Ceil entry = new UI_Awaken_Addition_Ceil();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            cell.BindUserData(entry);
            ceilEntry.Add(entry);

        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            if (iAList.Count == 0)
            {
                return;
            }
            UI_Awaken_Addition_Ceil entry = cell.mUserData as UI_Awaken_Addition_Ceil;
            entry.SetAttrData(additionType,iAList[index]);
            
        }

        private void RefreshPanel(int _type)
        {
            ceilEntry.Clear();
            aEntry = Sys_TravellerAwakening.Instance.GetImprintAttrEntry((uint)additionType + 1);
            iAList = aEntry.aList;
            messageGrid.CellCount = iAList.Count;
            messageGrid.ForceRefreshActiveCell();

        }

        private void OnBtnCloseClicked()
        {
            UIManager.CloseUI(EUIID.UI_Awaken_Addition);
        }

    }
}