using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Pvp_RiseInRank_Layout 
    {
        ClickItemGroup<RectItem> m_ItemGroup ;

        Button m_BtnOneKeyGet;
        Button m_BtnClose;

        ScrollRect m_ScrollRect;
        GridLayoutGroup m_LayoutGroup;

        IListener m_Listener;
        public void Load(Transform root)
        {
            m_ItemGroup = new ClickItemGroup<RectItem>(root.Find("Animator/Rect/Rectlist/Image_line"));

            m_BtnOneKeyGet = root.Find("Animator/Btn_01").GetComponent<Button>();

            m_BtnClose = root.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();

            m_ScrollRect = root.Find("Animator/Rect").GetComponent<ScrollRect>();

            m_LayoutGroup = root.Find("Animator/Rect/Rectlist").GetComponent<GridLayoutGroup>();
        }

        public void SetListener(IListener listener)
        {
            m_Listener = listener;

            m_BtnOneKeyGet.onClick.AddListener(listener.OnClickOneKeyGet);
            m_BtnClose.onClick.AddListener(listener.OnClickClose);

            m_ItemGroup.SetAddChildListenter(OnAddRankItem);
        }


        private void OnAddRankItem(RectItem item)
        {
            item.SetOnClickListener(m_Listener.OnClickRankItemGet);
        }
        public void SetReward(int index, List<ItemIdCount> itemIds)
        {
            var item = m_ItemGroup.getAt(index);

            if (item == null)
                return;

            item.SetReward(itemIds);
        }

        public void SetRankSize(int size)
        {
            m_ItemGroup.SetChildSize(size);
        }

        public void SetSelectRank(int index)
        {
           float space = m_ScrollRect.vertical ? m_LayoutGroup.spacing.y : m_LayoutGroup.spacing.x;
       
            float value = index / (m_ItemGroup.Count * 1f);

            float yspace = m_ScrollRect.vertical ? value : 0;
            float xspace = m_ScrollRect.horizontal ? value : 0;

            m_ScrollRect.normalizedPosition = new Vector2(1 - xspace, 1 - yspace);
        }
        public void SetRankItemName(int index, string name)
        {
            var item = m_ItemGroup.getAt(index);

            if (item == null)
                return;

            item.SetName(name);
        }

        public void SetGetActive(int index,bool b, bool inseractable)
        {
            var item = m_ItemGroup.getAt(index);

            if (item == null)
                return;

            item.SetGetActive(b,inseractable);
        }

        public void SetHadGetActive(int index, bool b)
        {
            var item = m_ItemGroup.getAt(index);

            if (item == null)
                return;

            item.SetHadGetActive(b);
        }
        public void SetGetStateTex(int index, string tex)
        {
            var item = m_ItemGroup.getAt(index);

            if (item == null)
                return;

            item.SetGetTex(tex);
        }

        public void SetConfigID(int index, int id)
        {
            var item = m_ItemGroup.getAt(index);

            if (item == null)
                return;

            item.Index = id;
        }

        public int GetRankItemIndex(int configID)
        {
            int count = m_ItemGroup.Count;

            for (int i = 0; i < count; i++)
            {
                var item = m_ItemGroup.getAt(i);

                if (item != null && item.Index == configID)
                    return i;
            }

            return -1;
        }
    }


    public partial class UI_Pvp_RiseInRank_Layout
    {
        public interface IListener
        {
            void OnClickClose();
            void OnClickOneKeyGet();

            void OnClickRankItemGet(int index);
        }
    }

    public partial class UI_Pvp_RiseInRank_Layout
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
                m_ItemData.count =item.count;

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Pvp_Single, m_ItemData));
            }
        }
        class RectItem : IntClickItem
        {
            ClickItemGroup<RectPropItem> mRectPropItemGroup = new ClickItemGroup<RectPropItem>() { AutoClone = false };

            private Text m_TexLabel;
            private Button m_BtnGet;

            private Transform m_TransHadGet;

            public Text m_TexState;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TexLabel = root.Find("Label").GetComponent<Text>();
                m_BtnGet = root.Find("Btn_01_Small").GetComponent<Button>();

                m_TexState = m_BtnGet.transform.Find("Text_01").GetComponent<Text>();

                m_TransHadGet = root.Find("Image_Text");

                Transform gridTrans = root.Find("Grid");
                int gridCount = gridTrans.childCount;

                for (int i = 0; i < gridCount; i++)
                {
                    Transform item = gridTrans.Find(string.Format("PropItem ({0})", i));

                    if (item != null)
                    {
                        RectPropItem pitem = new RectPropItem();
                        pitem.Load(item);
                        mRectPropItemGroup.AddChild(pitem);
                    }
                }

                m_BtnGet.onClick.AddListener(OnClickGet);

            }

            public override ClickItem Clone()
            {
                return Clone<RectItem>(this);
            }
            private void OnClickGet()
            {
                clickItemEvent.Invoke(Index);
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


            public void SetName(string tex)
            {
                m_TexLabel.text = tex;
            }

            public void SetOnClickListener(UnityAction<int>  action)
            {
                clickItemEvent.AddListener(action);
            }

            public void SetGetActive(bool b,bool inseractable)
            {
                m_BtnGet.gameObject.SetActive (b);
                m_BtnGet.interactable = inseractable;
            }

            public void SetHadGetActive(bool b)
            {
                m_TransHadGet.gameObject.SetActive(b);
            }
            public void SetGetTex(string tex)
            {
                m_TexState.text = tex;
            }
        }
    }
}
