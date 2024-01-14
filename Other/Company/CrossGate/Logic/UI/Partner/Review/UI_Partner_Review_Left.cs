using Logic.Core;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class UI_Partner_Review_Left
    {
        private Transform transform;

        public Button btnPoint;
        public Button btnStory;
        public Button btnPresent;
        public Button btnWhole;
        public Button btnGenius;
        public Button btnProperty;

        private Text textLabel;
        private Image imgProfession;
        private Text textName01;
        private Text textProfession;

        private uint infoId;

        public void Init(Transform trans)
        {
            transform = trans;

            btnPoint = transform.Find("Btn_Point").GetComponent<Button>();
            btnPoint.onClick.AddListener(OnClickPoint);
            btnStory = transform.Find("Btn_Story").GetComponent<Button>();
            btnStory.onClick.AddListener(OnClickStory);
            btnPresent = transform.Find("Btn_Present").GetComponent<Button>();
            btnPresent.onClick.AddListener(OnClickPresent);
            btnWhole = transform.Find("Btn_Whole").GetComponent<Button>();
            btnWhole.onClick.AddListener(OnClickWhole);
            btnGenius = transform.Find("Btn_Genius").GetComponent<Button>();
            btnGenius.onClick.AddListener(OnClickGenius);
            
            btnProperty = transform.Find("Btn_shuxingqianghua").GetComponent<Button>();
            btnProperty.onClick.AddListener(OnClickProperty);

            textLabel = transform.Find("View_Info/Tag_bg/Text").GetComponent<Text>();
            //imgIcon = transform.Find("Image_Icon").GetComponent<Image>();
            //imgProfession = transform.Find("View_Info/Image_Profession").GetComponent<Image>();
            //textName02 = transform.Find("View_Info/Text_Name02").GetComponent<Text>();
            textName01 = transform.Find("View_Info/Text_Name01").GetComponent<Text>();
            textProfession = transform.Find("View_Info/Text_Profession").GetComponent<Text>();
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

        }

        private void OnClickPoint()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(10602, true)) { return; }
        }

        private void OnClickStory()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(10603, true)) { return; }
        }

        private void OnClickPresent()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(10604, true)) { return; }
        }

        private void OnClickWhole()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(10605, true)) { return; }
        }

        private void OnClickGenius()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(10606, true)) { return; }
            bool isUnLock = Sys_Partner.Instance.IsUnLock(infoId);
            if (isUnLock)
            {
                PartnerUIParam param = new PartnerUIParam();
                param.tabIndex = (int)EPartnerTabType.PartnerRuneEquip;
                param.partnerId = infoId;
                UIManager.OpenUI(EUIID.UI_Partner, false, param);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2006105u));
            }
        }

        private void OnClickProperty()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(10606, true)) { return; }
            bool isUnLock = Sys_Partner.Instance.IsUnLock(infoId);
            if (isUnLock)
            {
                PartnerUIParam param = new PartnerUIParam();
                param.tabIndex = (int)EPartnerTabType.PartnerProperty;
                param.partnerId = infoId;
                UIManager.OpenUI(EUIID.UI_Partner, false, param);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2006105u));
            }
        }

        public void UpdateInfo(uint _infoId)
        {
            infoId = _infoId;

            CSVPartner.Data infoData = CSVPartner.Instance.GetConfData(infoId);
            textLabel.text = LanguageHelper.GetTextContent(infoData.label);

            //ImageHelper.SetIcon(imgIcon, infoData.battle_headID);
            //ImageHelper.SetIcon(imgProfession, OccupationHelper.GetIconID(infoData.profession));

            textName01.text = LanguageHelper.GetTextContent(infoData.name);
            textProfession.text = LanguageHelper.GetTextContent(infoData.occupation);
        }
    }
}


