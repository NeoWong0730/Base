using UnityEngine;
using System.Collections;
using Logic.Core;
using Logic;
using Packet;
using System.Collections.Generic;
using Table;

/*
 * 队伍信息，管理无队伍状态下 和 有队伍状态下 的角色展示 ， 点击菜单，修改队伍目标入口，战斗指令入口，一键喊话 ,委托
 */
public partial class UI_Team_Member : UIBase, UI_Team_Member_Layout.IListener
{
    readonly uint[] MemberStateArray = { 990304, 990303, 990302, 990301, 0 };

    private bool m_bDelegationCommand = false;
    private bool m_bChangeOrder = false;
    private ulong m_DoChangeMemID = 0;
    private int m_FocusItemIndex = 0;
    public enum MemberState
    {
        None,
        Create,
        Member,
        Cap
    }


    private MemberState m_MembersInfoState;
    public MemberState MembersInfoState
    {
        get { return m_MembersInfoState; }
        set
        {
            //if (m_MembersInfoState == value)
            //    return;
            m_MembersInfoState = value;
            SetState(m_MembersInfoState);
        }
    }

    private List<TeamMem> m_Memlist = new List<TeamMem>();

    private void SetState(MemberState memberState)
    {

        switch (memberState)
        {
            case MemberState.Create:
                ShowCreate();
                break;
            case MemberState.Member:
                ShowMember();
                break;
            case MemberState.Cap:
                ShowCap();
                break;
        }
    }

    private void RefreshTargetInfo()
    {
        if (Sys_Team.Instance.TeamMemsCount == 0)
        {
            string str = LanguageHelper.GetTextContent(2002025);
            m_Layout.SetTargetInfotex(str);
            return;
        }
        var data = CSVTeam.Instance.GetConfData(Sys_Team.Instance.teamTargetID);

        string targetDes;
        bool result = Sys_Team.Instance.getCustomTargetName(Sys_Team.Instance.teamTargetID, out targetDes);

        if (result == false)
            targetDes = LanguageHelper.GetTextContent(data.subclass_name);
        string languageid = data == null ? string.Empty :

           (LanguageHelper.GetTextContent(2002024) + targetDes // 
            + string.Format("    ({0}/{1})", Sys_Team.Instance.teamTarget.LowLv.ToString(), Sys_Team.Instance.teamTarget.HighLv.ToString()));



        m_Layout.SetTargetInfotex(LanguageHelper.GetTextContent(1000002, languageid));
    }

    private void ShowCreate()
    {
        m_Layout.CreateTeam();

        RefreshTargetInfo();

        if (m_Layout.isActive() == false)
            return;

        m_Layout.SetMemberSize(5);

        m_Layout.RestShowScene();

        m_Layout.SetTeamMarkActive(false);

        var id = Sys_Partner.Instance.GetCurFmList();
        var partner = Sys_Partner.Instance.GetFormationByIndex((int)id);

        int partenercount = partner.Pa.Count;

        int index = 0;

        SetRoleInfo(index++);


        for (int i = 0; i < partenercount; i++)
        {
            var value = partner.Pa[i];

            if (value > 0)
            {
                var partnerData = CSVPartner.Instance.GetConfData(value);

                var info = Sys_Partner.Instance.GetPartnerInfo(value);

                SetParternInfo(index++, partnerData, info);
            }

        }

        for (; index < 5; index++)
        {
            UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(index);
            member.SetEmpty(true);
        }


        m_Layout.UpdateShowScene();
    }


    private void SetRoleInfo(int index)
    {
        RoleBase role = Sys_Role.Instance.Role;

        UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(index);

        if (member == null)
            return;

        member.bSclect = false;

        member.bDelegation = false;

        member.Index = index + 1;

        member.MemType = 0;

        member.RoleID = role.RoleId;

        string familyName = string.Empty;
        uint familyPos = 0;
        if (Sys_Family.Instance.familyData != null && null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info && Sys_Family.Instance.familyData.isInFamily)
        {
            familyName = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GName.ToStringUtf8();
            familyPos = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.MyPosition % 10000;
        }

        SetMemberItemInfo(index, role.Name.ToStringUtf8(), role.Career, role.Level, Sys_Title.Instance.curShowTitle, role.CareerRank, familyName, familyPos,Sys_Attr.Instance.curTransformCard);

        member.SetEmpty(false);

        // member.OnDressID = GetRoleDress;

        var DressValue = GetDressData();

        // m_Layout.SetModelActorEx(index, role.HeroId, role.HeroId, role.Career, DressValue);

        uint dressID = GetRoleDress(role.RoleId);

        m_Layout.SetMemberModel(UI_Team_Member_Layout.EMemeModelShow.Role, (UI_Team_Member_Layout.EMemeModelShowPos)index, role.RoleId, role.HeroId, role.Career, Sys_Equip.Instance.GetCurWeapon(), dressID, DressValue);
    }

