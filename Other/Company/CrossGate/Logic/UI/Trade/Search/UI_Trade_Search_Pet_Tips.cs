using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Search_Pet_Tips : UIBase
    {
        private class AttrCell
        {
            public Transform transform;

            private Text _textName;
            private Text _textValue;

            public void Init(Transform trans)
            {
                transform = trans;

                _textName = transform.Find("Text_Property").GetComponent<Text>();
                _textValue = transform.Find("Text_Num").GetComponent<Text>();
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            public void SetData(uint nameId, string strValue)
            {
                _textName.text = LanguageHelper.GetTextContent(nameId);
                _textValue.text = strValue;
            }

        }

        private class SkillCell
        {
            public Transform transform;

            private PetSkillItem02 petskill;
            public void Init(Transform trans)
            {
                transform = trans;

                petskill = new PetSkillItem02();
                petskill.Bind(transform.gameObject);
            }

            public void SetSkill(uint skillId, bool isUniq, bool isRemake)
            {
                petskill.SetDate(skillId);
                petskill.skillImage.gameObject.SetActive(true);

                petskill.uniqueGo.SetActive(isUniq);
                petskill.buildGo.SetActive(isRemake);
                petskill.mountGo.SetActive(false);
            }
        }

        private class SkillId
        {
            public uint skillId;
            public bool uniq = false;
            public bool remake = false;
        }

        private Text _textTitle;
        private Text _textSkillTitle;
        private AttrCell[] attrArray = new AttrCell[6];
        private List<SkillCell> skillList = new List<SkillCell>(8);

        private uint _petInfoId;
        private CSVPetNew.Data _petData;
        protected override void OnOpen(object arg)
        {
            _petInfoId = 0u;
            if (arg != null)
                _petInfoId = (uint)arg;

            _petData = CSVPetNew.Instance.GetConfData(_petInfoId);
        }

        protected override void OnLoaded()
        {
            _textTitle = transform.Find("ImageBG/Image_Title/Text_Title").GetComponent<Text>();
            
            //attr
            for (int i = 0; i < 3; ++i)
            {
                int index = i * 2;

                attrArray[index] = new AttrCell();
                attrArray[index].Init(transform.Find(string.Format("ImageBG/Attr{0}/Left", i)));

                attrArray[index + 1] = new AttrCell();
                attrArray[index + 1].Init(transform.Find(string.Format("ImageBG/Attr{0}/Right", i)));
            }

            //skill
            _textSkillTitle = transform.Find("ImageBG/Image_Skill/Text_Title").GetComponent<Text>();
            Transform parent = transform.Find("ImageBG/Image_Skill/Grid");
            for (int i = 0; i < parent.childCount; ++i)
            {
                SkillCell cell = new SkillCell();
                cell.Init(parent.GetChild(i));

                skillList.Add(cell);
            }

            Button btnClose = transform.Find("Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void OnClickClose()
        {
            this.CloseSelf();
        }

        private void UpdateInfo()
        {
            if (_petData != null)
            {
                _textTitle.text = LanguageHelper.GetTextContent(_petData.name);
                _textSkillTitle.text = LanguageHelper.GetTextContent(2011048u);
                //vitGrade = 2011135, //体力 /10000
                //snhGrade = 2011136, //力量 /10000
                //intenGrade = 2011137, //强度 /10000
                //speedGrade = 2011138, //速度 /10000
                //magicGrade = 2011139, //魔法 /10000
                //growthGrade = 2011140, //成长 /1000
                attrArray[0].SetData((uint)Sys_Trade.EPetQuality.vitGrade, _petData.endurance.ToString());
                attrArray[1].SetData((uint)Sys_Trade.EPetQuality.snhGrade, _petData.strength.ToString());
                attrArray[2].SetData((uint)Sys_Trade.EPetQuality.intenGrade, _petData.strong.ToString());
                attrArray[3].SetData((uint)Sys_Trade.EPetQuality.speedGrade, _petData.speed.ToString());
                attrArray[4].SetData((uint)Sys_Trade.EPetQuality.magicGrade, _petData.magic.ToString());
                //attrArray[5].SetData((uint)Sys_Trade.EPetQuality.growthGrade, string.Format("{0}-{1}", (_petData.growth_value[0] / 1000f).ToString("F2"), (_petData.growth_value[1] / 1000f).ToString("F2")));
                attrArray[5].Hide();

                List<SkillId> skillIds = new List<SkillId>(8);

                //初始技能
                if (_petData.required_skills != null)
                {
                    for (int i = 0; i < _petData.required_skills.Count; ++i)
                    {
                        SkillId skill = new SkillId();
                        skill.skillId = _petData.required_skills[i][0];

                        skillIds.Add(skill);
                    }
                }

                //专属技能
                if (_petData.unique_skills != null)
                {
                    for (int i = 0; i < _petData.unique_skills.Count; ++i)
                    {
                        SkillId skill = new SkillId();
                        skill.skillId = _petData.unique_skills[i][0];
                        skill.uniq = true;

                        skillIds.Add(skill);
                    }
                }

                //改造技能
                if (_petData.remake_skills != null)
                {
                    for (int i = 0; i < _petData.remake_skills.Count; ++i)
                    {
                        SkillId skill = new SkillId();
                        skill.skillId = _petData.remake_skills[i];
                        skill.remake = true;
                
                        skillIds.Add(skill);
                    }
                }


                for (int i = 0; i < skillList.Count; ++i)
                {
                    if (i < skillIds.Count)
                    {
                        skillList[i].transform.gameObject.SetActive(true);
                        skillList[i].SetSkill(skillIds[i].skillId, skillIds[i].uniq, skillIds[i].remake);
                    }
                    else
                    {
                        skillList[i].transform.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}


