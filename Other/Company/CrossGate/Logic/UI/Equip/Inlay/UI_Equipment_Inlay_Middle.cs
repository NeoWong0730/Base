using System.Collections;
using System.Collections.Generic;
using System;
using Framework;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using Logic.Core;

namespace Logic
{
    public class UI_Equipment_Inlay_Middle : UIParseCommon, UI_Equipment_Inlay_Slot.IListener
    {
        //宝石等级总和添加的属性
        public class GemLevSumAttr
        {
            private Transform transform;
            private CP_Toggle toggle;
            private Text txtName;
            private Text txtValue;
            private int intIndex;

            public void Init(Transform trans)
            {
                transform = trans;
                toggle = transform.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);
                txtName = transform.Find("Text").GetComponent<Text>();
                txtValue = transform.Find("Text/Text1").GetComponent<Text>();
            }

            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    Sys_Equip.Instance.OnJewelLevelAttrSelectReq(intIndex);
                }
            }

            public void SetIndex(int index)
            {
                intIndex = index;
            }

            public void OnSelect(bool isSelect)
            {
                toggle.SetSelected(isSelect, true);
            }

            public void UpdateInfo(Int2 data)
            {
                uint attrId = (uint)data.X;
                uint attrValue = (uint) data.Y;
                CSVAttr.Data attr = CSVAttr.Instance.GetConfData(attrId);
                txtName.text = LanguageHelper.GetTextContent(attr.name);
                txtValue.text = string.Format("+{0}", Sys_Attr.Instance.GetAttrValue(attr, attrValue));
            }
        }
        
        //parent
        private GameObject rightItemParent;
        //private GameObject rightNoneTip;
        private UI_Equipment_Inlay_Middle_Attr rightAttr;

        private Text textTopLevelTip;

        //equip op variables
        private EquipItem2 opEquipItem;
        private Text opEquipName;
        //private Text opEquipType;
        private Text opEquipLvl;

        private Text transGemSlotTips;
        private Transform transGemSlot;
        private Button btnGemSlotTip;
        private InfinityGrid _infinityGrid;
        private List<Int2> listGemLevDatas = new List<Int2>();
        private int gemLevIndex = 0;

        private List<UI_Equipment_Inlay_Slot> inlaySlots = new List<UI_Equipment_Inlay_Slot>();

        private ItemData curOpEquip;
        private ulong _uuId;

        private IListener curListener;

        protected override void Parse()
        {
            rightItemParent = transform.Find("View01").gameObject;
            //rightNoneTip = transform.Find("ViewNone").gameObject;
            rightAttr = new UI_Equipment_Inlay_Middle_Attr();
            rightAttr.Init(transform.Find("View_Attribute"));

            opEquipItem = new EquipItem2();
            opEquipItem.Bind(transform.Find("View01/View_Item/EquipItem2").gameObject);
            opEquipItem.btn.onClick.AddListener(OnClickEquipment);

            opEquipName = transform.Find("View01/View_Item/Text_Name").GetComponent<Text>();
            //opEquipType = transform.Find("View01/View_Item/Text_Type").GetComponent<Text>();
            opEquipLvl = transform.Find("View01/View_Item/Text_Level").GetComponent<Text>();
            textTopLevelTip = transform.Find("View01/Text_Tips02").GetComponent<Text>();

            for (uint i = 0; i < 3; ++i)
            {
                string strnode = string.Format("View01/View_Item/Gem/GemItem{0}", i + 1);
                UI_Equipment_Inlay_Slot inlaySlot = new UI_Equipment_Inlay_Slot();
                inlaySlot.Init(transform.Find(strnode));
                inlaySlot.inlayPos = i;
                inlaySlot.ResiterListener(this);
                inlaySlots.Add(inlaySlot);
            }

            transGemSlotTips = transform.Find("Text_Tips").GetComponent<Text>();
            transGemSlot = transform.Find("Add_Attr");
            btnGemSlotTip = transform.Find("Add_Attr/Text_Tips02/Button").GetComponent<Button>();
            btnGemSlotTip.onClick.AddListener(OnClickGemTip);
            
            _infinityGrid = transform.Find("Add_Attr/Scroll View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void OnDestroy()
        {
            for (int i = 0; i < inlaySlots.Count; ++i)
            {
                inlaySlots[i].OnDestroy();
            }
        }

        private void OnClickGemTip()
        {
            UIManager.OpenUI(EUIID.UI_Equipment_Inlay_GemTip);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            GemLevSumAttr lev = new GemLevSumAttr();
            lev.Init(cell.mRootTransform);
            cell.BindUserData(lev);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            GemLevSumAttr lev = cell.mUserData as GemLevSumAttr;
            lev.SetIndex(index);
            lev.OnSelect(index == gemLevIndex);
            lev.UpdateInfo(listGemLevDatas[index]);
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

        public void RegisterListener(IListener listener)
        {
            curListener = listener;
        }

        public void OnUnloadJewelPos(uint _pos)
        {
            if (curOpEquip != null)
            {
                Sys_Equip.Instance.UnloadJewelReq(curOpEquip.Uuid, _pos);
            }
            else
            {
                Debug.LogError("InlayGem Error!!!!");
            }
        }

        public void OnSelectInlayPos(uint _pos)
        {
            //selectInlayPos = _pos;
            for (int i = 0; i < inlaySlots.Count; ++i)
                inlaySlots[i].OnSelect(inlaySlots[i].inlayPos == _pos);

            Sys_Equip.Instance.InlaySlotPos = _pos;
            curListener?.OnSelectInlaySlot(curOpEquip, inlaySlots[(int)_pos].jewelInfoId);
        }

        public override void UpdateInfo(ItemData _item)
        {
            curOpEquip = _item;

            _uuId = curOpEquip != null ? curOpEquip.Uuid : 0L;

            opEquipItem.SetData(curOpEquip);

            opEquipName.text = Sys_Equip.Instance.GetEquipmentName(curOpEquip);

            CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(curOpEquip.Id);

            opEquipLvl.text = LanguageHelper.GetTextContent(1000002, equipData.TransLevel().ToString());

            textTopLevelTip.text = LanguageHelper.GetTextContent(4001, equipData.jewel_level.ToString());

            //gem info
            uint slotNum = equipData.jewel_number;
            for (int i = 0; i < inlaySlots.Count; ++i)
            {
                inlaySlots[i].gameObject.SetActive(i < slotNum);
                inlaySlots[i].UpdateInfo(0, curOpEquip);
            }

            for (int i = 0; i < curOpEquip.Equip.JewelinfoId.Count; ++i)
            {
                uint jewelInfoId = curOpEquip.Equip.JewelinfoId[i];
                if (jewelInfoId != 0)
                    inlaySlots[i].UpdateInfo(jewelInfoId, curOpEquip);
            }

            //cal default selectPos
            uint defaultInlayPos = 0;
            for (int i = 0; i < curOpEquip.Equip.JewelinfoId.Count; ++i)
            {
                ulong jewelUUId = curOpEquip.Equip.JewelinfoId[i];
                if (jewelUUId == 0L)
                {
                    defaultInlayPos = (uint)i;
                    break;
                }
            }
            OnSelectInlayPos(defaultInlayPos);

            rightAttr.UpdateInfo(curOpEquip);

            #region 宝石等级总和属性加成

            int tempIndex = -1;
            uint gemSumLev = 0;
            for (int i = 0; i < curOpEquip.Equip.JewelinfoId.Count; ++i)
            {
                uint jewelInfoId = curOpEquip.Equip.JewelinfoId[i];
                if (jewelInfoId != 0)
                {
                    CSVJewel.Data jewInfo = CSVJewel.Instance.GetConfData(jewelInfoId);
                    gemSumLev += jewInfo.level;
                }
            }

            if (equipData.jew_lev_score != null) //没配置 就没有选项
            {
                for (int i = equipData.jew_lev_score.Count - 1; i >= 0; --i)
                {
                    if (gemSumLev >= equipData.jew_lev_score[i][0])
                    {
                        tempIndex = i;
                        break;
                    }
                }
            }
            
            transGemSlot.gameObject.SetActive(tempIndex >= 0);
            listGemLevDatas.Clear();
            if (tempIndex >= 0) //需要显示属性
            {
                List<uint> attrList = equipData.jew_lev_attr[tempIndex];
                int count = attrList.Count / 2;
                for (int i = 0; i < count; ++i)
                {
                    int startIndex = i * 2;
                    Int2 temp = new Int2();
                    temp.X = (int)attrList[startIndex];
                    temp.Y = (int)attrList[startIndex + 1];
                    listGemLevDatas.Add(temp);
                }

                transGemSlotTips.text = "";
            }
            else
            {
                transGemSlotTips.text = LanguageHelper.GetTextContent(4258, equipData.jew_lev_score[0][0].ToString());
            }

            gemLevIndex = (int)curOpEquip.Equip.JewelLevleAttrSelect;
            _infinityGrid.CellCount = listGemLevDatas.Count;
            _infinityGrid.ForceRefreshActiveCell();

            #endregion 宝石等级总和属性加成
        }

        #region NetNoty
        public void OnRefreshEquipment()
        {
            if (_uuId != 0)
            {
                curOpEquip = Sys_Equip.Instance.GetItemData(_uuId);
                UpdateInfo(curOpEquip);
            }
        }
        #endregion

        public interface IListener
        {
            void OnSelectInlaySlot(ItemData equipItem, uint jewelInfoId);
        }
    }
}