    private void SetParternInfo(int index, CSVPartner.Data info, Partner netinfo)
    {
        UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(index);

        if (member == null)
            return;

        member.bSclect = false;

        member.bDelegation = false;

        SetMemberItemInfo(index, LanguageHelper.GetTextContent(info.name), info.profession, netinfo.Level, 0, 0);

        member.SetEmpty(false);

        member.MemType = 4;

        var DressValue = GetDressData(info);

        //m_Layout.SetModelActorEx(index, info, DressValue);

        m_Layout.SetMemberModel(UI_Team_Member_Layout.EMemeModelShow.Partnet, (UI_Team_Member_Layout.EMemeModelShowPos)index, info.id, info.id, 0, 0, 0, DressValue);
    }

    private void SetTeamMemberInfo(int index, TeamMem info)
    {
        UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(index);

        if (member == null)
            return;

        member.bSclect = false;

        //var titleData = CSVTitle.Instance.GetConfData(info.Title);

        //var titleLan = titleData == null ? 0 : titleData.titleLan;

        SetMemberItemInfo(index, info.Name.ToStringUtf8(), info.Career, info.Level, info.Title, info.CareerRank, info.GuildName.ToStringUtf8(), info.GuildPos,info.ShapeShiftCardId);

        member.bDelegation = false;

        member.Index = index + 1;

        member.RoleID = info.MemId;

        member.MemType = getMemberState(info, Sys_Team.Instance.isCaptain(info.MemId), info.IsRob());

        member.WeaponID = info.WeaponItemID;

        // member.OnDressID = GetRoleDress;

        //RefreshMemberShowActor(index);

        member.SetmEntrustState(Sys_Team.Instance.isHadEntrust(info.MemId));

        member.SetEmpty(false);

        var DressValue = GetDressData(info);
        uint dressID = GetRoleDress(info.MemId);
        m_Layout.SetMemberModel(UI_Team_Member_Layout.EMemeModelShow.Role, (UI_Team_Member_Layout.EMemeModelShowPos)index, info.MemId, info.HeroId, info.Career, info.WeaponItemID, dressID, DressValue);

    }

    private void SetTeamMemberInfoTitle(int index, TeamMem info)
    {
        UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(index);

        if (member == null)
            return;

        SetMemberItemInfo(index, info.Name.ToStringUtf8(), info.Career, info.Level, info.Title, info.CareerRank, info.GuildName.ToStringUtf8(), info.GuildPos, info.ShapeShiftCardId);

    }
    /// <summary>
    /// 获取组队成员的时装模型ID
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private uint GetDressMainID(int index)
    {
        TeamMem mem = Sys_Team.Instance.getTeamMem(index - 1);

        if (mem == null)
            return 0;

        int count = mem.FashionList.Count;

        for (int i = 0; i < count; ++i)
        {
            EHeroModelParts part;
            bool bresult = Sys_Fashion.Instance.parts.TryGetValue(mem.FashionList[i].FashionId, out part);
            if (bresult && part == EHeroModelParts.Main)
            {
                return mem.FashionList[i].FashionId;
            }
        }

        return 100;
    }


