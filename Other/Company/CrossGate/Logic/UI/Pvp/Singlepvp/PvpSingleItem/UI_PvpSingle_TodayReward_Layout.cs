using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{ 
    public partial class UI_PvpSingle_TodayReward_Layout
    {
        private Text m_TexTitle;

        private ClickItemGroup<GridPropItem> mPropList = new ClickItemGroup<GridPropItem>() { AutoClone = false};

        private Button m_BtnClose;

        private Transform m_TransReward;

        private Transform m_TransRewardItemGroup;

        private Canvas m_Canvas;
        public void Load(Transform root)
        {
            m_TransReward = root;

            m_TransRewardItemGroup = root.Find("group");

            m_BtnClose = root.Find("Image_off").GetComponent<Button>();

            m_TexTitle = root.Find("group/Image_BG/Image_Title/Text_Title").GetComponent<Text>();

            Transform gridTrans = root.Find("group/Image_BG/Grid");

            int gridCount = gridTrans.childCount;

            for (int i = 0; i < gridCount; i++)
            {
                Transform item = gridTrans.Find(string.Format("PropItem{0}", i));

                if (item != null)
                {
                    mPropList.AddChild(item);
                }
            }

            m_Canvas = root.GetComponent<Canvas>();
        }

        public void SetSort(int sort)
        {
            m_Canvas.sortingOrder = sort;
        }
        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
        }

        public void SetTitleTex(uint tex,int num)
        {
            TextHelper.SetText(m_TexTitle, tex,num.ToString());
        }

        public void SetReward(List<ItemIdCount> rewardlist, bool hadUsed)
        {
            int count = rewardlist.Count;

            mPropList.SetChildSize(count);

            for (int i = 0; i < count; i++)
            {
                var item = mPropList.getAt(i);

                if (item != null)
                {
                    item.SetReward(rewardlist[i], hadUsed);
                }
            }


        }

        private Vector3[] m_tempPoints = new Vector3[4];
        //private Bounds m_RewardBounds = new Bounds();
        public void SetRewardShowPosition(Vector3[] points)
        {
            var rewardRect = m_TransReward as RectTransform;

            var worldToLocalMat = rewardRect.worldToLocalMatrix;

            Vector3 maxValue = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 minValue = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            for (int i = 0; i < 4; i++)
            {
                m_tempPoints[i] = worldToLocalMat.MultiplyPoint3x4(points[i]);

                maxValue = Vector3.Max(m_tempPoints[i], maxValue);
                minValue = Vector3.Min(m_tempPoints[i], minValue);
            }

            Bounds bounds = new Bounds(minValue, Vector3.zero);

            bounds.Encapsulate(maxValue);

            var rewardGroupRect = m_TransRewardItemGroup as RectTransform;

            Bounds groupGrounds = new Bounds(rewardGroupRect.rect.center, rewardGroupRect.rect.size);



            Vector3 pos = m_TransRewardItemGroup.localPosition;

            pos.y = bounds.center.y;// + rewardGroupRect.rect.size.y/2;

            m_TransRewardItemGroup.localPosition = pos;
        }

    }

    public partial class UI_PvpSingle_TodayReward_Layout
    {
        class GridPropItem:ClickItem
        {
            PropItem m_Item;
            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);
            }

            public void SetReward(ItemIdCount item,bool hadUsed)
            {
                m_ItemData.id = item.id;
                m_ItemData.count = item.count;
                //m_ItemData.bSelected = hadUsed;

                m_Item.SetGot(hadUsed);
               // m_Item.SetData(m_ItemData);

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Pvp_Single, m_ItemData));
            }
        }
    }

    public partial class UI_PvpSingle_TodayReward_Layout
    {
        public interface IListener
        {
            void OnClickClose();
        }
    }
}
