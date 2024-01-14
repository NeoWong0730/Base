using System;
using System.Collections.Generic;
using Logic;
using Logic.Core;
using Table;
using Framework;
using Packet;

namespace Logic
{
    public partial class UI_BattlePass_VIP : UIBase
    {
        UI_BattlePass_VIP_Layout m_Layout = new UI_BattlePass_VIP_Layout();

        private CutSceneModelShowController cutSceneController = new CutSceneModelShowController();

        private CSVBattlePassPurchase.Data m_NormalData;
        private CSVBattlePassPurchase.Data m_SuperData;

        private bool m_IsHadLoadShowScene = false;

        private List<ItemIdCount> m_NormalItems;
        private List<ItemIdCount> m_SuperItems;
        protected override void OnLoaded()
        {
            m_Layout.OnLoaded(gameObject.transform);

            m_Layout.SetListener(this);

            cutSceneController.mInfoAssetDependencies = gameObject.GetComponent<AssetDependencies>();
            cutSceneController.LoadShowScene();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_BattlePass.Instance.eventEmitter.Handle(Sys_BattlePass.EEvents.BattlePassTypeChange, OnBattlePassTypeChange, toRegister);
        }
        protected override void OnOpen(object arg)
        {
            m_NormalData = Sys_BattlePass.Instance.GetRewardPurchaseTableData(1);
            m_SuperData = Sys_BattlePass.Instance.GetRewardPurchaseTableData(2);
        }

        protected override void OnShow()
        {
            Refresh();

           var data = CSVItem.Instance.GetConfData(1);

            m_Layout.SetCostIcon(data.icon_id);

            if (!m_IsHadLoadShowScene)
                LoadModel();

        }

        private void LoadModel()
        {
            m_IsHadLoadShowScene = true;

            BattlePassModelShow modeshow = new BattlePassModelShow();
            modeshow.m_Data = Sys_BattlePass.Instance.GetRewardDisplayTableData(Sys_Role.Instance.Role.Career);
            modeshow.ModelIndex = Sys_BattlePass.Instance.GetRewardModelAssetIndex();

            cutSceneController.AddModelShow(modeshow);
            m_Layout.SetModelController(cutSceneController);
        }
        protected override void OnClose()
        {
            cutSceneController.UnLoadShowScene();

            m_IsHadLoadShowScene = false;
        }


        void Refresh()
        {



            m_Layout.SetNormalCost(m_NormalData.Price);
            m_Layout.SetNormalTips(m_NormalData.Title_Des);


            m_Layout.SetSuperCost(m_SuperData.Price);
            m_Layout.SetSuperTips(m_SuperData.Title_Des);

            m_SuperItems = CSVDrop.Instance.GetDropItem(m_SuperData.Drop_Reward);

            m_NormalItems = CSVDrop.Instance.GetDropItem(m_NormalData.Drop_Reward);

            m_Layout.SetNormalItems(m_NormalItems);
            m_Layout.SetSuperItems(m_SuperItems);

  

        }

        void OnBattlePassTypeChange()
        {
            m_Layout.SetFinishActive(true);

            var data = Sys_BattlePass.Instance.GetRewardPurchaseTableData(Sys_BattlePass.Instance.Info.BattlePassType);

            m_Layout.SetVipInfo(data);

           // m_Layout.SetSuperVipFxActive(Sys_BattlePass.Instance.Info.BattlePassType == (uint)(BattlePassType.Advance));
        }
    }
    public partial class UI_BattlePass_VIP : UI_BattlePass_VIP_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickCloseFinish()
        {
            m_Layout.SetFinishActive(false);
            CloseSelf();

            if (Sys_BattlePass.Instance.VipLastLevel >= 0)
            {
                Sys_BattlePass.Instance.VipLastLevel = 0;

                UIManager.OpenUI(EUIID.UI_BattlePass_SpecialLv, false, new UI_BattlePass_LevelUp_Parmas()
                { LastLevel = (uint)Sys_BattlePass.Instance.VipLastLevel, Level = Sys_BattlePass.Instance.Info.Level });
            }
            
        }
        public void OnClickSureNormal()
        {
            m_Layout.SetTipsActive(true);

            m_Layout.SetTipsContext(LanguageHelper.GetTextContent(3910010118));
        }

        private void GotoMall()
        {
            MallPrama mallPrama = new MallPrama();
            mallPrama.mallId = 101;
            mallPrama.isCharge = true;

            UIManager.OpenUI(EUIID.UI_Mall, false,mallPrama);
        }
        public void OnClickSureSuper()
        {
            if (Sys_Bag.Instance.GetItemCount(1) < m_SuperData.Price)
            {
                GotoMall();
                return;
            }

            Sys_BattlePass.Instance.SendBuyBattlePass(2);
        }

        public void OnClickTipsCancle()
        {
            m_Layout.SetTipsActive(false);
        }

        public void OnClickTipsSure()
        {
            if (Sys_Bag.Instance.GetItemCount(1) < m_NormalData.Price)
            {
                GotoMall();
                return;
            }

            m_Layout.SetTipsActive(false);
            Sys_BattlePass.Instance.SendBuyBattlePass(1);
        }

        public void OnNormalInfinityGridCellChange(InfinityGridCell cell,int index)
        {
           var item =  cell.mUserData as UI_BattlePass_VIP_Layout.UIItemLayout;


            item.SetItem(m_NormalItems[index].id, (uint)m_NormalItems[index].count);
        }

        public void OnSuperInfinityGridCellChange(InfinityGridCell cell, int index)
        {
            var item = cell.mUserData as UI_BattlePass_VIP_Layout.UIItemLayout;


            item.SetItem(m_SuperItems[index].id, (uint)m_SuperItems[index].count);
        }
    }
}