    private uint GetRoleDress(ulong roleid)
    {
        if (Sys_Team.Instance.TeamMemsCount == 0)
            return GetRoleDressMainID(0);


        int index = Sys_Team.Instance.MemIndex(roleid) + 1;

        return GetDressMainID(index);
    }
    /// <summary>
    /// 获取玩家自己的时装模型ID
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private uint GetRoleDressMainID(int index)
    {
        return Sys_Fashion.Instance.GetDressedId(EHeroModelParts.Main);
    }
    private void SetMemberItemInfo(int index, string name, uint career, uint level, uint title, uint rank, string guildName = null, uint guildpos = 0,uint figuration = 0)
    {
        UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(index);


        member.Level = (int)level;

        member.Name = name;

        member.Index = index + 1;

        member.Profession = career;

        member.Rank = rank;

        var titleData = CSVTitle.Instance.GetConfData(title);

        string titlestr = string.Empty;


        bool isSpetype = false;

        if (titleData != null)
        {
            if (titleData.titleShowLan != 0)
                titlestr = LanguageHelper.GetTextContent(titleData.titleShowLan);

            isSpetype = (titleData.titleGetType == 9);

            if (titleData.titleGetType == 12)
            {
                titlestr = string.Empty;
                titleData = null;
            }
        }


        if (titleData != null && isSpetype && string.IsNullOrEmpty(guildName) == false)
        {
            titlestr = guildName;

            if (guildpos != 0)
            {
                var data = CSVFamilyPostAuthority.Instance.GetConfData(guildpos);

                if (data != null)
                {
                    titlestr += "-";
                    titlestr += LanguageHelper.GetTextContent(data.PostName);
                }

            }
            member.SetTitile(titlestr, titleData == null ? null : titleData.titleShow);
        }
        else
        {
            CSVWordStyle.Data wordStyle = null;
            if (titleData != null)
            {
                if (CSVLanguage.Instance.TryGetValue(titleData.titleShowLan, out CSVLanguage.Data data))
                {
                    wordStyle = CSVWordStyle.Instance.GetConfData(data.wordStyle);
                }
            }


            member.SetTitile(titlestr, wordStyle);
        }



        member.SetTitileIcon(titleData != null ? titleData.titleShowIcon : 0);

        member.TransFiguration.gameObject.SetActive(figuration > 0);
        if (figuration > 0)
            member.Figuration.SetData(figuration, false);
    }
    private uint getMemberState(TeamMem mem, bool isCap, bool isfriend)
    {


        if (mem.IsOffLine())
            return 3;

        if (mem.IsLeave())
            return 2;

        if (isCap)
            return 1;

        if (isfriend)
            return 4;

        return 0;
    }

    private void RefreshMembersInfo()
    {

        if (Sys_Team.Instance.TeamMemsCount == 0)
        {
            MembersInfoState = MemberState.Create;
            return;
        }

        if (Sys_Team.Instance.isCaptain(Sys_Role.Instance.Role.RoleId))
        {
            MembersInfoState = MemberState.Cap;
            return;
        }

        MembersInfoState = MemberState.Member;

    }
    private void ShowMember()
    {

        TeamMem teamMem = Sys_Team.Instance.getTeamMem(Sys_Role.Instance.RoleId);

        m_Layout.TeamMember(teamMem.IsLeave());

        SetMembersInfos();

        m_Layout.SetMatch(Sys_Team.Instance.isMatching);

        RefreshTargetInfo();

        m_Layout.SetTeamMarkActive(true);
        m_Layout.SetTeamMark(Sys_Team.Instance.TeamInfo.TeamInfo.Mems[0].TeamLogo);

        m_Layout.SetTeamMaskFx(Sys_Team.Instance.TeamMemsCount == 5);
    }

    private void ShowCap()
    {
        m_Layout.TeamCap();

        SetMembersInfos();

        m_Layout.SetMatch(Sys_Team.Instance.isMatching);
        RefreshTargetInfo();

        m_Layout.SetTeamMarkActive(true);
        m_Layout.SetTeamMark(Sys_Team.Instance.TeamInfo.TeamInfo.Mems[0].TeamLogo);
        m_Layout.SetTeamMaskFx(Sys_Team.Instance.TeamMemsCount == 5);
    }

    private void RefreshStateLayout()
    {
        switch (MembersInfoState)
        {
            case MemberState.Create:
                m_Layout.CreateTeam();
                RefreshTargetInfo();
                break;
            case MemberState.Member:
                {
                    TeamMem teamMem = Sys_Team.Instance.getTeamMem(Sys_Role.Instance.RoleId);

                    m_Layout.TeamMember(teamMem.IsLeave());

                    m_Layout.SetMatch(Sys_Team.Instance.isMatching);
                }
                break;
            case MemberState.Cap:
                {
                    m_Layout.TeamCap();
                    m_Layout.SetMatch(Sys_Team.Instance.isMatching);
                    RefreshTargetInfo();
                }

                break;
        }
    }
    private void ShowMembersDelegation()
    {
        if (m_Memlist.Count < 2)
            return;

        m_bDelegationCommand = true;
        for (int i = 0; i < m_Memlist.Count; i++)
        {

            UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(i);

            member.bDelegation = !Sys_Team.Instance.isCaptain(m_Memlist[i].MemId);
        }
    }

