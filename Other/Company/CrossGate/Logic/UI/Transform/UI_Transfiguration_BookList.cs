using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using System;

namespace Logic
{
    public class UI_Transfiguration_BookList_Layout
    {
        public Transform transform;
        public Button closeBtn;
        public Button getWayBtn;
        public Button addTipBtn;
        public Toggle showAddToggle;
        public Toggle useShapeShiftToggle;
        public GameObject attrGo;

        public Text name;
        public Text type;
        public Text lastTime;
        public Text add;
        public Text count;

        public Image eventImage;
        public InfinityGrid infinityGridMenu;
        public InfinityGrid infinityGridCard;

        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            getWayBtn = transform.Find("Animator/View_Info/View_Info/Btn_Get").GetComponent<Button>();
            addTipBtn = transform.Find("Animator/View_Info/View_Info/View_Addition/Image_Title").GetComponent<Button>();
            showAddToggle = transform.Find("Animator/View_Info/View_Info/View_Addition/Toggle").GetComponent<Toggle>();
            useShapeShiftToggle = transform.Find("Animator/View_Info/Toggle").GetComponent<Toggle>();
            attrGo = transform.Find("Animator/View_Info/View_Info/AttrGroup").gameObject;
            name = transform.Find("Animator/View_Info/Image_Namebg/Text_Name").GetComponent<Text>();
            type = transform.Find("Animator/View_Info/Image_Namebg/Text_Tag").GetComponent<Text>();
            lastTime = transform.Find("Animator/View_Info/View_Info/View_Time/TextValue").GetComponent<Text>();
            add = transform.Find("Animator/View_Info/View_Info/View_Addition/TextValue").GetComponent<Text>();
            count = transform.Find("Animator/View_Info/View_Info/Text").GetComponent<Text>();
            eventImage = transform.Find("Animator/View_Info/EventImage").GetComponent<Image>();
            infinityGridMenu = transform.Find("Animator/View_List/Scroll View").GetComponent<InfinityGrid>();
            infinityGridCard = transform.Find("Animator/View_Card/Scroll View").GetComponent<InfinityGrid>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            getWayBtn.onClick.AddListener(listener.OngetWayBtnClicked);
            showAddToggle.onValueChanged.AddListener(listener.OnshowAddValueChanged);
            useShapeShiftToggle.onValueChanged.AddListener(listener.OnuseShapeShiftValueChanged);
            addTipBtn.onClick.AddListener(listener.OnAddTipBtnClicked);
        }

