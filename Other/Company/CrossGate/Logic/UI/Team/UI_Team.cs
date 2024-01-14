using UnityEngine;
using System.Collections;
using Logic.Core;
using Logic;
using Packet;

public class UI_Team : UIBase
{

    UI_Team_Layout m_Layout = new UI_Team_Layout();

    
    protected override void OnLoaded()
    {
        m_Layout.Loaded(gameObject.transform);

       // m_Team = new UI_Team_Team(m_Layout.m_Team.gameObject,this);

       // m_InMain = new UI_Team_InMain(m_Layout.m_InMain.gameObject,this);


    }

    protected override void ProcessEvents(bool toRegister)
    {
        Sys_Team.Instance.eventEmitter.Handle<CmdTeamInfoNtf>(Sys_Team.EEvents.NetMsg_InfoNtf, TeamInfo, toRegister);
    }



    protected void TeamInfo(CmdTeamInfoNtf info)
    {

    }

    /// <summary>
    /// 创建队伍
    /// </summary>
    public void TeamCreator()
    {
       // m_Team.ShowWithState(UI_Team_Team.EState.Creat);
    }

    /// <summary>
    /// 便捷组队
    /// </summary>
    public void TeamCreatorJion()
    {

    }

    /// <summary>
    /// 队伍界面
    /// </summary>
    /// <param name="isCapton">是否为队长</param>
    public void TeamInfoDetail(bool isCapton)
    {

    }

    /// <summary>
    /// 队长收到入队申请
    /// </summary>
    public void ApplyNtf()
    {

    }

    /// <summary>
    /// 收到邀请
    /// </summary>
    public void InviteNtf()
    {

    }

    /// <summary>
    /// 队长收到队员邀请他人入队申请
    /// </summary>
    public void CaptainInvite()
    {

    }

}
