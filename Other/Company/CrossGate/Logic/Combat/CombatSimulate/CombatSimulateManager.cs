#if UNITY_EDITOR_NO_USE
using Google.Protobuf.Collections;
using Lib.Core;
using Logic;
using Net;
using Packet;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

public class CombatSimulateUnitInfo
{
    public int ServerNum;
    public CSVTestRole.Data TestRoleDataTb;
    public CSVTestPet.Data TestPetDataTb;
}

public class CombatSimulateStatisticsInfo
{
    public long Dmg;
    public int KillNum;
    public long AddHp;
    public long BeDmg;
    public int BeControlledNum;
}

public class CombatSimulateManager : Logic.Singleton<CombatSimulateManager>
{
    public int m_CombatUnitSum;

    public int m_SimulateType;
    public bool m_IsAlready;
    public bool m_IsSendEnterBattle;
    public bool m_IsSendSimulationCommand;

    private Dictionary<int, CombatSimulateUnitInfo> _combatSimulateUnitInfoDic = new Dictionary<int, CombatSimulateUnitInfo>();

    private Dictionary<int, CombatSimulateStatisticsInfo> _CombatSimulateStatisticsInfoDic = new Dictionary<int, CombatSimulateStatisticsInfo>();

    private RepeatedField<SimulationUnit> _simulationUnits = new RepeatedField<SimulationUnit>();

    public void OnAwake()
    {
#if UNITY_EDITOR
        EventDispatcher.Instance.AddEventListener((ushort)CmdBattle.SimulationReq, (ushort)CmdBattle.SimulationRes, OnBattleSimulationRes, CmdBattleSimulationRes.Parser);
        EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattle.SimulationNtf, OnSimulationNtf, CmdBattleSimulationNtf.Parser);
        EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattle.SimulationResultNtf, OnSimulationResultNtf, CmdBattleSimulationResultNtf.Parser);
#endif
    }

    public void OnDestroy()
    {
#if UNITY_EDITOR
        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.SimulationRes, OnBattleSimulationRes);
        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.SimulationNtf, OnSimulationNtf);
        EventDispatcher.Instance.RemoveEventListener((ushort)CmdBattle.SimulationResultNtf, OnSimulationResultNtf);
#endif
    }

    public void Init()
    {
        m_IsAlready = false;
        m_IsSendEnterBattle = false;
        m_IsSendSimulationCommand = false;
    }

