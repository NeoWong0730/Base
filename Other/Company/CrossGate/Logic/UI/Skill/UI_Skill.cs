using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Packet;
using static Net_Combat;
using Framework;

namespace Logic
{
    public class SkillPreView : Singleton<SkillPreView>
    {
        public uint RoleID
        {
            get;
            private set;
        }

        public uint CareerID
        {
            get;
            private set;
        }

        public uint WeaponID
        {
            get;
            private set;
        }

        public RenderTexture RenderTexture
        {
            get;
            set;
        }

        public uint SkillInfoID
        {
            get;
            private set;
        }

        private List<MobEntity> _targetList = new List<MobEntity>();
        private Timer _playCombatSkillTimer;
        private float _playTime;
        private int _playIndex;

        private List<SkillPreViewMonster> _skillPreViewMonsters = new List<SkillPreViewMonster>();
        private List<SkillPreViewFightHero> _skillPreViewFightHeros = new List<SkillPreViewFightHero>();

        public void Init(uint roleID, uint carrerID, uint weaponID, uint skillInfoID)
        {
            RoleID = roleID;
            CareerID = carrerID;
            WeaponID = weaponID;
            SkillInfoID = skillInfoID;
        }

        public void Dispose()
        {
            if (CombatManager.Instance.m_CombatStyleState == 1)
                CombatManager.Instance.OnDisable();
            _targetList.Clear();
            if (_playCombatSkillTimer != null)
            {
                _playCombatSkillTimer.Cancel();
                _playCombatSkillTimer = null;
            }

            //GameCenter.SkillPreViewWorld?.Dispose();
            for (int i = _skillPreViewMonsters.Count - 1; i >= 0; --i)
            {
                SkillPreViewMonster skillPreViewMonster = _skillPreViewMonsters[i];
                World.CollecActor(ref skillPreViewMonster);
            }
            _skillPreViewMonsters.Clear();

            for (int i = _skillPreViewFightHeros.Count - 1; i >= 0; --i)
            {
                SkillPreViewFightHero skillPreViewFightHero = _skillPreViewFightHeros[i];
                World.CollecActor(ref skillPreViewFightHero);
            }
            _skillPreViewFightHeros.Clear();
        }


        public void ShowSkillPreView(uint skillShowID)
        {
            CSVActiveSkillShow.Data cSVActiveSkillShowData = CSVActiveSkillShow.Instance.GetConfData(skillShowID);
            if (cSVActiveSkillShowData == null)
            {
                return;
            }
            UIManager.OpenUI(EUIID.UI_SkillPlay, false, SkillInfoID);

            CombatManager.Instance.EnterCombatSkillPreView();

            LoadMonsters(cSVActiveSkillShowData);

            LoadPlayers(cSVActiveSkillShowData);

            _playIndex = -1;
            _playTime = 0f;
            _playCombatSkillTimer = Timer.Register(9999f, null, (float f) =>
            {
                DoPlayCombatSkill(cSVActiveSkillShowData);
            }, true);
        }

        public void ShowSkillPreViewForParnter(uint skillShowID, uint parnterID)
        {
            CSVActiveSkillShow.Data cSVActiveSkillShowData = CSVActiveSkillShow.Instance.GetConfData(skillShowID);
            if (cSVActiveSkillShowData == null)
            {
                return;
            }
            UIManager.OpenUI(EUIID.UI_SkillPlay, false, skillShowID);
            CombatManager.Instance.EnterCombatSkillPreView();

            LoadMonsters(cSVActiveSkillShowData);

            LoadPlayers(cSVActiveSkillShowData, parnterID);

            _playIndex = -1;
            _playTime = 0f;
            _playCombatSkillTimer = Timer.Register(9999f, null, (float f) =>
            {
                DoPlayCombatSkill(cSVActiveSkillShowData);
            }, true);
        }

