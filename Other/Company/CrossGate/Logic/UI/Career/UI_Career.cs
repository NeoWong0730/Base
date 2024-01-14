using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Logic.Core;
using System;
using Logic;
using Table;
using Lib.AssetLoader;
using UnityEngine.EventSystems;
using Lib.Core;
using UnityEngine.UI.Extensions;

namespace Logic
{
    public class UI_HeadIcon_Component : UIComponent
    {
        public uint id;
        private Image image;
        private Text _name;
        private Button button;
        private Action<uint> onChangeCareer;
        private CSVCareer.Data csvCareerData;

        private GameObject selected;
        private GameObject notselected;
        private GameObject fx;
        private bool bSelseted;
        public bool Selected
        {
            get
            {
                return bSelseted;
            }
            set
            {
                bSelseted = value;
                if (bSelseted)
                {
                    selected.SetActive(false);
                    notselected.SetActive(true);
                    image = transform.Find("View_Dark/Image_Profession").GetComponent<Image>();
                    _name = transform.Find("View_Dark/Text_Profession").GetComponent<Text>();

                    transform.GetComponent<Image>().enabled = true;
                }
                else
                {
                    selected.SetActive(true);
                    notselected.SetActive(false);
                    image = transform.Find("View_Light/Image_Profession").GetComponent<Image>();
                    transform.GetComponent<Image>().enabled = false;
                }
                UpdateUI();
            }
        }

        private bool isOpen;
        private bool Opened
        {
            get
            {
                return isOpen;
            }
            set
            {
                isOpen = value;
            }
        }

        public UI_HeadIcon_Component(Action<uint> action, uint _id) : base()
        {
            onChangeCareer = action;
            id = _id;
        }

        protected override void Loaded()
        {
            ParseComponent();
        }

        private void ParseComponent()
        {
            selected = transform.Find("View_Light").gameObject;
            notselected = transform.Find("View_Dark").gameObject;
            fx = transform.Find("Fx_ui_jiuzhi01").gameObject;
            button = transform.GetComponent<Button>();
            RegsiterEvt();
        }

        public void UpdateUI()
        {
            csvCareerData = CSVCareer.Instance.GetConfData(id);
            Opened = CSVCareer.Instance.GetConfData(id).open == 1;
            if (!Opened)
                return;
            if (!Selected)
            {
                ImageHelper.SetIcon(image, csvCareerData.icon);
            }
            else
            {
                _name.text = CSVLanguage.Instance.GetConfData(csvCareerData.name).words;
                ImageHelper.SetIcon(image, csvCareerData.select_icon);
            }
        }

        private void RegsiterEvt()
        {
            button.onClick.AddListener(onButtonClicked);
        }

        private void onButtonClicked()
        {
            if (!Opened)
                return;
            Selected = true;
            onChangeCareer(id);
        }

        public void Release()
        {
            Selected = false;
        }

        public void Select()
        {
            Selected = true;
        }

        public void ShowFx(bool show)
        {
            fx.SetActive(show);
        }
    }


    public class UI_SkillList_Component
    {
        public uint careerId;
        public uint roleId;
        private CSVCareer.Data m_CSVCareerData;
        private GameObject m_Go;
        private GameObject m_SkillTempGo;
        private List<SkillItem> skillItems = new List<SkillItem>();
        private CP_ToggleRegistry m_CP_ToggleRegistry;
        private Action<uint> m_OnRefreshSkillInfo;
        private int m_curSelectSkillIndex;
        private Action<int> m_SetSkillIndex;

        public class SkillItem
        {
            private GameObject _go;
            private Image _iamge;
            private Button _button;
            private uint _skillInfo;
            private uint _skillId;
            private uint _careerId;
            private uint _weaponId;
            private uint _roleId;
            private Action<uint> m_OnRefreshSkillInfo;


            public void BindGameObject(GameObject gameObject)
            {
                _go = gameObject;

                _iamge = _go.transform.Find("Image_Icon").GetComponent<Image>();
                _button = _go.transform.Find("Btn_Play").GetComponent<Button>();
                _button.onClick.AddListener(OnSLButton_Clicked);
            }

