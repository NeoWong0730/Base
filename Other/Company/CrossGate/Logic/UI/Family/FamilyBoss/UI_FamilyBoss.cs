
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Logic.Core;
using Framework;
using Lib.Core;

namespace Logic
{
    public class UI_FamilyBoss : UIBase, UI_FamilyBoss_Right.IListener
    {
        /// <summary> 货币标题通用界面 </summary>
        private UI_CurrencyTitle ui_CurrencyTitle;
        private UI_FamilyBoss_Right m_Right;
        private UIButtonCD m_cdSeize;
        private UIButtonCD m_cdChallenge;

        private AssetDependencies dependence;
        private RawImage rawImage;
        //private ParticleSystem particleShow;
        private Image m_ImgPartner;
        private Image m_ImgPet;

        private Transform transRaceTip;

        private ShowSceneControl showSceneControl;
        private DisplayControl<EHeroModelParts> heroDisplay;
        private Monster showActor;

        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;

        private Timer m_RankInfoTimer;
        private Timer m_AttackInfoTimer;

        private uint raceId;
        
        private Vector3 vPos;

        protected override void OnLoaded()
        {
            dependence = transform.GetComponent<AssetDependencies>();

            ui_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            transform.Find("Animator/View_Title08/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);

            transform.Find("Animator/View_Title08/BtnRule").GetComponent<Button>().onClick.AddListener(OnClickRule);

            m_ImgPartner = transform.Find("Animator/View_Left/Extra/Partner/Image_Icon").GetComponent<Image>();
            m_ImgPartner.gameObject.SetActive(false);
            m_ImgPet = transform.Find("Animator/View_Left/Extra/Image_Frame/Image_Profession").GetComponent<Image>();
            m_ImgPet.gameObject.SetActive(false);

            Button btnPartner = transform.Find("Animator/View_Left/Extra/Partner/Image_Icon").GetComponent<Button>();
            btnPartner.onClick.AddListener(OnClickPartner);

            Button btnRace = transform.Find("Animator/View_Left/Extra/Image_Frame/Image_Profession").GetComponent<Button>();
            btnRace.onClick.AddListener(OnClickRace);

            m_Right = new UI_FamilyBoss_Right();
            m_Right.Init(transform.Find("Animator/View_Right"));
            m_Right.Register(this);

            m_cdSeize = new UIButtonCD();
            m_cdSeize.Init(transform.Find("Animator/Button_Seize"));
            m_cdSeize.OnClick = OnClickSeize;

            m_cdChallenge = new UIButtonCD();
            m_cdChallenge.Init(transform.Find("Animator/Button_Go"));
            m_cdChallenge.OnClick = OnClickChallege;

            transRaceTip = transform.Find("Animator/Tips");
            Button btnTipClose = transRaceTip.Find("Image_Close").GetComponent<Button>();
            btnTipClose.onClick.AddListener(() => { transRaceTip.gameObject.SetActive(false); });
            //Image eventImg = transform.Find("Animator/EventImage").GetComponent<Image>();
            //eventImg.gameObject.SetActive(true);
            //Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImg);
            //eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
            //eventListener.AddEventListener(EventTriggerType.PointerClick, OnModelClick);
        }
        protected override void OnDestroy()
        {
            ui_CurrencyTitle.Dispose();
           
        }
        protected override void OnOpen(object arg)
        {
            //eApplyFamilyMenu = null == arg ? EApplyFamilyMenu.Join : (EApplyFamilyMenu)System.Convert.ToInt32(arg);
        }

        protected override void OnShow()
        {
            //ui_CurrencyTitle.InitUi();

            m_Right.OnHide();
            Sys_FamilyBoss.Instance.OnGuildBossSimpleInfoReq();
            Sys_FamilyBoss.Instance.OnGuildBossInfoReq();
            UIManager.CloseUI(EUIID.UI_Chat);
            UIManager.OpenUI(EUIID.UI_ChatSimplify);
 
            transRaceTip.gameObject.SetActive(false);

            LoadSceneShow();
        }

        protected override void OnHide()
        {
            UIManager.CloseUI(EUIID.UI_ChatSimplify);

            m_AttackInfoTimer?.Cancel();
            m_AttackInfoTimer = null;
            m_RankInfoTimer?.Cancel();
            m_RankInfoTimer = null;
            m_cdChallenge?.Dispose();
            m_cdSeize?.Dispose();
            m_Right?.OnDispose();

            if (_uiModelShowManagerEntity != null)
            {
                _uiModelShowManagerEntity.Dispose();
                _uiModelShowManagerEntity = null;
            }

            if (showActor != null)
            {
                //if (GameCenter.modelShowWorld != null)
                //    GameCenter.modelShowWorld.DestroyActor(showActor);
                //showActor = null;
                World.CollecActor(ref showActor);
            }
            UnloadSceneModel();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_FamilyBoss.Instance.eventEmitter.Handle<uint, uint>(Sys_FamilyBoss.EEvents.OnBossSimpleInfo, this.OnSimpleInfo, toRegister);
            Sys_FamilyBoss.Instance.eventEmitter.Handle(Sys_FamilyBoss.EEvents.OnBossInfo, this.OnBossInfo, toRegister);
            Sys_FamilyBoss.Instance.eventEmitter.Handle(Sys_FamilyBoss.EEvents.OnBossRankInfo, this.OnBossRankInfo, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginEnter, OnUIEnter, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndExit, OnUIExit, toRegister);
        }

        private void OnUIEnter(uint stackId, int uId)
        {
            if (uId == (uint)EUIID.UI_Pet_Details)
            {
                if (showSceneControl != null)
                {
                    if (showSceneControl.mModelPos != null)
                        showSceneControl.mRoot.transform.localPosition = vPos + new Vector3(8, 0f, 0f);
                }
            }
        }

        private void OnUIExit(uint stackId, int uId)
        {
            if (uId == (uint)EUIID.UI_Pet_Details)
            {
                if (showSceneControl != null)
                {
                    if (showSceneControl.mModelPos != null)
                        showSceneControl.mRoot.transform.localPosition = vPos ;
                }
            }
        }

        public void OnClick_Close()
        {
            CloseSelf();
        }

        private void OnClickRule()
        {
            Sys_CommonCourse.Instance.OpenCommonCourse(2u, 201u);
        }

        private void OnClickPartner()
        {
            UIManager.OpenUI(EUIID.UI_Partner);
        }

        private void OnClickRace()
        {
            transRaceTip.gameObject.SetActive(true);
            CSVGenus.Data cSVGenusData = CSVGenus.Instance.GetConfData(this.raceId);
            if (cSVGenusData != null)
            {
                Text txt = transRaceTip.Find("Image_BG/Text").GetComponent<Text>();
                txt.text = LanguageHelper.GetTextContent(cSVGenusData.rale_name);
            }
        }

        private void OnClickSeize(bool isInCD)
        {
            if(isInCD)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3910010169));
            else
                UIManager.OpenUI(EUIID.UI_FamilyBoss_Seize);
        }

