using UnityEngine;
using System.Collections;
using Logic.Core;
using Logic;
using Packet;
using System.Collections.Generic;
using Table;

public class UI_Team_Fast : UIBase, UI_Team_Fast_Layout.IListener
{
    public class TargetItem
    {
        public string Name;

        public uint ID;

        //没有子类的时候有效
        public uint dataID = 0;

        public List<CSVTeam.Data>  mItems = new List<CSVTeam.Data>();
    }

    UI_Team_Fast_Layout m_Layout = new UI_Team_Fast_Layout();

    public Dictionary<uint, TargetItem> mtargetItemsDic = new Dictionary<uint, TargetItem>();

    private uint mShowType = 0;
    private uint mShowChildType = 0;

    private uint mShowAllID = 0;
    private uint mShowAllType = 0;

    private uint mFocusID = 0;
    protected override void OnLoaded()
    {
        LoadItemFromConfig();

        m_Layout.Loaded(gameObject.transform);

        InitContent();

        mShowAllID = getAllID();
        mShowAllType = getAllType();
    }

    protected override void ProcessEvents(bool toRegister)
    {
        m_Layout.RegisterEvents(this);        
    }

    protected override void ProcessEventsForEnable(bool toRegister)
    {
        Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.TargetList, OnRefreshList, toRegister);
        Sys_Team.Instance.eventEmitter.Handle<uint, uint, bool>(Sys_Team.EEvents.TargetMatching, OnMatching, toRegister);

        Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.HaveTeam, OnHaveTeam, toRegister);
    }

    protected override void OnOpen(object arg)
    {
        mFocusID = arg == null ? 0 : (uint)arg;

        var data = CSVTeam.Instance.GetConfData(mFocusID);

        if (mFocusID != 0 && data != null && Sys_FunctionOpen.Instance.IsOpen(data.FunctionOpenId) == false)
            mFocusID = 0;

       

        
    }
    protected override void OnShow()
    {
        //if (isVisibleAndOpen)
        //    return;

        uint targetid = mFocusID == 0 ? 1 : mFocusID;


        if (mFocusID == 0)
        {
            DoFocusType(1);
            mShowType = 1;
            mShowChildType = 0;

            Sys_Team.Instance.ApplyQueryMatchList(targetid, mFocusID == 0);
        }
            


      


        //base.OnShow();

        m_Layout.Init();

        if (Sys_Team.Instance.isMatching == false)
            m_Layout.SetMatchInteractable(false);


       // m_Layout.SetMatch(Sys_Team.Instance.isMatching);

        if (mFocusID > 0)
            DoFocusItem();


        if(Sys_Team.Instance.isMatching)
            SetMatchingSign(Sys_Team.Instance.MatchingTarget, true);

    }

    protected override void OnHide()
    {
        m_Layout.Hide();

        CloseSelf();
    }
    private void LoadItemFromConfig()
    {
        foreach (var kvp in CSVTeam.Instance.GetAll())
        {
            if (kvp.module == 1)
                continue;


            CSVTeam.Data data = kvp;


            if (kvp.play_type == 11 && Sys_GoddnessTrial.Instance.IsVailtInTeam(data) == false)
            {
                continue;
            }


            TargetItem ti;

            bool bresult = mtargetItemsDic.TryGetValue(data.play_type, out ti);


            if (bresult)
            {
                ti.mItems.Add(data);
            }
            else
            {
                ti = new TargetItem()
                {
                    ID = data.play_type,

                    Name = LanguageHelper.GetTextContent(data.play_name)

                };

                if (data.subclass > 0)
                    ti.mItems.Add(data);
                else
                    ti.dataID = data.id;

                mtargetItemsDic.Add(data.play_type, ti);
            }

        }
    }

    private void InitContent()
    {
        m_Layout.SetLeftItemCount(mtargetItemsDic.Count);

        int index = 0;
        foreach (KeyValuePair<uint, TargetItem> kvp in mtargetItemsDic)
        {
            kvp.Value.mItems.Sort((a, b) =>
            {

                if (a.seq_id < b.seq_id)
                    return -1;
                if (a.seq_id > b.seq_id)
                    return 1;
                return 0;
            });


            TargetItem target = kvp.Value;

            var itemLayout = m_Layout.getLeftItem(index);

            if (itemLayout == null)
                continue;

            itemLayout.NameText = target.Name;

            itemLayout.Index = index;

            itemLayout.ID = kvp.Key;

            itemLayout.isSign = kvp.Key == 1 ? false : true;

            itemLayout.SetChildCount(target.mItems.Count);

            int childShowCount = 0;

            for (int n = 0; n < target.mItems.Count; n++)
            {
                if (itemLayout.mChildItemGroup.getAt(n) == null)
                    continue;

                if (kvp.Key > 1 && Sys_FunctionOpen.Instance.IsOpen(target.mItems[n].FunctionOpenId) == false)
                {
                    itemLayout.mChildItemGroup.getAt(n).Hide();
                    continue;
                }
                itemLayout.mChildItemGroup.getAt(n).text = LanguageHelper.GetTextContent(target.mItems[n].subclass_name);

                itemLayout.mChildItemGroup.getAt(n).Index = n;

                itemLayout.mChildItemGroup.getAt(n).ID = target.mItems[n].id;

                childShowCount += 1;
            }

            if (childShowCount == 0 && kvp.Key > 1)
            {
                itemLayout.Hide();
            }
            index += 1;
        }

    }

    private void OnRefreshList()
    {
        m_Layout.SetContentItemCount(Sys_Team.Instance.MatchList.Count);

        int count = Sys_Team.Instance.MatchList.Count;

        m_Layout.SetWaitNum(Sys_Team.Instance.MatchInfo.TeamNum, Sys_Team.Instance.MatchInfo.RoleNum);

        for (int i = 0; i < Sys_Team.Instance.MatchList.Count; i++)
        {
            var member = Sys_Team.Instance.MatchList[i];

            var layItem = m_Layout.getContentItem(i);
            layItem.ID = member.TargetId;
            layItem.Index = i;
            layItem.RoleName = member.Leader.Name.ToStringUtf8();
            layItem.Desc = member.Desc.ToStringUtf8();
            layItem.Profession = OccupationHelper.GetTextID(member.Leader.Career, member.Leader.CareerRank);
            layItem.PropIcon = OccupationHelper.GetLogonIconID(member.Leader.Career);
            layItem.RoleIcon = CharacterHelper.getHeadID(member.Leader.HeroId, member.Leader.Photo);
            layItem.RoleFrameIcon = CharacterHelper.getHeadFrameID(member.Leader.PhotoFrame);
            layItem.Number = member.Leader.Level.ToString();


            var data = CSVTeam.Instance.GetConfData(member.TargetId);

            

           // string str0 = data != null ? LanguageHelper.GetTextContent(data.play_name) : string.Empty;

            string str1 = string.Empty;

            if (member.CustomInfo.IsEmpty == false)
                str1 = member.CustomInfo.ToStringUtf8();
            else
                str1 = data != null ? LanguageHelper.GetTextContent(data.subclass_name) : string.Empty;

            string str2 = string.Format("  ({0}/5)", member.CurNum.ToString());

            layItem.Task = str1 + str2;

            var memscount = member.Briefs.Count;

            layItem.SetMemberCount(memscount);
            for (int n = 0; n < memscount; n++)
            {
                layItem.SetMemberJob(n,OccupationHelper.GetLogonIconID(member.Briefs[n].Career), member.Briefs[n].Level);
            }


        }

        // m_Layout.RestContentLayout();

        //mShowChildType = 0;
        //mShowType = mShowAllType;
        //m_Layout.SetMark(0, true);

         m_Layout.ShowItemCondition(OnCondition, false);


    }

    private void OnHaveTeam()
    {
        CloseSelf();

        UIManager.OpenUI(EUIID.UI_Team_Member,false, UI_Team_Member.EType.Team);
    }
    private void OnMatching(uint lastid, uint id, bool isMatching)
    {
        m_Layout.SetMatch(Sys_Team.Instance.isMatching && Sys_Team.Instance.MatchingTarget == mShowChildType);

        if ((mShowType == mShowAllType  || mShowChildType == 0)&& Sys_Team.Instance.isMatching == false)
        {
            m_Layout.SetMatchInteractable(false);
        }

        SetMatchingSign(lastid, false);

        SetMatchingSign(id, true);

    }

    private void SetMatchingSign(uint lastid, bool active)
    {
        var data = CSVTeam.Instance.GetConfData(lastid);
        if (data == null)
            return;

        uint playType = data.play_type;
        var Typeitemid = m_Layout.getLeftItemByID(playType);
        var Typeitem = m_Layout.getLeftItem(Typeitemid);
        var childindex = Typeitem.getItemByID(lastid);

        var childitem = Typeitem.getItem(childindex);

        if (childitem == null)
            return;


        childitem.SetMatchActive(active);

    }
    private bool OnCondition(uint id, bool isType)
    {
        //if (isType == false && id != mShowChildType)
        //    return false;

        //if (isType)
        //{
        //    var data = CSVTeam.Instance.GetConfData(id);

        //    TargetItem item;

        //    bool result = mtargetItemsDic.TryGetValue(mShowType, out item);

        //    if (result && item.dataID != 0)
        //        return true;

        //    if (data.play_type != mShowType)
        //        return false;
        //}
        return true;
    }
    public void CreateTeam()
    {
        UIManager.HitButton(EUIID.UI_Team_Fast, "CreateTeam");


        if (Sys_Team.Instance.OpenFamilySkipTips(mShowChildType))
        {
            return;
        }
        ulong roleid = Sys_Role.Instance.Role.RoleId;

        Sys_Team.Instance.ApplyCreateTeam(roleid, mShowChildType);

        //CloseSelf();
    }

    /// <summary>
    /// 获得全部活动 条目的 ID
    /// </summary>
    /// <returns></returns>
    private uint getAllID()
    {
        foreach (KeyValuePair<uint, TargetItem> kvp in mtargetItemsDic)
        {
            if (kvp.Value.dataID != 0)
                return kvp.Value.dataID;
        }

        return 0;
    }

    /// <summary>
    /// 获得全部活动 条目的 类型
    /// </summary>
    /// <returns></returns>
    private uint getAllType()
    {
        foreach (KeyValuePair<uint, TargetItem> kvp in mtargetItemsDic)
        {
            if (kvp.Value.dataID != 0)
                return kvp.Key;
        }

        return 0;
    }

    /// <summary>
    /// 展开并将焦点转到 指定的活动ID 的UI上
    /// </summary>
    private void DoFocusItem()
    {
        var data = CSVTeam.Instance.GetConfData(mFocusID);

        if (data == null)
            return;

        uint playType = data.play_type;

        m_Layout.FocueChildItem(playType, mFocusID);


    }

    private void DoFocusType(uint type)
    {
        var value = m_Layout.getLeftItemByID(type);

        if (value < 0)
            return;

        var item = m_Layout.getLeftItem(value);

        item.HoldOn = true;
    }
    /// <summary>
    /// 获取玩法的类型 UI控件的下标
    /// </summary>
    /// <param name="id"> 玩法ID</param>
    /// <returns></returns>
    private int getFocusTypeIndex(uint id)
    {
        var data = CSVTeam.Instance.GetConfData(id);

        uint playType = data.play_type;

        var index = m_Layout.getLeftItemByID(playType);

        return index;

    }

    /// <summary>
    /// 获取玩法 UI控件的下标
    /// </summary>
    /// <param name="id">玩法ID</param>
    /// <param name="parentIndex">父控件的下标，可通过getFocusTypeIndex 获得</param>
    /// <returns></returns>
    private int getFocusChildIndex(uint id, int parentIndex)
    {
        var typeItem = m_Layout.getLeftItem(parentIndex);

        if (typeItem == null)
            return -1;

        int index = typeItem.getItemByID(id);

        return index;
    }


    public void Refresh()
    {
        UIManager.HitButton(EUIID.UI_Team_Fast, "Refresh");

        uint targetID = mShowChildType != 0 ? mShowChildType : mShowType;

        Sys_Team.Instance.ApplyQueryMatchList(targetID, mShowChildType == 0);
    }
    public void Match()
    {
        UIManager.HitButton(EUIID.UI_Team_Fast, "Match");

        if (mShowChildType == 0 && Sys_Team.Instance.isMatching == false)
            return;



        var data = CSVTeam.Instance.GetConfData(mShowChildType);

        if (Sys_Team.Instance.isMatching == false)
        {
            if (data != null && (data.lv_min > Sys_Role.Instance.Role.Level || data.lv_max < Sys_Role.Instance.Role.Level))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11630, data.lv_min.ToString(), data.lv_max.ToString()));
                return;
            }
        }


        bool isMatchitem = Sys_Team.Instance.MatchingTarget == mShowChildType && Sys_Team.Instance.isMatching;

        uint op = (uint)(isMatchitem ? 1 : 0);

        if (op == 0 && Sys_Team.Instance.OpenFamilySkipTips(mShowChildType))
        {        
            return;
        }

        if (Sys_Team.Instance.isMatching)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12113));
        }
        else
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12112));
        }

        Sys_Team.Instance.ApplyMatching(op, mShowChildType);
        //  Close();
    }

    public void Close()
    {
        UIManager.HitButton(EUIID.UI_Team_Fast, "Close");

        CloseSelf();
    }

    public void OnClickTypeContent(uint id, int index)
    {
        if (Sys_Team.Instance.isMatching == false)
            m_Layout.SetMatchInteractable(false);

        m_Layout.SetMark(index, true);

        mShowType = id;
        mShowChildType = 0;

        Sys_Team.Instance.ApplyQueryMatchList(mShowType,true);
        //m_Layout.ShowItemCondition(OnCondition, true);

        //TargetItem value;
        //bool result = mtargetItemsDic.TryGetValue(mShowType, out value);

        UIManager.HitButton(EUIID.UI_Team_Fast, "Type-ID(" + id.ToString() + ")" );
    }

    private void SetChildContent(uint parentID, int parentindex, uint id, int index)
    {
        m_Layout.SetMatchInteractable(true);

        m_Layout.SetMark(parentindex, true);

        mShowType = parentID;
        mShowChildType = id;

       // m_Layout.ShowItemCondition(OnCondition, false);

        Sys_Team.Instance.ApplyQueryMatchList(mShowChildType,false);

        m_Layout.SetMatch(Sys_Team.Instance.isMatching && Sys_Team.Instance.MatchingTarget == id);

        var teamdata = CSVTeam.Instance.GetConfData(id);

        m_Layout.SetFamilyTipsActive(teamdata != null && teamdata.only_guild);
    }
    
    public void OnFocusChildContent(uint parentID, int parentindex, uint id, int index)
    {
        SetChildContent(parentID, parentindex, id, index);
    }

    public void OnClickChildContent(uint parentID, int parentindex, uint id, int index)
    {
        //  if(mShowType == parentID && )
        SetChildContent(parentID, parentindex, id, index);

        UIManager.HitButton(EUIID.UI_Team_Fast, "ChildItem-ID(" + id.ToString() + ")");
    }

    public void Apply(int index)
    {
        UIManager.HitButton(EUIID.UI_Team_Fast, "Apply");

        if (index < 0 || index >= Sys_Team.Instance.MatchList.Count)
            return;

        var item = Sys_Team.Instance.MatchList[index];


        Sys_Team.Instance.ApplyJoinTeam(item.TeamId, Sys_Role.Instance.RoleId,1);
    }

}