    private void HideMembersDelegation()
    {
        m_bDelegationCommand = false;
        for (int i = 0; i < m_Memlist.Count; i++)
        {
            UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(i);

            member.bDelegation = false;
        }
    }


    /// <summary>
    /// 调换位置
    ///roleID  发起方
    /// </summary>
    private void ShowMemberChangeOrder(ulong roleID)
    {
        int count = m_Memlist.Count;
        if (count < 2)
            return;

        m_DoChangeMemID = roleID;

        m_bChangeOrder = true;

        for (int i = 0; i < count; i++)
        {
            UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(i);

            var memID = m_Memlist[i].MemId;

            if (/*memID != Sys_Team.Instance.CaptainRoleId &&*/ memID != roleID)
                member.SetShowChangeOrder(true);
        }

    }

    private void HideMemberChangeOrder()
    {
        m_bChangeOrder = false;

        int count = m_Memlist.Count;

        for (int i = 0; i < count; i++)
        {
            UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(i);


            member.SetShowChangeOrder(false);
        }
    }
    public void CloseItemMenu()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "CloseTeamItemMenu");
        HideItemMenu();
    }
    private void SetMembersInfos()
    {
        if (m_Layout.isActive() == false)
            return;

        getMembers();

        int memCount = m_Memlist.Count;
        int count = 5;
        m_Layout.RestShowScene();
        m_Layout.SetMemberSize(count);




        int index = 0;
        for (int i = 0; i < m_Memlist.Count; i++, index++)
        {

            SetTeamMemberInfo(index, m_Memlist[i]);

        }

        if (memCount == 1)
        {
            var id = Sys_Partner.Instance.GetCurFmList();
            var partner = Sys_Partner.Instance.GetFormationByIndex((int)id);

            int partenercount = partner.Pa.Count;


            for (int i = 0; i < partenercount; i++)
            {
                var value = partner.Pa[i];

                if (value > 0)
                {
                    var partnerData = CSVPartner.Instance.GetConfData(value);

                    var info = Sys_Partner.Instance.GetPartnerInfo(value);

                    SetParternInfo(index++, partnerData, info);
                }

            }
        }


        for (; index < 5; index++)
        {
            UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(index);
            member.SetEmpty(true);
        }

        m_Layout.UpdateShowScene();
    }


    /// <summary>
    /// 刷新指挥标记
    /// </summary>
    private void RefreshEntrustState()
    {
        for (int i = 0; i < m_Memlist.Count; i++)
        {
            UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(i);

            member.SetmEntrustState(Sys_Team.Instance.isHadEntrust(m_Memlist[i].MemId));

        }
    }


    private readonly Dictionary<uint, List<dressData>> mDressValueNone = new Dictionary<uint, List<dressData>>();
    private Dictionary<uint, List<dressData>> GetDressData(int index)
    {
        // return Sys_Fashion.Instance.GetDressData();

        if (index >= m_Memlist.Count)
            return mDressValueNone;

        bool isTeam = Sys_Team.Instance.HaveTeam;

        Dictionary<uint, List<dressData>> DressValue = null;

        if (isTeam)
        {
            var config = m_Memlist[index].FashionList;//Sys_Team.Instance.getFashion(m_Memlist[index].MemId);

            if (config == null)
                return mDressValueNone;

            DressValue = Sys_Fashion.Instance.GetDressData(config, m_Memlist[index].HeroId);

            return DressValue;
        }

        return Sys_Fashion.Instance.GetDressData();


    }

    private Dictionary<uint, List<dressData>> GetDressData(TeamMem info)
    {
        var config = Sys_Team.Instance.getFashion(info.MemId);

        if (config == null)
            return mDressValueNone;

        Dictionary<uint, List<dressData>> DressValue = Sys_Fashion.Instance.GetDressData(config, info.HeroId);

        return DressValue;
    }


    private Dictionary<uint, List<dressData>> GetDressData(CSVPartner.Data info)
    {

        // Dictionary<uint, List<dressData>> DressValue = Sys_Fashion.Instance.GetDressData(info.);

        return mDressValueNone;
    }

    private Dictionary<uint, List<dressData>> GetDressData()
    {
        return Sys_Fashion.Instance.GetDressData();
    }


    private void RefreshMemberShowActor(int index)
    {
        var info = m_Memlist[index];

        var DressValue = GetDressData(index);

        uint dressID = GetRoleDress(info.MemId);

        m_Layout.SetMemberModel(UI_Team_Member_Layout.EMemeModelShow.Role, (UI_Team_Member_Layout.EMemeModelShowPos)index, info.MemId, info.HeroId, info.Career, info.WeaponItemID, dressID, DressValue);

    }
    private void RefreshMemberInfo(int teamIndex)
    {
        getMembers();


        UpdateMemberInfo(teamIndex);
    }

    private void RefreshMemberTitle(int index)
    {
        SetTeamMemberInfoTitle(index, m_Memlist[index]);
    }

    private void RefreshMemberLogoAndPhoto(int index)
    {
        if (index == 0)
        {
            m_Layout.SetTeamMark(m_Memlist[index].TeamLogo);
        }
    }
    private void UpdateMemberInfo(int index)
    {
        UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(index);

        if (member == null)
            return;

        member.MemType = getMemberState(m_Memlist[index], index == 0, m_Memlist[index].IsRob());

        member.Level = (int)m_Memlist[index].Level;

        member.Profession = m_Memlist[index].Career;
        member.Rank = m_Memlist[index].CareerRank;

        uint figuration = m_Memlist[index].ShapeShiftCardId;
        member.TransFiguration.gameObject.SetActive(figuration > 0);
        if (figuration > 0)
            member.Figuration.SetData(figuration, false);
    }

    private void UpdateMemberInfo(ulong roleid)
    {
        int index = -1;

        for (int i = 0; i < m_Memlist.Count; i++)
        {
            if (m_Memlist[i].MemId == roleid)
            {
                index = i;
                break;
            }
        }


        UpdateMemberInfo(index);

        RefreshStateLayout();
    }

    public bool isCreateTeam() { return m_MembersInfoState == MemberState.Create; }
    private void getMembers()
    {
        m_Memlist.Clear();

        foreach (TeamMem teamMem in Sys_Team.Instance.getTeamMems())
        {
            m_Memlist.Add(teamMem);
        }

        
        if (isCreateTeam())
        {
            RoleBase role = Sys_Role.Instance.Role;

            TeamMem mem = new TeamMem()
            {
                MemId = role.RoleId,
                Name = role.Name,
                State = 0,
                HeroId = role.HeroId,
                Career = role.Career,
                Level = role.Level
            };

            m_Memlist.Add(mem);
        }


        m_Memlist.Sort((value0, value1) =>
        {
            if (value0.FightPos > value1.FightPos)
                return 1;
            if (value0.FightPos < value1.FightPos)
                return -1;

            return 0;
        });
    }

    Vector3[] infoItemCorners = new Vector3[4];

    private void OnClickMemberInfo(int index)
    {
        UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(index - 1);

        if (member == null)
            return;

        ulong roleid = Sys_Role.Instance.Role.RoleId;

        if (index > m_Memlist.Count)
            return;

        TeamMem mem = m_Memlist[index - 1];//Sys_Team.Instance.getTeamMem(index - 1);

        if (mem == null)
            return;

       
        //委托指挥
        if (m_bDelegationCommand)
        {
            if (Sys_Team.Instance.isCaptain(mem.MemId))
                return;


            HideMembersDelegation();

            if (Sys_Team.Instance.isHadEntrust(mem.MemId))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002258, mem.Name.ToStringUtf8()));
                return;
            }


            Sys_Team.Instance.GiveToCommand(mem.MemId);

            return;
        }

        if (m_bChangeOrder)
        {
            if (/*Sys_Team.Instance.isCaptain(mem.MemId) == false &&*/ mem.MemId != m_DoChangeMemID)
                Sys_Team.Instance.SendChangeMemberOrder(m_DoChangeMemID, mem.MemId);

            HideMemberChangeOrder();

            return;
        }
        //弹出菜单
        if (mem != null && roleid != mem.MemId)
        {
            int memindex = Sys_Team.Instance.MemIndex(mem.MemId);

            RectTransform memRect = member.m_transform as RectTransform;

            memRect.GetWorldCorners(infoItemCorners);

            ShowItemMenu(infoItemCorners, (index == 4 ? 0 : 1), memindex + 1);

            FocusItem(index - 1);
        }


    }

    private void FocusItem(int index)
    {
        UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(index);

        if (member == null)
            return;

        m_FocusItemIndex = index;

        member.bSclect = true;
    }

    private void LostFocusItem()
    {
        UI_Team_Member_Layout.Member member = m_Layout.getMemberAt(m_FocusItemIndex);

        if (member == null)
            return;

        member.bSclect = false;
    }
    public void CreateTeam()
    {
        ulong roleid = Sys_Role.Instance.Role.RoleId;

        Sys_Team.Instance.ApplyCreateTeam(roleid);

        UIManager.HitButton(EUIID.UI_Team_Member, "CreateTeam");

        // RefreshMembersInfo();
    }

    //战斗指挥
    public void Command()
    {

        m_Layout.SetDelegaBtnActive(Sys_Team.Instance.isCaptain());
        m_Layout.ShowCommandTransMenu();

        UIManager.HitButton(EUIID.UI_Team_Member, "FightCommand");

    }

    //我的伙伴
    public void MyMate()
    {
        //throw new System.NotImplementedException();

        UIManager.OpenUI(EUIID.UI_Partner);

        UIManager.HitButton(EUIID.UI_Team_Member, "MyMate");
    }

    //便捷组队
    public void FastBuild()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "FastBuildTeam");


        if (Sys_FunctionOpen.Instance.IsOpen(30302, true) == false)
            return;

        UIManager.OpenUI(EUIID.UI_Team_Fast);

        // Sys_Hint.Instance.PushContent_Normal("功能开发中");
    }
    //一键喊话
    public void Talk()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "OneKeyTalk");
        m_Layout.SetTalkGridActive(true);


    }

    //退出队伍
    public void ExitTeam()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "ExitTeam");
        Sys_Team.Instance.ApplyExitTeam();
    }

    //暂时离队
    public void OffLineTeam()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "LeaveOrComabackTeam");

        TeamMem teamMem = Sys_Team.Instance.getTeamMem(Sys_Role.Instance.RoleId);

        if (teamMem.IsLeave())
            Sys_Team.Instance.ApplyComeBack(Sys_Role.Instance.RoleId);
        else
            Sys_Team.Instance.ApplyLeave(Sys_Role.Instance.RoleId);
    }

    public void CustomTarget()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "CustomTarget");

        UIManager.OpenUI(EUIID.UI_Team_Target);

    }

    public void AutoFind()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "AutoFind");

        if (Sys_Team.Instance.isCaptain())
        {
            uint tipsid = Sys_Team.Instance.isMatching ? 12113u : 12112u;

            if (!Sys_Team.Instance.isMatching && Sys_Team.Instance.TeamMemsCount >= 5)
                tipsid = 0;

            if (tipsid > 0)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(tipsid));
        }

        uint op = Sys_Team.Instance.isMatching ? (uint)1 : 0;

        if (op == 0 && Sys_Team.Instance.OpenFamilySkipTips(Sys_Team.Instance.teamTargetID))
        {
            return;
        }
        Sys_Team.Instance.ApplyMatching(op, Sys_Team.Instance.teamTargetID);
    }

    public void OnClickApplyTeam()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "ApplyTeam");

        UIManager.OpenUI(EUIID.UI_Team_Apply);
    }

    public void OnClickInvite()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "Invite");

        UIManager.OpenUI(EUIID.UI_Team_Invite, false, new UI_Team_Invite.Parmas());
        //ShowInviteMenu();
    }

    public void OnClickTeamMask()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "TeamMask");
        if (Sys_Team.Instance.isCaptain() == false)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002152));
            return;
        }


        UIManager.OpenUI(EUIID.UI_Head, false, EHeadViewType.TeamFalgView);
    }

    public void OnClickTalkGrid()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "OneKeyTalkClose");
        m_Layout.SetTalkGridActive(false);
    }
    public void OnClickTalkTeam()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "Shout-Team");

        OpenTalkMessage(0);

        m_Layout.SetTalkGridActive(false);
    }
    public void OnClickTalkFamliy()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "Shout-Famliy");
        OpenTalkMessage(1);

        m_Layout.SetTalkGridActive(false);
    }


    private void OpenTalkMessage(uint type)
    {
        if (Sys_Team.Instance.HaveTeam == false ||
            Sys_Team.Instance.teamTarget == null ||
            Sys_Team.Instance.teamTarget.TargetId == 0)
            return;

        var cdtype = type == 0 ? Sys_Team.CDController.EType.TalkTeam : Sys_Team.CDController.EType.TalkFamliy;
        if (Sys_Team.Instance.isCDReady(cdtype) == false)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11882, Sys_Team.Instance.RemainingCDTime(cdtype).ToString()));
            return;
        }

        UI_Team_TalkMessage_Parma parma = new UI_Team_TalkMessage_Parma();

        parma.Type = type;

        UIManager.OpenUI(EUIID.UI_Team_TalkMessage, false, parma);
    }
}
