using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_PvpSingle_RankReward_Layout
    {
        private Button m_BtnClose;
        private Button m_BtnClose0;

        ClickItemGroup<RectItem> mRectPropItemGroup = new ClickItemGroup<RectItem>();
        public void Load(Transform root)
        {
            m_BtnClose = root.Find("Image_off").GetComponent<Button>();
            m_BtnClose0 = root.Find("Animator/View_TipsBg01_Square/Btn_Close").GetComponent<Button>();

            Transform rectlistTrans = root.Find("Animator/GameObject/Rect/Rectlist");

            int rectitmecount = rectlistTrans.childCount;

            for (int i = 0; i < rectitmecount; i++)
            {
                Transform item = rectlistTrans.Find("Image_line" + i);
                if (item != null)
                {
                    RectItem rectItem = new RectItem();
                    rectItem.Load(item);
                    mRectPropItemGroup.AddChild(rectItem);

                    if (i == 3)
                        mRectPropItemGroup.OriginItem = rectItem;
                }
            }

        }

        public void SetListener(IListener listener)
        {
           // m_BtnClose.onClick.AddListener(listener.OnClickClose);
            m_BtnClose0.onClick.AddListener(listener.OnClickClose);
        }
        public void SetRewardSize(int size)
        {
            mRectPropItemGroup.SetChildSize(size);
        }
        public void SetReward(int index, List<ItemIdCount> items)
        {
            var item = mRectPropItemGroup.getAt(index);

            if (item == null)
                return;

            item.SetReward(items);
        }
        public void SetRewardTitle(int index,string tex)
        {
            var item = mRectPropItemGroup.getAt(index);

            if (item == null)
                return;

            item.SetTitle(tex);
        }
    }

    public partial class UI_PvpSingle_RankReward_Layout
    {
        public interface IListener
        {
            void OnClickClose();
        }
    }
    public partial class UI_PvpSingle_RankReward_Layout
    {
        class RectPropItem : ClickItem
        {
            PropItem m_Item;
            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);
            }

            public void SetReward(ItemIdCount item)
            {
                m_ItemData.id = item.id;
                m_ItemData.count = item.count;

            
                 m_Item.SetData(new MessageBoxEvt(EUIID.UI_Pvp_Single, m_ItemData));
            }
        }
        class RectItem : ClickItem
        {
            ClickItemGroup<RectPropItem> mRectPropItemGroup = new ClickItemGroup<RectPropItem>() { AutoClone = false };

            private Text m_TexTitle;
            private Text m_TexLimite;
            public override void Load(Transform root)
            {
                base.Load(root);

                Transform titleTexTrans = root.Find("Image_bg/Image_Title/Text");

                if (titleTexTrans != null)
                    m_TexTitle = titleTexTrans.GetComponent<Text>();

                m_TexLimite = root.Find("Image_bg/Image_Title/Text_Limit").GetComponent<Text>();

                Transform gridTrans = root.Find("Grid");
                int gridCount = gridTrans.childCount;

                for (int i = 0; i < gridCount; i++)
                {
                    string strname = i == 0 ? "PropItem" : string.Format("PropItem ({0})", i);

                    Transform item = gridTrans.Find(strname);

                    if (item != null)
                    {
                        RectPropItem pitem = new RectPropItem();
                        pitem.Load(item);
                        mRectPropItemGroup.AddChild(pitem);
                    }
                }

            }
            public override ClickItem Clone()
            {
                return Clone<RectItem>(this);
            }
            public void SetReward(List<ItemIdCount> items)
            {
                mRectPropItemGroup.SetChildSize(items.Count);

                for (int i = 0; i < items.Count; i++)
                {
                    var item = mRectPropItemGroup.getAt(i);

                    if (item != null)
                    {
                        item.SetReward(items[i]);
                    }
                }
            }

            public void SetTitle(string tex)
            {
                m_TexTitle.text = tex;
            }

            public void SetTitleLimite(string tex)
            {
                m_TexLimite.text = tex;
            }
        }
    }
}
