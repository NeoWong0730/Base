using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Mall_Charge : UIComponent
    {
        public class ChargeItem : UIComponent
        {
            private Button _btn;
         
            private Image _ImgIcon;
            private Text _textNum;
            private Text _textPrice;

            private GameObject _goTip1;
            private Text _textNameTip1;
            private Image _imgIconTip1;
            private Text _textNumTip1;

            private GameObject _goTip2;
            private Text _textNameTip2;
            private Image _imgIconTip2;
            private Text _textNumTimp2;
            //private Text _textNumTip2;

            private uint _chargeId;
            private CSVChargeList.Data _csvData;
            protected override void Loaded()
            {
                _btn = transform.GetComponent<Button>();
                _ImgIcon = transform.Find("Image_ICON").GetComponent<Image>();
                _textNum = transform.Find("Text_Num").GetComponent<Text>();
                _textPrice = transform.Find("Text_CNY").GetComponent<Text>();

                _goTip1 = transform.Find("Grid/Image_Mask").gameObject;
                _textNameTip1 = transform.Find("Grid/Image_Mask/Text").GetComponent<Text>();
                _imgIconTip1 = transform.Find("Grid/Image_Mask/Text/Image_ICON").GetComponent<Image>();
                _textNumTip1 = transform.Find("Grid/Image_Mask/Text/Text_Num").GetComponent<Text>();

                _goTip2 = transform.Find("Grid/Image_Mask01").gameObject;
                _textNameTip2 = transform.Find("Grid/Image_Mask01/Text").GetComponent<Text>();
                _imgIconTip2 = transform.Find("Grid/Image_Mask01/Image_ICON").GetComponent<Image>();
                _textNumTimp2 = transform.Find("Grid/Image_Mask01/Text (1)").GetComponent<Text>();
                //_textNumTip2 = transform.Find("Grid/Image_Mask01/Text/Text_Num").GetComponent<Text>();

                //Text firstTip = transform.Find("Image_Mask/Text").GetComponent<Text>();
                //firstTip.text = LanguageHelper.GetTextContent(11645u);

                //_imgFirstIcon = transform.Find("Image_Mask/Text/Image_ICON").GetComponent<Image>();
                //_textFirst = transform.Find("Image_Mask/Text/Text_Num").GetComponent<Text>();

                _btn.onClick.AddListener(OnClick);

                ProcessEvents(true);
            }

            public override void OnDestroy()
            {
                ProcessEvents(false);
            }

            private void ProcessEvents(bool toRegister)
            {
                Sys_Charge.Instance.eventEmitter.Handle<uint>(Sys_Charge.EEvents.OnChargedNotify, OnChargedNotify, toRegister);
                //Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnRefreshShopData, OnRefreshShopData, toRegister);
                //Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChnange, toRegister);
            }

            private void OnClick()
            {
                Sys_Charge.Instance.OnChargeReq(_csvData.id);

                UIManager.HitButton(EUIID.UI_Mall, _chargeId.ToString(), "Charge");
            }

            public void UpdateInfo(uint chargeId)
            {
                _chargeId = chargeId;
                _csvData = CSVChargeList.Instance.GetConfData(chargeId);

                ImageHelper.SetIcon(_ImgIcon, _csvData.BackGround);
                _textNum.text = LanguageHelper.GetTextContent(11644u, _csvData.RechargeDiamond.ToString());
                _textPrice.text = LanguageHelper.GetTextContent(11647u, (_csvData.RechargeCurrency / 100f).ToString());

                _goTip1.SetActive(false);
                if (_csvData.ReturnMoneyId != 0 && !Sys_Charge.Instance.IsCharged(_chargeId))
                {
                    //首充返还
                    _goTip1.SetActive(true);
                    CSVItem.Data itemReturn = CSVItem.Instance.GetConfData(_csvData.ReturnMoneyId);
                    _textNameTip1.text = LanguageHelper.GetTextContent(11645u);
                    ImageHelper.SetIcon(_imgIconTip1, itemReturn.small_icon_id);
                    _textNumTip1.text = _csvData.ReturnMoneyNum.ToString();
                }
                else
                {
                    //常驻返还
                    if (_csvData.MoneyId != 0u && _csvData.MoneyNum != 0u)
                    {
                        _goTip1.SetActive(true);
                        CSVItem.Data itemReturn = CSVItem.Instance.GetConfData(_csvData.MoneyId);
                        _textNameTip1.text = LanguageHelper.GetTextContent(11706u);
                        ImageHelper.SetIcon(_imgIconTip1, itemReturn.small_icon_id);
                        _textNumTip1.text = _csvData.MoneyNum.ToString();
                    }
                }

                _goTip2.SetActive(false);
                if (_csvData.MoneyItem != 0u)
                {
                    _goTip2.SetActive(true);

                    CSVItem.Data itemData = CSVItem.Instance.GetConfData(_csvData.MoneyItem);
                    _textNameTip2.text = LanguageHelper.GetTextContent(11785);
                    ImageHelper.SetIcon(_imgIconTip2, itemData.small_icon_id);
                    _textNumTimp2.text = _csvData.MoneyItemNum.ToString();
                }
            }

            private void OnChargedNotify(uint chargeId)
            {
                if (_chargeId == chargeId)
                    UpdateInfo(chargeId);
            }

        }


        private InfinityGrid _infinityGrid;
        private Dictionary<GameObject, ChargeItem> dicCells = new Dictionary<GameObject, ChargeItem>();
        private Button _btnPreview;
        private Button _btnRule;
        private Button _btnJump;

        private List<uint> _listIds = new List<uint>();

        protected override void Loaded()
        {

            _infinityGrid = transform.Find("Scroll_View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            _btnPreview = transform.Find("Btn_02").GetComponent<Button>();
            _btnRule = transform.Find("Btn_01").GetComponent<Button>();
            _btnJump = transform.Find("Btn_03").GetComponent<Button>();

            _btnPreview.onClick.AddListener(OnClickPreview);
            _btnRule.onClick.AddListener(OnClickRule);
            _btnJump.onClick.AddListener(OnClickJump);
        }

        public override void OnDestroy()
        {
            foreach (var cell in dicCells)
            {
                cell.Value.OnDestroy();
            }
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            ChargeItem entry = new ChargeItem();

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);

            dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            ChargeItem entry = cell.mUserData as ChargeItem;
            entry.UpdateInfo(_listIds[index]);
        }

        private void OnClickPreview()
        {
            UIManager.OpenUI(EUIID.UI_Lotto_Preview,false,1);
        }

        private void OnClickRule()
        {
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam
            {
                StrContent = LanguageHelper.GetTextContent(11708)
            });
        }

        private void OnClickJump()
        {
            if (Sys_FunctionOpen.Instance.IsOpen(50906, true))
            {
                OperationalActivityPrama param = new OperationalActivityPrama();
                param.pageType = (uint) EOperationalActivity.TotalCharge;
                UIManager.OpenUI(EUIID.UI_OperationalActivity, false, param);
            }
            
           
        }

        public void UpdateCharge()
        {
            _listIds = Sys_Charge.Instance.GetChargeList(1u);

            _infinityGrid.CellCount = _listIds.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);
        }
    }
}


