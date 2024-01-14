using System;
using System.Collections.Generic;
using Logic;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_BattlePass_GotReward_Layout
    {
        Button m_BtnSure;


        public InfinityGrid m_GridRewardGoup;

        public InfinityGrid m_GridRewardVipGoup;

        public Transform m_TransVipReward;
        public void OnLoaded(Transform root)
        {

            m_BtnSure = root.Find("Image_Black").GetComponent<Button>();
         
            m_GridRewardGoup = root.Find("Animator/Reward/Layout/Reward1/Scroll View").GetComponent<InfinityGrid>();


            m_TransVipReward = root.Find("Animator/Reward/Layout/Reward2");

            m_GridRewardVipGoup = root.Find("Animator/Reward/Layout/Reward2/Scroll View2").GetComponent<InfinityGrid>();


        }

        public void SetListener(IListener listener)
        {
            m_BtnSure.onClick.AddListener(listener.OnClickSure);

            m_GridRewardGoup.onCreateCell = OnInfinityGridCreateCell;

            m_GridRewardGoup.onCellChange = listener.OnRewadInfinityChange;

            m_GridRewardVipGoup.onCreateCell = OnVipInfinityGridCreateCell;
            m_GridRewardVipGoup.onCellChange = listener.OnVipRewadInfinityChange;

        }

        private void OnInfinityGridCreateCell(InfinityGridCell cell)
        {
            RewardPropItem item = new RewardPropItem();

            item.Load(cell.mRootTransform);

            cell.BindUserData(item);
        }


        private void OnVipInfinityGridCreateCell(InfinityGridCell cell)
        {
            RewardPropItem item = new RewardPropItem();

            item.Load(cell.mRootTransform);

            cell.BindUserData(item);
        }
    }

    public partial class UI_BattlePass_GotReward_Layout
    {
        public interface IListener
        {

            void OnClickSure();

            void OnRewadInfinityChange(InfinityGridCell cell, int index);

            void OnVipRewadInfinityChange(InfinityGridCell cell, int index);
        }
    }

    public partial class UI_BattlePass_GotReward_Layout
    {
        class RewardPropItem
        {
            PropItem m_Item;
            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);

            public void Load(Transform transform)
            {

                m_Item = new PropItem();

                m_Item.BindGameObject(transform.gameObject);
            }

            public void SetItem(uint id, uint count)
            {
                m_ItemData.id = id;
                m_ItemData.count = count;
                m_ItemData.SetQuality(0);
                m_Item.SetData(new MessageBoxEvt() { sourceUiId = EUIID.UI_BattlePass_Collect, itemData = m_ItemData });
            }
        }
    }
    public partial class UI_BattlePass_GotReward_Layout
    {
        public void SetReward(InfinityGridCell cell, uint itemid, uint count)
        {
           var item = cell.mUserData as RewardPropItem;

            if (item == null)
                return;

            item.SetItem(itemid, count);
        }


        public void SetRewardCount(int count)
        {
            m_GridRewardGoup.CellCount = count;
            m_GridRewardGoup.ForceRefreshActiveCell();
            m_GridRewardGoup.MoveToIndex(0);
     
        }


        public void SetVipRewardCount(int count)
        {
            m_GridRewardVipGoup.CellCount = count;
            m_GridRewardVipGoup.ForceRefreshActiveCell();
            m_GridRewardVipGoup.MoveToIndex(0);

        }

        public void SetVipRewardActive(bool active)
        {
            m_TransVipReward.gameObject.SetActive(active);
        }

    }


}
