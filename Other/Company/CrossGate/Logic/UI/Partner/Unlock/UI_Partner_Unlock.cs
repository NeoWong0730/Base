using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Partner_Unlock : UIBase
    {
        //partner
        private Image imgIcon;
        private Text textName;
        private Image imgProfession;

        private Text textCostNum;
        private Image imgCostIcon;
        //private Button btnUnlock;

        private uint infoId;
        private CSVPartner.Data partnerData;

        protected override void OnLoaded()
        {
            Button btnClose = transform.Find("Animator/View_TipsBg02_Small/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_PartnerUnlock);
            });

            imgIcon = transform.Find("Animator/Toggle_Partner01/Image_Icon").GetComponent<Image>();
            textName = transform.Find("Animator/Toggle_Partner01/Image_Frame01/Text_Name").GetComponent<Text>();
            imgProfession = transform.Find("Animator/Toggle_Partner01/Image_Frame01/Image_Profession").GetComponent<Image>();

            textCostNum = transform.Find("Animator/View_Lock/Text_Num").GetComponent<Text>();
            imgCostIcon = transform.Find("Animator/View_Lock/Icon").GetComponent<Image>();

            Button btnUnlock = transform.Find("Animator/View_Lock/Btn_01").GetComponent<Button>();
            btnUnlock.onClick.AddListener(OnClickUnlock);
        }

        protected override void OnOpen(object arg)
        {            
            infoId = (uint)arg;
            partnerData = CSVPartner.Instance.GetConfData(infoId);
        }

        //protected override void ProcessEventsForEnable(bool toRegister)
        //{            
        //    //Sys_Partner.Instance.eventEmitter.Handle<uint>(Sys_Partner.EEvents.OnNewPartnerNotification, OnUnlockSuccess, toRegister);
        //}

        protected override void OnShow()
        {            
            UpdateInfo();
        }        
        
        private void OnClickUnlock()
        {
            this.CloseSelf();
            Sys_Partner.Instance.PartnerUnlockReq(infoId);
        }

        private void UpdateInfo()
        {
            ImageHelper.SetIcon(imgIcon, partnerData.headid);
            textName.text = LanguageHelper.GetTextContent(partnerData.name);

            ImageHelper.SetIcon(imgProfession, OccupationHelper.GetIconID(partnerData.profession));

            ImageHelper.SetIcon(imgCostIcon, CSVItem.Instance.GetConfData(partnerData.deblock_condition[0]).icon_id);
            textCostNum.text = partnerData.deblock_condition[1].ToString();
        }

        //private void OnUnlockSuccess(uint _infoId)
        //{
        //    UIManager.CloseUI(EUIID.UI_PartnerUnlock);
        //}
    }
}


