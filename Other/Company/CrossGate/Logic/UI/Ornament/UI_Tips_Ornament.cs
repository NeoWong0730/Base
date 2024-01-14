using UnityEngine.UI;
using UnityEngine;
using Logic.Core;
using Table;

namespace Logic
{
    public class OrnamentTipsData
    {
        public ItemData equip;
        public bool isCompare = true;
        public bool isShowOpBtn = true;
        public bool isShowSourceBtn = false;
        public bool isShowLock = true;
        public EUIID sourceUiId;
    }
    public class UI_Tips_Ornament : UIBase, UI_Tips_Ornament_Layout.IListener
    {
        private UI_Tips_Ornament_Layout layout;
        private ItemData itemEquip;
        private bool isCompare = true;
        private bool isShowOpBtn = true;
        private bool isShowSourceBtn = false;
        private bool isShowLock = true;
        private CSVOrnamentsUpgrade.Data ornamentData;
        private ItemData leftItemData = null;

        private ItemSource m_ItemSource;
        private GameObject m_SourceViewRoot;
        private bool bSourceActive;
        private uint sourceUiID;

        protected override void OnLoaded()
        {            
            layout = new UI_Tips_Ornament_Layout();
            layout.Parse(gameObject);
            layout.RegisterEvents(this);

            gameObject.transform.Find("Close").GetComponent<Button>().onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_Tips_Ornament);
            });

            m_SourceViewRoot = transform.Find("View_Tips/View_Right").gameObject;
            m_ItemSource = new ItemSource();
            m_ItemSource.BindGameObject(m_SourceViewRoot);
        }

        protected override void OnOpen(object arg)
        {
            //base.OnOpen(arg0);
            OrnamentTipsData data = (OrnamentTipsData)arg;
            itemEquip = data.equip;
            isCompare = data.isCompare;
            isShowOpBtn = data.isShowOpBtn;
            isShowSourceBtn = data.isShowSourceBtn;
            isShowLock = data.isShowLock;
            sourceUiID = (uint)data.sourceUiId;
            ornamentData = CSVOrnamentsUpgrade.Instance.GetConfData(itemEquip.Id);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            //Sys_Ornament.Instance.eventEmitter.Handle<ItemData>(Sys_Ornament.EEvents.OnShowMsg, OnShowMessage, toRegister);
            Sys_Equip.Instance.eventEmitter.Handle<ItemData>(Sys_Equip.EEvents.OnShowMsg, OnShowMessage, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnItemLockedChange, OnItemLockedChange, toRegister);
        }

        protected override void OnShow()
        {            
            ProcessItemSource();
            UpdateInfo(itemEquip);
        }        

        protected override void OnDestroy()
        {
            layout?.OnDestroy();
        }

        private void UpdateInfo(ItemData equip)
        {
            itemEquip = equip;

            leftItemData = null;

            layout.message.gameObject.SetActive(false);
            layout.btnRoot.bSourceActive = bSourceActive;
            layout.btnRoot.UpdateInfo(itemEquip);
            layout.btnRoot.gameObject.SetActive(isCompare && isShowOpBtn);

            //right compare
            layout.rightCompare.UpdateItemInfo(itemEquip, isShowOpBtn, isShowLock);
            layout.rightCompare.backgroundFirst.BtnSwitch.gameObject.SetActive(false);
            layout.rightCompare.tipInfo.btnSource.gameObject.SetActive(isShowSourceBtn);
            layout.leftCompare.gameObject.SetActive(false);
            layout.viewSource.gameObject.SetActive(false);

            if (!isCompare)
                return;

            //left compare
            if (Sys_Ornament.Instance.IsEquiped(itemEquip))
            {
                layout.leftCompare.gameObject.SetActive(false);
            }
            else
            {
                if (Sys_Ornament.Instance.IsShowCompare(itemEquip, ref leftItemData))
                {
                    layout.leftCompare.gameObject.SetActive(true);
                    layout.leftCompare.UpdateInfo(leftItemData, false);
                }
                else
                {
                    layout.leftCompare.gameObject.SetActive(false);
                }
            }
        }

        private void OnShowMessage(ItemData item)
        {
            if (layout.message.gameObject.activeInHierarchy)
            {
                layout.message.gameObject.SetActive(false);

                if (isCompare && !Sys_Ornament.Instance.IsEquiped(itemEquip))
                {
                    layout.leftCompare.gameObject.SetActive(true);
                }
            }
            else
            {
                layout.message.gameObject.SetActive(true);
                layout.message.UpdateInfo(item);

                if (isCompare && !Sys_Ornament.Instance.IsEquiped(itemEquip))
                {
                    layout.leftCompare.gameObject.SetActive(false);
                }
            }
        }
        private void ProcessItemSource()
        {
            bSourceActive = m_ItemSource.SetData(itemEquip.Id, sourceUiID, EUIID.UI_Tips_Ornament);
        }
        
        private void OnItemLockedChange()
        {
            if (layout.leftCompare.gameObject.activeInHierarchy)
                layout.leftCompare.OnUpdateLockState();
            if (layout.rightCompare.gameObject.activeInHierarchy)
                layout.rightCompare.OnUpdateLockState(isShowLock);
        }
        
        #region interface
        public void OnIntensifyBtnClicked()
        {
            //先判断一下饰品升级是否开启
            if (Sys_Ornament.Instance.CheckIsOpen(true))
            {
                UIManager.CloseUI(EUIID.UI_Tips_Ornament);
                if (Sys_Ornament.Instance.IsEquiped(itemEquip))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000550));//穿戴的装备无法升级，请先卸下;
                }
                else
                {
                    Sys_Ornament.Instance.UpgradeTargetUuid = itemEquip.Uuid;
                    var prama = new OrnamentPrama
                    {
                        itemUuid = itemEquip.Uuid
                    };
                    UIManager.OpenUI(EUIID.UI_Ornament, false, prama);
                }
            }
        }

        public void OnRecastBtnClicked() 
        {
            //满足重铸条件
            if (Sys_Ornament.Instance.CheckRecastIsOpen(true))
            {
                var prama = new OrnamentPrama
                {
                    itemUuid = itemEquip.Uuid,
                    pageType = 2
                };
                UIManager.OpenUI(EUIID.UI_Ornament, false, prama);
                UIManager.CloseUI(EUIID.UI_Tips_Ornament);
            }
        }
        public void OnRepalceBtnClicked()
        {
            if (Sys_Ornament.Instance.IsEquiped(itemEquip)) //身上装备
            {
                Sys_Ornament.Instance.OrnamentFitReq(ornamentData.type);
            }
            else
            {
                if (Sys_Ornament.Instance.CheckCanEquipByLv(itemEquip,true))
                {
                    Sys_Ornament.Instance.OrnamentFitReq(ornamentData.type, itemEquip.Uuid);
                }
            }
            this.CloseSelf();
        }

        public void OnDecomposeBtnClicked()
        {
            if (itemEquip.IsLocked)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, LanguageHelper.GetTextContent(itemEquip.cSVItemData.name_id), LanguageHelper.GetTextContent(itemEquip.cSVItemData.name_id));
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_Bag.Instance.OnItemLockReq(itemEquip.Uuid, false);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            else if (Sys_Ornament.Instance.CheckCanResolve(itemEquip, true))
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_Ornament.Instance.OrnamentDecomposeReq(itemEquip.Uuid);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(4231);
                UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
            }
            this.CloseSelf();
        }

        public void OnSaleBtnClicked()
        {
            PromptBoxParameter.Instance.OpenPromptBox(4230, 0, () =>
            {
                if (itemEquip.Count > 1)
                {
                    UIManager.OpenUI(EUIID.UI_Sale_Prop, false, itemEquip);
                }
                else
                {
                    Sys_Bag.Instance.SaleItem(itemEquip.Uuid, itemEquip.Count);
                }
                this.CloseSelf();
            });
        }

        public void OnTradeBtnClicked()
        {
            if (null == itemEquip)
            {
                Lib.Core.DebugUtil.LogError("item is NULL");
                return;
            }
            if (itemEquip.bBind)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4185));
                return;
            }

            Sys_Trade.Instance.SaleItem(itemEquip);
            this.CloseSelf();
        }

        public void OnSourceBtnClicked()
        {
            m_ItemSource.Show();
            //layout.viewSource.UpdateInfo(itemEquip);
        }

        public void OnBagSourceBtnClicked()
        {
            m_ItemSource.Show();
        }
        #endregion
    }
}
