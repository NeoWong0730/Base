using UnityEngine;
using System.Collections;
using Logic.Core;
using Logic;
using Packet;
using System.Collections.Generic;
using Table;

public class UI_Team_OrderCompiler : UIBase, UI_Team_OrederCompiler_Layout.IListener
{

    //readonly uint ClearInt = 2002081;
    //readonly uint ClearAllInt = 2002082;
    //readonly uint MineInt = 2002062;
    //readonly uint EnemyInt = 2002071;


    UI_Team_OrederCompiler_Layout m_Layout = new UI_Team_OrederCompiler_Layout();

    private int mState = 0; // 0 我方，1 敌方

    private bool mCanEditorCustom = true;
    private List<string> OwnCommands { get { return Sys_Team.Instance.PlayerOwnCommands; } }

    private List<string> EnemyCommands { get { return Sys_Team.Instance.PlayerEnemyCommands; } }


    private bool m_FastEdit = false;

    private int m_FastEditIndex = -1;

    private bool m_FastAdd = false;
    public class Pamras
    {
        /// <summary>
        /// 是否可自定义
        /// </summary>
        public bool Custom { get; set; } = true;

        /// <summary>
        /// 默认选中 , 0 我方， 1 敌方
        /// </summary>
        public int Focus { get; set; } = 0;
    }
    protected override void OnOpen(object arg)
    {        
        Pamras parmes =  (arg as Pamras);

        if (parmes != null)
        {
            mCanEditorCustom = parmes.Custom;
            mState = parmes.Focus;
        }
       


    }
    protected override void OnLoaded()
    {
        m_Layout.Loaded(gameObject.transform);
    }

    protected override void ProcessEvents(bool toRegister)
    {
        m_Layout.RegisterEvents(this);        

        Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.WarCommandNtf, OnCommandNtf,toRegister);

        Sys_Team.Instance.eventEmitter.Handle<int>(Sys_Team.EEvents.WarCommandRes,OnCommandRefresh,toRegister);

