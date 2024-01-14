using System;
using System.Collections.Generic;

using Logic.Core;
using Packet;
using Table;
using UnityEngine;

namespace Logic
{
    public class UIPubParmas
    {
        public ulong UUID { get; set; } = 0;
         public uint Count { get; set; } = 0;
    }
    public partial class UI_Pub : UIBase, UI_Pub_Layout.IListener
    {
        UI_Pub_Layout m_Layout = new UI_Pub_Layout();

        UIPubParmas mPubParmas = null;

        bool m_bShowEffectAnimator = false;

        bool m_bFinish = false;
        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Bag.Instance.eventEmitter.Handle<uint, List<uint>, List<uint>>(Sys_Bag.EEvents.OnUseItemRes, OnPubRefresh, toRegister);
          
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnRoundNtf, OnStop, toRegister);
        }
        protected override void OnOpen(object arg)
        {            
            mPubParmas = arg as UIPubParmas;

            m_bShowEffectAnimator = true;

            m_bFinish = false;
        }
        protected override void OnShow()
        {

            if (m_bShowEffectAnimator)
            {
                m_Layout.SetRewardListActive(false);
                m_Layout.ShowCard();

                m_bShowEffectAnimator = false;
            }
            else
            {
                m_Layout.HidCard();
                CardAnimEnd();
            }

        }

        protected override void OnClose()
        {
            m_Layout.DestoryShowScene();
        }

        protected override void OnHide()
        {
            if (!m_bFinish)
            {
                Sys_Hint.Instance.PushContent_Normal("使用失败");
            }
            CloseSelf();
        }

        public void OnCardAnimEnd()
        {

            CardAnimEnd();
        }


        private void CardAnimEnd()
        {
            m_Layout.HidCard();
            m_Layout.SetRewardListActive(true);
            m_Layout.PlayEnterRewardAnim();

            if (isVisibleAndOpen == false)
                return;

           
            if (mPubParmas != null && mPubParmas.UUID != 0)
            {
                m_bFinish = true;

                Sys_Bag.Instance.UseItemByUuid(mPubParmas.UUID, mPubParmas.Count);

                mPubParmas = null;
            }
        }
        public void OnClickRewardClose()
        {
            m_Layout.PlayEndRewardAnim();
           // m_Layout.SetRewardListActive(false);
            CloseSelf();
        }

        public void OnClickRewardSrue()
        {
            m_Layout.PlayEndRewardAnim();
            // m_Layout.SetRewardListActive(false);
            CloseSelf();
        }

        public void OnClickSkipCardAnim()
        {
            m_Layout.HidCard();
            CardAnimEnd();
        }


        private List<ItemIdCount> mDropCahce = new List<ItemIdCount>();
        private void OnPubRefresh(uint itemid, List<uint> ids, List<uint> counts)
        {
            mDropCahce.Clear();

            var dataitem = CSVItem.Instance.GetConfData(itemid);

            if (dataitem == null || dataitem.type_id != 1405)
            {
                return;
            }

            int count = ids.Count;

            for (int i = 0; i < count; i++)
            {
                var data = CSVItem.Instance.GetConfData(ids[i]);

                ItemIdCount temp = new ItemIdCount();

                temp.id = ids[i];
                temp.count = counts[i];

                mDropCahce.Add(temp);
            }

            m_Layout.SetReward(count);

        }

        private void OnStop()
        {
            CloseSelf();
        }

        public void OnCellChange(InfinityGridCell cell, int index)
        {
            var item = cell.mUserData as UI_Pub_Layout.RawardItem;
            if (item != null && mDropCahce.Count > index)
                item.SetItem(mDropCahce[index]);
        }
    }
}