        void LoadMonsters(CSVActiveSkillShow.Data cSVActiveSkillShowData)
        {
            if (cSVActiveSkillShowData.enemy_position != null)
            {
                foreach (var pair in cSVActiveSkillShowData.enemy_position)
                {
                    CreateMonster(pair[0], pair[1]);
                }
            }
        }

        void CreateMonster(uint pos, uint monsterID)
        {
            CSVMonster.Data monsterData = CSVMonster.Instance.GetConfData(monsterID);

            if (monsterData == null)
            {
                Debug.LogError($"monsterData is null id:{monsterID}");
                return;
            }

            //SkillPreViewMonster skillPreViewMonster = GameCenter.SkillPreViewWorld.CreateActor<SkillPreViewMonster>(pos);            
            SkillPreViewMonster skillPreViewMonster = World.AllocActor<SkillPreViewMonster>(pos);
            _skillPreViewMonsters.Add(skillPreViewMonster);

            skillPreViewMonster.SetName($"SkillPreViewMonster_{pos.ToString()}");
            skillPreViewMonster.SetParent(GameCenter.modelShowRoot);

            skillPreViewMonster.cSVMonsterData = monsterData;
            //skillPreViewMonster.weaponComponent = World.AddComponent<WeaponComponent>(skillPreViewMonster);

            //skillPreViewMonster.skillComponent = World.AddComponent<MonsterSkillComponent>(skillPreViewMonster);
            skillPreViewMonster.monsterSkillComponent.DoInit();

            skillPreViewMonster.LoadModel(monsterData.model, (System.Action<SceneActor>)((actor) =>
            {
                skillPreViewMonster.weaponComponent.UpdateWeapon(skillPreViewMonster.cSVMonsterData.weapon_id, false);

                //AnimationComponent animationComponent = World.AddComponent<AnimationComponent>(actor);
                skillPreViewMonster.animationComponent.SetSimpleAnimation(skillPreViewMonster.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                skillPreViewMonster.animationComponent.UpdateHoldingAnimations(skillPreViewMonster.cSVMonsterData.monster_id, monsterData.weapon_id);

                AddMob(UnitType.Monster, monsterID, (int)pos, skillPreViewMonster.gameObject, skillPreViewMonster.weaponComponent.CurWeaponID, skillPreViewMonster.animationComponent);
            }));
        }

        void LoadPlayers(CSVActiveSkillShow.Data cSVActiveSkillShowData)
        {
            CreateMainPlayer(2);
            if (cSVActiveSkillShowData.friend_position != null)
            {
                foreach (var pair in cSVActiveSkillShowData.friend_position)
                {
                    CreateParnter(pair[0], pair[1]);
                }
            }
        }

        void LoadPlayers(CSVActiveSkillShow.Data cSVActiveSkillShowData, uint parnterID)
        {
            CreateParnter(2, parnterID);
            if (cSVActiveSkillShowData.friend_position != null)
            {
                foreach (var pair in cSVActiveSkillShowData.friend_position)
                {
                    CreateParnter(pair[0], pair[1]);
                }
            }
        }

        void CreateMainPlayer(uint pos)
        {
            CSVCharacter.Data cSVCharacterData = CSVCharacter.Instance.GetConfData(RoleID);
            if (cSVCharacterData == null)
            {
                Debug.LogError($"cSVCharacterData is null id:{RoleID}");
                return;
            }

            //SkillPreViewFightHero skillPreViewFightHero = GameCenter.SkillPreViewWorld.CreateActor<SkillPreViewFightHero>(pos);

            SkillPreViewFightHero skillPreViewFightHero = World.AllocActor<SkillPreViewFightHero>(pos);
            _skillPreViewFightHeros.Add(skillPreViewFightHero);

            skillPreViewFightHero.SetName($"SkillPreViewFightHero_{pos.ToString()}");
            skillPreViewFightHero.SetParent(GameCenter.modelShowRoot);

            //skillPreViewFightHero.careerComponent = World.AddComponent<CareerComponent>(skillPreViewFightHero);
            skillPreViewFightHero.careerComponent.UpdateCareerType((ECareerType)CareerID);

            //skillPreViewFightHero.weaponComponent = World.AddComponent<WeaponComponent>(skillPreViewFightHero);


            //skillPreViewFightHero.skillComponent = World.AddComponent<HeroSkillComponent>(skillPreViewFightHero);
            //((HeroSkillComponent)(skillPreViewFightHero.skillComponent)).IsPlayer = true;
            //((HeroSkillComponent)(skillPreViewFightHero.skillComponent)).InitCareerSkill();

            skillPreViewFightHero.LoadModel(cSVCharacterData.model, (System.Action<SceneActor>)((actor) =>
            {
                skillPreViewFightHero.weaponComponent.UpdateWeapon(WeaponID);

                //AnimationComponent animationComponent = World.AddComponent<AnimationComponent>(actor);
                skillPreViewFightHero.animationComponent.SetSimpleAnimation(skillPreViewFightHero.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                skillPreViewFightHero.animationComponent.UpdateHoldingAnimations(RoleID, skillPreViewFightHero.weaponComponent.CurWeaponID);

                AddMob(UnitType.Hero, RoleID, (int)pos, skillPreViewFightHero.gameObject, skillPreViewFightHero.weaponComponent.CurWeaponID, skillPreViewFightHero.animationComponent);
            }));
        }

        void CreateParnter(uint pos, uint parnterID)
        {
            CSVPartner.Data partnerInfoData = CSVPartner.Instance.GetConfData(parnterID);
            if (partnerInfoData == null)
            {
                Debug.LogError($"CSVPartner.Data is null id:{parnterID}");
                return;
            }

            //SkillPreViewFightHero skillPreViewFightHero = GameCenter.SkillPreViewWorld.CreateActor<SkillPreViewFightHero>(pos);
            SkillPreViewFightHero skillPreViewFightHero = World.AllocActor<SkillPreViewFightHero>(pos);
            _skillPreViewFightHeros.Add(skillPreViewFightHero);

            skillPreViewFightHero.SetName($"SkillPreViewFightHero_{pos.ToString()}");
            skillPreViewFightHero.SetParent(GameCenter.modelShowRoot);

            //skillPreViewFightHero.careerComponent = World.AddComponent<CareerComponent>(skillPreViewFightHero);
            skillPreViewFightHero.careerComponent.UpdateCareerType((ECareerType)partnerInfoData.occupation);

            //skillPreViewFightHero.weaponComponent = World.AddComponent<WeaponComponent>(skillPreViewFightHero);

            //skillPreViewFightHero.skillComponent = World.AddComponent<HeroSkillComponent>(skillPreViewFightHero);
            //((HeroSkillComponent)(skillPreViewFightHero.skillComponent)).IsPlayer = false;
            //((HeroSkillComponent)(skillPreViewFightHero.skillComponent)).InitCareerSkill();

            skillPreViewFightHero.LoadModel(partnerInfoData.model, (System.Action<SceneActor>)((actor) =>
            {
                skillPreViewFightHero.weaponComponent.UpdateWeapon(partnerInfoData.weaponID, false);

                //AnimationComponent animationComponent = World.AddComponent<AnimationComponent>(actor);
                skillPreViewFightHero.animationComponent.SetSimpleAnimation(skillPreViewFightHero.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                skillPreViewFightHero.animationComponent.UpdateHoldingAnimations(parnterID, partnerInfoData.weaponID);

                AddMob(UnitType.Partner, parnterID, (int)pos, skillPreViewFightHero.gameObject, partnerInfoData.weaponID, skillPreViewFightHero.animationComponent);
            }));
        }

        private void AddMob(UnitType unitType, uint unitInfoId, int pos, GameObject go, uint weaponId, AnimationComponent animationComponent)
        {
            BattleUnit battleUnit = new BattleUnit();
            battleUnit.UnitId = ++CombatHelp.m_UnitStartId;
            battleUnit.UnitType = (uint)unitType;
            battleUnit.UnitInfoId = unitInfoId;
            battleUnit.RoleId = RoleID;
            battleUnit.Pos = pos;
            battleUnit.MaxHp = 9999u;
            battleUnit.CurHp = 9999;
            if (unitType == UnitType.Partner)
            {
                battleUnit.Level = 1;
            }
            battleUnit.Side = CombatHelp.GetServerCampSide(battleUnit.Pos);

            MobManager.Instance.AddMob(battleUnit, animationComponent, go, weaponId);
        }

        public void DoPlayCombatSkill(CSVActiveSkillShow.Data cSVActiveSkillShowData)
        {
            if (_playIndex == -1)
            {
                _playIndex = 0;
                _playTime = Time.time + cSVActiveSkillShowData.interval[0] * 0.001f;
                foreach (var kv in MobManager.Instance.m_MobDic)
                {
                    MobEntity me = kv.Value;
                    if (me == null)
                        continue;

                    me.m_MobCombatComponent.SetMobReviveState();
                }
            }

            if (Time.time < _playTime)
                return;

            if (_playIndex >= cSVActiveSkillShowData.skill_combat_id.Count)
            {
                _playIndex = -1;
                return;
            }

            DLogManager.Log(ELogType.eCombat, $"<color=yellow>开始技能演示Id为：{cSVActiveSkillShowData.skill_combat_id[_playIndex].ToString()}</color>");
            #region 技能演示
            CSVActiveSkillShowId.Data assid = CSVActiveSkillShowId.Instance.GetConfData(cSVActiveSkillShowData.skill_combat_id[_playIndex]);
            if (assid != null && assid.attack_nums != null && assid.attack_nums.Count > 0)
            {
                if (assid.buffid != null)
                {
                    for (int i = 0; i < assid.buffid.Count; i++)
                    {
                        List<uint> buffInfo = assid.buffid[i];
                        if (buffInfo[3] == 0u)
                        {
                            var def = MobManager.Instance.GetMobByClientNum((int)buffInfo[0]);
                            if (def == null)
                            {
                                DebugUtil.LogError($"没有客户端位置为：{buffInfo[0].ToString()}的目标添加buff");
                                continue;
                            }
                            def.UpdateBuffComponent(buffInfo[1], buffInfo[2]);
                        }
                    }
                }

                if (assid.target_nums != null)
                {
                    _targetList.Clear();
                    foreach (var targerNum in assid.target_nums)
                    {
                        var target = MobManager.Instance.GetMobByClientNum((int)targerNum); ;
                        if (target == null)
                        {
                            DebugUtil.LogError($"没有客户端位置为：{targerNum.ToString()}的目标");
                            continue;
                        }
                        int hpChange = (int)assid.value;
                        target.m_MobCombatComponent.m_BeHitToFlyState = assid.hit_to_fly == 1u;

                        CombatHpChangeData curHpChangeData = CombatObjectPool.Instance.Get<CombatHpChangeData>();
                        curHpChangeData.m_AnimType = (AnimType)assid.attack_type;
                        curHpChangeData.m_HpChange = hpChange;
                        curHpChangeData.m_Revive = (target.m_MobCombatComponent.m_Death && hpChange > 0);
                        curHpChangeData.m_Death = assid.is_dead == 1u;
                        curHpChangeData.m_ChangeType = (int)assid.damage_type;

                        target.m_MobCombatComponent.m_Death = curHpChangeData.m_Death;
                        if (assid.attack_nums.Count > 1)
                            curHpChangeData.m_AnimType |= AnimType.e_ConverAttack;
                        target.m_MobCombatComponent.m_HpChangeDataQueue.Enqueue(curHpChangeData);
                        _targetList.Add(target);
                    }
                }

                if (assid.attack_nums != null)
                {
                    int excuteTurnIndex = 9999;

                    if (!Net_Combat.Instance.m_NetExcuteTurnInfoDic.TryGetValue(excuteTurnIndex, out NetExcuteTurnInfo netExcuteTurnInfo) || netExcuteTurnInfo == null)
                    {
                        netExcuteTurnInfo = new NetExcuteTurnInfo();
                        Net_Combat.Instance.m_NetExcuteTurnInfoDic[excuteTurnIndex] = netExcuteTurnInfo;
                    }
                    else
                        netExcuteTurnInfo.Clear();

                    if (netExcuteTurnInfo.CombineAttack_SrcUnits == null)
                        netExcuteTurnInfo.CombineAttack_SrcUnits = new List<uint>();
                    netExcuteTurnInfo.CombineAttack_SrcUnits.Clear();
                    netExcuteTurnInfo.CombineAttack_ScrIndex = 0;
                    netExcuteTurnInfo.CombineAttack_WaitCount = 0;

                    CSVActiveSkill.Data skillDataTb = CSVActiveSkill.Instance.GetConfData(assid.skillid);
                    if (skillDataTb == null)
                    {
                        DebugUtil.LogError($"CSVActiveSkill表中没有技能Id：{assid.skillid.ToString()}");
                        return;
                    }

                    foreach (var attackNum in assid.attack_nums)
                    {
                        var attack = MobManager.Instance.GetMobByClientNum((int)attackNum); ;
                        if (attack == null)
                        {
                            DebugUtil.LogError($"攻击者没有客户端位置为：{attackNum.ToString()}");
                            continue;
                        }

                        netExcuteTurnInfo.CombineAttack_SrcUnits.Add(attack.m_MobCombatComponent.m_BattleUnit.UnitId);

                        for (int targetIndex = 0, count = _targetList.Count; targetIndex < count; targetIndex++)
                        {
                            for (int skillLogicIdIndex = 0, skillLogicCount = skillDataTb.skill_effect_id.Count; skillLogicIdIndex < skillLogicCount; skillLogicIdIndex++)
                            {
                                uint skillEffectLogicId = skillDataTb.skill_effect_id[skillLogicIdIndex];
                                if (skillEffectLogicId == 0u)
                                    continue;

                                Net_Combat.Instance.AddTurnBehaveInfo(netExcuteTurnInfo.TurnBehaveInfoList, 1, 0, attack.m_MobCombatComponent.m_BattleUnit.UnitId,
                                                        _targetList[targetIndex].m_MobCombatComponent.m_BattleUnit.UnitId, ++CombatHelp.m_UnitStartId, 1,
                                                        assid.skillid, 0u, 0u, assid.attack_nums.Count > 1, null, 0u, skillEffectLogicId);
                            }
                        }
                    }

                    bool isHaveBehave = false;
                    Net_Combat.Instance.PerformBehave(netExcuteTurnInfo.TurnBehaveInfoList, excuteTurnIndex, true, true, -1, ref isHaveBehave);
                }

                if (assid.buffid != null)
                {
                    for (int i = 0; i < assid.buffid.Count; i++)
                    {
                        List<uint> buffInfo = assid.buffid[i];
                        if (buffInfo[3] == 1u)
                        {
                            var def = MobManager.Instance.GetMobByClientNum((int)buffInfo[0]);
                            if (def == null)
                            {
                                DebugUtil.LogError($"没有客户端位置为：{buffInfo[0].ToString()}的目标添加buff");
                                continue;
                            }
                            def.UpdateBuffComponent(buffInfo[1], buffInfo[2]);
                        }
                    }
                }
            }
            #endregion

            ++_playIndex;

            if (_playIndex >= cSVActiveSkillShowData.skill_combat_id.Count)
                _playTime = Time.time + cSVActiveSkillShowData.repeat_interval * 0.001f;
            else
                _playTime = Time.time + cSVActiveSkillShowData.interval[_playIndex] * 0.001f;
        }
    }
}