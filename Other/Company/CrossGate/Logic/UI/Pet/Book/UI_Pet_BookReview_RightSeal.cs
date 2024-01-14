using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using System.Collections.Generic;
using Lib.Core;
using Packet;
using Table;
using System;

namespace Logic
{
    public class UI_PetSealInfoView
    {
        public class GetWayButton
        {

            public enum EGetWayType
            {
                Activity,
                Location,
                Another_location,

            }

            Transform transform;
            public Button button;
            private Text wayText;
            public EGetWayType eGetWay = EGetWayType.Location;
            public uint getId;
            public uint petId;
            public void Init(Transform _transform)
            {
                transform = _transform;
                wayText = transform.Find("Text").GetComponent<Text>();
            }

            public void SetData(uint _getId, uint _petId, EGetWayType _eGetWay)
            {
                petId = _petId;
                getId = _getId;
                eGetWay = _eGetWay;
                wayText.text = LanguageHelper.GetTextContent(getId);
            }
        }

        public Transform transform;
        public GameObject getWaylGo;
        public Text mapText;
        public Text findText;
        public Text showTimesText;
        public GameObject showTimesGo;
        public Text mapInfoText;
        public Text mapPosInfoText;
        public Button openMapBtn;
        public Button sealSetingBtn;
        public Button getPetSourceBtn;

        public uint currentPetId;
        public void Init(Transform transform)
        {
            this.transform = transform;
            getWaylGo = transform.Find("Layout_1/Image").gameObject;
            
            mapText = transform.Find("Layout_2/View_Drop/Text").GetComponent<Text>();
            findText = transform.Find("Layout_2/View_Drop/Text_Detail").GetComponent<Text>();
            showTimesGo = transform.Find("Layout_2/View_Probability").gameObject;
            showTimesText = transform.Find("Layout_2/View_Probability/Image/Text").GetComponent<Text>();
            
            mapInfoText = transform.Find("Layout_2/View_Site/Text2").GetComponent<Text>();
            mapPosInfoText = transform.Find("Layout_2/View_Site/Text_Detail").GetComponent<Text>();
            
            openMapBtn = transform.Find("Layout_2/Image_Button").GetComponent<Button>();
            openMapBtn.onClick.AddListener(OpenMapBtnClicked);

            sealSetingBtn = transform.Find("Layout_2/Btn_Setting").GetComponent<Button>();
            sealSetingBtn.onClick.AddListener(SealSetingBtnClicked);

            getPetSourceBtn = transform.Find("Btn_Go").GetComponent<Button>();
            getPetSourceBtn.onClick.AddListener(GoGetSourceBtnClickEd);
        }

        private void GoGetSourceBtnClickEd()
        {
            //uint petNewParamId = 31;
            PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);
            CSVPetNew.Data cSVPetData = CSVPetNew.Instance.GetConfData(currentPetId);
            if (cSVPetData != null)
            {
                iItemData.id = cSVPetData.jump_item;
            }

            var boxEvt = new MessageBoxEvt(EUIID.UI_Pet_BookReview, iItemData);
            boxEvt.b_changeSourcePos = true;
            boxEvt.pos = getPetSourceBtn.transform.position;
            boxEvt.b_ForceShowScource = true;
            boxEvt.b_ShowItemInfo = false;
            UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
        }

        private void SealSetingBtnClicked()
        {
            Sys_Pet.Instance.PetCatchSettingsReq();
            UIManager.OpenUI(EUIID.UI_Pet_SealSetting, false, currentPetId);
        }

