using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using Google.Protobuf.Collections;

namespace Logic
{
    public class UI_Equipment_Remake_Preview_Layout
    {
        public class IconRoot
        {
            private Transform transform;

            private RawImage imgQuality;
            private Text EquipName;
            private Text EquipLevel;
            private Text EquipProfession;
            private Text EquipWearLevel;

            private PropItem propItem;

            public void Init(Transform trans)
            {
                transform = trans;

                imgQuality = transform.Find("Image_Quality").GetComponent<RawImage>();
                EquipName = transform.Find("Text_Name").GetComponent<Text>();
                EquipLevel = transform.Find("Text_Type").GetComponent<Text>();
                EquipProfession = transform.Find("Text_Profession").GetComponent<Text>();
                EquipWearLevel = transform.Find("Text_Wear").GetComponent<Text>();

                propItem = new PropItem();
                propItem.BindGameObject(transform.Find("PropItem").gameObject);
            }

            public void UpdateInfo(ItemData itemEquip)
            {
                // uint quality = Sys_Equip.Instance.CalPreviewQuality(itemEquip.EquipParam);
                //
                // itemEquip.SetQuality(quality);

                SetBgQuality(itemEquip.Quality);

                PropIconLoader.ShowItemData propData = new PropIconLoader.ShowItemData(itemEquip.Id, 1, true, itemEquip.bBind, false, false, false);
                propData.SetQuality(itemEquip.Quality);
                propData.SetMarketEnd(itemEquip.bMarketEnd);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_Equipment_Remake_Preview, propData));
                propItem.Layout.imgQuality.enabled = true;

                EquipName.text = LanguageHelper.GetLanguageColorWordsFormat(LanguageHelper.GetTextContent(itemEquip.cSVItemData.name_id), itemEquip.Quality + 2007416);

                CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(itemEquip.Id);
                EquipLevel.text = LanguageHelper.GetTextContent(4011, equipData.TransLevel().ToString());

                EquipWearLevel.text = LanguageHelper.GetTextContent(4198, equipData.equipment_level.ToString());

                if (equipData.career_condition == null)
                {
                    EquipProfession.text = LanguageHelper.GetTextContent(4012, LanguageHelper.GetTextContent(4183));
                }
                else
                {
                    System.Text.StringBuilder strBuilder = Lib.Core.StringBuilderPool.GetTemporary();

                    for (int i = 0; i < equipData.career_condition.Count; ++i)
                    {
                        strBuilder.Append(LanguageHelper.GetTextContent(OccupationHelper.GetTextID(equipData.career_condition[i])));
                        if (i != equipData.career_condition.Count - 1)
                        {
                            strBuilder.Append(".");
                        }
                    }

                    EquipProfession.text = LanguageHelper.GetTextContent(4012, Lib.Core.StringBuilderPool.ReleaseTemporaryAndToString(strBuilder));
                }
            }

