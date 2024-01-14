using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Equipment_Layout
    {
        public GameObject root;
        private Transform trans;
        private Button btnClose;

        public UI_CurrencyTitle currencyTitle;
        public UI_Equipment_Inlay viewInlay;
        public UI_Equipment_Smelt viewSmelt;
        public UI_Equipment_Quenching viewQuenching;
        public UI_Equipment_Repair repair;
        public UI_Equipment_Make make;
        public UI_Equipment_ReMake reMake;
        public UI_Equipment_RefreshEffect refreshEffect;

        public UI_Equipment_Left leftList;
        public UI_Equipment_ViewLeftTabs leftTabs;

        public GameObject noneTip;
        private Text textNoneTip;

        public void Parse(GameObject _root)
        {
            root = _root;
            trans = root.transform;

            currencyTitle = new UI_CurrencyTitle(trans.Find("Animator/UI_Property").gameObject);

            btnClose = trans.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();

            viewInlay = new UI_Equipment_Inlay();
            viewInlay.Init(trans.Find("Animator/View_Gem"));

            viewSmelt = new UI_Equipment_Smelt();
            viewSmelt.Init(trans.Find("Animator/View_Smelt"));

            viewQuenching = new UI_Equipment_Quenching();
            viewQuenching.Init(trans.Find("Animator/View_Quenching"));

            repair = new UI_Equipment_Repair();
            repair.Init(trans.Find("Animator/View_Repair"));

            make = new UI_Equipment_Make();
            make.Init(trans.Find("Animator/View_Make"));
            
            reMake = new UI_Equipment_ReMake();
            reMake.Init(trans.Find("Animator/View_ReMake"));
            
            refreshEffect = new UI_Equipment_RefreshEffect();
            refreshEffect.Init(trans.Find("Animator/View_Clear"));

            leftList = new UI_Equipment_Left();
            leftList.Init(trans.Find("Animator/Scroll_Equip"));

            leftTabs = new UI_Equipment_ViewLeftTabs();
            leftTabs.Init(trans.Find("Animator/View_Left_Tabs"));

            noneTip = trans.Find("Animator/View_None").gameObject;
            textNoneTip = noneTip.transform.Find("View_Npc022/Text").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener)
        {
            btnClose.onClick.AddListener(listener.OnClickClose);
        }

        public void SetNoneTip(uint lanId)
        {
            textNoneTip.text = LanguageHelper.GetTextContent(lanId);
        }

        public interface IListener
        {
            void OnClickClose();
        }
    }
}


