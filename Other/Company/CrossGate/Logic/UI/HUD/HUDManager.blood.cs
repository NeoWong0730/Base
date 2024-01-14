using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;

namespace Logic
{
    public partial class HUD : UIBase
    {
        #region Blood

        private GameObject GetTemplateBlood()
        {
            GameObject go;
            go = BdPools.Get(root_BD);
            return go;
        }

        private GameObject GetTemplateBattleOrderFlag()
        {
            GameObject go;
            go = ComBatOrderFlagPools.Get(root_CombatOrderFlag);
            return go;
        }

        private GameObject GetCombatOrderName()
        {
            GameObject go;
            go = ComBatOrderNamePools.Get(root_ComOrderName);
            return go;
        }


        private GameObject GetTemplateBattleSelect()
        {
            GameObject go;
            go = ComBatSelectPools.Get(root_CombatSelect);
            return go;
        }

        public void CreateBlood(CreateBloodEvt createBloodEvt)
        {
            if (!Sys_HUD.Instance.battleHuds.ContainsKey(createBloodEvt.id))
            {
                Sys_HUD.Instance.battleHuds.Add(createBloodEvt.id, createBloodEvt.gameObject);
                Sys_HUD.Instance.battleAttrs.Add(createBloodEvt.id, createBloodEvt.attributeData);
            }
            if (!bloods.ContainsKey(createBloodEvt.id))
            {
                BDShow bDShow;
                GameObject bloodGo = GetTemplateBlood();
                bloodGo.SetActive(true);
                GameObject combatOrderFlagGo = GetTemplateBattleOrderFlag();
                combatOrderFlagGo.SetActive(true);
                GameObject combatOrderName = GetCombatOrderName();
                combatOrderName.SetActive(true);
                GameObject combatSelectGo = GetTemplateBattleSelect();
                combatSelectGo.SetActive(false);
#if UNITY_EDITOR
                combatSelectGo.name = createBloodEvt.id.ToString();
#endif
                bDShow = HUDFactory.Get<BDShow>();
                bDShow.Construct(createBloodEvt.id, bloodGo, combatOrderFlagGo, combatOrderName, combatSelectGo);
                bDShow.Init(createBloodEvt.attributeData);
                bDShow.SetTarget(createBloodEvt.gameObject.transform);
                //怪物的名字处理 非怪物默认显示
                if (createBloodEvt.attributeData.fightUnitType == (uint)EFightActorType.Monster)
                {
                    CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(createBloodEvt.attributeData.notPlayerAttr);
                    if (cSVMonsterData != null && cSVMonsterData.show_name == 0)
                    {
                        bDShow.HideName();
                    }
                }

                if (Net_Combat.Instance.m_IsWatchBattle)
                {
                    if (!CombatManager.Instance.m_BattleTypeTb.show_enemy_hp)
                    {
                        bDShow.HideHpAttr();
                    }
                    if (!CombatManager.Instance.m_BattleTypeTb.show_enemy_mp)
                    {
                        bDShow.HideMp();
                    }
                    //if (!CombatManager.Instance.m_BattleTypeTb.show_enemy_name)
                    //{
                    //    bDShow.HideName();
                    //}
                    if (!CombatManager.Instance.m_BattleTypeTb.show_enemy_element)
                    {
                        bDShow.HideAttr();
                    }
                    bDShow.HideRoleCareerIcon();
                }
                else
                {
                    if (GameCenter.mainFightHero != null)
                    {
                        bool isSameCamp = Sys_Fight.Instance.mainSide == createBloodEvt.side;
                        if (isSameCamp)
                        {
                            if (!CombatManager.Instance.m_BattleTypeTb.show_self_hp)
                            {
                                bDShow.HideHpAttr();
                            }
                            if (!CombatManager.Instance.m_BattleTypeTb.show_self_mp)
                            {
                                bDShow.HideMp();
                            }
                            //if (!CombatManager.Instance.m_BattleTypeTb.show_self_name)
                            //{
                            //    bDShow.HideName();
                            //}
                            if (!CombatManager.Instance.m_BattleTypeTb.show_self_element)
                            {
                                bDShow.HideAttr();
                            }
                            bDShow.HideRoleCareerIcon();
                        }
                        else
                        {
                            if (!CombatManager.Instance.m_BattleTypeTb.show_enemy_hp)
                            {
                                bDShow.HideHpAttr();
                            }
                            if (!CombatManager.Instance.m_BattleTypeTb.show_enemy_mp)
                            {
                                bDShow.HideMp();
                            }
                            //if (!CombatManager.Instance.m_BattleTypeTb.show_enemy_name)
                            //{
                            //    bDShow.HideName();
                            //}
                            if (!CombatManager.Instance.m_BattleTypeTb.show_enemy_element)
                            {
                                bDShow.HideAttr();
                            }
                        }
                    }
                }
                bloods.Add(createBloodEvt.id, bDShow);
                bloodsList.Add(bDShow);
            }
        }

        public void UpdateHp(HpValueChangedEvt hpValueChangedEvt)
        {
            if (bloods.TryGetValue(hpValueChangedEvt.id, out BDShow bDShow) && bDShow != null)
            {
                bDShow.UpdateBlood(hpValueChangedEvt.ratio);
            }
        }

