using UnityEngine;
using UnityEngine.UI;
using System;
using Table;
using System.Collections.Generic;

namespace Logic
{
    public class UI_Pet_Seal_PetCeil 
    {
        public uint curPetId;
        public Transform transform;
        private Text petName;
        private Text petFightLevel;
        private Text gradeNum;
        private Text sliverCardNum;
        private Text goldCardNum;
        private Toggle sealToggle;
        private Button petIconBtn;
        private Image petIcon;

        private Action<uint> action;
        private CSVPetNew.Data curPet;
        private List<uint> sealItems = new List<uint>();

        public void Init(Transform _transform)
        {
            transform = _transform;

            petName = transform.Find("Text_Name").GetComponent<Text>();
            petFightLevel = transform.Find("Text_Level/Value").GetComponent<Text>();
            gradeNum = transform.Find("Text_Gears/Value").GetComponent<Text>();
            sliverCardNum = transform.Find("Text_SilverCard/Value").GetComponent<Text>();
            goldCardNum = transform.Find("Text_GoldCard/Value").GetComponent<Text>();
            petIcon = transform.Find("Image_head/Icon").GetComponent<Image>();
            petIconBtn = transform.Find("Image_head/Icon").GetComponent<Button>();
            petIconBtn.onClick.AddListener(OnPetClicked);
            sealToggle = transform.Find("Toggle").GetComponent<Toggle>();
            sealToggle.onValueChanged.AddListener(OnSealToggleValueChanged);
        }

        private void OnSealToggleValueChanged(bool isOn)
        {
            Sys_Pet.Instance.PetCatchSetReq(curPetId, !isOn);
        }

        private void OnPetClicked()
        {
            action?.Invoke(curPetId);
        }

        public void SetData(uint id)
        {
            curPetId = id;
            curPet = CSVPetNew.Instance.GetConfData(curPetId);
            RefreshView();
        }

        public void RefreshView()
        {
            if (curPet == null)
            {
                return;
            }
            ImageHelper.SetIcon(petIcon, curPet.icon_id);
            petName.text = LanguageHelper.GetTextContent(curPet.name);
            if (Sys_Role.Instance.Role.Level < curPet.participation_lv)
            {
                petFightLevel.text = "<color=red>"+curPet.participation_lv.ToString()+"</color>";
            }
            else
            {
                petFightLevel.text =  curPet.participation_lv.ToString();
            }
            bool flag = false;
            for(int i = 0; i < Sys_Pet.Instance.petSetList.Count; ++i)
            {
                if (Sys_Pet.Instance.petSetList[i].PetId == curPetId)
                {
                    sealToggle.SetIsOnWithoutNotify(!Sys_Pet.Instance.petSetList[i].AutoCatch);
                    flag = true;
                    break;
                }
            }
            if(!flag)
            {
                sealToggle.SetIsOnWithoutNotify(true);
            }
            SetGradeInfo();
            SetSealCard();
        }

        private void SetGradeInfo()
        {
            int maxgrade = 0;
            if (null != curPet)
            {
                maxgrade = curPet.endurance + curPet.strength + curPet.strong + curPet.speed + curPet.magic;
                gradeNum.text = (maxgrade - curPet.max_lost_gear).ToString() + "/" + maxgrade.ToString(); 
            }
        }

        private void SetSealCard()
        {
            string itemids = CSVParam.Instance.GetConfData(610).str_value;
            string[] ids = itemids.Split('|');
            for (int i = 0; i < ids.Length; i++)
            {
                sealItems.Add(uint.Parse(ids[i]));
            }
            CSVPetNewSeal.Instance.TryGetValue(curPetId, out CSVPetNewSeal.Data cSVPetSealData);
            if (null != cSVPetSealData)
            {
                uint captureData = Sys_Adventure.Instance.GetCaptureProbability();
                uint qyData = Sys_OperationalActivity.Instance.GetSpecialCardCaptureProbability();
                bool isHasCaptureOrQy = captureData != 0 || qyData != 0;
                CSVWordStyle.Data cSVWordStyleDataCa = null;
                cSVWordStyleDataCa = isHasCaptureOrQy ? CSVWordStyle.Instance.GetConfData(74u) : CSVWordStyle.Instance.GetConfData(152u);
                TextHelper.SetText(sliverCardNum, GetSealP(sealItems[1], cSVPetSealData, captureData + qyData).ToString() + "%", cSVWordStyleDataCa);
                TextHelper.SetText(goldCardNum, GetSealP(sealItems[2], cSVPetSealData, captureData + qyData).ToString() + "%" , cSVWordStyleDataCa);
            }
        }

        private float GetSealP(uint itemId, CSVPetNewSeal.Data petSealData, uint otherValue)
        {
            float pn = 10000;
            CSVItem.Data item = CSVItem.Instance.GetConfData(itemId);
            if (null != item)
            {
                CSVActiveSkill.Data skii = CSVActiveSkill.Instance.GetConfData(item.active_skillid);
                if (null != skii)
                {
                    if (null != skii.skill_effect_id)
                    {
                        int count = skii.skill_effect_id.Count;
                        for (int i = 0; i < count; i++)
                        {
                            CSVActiveSkillEffective.Data skillEff = CSVActiveSkillEffective.Instance.GetConfData(skii.skill_effect_id[i]);
                            if (null != skillEff)
                            {
                                pn += (skillEff.effect_to_target + 0f);
                            }
                        }
                    }
                }
            }
            pn += otherValue;
            if (null != petSealData)
            {
                pn -= (petSealData.seal_difficulty + 0f);
            }
            return Math.Min((pn / 100.0f), 100f);
        }

        public void Register(Action<uint> action)
        {
            this.action = action;
        }
    }
}
