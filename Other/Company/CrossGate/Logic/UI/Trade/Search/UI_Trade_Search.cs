using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Logic.Core;
using Table;


namespace Logic
{
    public class UI_Trade_Search : UIBase
    {
        private Button m_BtnClose;

        //private PlayableDirector playableDirector;

        private UI_Trade_Search_Page m_Page;
        private UI_Trade_Search_Normal m_NormalPanel;
        private UI_Trade_Search_Equipment m_EquipmentPanel;
        private UI_Trade_Search_Pet m_PetPanel;
        private UI_Trade_Search_MagicCore m_MagicCore;
        private UI_Trade_Search_Ornament m_Ornament;

        private Sys_Trade.SearchPageType _SearchPage = Sys_Trade.SearchPageType.Normal;

        //protected virtual void OnInit() { }
        protected override void OnOpen(object arg)
        {
            if (arg != null)
                Sys_Trade.Instance.SearchParam.bCross = (bool)arg;
        }

        protected override void OnLoaded()
        {
            //m_Currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            //animator = transform.Find("Animator/View_TipsBg02_Big").GetComponent<Animator>();
            //animator.speed = 0f;

            m_BtnClose = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            m_BtnClose.onClick.AddListener(OnClickClose);

            //playableDirector = transform.Find("Animator/View_TipsBg02_Big/Timeline/Girl_In").GetComponent<PlayableDirector>();

            m_Page = new UI_Trade_Search_Page();
            m_Page.Init(transform.Find("Animator/View_Detail/View_Top/Toggles"));

            m_NormalPanel = new UI_Trade_Search_Normal();
            m_NormalPanel.Init(transform.Find("Animator/View_Detail/Image_BG0"));

            m_EquipmentPanel = new UI_Trade_Search_Equipment();
            m_EquipmentPanel.Init(transform.Find("Animator/View_Detail/Image_BG1"));

            m_PetPanel = new UI_Trade_Search_Pet();
            m_PetPanel.Init(transform.Find("Animator/View_Detail/Image_BG2"));
            
            m_MagicCore = new UI_Trade_Search_MagicCore();
            m_MagicCore.Init(transform.Find("Animator/View_Detail/Image_BG3"));
            
            m_Ornament = new UI_Trade_Search_Ornament();
            m_Ornament.Init(transform.Find("Animator/View_Detail/Image_BG4"));
        }

        protected override void OnShow()
        {
            //playableDirector.gameObject.SetActive(true);

            UpdateInfo();
        }

        protected override void OnHide()
        {
            Sys_Trade.Instance.SaveHistory();            
        }

        protected override void OnDestroy()
        {
            m_NormalPanel?.Destroy();
            m_EquipmentPanel?.Destroy();
            m_PetPanel?.Destroy();
            m_MagicCore?.Destroy();
            m_Ornament?.Destroy();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Trade.Instance.eventEmitter.Handle<Sys_Trade.SearchPageType>(Sys_Trade.EEvents.OnSearchPageType, OnSearchPageType, toRegister);
        }

        private void OnClickClose()
        {
            UIManager.CloseUI(EUIID.UI_Trade_Search, false, false);
        }

        private void UpdateInfo()
        {
            m_NormalPanel?.Hide();
            m_EquipmentPanel?.Hide();
            m_PetPanel?.Hide();
            m_MagicCore?.Hide();
            m_Ornament?.Hide();

            m_Page?.OnSelect(_SearchPage);
        }

        private void OnSearchPageType(Sys_Trade.SearchPageType type)
        {
            m_NormalPanel?.Hide();
            m_EquipmentPanel?.Hide();
            m_PetPanel?.Hide();
            m_MagicCore?.Hide();
            m_Ornament?.Hide();

            _SearchPage = type;
            switch (type)
            {
                case Sys_Trade.SearchPageType.Normal:
                    m_NormalPanel?.Show();
                    break;
                case Sys_Trade.SearchPageType.Equipment:
                    m_EquipmentPanel?.Show();
                    break;
                case Sys_Trade.SearchPageType.Pet:
                    m_PetPanel?.Show();
                    break;
                case Sys_Trade.SearchPageType.MagicCore:
                    m_MagicCore?.Show();
                    break;
                case Sys_Trade.SearchPageType.Ornament:
                    m_Ornament?.Show();
                    break;
            }
        }
    }
}


