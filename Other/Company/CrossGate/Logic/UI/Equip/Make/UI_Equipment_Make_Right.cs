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
    public class UI_Equipment_Make_Right : UIParseCommon, UI_Equipment_Make_Right_Select.IListener
    {
        private class PropCell
        {
            private Transform transform;
            public GameObject parent;

            public PropItem item;
            public Text name;

            public void Init(Transform trans)
            {
                transform = trans;
                parent = transform.gameObject;

                item = new PropItem();
                item.BindGameObject(transform.Find("PropItem").gameObject);

                name = transform.Find("Text_Name").GetComponent<Text>();
            }
        }

        private List<PropCell> propList = new List<PropCell>();

        private Button btnAdd;

        private PropItem propPaper;
        private Text textPropPaper;

        private EquipItem2 equipItem;
        private Text equipName;
        private Text equiplevel;
        
        private Button btnMake;
        private Button btnPreview;

        private Text textCost;
        private Image imgCost;

        private UI_Equipment_Make_Right_Select selectGo;
        private UI_Equipment_Make_Right_Property_Before propBeforeGo;
        private UI_Equipment_Make_Right_Property_After propAfterGo;

        private ItemData curOpEquip;
        private ItemData curPaper;

        private uint paperId;
        private bool canMake;

        private IListener listener;

        protected override void Parse()
        {
            btnAdd = transform.Find("View01/View_Item/Btn_Add").GetComponent<Button>();
            btnAdd.onClick.AddListener(OnClickAddPaper);

            propPaper = new PropItem();
            propPaper.BindGameObject(transform.Find("View01/View_Item/PropItem01").gameObject);
            textPropPaper = transform.Find("View01/View_Item/Text_Name").GetComponent<Text>();

            propList.Clear();
            for(int i = 1; i < 4; i++)
            {
                PropCell cell = new PropCell();
                cell.Init(transform.Find(string.Format("View01/View_Item/Item{0}", i)));
                propList.Add(cell);
            }

            //equip root
            equipName = transform.Find("View01/Text_Name").GetComponent<Text>();
            equiplevel = transform.Find("View01/Text_Level").GetComponent<Text>();

            equipItem = new EquipItem2();
            equipItem.Bind(transform.Find("View01/EquipItem2").gameObject);
            equipItem.btn.onClick.AddListener(OnClickEquipment);

            textCost = transform.Find("View01/Text_Cost").GetComponent<Text>();
            imgCost = transform.Find("View01/Text_Cost/Image_Coin").GetComponent<Image>();
            textCost.gameObject.SetActive(false);
            
            btnMake = transform.Find("View01/Button_Clear_Up").GetComponent<Button>();
            btnMake.onClick.AddListener(OnClickMake);

            btnPreview = transform.Find("View01/Image_Preview").GetComponent<Button>();
            btnPreview.onClick.AddListener(OnClickPreview);

            selectGo = new UI_Equipment_Make_Right_Select();
            selectGo.Init(transform.Find("Object_Tips"));
            selectGo.Register(this);

            propBeforeGo = new UI_Equipment_Make_Right_Property_Before();
            propBeforeGo.Init(transform.Find("Suit_Before"));
            propAfterGo = new UI_Equipment_Make_Right_Property_After();
            propAfterGo.Init(transform.Find("Suit_After"));
        }

        private void OnClickAddPaper()
        {
            selectGo.Show();
        }

        private void OnClickMake()
        {
            if (canMake)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(4229);
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    //TODO: 发送打造套装请求
                    Sys_Equip.Instance.OnEquipmentBuildReq(curOpEquip, curPaper);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            else
            {
                //TODO: 材料不足提示
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4093));
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

        private void OnClickPreview()
        {
            MakePreviewParam param = new MakePreviewParam();
            param.itemEquip = curOpEquip;
            param.paperId = paperId;

            UIManager.OpenUI(EUIID.UI_Make_Preview, false, param);
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public  override void UpdateInfo(ItemData  item)
        {
            curOpEquip = item;

            equipItem.SetData(curOpEquip);

            CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(curOpEquip.Id);

            equipName.text = Sys_Equip.Instance.GetEquipmentName(curOpEquip);
            equiplevel.text = LanguageHelper.GetTextContent(1000002, equipData.TransLevel().ToString());

            btnAdd.gameObject.SetActive(true);
            propPaper.transform.gameObject.SetActive(false);
            textPropPaper.text = "";
            propBeforeGo.Hide();
            propAfterGo.Hide();

            selectGo.Hide();
            selectGo.UpdateInfo(curOpEquip);

            curPaper = null;
            paperId = 0u;
            canMake = true;
            for (int i = 0; i < equipData.suit_item_base.Count; ++i)
            {
                if (i < propList.Count)
                {
                    PropCell cell = propList[i];
                    cell.parent.SetActive(true);

                    uint propId = equipData.suit_item_base[i][0];
                    uint propNum = equipData.suit_item_base[i][1];

                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(propId, propNum, true, false, false, false, false, true, true, true);
                    cell.item.SetData(new MessageBoxEvt(EUIID.UI_Equipment, itemData));

                    cell.name.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(propId).name_id);

                    canMake &= Sys_Bag.Instance.GetItemCount(propId) >= propNum;
                }
            }

            ImageHelper.SetImageGray(btnMake.image, !canMake, true);

            //有套装属性,没有也会显示none提示
            propBeforeGo.Show();
            propBeforeGo.UpdateInfo(curOpEquip);

            //有属性未替换
            if (curOpEquip.Equip.SuitAttrTmp.Count != 0 && curOpEquip.Equip.SuitTypeIdTmp != 0)
            {
                propAfterGo.Show();
                propAfterGo.UpdateInfo(curOpEquip, curOpEquip.Equip.SuitTypeIdTmp);
            }
        }

        public void OnSelectPaper(ItemData paperItem)
        {
            selectGo.Hide();
            btnAdd.gameObject.SetActive(false);

            curPaper = paperItem;
            paperId = paperItem.cSVItemData.id;
            //canMake = true;

            //set paper
            propPaper.transform.gameObject.SetActive(true);
            PropIconLoader.ShowItemData paperItemData = new PropIconLoader.ShowItemData(paperId, 1, true, false, false, false, false, true, true, true, OnClickPaper);
            propPaper.SetData(new MessageBoxEvt(EUIID.UI_Equipment, paperItemData));
            propPaper.OnEnableLongPress(true);
            textPropPaper.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(paperId).name_id);

            //ImageHelper.SetImageGray(btnMake.image, paperId == 0u, true);
        }

        private void OnClickPaper(PropItem item)
        {
            curPaper = null;
            paperId = 0u;
            propPaper.transform.gameObject.SetActive(false);
            textPropPaper.text = ""; 
            btnAdd.gameObject.SetActive(true);
        }

        public interface IListener
        {
            void OnClickMake();
        }
    }
}


