using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class JewelQuickImproveData
    {
        public uint jewelInfoId;
        public ulong equipUId;
        public uint slotIndex;
    }

    public class UI_Jewel_Upgrade : UIBase, UI_Jewel_Upgrade_Left.IListener
    {
        private Button btnClose;
        private UI_Jewel_Upgrade_Left _LeftView;
        private UI_Jewel_Upgrade_Right _rightView;

        private JewelQuickImproveData improveData;
        private ItemData _equipItem;
        private uint _targetId;

        //private Animator animator;
        //private Timer timer;

        protected override void OnLoaded()
        {            
            btnClose = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);

            //animator = transform.Find("Animator/View_Left").GetComponent<Animator>();

            _LeftView = new UI_Jewel_Upgrade_Left();
            _LeftView.Init(transform.Find("Animator/View_Left"));
            _LeftView.Register(this);

            _rightView = new UI_Jewel_Upgrade_Right();
            _rightView.Init(transform.Find("Animator/View_Right"));
        }

        protected override void OnOpen(object arg)
        {            
            improveData = null;
            if (arg != null)
                improveData = (JewelQuickImproveData)arg;

            _equipItem = null;
            if (improveData != null)
                _equipItem = Sys_Bag.Instance.GetItemDataByUuid(improveData.equipUId);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            //base.ProcessEventsForEnable(toRegister);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfQuickCompose, OnJewelComposeNtf, toRegister);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNotifyJewelUpgradeChange, OnJewelUpgradeChange, toRegister);
        }

        protected override void OnShow()
        {            
            UpdatePanel();
        }

        //protected override void OnHide()
        //{
        //    //timer?.Cancel();
        //    //timer = null;
        //}

        //protected override void OnDestroy()
        //{
        //    //timer?.Cancel();
        //    //timer = null;
        //        
        //}

        private void UpdatePanel()
        {
            if (improveData == null || improveData.jewelInfoId == 0)
            {
                Debug.LogError("jewel is  null");
                return;
            }

            if (_equipItem == null)
            {
                Debug.LogError("jewelUpgrade _equipItem is  null");
                return;
            }

            _LeftView.UpdateInfo(improveData.jewelInfoId, _equipItem.Id);
        }

        private void OnClickClose()
        {
            this.CloseSelf();
        }

        private void OnJewelComposeNtf()
        {
            //animator.enabled = true;
            //animator.Play("UpgradeMosaic", 0, 0f);

            //timer = Timer.Register(1.55f, () =>
            //{
            //    UIManager.CloseUI(EUIID.UI_Jewel_Upgrade);
            //});
            this.CloseSelf();
        }

        private void OnJewelUpgradeChange()
        {
            _LeftView?.UpdateNeedBuy();
        }

        public void OnSelectJewel(uint jewelId, uint srcJewelId)
        {
            _targetId = jewelId;
            Sys_Equip.Instance.CalNeedUpCount(jewelId, srcJewelId);
            _rightView?.UpdateJewelList(jewelId);
            _LeftView?.UpdateNeedBuy();
        }

        public void OnClickUpgrade()
        {
            List<Sys_Equip.SubJewelData> listJewels = Sys_Equip.Instance.GetTotalSubJewelDatas();
            List<uint> jewelLvs = new List<uint>();
            List<uint> jewelNums = new List<uint>();
            for(int i = 0; i < listJewels.Count; ++i)
            {
                if (listJewels[i].jewelUseNum > 0)
                {
                    CSVJewel.Data jewelInfo = CSVJewel.Instance.GetConfData(listJewels[i].jewelId);
                    jewelLvs.Add(jewelInfo.level);
                    jewelNums.Add(listJewels[i].jewelUseNum);
                }
            }

            CSVJewel.Data targetJewel = CSVJewel.Instance.GetConfData(_targetId);

            Sys_Equip.Instance.OnQuickComposeReq(improveData.equipUId, improveData.slotIndex, jewelLvs,
                jewelNums, Sys_Equip.Instance.LeftUpgradeCount, targetJewel.level);
        }
    }
}
