using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using Logic.Core;

namespace Logic
{
    public class UI_Treasure_Display_Cell : UIParseCommon
    {
        //UI
        private GameObject mImgLock; 
        private Button mBtnAdd;
        private Button mImgOpenFactor;
        private Text mTxtLevelOpen;
        private Image mImgAmountOpen;
        private Text mTxtAmountOpen;

        private TreasureItem01 mTreasureItem;

        private uint mTreasureId;
        private CSVTreasuresUnlock.Data mUnlockData;

        protected override void Parse()
        {
            mImgLock = transform.Find("Image_Lock").gameObject;
            mBtnAdd = transform.Find("Button_Add").GetComponent<Button>();
            mBtnAdd.onClick.AddListener(OnClickAdd);
            mImgOpenFactor = transform.Find("Image_OpenFactor").GetComponent<Button>();
            mImgOpenFactor.onClick.AddListener(OnClickUnlock);
            mTxtLevelOpen = transform.Find("Image_OpenFactor/Text_Openlevel").GetComponent<Text>();
            mImgAmountOpen = transform.Find("Image_OpenFactor/Image_OpenAmount").GetComponent<Image>();
            mTxtAmountOpen = transform.Find("Image_OpenFactor/Image_OpenAmount/Text").GetComponent<Text>();

            mTreasureItem = new TreasureItem01();
            mTreasureItem.Bind(transform.Find("View_TreasureItem01").gameObject);
            mTreasureItem.transform.GetComponent<Button>().onClick.AddListener(OnClickTreasure);
            mTreasureItem.btn.onClick.AddListener(OnClickMinus);
        }

        public override void Show()
        {

        }

        public override void Hide()
        {

        }

        private void OnClickAdd()
        {
            //Debug.LogError("-------");
        }

        private void OnClickUnlock()
        {
            bool isFixLevel = Sys_Treasure.Instance.IsFixLevel(mUnlockData.store_lv);
            if (isFixLevel)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Item;
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2009206);
                PromptBoxParameter.Instance.itemId = mUnlockData.unlock[0];
                PromptBoxParameter.Instance.itemNum = mUnlockData.unlock[1];
               
                PromptBoxParameter.Instance.SetConfirm(true, CheckCanUnlock);
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                Sys_Adventure.Instance.ReportClickEventHitPoint("Treasure_Grid_Unlock_Open");
            }
            else
            {
                //TODO: tips
            }
        }

        private void OnClickTreasure()
        {
            UIManager.OpenUI(EUIID.UI_Treasure_Tips, false, mTreasureId);
            Sys_Adventure.Instance.ReportClickEventHitPoint("Treasure_IconTips_Open_TreasuresId" + mTreasureId);
        }

        private void OnClickMinus()
        {
            Sys_Treasure.Instance.UnEquipReq(mUnlockData.id);
            Sys_Adventure.Instance.ReportClickEventHitPoint("Treasure_Minus:" + mTreasureId);
        }

        private void CheckCanUnlock()
        {
            bool isCanUnlock = mUnlockData.unlock[1] <= Sys_Bag.Instance.GetItemCount(mUnlockData.unlock[0]);
            if (isCanUnlock)
            {
                Sys_Treasure.Instance.UnlockSlotReq();
            }
            else
            {
                ExchangeCoinParm exchangeCoinParm = new ExchangeCoinParm();
                exchangeCoinParm.ExchangeType = mUnlockData.unlock[0];
                exchangeCoinParm.needCount = mUnlockData.unlock[1];
                UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, exchangeCoinParm);
                //UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, mUnlockData.unlock[0]);
            }
            Sys_Adventure.Instance.ReportClickEventHitPoint("Treasure_Grid_Unlock_Confirm");
        }
        private void OnUnlockTipsCancel()
        {
            Sys_Adventure.Instance.ReportClickEventHitPoint("Treasure_Grid_Unlock_Cancel");
        }
        public void UpdateInfo(uint slotId)
        {
            mUnlockData = CSVTreasuresUnlock.Instance.GetConfData(slotId);

            bool isUnlock = Sys_Treasure.Instance.IsSlotUnlock(slotId);

            mImgLock.SetActive(false);
            mBtnAdd.gameObject.SetActive(false);

            if (isUnlock)
            {
                mImgOpenFactor.gameObject.SetActive(false);

                mTreasureId = Sys_Treasure.Instance.GetTreasureAtSlot(slotId);
                mTreasureItem.transform.gameObject.SetActive(mTreasureId != 0);

                if (mTreasureId != 0)
                {
                    CSVTreasures.Data data = CSVTreasures.Instance.GetConfData(mTreasureId);

                    ImageHelper.SetIcon(mTreasureItem.icon, data.icon_id);
                    mTreasureItem.textLevel.text = data.level.ToString();
                }
                else
                {
                    mBtnAdd.gameObject.SetActive(true);
                }
            }
            else
            {
                mTreasureItem.transform.gameObject.SetActive(false);

                bool isWaitUnlock = Sys_Treasure.Instance.IsWaitUnlock(slotId);
                if (isWaitUnlock)
                {
                    mImgOpenFactor.gameObject.SetActive(true);

                    bool isFixLevel = Sys_Treasure.Instance.IsFixLevel(mUnlockData.store_lv);
                    if (isFixLevel)
                    {
                        mTxtLevelOpen.gameObject.SetActive(false);
                        mImgAmountOpen.gameObject.SetActive(true);

                        uint itemId = mUnlockData.unlock[0];
                        uint itemNum = mUnlockData.unlock[1];
                        ImageHelper.SetIcon(mImgAmountOpen, CSVItem.Instance.GetConfData(itemId).icon_id);
                        mTxtAmountOpen.text = itemNum.ToString();
                    }
                    else
                    {
                        mTxtLevelOpen.gameObject.SetActive(true);
                        mImgAmountOpen.gameObject.SetActive(false);
                        mTxtLevelOpen.text = LanguageHelper.GetTextContent(2009205, mUnlockData.store_lv.ToString());
                    }
                }
                else
                {
                    mImgOpenFactor.gameObject.SetActive(false);
                    mImgLock.SetActive(true);
                }
            }
        }
    }
}
