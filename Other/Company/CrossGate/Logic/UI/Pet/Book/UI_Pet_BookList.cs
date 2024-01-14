using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Pet_BookList_Layout
    {
        public Transform transform;
        public Button closeButton;

        public CP_PopdownList popdownList;
        public GameObject typeListGo;
        public GameObject bookViewGo;
        public void Init(Transform transform)
        {
            this.transform = transform;
            popdownList = transform.Find("Contents/ContentProto/View_PopupList").GetComponent<CP_PopdownList>();
            typeListGo = transform.Find("View_TypeList").gameObject;
            bookViewGo = transform.Find("Contents").gameObject;
        }
    }

    public class UI_Pet_BookList : UIComponent, UI_Pet_BookListTypeList.IListener
    {
        public class UIPetbookPopdownItem : UISelectableElement
        {
            public uint select_Index;
            public string haunted_areaName;

            public Text text;
            public Button button;
            public GameObject highlight;

            protected override void Loaded()
            {
                button = transform.GetComponent<Button>();
                text = transform.Find("Text").GetComponent<Text>();
                highlight = transform.Find("Image").gameObject;
                button.onClick.AddListener(OnBtnClicked);
            }

            public void SetHighlight(bool setHighLight = false)
            {
                highlight.SetActive(setHighLight);
            }
            public void Refresh(uint select_Index, int index)
            {
                this.select_Index = select_Index;

                TextHelper.SetText(text, 680000700 + select_Index);
                haunted_areaName = text.text;
            }
            private void OnBtnClicked()
            {
                onSelected?.Invoke((int)select_Index, true);
            }
            public override void SetSelected(bool toSelected, bool force) { OnBtnClicked(); }
        }

        private UI_Pet_BookList_Layout layout = new UI_Pet_BookList_Layout();
        private UI_Pet_BookListTypeList type_List;
        private UI_Pet_BookListPetList pet_List;
        public UIElementContainer<UIPetbookPopdownItem, uint> popdownVds = new UIElementContainer<UIPetbookPopdownItem, uint>();
        private List<uint> currentShowPetIds;
        private int typeId = -1;
        private int hauntedIndex = -1;
        private List<uint> popupList = new List<uint>();
        private int currentBookType = 0;
        public Button oneKeyUpgradBookLevelBtn;
        protected override void Loaded()
        {
            layout.Init(transform);
            type_List = new UI_Pet_BookListTypeList();
            type_List.Init(layout.typeListGo.transform);
            pet_List = new UI_Pet_BookListPetList();
            pet_List.Init(layout.bookViewGo.transform);
            type_List.RegisterListener(this);
            int length = Enum.GetValues(typeof(EPetBookType)).Length;
            for (int i = 0; i < length; ++i)
            {
                popupList.Add((uint)i);
            }
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPlayerCloseSeal, CloseWhenEvent, true);
            oneKeyUpgradBookLevelBtn = transform.Find("Contents/ContentProto/Btn_01").GetComponent<Button>();
            oneKeyUpgradBookLevelBtn.onClick.AddListener(OnKeyUpgradBook);
        }

        private void OnKeyUpgradBook()
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15238);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                if(Sys_Pet.Instance.BookAllCanActive())
                {
                    Sys_Pet.Instance.OnPetLoveExpUpAllReq();
                }
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public override void Show()
        {
            base.Show();
            type_List.SetListByType((EPetBookType)currentBookType);
            PopdownListBuild();
        }

        public void ShowEx()
        {
            if ((EPetBookType)currentBookType == EPetBookType.Genus)
            {
                currentShowPetIds = Sys_Pet.Instance.GetBookList(typeId == -1 ? 0 : (uint)typeId);
            }
            else if ((EPetBookType)currentBookType == EPetBookType.Area)
            {
                currentShowPetIds = Sys_Pet.Instance.GetAreaBookList(typeId == -1 ? 0 : (uint)typeId);
            }
            else if ((EPetBookType)currentBookType == EPetBookType.Race)
            {
                currentShowPetIds = Sys_Pet.Instance.GetRarytyPetList(typeId == -1 ? 0 : (uint)typeId);
            }
            else if ((EPetBookType)currentBookType == EPetBookType.Mount)
            {
                currentShowPetIds = Sys_Pet.Instance.GetMountPetList(typeId == -1 ? false : (typeId - 1) == 1 ? true : false, (typeId == -1 || typeId == 0) ? true : false);
            }
            pet_List.Refresh2(currentShowPetIds);
            type_List.RefreshCellData();
        }

        public void SeInitType(int sub)
        {
            currentBookType = sub;
            //(EPetBookType)currentBookType = (EPetBookType)sub;
        }

        protected override void Update()
        {
            popdownVds.Update();
        }

        public override void OnDestroy()
        {
            popdownVds.Clear();
            base.OnDestroy();
        }

        public override void Hide()
        {
            base.Hide();
            type_List.Hide();
            currentBookType = 0;
        } 

        public void ChangeToggle()
        {
            base.Hide();
            type_List.Hide();
            currentBookType = 0;
        }

        private void PopdownListBuild()
        {
            popdownVds.BuildOrRefresh(layout.popdownList.optionProto, layout.popdownList.optionParent, popupList, (vd, data, index) =>
            {
                vd.SetUniqueId((int)data);
                vd.SetSelectedAction((_hauntedId, force) =>
                {
                    int vdIndex = index;
                    bool change = false;
                    if ((EPetBookType)currentBookType != (EPetBookType)vdIndex)
                    {
                        change = true;
                    }

                    currentBookType = vdIndex;
                    popdownVds.ForEach((e) =>
                    {
                        e.SetHighlight(false);
                    });
                    vd.SetHighlight(true);

                    layout.popdownList.Expand(false);
                    layout.popdownList.SetSelected(vd.haunted_areaName);
                    if (change)
                    {
                        SetBookTypeList();
                    }

                });
                vd.Refresh(data, index);
                vd.SetHighlight(false);
            });
            popdownVds.onFinish = OnFinish;
        }

        public void OnFinish()
        {
            if (currentBookType == 0)
            {
                if (popdownVds.Count > 0)
                {
                    popdownVds[currentBookType].SetSelected(true, true);
                }
            }
            else
            {
                if (0 <= currentBookType && currentBookType < popupList.Count)
                {
                    popdownVds[currentBookType].SetSelected(true, true);
                }
            }
        }

        private void CloseWhenEvent()
        {
            typeId = -1;
            //hauntedIndex = -1;
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPlayerCloseSeal, CloseWhenEvent, false);
        }

        private void SetBookTypeList()
        {
            type_List.SetListByType((EPetBookType)currentBookType);
        }

        public void OnSelectListIndex(int _typeId)
        {
            typeId = _typeId;
            if ((EPetBookType)currentBookType == EPetBookType.Genus)
            {
                currentShowPetIds = Sys_Pet.Instance.GetBookList(_typeId == -1 ? 0 : (uint)_typeId);
                pet_List.RefreshListView(currentShowPetIds);
            }
            else if ((EPetBookType)currentBookType == EPetBookType.Area)
            {
                currentShowPetIds = Sys_Pet.Instance.GetAreaBookList(_typeId == -1 ? 0 : (uint)_typeId);
                pet_List.RefreshListView(currentShowPetIds);
            }
            else if ((EPetBookType)currentBookType == EPetBookType.Race)
            {
                currentShowPetIds = Sys_Pet.Instance.GetRarytyPetList(_typeId == -1 ? 0 : (uint)_typeId);
                pet_List.RefreshListView(currentShowPetIds); 
            }
            else if ((EPetBookType)currentBookType == EPetBookType.Mount)
            {
                currentShowPetIds = Sys_Pet.Instance.GetMountPetList((_typeId - 1) == 1 ? true : false, (typeId == -1 || typeId == 0) ? true : false);
                pet_List.RefreshListView(currentShowPetIds);
            }
        }

        public void Active()
        {
            type_List.RefreshCellData();
            pet_List.Refresh();
        }
    }
}

