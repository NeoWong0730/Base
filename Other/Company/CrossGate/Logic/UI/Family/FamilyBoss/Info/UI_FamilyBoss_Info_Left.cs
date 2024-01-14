using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;


namespace Logic
{
    public class UI_FamilyBoss_Info_Left
    {
        private Transform transform;

        private Image m_ImgPartner;
        private Image m_ImgPet;
        private Button m_btnPartner;
        private Button m_btnRace;

        private IListener m_listner;
        private uint raceId;

        public void Init(Transform trans)
        {
            transform = trans;

            m_ImgPartner = transform.Find("Extra/Partner/Image_Icon").GetComponent<Image>();
            m_ImgPartner.gameObject.SetActive(false);

            m_btnPartner = transform.Find("Extra/Partner/Image_Icon").GetComponent<Button>();
            m_btnPartner.onClick.AddListener(OnClickPartner);

            m_ImgPet = transform.Find("Extra/Image_Frame/Image_Profession").GetComponent<Image>();
            m_ImgPet.gameObject.SetActive(false);
            
            m_btnRace = transform.Find("Extra/Image_Frame/Image_Profession").GetComponent<Button>();
            m_btnRace.onClick.AddListener(OnClickRace);
        }

        public void OnShow()
        {
            transform.gameObject.SetActive(true);
        }

        public void OnHide()
        {
            transform.gameObject.SetActive(false);
        }

        private void OnClickPartner()
        {
            UIManager.OpenUI(EUIID.UI_Partner);
        }

        private void OnClickRace()
        {
            m_listner?.OnClickRace(this.raceId);
        }

        public void Register(IListener listener)
        {
            m_listner = listener;
        }

        public void UpdateInfo(uint partnerId, uint petRaceId)
        {
            this.raceId = petRaceId;
            CSVPartner.Data csvPartnerData = CSVPartner.Instance.GetConfData(partnerId);
            if (csvPartnerData != null)
            {
                ImageHelper.SetIcon(m_ImgPartner, csvPartnerData.battle_headID);
                m_ImgPartner.gameObject.SetActive(true);
            }

            CSVGenus.Data cSVGenusData = CSVGenus.Instance.GetConfData(petRaceId);
            if (cSVGenusData != null)
            {
                ImageHelper.SetIcon(m_ImgPet, cSVGenusData.rale_icon);
                m_ImgPet.gameObject.SetActive(true);
            }
        }

        public interface IListener
        {
            void OnClickRace(uint raceId);
        }
    }
}