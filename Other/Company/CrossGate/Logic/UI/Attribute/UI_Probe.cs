using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Framework;
using UnityEngine.EventSystems;
using Table;
using DG.Tweening;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Lib.Core;
using System;

namespace Logic
{
    public class UI_ProbeView : UIComponent, UI_Probe_ExploreItem.IListener
    {
        private Text playerName;
        private Text playerGrade;
        private Text comprehensiveGrade;
        private Text skillName; //探索技能名称
        private Text skillView;  //技能描述
        private GameObject eleIcon;

        private GameObject exploreParent;
        private UI_Probe_ExploreItem exploreLook;//调查
        private UI_Probe_ExploreItem exploreOld;//考古
        private UI_Probe_ExploreItem exploreEye;//神眼
        private GameObject goPointer;
        private Vector2 posPointer;

        public AssetDependencies assetDependencies;
        private HeroLoader heroLoader;
        private ShowSceneControl showSceneControl;
        private RawImage rawImage;
        private Image eventImage;
        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;

        private List<UI_Attr> elelist = new List<UI_Attr>();
        private List<Toggle> tabList = new List<Toggle>();
        private uint skillId;
        private CSVExploringSkill.Data cSVExploringSkillData;
        private Tweener tweener;
        private AsyncOperationHandle<GameObject> mHandle;

        #region Title

        private GameObject titleGo;
        private Text mTitle_text1;

        private Text mTitle_text2;
        private Image mTitle_img2;

        private Image mTitle_img3;
        private Transform mTitle_Fx3parent;
        private GameObject mNo_Title;

        AsyncOperationHandle<GameObject> requestRef;
        private GameObject titleEffect;

        #endregion


        private Dictionary<EHeroModelParts, uint> showParts = new Dictionary<EHeroModelParts, uint>();


        protected override void Loaded()
        {
            tabList.Clear();
            playerName = transform.Find("View_Middle/Text_Name").GetComponent<Text>();
            playerGrade = transform.Find("View_Middle/Text_Role_Grade/Text_Num").GetComponent<Text>();
            comprehensiveGrade = transform.Find("View_Middle/Text_Comprehensive_Grade/Text_Num").GetComponent<Text>();
            skillName = transform.Find("View_Right/Object_Probe/Image_Title/Text_Title").GetComponent<Text>();
            skillView = transform.Find("View_Right/Object_Probe/Image_Title/Text").GetComponent<Text>();
            eleIcon = transform.Find("View_Middle/Image_Attr/Image_Bg").gameObject;
            exploreParent = transform.Find("View_Right/Toggle_Probe").gameObject;

            titleGo = transform.Find("View_Middle/Title").gameObject;
            mTitle_text1 = transform.Find("View_Middle/Title/Text").GetComponent<Text>();
            mTitle_text2 = transform.Find("View_Middle/Title/Image/Text").GetComponent<Text>();
            mTitle_img2 = transform.Find("View_Middle/Title/Image").GetComponent<Image>();
            mTitle_img3 = transform.Find("View_Middle/Title/Image1").GetComponent<Image>();
            mTitle_Fx3parent = transform.Find("View_Middle/Title/Image1/Fx");
            mNo_Title = transform.Find("View_Middle/Title/Image_None").gameObject;

            exploreLook = AddComponent<UI_Probe_ExploreItem>(transform.Find("View_Right/Toggle_Probe/Icon_Look"));
            exploreLook.RegisterListener(this);
            exploreOld = AddComponent<UI_Probe_ExploreItem>(transform.Find("View_Right/Toggle_Probe/Icon_Old"));
            exploreOld.RegisterListener(this);
            exploreEye = AddComponent<UI_Probe_ExploreItem>(transform.Find("View_Right/Toggle_Probe/Icon_Eye"));
            exploreEye.RegisterListener(this);

            goPointer = transform.Find("View_Right/Image_Select").gameObject;
            posPointer = RectTransformUtility.WorldToScreenPoint(UIManager.mUICamera, goPointer.transform.position);

            eventImage = transform.Find("View_Middle/EventImage").GetComponent<Image>();
            rawImage = transform.Find("View_Middle/Charapter").GetComponent<RawImage>();

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
        }