        private void OpenMapBtnClicked()
        {
            CSVPetNewSeal.Data cSVPetSealData = CSVPetNewSeal.Instance.GetConfData(currentPetId);
            if (null == cSVPetSealData) return;

            Vector2 center = Vector2.zero;
            if (null != cSVPetSealData.coordinate_center && cSVPetSealData.coordinate_center.Count >= 2)
            {
                center = new Vector2(cSVPetSealData.coordinate_center[0] / 100f, cSVPetSealData.coordinate_center[1] / 100f);
            }
            Vector2 range = Vector2.zero;
            if (null != cSVPetSealData.center_range && cSVPetSealData.center_range.Count >= 2)
            {
                range = new Vector2(cSVPetSealData.center_range[0] / 100f, cSVPetSealData.center_range[1] / 100f);
            }

            UIManager.OpenUI(EUIID.UI_Map, false, new Sys_Map.CatchPetParameter(cSVPetSealData.map, range, center));
        }

        private void GenGetWayGo(uint currentPetId)
        {
            CSVPetNew.Data cSVPetData = CSVPetNew.Instance.GetConfData(currentPetId);

            getWaylGo.SetActive(true);
            for (int i = 0; i < getWaylGo.transform.parent.transform.childCount; i++)
            {
                if (i >= 1) GameObject.Destroy(getWaylGo.transform.parent.transform.GetChild(i).gameObject);
            }

            if (null != cSVPetData)
            {
                if (null != cSVPetData.activity)
                {
                    for (int i = 0; i < cSVPetData.activity.Count; i++)
                    {
                        GameObject go = GameObject.Instantiate<GameObject>(getWaylGo, getWaylGo.transform.parent);
                        GetWayButton remodel = new GetWayButton();
                        remodel.Init(go.transform);
                        remodel.SetData(cSVPetData.activity[i], currentPetId, GetWayButton.EGetWayType.Activity);
                    }
                }

                if (getWaylGo.transform.parent != null)
                {
                    FrameworkTool.ForceRebuildLayout(getWaylGo.transform.parent.gameObject);
                }
            }

            getWaylGo.SetActive(false);
        }

