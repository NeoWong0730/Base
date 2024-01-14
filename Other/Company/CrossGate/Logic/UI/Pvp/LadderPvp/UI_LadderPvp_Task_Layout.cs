using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_LadderPvp_Task_Layout
    {


        Button m_BtnClose;

        GridLayoutGroup m_LayoutGroup;

        public InfinityGrid m_InfinityGrid;

        IListener m_Listener;
        public void Load(Transform root)
        {

            m_BtnClose = root.Find("Animator/View_TipsBg03_Big/Btn_Close").GetComponent<Button>();

            m_InfinityGrid = root.Find("Animator/Scroll View").GetComponent<InfinityGrid>();

            //m_LayoutGroup = root.Find("Animator/Rect/Rectlist").GetComponent<GridLayoutGroup>();
        }

        public void SetListener(IListener listener)
        {
            m_Listener = listener;


            m_BtnClose.onClick.AddListener(listener.OnClickClose);

            m_InfinityGrid.onCreateCell = listener.OnInfinityCellCreate;
            m_InfinityGrid.onCellChange = listener.OnInfinityUpdate;
        }


        private void OnAddRankItem(RectItem item)
        {
            item.SetOnClickListener(m_Listener.OnClickRankItemGet);
        }

    }


    public partial class UI_LadderPvp_Task_Layout
    {
        public interface IListener
        {
            void OnClickClose();
            void OnClickOneKeyGet();

            void OnClickRankItemGet(int index);

            void OnInfinityCellCreate(InfinityGridCell cell);

            void OnInfinityUpdate(InfinityGridCell cell, int index);
        }
    }

    public partial class UI_LadderPvp_Task_Layout
    {
        public class RectPropItem : ClickItem
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

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_LadderPvp_TaskReward, m_ItemData));
            }
        }
        public class RectItem : IntClickItem
        {
            ClickItemGroup<RectPropItem> mRectPropItemGroup = new ClickItemGroup<RectPropItem>() { AutoClone = false };

            public Text m_TexLabel;
            private Button m_BtnGet;

            public Transform m_TransHadGet;

            public Text m_TexState;

            public Text m_TexProcess;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TexLabel = root.Find("Text_Task").GetComponent<Text>();
                m_BtnGet = root.Find("Btn_01_Small").GetComponent<Button>();

                m_TexState = m_BtnGet.transform.Find("Text_01").GetComponent<Text>();

                m_TransHadGet = root.Find("Finish");

                m_TexProcess = root.Find("Text_Num").GetComponent<Text>();

                Transform gridTrans = root.Find("Scroll/Viewport/Content");
                int gridCount = gridTrans.childCount;
                for (int i = 0; i < gridCount; i++)
                {
                    Transform item = gridTrans.Find(string.Format("PropItem ({0})", i));

                    if (item == null && i == 0)
                    {
                        item = gridTrans.Find("PropItem");
                    }

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

            public void SetOnClickListener(UnityAction<int> action)
            {
                clickItemEvent.AddListener(action);
            }

            public void SetGetActive(bool b, bool inseractable)
            {
                m_BtnGet.gameObject.SetActive(b);
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
