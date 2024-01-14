using System.Collections.Generic;
using Logic.Core;
using Lib.Core;
using Packet;
using Table;

namespace Logic
{
    public class HpValueUpdateEvt
    {
        public uint id;          //  0为主角 1为宠物
        public float ratio;       //当前hp百分比
    }

    public class MpValueUpdateEvt
    {
        public uint id;          //  0为主角 1为宠物
        public float ratio;       //当前mp百分比
    }

    public class ChooseEvt
    {
        public int type;     
        public List<uint> req; 
        public uint id;
        public bool isHero;
        public bool isSealPet;
    }

    public class AutoBattleSkillEvt
    {
        public uint heroid;
        public uint heroid2;
        public uint petid;
        public bool isPetSkillUpdate;
    }

    public enum EFightCommandingState
    {
        None,
        Ready,
        Opreating,
        Over
    }

    public class FightCommand
    {
        public uint MarkedID;
        public uint CommandIndex;
        public uint Side;
    }

    public class BossHpUpdateEvt
    {
        public int curHp;
        public uint maxHp;
    }

    public class FightControl : Actor
    {
        public EOperationState operationState;

        public List<BattleCommand> battleCommands = new List<BattleCommand>();

        public EOperationType currentOperationTpye;
        public uint currentOperationID;
        public uint currentUsePetTime;

        public List<MobEntity> m_SelectList = new List<MobEntity>();
        public List<MobEntity> m_SelectListTeamOp = new List<MobEntity>();
        public List<uint> hasCommends = new List<uint>();
        public List<uint> forbidpetsList = new List<uint>();
        private List<MobEntity> m_SelftList = new List<MobEntity>();
        private List<MobEntity> m_EnemytList = new List<MobEntity>();
        private List<MobEntity> m_TempList = new List<MobEntity>();
        private List<MobEntity> m_TempList01 = new List<MobEntity>();
        private List<MobEntity> m_TempList02 = new List<MobEntity>();
        private List<MobEntity> m_TempList03 = new List<MobEntity>();
        private List<SkillColdDwonInfo> m_TempSkillColdList = new List<SkillColdDwonInfo>();
        public List<SkillColdDwonInfo> m_HeroSkillColdList = new List<SkillColdDwonInfo>();
        public List<SkillColdDwonInfo> m_PetSkillColdList = new List<SkillColdDwonInfo>();
        public Dictionary<uint, BossHpUpdateEvt> BossUnitDic = new Dictionary<uint, BossHpUpdateEvt>();
        public List<GroupScore> groupScores = new List<GroupScore>();

        public Timer fxtime;
        private uint curSkillId;
        public uint sequenceRound;
        public float doubleTime;
        public AutoBattleSkillEvt autoSkillEvt=new AutoBattleSkillEvt ();
        public bool isDoRound { get; set; } = false;


        public bool isCommanding { get; set; } = false;
        public bool isGuide { get; set; } = false;

        public EFightCommandingState CommandingState { get; set; } = EFightCommandingState.None;

        public Dictionary<uint, FightCommand> m_DicFightCommand = new Dictionary<uint, FightCommand>();
        public bool CanUseSkill
        {
            get;
            set;
        } = true;

        public enum EOperationType
        {
            None = 0,
            NormalAttack = 1,               //普通攻击
            CastSkill = 2,                  //使用技能
            Pet = 3,                        //宠物操作
            Defense = 4,                    //防御
            UseItem = 5,                    //使用道具
            ChangePos = 6,                  //换位置
            Escape = 7,                     //逃跑
        }

