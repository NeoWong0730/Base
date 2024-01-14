using UnityEngine;
using System.Collections;
using Logic.Core;
using Logic;
using Packet;
using System.Collections.Generic;
using Table;
using Lib.Core;

public class UI_Team_Target_Parma
{
    public uint FocusID;

}
public class UI_Team_Target : UIBase, UI_Team_Target_Layout.IListener
{

    public class TargetItem
    {
        public string Name;

        public uint ID;

        public List<CSVTeam.Data>  mItems = new List<CSVTeam.Data>();
    }

    UI_Team_Target_Layout m_Layout = new UI_Team_Target_Layout();

    public SortedDictionary<uint, TargetItem> mtargetItemsDic = new SortedDictionary<uint, TargetItem>();

    private uint mFocusTarget;

    private uint mLowLv;
    private uint mHightLv;

    private bool mAutoFind = true;

    private bool mAllowRobot = true;

    private string mDesc = string.Empty;

    private uint mcurCustomEditID;
    private int mcurCustomEditIndex;

    private int mLastToggleTargetIndex;


    UI_Team_Target_Parma m_Parma = null;
    protected override void OnLoaded()
    {
        m_Layout.Loaded(gameObject.transform);

        LoadItemFromConfig();

        m_Layout.RegisterEvents(this);
    }


