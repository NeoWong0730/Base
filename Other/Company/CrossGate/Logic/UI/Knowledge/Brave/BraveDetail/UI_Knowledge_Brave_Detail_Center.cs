using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;
using Framework;

namespace Logic
{
    public class UI_Knowledge_Brave_Detail_Center
    {
        private ShowSceneControl showSceneControl;
        private ShowBrave showHeroActor;

        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;
        //private HeroLoader heroLoader;

        private Object sceneModelObj;
        public Object SceneObj
        {
            set
            {
                sceneModelObj = value;
                Load3DSceneShow();
            }
        }

        private Transform transform;

        private RawImage rawImage;
        private Text _textName;

        private uint _braveId;
        private CSVBrave.Data _braveData;

        public void Init(Transform trans)
        {
            transform = trans;

            _textName = transform.Find("Text_Name").GetComponent<Text>();

            rawImage = transform.Find("Charapter").GetComponent<RawImage>();
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(rawImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
        }

        public void OnShow()
        {
            transform.gameObject.SetActive(true);
        }

        public void OnHide()
        {
            UnLoadScene();

            transform.gameObject.SetActive(false);
        }

        private void UnLoadScene()
        {
            rawImage.texture = null;
            //heroLoader?.Dispose();
            //heroLoader = null;
            if (_uiModelShowManagerEntity != null)
            {
                _uiModelShowManagerEntity.Dispose();
                _uiModelShowManagerEntity = null;
            }

            if (showHeroActor != null)
            {
                //GameCenter.modelShowWorld.DestroyActor(showHeroActor);
                //showHeroActor = null;

                World.CollecActor(ref showHeroActor);
            }

            showSceneControl?.Dispose();
            showSceneControl = null;
        }

        private void Load3DSceneShow()
        {
            if(showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(sceneModelObj as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);

            //设置RenderTexture纹理到RawImage
            rawImage.texture = showSceneControl.GetTemporary(0, 0, 16, RenderTextureFormat.ARGB32, 1f);
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

            //partnerData = CSVPartner.Instance.GetConfData(_infoId);
            //CSVEquipment.Data equipmentData = CSVEquipment.Instance.GetConfData(partnerData.weaponID);

            //showHeroActor = GameCenter.modelShowWorld.CreateActor<ShowBrave>(999);

            //showHeroActor = ShowBrave.Create(999, GameCenter.modelShowWorld);
            showHeroActor = World.AllocActor<ShowBrave>(999);
            showHeroActor.SetParent(GameCenter.modelShowRoot);
            showHeroActor.SetName($"ShowBrave_999");

            showHeroActor.cSVBraveData = _braveData;

            //showHeroActor.animationComponent = World.AddComponent<AnimationComponent>(showHeroActor);

            showHeroActor.LoadModel(_braveData.show_modle, (actor) =>
            {
                actor.transform.SetParent(showSceneControl.mModelPos.transform, false);
                actor.transform.localPosition = Vector3.zero;
                actor.transform.localScale = Vector3.one;
                actor.transform.localRotation = Quaternion.identity;
                //showHeroActor.WeaponID = showHeroActor.cSVPartnerData.show_weapon_id;
                
                
                showHeroActor.animationComponent.SetSimpleAnimation(showHeroActor.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                showHeroActor.animationComponent.UpdateHoldingAnimations(showHeroActor.cSVBraveData.id, showHeroActor.WeaponID, Constants.IdleAnimationClipHashSet, EStateType.None, actor.gameObject);
                actor.gameObject.SetActive(false);

                _uiModelShowManagerEntity = WS_UIModelShowManagerEntity.Start(500000, showHeroActor.animationComponent, null, actor.gameObject);
            });
        }

        //private void OnShowModelLoaded(int obj)
        //{
        //    if (obj == 0)
        //    {
        //        CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career);
        //        uint highId = Hero.GetMainHeroHighModelAnimationID();
        //        heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(highId, Sys_Equip.Instance.GetCurWeapon());
        //    }
        //}

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

        public void UpdateInfo(uint braveId)
        {
            _braveId = braveId;
            _braveData = CSVBrave.Instance.GetConfData(_braveId);

            if (_braveData != null)
            {
                _textName.text = LanguageHelper.GetTextContent(_braveData.name_id);

                Load3DModel();
            }
        }
    }
}


