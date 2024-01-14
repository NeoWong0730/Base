using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public partial class HUD : UIBase
    {
        private Dictionary<uint, AnimQueue> m_AnimQueue = new Dictionary<uint, AnimQueue>();

        private uint m_GlobalPassiveNameCount = 29;

        public class AnimQueue
        {
            private Queue<TriggerAnimEvt> m_AnimQueue = new Queue<TriggerAnimEvt>();
            private float m_AnimPlayInterval;
            private float m_AnimPlayTimer;
            private Action<TriggerAnimEvt> m_CreateAction;
            private Action<AnimQueue> m_DestroyAction;
            private bool b_Valid = false;
            public int count
            {
                get
                {
                    return m_AnimQueue.Count;
                }
            }

            public void Start(Action<TriggerAnimEvt> createAction)
            {
                m_AnimPlayInterval = int.Parse(CSVParam.Instance.GetConfData(113).str_value) / 1000f;
                m_AnimPlayTimer = 0;
                m_CreateAction = createAction;
                b_Valid = true;
            }

            public void PushAnim(TriggerAnimEvt triggerAnimEvt)
            {
                m_AnimQueue.Enqueue(triggerAnimEvt);
            }

            public void OnUpdate()
            {
                if (!b_Valid)
                {
                    return;
                }
                if (m_AnimQueue.Count == 0)
                {
                    m_DestroyAction?.Invoke(this);
                    Dispose();
                    return;
                }
                else
                {
                    float dt = Sys_HUD.Instance.GetDeltaTime();
                    m_AnimPlayTimer -= dt;
                    if (m_AnimPlayTimer <= 0)
                    {
                        m_AnimPlayTimer = m_AnimPlayInterval;
                        TriggerAnimEvt triggerAnimEvt = m_AnimQueue.Dequeue();
                        m_CreateAction.Invoke(triggerAnimEvt);
                        triggerAnimEvt.Push();
                    }
                }
            }

            public void Dispose()
            {
                b_Valid = false;
                m_CreateAction = null;
            }
        }

        private void __CreateAnim(TriggerAnimEvt triggerAnimEvt)
        {
            if (!m_AnimQueue.TryGetValue(triggerAnimEvt.id, out AnimQueue animQueue))
            {
                animQueue = new AnimQueue();
                m_AnimQueue[triggerAnimEvt.id] = animQueue;
            }
            animQueue.PushAnim(triggerAnimEvt);
            //animQueue.Start(_CreateAnim);
        }

        private GameObject GetAnimGameobject()
        {
            GameObject animGo;
            animGo = AnimPools.Get(root_Anim);
            animGo.SetActive(true);
            return animGo;
        }


        public void CreateAnim(TriggerAnimEvt triggerAnimEvt)
        {
            if (!Sys_HUD.Instance.battleHuds.ContainsKey(triggerAnimEvt.id))
            {
                DebugUtil.LogFormat(ELogType.eHUD, "战斗内不含战斗单位 {0},但还需要飘字", triggerAnimEvt.id);
                return;
            }
#if DEBUG_MODE
            Transform mobEntity = MobManager.Instance.GetMob(triggerAnimEvt.id).m_Trans;
            DebugUtil.Log(ELogType.eHUD, string.Format($"id: {triggerAnimEvt.id}, type:  {triggerAnimEvt.AnimType}  value:{triggerAnimEvt.finnaldamage}  name: {mobEntity.name}"));
#endif
            Sys_HUD.Instance.CheckToInitDamageShowTypeState(triggerAnimEvt.id);
            b_CheckedDestroyAnim = false;
            uint id = 0;
            CSVDamageShowConfig.Data cSVDamageShowConfigData = null;
            if (HasAnimType(triggerAnimEvt.AnimType, AnimType.e_Combo))
            {
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_Combo, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }
                ComboGroup comboGroup;
                if (!combos.TryGetValue(Sys_HUD.Instance.battleHuds[triggerAnimEvt.id], out comboGroup))
                {
                    comboGroup = new ComboGroup(templete_Anim, cSVDamageShowConfigData, GetAnimGo, PushAnimGo);
                    combos.Add(Sys_HUD.Instance.battleHuds[triggerAnimEvt.id], comboGroup);
                    combosList.Add(comboGroup);
                }

                ComboData comboData = CombatObjectPool.Instance.Get<ComboData>();
                comboData.animType = triggerAnimEvt.AnimType & (~AnimType.e_Combo);
                comboData.finnaldamage = triggerAnimEvt.finnaldamage;
                comboData.floatingdamage = triggerAnimEvt.floatingdamage;
                comboData.attackType = triggerAnimEvt.attackType;
                comboData.hitType = triggerAnimEvt.hitType;
                comboData.attackInfoId = triggerAnimEvt.attackInfoId;
                comboData.hitInfoId = triggerAnimEvt.hitInfoId;
                comboData.race_hit = triggerAnimEvt.race_hit;
                comboData.race_attack = triggerAnimEvt.race_attack;
                
                comboGroup.PushCombData(comboData);
                if (Sys_HUD.Instance.battleHuds[triggerAnimEvt.id] == null)
                {
                    DebugUtil.LogErrorFormat("HUD.CreateAnim===>不存在战斗单位{0}", triggerAnimEvt.id);
                    return;
                }
                comboGroup.SetTarget(Sys_HUD.Instance.battleHuds[triggerAnimEvt.id].transform);
                //CombatObjectPool.Instance.Push(comboData);
                return;
            }

            if (triggerAnimEvt.AnimType == AnimType.e_Normal)
            {
                GameObject go_ui = GetAnimGameobject();
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_Normal, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }
                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_Normal;
                animData.uiObj = go_ui;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = triggerAnimEvt.finnaldamage;
                animData.floatingdamage = triggerAnimEvt.floatingdamage;
                animData.content = string.Empty;
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;
                animData.attackType = triggerAnimEvt.attackType;
                animData.hitType = triggerAnimEvt.hitType;
                animData.attackInfoId = triggerAnimEvt.attackInfoId;
                animData.hitInfoId = triggerAnimEvt.hitInfoId;
                animData.race_hit = triggerAnimEvt.race_hit;
                animData.race_attack = triggerAnimEvt.race_attack;

                NormalCharacterShow normalCharacterShow = HUDFactory.Get<NormalCharacterShow>();
                normalCharacterShow.Construct(animData, cSVDamageShowConfigData, RecyleAnim);
                anim_BaseShows.Add(normalCharacterShow);
            }
            if (triggerAnimEvt.AnimType == AnimType.e_Crit)
            {
                GameObject go_ui = GetAnimGameobject();
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_Crit, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }
                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_Crit;
                animData.uiObj = go_ui;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = triggerAnimEvt.finnaldamage;
                animData.floatingdamage = triggerAnimEvt.floatingdamage;
                animData.content = string.Empty;
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;
                animData.attackType = triggerAnimEvt.attackType;
                animData.hitType = triggerAnimEvt.hitType;
                animData.attackInfoId = triggerAnimEvt.attackInfoId;
                animData.hitInfoId = triggerAnimEvt.hitInfoId;
                animData.race_hit = triggerAnimEvt.race_hit;
                animData.race_attack = triggerAnimEvt.race_attack;

                CritCharacterShow critCharacterShow = HUDFactory.Get<CritCharacterShow>();
                critCharacterShow.Construct(animData, cSVDamageShowConfigData, RecyleAnim);
                anim_BaseShows.Add(critCharacterShow);
            }
            if (triggerAnimEvt.AnimType == AnimType.e_Poison)
            {
                GameObject go_ui = GetAnimGameobject();
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_Poison, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }

                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_Poison;
                animData.uiObj = go_ui;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = triggerAnimEvt.finnaldamage;
                animData.floatingdamage = triggerAnimEvt.floatingdamage;
                animData.content = string.Empty;
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;
                animData.attackType = triggerAnimEvt.attackType;
                animData.hitType = triggerAnimEvt.hitType;
                animData.attackInfoId = triggerAnimEvt.attackInfoId;
                animData.hitInfoId = triggerAnimEvt.hitInfoId;
                animData.race_hit = triggerAnimEvt.race_hit;
                animData.race_attack = triggerAnimEvt.race_attack;

                PoisonCharacterShow poisonCharacterShow = HUDFactory.Get<PoisonCharacterShow>();
                poisonCharacterShow.Construct(animData, cSVDamageShowConfigData, RecyleAnim);
                anim_BaseShows.Add(poisonCharacterShow);
            }
            if (triggerAnimEvt.AnimType == AnimType.e_Drunk)
            {
                GameObject go_ui = GetAnimGameobject();
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_Drunk, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }

                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_Drunk;
                animData.uiObj = go_ui;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = triggerAnimEvt.finnaldamage;
                animData.floatingdamage = triggerAnimEvt.floatingdamage;
                animData.content = string.Empty;
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;
                animData.attackType = triggerAnimEvt.attackType;
                animData.hitType = triggerAnimEvt.hitType;
                animData.attackInfoId = triggerAnimEvt.attackInfoId;
                animData.hitInfoId = triggerAnimEvt.hitInfoId;
                animData.race_hit = triggerAnimEvt.race_hit;
                animData.race_attack = triggerAnimEvt.race_attack;

                DrunkCharacterShow drunkCharacterShow = HUDFactory.Get<DrunkCharacterShow>();
                drunkCharacterShow.Construct(animData, cSVDamageShowConfigData, RecyleAnim);
                anim_BaseShows.Add(drunkCharacterShow);
            }
            if (triggerAnimEvt.AnimType == AnimType.e_AddHp)
            {
                GameObject go_ui = GetAnimGameobject();
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_AddHp, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }

                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_AddHp;
                animData.uiObj = go_ui;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = triggerAnimEvt.finnaldamage;
                animData.floatingdamage = triggerAnimEvt.floatingdamage;
                animData.content = string.Empty;
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;

                AddHpCharacterShow addBloodCharacterShow = HUDFactory.Get<AddHpCharacterShow>();
                addBloodCharacterShow.Construct(animData, cSVDamageShowConfigData, RecyleAnim);
                anim_BaseShows.Add(addBloodCharacterShow);
            }
            if (triggerAnimEvt.AnimType == AnimType.e_AddMp)
            {
                GameObject go_ui = GetAnimGameobject();
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_AddMp, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }

                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_AddMp;
                animData.uiObj = go_ui;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = triggerAnimEvt.finnaldamage;
                animData.floatingdamage = triggerAnimEvt.floatingdamage;
                animData.content = string.Empty;
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;

                AddMpCharacterShow addMpCharacterShow = HUDFactory.Get<AddMpCharacterShow>();
                addMpCharacterShow.Construct(animData, cSVDamageShowConfigData, RecyleAnim);
                anim_BaseShows.Add(addMpCharacterShow);
            }
            if (triggerAnimEvt.AnimType == AnimType.e_DeductMp)
            {
                GameObject go_ui = GetAnimGameobject();
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_DeductMp, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }

                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_DeductMp;
                animData.uiObj = go_ui;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = triggerAnimEvt.finnaldamage;
                animData.floatingdamage = triggerAnimEvt.floatingdamage;
                animData.content = string.Empty;
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;

                DeductMpCharacterShow deductMpCharacterShow = HUDFactory.Get<DeductMpCharacterShow>();
                deductMpCharacterShow.Construct(animData, cSVDamageShowConfigData, RecyleAnim);
                anim_BaseShows.Add(deductMpCharacterShow);
            }
            if (triggerAnimEvt.AnimType == AnimType.e_ExtractMp)
            {
                GameObject go_ui = GetAnimGameobject();
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_ExtractMp, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }

                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_ExtractMp;
                animData.uiObj = go_ui;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = triggerAnimEvt.finnaldamage;
                animData.floatingdamage = triggerAnimEvt.floatingdamage;
                animData.content = string.Empty;
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;

                ExtractMpCharacterShow extractMpCharacterShow = HUDFactory.Get<ExtractMpCharacterShow>();
                extractMpCharacterShow.Construct(animData, cSVDamageShowConfigData, RecyleAnim);
                anim_BaseShows.Add(extractMpCharacterShow);
            }
            if (triggerAnimEvt.AnimType == AnimType.e_Error)
            {
                GameObject go_ui = GetAnimGameobject();
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_Error, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }

                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_Error;
                animData.uiObj = go_ui;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = -1;
                animData.floatingdamage = -1;
                animData.content = "失误";
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;

                ErrorCharactorShow errorCharactorShow = HUDFactory.Get<ErrorCharactorShow>();
                errorCharactorShow.Construct(animData, cSVDamageShowConfigData, RecyleAnim);
                anim_BaseShows.Add(errorCharactorShow);
            }
            if (triggerAnimEvt.AnimType == AnimType.e_Miss)
            {
                GameObject go_ui = GetAnimGameobject();
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_Miss, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }

                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_Miss;
                animData.uiObj = go_ui;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = -1;
                animData.floatingdamage = -1;
                animData.content = "闪避";
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;

                MissCharacterShow specialCharacterShow = HUDFactory.Get<MissCharacterShow>();
                specialCharacterShow.Construct(animData, cSVDamageShowConfigData, RecyleAnim);
                anim_BaseShows.Add(specialCharacterShow);
            }
            if (triggerAnimEvt.AnimType == AnimType.e_PassiveDamage)
            {
                GameObject go_ui = GetAnimGameobject();
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_Drunk, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }

                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_PassiveDamage;
                animData.uiObj = go_ui;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = triggerAnimEvt.finnaldamage;
                animData.floatingdamage = triggerAnimEvt.floatingdamage;
                animData.content = string.Empty;
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;
                //animData.attackType = triggerAnimEvt.attackType;
                //animData.hitType = triggerAnimEvt.hitType;
                //animData.attackInfoId = triggerAnimEvt.attackInfoId;
                //animData.hitInfoId = triggerAnimEvt.hitInfoId;

                PassiveDamageShow passiveDamageShow = HUDFactory.Get<PassiveDamageShow>();
                passiveDamageShow.Construct(animData, cSVDamageShowConfigData, RecyleAnim);
                anim_BaseShows.Add(passiveDamageShow);
            }

            if (triggerAnimEvt.AnimType == AnimType.e_PassiveName)
            {
                GameObject go_ui;
                go_ui = PassiveNamePools.Get(root_PassiveName);
                go_ui.SetActive(true);

                CalPassiveCount();
                cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData((uint)m_GlobalPassiveNameCount);
                //id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_PassiveName, 2) + 1;
                //if (triggerAnimEvt.playType == 0)
                //{
                //    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                //}
                //else
                //{
                //    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                //}

                CSVPassiveSkill.Data cSVPassiveSkillData = CSVPassiveSkill.Instance.GetConfData(triggerAnimEvt.passiveId);
                string content = string.Empty;
                if (cSVPassiveSkillData != null)
                {
                    CSVLanguage.Data cSVLanguageData = CSVLanguage.Instance.GetConfData(cSVPassiveSkillData.behavior_name);
                    if (cSVLanguageData != null)
                    {
                        content = cSVLanguageData.words;
                    }
                    else
                    {
                        DebugUtil.LogErrorFormat($"不存在语言id {cSVPassiveSkillData.behavior_name}");
                    }
                }
                else
                {
                    DebugUtil.LogErrorFormat($"PassiveSkillData {triggerAnimEvt.passiveId}");
                }
                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_PassiveName;
                animData.uiObj = go_ui;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = triggerAnimEvt.finnaldamage;
                animData.floatingdamage = triggerAnimEvt.floatingdamage;
                animData.content = content;
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;

                PassiveNameShow passiveNameShow = HUDFactory.Get<PassiveNameShow>();
                passiveNameShow.Construct(animData, cSVDamageShowConfigData, RecyclePassiveName);
                anim_BaseShows.Add(passiveNameShow);
            }

            if (triggerAnimEvt.AnimType == AnimType.e_MagicShort)
            {
                GameObject go_ui1;
                go_ui1 = AnimNamePools.Get(root_SkillShow);
                go_ui1.SetActive(true);
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_MagicShort, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }

                string content = CSVLanguage.Instance.GetConfData(uint.Parse(CSVParam.Instance.GetConfData(605).str_value)).words;

                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_MagicShort;
                animData.uiObj = go_ui1;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = triggerAnimEvt.finnaldamage;
                animData.floatingdamage = triggerAnimEvt.floatingdamage;
                animData.content = content;
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;

                PassiveNameShow passiveNameShow = HUDFactory.Get<PassiveNameShow>();
                passiveNameShow.Construct(animData, cSVDamageShowConfigData, RecycleAnimName);
                anim_BaseShows.Add(passiveNameShow);
            }
            if (triggerAnimEvt.AnimType == AnimType.e_EnergyShort)
            {
                GameObject go_ui1;
                go_ui1 = AnimNamePools.Get(root_SkillShow);
                go_ui1.SetActive(true);
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_EnergyShort, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }

                string content = CSVLanguage.Instance.GetConfData(uint.Parse(CSVParam.Instance.GetConfData(606).str_value)).words;

                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_EnergyShort;
                animData.uiObj = go_ui1;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = triggerAnimEvt.finnaldamage;
                animData.floatingdamage = triggerAnimEvt.floatingdamage;
                animData.content = content;
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;

                PassiveNameShow passiveNameShow = HUDFactory.Get<PassiveNameShow>();
                passiveNameShow.Construct(animData, cSVDamageShowConfigData, RecycleAnimName);
                anim_BaseShows.Add(passiveNameShow);
            }

            if (HasAnimType(triggerAnimEvt.AnimType, AnimType.e_ConverAttack))
            {
                GameObject go_ui = GetAnimGameobject();
                id = (uint)NGUIMath.GetLogarithmic((uint)AnimType.e_ConverAttack, 2) + 1;
                if (triggerAnimEvt.playType == 0)
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(Sys_HUD.Instance.GetDamageShowConfigIdByShowTypeId(id));
                }
                else
                {
                    cSVDamageShowConfigData = CSVDamageShowConfig.Instance.GetConfData(triggerAnimEvt.playType);
                }

                AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
                animData.animType = AnimType.e_Normal;
                animData.uiObj = go_ui;
                animData.battleObj = Sys_HUD.Instance.battleHuds[triggerAnimEvt.id];
                animData.finnaldamage = triggerAnimEvt.finnaldamage;
                animData.floatingdamage = triggerAnimEvt.floatingdamage;
                animData.content = string.Empty;
                animData.bUseTrans = true;
                animData.pos = Vector3.zero;

                NormalCharacterShow converCharacterShow = HUDFactory.Get<NormalCharacterShow>();
                converCharacterShow.Construct(animData, cSVDamageShowConfigData, RecyleAnim);
                converCharacterShow.DoAction();
                anim_BaseShows.Add(converCharacterShow);

                GameObject @object = AnimPools.Get(root_Anim);
                @object.SetActive(true);
                AnimData animData2 = CombatObjectPool.Instance.Get<AnimData>();
                animData2.animType = AnimType.e_ConverAttack;
                animData2.uiObj = @object;
                animData2.battleObj = null;
                animData2.finnaldamage = -1;
                animData2.floatingdamage = -1;
                animData2.content = string.Format($"{triggerAnimEvt.converCount}合击");
                animData2.bUseTrans = true;
                animData2.pos = Vector3.zero;
                ConverCharacterShow conver = HUDFactory.Get<ConverCharacterShow>();
                conver.Construct(animData2, cSVDamageShowConfigData, RecyleAnim);
                if (triggerAnimEvt.IsEnemy)
                {
                    conver.SetUIPosition(0.12f, 0.8f);
                }
                else
                {
                    conver.SetUIPosition(0.88f, 0.2f);
                }
                conver.DoAction();
                anim_BaseShows.Add(conver);
            }
        }

        private bool HasAnimType(AnimType parent, AnimType child)
        {
            return (parent & child) == child;
        }


        public void RecyleAnim(Anim_BaseShow anim_BaseShow)
        {
            AnimPools.Recovery(anim_BaseShow.mRootGameObject);
            anim_BaseShow.Dispose();
            HUDFactory.Recycle(anim_BaseShow);
            anim_BaseShows.Remove(anim_BaseShow);
        }

        public void RecycleAnimName(Anim_BaseShow anim_BaseShow)
        {
            AnimNamePools.Recovery(anim_BaseShow.mRootGameObject);
            anim_BaseShow.Dispose();
            HUDFactory.Recycle(anim_BaseShow);
            anim_BaseShows.Remove(anim_BaseShow);
        }

        public void RecyclePassiveName(Anim_BaseShow anim_BaseShow)
        {
            PassiveNamePools.Recovery(anim_BaseShow.mRootGameObject);
            anim_BaseShow.Dispose();
            HUDFactory.Recycle(anim_BaseShow);
            anim_BaseShows.Remove(anim_BaseShow);
        }

        private void CalPassiveCount()
        {
            if (m_GlobalPassiveNameCount == 34)
            {
                m_GlobalPassiveNameCount = 30;
            }
            m_GlobalPassiveNameCount++;
        }
    }
}

