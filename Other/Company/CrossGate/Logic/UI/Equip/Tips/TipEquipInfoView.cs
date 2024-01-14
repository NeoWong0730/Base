
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
    public class TipEquipInfoView : UIParseCommon
    {
        //green
        public TipEquipInfoGreen greenInfo;

        //basic
        public TipEquipInfoBasic basicInfo;

        //special
        public TipEquipInfoSpecial specialInfo;

        //Jewel
        public TipEquipInfoJewel jewelInfo;

        //smelt
        public TipEquipInfoSmelt smeltInfo;

        //magic defence
        public TipEquipInfoEnchant enchantInfo;

        //suit
        public TipEquipInfoSuit suitInfo;

        protected override void Parse()
        {
            greenInfo = new TipEquipInfoGreen();
            greenInfo.Init(transform.Find("GameObject/View_Green_Prop"));

            basicInfo = new TipEquipInfoBasic();
            basicInfo.Init(transform.Find("GameObject/View_Basic_Prop"));

            specialInfo = new TipEquipInfoSpecial();
            specialInfo.Init(transform.Find("GameObject/View_SpecicalEffects_Prop"));

            jewelInfo = new TipEquipInfoJewel();
            jewelInfo.Init(transform.Find("GameObject/View_Diamond_Prop"));

            smeltInfo = new TipEquipInfoSmelt();
            smeltInfo.Init(transform.Find("GameObject/View_Smelt_Prop"));

            enchantInfo = new TipEquipInfoEnchant();
            enchantInfo.Init(transform.Find("GameObject/View_Enchant_Prop"));

            suitInfo = new TipEquipInfoSuit();
            suitInfo.Init(transform.Find("GameObject/View_Suit_Prop"));
        }

        public override void UpdateInfo(ItemData item)
        {
            CheckInfoShow(basicInfo, item, item.Equip.BaseAttr);

            //绿字属性
            RepeatedField<global::Packet.AttributeElem> greenList = new RepeatedField<AttributeElem>();
            for (int i = 0; i < item.Equip.GreenAttr.Count; ++i)
            {
                if (item.Equip.GreenAttr[i].Attr2.Value != 0)
                    greenList.Add(item.Equip.GreenAttr[i]);
            }
            CheckInfoShow(greenInfo, item, greenList);

            CheckInfoShow(specialInfo, item, item.Equip.EffectAttr);

            //宝石需要特别处理
            bool jewelShow = false;
            foreach (uint jewelInfoId in item.Equip.JewelinfoId)
            {
                if (jewelInfoId != 0)
                {
                    jewelShow = true;
                    break;
                }
            }

            jewelInfo.gameObject.SetActive(jewelShow);
            if (jewelShow)
            {
                jewelInfo.UpdateJewelAttr(item, item.Equip.JewelinfoId);
            }

            //附魔属性
            RepeatedField<global::Packet.AttributeElem> enchantList = new RepeatedField<AttributeElem>();
            for (int i = 0; i < item.Equip.EnchantAttr.Count; ++i)
            {
                AttributeElem elem = item.Equip.EnchantAttr[i];
                if (elem.EndTime > Sys_Time.Instance.GetServerTime())
                    enchantList.Add(elem);
            }
            CheckInfoShow(enchantInfo, item, enchantList);

            //熔炼属性
            RepeatedField<global::Packet.AttributeElem> smeltAttrList = new RepeatedField<global::Packet.AttributeElem>();
            for (int i = 0; i < item.Equip.SmeltAttr.Count; ++i)
            {
                smeltAttrList.Add(item.Equip.SmeltAttr[i].Attr);
            }
            CheckInfoShow(smeltInfo, item, smeltAttrList);

            //套装属性
            RepeatedField<global::Packet.AttributeElem> suitAttrList = new RepeatedField<global::Packet.AttributeElem>();
            for (int i = 0; i < item.Equip.SuitAttr.Count; ++i)
            {
                suitAttrList.Add(item.Equip.SuitAttr[i]);
            }
            CheckInfoShow(suitInfo, item, suitAttrList);
            //ForceRebuildLayout(basicInfo.gameObject);
            //ForceRebuildLayout(greenInfo.gameObject);
            //ForceRebuildLayout(specialInfo.gameObject);
            //ForceRebuildLayout(diamondInfo.gameObject);
            //ForceRebuildLayout(enchantInfo.gameObject);
        }

        private void CheckInfoShow(TipEquipInfoBase info, ItemData item, RepeatedField<AttributeElem> attrList)
        {
            bool haveAttr = attrList.Count > 0;
            info.gameObject.SetActive(haveAttr);
            if (haveAttr)
                info.UpdateInfo(item, attrList);
        }

        public override void OnDestroy()
        {
            basicInfo.OnDestroy();
            enchantInfo.OnDestroy();
        }
    }

    public class TipEquipInfoBase : UIParseCommon
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

        public virtual void UpdateInfo(ItemData item, RepeatedField<AttributeElem> attrList) { }

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

    public class TipEquipInfoGreen : TipEquipInfoBase
    {
        private List<GameObject> goList = new List<GameObject>(2);

        protected override void Parse()
        {
            int count = transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                GameObject go = transform.GetChild(i).gameObject;
                goList.Add(go);
            }
        }

        public override void UpdateInfo(ItemData item, RepeatedField<AttributeElem> attrList)
        {
            for (int i = 0; i < goList.Count; ++i)
            {
                if (i < attrList.Count)
                {
                    goList[i].SetActive(true);

                    Text textName = goList[i].transform.Find("Text_Property01").GetComponent<Text>();
                    Text textNum = goList[i].transform.Find("Number").GetComponent<Text>();

                    AttributeElem equipAttr = attrList[i];
                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(equipAttr.Attr2.Id);

                    textName.text = LanguageHelper.GetTextContent(attrData.name);
                    textNum.text = Sys_Attr.Instance.GetAttrValue(attrData, equipAttr.Attr2.Value);
                }
                else
                {
                    goList[i].SetActive(false);
                }
            }
        }
    }

    public class TipEquipInfoBasic : TipEquipInfoBase
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

        public override void UpdateInfo(ItemData _item, RepeatedField<AttributeElem> attrList)
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

                    listEntry[i].attrId = attrList[i].Attr2.Id;
                    listEntry[i].attrValue = attrList[i].Attr2.Value;

                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrList[i].Attr2.Id);
                    listEntry[i].name.text = LanguageHelper.GetTextContent(attrData.name);
                    listEntry[i].value.text = Sys_Attr.Instance.GetAttrValue(attrData, attrList[i].Attr2.Value);
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

            if (_item.Equip.BaseAttr.Count == 0)
                return;

            //refresh arrow
            for (int i = 0; i < listEntry.Count; ++i)
            {
                if (listEntry[i].attrId != 0)
                {
                    listEntry[i].upArrow.SetActive(false);
                    listEntry[i].downArrow.SetActive(false);

                    AttributeElem attr = CheckContainsAttrId(_item, listEntry[i].attrId);
                    if (attr != null)
                    {
                        if (listEntry[i].attrValue > attr.Attr2.Value)
                        {
                            listEntry[i].upArrow.SetActive(true);
                        }
                        else if (listEntry[i].attrValue < attr.Attr2.Value)
                        {
                            listEntry[i].downArrow.SetActive(true);
                        }
                    }
                }
            }
        }

        private AttributeElem CheckContainsAttrId(ItemData _item, uint attrId)
        {
            foreach (AttributeElem attrEle in _item.Equip.BaseAttr)
            {
                if (attrEle.Attr2.Id == attrId)
                    return attrEle;
            }

            return null;
        }
    }

    public class TipEquipInfoSpecial : TipEquipInfoBase
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

        public override void UpdateInfo(ItemData item, RepeatedField<AttributeElem> attrList)
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

                    CSVEquipmentEffect.Data effectData = CSVEquipmentEffect.Instance.GetDataByEffectId(attrList[i].Attr2.Id);
                    if (effectData != null)
                        name.text = LanguageHelper.GetTextContent(effectData.name);

                    //AttributeRow row = attrList[i].Attr2;
                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(attrList[i].Attr2.Id);
                    if (skillInfo != null)
                        des.text = LanguageHelper.GetTextContent(skillInfo.desc);
                    else
                        DebugUtil.LogErrorFormat("CSVPassiveSkillInfo 找不到 id {0}", attrList[i].Attr2.Id);
                }
                else
                {
                    desList[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public class TipEquipInfoJewel : TipEquipInfoBase
    {
        private List<GameObject> goList = new List<GameObject>();

        protected override void Parse()
        {
            Transform parent = transform.Find("View_Diamond_Property");
            int count = parent.childCount;
            for (int i = 0; i < count; ++i)
            {
                GameObject go = parent.GetChild(i).gameObject;
                goList.Add(go);
            }
        }

        public void UpdateJewelAttr(ItemData item, RepeatedField<uint> jewelList)
        {
            for (int i = 0; i < goList.Count; ++i)
            {
                if (i < jewelList.Count && jewelList[i] != 0u)
                {
                    goList[i].SetActive(true);

                    CSVItem.Data itemJewel = CSVItem.Instance.GetConfData(jewelList[i]);
                    CSVJewel.Data jewelInfo = CSVJewel.Instance.GetConfData(jewelList[i]);

                    Text name = goList[i].GetComponent<Text>();
                    name.text = LanguageHelper.GetTextContent(itemJewel.name_id);

                    List<GameObject> listAttr = new List<GameObject>();
                    int count = goList[i].transform.childCount;
                    for (int j = 0; j < count; ++j)
                    {
                        GameObject temp = goList[i].transform.Find(string.Format("Basis_Property{0}", j)).gameObject;
                        temp.SetActive(false);
                        listAttr.Add(temp);
                    }

                    if (jewelInfo.percent != 0) //策划确认，百分比属性只有一个
                    {
                        listAttr[0].SetActive(true);

                        Text attrName = listAttr[0].GetComponent<Text>();
                        Text attrValue = listAttr[0].transform.Find("Number").GetComponent<Text>();

                        attrName.text = LanguageHelper.GetTextContent(4050);
                        attrValue.text = string.Format("+{0}%", jewelInfo.percent);
                    }
                    else
                    {
                        for (int j = 0; j < listAttr.Count; ++j)
                        {
                            if (j < jewelInfo.attr.Count)
                            {
                                listAttr[j].SetActive(true);

                                Text attrName = listAttr[j].GetComponent<Text>();
                                Text attrValue = listAttr[j].transform.Find("Number").GetComponent<Text>();

                                List<uint> attr = jewelInfo.attr[j];

                                CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(jewelInfo.attr[j][0]);
                                attrName.text = LanguageHelper.GetTextContent(attrData.name);
                                attrValue.text = string.Format("+{0}", Sys_Attr.Instance.GetAttrValue(attrData, jewelInfo.attr[j][1]));
                            }
                            else
                            {
                                listAttr[j].SetActive(false);
                            }
                        }
                    }
                }
                else
                {
                    goList[i].SetActive(false);
                }
            }
        }
    }

    public class TipEquipInfoSmelt : TipEquipInfoBase
    {
        private List<InfoAttrEntry> listEntry = new List<InfoAttrEntry>();

        protected override void Parse()
        {
            Transform parent = transform.Find("View_Smelt_Property");
            int count = parent.childCount;
            for (int i = 0; i < count; ++i)
            {
                InfoAttrEntry entry = new InfoAttrEntry();
                entry.root = parent.GetChild(i).gameObject;
                entry.name = entry.root.transform.Find("Basis_Property01").GetComponent<Text>();
                entry.value = entry.root.transform.Find("Number").GetComponent<Text>();

                listEntry.Add(entry);
            }
        }

        public override void UpdateInfo(ItemData item, RepeatedField<AttributeElem> attrList)
        {
            for (int i = 0; i < listEntry.Count; ++i)
            {
                if (i < attrList.Count)
                {
                    listEntry[i].root.SetActive(true);


                    AttributeElem equipAttr = attrList[i];
                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(equipAttr.Attr2.Id);

                    listEntry[i].name.text = LanguageHelper.GetTextContent(attrData.name);
                    listEntry[i].value.text = Sys_Attr.Instance.GetAttrValue(attrData, equipAttr.Attr2.Value);
                }
                else
                {
                    listEntry[i].root.SetActive(false);
                }
            }
        }
    }

    public class TipEquipInfoEnchant : TipEquipInfoBase
    {
        private List<InfoAttrEntry> listEntry = new List<InfoAttrEntry>();

        protected override void Parse()
        {
            Transform parent = transform.Find("View_Enchant_Property");
            int count = parent.childCount;

            for (int i = 0; i < count; ++i)
            {
                InfoAttrEntry entry = new InfoAttrEntry();
                entry.root = parent.GetChild(i).gameObject;
                entry.name = entry.root.transform.Find("Basis_Property01").GetComponent<Text>();
                entry.value = entry.root.transform.Find("Number").GetComponent<Text>();
                entry.time = entry.root.transform.Find("Text_Time").GetComponent<Text>();

                listEntry.Add(entry);
            }
        }

        public override void OnDestroy()
        {
            OnClearEntry();
            base.OnDestroy();
        }

        private void OnClearEntry()
        {
            for (int i = 0; i < listEntry.Count; ++i)
            {
                listEntry[i].StopTimer();
            }
        }

        public override void UpdateInfo(ItemData item, RepeatedField<AttributeElem> attrList)
        {
            OnClearEntry();

            for (int i = 0; i < listEntry.Count; ++i)
            {
                if (i < attrList.Count)
                {
                    listEntry[i].root.SetActive(true);

                    AttributeElem equipAttr = attrList[i];
                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(equipAttr.Attr2.Id);

                    listEntry[i].name.text = LanguageHelper.GetTextContent(attrData.name);
                    listEntry[i].value.text = Sys_Attr.Instance.GetAttrValue(attrData, equipAttr.Attr2.Value);
                    listEntry[i].StartTimer(equipAttr.EndTime);
                }
                else
                {
                    listEntry[i].root.SetActive(false);
                }
            }
        }
    }

    public class TipEquipInfoSuit : TipEquipInfoBase
    {
        private List<GameObject> goList = new List<GameObject>();

        protected override void Parse()
        {
            Transform parent = transform.Find("Suit_Property01");
            int count = parent.childCount;
            for (int i = 0; i < count; ++i)
            {
                goList.Add(parent.GetChild(i).gameObject);
            }
        }

        public override void UpdateInfo(ItemData item, RepeatedField<AttributeElem> attrList)
        {
            //default false
            for (int i = 0; i < goList.Count; ++i)
                goList[i].SetActive(false);

            int index = 0;
            for (int i = 0; i < attrList.Count; ++i) //本身套装属性
            {
                if (index < goList.Count)
                {
                    goList[index].SetActive(true);

                    Text attrName = goList[i].transform.Find("Basis_Property").GetComponent<Text>();
                    Text attrValue = goList[i].GetComponent<Text>();
                    attrName.text = "";
                    attrValue.text = "";

                    AttributeRow row = attrList[i].Attr2;
                    if (row.AttrType == 1) //被动技能
                    {
                        CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(attrList[i].Attr2.Id);
                        //CSVPassiveSkill.Data skillInfo = CSVPassiveSkill.Instance.GetConfData(attrList[i].Attr2.Id);
                        if (skillInfo != null)
                            attrName.text = LanguageHelper.GetTextContent(skillInfo.desc);
                    }
                    else
                    {
                        CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrList[i].Attr2.Id);
                        attrName.text = LanguageHelper.GetTextContent(attrData.name);
                        attrValue.text = Sys_Attr.Instance.GetAttrValue(attrData, attrList[i].Attr2.Value);
                    }

                    index++;
                }
            }

            //没有套装
            if (item.Equip.SuitTypeId == 0u)
                return;

            //套装属性
            uint suitNum = Sys_Equip.Instance.CalSuitNumber(item.Equip.SuitTypeId);

            List<CSVSuitEffect.Data>  allEffects = CSVSuitEffect.Instance.GetSuitEffectList(item.Equip.SuitTypeId, Sys_Role.Instance.Role.Career);

            for (int i = 0; i < allEffects.Count; ++i)
            {
                CSVSuitEffect.Data effectData = allEffects[i];
                if (index < goList.Count)
                {
                    goList[index].SetActive(true);

                    bool isActive = effectData.num <= suitNum;
                    uint colorId = Sys_Equip.Instance.IsEquiped(item) && isActive ? 4097u : 4098u;

                    if (effectData.effect != 0)
                    {
                        Text propName = goList[index].transform.Find("Basis_Property").GetComponent<Text>();
                        Text propValue = goList[index].GetComponent<Text>();

                        CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(effectData.effect);
                        //CSVPassiveSkill.Data skillInfo = CSVPassiveSkill.Instance.GetConfData(effectData.effect);

                        string nameContent = LanguageHelper.GetTextContent(4096, effectData.num.ToString());
                        propName.text = LanguageHelper.GetLanguageColorWordsFormat(nameContent, colorId);
                        string valueContent = LanguageHelper.GetTextContent(skillInfo.desc);
                        propValue.text = LanguageHelper.GetLanguageColorWordsFormat(valueContent, colorId);

                        index++;
                    }
                    else
                    {
                        if (effectData.attr != null)
                        {
                            System.Text.StringBuilder strBuilder = StringBuilderPool.GetTemporary();

                            for (int j = 0; j < effectData.attr.Count; ++j)
                            {
                                uint attrId = effectData.attr[j][0];
                                uint attrValue = effectData.attr[j][1];

                                CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);
                                if (attrData != null)
                                {
                                    string valueContent = string.Format("{0} {1}", LanguageHelper.GetTextContent(attrData.name), Sys_Attr.Instance.GetAttrValue(attrData, attrValue));
                                    strBuilder.Append(valueContent);
                                    strBuilder.Append("  ");
                                }
                                else
                                {
                                    Debug.LogErrorFormat("attrId is Error : {0}", attrId);
                                }
                            }

                            string strResult = StringBuilderPool.ReleaseTemporaryAndToString(strBuilder);

                            if (index < goList.Count)
                            {
                                Text propName = goList[index].transform.Find("Basis_Property").GetComponent<Text>();
                                Text propValue = goList[index].GetComponent<Text>();

                                string nameContent = LanguageHelper.GetTextContent(4096, effectData.num.ToString());
                                propName.text = LanguageHelper.GetLanguageColorWordsFormat(nameContent, colorId);

                                propValue.text = LanguageHelper.GetLanguageColorWordsFormat(strResult, colorId);
                            }
                            else
                            {
                                Debug.LogError("suit attr is ount of 3");
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("suit attr is ount of 3");
                }
            }
        }
    }
}

