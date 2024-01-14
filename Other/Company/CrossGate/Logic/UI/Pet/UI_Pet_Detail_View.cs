using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Framework;
using Lib.Core;
using UnityEngine.EventSystems;
using Packet;
using static Packet.PetPkAttr.Types;
using System;

namespace Logic
{
    public class UI_Pet_Detail_View : UIComponent
    {
        public Text petName;
        public Text petLv;
        public Text petGrade;
        public Text loyatyNum;
        public Text remouldTime;
        public Text remouldSkill;
        public Text skillGrid;
        public Text resistLevel;
        public Text grarNum;
        public Text remouldGradeAdd;
        public Text magicSoulActive;
        public Text contractLv;

        public RawImage rawImage;
        public Image eventImage;
        public Button closeBtn;
        public Toggle basicSkillToggle;
        public Toggle remouldSkillToggle;
        public Toggle mountSkillToggle;
        public Toggle magicSoulToggle;

        public GameObject eleGo;
        public GameObject rareGo;
        public GameObject grarView;
        public GameObject attrPage;
        public GameObject mountTag;
        public Transform starParent;

        public Image cardQuality;
        public Image cardLevel;
        public Image remouldSkillToggleDark;
        public Button remouldDrakBtn;
        public Image mountSkillToggleDark;
        public Button mountDrakBtn;
        public Image magicSoulToggleDark;
        public Button magicSoulDrakBtn;
        public GameObject attrGridGo;
        public GameObject mountSkillCellGo;
        public Text mountSkillCellCount;
        public Text advance;
        public GameObject advanceGo;

        private ClientPet curClientPet;
        private PetUnit curPetUnit;
        private CSVPetNew.Data csvPetData;
        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;

        private InfinityGrid infinityGrid;
        private UI_Pet_AdvanceView mUI_Pet_AdvcanceView;
        private UI_Pet_MsgAttrView uI_Pet_MsgAttrView;
        public UI_PetEquipSlot petEquipSlot;
        private List<uint> skillIdList = new List<uint>();
        private int infinityCount;
        private int buildSkillCount;
        private int mountSkillCount;

        private float showSceneControlPosX;
        private float showSceneControlPosY;
        private float showSceneControlPosZ;

