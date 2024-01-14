
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Table;
using Packet;
using Google.Protobuf.Collections;
using Framework;

namespace Logic
{
    public class TipPetEquipInfoView : UIParseCommon
    {
        //basic
        public TipPetEquipInfoBasic basicInfo;

        //special
        public TipPetEquipInfoSpecial specialInfo;

        //suit
        public TipPetEquipInfoSuit suitInfo;

        protected override void Parse()
        {
            basicInfo = new TipPetEquipInfoBasic();
            basicInfo.Init(transform.Find("GameObject/View_Basic_Prop"));

            specialInfo = new TipPetEquipInfoSpecial();
            specialInfo.Init(transform.Find("GameObject/View_SpecicalEffects_Prop"));

            suitInfo = new TipPetEquipInfoSuit();
            suitInfo.Init(transform.Find("GameObject/View_Set_Prop"));
        }

        public override void UpdateInfo(ItemData item)
        {
            bool haveAttr = item.petEquip.BaseAttr.Count > 0;
            basicInfo.gameObject.SetActive(haveAttr);
            if (haveAttr)
                basicInfo.UpdateInfo(item, item.petEquip.BaseAttr);

            haveAttr = item.petEquip.EffectAttr.Count > 0;
            specialInfo.gameObject.SetActive(haveAttr);
            if (haveAttr)
                specialInfo.UpdateInfo(item, item.petEquip.EffectAttr);

            haveAttr = item.petEquip.SuitSkill > 0;
            suitInfo.gameObject.SetActive(haveAttr);
            if (haveAttr)
                suitInfo.UpdateInfo(item);
        }

        public override void OnDestroy()
        {
            basicInfo.OnDestroy();
        }
    }

    public class TipPetEquipInfoBase : UIParseCommon
    {
        protected string lineStr = "Image_Title_Splitline";

        protected class InfoAttrEntry
        {
            public GameObject root;
            public Text name;
            public Text value;
            public Text time;
            public Text diffValue;
            public GameObject upArrow;
            public GameObject downArrow;
            public uint attrId;
            public long attrValue;

            private Timer timer;
            private int endTime;
            public void StartTimer(int timeValue)
            {
                endTime = timeValue;
                timer?.Cancel();
                timer = null;
                time.text = LanguageHelper.GetTextContent(4089, CalLeftTime((uint)endTime));
                timer = Timer.Register(1f, () => {
                    time.text = LanguageHelper.GetTextContent(4089, CalLeftTime((uint)endTime));
                }, null, true);
            }

            public void StopTimer()
            {
                timer?.Cancel();
                timer = null;
            }

            private string CalLeftTime(uint endTime)
            {
                uint leftTime = 0u;
                if (endTime > Sys_Time.Instance.GetServerTime())
                    leftTime = endTime - Sys_Time.Instance.GetServerTime();

                return LanguageHelper.TimeToString(leftTime, LanguageHelper.TimeFormat.Type_1);
            }
        }

        public int CalEntryCount(int attrCount)
        {
            //int result = attrCount / 2;
            //int left = attrCount % 2;
            //return result + left;
            return (attrCount + 1) / 2;
        }

        public void DestroyChildren(GameObject parent, params string[] list)
        {
            List<string> dontList = new List<string>();
            foreach (string var in list)
            {
                dontList.Add(var);
            }
            parent.DestoryAllChildren(dontList, true);
        }
    }

    public class TipPetEquipInfoBasic : TipPetEquipInfoBase
    {
        private List<InfoAttrEntry> listEntry = new List<InfoAttrEntry>();

        private ItemData item;

