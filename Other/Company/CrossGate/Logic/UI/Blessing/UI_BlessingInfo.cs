using System;
using System.Collections.Generic;

using Logic.Core;
using Table;
namespace Logic
{
    public class UI_BlessingInfo_Parma
    {
        public uint id;
        public uint times;
    }
    public class UI_BlessingInfo : UIBase, UI_BlessingInfo_Layout.IListener
    {
        private UI_BlessingInfo_Layout m_Laout = new UI_BlessingInfo_Layout();

        private bool fristEnter = false;

        UI_BlessingInfo_Parma parma;
        protected override void OnLoaded()
        {
            m_Laout.Load(gameObject.transform);

            m_Laout.SetListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Blessing.Instance.eventEmitter.Handle(Sys_Blessing.EEvents.Start, OnEventStart, toRegister);
        }

        protected override void OnOpen(object arg)
        {
            parma = arg as UI_BlessingInfo_Parma;
        }
        protected override void OnShow()
        {
            Refresh();
        }

        protected override void OnHide()
        {
            
        }


        protected override void OnClose()
        {
            
        }

        public void OnClickClose()
        {
            CloseSelf();
        }

        private void Refresh()
        {
            var data = CSVBless.Instance.GetConfData(parma.id);

            var dropreward = CSVDrop.Instance.GetDropItem(data.Reward);
            uint rewardid = 0;
            long rewardcount = 0;
            if (dropreward.Count > 0)
            {
                rewardid = dropreward[0].id;
                rewardcount = dropreward[0].count;
            }

            m_Laout.m_ItemData.id = rewardid;
            m_Laout.m_ItemData.count = rewardcount;

            m_Laout.propItem.SetData(m_Laout.m_ItemData, EUIID.UI_Blessing_Info);

            TextHelper.SetText(m_Laout.TexInfo, data.StoryLan);

            var timesdata = CSVBlessTimes.Instance.GetConfData(parma.times + 1);

            m_Laout.NeedPropItem.transform.gameObject.SetActive(timesdata != null);

            if (timesdata != null)
            {
                var bagitem = Sys_Bag.Instance.GetItemCount(timesdata.itemNum[0]);

                m_Laout.m_NeedItemData.id = timesdata.itemNum[0];
                m_Laout.m_NeedItemData.count = timesdata.itemNum[1];
                m_Laout.m_NeedItemData.bShowBtnNo = (timesdata.itemNum[1] > bagitem);

                m_Laout.m_NeedItemData.onclick = OnClickNeedItem;

                m_Laout.NeedPropItem.SetData(m_Laout.m_NeedItemData, EUIID.UI_Blessing_Info);

                m_Laout.SetNeedItemCount(timesdata.itemNum[1], bagitem);

            }

            m_Laout.TogAuto.isOn = Sys_Blessing.Instance.IsAuto(data.id);
        }

        private void OnClickNeedItem(PropItem item)
        {
            m_Laout.NeedBoxEvt.itemData = m_Laout.m_NeedItemData;
            m_Laout.NeedBoxEvt.b_ForceShowScource = true;

            UIManager.OpenUI(EUIID.UI_Message_Box, false, m_Laout.NeedBoxEvt);
        }
        private void OnEventStart()
        {
            var data = CSVBless.Instance.GetConfData(parma.id);

            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(data.npcID);

            CloseSelf();

            if (UIManager.IsOpen(EUIID.UI_Blessing))
            {
                UIManager.CloseUI(EUIID.UI_Blessing);
            }
        }
        public void OnClickBless()
        {
            Sys_Blessing.Instance.SendNetStartReq(parma.id);
        }

        public void OnClickTogAuto(bool state)
        {
            Sys_Blessing.Instance.SetAuto(parma.id,state);
        }
    }
}