        protected override void Loaded()
        {
            petName = transform.Find("Animator/View_Left/Detail/Image_Namebg/Text_Name").GetComponent<Text>();
            petLv = transform.Find("Animator/View_Left/Detail/Image_Namebg/Text_Label/Text_Level").GetComponent<Text>();
            petGrade = transform.Find("Animator/View_Left/Detail/Image_Point/Text_Num").GetComponent<Text>();
            loyatyNum = transform.Find("Animator/View_Right/Part2/Loyalty/Text_Percent").GetComponent<Text>();
            remouldTime = transform.Find("Animator/View_Right/Part2/Reform_Number/Text_Amount").GetComponent<Text>();
            remouldSkill = transform.Find("Animator/View_Right/Part2/Reform_Skill/Text_Amount").GetComponent<Text>();
            skillGrid = transform.Find("Animator/View_Right/Part2/Field/Text_Amount").GetComponent<Text>();
            resistLevel = transform.Find("Animator/View_Right/Part2/Level/Text_Amount").GetComponent<Text>();
            grarNum = transform.Find("Animator/View_Right/Part1/Title/Text_Percent").GetComponent<Text>();
            remouldGradeAdd = transform.Find("Animator/View_Right/Part2/Addition/Text_Amount").GetComponent<Text>();
            mountSkillCellCount = transform.Find("Animator/View_Right/Part2/Riding_Skill/Text_Amount").GetComponent<Text>();
            advance = transform.Find("Animator/View_Right/Part2/Advance/Text_Amount").GetComponent<Text>();
            magicSoulActive = transform.Find("Animator/View_Right/Part2/Magic_Soul/Text_Amount").GetComponent<Text>();
            contractLv = transform.Find("Animator/View_Right/Part2/IntensifyLevel/Text_Amount").GetComponent<Text>();

            rawImage = transform.Find("Animator/View_Left/Detail/Charapter").GetComponent<RawImage>();
            eventImage = transform.Find("Animator/View_Left/Detail/EventImage").GetComponent<Image>();
            closeBtn = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            remouldDrakBtn = transform.Find("Animator/View_Left/Menu/ListItem (1)/Btn_Menu_Dark/Button").GetComponent<Button>();
            mountDrakBtn = transform.Find("Animator/View_Left/Menu/ListItem (2)/Btn_Menu_Dark/Button").GetComponent<Button>();
            magicSoulDrakBtn = transform.Find("Animator/View_Left/Menu/ListItem (3)/Btn_Menu_Dark/Button").GetComponent<Button>();

            basicSkillToggle = transform.Find("Animator/View_Left/Menu/ListItem").GetComponent<Toggle>();
            remouldSkillToggle = transform.Find("Animator/View_Left/Menu/ListItem (1)").GetComponent<Toggle>();
            mountSkillToggle = transform.Find("Animator/View_Left/Menu/ListItem (2)").GetComponent<Toggle>();
            magicSoulToggle = transform.Find("Animator/View_Left/Menu/ListItem (3)").GetComponent<Toggle>();

            eleGo = transform.Find("Animator/View_Left/Detail/Image_Attr/Image_Bg").gameObject;
            rareGo = transform.Find("Animator/View_Left/Detail/Image_Quality/Image_Rare").gameObject;
            grarView = transform.Find("Animator/View_Right/Part1/Gird").gameObject;
            attrPage = transform.Find("Animator/View_Right/Part3").gameObject;
            mountTag = transform.Find("Animator/View_Left/Detail/Image_Quality/Image_Mount").gameObject;
            starParent = transform.Find("Animator/View_Right/Part2/Grade/Grade").transform;
            attrGridGo = transform.Find("Animator/View_Right/Part3/Scroll View/Viewport/Content/Attr_Grid").gameObject;
            mountSkillCellGo= transform.Find("Animator/View_Right/Part2/Riding_Skill").gameObject;
            advanceGo = transform.Find("Animator/View_Right/Part2/Advance").gameObject;

            cardQuality = transform.Find("Animator/View_Left/Detail/Image_Quality/Image_Card").GetComponent<Image>();
            cardLevel = transform.Find("Animator/View_Left/Detail/Image_Quality/Image_Card/Text_CardLevel").GetComponent<Image>();
            remouldSkillToggleDark = transform.Find("Animator/View_Left/Menu/ListItem (1)/Btn_Menu_Dark").GetComponent<Image>();
            mountSkillToggleDark = transform.Find("Animator/View_Left/Menu/ListItem (2)/Btn_Menu_Dark").GetComponent<Image>();
            magicSoulToggleDark = transform.Find("Animator/View_Left/Menu/ListItem (3)/Btn_Menu_Dark").GetComponent<Image>();

            closeBtn.onClick.AddListener(OncloseBtnClicked);
            remouldDrakBtn.onClick.AddListener(OnremouldDrakBtnClicked);
            mountDrakBtn.onClick.AddListener(OnmountDrakBtnClicked);
            magicSoulDrakBtn.onClick.AddListener(OnMagicSoulDrakBtnClicked);

            basicSkillToggle.onValueChanged.AddListener(OnbasicSkillToggleValueChanged);
            remouldSkillToggle.onValueChanged.AddListener(OnremouldSkillToggleValueChanged);
            mountSkillToggle.onValueChanged.AddListener(OnMountSkillToggleValueChanged);
            magicSoulToggle.onValueChanged.AddListener(OnMagicSoulToggleValueChanged);

            assetDependencies = transform.GetComponent<AssetDependencies>();
            infinityGrid = transform.Find("Animator/View_Left/Scroll View/Viewport/Content").GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;

            mUI_Pet_AdvcanceView = new UI_Pet_AdvanceView();
            mUI_Pet_AdvcanceView.Init(grarView.transform);

            uI_Pet_MsgAttrView = new UI_Pet_MsgAttrView();
            uI_Pet_MsgAttrView.Init(attrPage.transform);

            petEquipSlot = AddComponent<UI_PetEquipSlot>(transform.Find("Animator/View_Left/Detail/Grid_PetMagicCore"));
            petEquipSlot.openUI = EUIID.UI_Pet_Details;            
        }

