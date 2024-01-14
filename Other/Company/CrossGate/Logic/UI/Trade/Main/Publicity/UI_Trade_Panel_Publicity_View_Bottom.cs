using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Trade_Panel_Publicity_View_Bottom
    {
        private Transform transform;
        private GameObject gameObject;

        private Button m_BtnRelation;
        private Button m_BtnRight;
        private Button m_BtnLeft;
        //private Text m_TextPage;
        private UI_Common_Num m_Num;

        private IListner m_Listner;
        private bool _isTurnPage;
        private string strText;

        public void Init(Transform trans)
        {
            transform = trans;
            gameObject = transform.gameObject;

            m_BtnRelation = transform.Find("Btn_01").GetComponent<Button>();
            m_BtnLeft = transform.Find("Button_Left").GetComponent<Button>();
            m_BtnRight = transform.Find("Button_Left/Button_Right").GetComponent<Button>();
            //m_TextPage = transform.Find("Button_Left/Text").GetComponent<Text>();
            m_Num = new UI_Common_Num();
            m_Num.Init(transform.Find("Button_Left/Image_input"));
            m_Num.RegEnd(OnNumInput);

            m_BtnRelation.onClick.AddListener(OnClickRelation);
            m_BtnLeft.onClick.AddListener(OnClickLeft);
            m_BtnRight.onClick.AddListener(OnClickRight);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnClickRelation()
        {
            Debug.LogErrorFormat("OnClickRelation");
        }

        private void OnClickLeft()
        {
            if (_isTurnPage)
                m_Listner?.OnPageLeft();
            else
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011167));
        }

        private void OnClickRight()
        {
            if (_isTurnPage)
                m_Listner?.OnPageRight();
            else
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011167));
        }
        
        private void OnNumInput(uint num)
        {
            if (_isTurnPage)
            {
                m_Listner?.OnChangePage(num);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011167));
                m_Num.Dsiplay(strText);
            }
        }

        public void Register(IListner listner)
        {
            m_Listner = listner;
        }

        public void EnableRelation(bool isActive)
        {
            m_BtnRelation.gameObject.SetActive(isActive);
        }

        public void SetPage(string page)
        {
            m_Num.Dsiplay(page);
            strText = page;
            CSVCommodityCategory.Data data = CSVCommodityCategory.Instance.GetConfData(Sys_Trade.Instance.CurPublicitySubCategory);
            _isTurnPage = data.turn_page;
        }

        public interface IListner
        {
            void OnPageLeft();
            void OnPageRight();
            void OnChangePage(uint num);
        }
    }
}


