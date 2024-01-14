using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using UnityEngine.Events;
using Framework;

//
public partial class UI_Team_Member_Layout
{
    

    IListener m_listener;

  
    public void Loaded(Transform root)
    {
        LoadTeam(root);

       // LoadContentMenu(root);

        LoadInviteMenu(root);

       // LoadApply(root);

        LoadItemMenu(root);

        LoadCommandTransMenu(root);
    }


    public void RegisterEvents(IListener listener)
    {
        m_listener = listener;

        m_BtnClose.onClick.AddListener(listener.Close);

        SetTeamInfoListener(listener);
    }

 





    public interface IListener
    {
        void CreateTeam();
        void Command();
        void MyMate();
        void FastBuild();

        void Talk();

        void OnClickTalkGrid();
        void OnClickTalkTeam();
        void OnClickTalkFamliy();

        void ExitTeam();

        void OffLineTeam();

        void Close();




        void EditCommand();
        void DelegationCommand();

        void CustomTarget();

        void CloseItemMenu();

        void AutoFind();

        void OnClickApplyTeam();

        void OnClickInvite();

        void OnClickTeamMask();
    }



 


 

 

  





}
