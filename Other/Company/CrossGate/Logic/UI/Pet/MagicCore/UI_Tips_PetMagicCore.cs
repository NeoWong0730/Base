using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    public class PetEquipTipsData
    {
        public ItemData petEquip;
        public EUIID openUI;
        public uint petUid;
        public bool isCompare = false;//是否比较
        public bool isShowOpBtn = true;//是否显示左侧功能按钮
        public bool isShowLock = true;
    }
    public class UI_Tips_PetMagicCore_Layout
    {
        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }

        public TipPetEquipBtnRoot btnRoot;
        public TipPetEquipMessage message;

        public TipPetEquipRightCompare rightCompare;
        public TipPetEquipLeftCompare leftCompare;

        public void Parse(GameObject root)
        {
            mRoot = root;
            mTrans = root.transform;

            btnRoot = new TipPetEquipBtnRoot();
            btnRoot.Init(mTrans.Find("View_Tips/Button_Root"));

            message = new TipPetEquipMessage();
            message.Init(mTrans.Find("View_Tips/Message"));

            rightCompare = new TipPetEquipRightCompare();
            rightCompare.Init(mTrans.Find("View_Tips/Tips01"));

            leftCompare = new TipPetEquipLeftCompare();
            leftCompare.Init(mTrans.Find("View_Tips/Tips02"));
        }

        public void OnDestroy()
        {
            btnRoot.OnDestroy();
            message.OnDestroy();
            rightCompare.OnDestroy();
            leftCompare.OnDestroy();
        }

        public void RegisterEvents(IListener listener)
        {
            btnRoot.btnDisboard.onClick.AddListener(listener.OnDisBoardBtnClicked);
            btnRoot.btnReplace.onClick.AddListener(listener.OnRepalceBtnClicked);
            btnRoot.btnDecompose.onClick.AddListener(listener.OnDecomposeBtnClicked);
            btnRoot.btnTrade.onClick.AddListener(listener.OnTradeBtnClicked);
            btnRoot.btnRemake.onClick.AddListener(listener.OnRemakeBtnClicked);
        }

        public interface IListener
        {
            void OnDisBoardBtnClicked();
            void OnRepalceBtnClicked();
            void OnDecomposeBtnClicked();
            void OnTradeBtnClicked();
            void OnRemakeBtnClicked();
        }
    }

    public class UI_Tips_PetMagicCore : UIBase, UI_Tips_PetMagicCore_Layout.IListener
    {
        private UI_Tips_PetMagicCore_Layout layout;
        private ItemData itemEquip;
        private bool isCompare = true;
        private bool isShowOpBtn = true;
        private bool isShowLock = true;
        private CSVPetEquip.Data euipInfoData;
        private ItemData leftItemData = null;
        private uint petUid;
        private EUIID openUIId;
        private Lib.Core.Timer _timer;

        protected override void OnLoaded()
        {
            layout = new UI_Tips_PetMagicCore_Layout();
            layout.Parse(gameObject);
            layout.RegisterEvents(this);

            gameObject.transform.Find("Close").GetComponent<Button>().onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_Tips_PetMagicCore);
            });
        }

        protected override void OnOpen(object arg)
        {
            //base.OnOpen(arg0);
            PetEquipTipsData data = (PetEquipTipsData)arg;
            itemEquip = data.petEquip;
            isCompare = data.isCompare;
            isShowOpBtn = data.isShowOpBtn;
            isShowLock = data.isShowLock;
            petUid = data.petUid;
            openUIId = data.openUI;
            euipInfoData = CSVPetEquip.Instance.GetConfData(itemEquip.Id);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Equip.Instance.eventEmitter.Handle<ItemData>(Sys_Equip.EEvents.OnShowMsg, OnShowMessage, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnItemLockedChange, OnItemLockedChange, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<uint>(Sys_Bag.EEvents.OnItemPetEquipLockedChange, OnItemPetEquipLockedChange, toRegister);
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

        private void UpdateInfo(ItemData petEquip)
        {
            itemEquip = petEquip;
            if (null == itemEquip || itemEquip.petEquip == null)
            {
                this.CloseSelf();
                return;
            }

            leftItemData = null;

            layout.message.gameObject.SetActive(false);
            bool showBtn = isShowOpBtn && (openUIId == EUIID.UI_Bag || openUIId == EUIID.UI_Pet_Message);
            if (showBtn)
            {
                layout.btnRoot.UpdateInfo(itemEquip, petUid, openUIId);
            }
            
            layout.btnRoot.gameObject.SetActive(showBtn);

            //right compare
            layout.rightCompare.UpdateItemInfo(itemEquip, petUid, isShowOpBtn, isShowLock);

            layout.leftCompare.gameObject.SetActive(false);

            if (!isCompare)
                return;

            //left compare
            if (Sys_Pet.Instance.IsEquiped(itemEquip.Uuid, petUid))
            {
                layout.leftCompare.gameObject.SetActive(false);
            }
            else
            {
                if (Sys_Pet.Instance.IsShowCompare(itemEquip.Id, petUid, ref leftItemData))
                {
                    layout.leftCompare.gameObject.SetActive(true);
                    layout.leftCompare.UpdateInfo(leftItemData, petUid, isShowLock);
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
                if (Sys_Pet.Instance.IsShowCompare(itemEquip.Id, petUid, ref leftItemData))
                {
                    layout.leftCompare.gameObject.SetActive(isCompare && true);
                }
            }
            else
            {
                layout.message.gameObject.SetActive(true);
                layout.message.UpdateInfo(item);
                layout.leftCompare.gameObject.SetActive(false);
            }
        }
        
        private void OnItemLockedChange()
        {
            if (layout.leftCompare.gameObject.activeInHierarchy)
                layout.leftCompare.OnUpdateLockState(isShowLock);
            if (layout.rightCompare.gameObject.activeInHierarchy)
                layout.rightCompare.OnUpdateLockState(isShowLock);
        }

        private void OnItemPetEquipLockedChange(uint petUid)
        {
            if (layout.rightCompare.gameObject.activeInHierarchy)
                layout.rightCompare.OnUpdatePetEquipLockState(petUid);
        }

        #region interface

        //替换或者装备
        public void OnRepalceBtnClicked()
        {
            if (Sys_Pet.Instance.IsEquiped(itemEquip.Uuid, petUid)) //身上装备
            {
                if (Sys_Bag.Instance.BoxFull(Packet.BoxIDEnum.BoxIdNormal))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000900));
                    return;
                }
                UIManager.OpenUI(EUIID.UI_SelectPetEquip, false, new Tuple<uint, uint, uint>(petUid, euipInfoData.equipment_category, 1018201u));
                this.CloseSelf();
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_Pet_Message, false, new MessageEx
                {
                    messageState = EPetMessageViewState.Attribute,
                });
                this.CloseSelf();
            }
        }

        public void OnDecomposeBtnClicked()
        {
            //安全锁
            if (Sys_Pet.Instance.IsPetEquipBeEffectWithSecureLock(itemEquip.petEquip))
                return;
            
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
                PromptBoxParameter.Instance.OpenPromptBox(680000920, 0, () => 
                {
                    Sys_Pet.Instance.ItemDecomposePetEquipReq(itemEquip.Uuid);
                });
            }
            
            this.CloseSelf();
        }

        public void OnTradeBtnClicked()
        {
            if (null == itemEquip)
            {
                Lib.Core.DebugUtil.LogError("item is NULL");
                return;
            }
            if (itemEquip.petEquip.Color < 4)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000923));
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

            

            CSVPetEquip.Data equipInfo = CSVPetEquip.Instance.GetConfData(itemEquip.Id);
            if (null == equipInfo)
            {
                Lib.Core.DebugUtil.LogErrorFormat("CSVPetEquip is NULL = {0}", itemEquip.Id);
                return;
            }


            Sys_Trade.Instance.SaleItem(itemEquip);
            this.CloseSelf();
        }

        public void OnDisBoardBtnClicked()
        {
            if(Sys_Bag.Instance.BoxFull(Packet.BoxIDEnum.BoxIdNormal))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000900));
                return;
            }
            Sys_Pet.Instance.ItemFitPetEquipReq(euipInfoData.equipment_category, 0, petUid);
            this.CloseSelf();
        }

        public void OnRemakeBtnClicked()
        {
            MagicCorePrama magicCorePrama = new MagicCorePrama
            {
                pageType = 2
            };
            UIManager.OpenUI(EUIID.UI_PetMagicCore, false, magicCorePrama);
            this.CloseSelf();
        }
        #endregion
    }
}