        public UI_Pet_Detail_View(ClientPet _pet) : base()
        {
            curClientPet = _pet;
        }


        public override void Show()
        {
            base.Show();
            
            _LoadShowScene();
            
            SetValue();
        }

        public override void Hide()
        {
            base.Hide();
            //OnDestroyModel();
            _UnloadShowContent();
        }

        public override void OnDestroy()
        {
            _UnloadShowContent();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
        }

        public void UpdateClientData(ClientPet _pet)
        {
            curClientPet = _pet;
            SetValue();
        }

        #region Function
        private void SetValue()
        {
            curPetUnit = curClientPet.petUnit;
            csvPetData = CSVPetNew.Instance.GetConfData(curPetUnit.SimpleInfo.PetId);

            OnCreateModel();
            basicSkillToggle.isOn = true;
            infinityCount = curClientPet.GetPetSkillGridsCount();

            SetBasicSkillInfo();
            uI_Pet_MsgAttrView.SetValue(curClientPet);
            petEquipSlot.SetPetEquip(curClientPet);
            petName.text = Sys_Pet.Instance.GetPetName(curClientPet);
            petLv.text = LanguageHelper.GetTextContent(2009330, curPetUnit.SimpleInfo.Level.ToString());
            petGrade.text = curPetUnit.SimpleInfo.Score.ToString();
            skillGrid.text = curClientPet.GetPetSkillGridsCount().ToString();
            loyatyNum.text = LanguageHelper.GetTextContent(10884, curPetUnit.SimpleInfo.Loyalty.ToString(), Sys_Pet.Instance.MaxLoyalty.ToString());
            remouldTime.text = LanguageHelper.GetTextContent(10884, curClientPet.GetPeBuildCount().ToString(), curClientPet.GetPetLevelCanRemakeTimes().ToString());
            remouldSkill.text = LanguageHelper.GetTextContent(10884, curClientPet.GetPeBuildtSkillNotZeroCount().ToString(), curClientPet.GetPeBuildtSkillCount().ToString());
            uint enchanceLv = 0;
            if(curPetUnit.EnhanceInfo != null)
            {
                enchanceLv = curPetUnit.EnhanceInfo.Enhancelvl;
            }
            else if(curPetUnit.EnhancePlansData != null)
            {
                enchanceLv = curPetUnit.EnhancePlansData.Enhancelvl;
            }
            resistLevel.text = enchanceLv.ToString();
            grarNum.text = LanguageHelper.GetTextContent(10884, curClientPet.GetPetCurrentGradeCount().ToString(), curClientPet.GetPetBuildMaxGradeCount().ToString());
            remouldGradeAdd.text = curClientPet.GetPeBuildGradeCount().ToString();
            ImageHelper.GetPetCardLevel(cardLevel, (int)csvPetData.card_lv);
            ImageHelper.SetIcon(cardQuality, Sys_Pet.Instance.SetPetQuality(csvPetData.card_type));
            ImageHelper.SetIcon(rareGo.GetComponent<Image>(), Sys_Pet.Instance.GetQuality_ScoreImage(curClientPet));
            buildSkillCount = curClientPet.GetPeBuildtSkillCount(); 
            ImageHelper.SetImageGray(remouldSkillToggleDark, buildSkillCount == 0, true);
            remouldSkillToggle.enabled = buildSkillCount != 0;
            remouldDrakBtn.gameObject.SetActive(buildSkillCount == 0);
            mountSkillCount = curClientPet.GetMountSkill().Count;
            ImageHelper.SetImageGray(mountSkillToggleDark, mountSkillCount == 0, true);
            mountSkillToggle.gameObject.SetActive(csvPetData.mount);
            mountDrakBtn.gameObject.SetActive(mountSkillCount==0);
            mountTag.SetActive(csvPetData.mount);
            SetEleAttr();
            SetStart();
            mUI_Pet_AdvcanceView.RefreshView(curClientPet);
            mountSkillCellGo.SetActive(csvPetData.mount);
            if (csvPetData.mount)
            {
                SetMountAttr();
            }
            SetAdvanceMessage();
            if (curClientPet.GetDemonSpiritIsActive())
            {
                TextHelper.SetText(magicSoulActive, 10884, 1.ToString(), 1.ToString());
                ImageHelper.SetImageGray(magicSoulToggleDark, false, true);
            }
            else
            {
                TextHelper.SetText(magicSoulActive, 10884, 0.ToString(), 1.ToString());
                ImageHelper.SetImageGray(magicSoulToggleDark, true, true);
            }
            magicSoulDrakBtn.gameObject.SetActive(!curClientPet.GetDemonSpiritIsActive());
            contractLv.text = curClientPet.ContractLevel.ToString();
        }

