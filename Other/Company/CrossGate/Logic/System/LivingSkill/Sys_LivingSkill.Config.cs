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
        //技能未解锁时 获取展示配方列表
        public List<uint> GetLockedSkillFormulas(uint skillType)
        {
            List<CSVFormula.Data>  res = new List<CSVFormula.Data>();
            List<uint> _res = new List<uint>();
            var dics = CSVFormula.Instance.GetAll();
            foreach (var item in dics)
            {
                CSVFormula.Data cSVFormulaData = item;
                if (cSVFormulaData.type == skillType)
                {
                    res.Add(cSVFormulaData);
                }
            }
            res.Sort((s1, s2) =>
            {
                return s1.level_formula.CompareTo(s2.level_formula);
            });
            foreach (var item in res)
            {
                _res.Add(item.id);
            }
            return _res;
        }

        //技能未解锁时，获取展示道具列表
        public List<uint> GetLockedSkillItems(uint skillType)
        {
            List<uint> res = new List<uint>();
            var dics = CSVLifeSkillLevel.Instance.GetAll();
            foreach (var item in dics)
            {
                CSVLifeSkillLevel.Data cSVLifeSkillLevelData = item;
                if (cSVLifeSkillLevelData.skill_id == skillType && cSVLifeSkillLevelData.level > 0)
                {
                    res.AddRange(cSVLifeSkillLevelData.collection_item_id);
                }
            }
            return res;
        }

        //技能解锁时，获取展示道具列表
        public List<uint> GetUnLockSkillItems(uint skillType, uint skilllevel)
        {
            List<uint> res = new List<uint>();
            var dics = CSVLifeSkillLevel.Instance.GetAll();
            foreach (var item in dics)
            {
                CSVLifeSkillLevel.Data cSVLifeSkillLevelData = item;
                if (cSVLifeSkillLevelData.skill_id == skillType && cSVLifeSkillLevelData.level == skilllevel)
                {
                    if (cSVLifeSkillLevelData.collection_item_id == null)
                    {
                        continue;
                    }
                    res.AddRange(cSVLifeSkillLevelData.collection_item_id);
                }
            }
            return res;
        }

        //技能解锁时，获取展示配方列表
        public List<uint> GetUnlockSkillFormulas(uint skillType, uint skilllevel)
        {
            List<uint> res = new List<uint>();
            var dics = CSVFormula.Instance.GetAll();
            foreach (var item in dics)
            {
                CSVFormula.Data cSVFormulaData = item;
                if (cSVFormulaData.type == skillType && cSVFormulaData.level_skill == skilllevel)
                {
                    res.Add(cSVFormulaData.id);
                }
            }
            return res;
        }

        public List<uint> GetUnlockSkillFormulas_NotEquip(uint skillType, uint skilllevel)
        {
            List<uint> res = new List<uint>();

            var formulas = CSVFormula.Instance.GetAll();
            for (int i = 0, len = formulas.Count; i < len; i++)
            {
                CSVFormula.Data cSVFormulaData = formulas[i];
                if (!cSVFormulaData.isequipment)
                {
                    continue;
                }
                if (cSVFormulaData.type == skillType && cSVFormulaData.level_skill == skilllevel)
                {
                    res.Add(cSVFormulaData.id);
                }
            }
            return res;
        }

        //等级变更获取解锁配方
        public List<uint> GetUnlockSkillFormulasByLevel(uint skillType, uint skilllevel)
        {
            List<uint> res = new List<uint>();
            var dics = CSVFormula.Instance.GetAll();
            foreach (var item in dics)
            {
                CSVFormula.Data cSVFormulaData = item;
                if (cSVFormulaData.unlock && cSVFormulaData.type == skillType && cSVFormulaData.level_skill <= skilllevel)
                {
                    res.Add(cSVFormulaData.id);
                }
            }
            return res;
        }

        public CSVLifeSkillLevel.Data GetLifeSkillLevelData(uint skillId, uint level)
        {
            foreach (var item in CSVLifeSkillLevel.Instance.GetAll())
            {
                CSVLifeSkillLevel.Data cSVLifeSkillLevelData = item;
                if (cSVLifeSkillLevelData.skill_id == skillId && cSVLifeSkillLevelData.level == level)
                {
                    return cSVLifeSkillLevelData;
                }
            }
            DebugUtil.LogErrorFormat($"没有找到技能类型为 {skillId} ,技能等级为 {level} 的技能");
            return null;
        }
    }
}