    protected override void ProcessEventsForEnable(bool toRegister)
    {

        Sys_Team.Instance.eventEmitter.Handle<uint>(Sys_Team.EEvents.CustomTargetInfo, OnCustomInfo, toRegister);

        Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.TargetDesc, OnTargetDesc, toRegister);
    }


    protected override void OnOpen(object arg)
    {
        var parmas = arg as UI_Team_Target_Parma;

        if (parmas != null)
        {
            m_Parma = parmas;
        }
        else
        {
            m_Parma = new UI_Team_Target_Parma();

            if (Sys_Team.Instance.teamTarget != null)
                m_Parma.FocusID = Sys_Team.Instance.teamTarget.TargetId;
        }
    }
    protected override void OnShow()
    {
        RefreshTargets();

        SetCurTargetInfo();


    }

    protected override void OnHide()
    {
        m_Layout.Hide();
    }

    protected override void OnClose()
    {

        if (m_MoveTargetTimer != null)
            m_MoveTargetTimer.Cancel();
    }
    protected override void OnUpdate()
    {
        m_Layout.Update(deltaTime);
    }
    private void RefreshTargets()
    {
        m_Layout.ItemGroup.SetChildSize(mtargetItemsDic.Count);

        int index = 0;
        foreach (KeyValuePair<uint, TargetItem> kvp in mtargetItemsDic)
        {
            TargetItem target = kvp.Value;

            var itemLayout = m_Layout.ItemGroup.getAt(index);

            if (itemLayout == null)
                continue;

            kvp.Value.mItems.Sort((a, b) =>
            {
                if (a.seq_id < b.seq_id)
                    return -1;
                if (a.seq_id > b.seq_id)
                    return 1;
                return 0;
            });

            itemLayout.NameText = target.Name;

            itemLayout.Index = index;
            itemLayout.ID = kvp.Key;

            itemLayout.mChildItemGroup.SetChildSize(target.mItems.Count);

            int childShowCount = 0;

            for (int n = 0; n < target.mItems.Count; n++)
            {
                if (itemLayout.mChildItemGroup.getAt(n) == null)
                    continue;

                if ( Sys_FunctionOpen.Instance.IsOpen(target.mItems[n].FunctionOpenId) == false)
                {
                    itemLayout.mChildItemGroup.getAt(n).Hide();
                    continue;
                }

                string name = string.Empty;
                bool isgetCustom = false;

                if (target.mItems[n].play_type == 0)
                {
                    isgetCustom = Sys_Team.Instance.getCustomTargetName(target.mItems[n].id, out name);
                }

                itemLayout.mChildItemGroup.getAt(n).text = isgetCustom ? name : LanguageHelper.GetTextContent(target.mItems[n].subclass_name);


                itemLayout.mChildItemGroup.getAt(n).Edit = (kvp.Key == 0);

                itemLayout.mChildItemGroup.getAt(n).Index = n;

                itemLayout.mChildItemGroup.getAt(n).ID = target.mItems[n].id;

                itemLayout.mChildItemGroup.getAt(n).OnClickEditAction = OnClickEdit;

                childShowCount += 1;
            }

            if (childShowCount == 0 && kvp.Key > 1)
            {
                itemLayout.Hide();
            }

            index += 1;

            
        }

    }

    private void LoadItemFromConfig()
    {
        foreach (var kvp in CSVTeam.Instance.GetAll())
        {
            if (kvp.module == 2)
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

                ti.mItems.Add(data);

                mtargetItemsDic.Add(data.play_type, ti);
            }

        }
    }

    private void SetTargetInfo(CSVTeam.Data target)
    {

        bool isTeamtarget = Sys_Team.Instance.isTeamTarget(target.id);

        m_Layout.SetLevelRangle((int)target.lv_min, (int)target.lv_max);

        uint min = isTeamtarget ? Sys_Team.Instance.teamTarget.LowLv : target.lv_min;

        uint max = isTeamtarget ? Sys_Team.Instance.teamTarget.HighLv : target.lv_max;

        mLowLv = min;
        mHightLv = max;

        m_Layout.MinLevevFocus((int)(min - target.lv_min));
        m_Layout.MaxLevelFocus((int)(max - target.lv_min));


    }


    private void SetCurTargetInfo()
    {

        CSVTeam.Data data =  m_Parma == null || m_Parma.FocusID == 0 ? mtargetItemsDic[0].mItems[0] : CSVTeam.Instance.GetConfData(m_Parma.FocusID);


        mFocusTarget = data.id;

        SetTargetInfo(data);

        m_Layout.SetDesc(Sys_Team.Instance.teamDesc);

        mDesc = Sys_Team.Instance.teamDesc;

        m_Layout.SetTips(LanguageHelper.GetTextContent(data.play_desc));


        m_Layout.SetAutoFind(Sys_Team.Instance.isAutoApply);

        mAutoFind = Sys_Team.Instance.isAutoApply;


        m_Layout.SetPlayTypeMask(true, data.play_type);

        //m_Layout.SetGrandSonFocus(data.id,true);

        mAllowRobot = Sys_Team.Instance.isTargetAllowRobot;

        m_Layout.SetAutoMatchRobbot(mAllowRobot);

        MoveTarget(data.id);

    }

    private Timer m_MoveTargetTimer = null;
    private void MoveTarget(uint id)
    {
        m_Layout.SetGrandSonFocus(id, true);

        if (m_MoveTargetTimer != null)
            m_MoveTargetTimer.Cancel();

        m_MoveTargetTimer = Timer.Register(0.2f, () => { m_Layout.MoveToTarget(id); });
    }
    public void Close()
    {
        UIManager.HitButton(EUIID.UI_Team_Target, "Close");

        UIManager.CloseUI(EUIID.UI_Team_Target);
    }

    //自动匹配
    public void AutoFind(bool b)
    {
        UIManager.HitButton(EUIID.UI_Team_Target, "AutoFind");

        if (Sys_RoleName.Instance.HasBadNames(mDesc))
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10023));

            return;
        }

        if (Sys_Team.Instance.OpenFamilySkipTips(mFocusTarget))
        {
            return;
        }
        UIManager.CloseUI(EUIID.UI_Team_Target);

        if (mLowLv > mHightLv)
        {
            var temp = mHightLv;
            mHightLv = mLowLv;
            mLowLv = temp;
        }

       // Debug.LogError("send chang target id  " + mFocusTarget.ToString());
        Sys_Team.Instance.ApplyEditTarget(mFocusTarget, mLowLv, mHightLv, mAutoFind, mAllowRobot, mDesc);
    }

    /// <summary>
    /// 点击子菜单
    /// </summary>
    /// <param name="targetID"></param>
    /// <param name="parent"></param>
    /// <param name="child"></param>
    public void ClickItem(uint targetID, int parent, int child)
    {
        UIManager.HitButton(EUIID.UI_Team_Target, "ClickItem");

        TargetItem target = null;

        if (mtargetItemsDic.TryGetValue(targetID, out target) == false)
            return;

        var data = CSVTeam.Instance.GetConfData(mFocusTarget);
        if (data != null)
            m_Layout.SetPlayTypeMask(false, data.play_type);

        mFocusTarget = target.mItems[child].id;

        SetTargetInfo(target.mItems[child]);

        m_Layout.SetTips(LanguageHelper.GetTextContent(target.mItems[child].play_desc));

        m_Layout.SetPlayTypeMask(true, parent);
    }

    private void OnClickEdit(uint id, int index)
    {
        UIManager.HitButton(EUIID.UI_Team_Target, "Edit");

        CSVTeam.Data data = CSVTeam.Instance.GetConfData(id);

        if (data == null)
            return;

        mcurCustomEditID = id;
        mcurCustomEditIndex = index;

        string targetName;

        bool result = Sys_Team.Instance.getCustomTargetName(id, out targetName);

        m_Layout.SetCustomInputText(result ? targetName : LanguageHelper.GetTextContent(data.subclass_name));

        m_Layout.OpenCustomInput();
    }
    public void OnMinLvChange(int index)
    {
        CSVTeam.Data data = CSVTeam.Instance.GetConfData(mFocusTarget);

        mLowLv = data.lv_min + (uint)index;
    }
    public void OnMaxLvChange(int index)
    {
        CSVTeam.Data data = CSVTeam.Instance.GetConfData(mFocusTarget);

        mHightLv = data.lv_min + (uint)index;
    }

    public void OnTextInputEnd(string text)
    {
        UIManager.HitButton(EUIID.UI_Team_Target, "InputEnd");

        mDesc =  text.Length > 10 ? text.Substring(0,10) : text;

        m_Layout.SetDesc(mDesc);

        Sys_Team.Instance.ApplyEditDesc(mDesc);
    }

    public void OnTextDelete()
    {
        UIManager.HitButton(EUIID.UI_Team_Target, "TextDelete");

        mDesc = string.Empty;

        m_Layout.SetDesc(mDesc);
    }

    public void OnCustomInput(string str)
    {
        UIManager.HitButton(EUIID.UI_Team_Target, "EditSure");

        Sys_Team.Instance.ApplyEditCustomInfo(mcurCustomEditID, str);

        m_Layout.CloseCustomInput();
    }

    public void OnAutoChange(bool b)
    {
        UIManager.HitButton(EUIID.UI_Team_Target, "Auto");

        mAutoFind = b;
    }
    private void OnCustomInfo(uint id)
    {
        var item = m_Layout.FindGrandSonByID(id);

        if (item == null)
            return;

        string text = string.Empty;

        bool result = Sys_Team.Instance.getCustomTargetName(id, out text);

        if (result)
            item.text = text;
    }

    private void OnTargetDesc()
    {
        mDesc = Sys_Team.Instance.teamDesc;

        m_Layout.SetDesc(mDesc);
    }

    public void OnTogAutoMatchRobbot(bool b)
    {
        mAllowRobot = b;
    }
}
