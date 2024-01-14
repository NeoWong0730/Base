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
    public class UI_Equipment_Repair_Right : UIParseCommon
    {
        //parent
        //private GameObject rightParent;
        //private GameObject rightNoneTip;

        //Tip
        //private Text Tips01;
        private Text TipsNum;
        private Text TipsRedNum;

        //Item-IconRoot
        private EquipItem2 equipItem;
        private Text equipName;
        private Text equiplevel;
        private Text equipRepair;
        private Text equipRepairNum;

        //materialItem
        private GameObject materialParent;
        private GameObject materialTemplate;

        private Button btnRepairNormal;
        private Button btnRepairStrong;

        private Text textCost;
        private Image imgCost;

        private ItemData curOpEquip;

        private bool isCommonRepair, isStrongRepair;
        private uint repairMaxNum;
        private uint redNum;

        private IListener listener;

        protected override void Parse()
        {
            //rightParent = transform.Find("View01").gameObject;
            //rightNoneTip = transform.Find("ViewNone").gameObject;

            TipsNum = transform.Find("View01/Text_Tips01").GetComponent<Text>();
            TipsRedNum = transform.Find("View01/Text_Tips01/Text_Amount").GetComponent<Text>();
            //Tips01 = rightParent.transform.Find("Text_Tips02").GetComponent<Text>();

            //equip root
            equipName = transform.Find("View01/View_Item/IconRoot/Text_Name").GetComponent<Text>();
            equiplevel = transform.Find("View01/View_Item/IconRoot/Text_Type").GetComponent<Text>();
            //EquipProfession = rightParent.transform.Find("View_Item/IconRoot/Text_Profession").GetComponent<Text>();
            equipRepair = transform.Find("View01/View_Item/IconRoot/Text_Repair").GetComponent<Text>();
            equipRepairNum = transform.Find("View01/View_Item/IconRoot/Text_RepairNum").GetComponent<Text>();

            equipItem = new EquipItem2();
            equipItem.Bind(transform.Find("View01/View_Item/IconRoot/EquipItem2").gameObject);
            equipItem.btn.onClick.AddListener(OnClickEquipment);

            materialParent = transform.Find("View01/Grid_CostItem").gameObject;
            materialTemplate = materialParent.transform.Find("PropItem").gameObject;
            materialTemplate.SetActive(false);

            textCost = transform.Find("View01/Text_Cost").GetComponent<Text>();
            imgCost = transform.Find("View01/Text_Cost/Image_Coin").GetComponent<Image>();
            
            btnRepairNormal = transform.Find("View01/Button_Repair").GetComponent<Button>();
            btnRepairNormal.onClick.AddListener(OnClickNormal);

            btnRepairStrong = transform.Find("View01/Button_Strong").GetComponent<Button>();
            btnRepairStrong.onClick.AddListener(OnClickStrong);

            uint.TryParse(CSVParam.Instance.GetConfData(209).str_value, out repairMaxNum);
            uint.TryParse(CSVParam.Instance.GetConfData(217).str_value, out redNum);
        }

        private void OnClickNormal()
        {
            if (!isCommonRepair)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4052));
            }
            else
            {
                if (LeftRepairNum() > 0)
                {
                    if (!IsDurabilityMax())
                    {
                        listener.OnClickRepairNormal();
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4049));
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4090));
                }
            }
        }

        private void OnClickStrong()
        {
            if (!isStrongRepair)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4052));
            }
            else
            {
                if (!IsDurabilityMax())
                {
                    listener.OnClickRepairStrong();
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4049));
                }
            }
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

        private bool IsDurabilityMax()
        {
            return curOpEquip.Equip.DurabilityData.CurrentDurability >= curOpEquip.Equip.DurabilityData.MaxDurability;
        }

        private uint LeftRepairNum()
        {
            if (repairMaxNum >= curOpEquip.Equip.DurabilityData.CommonRepairTimes)
                return repairMaxNum - curOpEquip.Equip.DurabilityData.CommonRepairTimes;
            else
                return 0;
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public  override void UpdateInfo(ItemData  item)
        {
            //rightParent.SetActive(item != null);
            //rightNoneTip.SetActive(item == null);

            //if (item == null)
            //    return;

            curOpEquip = item;

            equipItem.SetData(curOpEquip);

            equipName.text = Sys_Equip.Instance.GetEquipmentName(curOpEquip);
            
            equipRepairNum.text = string.Format("{0}/{1}", curOpEquip.Equip.DurabilityData.CurrentDurability, curOpEquip.Equip.DurabilityData.MaxDurability);

            CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(curOpEquip.Id);
            equiplevel.text = LanguageHelper.GetTextContent(1000002, equipData.TransLevel().ToString());

            isCommonRepair = isStrongRepair = true;

            //strong repair
            Lib.Core.FrameworkTool.DestroyChildren(materialParent, materialTemplate.name);

            if (equipData.intensify_repair != null)
            {
                for (int i = 0; i < equipData.intensify_repair.Count; ++i)
                {
                    uint costId = equipData.intensify_repair[i][0];
                    uint costNum = equipData.intensify_repair[i][1];

                    GameObject propGo = GameObject.Instantiate<GameObject>(materialTemplate, materialParent.transform);
                    propGo.SetActive(true);

                    PropItem propItem = new PropItem();
                    propItem.BindGameObject(propGo);

                    PropIconLoader.ShowItemData costData = new PropIconLoader.ShowItemData(costId, costNum, true, false, false, false, false, true, true, true);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_Equipment, costData));

                    if (isStrongRepair)
                        isStrongRepair = costNum <= Sys_Bag.Instance.GetItemCount(costId);
                }
            }

            //common repair , 策划确定只消耗货币道具
            TipsNum.text = LanguageHelper.GetTextContent(4009);
            uint leftNum = LeftRepairNum();
            bool isRed = leftNum < redNum;
            uint colorId = isRed ? (uint)1000998 : 1000997;
            TipsRedNum.text = LanguageHelper.GetLanguageColorWordsFormat(leftNum.ToString(), colorId);

            uint costCommonId = equipData.common_repair[0][0];
            uint costCommonNum = equipData.common_repair[0][1];
            long totalNum = Sys_Bag.Instance.GetItemCount(costCommonId);
            isCommonRepair = costCommonNum <= totalNum;
            string temp = string.Format("{0}/{1}", Sys_Bag.Instance.GetValueFormat(totalNum), costCommonNum.ToString());
            uint costColorId = isCommonRepair ? (uint)1000997 : 1000998;
            textCost.text = LanguageHelper.GetLanguageColorWordsFormat(temp, costColorId);
            ImageHelper.SetIcon(imgCost, CSVItem.Instance.GetConfData(costCommonId).icon_id);

            ImageHelper.SetImageGray(btnRepairStrong.image, !isStrongRepair);
            ImageHelper.SetImageGray(btnRepairNormal.image, !isCommonRepair);
            
        }

        public interface IListener
        {
            void OnClickRepairNormal();
            void OnClickRepairStrong();
        }
    }
}