            public void AddEvent(Action<uint> onRefreshSkillInfo)
            {
                m_OnRefreshSkillInfo = onRefreshSkillInfo;
            }

            public void BindData(uint skillInfo, uint skillId, uint roleId, uint careerId, uint weaponId)
            {
                _skillInfo = skillInfo;
                _skillId = skillId;
                _careerId = careerId;
                _weaponId = weaponId;
                _roleId = roleId;
            }

            public void Dispose()
            {
                _button.onClick.RemoveListener(OnSLButton_Clicked);
                m_OnRefreshSkillInfo = null;
                _go = null;
                _button = null;
                _iamge = null;
            }

            private void OnSLButton_Clicked()
            {
                if (!CombatManager.Instance.m_IsFight)
                {
                    SkillPreView.Instance.Init(_roleId, _careerId, _weaponId, _skillInfo);
                    SkillPreView.Instance.ShowSkillPreView(_skillId);
                }
            }

            public void OnToggleSelect()
            {
                m_OnRefreshSkillInfo?.Invoke(_skillInfo);
            }

            public void UpdateItem()
            {
                CSVActiveSkillInfo.Data cSVActiveSkillBehaviorData = CSVActiveSkillInfo.Instance.GetConfData(_skillId);
                if (cSVActiveSkillBehaviorData == null)
                {
                    DebugUtil.LogErrorFormat($"没有配置对应技能");
                    return;
                }
                uint icon = cSVActiveSkillBehaviorData.icon;
                if (icon == 0)
                {
                    DebugUtil.LogErrorFormat($"没有配置对应icon");
                    return;
                }
                ImageHelper.SetIcon(_iamge, icon);
            }
        }

        public void BindGameObject(GameObject _gameObject)
        {
            m_Go = _gameObject;

            m_SkillTempGo = m_Go.transform.Find("SkillItem01").gameObject;
            m_CP_ToggleRegistry = m_Go.transform.GetComponent<CP_ToggleRegistry>();
            m_CP_ToggleRegistry.onToggleChange = OnToggleChanged;
        }

        public void AddEvent(Action<uint> onRefreshSkillInfo)
        {
            m_OnRefreshSkillInfo = onRefreshSkillInfo;
        }

        private void OnToggleChanged(int cur, int old)
        {
            if (cur == old)
            {
                return;
            }
            m_curSelectSkillIndex = cur;
            m_SetSkillIndex?.Invoke(m_curSelectSkillIndex);
            skillItems[cur].OnToggleSelect();
            UIManager.HitButton(EUIID.UI_Career, careerId.ToString() + "-skillIndex" + m_curSelectSkillIndex.ToString());
        }

        public void SetData(uint _id, uint _roleid, int _curSelectSkillIndex, Action<int> setSkillIndex)
        {
            foreach (var item in skillItems)
            {
                item.Dispose();
            }
            skillItems.Clear();
            careerId = _id;
            roleId = _roleid;
            m_CSVCareerData = CSVCareer.Instance.GetConfData(careerId);
            m_curSelectSkillIndex = _curSelectSkillIndex;
            m_SetSkillIndex = setSkillIndex;
            UpdateUI();
        }

        public void UpdateUI()
        {
            int count = m_CSVCareerData.inti_skill.Count;
            FrameworkTool.CreateChildList(m_Go.transform, count);
            for (int i = 0; i < count; i++)
            {
                uint skillInfo = CSVCareer.Instance.GetConfData(careerId).inti_skill[i];
                uint skillId = CSVActiveSkillInfo.Instance.GetConfData(skillInfo).skill_show_id;
                SkillItem skillItem = new SkillItem();
                skillItem.BindGameObject(m_Go.transform.GetChild(i).gameObject);
                skillItem.BindData(skillInfo, skillId, roleId, careerId, m_CSVCareerData.weapon);
                skillItem.AddEvent(m_OnRefreshSkillInfo);
                skillItem.UpdateItem();
                skillItems.Add(skillItem);
            }
            m_CP_ToggleRegistry.SwitchTo(m_curSelectSkillIndex);
        }
    }

