using UnityEngine;
using System.Collections;
using Logic.Core;
using Logic;
using Packet;
using System.Collections.Generic;
using Table;

public class UI_Team_FrightingClick : UIBase, UI_Team_FrightingClick_Layout.IListener
{
   // readonly uint ClearInt = 2002081;
   // readonly uint ClearAllInt = 2002082;

    UI_Team_FrightingClick_Layout m_Layout = new UI_Team_FrightingClick_Layout();

    private int mState = 1; // 0 我方，1 敌方

    private FightActor mCurActor;
    private List<string> OwnCommands { get { return Sys_Team.Instance.PlayerOwnCommands; } }

    private List<string> EnemyCommands { get { return Sys_Team.Instance.PlayerEnemyCommands; } }


    private bool isWaitClickHero { get; set; } = false;

    private int CommandIndex { get; set; }


    private bool isClearCommand { get; set; } = false;

    private int ShowActorType { get; set; } = 0;

    private List<uint> mShowTemp = new List<uint>(0) {2};
    protected override void OnLoaded()
    {
        m_Layout.Loaded(gameObject.transform);
        
        m_Layout.RegisterEvents(this);
    }

    protected override void ProcessEventsForEnable(bool toRegister)
    {
        // Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.WarCommandNtf, OnCommandNtf,toRegister);

        //  Sys_Team.Instance.eventEmitter.Handle<int>(Sys_Role_WarCommand.EEvents.WarCommandRes,OnCommandRefresh,toRegister);

        Sys_Interactive.Instance.eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.Click, OnClickFrightHero, toRegister);

        ProcedureManager.eventEmitter.Handle(ProcedureManager.EEvents.OnEnterNormal, OnEnterNormal, toRegister);
    }

    private void OnEnterNormal()
    {
        CloseSelf();
    }
    protected override void OnShow()
    {        
        m_Layout.Init();

        m_Layout.SetActive(true);

        m_Layout.SetEnemyTeamToggleState(true);

        if (GameCenter.fightControl != null) //战斗对象没有时为非法
        {
            GameCenter.fightControl.isCommanding = true;

            GameCenter.fightControl.CommandingState = EFightCommandingState.Ready;
        }


        Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCommandChoose, true);
    }

    protected override void OnOpen(object arg)
    {
        var value = arg as FightActor;

    }


    protected override void OnHide()
    {        
       //if (isWaitClickHero)
        Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCombatOperateOver);

        isWaitClickHero = false;
        mState = 1;
        CommandIndex = 0;

        CloseSelf();

        if (GameCenter.fightControl != null)
            GameCenter.fightControl.CheckShowSelect(false, ShowActorType, mShowTemp, true, true);

        Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCommandChoose, false);

        if (GameCenter.fightControl != null)
            GameCenter.fightControl.CommandingState = EFightCommandingState.None;
    }
    public void Command(int index)
    {
        CommandIndex = index;

        isWaitClickHero = true;

        m_Layout.SetActive(false);

        ShowActorType = mState != GameCenter.mainFightHero.battleUnit.Side ? 6 : 5;

        GameCenter.fightControl.CheckShowSelect(true, ShowActorType, mShowTemp, true, true);


        Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCommandOperateStart);
    }
    public void Close()
    {

        CloseSelf();
    }

    public void Clear()
    {
        isClearCommand = true;
        ShowActorType = mState != GameCenter.mainFightHero.battleUnit.Side ? 6 : 5;

        GameCenter.fightControl.CheckShowSelect(true, ShowActorType, mShowTemp, true, true);
        //  GameCenter.fightControl.isCommanding = true;

        Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCommandOperateStart);

        isWaitClickHero = true;

        m_Layout.SetActive(false);

    }

    public void ClearAll()
    {
        var own = GameCenter.mainFightHero;

        int side = mState;

        Net_Combat.Instance.SendSetTagClearSide(side);

        CloseSelf();
    }

    public void AddCommand()
    {
        UIManager.OpenUI(EUIID.UI_Team_WarCommand, false, new UI_Team_OrderCompiler.Pamras() { Custom = false, Focus = mState });
    }

    public void OnClickMyTeam(bool state)
    {
        m_Layout.SetCommand(OwnCommands);

        mState =GameCenter.mainFightHero.battleUnit.Side;

        m_Layout.SetSideClearTex(2002082);
    }
    public void OnClickEnemyTeam(bool state)
    {
        m_Layout.SetCommand(EnemyCommands);

        mState = GameCenter.mainFightHero.battleUnit.Side == 1 ? 0 : 1;

        m_Layout.SetSideClearTex(2002084);
    }

    private void OnClickFrightHero(InteractiveEvtData data)
    {
        if (GameCenter.fightControl==null || isWaitClickHero == false || data.sceneActor == null)
            return;

        bool isSide = false;
        uint battleid = 0;

        if (data.sceneActor is MonsterPart)
        {
            isSide = GameCenter.fightControl.isOwnSideActor(data.sceneActor as MonsterPart);
            battleid = GameCenter.fightControl.getUnitID(data.sceneActor as MonsterPart);
        }

        else
        {
            isSide = GameCenter.fightControl.isOwnSideActor(data.sceneActor as FightActor);
            battleid = GameCenter.fightControl.getUnitID(data.sceneActor as FightActor);
        }

        if (GameCenter.fightControl.IsSelectActorRight(battleid) == false)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1012008));
            return;
        }

        uint uID = GameCenter.mainFightHero.battleUnit.UnitId;

        if (isSide && mState != GameCenter.mainFightHero.battleUnit.Side)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002103));//"标记的是我方英雄，请选择敌方"  );
            return;
        }
        if (!isSide && mState == GameCenter.mainFightHero.battleUnit.Side)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002104));//("标记的是敌方英雄，请选择我方");
            return;
        }

        uint isfrend = isSide ? (uint)1 : 0;

        uint isCap = (uint)(Sys_Team.Instance.isCaptain() ? 0 : 1);

        if (isClearCommand)
        {
            Net_Combat.Instance.SendSetTagClear(battleid);
            isClearCommand = false;
        }
        else
        {
            Net_Combat.Instance.SendSetTag(battleid, uID, (uint)CommandIndex, isCap, isfrend);

            if (GameCenter.fightControl != null && mState == 1 && CommandIndex == 0)
            {
                foreach (var kvp in GameCenter.fightControl.m_DicFightCommand)
                {
                    if (kvp.Value.CommandIndex == 0 && kvp.Value.Side == 0)
                    {
                        Net_Combat.Instance.SendSetTagClear(kvp.Value.MarkedID);
                    }
                }
            }
        }
            


        isWaitClickHero = false;

        GameCenter.fightControl.CheckShowSelect(false, ShowActorType, mShowTemp, true, true);


        Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnCombatOperateOver);

        CloseSelf();
    }
}