        protected override void Parse()
        {
            for (int i = 0; i < 3; ++i)
            {
                GameObject attrGo = transform.Find(string.Format("View_Basis_Property{0}", i)).gameObject;

                InfoAttrEntry entry = new InfoAttrEntry();
                entry.root = attrGo.transform.Find("Basis_Property").gameObject;
                entry.name = entry.root.transform.Find("Basis_Property01").GetComponent<Text>();
                entry.value = entry.root.transform.Find("Number").GetComponent<Text>();
                entry.upArrow = entry.root.transform.Find("Image_UpArrow").gameObject;
                entry.downArrow = entry.root.transform.Find("Image_DownArrow").gameObject;

                listEntry.Add(entry);
            }

            Sys_Equip.Instance.eventEmitter.Handle<ItemData>(Sys_Equip.EEvents.OnCompareBasicAttrValue, OnCompareValue, true);
        }

        public override void OnDestroy()
        {
            Sys_Equip.Instance.eventEmitter.Handle<ItemData>(Sys_Equip.EEvents.OnCompareBasicAttrValue, OnCompareValue, false);
        }

        public void UpdateInfo(ItemData _item, RepeatedField<PetEquip.Types.BaseAttr> attrList)
        {
            this.item = _item;
            for (int i = 0; i < listEntry.Count; ++i)
            {
                if (i < attrList.Count)
                {
                    listEntry[i].root.transform.parent.gameObject.SetActive(true);

                    listEntry[i].root.SetActive(true);
                    listEntry[i].upArrow.SetActive(false);
                    listEntry[i].downArrow.SetActive(false);

                    listEntry[i].attrId = attrList[i].AttrId;
                    listEntry[i].attrValue = attrList[i].AttrValue;

                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrList[i].AttrId);
                    listEntry[i].name.text = LanguageHelper.GetTextContent(attrData.name);
                    listEntry[i].value.text = Sys_Attr.Instance.GetAttrValue(attrData, attrList[i].AttrValue);
                }
                else
                {
                    listEntry[i].root.transform.parent.gameObject.SetActive(false);
                }
            }
        }

        private void OnCompareValue(ItemData _item)
        {
            if (item == null || _item == null)
                return;

            if (_item.Uuid == item.Uuid)
                return;

            if (_item.petEquip.BaseAttr.Count == 0)
                return;

            //refresh arrow
            for (int i = 0; i < listEntry.Count; ++i)
            {
                if (listEntry[i].attrId != 0)
                {
                    listEntry[i].upArrow.SetActive(false);
                    listEntry[i].downArrow.SetActive(false);

                    PetEquip.Types.BaseAttr attr = CheckContainsAttrId(_item, listEntry[i].attrId);
                    if (attr != null)
                    {
                        if (listEntry[i].attrValue > attr.AttrValue)
                        {
                            listEntry[i].upArrow.SetActive(true);
                        }
                        else if (listEntry[i].attrValue < attr.AttrValue)
                        {
                            listEntry[i].downArrow.SetActive(true);
                        }
                    }
                }
            }
        }

