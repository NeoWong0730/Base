using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Field_Upgrade_SkillPreview : UIBase
    {
        private class Cell
        {
            public class SkillItem
            {
                private Transform transform;
                private Image imgIcon;
                private uint _skillId;
                public void Init(Transform trans)
                {
                    transform = trans;
                    imgIcon = transform.Find("Icon").GetComponent<Image>();
                    Lib.Core.EventTrigger.AddEventListener(imgIcon.gameObject, EventTriggerType.PointerClick, (eventData) =>
                    {
                        UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(_skillId, 0));
                    });
                }

                public void UpdateSkill(uint skillId)
                {
                    _skillId = skillId;
                    CSVPassiveSkillInfo.Data data = CSVPassiveSkillInfo.Instance.GetConfData(_skillId);
                    ImageHelper.SetIcon(imgIcon, data.icon);
                }
            }
        
            private Transform transform;
            private Text txtSkillName;
            private Text txtSkillock;
            private Transform transSkillTemplate;

            public void Init(Transform trans)
            {
                transform = trans;

                txtSkillName = transform.Find("Title/Text").GetComponent<Text>();
                txtSkillock = transform.Find("Title/Text1").GetComponent<Text>();
                
                transSkillTemplate = transform.Find("SkillGroup/Image1");
                transSkillTemplate.gameObject.SetActive(false);
            }

            public void UpdateInfo(uint groupId, int index)
            {
                txtSkillName.text = LanguageHelper.GetTextContent(4250 + (uint)index);
                txtSkillock.text = "";
                FrameworkTool.DestroyChildren(transSkillTemplate.parent.gameObject, transSkillTemplate.name);
                List<uint> skills = new List<uint>();
                skills.AddRange(Sys_Equip.Instance.GetEffects(groupId));
                for (int i = 0; i < skills.Count; ++i)
                {
                    GameObject go = GameObject.Instantiate(transSkillTemplate.gameObject, transSkillTemplate.parent);
                    go.SetActive(true);
                    SkillItem skill = new SkillItem();
                    skill.Init(go.transform);
                    skill.UpdateSkill(skills[i]);
                }
                
                FrameworkTool.ForceRebuildLayout(transSkillTemplate.parent.gameObject);
            }
        }

        private Transform transCellTemplate;

        protected override void OnLoaded()
        {
            Lib.Core.EventTrigger.AddEventListener(transform.Find("Blank").gameObject, EventTriggerType.PointerClick, (eventData) =>
            {
                this.CloseSelf();
            });

            transCellTemplate = transform.Find("Scroll View/Viewport/Content/Item");
            transCellTemplate.gameObject.SetActive(false);
        }

        protected override void OnOpen(object arg)
        {

        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
 
        }

        protected override void OnClose()
        {
            
        }

        private void UpdateInfo()
        {
            FrameworkTool.DestroyChildren(transCellTemplate.parent.gameObject, transCellTemplate.name);
            List<uint> listIds = ReadHelper.ReadArray_ReadUInt(CSVParam.Instance.GetConfData(246).str_value, '|');
            for (int i = 0; i < listIds.Count; ++i)
            {
                GameObject go = GameObject.Instantiate(transCellTemplate.gameObject, transCellTemplate.parent);
                go.SetActive(true);
                Cell cell = new Cell();
                cell.Init(go.transform);
                cell.UpdateInfo(listIds[i], i);
            }
            
            FrameworkTool.ForceRebuildLayout(transCellTemplate.parent.gameObject);
        }
    }
}

