using System.Collections;
using System.Collections.Generic;
using Lib.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{    
    public class BagEquipmentBase
    {
        private Item0_Layout Layout = new Item0_Layout();
        private Image imgEmptyIcon;

        private ItemData curItem;
        public uint slotId = 0;

        public void BindGameObject(GameObject go)
        {
            Transform transform = go.transform;
            Layout.BindGameObject(transform.Find("Btn_Item").gameObject);
            imgEmptyIcon = transform.Find("Image_Equip").GetComponent<Image>();

            Layout.btnItem.onClick.AddListener(OnIconCliked);
        }

        public void SetData(ItemData _item)
        {
            curItem = _item;

            Layout.imgIcon.gameObject.SetActive(curItem != null);
            Layout.imgQuality.gameObject.SetActive(curItem != null);
            imgEmptyIcon.gameObject.SetActive(false);
            //imgEmptyIcon.gameObject.SetActive(curItem == null);

            if (curItem != null)
            {
                Layout.SetData(_item.cSVItemData, true);
                ImageHelper.GetQualityColor_Frame(Layout.imgQuality, (int)_item.Quality);
            }
        }

        private void OnIconCliked()
        {
            if (curItem != null)
            {
                EquipTipsData tipData = new EquipTipsData();
                tipData.equip = curItem;
                
                UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
            }
            else
            {
                //TODO: tips
            }
        }
    }

    public class UI_Bag_Equipment : UIComponent
    {
        private Dictionary<uint, string> dictParam = new Dictionary<uint, string>();
        private Dictionary<uint, BagEquipmentBase> bagEquipDict = new Dictionary<uint, BagEquipmentBase>();
        private Text battleScore;

        private RawImage rawImage;
        private Button btnUpgrade;

        private ShowSceneControl showSceneControl;
        private HeroLoader heroLoader;

        private Object sceneModelObj;
        public Object SceneObj
        {
            set {
                sceneModelObj = value;
                ProcessSceenShow(value);
            }
        }

        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;

        protected override void Loaded()
        {
            base.Loaded();

            //set param
            dictParam.Clear();
            dictParam.Add((uint)EquipmentSlot.EquipSlotWeapon1, "View_Equip/PropItemWeapon01");
            dictParam.Add((uint)EquipmentSlot.EquipSlotWeapon2, "View_Equip/PropItemWeapon02");
            //dictParam.Add((uint)EquipmentSlot.EquipSlotCrystal, "View_Equip/PropItemCrystal");
            dictParam.Add((uint)EquipmentSlot.EquipSlotHead, "View_Equip/PropItemHead");
            dictParam.Add((uint)EquipmentSlot.EquipSlotClothes, "View_Equip/PropItemCloth");
            dictParam.Add((uint)EquipmentSlot.EquipSlotFoot, "View_Equip/PropItemFoot");
            dictParam.Add((uint)EquipmentSlot.EquipSlotAmulet, "View_Equip/PropItemJewels01");
            //dictParam.Add((uint)EquipmentSlot.EquipSlotOrnament2, "View_Equip/PropItemJewels02");

            //bing EquipBase
            bagEquipDict.Clear();
            foreach (var data in dictParam)
            {
                GameObject go = transform.Find(data.Value).gameObject;
                BagEquipmentBase equip = new BagEquipmentBase();
                equip.BindGameObject(go);

                equip.slotId = data.Key;

                bagEquipDict.Add(data.Key, equip);
            }

            battleScore = transform.Find("TextBattle").GetComponent<Text>();

            rawImage = transform.Find("Charapter").GetComponent<RawImage>();

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(rawImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);

            btnUpgrade = transform.Find("Button_Equip").GetComponent<Button>();
            btnUpgrade.onClick.AddListener(OnClickSlotUpgrade);

            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateAttr, OnUpdateAttr, true);
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            UpdateInfo();
        }

        public override void Hide()
        {
            UnloadSceneModel();
            gameObject.SetActive(false);
        }

        public override void OnDestroy()
        {
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateAttr, OnUpdateAttr, false);

            UnloadSceneModel();

            base.OnDestroy();
        }

        private void UnloadSceneModel()
        {
            _uiModelShowManagerEntity?.Dispose();
            _uiModelShowManagerEntity = null;

            rawImage.texture = null;
            heroLoader?.Dispose();
            heroLoader = null;
            showSceneControl?.Dispose();
            showSceneControl = null;
        }

        private void ProcessSceenShow(Object _sceneObj)
        {
            Load3DSceneShow(_sceneObj);

            LoadModelShow();
        }

        private void Load3DSceneShow(Object _sceneObj)
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(_sceneObj as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);

            //设置RenderTexture纹理到RawImage
            rawImage.texture = showSceneControl.GetTemporary(0, 0, 16, RenderTextureFormat.ARGB32, 1f);
        }

        private void LoadModelShow()
        {
            if (heroLoader == null)
            {
                heroLoader = HeroLoader.Create(true);
            }

            heroLoader.LoadHero(Sys_Role.Instance.Role.HeroId, GameCenter.mainHero.weaponComponent.CurWeaponID, ELayerMask.ModelShow, Sys_Fashion.Instance.GetDressData(), (go)=>
            {
                heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            });            

            heroLoader.heroDisplay.onLoaded -= OnShowModelLoaded;
            heroLoader.heroDisplay.onLoaded += OnShowModelLoaded;
        }

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                _uiModelShowManagerEntity?.Dispose();
                _uiModelShowManagerEntity = null;

                CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career);
                uint highId = Hero.GetMainHeroHighModelAnimationID();
                heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(highId, Sys_Equip.Instance.GetCurWeapon());
                //heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).gameObject.SetActive(false);

                GameObject mainGo = heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).gameObject;
                mainGo.SetActive(false);
                _uiModelShowManagerEntity = WS_UIModelShowManagerEntity.Start(500000, null, heroLoader.heroDisplay.mAnimation, mainGo);
            }
        }

        private void OnDrag(BaseEventData eventData)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        private void AddEulerAngles(Vector3 angle)
        {
            if (showSceneControl.mModelPos.transform != null)
            {
                Vector3 localAngle = showSceneControl.mModelPos.transform.localEulerAngles;
                //localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z); --old
                Vector3 newAngle = new Vector3(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);

                showSceneControl.mModelPos.transform.localEulerAngles = newAngle;
            }
        }

        private void OnUpdateAttr()
        {
            //role score
            battleScore.text = Sys_Attr.Instance.AttrNtf.RolePower.ToString();
        }

        public void UpdateInfo()
        {
            foreach (var data in bagEquipDict)
            {
                ItemData equip = Sys_Equip.Instance.SameEquipment(data.Key);
                data.Value.SetData(equip);
            }

            if (showSceneControl == null)
            {
                ProcessSceenShow(sceneModelObj);
            }
            
            //updateModel
            heroLoader.LoadWeaponPart(Sys_Fashion.Instance.GetCurDressedFashionWeapon(), Sys_Equip.Instance.GetCurWeapon());

            //role score
            battleScore.text = Sys_Attr.Instance.AttrNtf.RolePower.ToString();

            Timer.Register(0.1f, CheckSlotUpgrade);
        }

        private void CheckSlotUpgrade()
        {
            bool isOpen = Sys_FunctionOpen.Instance.IsOpen(10308) && Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(221);
            btnUpgrade.gameObject.SetActive(isOpen);
        }

        private void OnClickSlotUpgrade()
        {
           UIManager.OpenUI(EUIID.UI_EquipSlot_Upgrade);
        }
    }
}