        private void SetSealInfo(uint currentPetId)
        {
            CSVPetNewSeal.Data cSVPetSealData = CSVPetNewSeal.Instance.GetConfData(currentPetId);
            CmdPetGetHandbookRes.Types.HBSeverData nameData = Sys_Pet.Instance.GetPetBookNameData(currentPetId);
            bool showMapBtn = false;
            bool showFindText = false;
            bool hasSealInfo = null != cSVPetSealData;
            
            if (hasSealInfo)
            {
                uint sealType = cSVPetSealData.seal_type;
                if (sealType == 1)
                {
                    showFindText = true;
                }
                else if (sealType == 2)
                {
                    showMapBtn = true;
                    showFindText = true;
                }
                else if (sealType == 3)
                {
                    showFindText = cSVPetSealData.is_msg;
                }
                mapInfoText.gameObject.SetActive(sealType != 2 || Sys_Pet.Instance.CanAutoSeal(currentPetId)); //地图-坐标或则信息
                showTimesText.text = LanguageHelper.GetTextContent(cSVPetSealData.frequency); // 频率
                mapText.text = LanguageHelper.GetTextContent(10996, LanguageHelper.GetTextContent(cSVPetSealData.seal_area_spe));//地图-发现者    
                switch(sealType)
                {
                    case 1:
                        if (cSVPetSealData.AccInfo != 0)
                        {
                            mapInfoText.text = LanguageHelper.GetTextContent(10997, LanguageHelper.GetTextContent(cSVPetSealData.AccInfo));//地图-坐标或则信息
                            mapPosInfoText.text = "";
                        }
                        else
                        {
                            mapInfoText.text = LanguageHelper.GetTextContent(10997, LanguageHelper.GetTextContent(cSVPetSealData.seal_area_spe));//地图-坐标或则信息
                            mapPosInfoText.text = LanguageHelper.GetTextContent(11003, Mathf.Abs(cSVPetSealData.coordinate[0] / 100f).ToString(), Mathf.Abs(cSVPetSealData.coordinate[1] / 100f).ToString());//地图-坐标
                        }
                        break;
                    case 2:
                        if(cSVPetSealData.AccInfo != 0)
                        {
                            if (nameData != null && !string.IsNullOrEmpty(nameData.DiscovererName))
                            {
                                mapInfoText.text = LanguageHelper.GetTextContent(10997, LanguageHelper.GetTextContent(cSVPetSealData.AccInfo));//地图-坐标或则信息
                                mapPosInfoText.text = "";
                            }
                            else
                            {
                                mapInfoText.text = LanguageHelper.GetTextContent(11891, LanguageHelper.GetTextContent(cSVPetSealData.seal_area_spe) + LanguageHelper.GetTextContent(11003, Mathf.Abs(cSVPetSealData.coordinate[0] / 100f).ToString(), Mathf.Abs(cSVPetSealData.coordinate[1] / 100f).ToString()));//地图-坐标或则信息
                                mapPosInfoText.text = "";//地图-坐标
                            }
                        }
                        else
                        {
                            if (nameData != null && !string.IsNullOrEmpty(nameData.DiscovererName))
                            {
                                mapInfoText.text = LanguageHelper.GetTextContent(10997, LanguageHelper.GetTextContent(cSVPetSealData.seal_area_spe));//地图-坐标或则信息
                                mapPosInfoText.text = LanguageHelper.GetTextContent(11003, Mathf.Abs(cSVPetSealData.coordinate[0] / 100f).ToString(), Mathf.Abs(cSVPetSealData.coordinate[1] / 100f).ToString());//地图-坐标
                            }
                            else
                            {
                                mapInfoText.text = LanguageHelper.GetTextContent(11891, LanguageHelper.GetTextContent(cSVPetSealData.seal_area_spe) + LanguageHelper.GetTextContent(11003, Mathf.Abs(cSVPetSealData.coordinate[0] / 100f).ToString(), Mathf.Abs(cSVPetSealData.coordinate[1] / 100f).ToString()));//地图-坐标或则信息
                                mapPosInfoText.text = "";//地图-坐标
                            }
                        }
                        break;
                    case 3:
                        if (cSVPetSealData.AccInfo != 0)
                        {
                            if (!Sys_Pet.Instance.CanAutoSeal(currentPetId))
                            {
                                if (cSVPetSealData.is_unloce && nameData != null && !string.IsNullOrEmpty(nameData.DiscovererName))
                                {
                                    mapInfoText.text = LanguageHelper.GetTextContent(10997, LanguageHelper.GetTextContent(cSVPetSealData.AccInfo));//地图-坐标或则信息
                                    mapPosInfoText.text = "";
                                }
                                else
                                {
                                    mapInfoText.text = LanguageHelper.GetTextContent(11643);//地图-坐标或则信息
                                    mapPosInfoText.text = "";
                                }
                            }
                            else
                            {
                                mapInfoText.text = LanguageHelper.GetTextContent(10997, LanguageHelper.GetTextContent(cSVPetSealData.AccInfo));//地图-坐标或则信息
                                mapPosInfoText.text = "";
                            }
                        }
                        else
                        {
                            if (!Sys_Pet.Instance.CanAutoSeal(currentPetId))
                            {
                                if (cSVPetSealData.is_unloce && nameData != null && !string.IsNullOrEmpty(nameData.DiscovererName))
                                {
                                    mapInfoText.text = LanguageHelper.GetTextContent(10997, LanguageHelper.GetTextContent(cSVPetSealData.seal_area_spe));//地图-坐标或则信息
                                    mapPosInfoText.text = LanguageHelper.GetTextContent(11003, Mathf.Abs(cSVPetSealData.coordinate[0] / 100f).ToString(), Mathf.Abs(cSVPetSealData.coordinate[1] / 100f).ToString());//地图-坐标
                                }
                                else
                                {
                                    mapInfoText.text = LanguageHelper.GetTextContent(11643);//地图-坐标或则信息
                                    mapPosInfoText.text = "";
                                }
                            }
                            else
                            {
                                mapInfoText.text = LanguageHelper.GetTextContent(10997, LanguageHelper.GetTextContent(cSVPetSealData.seal_area_spe));//地图-坐标或则信息
                                mapPosInfoText.text = LanguageHelper.GetTextContent(11003, Mathf.Abs(cSVPetSealData.coordinate[0] / 100f).ToString(), Mathf.Abs(cSVPetSealData.coordinate[1] / 100f).ToString());//地图-坐标
                            }
                        }
                        break;
                }
                if (null != nameData)
                {
                    if (string.IsNullOrEmpty(nameData.DiscovererName))
                    {
                        findText.text = "";
                    }
                    else
                    {
                        if (null != cSVPetSealData.coordinate && cSVPetSealData.coordinate.Count >= 2)
                        {
                            findText.text = LanguageHelper.GetTextContent(10935, nameData.DiscovererName);
                        }
                    }
                }
                else
                {
                    findText.text = "";
                }
            }
            else
            {
                mapText.text = LanguageHelper.GetTextContent(11782);//无法封印
            }

            showTimesGo.SetActive(hasSealInfo);
            findText.gameObject.SetActive(showFindText);
            mapInfoText.gameObject.SetActive(hasSealInfo);
            mapPosInfoText.gameObject.SetActive(hasSealInfo);
            openMapBtn.gameObject.SetActive(showMapBtn);

            CSVPetNew.Data cSVPetData = CSVPetNew.Instance.GetConfData(currentPetId);
            if (cSVPetData != null)
            {
                getPetSourceBtn.gameObject.SetActive(cSVPetData.jump_item > 0);
                sealSetingBtn.gameObject.SetActive(cSVPetData.is_seal ==1);
            }

        }

