using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Table;
using Packet;
using Google.Protobuf.Collections;
using Logic.Core;

namespace Logic
{
    public class UI_Equipment_Smelt_Right : UIParseCommon
    {
        //parent
        //private GameObject rightParent;
        //private GameObject rightNoneTip;

        //Item-EquipRoot
        private EquipItem2 equipItem;
        private Text textEquipName;
        private Text textEquipLevel;
        private Image imgEquiped;

        //Item-Material
        private GameObject materialParent;
        private GameObject materialTemplate;
        private Text textMaterial;

        //private Text textCost;
        //private Image imgCost;

        //Property
        private class AttrSmelt
        {
            public uint fromAttrUId;
            public uint attrId;
            public int attrValue;
            public int resultValue;
            public int diff;

            public void Reset()
            {
                fromAttrUId = 0u;
                attrId = 0u;
                attrValue = 0;
                resultValue = 0;
                diff = 0;
            }

            public GameObject rootAttr;
            public Text textName;
            public Text textValue;
            public Image imgArrow;
            public Text textDiff;
            public Text textResult;
        }
        private List<AttrSmelt> listAttr = new List<AttrSmelt>();
        //private GameObject propParent;
        //private GameObject propTemplate;

        private Text textRevertCost;
        private Image imgRevertCost;
        private Button btnRevert;
        private Button btnSmelt;
        private Button btnMsg;

        private bool IsCanSmelt = true;

        private IListener listener;

        private ItemData curOpItem;