        public interface IListener
        {
            void OnAddTipBtnClicked();
            void OncloseBtnClicked();
            void OngetWayBtnClicked();
            void OnshowAddValueChanged(bool isOn);
            void OnuseShapeShiftValueChanged(bool isOn);
        }
    }

    public class UI_Transfiguration_BookList_Menu
    {
        public Transform transform;
        private  CP_Toggle toggle;
        private Image iconDark;
        private Text textDark;
        private Text textLight;
        public uint raceId;

        public void Init(Transform _transform)
        {
            transform = _transform;
            toggle = transform.GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener(onValueChanged);
            iconDark = transform.Find("Btn_Menu_Dark/Icon").GetComponent<Image>();
            textDark = transform.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
            textLight = transform.Find("Image_Menu_Light/Text_Menu_Light").GetComponent<Text>();
        }

        private void onValueChanged(bool isOn)
        {
            if (isOn)
            {
                Sys_Transfiguration.Instance.eventEmitter.Trigger<uint>(Sys_Transfiguration.EEvents.OnSelectRaceMenu, raceId);
            }
        }

        public void SetData(uint id)
        {
            raceId = id;
            CSVGenus.Data data = CSVGenus.Instance.GetConfData(raceId);
            if (data == null)
            {
                return;
            }
            ImageHelper.SetIcon(iconDark,data.rale_icon);
            TextHelper.SetText(textDark, data.rale_name);
            TextHelper.SetText(textLight, data.rale_name);
        }

        public void SetToggleOn()
        {
            toggle.SetSelected(true,true);
        }
    }

    public class UI_Transfiguration_BookList : UIBase, UI_Transfiguration_BookList_Layout.IListener
    {
        private UI_Transfiguration_BookList_Layout layout = new UI_Transfiguration_BookList_Layout();
        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        private uint curPetId;
        private uint curCardId;
        private uint curRaceId;
        private CSVRaceChangeCard.Data curCSVRaceChangeCardData;

        private Dictionary<GameObject, TransformCell> itemCeils = new Dictionary<GameObject, TransformCell>();
        private List<TransformCell> transformCellItems = new List<TransformCell>();
        private int infinityCountCard;

        private Dictionary<GameObject, UI_Transfiguration_BookList_Menu> itemMenus = new Dictionary<GameObject, UI_Transfiguration_BookList_Menu>();
        private List<UI_Transfiguration_BookList_Menu> menuItems = new List<UI_Transfiguration_BookList_Menu>();
        private int infinityCountMenu;

        private List<uint> raceList = new List<uint>();
        private List<uint> curCardList = new List<uint>();
        private List<TransformAttrAddItem> attrItems = new List<TransformAttrAddItem>();

        protected override void OnOpen(object arg)
        {

        }

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            assetDependencies = transform.GetComponent<AssetDependencies>();
            raceList = Sys_Transfiguration.Instance.GetRacesIds();
            if (raceList.Count > 0)
            {
                curCardList = Sys_Transfiguration.Instance.GetTransformCardIdsByRaceId(raceList[0]);
                curRaceId = raceList[0];
                curCardId = curCardList[0];
            }
            layout.infinityGridCard.onCreateCell += OnCreateCellCard;
            layout.infinityGridCard.onCellChange += OnCellChangeCard;
            layout.infinityGridMenu.onCreateCell += OnCreateCellMenu;
            layout.infinityGridMenu.onCellChange += OnCellChangeMenu;

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(layout.eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
            infinityCountMenu = raceList.Count;
            layout.infinityGridMenu.CellCount = infinityCountMenu;
            layout.infinityGridMenu.ForceRefreshActiveCell();
        }

        protected override void OnShow()
        {
            base.OnShow();
            infinityCountMenu = raceList.Count;
            layout.infinityGridMenu.CellCount = infinityCountMenu;
            layout.infinityGridMenu.ForceRefreshActiveCell();
            layout.useShapeShiftToggle.SetIsOnWithoutNotify(OptionManager.Instance.GetBoolean(OptionManager.EOptionID.UseShapeShift, false));
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Transfiguration.Instance.eventEmitter.Handle<uint>(Sys_Transfiguration.EEvents.OnSelectRaceMenu, OnSelectRaceMenu, toRegister);
            OptionManager.Instance.eventEmitter.Handle<int>(OptionManager.EEvents.OptionFinalChange, OptionFinalChange, toRegister);
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.NetMsg_MemLeaveNtf, NetMsg_MemLeaveNtf, toRegister);
        }

        private void NetMsg_MemLeaveNtf()
        {
           
        }

        protected override void OnHide()
        {
            OnDestroyModel();
            DefaultAttrItems();
        }

        protected override void OnDestroy()
        {
            curRaceId = 0;
            curPetId = 0;
            curCardId = 0;
            curCardList.Clear();
            raceList.Clear();
        }

        #region Function

        private void OnSelectRaceMenu(uint raceId)
        {
            curCardList = Sys_Transfiguration.Instance.GetTransformCardIdsByRaceId(raceId);
            infinityCountCard = curCardList.Count;
            layout.infinityGridCard.CellCount = infinityCountCard;
            layout.infinityGridCard.ForceRefreshActiveCell();
        }


        private void OptionFinalChange(int optionId)
        {
            if (optionId == (int)OptionManager.EOptionID.UseShapeShift)
            {
                if (OptionManager.Instance.GetBoolean(OptionManager.EOptionID.UseShapeShift))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2013306));
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2013307));
                }
            }
        }

        private void OnCreateCellMenu(InfinityGridCell cell)
        {
            itemMenus.Clear();
            UI_Transfiguration_BookList_Menu entry = new UI_Transfiguration_BookList_Menu();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
            itemMenus.Add(go, entry);
        }

        private void OnCellChangeMenu(InfinityGridCell cell, int index)
        {
            uint id = raceList[index];
            UI_Transfiguration_BookList_Menu item = cell.mUserData as UI_Transfiguration_BookList_Menu;
            item.SetData(id);
            menuItems.Add(item);
            if (id == curRaceId)
            {
                item.SetToggleOn();
            }
        }

        private void OnCreateCellCard(InfinityGridCell cell)
        {
            itemCeils.Clear();
            TransformCell entry = new TransformCell();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            entry.AddAction(OnSelectItem);
            cell.BindUserData(entry);
            itemCeils.Add(go, entry);
        }

        private void OnCellChangeCard(InfinityGridCell cell, int index)
        {
            uint id = curCardList[index];
            TransformCell item = cell.mUserData as TransformCell;
            item.SetData(id, true, false);
            transformCellItems.Add(item);
            if (!curCardList.Contains(curCardId))
            {
                OnSelectItem(curCardList[0]);
            }
            else
            {
                OnSelectItem(curCardId);
            }
        }

        private void OnSelectItem(ulong cardId)
        {
            curCardId = (uint)cardId;
            curCSVRaceChangeCardData = CSVRaceChangeCard.Instance.GetConfData(curCardId);
            if (curCSVRaceChangeCardData == null)
            {
                return;
            }
            curRaceId = curCSVRaceChangeCardData.type;
            CSVTransform.Data csvTransformData = CSVTransform.Instance.GetConfData(curCSVRaceChangeCardData.change_id);
            if (csvTransformData == null)
            {
                return;
            }
            curPetId = csvTransformData.petid;
            OnCreateModel();
            if (Sys_Transfiguration.Instance.shapeShiftAddDic.ContainsKey(curCSVRaceChangeCardData.type))
            {
                uint num = Sys_Transfiguration.Instance.shapeShiftAddDic[curCSVRaceChangeCardData.type] / 100;
                TextHelper.SetText(layout.add, 10882, num.ToString());
            }
            else
            {
                TextHelper.SetText(layout.add, 10882, 0.ToString());
            }
            TextHelper.SetText(layout.name, CSVItem.Instance.GetConfData(curCardId).name_id);
            TextHelper.SetText(layout.type, Sys_Transfiguration.Instance.GetTypeLanIdByCareer(curCSVRaceChangeCardData.career));
            TextHelper.SetText(layout.lastTime, 10156, (curCSVRaceChangeCardData.last_time / 60).ToString());
            ulong countNum = Sys_Transfiguration.Instance.GetTransfornCardItemCount(curCardId);
            TextHelper.SetText(layout.count, 2013409, countNum.ToString());
            ShowAttrView();

            for (int i = 0; i < transformCellItems.Count; ++i)
            {
                transformCellItems[i].SetSelect(curCardId);
            }
        }

        private void ShowAttrView()
        {
            DefaultAttrItems();
            FrameworkTool.CreateChildList(layout.attrGo.transform, curCSVRaceChangeCardData.base_attr.Count);
            for(int i=0;i< layout.attrGo.transform.childCount; ++i)
            {
                if (i != 0)
                {
                    layout.attrGo.transform.GetChild(i).name = curCSVRaceChangeCardData.base_attr[i][0].ToString();
                }
                TransformAttrAddItem item = new TransformAttrAddItem();
                item.Init(layout.attrGo.transform.GetChild(i));
                float addNum =0;
                if (Sys_Transfiguration.Instance.shapeShiftAddDic.ContainsKey(curCSVRaceChangeCardData.type))
                {
                    addNum = Sys_Transfiguration.Instance.shapeShiftAddDic[curCSVRaceChangeCardData.type] / 100.0f;
                }
                item.SetData(curCSVRaceChangeCardData.base_attr[i][0], curCSVRaceChangeCardData.base_attr[i][1], addNum);
                attrItems.Add(item);
            }
            layout.showAddToggle.isOn = Sys_Transfiguration.Instance.isToggleShow;
            if (Sys_Transfiguration.Instance.isToggleShow)
            {
                for (int i = 0; i < attrItems.Count; ++i)
                {
                    attrItems[i].ShowAddAttr(Sys_Transfiguration.Instance.isToggleShow);
                }
            }
        }

        private void DefaultAttrItems()
        {
            attrItems.Clear();
            FrameworkTool.DestroyChildren(layout.attrGo, layout.attrGo.transform.GetChild(0).name);
        }
        #endregion

        #region  ModelShow
        private void OnCreateModel()
        {
            OnDestroyModel();
            _LoadShowScene();
            _LoadShowModel();
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            sceneModel.transform.localPosition = new Vector3((int)(EUIID.UI_Transfiguration_BookList), 0, 0);
            showSceneControl.Parse(sceneModel);

            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }
        }

        private void _LoadShowModel()
        {
            CSVPetNew.Data petdata = CSVPetNew.Instance.GetConfData(curPetId);
            string _modelPath = petdata.model_show;

            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x + petdata.translation, petdata.height, showSceneControl.mModelPos.transform.localPosition.z);
            showSceneControl.mModelPos.transform.localRotation = Quaternion.Euler(petdata.angle1, petdata.angle2, petdata.angle3);
            showSceneControl.mModelPos.transform.localScale = new Vector3(petdata.size, petdata.size, petdata.size);

        }

        public GameObject modelGo;
        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                uint weaponId = Constants.UMARMEDID;
                uint highId = curPetId;
                modelGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
                modelGo.SetActive(false);
                SceneActor.PetWearSet(Constants.PETWEAR_EQUIP, 0, modelGo.transform);
                petDisplay.mAnimation.UpdateHoldingAnimations(CSVPetNew.Instance.GetConfData(highId).action_id_show, weaponId, Constants.PetShowAnimationClipHashSet, go: modelGo);
            }
        }

        private void OnDestroyModel()
        {
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl?.Dispose();
            showSceneControl = null;
            modelGo = null;
        }

        public void OnDrag(BaseEventData eventData)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        public void AddEulerAngles(Vector3 angle)
        {
            if (showSceneControl.mModelPos.transform != null)
            {
                Vector3 localAngle = showSceneControl.mModelPos.transform.localEulerAngles;
                Vector3 newAngle = new Vector3(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = newAngle;
            }
        }

        #endregion

        #region ButtonClicked
        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Transfiguration_BookList);
        }

        public void OngetWayBtnClicked()
        {
            PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);
            iItemData.id = curCardId;
            var boxEvt = new MessageBoxEvt(EUIID.UI_Transfiguration_BookList, iItemData);
            boxEvt.b_changeSourcePos = true;
            boxEvt.pos = layout.eventImage.transform.position;
            boxEvt.b_ForceShowScource = true;
            boxEvt.b_ShowItemInfo = false;
            UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
        }

        public void OnshowAddValueChanged(bool isOn)
        {
            Sys_Transfiguration.Instance.isToggleShow = isOn;
            Sys_Transfiguration.Instance.SaveRecordFile();
            for (int i=0;i< attrItems.Count; ++i)
            {
                attrItems[i].ShowAddAttr(isOn);
            }
        }

        public void OnAddTipBtnClicked()
        {
            AttributeTip tip = new AttributeTip();
            tip.tipLan = 2013037;
            tip.pos = CameraManager.mUICamera.WorldToScreenPoint(layout.addTipBtn.GetComponent<RectTransform>().position - new Vector3(5f, 0, 0));
            UIManager.OpenUI(EUIID.UI_Attribute_Tips, false, tip);
        }

        public void OnuseShapeShiftValueChanged(bool isOn)
        {
            OptionManager.Instance.SetBoolean(OptionManager.EOptionID.UseShapeShift, isOn, false);
        }
        #endregion
    }
}

