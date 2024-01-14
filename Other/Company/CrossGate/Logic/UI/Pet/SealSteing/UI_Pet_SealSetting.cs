using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Table;

namespace Logic
{
    public class UI_Pet_SealSetting_Layout
    {
        public Transform transform;
        public Button closeButton;

        public CP_PopdownList popdownList;
        public GameObject typeListGo;
        public GameObject bookViewGo;
        public Text tipQYText;
        public Text tipMXText;
        public void Init(Transform transform)
        {
            this.transform = transform;
            popdownList = transform.Find("Animator/View_Setting/Contents/ContentProto/View_PopupList").GetComponent<CP_PopdownList>();
            typeListGo = transform.Find("Animator/View_Setting/View_TypeList").gameObject;
            bookViewGo = transform.Find("Animator/View_Setting/Contents").gameObject;
            closeButton= transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            tipQYText = transform.Find("Animator/View_Setting/Contents/ContentProto/Text_QY/Text_Num").GetComponent<Text>();
            tipMXText = transform.Find("Animator/View_Setting/Contents/ContentProto/Text_MX/Text_Num").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OncloseBtnClicked);    
        }

        public interface IListener
        {
            void OncloseBtnClicked();       
        }
    }

    public class UI_Pet_SealSetting : UIBase, UI_Pet_BookListTypeList.IListener, UI_Pet_SealSetting_Layout.IListener
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

        private UI_Pet_SealSetting_Layout layout = new UI_Pet_SealSetting_Layout();
        private UI_Pet_BookListTypeList type_List;
        private UI_Pet_SealPetList pet_List;
        public UIElementContainer<UIPetbookPopdownItem, uint> popdownVds = new UIElementContainer<UIPetbookPopdownItem, uint>();
        private List<uint> currentShowPetIds = new List<uint>();
        private List<uint> currentShowSealPetIds = new List<uint>();
        private List<uint> popupList = new List<uint>();

        private int typeId = -1;
        private uint currentPetId = 0;
        private int currentBookType = 0;

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                currentPetId = (uint)arg;
            }
        }

        protected override void OnLoaded()
        {     
            layout.Init(transform);
            layout.RegisterEvents(this);
            type_List = new UI_Pet_BookListTypeList();
            type_List.Init(layout.typeListGo.transform,true);
            pet_List = new UI_Pet_SealPetList();
            pet_List.Init(layout.bookViewGo.transform);
            type_List.RegisterListener(this);
            int length = Enum.GetValues(typeof(EPetBookType)).Length;
            for (int i = 0; i < length; ++i)
            {
                popupList.Add((uint)i);
            }
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPlayerCloseSeal, CloseWhenEvent, true);
        }

        protected override void OnShow()
        { 
            type_List.SetListByType((EPetBookType)currentBookType);
            PopdownListBuild();

            uint captureData = Sys_Adventure.Instance.GetCaptureProbability();
            uint qyData = Sys_OperationalActivity.Instance.GetSpecialCardCaptureProbability();
            bool isHasCapture = captureData != 0 ;
            bool isHasQY = qyData != 0;
            CSVWordStyle.Data cSVWordStyleDataCa = null;
            CSVWordStyle.Data cSVWordStyleDataQy = null;
            cSVWordStyleDataCa = isHasCapture?  CSVWordStyle.Instance.GetConfData(74u) : CSVWordStyle.Instance.GetConfData(152u);
            cSVWordStyleDataQy = isHasQY ?  CSVWordStyle.Instance.GetConfData(74u) : CSVWordStyle.Instance.GetConfData(152u);
            TextHelper.SetText(layout.tipQYText, LanguageHelper.GetTextContent(12493, (qyData / 100.0f).ToString()), cSVWordStyleDataQy);
            TextHelper.SetText(layout.tipMXText, LanguageHelper.GetTextContent(12493, (captureData / 100.0f).ToString()), cSVWordStyleDataCa);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPetSealSetting, OnPetSealSetting ,toRegister);
        }

        protected override void OnUpdate()
        {    
            popdownVds.Update();
        }

        protected override void OnDestroy()
        {
            popdownVds.Clear();
        }

        protected override void OnHide()
        {
            type_List.Hide();
            currentBookType = 0;
            currentPetId = 0;
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
            }
            else if ((EPetBookType)currentBookType == EPetBookType.Area)
            {
                currentShowPetIds = Sys_Pet.Instance.GetAreaBookList(_typeId == -1 ? 0 : (uint)_typeId);
            }
            else if ((EPetBookType)currentBookType == EPetBookType.Race)
            {
                currentShowPetIds = Sys_Pet.Instance.GetRarytyPetList(_typeId == -1 ? 0 : (uint)_typeId);
            }
            else if ((EPetBookType)currentBookType == EPetBookType.Mount)
            {
                currentShowPetIds = Sys_Pet.Instance.GetMountPetList((_typeId - 1) == 1 ? true : false, (typeId == -1 || typeId == 0) ? true : false);
            }
            currentShowSealPetIds.Clear();
            currentShowSealPetIds = Sys_Pet.Instance.GetCanSealBookList(currentShowPetIds);

            pet_List.RefreshListView(currentShowSealPetIds, currentPetId);
            currentPetId = 0;
        }

        private void OnPetSealSetting()
        {
            pet_List.Refresh();
        }

        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_SealSetting);
        }
    }
}