        public void SetData(uint petId)
        {
            currentPetId = petId;
            GenGetWayGo(currentPetId);
            SetSealInfo(currentPetId);
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }
    }


    public class UI_Pet_BookReview_RightSeal : UIComponent
    {
        private uint currentPetId;
        private UI_Pet_AttributeReview uI_Pet_AttributeReview;
        public GameObject petSkillGo;
        private InfinityGridLayoutGroup infinity;
        private Dictionary<GameObject, PetSkillCeil> skillCeilGrids = new Dictionary<GameObject, PetSkillCeil>();
        private List<PetSkillCeil> skillGrids = new List<PetSkillCeil>();
        private int visualGridCount;
        private List<SkillClientEx> skillList = new List<SkillClientEx>();

        //出没信息
        //private Text mapText;
        //private Text findText;
        private GameObject tipsGo;
        private Image noneIcon;
        private Text noneName;
        private Text levelText;
        private Image cardLevelText;
        private Image cardLevelBgImage;
        private Image PetCardCircleImage;

        private Button gotoSealBtn;
        private Button activeGoBtn;
        private Button setSealBtn;
        private PropItem activePropItem;
        private List<uint> sealItems = new List<uint>();

        private GameObject gotoSealGo;
        private GameObject activeGo;
        private GameObject sealTipsGo;
        private Transform v1Parent;
        private GameObject v2Sub;
        private Text mxText;
        private Text tQText;
        private List<UI_PetReodel_AttrReview> uI_PetReodel_AttrReviews = new List<UI_PetReodel_AttrReview>();
        protected override void Loaded()
        {
            uI_Pet_AttributeReview = new UI_Pet_AttributeReview();
            uI_Pet_AttributeReview.Init(transform.Find("View_Property"));
            petSkillGo = transform.Find("Scroll_View_Skill/Grid/Image").gameObject;
            infinity = transform.Find("Scroll_View_Skill/Grid").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();

            visualGridCount = infinity.transform.childCount;
            infinity.updateChildrenCallback = UpdateChildrenCallback;
            infinity.minAmount = 9;
            for (int i = 0; i < infinity.transform.childCount; i++)
            {
                GameObject go = petSkillGo.transform.parent.GetChild(i).gameObject;
                PetSkillCeil petSkillCeil = new PetSkillCeil();
                petSkillCeil.BingGameObject(go);
                petSkillCeil.AddClickListener(OnSkillSelect);
                skillCeilGrids.Add(go, petSkillCeil);
                skillGrids.Add(petSkillCeil);
            }

            tipsGo = transform.Find("NoSeal").gameObject;
            noneIcon = transform.Find("NoSeal/Item/GameObject/HeadImage").GetComponent<Image>();
            noneName = transform.Find("NoSeal/Item/GameObject/Image_Namebg/Text_Name").GetComponent<Text>();
            levelText = transform.Find("NoSeal/Item/GameObject/Text_Battle").GetComponent<Text>();
            cardLevelText = transform.Find("NoSeal/Item/GameObject/card_lv/Text").GetComponent<Image>();
            cardLevelBgImage = transform.Find("NoSeal/Item/GameObject/Image_Levelbg").GetComponent<Image>();
            PetCardCircleImage = transform.Find("NoSeal/Item/GameObject/card_lv").GetComponent<Image>();

            gotoSealGo = transform.Find("activate").gameObject;
            gotoSealBtn = transform.Find("activate/Btn_01").GetComponent<Button>();
            v1Parent = transform.Find("activate/ItemGroup");
            gotoSealBtn.onClick.AddListener(OnGotoSealClicked);
            setSealBtn = transform.Find("activate/Image_Bottom/Image").GetComponent<Button>();
            setSealBtn.onClick.AddListener(SetSealBtnClicked);
            activeGo = transform.Find("nonactivated").gameObject;
            activeGoBtn = transform.Find("nonactivated/Btn_01").GetComponent<Button>();
            activeGoBtn.onClick.AddListener(OnActiveSealClicked);
            v2Sub = transform.Find("nonactivated/ItemGroup/PropItem").gameObject;
            sealTipsGo = transform.Find("nonactivated/Text").gameObject;
            activePropItem = new PropItem();
            activePropItem.BindGameObject(v2Sub);

            mxText = transform.Find("activate/Text04").GetComponent<Text>();
            tQText = transform.Find("activate/Text_Card").GetComponent<Text>();
            string itemids = CSVParam.Instance.GetConfData(610).str_value;
            string[] ids = itemids.Split('|');
            for (int i = 0; i < ids.Length; i++)
            {
                sealItems.Add(uint.Parse(ids[i]));
            }
            int count = sealItems.Count;
            FrameworkTool.CreateChildList(v1Parent, count);
            for (int i = 0; i < sealItems.Count; i++)
            {

                Transform tran = v1Parent.GetChild(i);
                PropItem item = new PropItem();
                item.BindGameObject(tran.gameObject);
                itemList.Add(item);
            }
        }

        public override void SetData(params object[] arg)
        {
            base.Show();
            currentPetId = (uint)arg[0];
            UpdateView();
        }

        private void OnGotoSealClicked()
        {
            CSVPetNewSeal.Data cSVPetSealData = CSVPetNewSeal.Instance.GetConfData(currentPetId);
            if (null == cSVPetSealData) return;
            CmdPetGetHandbookRes.Types.HBSeverData nameData = Sys_Pet.Instance.GetPetBookNameData(currentPetId);
            bool find = false;
            if (nameData != null && !string.IsNullOrEmpty(nameData.DiscovererName))
                find = true;
            if (!Sys_Pet.Instance.CanAutoSeal(currentPetId))
            {
                if (cSVPetSealData.seal_type != 1 && (!cSVPetSealData.is_unloce || !find))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10936));
                    return;
                }
            }
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                Sys_Hint.Instance.PushForbidOprationInFight();  //战斗内提示：当前处于战斗中，无法进行该操作
                return;
            }
            Sys_Pet.Instance.DoSealPositon_CatchPet(currentPetId);
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnPlayerCloseSeal);
            UIManager.ClearUntilMain();
        }

        private void OnActiveSealClicked()
        {
            CSVPetNew.Data cSVPetData = CSVPetNew.Instance.GetConfData(currentPetId);
            int count = (int)Sys_Bag.Instance.GetItemCount(cSVPetData.PetBooks);
            if (count >= 1)
            {
                Sys_Pet.Instance.OnPetActivateReq(currentPetId);
            }
            else
            {
                CSVItem.Data itemData = CSVItem.Instance.GetConfData(cSVPetData.PetBooks);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009468, LanguageHelper.GetTextContent(itemData.name_id)));
            }
        }

        private void SetSealBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Setting, false, Tuple.Create<ESettingPage, ESetting>(ESettingPage.Settings, ESetting.Seal));
        }

        private void UpdateView()
        {
            RefreshSealInfo();
            uI_Pet_AttributeReview.SetData(currentPetId, 0);
            RefreshSkill();
        }

        public void RefreshNetMessage()
        {
            RefreshSealInfo();
        }

        private void SetNoneCard()
        {
            CSVPetNew.Data curPet = CSVPetNew.Instance.GetConfData(currentPetId);
            if (null != curPet)
            {
                ImageHelper.SetIcon(cardLevelBgImage, Sys_Pet.Instance.SetPetBookQuality(curPet.card_type));
                ImageHelper.SetIcon(PetCardCircleImage, Sys_Pet.Instance.SetPetBookCircleQuality(curPet.card_type));
                ImageHelper.GetPetCardLevel(cardLevelText, (int)curPet.card_lv);
                ImageHelper.SetIcon(noneIcon, curPet.bust);
                noneName.text = LanguageHelper.GetTextContent(curPet.name);
                levelText.text = LanguageHelper.GetTextContent(2009405, curPet.participation_lv.ToString());
            }
        }

        private float GetSealP(uint itemId, CSVPetNewSeal.Data petSealData, uint otherValue)
        {
            float pn = 10000;
            CSVItem.Data item = CSVItem.Instance.GetConfData(itemId);
            if (null != item)
            {
                CSVActiveSkill.Data skii = CSVActiveSkill.Instance.GetConfData(item.active_skillid);
                if (null != skii)
                {
                    if (null != skii.skill_effect_id)
                    {
                        int count = skii.skill_effect_id.Count;
                        for (int i = 0; i < count; i++)
                        {
                            CSVActiveSkillEffective.Data skillEff = CSVActiveSkillEffective.Instance.GetConfData(skii.skill_effect_id[i]);
                            if (null != skillEff)
                            {
                                pn += (skillEff.effect_to_target + 0f);
                            }
                        }

                    }

                }

            }
            pn += otherValue;
            if (null != petSealData)
            {
                pn -= (petSealData.seal_difficulty + 0f);
            }
            return Math.Min((pn / 100.0f), 100f);
        }

        List<PropItem> itemList = new List<PropItem>();

        private void RefreshSealInfo()
        {
            CSVPetNewSeal.Data cSVPetSealData = CSVPetNewSeal.Instance.GetConfData(currentPetId);
            bool isActive = Sys_Pet.Instance.GetPetIsActive(currentPetId);
            if (isActive)
            {
                if (null != cSVPetSealData)
                {
                    uint captureData = Sys_Adventure.Instance.GetCaptureProbability();
                    uint tQData = Sys_OperationalActivity.Instance.GetSpecialCardCaptureProbability();
                    bool isHasCapture = captureData != 0 || tQData != 0;
                    CSVWordStyle.Data cSVWordStyleData = null;
                    if (isHasCapture)
                    {
                        cSVWordStyleData = CSVWordStyle.Instance.GetConfData(150u);
                    }
                    else
                    {
                        cSVWordStyleData = CSVWordStyle.Instance.GetConfData(93u);
                    }
                    int count = itemList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (cSVPetSealData.seal_quality.Count >= 1 && (cSVPetSealData.seal_quality[0] <= (i + 1)))
                        {
                            PropItem item = itemList[i];
                            if (i == 0)
                            {
                                item.SetData(new MessageBoxEvt(EUIID.UI_Pet_BookReview, new PropIconLoader.ShowItemData(sealItems[i], 0,
                               true, false, false, false, false, _bShowCount: false, _bShowBagCount: false)));
                            }
                            else
                            {
                                item.SetData(new MessageBoxEvt(EUIID.UI_Pet_BookReview, new PropIconLoader.ShowItemData(sealItems[i], 1,
                               true, false, false, false, false, _bShowCount: true, _bShowBagCount: true)));
                            }
                            if (null != item)
                            {
                                TextHelper.SetText(item.txtName, LanguageHelper.GetTextContent(2009295, GetSealP(sealItems[i], cSVPetSealData, captureData + tQData).ToString()), cSVWordStyleData);
                                item.txtName.gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            itemList[i].SetActive(false);
                        }
                    }
                    
                    TextHelper.SetText(mxText, LanguageHelper.GetTextContent(11888, (captureData / 100.0f).ToString()));                    
                    TextHelper.SetText(tQText, LanguageHelper.GetTextContent(11887, (tQData / 100.0f).ToString()));
                }
            }
            else
            {
                CSVPetNew.Data petData = CSVPetNew.Instance.GetConfData(currentPetId);
                if (null != petData)
                {
                    activePropItem.SetData(new MessageBoxEvt(EUIID.UI_Pet_BookReview, new PropIconLoader.ShowItemData(petData.PetBooks, 1, true, false, false, false, false, true, true, true)));
                    activePropItem.SetActive(true);
                }
            }


            activeGo.SetActive(!isActive);
            gotoSealGo.SetActive(null != cSVPetSealData && isActive);
            sealTipsGo.SetActive(cSVPetSealData != null);

            bool showNone = isActive && cSVPetSealData == null;
            tipsGo.SetActive(showNone);
            if (showNone)
            {
                SetNoneCard();
            }
        }

        public override void Hide()
        {
            base.Hide();
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;
            if (skillCeilGrids.ContainsKey(trans.gameObject))
            {
                PetSkillCeil petSkillCeil = skillCeilGrids[trans.gameObject];
                petSkillCeil.SetData(skillList[index].skillId, skillList[index].eSkillOutType == ESkillClientType.Unique, skillList[index].eSkillOutType == ESkillClientType.Build,isMountSkill: skillList[index].eSkillOutType == ESkillClientType.Mount, isDemonSpiritSkill: skillList[index].eSkillOutType == ESkillClientType.DemonSpiritSkill);
            }
        }

        private void OnSkillSelect(PetSkillCeil petSkillCeil)
        {
            if (petSkillCeil.petSkillBase.isMountSkill)
            {
                UI_MountSkill_TipsParam param = new UI_MountSkill_TipsParam();
                param.pet = null;
                param.skillId = petSkillCeil.petSkillBase.skillId;
                UIManager.OpenUI(EUIID.UI_MountSkill_Tips, false, param);
            }
            else
            {
                if (petSkillCeil.petSkillBase.isUnique)
                {
                    UIManager.OpenUI(EUIID.UI_zhuanshuSkill_Tips, false, new Tuple<uint, uint>(petSkillCeil.petSkillBase.skillId, petSkillCeil.petSkillBase.isBuild ? 11633u : 0u));
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(petSkillCeil.petSkillBase.skillId, petSkillCeil.petSkillBase.isBuild ? 11633u : 0u));
                }
            }
            
        }

        private void RefreshSkill()
        {
            skillList.Clear();
            skillList = Sys_Pet.Instance.GetPetAllSkill(currentPetId);
            visualGridCount = skillList.Count;
            infinity.SetAmount(visualGridCount);
        }
    }
}
