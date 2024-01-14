using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;

namespace Logic
{
    public class UI_Partner_Get_SkillInfo : UIComponent
    {
        private InfinityGridLayoutGroup gridGroup;
        private Dictionary<GameObject, SkillItem02> dicSkills = new Dictionary<GameObject, SkillItem02>();
        private int visualGridCount;

        private List<uint> skillIds = new List<uint>();
        private uint partnerId;        

        protected override void Loaded()
        {
            base.Loaded();

            gridGroup = transform.Find("Grid").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 6;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                GameObject go = gridGroup.transform.GetChild(i).gameObject;

                SkillItem02 skill = new SkillItem02();
                skill.Bind(go);
                skill.RegisterAction(OnClickSkill);
                dicSkills.Add(go, skill);
            }
        }

        private void OnClickSkill(uint skillId)
        {
            UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicSkills.ContainsKey(trans.gameObject))
            {
                SkillItem02 cell = dicSkills[trans.gameObject];
                cell.SetData(skillIds[index]);
            }
        }

        public void UpdateInfo(uint infoId)
        {
            partnerId = infoId;
            skillIds.Clear();

            CSVPartnerSkill.Data skillData = CSVPartnerSkill.Instance.GetConfData(infoId);
            if (skillData != null)
            {
                skillIds.AddRange(skillData.Active_Skill);
            }

            visualGridCount = skillIds.Count;
            gridGroup.SetAmount(visualGridCount);
        }
    }
}