            public void SetBgQuality(uint quality)
            {
                string bgPath = null;
                switch ((EItemQuality)quality)
                {
                    case EItemQuality.White:
                        bgPath = Constants.TipBgWhite;
                        break;
                    case EItemQuality.Green:
                        bgPath = Constants.TipBgGreen;
                        break;
                    case EItemQuality.Blue:
                        bgPath = Constants.TipBgBlue;
                        break;
                    case EItemQuality.Purple:
                        bgPath = Constants.TipBgPurple;
                        break;
                    case EItemQuality.Orange:
                        bgPath = Constants.TipBgOrange;
                        break;
                    default:
                        break;
                }

                ImageHelper.SetTexture(imgQuality, bgPath);
            }
        }

        public class InfoBasic
        {
            protected class InfoAttrEntry
            {
                public GameObject root;
                public Text name;
                public Text value;
            }

            protected class InfoAttr
            {
                public uint attrId;
                public uint minValue;
                public uint maxValue;
            }

            private Transform transform;

            private List<InfoAttrEntry> listEntry = new List<InfoAttrEntry>();
            //private ItemData item;

            public void Init(Transform trans)
            {
                transform = trans;

                for (int i = 0; i < 3; ++i)
                {
                    GameObject attrGo = transform.Find(string.Format("View_Basis_Property{0}", i)).gameObject;

                    InfoAttrEntry entry = new InfoAttrEntry();
                    entry.root = attrGo.transform.Find("Basis_Property").gameObject;
                    entry.name = entry.root.transform.Find("Basis_Property01").GetComponent<Text>();
                    entry.value = entry.root.transform.Find("Number").GetComponent<Text>();

                    listEntry.Add(entry);
                }
            }

            public void UpdateInfo(EquipRemakeParam param)
            {
                //this.item = _item;
                List<InfoAttr> attrList = new List<InfoAttr>(4);

                CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(param.paramEquip.Id);
                if (equipData.attr != null && equipData.attr.Count > 0)
                {
                    for (int i = 0; i < equipData.attr.Count; ++i)
                    {
                        InfoAttr attr = new InfoAttr();
                        attr.attrId = equipData.attr[i][0];
                        attr.minValue = (uint)Sys_Equip.Instance.GetRemakeBasicValue(param.rebuildId, i, attr.attrId, param.paramEquip.Id);
                        // attr.minValue = equipData.attr[i][1];
                        // attr.maxValue = equipData.attr[i][2];

                        attrList.Add(attr);
                    }
                }

                for (int i = 0; i < listEntry.Count; ++i)
                {
                    if (i < attrList.Count)
                    {
                        listEntry[i].root.transform.parent.gameObject.SetActive(true);

                        listEntry[i].root.SetActive(true);

                        CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrList[i].attrId);
                        listEntry[i].name.text = LanguageHelper.GetTextContent(attrData.name);
                        listEntry[i].value.text =
                            Sys_Attr.Instance.GetAttrValue(attrData, attrList[i].minValue).ToString();
                        //  LanguageHelper.GetTextContent(10926, Sys_Attr.Instance.GetAttrValue(attrData, attrList[i].minValue),
                        // Sys_Attr.Instance.GetAttrValue(attrData, attrList[i].maxValue));
                    }
                    else
                    {
                        listEntry[i].root.transform.parent.gameObject.SetActive(false);
                    }
                }
            }
        }

        public class InfoGreen
        {
            private Transform transform;

            private Text m_TextName;
            private Text m_TextValue;

            public void Init(Transform trans)
            {
                transform = trans;

                m_TextName = transform.Find("Describle01/Text_Name").GetComponent<Text>();
                m_TextValue = transform.Find("Describle01/Text0").GetComponent<Text>();
            }

            public void UpdateInfo(ItemData itemData)
            {
                //uint quality = Sys_Equip.Instance.CalPreLowerQuality(itemData.EquipParam);
                transform.gameObject.SetActive(false);
            }
        }

        public class InfoEffect
        {
            private Transform transform;

            private Transform transTemplate;
            private Text m_TextName;
            private Text m_TextValue;

            public void Init(Transform trans)
            {
                transform = trans;

                transTemplate = transform.Find("Describle01");
                transTemplate.gameObject.SetActive(false);
                // m_TextName = transform.Find("Describle01/Text_Name").GetComponent<Text>();
                // m_TextValue = transform.Find("Describle01/Text0").GetComponent<Text>();
            }

            public void UpdateInfo(ItemData itemData)
            {
                ItemData opItem = Sys_Bag.Instance.GetItemDataByUuid(Sys_Equip.Instance.CurOpEquipUId);
                for (int i = 0; i < opItem.Equip.EffectAttr.Count; ++i)
                {
                    GameObject go = GameObject.Instantiate(transTemplate.gameObject);
                    go.transform.SetParent(transTemplate.parent);
                    go.transform.localScale = transTemplate.localScale;

                    Text des = go.transform.Find("Text_Name").GetComponent<Text>();
                    Text name = go.transform.Find("Text0").GetComponent<Text>();
                    des.text = "";
                    name.text = "";

                    CSVEquipmentEffect.Data effectData = CSVEquipmentEffect.Instance.GetDataByEffectId(opItem.Equip.EffectAttr[i].Attr2.Id);
                    if (effectData != null)
                        name.text = LanguageHelper.GetTextContent(effectData.name);

                    //AttributeRow row = attrList[i].Attr2;
                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(opItem.Equip.EffectAttr[i].Attr2.Id);
                    if (skillInfo != null)
                        des.text = LanguageHelper.GetTextContent(skillInfo.desc);
                    else
                        Debug.LogErrorFormat("CSVPassiveSkillInfo 找不到 id {0}", opItem.Equip.EffectAttr[i].Attr2.Id);
                }
            }
        }

        public class InfoQuality
        {
            public class QualityRate
            {
                private Transform transform;

                private Text m_TextName;
                private Text m_TextValue;
                public void Init(Transform trans)
                {
                    transform = trans;

                    m_TextName = transform.GetComponent<Text>();
                    m_TextValue = transform.Find("Number").GetComponent<Text>();
                }

                public void Show()
                {
                    transform.gameObject.SetActive(true);
                }

                public void Hide()
                {
                    transform.gameObject.SetActive(false);
                }

                public void UpdateInfo(string name, string rate)
                {
                    m_TextName.text = name;
                    m_TextValue.text = rate;
                }
            }

            private Transform transform;

            private List<QualityRate> list = new List<QualityRate>(5);
            public void Init(Transform trans)
            {
                transform = trans;

                for (int i = 0; i < 5; ++i)
                {
                    QualityRate quality = new QualityRate();
                    quality.Init(transform.Find(string.Format("View_Quality_Property/Quality_Property{0}", i)));
                    list.Add(quality);
                }
            }

            public void UpdateInfo(ItemData itemData)
            {
                CSVEquipmentParameter.Data paraData = CSVEquipmentParameter.Instance.GetConfData(itemData.EquipParam);
                if (paraData != null)
                {
                    uint totalWeight = 0;
                    for (int i = 0; i < paraData.quality_weight.Count; ++i)
                        totalWeight += paraData.quality_weight[i];

                    for (int i = 0; i < paraData.quality_weight.Count; ++i)
                    {
                        if (i < list.Count)
                        {
                            bool isShow = paraData.quality_weight[i] != 0;
                            if (isShow)
                            {
                                list[i].Show();

                                string name = LanguageHelper.GetTextContent(4215 + (uint)i);
                                string value = string.Format("{0}%", paraData.quality_weight[i] * 100 / totalWeight);
                                list[i].UpdateInfo(name, value);
                            }
                            else
                            {
                                list[i].Hide();
                            }
                        }
                    }
                }
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }
        }

        private Transform transform;

        public IconRoot iconRoot;
        public InfoBasic infoBasic;
        public InfoGreen infoGreen;
        public InfoEffect infoEffect;
        public InfoQuality infoQuality;

        private GameObject _content;
        private ScrollRect _scrollRect;
        private ContentSizeFitter _sizeFitter;
        private RectTransform _rectFitter;

        private Button btnClose;

        public void Init(Transform trans)
        {
            transform = trans;

            btnClose = transform.Find("Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClose);

            iconRoot = new IconRoot();
            iconRoot.Init(transform.Find("View_Tips/Tips/Background_Root/IconRoot"));

            infoBasic = new InfoBasic();
            infoBasic.Init(transform.Find("View_Tips/Tips/Background_Root/Scroll_View/GameObject/View_Basic_Prop"));

            infoGreen = new InfoGreen();
            infoGreen.Init(transform.Find("View_Tips/Tips/Background_Root/Scroll_View/GameObject/View_Green_Prop"));

            infoEffect = new InfoEffect();
            infoEffect.Init(transform.Find("View_Tips/Tips/Background_Root/Scroll_View/GameObject/View_Special_Prop"));

            infoQuality = new InfoQuality();
            infoQuality.Init(transform.Find("View_Tips/Tips/Background_Root/Scroll_View/GameObject/View_Quality_Prop"));
            infoQuality.Hide();

            _content = transform.Find("View_Tips/Tips/Background_Root/Scroll_View/GameObject").gameObject;
            _scrollRect = transform.Find("View_Tips/Tips/Background_Root/Scroll_View").gameObject.GetComponent<ScrollRect>();
            _sizeFitter = transform.Find("View_Tips/Tips/Background_Root/Scroll_View").gameObject.GetComponent<ContentSizeFitter>();
            _rectFitter = _sizeFitter.GetComponent<RectTransform>();
        }

        private void OnClose()
        {
            UIManager.CloseUI(EUIID.UI_Equipment_Remake_Preview);
        }

        public void UpdateInfo(EquipRemakeParam param)
        {
            iconRoot?.UpdateInfo(param.paramEquip);
            infoBasic?.UpdateInfo(param);
            infoGreen?.UpdateInfo(param.paramEquip);
            infoEffect?.UpdateInfo(param.paramEquip);
            infoQuality?.UpdateInfo(param.paramEquip);
        }
    }
}

