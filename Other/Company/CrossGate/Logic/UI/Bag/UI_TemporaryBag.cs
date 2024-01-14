using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class UI_TemporaryBag : UIBase
    {
        private Dictionary<GameObject, CeilGrid> ceilGrids = new Dictionary<GameObject, CeilGrid>();
        private List<CeilGrid> uuidGrids = new List<CeilGrid>();
        List<ItemData> itemDatas = new List<ItemData>();
        private InfinityGridLayoutGroup infinity;
        private Transform parent;
        private Button closeBtn;
        private Button fetchBtn;
        private int curSelectDataIndex = -1;

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnRefreshTemporaryBagData, RefreshUI, toRegister);
        }


        protected override void OnShow()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            itemDatas = Sys_Bag.Instance.BagItems[6];
            infinity.SetAmount(itemDatas.Count);
        }

        protected override void OnLoaded()
        {
            closeBtn = transform.Find("Animator/View_TipsBg02_Smallest/Btn_Close").GetComponent<Button>();
            fetchBtn = transform.Find("Animator/View_List/Button_Tackout").GetComponent<Button>();
            parent = transform.Find("Animator/View_List/Scroll_View_Bag/TabList").transform;
            closeBtn.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_TemporaryBag);
            });
            fetchBtn.onClick.AddListener(OnFetchItems);

            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject go = parent.GetChild(i).gameObject;
                CeilGrid bagCeilGrid = new CeilGrid();
                bagCeilGrid.BindGameObject(go);
                bagCeilGrid.AddClickListener(OnGridSelected);

                ceilGrids.Add(go, bagCeilGrid);
                uuidGrids.Add(bagCeilGrid);
            }
            infinity = parent.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 12;
            infinity.updateChildrenCallback = UpdateChildrenCallback;
        }

        private void OnGridSelected(CeilGrid bagCeilGrid)
        {
            curSelectDataIndex = bagCeilGrid.gridIndex;
            foreach (var item in uuidGrids)
            {
                if (item.gridIndex == curSelectDataIndex)
                {
                    item.Select();

                    if (bagCeilGrid.mItemData.cSVItemData.type_id == (uint)EItemType.Equipment)
                    {
                        EquipTipsData tipData = new EquipTipsData();
                        tipData.equip = bagCeilGrid.mItemData;
                        tipData.isShowOpBtn = false;

                        UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
                    }
                    else if (bagCeilGrid.mItemData.cSVItemData.type_id == (uint)EItemType.Ornament)
                    {
                        OrnamentTipsData tipData = new OrnamentTipsData();
                        tipData.equip = bagCeilGrid.mItemData;
                        tipData.isCompare = false;
                        tipData.sourceUiId = EUIID.UI_TemporaryBag;

                        UIManager.OpenUI(EUIID.UI_Tips_Ornament, false, tipData);
                    }
                    else
                    {
                        if (item.mItemData != null)
                        {
                            PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(item.mItemData.Id,
                                item.mItemData.Count, true, item.mItemData.bBind, item.mItemData.bNew, false, false);
                            UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_TemporaryBag, showItemData));
                        }
                    }
                }
                else
                {
                    item.Release();
                }
            }
        }


        private void OnFetchItems()
        {
            if (Sys_Bag.Instance.BagItems[6].Count == 0)
                return;
            uint boxid;
            if (JudgeMainBagFull(out boxid))
            {
                string tabName = CSVBoxType.Instance.GetConfData(boxid).tab_name;
                string str = CSVLanguage.Instance.GetConfData(uint.Parse(tabName)).words;
                string content = string.Format(LanguageHelper.GetTextContent(1000922), str);
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }
            Sys_Bag.Instance.MergeTemporaryBoxReq();
        }

        private bool JudgeMainBagFull(out uint boxid)
        {
            List<uint> box_id = new List<uint>();
            for (int i = 0; i < itemDatas.Count; i++)
            {
                if (!box_id.Contains(itemDatas[i].cSVItemData.box_id))
                {
                    box_id.Add(itemDatas[i].cSVItemData.box_id);
                }
            }
            for (int i = 0; i < box_id.Count; i++)
            {
                boxid = box_id[i];
                int datacount = Sys_Bag.Instance.BagItems[(int)boxid].Count;
                int MaxCount = Sys_Bag.Instance.GetBoxMaxCeilCount(boxid);
                if (datacount >= MaxCount)
                {
                    return true;
                }
            }
            boxid = 0;
            return false;
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index >= itemDatas.Count)
                return;
            CeilGrid taskDataGrid = ceilGrids[trans.gameObject];
            if (curSelectDataIndex != index)
            {
                taskDataGrid.Release();
            }
            taskDataGrid.SetData(itemDatas[index], index, CeilGrid.EGridState.Normal, CeilGrid.ESource.e_TemplateBag);
        }
    }
}

