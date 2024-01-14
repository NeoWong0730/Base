using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using System;

//左边页签
public partial class UI_Team_Member_Layout
{
    private Transform m_ContentTransform;

    private ToggleGroup m_TGContent;

    Toggle m_toggle0;
    bool m_btoggle0;
 
    Toggle m_toggle1;
    bool m_btoggle1;

    //public Action<bool, int> OnClickToggle;

    //private Transform m_TransApplyRed;
    private void LoadContentMenu(Transform root)
    {
        m_ContentTransform = root.Find("Animator/View_Left_Tabs");//

        Transform listTran = m_ContentTransform.Find("Scroll View/TabList");

        m_TGContent = listTran.GetComponent<ToggleGroup>();


        m_toggle0 = listTran.Find("TabItem").GetComponent<Toggle>();

        m_toggle1 = listTran.Find("TabItem (1)").GetComponent<Toggle>();

        //m_toggle0.isOn = true;
        //m_toggle1.isOn = false;

        m_toggle0.onValueChanged.AddListener(OnClickToggle0);
        m_toggle1.onValueChanged.AddListener(OnClickToggle1);


        

    }

    private  void RegisterEventContent(bool b)
    {
        //if (b)
        //{
        //    m_toggle0.onValueChanged.AddListener(OnClickToggle0);
        //    m_toggle1.onValueChanged.AddListener(OnClickToggle1);
        //}
        //else
        //{
        //    m_toggle0.onValueChanged.RemoveListener(OnClickToggle0);
        //    m_toggle1.onValueChanged.RemoveListener(OnClickToggle1);
        //}
    }

    private void OnClickToggle0(bool state)
    {
       // if (m_btoggle0 != state)
        {
            m_btoggle0 = state;

            //OnClickToggle?.Invoke(state, 0);
        }
    }

    private void OnClickToggle1(bool state)
    {
        //if (m_btoggle1 != state)
        {
            m_btoggle1 = state;

           // OnClickToggle?.Invoke(state, 1);
        }
    }

    public void SetFocusContent(int index)
    {
        // m_TGContent.NotifyToggleOn()

        if (index == 0)
        {
            m_toggle0.isOn = true;
            m_toggle1.isOn = false;
        }



        if (index == 1)
        {
            m_toggle0.isOn = false;
            m_toggle1.isOn = true;
        }
           
    }

    public void CloseContent()
    {


        m_btoggle1 = false;
        m_btoggle0 = false;
    }



}
