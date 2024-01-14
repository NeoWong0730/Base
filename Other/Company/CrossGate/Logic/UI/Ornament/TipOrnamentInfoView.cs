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
    public class TipOrnamentInfoView : UIParseCommon
    {
        //basic
        public TipOrnamentInfoBasic basicInfo;
        //extra
        public TipOrnamentInfoExtra ExtraInfo;

        protected override void Parse()
        {
            basicInfo = new TipOrnamentInfoBasic();
            basicInfo.Init(transform.Find("GameObject/View_Basic_Prop"));
            ExtraInfo = new TipOrnamentInfoExtra();
            ExtraInfo.Init(transform.Find("GameObject/View_Extra_Prop"));
        }

        public override void UpdateInfo(ItemData item)
        {
            basicInfo.UpdateView(item);
            //额外属性
            if (item.ornament != null && (item.ornament.ExtAttr.Count > 0 || item.ornament.ExtSkill.Count > 0))
            {
                ExtraInfo.gameObject.SetActive(true);
                ExtraInfo.UpdateView(item);
            }
            else
            {
                ExtraInfo.gameObject.SetActive(false);
            }
        }

        public override void OnDestroy()
        {
            basicInfo.OnDestroy();
            ExtraInfo.OnDestroy();
        }
    }

    public class TipOrnamentInfoBase : UIParseCommon
    {
        protected string lineStr = "Image_Title_Splitline";
        protected List<InfoAttrEntry> listEntry = new List<InfoAttrEntry>();

        protected ItemData item;

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
        }
        protected override void Parse()
        {
            Sys_Ornament.Instance.eventEmitter.Handle<ItemData>(Sys_Ornament.EEvents.OnCompareBasicAttrValue, OnCompareValue, true);
        }
        public override void OnDestroy()
        {
            Sys_Ornament.Instance.eventEmitter.Handle<ItemData>(Sys_Ornament.EEvents.OnCompareBasicAttrValue, OnCompareValue, false);
        }
        public void OnCompareValue(ItemData _item)
        {
            if (item == null || _item == null)
                return;
            if (_item.Uuid == item.Uuid)
                return;
            //refresh arrow
            for (int i = 0; i < listEntry.Count; ++i)
            {
                if (listEntry[i].attrId != 0)
                {
                    listEntry[i].upArrow.SetActive(false);
                    listEntry[i].downArrow.SetActive(false);

                    uint attrValue = CheckContainsAttrId(_item, listEntry[i].attrId);
                    if (attrValue > 0)
                    {
                        if (listEntry[i].attrValue > attrValue)
                        {
                            listEntry[i].upArrow.SetActive(true);
                        }
                        else if (listEntry[i].attrValue < attrValue)
                        {
                            listEntry[i].downArrow.SetActive(true);
                        }
                    }
                }
            }
        }

        public virtual uint CheckContainsAttrId(ItemData _item, uint attrId)
        {
            return 0;
        }
    }

    public class TipOrnamentInfoBasic : TipOrnamentInfoBase
    {
        protected override void Parse()
        {
            base.Parse();
            for (int i = 0; i < 4; ++i)
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
        }

        public void UpdateView(ItemData _item)
        {
            this.item = _item;
            CSVOrnamentsUpgrade.Data ornamentData = CSVOrnamentsUpgrade.Instance.GetConfData(item.Id);
            if (ornamentData != null)
            {
                var attrList = ornamentData.base_attr;
                for (int i = 0; i < listEntry.Count; ++i)
                {
                    if (i < attrList.Count)
                    {
                        listEntry[i].root.transform.parent.gameObject.SetActive(true);

                        listEntry[i].root.SetActive(true);
                        listEntry[i].upArrow.SetActive(false);
                        listEntry[i].downArrow.SetActive(false);

                        uint attrId = attrList[i][0];
                        uint attrValue = attrList[i][1];

                        listEntry[i].attrId = attrId;
                        listEntry[i].attrValue = attrValue;
                        CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);
                        listEntry[i].name.text = LanguageHelper.GetTextContent(attrData.name);
                        listEntry[i].value.text = Sys_Attr.Instance.GetAttrValue(attrData, attrValue);
                    }
                    else
                    {
                        listEntry[i].root.transform.parent.gameObject.SetActive(false);
                    }
                }

            }
        }
        public override uint CheckContainsAttrId(ItemData _item, uint attrId)
        {
            CSVOrnamentsUpgrade.Data ornamentData = CSVOrnamentsUpgrade.Instance.GetConfData(_item.Id);
            if (ornamentData != null)
            {
                var attrList = ornamentData.base_attr;
                for (int i = 0; i < attrList.Count; i++)
                {
                    if (attrList[i][0] == attrId)
                        return attrList[i][1];
                }
            }
            return 0;
        }
    }

    public class TipOrnamentInfoExtra : TipOrnamentInfoBase
    {
        protected override void Parse()
        {
            base.Parse();
            Transform parent = transform.Find("View_Smelt_Property");
            int count = parent.childCount;
            for (int i = 0; i < count; ++i)
            {
                InfoAttrEntry entry = new InfoAttrEntry();
                entry.root = parent.GetChild(i).gameObject;
                entry.name = entry.root.transform.Find("Basis_Property01").GetComponent<Text>();
                entry.value = entry.root.transform.Find("Number").GetComponent<Text>();
                entry.upArrow = entry.root.transform.Find("Image_UpArrow").gameObject;
                entry.downArrow = entry.root.transform.Find("Image_DownArrow").gameObject;
                listEntry.Add(entry);
            }
        }

        public void UpdateView(ItemData _item)
        {
            this.item = _item;
            var attrList = item.ornament.ExtAttr;
            var skillList = item.ornament.ExtSkill;
            for (int i = 0; i < listEntry.Count; ++i)
            {
                if (i < attrList.Count)
                {
                    listEntry[i].root.SetActive(true);
                    var attr = attrList[i];
                    listEntry[i].attrId = attr.AttrId;
                    listEntry[i].attrValue = attr.AttrValue;
                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attr.AttrId);
                    listEntry[i].name.text = LanguageHelper.GetTextContent(attrData.name);
                    listEntry[i].value.text = Sys_Ornament.Instance.GetExtAttrShowString(attr.AttrValue, attr.InfoId);
                }
                else if (i < attrList.Count + skillList.Count)
                {
                    listEntry[i].root.SetActive(true);
                    var skill = skillList[i - attrList.Count];
                    CSVPassiveSkillInfo.Data skillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skill.SkillId);
                    listEntry[i].name.text = LanguageHelper.GetTextContent(skillInfoData.name);
                    listEntry[i].value.text = Sys_Ornament.Instance.GetPassiveSkillShowString(skill.InfoId, skillInfoData);
                }
                else
                {
                    listEntry[i].root.SetActive(false);
                }
            }
        }

        public override uint CheckContainsAttrId(ItemData _item, uint attrId)
        {
            if (_item.ornament != null && _item.ornament.ExtAttr.Count > 0)
            {
                var attrList = _item.ornament.ExtAttr;
                for (int i = 0; i < attrList.Count; i++)
                {
                    if (attrList[i].AttrId == attrId)
                        return attrList[i].AttrValue;
                }
            }
            return 0;
        }
    }
}