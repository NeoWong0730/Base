using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Framework;
using Lib.Core;
using UnityEngine.EventSystems;
using Packet;
using System;

namespace Logic
{
    public class PetAutoOpenEvt
    {
        public ClientPet clientPet;
        public uint index;
    }

    public class UI_Pet_Automatical_Layout
    {
        public Transform transform;
        public Text petName;
        public Text petLv;
        public Text tips;
        public RawImage rawImage;
        public Image eventImage;
        public Button closeBtn;
        public Button okBtn;
        public Button resetBtn;
        public GameObject ItemGo;

        public void Init(Transform transform)
        {
            this.transform = transform;
            petName = transform.Find("Animator/View_Left/Image_Namebg/Text_Name").GetComponent<Text>();
            petLv = transform.Find("Animator/View_Left/Image_Namebg/Text_Label/Text_Level").GetComponent<Text>();
            tips = transform.Find("Animator/View_Right/Toggle/Text").GetComponent<Text>();
            rawImage = transform.Find("Animator/View_Left/Charapter").GetComponent<RawImage>();
            eventImage = transform.Find("Animator/View_Left/EventImage").GetComponent<Image>();
            closeBtn = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            okBtn = transform.Find("Animator/View_Right/Button_ok").GetComponent<Button>();
            resetBtn = transform.Find("Animator/View_Right/Button_Reset").GetComponent<Button>();
            ItemGo = transform.Find("Animator/View_Right/Viewport/Item").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            okBtn.onClick.AddListener(listener.OnokBtnClicked);
            resetBtn.onClick.AddListener(listener.OnresetBtnClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnokBtnClicked();
            void OnresetBtnClicked();
        }
    }

    public class UI_Pet_Automatical_Item : UIComponent
    {
        public uint id;
        public int autoPoint; 
        private Text attrname;
        private Text number;
        private Text addpoint;
        private Text subpoint;
        private Button addBtn;
        private Button subBtn;
        private Button tipbutton;

        public UI_Pet_Automatical_Item(uint _id,int _autoPoint) : base()
        {
            id = _id;
            autoPoint = _autoPoint;
        }

        protected override void Loaded()
        {
            attrname = transform.Find("Text_AttrName").GetComponent<Text>();
            number = transform.Find("1").GetComponent<Text>();
            addBtn = transform.Find("Button_Add").GetComponent<Button>();
            addBtn.onClick.AddListener(OnaddBtnClicked);
            subBtn = transform.Find("Button_Sub").GetComponent<Button>();
            subBtn.onClick.AddListener(OnsubBtnClicked);
            tipbutton = transform.Find("Button").GetComponent<Button>();
            tipbutton.onClick.AddListener(OnTipBtnClicked);
        }

        private void OnTipBtnClicked()
        {
            AttributeTip tip = new AttributeTip();
            tip.tipLan = CSVAttr.Instance.GetConfData(id).desc;
            UIManager.OpenUI(EUIID.UI_Attribute_Tips, false, tip);
        }

        private void OnsubBtnClicked()
        {
            Sys_Pet.Instance.canAddAutoPoint = true;
            autoPoint = 0;
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnChangeAutoPoint);
        }

        private void OnaddBtnClicked()
        {
            Sys_Pet.Instance.canAddAutoPoint = false;
            autoPoint =1;
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnChangeAutoPoint);
        }

