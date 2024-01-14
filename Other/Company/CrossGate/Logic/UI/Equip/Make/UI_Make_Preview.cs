using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class MakePreviewParam
    {
        public ItemData itemEquip;
        public uint paperId;
    }

    public class UI_Make_Preview : UIBase, UI_Make_Preview.SuitEffectPre.IListener
    {
        public class SuitEffectPre
        {
            public class SuitEffectBase
            {
                private Transform transform;
                private GameObject goTemplate;

                public void Init(Transform trans)
                {
                    transform = trans;
                    goTemplate = transform.Find("Text").gameObject;
                    goTemplate.gameObject.SetActive(false);
                }

                public void UpdateInfo(uint suitId)
                {
                    Lib.Core.FrameworkTool.DestroyChildren(goTemplate.transform.parent.gameObject, goTemplate.name);
                    CSVSuit.Data suitInfo = CSVSuit.Instance.GetConfData(suitId);

                    foreach (var attrInfo in suitInfo.attr)
                    {
                        uint attrId = attrInfo[0];
                        uint attrMinValue = attrInfo[1];
                        uint attrMaxValue = attrInfo[2];

                        GameObject propGo = GameObject.Instantiate<GameObject>(goTemplate, goTemplate.transform.parent);
                        propGo.SetActive(true);

                        Text propName = propGo.GetComponent<Text>();
                        Text propValue = propGo.transform.Find("Text1").GetComponent<Text>();

                        propName.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(attrId).name);
                        CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);
                        propValue.text = string.Format("{0}~{1}", Sys_Attr.Instance.GetAttrValue(attrData, attrMinValue)
                            , Sys_Attr.Instance.GetAttrValue(attrData, attrMaxValue));
                    }
                }
            }

            public class SuitEffectSuit
            {
                private Transform transform;
                private GameObject goTemplate;

                public void Init(Transform trans)
                {
                    transform = trans;
                    goTemplate = transform.Find("Text").gameObject;
                    goTemplate.gameObject.SetActive(false);
                }

                public void UpdateInfo(ItemData euipItem, uint suitId)
                {
                    Lib.Core.FrameworkTool.DestroyChildren(goTemplate.transform.parent.gameObject, goTemplate.name);
                    CSVSuit.Data suitInfo = CSVSuit.Instance.GetConfData(suitId);

                    //套装属性
                    uint suitNum = Sys_Equip.Instance.CalSuitNumber(suitInfo.suit_id);

                    List<CSVSuitEffect.Data> allEffects = CSVSuitEffect.Instance.GetSuitEffectList(suitInfo.suit_id, Sys_Role.Instance.Role.Career);
                    foreach (CSVSuitEffect.Data effectData in allEffects)
                    {
                        bool isActive = effectData.num <= suitNum;
                        uint colorId = Sys_Equip.Instance.IsEquiped(euipItem) && isActive ? 4097u : 4098u;

                        if (effectData.effect != 0)
                        {
                            GameObject propGo = GameObject.Instantiate<GameObject>(goTemplate, goTemplate.transform.parent);
                            propGo.SetActive(true);

                            Text propName = propGo.transform.GetComponent<Text>();
                            Text propValue = propGo.transform.Find("Text1").GetComponent<Text>();

                            CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(effectData.effect);

                            string nameContent = LanguageHelper.GetTextContent(4096, effectData.num.ToString());
                            propName.text = LanguageHelper.GetLanguageColorWordsFormat(nameContent, colorId);
                            string valueContent = LanguageHelper.GetTextContent(skillInfo.desc);
                            propValue.text = LanguageHelper.GetLanguageColorWordsFormat(valueContent, colorId);
                        }
                        else
                        {
                            foreach (var attrInfo in effectData.attr)
                            {
                                uint attrId = attrInfo[0];
                                uint attrValue = attrInfo[1];

                                GameObject propGo = GameObject.Instantiate<GameObject>(goTemplate, goTemplate.transform.parent);
                                propGo.SetActive(true);

                                Text propName = propGo.GetComponent<Text>();
                                Text propValue = propGo.transform.Find("Text1").GetComponent<Text>();

                                CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);
                                string nameContent = LanguageHelper.GetTextContent(4096, effectData.num.ToString());
                                propName.text = LanguageHelper.GetLanguageColorWordsFormat(nameContent, colorId);
                                string valueContent = string.Format("{0} {1}", LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(attrId).name)
                                    , Sys_Attr.Instance.GetAttrValue(attrData, attrValue));
                                propValue.text = LanguageHelper.GetLanguageColorWordsFormat(valueContent, colorId);
                            }
                        }
                    }
                }
            }

            private Transform transform;
            private Button btnClick;
            private Text txtTitle;
            private Transform transArrow;
            private Transform transArrowExpand;
            private Transform transSub;
            private SuitEffectBase effectBase;
            private SuitEffectSuit effectSuit;

            private IListener mListener;
            private bool isExpand = false;

            public void Init(Transform trans)
            {
                transform = trans;

                btnClick = transform.Find("Title").GetComponent<Button>();
                btnClick.onClick.AddListener(OnClick);

                txtTitle = transform.Find("Title/Text_Dark").GetComponent<Text>();
                transArrow = transform.Find("Title/Image_Arrow");
                transArrowExpand = transform.Find("Title/Image_Arrow_Down");

                transSub = transform.Find("List_Small");

                effectBase = new SuitEffectBase();
                effectBase.Init(transform.Find("List_Small/Grid"));
                effectSuit = new SuitEffectSuit();
                effectSuit.Init(transform.Find("List_Small/Grid1"));
            }

            public void Register(IListener listener)
            {
                mListener = listener;
            }

            public void UpdateInfo(ItemData itemEquip, uint suitId, uint percent)
            {
                CSVSuit.Data suitInfo = CSVSuit.Instance.GetConfData(suitId);
                txtTitle.text = LanguageHelper.GetTextContent(4225, LanguageHelper.GetTextContent(suitInfo.suit_name), percent.ToString() + "%");
                effectBase.UpdateInfo(suitId);
                effectSuit.UpdateInfo(itemEquip, suitId);

                isExpand = false;
                transSub.gameObject.SetActive(isExpand);
                transArrow.gameObject.SetActive(!isExpand);
                transArrowExpand.gameObject.SetActive(isExpand);
            }

            private void OnClick()
            {
                isExpand = !isExpand;
                transSub.gameObject.SetActive(isExpand);
                transArrow.gameObject.SetActive(!isExpand);
                transArrowExpand.gameObject.SetActive(isExpand);

                mListener?.OnExpand();
            }

            public interface IListener
            {
                void OnExpand();
            }
        }

        private Button btnClose;
        private GameObject goTemplate;

        private MakePreviewParam param;

        protected override void OnLoaded()
        {            
            btnClose = transform.Find("Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);

            goTemplate = transform.Find("View_Content/Scroll01/Content/Item").gameObject;
            goTemplate.SetActive(false);
        }

        protected override void OnOpen(object arg)
        {            
            param = null;
            if (arg != null)
                param = (MakePreviewParam)arg;
        }

        protected override void OnShow()
        {
            UpdatePanel();
        }        

        private void UpdatePanel()
        {
            Lib.Core.FrameworkTool.DestroyChildren(goTemplate.transform.parent.gameObject, goTemplate.name);
            if (null == param)
                return;

            Dictionary<uint, uint> dictSuits = new Dictionary<uint, uint>();
            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(param.itemEquip.Id);
            uint totalRate = 0u;
            if (param.paperId != 0u)
            {
                if (equipInfo != null)
                {
                    int index = 0;
                    for (int i = 0; i < equipInfo.suit_item_special.Count; ++i)
                    {
                        if (param.paperId == equipInfo.suit_item_special[i][0])
                        {
                            index = i;
                            break;
                        }
                    }
                    
                    CSVSuit.Data suitInfo = CSVSuit.Instance.GetSuitData(equipInfo.suit_item_special[index][2], equipInfo.slot_id[0]);
                    dictSuits.Add(suitInfo.id, 100u);
                    totalRate += 100u;
                }
            }
            else
            {
                if (equipInfo != null && equipInfo.suit_pro_base != null)
                {
                    int count = equipInfo.suit_pro_base.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        CSVSuit.Data suitInfo = CSVSuit.Instance.GetSuitData(equipInfo.suit_pro_base[i][0], equipInfo.slot_id[0]);
                        dictSuits.Add(suitInfo.id, equipInfo.suit_pro_base[i][1]);
                        totalRate += equipInfo.suit_pro_base[i][1];
                    }
                }
            }

            foreach(var data in dictSuits)
            {
                GameObject propGo = GameObject.Instantiate<GameObject>(goTemplate, goTemplate.transform.parent);
                propGo.SetActive(true);

                SuitEffectPre effect = new SuitEffectPre();
                effect.Init(propGo.transform);
                effect.Register(this);
                effect.UpdateInfo(param.itemEquip, data.Key, data.Value * 100 / totalRate);
            }

            Lib.Core.FrameworkTool.ForceRebuildLayout(goTemplate.transform.parent.gameObject);
        }

        private void OnClickClose()
        {
            UIManager.CloseUI(EUIID.UI_Make_Preview);
        }

        public void OnExpand()
        {
            Lib.Core.FrameworkTool.ForceRebuildLayout(goTemplate.transform.parent.gameObject);
        }
    }
}
