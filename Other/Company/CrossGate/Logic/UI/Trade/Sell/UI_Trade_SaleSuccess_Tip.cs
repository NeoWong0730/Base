using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_SaleSuccess_Tip : UIBase
    {
        private Button _btnClose;
        private Text _textTitle;

        private Transform _tipBg;
        private Transform[] _transArray = new Transform[5];
        private Toggle _toggle;
        private Button _btnConfirm;

        private TradeBrief _brief;
        private CSVCommodity.Data _commodityData;
        private bool _isDisableTip = false;


        protected override void OnOpen(object arg)
        {
            _brief = null;
            if (arg != null)
                _brief = (TradeBrief)arg;

            if (_brief != null)
                _commodityData = CSVCommodity.Instance.GetConfData(_brief.InfoId);
        }

        protected override void OnLoaded()
        {
            _btnClose = transform.Find("Animator/View_TipsBgNew04/Btn_Close").GetComponent<Button>();
            _btnClose.onClick.AddListener(OnClickClose);

            _textTitle = transform.Find("Animator/View_TipsBgNew04/Text_Title").GetComponent<Text>();

            _tipBg = transform.Find("Animator/View_TipsBgNew04/Image_bg01");
            for (int i = 0; i < 5; ++i)
            {
                _transArray[i] = _tipBg.Find(string.Format("Image_BG2/Content_Desc{0}", i));
            }
            _toggle = _tipBg.Find("Toggle/Background").GetComponent<Toggle>();
            _btnConfirm = _tipBg.Find("Button/Btn_01").GetComponent<Button>();
            _btnConfirm.onClick.AddListener(OnClickClose);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void OnClickClose()
        {
            _isDisableTip = _toggle.isOn;
            if (_isDisableTip)
                Sys_Trade.Instance.SaveSuccessInfo();
            this.CloseSelf();
        }

        private void UpdateInfo()
        {
            //title
            bool isPublicity = _commodityData.publicity;
            bool isBargin = Sys_Trade.Instance.IsBargin(_brief);

            uint titleLanId = (isPublicity && !isBargin) ? 2011020u : 2011018u; //公示成功：上架成功
            _textTitle.text = LanguageHelper.GetTextContent(titleLanId);

            //content
            for(int i = 0; i < _transArray.Length; ++i)
            {
                _transArray[i].gameObject.SetActive(false);
            }

            int index = 0;
            if (isPublicity && !isBargin)
            {
                uint publicityTime = Sys_Trade.Instance.GetPublicityTime() / 3600u;
                _transArray[index].gameObject.SetActive(true);
                Text text = _transArray[index].Find("Text_Des").GetComponent<Text>();
                text.text = LanguageHelper.GetTextContent(2011021, publicityTime.ToString());
                index++;
            }

            bool isCheck = _commodityData.check;
            if (isCheck && !isBargin)
            {
                uint checkTime = Sys_Trade.Instance.GetCheckTime();
                _transArray[index].gameObject.SetActive(true);
                Text text = _transArray[index].Find("Text_Des").GetComponent<Text>();
                text.text = LanguageHelper.GetTextContent(2011022, checkTime.ToString());
                index++;
            }

            if (isBargin)
            {
                uint saleTime = Sys_Trade.Instance.SaleTime(_commodityData.treasure);
                _transArray[index].gameObject.SetActive(true);
                Text text = _transArray[index].Find("Text_Des").GetComponent<Text>();
                text.text = LanguageHelper.GetTextContent(2011049, saleTime.ToString(), saleTime.ToString());
                index++;
            }
            else
            {
                uint saleTime = Sys_Trade.Instance.SaleTime(_commodityData.treasure);
                _transArray[index].gameObject.SetActive(true);
                Text text = _transArray[index].Find("Text_Des").GetComponent<Text>();
                text.text = LanguageHelper.GetTextContent(2011019, saleTime.ToString());
                index++;
            }

            bool freePricing = _commodityData.pricing_type == 1u;
            if (freePricing && !isBargin)
            {
                _transArray[index].gameObject.SetActive(true);
                Text text = _transArray[index].Find("Text_Des").GetComponent<Text>();
                text.text = LanguageHelper.GetTextContent(2011023);
                index++;
            }

            //税率
            if (!isBargin)
            {
                uint rate = Sys_Trade.Instance.GetTradeRate(_brief);
                _transArray[index].gameObject.SetActive(true);
                Text textRate = _transArray[index].Find("Text_Des").GetComponent<Text>();
                textRate.text = LanguageHelper.GetTextContent(2011024, rate.ToString());
            }

            _isDisableTip = false;
            _toggle.isOn = _isDisableTip;

            Lib.Core.FrameworkTool.ForceRebuildLayout(_tipBg.gameObject);
        }
    }
}