        protected override void Parse()
        {
            //rightParent = transform.Find("View01").gameObject;
            //rightNoneTip = transform.Find("ViewNone").gameObject;

            Button btnPreview = transform.Find("Image_Preview").GetComponent<Button>();
            btnPreview.onClick.AddListener(() =>
            {
                UIManager.OpenUI(EUIID.UI_Tips_Preview, false, curOpItem);
            });

            equipItem = new EquipItem2();
            equipItem.Bind(transform.Find("View01/EquipItem2").gameObject);
            equipItem.btn.onClick.AddListener(OnClickEquip);

            textEquipName = transform.Find("View01/Text_Name").GetComponent<Text>();
            textEquipLevel = transform.Find("View01/Text_Level").GetComponent<Text>();

            materialParent = transform.Find("View01/Grid").gameObject;
            materialTemplate = transform.Find("View01/Grid/PropItem").gameObject;
            materialTemplate.SetActive(false);

            textMaterial = transform.Find("View01/Text_Cost1").GetComponent<Text>();

            //textCost = transform.Find("View01/Text_Cost").GetComponent<Text>();
            //imgCost = transform.Find("View01/Text_Cost/Image_Coin").GetComponent<Image>();

            Transform parent = transform.Find("View01/View_Property/ScrollView/GameObject");
            for (int i = 0; i < parent.childCount; ++i)
            {
                Transform trans = parent.GetChild(i);

                AttrSmelt attr = new AttrSmelt();
                attr.rootAttr = trans.gameObject;
                attr.textName = trans.Find("Text_Property").GetComponent<Text>();
                attr.textValue = trans.Find("Attribute_Smelt/Text_Num").GetComponent<Text>();
                attr.textDiff = trans.Find("Attribute_Smelt/Text_Num1").GetComponent<Text>();
                attr.imgArrow = trans.Find("Attribute_Smelt/Image_Arrow").GetComponent<Image>();
                attr.textResult = trans.Find("Attribute_Smelt/Text_Num2").GetComponent<Text>();

                listAttr.Add(attr);
            }
            //propParent = transform.Find("View01/View_Property/ScrollView/GameObject").gameObject;
            //propTemplate = propParent.transform.Find("View_Attribute").gameObject;
            //propTemplate.SetActive(false);
            textRevertCost = transform.Find("View01/View_Property/Text_Cost").GetComponent<Text>();
            imgRevertCost = transform.Find("View01/View_Property/Text_Cost/Image_Coin").GetComponent<Image>();

            btnRevert = transform.Find("View01/View_Property/Button_Clear").GetComponent<Button>();
            btnRevert.onClick.AddListener(OnClickRevert);

            btnSmelt = transform.Find("View01/Button_Clear_Up").GetComponent<Button>();
            btnSmelt.onClick.AddListener(OnClickSmelt);

            btnMsg = transform.Find("View01/View_Property/Button_Message").GetComponent<Button>();
            btnMsg.onClick.AddListener(() =>
            {
                UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(4006) });
            });
        }

        private void OnClickEquip()
        {
            EquipTipsData tipData = new EquipTipsData();
            tipData.equip = curOpItem;
            tipData.isCompare = false;

            UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
        }

        private void OnClickRevert()
        {
            CSVEquipment.Data infoData = CSVEquipment.Instance.GetConfData(curOpItem.Id);
            if (infoData != null)
            {
                if (infoData.re_smelt[0][1] > Sys_Bag.Instance.GetItemCount(infoData.re_smelt[0][0]))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4052));
                }
                else
                {
                    listener.OnClickRevert();
                }
            }
        }

        private void OnClickSmelt()
        {
            if (!IsCanSmelt)
            {
                //CSVItem.Data matItemData = CSVItem.Instance.GetConfData(matItemId);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4052));
            }
            else
            {
                listener.OnClickSmelt();
            }
        }

        public void Registerlistener(IListener _listener)
        {
            listener = _listener;
        }

        public override  void UpdateInfo(ItemData _item)
        {
            //rightParent.SetActive(_item != null);
            //rightNoneTip.SetActive(_item == null);

            //if (_item == null)
            //    return;

            curOpItem = _item;
            equipItem.SetData(_item);

            textEquipName.text = Sys_Equip.Instance.GetEquipmentName(curOpItem); 

            CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(curOpItem.Id);
            textEquipLevel.text = LanguageHelper.GetTextContent(1000002, equipData.TransLevel().ToString());

            IsCanSmelt = true;
            Lib.Core.FrameworkTool.DestroyChildren(materialParent, materialTemplate.name);

            if (equipData.smelt != null)
            {
                for (int i = 0; i < equipData.smelt.Count; ++i)
                {
                    uint costId = equipData.smelt[i][0];
                    uint costNum = equipData.smelt[i][1];

                    GameObject propGo = GameObject.Instantiate<GameObject>(materialTemplate, materialParent.transform);
                    propGo.SetActive(true);

                    PropItem propItem = new PropItem();
                    propItem.BindGameObject(propGo);

                    PropIconLoader.ShowItemData costData = new PropIconLoader.ShowItemData(costId, costNum, true, false, false, false, false, true, true, true);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_Equipment, costData));

                    if (IsCanSmelt)
                        IsCanSmelt = costNum <= Sys_Bag.Instance.GetItemCount(costId);
                }
            }

            ImageHelper.SetImageGray(btnSmelt.image, !IsCanSmelt);

            bool isCanRevert = curOpItem.Equip.RevertSmeltData.SmeltAttrUID != 0;
            btnRevert.gameObject.SetActive(isCanRevert);
            textRevertCost.gameObject.SetActive(isCanRevert);
            if (isCanRevert)
            {
                if (equipData != null)
                {
                    CSVItem.Data itemCost = CSVItem.Instance.GetConfData(equipData.re_smelt[0][0]);
                    ImageHelper.SetIcon(imgRevertCost, itemCost.small_icon_id);
                    textRevertCost.text = equipData.re_smelt[0][1].ToString();
                }
            }

            CalAttr();
        }

        private void CalAttr()
        {
            for (int i = 0; i < listAttr.Count; ++i)
                listAttr[i].Reset();

            //策划说,最多5条 基础配置 + 绿字属性
            //basic attr
            CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(curOpItem.Id);
            int index = 0;
            for (int i = 0; i < equipData.attr.Count; ++i)
            {
                listAttr[index].attrId = equipData.attr[i][0];
                ++index;
            }

            //green attr
            for (int i = 0; i < curOpItem.Equip.GreenAttr.Count; ++i)
            {
                listAttr[index].attrId = curOpItem.Equip.GreenAttr[i].Attr2.Id;
                ++index;
            }

            //smelt attr
            for (int i = 0; i < curOpItem.Equip.SmeltAttr.Count; ++i)
            {
                for (int j = 0; j < listAttr.Count; ++j)
                {
                    if (curOpItem.Equip.SmeltAttr[i].Attr.Attr2.Id == listAttr[j].attrId
                       && listAttr[j].fromAttrUId == 0u)
                    {
                        listAttr[j].attrValue = listAttr[j].resultValue = curOpItem.Equip.SmeltAttr[i].Attr.Attr2.Value;
                        listAttr[j].fromAttrUId = curOpItem.Equip.SmeltAttr[i].FromAttrUID;
                        break;
                    }
                }
            }

            //计算熔炼变化
            if (curOpItem.Equip.RevertSmeltData != null)
            {
                for (int i = 0; i < listAttr.Count; ++i)
                {
                    if (listAttr[i].fromAttrUId == curOpItem.Equip.RevertSmeltData.SmeltAttrUID)
                    {
                        listAttr[i].attrValue = curOpItem.Equip.RevertSmeltData.OldValue;
                        listAttr[i].diff = listAttr[i].resultValue - listAttr[i].attrValue;
                        break;
                    }
                }
            }

            for (int i = 0; i < listAttr.Count; ++i)
            {
                ShowAttr(listAttr[i]);
            }
        }

        private void ShowAttr(AttrSmelt attrInfo)
        {
            if (attrInfo.attrId == 0u) //没有属性
            {
                attrInfo.rootAttr.SetActive(false);
                return;
            }

            attrInfo.rootAttr.SetActive(true);

            CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrInfo.attrId);
            if (attrInfo.diff == 0u) //没变化
            {
                attrInfo.textName.text = LanguageHelper.GetTextContent(attrData.name);
                attrInfo.textValue.text = Sys_Attr.Instance.GetAttrValue(attrData, attrInfo.attrValue);
                attrInfo.textDiff.gameObject.SetActive(false);
                attrInfo.imgArrow.gameObject.SetActive(false);
                attrInfo.textResult.gameObject.SetActive(false);
            }
            else
            {
                attrInfo.textName.text = LanguageHelper.GetTextContent(attrData.name);
                attrInfo.textValue.text = Sys_Attr.Instance.GetAttrValue(attrData, attrInfo.attrValue);

                if (attrInfo.diff > 0)
                {
                    TextHelper.SetText(attrInfo.textResult, Sys_Attr.Instance.GetAttrValue(attrData, attrInfo.resultValue), LanguageHelper.GetTextStyle(74));
                    TextHelper.SetText(attrInfo.textDiff, string.Format("+{0}", Sys_Attr.Instance.GetAttrValue(attrData, attrInfo.diff)), LanguageHelper.GetTextStyle(74));
                }
                else
                {
                    TextHelper.SetText(attrInfo.textResult, Sys_Attr.Instance.GetAttrValue(attrData, attrInfo.resultValue), LanguageHelper.GetTextStyle(75));
                    TextHelper.SetText(attrInfo.textDiff, Sys_Attr.Instance.GetAttrValue(attrData, attrInfo.diff), LanguageHelper.GetTextStyle(75));
                }

                attrInfo.textDiff.gameObject.SetActive(true);
                attrInfo.imgArrow.gameObject.SetActive(true);
                attrInfo.textResult.gameObject.SetActive(true);
            }
        }

        public interface IListener
        {
            void OnClickRevert();
            void OnClickSmelt();
        }
    }
}


