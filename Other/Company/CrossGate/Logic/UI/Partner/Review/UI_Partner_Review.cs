using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using Framework;
using Packet;

namespace Logic
{
    public class UI_Partner_Review : UIBase, UI_Partner_Review_List.IListener
    {
        public class ArrElement
        {
            public uint attrId;
            public uint attrValue;
        }

        private UI_CurrencyTitle _currencyUI;
        private UI_Partner_Review_List list;
        private UI_Partner_Review_Left left;
        private UI_Partner_Review_Right right;

        private GameObject goElementTemplate;

        //private uint _lastInfoId;
        private uint _infoId;
        private CSVPartner.Data partnerData;

        //model show
        private AssetDependencies dependence;
        private RawImage rawImage;
        //private ParticleSystem particleShow;

        private ShowSceneControl showSceneControl;
        private DisplayControl<EHeroModelParts> heroDisplay;
        private ShowParnter showHeroActor;

        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;


        protected override void OnLoaded()
        {            
            dependence = transform.GetComponent<AssetDependencies>();

            Button btnClose = transform.Find("Animator/View_Title03/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(()=>{ this.CloseSelf(); });

            _currencyUI = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);

            list = new UI_Partner_Review_List();
            list.Init(transform.Find("Animator/Scroll_View_Partner"));
            list.Register(this);

            left = new UI_Partner_Review_Left();
            left.Init(transform.Find("Animator/View_Left"));

            right = new UI_Partner_Review_Right();
            right.Init(transform.Find("Animator/View_Right"));

            goElementTemplate = transform.Find("Animator/Image_Attr/Image_Bg").gameObject;
            goElementTemplate.SetActive(false);
            //rawImage = transform.Find("Animator/Texture").GetComponent<RawImage>();
            //rawImage.gameObject.SetActive(false);
            //particleShow = transform.transform.Find("Animator/Fx_ui_PartnerReview/Particle System (2)").GetComponent<ParticleSystem>();
            //particleShow.gameObject.SetActive(false);

            Image eventImg = transform.Find("Animator/EventImage").GetComponent<Image>();
            eventImg.gameObject.SetActive(true);
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImg);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnModelClick);
        }

        protected override void OnOpen(object arg)
        {            
            _infoId = (uint)arg;
        }

        protected override void OnShow()
        {            
            _currencyUI?.InitUi();
            list.UpdateInfo(_infoId);

            LoadSceneShow();
        }

        protected override void OnHide()
        {            
            if (_uiModelShowManagerEntity != null)
            {
                _uiModelShowManagerEntity.Dispose();
                _uiModelShowManagerEntity = null;
            }

            if (showHeroActor != null)
            {
                World.CollecActor(ref showHeroActor);
                //if (GameCenter.modelShowWorld != null)
                //    GameCenter.modelShowWorld.DestroyActor(showHeroActor);
                //showHeroActor = null;
            }
            UnloadSceneModel();
        }

        protected override void OnDestroy()
        {
            _currencyUI?.Dispose();
            left?.OnDestroy();
            right?.OnDestroy();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Partner.Instance.eventEmitter.Handle<uint>(Sys_Partner.EEvents.OnAttrChangeNotification, OnAttrChange, toRegister);
        }

        private void OnAttrChange(uint infoId)
        {
            if (_infoId == infoId)
            {
                right?.UpdateInfo(infoId);
            }
        }

        public void OnSelectParnter(uint partnerId)
        {
            _infoId = partnerId;
            partnerData = CSVPartner.Instance.GetConfData(_infoId);

            UpdateInfo(_infoId);
        }

        private void UnloadSceneModel()
        {
            //rawImage.texture = null;
            //heroDisplay?.Dispose();
            //heroDisplay = null;

            DisplayControl<EHeroModelParts>.Destory(ref heroDisplay);

            showSceneControl?.Dispose();
            showSceneControl = null;
        }

        private void UpdateInfo(uint infoId)
        {
            //particleShow.gameObject.SetActive(false);

            left.UpdateInfo(infoId);
            right.UpdateInfo(infoId);
            UpdateElement();

            Load3DModel();
        }

        private void UpdateElement()
        {
            FrameworkTool.DestroyChildren(goElementTemplate.transform.parent.gameObject, goElementTemplate.name);

            uint level = 1;
            Partner partner = Sys_Partner.Instance.GetPartnerInfo(_infoId);
            if (partner != null)
                level = partner.Level;

            List<ArrElement> list = new List<ArrElement>(2);
            CSVPartnerLevel.Data levelUpData = CSVPartnerLevel.Instance.GetUniqData(_infoId, level);
            foreach(var data in levelUpData.attribute)
            {
                if (data[0] <= 4)
                {
                    ArrElement attr = new ArrElement();
                    attr.attrId = data[0];
                    attr.attrValue = data[1];

                    list.Add(attr);
                }
            }

            for (int i = 0; i < list.Count; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(goElementTemplate);
                go.transform.SetParent(goElementTemplate.transform.parent);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                go.SetActive(true);

                Image img = go.transform.Find("Image_Attr").GetComponent<Image>();
                Text text = go.transform.Find("Image_Attr/Text").GetComponent<Text>();

                CSVAttr.Data data = CSVAttr.Instance.GetConfData(list[i].attrId);
                ImageHelper.SetIcon(img, data.attr_icon);
                text.text = list[i].attrValue.ToString();
            }
        }

        private void LoadSceneShow()
        {
            if (showSceneControl != null)
            {
                UnloadSceneModel();
            }

            showSceneControl = new ShowSceneControl();

            GameObject sceneModel = GameObject.Instantiate<GameObject>(dependence.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);
        }

        private void Load3DModel()
        {
            if (_uiModelShowManagerEntity != null)
            {
                _uiModelShowManagerEntity.Dispose();
                _uiModelShowManagerEntity = null;
            }

            if (showHeroActor != null)
            {
                //GameCenter.modelShowWorld.DestroyActor(showHeroActor);
                World.CollecActor(ref showHeroActor);
            }

            CSVEquipment.Data equipmentData = CSVEquipment.Instance.GetConfData(partnerData.weaponID);

            //showHeroActor = GameCenter.modelShowWorld.CreateActor<ShowParnter>(999);
            //showHeroActor.AnimationComponent = World.AddComponent<AnimationComponent>(showHeroActor);
            showHeroActor = World.AllocActor<ShowParnter>(999);
            showHeroActor.SetParent(GameCenter.modelShowRoot);
            showHeroActor.SetName($"ShowParnter_999");

            showHeroActor.cSVPartnerData = partnerData;     
            showHeroActor.LoadModel(partnerData.model_show, (actor) =>
            {
                actor.transform.SetParent(showSceneControl.mModelPos.transform, false);
                actor.transform.localPosition = Vector3.zero;
                actor.transform.localScale = Vector3.one;
                actor.transform.localRotation = Quaternion.identity;
                showHeroActor.WeaponID = showHeroActor.cSVPartnerData.show_weapon_id;
                
                showHeroActor.animationComponent.SetSimpleAnimation(showHeroActor.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                showHeroActor.animationComponent.UpdateHoldingAnimations(showHeroActor.cSVPartnerData.id + 100, showHeroActor.WeaponID, Constants.UIModelShowAnimationClipHashSet, EStateType.Idle, actor.gameObject);
                actor.gameObject.SetActive(false);

                _uiModelShowManagerEntity = WS_UIModelShowManagerEntity.Start(partnerData.model_show_workid, showHeroActor.animationComponent, null, actor.gameObject);
            });         
        }

        private void OnModelClick(BaseEventData eventData)
        {
            //if (!_IsUnlock)
            //    return;

            if (_uiModelShowManagerEntity != null)
                _uiModelShowManagerEntity.TouchModelOperation();
        }

        private void OnDrag(BaseEventData eventData)
        {
            //if (!_IsUnlock)
            //    return;

            if (_uiModelShowManagerEntity != null &&
                !_uiModelShowManagerEntity.IsCanControlUIOperation(WS_UIModelShowManagerEntity.ControlUIOperationEnum.OnRotateModel))
                return;

            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        private void AddEulerAngles(Vector3 angle)
        {
            if (showSceneControl.mModelPos.transform != null)
            {
                Vector3 localAngle = showSceneControl.mModelPos.transform.localEulerAngles;
                //localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                Vector3 newAngle = new Vector3(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = newAngle;
            }
        }
    }
}


