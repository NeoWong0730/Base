using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Partner_Review_Right_ViewLockInfo
    {
        private Transform transform;

        //private Text textCostName;
        private Transform transTitle;
        private Image imgIcon;
        private Text textCostNum;
        private Text textUnlockDes;

        private Button btnUnLock;
        private Button btnNpcUnInquiry;
        private Button btnNpcInquiry;

        private uint infoId;
        private CSVPartner.Data partnerData;
        private bool isCanUnlock = false;
        private bool isOpen = true;

        public void Init(Transform trans)
        {
            transform = trans;

            Text textCostName = transform.Find("Image_Title/Text").GetComponent<Text>();
            textCostName.text = "";

            transTitle = transform.Find("Image_Title");
            imgIcon = transform.Find("Image_Title/Icon").GetComponent<Image>();
            textCostNum = transform.Find("Image_Title/Text_Num").GetComponent<Text>();
            textUnlockDes = transform.Find("Text").GetComponent<Text>();

            btnUnLock = transform.Find("Btn_01").GetComponent<Button>();
            btnUnLock.onClick.AddListener(OnClickUnlock);

            btnNpcUnInquiry = transform.Find("Btn_02").GetComponent<Button>();
            btnNpcUnInquiry.onClick.AddListener(OnClickUnInquiry);

            btnNpcInquiry = transform.Find("Btn_03").GetComponent<Button>();
            btnNpcInquiry.onClick.AddListener(OnClickInquiry);

            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChanged, true);
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void OnDestroy()
        {
            Sys_Bag.Instance.eventEmitter.Handle<uint,long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChanged, false);
        }

        private void OnClickUnlock()
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
                            //UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, partnerData.deblock_condition[0]);
                    }
                    break;
                case 2:
                    //UIManager.OpenUI(EUIID.UI_PartnerUnlock, false, infoId);
                    break;
                default:
                    break;
            }
        }

        private void OnClickUnInquiry()
        {
            UIRuleParam param = new UIRuleParam ();
            param.TitlelanId = 2006207u;
            param.StrContent = LanguageHelper.GetTextContent(partnerData.Favorability_info);

            UIManager.OpenUI(EUIID.UI_Rule, false, param);
        }

        private void OnClickInquiry()
        {
            UIManager.OpenUI(EUIID.UI_FavorabilityNPCShow, false, partnerData.npcID);
        }

        private void OnCurrencyChanged(uint id, long value)
        {
            if (infoId != 0)
            {
                UpdateInfo(infoId);
            }
        }

        public void UpdateInfo(uint _infoId)
        {
            infoId = _infoId;

            partnerData = CSVPartner.Instance.GetConfData(infoId);

            btnUnLock.gameObject.SetActive(false);
            btnNpcUnInquiry.gameObject.SetActive(false);
            btnNpcInquiry.gameObject.SetActive(false);

            textUnlockDes.gameObject.SetActive(false);
            transTitle.gameObject.SetActive(false);
            textCostNum.gameObject.SetActive(false);
            imgIcon.gameObject.SetActive(false);

            isOpen = true;

            //string des = "";
            switch (partnerData.deblock_type)
            {
                case 0: //任务解锁
                    //des = LanguageHelper.GetTextContent(2006901);
                    textUnlockDes.gameObject.SetActive(true);
                    textUnlockDes.text = LanguageHelper.GetTextContent(2006901);
                    break;
                case 1: //道具解锁
                    isOpen = Sys_Partner.Instance.CheckPartnerOpenUnlock(partnerData.open_unlock);
                    if (isOpen)
                    {
                        transTitle.gameObject.SetActive(true);
                        textCostNum.gameObject.SetActive(true);
                        imgIcon.gameObject.SetActive(true);

                        long totalNum = Sys_Bag.Instance.GetItemCount(partnerData.deblock_condition[0]);
                        //des = partnerData.deblock_condition[1].ToString();
                        isCanUnlock = partnerData.deblock_condition[1] <= totalNum;
                        uint costColorId = isCanUnlock ? (uint)2006208 : 2006209;
                        textCostNum.text = LanguageHelper.GetLanguageColorWordsFormat(partnerData.deblock_condition[1].ToString(), costColorId);
                        ImageHelper.SetIcon(imgIcon, CSVItem.Instance.GetConfData(partnerData.deblock_condition[0]).icon_id);
                        btnUnLock.gameObject.SetActive(true);
                    }
                    else
                    {
                        isCanUnlock = false;
                        textUnlockDes.gameObject.SetActive(true);
                        textUnlockDes.text = LanguageHelper.GetTextContent(partnerData.open_unlock_info);
                    }
                   
                    break;
                case 2: //好感度解锁
                    textUnlockDes.gameObject.SetActive(true);
                    textUnlockDes.text = LanguageHelper.GetTextContent(2006902);

                    if (Sys_FunctionOpen.Instance.IsOpen(20701u))
                    {
                        if (Sys_NPCFavorability.Instance.TryGetNpc(partnerData.npcID, out var npc, false))
                        {
                            btnNpcInquiry.gameObject.SetActive(true);
                        }
                        else
                        {
                            btnNpcUnInquiry.gameObject.SetActive(true);
                        }
                    }
                    break;
                case 3:
                    textUnlockDes.gameObject.SetActive(true);
                    textUnlockDes.text = LanguageHelper.GetTextContent(2006903);
                    break;
                default:
                    break;
            }
        }
    }
}


