using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using Packet;
using System;


namespace Logic
{
    public class UI_Pet_DemonParam
    {
        public uint type;
        public uint tuple;
    }
    public class UI_Pet_DemonPet_Layout
    {
        public Transform transform;
        public Button closeBtn;
        public Button confirmBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        private UI_Pet_MountPetInfo mountPetInfo;
        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Blank").GetComponent<Button>();
            infinityGrid = transform.Find("Animator/Scroll View").GetComponent<InfinityGrid>();
            confirmBtn = transform.Find("Animator/Btn_Confirm").GetComponent<Button>();
            mountPetInfo = new UI_Pet_MountPetInfo();
            mountPetInfo.Init(transform.Find("Animator/View_Right/Background_Root"));
        }

        public void RefreshView(ClientPet clientPet)
        {
            mountPetInfo.transform.parent.gameObject.SetActive(clientPet != null);
            if (clientPet != null)
            {
                mountPetInfo.SetPetInfo(clientPet);
            }
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            confirmBtn.onClick.AddListener(listener.OnConfirmBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnConfirmBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_Pet_DemonPet : UIBase, UI_Pet_DemonPet_Layout.IListener
    {
        public class UI_PetItem : UIComponent
        {
            private Text petNameText;
            private Text petPointText;
            private PetItem01 petitem;
            public Button itemBtn;
            private GameObject selectGo;
            private GameObject contractGo;
            public Action<UI_PetItem> action;
            public ClientPet clientPet;
            public int index;
            protected override void Loaded()
            {
                petitem = new PetItem01();
                petitem.Bind(transform.Find("PetItem01"));
                petNameText = transform.Find("Text_Name").GetComponent<Text>();
                petPointText =transform.Find("Text").GetComponent<Text>();
                selectGo = transform.Find("Image_Select").gameObject;
                contractGo =transform.Find("Image_Contract").gameObject;
                itemBtn = transform.GetComponent<Button>();
                itemBtn.onClick.AddListener(OnitemBtnClicked);
            }

            public void AddAction(Action<UI_PetItem> action)
            {
                this.action = action;
            }

            public void RefreshItem(ClientPet clientPet, int index, bool isSelect, ClientPet currentPet)
            {
                this.index = index;
                this.clientPet = clientPet;
                if (null != clientPet)
                {
                    this.clientPet = clientPet;
                    petitem.SetData(clientPet);
                    petNameText.text = clientPet.GetPetNmae();
                    petPointText.text = LanguageHelper.GetTextContent(680003043, clientPet.petUnit.SimpleInfo.Score.ToString());
                    bool hasSub = clientPet.HasSubPet();
                    bool hasPartner = clientPet.HasPartnerPet();
                    contractGo.SetActive(hasSub || hasPartner);
                }
                SetSelectState(isSelect);
            }

            public void SetSelectState(bool isSelect)
            {
                selectGo.SetActive(isSelect);
            }

            private void OnitemBtnClicked()
            {
                action?.Invoke(this);
            }
        }

        private UI_Pet_DemonPet_Layout layout = new UI_Pet_DemonPet_Layout();
        private List<UI_PetItem> itemlist = new List<UI_PetItem>();
        private UI_Pet_DemonParam param;
        private int infinityCount;
        private List<ClientPet> pets = new List<ClientPet>();
        private ClientPet partnerClient;
        private int selectIndex = -1;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg)
        {
            param = arg as UI_Pet_DemonParam;
        }

        protected override void OnShow()
        {
            SetItemView();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnNumberChangePet, OnChangePetCount, toRegister);
        }


        private void OnChangePetCount()
        {
            if(null != param)
            {
                SetItemView();
            }
        }

        protected override void OnClose()
        {
            param = null;
            partnerClient = null;
        }

        private void SetItemView()
        {
            if(param.type == 0) // 专属魔魂激活
            {
                partnerClient = Sys_Pet.Instance.GetPetByUId(param.tuple);
                pets = Sys_Pet.Instance.GetPetDemonSpiritActives(partnerClient);
            }
            else if(param.type == 1 || param.type == 2) // 改造的选宠物
            {
                partnerClient = Sys_Pet.Instance.GetPetByUId(param.tuple);
                pets = Sys_Pet.Instance.GetPetDemonSpiritRemakeActives(partnerClient, param.type);
            }
            else if(param.type == 3) //魔珠的加经验选宠
            {
                pets = Sys_Pet.Instance.GetPetDemonSpiritUpgrades(param.tuple); 
            }
            
            infinityCount = pets.Count;
            layout.SetInfinityGridCell(infinityCount);
            selectIndex = -1;
            layout.RefreshView(null);
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_PetItem entry = new UI_PetItem();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            entry.AddAction(OnItemClick);
            cell.BindUserData(entry);
            itemlist.Add(entry);
        }
        
        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= infinityCount)
                return;
            UI_PetItem entry = cell.mUserData as UI_PetItem;
            entry.RefreshItem(pets[index], index, selectIndex == index, partnerClient);
        }

        public void OnItemClick(UI_PetItem itemData)
        {
            if(selectIndex != itemData.index)
            {
                selectIndex = itemData.index;
                layout.RefreshView(itemData.clientPet);
                for (int i = 0; i < itemlist.Count; i++)
                {
                    itemlist[i].SetSelectState(selectIndex == itemlist[i].index);
                }
            }
        }
        
        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_DemonPet);
        }


        public void OnConfirmBtnClicked()
        {

            if (selectIndex >= 0 && selectIndex < pets.Count)
            {
                ClientPet clientPet = pets[selectIndex];
                var partnerPet = Sys_Pet.Instance.GetPetByUId(param.tuple);
                if (param.type <= 2 && null == partnerPet)
                    return;
                if (Sys_Pet.Instance.fightPet.IsSamePet(clientPet.petUnit))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002002));
                }
                else if (Sys_Pet.Instance.mountPetUid == clientPet.GetPetUid())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002003));
                }
                else if (Sys_Pet.Instance.followPetUid == clientPet.GetPetUid())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002004));
                }
                else if (clientPet.IsHasEquip())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002005));
                }
                else if (clientPet.HasPartnerPet())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002006));
                }
                else if (clientPet.HasSubPet())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002007));
                }
                else if (clientPet.HasEquipDemonSpiritSphere())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002008));
                }
                else if (param.type <= 2)
                {
                    if (null != clientPet.petData.soul_activate_cost && clientPet.petData.soul_activate_cost.Count >= 3)
                    {
                        var targetPoint = clientPet.petData.soul_activate_cost[2];
                        if (clientPet.petUnit.SimpleInfo.Score < targetPoint)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002009));
                            return;
                        }
                    }
                    Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnOwnDemonSpiritPetSelect, clientPet.GetPetUid());
                    OncloseBtnClicked();
                }
                else
                {
                    Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnOwnDemonSpiritPetSelect, clientPet.GetPetUid());
                    OncloseBtnClicked();
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002001));
            }
        }
    }
}
