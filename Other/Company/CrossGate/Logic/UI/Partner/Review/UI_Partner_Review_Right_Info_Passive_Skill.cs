using System.Collections.Generic;
using UnityEngine;
using Table;
using Lib.Core;
using Packet;

namespace Logic
{
    public class UI_Partner_Review_Right_Info_Passive_Skill
    {
        private class SkillTemp
        {
            public uint partnerId;
            public ERuneType runeType;
            public uint skillId;
            public uint slotId;
        }

        private Transform transform;

        private List<SkillItem01> listSkillItems = new List<SkillItem01>();

        private List<uint> skillIds = new List<uint>();
        //private uint partnerId;

        public void Init(Transform trans)
        {
            transform = trans;

            int count = transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                SkillItem01 skill = new SkillItem01();
                skill.Bind(transform.GetChild(i).gameObject);
                //skill.RegisterLongPress(OnLongPressSkill);

                listSkillItems.Add(skill);
            }
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void UpdateInfo(uint infoId)
        {
            //partnerId = infoId;

            for (int i = 0; i < listSkillItems.Count; ++i)
                listSkillItems[i].transform.gameObject.SetActive(false);

            skillIds.Clear();

            //等级被动技能
            uint level = 1; //默认1级
            Partner partner = Sys_Partner.Instance.GetPartnerInfo(infoId);
            if (partner != null)
                level = partner.Level;

            CSVPartnerLevel.Data leveData = CSVPartnerLevel.Instance.GetUniqData(infoId, level);
            if (leveData != null && leveData.Passive_Skill != null)
            {
                skillIds.AddRange(leveData.Passive_Skill);
            }

            int index = 0;
            for (int i = 0; i < skillIds.Count; ++i)
            {
                listSkillItems[index].transform.gameObject.SetActive(true);
                listSkillItems[index].SetData(skillIds[i]);
                listSkillItems[index].OnEnableBtn(true);

                index++;
            }

            //伙伴技能表,战斗线，生活线，被动技能
            List<SkillTemp> listSkills = new List<SkillTemp>();
            CSVPartnerSkill.Data skillData = CSVPartnerSkill.Instance.GetConfData(infoId);
            if(skillData != null)
            {
                //战斗线被动技能
                if (skillData.Battle_PassiveSkill != null)
                {
                    for (int i = 0; i < skillData.Battle_PassiveSkill.Count; ++i)
                    {
                        SkillTemp skill = new SkillTemp();
                        skill.partnerId = infoId;
                        skill.runeType = ERuneType.Battle;
                        skill.skillId = skillData.Battle_PassiveSkill[i][0];
                        skill.slotId = skillData.Battle_PassiveSkill[i][2];

                        listSkills.Add(skill);
                    }
                }

                //生活线被动技能
                if (skillData.Overall_PassiveSkill != null)
                {
                    for (int i = 0; i < skillData.Overall_PassiveSkill.Count; ++i)
                    {
                        SkillTemp skill = new SkillTemp();
                        skill.partnerId = infoId;
                        skill.runeType = ERuneType.Life;
                        skill.skillId = skillData.Overall_PassiveSkill[i][0];
                        skill.slotId = skillData.Overall_PassiveSkill[i][2];

                        listSkills.Add(skill);
                    }
                }
            }

            for(int i = 0; i < listSkills.Count; ++i)
            {
                SkillTemp skill = listSkills[i];
                listSkillItems[index].transform.gameObject.SetActive(true);
                listSkillItems[index].SetData(skill.skillId);
                
                bool isActive = Sys_Partner.Instance.IsSkillActive(skill.partnerId, skill.runeType, (int)skill.slotId); 
                listSkillItems[index].OnEnableBtn(isActive);
                ImageHelper.SetImageGray(listSkillItems[i].transform, !isActive, true);

                index++;
            }

            //金符文技能
            skillIds.Clear();
            skillIds = Sys_Partner.Instance.GetPartnerRuneSkillByPartnerId(infoId);
            for (int i = 0;  i < skillIds.Count; ++i)
            {
                listSkillItems[index].transform.gameObject.SetActive(true);
                listSkillItems[index].SetData(skillIds[i]);
                listSkillItems[index].OnEnableBtn(true);
                index++;
            }
        }
    }
}


