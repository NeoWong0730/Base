using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Goddness_Result_Layout
    {
        class ResultPropItem : ClickItem
        {
            private PropItem m_Item = null;

            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, false, false, false, false, false, true, false);
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);
            }

            public void SetItem(ItemIdCount item)
            {
                m_ItemData.id = item.id;
                m_ItemData.count = item.count;

                m_Item.SetData(m_ItemData, EUIID.UI_Multi_Reward);
            }


            public void SetItem(uint id, uint count)
            {
                m_ItemData.id = id;
                m_ItemData.count = count;

                m_Item.SetData(m_ItemData, EUIID.UI_Multi_Reward);
            }
            public override ClickItem Clone()
            {
                return Clone<ResultPropItem>(this);
            }
        }
    }
    public partial class UI_Goddness_Result_Layout
    {
        private Button m_BtnClose;
        private ClickItemGroup<ResultPropItem> m_PropItemGroup = new ClickItemGroup<ResultPropItem>();


        private Text m_TexLevelInfo;
        public void Load(Transform root)
        {
            m_BtnClose = root.Find("Animator/off-bg").GetComponent<Button>();

            Transform grid = root.Find("Animator/Grid");

            int childGrid = grid.childCount;

            for (int i = 0; i < childGrid; i++)
            {
                m_PropItemGroup.AddChild(grid.Find("PropItem" + i));
            }


            m_TexLevelInfo = root.Find("Animator/Text_Level").GetComponent<Text>();
        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
        }

        public void SetReward(List<ItemIdCount> droplist)
        {
            int count = droplist.Count;

            m_PropItemGroup.SetChildSize(count);

            for (int i = 0; i < count; i++)
            {
                var item = m_PropItemGroup.getAt(i);

                if (item != null)
                    item.SetItem(droplist[i]);
            }
        }


        public void SetRewardCount(int count)
        {
            m_PropItemGroup.SetChildSize(count);
        }

        public void SetReward(int index, uint id, uint count)
        {
            var item = m_PropItemGroup.getAt(index);

            if (item != null)
                item.SetItem(id,count);
        }

        public void SetLevelInfo(string str)
        {
            m_TexLevelInfo.text = str;
        }
    }

    public partial class UI_Goddness_Result_Layout
    {
        public interface IListener
        {
            void OnClickClose();
        }
    }
}
