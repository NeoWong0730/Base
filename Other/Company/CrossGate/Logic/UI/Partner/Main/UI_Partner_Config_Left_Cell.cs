using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using Logic.Core;

namespace Logic
{
    public class UI_Partner_Config_Left_Cell : UIParseCommon
    {
        public int gridIndex;
        private Partner partner;
        private bool isOpState = false;

        //UI
        private Toggle toggle;
        private PartnerItem01 partnerItem;
        private Text textName;
        private Text textLv;
        private Image imgProfession;
        private Text textProfession;
        private Image imgStatus;

        protected override void Parse()
        {
            toggle = transform.gameObject.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnClickToggle);

            partnerItem = new PartnerItem01();
            partnerItem.Bind(transform.Find("PartnerItem01").gameObject);
            partnerItem.transform.GetComponent<Toggle>().enabled = false;
            partnerItem.btnBlank.onClick.AddListener(OnClickBlank);

            textName = transform.Find("Text").GetComponent<Text>();
            textLv = transform.Find("Text_Lv").GetComponent<Text>();
            imgProfession = transform.Find("Image_Profession").GetComponent<Image>();
            textProfession = transform.Find("Text_Profession").GetComponent<Text>();
            imgStatus = transform.Find("Image_Mark").GetComponent<Image>();
        }

        public override void Show()
        {
            Sys_Partner.Instance.eventEmitter.Handle<Sys_Partner.PartnerFormOperation>(Sys_Partner.EEvents.OnFormSelectNotification, OnSelectFormPos, false);
            Sys_Partner.Instance.eventEmitter.Handle<Sys_Partner.PartnerFormOperation>(Sys_Partner.EEvents.OnFormSelectNotification, OnSelectFormPos, true);
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnFormRefreshNotification, OnRefresh, false);
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnFormRefreshNotification, OnRefresh, true);
        }

        public override void Hide()
        {
            toggle.isOn = false;

            Sys_Partner.Instance.eventEmitter.Handle<Sys_Partner.PartnerFormOperation>(Sys_Partner.EEvents.OnFormSelectNotification, OnSelectFormPos, false);
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnFormRefreshNotification, OnRefresh, false);
        }

        private void OnClickToggle(bool isOn)
        {
            if (isOn && partner != null)
            {
                if (isOpState)
                {
                    Sys_Partner.Instance.OnUpForm(partner.InfoId);
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_PartnerReview, false, partner.InfoId);
                }
            }
        }

        private void OnClickBlank()
        {
            if (isOpState && partner != null)
            {
                Sys_Partner.Instance.OnUpForm(partner.InfoId);
            }
        }
        

        private void OnSelectFormPos(Sys_Partner.PartnerFormOperation _formData)
        {
            if (partner != null)
            {
                OnDealSelectForm(_formData);
            }
        }

        private void OnDealSelectForm(Sys_Partner.PartnerFormOperation _formData)
        {
            isOpState = Sys_Partner.Instance.IsCanReplace(partner.InfoId, _formData);

            partnerItem.imgBlank.gameObject.SetActive(isOpState);
            partnerItem.imgArrowUp.gameObject.SetActive(isOpState);
        }

        private void OnRefresh()
        {
            if (partner != null)
            {
                UpdateInfo(partner, gridIndex);
            }
        }

        public void UpdateInfo(Partner _partner, int _index)
        {
            gridIndex = _index;
            partner = _partner;

            toggle.isOn = false;

            isOpState = false;
            partnerItem.HideBlank();

            bool isForm = Sys_Partner.Instance.IsInForm(_partner.InfoId);
            imgStatus.enabled = isForm;

            CSVPartner.Data baseData = CSVPartner.Instance.GetConfData(partner.InfoId);

            ImageHelper.SetIcon(partnerItem.imgIcon, baseData.battle_headID);
            textName.text = LanguageHelper.GetTextContent(baseData.name);
            textLv.text = LanguageHelper.GetTextContent(2006104, _partner.Level.ToString());
            textProfession.text = LanguageHelper.GetTextContent(baseData.occupation);

            ImageHelper.SetIcon(imgProfession, CSVCareer.Instance.GetConfData(baseData.profession).logo_icon);

            if (Sys_Partner.Instance.isSelectOp)
            {
                OnDealSelectForm(Sys_Partner.Instance.formSelectOp);
            }
        }
    }
}