    public class UI_Career : UIBase
    {
        #region Layout
        private UI_SkillList_Component UI_SkillList_Component;
        private List<UI_HeadIcon_Component> uI_HeadIcon_Components = new List<UI_HeadIcon_Component>();
        private Button closeUiBtn;
        private Button okBtn;
        private Button experBtn;
        private Text careerName;
        private Text weaponName;
        private Text careerDescribe;
        private Text armor;
        private Text skillDescribe;
        private Text skillName;
        private Image careerIcon;
        private Image careerIcon2;
        private Image eventImage;
        private GameObject headparent;
        private GameObject headtemplate;
        private GameObject skillListRoot;
        private GameObject fx_jiuzhi_1;
        private GameObject fx_jiuzhi_2;
        private GameObject rolemodel;
        private Transform modelParent;
        private GameObject arrowUp;
        private GameObject arrowDown;
        private ScrollRect scrollRect;
        private AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private HeroDisplayControl heroDisplay;
        private CP_ToggleRegistry m_CP_ToggleRegistry_Sk_Des;
        private UIPolygon m_UIPolygon;

        private Text txtCondition;
        #endregion

        #region Data
        private CSVCareer.Data csvCareerdata;
        private CSVPassiveSkill.Data cSVPassiveSkillData_1;
        private CSVPassiveSkill.Data cSVPassiveSkillData_2;
        private AnimationComponent animationComponent;
        private bool onshow = false;
        private uint careerid;
        private float bgWidth;
        private float bgHeight;        

        private int curSelectIndex_Sk_Des = 0;
        private int curSelectSkillIndex = 0;
        //private RectTransform bg;
        #endregion

        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;

        protected override void OnInit()
        {
            curSelectIndex_Sk_Des = 0;
            curSelectSkillIndex = 0;
        }

        protected override void OnLoaded()
        {
            ParseComponent();
            //List<uint> ls = new List<uint>();
            //ls.AddRange(CSVCareer.Instance.GetDictData().Keys);
            //ls.RemoveAt(0);
            BuildHeadList();
            UI_SkillList_Component = new UI_SkillList_Component();
            UI_SkillList_Component.BindGameObject(skillListRoot);
            UI_SkillList_Component.AddEvent(OnRefreshSkillDes);
            //careerid = Sys_Role.Instance.Role.Career == (uint)ECareerType.None ? ls[0] : Sys_Role.Instance.Role.Career;
            careerid = Sys_Role.Instance.Role.Career == (uint)ECareerType.None ? 101 : Sys_Role.Instance.Role.Career;
            fTime = float.Parse(CSVParam.Instance.GetConfData(570).str_value) / 1000f;
            _delayTime = float.Parse(CSVParam.Instance.GetConfData(569).str_value) / 1000f;
        }

        protected override void OnShow()
        {
            Sys_Input.Instance.bForbidControl = true;
            onshow = true;
            //加载展示场景
            _LoadShowScene();
            //刷新UI 并加载展示模型
            OnSelectedCareerChanged(careerid);
            ClickComponent.GlobalEnableFlag = false;
            ResetTime();

            if (UIManager.IsOpen(EUIID.HUD))
            {
                Sys_HUD.Instance.CloseHud();
            }
        }

        /// <summary>
        /// 展示场景加载
        /// </summary>
        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);

            //设置RenderTexture纹理到RawImage
            //rawImage.texture = showSceneControl.GetTemporary(0, 0, 16, RenderTextureFormat.ARGB32, 1f);