        private void SetMountAttr()
        {
            CSVPetMount.Data mountData = CSVPetMount.Instance.GetConfData(curPetUnit.SimpleInfo.PetId);
            if (null != mountData)
            {
                mountSkillCellCount.text = mountData.skill_grid.ToString();
                uint lowG = curClientPet.GetPetMaxGradeCount() - curClientPet.GetPetGradeCount();
                for (int i = 0; i < mountData.indenture_effect.Count; ++i)
                {
                    GameObject grid = GameObject.Instantiate<GameObject>(attrGridGo, attrGridGo.transform.parent);
                    grid.SetActive(true);
                    Text name = grid.transform.Find("Title_Tips01/Text_Title").GetComponent<Text>();
                    if (i == 0)
                    {
                        name.text = LanguageHelper.GetTextContent(680000350);
                    }
                    else if (i == 1)
                    {
                        name.text = LanguageHelper.GetTextContent(680000351);
                    }
                    else if (i == 2)
                    {
                        name.text = LanguageHelper.GetTextContent(680000352);
                    }
                    else if (i == 3)
                    {
                        name.text = LanguageHelper.GetTextContent(680000353);
                    }
                    GameObject itemGo = grid.transform.Find("Text_Attr01").gameObject;
                    Transform itemParentTran = grid.transform.Find("Grid_Attr").transform;
                    CSVPetMountAttr.Data data = Sys_Pet.Instance.GetAttrListByGreatAndId((int)mountData.indenture_effect[i], lowG);
                    if (data == null)
                    {
                        return;
                    }
                    for (int j = 0; j < data.base_attr.Count; ++j)
                    {
                        GameObject attr = GameObject.Instantiate<GameObject>(itemGo, itemParentTran);
                        attr.SetActive(true);
                        int id = data.base_attr[j][0];
                        if (CSVAttr.Instance.TryGetValue((uint)id, out CSVAttr.Data cSVAttrData))
                        {
                            attr.transform.GetComponent<Text>().text = LanguageHelper.GetTextContent(cSVAttrData.name);
                            float strengthenBonuP = 0f;
                            var strengtgenData = CSVPetMountStrengthen.Instance.GetMountPetIntensifyData(curClientPet.ContractLevel, curClientPet.mountData.strengthen_type);
                            if (null != strengtgenData)
                                strengthenBonuP = strengtgenData.attribute_bonus / 10000f;
                            float num = data.base_attr[j][1] * (1 + strengthenBonuP) / 100f;
                            if (cSVAttrData.show_type == 1)
                            {
                                attr.transform.Find("Text_Number").GetComponent<Text>().text = num.ToString();
                            }
                            else
                            {
                                attr.transform.Find("Text_Number").GetComponent<Text>().text = LanguageHelper.GetTextContent(2009361, num.ToString());

                            }
                        }
                    }
                }

                Lib.Core.FrameworkTool.ForceRebuildLayout(attrGridGo.transform.parent.gameObject);
            }
        }

