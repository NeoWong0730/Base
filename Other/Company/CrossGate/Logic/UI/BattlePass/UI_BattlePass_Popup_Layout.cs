using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Logic
{
    public class UI_BattlePass_Popup_Layout
    {

       // private ScrollRect m_SRView;

        private UIScrollCenterEx m_ItemGrid;

        Button m_BtnLeft;
        Button m_BtnRight;
        Button m_BtnStart;

        IListener m_Listener;
        public interface IListener
        {
            void OnClickStart();
            void OnClickLeft();
            void OnClickRight();

            string OnItemUpdate(int index);
        }
        public void Load(Transform root)
        {
           // m_SRView = root.Find("Animator/Pages/Mask/ScrollView").GetComponent<ScrollRect>();
            m_ItemGrid = root.Find("Animator/Pages/Mask/ScrollView").GetComponent<UIScrollCenterEx>();

            m_BtnLeft = root.Find("Animator/View_PageTurn/Arrow_Left/Button_Left").GetComponent<Button>();
            m_BtnRight = root.Find("Animator/View_PageTurn/Arrow_Right/Button_Right").GetComponent<Button>();
            m_BtnStart = root.Find("Animator/Pages/View_Bottom/Btn_01").GetComponent<Button>();

            m_ItemGrid.SetParam(null, 3);

        }

        public void SetListener(IListener listener)
        {
            m_Listener = listener;

            m_ItemGrid.m_itemSetHandler = OnItemGridUpdate;

            m_BtnLeft.onClick.AddListener(listener.OnClickLeft);

            m_BtnRight.onClick.AddListener(listener.OnClickRight);

            m_BtnStart.onClick.AddListener(listener.OnClickStart);
        }

        private void OnItemGridUpdate(int index, Transform trans)
        {
            string itemPath = m_Listener.OnItemUpdate(index);

            var rawimage = trans.Find("RawImage").GetComponent<RawImage>();

            ImageHelper.SetTexture(rawimage, itemPath);
        }

        public void SetCount(int count)
        {
            m_ItemGrid.scrollOffset = 0;
            m_ItemGrid.Init(count);

           
        }

        public void SetFocus(int index)
        {
            m_ItemGrid.SwitchIndex(index,false);
        }


        public void SetLeftActive(bool active)
        {
            m_BtnLeft.gameObject.SetActive(active);
        }

        public void SetRightActive(bool active)
        {
            m_BtnRight.gameObject.SetActive(active);
        }
    }
}
