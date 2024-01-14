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
    public class UI_Pet_MountSelectItemParam
    {
        public int index;
        public uint petUid;
    }
    public class UI_Pet_MountSelectItem_Layout
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

    public class UI_Pet_MountSelectItem : UIBase, UI_Pet_MountSelectItem_Layout.IListener
    {
        public class UI_PetItem : UIComponent
        {
            private Text petNameText;
            private PetItem01 petitem;
            public Button itemBtn;
            private GameObject selectGo;
            private GameObject noneGo;
            private GameObject contractGo;
            private Transform iconTrans;
            public Action<UI_PetItem> action;
            public ClientPet clientPet;
            public int index;
            protected override void Loaded()
            {
                petitem = new PetItem01();
                petitem.Bind(transform.Find("PetItem01"));
                petNameText = transform.Find("Text_Name").GetComponent<Text>();
                selectGo = transform.Find("Image_Select").gameObject;
                noneGo = transform.Find("Text_Not").gameObject;
                iconTrans = transform.Find("Group");
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
                    bool hasSub = clientPet.HasSubPet();
                    bool hasPartner = clientPet.HasPartnerPet();
                    noneGo.SetActive(!hasSub && !hasPartner);
                    List<uint> subs = null;
                    if (hasSub)
                    {
                        subs = clientPet.GetSubsPetUid();
                    }
                    else if(hasPartner)
                    {
                        subs = new List<uint>() { clientPet.PartnerUid };
                    }
                     
                    if(null != subs)
                    {
                        for (int i = subs.Count - 1; i >= 0; i--)
                        {
                            if (subs[i] == 0)
                            {
                                subs.RemoveAt(i);
                            }
                        }
                        FrameworkTool.CreateChildList(iconTrans, subs.Count);
                        int count = subs.Count;
                        for (int i = 0; i < count; i++)
                        {
                            Transform item = iconTrans.GetChild(i);
                            ClientPet subClientPet = Sys_Pet.Instance.GetPetByUId(subs[i]);
                            if (null != subClientPet)
                            {
                                Image iconImage = item.Find("Icon").GetComponent<Image>();
                                if (null != iconImage)
                                {
                                    ImageHelper.SetIcon(iconImage, subClientPet.petData.icon_id);
                                }
                            }
                        }
                    }
                    else
                    {
                        FrameworkTool.CreateChildList(iconTrans, 0);
                    }
                    contractGo.SetActive(currentPet.IsMySubPetUid(clientPet.GetPetUid()));
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

        private UI_Pet_MountSelectItem_Layout layout = new UI_Pet_MountSelectItem_Layout();
        private List<UI_PetItem> itemlist = new List<UI_PetItem>();
        private UI_Pet_MountSelectItemParam param;
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
            param = arg as UI_Pet_MountSelectItemParam;
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
            pets = Sys_Pet.Instance.GetPetsExceptParamId(param.petUid);
            partnerClient = Sys_Pet.Instance.GetPetByUId(param.petUid);
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
            UIManager.CloseUI(EUIID.UI_Pet_MountSelectItem);
        }


        public void OnConfirmBtnClicked()
        {
            if(selectIndex >= 0 && selectIndex < pets.Count)
            {
                ClientPet clientPet = pets[selectIndex];
                var partnerPet = Sys_Pet.Instance.GetPetByUId(param.petUid);
                if (null == partnerPet)
                    return;
                if (clientPet.IsMyPartnerPetUid(partnerPet.GetPetUid()))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000204));
                }
                else if(clientPet.HasPartnerPet() || clientPet.HasSubPet())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000205));
                }
                else if (clientPet.GetPetIsDomestication())
                {
                    if(clientPet.GetMountSkill().Count > 0)
                    {
                        PromptBoxParameter.Instance.Clear();
                        PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000206);
                        PromptBoxParameter.Instance.SetConfirm(true, () =>
                        {
                            if(null != partnerPet)
                            {
                                Sys_Pet.Instance.OnPetContractSetUpReq(param.petUid, (uint)param.index, clientPet.GetPetUid());
                            }
                            OncloseBtnClicked();
                        });
                        PromptBoxParameter.Instance.SetCancel(true, null);
                        UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    }
                    else
                    {
                        if (null != partnerPet)
                        {
                            Sys_Pet.Instance.OnPetContractSetUpReq(param.petUid, (uint)param.index, clientPet.GetPetUid());
                        }
                        OncloseBtnClicked();
                    }
                }
                else
                {
                    if (null != partnerPet)
                    {
                        Sys_Pet.Instance.OnPetContractSetUpReq(param.petUid, (uint)param.index, clientPet.GetPetUid());
                    }
                    OncloseBtnClicked();
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000203));
            }
        }
    }
}
