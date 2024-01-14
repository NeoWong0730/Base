using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Table;
using System;

namespace Logic
{
    public partial class Sys_LivingSkill : SystemModuleBase<Sys_LivingSkill>
    {
        private Timer m_GuideTimer;

        public bool SkillUnLock(uint skillId)
        {
            if (livingSkills.TryGetValue(skillId, out LivingSkill livingSkill))
            {
                return livingSkill.Unlock;
            }
            else
            {
                DebugUtil.LogErrorFormat($"找不到技能id:{skillId}");
                return false;
            }
        }

        public void UpdateGradeCanLock()
        {
            int MaxRank = int.Parse(CSVParam.Instance.GetConfData(650).str_value);
            if (grade == MaxRank)
            {
                CanGradeLevelUp = false;
                return;
            }
            CSVLifeSkillRank.Data cSVLifeSkillRankData = CSVLifeSkillRank.Instance.GetConfData(grade);
            uint maxlevel = cSVLifeSkillRankData.level_max;
            foreach (var item in livingSkills)
            {
                if (item.Value.Level == maxlevel)
                {
                    CanGradeLevelUp = true;
                    return;
                }
            }
            CanGradeLevelUp = false;
        }

        public bool IsSkillFormulaUnlock(uint formulaId)
        {
            return openFormulas.Contains(formulaId);
        }

        public void UpdateCurSKillId(uint skillId)
        {
            SkillId = skillId;
        }

        public uint GetLifeSkillLevel(uint skillId)
        {
            return livingSkills[skillId].Level;
        }


        public void SkipToItem(uint formula, Action onGuide)
        {
            eventEmitter.Trigger<uint>(EEvents.OnSkipToFormula, formula);
            m_GuideTimer?.Cancel();
            m_GuideTimer = Timer.Register(1f, onGuide);
        }

        public bool HasLivingSkillLevelUp()
        {
            foreach (var item in livingSkills)
            {
                LivingSkill livingSkill = item.Value;
                if (livingSkill.Unlock && livingSkill.bExpFull && Sys_Role.Instance.Role.Level >= livingSkill.cSVLifeSkillLevelData.role_level)
                {
                    bool itemEnough = true;
                    if (livingSkill.cSVLifeSkillLevelData.cost_item == null)
                    {
                        itemEnough = false;
                    }
                    else
                    {
                        for (int i = 0; i < livingSkill.cSVLifeSkillLevelData.cost_item.Count; ++i)
                        {
                            uint itemId = livingSkill.cSVLifeSkillLevelData.cost_item[0][0];
                            uint needCount = livingSkill.cSVLifeSkillLevelData.cost_item[0][1];
                            if (Sys_Bag.Instance.GetItemCount(itemId) < needCount)
                            {
                                itemEnough = false;
                                break;
                            }
                        }
                    }
                    return itemEnough;
                }
            }
            return false;
        }

        public void SkipToLivingSkillForLevelUp()
        {
            bool can = false;
            LivingSkill livingSkill = null;
            foreach (var item in livingSkills)
            {
                LivingSkill lv = item.Value;
                if (lv.Unlock && lv.bExpFull)
                {
                    bool enough = true;
                    for (int i = 0; i < lv.cSVLifeSkillLevelData.cost_item.Count; ++i)
                    {
                        uint itemId = lv.cSVLifeSkillLevelData.cost_item[0][0];
                        uint needCount = lv.cSVLifeSkillLevelData.cost_item[0][1];
                        if (Sys_Bag.Instance.GetItemCount(itemId) < needCount)
                        {
                            enough = false;
                            break;
                        }
                    }
                    if (enough)
                    {
                        livingSkill = lv;
                        can = true;
                        break;
                    }
                }
            }
            if (!can)
            {
                return;
            }
            LifeSkillOpenParm lifeSkillOpenParm = new LifeSkillOpenParm();
            lifeSkillOpenParm.skillId = livingSkill.SkillId;
            lifeSkillOpenParm.itemId = 0;
            UIManager.OpenUI(EUIID.UI_LifeSkill_Message, false, lifeSkillOpenParm);
            //UIManager.OpenUI(EUIID.UI_LifeSkill_Message, false, livingSkill.SkillId);
        }

        public uint GetMaxSkillLifeLevel()
        {
            uint maxLevel = 0;
            foreach (var item in livingSkills)
            {
                maxLevel = (uint)Mathf.Max((int)maxLevel, (int)item.Value.Level);
            }
            return maxLevel;
        }

        public uint GetLearnedSkillLifeNum()
        {
            uint num = 0;
            foreach (var item in livingSkills)
            {
                if (item.Value.category == 2)
                {
                    continue;
                }
                if (item.Value.Level > 0)
                {
                    num++;
                }
            }
            return num;
        }


        public int GetSelectFormula(uint skillId, uint skillLevel)
        {
            uint calId = Sys_Role.Instance.Role.Career * 1000 + skillId * 100 + skillLevel;
            CSVFormulaSelect.Data cSVFormulaSelectData = CSVFormulaSelect.Instance.GetConfData(calId);
            if (cSVFormulaSelectData == null)
            {
                return -1;
            }
            else
            {
                return (int)cSVFormulaSelectData.id_formula;
            }
        }
    }
}