        private PetEquip.Types.BaseAttr CheckContainsAttrId(ItemData _item, uint attrId)
        {
            foreach (PetEquip.Types.BaseAttr attrEle in _item.petEquip.BaseAttr)
            {
                if (attrEle.AttrId == attrId)
                    return attrEle;
            }

            return null;
        }
    }

    public class TipPetEquipInfoSpecial : TipPetEquipInfoBase
    {
        private List<Transform> desList = new List<Transform>(2);

        protected override void Parse()
        {
            Transform parent = transform.Find("Describle01");
            int count = parent.childCount;
            for (int i = 0; i < count; ++i)
            {
                Transform trans = parent.GetChild(i);
                desList.Add(trans);
            }
        }

        public void UpdateInfo(ItemData item, RepeatedField<PetEquip.Types.EffectAttr> attrList)
        {
            for (int i = 0; i < desList.Count; ++i)
            {
                if (i < attrList.Count)
                {
                    desList[i].gameObject.SetActive(true);

                    Text des = desList[i].GetComponent<Text>();
                    Text name = desList[i].Find("Text_Name").GetComponent<Text>();

                    des.text = "";
                    name.text = "";

                    var petEquipEffect = CSVPetEquipEffect.Instance.GetConfData(attrList[i].InfoId);
                    if (null != petEquipEffect)
                    {
                        var passiveSkillInfo = CSVPassiveSkillInfo.Instance.GetConfData(petEquipEffect.effect);
                        if (null != passiveSkillInfo)
                        {
                            TextHelper.SetText(name, passiveSkillInfo.name);
                            TextHelper.SetText(des, passiveSkillInfo.desc);
                        }
                        else
                        {
                            DebugUtil.LogError($"Not Find Id = {petEquipEffect.effect} In Table CSVPassiveSkillInfo");
                        }
                    }
                    else
                    {
                        DebugUtil.LogError($"Not Find Id = {attrList[i].InfoId} In Table CSVPetEquipEffect");
                    }
                }
                else
                {
                    desList[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public class TipPetEquipInfoSuit : TipPetEquipInfoBase
    {
        private Transform parent;

        protected override void Parse()
        {
            parent = transform.Find("View_Set_Property");
        }

        public new void UpdateInfo(ItemData item)
        {
            //生成套装外观预览
            int count = 0;
            bool hasSuitAppearanceSkill = item.petEquip.SuitSkill > 0;
            if (hasSuitAppearanceSkill)
            {
                count += 1;
                uint suitAppearanceId = item.petEquip.SuitAppearance;
                bool hasSuitAppearanceId = suitAppearanceId > 0;
                if (hasSuitAppearanceId)
                {
                    count += 1;
                }
            }
            Lib.Core.FrameworkTool.CreateChildList(parent, count);
            for (int i = 0; i < count; i++)
            {
                if(i == 0)
                {
                    var skillName = parent.GetChild(i).GetComponent<Text>();
                    uint suitSkillId = item.petEquip.SuitSkill;
                    CSVPetEquipSuitSkill.Data suitSkillData = CSVPetEquipSuitSkill.Instance.GetConfData(suitSkillId);
                    if (null != suitSkillData)
                    {
                        bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(suitSkillData.upgrade_skill);
                        if (isActiveSkill) //主动技能
                        {
                            CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(suitSkillData.upgrade_skill);
                            if (skillInfo != null)
                            {
                                TextHelper.SetText(skillName, LanguageHelper.GetTextContent(680000919, LanguageHelper.GetTextContent(680000911, LanguageHelper.GetTextContent(skillInfo.name))));
                            }
                            else
                            {
                                Debug.LogErrorFormat("not found skillId={0}", suitSkillData.upgrade_skill);
                            }
                        }
                        else
                        {
                            CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(suitSkillData.upgrade_skill);
                            if (skillInfo != null)
                            {
                                TextHelper.SetText(skillName, LanguageHelper.GetTextContent(680000919, LanguageHelper.GetTextContent(680000911, LanguageHelper.GetTextContent(skillInfo.name))));
                            }
                            else
                            {
                                Debug.LogErrorFormat("not found skillId={0}", suitSkillData.upgrade_skill);
                            }
                        }
                    }
                }
                else
                {
                    uint suitAppearanceId = item.petEquip.SuitAppearance;
                    bool hasSuitAppearanceId = suitAppearanceId > 0;
                    if (hasSuitAppearanceId)
                    {
                        CSVPetEquipSuitAppearance.Data suitAppearanceData = CSVPetEquipSuitAppearance.Instance.GetConfData(suitAppearanceId);
                        if (null != suitAppearanceData)
                        {
                            var fashionName = parent.GetChild(i).GetComponent<Text>();
                            TextHelper.SetText(fashionName, LanguageHelper.GetTextContent(680000919, LanguageHelper.GetTextContent(680000913, LanguageHelper.GetTextContent(suitAppearanceData.name))));
                        }
                    }
                }
            }

            
        }
    }
}

