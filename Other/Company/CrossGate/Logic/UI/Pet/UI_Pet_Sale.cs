using UnityEngine;
using System.Collections;
using Logic.Core;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Pet_Sale : UIBase
    {
        public uint petUid;
        public Text tipsText;
        public Button closeBtn;
        public Button tradeBtn;
        public Button exchangeBtn;
        public Button saleSliverBtn;
        public Button godPetExchangeBtn;
        public Button ruleBtn;
        public Button rule2Btn;
        protected override void OnLoaded()
        {
            tipsText = transform.Find("Animator/View_List/Image_Frame/Text_Tip").GetComponent<Text>();

            closeBtn = transform.Find("Animator/View_TipsBg02_Smallest/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(CloseBtnClicked);
            tradeBtn = transform.Find("Animator/View_List/Scroll_View/Grid/Stack/Button").GetComponent<Button>();
            tradeBtn.onClick.AddListener(TradeBtnClicked);
            exchangeBtn = transform.Find("Animator/View_List/Scroll_View/Grid/Exchange/Button").GetComponent<Button>();
            exchangeBtn.onClick.AddListener(ExchangeBtnClicked);
            godPetExchangeBtn = transform.Find("Animator/View_List/Scroll_View/Grid/Exchange_Stamp/Button").GetComponent<Button>();
            godPetExchangeBtn.onClick.AddListener(ExchangeStampBtnClicked);
            saleSliverBtn = transform.Find("Animator/View_List/Scroll_View/Grid/Sale/Button").GetComponent<Button>();
            saleSliverBtn.onClick.AddListener(SaleSliverBtnClicked);
            ruleBtn = transform.Find("Animator/View_List/Scroll_View/Grid/Exchange/Button_Tips").GetComponent<Button>();
            ruleBtn.onClick.AddListener(RuleBtnClicked);
            rule2Btn = transform.Find("Animator/View_List/Scroll_View/Grid/Exchange_Stamp/Button_Tips").GetComponent<Button>();
            rule2Btn.onClick.AddListener(RuleBtn2Clicked);
        }

        protected override void OnOpen(object arg = null)
        {
            if(null != arg)
                petUid = Convert.ToUInt32(arg);
        }

        protected override void OnShow()
        {
            ClientPet clientPet = Sys_Pet.Instance.GetPetByUId(petUid);
            if (null != clientPet)
            {
                bool canCharge = clientPet.petData.is_gold_charge;
                uint langId = 11968;
                if (canCharge)
                {
                    langId = 12471;
                }
                tipsText.text = LanguageHelper.GetTextContent(langId, clientPet.abandonCoin.ToString());
                godPetExchangeBtn.transform.parent.gameObject.SetActive(clientPet.petData.is_gold_charge && Sys_FunctionOpen.Instance.IsOpen(10552));
            }
        }

        private void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Sale);
        }

        private void TradeBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Sale);
            Sys_Trade.Instance.SaleItem(null);
        }

        private void ExchangeBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Sale);
            UIManager.OpenUI(EUIID.UI_Pet_Exchange, false, petUid);
        }

        private void ExchangeStampBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Sale);
            UIManager.OpenUI(EUIID.UI_Pet_Exchange_Stamp, false, petUid);
        }

        private void RuleBtnClicked()
        {
            UIRuleParam param = new UIRuleParam();
            param.StrContent = LanguageHelper.GetTextContent(12284);
            UIManager.OpenUI(EUIID.UI_Rule, false, param);
        }

        private void RuleBtn2Clicked()
        {
            UIRuleParam param = new UIRuleParam();
            param.StrContent = LanguageHelper.GetTextContent(12472);
            UIManager.OpenUI(EUIID.UI_Rule, false, param);
        }

        private void SaleSliverBtnClicked()
        {
            ClientPet clientPet = Sys_Pet.Instance.GetPetByUId(petUid);
            if(null != clientPet)
            {
                if (clientPet.petUnit.Islocked)
                {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, clientPet.GetPetNmae(), clientPet.GetPetNmae());
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {

                        Sys_Pet.Instance.OnPetLockReq(clientPet.GetPetUid(), false);
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    return;
                }
                
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(11969u, clientPet.abandonCoin.ToString());
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    if (clientPet != null)
                    {
                        UIManager.CloseUI(EUIID.UI_Pet_Sale);
                        Sys_Pet.Instance.OnPetAbandonPetReq(clientPet.petUnit.Uid);
                    }
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }
    }
}

