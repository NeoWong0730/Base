using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_FamilyParty_Record : UIBase
    {
        private Button btnClose;
        private Text txtNum;
        private InfinityGrid infinity;
        private List<Sys_Family.FamilyData.FamilyPartyInfo.PartyRecord> listRecords = new List<Sys_Family.FamilyData.FamilyPartyInfo.PartyRecord>();
        private Dictionary<GameObject, UI_FamilyParty_RecordCell> CeilGrids = new Dictionary<GameObject, UI_FamilyParty_RecordCell>();

        #region 系统函数
        protected override void OnOpen(object arg)
        {
        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            Sys_Family.Instance.GetCuisineRecordReq();
            UpdateView();
        }
        protected override void OnDestroy()
        {
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnPartyRecordListUpdate, UpdateView, toRegister);
        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            txtNum = transform.Find("Animator/List/Image_Bottom/Text").GetComponent<Text>();
            infinity = transform.Find("Animator/List/Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;
        }
        private void UpdateView()
        {
            listRecords = Sys_Family.Instance.familyData.familyPartyInfo.listPartyRecords;
            txtNum.text = LanguageHelper.GetTextContent(6231, listRecords.Count.ToString());//参与人数：{0}
            infinity.CellCount = listRecords.Count;
            infinity.ForceRefreshActiveCell();
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_FamilyParty_RecordCell mCell = new UI_FamilyParty_RecordCell();
            mCell.Init(cell.mRootTransform.transform);
            cell.BindUserData(mCell);
            CeilGrids.Add(cell.mRootTransform.gameObject, mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_FamilyParty_RecordCell mCell = cell.mUserData as UI_FamilyParty_RecordCell;
            mCell.UpdateCellView(listRecords[index], index);
        }
        #endregion

        public class UI_FamilyParty_RecordCell
        {
            private Transform transform;
            private Text txtRankNum;
            private Text txtName;
            private Text txtCooking;
            private Text txtValue;

            public void Init(Transform trans)
            {
                transform = trans;
                txtRankNum = transform.Find("Num").GetComponent<Text>();
                txtName = transform.Find("Name").GetComponent<Text>();
                txtCooking = transform.Find("Cooking").GetComponent<Text>();
                txtValue = transform.Find("Value").GetComponent<Text>();
            }

            public void UpdateCellView(Sys_Family.FamilyData.FamilyPartyInfo.PartyRecord recordData, int index)
            {
                txtRankNum.text = (index + 1).ToString();
                txtName.text = recordData.name;
                txtCooking.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(recordData.itemId).name_id);
                txtValue.text = Sys_Family.Instance.GetFamilyPartyValue(recordData.itemId).ToString();
            }
        }
    }
}
