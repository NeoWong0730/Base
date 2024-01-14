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
    private Transform m_CommandTransMenu;

    private Button m_EditCommandBtn;
    private Button m_DelegaCommandBtn;

    private void LoadCommandTransMenu(Transform root)
    {
        m_CommandTransMenu = root.Find("Animator/View_Team/Menu_Grid_Command");

        m_DelegaCommandBtn = m_CommandTransMenu.Find("menu/Button_1").GetComponent<Button>();

        m_EditCommandBtn = m_CommandTransMenu.Find("menu/Button_2").GetComponent<Button>();

        m_DelegaCommandBtn.onClick.AddListener(OnClickDelegaCommand);
        m_EditCommandBtn.onClick.AddListener(OnClickEditCommand);


        Button btn  = m_CommandTransMenu.GetComponent<Button>();
        btn.onClick.AddListener(CloseCommandTransMenu);
    }

    private void OnClickEditCommand()
    {
        
        m_listener?.EditCommand();
       
    }

    private void OnClickDelegaCommand()
    {
        //HideCommandTransMenu();
        m_listener?.DelegationCommand();
        
    }


    public void HideCommandTransMenu()
    {
        m_CommandTransMenu.gameObject.SetActive(false);
    }

    public void ShowCommandTransMenu()
    {
        m_CommandTransMenu.gameObject.SetActive(true);
    }

    public void SetDelegaBtnActive(bool b)
    {
        m_DelegaCommandBtn.gameObject.SetActive(b);
   
    }
    private void CloseCommandTransMenu()
    {
        HideCommandTransMenu();
    }
}
