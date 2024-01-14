using System;
using System.Collections.Generic;

using Logic.Core;
using Table;
namespace Logic
{
    public class UI_Blessing:UIBase,UI_Blessing_Layout.IListener
    {
        private UI_Blessing_Layout m_Laout = new UI_Blessing_Layout();

        private bool fristEnter = false;
        protected override void OnLoaded()
        {
            m_Laout.Load(gameObject.transform);

            m_Laout.SetListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Blessing.Instance.eventEmitter.Handle(Sys_Blessing.EEvents.InfoRefresh, Refresh, toRegister);
            Sys_Blessing.Instance.eventEmitter.Handle(Sys_Blessing.EEvents.Start, OnNetLocalRefresh, toRegister);
            Sys_Blessing.Instance.eventEmitter.Handle(Sys_Blessing.EEvents.TakeAward, OnNetLocalRefresh, toRegister);
        }

        protected override void OnOpen(object arg)
        {
            Sys_Blessing.Instance.SendNetInfoReq();
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

        private void OnClickBtnBless(int index)
        {
            var data = CSVBless.Instance.GetByIndex(index);

            int state = Sys_Blessing.Instance.GetState(data.id);

            if (state == 0)
            {
                UIManager.OpenUI(EUIID.UI_Blessing_Info, false, new UI_BlessingInfo_Parma() { id = data.id, times = Sys_Blessing.Instance.GetTimes(data.id) });
            }
            else if (state == 1)
            {
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(data.npcID);
                CloseSelf();
            }
            else if (state == 2)
            {
                Sys_Blessing.Instance.SendNetTakeAwardReq(data.id);
            }
            
        }
        public void OnGridCreate(InfinityGridCell cell)
        {
            UI_Blessing_Layout.BlessItem item = new UI_Blessing_Layout.BlessItem();
            item.Load(cell.mRootTransform);
            cell.BindUserData(item);

            item.OnClickBtnBless = OnClickBtnBless;
        }

        public void OnGridUpdate(InfinityGridCell cell, int index)
        {
            var data = CSVBless.Instance.GetByIndex(index);

            var uiitem = cell.mUserData as UI_Blessing_Layout.BlessItem;

            uiitem.Index = index;
            ImageHelper.SetTexture(uiitem.ImgBackGround, "Texture/Big/" + data.image +".png");

            

            TextHelper.SetText(uiitem.TexInfo, GetConLanParma(data.id,data.CondLan));

           
            var dropreward = CSVDrop.Instance.GetDropItem(data.Reward);
            uint rewardid = 0;
            long rewardcount = 0;
            if (dropreward.Count > 0)
            {
                rewardid = dropreward[0].id;
                rewardcount = dropreward[0].count;
            }

            uiitem.SetReward(rewardid, rewardcount);

            uiitem.TexTimes.text = (data.Times - Sys_Blessing.Instance.GetTimes(data.id)).ToString() + "\n/\n" + data.Times.ToString();

            int state = Sys_Blessing.Instance.GetState(data.id);
            uiitem.SetState(state);

            uiitem.TransRedPoint.gameObject.SetActive(state == 2);
        }

        private string GetConLanParma(uint id,uint condLan)
        {
            var worldLevelData = CSVWorldLevel.Instance.GetConfData(Sys_Role.Instance.openServiceDay);

            string str = string.Empty;

            switch(id)
            {
                case 1:
                    str = LanguageHelper.GetTextContent(condLan, worldLevelData.LvRecom.ToString());
                    break;
                case 2:
                    {
                        str = LanguageHelper.GetTextContent(condLan, (worldLevelData.RepuRecom / 100 + 1).ToString(), (worldLevelData.RepuRecom % 100).ToString()); 
                    }
                    break;
                case 3:
                    str = LanguageHelper.GetTextContent(condLan, worldLevelData.TalentPoint.ToString());
                    break;
                case 4:
                    str = LanguageHelper.GetTextContent(condLan, worldLevelData.PracRecom.ToString());
                    break;
                case 5:
                    {
                        str = LanguageHelper.GetTextContent(condLan, (worldLevelData.TameRecom / 100 + 1).ToString(), (worldLevelData.TameRecom % 100).ToString());
                    }
                    break;
            }

            return str;
        }
        private void Refresh()
        {

            if (m_Laout.PropertyGrid == null)
                return;

            int count = CSVBless.Instance.GetAll().Count;

            m_Laout.PropertyGrid.CellCount = count;

            m_Laout.PropertyGrid.ForceRefreshActiveCell();
            m_Laout.PropertyGrid.Apply();

            m_Laout.currencyTitle.InitUi();
        }

        private void OnNetLocalRefresh()
        {
            if (m_Laout.PropertyGrid == null)
                return;

            m_Laout.PropertyGrid.ForceRefreshActiveCell();
            m_Laout.PropertyGrid.Apply();
        }
    }
}
