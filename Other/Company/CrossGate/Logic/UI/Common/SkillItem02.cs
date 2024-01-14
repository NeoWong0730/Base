using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using System;

namespace Logic
{
    public class SkillItem02
    {
        public Transform transform;

        public Toggle toggle;
        public Image imgSelect;
        public Image imgIcon;

        private uint mSkillId;
        private Action<uint> mAction;

        public void Bind(GameObject go)
        {
            transform = go.transform;

            toggle = transform.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnClickSkill);
            //btnNormal = transform.GetComponent<Button>();
            //btnNormal.onClick.AddListener(OnClickSkill);
            imgSelect = transform.Find("Image_Select").GetComponent<Image>();
            imgIcon = transform.Find("Image_Icon").GetComponent<Image>();
        }

        private void OnClickSkill(bool click)
        {
            mAction?.Invoke(mSkillId);
        }

        public void SetData(uint skillId)
        {
            mSkillId = skillId;
            if (Sys_Skill.Instance.IsActiveSkill(mSkillId)) //主动技能
            {
                CSVActiveSkillInfo.Data cSVActiveSkillBehaviorData = CSVActiveSkillInfo.Instance.GetConfData(mSkillId);
                if (cSVActiveSkillBehaviorData == null)
                {
                    Debug.LogErrorFormat($"没有配置对应技能");
                    return;
                }
                uint icon = cSVActiveSkillBehaviorData.icon;
                if (icon == 0)
                {
                    Debug.LogErrorFormat($"没有配置对应icon");
                    return;
                }
                ImageHelper.SetIcon(imgIcon, icon);
            }
            else
            {
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(mSkillId);

                if (skillInfo == null)
                {
                    Debug.LogErrorFormat($"没有配置对应技能");
                    return;
                }
                uint icon = skillInfo.icon;
                if (icon == 0)
                {
                    Debug.LogErrorFormat($"没有配置对应icon");
                    return;
                }

                ImageHelper.SetIcon(imgIcon, skillInfo.icon);
            }
        }

        public void RegisterAction(Action<uint> action)
        {
            mAction = action;
        }
    }
}

