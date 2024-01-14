using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Multi_StageReward_Layout
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

            public override ClickItem Clone()
            {
                return Clone<MemeberChild>(this);
            }

        }


        Button m_BtnClose;

        ClickItemGroup<MemeberChild> rewardGroup = new ClickItemGroup<MemeberChild>();
        public void Load(Transform root)
        {
            m_BtnClose = root.Find("Image_Black").GetComponent<Button>();

            rewardGroup.AddChild(root.Find("Animator/Scroll_View/Viewport/PropItem"));
        }



        public void setListener(IListener listener)
        {

            m_BtnClose.onClick.AddListener(listener.OnClickClose);

        }


        public void SetRewardCount(int count)
        {
            rewardGroup.SetChildSize(count);
        }


        public void SetReward(int index,uint id, uint count)
        {
            var item = rewardGroup.getAt(index);

            if (item == null)
                return;

            item.SetReward(id, count);
        }
        public interface IListener
        {
            void OnClickClose();
   
        }
    }
}
