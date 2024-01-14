using System;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Logic.Core;

namespace Logic
{
    public class UI_Partner_List_Right_Cell : UIParseCommon
    {
        public int gridIndex;
        private uint infoId;
        private Action<int> toggleAction;

        //UI
        //private Toggle toggle;
        private Button btn;
        private Image imgIcon;
        private Image imgNone;
        private Image imgQuality;
        private Text textName;
        private Image imgProfessionBg;
        private Image imgProfession;

        private GameObject objNeedLock;
        private Button btnNeedlock;
        private Text textNeedCost;
        private Image imgNeed;

        private Text textLabel;

        //unlock state
        CSVPartner.Data partnerData;
        bool isLock = false;
        bool isCanUnlock = false;
        bool isOpen = true;

        protected override void Parse()
        {
            btn = transform.GetComponent<Button>();
            btn.onClick.AddListener(OnClick);
            //toggle = transform.gameObject.GetComponent<Toggle>();
            //toggle.onValueChanged.AddListener((isOn) =>
            //{
            //    OnToggleClick(isOn, gridIndex);
            //});

            imgIcon = transform.Find("Image_Icon").GetComponent<Image>();
            imgNone = transform.Find("Image_IconNone").GetComponent<Image>();
            imgQuality = transform.Find("Image_Quality").GetComponent<Image>();
            textName = transform.Find("Image_Frame01/Text_Name").GetComponent<Text>();
            imgProfessionBg = transform.Find("Image_Frame01/Image_Professionbg").GetComponent<Image>();
            imgProfession = transform.Find("Image_Frame01/Image_Profession").GetComponent<Image>();

            objNeedLock = transform.Find("Image_Frame02").gameObject;
            btnNeedlock = transform.Find("Image_Frame02").GetComponent<Button>();
            btnNeedlock.onClick.AddListener(OnClickNeedLock);
            textNeedCost = transform.Find("Image_Frame02/Text_Need").GetComponent<Text>();
            imgNeed = transform.Find("Image_Frame02/Image_Need").GetComponent<Image>();

            transform.Find("Tag_bg").gameObject.SetActive(true);
            textLabel = transform.Find("Tag_bg/Text").GetComponent<Text>();

            Sys_Partner.Instance.eventEmitter.Handle<uint>(Sys_Partner.EEvents.OnNewPartnerNotification, OnUnlockSuccess, true);
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChanged, true);
        }

        public override void Show()
        {

        }

        public override void Hide()
        {
            //toggle.isOn = false;

        }

        public override void OnDestroy()
        {
            Sys_Partner.Instance.eventEmitter.Handle<uint>(Sys_Partner.EEvents.OnNewPartnerNotification, OnUnlockSuccess, false);
            Sys_Bag.Instance.eventEmitter.Handle<uint,long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChanged, false);

            base.OnDestroy();
        }

        private void OnClick()
        {
            UIManager.OpenUI(EUIID.UI_PartnerReview, false, infoId);
        }

        private void OnClickNeedLock()
        {
            if (partnerData == null)
            {
                Debug.LogError("partnerData is error");
                return;
            }

            switch (partnerData.deblock_type)
            {
                case 0:
                    break;
                case 1:
                    if (isOpen)
                    {
                        if (isCanUnlock)
                            UIManager.OpenUI(EUIID.UI_PartnerUnlock, false, infoId);
                        else
                        {
                            ExchangeCoinParm exchangeCoinParm = new ExchangeCoinParm();
                            exchangeCoinParm.ExchangeType = partnerData.deblock_condition[0];
                            exchangeCoinParm.needCount = 0;
                            UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, exchangeCoinParm);
                        }
                           // UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, partnerData.deblock_condition[0]);
                    }
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }

        private void OnCurrencyChanged(uint id, long value)
        {
            if (infoId != 0)
            {
                UpdateInfo(infoId, gridIndex);
            }
        }

        private void OnUnlockSuccess(uint _infoId)
        {
            if (_infoId == infoId)
            {
                UpdateInfo(_infoId, gridIndex);
            }
        }

        public void UpdateInfo(uint _infoId, int _index)
        {
            infoId = _infoId;
            gridIndex = _index;

            transform.gameObject.name = infoId.ToString();

            partnerData = CSVPartner.Instance.GetConfData(infoId);
            ImageHelper.SetIcon(imgIcon, partnerData.headid);

            CSVPartnerQuality.Data qualityData = CSVPartnerQuality.Instance.GetConfData(partnerData.quality);
            ImageHelper.SetIcon(imgQuality, qualityData.portrait);

            isOpen = true;
            isLock = Sys_Partner.Instance.IsUnLock(infoId);
            imgNone.gameObject.SetActive(!isLock);
            objNeedLock.gameObject.SetActive(!isLock);
            if (!isLock)
            {
                string des = "";
                switch (partnerData.deblock_type)
                {
                    case 0: //任务解锁
                        des = LanguageHelper.GetTextContent(2006901);
                        textNeedCost.text = des;
                        imgNeed.gameObject.SetActive(false);
                        break;
                    case 1: //道具解锁
                        isOpen = Sys_Partner.Instance.CheckPartnerOpenUnlock(partnerData.open_unlock);
                        if (isOpen)
                        {
                            long totalNum = Sys_Bag.Instance.GetItemCount(partnerData.deblock_condition[0]);
                            des = partnerData.deblock_condition[1].ToString();
                            isCanUnlock = partnerData.deblock_condition[1] <= totalNum;
                            uint costColorId = isCanUnlock ? (uint)1000997 : 1000998;
                            textNeedCost.text = LanguageHelper.GetLanguageColorWordsFormat(des, costColorId);
                            imgNeed.gameObject.SetActive(true);
                            ImageHelper.SetIcon(imgNeed, CSVItem.Instance.GetConfData(partnerData.deblock_condition[0]).small_icon_id);
                        }
                        else
                        {
                            isCanUnlock = false;
                            des = LanguageHelper.GetTextContent(partnerData.open_unlock_info);
                            textNeedCost.text = des;
                            imgNeed.gameObject.SetActive(false);
                        }
                        break;
                    case 2: //好感度解锁
                        des = LanguageHelper.GetTextContent(2006902);
                        textNeedCost.text = des;
                        imgNeed.gameObject.SetActive(false);
                        break;
                    case 3: //首充解锁
                        des = LanguageHelper.GetTextContent(2006903);
                        textNeedCost.text = des;
                        imgNeed.gameObject.SetActive(false);
                        break;
                    default:
                        break;
                }
            }

            textName.text = LanguageHelper.GetTextContent(partnerData.name);

            ImageHelper.SetIcon(imgProfession, OccupationHelper.GetCareerLogoIcon(partnerData.profession));
            ImageHelper.SetIcon(imgProfessionBg, 2516 + partnerData.quality);

            textLabel.text = LanguageHelper.GetTextContent(partnerData.label);
        }
    }
}
