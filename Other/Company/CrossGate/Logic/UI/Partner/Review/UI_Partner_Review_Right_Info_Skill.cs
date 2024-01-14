using System.Collections.Generic;
using UnityEngine;
using Table;
using Lib.Core;
using Packet;

namespace Logic
{
    public class UI_Partner_Review_Right_Info_Skill
    {
        private class SkillTemp
        {
            public bool defenceSkill = false; //主动被动
            public bool enableBtn = false;
            public uint partnerId;
            public ERuneType runeType;
            public uint skillId;
            public uint slotId;
        }

        private class SkillCell
        {
            private Transform transform;
            private SkillItem01 skillItem;
            private Transform transLockRune;

            private uint partnerId;

            public void Init(Transform trans)
            {
                transform = trans;

                skillItem = new SkillItem01();
                skillItem.Bind(transform.gameObject);

                transLockRune = transform.Find("Lock_Rune");
            }

            public void UpadteInfo(SkillTemp temp)
            {
                partnerId = temp.partnerId;
                skillItem.SetData(temp.skillId);
                transLockRune.gameObject.SetActive(false);
                ImageHelper.SetImageGray(skillItem.transform, false, true);
                if (temp.defenceSkill)
                {
                    if (temp.enableBtn)
                    {
                        skillItem.OnEnableBtn(true);
                    }
                    else
                    {
                        bool isActive = Sys_Partner.Instance.IsSkillActive(temp.partnerId, temp.runeType, (int)temp.slotId);
                        skillItem.OnEnableBtn(isActive);
                        ImageHelper.SetImageGray(skillItem.transform, !isActive, true);
                        transLockRune.gameObject.SetActive(!isActive);
                    }

                }
                else
                {
                    skillItem.RegisterLongPress(OnLongPressSkill);
                }
            }

            private void OnLongPressSkill(uint skillId)
            {
                if (!CombatManager.Instance.m_IsFight)
                    SkillPreView.Instance.ShowSkillPreViewForParnter(skillId, partnerId);
            }
        }


        private Transform transform;

        private InfinityGrid _infinityGrid;
        private List<SkillTemp> listSkills = new List<SkillTemp>();

        private uint partnerId;
        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnChangeCell;
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            SkillCell skillItem = new SkillCell();
            skillItem.Init(cell.mRootTransform);
            cell.BindUserData(skillItem);
        }

        private void OnChangeCell(InfinityGridCell cell, int index)
        {
            SkillCell skillCell = cell.mUserData as SkillCell;
            skillCell.UpadteInfo(listSkills[index]);
        }

        public void UpdateInfo(uint infoId)
        {
            partnerId = infoId;
            listSkills.Clear();
            Partner partner = Sys_Partner.Instance.GetPartnerInfo(infoId);
            uint level = 1;
            if (partner != null)
                level = partner.Level;

            //主动技能
            CSVPartnerLevel.Data leveData = CSVPartnerLevel.Instance.GetUniqData(infoId, level);
            if (leveData != null)
            {
                if (leveData.Active_Skill != null)
                {
                    for (int i = 0; i < leveData.Active_Skill.Count; ++i)
                    {
                        SkillTemp temp = new SkillTemp();
                        temp.defenceSkill = false;
                        temp.enableBtn = false;
                        temp.partnerId = infoId;
                        temp.skillId = leveData.Active_Skill[i];
                        temp.slotId = 0;

                        listSkills.Add(temp);
                    }
                }

               if (leveData.Passive_Skill != null)
               {
                    for (int i = 0; i < leveData.Passive_Skill.Count; ++i)
                    {
                        SkillTemp temp = new SkillTemp();
                        temp.defenceSkill = true;
                        temp.enableBtn = true;
                        temp.partnerId = infoId;
                        temp.skillId = leveData.Passive_Skill[i];
                        temp.slotId = 0;

                        listSkills.Add(temp);
                    }
               }
            }
            
            //伙伴加点解锁被动技能
            List<uint> propertySkills = Sys_Partner.Instance.GetPropertySkills(infoId);
            for (int i = 0; i < propertySkills.Count; ++i)
            {
                SkillTemp temp = new SkillTemp();
                temp.defenceSkill = true;
                temp.enableBtn = true;
                temp.partnerId = infoId;
                temp.skillId = propertySkills[i];
                temp.slotId = 0;
                
                listSkills.Add(temp);
            }

            //伙伴技能表,战斗线，生活线，被动技能
            CSVPartnerSkill.Data skillData = CSVPartnerSkill.Instance.GetConfData(infoId);
            if (skillData != null)
            {
                //战斗线被动技能
                if (skillData.Battle_PassiveSkill != null)
                {
                    for (int i = 0; i < skillData.Battle_PassiveSkill.Count; ++i)
                    {
                        SkillTemp skill = new SkillTemp();
                        skill.defenceSkill = true;
                        skill.enableBtn = false;
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
                        skill.defenceSkill = true;
                        skill.enableBtn = false;
                        skill.partnerId = infoId;
                        skill.runeType = ERuneType.Life;
                        skill.skillId = skillData.Overall_PassiveSkill[i][0];
                        skill.slotId = skillData.Overall_PassiveSkill[i][2];

                        listSkills.Add(skill);
                    }
                }
            }

            //金符文技能
            List<uint> skillIds = Sys_Partner.Instance.GetPartnerRuneSkillByPartnerId(infoId);
            for (int i = 0; i < skillIds.Count; ++i)
            {
                SkillTemp skill = new SkillTemp();
                skill.defenceSkill = true;
                skill.enableBtn = true;
                skill.partnerId = infoId;
                skill.runeType = ERuneType.Life;
                skill.skillId = skillIds[i];

                listSkills.Add(skill);
            }

            _infinityGrid.CellCount = listSkills.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
    }
}


