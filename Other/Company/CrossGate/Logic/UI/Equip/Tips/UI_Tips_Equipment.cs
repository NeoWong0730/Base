using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class EquipTipsData
    {
        public ItemData equip;
        public bool isCompare = true;
        public bool isShowOpBtn = true;
        public bool isShowLock = true;
    }

    public class UI_Tips_Equipment : UIBase, UI_Tips_Equipment_Layout.IListener
    {
        private UI_Tips_Equipment_Layout layout;
        private ItemData itemEquip;
        private bool isCompare = true;
        private bool isShowOpBtn = true;
        private bool isShowLock = true;
        private CSVEquipment.Data euipInfoData;
        private ItemData leftItemData = null;

        private Lib.Core.Timer _timer;

        protected override void OnLoaded()
        {            
            layout = new UI_Tips_Equipment_Layout();
            layout.Parse(gameObject);
            layout.RegisterEvents(this);

            gameObject.transform.Find("Close").GetComponent<Button>().onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_TipsEquipment);
                Sys_LivingSkill.Instance.eventEmitter.Trigger<ulong>(Sys_LivingSkill.EEvents.OnCloseEquipTips, itemEquip.Uuid);
            });
        }

        protected override void OnOpen(object arg)
        {
            //base.OnOpen(arg0);
            EquipTipsData data = (EquipTipsData)arg;
            itemEquip = data.equip;
            isCompare = data.isCompare;
            isShowOpBtn = data.isShowOpBtn;
            isShowLock = data.isShowLock;

            euipInfoData = CSVEquipment.Instance.GetConfData(itemEquip.Id);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Equip.Instance.eventEmitter.Handle<ItemData>(Sys_Equip.EEvents.OnShowMsg, OnShowMessage, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnItemLockedChange, OnItemLockedChange, toRegister);
        }

        protected override void OnShow()
        {            
            UpdateInfo(itemEquip);
        }

        protected override void OnHide()
        {
            _timer?.Cancel();
            _timer = null;
        }

        protected override void OnDestroy()
        {
            layout?.OnDestroy();
        }

        private void UpdateInfo(ItemData equip)
        {
            itemEquip = equip;
            if (null == itemEquip || itemEquip.Equip == null)
            {
                this.CloseSelf();
                return;
            }

            leftItemData = null;

            layout.message.gameObject.SetActive(false);
            layout.btnRoot.UpdateInfo(itemEquip);
            layout.btnRoot.gameObject.SetActive(isCompare && isShowOpBtn);

            //right compare
            layout.rightCompare.UpdateItemInfo(itemEquip, isShowOpBtn, isShowLock);
            layout.rightCompare.backgroundFirst.BtnSwitch.gameObject.SetActive(false);

            layout.leftCompare.gameObject.SetActive(false);

            if (!isCompare)
                return;

            //left compare
            if (Sys_Equip.Instance.IsEquiped(itemEquip))
            {
                layout.leftCompare.gameObject.SetActive(false);
            }
            else
            {
                if (Sys_Equip.Instance.IsShowCompare(itemEquip.Id, ref leftItemData))
                {
                    layout.leftCompare.gameObject.SetActive(true);
                    layout.leftCompare.UpdateInfo(leftItemData);
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

                if (isCompare && !Sys_Equip.Instance.IsEquiped(itemEquip) && Sys_Equip.Instance.SameCountsEquipment(euipInfoData.slot_id))
                {
                    layout.leftCompare.gameObject.SetActive(true);
                }
            }
            else
            {
                layout.message.gameObject.SetActive(true);
                layout.message.UpdateInfo(item);

                if (isCompare && !Sys_Equip.Instance.IsEquiped(itemEquip) && Sys_Equip.Instance.SameCountsEquipment(euipInfoData.slot_id))
                {
                    layout.leftCompare.gameObject.SetActive(false);
                }
            }
        }

        private void OnItemLockedChange()
        {
            if (layout.leftCompare.gameObject.activeInHierarchy)
                layout.leftCompare.OnUpdateLockState();
            if (layout.rightCompare.gameObject.activeInHierarchy)
                layout.rightCompare.OnUpdateLockState(isShowLock);

            if (itemEquip != null)
                itemEquip = Sys_Bag.Instance.GetItemDataByUuid(itemEquip.Uuid);
        }

        #region interface
        public void OnIntensifyBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_TipsEquipment);

            if (!Sys_FunctionOpen.Instance.IsOpen(10300, true))
                return;

            Sys_Equip.UIEquipPrama data = new Sys_Equip.UIEquipPrama();
            data.curEquip = itemEquip;
            data.opType = Sys_Equip.EquipmentOperations.Inlay;

            UIManager.OpenUI(EUIID.UI_Equipment, false, data);
        }

        public void OnRepalceBtnClicked()
        {
            if (Sys_Equip.Instance.IsEquiped(itemEquip)) //身上装备
            {
                Sys_Equip.Instance.UnloadEquipReq(itemEquip.Uuid);
            }
            else
            {
                uint desSlotId = euipInfoData.slot_id[0];
                bool canEquiped = true;

                if (canEquiped)
                {
                    //需要判断是否替换装备
                    if (leftItemData != null)
                    {
                        if (Sys_Equip.Instance.IsInlayJewel(leftItemData))
                        {
                            PromptBoxParameter.Instance.OpenPromptBox(4100, 0, 
                            () =>
                            {
                                //不满足条件，弹提示
                                if (!Sys_Equip.Instance.CanReplaceEquipJewels(leftItemData, itemEquip))
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4180));
                                }
                                else
                                {
                                    Sys_Equip.Instance.OnEquipReq(desSlotId, itemEquip, true);
                                }
                            }, 
                            ()=> 
                            {
                                Sys_Equip.Instance.OnEquipReq(desSlotId, itemEquip);
                            });
                        }
                        else
                        {
                            Sys_Equip.Instance.OnEquipReq(desSlotId, itemEquip);
                        }
                    }
                    else
                    {
                        Sys_Equip.Instance.OnEquipReq(desSlotId, itemEquip);
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4055));
                }
            }
        }

        public void OnDecomposeBtnClicked()
        {
            //安全锁
            if (Sys_Equip.Instance.IsSecureLock(itemEquip))
                return;
            
            //镶嵌宝石
            if (Sys_Equip.Instance.IsInlayJewel(itemEquip))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4211u));
                return;
            }

            //品质提示
            if (itemEquip.Quality >= Sys_Equip.Instance.QualityLimit)
            {
                PromptBoxParameter.Instance.OpenPromptBox(4220, 0, 
                () => {
                    _timer?.Cancel();
                    _timer = null;
                    _timer = Lib.Core.Timer.Register(0.1f, () =>
                    {
                       OnDecomomposeLock();
                    });
                }, null);
                return;
            }

            OnDecomomposeLock();
        }

        private void OnDecomomposeLock()
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
            else
            {
                Sys_Equip.Instance.OnEquipmentDecomposeReq(itemEquip.Uuid);
                this.CloseSelf();
            }
        }

        public void OnSaleBtnClicked()
        {
            //安全锁
            if (Sys_Equip.Instance.IsSecureLock(itemEquip))
                return;
        
            //宝石提示
            if (Sys_Equip.Instance.IsInlayJewel(itemEquip))
            {
                PromptBoxParameter.Instance.OpenPromptBox(4207, 0, 
                () =>{
                    _timer?.Cancel();
                    _timer = null;
                    _timer = Lib.Core.Timer.Register(0.1f, () =>
                    {
                        //品质提示
                        if (itemEquip.Quality >= Sys_Equip.Instance.QualityLimit)
                        {
                            PromptBoxParameter.Instance.OpenPromptBox(4221, 0,
                            () => {
                                _timer?.Cancel();
                                _timer = null;
                                _timer = Lib.Core.Timer.Register(0.1f, () =>
                                {
                                    OnSlaleCheckLock();
                                });
                            }, null);
                            return;
                        }
                        else
                        {
                            OnSlaleCheckLock();
                        }
                    });
                }, null);

                return;
            }

            //品质提示
            if (itemEquip.Quality >= Sys_Equip.Instance.QualityLimit)
            {
                PromptBoxParameter.Instance.OpenPromptBox(4221, 0,
                () => {
                    _timer?.Cancel();
                    _timer = null;
                    _timer = Lib.Core.Timer.Register(0.1f, () =>
                    {
                        OnSlaleCheckLock();
                    });
                }, null);
                return;
            }
            
            
            OnSlaleCheckLock();
        }

        private void OnSlaleCheckLock()
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
            else
            {
                Sys_Bag.Instance.SaleItem(itemEquip.Uuid, itemEquip.Count);
                this.CloseSelf();
            }
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

            if (itemEquip.marketendTimer.foreverMarket)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011281));
                return;
            }

            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(itemEquip.Id);
            if (null == equipInfo)
            {
                Lib.Core.DebugUtil.LogErrorFormat("CSVEquipment is NULL = {0}", itemEquip.Id);
                return;
            }

            bool isFixedScore = equipInfo.sale_least != 0u && Sys_Equip.Instance.CalEquipQualityScore(itemEquip) >= equipInfo.sale_least;
            if (!isFixedScore)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4184));
                return;
            }

            //是否镶嵌了宝石
            if (Sys_Equip.Instance.IsInlayJewel(itemEquip))
            {
                PromptBoxParameter.Instance.OpenPromptBox(4207, 0, () => {
                    Sys_Trade.Instance.SaleItem(itemEquip);
                    this.CloseSelf();
                }, null);
            }
            else
            {
                Sys_Trade.Instance.SaleItem(itemEquip);
                this.CloseSelf();
            }
        }
        #endregion
    }
}

