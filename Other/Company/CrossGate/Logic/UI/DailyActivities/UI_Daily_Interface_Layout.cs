using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Daily_Interface_Layout
    {
        private Button m_Mall;
        public Transform transMallRed;
        private Button m_Activity;
        private Button m_LimitActivity;
        private Button m_ActivityName;
        private Text m_TexActivityName;
        private Text m_TexActivityNameTime;
        private Transform m_TransActivityRed;
        private Button m_Trade;
        private Button m_Open;
        private Image m_Open_Icon;
        public GameObject m_Menu;
        private Button m_Hangup;


        private Transform m_TransDailyRed;
        private Transform m_TransDailyNew;
        public void Load(Transform root)
        {
            m_Mall = root.Find("Grid01/Button_Mall").GetComponent<Button>();
            transMallRed = root.Find("Grid01/Button_Mall/Image_Dot");
            m_Activity = root.Find("Grid01/Button_Activity").GetComponent<Button>();
            m_LimitActivity = root.Find("Grid01/Button_LimitActivity").GetComponent<Button>();
            m_ActivityName = root.Find("Grid01/Button_ActivityName").GetComponent<Button>();
            m_TransActivityRed = root.Find("Grid01/Button_Activity/Image_RedTips");

            m_TexActivityName = m_ActivityName.transform.Find("Text").GetComponent<Text>();
            m_TexActivityNameTime = m_ActivityName.transform.Find("Text01").GetComponent<Text>();

            m_Trade = root.Find("Grid01/Button_Trade").GetComponent<Button>();
            m_Trade.gameObject.SetActive(false);

            m_Open = root.Find("Button01").GetComponent<Button>();  //--可以删除
            m_Open_Icon = root.Find("Button01/Image_Icon").GetComponent<Image>();
            m_Menu = root.Find("Grid01").gameObject;

            m_Hangup = root.Find("Grid01/Button_Hangup").GetComponent<Button>();

            m_TransDailyRed = m_Activity.transform.Find("Image_RedTips");

            m_TransDailyNew = m_Activity.transform.Find("Image_New");
        }

        public void SetListener(IListener listener)
        {
            m_Mall.onClick.AddListener(listener.OnClickMall);
            m_Activity.onClick.AddListener(listener.OnClickActivity);
            m_LimitActivity.onClick.AddListener(listener.OnClickLimitActivity);
            m_ActivityName.onClick.AddListener(listener.OnClickActivityName);
            m_Open.onClick.AddListener(listener.OnClickOpen);
            m_Trade.onClick.AddListener(listener.OnClickTrade);
            m_Hangup.onClick.AddListener(listener.OnClickHangup);
        }
        public interface IListener
        {
            void OnClickMall();
            void OnClickActivity();
            void OnClickLimitActivity();
            void OnClickActivityName();
            void OnClickOpen();
            void OnClickTrade();
            void OnClickHangup();
        }

        public void SetActivityActive(bool b)
        {
            m_Activity.gameObject.SetActive(b);
        }

        public void SetLimitActivityActive(bool b)
        {
            m_LimitActivity.gameObject.SetActive(b);
        }

        public void SetActivityNameActive(bool b)
        {
            m_ActivityName.gameObject.SetActive(b);
        }

        public void SetActivityRedActive(bool b)
        {
            m_TransActivityRed.gameObject.SetActive(b);
        }


        public void SetActivityNameActiveName(uint langue, string opentime)
        {
            m_TexActivityName.text = LanguageHelper.GetTextContent(langue);

            m_TexActivityNameTime.text = opentime;
        }

        public void SetOpenIconRoll(bool isExpand)
        {
            if (isExpand)
                m_Open_Icon.transform.localEulerAngles = new Vector3(0, 0, -90);
            else
                m_Open_Icon.transform.localEulerAngles = new Vector3(0, 0, 90);
        }

        public void SetDailyRed(bool active)
        {
            if (m_TransDailyRed.gameObject.activeSelf != active)
                m_TransDailyRed.gameObject.SetActive(active);
        }

        public void SetActivityNewActive(bool b)
        {
            m_TransDailyNew.gameObject.SetActive(b);
        }
    }
}