#region Net
    public void SendEnterSimulateCombat(int simulateType, string idStr)
    {
        m_SimulateType = simulateType;
        
        EventDispatcher.Instance.AddEventListener((ushort)CmdGm.Req, (ushort)CmdGm.Res, OnReceivedGM, CmdGmRes.Parser);

        CmdGmReq req = new CmdGmReq();
        req.Cmd = Google.Protobuf.ByteString.CopyFrom("si_fight", System.Text.Encoding.UTF8);
        req.Param = Google.Protobuf.ByteString.CopyFrom($"{simulateType.ToString()} {idStr}", System.Text.Encoding.UTF8);

        NetClient.Instance.SendMessage((ushort)CmdGm.Req, req);
    }

    private void OnReceivedGM(NetMsg msg)
    {
        CmdGmRes res = NetMsgUtil.Deserialize<CmdGmRes>(CmdGmRes.Parser, msg);
        string retStr = "";
        if (res.Ret == 0)
        {
            retStr = res.Result.ToStringUtf8();
        }
        else
        {
            retStr = res.Ret.ToString();
        }

        CombatManager.Instance.m_BattleId = uint.Parse(retStr);
        m_IsAlready = true;

        EventDispatcher.Instance.RemoveEventListener((ushort)CmdGm.Res, OnReceivedGM);

        if (m_SimulateType != 2)
            SendBattleSimulationReq(CombatManager.Instance.m_BattleId, m_SimulateType);
    }

    public void SendBattleSimulationReq(uint battleId, int simulateType)
    {
        CmdBattleSimulationReq req = new CmdBattleSimulationReq();
        req.Unit.AddRange(_simulationUnits);
        req.BattleId = battleId;

        m_SimulateType = simulateType;

        m_IsSendEnterBattle = true;

        if (!m_IsAlready)
            CombatManager.Instance.m_BattleId = battleId;

        foreach (var item in _simulationUnits)
        {
            Debug.Log($"CmdBattleSimulationReq---u_type:{item.UType.ToString()}----id:{item.Id.ToString()}-------Pos:{item.Pos.ToString()}");
        }

        NetClient.Instance.SendMessage((ushort)CmdBattle.SimulationReq, req);
    }

    private void OnBattleSimulationRes(NetMsg msg)
    {
        //CmdBattleSimulationRes res = NetMsgUtil.Deserialize<CmdBattleSimulationRes>(CmdBattleSimulationRes.Parser, msg);

    }

    private void OnSimulationNtf(NetMsg msg)
    {
        CmdBattleSimulationNtf ntf = NetMsgUtil.Deserialize<CmdBattleSimulationNtf>(CmdBattleSimulationNtf.Parser, msg);

        m_IsAlready = false;
        m_IsSendEnterBattle = false;

        m_CombatUnitSum = ntf.Units.Count;
        ClearData();

        for (int i = 0, count = ntf.Units.Count; i < count; i++)
        {
            ResetSimulateData(ntf.Units[i]);
        }

        Sys_Fight.Instance.OnSimulateStart(ntf.Units);
    }

    public void SendCmdBattleSimulationCommandReq(RepeatedField<BattleCommand> battleCommands)
    {
        m_IsSendSimulationCommand = true;

        CmdBattleSimulationCommandReq req = new CmdBattleSimulationCommandReq();
        req.BattleId = CombatManager.Instance.m_BattleId;
        req.Cmd.AddRange(battleCommands);

        NetClient.Instance.SendMessage((ushort)CmdBattle.SimulationCommandReq, req);
    }

    private void OnSimulationResultNtf(NetMsg msg)
    {
        m_IsSendSimulationCommand = false;

        CmdBattleSimulationResultNtf ntf = NetMsgUtil.Deserialize<CmdBattleSimulationResultNtf>(CmdBattleSimulationResultNtf.Parser, msg);
        
        Net_Combat.Instance.DoRound(ntf.Ntf);
    }
#endregion