        private void SetAdvanceMessage()
        {
            bool canAdvanced = Sys_FunctionOpen.Instance.IsOpen(10553);
            uint langId = 12476;
            if (curClientPet.petData.is_gold_adv)
            {
                uint advanceNum = curClientPet.GetAdvancedNum();
                if (advanceNum == 1)
                {
                    langId = 12477;
                }
                else if (advanceNum == 2)
                {
                    langId = 12478;
                }
            }
            else
            {
                langId = 12479;
            }
            TextHelper.SetText(advance, langId);
        }

        private void SetStart()
        {
            FrameworkTool.CreateChildList(starParent, curClientPet.GetPeBuildCount());
            for (int i = 0; i < starParent.childCount; i++)
            {
                Image image = starParent.GetChild(i).GetComponent<Image>();
                ImageHelper.SetIcon(image, Sys_Pet.Instance.GetGradeStampStarImageId((uint)i + 1, curClientPet.GetBuildPointByIndex(i)));
            }
        }

        private void SetEleAttr()
        {
            FrameworkTool.DestroyChildren(eleGo.transform.parent.gameObject, eleGo.transform.name);

            List<AttrPair> attrPairs = curClientPet.GetPetEleAttrList();
            int needCount = attrPairs.Count;
            FrameworkTool.CreateChildList(eleGo.transform.parent, needCount);
            for (int i = 0; i < needCount; i++)
            {
                GameObject go = eleGo.transform.parent.transform.GetChild(i).gameObject;
                uint id = attrPairs[i].AttrId;
                ImageHelper.SetIcon(go.transform.Find("Image_Attr").GetComponent<Image>(), CSVAttr.Instance.GetConfData(id).attr_icon);
                go.transform.Find("Image_Attr/Text").GetComponent<Text>().text = attrPairs[i].AttrValue.ToString();
            }
        }

        private void SetBasicSkillInfo()
        {
            skillIdList.Clear();
            infinityCount = 0;
            if (curClientPet != null)
            {
                skillIdList.AddRange(curClientPet.GetPetSkillList());
                infinityCount = skillIdList.Count;
                infinityGrid.CellCount = infinityCount;
                infinityGrid.ForceRefreshActiveCell();
            }
            else
            {
                infinityGrid.CellCount = infinityCount;
                infinityGrid.ForceRefreshActiveCell();
            }
        }

        private void SetRemouldSkillInfo()
        {
            skillIdList.Clear();
            infinityCount = 0;
            if (curClientPet != null)
            {
                skillIdList.AddRange(curClientPet.GetPetBuildSkillList());
                infinityCount = buildSkillCount;
                infinityGrid.CellCount = infinityCount;
                infinityGrid.ForceRefreshActiveCell();
            }
            else
            {
                infinityGrid.CellCount = infinityCount;
                infinityGrid.ForceRefreshActiveCell();
            }
        }

        private void SetMountSkillInfo()
        {
            skillIdList.Clear();
            infinityCount = 0;
            if (curClientPet != null)
            {
                skillIdList.AddRange(curClientPet.GetMountSkill());
                infinityCount = skillIdList.Count;
                infinityGrid.CellCount = infinityCount;
                infinityGrid.ForceRefreshActiveCell();
            }
            else
            {
                infinityGrid.CellCount = infinityCount;
                infinityGrid.ForceRefreshActiveCell();
            }
        }

        private void SetMagicSoulSkillInfo()
        {
            skillIdList.Clear();
            infinityCount = 0;
            if (curClientPet != null)
            {
                skillIdList.AddRange(curClientPet.GetDemonSpiritSkills());
                infinityCount = skillIdList.Count;
                infinityGrid.CellCount = infinityCount;
                infinityGrid.ForceRefreshActiveCell();
            }
            else
            {
                infinityGrid.CellCount = infinityCount;
                infinityGrid.ForceRefreshActiveCell();
            }
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            PetSkillCeil entry = cell.mUserData as PetSkillCeil;
            if (index < skillIdList.Count)
            {
                uint skillId = skillIdList[index];
                bool isRemake = curClientPet.IsBuildSkill(skillId);
                bool isMount = curClientPet.IsHasSameMountSkill(skillId);

                if (isRemake)
                {
                    entry.SetData(skillId, curClientPet.IsUniqueSkill(skillId), isRemake, index: index, hasHight: skillId == 0 ? false : curClientPet.IsHasHighBuildSkill(skillId));
                }
                else if (isMount)
                {
                    entry.SetData(skillId, curClientPet.IsUniqueSkill(skillId), false, 0, 0, false, false,true, isMount);
                }
                else
                {
                    entry.SetData(skillId, curClientPet.IsUniqueSkill(skillId), isRemake,0,0,false,false,true);
                }
            }
            else
            {
                entry.SetData(0, false, false);
            }
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            PetSkillCeil entry = new PetSkillCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            entry.AddClickListener(OnSkillSelect);
            cell.BindUserData(entry);
        }

