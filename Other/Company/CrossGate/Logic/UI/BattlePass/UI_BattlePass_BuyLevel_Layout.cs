using System;
using System.Collections.Generic;
using Logic;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_BattlePass_BuyLevel_Layout
    {
        Button m_BtnClose;

        Button m_BtnSub;
        Button m_BtnAdd;
        Slider m_SliHandler;

        Text m_TexChange;

        Text m_TexTips;

        Button m_BtnBuy;
        Text m_TexCost;
        Image m_ImCostIcon;


        InfinityGrid m_ItemGroup;
        public void OnLoaded(Transform root)
        {
            m_BtnClose = root.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();

            m_BtnSub = root.Find("Animator/Content/Num/Btn_Min").GetComponent<Button>();
            m_BtnAdd = root.Find("Animator/Content/Num/Btn_Add").GetComponent<Button>();

            m_SliHandler = root.Find("Animator/Content/Num/Slider").GetComponent<Slider>();

            m_BtnBuy = root.Find("Animator/Content/Btn_01").GetComponent<Button>();

            m_TexCost = root.Find("Animator/Content/Btn_01/Common_Cost01/Text_Num").GetComponent<Text>();
            m_ImCostIcon = root.Find("Animator/Content/Btn_01/Common_Cost01/Image_Icon").GetComponent<Image>();

            m_ItemGroup = root.Find("Animator/Content/Scroll View").GetComponent<InfinityGrid>();

            m_TexTips = root.Find("Animator/Content/Text").GetComponent<Text>();

            m_TexChange = root.Find("Animator/Content/Num/Text").GetComponent<Text>();
        }

        public void SetListener(IListener listener)
        {

            m_BtnClose.onClick.AddListener(listener.OnClickClose);

            m_ItemGroup.onCreateCell = OnInfinityGridCreateCell;


            m_ItemGroup.onCellChange = listener.OnCellChange;

            m_BtnAdd.onClick.AddListener(listener.OnClickCountAdd);
            m_BtnSub.onClick.AddListener(listener.OnClickCountSub);
            m_SliHandler.onValueChanged.AddListener(listener.OnCountChange);

            m_BtnBuy.onClick.AddListener(listener.OnClickBuy);
        }

        private void OnInfinityGridCreateCell(InfinityGridCell cell)
        {
            RewardPropItem item = new RewardPropItem();

            item.Load(cell.mRootTransform);

            cell.BindUserData(item);
        }

        public void SetRewardCount(int count)
        {
            m_ItemGroup.CellCount = count;
            m_ItemGroup.ForceRefreshActiveCell();
            m_ItemGroup.MoveToIndex(0);
        }

        public void SetRewardItem(InfinityGridCell cell, uint id, uint count)
        {
            RewardPropItem item = cell.mUserData as RewardPropItem;

            if (item == null)
                return;

            item.SetItem(id, count);
        }

        public void SetBuyProcess(float value)
        {
            m_SliHandler.value = value;
        }

        public void SetBuyCost(uint cost)
        {
            TextHelper.SetText(m_TexCost, cost.ToString());
        }

        public void SetBuyCount(uint buylevelcount, uint buyafterlevel)
        {
            TextHelper.SetText(m_TexChange, LanguageHelper.GetTextContent(3910010114, buylevelcount.ToString(),buyafterlevel.ToString()));
        }

        public void SetTips(uint level, uint rewardcount)
        {
            TextHelper.SetText(m_TexTips, LanguageHelper.GetTextContent(3910010113, level.ToString(), rewardcount.ToString()));
        }

        public void SetCostIcon(uint icon)
        {
            ImageHelper.SetIcon(m_ImCostIcon, icon);
        }
    }

    public partial class UI_BattlePass_BuyLevel_Layout
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

                m_Item.SetData(new MessageBoxEvt() { sourceUiId = EUIID.UI_BattlePass, itemData = m_ItemData });
            }
        }
    }
    public partial class UI_BattlePass_BuyLevel_Layout
    {
        public interface IListener
        {
            void OnClickClose();

            void OnClickBuy();

            void OnCellChange(InfinityGridCell cell, int index);

            void OnClickCountAdd();
            void OnClickCountSub();

            void OnCountChange(float value);
        }
    }
}