            if (heroDisplay == null)
            {
                heroDisplay = HeroDisplayControl.Create(true);
                heroDisplay.onLoaded = OnShowModelLoaded;
            }
        }

        /// <summary>
        /// 加载展示模型
        /// </summary>
        /// <param name="careerid"></param>
        private void _LoadShowModel(uint careerid)
        {
            string _modelPath = CSVCharacter.Instance.GetConfData(Sys_Role.Instance.Role.HeroId).model_show;
            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(careerid);

            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(cSVCareerData.weapon);
            string finalModelPath = cSVEquipmentData.show_model;

            DebugUtil.LogFormat(ELogType.eCareer, $"_modelPath:{_modelPath},finalModelPath:{finalModelPath}");

            heroDisplay.eLayerMask = ELayerMask.ModelShow;
            //heroDisplay.WeaponID = cSVCareerData.weapon;
            //heroDisplay.CharID= Sys_Role.Instance.Role.HeroId + 100;
            heroDisplay.LoadMainModel(EHeroModelParts.Main, _modelPath, EHeroModelParts.None, null);
            //heroDisplay.LoadMainModel(EHeroModelParts.Weapon, finalModelPath, EHeroModelParts.Main, cSVEquipmentData.equip_pos);
            heroDisplay.LoadWeaponModel(cSVEquipmentData, true);
            
            heroDisplay.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
        }


        /// <summary>
        /// 卸载展示内容
        /// </summary>
        private void _UnloadShowContent()
        {
            if (_uiModelShowManagerEntity != null)
            {
                _uiModelShowManagerEntity.Dispose();
                _uiModelShowManagerEntity = null;
            }

            //设置RenderTexture纹理到RawImage
            //rawImage.texture = null;
            //heroDisplay.Dispose();
            HeroDisplayControl.Destory(ref heroDisplay);
            showSceneControl.Dispose();
        }

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0 || obj == 1)
            {
                DebugUtil.LogFormat(ELogType.eCareer, $"OnShowModelLoaded obj={obj}");
                VirtualGameObject mainVGO = heroDisplay.GetPart(0);
                if (mainVGO != null)
                {
                    GameObject mainGo = mainVGO.gameObject;
                    if (mainGo != null)
                    {
                        mainGo.SetActive(false);

                        VirtualGameObject weaponVGO = heroDisplay.GetPart(1);
                        if (weaponVGO != null)
                        {
                            GameObject weaponGo = weaponVGO.gameObject;
                            if (weaponGo != null)
                            {
                                CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(careerid);
                                if (csvCareerdata != null)
                                {
                                    var csvCharacterTb = CSVCharacter.Instance.GetConfData(Sys_Role.Instance.Role.HeroId);
                                    if (csvCharacterTb != null)
                                    {
                                        uint charId = Hero.GetHighModelID(Sys_Role.Instance.HeroId);
                                        heroDisplay.mAnimation.UpdateHoldingAnimations(charId, cSVCareerData.weapon, Constants.UIModelShowAnimationClipHashSet, EStateType.Idle, mainGo);

                                        mainGo.SetActive(false);
                                        if (_uiModelShowManagerEntity != null)
                                        {
                                            _uiModelShowManagerEntity.Dispose();
                                            _uiModelShowManagerEntity = null;
                                        }
                                        SetPosEulerAngles(0f);
                                        
                                        VirtualGameObject weapon02VGO = heroDisplay.GetOtherWeapon();

                                        _uiModelShowManagerEntity = WS_UIModelShowManagerEntity.Start(csvCareerdata.id * 100u + csvCharacterTb.id - (csvCharacterTb.id / 100u) * 100u, null, heroDisplay.mAnimation, mainGo, cSVCareerData.weapon, weaponGo, weapon02VGO);
                                    }

                                }
                                else
                                {
                                    DebugUtil.LogErrorFormat("找不到职业配置{0}", careerid);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ParseComponent()
        {
            closeUiBtn = transform.Find("Animator/View_Title02/Btn_Close").GetComponent<Button>();
            experBtn = transform.Find("Animator/View_Right/Btn_02").GetComponent<Button>();
            okBtn = transform.Find("Animator/View_Right/Btn_01").GetComponent<Button>();
            careerName = transform.Find("Animator/View_Right/Text_Profession").GetComponent<Text>();
            weaponName = transform.Find("Animator/View_Right/Text_Weapon").GetComponent<Text>();
            armor = transform.Find("Animator/View_Right/Text_Discrib").GetComponent<Text>();
            careerDescribe = transform.Find("Animator/View_Middle/View_Discrib/Text_Discrib").GetComponent<Text>();
            skillDescribe = transform.Find("Animator/View_Middle/View_Skill/Text_Discrib").GetComponent<Text>();
            skillName = transform.Find("Animator/View_Middle/View_Skill/Text_Name").GetComponent<Text>();
            headparent = transform.Find("Animator/View_Left/ProfessionItem_Scroll01/Content").gameObject;
            fx_jiuzhi_1 = okBtn.transform.Find("Fx_ui_jiuzhi").gameObject;
            headtemplate = transform.Find("Animator/View_Left/ProfessionItem_Scroll01/Content/ProfessionItem").gameObject;
            skillListRoot = transform.Find("Animator/View_Middle/View_Skill/Skill_Grid").gameObject;
            arrowUp = transform.Find("Animator/View_Left/Image_Arrow01").gameObject;
            arrowDown = transform.Find("Animator/View_Left/Image_Arrow02").gameObject;
            eventImage = transform.Find("Animator/EventImage").GetComponent<Image>();
            careerIcon = transform.Find("Animator/Image_Profession").GetComponent<Image>();
            careerIcon2 = transform.Find("Animator/Image_Profession (1)").GetComponent<Image>();
            assetDependencies = transform.GetComponent<AssetDependencies>();
            scrollRect = transform.Find("Animator/View_Left/ProfessionItem_Scroll01").GetComponent<ScrollRect>();
            m_CP_ToggleRegistry_Sk_Des = transform.Find("Animator/View_Middle/Menu").GetComponent<CP_ToggleRegistry>();
            m_UIPolygon = transform.Find("Animator/View_Middle/View_Discrib/Radar/UI Polygon").GetComponent<UIPolygon>();
            scrollRect.onValueChanged.AddListener(OnValueChanged);
            closeUiBtn.onClick.AddListener(OnCloseUiBtnClicked);
            okBtn.onClick.AddListener(OnOkBtnClicked);
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnModelClick);
            m_CP_ToggleRegistry_Sk_Des.onToggleChange = OnToggleChanged;
            txtCondition = transform.Find("Animator/Text_Bottom").GetComponent<Text>();
            //rawImage = transform.Find("Animator/Texture").GetComponent<RawImage>();
            //talent_1 = transform.Find("Animator/View_Right/Genius_Grid/GeniusItem01").GetComponent<Button>();
            //talent_2 = transform.Find("Animator/View_Right/Genius_Grid/GeniusItem02").GetComponent<Button>();
            //talent_select1 = talent_1.transform.Find("Image_Select").gameObject;
            //talent_select2 = talent_2.transform.Find("Image_Select").gameObject;
            //talenticon_1 = talent_1.transform.Find("Image_Icon").GetComponent<Image>();
            //talenticon_2 = talent_2.transform.Find("Image_Icon").GetComponent<Image>();
            //talentName_1 = talent_1.transform.Find("Text_Name").GetComponent<Text>();
            //talentName_2 = talent_2.transform.Find("Text_Name").GetComponent<Text>();
            //talentTips = transform.Find("Animator/View_GeniusTips").gameObject;
            //talentparent = transform.Find("Animator/View_GeniusTips/bg/Type_Grid");
            //talentTip_name = transform.Find("Animator/View_GeniusTips/bg/Text_Name").GetComponent<Text>();
            //talentTip_desc = transform.Find("Animator/View_GeniusTips/bg/Text_Tips").GetComponent<Text>();
            //ProcessTalentEvent();
        }

        private void OnToggleChanged(int cur, int old)
        {
            if (cur == old)
            {
                return;
            }
            curSelectIndex_Sk_Des = cur;
            if (cur == 0)
            {
                UpdatePolygonInfo();
            }
           
            else if (cur == 1)
            {
                //BindSkillData(careerid, Sys_Role.Instance.Role.HeroId);
            }

            UIManager.HitButton(EUIID.UI_Career, cur == 0 ? careerid.ToString() + "-des" : careerid.ToString() + "-skill");
        }

        private void UpdatePolygonInfo()
        {
            for (int i = 0; i < csvCareerdata.Capability_value.Count; ++i)
            {
                m_UIPolygon.VerticesDistances[i] = csvCareerdata.Capability_value[i] / 100f;
            }
            m_UIPolygon.enabled = false;

            if (_handler != null)
            {
                Lib.Core.CoroutineManager.Instance.Stop(_handler);
                _handler = null;
            }
            Lib.Core.CoroutineManager.Instance.StartHandler(ShowPolygon());
        }
        private void UpdateConditionText(uint taskId,uint languageId)
        {
            bool isShow = taskId > 0;
            txtCondition.gameObject.SetActive(isShow);
            if (isShow)
            {
                bool isFinish = Sys_Task.Instance.IsSubmited(taskId);
                uint worldStyleId = isFinish ? (uint)170 : 171;
                TextHelper.SetText(txtCondition, LanguageHelper.GetTextContent(languageId), CSVWordStyle.Instance.GetConfData(worldStyleId));
            }
        }
        private IEnumerator ShowPolygon()
        {
            yield return new WaitForEndOfFrame();
            m_UIPolygon.enabled = true;
        }

        private Lib.Core.CoroutineHandler _handler;

        private void OnValueChanged(Vector2 vector2)
        {
            arrowUp.SetActive(true);
            arrowDown.SetActive(true);
            if (vector2.y >= 0.99f)
            {
                arrowDown.SetActive(false);
            }
            if (vector2.y <= 0.01f)
            {
                arrowUp.SetActive(false);
            }
        }

        public void OnModelClick(BaseEventData eventData)
        {
            if (_uiModelShowManagerEntity != null)
                _uiModelShowManagerEntity.TouchModelOperation();
        }

        public void OnDrag(BaseEventData eventData)
        {
            if (_uiModelShowManagerEntity != null &&
                !_uiModelShowManagerEntity.IsCanControlUIOperation(WS_UIModelShowManagerEntity.ControlUIOperationEnum.OnRotateModel))
                return;

            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        public void AddEulerAngles(Vector3 angle)
        {
            if (showSceneControl.mModelPos.transform != null)
            {
                Vector3 localAngle = showSceneControl.mModelPos.transform.localEulerAngles;
                //localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                Vector3 newAngle = new Vector3(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = newAngle;
            }
        }

        public void SetPosEulerAngles(float eulerAnglesY)
        {
            Transform posTrans = showSceneControl.mModelPos.transform;
            if (posTrans != null)
            {
                Vector3 localAngle = posTrans.localEulerAngles;
                posTrans.localEulerAngles = new Vector3(localAngle.x, eulerAnglesY, localAngle.z);
            }
        }

        private void BuildHeadList()
        {
            var enumerator = CSVCareer.Instance.GetAll().GetEnumerator();
            int index = 0;
            while (enumerator.MoveNext())
            {
                var data = enumerator.Current;
                uint id = data.id;
                CSVCareer.Data _data = data;

                if (_data.id == 100) //职业表去掉career字段,用id=100判断.
                    continue;
                index++;
                GameObject go = index == 1 ? headtemplate : GameObject.Instantiate<GameObject>(headtemplate, headparent.transform);
                UI_HeadIcon_Component uI_HeadIcon_Component = new UI_HeadIcon_Component(OnSelectedCareerChanged, id);
                uI_HeadIcon_Component.Init(go.transform);
                uI_HeadIcon_Components.Add(uI_HeadIcon_Component);
            }
            for (int i = 0; i < uI_HeadIcon_Components.Count; i++)
            {
                if (i == 0)
                {
                    uI_HeadIcon_Components[i].Select();
                }
                else
                {
                    uI_HeadIcon_Components[i].Release();
                }
            }
        }


        private void BindSkillData(uint careerId, uint roleId)
        {
            UI_SkillList_Component.SetData(careerId, roleId, curSelectSkillIndex, OnSetSkillIndex);
            //UI_SkillList_Component.UpdateUI();
        }

        private void OnSetSkillIndex(int index)
        {
            curSelectSkillIndex = index;
        }

        private void OnRefreshSkillDes(uint skillInfoId)
        {
            TextHelper.SetText(skillName, CSVActiveSkillInfo.Instance.GetConfData(skillInfoId).name);
            TextHelper.SetText(skillDescribe, Sys_Skill.Instance.GetSkillDesc(skillInfoId));
        }


        protected override void OnClose()
        {
            Sys_Input.Instance.bForbidControl = false;
            CameraManager.mCamera.gameObject.SetActive(true);
        }

        protected override void OnHide()
        {
            if (_uiModelShowManagerEntity != null)
            {
                _uiModelShowManagerEntity.Dispose();
                _uiModelShowManagerEntity = null;
            }

            _UnloadShowContent();

            ClickComponent.GlobalEnableFlag = true;            
            ResetTime();
        }

        uint pageShowTime;
        private void OnSelectedCareerChanged(uint id)
        {
            //DebugUtil.LogFormat(ELogType.eCareer, $"id={id},careerId={careerid}");
            //DebugUtil.LogFormat(ELogType.eCareer, $"onshow={onshow}");
            //DebugUtil.LogFormat(ELogType.eCareer, "OnSelectedCareerChanged");
            if (careerid == id && !onshow) return;
            onshow = false;
            foreach (var item in uI_HeadIcon_Components)
            {
                if (item.id != id)
                {
                    item.Release();
                    if (item.id == careerid)
                    {
                        UIManager.HitPointHide(EUIID.UI_Career, pageShowTime, ((ECareerType)careerid).ToString());
                    }
                }
                else
                {
                    item.Select();
                    UIManager.HitPointShow(EUIID.UI_Career, ((ECareerType)id).ToString());
                    pageShowTime = Sys_Time.Instance.GetServerTime();
                }
            }
            //UIManager.HitButton(EUIID.UI_Career, "careerid: " + id);
            careerid = id;
            OnUpdateUI();
            _LoadShowModel(careerid);
            ResetTime();

            uint skillInfo = CSVCareer.Instance.GetConfData(careerid).inti_skill[curSelectSkillIndex];
            OnRefreshSkillDes(skillInfo);
            //talent_select1.SetActive(false);
            //talent_select2.SetActive(false);
        }

        private void OnUpdateUI()
        {
            csvCareerdata = CSVCareer.Instance.GetConfData(careerid);
            if (csvCareerdata == null)
            {
                DebugUtil.LogErrorFormat($"不存在职业id{careerid}");
                return;
            }
            BindSkillData(careerid, Sys_Role.Instance.Role.HeroId);
            m_CP_ToggleRegistry_Sk_Des.SwitchTo(curSelectIndex_Sk_Des);
            ImageHelper.SetIcon(careerIcon, csvCareerdata.icon);
            TextHelper.SetText(careerName, csvCareerdata.desc[0]);
            TextHelper.SetText(weaponName, csvCareerdata.desc[1]);
            TextHelper.SetText(careerDescribe, csvCareerdata.desc[3]);
            TextHelper.SetText(armor, csvCareerdata.desc[2]);
            ImageHelper.SetIcon(careerIcon, null, csvCareerdata.profession_icon1, true);
            ImageHelper.SetIcon(careerIcon2, null, csvCareerdata.profession_icon2, true);
            UpdatePolygonInfo();
            UpdateConditionText(csvCareerdata.job_task_ID, csvCareerdata.job_lan_ID);
            //UpdatePolygonInfo();
            //cSVPassiveSkillData_1 = CSVPassiveSkill.Instance.GetConfData(csvCareerdata.passive_skill_show[0]);
            //cSVPassiveSkillData_2 = CSVPassiveSkill.Instance.GetConfData(csvCareerdata.passive_skill_show[1]);
            //ImageHelper.SetIcon(talenticon_1, cSVPassiveSkillData_1.icon);
            //ImageHelper.SetIcon(talenticon_2, cSVPassiveSkillData_2.icon);
            //TextHelper.SetText(talentName_1, cSVPassiveSkillData_1.name);
            //TextHelper.SetText(talentName_2, cSVPassiveSkillData_2.name);
        }

        private void OnCloseUiBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Career);
            UIManager.HitButton(EUIID.UI_Career, "OnClose");
        }

        private void OnOkBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_Career, "OnCareer");
            if (careerid == Sys_Role.Instance.Role.Career)
                return;
            if (csvCareerdata.job_task_ID>0 && !Sys_Task.Instance.IsSubmited(csvCareerdata.job_task_ID))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(csvCareerdata.job_lan_ID));
                return;
            }
            PromptBoxParameter.Instance.Clear();
            string name = CSVLanguage.Instance.GetConfData(csvCareerdata.name).words;
            string content = CSVLanguage.Instance.GetConfData(1000410).words;
            PromptBoxParameter.Instance.content = string.Format(content, name);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                UIManager.CloseUI(EUIID.UI_Career);
                Sys_Inaugurate.Instance.RoleSelectCareerReq(careerid);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        private float fTime = 0;
        private float fTimer = 0;
        private bool _bShowFx = false;
        private float _delayTime = 0;
        private Timer fxTimer;
        private bool bShowFx
        {
            get { return _bShowFx; }
            set
            {
                if (_bShowFx != value)
                {
                    _bShowFx = value;
                    ShowFx(_bShowFx);
                }
            }
        }

        private void ShowFx(bool show)
        {
            foreach (var item in uI_HeadIcon_Components)
            {
                item.ShowFx(item.Selected && show);
            }
            if (show)
            {
                fxTimer = Timer.Register(_delayTime, () => fx_jiuzhi_1.SetActive(show));
            }
            else
            {
                fx_jiuzhi_1.SetActive(show);
            }
        }


        protected override void OnUpdate()
        {
            fTimer -= deltaTime;
            if (fTimer <= 0)
            {
                bShowFx = true;
            }
        }

        private void ResetTime()
        {
            fxTimer?.Cancel();
            fxTimer = null;
            fTimer = fTime;
            bShowFx = false;
        }

        #region Abandon
        //private Button talent_1;
        //private Button talent_2;
        //private GameObject talent_select1;
        //private GameObject talent_select2;
        //private Image talenticon_1;
        //private Image talenticon_2;
        //private GameObject talentTips;
        //private Text talentTip_name;
        //private Text talentTip_desc;
        //private Transform talentparent;
        //private Text talentName_1;
        //private Text talentName_2;


        //private void ProcessTalentEvent()
        //{
        //    talent_1.onClick.AddListener(() =>
        //    {
        //        talentTips.SetActive(true);
        //        RefreshTalentTips(cSVPassiveSkillData_1);
        //        talent_select1.SetActive(true);
        //        talent_select2.SetActive(false);
        //        TextHelper.SetText(talent_select1.transform.Find("Text_Name").GetComponent<Text>(), cSVPassiveSkillData_1.name);
        //        ImageHelper.SetIcon(talent_select1.transform.Find("Icon_Select").GetComponent<Image>(), cSVPassiveSkillData_1.icon);
        //    });
        //    talent_2.onClick.AddListener(() =>
        //    {
        //        talentTips.SetActive(true);
        //        RefreshTalentTips(cSVPassiveSkillData_2);
        //        talent_select1.SetActive(false);
        //        talent_select2.SetActive(true);
        //        TextHelper.SetText(talent_select2.transform.Find("Text_Name").GetComponent<Text>(), cSVPassiveSkillData_2.name);
        //        ImageHelper.SetIcon(talent_select2.transform.Find("Icon_Select").GetComponent<Image>(), cSVPassiveSkillData_2.icon);
        //    });


        //    Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(talentTips.transform.Find("Blank"));
        //    eventListener.AddEventListener(EventTriggerType.PointerClick, (_) => { talentTips.SetActive(false); });
        //}

        //private void RefreshTalentTips(CSVPassiveSkill.Data cSVPassiveSkillData)
        //{
        //    for (int i = 0; i < talentparent.childCount; i++)
        //    {
        //        talentparent.GetChild(i).gameObject.SetActive(false);
        //    }
        //    int tipsCount = cSVPassiveSkillData.tab_desc.Count;
        //    for (int i = 0; i < tipsCount; i++)
        //    {
        //        talentparent.GetChild(i).gameObject.SetActive(true);
        //        TextHelper.SetText(talentparent.GetChild(i).Find("Text").GetComponent<Text>(), cSVPassiveSkillData.tab_desc[i]);
        //    }
        //    TextHelper.SetText(talentTip_name, cSVPassiveSkillData.name);
        //    TextHelper.SetText(talentTip_desc, cSVPassiveSkillData.desc);
        //}
        #endregion
    }
}

