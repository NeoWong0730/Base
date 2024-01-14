using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Packet;

namespace Logic
{
    public class UI_JSBattle_Quick_Tip : UIBase
    {
        #region 界面组件
        private Text tipsText;
        private InfinityGrid infinityGrid;
        #endregion
        private CmdVictoryArenaFastChallengeRes res;
        List<CmdVictoryArenaFastChallengeRes.Types.ItemCount> items = new List<CmdVictoryArenaFastChallengeRes.Types.ItemCount>();
        
        protected override void OnLoaded()
        {
            transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>().onClick.AddListener(CloseBtnClicked);
            transform.Find("Animator/Btn_01").GetComponent<Button>().onClick.AddListener(CloseBtnClicked);
            tipsText = transform.Find("Animator/Layout/Text").GetComponent<Text>();
            infinityGrid = transform.Find("Animator/Layout/Scroll View").GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell = OnCreateCell;
            infinityGrid.onCellChange = OnCellChange;
        }

        protected override void OnOpen(object arg = null)
        {
            if (null != arg)
            {
                res = arg as CmdVictoryArenaFastChallengeRes;
                for (int i = 0; i < res.Items.Count; i++)
                {
                    var item = res.Items[i];
                    bool hasData = false;
                    for (int j = 0; j < items.Count; j++)
                    {
                        if (items[j].Itemtid == item.Itemtid)
                        {
                            hasData = true;
                            items[j].Count += item.Count;
                        }
                    }
                    if (!hasData)
                    {
                        items.Add(item);
                    }
                }
            }
        }

        protected override void OnShow()
        {
            RefreshView();
        }

        private void RefreshView()
        {
            if(null != res)
            {
                TextHelper.SetText(tipsText, 2024725, res.Times.ToString());
                infinityGrid.CellCount = items.Count;
                infinityGrid.ForceRefreshActiveCell();
            }
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            PropItem entry = new PropItem();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BindGameObject(go);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= items.Count)
                return;
            PropItem entry = cell.mUserData as PropItem;
            CmdVictoryArenaFastChallengeRes.Types.ItemCount item = items[index];
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(item.Itemtid, item.Count, true, false, false, false, false, true, false, true);
            entry.SetData(itemData, EUIID.UI_JSBattle_Reward);
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_JSBattle_Quick_Tip);
        }
    }
}