        public enum EOperationState
        {
            None,
            WaitForFirstOperation,          //等待1动操作
            WaitForFirstChoose,             //等待1动选择
            WaitForSecondOperation,         //等待2动操作
            WaitForSecondChoose,            //等待2动选择
            OperationOver,                  //操作结束
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            currentUsePetTime = GetPetMaxFightTime();
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnRoundNtf, OnRoundNtf, true);
            Net_Combat.Instance.eventEmitter.Handle<uint, bool>(Net_Combat.EEvents.OnSkillColdUpdate, OnSkillColdUpdate, true);
            float dTime = 0;
            float.TryParse(CSVParam.Instance.GetConfData(612).str_value, out dTime);
            doubleTime = dTime / 1000;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnRoundNtf, OnRoundNtf, false);
            Net_Combat.Instance.eventEmitter.Handle<uint, bool>(Net_Combat.EEvents.OnSkillColdUpdate, OnSkillColdUpdate, false);
            operationState = EOperationState.None;
            isCommanding = false;
            forbidpetsList.Clear();
            m_HeroSkillColdList.Clear();
            BossUnitDic.Clear();
            Clear();
            Net_Combat.Instance.ClearAttackCountDic(); 
            m_DicFightCommand.Clear();
            curSkillId = 0;
            sequenceRound = 0;
            autoSkillEvt.heroid = 0;
            autoSkillEvt.heroid2 = 0;
            autoSkillEvt.petid = 0;
            isGuide = false;
            groupScores.Clear();
        }

        void OnRoundNtf()
        {
            Clear();
            operationState = EOperationState.WaitForFirstOperation;
        }

        #region SkillCold
        private void OnSkillColdUpdate(uint skillId, bool isHero)
        {
            bool hasSameSkill = false;
            for (int i = 0; i < m_HeroSkillColdList.Count; ++i)
            {
                if (m_HeroSkillColdList[i].SkillId == skillId)
                {
                    hasSameSkill = true;
                    m_HeroSkillColdList[i].LastRound = (int)Net_Combat.Instance.m_CurRound;
                    break;
                }
            }
            if (!hasSameSkill && CSVActiveSkill.Instance.GetConfData(skillId).cold_time != 0)
            {
                SkillColdDwonInfo info = new SkillColdDwonInfo();
                info.SkillId = (int)skillId;
                if (isHero)
                {
                    info.LastRound = (int)Net_Combat.Instance.m_CurRound;
                    m_HeroSkillColdList.Add(info);
                }
            }
        }

        public void RefreshSkillColdData()
        {
            m_TempSkillColdList.Clear();
            for (int i = 0; i < m_HeroSkillColdList.Count; ++i)
            {
                uint coldTime = Net_Combat.Instance.m_CurRound - (uint)m_HeroSkillColdList[i].LastRound;
                if (coldTime > CSVActiveSkill.Instance.GetConfData((uint)m_HeroSkillColdList[i].SkillId).cold_time)
                {
                    m_TempSkillColdList.Add(m_HeroSkillColdList[i]);
                }
            }
            for (int i = 0; i < m_TempSkillColdList.Count; ++i)
            {
                if (m_HeroSkillColdList.Contains(m_TempSkillColdList[i]))
                {
                    m_HeroSkillColdList.Remove(m_TempSkillColdList[i]);
                }
            }
        }

        public SkillColdDwonInfo HaveSkillCold(uint skillId)
        {
            for (int i = 0; i < m_HeroSkillColdList.Count; ++i)
            {
                if (m_HeroSkillColdList[i].SkillId == skillId)
                {
                    return m_HeroSkillColdList[i];
                }
            }
            return null;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">被标记单位的ID</param>
        /// <param name="commandIndex">命令下标</param>
        /// <param name="side">敌方or己方</param>
        public void SetFightCommand(uint id, bool isClear, uint commandIndex, uint side)
        {
            if (isClear && m_DicFightCommand.ContainsKey(id))
            {
                m_DicFightCommand.Remove(id);
            }
            else
            {
                FightCommand fightCommand;

                if (m_DicFightCommand.TryGetValue(id, out fightCommand))
                {
                    fightCommand.CommandIndex = commandIndex;
                    fightCommand.Side = side;
                }
                else {

                    fightCommand = new FightCommand() { MarkedID = id, CommandIndex = commandIndex, Side = side };

                    m_DicFightCommand.Add(id, fightCommand);
                }
            }
        }

        //通过普攻或者技能按钮进行攻击
        public void AttackById(uint skillId, uint itemId)
        {
            ChooseEvt evt = new ChooseEvt();
            evt.type = CSVActiveSkill.Instance.GetConfData(skillId).choose_type;
            evt.req = CSVActiveSkill.Instance.GetConfData(skillId).choose_req;
            evt.id = skillId;
            evt.isHero = true;
            curSkillId = skillId;
            if (skillId == Constants.NEARNORMALATTACKID) //普攻
            {
                currentOperationTpye = EOperationType.NormalAttack;
                currentOperationID = GameCenter.mainFightHero.heroSkillComponent.GetNormalAttackSkill().CSVActiveSkillInfoData.id;
            }
            else
            {
                if (itemId != 0)
                {
                    currentOperationTpye = EOperationType.UseItem;
                    currentOperationID = itemId;
                }
                else
                {
                    currentOperationTpye = EOperationType.CastSkill;
                    currentOperationID = skillId;
                }
            }
            //第二次选择，是否有宠物
            if (operationState == EOperationState.WaitForSecondOperation)
            {
                MobEntity pet = null;
                if (Sys_Fight.Instance.HasPet())
                {
                    pet = MobManager.Instance.GetMob((uint)GameCenter.mainFightPet.uID);
                }

                if (pet != null && !pet.m_MobCombatComponent.m_Death)
                {
                    evt.isHero = false;

                    //如果是普攻， 设置当前技能id为宠物技能
                    if (skillId == Constants.NEARNORMALATTACKID)
                    {
                        currentOperationID = GameCenter.mainFightPet.fightPetSkillComponent.GetNormalAttackSkill().CSVActiveSkillInfoData.id;
                    }
                }
            }

            if (operationState == EOperationState.WaitForFirstOperation)
            {
                operationState = EOperationState.WaitForFirstChoose;
            }
            else if (operationState == EOperationState.WaitForSecondOperation)
            {
                operationState = EOperationState.WaitForSecondChoose;
            }

            m_SelftList.Clear();
            m_EnemytList.Clear();
            m_SelftList = GetSameCampAlive(true, GameCenter.mainFightHero.battleUnit, m_SelftList);
            m_EnemytList = GetSameCampAlive(false, GameCenter.mainFightHero.battleUnit, m_EnemytList);
            switch (evt.type)
            {
                case -1:
                    DoSelectedMob(null, 0);
                    break;
                case 7: //choose my self ,no select
                    DoSelectedMob(MobManager.Instance.GetMob((uint)GameCenter.mainFightHero.uID));
                    break;
                case 8:   //choose enemy all, no select
                    DoSelectedMob(m_EnemytList[0]);
                    break;
                case 9:   //choose self all, no select
                    DoSelectedMob(MobManager.Instance.GetMob((uint)GameCenter.mainFightHero.uID));
                    break;
                case 14:   //choose enemy front row, no select
                    for (int i = 0; i < m_EnemytList.Count; ++i)
                    {
                        if (IsFrontRow(m_EnemytList[i]))
                        {
                            DoSelectedMob(m_EnemytList[i]);
                            break;
                        }
                    }
                    if (GameCenter.mainFightHero.battleUnit.Pos < 20)
                    {
                        DoSelectedMob(null, 20);
                    }
                    else
                    {
                        DoSelectedMob(null, 10);
                    }
                    break;
                case 15:   //choose self front row, no select
                    for (int i = 0; i < m_SelftList.Count; ++i)
                    {
                        if (IsFrontRow(m_SelftList[i]))
                        {
                            DoSelectedMob(m_SelftList[i]);
                            break;
                        }
                    }
                    if (GameCenter.mainFightHero.battleUnit.Pos < 20)
                    {
                        DoSelectedMob(null, 10);
                    }
                    else
                    {
                        DoSelectedMob(null, 20);
                    }
                    break;
                case 16:   //choose enemy back row, no select
                    for (int i = 0; i < m_EnemytList.Count; ++i)
                    {
                        if (!IsFrontRow(m_EnemytList[i]))
                        {
                            DoSelectedMob(m_EnemytList[i]);
                            break;
                        }
                    }
                    if (GameCenter.mainFightHero.battleUnit.Pos < 20)
                    {
                        DoSelectedMob(null, 30);
                    }
                    else
                    {
                        DoSelectedMob(null, 10);
                    }
                    break;
                case 17:   //choose self back row, no select
                    for (int i = 0; i < m_SelftList.Count; ++i)
                    {
                        if (!IsFrontRow(m_SelftList[i]))
                        {
                            DoSelectedMob(m_SelftList[i]);
                            break;
                        }
                    }
                    if (GameCenter.mainFightHero.battleUnit.Pos < 20)
                    {
                        DoSelectedMob(null, 0);
                    }
                    else
                    {
                        DoSelectedMob(null, 30);
                    }
                    break;
                case 21://choose enemy target by use weapon
                    for (int i = 0; i < m_EnemytList.Count; ++i)
                    {
                        if (CSVEquipment.Instance.GetConfData((uint)GameCenter.mainFightHero.battleUnit.WeaponId).distance == 1)//distance weapon
                        {
                            DoSelectedMob(m_EnemytList[i]);
                            break;
                        }
                        else
                        {
                            if (evt.isHero)
                            {
                                bool selfHaveFront = CheckHaveFrontMob(MobManager.Instance.GetMob(GameCenter.mainFightHero.battleUnit.UnitId));
                                if (!selfHaveFront)
                                {
                                    DoSelectedMob(m_EnemytList[i]);
                                    break;
                                }
                                else
                                {
                                    if (!CheckHaveFrontMob(m_EnemytList[i]))
                                    {
                                        DoSelectedMob(m_EnemytList[i]);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                DoSelectedMob(m_EnemytList[i]);
                                break;
                            }
                        }
                    }
                    break;
                case 22: //choose my pet ,no select
                    if (GameCenter.mainFightPet != null)
                    {
                        DoSelectedMob(MobManager.Instance.GetMob((uint)GameCenter.mainFightPet.uID));
                    }
                    break;
                default:
                    Net_Combat.Instance.eventEmitter.Trigger<ChooseEvt>(Net_Combat.EEvents.OnCloseShowOn, evt);
                    break;
            }
        }

        //选择释放对象
        public void DoSelectedMob(MobEntity mob, int pos = 0)
        {
            AudioUtil.PlayAudio(15001);

            BattleCommand command = new BattleCommand();
            command.MainCmd = (uint)currentOperationTpye;
            if (mob == null)
            {
                command.TarPos = pos;
            }
            else
            {
                command.TarPos = mob.m_MobCombatComponent.m_BattleUnit.Pos;
            }
            command.Param1 = currentOperationID;

            if (operationState == EOperationState.WaitForFirstChoose)
            {
                command.SrcUnitId = GameCenter.mainFightHero.battleUnit.UnitId;
                operationState = EOperationState.WaitForSecondOperation;
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnFirstOperationOver);
                bool arg = CombatManager.Instance.m_BattleTypeTb.show_UI_hp ? true : false;
                Net_Combat.Instance.eventEmitter.Trigger<bool>(Net_Combat.EEvents.OnCloseShowOff, arg);
            }
            else if (operationState == EOperationState.WaitForSecondChoose)
            {
                command.SrcUnitId = Sys_Fight.Instance.HasPet() ? GameCenter.mainFightPet.battleUnit.UnitId : GameCenter.mainFightHero.battleUnit.UnitId;
                operationState = EOperationState.OperationOver;
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnSecondOperationOver);
            }

            if (currentOperationTpye == EOperationType.CastSkill)
            {
                if (!Sys_Fight.Instance.HasPet())
                {
                    Net_Combat.Instance.eventEmitter.Trigger<bool>(Net_Combat.EEvents.OnDisableSkillBtn, false);
                }
                if (command.SrcUnitId == GameCenter.mainFightHero.battleUnit.UnitId)
                {
                    if (CSVActiveSkillInfo.Instance.GetConfData(command.Param1).quick_show)
                    {
                        Net_Combat.Instance._lastRoleSkillData = command.Param1;
                    }
                }
                else
                {
                    if (CSVActiveSkillInfo.Instance.GetConfData(command.Param1).quick_show)
                    {
                        Net_Combat.Instance._lastPetSkillData = command.Param1;
                    }
                }
            }
            if (currentOperationTpye == EOperationType.UseItem)
            {
                if (!Sys_Fight.Instance.HasPet())
                {
                    Net_Combat.Instance.eventEmitter.Trigger<bool>(Net_Combat.EEvents.OnDisableSkillBtn, false);
                }
                if (mob != null && mob.m_MobSelectComponent != null && mob.m_MobCombatComponent != null && mob.m_MobCombatComponent.m_BattleUnit != null)
                {
                    if (mob.m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Monster)
                    {
                        uint id = mob.m_MobCombatComponent.m_BattleUnit.UnitInfoId;
                        if (CSVMonster.Instance.TryGetValue(id, out CSVMonster.Data csvMonsterData) && csvMonsterData.target_select_type == 2)
                        {
                            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnSelected, mob.m_MobCombatComponent.m_BattleUnit.UnitId);
                        }
                        else
                        {
                            mob.m_MobSelectComponent.ShowSelected();
                        }
                    }
                    else
                    {
                        mob.m_MobSelectComponent.ShowSelected();
                    }
                }
                Sys_Bag.Instance.eventEmitter.Trigger(Sys_Bag.EEvents.OnSelectMob_UseItem);
            }
            else
            {
                var data = CSVActiveSkill.Instance.GetConfData(currentOperationID);
                if (mob != null && mob.m_MobSelectComponent != null && mob.m_MobCombatComponent != null && mob.m_MobCombatComponent.m_BattleUnit != null && data != null && data.choose_type != 7)
                {
                    if (mob.m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Monster)
                    {
                        uint id = mob.m_MobCombatComponent.m_BattleUnit.UnitInfoId;
                        if (CSVMonster.Instance.TryGetValue(id, out CSVMonster.Data csvMonsterData) && csvMonsterData.target_select_type == 2)
                        {
                            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnSelected, mob.m_MobCombatComponent.m_BattleUnit.UnitId);
                        }
                        else
                        {
                            mob.m_MobSelectComponent.ShowSelected();
                        }
                    }
                    else
                    {
                        mob.m_MobSelectComponent.ShowSelected();
                    }
                }
            }
            battleCommands.Clear();
            battleCommands.Add(command);
            Net_Combat.Instance.BattleCommandReq(CombatManager.Instance.m_BattleId, battleCommands);
            if (CombatManager.Instance.m_BattleTypeTb.show_UI_hp) //特殊战斗
            {
                fxtime = Timer.Register(0.5f, () =>
                {
                    //清除可选框
                    CheckShowSelect(false);
                    fxtime?.Cancel();
                }, null, false,true);
            }
            else
            {
                //清除可选框
                CheckShowSelect(false);
            }
            //选中以后，清除可选的mob
            m_SelectList.Clear();
        }

        //防御
        public void DoDefense()
        {
            currentOperationTpye = EOperationType.Defense;
            currentOperationID = Constants.DEFFENSEID;

            BattleCommand command = new BattleCommand();
            command.MainCmd = (uint)currentOperationTpye;
            command.Param1 = currentOperationID;

            if (operationState == EOperationState.WaitForFirstOperation)
            {
                command.SrcUnitId = GameCenter.mainFightHero.battleUnit.UnitId;
                MobEntity mob = MobManager.Instance.GetMob(GameCenter.mainFightHero.battleUnit.UnitId);
                command.TarPos = mob.m_MobCombatComponent.m_BattleUnit.Pos;
                operationState = EOperationState.WaitForSecondOperation;
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnFirstOperationOver);
            }
            else if (operationState == EOperationState.WaitForSecondOperation)
            {
                if (Sys_Fight.Instance.HasPet())
                {
                    command.SrcUnitId = GameCenter.mainFightPet.battleUnit.UnitId;
                    MobEntity mob = MobManager.Instance.GetMob(GameCenter.mainFightPet.battleUnit.UnitId);
                    command.TarPos = mob.m_MobCombatComponent.m_BattleUnit.Pos;
                }
                else
                {
                    command.SrcUnitId = GameCenter.mainFightHero.battleUnit.UnitId;
                    MobEntity mob = MobManager.Instance.GetMob(GameCenter.mainFightHero.battleUnit.UnitId);
                    command.TarPos = mob.m_MobCombatComponent.m_BattleUnit.Pos;
                }

                operationState = EOperationState.OperationOver;
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnSecondOperationOver);
            }
            battleCommands.Clear();
            battleCommands.Add(command);
            Net_Combat.Instance.BattleCommandReq(CombatManager.Instance.m_BattleId, battleCommands);
        }

        //逃跑
        public void DoEscape()
        {
            currentOperationTpye = EOperationType.Escape;
            currentOperationID = Constants.ESCAPEID;

            BattleCommand command = new BattleCommand();
            command.MainCmd = (uint)currentOperationTpye;
            command.Param1 = currentOperationID;
            command.SrcUnitId = GameCenter.mainFightHero.battleUnit.UnitId;
            MobEntity mob = MobManager.Instance.GetMob(GameCenter.mainFightHero.battleUnit.UnitId);
            command.TarPos = mob.m_MobCombatComponent.m_BattleUnit.Pos;
            battleCommands.Clear();
            battleCommands.Add(command);
            Net_Combat.Instance.BattleCommandReq(CombatManager.Instance.m_BattleId, battleCommands);

            if (operationState == EOperationState.WaitForFirstOperation)
            {
                operationState = EOperationState.WaitForSecondOperation;
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnFirstOperationOver);
            }
            else if (operationState == EOperationState.WaitForSecondOperation)
            {
                operationState = EOperationState.OperationOver;
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnSecondOperationOver);
            }
        }

        //换位
        public void DoTransposition()
        {
            currentOperationTpye = EOperationType.ChangePos;
            BattleCommand command = new BattleCommand();
            command.MainCmd = (uint)currentOperationTpye;
            command.Param1 = 0;
            command.SrcUnitId = GameCenter.mainFightHero.battleUnit.UnitId;
            MobEntity mob = MobManager.Instance.GetMob(GameCenter.mainFightHero.battleUnit.UnitId);
            command.TarPos = mob.m_MobCombatComponent.m_BattleUnit.Pos;
            battleCommands.Clear();
            battleCommands.Add(command);
            Net_Combat.Instance.BattleCommandReq(CombatManager.Instance.m_BattleId, battleCommands);

            if (operationState == EOperationState.WaitForFirstOperation)
            {
                operationState = EOperationState.WaitForSecondOperation;
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnFirstOperationOver);
            }
            else if (operationState == EOperationState.WaitForSecondOperation)
            {
                operationState = EOperationState.OperationOver;
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnSecondOperationOver);
            }
            Net_Combat.Instance.eventEmitter.Trigger<bool>(Net_Combat.EEvents.OnDisableSkillBtn, false);
        }

        //选择宠物
        public void DoSelectPet()
        {
            BattleCommand command = new BattleCommand();
            command.MainCmd = (uint)currentOperationTpye;
            command.Param1 = currentOperationID;
            command.SrcUnitId = GameCenter.mainFightHero.battleUnit.UnitId;
            MobEntity mob = MobManager.Instance.GetMob(GameCenter.mainFightHero.battleUnit.UnitId);
            command.TarPos = mob.m_MobCombatComponent.m_BattleUnit.Pos;
            battleCommands.Clear();
            battleCommands.Add(command);
            Net_Combat.Instance.BattleCommandReq(CombatManager.Instance.m_BattleId, battleCommands);

            //当前战斗宠物即将被替换 存入禁用list
            if (GameCenter.mainFightPet != null && currentOperationID != GameCenter.mainFightPet.battleUnit.PetId)
            {
                forbidpetsList.Add((uint)GameCenter.mainFightPet.battleUnit.PetId);
            }
            if (operationState == EOperationState.WaitForFirstOperation)
            {
                operationState = EOperationState.WaitForSecondOperation;
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnFirstOperationOver);
            }
            else if (operationState == EOperationState.WaitForSecondOperation)
            {
                operationState = EOperationState.OperationOver;
                Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnSecondOperationOver);
            }
            Net_Combat.Instance.eventEmitter.Trigger<bool>(Net_Combat.EEvents.OnCloseShowOff, false);
            Net_Combat.Instance.eventEmitter.Trigger<bool>(Net_Combat.EEvents.OnDisableSkillBtn, false);
        }

        public void DoCloseChooseState()
        {
            if (operationState == EOperationState.WaitForFirstChoose)
            {
                operationState = EOperationState.WaitForFirstOperation;
            }
            else if (operationState == EOperationState.WaitForSecondChoose)
            {
                operationState = EOperationState.WaitForSecondOperation;
            }

            Net_Combat.Instance.eventEmitter.Trigger<bool>(Net_Combat.EEvents.OnCloseShowOff, false);
        }

        void Clear()
        {
            currentOperationTpye = EOperationType.None;
            currentOperationID = 0;
            if (!Sys_Fight.Instance.AutoFightData.AutoState)
                CanUseSkill = true;
            battleCommands.Clear();
            curSkillId = 0;
        }

        //是否展示可选择目标脚底光圈
        public void CheckShowSelect(bool show, int choose_type = 0, List<uint> choose_req = null, bool isHero = false, bool isTeamOp=false)
        {
            m_SelectList.Clear();
            m_SelectListTeamOp.Clear();
            if (!show)
            {
                foreach (var data in MobManager.Instance.m_MobDic)
                {
                    if (null != data.Value && null != data.Value.m_MobSelectComponent)
                    {
                        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnShowOrHideSelect, data.Value.m_MobCombatComponent.m_BattleUnit.UnitId, false);
                        data.Value.m_MobSelectComponent.ShowCanSelect(false);
                    }
                }

            }
            else
            {
                if (isTeamOp)
                {
                    CalShowSelect(choose_type, choose_req, isHero, m_SelectListTeamOp);
                    InitSelectListData(m_SelectListTeamOp);
                }
                else
                {
                    CalShowSelect(choose_type, choose_req, isHero, m_SelectList);
                    InitSelectListData(m_SelectList);
                }
            }
        }

        public void InitSelectListData( List<MobEntity> list)
        {
            foreach (MobEntity data in list)
            {
                if (data.m_MobSelectComponent != null&& data.m_MobCombatComponent != null&& data.m_MobCombatComponent.m_BattleUnit!=null)
                {
                    if (data.m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Monster)
                    {
                        uint id = data.m_MobCombatComponent.m_BattleUnit.UnitInfoId;
                        if(CSVMonster.Instance.TryGetValue(id,out CSVMonster.Data csvMonsterData)&& csvMonsterData.target_select_type == 2)
                        {
                            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnShowOrHideSelect, data.m_MobCombatComponent.m_BattleUnit.UnitId, true);
                            DebugUtil.LogFormat(ELogType.eBattleCommand, "显示可选择框Pos" + data.m_MobCombatComponent.m_BattleUnit.Pos.ToString());
                        }
                        else
                        {
                            data.m_MobSelectComponent.ShowCanSelect(true);
                        }
                    }
                    else
                    {
                        data.m_MobSelectComponent.ShowCanSelect(true);
                    }
                }
            }
        }

        //可选组目标list
        public void CalShowSelect(int choose_type, List<uint> choose_req, bool isHero, List<MobEntity> listMob)
        {
            MobEntity mob = null;
            mob = isHero ? MobManager.Instance.GetMob(GameCenter.mainFightHero.battleUnit.UnitId) : MobManager.Instance.GetMob(GameCenter.mainFightPet.battleUnit.UnitId);
            CalShowSelect(choose_type, choose_req, mob, listMob, GameCenter.mainFightHero.battleUnit, isHero);
        }

        public void CalShowSelect(int choose_type, List<uint> choose_req, MobEntity mob, List<MobEntity> listMob, BattleUnit selfBattleUnit,bool isHero)
        {
            if (mob != null)
            {
                //判断自己阵营
                bool selfCamp = CombatHelp.IsSameCamp(mob.m_MobCombatComponent.m_BattleUnit, selfBattleUnit);
                switch (choose_type)
                {
                    case 0:  //选择自己
                        listMob.Add(mob);
                        break;
                    case 1: //选择活着的队友单位（包括宠物）
                        GetSameCampAlive(true, selfBattleUnit, listMob);
                        listMob.Remove(mob);
                        break;
                    case 2: //选择活着的己方全体
                        GetSameCampAlive(true, selfBattleUnit, listMob);
                        break;
                    case 3: //选择活着的敌方全体
                        //任意单位
                        if (choose_req == null || choose_req.Count == 0 || choose_req.Contains(0))
                        {
                            GetSameCampAlive(false, selfBattleUnit, listMob);
                        }
                        else
                        {
                            GetSameCampAlive(false, selfBattleUnit, listMob);
                            //不可隔两个单位
                            m_TempList01.Clear();
                            m_TempList02.Clear();
                            m_TempList03.Clear();
                            m_TempList.Clear();

                            if (choose_req.Contains(1))
                            {              
                                if (CSVEquipment.Instance.GetConfData((uint)GameCenter.mainFightHero.battleUnit.WeaponId).distance != 1|| !isHero)
                                {
                                    bool selfHaveFront = CheckHaveFrontMob(mob);
                                    if (selfHaveFront)
                                    {
                                        for (int i = 0; i < listMob.Count; ++i)
                                        {
                                            if (CheckHaveFrontMob(listMob[i]))
                                            {
                                                m_TempList01.Add(listMob[i]);
                                            }
                                        }
                                    }
                                }
                            }
                            //不可选择拥有buffID为101061的单位
                            if (choose_req.Contains(2))
                            {
                                for (int i = 0; i < listMob.Count; ++i)
                                {
                                    MobBuffComponent mobBuffComponent = listMob[i].GetNeedComponent<MobBuffComponent>();
                                    if (mobBuffComponent.HaveBuff(101061))
                                    {
                                        m_TempList02.Add(listMob[i]);
                                    }
                                }
                            }
                            if (choose_req.Contains(3))
                            {                       
                                for (int i = 0; i < listMob.Count; ++i)
                                {
                                    MobBuffComponent mobBuffComponent = listMob[i].GetNeedComponent<MobBuffComponent>();
                                    for (int j = 0; j < mobBuffComponent.m_Buffs.Count; ++j)
                                    {
                                        if (mobBuffComponent.m_Buffs[j].m_BuffTb.effect_buff == 38)
                                        {
                                            m_TempList03.Add(listMob[i]);
                                        }
                                    }
                                }
                            }
                            for (int i = 0; i < listMob.Count; ++i)
                            {
                                if (m_TempList01.Contains(listMob[i]) || m_TempList02.Contains(listMob[i]) || m_TempList03.Contains(listMob[i]))
                                {
                                    m_TempList.Add(listMob[i]);
                                }
                            }
                            for (int i = 0; i < m_TempList.Count; ++i)
                            {
                                if (listMob.Contains(m_TempList[i]))
                                {
                                    listMob.Remove(m_TempList[i]);
                                }
                            }
                        }
                        break;                                        
                    case 4: //选择活着的双方所有目标
                        foreach (var data in MobManager.Instance.m_MobDic)
                        {
                            if (data.Value.m_MobCombatComponent != null && !data.Value.m_MobCombatComponent.CheckMobCanNotFight())
                            {
                                listMob.Add(data.Value);
                            }
                        }

                        break;
                    case 5: //选择己方所有的死活单位
                        GetSameCampAll(true, selfBattleUnit, listMob);
                        break;
                    case 6: //选择敌方所有的死活单位
                        {

                            if (choose_req.Contains(1)) //不可隔两个单位
                            {
                                bool selfHaveFront = CheckHaveFrontMob(mob);
                                if (!selfHaveFront || (GameCenter.mainFightHero.battleUnit.WeaponId!=0&&CSVEquipment.Instance.GetConfData((uint)GameCenter.mainFightHero.battleUnit.WeaponId).distance == 1 && isHero))
                                {
                                    GetSameCampAll(false, selfBattleUnit, listMob);
                                }
                                else
                                {
                                    m_TempList.Clear();
                                    m_TempList = GetSameCampAll(false, selfBattleUnit, m_TempList);
                                    for (int i = 0; i < m_TempList.Count; ++i)
                                    {
                                        if (!CheckHaveFrontMob(m_TempList[i]))
                                        {
                                            listMob.Add(m_TempList[i]);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                GetSameCampAll(false, selfBattleUnit, listMob);
                            }
                            if (choose_req.Contains(2))
                            {
                                m_TempList.Clear();
                                for (int i = 0; i < listMob.Count; ++i)
                                {
                                    MobBuffComponent mobBuffComponent = listMob[i].GetNeedComponent<MobBuffComponent>();
                                    if (mobBuffComponent.HaveBuff(101061))
                                    {
                                        m_TempList.Add(listMob[i]);
                                    }
                                }
                                for (int i = 0; i < m_TempList.Count; ++i)
                                {
                                    if (listMob.Contains(m_TempList[i]))
                                    {
                                        listMob.Remove(m_TempList[i]);
                                    }
                                }
                            }
                        }
                        break;
                    case 10://选择敌方前排所有的死活单位
                        m_TempList.Clear();
                        m_TempList = GetSameCampAll(false, selfBattleUnit, m_TempList);
                        for (int i = 0; i < m_TempList.Count; ++i)
                        {
                            if (IsFrontRow(m_TempList[i]))
                            {
                                listMob.Add(m_TempList[i]);
                            }
                        }
                        break;
                    case 11://选择己方前排所有的死活单位
                        m_TempList.Clear();
                        m_TempList = GetSameCampAll(true, selfBattleUnit, m_TempList);
                        for (int i = 0; i < m_TempList.Count; ++i)
                        {
                            if (IsFrontRow(m_TempList[i]))
                            {
                                listMob.Add(m_TempList[i]);
                            }
                        }
                        break;
                    case 12://选择敌方后排所有的死活单位
                        m_TempList.Clear();
                        m_TempList = GetSameCampAll(false, selfBattleUnit, m_TempList);
                        for (int i = 0; i < m_TempList.Count; ++i)
                        {
                            if (!IsFrontRow(m_TempList[i]))
                            {
                                listMob.Add(m_TempList[i]);
                            }
                        }
                        break;
                    case 13://选择己方后排所有的死活单位
                        m_TempList.Clear();
                        m_TempList = GetSameCampAll(false, selfBattleUnit, m_TempList);
                        for (int i = 0; i < m_TempList.Count; ++i)
                        {
                            if (!IsFrontRow(m_TempList[i]))
                            {
                                listMob.Add(m_TempList[i]);
                            }
                        }
                        break;
                    case 18://选择敌方宠物
                        m_TempList.Clear();
                        m_TempList = GetSameCampAll(false, selfBattleUnit, m_TempList);
                        for (int i = 0; i < m_TempList.Count; ++i)
                        {
                            if (m_TempList[i].m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Pet)
                            {
                                listMob.Add(m_TempList[i]);
                            }
                        }
                        break;
                    case 20:  //选择可封印的宠物
                        GetCanSealPet(GetSameCampAlive(false, selfBattleUnit, listMob));
                        break;
                    case 23:  //选择除自己外的己方单位
                        GetSameCampAll(true, selfBattleUnit, listMob);
                        MobEntity selfMob = MobManager.Instance.GetMob(GameCenter.mainFightHero.battleUnit.UnitId);
                        if (listMob.Contains(selfMob))
                        {
                            listMob.Remove(selfMob);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                DebugUtil.LogErrorFormat("not found my mob");
            }

        }

        //所有己方或敌方存活单位list
        private List<MobEntity> GetSameCampAlive(bool self, BattleUnit selfBattleUnit, List<MobEntity> listMob)
        {
            if (listMob == null)
                listMob = new List<MobEntity>();

            MobEntity mob = MobManager.Instance.GetMob(selfBattleUnit.UnitId);
            if (mob == null || mob.m_MobCombatComponent == null)
            {
                bool isExist = false;
                for (int buIndex = 0, buCount = Sys_Fight.Instance.battleUnits.Count; buIndex < buCount; buIndex++)
                {
                    var bu = Sys_Fight.Instance.battleUnits[buIndex];
                    if (bu == null)
                        continue;

                    if (bu == selfBattleUnit)
                    {
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                    DebugUtil.LogError($"GetSameCampAlive方法获取{selfBattleUnit.UnitId.ToString()}的MobEntity数据不存在");
                else
                    DebugUtil.LogFormat(ELogType.eBattleCommand, $"GetSameCampAlive方法获取{selfBattleUnit.UnitId.ToString()}的MobEntity数据可能还没加载完");

                return listMob;
            }
            bool selfCamp = CombatHelp.IsSameCamp(mob.m_MobCombatComponent.m_BattleUnit, selfBattleUnit);

            foreach (var data in MobManager.Instance.m_MobDic)
            {
                if (data.Value.m_MobCombatComponent != null && !data.Value.m_MobCombatComponent.CheckMobCanNotFight())
                {
                    bool camp = CombatHelp.IsSameCamp(data.Value.m_MobCombatComponent.m_BattleUnit, selfBattleUnit);
                    if ((camp == selfCamp) == self && !listMob.Contains(data.Value))
                    {
                        listMob.Add(data.Value);
                        DebugUtil.LogFormat(ELogType.eBattleCommand, "活着的单位UnitID：" + data.Value.m_MobCombatComponent.m_BattleUnit.UnitId.ToString());
                    }
                }
            }

            return listMob;
        }

        //所有存活单位（己方和敌方）
        private List<MobEntity> GetSameCampAll(bool self, BattleUnit selfBattleUnit, List<MobEntity> listMob)
        {
            if (listMob == null)
                listMob = new List<MobEntity>();

            MobEntity mob = MobManager.Instance.GetMob(selfBattleUnit.UnitId);
            if (mob == null || mob.m_MobCombatComponent == null)
            {
                DebugUtil.LogError($"GetSameCampAlive方法获取{selfBattleUnit.UnitId.ToString()}的MobEntity数据不存在");
                return listMob;
            }
            bool selfCamp = CombatHelp.IsSameCamp(mob.m_MobCombatComponent.m_BattleUnit, selfBattleUnit);

            foreach (var data in MobManager.Instance.m_MobDic)
            {
                if (data.Value.m_MobCombatComponent != null)
                {
                    bool camp = CombatHelp.IsSameCamp(data.Value.m_MobCombatComponent.m_BattleUnit, selfBattleUnit);
                    if ((camp == selfCamp) == self)
                    {
                        listMob.Add(data.Value);
                    }
                }
            }

            return listMob;
        }

        //所有存活为英雄的单位
        private List<MobEntity> GetSameCampAliveHero(bool self)
        {
            List<MobEntity> listMob = new List<MobEntity>();

            m_TempList.Clear();
            m_TempList = GetSameCampAlive(self, GameCenter.mainFightHero.battleUnit, m_TempList);
            if (m_TempList.Count > 0)
            {
                foreach (var v in m_TempList)
                {
                    if (v.m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Hero)
                    {
                        listMob.Add(v);
                    }
                }
            }

            return listMob;
        }

        //判断前排是否有存活单位
        private bool CheckHaveFrontMob(MobEntity mob)
        {
            bool have = false;

            if (!IsFrontRow(mob))
            {
                if (mob.m_MobCombatComponent != null)
                {
                    int frontClientNum = 0;
                    if (mob.m_MobCombatComponent.m_ClientNum < 5)
                    {
                        frontClientNum = mob.m_MobCombatComponent.m_ClientNum + 5;
                    }
                    else if (mob.m_MobCombatComponent.m_ClientNum > 14)
                    {
                        frontClientNum = mob.m_MobCombatComponent.m_ClientNum - 5;
                    }

                    // 检测 frontClientNum 是否有人
                    foreach (var data in MobManager.Instance.m_MobDic)
                    {
                        if (data.Value.m_MobCombatComponent != null && !data.Value.m_MobCombatComponent.CheckMobCanNotFight())
                        {
                            if (frontClientNum == data.Value.m_MobCombatComponent.m_ClientNum)
                            {
                                have = true;
                                break;
                            }
                        }
                    }
                }
            }

            return have;
        }

        //判断所在是否为前排
        private bool IsFrontRow(MobEntity mob)
        {
            if (mob.m_MobCombatComponent != null)
            {
                // 5-14 是前排
                return mob.m_MobCombatComponent.m_ClientNum >= 5 && mob.m_MobCombatComponent.m_ClientNum <= 14;
            }

            return false;
        }

        //是否多人战斗
        public bool IsManyPeopleFight()
        {
            int hero = 0;
            foreach (var data in MobManager.Instance.m_MobDic)
            {
                if (data.Value.m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Hero)
                {
                    if (++hero > 1)
                    {
                        return true;
                    }

                }
            }
            return false;
        }

        //是否需要展示等待其他人行动
        public bool isWaitOtherOp(uint id)
        {
            if (IsManyPeopleFight())
            {
                if (hasCommends.Count == 0)
                {
                    return GetSameCampAliveHero(true).Count != 1;
                }
                else
                {
                    return hasCommends.Contains(id);
                }
            }
            else
            {
                return false;
            }
        }

        //是否为己方最后一次行动结束
        public bool isLastOpInFight(uint id)
        {
            if (IsManyPeopleFight())
            {
                if (hasCommends.Count == 0)
                {
                    return GetSameCampAliveHero(true).Count != 1;
                }
                else
                {
                    return hasCommends.Contains(id);
                }
            }
            else
            {
                return false;
            }
        }

        public static BattleCommand GetBattleCommand(int battleCommandType, uint srcUnitId, int tarServerPos, uint skillId)
        {
            BattleCommand command = null;
            switch (battleCommandType)
            {
                case 1:
                    command = new BattleCommand();
                    command.MainCmd = (uint)EOperationType.CastSkill;
                    command.Param1 = Constants.DEFFENSEID;
                    break;

                case 4:
                    command = new BattleCommand();
                    command.MainCmd = skillId == Constants.NEARNORMALATTACKID ? (uint)EOperationType.NormalAttack : (uint)EOperationType.CastSkill;
                    command.Param1 = skillId;
                    break;
            }

            if (command != null)
            {
                command.SrcUnitId = srcUnitId;
                command.TarPos = tarServerPos;
            }
            return command;
        }

        //可封印宠物单位
        private List<MobEntity> GetCanSealPet(List<MobEntity> list)
        {
            m_TempList.Clear();
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].m_MobCombatComponent != null && list[i].m_MobCombatComponent.m_BattleUnit != null)
                {
                    var monsterInfo = CSVMonster.Instance.GetConfData(list[i].m_MobCombatComponent.m_BattleUnit.UnitInfoId);
                    if (!(monsterInfo != null && monsterInfo.unlock_pet != 0u && !list[i].m_MobCombatComponent.CheckMobCanNotFight()))
                    {
                        m_TempList.Add(list[i]);
                    }
                }
                else
                {
                    DebugUtil.LogError("GetCanSealPet: MobEntity.m_MobCombatComponent is null or it's battleUnit is null");
                }
            }

            for (int i = 0; i < m_TempList.Count; ++i)
            {
                list.Remove(m_TempList[i]);
            }

            return list;
        }

        public uint HaveSealedPet()
        {
            if (GameCenter.mainFightHero == null || GameCenter.mainFightHero.battleUnit == null)
                return 0;

            GetSameCampAlive(false, GameCenter.mainFightHero.battleUnit, m_SelectList);
            GetCanSealPet(m_SelectList);

            if (m_SelectList.Count == 0)
            {
                return 0;
            }
            else
            {
                uint petId = CSVMonster.Instance.GetConfData(m_SelectList[0].m_MobCombatComponent.m_BattleUnit.UnitInfoId).unlock_pet;
                return petId;
            }
        }

        //判断己方或者敌方是否有前排
        public bool CheckSameCampHaveFrontMob(bool self)
        {
            if (self)
            {
                foreach (var data in MobManager.Instance.m_MobDic)
                {
                    if (data.Value.m_MobCombatComponent != null && !data.Value.m_MobCombatComponent.CheckMobCanNotFight())
                    {
                        if (data.Value.m_MobCombatComponent.m_ClientNum >= 5 && data.Value.m_MobCombatComponent.m_ClientNum <= 9)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {
                foreach (var data in MobManager.Instance.m_MobDic)
                {
                    if (data.Value.m_MobCombatComponent != null && !data.Value.m_MobCombatComponent.CheckMobCanNotFight())
                    {
                        if (data.Value.m_MobCombatComponent.m_ClientNum >= 10 && data.Value.m_MobCombatComponent.m_ClientNum <= 14)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        //判断己方或者敌方是否有后排
        public bool CheckSameCampHaveBackMob(bool self)
        {
            if (self)
            {
                foreach (var data in MobManager.Instance.m_MobDic)
                {
                    if (data.Value.m_MobCombatComponent != null && !data.Value.m_MobCombatComponent.CheckMobCanNotFight())
                    {
                        if (data.Value.m_MobCombatComponent.m_ClientNum >= 0 && data.Value.m_MobCombatComponent.m_ClientNum <= 4)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {
                foreach (var data in MobManager.Instance.m_MobDic)
                {
                    if (data.Value.m_MobCombatComponent != null && !data.Value.m_MobCombatComponent.CheckMobCanNotFight())
                    {
                        if (data.Value.m_MobCombatComponent.m_ClientNum >= 15 && data.Value.m_MobCombatComponent.m_ClientNum <= 19)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        //判断己方或者敌方是否有宠物单位
        public bool CheckHasPetMob(bool self)
        {
            if (self)
            {
                m_TempList.Clear();
                m_TempList = GetSameCampAll(true, GameCenter.mainFightHero.battleUnit, m_TempList);
                for (int i = 0; i < m_TempList.Count; ++i)
                {
                    if (m_TempList[i].m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Pet)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                m_TempList.Clear();
                m_TempList = GetSameCampAll(false, GameCenter.mainFightHero.battleUnit, m_TempList);
                for (int i = 0; i < m_TempList.Count; ++i)
                {
                    if (m_TempList[i].m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Pet)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        //判断己方或者敌方是否有英雄单位
        public bool CheckHasHeroMob(bool self)
        {
            if (self)
            {
                m_TempList.Clear();
                m_TempList = GetSameCampAll(true, GameCenter.mainFightHero.battleUnit, m_TempList);
                for (int i = 0; i < m_TempList.Count; ++i)
                {
                    if (m_TempList[i].m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Hero)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                m_TempList.Clear();
                m_TempList = GetSameCampAll(false, GameCenter.mainFightHero.battleUnit, m_TempList);
                for (int i = 0; i < m_TempList.Count; ++i)
                {
                    if (m_TempList[i].m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Hero)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        //选择非释放对象 弹对应提示
        public void DoSelectedForbitMob(MobEntity mob)
        {
            CSVActiveSkill.Data data = CSVActiveSkill.Instance.GetConfData(curSkillId);
            if (m_SelectList.Contains(mob) || data == null || data.choose_wrong_target==null)
            {
                return;
            }
            List<uint> list = new List<uint>();
            if (data.choose_wrong_target.Contains(101) && !CombatHelp.IsSameCamp(mob.m_MobCombatComponent.m_BattleUnit, GameCenter.mainFightHero.battleUnit))
            {
                if (CheckSameCampHaveFrontMob(true))
                {
                    if (!IsFrontRow(mob))
                    {
                        list.Add(101);
                    }
                }
            }
            MobBuffComponent mobBuffComponent = mob.GetNeedComponent<MobBuffComponent>();
            if (data.choose_wrong_target.Contains(201) && mobBuffComponent.HaveBuff(CSVChoseTargetLimit.Instance.GetConfData(201).value))
            {
                list.Add(201);
            }
            if (data.choose_wrong_target.Contains(301) && HasSameTypeBuff(mobBuffComponent,38))
            {
                list.Add(301);
            }
            if((data.choose_type == 10 || data.choose_type == 11) && data.choose_wrong_target.Contains(401))
            {
                list.Add(401);
            }
           if ((data.choose_type == 12 || data.choose_type == 13) && data.choose_wrong_target.Contains(501))
            {
                list.Add(501);
            }
            if (data.choose_wrong_target.Contains(601))
            {
                list.Add(601);
            }
            if ((data.choose_type == 18) && data.choose_wrong_target.Contains(701))
            {
                list.Add(701);
            }
            if (data.choose_wrong_target.Contains(801))
            {
                list.Add(801);
            }
            if (data.choose_wrong_target.Contains(901))
            {
                list.Add(901);
            }
            if (list.Count == 0)
            {
                return;
            }
            list.Sort((dataA, dataB) =>
            {
                if (CSVChoseTargetLimit.Instance.GetConfData(dataA).priority > CSVChoseTargetLimit.Instance.GetConfData(dataB).priority)
                    return -1;
                else if (CSVChoseTargetLimit.Instance.GetConfData(dataA).priority < CSVChoseTargetLimit.Instance.GetConfData(dataB).priority)
                    return 1;
                else
                {
                    if (dataA > dataB)
                        return -1;
                    else
                        return 1;
                }
            });
          Sys_Hint.Instance.PushContent_Normal( LanguageHelper.GetTextContent(CSVChoseTargetLimit.Instance.GetConfData(list[0]).tips));
          AudioUtil.PlayAudio(CSVChoseTargetLimit.Instance.GetConfData(list[0]).audio);
        }

        private bool HasSameTypeBuff(MobBuffComponent mobBuffComponent,uint buffType)
        {
            for (int i = 0; i < mobBuffComponent.m_Buffs.Count; ++i)
            {
                if (mobBuffComponent.m_Buffs[i].m_BuffTb.effect_buff == buffType)
                {
                    return true;
                }
            }
            return false;
        }

        private uint GetPetMaxFightTime()
        {
            CSVCharacterAttribute.Data data = CSVCharacterAttribute.Instance.GetConfData(Sys_Role.Instance.Role.Level);
            if (data == null)
            {
                return 0;
            }
            else
            {
                if (Sys_Pet.Instance.fightPet.HasFightPet())
                    return data.pet_summon_num - (uint)1;
                else
                    return data.pet_summon_num ;
            }
        }

        public List<CSVChoseSkillLimit.Data>  CheckSkillLimitList(uint skillId,bool isAvailable,bool isPetSkill, List<MobBuffComponent.BuffData> buffs)
        {
            CSVActiveSkill.Data csvActiveSkillData = CSVActiveSkill.Instance.GetConfData(skillId);
            List<CSVChoseSkillLimit.Data>  lists = new List<CSVChoseSkillLimit.Data>();
            if (csvActiveSkillData==null || csvActiveSkillData.choose_skill_condition == null)
                return lists;
            if (csvActiveSkillData.choose_skill_condition.Contains(101) && !isAvailable)
            {
                lists.Add(CSVChoseSkillLimit.Instance.GetConfData(101));
            }
            if (csvActiveSkillData.choose_skill_condition.Contains(201) )
            {
                if (isPetSkill)
                {
                    if (GameCenter.mainFightPet.battleUnit.CurMp < csvActiveSkillData.mana_cost)
                    {
                        lists.Add(CSVChoseSkillLimit.Instance.GetConfData(201));
                    }
                }
                else
                {
                    if (GameCenter.mainFightHero.battleUnit.CurMp < csvActiveSkillData.mana_cost)
                    {
                        lists.Add(CSVChoseSkillLimit.Instance.GetConfData(201));
                    }
                }
            }
            if (csvActiveSkillData.choose_skill_condition.Contains(301) && Net_Combat.Instance.m_EnergePoint < csvActiveSkillData.energy_cost)
            {
                lists.Add(CSVChoseSkillLimit.Instance.GetConfData(301));
            }
            if (csvActiveSkillData.choose_skill_condition.Contains(401) && !CanUseByLimitBuff(csvActiveSkillData,buffs))
            {
                lists.Add(CSVChoseSkillLimit.Instance.GetConfData(401));
            }
            if (csvActiveSkillData.choose_skill_condition.Contains(501) && !CanUseByNeedBuff(csvActiveSkillData, buffs))
            {
                lists.Add(CSVChoseSkillLimit.Instance.GetConfData(501));
            }
           SkillColdDwonInfo  cdInfo = GameCenter.fightControl.HaveSkillCold(csvActiveSkillData.id);
            if (csvActiveSkillData.choose_skill_condition.Contains(601) && csvActiveSkillData.cold_time != 0 && cdInfo != null)
            {
                lists.Add(CSVChoseSkillLimit.Instance.GetConfData(601));
            }
            if (csvActiveSkillData.choose_skill_condition.Contains(701) )
            {
                if (isPetSkill)
                {
                    ClientPet clientPet = Sys_Pet.Instance.GetFightPetClient((uint)GameCenter.mainFightPet.battleUnit.PetId);
                    if (clientPet != null && (!clientPet.pkAttrs.ContainsKey(27) || clientPet.pkAttrs[27] < csvActiveSkillData.min_spirit))
                    {
                        lists.Add(CSVChoseSkillLimit.Instance.GetConfData(701));
                    }
                }
                else
                {
                    if (Sys_Attr.Instance.pkAttrs[27] < csvActiveSkillData.min_spirit)
                    {
                        lists.Add(CSVChoseSkillLimit.Instance.GetConfData(701));
                    }
                }           
            }
            if (csvActiveSkillData.choose_skill_condition.Contains(801) && !GameCenter.fightControl.CheckSameCampHaveFrontMob(false))
            {
                lists.Add(CSVChoseSkillLimit.Instance.GetConfData(801));
            }
            if (csvActiveSkillData.choose_skill_condition.Contains(901) && !GameCenter.fightControl.CheckSameCampHaveBackMob(false))
            {
                lists.Add(CSVChoseSkillLimit.Instance.GetConfData(901));
            }
            if (csvActiveSkillData.choose_skill_condition.Contains(1001) && !GameCenter.fightControl.CheckHasHeroMob(false))
            {
                lists.Add(CSVChoseSkillLimit.Instance.GetConfData(1001));
            }
            if (csvActiveSkillData.choose_skill_condition.Contains(1101) && !GameCenter.fightControl.CheckHasPetMob(false))
            {
                lists.Add(CSVChoseSkillLimit.Instance.GetConfData(1101));
            }
            if (csvActiveSkillData.choose_skill_condition.Contains(1201) && !GameCenter.fightControl.CheckHasHeroMob(true))
            {
                lists.Add(CSVChoseSkillLimit.Instance.GetConfData(1201));
            }

            return lists;
        }

        public uint CheckSkillLimitId(uint skillId, bool isAvailable, bool isPetSkill, List<MobBuffComponent.BuffData> buffs)
        {
            List<CSVChoseSkillLimit.Data>  list = new List<CSVChoseSkillLimit.Data>();
            list= CheckSkillLimitList(skillId, isAvailable, isPetSkill, buffs);
            if (list.Count == 0)
            {
                return 0;
            }
            else
            {
                list.Sort((dataA, dataB) =>
                {
                    if (dataA.priority > dataB.priority)
                        return -1;
                    else if (dataA.priority < dataB.priority)
                        return 1;
                    else
                    {
                        if (dataA.id > dataB.id)
                            return -1;
                        else
                            return 1;
                    }
                });
                return list[0].id;
            }
        }

        private bool CanUseByNeedBuff(CSVActiveSkill.Data csvActiveSkillData, List<MobBuffComponent.BuffData> buffs)
        {
            if (csvActiveSkillData.buff_condition == 0)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < buffs.Count; ++i)
                {
                    if (buffs[i].m_BuffTb.buff_effective==(int)csvActiveSkillData.buff_condition)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private bool CanUseByLimitBuff(CSVActiveSkill.Data csvActiveSkillData, List<MobBuffComponent.BuffData> buffs)
        {
            for (int i = 0; i < buffs.Count; ++i)
            {
                if (buffs[i].m_BuffTb.buff_effective==(int)csvActiveSkillData.buff_limit_condition)
                {
                    return false;
                }
            }
            return true;
        }

        public List<uint> GetFightPetSkills(uint petUid)
        {
            List<uint> skillList = new List<uint>();
            BaseSkill baseSkill = new BaseSkill();
            for (int i = 0; i < Net_Combat.Instance.petsInBattle.Count; ++i)
            {
                if (Net_Combat.Instance.petsInBattle[i].Pet.Uid == petUid)
                {
                    PetUnit petUnit = Net_Combat.Instance.petsInBattle[i].Pet;
                    if (null == petUnit || null == petUnit.BaseSkillInfo)
                        return skillList;
                    baseSkill = petUnit.BaseSkillInfo;
                    break;
                }
            }
            for (int i = 0; i < baseSkill.Skills.Count; ++i)
            {
                if (CSVActiveSkill.Instance.TryGetValue(baseSkill.Skills[i], out CSVActiveSkill.Data data))
                {
                    skillList.Add(baseSkill.Skills[i]);
                }
            }
            for (int i = 0; i < baseSkill.UniqueSkills.Count; ++i)
            {
                if (CSVActiveSkill.Instance.TryGetValue(baseSkill.UniqueSkills[i], out CSVActiveSkill.Data data))
                {
                    skillList.Add(baseSkill.UniqueSkills[i]);
                }
            }
            for (int i = 0; i < baseSkill.RidingSkills.Count; ++i)
            {
                if (CSVActiveSkill.Instance.TryGetValue(baseSkill.RidingSkills[i], out CSVActiveSkill.Data data))
                {
                    skillList.Add(baseSkill.RidingSkills[i]);
                }
            }
            for (int i = 0; i < baseSkill.UniqueRidingSkills.Count; ++i)
            {
                if (CSVActiveSkill.Instance.TryGetValue(baseSkill.UniqueRidingSkills[i], out CSVActiveSkill.Data data))
                {
                    skillList.Add(baseSkill.UniqueRidingSkills[i]);
                }
            }
            return skillList;
        }

        public void DoubleClickedOp(MobEntity mob)
        {
            MobCombatComponent mobCombatComponent = mob.m_MobCombatComponent;
            BattleUnit battleUnit = mob.m_MobCombatComponent.m_BattleUnit;
            bool selfCamp = CombatHelp.IsSameCamp(battleUnit, GameCenter.mainFightHero.battleUnit);
            if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForFirstOperation || (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation && !Sys_Fight.Instance.HasPet()))
            {
                if (CSVActiveSkillInfo.Instance.TryGetValue(Net_Combat.Instance._lastRoleSkillData, out CSVActiveSkillInfo.Data csvData) && GameCenter.fightControl.CanUseSkill)
                {
                    if (selfCamp && csvData.skill_category == 2)  //治疗
                    {
                        DoubleClickedCheckOp(FightControl.EOperationType.CastSkill, Net_Combat.Instance._lastRoleSkillData);
                        GameCenter.fightControl.DoSelectedMob(mob);
                    }
                    else if ((csvData.skill_category == 1 || csvData.skill_category == 3) && !selfCamp)
                    {
                        DoubleClickedCheckOp(FightControl.EOperationType.CastSkill, Net_Combat.Instance._lastRoleSkillData);
                        GameCenter.fightControl.DoSelectedMob(mob);
                    }
                }
                else
                {
                    if (!selfCamp)
                    {
                        DoubleClickedCheckOp(FightControl.EOperationType.NormalAttack, Constants.NEARNORMALATTACKID);
                        GameCenter.fightControl.DoSelectedMob(mob);
                    }
                }
            }
            else if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation && Sys_Fight.Instance.HasPet())
            {
                if (CSVActiveSkillInfo.Instance.TryGetValue(Net_Combat.Instance._lastPetSkillData, out CSVActiveSkillInfo.Data csvData) && GameCenter.mainFightPet.battleUnit.CurHp > 0)
                {
                    if (selfCamp && csvData.skill_category == 2)  //治疗
                    {
                        DoubleClickedCheckOp(FightControl.EOperationType.CastSkill, Net_Combat.Instance._lastPetSkillData);
                        GameCenter.fightControl.DoSelectedMob(mob);
                    }
                    else if ((csvData.skill_category == 1 || csvData.skill_category == 3) && !selfCamp)
                    {
                        DoubleClickedCheckOp(FightControl.EOperationType.CastSkill, Net_Combat.Instance._lastPetSkillData);
                        GameCenter.fightControl.DoSelectedMob(mob);
                    }
                }
                else
                {
                    if (!selfCamp)
                    {
                        DoubleClickedCheckOp(FightControl.EOperationType.NormalAttack, Constants.NEARNORMALATTACKID);
                        GameCenter.fightControl.DoSelectedMob(mob);
                    }
                }
            }
        }

        public void DoubleClickedCheckOp(EOperationType type, uint id)
        {
            GameCenter.fightControl.currentOperationTpye = type;
            GameCenter.fightControl.currentOperationID = id;
            if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForFirstOperation)
            {
                GameCenter.fightControl.operationState = FightControl.EOperationState.WaitForFirstChoose;
            }
            else if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation)
            {
                GameCenter.fightControl.operationState = FightControl.EOperationState.WaitForSecondChoose;
            }
        }

        #region   TeamCommand
        public bool isOwnSideActor(FightActor value)
        {
            var own = GameCenter.mainFightHero;
            if (value.battleUnit.Side != own.battleUnit.Side)
                return false;
            return true;
        }

        public bool isOwnSideActor(MonsterPart value)
        {
            var own = GameCenter.mainFightHero;
            if (value.battleUnit.Side != own.battleUnit.Side)
                return false;
            return true;
        }

        public bool IsSelectActorRight(uint battleid)
        {
            if (GameCenter.fightControl == null)
                return false;
            int count = GameCenter.fightControl.m_SelectListTeamOp.Count;
            for (int i = 0; i < count; i++)
            {
                var value = GameCenter.fightControl.m_SelectListTeamOp[i];
                if (value.m_MobCombatComponent == null)
                    continue;
                var battleunit = value.m_MobCombatComponent.m_BattleUnit;
                if (battleunit == null)
                    continue;
                if (battleunit.UnitId != battleid)
                    continue;
                return true;
            }
            return false;
        }

        public uint getUnitID(FightActor value)
        {
            return value.battleUnit.UnitId;
        }

        public uint getUnitID(MonsterPart value)
        {
            return value.battleUnit.UnitId;
        }
        #endregion

        //教学观战斗内携带道具数据
        public List<ItemData> GetTeachingItemData(uint itemType)
        {
            List<ItemData> list = new List<ItemData>();
            for (int i=0; i< GameCenter.mainFightHero.battleUnit.Iteminfo.Count; ++i)
            {
                ItemInfo info = GameCenter.mainFightHero.battleUnit.Iteminfo[i];
                if (CSVItem.Instance.TryGetValue(info.Itemid,out  CSVItem.Data data))
                {
                    if (itemType == 1)
                    {
                        List<uint> itemTbs = CombatManager.Instance.m_BattleTypeTb.normal_medic;
                        InitItemDataByType(data.type_id, itemTbs, info, list);
                    }
                    else  if (itemType == 2)
                    {
                        List<uint> itemTbs = CombatManager.Instance.m_BattleTypeTb.special_medic;
                        InitItemDataByType(data.type_id, itemTbs, info, list);
                    }
                }
            }
            return list;
        }

        private void InitItemDataByType(uint typeID, List<uint> itemTbs, ItemInfo info, List<ItemData> list)
        {
            for (int j = 0; j < itemTbs.Count; ++j)
            {
                if (itemTbs[j] == typeID)
                {
                    ItemData itemData = new ItemData();
                    itemData.SetData(0, 0, info.Itemid, info.Count, 0, false, false, null, null, 0);
                    list.Add(itemData);
                }
            }
        }
    }
}
