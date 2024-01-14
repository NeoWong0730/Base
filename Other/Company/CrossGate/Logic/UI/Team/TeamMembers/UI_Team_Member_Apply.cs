using UnityEngine;
using System.Collections;
using Logic.Core;
using Logic;
using Packet;
using System.Collections.Generic;
using Table;

public partial class UI_Team_Member_Apply : UIBase, UI_Team_Member_Apply_Layout.IListener
{
    UI_Team_Member_Apply_Layout m_Layout = new UI_Team_Member_Apply_Layout();

    protected override void OnLoaded()
    {
        m_Layout.LoadApply(gameObject.transform);

        m_Layout.SetListener(this);
    }

    protected override void ProcessEventsForEnable(bool toRegister)
    {
        Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.NetMsg_ApplyListOpRes, OnApplyListOpResMsg, toRegister);
    }
    protected override void OnShow()
    {
        RefreshApply();
    }

    protected override void OnClose()
    {
        
    }
}
public partial class UI_Team_Member_Apply : UIBase, UI_Team_Member_Apply_Layout.IListener
{


    private void RefreshApply()
    {
        int count =  Sys_Team.Instance.ApplyRolesCount;

        m_Layout.SetApplyMemberSize(count);

        for (int i = 0; i < count; i++)
        {
          
            ApplyRole ar = Sys_Team.Instance.ApplyRoles[i];

            var applyMember = m_Layout.getApplyAt(i);

            applyMember.SetName(ar.Info.Name.ToStringUtf8());

            applyMember.SetLevel(ar.Info.Level);

            applyMember.SetProfession(ar.Info.Career, ar.Info.CareerRank);

            string title = ar.Info.Title == 0 ? string.Empty : LanguageHelper.GetTextContent(CSVTitle.Instance.GetConfData(ar.Info.Title).titleLan);
            applyMember.SetTitle(title);

            applyMember.Index = i;


            applyMember.MemID = ar.Info.MemId;

            applyMember.SetIcon(ar.Info.HeroId, ar.Info.Photo, ar.Info.PhotoFrame);

        }
    }

    public void OnClickClearApply()
    {
        Sys_Team.Instance.ClearApplyList();
    }

    public void OnClickClose()
    {
        CloseSelf();
    }

    public void OnClickExitTeam()
    {
        Sys_Team.Instance.ApplyExitTeam();

        CloseSelf();
    }

    public void OnClickAccpet(int index,ulong id)
    {
        ApplyRole applyRole = Sys_Team.Instance.getAtApplyRole(index);

        if (applyRole == null)
            return;

        Sys_Team.Instance.ApplyCaptainAnswer(0, applyRole.Info.MemId);
    }
    private void OnApplyListOpResMsg()
    {
        RefreshApply();
    }
}