        ProcedureManager.eventEmitter.Handle(ProcedureManager.EEvents.OnEnterNormal, OnEnterNormal, toRegister);

        Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.EditQuickCommandRes, OnQuickCommandRes, toRegister);
        
    }

    private void OnEnterNormal()
    {
        CloseSelf();
    }

    protected override void OnShow()
    {        
        m_Layout.Init(mState);

        if (mState == 0)
            SetMyCommand();

        if (mState == 1)
            SetEnemyCommand();

       bool  isHadCustom = Sys_Team.Instance.isHadCustomCommand();

        m_Layout.SetSystemOrderState(isHadCustom);

        m_FastEdit = false;

        OnRefreshFastCommand();
    }

    protected override void OnUpdate()
    {
        m_Layout.Update(deltaTime);
    }

    protected override void OnHide()
    {
        mState = 0;
        mCanEditorCustom = true;

        CloseSelf();

        m_FastEdit = false;
        m_FastAdd = false;
        m_Layout.SetFastEdit(false);
    }

    private void OnRefreshFastCommand()
    {
        int count = Sys_Team.Instance.FastCommand.Count;

        m_Layout.SetFastCount(count);

        for(int i = 0; i < count; i++)
        {
            var item = Sys_Team.Instance.FastCommand[i];

            bool bHad = item == null || item.Index < 0;

            m_Layout.SetFastItemHad(i, !bHad);

            string name = bHad ? string.Empty : item.Name;
       
            m_Layout.SetFastItem(i, name, i);
        
        }

        
        m_Layout.SetFastEdit(m_FastEdit && !m_FastAdd);


    }
    private void ClearCommand()
    {

    }

    private void ClearAllCommand()
    {

    }


    private void OnCommandNtf()
    {

        m_Layout.SetCommand(mState == 0 ? OwnCommands : EnemyCommands, mCanEditorCustom,isCustom);

        bool isHadCustom = Sys_Team.Instance.isHadCustomCommand();

        m_Layout.SetSystemOrderState(isHadCustom);

        OnRefreshFastCommand();
    }

    private bool isCustom(string name, int index)
    {
        if (index > 4)
            return true;

        return false;
    }
    private void OnCommandRefresh(int kind)
    {
        if(kind == 1 && mState == 0)
            m_Layout.SetCommand(OwnCommands, mCanEditorCustom, isCustom);

        if(kind == 0 && mState == 1)
            m_Layout.SetCommand(EnemyCommands, mCanEditorCustom, isCustom);

        bool isHadCustom = Sys_Team.Instance.isHadCustomCommand();

        m_Layout.SetSystemOrderState(isHadCustom);

        m_Layout.SetAddBtnActive(true);
    }

    private void SetMyCommand()
    {
        mState = 0;
        m_Layout.SetCommand(OwnCommands, mCanEditorCustom, isCustom);
    }
    public void MyCommand()
    {
        SetMyCommand();

        UIManager.HitButton(EUIID.UI_Team_WarCommand, "MyCommand");
    }

    private void SetEnemyCommand()
    {
        mState = 1;
        m_Layout.SetCommand(EnemyCommands, mCanEditorCustom, isCustom);
    }
    public void EnemyCommand()
    {
        SetEnemyCommand();
        UIManager.HitButton(EUIID.UI_Team_WarCommand, "EnemyCommand");
    }

    public void ResetCommand()
    {

        Sys_Team.Instance.ApplyRestAllWarCommand();

        UIManager.HitButton(EUIID.UI_Team_WarCommand, "ResetCommand");
    }

    public void EditCommand(int index, string newStr)
    {
        index = mState == 0 ? (index - Sys_Team.Instance.PlayerOwnCommandDefaultCount) : (index - Sys_Team.Instance.PlayerEnemyCommandDefaultCount);

        Sys_Team.Instance.ApplyWarEditAdd(mState == 0, (uint)index, newStr);

        UIManager.HitButton(EUIID.UI_Team_WarCommand, "EditCommand - index(" + index +") - " + newStr);
    }


    public void AddCommand(string newstr)
    {

        int index = mState == 0 ? (OwnCommands.Count - Sys_Team.Instance.PlayerOwnCommandDefaultCount): (EnemyCommands.Count - Sys_Team.Instance.PlayerEnemyCommandDefaultCount);

        Sys_Team.Instance.ApplyWarEditAdd(mState == 0, (uint)index, newstr);

        UIManager.HitButton(EUIID.UI_Team_WarCommand, "AddCommand-"+newstr);
    }


    public void Command( int index)
    {
        if (m_FastEdit)
        {
            Sys_Team.Instance.SetFastCommand(mState, m_FastEditIndex,index);

            OnRefreshFastCommand();

            m_FastAdd = false;

            m_FastEdit = false;

            m_Layout.SetAddBtnActive(true);
        }
    }
    public void Close()
    {
        CloseSelf();
        UIManager.HitButton(EUIID.UI_Team_WarCommand, "Close" );
    }

    public void OnLongPress()
    {
        m_FastEdit = true;
        m_Layout.SetFastEdit(true);
    }
    public void OnRemoveFast(int id)
    {
        Sys_Team.Instance.RemoveFastCommand(id);
        OnRefreshFastCommand();
    }

    public void OnClickFastAdd(int index)
    {
        m_FastEdit = true;
        //  m_Layout.SetFastEdit(true);
        m_FastAdd = true;

        m_Layout.SetAddBtnActive(false);

        m_FastEditIndex = index;

        OnRefreshFastCommand();
    }
    public void OnClickBack()
    {
        m_FastEdit = false;

        m_Layout.SetFastEdit(false);

        
    }

    private void OnQuickCommandRes()
    {
        OnRefreshFastCommand();
    }
}
