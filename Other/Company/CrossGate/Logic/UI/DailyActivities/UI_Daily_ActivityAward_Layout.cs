using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Daily_ActivityAward_Layout
    {
        public class AwardItemChild : IntClickItem
        {
            PropItem m_Item;

            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);


            private Transform m_GetTransform;

            private Transform m_CanGetTransform;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);

                m_GetTransform = root.Find("Btn_Get");

                m_CanGetTransform = root.Find("Fx_ui_Select");

            }

            public override ClickItem Clone()
            {
                return Clone<AwardItemChild>(this);
            }

            public void SetItem(ItemIdCount item)
            {
                m_ItemData.id = item.id;
                m_ItemData.count = item.count;

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_DailyActivites, m_ItemData));
            }

            public void SetClickCall(Action<PropItem> active)
            {
                m_ItemData.onclick = active;
            }

            /// 设置奖励状态 1 可领取 2 已领取 其他为 none
            public void SetState(int state)
            {
                bool getEnable = state == 2;
                if (m_GetTransform.gameObject.activeSelf != getEnable)
                    m_GetTransform.gameObject.SetActive(getEnable);

                bool cangetEnable = state == 1;
                if (m_CanGetTransform.gameObject.activeSelf != cangetEnable)
                    m_CanGetTransform.gameObject.SetActive(cangetEnable);
            }
        }
    }
    public partial class UI_Daily_ActivityAward_Layout
    {
        public class AwardItem : IntClickItem
        {
            private Text mTexNum;


            private ClickItemGroup<AwardItemChild> mGroup;

            private Button mBtnClick;

            public uint ConfigID { get; set; }
            public override void Load(Transform root)
            {
                base.Load(root);

                mTexNum = root.Find("Image_Title/Text_Num").GetComponent<Text>();

                Transform childitme = root.Find("Scroll_View/Viewport/Item");

                mGroup = new ClickItemGroup<AwardItemChild>(childitme);

                mBtnClick = root.Find("Scroll_View").GetComponent<Button>();

                mBtnClick.onClick.AddListener(OnClickItem);

                mGroup.SetAddChildListenter(OnAddChildItem);
            }

            private void OnAddChildItem(AwardItemChild item)
            {
                item.SetClickCall(OnClickChildItem);
            }

            private void OnClickChildItem(PropItem item)
            {
                clickItemEvent?.Invoke((int)ConfigID);
            }

            private void OnClickItem()
            {
                clickItemEvent?.Invoke((int)ConfigID);
            }
            public void SetName(string tex)
            {
                mTexNum.text = tex;
            }
            public override ClickItem Clone()
            {
                return Clone<AwardItem>(this);
            }

            public void SetReward(List<ItemIdCount> itmes)
            {
                int count = itmes.Count;

                mGroup.SetChildSize(count);

                for (int i = 0; i < count; i++)
                {
                    var item = mGroup.getAt(i);

                    item.SetItem(itmes[i]);
                }
            }

           /// 设置奖励状态 1 可领取 2 已领取 其他为 none
            public void SetRewardState(int state)
            {
                for (int i = 0; i < mGroup.Count; i++)
                {
                    mGroup.getAt(i).SetState(state);
                }
            }
        }

        public void SetAwardCount(int count)
        {
            mGroup.SetChildSize(count);
        }

        public void SetAwardName(int index, uint cur, uint total)
        {
            var item = mGroup.getAt(index);

            if (item == null)
                return;

            item.SetName(cur + "/" + total);
        }

        public void SetReward(int index, List<ItemIdCount> items)
        {
            var item = mGroup.getAt(index);

            if (item == null)
                return;

            item.SetReward(items);
        }

        public void SetRewardConfigID(int index, uint configid )
        {
            var item = mGroup.getAt(index);

            if (item == null)
                return;

            item.ConfigID = configid;
        }

        /// <summary>
        /// 设置奖励状态 1 可领取 2 已领取 其他为 none
        /// </summary>
        /// <param name="index"></param>
        /// <param name="state"></param>
        public void SetRewardState(int index, int state)
        {
            var item = mGroup.getAt(index);

            if (item == null)
                return;

            item.SetRewardState(state);
        }
    }

    public partial class UI_Daily_ActivityAward_Layout
    {
        private ClickItemGroup<AwardItem> mGroup;

        private Button mBtnClose;

        private IListener m_Listener;
        public void Load(Transform root)
        {
            Transform item = root.Find("Animator/View_Content/Scroll_View01/Viewport/Item01");
            mGroup = new ClickItemGroup<AwardItem>(item);

            mBtnClose = root.Find("Black").GetComponent<Button>();

            mGroup.SetAddChildListenter(OnAddRewardItem);
        }

        public void SetListener(IListener listener)
        {
            m_Listener = listener;

            mBtnClose.onClick.AddListener(listener.OnClickClose);
        }

        private void OnAddRewardItem(AwardItem awardItem)
        {
            awardItem.clickItemEvent.AddListener(OnClickRewardItem);
        }
        private void OnClickRewardItem(int id)
        {
            m_Listener.OnClickRewardItem((uint)id);
        }
    }

    public partial class UI_Daily_ActivityAward_Layout
    {
        public interface IListener
        {
            void OnClickClose();


            void OnClickRewardItem(uint id);
        }
    }
}
