using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Multi_Reward_Layout
    {
        class  InfoItem: ToggleIntClickItem
        {
          
            private Text m_TexState;
            private Transform m_UsedTrans;

            private Text m_TexName;

            private ClickItemGroup<ChildItem> m_InfoChildGroup;

            public void SetStateTex(string tex)
            {
                m_TexState.text = tex;
            }

            public void SetUsed(bool state)
            {
                m_UsedTrans.gameObject.SetActive(state);
            }

            public void SetChildCount(int count)
            {
                m_InfoChildGroup.SetChildSize(count);
            }

            public ChildItem getChildAt(int index)
            {
                return m_InfoChildGroup.getAt(index);
            }


            public void SetReward(List<ItemIdCount> value)
            {
                int count = value.Count;

                SetChildCount(count);

                for (int i = 0; i < count; i++)
                {
                    SetReward(i, value[i].id, value[i].count);
                }
            }
            private void SetReward(int index,uint ID,long count)
            {
                var item = getChildAt(index);

                item.SetReward(ID,count);
            }

            public void SetName(uint langugeID)
            {
                TextHelper.SetText(m_TexName, langugeID);
            }
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TexState = root.Find("Btn_BG/Text").GetComponent<Text>();
                m_UsedTrans = root.Find("Image_Happen");

                m_TexName = root.Find("Text_Name").GetComponent<Text>();

                Transform item = root.Find("Scroll/Viewport/Content/PropItem");

                var it = new ChildItem();
                it.Load(item);
                m_InfoChildGroup = new ClickItemGroup<ChildItem>(it);

            }

            public override ClickItem Clone()
            {
                return Clone<InfoItem>(this);
            }

            protected override void OnClick(bool b)
            {
                base.OnClick(b);
              
            }
        }
    }

    public partial class UI_Multi_Reward_Layout
    {
        class ChildItem:ClickItem
        {

            private Image m_IIcon;

            PropItem m_Item;

            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0,1,false,false,false,false,false,true,false);
            public void SetReward(uint ID, long count)
            {
               // ImageHelper.SetIcon(m_IIcon, iconID);

                m_ItemData.id = ID;
                m_ItemData.count = count;

                m_Item.SetData(m_ItemData, EUIID.UI_Multi_Reward);
            }


            public override void Load(Transform root)
            {
                base.Load(root);

                m_IIcon = root.Find("Btn_Item/Image_Icon").GetComponent<Image>();

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);
            }

            public override ClickItem Clone()
            {
                return Clone<ChildItem>(this);
            }
        }
    }

    public partial class UI_Multi_Reward_Layout
    {

        private Button mBtnClose;

        private ClickItemGroup<InfoItem> m_InfoGroup;
        

        private IListener m_Listener;

        public void Load(Transform root)
        {
            Transform infotrans = root.Find("Animator/Scroll View01/Viewport/Content/RewardItem");

            InfoItem infoItem = new InfoItem();
            infoItem.Load(infotrans);
            m_InfoGroup = new ClickItemGroup<InfoItem>(infoItem);
            m_InfoGroup.SetAddChildListenter(AddInfoItem);

            mBtnClose = root.Find("Animator/View_TipsBg03_Big/Btn_Close").GetComponent<Button>();

            mBtnClose.onClick.AddListener(OnClickClose);

        }

        public void setListener(IListener listener)
        {
            m_Listener = listener;
        }

        private void AddInfoItem(InfoItem item)
        {
            item.clickItemEvent.AddListener(OnClickInfoItem);
        }
        private void OnClickInfoItem(int index)
        {
            m_Listener?.OnClickInfo(index);
        }
        public void SetInfoCount(int count)
        {
            m_InfoGroup.SetChildSize(count);
        }

        public void SetInfoIndex(int index, int value)
        {
            var item = m_InfoGroup.getAt(index);

            if (item == null)
                return;

            item.Index = value;
        }

        public void SetInfoName(int index,uint name)
        {
            var item = m_InfoGroup.getAt(index);

            if (item == null)
                return;

            item.SetName(name);
        }

        public void SetReward(int index, List<ItemIdCount> value)
        {
            var item = m_InfoGroup.getAt(index);

            if (item == null)
                return;

            item.SetReward(value);
        }

        public void FocusInfo(int index)
        {
            var item = m_InfoGroup.getAt(index);

            if (item == null)
                return;

            item.Togg.isOn = true;
        }

        /// <summary>
        /// 选择生效
        /// </summary>
        /// <param name="index"></param>
        /// <param name="state"></param>
        public void SetUsed(int index, bool state)
        {
            var item = m_InfoGroup.getAt(index);

            if (item == null)
                return;

            item.SetUsed(state);
        }

        /// <summary>
        /// 可选择、已选择 文本
        /// </summary>
        /// <param name="index"></param>
        /// <param name="state"></param>
        public void SetStateTex(int index, string stri)
        {
            var item = m_InfoGroup.getAt(index);

            if (item == null)
                return;

            item.SetStateTex(stri);
        }

        private void OnClickClose()
        {
            m_Listener?.Close();
        }


        public interface IListener
        {
            void OnClickInfo(int index);
            void OnUse();

            void Close();
        }
    }
}