        private void OnClickChallege(bool isInCD)
        {
            if (isInCD)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3910010168));
            else
                Sys_FamilyBoss.Instance.OnGuildBossAttackReq();
        }

        private void OnSimpleInfo(uint partnerId, uint petRaceId)
        {
            this.raceId = petRaceId;

            if (Sys_FamilyBoss.Instance.State == 0)
            {
                UIManager.OpenUI(EUIID.UI_FamilyBoss_Sure);
            }
            CSVPartner.Data csvPartnerData = CSVPartner.Instance.GetConfData(partnerId);
            if (csvPartnerData != null)
            {
                ImageHelper.SetIcon(m_ImgPartner, csvPartnerData.battle_headID);
                m_ImgPartner.gameObject.SetActive(true);
            }

            CSVGenus.Data cSVGenusData = CSVGenus.Instance.GetConfData(petRaceId);
            if (cSVGenusData != null)
            {
                ImageHelper.SetIcon(m_ImgPet, cSVGenusData.rale_icon);
                m_ImgPet.gameObject.SetActive(true);
            }
        }

        private void OnBossInfo()
        {
            m_Right.OnShow();
            m_Right.UpdateBossInfo();
            m_Right.UpdateAllRankInfo();

            m_cdChallenge.Start(Sys_FamilyBoss.Instance.AttackBossCD);
            m_cdSeize.Start(Sys_FamilyBoss.Instance.AttackRoleCD);

            OnAttackInfo();

            Load3DModel();
        }

        private void OnAttackInfo()
        {
            //m_AttackInfoTimer?.Cancel();
            //m_AttackInfoTimer = Timer.Register(3f, () =>
            //{
            //    Sys_FamilyBoss.Instance.OnGuildBossWorldAttackInfoReq();
            //}, null, true); //定时请求更新数据
            //Sys_FamilyBoss.Instance.OnGuildBossWorldAttackInfoReq();
        }

        private void OnBossRankInfo()
        {
            m_Right.UpdateRankInfo();
        }

        public void OnSelectRankType(int index)
        {
            m_RankInfoTimer?.Cancel();
            m_RankInfoTimer = null;
            m_RankInfoTimer = Timer.Register(10f, () =>
            {
                Sys_FamilyBoss.Instance.OnGuildBossRankInfoReq((uint)(index + 1));
            }, null, true); //定时请求更新数据
            if (index == 1) //家族需要单独请求
                Sys_FamilyBoss.Instance.OnGuildBossRankInfoReq((uint)(index + 1));
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
            vPos = showSceneControl.mRoot.transform.localPosition;
        }

        private void Load3DModel()
        {
            if (_uiModelShowManagerEntity != null)
            {
                _uiModelShowManagerEntity.Dispose();
                _uiModelShowManagerEntity = null;
            }

            if (showActor != null)
            {
                //GameCenter.modelShowWorld.DestroyActor(showActor);
                World.CollecActor(ref showActor);
            }

            //CSVEquipment.Data equipmentData = CSVEquipment.Instance.GetConfData(partnerData.weaponID);

            CSVMonster.Data monsterData = CSVMonster.Instance.GetConfData(1700000);
            //showActor = GameCenter.modelShowWorld.CreateActor<Monster>(99);
            showActor = World.AllocActor<Monster>(99);
            showActor.SetName("MonsterShow_99");
            showActor.SetParent(GameCenter.modelShowRoot);

            showActor.cSVMonsterData = monsterData;

            showSceneControl.mModelPos.transform.localPosition = new Vector3(1.33f, 0f, -5.98f);
            showSceneControl.mModelPos.transform.localScale = Vector3.one * 0.6f;
            showSceneControl.mModelPos.transform.localRotation = Quaternion.Euler(0f, -30, 0f);
            showActor.LoadModel(monsterData.model, (actor) =>
            {
                actor.transform.SetParent(showSceneControl.mModelPos.transform, false);
                actor.transform.localPosition = Vector3.zero;
                actor.transform.localScale = Vector3.one;
                actor.transform.localRotation = Quaternion.identity;
                LayerMaskUtil.Setlayer(actor.transform, ELayerMask.ModelShow);
                //showHeroActor.WeaponID = npcData.cSVPartnerData.show_weapon_id;
                //showActor.AnimationComponent = World.AddComponent<AnimationComponent>(actor);
                actor.gameObject.SetActive(true);
                //AnimationComponent animationComponent = World.AddComponent<AnimationComponent>(actor);
                showActor.animationComponent.SetSimpleAnimation(showActor.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                showActor.animationComponent.UpdateHoldingAnimations(monsterData.monster_id, monsterData.weapon_id);
                //showActor.AnimationComponent.UpdateHoldingAnimations(cSVNpcData.id + 100, showHeroActor.WeaponID, Constants.UIModelShowAnimationClip, EStateType.Idle, actor.gameObject);

                //_uiModelShowManagerEntity = WS_UIModelShowManagerEntity.Start(partnerData.model_show_workid, showHeroActor.AnimationComponent, null, actor.gameObject);
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