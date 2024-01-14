using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Logic.Core;
using Packet;
using UnityEngine.Playables;

namespace Logic
{
    public class UI_TotalCharge : UI_OperationalActivityBase
    {
        private Button btnGotoCharge;

        private List<int> lstIndex;
        private InfinityGrid infinity;
        private Dictionary<GameObject, UI_TotalChargeCellView> CeilGrids = new Dictionary<GameObject, UI_TotalChargeCellView>();

        #region 系统函数
        protected override void Loaded()
        {
        }
        protected override void InitBeforOnShow()
        {
            Parse();
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            UpdateView();
        }
        public override void Hide()
        {
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateTotalChargeData, OnUpdateGrowthFundData, toRegister);
        }
        #endregion
        #region func
        private void Parse()
        {
            btnGotoCharge = transform.Find("bg/Button").GetComponent<Button>();
            btnGotoCharge.onClick.AddListener(OnBtnGoToRechargeClick);

            infinity = transform.Find("Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;
        }
        private void UpdateView()
        {
            lstIndex = Sys_OperationalActivity.Instance.GetTotalChargeIdIndex();
            infinity.CellCount = lstIndex.Count;
            infinity.ForceRefreshActiveCell();
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_TotalChargeCellView mCell = new UI_TotalChargeCellView();
            mCell.Init(cell.mRootTransform.transform);
            cell.BindUserData(mCell);
            CeilGrids.Add(cell.mRootTransform.gameObject, mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_TotalChargeCellView mCell = cell.mUserData as UI_TotalChargeCellView;
            //var totalChargeIds = Sys_OperationalActivity.Instance.LstTotalChargeIds;
            //mCell.UpdateCellView(totalChargeIds[lstIndex[index]]);

            CSVCareerChange.Data data = CSVCareerChange.Instance.GetByIndex(lstIndex[index]);
            mCell.UpdateCellView(data);
        }
        #endregion
        #region event
        private void OnBtnGoToRechargeClick()
        {
            //跳转 商城-充值 界面
            MallPrama mallPrama = new MallPrama
            {
                mallId = 101,
                shopId = 1001,
                isCharge = true
            };
            UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
            Sys_OperationalActivity.Instance.ReportTotlaChargeClickEventHitPoint("GotoChargeMall");
        }
        private void OnUpdateGrowthFundData()
        {
            UpdateView();
        }
        #endregion

        public class UI_TotalChargeCellView
        {
            private uint TotalChargeId;
            private CSVCareerChange.Data TotalChargeData;

            private Transform transform;
            private Image imgIcon;
            private Text txtValue;
            private Slider slider;
            private Text txtPercent;
            private GameObject goIsGet;
            private Button btnGet;
            private List<ItemIdCount> dropItems = new List<ItemIdCount>();
            private Dictionary<GameObject, PropItem> CeilGrids = new Dictionary<GameObject, PropItem>();


            private InfinityGrid infinity;

            public void Init(Transform trans)
            {
                transform = trans;
                imgIcon = transform.Find("Image_bg/Cost_Coin").GetComponent<Image>();
                txtValue = transform.Find("Image_bg/Cost_Coin/Text_Cost").GetComponent<Text>();
                slider = transform.Find("Image_bg/Slider").GetComponent<Slider>();
                txtPercent = transform.Find("Image_bg/Slider/Text_Percent").GetComponent<Text>();
                goIsGet = transform.Find("Image_bg/State/Image").gameObject;
                btnGet = transform.Find("Image_bg/State/Btn_01").GetComponent<Button>();
                btnGet.onClick.AddListener(OnBtnGetClick);

                infinity = transform.Find("Image_bg/Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
                infinity.onCreateCell += OnCreateCell;
                infinity.onCellChange += OnCellChange;
            }

            public void UpdateCellView(CSVCareerChange.Data data)//(uint _id)
            {
                //TotalChargeId = _id;
                //TotalChargeData = CSVCareerChange.Instance.GetConfData(TotalChargeId);
                TotalChargeId = data.id;
                TotalChargeData = data;

                if (TotalChargeData != null)
                {
                    var chargeValue = Sys_OperationalActivity.Instance.TotalChargeValue;
                    var needCharge = TotalChargeData.money;
                    var csvItemData = CSVItem.Instance.GetConfData(1);//魔币
                    ImageHelper.SetIcon(imgIcon, csvItemData.icon_id);
                    txtValue.text = needCharge.ToString();
                    if (chargeValue < needCharge)
                    {
                        slider.gameObject.SetActive(true);
                        txtPercent.text = LanguageHelper.GetTextContent(4733, chargeValue.ToString(), needCharge.ToString());
                        if (chargeValue <= 0)
                        {
                            slider.value = 0;
                        }
                        else
                        {
                            slider.value = (float)chargeValue / (float)needCharge;
                        }
                        btnGet.gameObject.SetActive(true);
                        btnGet.GetComponent<ButtonScaler>().enabled = false;
                        btnGet.interactable = false;
                        ImageHelper.SetImageGray(btnGet.GetComponent<Image>(), true, true);
                        goIsGet.SetActive(false);
                    }
                    else
                    {
                        slider.value = 1;
                        txtPercent.text = LanguageHelper.GetTextContent(4733, needCharge.ToString(), needCharge.ToString());
                        bool isGet = Sys_OperationalActivity.Instance.CheckTotalChargeGiftIsGet(TotalChargeId);
                        if (isGet)
                        {
                            btnGet.gameObject.SetActive(false);
                            goIsGet.SetActive(true);
                        }
                        else
                        {
                            btnGet.gameObject.SetActive(true);
                            btnGet.GetComponent<ButtonScaler>().enabled = true;
                            btnGet.interactable = true;
                            ImageHelper.SetImageGray(btnGet.GetComponent<Image>(), false, true);
                            goIsGet.SetActive(false);
                        }
                    }
                    dropItems = CSVDrop.Instance.GetDropItem(TotalChargeData.reward_Id);
                    infinity.CellCount = dropItems.Count;
                    infinity.ForceRefreshActiveCell();
                }
            }
            private void OnBtnGetClick()
            {
                Sys_OperationalActivity.Instance.GetTotalChargeGiftReq(TotalChargeId - 1);
                Sys_OperationalActivity.Instance.ReportTotlaChargeClickEventHitPoint("GetReward:" + TotalChargeId);
            }

            private void OnCreateCell(InfinityGridCell cell)
            {
                PropItem mCell = new PropItem();
                mCell.BindGameObject(cell.mRootTransform.gameObject);
                cell.BindUserData(mCell);
                CeilGrids.Add(cell.mRootTransform.gameObject, mCell);
            }
            private void OnCellChange(InfinityGridCell cell, int index)
            {
                PropItem mCell = cell.mUserData as PropItem;
                var dropItem = dropItems[index];
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(dropItem.id, dropItem.count, true, false, false, false, false, true);
                mCell.SetData(itemData, EUIID.UI_OperationalActivity);
            }
        }
    }
}
