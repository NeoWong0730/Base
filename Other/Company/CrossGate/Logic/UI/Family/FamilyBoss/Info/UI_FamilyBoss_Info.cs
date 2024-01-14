using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;


namespace Logic
{
    public class UI_FamilyBoss_Info : UIBase, UI_FamilyBoss_Info_Left.IListener
    {
        private UI_FamilyBoss_Info_Left m_Left;
        private UI_FamilyBoss_Info_Right m_Right;
        private Button m_btnRank;
        private Button m_btnTips;

        private Transform transRaceTip;

        protected override void OnLoaded()
        {
            Button btnClose = transform.Find("Animator/Image_Bg/Black").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);

            m_Left = new UI_FamilyBoss_Info_Left();
            m_Left.Init(transform.Find("Animator/View_Left"));
            m_Left.Register(this);

            m_Right = new UI_FamilyBoss_Info_Right();
            m_Right.Init(transform.Find("Animator/View_Right"));

            m_btnRank = transform.Find("Animator/View_Left/Button_Rank").GetComponent<Button>();
            m_btnRank.onClick.AddListener(OnClickRank);

            m_btnTips = transform.Find("Animator/View_Right/Button_Tips").GetComponent<Button>();
            m_btnTips.onClick.AddListener(OnClickTips);

            transRaceTip = transform.Find("Animator/Tips");
            Button btnTipClose = transRaceTip.Find("Image_Close").GetComponent<Button>();
            btnTipClose.onClick.AddListener(() => { transRaceTip.gameObject.SetActive(false); });
        }

        protected override void OnDestroy()
        {

        }
        protected override void OnOpen(object arg)
        {
            
        }

        protected override void OnShow()
        {
            m_Right.UpdateInfo(112);
            Sys_FamilyBoss.Instance.OnGuildBossSimpleInfoReq();
            Sys_Daily.Instance.CloseNewTips(112u);
            transRaceTip.gameObject.SetActive(false);
        }

        protected override void OnHide()
        {

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_FamilyBoss.Instance.eventEmitter.Handle<uint, uint>(Sys_FamilyBoss.EEvents.OnBossSimpleInfo, this.OnBossSimpleInfo, toRegister);
        }

        private void OnClickClose()
        {
            CloseSelf();
        }

        private void OnClickRank()
        {
            UIManager.OpenUI(EUIID.UI_FamilyBoss_Ranking);
        }

        private void OnClickTips()
        {
            UIRuleParam param = new UIRuleParam();
            param.TitlelanId = 3910010173;
            param.StrContent = LanguageHelper.GetTextContent(3910010174);
            UIManager.OpenUI(EUIID.UI_Rule, false, param);
        }

        private void OnBossSimpleInfo(uint partnerId, uint petRaceId)
        {
            m_Left.UpdateInfo(partnerId, petRaceId);
        }

        public void OnClickRace(uint raceId)
        {
            transRaceTip.gameObject.SetActive(true);
            CSVGenus.Data cSVGenusData = CSVGenus.Instance.GetConfData(raceId);
            if (cSVGenusData != null)
            {
                Text txt = transRaceTip.Find("Image_BG/Text").GetComponent<Text>();
                txt.text = LanguageHelper.GetTextContent(cSVGenusData.rale_name);
            }
        }
    }
}