        public void UpdateMp(MpValueChangedEvt mpValueChangedEvt)
        {
            if (bloods.TryGetValue(mpValueChangedEvt.id, out BDShow bDShow) && bDShow != null)
            {
                bDShow.UpdateMp(mpValueChangedEvt.ratio);
            }
        }

        public void UpdateShapeShiftValue(ShapeShiftChangedEvt ShapeShiftValueChangedEvt)
        {
            if (bloods.TryGetValue(ShapeShiftValueChangedEvt.id, out BDShow bDShow) && bDShow != null)
            {
                bDShow.SetRoleShapeShiftIcon(ShapeShiftValueChangedEvt.ShapeShiftId);
            }
        }

        public void UpdateShieldValue(ShieldValueChangedEvt shieldValueChangedEvt)
        {
            if (bloods.TryGetValue(shieldValueChangedEvt.id, out BDShow bDShow) && bDShow != null)
            {
                bDShow.UpdateShieldValue(shieldValueChangedEvt.ratio);
            }
        }
        public void UpdateEnergyValue(EnergyValueChangedEvt energyValueChangedEvt)
        {
            if (bloods.TryGetValue(energyValueChangedEvt.id, out BDShow bDShow) && bDShow != null)
            {
                bDShow.UpdateEnemgyValue(energyValueChangedEvt.ratio);
            }
        }
        public void ShowOrHideBlood(ShowOrHideBDEvt showOrHideBDEvt)
        {
            if (bloods.TryGetValue(showOrHideBDEvt.id, out BDShow bDShow) && bDShow != null)
            {
                bDShow.ShowOrHide(showOrHideBDEvt.flag);
            }
        }

        public void UpdateArrowState(UpdateArrowEvt updateArrowEvt)
        {
            if (bloods.TryGetValue(updateArrowEvt.id, out BDShow bDShow) && bDShow != null)
            {
                bDShow.SetArrow(updateArrowEvt.active);
            }
        }

        public void UpdateSparSkill(UpdateSparSkillEvt updateSparSkillEvt)
        {
            if (bloods.TryGetValue(updateSparSkillEvt.id, out BDShow bDShow) && bDShow != null)
            {
                bDShow.SetSparSkill(updateSparSkillEvt.active);
            }
        }

        private void OnTriggerBattleInstructionFlag(uint id, bool show, string content)
        {
            if (bloods.TryGetValue(id, out BDShow bDShow) && bDShow != null)
            {
                bDShow.OnTriggerBattleInstructionFlag(show, content);
            }
        }

        private void OnClearBattleFlag()
        {
            for (int i = 0; i < bloodsList.Count; ++i)
            {
                BDShow bDShow = bloodsList[i];
                bDShow?.ClearBattleFlag();
            }
        }

        public void RemoveBattleUint(uint id)
        {
            if (bloods.ContainsKey(id))
            {
                BDShow bDShow = bloods[id];
                bloodsList.Remove(bDShow);
                bDShow?.Dispose();
                BdPools.Recovery(bDShow.mRootGameObject);
                ComBatOrderNamePools.Recovery(bDShow.mBattleOrderName);
                ComBatOrderFlagPools.Recovery(bDShow.mBattleOrderFlag);
                ComBatSelectPools.Recovery(bDShow.mBattleSelect);
                HUDFactory.Recycle(bDShow);
                bloods.Remove(id);
            }
            if (buffhudShows.ContainsKey(id))
            {
                BuffHUDShow buffHUDShow = buffhudShows[id];
                buffHUDShow?.Dispose();
                BuffHUDPools.Recovery(buffHUDShow.mRootGameObject);
                buffhudShows.Remove(id);
                HUDFactory.Recycle(buffHUDShow);
            }
            if (m_AnimQueue.TryGetValue(id, out AnimQueue animQueue))
            {
                animQueue.Dispose();
                m_AnimQueue.Remove(id);
            }
            if (battleBubbleHuds.TryGetValue(id, out BubbleShow bubbleShow))
            {
                bubbleShow.SetTarget(null);
                //bubbleShow.Dispose();
                //HUDFactory.Recycle(bubbleShow);
                //BattleBubblePools.Recovery(bubbleShow.mRootGameobject);
                //battleBubbleHuds.Remove(id);
            }

            if (m_BattleBubbleTimer.TryGetValue(id, out Timer timer))
            {
                timer?.Cancel();
                m_BattleBubbleTimer.Remove(id);
            }
            m_BattleBubbleDatas.Remove(id);

            Sys_HUD.Instance.battleHuds.Remove(id);
            Sys_HUD.Instance.battleAttrs.Remove(id);
        }

        public void UpdateTimeSand(uint uintid, bool isActive)
        {
            if (bloods.TryGetValue(uintid, out BDShow bDShow) && bDShow != null)
            {
                bDShow.UpdateTimeSand(isActive);
            }
        }

        public void ShowOrHideSelect(uint unitid, bool isShow)
        {
            if (bloods.TryGetValue(unitid, out BDShow bDShow) && bDShow != null)
            {
                bDShow.ShowOrHideSelect(isShow);
            }
        }

        private void OnSelected(uint unitid)
        {
            if (bloods.TryGetValue(unitid, out BDShow bDShow) && bDShow != null)
            {
                bDShow.ShowSelected();
            }
        }

        #endregion
    }
}

