using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using Framework;

namespace Logic
{
    public class UI_Partner_Get : UIBase
    {
        private Text textName;
        private Text textlightName;
        private Image imgProfession;
        //private Text textProfession;

        private Text textTip02;
        private Text textTip04;

        private Text textTip01;
        private Text textTip03;

        private Text textDes;

        private uint infoId;

        private CSVPartner.Data partnerData;

        //model show
        private AssetDependencies dependence;
        private RawImage rawImage;

        private ShowSceneControl showSceneControl;
        //private DisplayControl<EHeroModelParts> heroDisplay;
        private ShowParnter showHeroActor;
        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;

        private bool isCanClose = false;
        private Timer timerClose;
        private float aniTime = 0f;

        private UI_Partner_Get_SkillInfo skillInfo;

        protected override void OnLoaded()
        {            
            dependence = transform.GetComponent<AssetDependencies>();

            textName = transform.Find("Animator/View_Bg02/Image_Title_01/Text_01").GetComponent<Text>();
            textlightName = transform.Find("Animator/View_Bg02/Image_Title_01/Text_01/Text_01").GetComponent<Text>();
            imgProfession = transform.Find("Animator/View_Bg02/Image_Title_01/Text_01/Image_Profession").GetComponent<Image>();
            //textProfession = transform.Find("Animator/View_Bg02/Image_Title_01/Text_01/Image_Profession/Text").GetComponent<Text>();

            textTip02 = transform.Find("Animator/Text_Tips02").GetComponent<Text>();
            textTip04 = transform.Find("Animator/Text_Tips02/Text_Tips04").GetComponent<Text>();

            textTip01 = transform.Find("Animator/Text_Tips01").GetComponent<Text>();
            textTip03 = transform.Find("Animator/Text_Tips01/Text_Tips03").GetComponent<Text>();

            textDes = transform.Find("Animator/Text_dis").GetComponent<Text>();
            rawImage = transform.Find("Animator/Texture").GetComponent<RawImage>();

            skillInfo = AddComponent<UI_Partner_Get_SkillInfo>(transform.Find("Animator/Scroll_View_Skill"));

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("Black").GetComponent<Image>());
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnClickBack);

            aniTime = float.Parse(CSVParam.Instance.GetConfData(554).str_value) / 1000f;
        }

        protected override void OnOpen(object arg)
        {            
            if (arg != null)
            {
                infoId = (uint)arg;
            }
        }

        //protected override void ProcessEventsForEnable(bool toRegister)
        //{            
        //    //Sys_Equip.Instance.eventEmitter.Handle<EquipmentOperations>(Sys_Equip.EEvents.OnOperationType, OnEquipOpType, toRegister);
        //}

        protected override void OnShow()
        {            
            isCanClose = false;
            timerClose?.Cancel();
            timerClose = Timer.Register(aniTime, OnCanClose);

            UpdateInfo(infoId);
        }

        protected override void OnHide()
        {            
            UIManager.CloseUI(EUIID.UI_Skill_Tips);
            timerClose?.Cancel();
            UnloadSceneModel();
        }

        private void OnClickBack(BaseEventData data)
        {
            if (isCanClose)
            {
                isCanClose = false;
                UIManager.CloseUI(EUIID.UI_PartnerGet);
            }
        }

        private void OnCanClose()
        {
            isCanClose = true;
        }

        private void UnloadSceneModel()
        {
            rawImage.texture = null;

            if (_uiModelShowManagerEntity != null)
            {
                _uiModelShowManagerEntity.Dispose();
                _uiModelShowManagerEntity = null;
            }

            if (showHeroActor != null)
            {
                //if (GameCenter.modelShowWorld != null)
                //    GameCenter.modelShowWorld.DestroyActor(showHeroActor);
                //showHeroActor = null;

                World.CollecActor(ref showHeroActor);
            }

            showSceneControl?.Dispose();
            showSceneControl = null;
        }

        private void UpdateInfo(uint _infoId)
        {
            partnerData = CSVPartner.Instance.GetConfData(_infoId);
            textName.text = textlightName.text = LanguageHelper.GetTextContent(partnerData.name);

            CSVCareer.Data data = CSVCareer.Instance.GetConfData(partnerData.profession);
            if (data != null)
                ImageHelper.SetIcon(imgProfession, data.profession_icon2);
            //textProfession.text = LanguageHelper.GetTextContent(partnerData.occupation);
            textDes.text = LanguageHelper.GetTextContent(partnerData.desc);

            skillInfo.UpdateInfo(_infoId);

            Load3DSceneShow();
        }

        private void Load3DSceneShow()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(dependence.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);

            //设置RenderTexture纹理到RawImage
            rawImage.texture = showSceneControl.GetTemporary(0, 0, 16, RenderTextureFormat.ARGB32, 1f);

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

            //if (heroDisplay == null)
            //{
            //    heroDisplay = DisplayControl<EHeroModelParts>.Create((int)EHeroModelParts.Count);
            //    heroDisplay.onLoaded = OnShowModelLoaded;
            //}

            //partnerData = CSVPartner.Instance.GetConfData(infoId);
            //CSVEquipment.Data equipmentData = CSVEquipment.Instance.GetConfData(partnerData.weaponID);

            //heroDisplay.eLayerMask = ELayerMask.ModelShow;
            //heroDisplay.LoadMainModel(EHeroModelParts.Main, partnerData.model_show, EHeroModelParts.None, null);
            //heroDisplay.LoadMainModel(EHeroModelParts.Weapon, equipmentData.show_model, EHeroModelParts.Main, equipmentData.equip_pos);

            //heroDisplay.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
        }

        //private void OnShowModelLoaded(int obj)
        //{
        //    if (obj == 0)
        //    {
        //        heroDisplay.mAnimation.UpdateHoldingAnimations(partnerData.id + 100, partnerData.show_weapon_id);
        //    }
        //}
    }
}


