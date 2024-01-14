using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Sure_Tip : UIBase
    {
        private Button _btnClose;

        private Text _textTitle;

        private PropSpecialItem _propItem;
        public GameObject goDomestication_0;
        public GameObject goDomestication_1;

        private Text _textName;

        private Text _textAssign;
        private Text _textAssignValue;

        private Text _textHot;
        private Text _textHotValue;

        private Text _textTip;

        private Button _btnSale;

        private Sys_Trade.SaleConfirmParam _salePram;
        private CSVCommodity.Data _GoodData;

        protected override void OnOpen(object arg)
        {
            _salePram = null;
            if (arg != null)
                _salePram = (Sys_Trade.SaleConfirmParam)arg;

            if (_salePram != null)
                _GoodData = CSVCommodity.Instance.GetConfData(_salePram.Brief.InfoId);
        }

        protected override void OnLoaded()
        {
            _btnClose = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            _btnClose.onClick.AddListener(OnClickClose);

            _textTitle = transform.Find("Animator/View_TipsBg01_Small/Text_Title").GetComponent<Text>();

            _propItem = new PropSpecialItem();
            _propItem.BindGameObject(transform.Find("Animator/Detail/Image_BG/PropItem").gameObject);

            goDomestication_0 = transform.Find("Animator/Detail/Image_BG/PropItem/Image_Domestication_0").gameObject;
            goDomestication_1 = transform.Find("Animator/Detail/Image_BG/PropItem/Image_Domestication_1").gameObject;

            _textName = transform.Find("Animator/Detail/Image_BG/Text_Name").GetComponent<Text>();

            _textAssign = transform.Find("Animator/Detail/Image_BG/Line/Image0/Label_title").GetComponent<Text>();
            _textAssignValue = transform.Find("Animator/Detail/Image_BG/Line/Image0/Label_Num").GetComponent<Text>();

            _textHot = transform.Find("Animator/Detail/Image_BG/Line/Image1/Label_title").GetComponent<Text>();
            _textHotValue = transform.Find("Animator/Detail/Image_BG/Line/Image1/Label_Num").GetComponent<Text>();

            _textTip = transform.Find("Animator/Detail/Text_Tip").GetComponent<Text>();

            _btnSale = transform.Find("Animator/Detail/Btn_01").GetComponent<Button>();
            _btnSale.onClick.AddListener(OnClickSale);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        //protected override void ProcessEvents(bool toRegister)
        //{
        //    //Sys_TerrorSeries.Instance.eventEmitter.Handle<int>(Sys_TerrorSeries.EEvents.OnSelectLine, OnSelectLine, toRegister);
        //    //Sys_TerrorSeries.Instance.eventEmitter.Handle(Sys_TerrorSeries.EEvents.OnUpdateTaskData, OnUpdateTaskData, toRegister);
        //}

        private void OnClickClose()
        {
            this.CloseSelf();
        }

        private void OnClickSale()
        {
            if (_salePram.BReSale)
            {
                Sys_Trade.Instance.OnReSaleReq(_salePram.Brief.BCross, _salePram.Brief.GoodsUid, _salePram.Brief.Price);
            }
            else
            {
                Sys_Trade.Instance.OnSaleReq(_salePram.Brief.BCross, _salePram.Brief.InfoId, _salePram.Brief.GoodsUid, _salePram.Brief.Price, _salePram.Brief.Count, _salePram.Brief.Price == 0u, _salePram.Brief.TargetId, _salePram.Brief.TargetPrice, _salePram.TargetLeast);
            }
        }
        
        private void UpdateInfo()
        {
            if (null == _salePram)
            {
                Debug.LogErrorFormat("m_Brief is Null !!!");
                return;
            }

            if (null == _GoodData)
            {
                Debug.LogErrorFormat("m_GoodData is Null !!!");
                return;
            }

            ItemData item = new ItemData(99, _salePram.Brief.GoodsUid, _salePram.Brief.InfoId, _salePram.Brief.Count, 0, false, false, null, null, 0, null);
            item.SetQuality(_salePram.Brief.Color);

            PropSpeicalLoader.ShowItemData showItem = new PropSpeicalLoader.ShowItemData(item, false, false);
            showItem.SetTradeEnd(true);
            showItem.SetCross(_salePram.Brief.BCross);
            showItem.SetLevel(_salePram.Brief.Petlv);
            _propItem.SetData(new PropSpeicalLoader.MessageBoxEvt(EUIID.UI_Trade_Sure_Tip, showItem));

            _textName.text = LanguageHelper.GetTextContent(item.cSVItemData.name_id);

            bool discuss = _salePram.Brief.Price == 0u;
            uint titleLanId = discuss ? 2011219u : 2011218u;
            _textTitle.text = LanguageHelper.GetTextContent(titleLanId);

            if (discuss)
            {
                _textHot.transform.parent.gameObject.SetActive(false);

                _textAssign.text = LanguageHelper.GetTextContent(2011035); //价格
                _textAssignValue.text = LanguageHelper.GetTextContent(2011090);//议价

                _textTip.text = LanguageHelper.GetTextContent(2011221);
            }
            else
            {
                if (_salePram.Brief.TargetId != 0L)
                {

                    _textHot.transform.parent.gameObject.SetActive(true);

                    _textAssign.text = LanguageHelper.GetTextContent(2011013); //指定价
                    _textAssignValue.text = _salePram.Brief.TargetPrice.ToString();

                    _textHot.text = LanguageHelper.GetTextContent(2011014); //一口价
                    _textHotValue.text = _salePram.Brief.Price.ToString();

                    _textTip.text = LanguageHelper.GetTextContent(2011082, Sys_Trade.Instance.GetAssignTradeRate().ToString());
                }
                else
                {
                    _textHot.transform.parent.gameObject.SetActive(false);

                    _textAssign.text = LanguageHelper.GetTextContent(2011035); //价格
                    _textAssignValue.text = _salePram.Brief.Price.ToString();

                    _textTip.text = LanguageHelper.GetTextContent(2011220);
                }
            }

            goDomestication_0.SetActive(false);
            goDomestication_1.SetActive(false);
            //宠物驯化判断
            if (_GoodData.type == 2u)
            {

                CSVPetNew.Data petNew = CSVPetNew.Instance.GetConfData(_salePram.Brief.InfoId);
                if (petNew != null && petNew.mount)
                {
                    ClientPet petItem = Sys_Pet.Instance.GetPetByUId((uint)_salePram.Brief.GoodsUid);

                    goDomestication_0.SetActive(petItem.petUnit.SimpleInfo.MountDomestication == 0);
                    goDomestication_1.SetActive(petItem.petUnit.SimpleInfo.MountDomestication == 1);
                }
            }
        }

    }
}