        private void OnSkillSelect(PetSkillCeil petSkillCeil)
        {
            if(petSkillCeil.petSkillBase.skillId != 0)
            {
                if (petSkillCeil.petSkillBase.isMountSkill)
                {
                    UI_MountSkill_TipsParam param = new UI_MountSkill_TipsParam();
                    param.pet = curClientPet;
                    param.skillId = petSkillCeil.petSkillBase.skillId;
                    param.isDetailShow = true;
                    UIManager.OpenUI(EUIID.UI_MountSkill_Tips, false, param);
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(petSkillCeil.petSkillBase.skillId, 0));
                }
            }
   
        }

        #endregion

        #region  ModelShow
        private void OnCreateModel()
        {
            _LoadShowModel(curPetUnit.SimpleInfo.PetId);
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

            rawImage.gameObject.SetActive(true);
            rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);

            showSceneControlPosX = showSceneControl.mModelPos.transform.localPosition.x;
            showSceneControlPosY = showSceneControl.mModelPos.transform.localPosition.y;
            showSceneControlPosZ = showSceneControl.mModelPos.transform.localPosition.z;
        }

        private void _LoadShowModel(uint petid)
        {
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            petDisplay = null;
            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }

            string _modelPath = csvPetData.model_show;

            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControlPosX + csvPetData.translation, showSceneControlPosY + csvPetData.height, showSceneControlPosZ);
            showSceneControl.mModelPos.transform.localRotation = Quaternion.Euler(csvPetData.angle1, csvPetData.angle2, csvPetData.angle3);
            showSceneControl.mModelPos.transform.localScale = new Vector3((float)csvPetData.size, (float)csvPetData.size, (float)csvPetData.size);

        }

        public GameObject modelGo;
        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                uint weaponId = Constants.UMARMEDID;
                modelGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
                if (null != modelGo)
                {
                    SceneActor.PetWearSet(Constants.PETWEAR_EQUIP, curClientPet.GetPetSuitFashionId(), modelGo.transform);
                }
                modelGo.SetActive(false);
                petDisplay.mAnimation.UpdateHoldingAnimations(csvPetData.action_id_show, weaponId, Constants.PetShowAnimationClipHashSet, go: modelGo);
            }
        }

        private void _UnloadShowContent()
        {
            rawImage.gameObject.SetActive(false);
            rawImage.texture = null;
            //petDisplay?.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl?.Dispose();
            showSceneControl = null;
            showSceneControlPosX = 0;
            showSceneControlPosY = 0;
            showSceneControlPosZ = 0;
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

        #region ButtonAndToggle

        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Details);
            UIManager.CloseUI(EUIID.UI_Tips_Pet);
        }

        public void OnremouldSkillToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                SetRemouldSkillInfo();
            }
        }

        public void OnbasicSkillToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                SetBasicSkillInfo();
            }
        }

        public void OnMountSkillToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                SetMountSkillInfo();
            }
        }

        private void OnMagicSoulToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                SetMagicSoulSkillInfo();
            }
        }

        public void OnremouldDrakBtnClicked()
        {
            if (curPetUnit.BuildInfo.BuildCount == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11789));
            }
        }

        public void OnmountDrakBtnClicked()
        {
            if (mountSkillCount== 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000226));
            }
        }

        private void OnMagicSoulDrakBtnClicked()
        {
            if (!curClientPet.GetDemonSpiritIsActive())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002037));
            }
        }
        #endregion
    }
}
