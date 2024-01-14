using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Goddness_EndAward_Layout
    {
        Button m_BtnClose;

        Text m_TexTitle;

        ClickItemGroup<EndAwardItem> m_EndAwardGrop = new ClickItemGroup<EndAwardItem>();
        public void Load(Transform root)
        {
            m_BtnClose = root.Find("Black").GetComponent<Button>();

            m_TexTitle = root.Find("Animator/Image_Bg/Image_Title/Text_Title").GetComponent<Text>();

            m_EndAwardGrop.AddChild(root.Find("Animator/Image_Bg/Scroll_View01/Viewport/Item01"));
        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
        }

        public void SetEndAwardCount(int count)
        {
            m_EndAwardGrop.SetChildSize(count);
        }

        public void SetEndAward(int index,List<ItemIdCount> itemIdCounts)
        {
           var item =  m_EndAwardGrop.getAt(index);

            if (item == null)
                return;

            item.SetAward(itemIdCounts);
        }

        public void SetEndAwardTextNum(int index, uint num)
        {

            var item = m_EndAwardGrop.getAt(index);

            if (item == null)
                return;

            item.m_TexNum.text = num.ToString();

        }
    }
    public partial class UI_Goddness_EndAward_Layout
    {
        public interface IListener
        {
            void OnClickClose();
        }


        class AwardItem : ClickItem
        {
            PropItem m_Item;
            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);
            }

            public void SetReward(uint id, uint count)
            {
                m_ItemData.id = id;
                m_ItemData.count = count;

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Goddness_EndAward, m_ItemData));
            }

            public override ClickItem Clone()
            {
                return Clone<AwardItem>(this);
            }
        }


        class EndAwardItem : ClickItem
        {

            public Text m_TexNum;

            ClickItemGroup<AwardItem> m_AwardGroup = new ClickItemGroup<AwardItem>();
            public override void Load(Transform root)
            {
                base.Load(root);

                m_AwardGroup.AddChild(root.Find("Scroll_View/Viewport/Item"));

                m_TexNum = root.Find("Text_Num").GetComponent<Text>();
            }

            public void SetAward(List<ItemIdCount> itemIdCounts)
            {
                int count = itemIdCounts.Count;

                m_AwardGroup.SetChildSize(count);

                for (int i = 0; i < count; i++)
                {
                   var item = m_AwardGroup.getAt(i);

                    item.SetReward(itemIdCounts[i].id, (uint)itemIdCounts[i].count);
                }
            }
            public override ClickItem Clone()
            {
                return Clone<EndAwardItem>(this);
            }
        }
    }
}
