using System;
using System.Collections.Generic;
using Logic;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_BattlePass_LevelUp_Layout
    {
        Button m_BtnSure;
        Button m_BtnSuper;

        public InfinityGrid m_GridRewardGoup;

        private Text m_TexLevelTips;
        private Text m_TexRewardTips;
        public void OnLoaded(Transform root)
        {

            m_BtnSure = root.Find("Animator/Buttons/Btn_02").GetComponent<Button>();
            m_BtnSuper = root.Find("Animator/Buttons/Btn_01").GetComponent<Button>();

            m_GridRewardGoup = root.Find("Animator/Reward/Scroll View").GetComponent<InfinityGrid>();

            m_TexLevelTips = root.Find("Animator/Image_BG/Title").GetComponent<Text>();
            m_TexRewardTips = root.Find("Animator/Reward/Text_Title/Num").GetComponent<Text>();

        }

        public void SetListener(IListener listener)
        {
            m_BtnSure.onClick.AddListener(listener.OnClickSure);
            m_BtnSuper.onClick.AddListener(listener.OnClickSureSuper);

            m_GridRewardGoup.onCreateCell = OnInfinityGridCreateCell;

            m_GridRewardGoup.onCellChange = listener.OnRewadInfinityChange;
        }

        private void OnInfinityGridCreateCell(InfinityGridCell cell)
        {
            RewardPropItem item = new RewardPropItem();

            item.Load(cell.mRootTransform);

            cell.BindUserData(item);
        }
    }

    public partial class UI_BattlePass_LevelUp_Layout
    {
        public interface IListener
        {

            void OnClickSure();
            void OnClickSureSuper();

            void OnRewadInfinityChange(InfinityGridCell cell, int index);
        }
    }

    public partial class UI_BattlePass_LevelUp_Layout
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
                m_Item.SetData(new MessageBoxEvt() { sourceUiId = EUIID.UI_BattlePass_SpecialLv, itemData = m_ItemData });
            }
        }
    }
    public partial class UI_BattlePass_LevelUp_Layout
    {
        public void SetReward(InfinityGridCell cell, uint itemid, uint count)
        {
           var item = cell.mUserData as RewardPropItem;

            if (item == null)
                return;

            item.SetItem(itemid, count);
        }

        public void SetLevel(uint level)
        {
            TextHelper.SetText(m_TexLevelTips, 3910010115, level.ToString());
        }

        public void SetRewardCount(int count)
        {
            m_GridRewardGoup.CellCount = count;
            m_GridRewardGoup.ForceRefreshActiveCell();
            m_GridRewardGoup.MoveToIndex(0);

            TextHelper.SetText(m_TexRewardTips, 3910010116, count.ToString());
        }

        public void SetSuperBtnActive(bool active)
        {
            if (m_BtnSuper.gameObject.activeSelf != active)
                m_BtnSuper.gameObject.SetActive(active);
        }
    }

    public partial class UI_BattlePass_LevelUp_Layout
    {
        
    }
}
