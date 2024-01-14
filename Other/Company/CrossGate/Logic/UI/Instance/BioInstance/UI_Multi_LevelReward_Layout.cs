using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Multi_LevelReward_Layout
    {
        public class MemeberChild : ClickItem
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
                m_ItemData.SetQuality(0);
                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Bio_LevelReward, m_ItemData));

                var data = CSVItem.Instance.GetConfData(id);
                if (data != null)
                    m_Item.txtName.text = LanguageHelper.GetTextContent(data.name_id);
            }
        }
        #region class member
        public class Member:ClickItem
        {
            InfinityGrid m_InfinityGrid;

            List<ItemIdCount> m_Reward = new List<ItemIdCount>(0);

            private Text m_TexName;
            public override void Load(Transform transform)
            {
                base.Load(transform);

                m_InfinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
                m_TexName = transform.Find("Image_bg/Image_Title/Text").GetComponent<Text>();

                m_InfinityGrid.onCellChange = OnRewardInfinityGridChange;
                m_InfinityGrid.onCreateCell = OnInfinityGridCreate;
            }

            public void SetReward(List<ItemIdCount> reward)
            {
                m_Reward = reward;
                int count = reward.Count;

                bool show = count > 0;

                if (m_InfinityGrid.Content.gameObject.activeSelf != show)
                    m_InfinityGrid.Content.gameObject.SetActive(show);

                m_InfinityGrid.CellCount = count;

                m_InfinityGrid.ForceRefreshActiveCell();
                m_InfinityGrid.MoveToIndex(0);

            }
            public void OnInfinityGridCreate(InfinityGridCell infinityGridCell)
            {
                MemeberChild child = new MemeberChild();

                child.Load(infinityGridCell.mRootTransform);

                infinityGridCell.BindUserData(child);

            }

            void OnRewardInfinityGridChange(InfinityGridCell infinityGridCell, int index)
            {
                MemeberChild child = infinityGridCell.mUserData as MemeberChild;

                if (child == null)
                    return;

                child.SetReward(m_Reward[index].id, (uint)m_Reward[index].count);
            }

            public void SetName(string name)
            {
                m_TexName.text = name;
            }
        }
        #endregion
    }
    public partial class UI_Multi_LevelReward_Layout
    {
        Button m_BtnClose;
        InfinityGrid m_InfinityGrid;

        private IListener m_Listener;
        public void Load(Transform root)
        {
            m_BtnClose = root.Find("Animator/View_TipsBgNew03/Btn_Close").GetComponent<Button>();
            m_InfinityGrid = root.Find("Animator/GameObject/Scroll View").GetComponent<InfinityGrid>();
        }

        public void OnInfinityGridCreate(InfinityGridCell infinityGridCell)
        {
            Member item = new Member();

            item.Load(infinityGridCell.mRootTransform);

            infinityGridCell.BindUserData(item);
        }

        public void setListener(IListener listener)
        {
            m_Listener = listener;

            m_BtnClose.onClick.AddListener(listener.OnClickClose);
            m_InfinityGrid.onCreateCell = OnInfinityGridCreate;
            m_InfinityGrid.onCellChange = listener.OnRewardInfinityGridChange;
        }


        public void SetLevelCount(int count)
        {
            bool show = count > 0;

            if (m_InfinityGrid.Content.gameObject.activeSelf != show)
                m_InfinityGrid.Content.gameObject.SetActive(show);

            m_InfinityGrid.CellCount = count;

            m_InfinityGrid.ForceRefreshActiveCell();
            m_InfinityGrid.MoveToIndex(0);
        }

        public void SetLevelReward(InfinityGridCell infinityGridCell, List<ItemIdCount> reward)
        {

        }

        public interface IListener
        {
            void OnClickClose();
            void OnRewardInfinityGridChange(InfinityGridCell infinityGridCell, int index);
        }
    }
}