#region Logic
    public void InitEditor()
    {
        ClearData();
    }

    public void ClearData()
    {
        _simulationUnits.Clear();
        _CombatSimulateStatisticsInfoDic.Clear();
    }

    public void AddSimulateUnit(uint unitType, uint infoId, uint serverPos)
    {
        for (int i = 0, count = _simulationUnits.Count; i < count; i++)
        {
            SimulationUnit simulationUnit = _simulationUnits[i];
            if (simulationUnit == null)
                continue;

            if (simulationUnit.Pos == serverPos)
            {
                simulationUnit.Id = infoId;
                simulationUnit.UType = unitType;
                return;
            }
        }

        SimulationUnit su = new SimulationUnit();
        su.Pos = serverPos;
        su.Id = infoId;
        su.UType = unitType;

        _simulationUnits.Add(su);
    }

    public CombatSimulateUnitInfo GetSimulateUnitInfo(int serverNum)
    {
        _combatSimulateUnitInfoDic.TryGetValue(serverNum, out CombatSimulateUnitInfo combatSimulateUnitInfo);
        return combatSimulateUnitInfo;
    }

    public CombatSimulateStatisticsInfo GetCombatSimulateStatisticsInfo(int serverNum)
    {
        if (!_CombatSimulateStatisticsInfoDic.TryGetValue(serverNum, out CombatSimulateStatisticsInfo combatSimulateStatisticsInfo) || combatSimulateStatisticsInfo == null)
        {
            combatSimulateStatisticsInfo = new CombatSimulateStatisticsInfo();
            _CombatSimulateStatisticsInfoDic[serverNum] = combatSimulateStatisticsInfo;
        }

        return combatSimulateStatisticsInfo;
    }

    public void RefreshUnitDataReq()
    {
        foreach (var kv in MobManager.Instance.m_MobDic)
        {
            if (kv.Value == null || kv.Value.m_MobCombatComponent == null || kv.Value.m_MobCombatComponent.m_BattleUnit == null)
                continue;

            Net_Combat.Instance.SendBattleUnitInfoReq(kv.Value.m_MobCombatComponent.m_BattleUnit.UnitId);
        }
    }

    public void ResetSimulateData(BattleUnit battleUnit)
    {
        AddSimulateUnit(battleUnit.UnitType, (uint)battleUnit.SinfoId, (uint)battleUnit.Pos);

        if (!_combatSimulateUnitInfoDic.TryGetValue(battleUnit.Pos, out CombatSimulateUnitInfo combatSimulateUnitInfo) || combatSimulateUnitInfo == null)
        {
            combatSimulateUnitInfo = new CombatSimulateUnitInfo();
            _combatSimulateUnitInfoDic[battleUnit.Pos] = combatSimulateUnitInfo;
        }
        combatSimulateUnitInfo.ServerNum = battleUnit.Pos;
        if (battleUnit.UnitType == (uint)UnitType.Hero || battleUnit.UnitType == (uint)UnitType.Partner)
        {
            combatSimulateUnitInfo.TestRoleDataTb = CSVTestRole.Instance.GetConfData((uint)battleUnit.SinfoId);
        }
        else if (battleUnit.UnitType == (uint)UnitType.Monster)
        {
            combatSimulateUnitInfo.TestPetDataTb = CSVTestPet.Instance.GetConfData((uint)battleUnit.SinfoId);
        }
    }
#endregion
}

namespace Logic
{
    public partial class Sys_Fight
    {
        public override void Dispose()
        {
            AddressablesUtil.ReleaseInstance(ref mHandle, loadCallback);
        }

        void CreateSimulateFightHeros()
        {
            foreach (var unit in battleUnits)
            {
                if (unit.UnitType == (uint)EFightActorType.Hero || unit.UnitType == (uint)EFightActorType.Partner)
                {
                    CreateSimulateFightHero(unit);
                }
            }
        }

        void CreateSimulateFightPets()
        {
            foreach (var unit in battleUnits)
            {
                if (unit.UnitType == (uint)EFightActorType.Pet)
                {
                    CreateFightPet(unit);
                }
            }
        }

        public AsyncOperationHandle<GameObject> mHandle;
        private Action<AsyncOperationHandle<GameObject>> loadCallback;