        public override void Show()
        {
            base.Show();

            OnCreateProbeModel();

            SetValue();

            UpdateTitle(Sys_Title.Instance.curShowTitle);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);
            Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnUpdateTitleAttrView, UpdateTitle, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnGradeUpdate, OnGradeUpdate, toRegister);
        }

        public override void Hide()
        {
            tweener?.Kill();
            _UnloadShowContent();
            AddressablesUtil.ReleaseInstance(ref requestRef, OnAssetLoaded);

            base.Hide();
        }

        public void OnSelectExploreId(uint exploreId, Vector2 pos)
        {
            UpdateData(exploreId);
            ProbeText();
            ChangeModel();

            CalPointerTarget(pos);
        }

        private void CalPointerTarget(Vector2 targetPos)
        {
            Vector2 direction = targetPos - posPointer;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion des = Quaternion.AngleAxis(angle, Vector3.forward);
            Vector3 desPos = des.eulerAngles + new Vector3(0f, 0f, -90f);

            //move pointer
            tweener?.Kill();
            tweener = DOTween.To(() => goPointer.transform.rotation, x => goPointer.transform.rotation = x, desPos, 0.5f).SetEase(Ease.Linear);
        }

        public void ProbeText()
        {
            TextHelper.SetText(skillName, cSVExploringSkillData.name);
            TextHelper.SetText(skillView, cSVExploringSkillData.desc);
        }

        private void UpdateData(uint _skillId)
        {
            skillId = _skillId;
            cSVExploringSkillData = CSVExploringSkill.Instance.GetConfData(skillId);
        }

        private void OnCreateProbeModel()
        {
            _LoadShowScene();
            _LoadShowModel();
        }

        private void ChangeModel()
        {
            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(cSVExploringSkillData.weapon_id);
            heroLoader.heroDisplay.LoadMainModel(EHeroModelParts.Weapon, cSVEquipmentData.show_model, EHeroModelParts.Main, cSVEquipmentData.equip_pos);
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

        private void _LoadShowModel()
        {
            if (heroLoader == null)
            {
                heroLoader = HeroLoader.Create(true);
                heroLoader.heroDisplay.onLoaded += OnShowModelLoaded;
            }

            heroLoader.LoadHero(Sys_Role.Instance.Role.HeroId, GameCenter.mainHero.weaponComponent.CurWeaponID, ELayerMask.ModelShow, Sys_Fashion.Instance.GetDressData(), (go) =>
            {
                heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            });
        }

        private void _UnloadShowContent()
        {
            _uiModelShowManagerEntity?.Dispose();
            _uiModelShowManagerEntity = null;
            rawImage.texture = null;
            heroLoader?.Dispose();
            heroLoader = null;
            showSceneControl?.Dispose();
        }

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                _uiModelShowManagerEntity?.Dispose();
                _uiModelShowManagerEntity = null;
                uint highId = Hero.GetMainHeroHighModelAnimationID();
                heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(highId, cSVExploringSkillData.weapon_id);
                heroLoader.heroDisplay.mAnimation.CrossFade((uint)EStateType.Idle, Constants.CORSSFADETIME);
                GameObject mainGo = heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).gameObject;
                mainGo.SetActive(false);
                _uiModelShowManagerEntity = WS_UIModelShowManagerEntity.Start(500000, null, heroLoader.heroDisplay.mAnimation, mainGo);
            }
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
                localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = localAngle;
            }
        }


        private void SetValue()
        {
            playerName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
            CSVCharacter.Data heroData = CSVCharacter.Instance.GetConfData(Sys_Role.Instance.Role.HeroId);
            OnGradeUpdate();

            exploreLook.UpdateInfo(1);
            exploreOld.UpdateInfo(2);
            exploreEye.UpdateInfo(3);


            exploreLook.OnSelected(true);

            //check function open
            bool isOpen = Sys_FunctionOpen.Instance.IsOpen(10105, false);
            exploreParent.SetActive(isOpen);
            AddEleList();
        }

        private void AddEleList()
        {
            FrameworkTool.DestroyChildren(eleIcon.transform.parent.gameObject, eleIcon.transform.name);
            eleIcon.SetActive(true);
            foreach (var item in Sys_Attr.Instance.pkAttrs)
            {
                CSVAttr.Data data = CSVAttr.Instance.GetConfData(item.Key);
                if (data.attr_type == 3 && item.Value!= 0)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(eleIcon, eleIcon.transform.parent);
                    ImageHelper.SetIcon(go.transform.Find("Image_Attr").GetComponent<Image>(), data.attr_icon);
                    go.transform.Find("Image_Attr/Text").GetComponent<Text>().text = Sys_Attr.Instance.pkAttrs[item.Key].ToString();
                }
            }
            eleIcon.SetActive(false);
        }

        #region Title
        public void UpdateTitle(uint titleId)
        {
            if (titleId == 0)
            {
                SetTitleShowType(0);
                return;
            }
            CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(titleId);
            if (cSVTitleData != null)
            {
                if (cSVTitleData.id == Sys_Title.Instance.familyTitle)
                {
                    if (cSVTitleData.titleShowIcon == 0)
                    {
                        SetTitleShowType(1);
                        TextHelper.SetText(mTitle_text1, Sys_Title.Instance.GetTitleFamiltyName());
                        TextHelper.SetTextGradient(mTitle_text1, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                    }
                    else
                    {
                        SetTitleShowType(2);
                        TextHelper.SetText(mTitle_text2, Sys_Title.Instance.GetTitleFamiltyName());
                        TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                        ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon, true);
                    }
                }
                else
                {
                    if (cSVTitleData.titleShowLan != 0)
                    {
                        if (cSVTitleData.titleShowIcon == 0)
                        {
                            SetTitleShowType(1);
                            TextHelper.SetText(mTitle_text1, cSVTitleData.titleShowLan);
                            //TextHelper.SetTextGradient(mTitle_text1, cSVTitleData.titleShow[0], cSVTitleData.titleShow[1]);
                            //TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                        }
                        else
                        {
                            SetTitleShowType(2);
                            TextHelper.SetText(mTitle_text2, cSVTitleData.titleShowLan);
                            //TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            //TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                            ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon);
                        }
                    }
                    else
                    {
                        SetTitleShowType(3);
                        ImageHelper.SetIcon(mTitle_img3, cSVTitleData.titleShowIcon);
                        uint FxId = cSVTitleData.titleShowEffect;
                        CSVSystemEffect.Data cSVSystemEffectData = CSVSystemEffect.Instance.GetConfData(FxId);
                        if (cSVSystemEffectData != null)
                        {
                            LoadTitleEffectAssetAsyn(cSVSystemEffectData.FxPath);
                        }
                    }
                }
            }
        }

        private void OnGradeUpdate()
        {
            playerGrade.text = Sys_Attr.Instance.rolePower.ToString();
            comprehensiveGrade.text = Sys_Attr.Instance.power.ToString();
        }

        private void LoadTitleEffectAssetAsyn(string path)
        {
            AddressablesUtil.InstantiateAsync(ref requestRef, path, OnAssetLoaded);
        }

        private void OnAssetLoaded(AsyncOperationHandle<GameObject> handle)
        {
            titleEffect = handle.Result;
            if (null != titleEffect)
            {
                titleEffect.transform.SetParent(mTitle_Fx3parent);
                RectTransform rectTransform = titleEffect.transform as RectTransform;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }
        }

        private void SetTitleShowType(int type)
        {
            if (type == 0)
            {
                mNo_Title.SetActive(true);
                mTitle_text1.gameObject.SetActive(false);
                mTitle_text2.gameObject.SetActive(false);
                mTitle_img2.gameObject.SetActive(false);
                mTitle_img3.gameObject.SetActive(false);
                mTitle_Fx3parent.gameObject.SetActive(false);
            }
            else if (type == 1)
            {
                mNo_Title.SetActive(false);
                mTitle_text1.gameObject.SetActive(true);
                mTitle_text2.gameObject.SetActive(false);
                mTitle_img2.gameObject.SetActive(false);
                mTitle_img3.gameObject.SetActive(false);
                mTitle_Fx3parent.gameObject.SetActive(false);
            }
            else if (type == 2)
            {
                mNo_Title.SetActive(false);
                mTitle_text1.gameObject.SetActive(false);
                mTitle_text2.gameObject.SetActive(true);
                mTitle_img2.gameObject.SetActive(true);
                mTitle_img3.gameObject.SetActive(false);
                mTitle_Fx3parent.gameObject.SetActive(false);
            }
            else
            {
                mNo_Title.SetActive(false);
                mTitle_text1.gameObject.SetActive(false);
                mTitle_text2.gameObject.SetActive(false);
                mTitle_img2.gameObject.SetActive(false);
                mTitle_img3.gameObject.SetActive(true);
                mTitle_Fx3parent.gameObject.SetActive(true);
            }
        }

        #endregion

        public override void OnDestroy()
        {
            //Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateAttr, OnUpdateAttr, false); 
            Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnUpdateTitleAttrView, UpdateTitle, false);
        }
    }

    public class UI_Probe : UIComponent
    {
        private uint id;
        private Text attrname;
        private Text number;
        private EPkAttr attr;

        public UI_Probe(uint _id, EPkAttr _attr)
            : base()
        {
            id = _id;
            attr = _attr;
        }

        protected override void Loaded()
        {
            attrname = transform.GetComponent<Text>();
            number = transform.Find("Text_Number").GetComponent<Text>();
        }

        public void RefreshItem()
        {
            attrname.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(id).name).ToString();
            number.text = Sys_Attr.Instance.pkAttrs[id].ToString();
        }
    }

}
