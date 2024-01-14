using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_Partner_Review_Right_ViewInfo
    {
        private Transform transform;

        //private Image imgIcon;
        //private Text textLabel;

        //private Image imgProfession;
        //private Text textName01;
        //private Text textProfession;

        private Slider sliderExp;
        private Text textPercent;
        private Text textLv;
        private Button btnMsg;

        private uint _infoId;
        private uint levelDiffMax = 0u;

        public void Init(Transform trans)
        {
            transform = trans;

            //textLabel = transform.Find("Tag_bg/Text").GetComponent<Text>();
            //imgIcon = transform.Find("Image_Icon").GetComponent<Image>();
            //imgProfession = transform.Find("Image_Profession").GetComponent<Image>();
            //textName02 = transform.Find("View_Info/Text_Name02").GetComponent<Text>();
            //textName01 = transform.Find("Text_Name01").GetComponent<Text>();
            //textProfession = transform.Find("Text_Profession").GetComponent<Text>();

            sliderExp = transform.Find("Slider_Exp").GetComponent<Slider>();
            textPercent = transform.Find("Text_Percent").GetComponent<Text>();
            textLv = transform.Find("Text_Level").GetComponent<Text>();
            btnMsg = transform.Find("Button_Message").GetComponent<Button>();
            btnMsg.onClick.AddListener(OnClickLevelUp);
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        private void OnClickLevelUp()
        {
            Partner partnerData = Sys_Partner.Instance.GetPartnerInfo(_infoId);
            if (partnerData != null)
            {
                if (partnerData.Level >= Sys_Role.Instance.Role.Level + levelDiffMax)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2006191, levelDiffMax.ToString()));
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_PartnerLevelUp, false, _infoId);
                }
            }
        }

        public void UpdateInfo(uint infoId)
        {
            _infoId = infoId;

            btnMsg.gameObject.SetActive(false);

            CSVPartner.Data infoData = CSVPartner.Instance.GetConfData(infoId);
            levelDiffMax = infoData.limit_level;

            //textLabel.text = LanguageHelper.GetTextContent(infoData.label);

            //ImageHelper.SetIcon(imgIcon, infoData.battle_headID);
            //ImageHelper.SetIcon(imgProfession, OccupationHelper.GetIconID(infoData.profession));

            //textName01.text = LanguageHelper.GetTextContent(infoData.name);
            //textProfession.text = LanguageHelper.GetTextContent(infoData.occupation);

            bool isUnlock = Sys_Partner.Instance.IsUnLock(infoId);
            if (!isUnlock)
            {
                textLv.text = "0";
                sliderExp.value = 0;
                textPercent.text = "";
            }
            else
            {
                Partner partnerData = Sys_Partner.Instance.GetPartnerInfo(infoId);
                if (partnerData != null)
                {
                    textLv.text = partnerData.Level.ToString();

                    if (partnerData.Level == 100u) //满级
                    {
                        sliderExp.value = 1.0f;
                        textPercent.text = LanguageHelper.GetTextContent(2006190);
                        return;
                    }

                    if (partnerData.Level >= infoData.auto_level)
                    {
                        CSVPartnerLevel.Data nextLevelData = CSVPartnerLevel.Instance.GetUniqData(infoId, partnerData.Level + 1u);
                        sliderExp.value = partnerData.Exp / (nextLevelData.upgrade_exp * 1.0f);
                        textPercent.text = string.Format("{0}/{1}", partnerData.Exp, nextLevelData.upgrade_exp);

                        btnMsg.gameObject.SetActive(true);
                    }
                    else
                    {
                        sliderExp.value = 0f;
                        textPercent.text = LanguageHelper.GetTextContent(2006199);
                    }
                }
            }
        }
    }
}