        public void CreateSimulateFightHero(BattleUnit battleUnit, bool isCalling = false)
        {
            if (isCalling)
            {
                //var actor = GameCenter.fightWorld.GetActor(typeof(FightHero), battleUnit.UnitId);
                //if (actor != null)
                //{
                //    actor.Dispose();
                //}
                //GameCenter.fightWorld.DestroyActor(typeof(FightHero), battleUnit.UnitId);

                RemoveFightHeroByUnitId(battleUnit.UnitId);
            }

            CSVCharacter.Data cSVCharacterData = CSVCharacter.Instance.GetConfData(battleUnit.UnitInfoId);
            if (cSVCharacterData == null)
            {
                Debug.LogError($"cSVCharacterData is null id:{battleUnit.UnitInfoId.ToString()}");
                return;
            }

            CombatSimulateUnitInfo combatSimulateUnitInfo = CombatSimulateManager.Instance.GetSimulateUnitInfo(battleUnit.Pos);
            if (combatSimulateUnitInfo == null)
            {
                Debug.LogError($"CombatSimulateManager is null pos:{battleUnit.Pos.ToString()}");
                return;
            }
            if (combatSimulateUnitInfo.TestRoleDataTb == null)
            {
                Debug.LogError($"combatSimulateUnitInfo.TestRoleDataTb is null pos:{battleUnit.Pos.ToString()}");
                return;
            }

            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(combatSimulateUnitInfo.TestRoleDataTb.job_id);
            uint curWeaponId = cSVCareerData == null ? Constants.UMARMEDID : cSVCareerData.weapon;

            //先卸载上一次加载的资源 并且注销上一个临时回调.
            //这是在UNITY_EDITOR模拟，不要动
            //AddressablesUtil.ReleaseInstance(ref mHandle, loadCallback);
            //一个新临时回调
            loadCallback = (AsyncOperationHandle<GameObject> handle) =>
            {
                //使用完成后将临时回调置空
                loadCallback = null;

                GameObject modelGameObject = handle.Result;

                //FightHero fightHero = GameCenter.fightWorld.CreateActor<FightHero>(battleUnit.UnitId);
                FightHero fightHero = World.AllocActor<FightHero>((uint)battleUnit.UnitId);
                fightHero.SetName($"FightHero_{battleUnit.Pos.ToString()}");
                fightHero.SetParent(GameCenter.fightActorRoot);
                FightHeroDic.Add(battleUnit.UnitId, fightHero);

                fightHero.battleUnit = battleUnit;

                if (battleUnit.RoleId == Sys_Role.Instance.Role.RoleId)
                {
                    GameCenter.mainFightHero = fightHero;
                }

                //fightHero.careerComponent = World.AddComponent<CareerComponent>(fightHero);
                fightHero.careerComponent.UpdateCareerType((ECareerType)combatSimulateUnitInfo.TestRoleDataTb.job_id);

                //fightHero.weaponComponent = World.AddComponent<WeaponComponent>(fightHero);
                fightHero.weaponComponent.UpdateWeapon(curWeaponId, false);

                //fightHero.skillComponent = World.AddComponent<HeroSkillComponent>(fightHero);
                fightHero.heroSkillComponent.IsSuperHero = false;

                if (battleUnit.UnitType == (uint)EFightActorType.Hero)
                {
                    fightHero.heroSkillComponent.IsPlayer = true;
                }
                else if (battleUnit.UnitType == (uint)EFightActorType.Partner)
                {
                    fightHero.heroSkillComponent.IsPlayer = false;
                }

                fightHero.heroSkillComponent.DoInit();
                fightHero.heroSkillComponent.InitActiveSkill();

                fightHero.LoadModel(modelGameObject, (actor) =>
                {
                    //fightHero.clickComponent = World.AddComponent<ClickComponent>(fightHero);
                    fightHero.clickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                    fightHero.clickComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                    //fightHero.doubleClickComponent = World.AddComponent<DoubleClickComponent>(fightHero);
                    fightHero.doubleClickComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                    fightHero.doubleClickComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                    //fightHero.longpressComponent = World.AddComponent<LongPressComponent>(fightHero);
                    fightHero.longpressComponent.InteractiveAimType = EInteractiveAimType.ChooseActorInFight;
                    fightHero.longpressComponent.LayMask = ELayerMask.Player | ELayerMask.Partner;

                    //AnimationComponent animationComponent = World.AddComponent<AnimationComponent>(fightHero);
                    fightHero.animationComponent.SetSimpleAnimation(fightHero.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                    fightHero.animationComponent.UpdateHoldingAnimations(battleUnit.UnitInfoId, curWeaponId);

                    MobManager.Instance.AddMob(battleUnit, fightHero.animationComponent, fightHero.gameObject, curWeaponId, isCalling);
                });
            };
            AddressablesUtil.InstantiateAsync(ref mHandle, cSVCharacterData.model, loadCallback);
        }
    }
}
#endif