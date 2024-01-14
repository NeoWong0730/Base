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
    public class UI_FamilyParty_StarReward : UIBase
    {
        private Button btnClose;
        //private List<uint> partyInfoIds = new List<uint>();
        private InfinityGrid infinity;
        private Dictionary<GameObject, UI_FamilyParty_StarRewardCell> CeilGrids = new Dictionary<GameObject, UI_FamilyParty_StarRewardCell>();

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
            UpdateView();
        }
        protected override void OnDestroy()
        {
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);

            infinity = transform.Find("Animator/List/Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;

            //partyInfoIds.Clear();
            //partyInfoIds.AddRange(CSVFamilyReception.Instance.GetDictData().Keys);
        }
        private void UpdateView()
        {
            //infinity.CellCount = partyInfoIds.Count;
            infinity.CellCount = CSVFamilyReception.Instance.Count;
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
            UI_FamilyParty_StarRewardCell mCell = new UI_FamilyParty_StarRewardCell();
            mCell.Init(cell.mRootTransform.transform);
            cell.BindUserData(mCell);
            CeilGrids.Add(cell.mRootTransform.gameObject, mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_FamilyParty_StarRewardCell mCell = cell.mUserData as UI_FamilyParty_StarRewardCell;
            //mCell.UpdateCellView(partyInfoIds[index]);
            mCell.UpdateCellView(CSVFamilyReception.Instance.GetByIndex(index));
        }
        #endregion

        public class UI_FamilyParty_StarRewardCell
        {
            private Transform transform;
            private Text txtProsperity;
            private Text txtStar;
            private Text txtValue;
            private Text txtNpc1;
            private Text txtNpc2;
            private Text txtNpc3;
            private Text txtNpcAll;

            public void Init(Transform trans)
            {
                transform = trans;
                txtProsperity = transform.Find("Prosperity").GetComponent<Text>();
                txtStar = transform.Find("Star").GetComponent<Text>();
                txtValue = transform.Find("Value").GetComponent<Text>();
                txtNpc1 = transform.Find("NPC0").GetComponent<Text>();
                txtNpc2 = transform.Find("NPC1").GetComponent<Text>();
                txtNpc3 = transform.Find("NPC2").GetComponent<Text>();
                txtNpcAll = transform.Find("NPC3").GetComponent<Text>();
            }

            public void UpdateCellView(CSVFamilyReception.Data partyData)//(uint id)
            {
                //CSVFamilyReception.Data partyData = CSVFamilyReception.Instance.GetConfData(id);
                if(partyData != null)
                {
                    txtProsperity.text = LanguageHelper.GetTextContent(10087, partyData.familyCastleLv.ToString());
                    txtStar.text = LanguageHelper.GetTextContent(6260, partyData.receptionStar.ToString());
                    txtValue.text = partyData.receptionValue.ToString();
                    txtNpc1.text = partyData.level1Food.ToString();
                    txtNpc2.text = partyData.level2Food.ToString();
                    txtNpc3.text = partyData.level3Food.ToString();
                    txtNpcAll.text = partyData.foodTotal.ToString();
                }
            }
        }




    }
}
