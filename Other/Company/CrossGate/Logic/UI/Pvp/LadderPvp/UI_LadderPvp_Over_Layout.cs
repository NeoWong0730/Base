using Framework;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Logic
{
    /// <summary>
    ///pvp 结算
    /// </summary>
    public partial class UI_LadderPvp_Over_Layout
    {

        private Transform m_TransSuccess;
        private Transform m_TransFaile;

        private ClickItemGroup<RectPropItem> m_RewardGroup = new ClickItemGroup<RectPropItem>() { AutoClone = false };

        private Button m_BtnClose;

        private Transform m_TransNoReward;
        public void Load(Transform root)
        {

            m_TransFaile = root.Find("Animator/Image_Failedbg");
            m_TransSuccess = root.Find("Animator/Image_Successbg");

            m_BtnClose = root.Find("Animator/off-bg").GetComponent<Button>();

            m_TransNoReward = root.Find("Animator/Text_NoReward");
            Transform transGrid = root.Find("Animator/Grid");

            int count = transGrid.childCount;

            for (int i = 0; i < count; i++)
            {
                string strname = i == 0 ? "PropItem" : string.Format("PropItem ({0})", i);

                var item = transGrid.Find(strname);

                if (item != null)
                {
                    m_RewardGroup.AddChild(item);
                }
            }
        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
        }


        public void SetOverResult(int state)
        {

            m_TransSuccess.gameObject.SetActive(state == 0);
            m_TransFaile.gameObject.SetActive(state == 1);
        }
             

        public void SetReward(IList<TTDanLvAward> awards)
        {
            int idscount = awards.Count;

            m_RewardGroup.SetChildSize(idscount);

            for (int i = 0; i < idscount; i++)
            {
                var item = m_RewardGroup.getAt(i);

                if (item != null)
                {
                    item.SetReward(awards[i].ItemId, awards[i].ItemNum);
                }
            }

            m_TransNoReward.gameObject.SetActive(idscount == 0);
        }
    }

    public partial class UI_LadderPvp_Over_Layout
    {
        class RectPropItem : IntClickItem
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

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Pvp_SingleFinale, m_ItemData));
            }

            public void SetReward(uint id, uint count)
            {
                m_ItemData.id = id;
                m_ItemData.count = count;

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Pvp_SingleFinale, m_ItemData));
            }
        }
    }
    public partial class UI_LadderPvp_Over_Layout
    {
        public interface IListener
        {
            void OnClickClose();

        }
    }


}
