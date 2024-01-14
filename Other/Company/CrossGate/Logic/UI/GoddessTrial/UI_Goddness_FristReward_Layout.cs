using System;
using System.Collections.Generic;

using Logic.Core;

using UnityEngine;
using UnityEngine.UI;
namespace Logic
{
    public class UI_Goddness_FristReward_Layout
    {
        public class RewardItem : ClickItem
        {
            PropItem m_Item;
            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);

            public override void Load(Transform root)
            {
                base.Load(root);

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);
            }

            public override ClickItem Clone()
            {
                return Clone<RewardItem>(this);
            }

            public void SetReward(uint id, uint count)
            {
                m_ItemData.id = id;
                m_ItemData.count = count;
                m_ItemData.SetQuality(0);
                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Goddness_FristAward, m_ItemData));
            }
        }

        public class RewardGroupItem : ClickItem
        {
            public ClickItemGroup<RewardItem> m_GroupReward = new ClickItemGroup<RewardItem>();

          
            public override void Load(Transform root)
            {
                m_GroupReward.AddChild(root.Find("Scroll_View/Viewport/Item"));

                
            }

            public override ClickItem Clone()
            {
                return Clone<RewardGroupItem>(this);
            }

            public RewardItem GetRewardItem(int index)
            {
                return m_GroupReward.getAt(index);
            }
        }

        public ClickItemGroup<RewardGroupItem> m_GroupReward = new ClickItemGroup<RewardGroupItem>();

        public Button m_BtnClose;

        public RectTransform m_TransLayout;
        public void Load(Transform root)
        {
            m_GroupReward.AddChild(root.Find("Animator/Image_Bg/Scroll_View01/Viewport/Item01"));

            m_BtnClose = root.Find("Black").GetComponent<Button>();

            m_TransLayout = root.Find("Animator/Image_Bg") as RectTransform;
        }

        public RewardGroupItem GetRewardGroup(int index)
        {
            return m_GroupReward.getAt(index);
        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
        }
        public interface IListener
        {
            void OnClickClose();
        }
    }
}
