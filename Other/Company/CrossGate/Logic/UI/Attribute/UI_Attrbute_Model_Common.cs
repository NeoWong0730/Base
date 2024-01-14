
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Table;
using Framework;
using UnityEngine.EventSystems;

namespace Logic
{
    public class UI_Attrbute_Model_Common : UIComponent
    {
        private Text playerName;
        private Text playerTitle;
        private Text playerGrade;
        private Text comprehensiveGrade;

        private RawImage rawImage;
        //private ModelShowActor player;
        private Image eventImage;

        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private HeroLoader heroLoader;
       
        protected override void Loaded()
        {
            playerName = transform.Find("Text_Name").GetComponent<Text>();
            playerTitle = transform.Find("Text_Title").GetComponent<Text>();
            playerGrade = transform.Find("Text_Role_Grade/Text_Num").GetComponent<Text>();
            comprehensiveGrade = transform.Find("Text_Comprehensive_Grade/Text_Num").GetComponent<Text>();
            rawImage = transform.Find("Charapter").GetComponent<RawImage>();
            eventImage = transform.Find("EventImage").GetComponent<Image>();
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            //Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnDestroyModel, OnDestroyModel, toRegister);
        }

        public override void Show()
        {
            base.Show();
            rawImage.gameObject.SetActive(true);
        }

        public override void Hide()
        {
            base.Hide();

            OnDestroyModel();
        }

        public  void SetData()
        {
            playerName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
            OnUpdaterGrade();
            OnCreateModel();
        }

        public void OnUpdaterGrade()
        {
            playerGrade.text = Sys_Attr.Instance.rolePower.ToString();
            comprehensiveGrade.text = Sys_Attr.Instance.power.ToString();
        }

        private void OnCreateModel()
        {
            //if (player != null)
            //{
            //    GameCenter.modelShowWorld?.DestroyActor(player);
            //}
            _LoadShowScene();
            _LoadShowModel((uint)GameCenter.mainHero.careerComponent.CurCarrerType);
            string _modelPath = CSVCharacter.Instance.GetConfData(Sys_Role.Instance.Role.HeroId).model_show;
            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(Sys_Equip.Instance.GetCurWeapon());
            if (heroLoader != null && cSVEquipmentData != null)
            {
                heroLoader.heroDisplay.LoadMainModel(EHeroModelParts.Weapon, cSVEquipmentData.show_model, EHeroModelParts.Main, cSVEquipmentData.equip_pos);
            }
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }
            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);

            rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
        }

        private void _LoadShowModel(uint careerid)
        {
            if (heroLoader == null)
            {
                heroLoader = HeroLoader.Create(true);
            }

            heroLoader.LoadHero(Sys_Role.Instance.Role.HeroId, GameCenter.mainHero.weaponComponent.CurWeaponID, ELayerMask.ModelShow, Sys_Fashion.Instance.GetDressData(), (go) =>
            {
                heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            });

            heroLoader.heroDisplay.onLoaded -= OnShowModelLoaded;
            heroLoader.heroDisplay.onLoaded += OnShowModelLoaded;
        }

        private void _UnloadShowContent()
        {
            rawImage.texture = null;
            heroLoader?.Dispose();
            heroLoader = null;
            showSceneControl?.Dispose();
        }

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData((uint)GameCenter.mainHero.careerComponent.CurCarrerType);
                uint highId = Hero.GetMainHeroHighModelAnimationID();
                heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(highId, Sys_Equip.Instance.GetCurWeapon());
            }
        }

        public void OnDestroyModel()
        {
            _UnloadShowContent();
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
                localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = localAngle;
            }
        }      
    }
}
