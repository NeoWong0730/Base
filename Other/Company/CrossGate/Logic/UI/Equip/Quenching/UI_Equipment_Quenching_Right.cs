using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Logic.Core;
using Table;
using Packet;
using Google.Protobuf.Collections;

namespace Logic
{
    public class UI_Equipment_Quenching_Right : UIParseCommon
    {
        //parent
        //private GameObject rightParent;
        //private GameObject rightNoneTip;

        //Item-IconRoot
        private EquipItem equipItem;
        private Text equipName;
        private Text equipLevel;

        //Item-Material
        private PropItem propItem;
        private Text propName;
        private Text propLevel;

        private Text textCost;
        private Image imgCost;

        private Button btnQuenching;

        private ItemData curOpEquip;

        private bool isCanQuenching = false;
       
        private IListener listener;

        protected override void Parse()
        {
            //rightParent = transform.Find("View01").gameObject;
            //rightNoneTip = transform.Find("ViewNone").gameObject;

            equipItem = new EquipItem();
            equipItem.Bind(transform.Find("View01/View_Item/IconRoot/Item01/EquipItem").gameObject);
            equipItem.Layout.btnItem.onClick.AddListener(OnClickEquipment);
            equipName = transform.Find("View01/View_Item/IconRoot/Item01/Text_Name").GetComponent<Text>();
            equipLevel = transform.Find("View01/View_Item/IconRoot/Item01/Text_Type").GetComponent<Text>();
           
            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("View01/View_Item/IconRoot/Item02/PropItem01").gameObject);
            propName = transform.Find("View01/View_Item/IconRoot/Item02/Text_Name").GetComponent<Text>();
            propLevel = transform.Find("View01/View_Item/IconRoot/Item02/Text_Type").GetComponent<Text>();

            textCost = transform.Find("View01/Text_Cost").GetComponent<Text>();
            imgCost = textCost.transform.Find("Image_Coin").GetComponent<Image>();

            btnQuenching = transform.Find("View01/Button_Clear_Up").GetComponent<Button>();
            btnQuenching.onClick.AddListener(OnClickQueching);

            Button btnMsg = transform.Find("Button_Message").GetComponent<Button>();
            btnMsg.onClick.AddListener(() =>
            {
                UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(4008) });
            });
        }

        private void OnClickEquipment()
        {
            if (curOpEquip != null)
            {
                EquipTipsData tipData = new EquipTipsData();
                tipData.equip = curOpEquip;
                tipData.isCompare = false;

                UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
            }
        }

        private void OnClickQueching()
        {
            if (isCanQuenching)
            {
                if (!Sys_Equip.Instance.IsEquiped(curOpEquip))
                {
                    listener.OnClickQuenching();
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4051));
                }
            }
            else
            {
                //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4052));
            }
        }

        public void Registerlistener(IListener _listener)
        {
            listener = _listener;
        }

        public override void UpdateInfo(ItemData _itemEquip)
        {
            //rightParent.SetActive(_itemEquip != null);
            //rightNoneTip.SetActive(_itemEquip == null);

            //if (_itemEquip == null)
            //    return;

            curOpEquip = _itemEquip;
            isCanQuenching = false;

            equipItem.SetData(curOpEquip);
            equipName.text = Sys_Equip.Instance.GetEquipmentName(curOpEquip);

            CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(curOpEquip.Id);
            equipLevel.text = LanguageHelper.GetTextContent(1000002, equipData.TransLevel().ToString());

            //cost material 策划确认只消耗一种道具
            uint costId = equipData.quenching[0][0];
            uint costNum = equipData.quenching[0][1];

            CSVItem.Data propInfoItem = CSVItem.Instance.GetConfData(costId);
            PropIconLoader.ShowItemData costData = new PropIconLoader.ShowItemData(costId, costNum, true, false, false, false, false, true, true, true);
            propItem.SetData(new MessageBoxEvt( EUIID.UI_Equipment, costData));
            propName.text = LanguageHelper.GetTextContent(propInfoItem.name_id);
            propLevel.text = LanguageHelper.GetTextContent(1000002, propInfoItem.lv.ToString());

            isCanQuenching = costNum <= Sys_Bag.Instance.GetItemCount(costId);
            ImageHelper.SetImageGray(btnQuenching.image, !isCanQuenching);

            //btnQuenching.gameObject.SetActive(_itemEquip != null);
        }

        public interface IListener
        {
            void OnClickQuenching();
        }
    }
}