        public void RefreshItem()
        {
            attrname.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(id).name).ToString();
            number.text = autoPoint.ToString();
            subBtn.enabled = autoPoint != 0;
            ImageHelper.SetImageGray(subBtn.GetComponent<Image>(), autoPoint == 0, true);
            addBtn.enabled = Sys_Pet.Instance.canAddAutoPoint;
            ImageHelper.SetImageGray(addBtn.GetComponent<Image>(), !Sys_Pet.Instance.canAddAutoPoint, true);

        }
    }

    public class UI_Pet_Automatical : UIBase, UI_Pet_Automatical_Layout.IListener
    {
        private UI_Pet_Automatical_Layout layout = new UI_Pet_Automatical_Layout();
        private List<UI_Pet_Automatical_Item> ItemsList = new List<UI_Pet_Automatical_Item>();

        private uint selectedAttrId;
        private ClientPet curClientPet;
        private CSVPetNew.Data csvPetData;
        public  AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        private uint index;
        private PetAutoOpenEvt petAutoOpenEvt;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            assetDependencies = transform.GetComponent<AssetDependencies>();
        }

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                petAutoOpenEvt = (PetAutoOpenEvt)arg;
                curClientPet = petAutoOpenEvt.clientPet;
                index = petAutoOpenEvt.index;
            }
        }

        protected override void OnShow()
        {
            SetValue();
            OnCreateModel();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnChangeAutoPoint, OnChangeAutoPoint, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPetAutoPoint, OnPetAutoPoint, toRegister);

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(layout.eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
        }

        private void OnChangeAutoPoint()
        {
           for(int i =0;i< ItemsList.Count; ++i)
            {
                ItemsList[i].RefreshItem();
            }
        }

        private void OnPetAutoPoint()
        {
            DefaultAttr();
            AddAttrList();
        }

        protected override void OnHide()
        {            
           OnDestroyModel();
        }

        #region Function

        private void SetValue()
        {
            csvPetData = CSVPetNew.Instance.GetConfData(curClientPet.petUnit.SimpleInfo.PetId);
            layout.petLv.text = curClientPet.petUnit.SimpleInfo.Level.ToString();
            layout.tips.text = LanguageHelper.GetTextContent(10889, Sys_Pet.Instance.GetPetName(curClientPet));
            layout.petName.text = Sys_Pet.Instance.GetPetName(curClientPet);

            DefaultAttr();
            AddAttrList();
        }
         
        private void AddAttrList()
        {
            ItemsList.Clear();
            Sys_Pet.Instance.canAddAutoPoint = curClientPet.petUnit.PetPointPlanData.Plans[(int)index].Autoselect== 0;
            foreach (var item in Sys_Pet.Instance.baseAttrs2Id)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.ItemGo, layout.ItemGo.transform.parent);
                int point = (item.Value == curClientPet.petUnit.PetPointPlanData.Plans[(int)index].Autoselect) ? 1 : 0;
                UI_Pet_Automatical_Item slider = new UI_Pet_Automatical_Item(item.Value, point);
                slider.Init(go.transform);
                slider.RefreshItem();
                ItemsList.Add(slider);
            }
            layout.ItemGo.SetActive(false);
        }

        private void DefaultAttr()
        {
            layout.ItemGo.SetActive(true);
            for (int i = 0; i < ItemsList.Count; ++i)
            {
                ItemsList[i].OnDestroy();
            }
            FrameworkTool.DestroyChildren(layout.ItemGo.transform.parent.gameObject, layout.ItemGo.name);
        }

        #endregion

        #region  ModelShow
        private void OnCreateModel()
        {
            _LoadShowScene();
            _LoadShowModel(curClientPet.petUnit.SimpleInfo.PetId);
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            sceneModel.transform.localPosition = new Vector3(100, 0, 0);
            showSceneControl.Parse(sceneModel);

            layout.rawImage.gameObject.SetActive(true);
            layout.rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);

            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }
        }

        private void _LoadShowModel(uint petid)
        {
            string _modelPath =csvPetData.model_show;

            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x + csvPetData.translation, showSceneControl.mModelPos.transform.localPosition.y + csvPetData.height, showSceneControl.mModelPos.transform.localPosition.z);
            showSceneControl.mModelPos.transform.localRotation = Quaternion.Euler(csvPetData.angle1, csvPetData.angle2, csvPetData.angle3);
            showSceneControl.mModelPos.transform.localScale = new Vector3(csvPetData.size, csvPetData.size, csvPetData.size);

        }

        public GameObject modelGo;
        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                uint weaponId = Constants.UMARMEDID;
                modelGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
                SceneActor.PetWearSet(Constants.PETWEAR_EQUIP, curClientPet.GetPetSuitFashionId(), modelGo.transform);
                petDisplay.mAnimation.UpdateHoldingAnimations(csvPetData.action_id_show, weaponId);
            }
        }

        private void OnDestroyModel()
        {
            _UnloadShowContent();
        }

        private void _UnloadShowContent()
        {
            layout.rawImage.gameObject.SetActive(false);
            layout.rawImage.texture = null;
            //petDisplay.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl.Dispose();
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

        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Automatical);
        }

        public void OnokBtnClicked()
        {
            selectedAttrId = 0;
            for (int i = 0; i < ItemsList.Count; ++i)
            {
                if (ItemsList[i].autoPoint == 1)
                {
                    selectedAttrId = ItemsList[i].id;
                }
            }
            uint total = curClientPet.petUnit.PetPointPlanData.TotalPoint;
            uint use= curClientPet.petUnit.PetPointPlanData.Plans[(int)index].UsePoint;
            uint freePoint = total- use;
            Sys_Pet.Instance.OnSetAutoPointReq(selectedAttrId, true, curClientPet.petUnit.Uid, index);
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12369));
            if (selectedAttrId != 0&& freePoint != 0)
            {           
                CSVAttr.Instance.TryGetValue(selectedAttrId, out CSVAttr.Data data);
                string content = LanguageHelper.GetTextContent(11971, freePoint.ToString(), LanguageHelper.GetTextContent(data.name));
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = content;
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    if (selectedAttrId == (uint)EBaseAttr.Vit)
                    {
                        Sys_Pet.Instance.OnAllocPointReq(curClientPet.petUnit.Uid, freePoint, 0, 0, 0, 0, index);
                    }
                    else if(selectedAttrId == (uint)EBaseAttr.Snh)
                    {
                        Sys_Pet.Instance.OnAllocPointReq(curClientPet.petUnit.Uid, 0, freePoint, 0, 0, 0, index);
                    }
                    else if (selectedAttrId == (uint)EBaseAttr.Inten)
                    {
                        Sys_Pet.Instance.OnAllocPointReq(curClientPet.petUnit.Uid, 0, 0, freePoint, 0, 0, index);
                    }
                    else if (selectedAttrId == (uint)EBaseAttr.Speed)
                    {
                        Sys_Pet.Instance.OnAllocPointReq(curClientPet.petUnit.Uid, 0, 0, 0, freePoint, 0, index);
                    }
                    else if (selectedAttrId == (uint)EBaseAttr.Magic)
                    {
                        Sys_Pet.Instance.OnAllocPointReq(curClientPet.petUnit.Uid, 0, 0, 0, 0, freePoint, index);
                    }
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }

        public void OnresetBtnClicked()
        {
            Sys_Pet.Instance.OnSetAutoPointReq(0, true, curClientPet.petUnit.Uid,index);
        }
    }
}
