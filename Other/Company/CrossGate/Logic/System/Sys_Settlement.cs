using Logic;
using Net;
using Packet;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using System;

public class Sys_Settlement : SystemModuleBase<Sys_Settlement>
{

    public class ItemBase
    {
        public uint infoid
        {
            get;
            set;
        }
        public uint count
        {
            get;
            set;
        }

        public ItemBase(uint _infoid, uint _count)
        {
            infoid = _infoid;
            count = _count;
        }

    }
    public class SkillExp
    {
        public uint skillid
        {
            get;
            set;
        }
        public uint addexp
        {
            get;
            set;
        }

        public SkillExp(uint _skillid, uint _addexp)
        {
            skillid = _skillid;
            addexp = _addexp;
        }

    }
    public List<ItemBase> itemList = new List<ItemBase>();
    public List<SkillExp> skillList = new List<SkillExp>();
    public bool IsOpen;

    public override void Init()
    {
       
        //Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnBattleSettlement, OnEndBattle, true);
    }
    public override void OnLogin()
    {
        base.OnLogin();
        IsOpen = true;

    }

    public override void Dispose()
    {
        base.Dispose();
        skillList.Clear();
        itemList.Clear();
    }

    private void OnEndBattle(CmdBattleEndNtf ntf)
    {
        skillList.Clear();
        itemList.Clear();
        if (!IsOpen)
        {
            return;
        }
        for (int i = 0; i < ntf.Rewards.Items.Count; i++)
        {
            ItemBase item = new ItemBase(ntf.Rewards.Items[i].Infoid, ntf.Rewards.Items[i].Count);
            itemList.Add(item);
        }
        for (int i = 0; i < ntf.Rewards.Skills.Count; i++)
        {
            SkillExp skill = new SkillExp(ntf.Rewards.Skills[i].Skillid, ntf.Rewards.Skills[i].Addexp);
            skillList.Add(skill);
        }

        if (ntf.BattleResult == 1 && CombatManager.Instance.m_BattleTypeTb.CloseReward == 1)
        {
            if (!UIManager.IsVisibleAndOpen(EUIID.UI_MainInterface))
            {
                return;
            }
            UIManager.OpenUI(EUIID.UI_Mine_Result);
        }
    }

}

