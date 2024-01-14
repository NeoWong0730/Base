using Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;

namespace Logic
{
    public class LifeSkillExpChangeEvt
    {
        public uint skillId;
        public float cur;
        public float max;
    }

    public class LivingSkill
    {
        public uint category { get; private set; }     //1:制造系   2:采集系

        public LifeSkillType LifeSkillType { get; private set; }

        public uint SkillId { get { return (uint)LifeSkillType; } }

        public CSVLifeSkill.Data cSVLifeSkillData { get; private set; }

        public CSVLifeSkillLevel.Data cSVLifeSkillLevelData { get; private set; }

        public string name { get; private set; }

        private uint _level;
        public uint Level
        {
            get { return _level; }
            set
            {
                if (_level != value)
                {
                    _level = value;
                    Proficiency = 0;
                    cSVLifeSkillLevelData = Sys_LivingSkill.Instance.GetLifeSkillLevelData(cSVLifeSkillData.id, _level);
                    UpdateMaxExp();
                    Sys_LivingSkill.Instance.UpdateGradeCanLock();
                    Sys_LivingSkill.Instance.eventEmitter.Trigger<uint, bool>(Sys_LivingSkill.EEvents.OnUpdateLevelUpButtonState, SkillId, false);
                    Sys_LivingSkill.Instance.eventEmitter.Trigger<uint>(Sys_LivingSkill.EEvents.OnLevelUp, SkillId);
                    LifeSkillExpChangeEvt lifeSkillExpChangeEvt = new LifeSkillExpChangeEvt();
                    lifeSkillExpChangeEvt.skillId = SkillId;
                    lifeSkillExpChangeEvt.cur = (float)_proficiency;
                    lifeSkillExpChangeEvt.max = (float)_MaxProficiency;
                    Sys_LivingSkill.Instance.eventEmitter.Trigger<LifeSkillExpChangeEvt>(Sys_LivingSkill.EEvents.OnUpdateExp, lifeSkillExpChangeEvt);
                    Sys_LivingSkill.Instance.eventEmitter.Trigger(Sys_LivingSkill.EEvents.OnUpdateGrade);
                }
            }
        }

        public uint MaxLevel
        {
            get
            {
                return CSVLifeSkillRank.Instance.GetConfData(Sys_LivingSkill.Instance.Grade).level_max;
            }
        }

        public string LevelState
        {
            get
            {
                return string.Format($"{Level.ToString()}/{MaxLevel.ToString()}");
            }
        }

        private uint _proficiency;
        public uint Proficiency
        {
            get { return _proficiency; }
            set
            {
                if (_proficiency != value)
                {
                    _proficiency = value;
                    //更新经验条  是否升级ui

                    LifeSkillExpChangeEvt lifeSkillExpChangeEvt = new LifeSkillExpChangeEvt();
                    lifeSkillExpChangeEvt.skillId = SkillId;
                    lifeSkillExpChangeEvt.cur = (float)_proficiency;
                    lifeSkillExpChangeEvt.max = (float)_MaxProficiency;
                    Sys_LivingSkill.Instance.eventEmitter.Trigger<LifeSkillExpChangeEvt>(Sys_LivingSkill.EEvents.OnUpdateExp, lifeSkillExpChangeEvt);

                    if (bExpFull)
                    {
                        Sys_LivingSkill.Instance.eventEmitter.Trigger<uint, bool>(Sys_LivingSkill.EEvents.OnUpdateLevelUpButtonState, SkillId, true);
                    }
                }
            }
        }

        public uint _MaxProficiency;

        public bool Unlock
        {
            get { return _level > 0; }
        }

        public bool bExpFull
        {
            get
            {
                return _proficiency >= _MaxProficiency;
            }
        }

        public bool bMaxLevel { get { return _level == cSVLifeSkillData.max_level; } }

        private uint _LuckyValue;

        public uint luckyValue
        {
            get
            {
                return _LuckyValue;
            }
            set
            {
                if (_LuckyValue != value)
                {
                    _LuckyValue = value;
                    Sys_LivingSkill.Instance.eventEmitter.Trigger<uint, uint>(Sys_LivingSkill.EEvents.OnUpdateLuckyValue, SkillId, _LuckyValue);
                }
            }
        }

        public uint orangeCount;

        public bool b_LuckyFull
        {
            get
            {
                return _LuckyValue >= cSVLifeSkillData.lucky__value_max;
            }
        }

        public void SetSkillType(LifeSkillType lifeSkillType)
        {
            this.LifeSkillType = lifeSkillType;
            cSVLifeSkillData = CSVLifeSkill.Instance.GetConfData((uint)LifeSkillType);
            category = cSVLifeSkillData.type;
            cSVLifeSkillLevelData = Sys_LivingSkill.Instance.GetLifeSkillLevelData(cSVLifeSkillData.id, _level);
            name = LanguageHelper.GetTextContent(cSVLifeSkillData.name_id);
        }

        private void UpdateMaxExp()
        {
            _MaxProficiency = cSVLifeSkillLevelData.proficiency;
        }

        public void AddProficiency(uint val)
        {
            uint dest = _proficiency + val;
            Proficiency = dest;
        }
    }

}


