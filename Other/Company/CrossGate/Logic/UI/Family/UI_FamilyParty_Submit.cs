using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Table;
using Lib.Core;
using Framework;

namespace Logic
{
    public class UI_FamilyParty_Submit : UIBase
    {
        readonly uint NPCId = 1521001;
        readonly int DEFAULT_ITEM_NUM = 20;//空格子最大数量

        private Button btnClose;

        private RawImage rawImageNPC;
        private PropItem targetFoodIcon;
        private List<Transform> listFoodMat = new List<Transform>();//食材列表

        private Button btnRecord;//上缴记录按钮
        private Button btnSubmit;//上缴按钮
        private Text txtTime;
        private Text txtTimeTitle;//时间前面的标题
        private Text txtPartyValue;//酒会价值
        private Text txtPersonalExp;//个人经验
        private Text txtResidueDegree;//剩余次数
        private InfinityGrid infinity;
        private List<ItemData> listFoods = new List<ItemData>();//菜品列表
        private ulong selectFoodUId = 0;//选中的菜品id
        private Timer timer;
        private float countDownTime = 0;
        private Text txtCookToolName;//锅具名称

        private Slider sliderExp;
        private Text txtSliderNum;
        private Button curStarBtn;
        private Text txtCurStar;
        private Button maxStarBtn;
        private Text txtMaxStar;
        private Transform lineParent;
        private Button btnStartRule;
        private List<UI_FamilyParty_SubmitSliderStarCell> listStar = new List<UI_FamilyParty_SubmitSliderStarCell>();

        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> modelDisplay;
        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;

        #region 系统函数
        protected override void OnOpen(object arg)
        {
        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            Sys_Family.Instance.GetCuisineInfoReq();
            UpdateView();
            OnCreateModel();
        }
        protected override void OnDestroy()
        {
            timer?.Cancel();
            OnDestroyModel();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnPartyDataUpdate, UpdateView, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnPartySubmitSucceed, OnPartySubmitSucceed, toRegister);
        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/View_FullTitle01_New/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);

            rawImageNPC = transform.Find("Animator/NPC/RawImageLeft").GetComponent<RawImage>();
            targetFoodIcon = new PropItem();
            targetFoodIcon.BindGameObject(transform.Find("Animator/NPC/Image_Menu/Image_BG/PropItem").gameObject);
            Transform foodDescParent = transform.Find("Animator/NPC/Image_Menu/Image_BG/Layout");
            txtCookToolName = transform.Find("Animator/NPC/Image_Menu/Image_BG/Layout/Text_Cooking/Text_Name").GetComponent<Text>();
            listFoodMat.Clear();
            for (int i = 0; i < 4; i++)
            {
                listFoodMat.Add(foodDescParent.GetChild(i));
            }

            btnRecord = transform.Find("Animator/Right/Btn_Record").GetComponent<Button>();
            btnRecord.onClick.AddListener(OnBtnRecordClick);
            btnSubmit = transform.Find("Animator/Right/Btn_01").GetComponent<Button>();
            btnSubmit.onClick.AddListener(OnBtnSubmitClick);
            txtTime = transform.Find("Animator/Right/Text_Time").GetComponent<Text>();
            txtTimeTitle = transform.Find("Animator/Right/Text_Time/Text").GetComponent<Text>();
            txtPartyValue = transform.Find("Animator/Right/Text_Value").GetComponent<Text>();
            txtPersonalExp = transform.Find("Animator/Right/Text_EXP").GetComponent<Text>();
            txtResidueDegree = transform.Find("Animator/Right/Text_Num").GetComponent<Text>();
            infinity = transform.Find("Animator/Right/Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;

            sliderExp = transform.Find("Animator/Bottom/Slider_Exp").GetComponent<Slider>();
            txtSliderNum = transform.Find("Animator/Bottom/Slider_Exp/Text_num").GetComponent<Text>();
            curStarBtn = transform.Find("Animator/Bottom/Slider_Exp/Handle/Button0").GetComponent<Button>();
            txtCurStar = transform.Find("Animator/Bottom/Slider_Exp/Handle/Button0/Text").GetComponent<Text>();
            curStarBtn.onClick.AddListener(OnCurStarBtnClick);
            maxStarBtn = transform.Find("Animator/Bottom/Button1").GetComponent<Button>();
            txtMaxStar = transform.Find("Animator/Bottom/Button1/Text").GetComponent<Text>();
            maxStarBtn.onClick.AddListener(OnMaxStarBtnClick);
            lineParent = transform.Find("Animator/Bottom/Line");
            btnStartRule = transform.Find("Animator/Bottom/Button_StarReward").GetComponent<Button>();
            btnStartRule.onClick.AddListener(OnBtnStartRuleClick);

            assetDependencies = transform.GetComponent<AssetDependencies>();
        }
        private void UpdateView()
        {
            UpdateFoodView();
            UpdateRightView();
            UpdateRightTimer();
            UpdateBottomView();
        }
        private void UpdateFoodView()
        {
            var id = Sys_Family.Instance.GetFashionFoodId();
            var itemData = new PropIconLoader.ShowItemData(id, 0, true, false, false, false, false);
            targetFoodIcon.SetData(itemData, EUIID.UI_FamilyParty_Submit);
            CSVCook.Data data = CSVCook.Instance.GetConfData(Sys_Family.Instance.familyData.familyPartyInfo.fashionFoodId);
            if (data != null)
            {
                txtCookToolName.text = Sys_Cooking.Instance.GetToolName(data.tool1);
                for (int i = 0; i < listFoodMat.Count; i++)
                {
                    Transform foodDesc = listFoodMat[i];
                    if (i < 4 && i < data.food1.Count)
                    {
                        var matData = data.food1[i];
                        foodDesc.gameObject.SetActive(true);
                        Text txtName = foodDesc.GetComponent<Text>();
                        txtName.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(matData[0]).name_id);
                        Text txtValue = foodDesc.Find("Text_Num").GetComponent<Text>();
                        txtValue.text = LanguageHelper.GetTextContent(6230, matData[1].ToString());
                    }
                    else
                    {
                        foodDesc.gameObject.SetActive(false);
                    }
                }
            }
        }
        private void UpdateRightView()
        {
            listFoods = Sys_Family.Instance.GetPartyCanSubmitItems();
            if (selectFoodUId == 0 && listFoods.Count > 0)
            {
                selectFoodUId = listFoods[0].Uuid;
            }
            var selectItem = Sys_Bag.Instance.GetItemDataByUuid(selectFoodUId);
            bool isSpecial = false;
            uint curValue = 0;
            uint curExp = 0;
            if (selectItem != null)
            {
                isSpecial = Sys_Family.Instance.CheckIsFashionFood(selectItem.Id);
                curValue = selectItem.cSVItemData.receptionValue;
                var rewardPaid = selectItem.cSVItemData.rewardPaid;
                for (int i = 0; i < rewardPaid.Count; i++)
                {
                    if (rewardPaid[i][0] == 4)
                    {
                        curExp = rewardPaid[i][1];
                    }
                }
            }
            if (isSpecial)
            {
                var extParam = float.Parse(CSVParam.Instance.GetConfData(1034).str_value);
                var extValue = curValue * (extParam - 100) / 100;
                txtPartyValue.text = LanguageHelper.GetTextContent(6228, (curValue + extValue).ToString(), extValue.ToString());//+{0}（额外+{1}）
                var extExp = curExp * (extParam - 100) / 100;
                txtPersonalExp.text = LanguageHelper.GetTextContent(6228, (curExp + extExp).ToString(), extExp.ToString());
            }
            else
            {
                txtPartyValue.text = LanguageHelper.GetTextContent(6243, curValue.ToString());
                txtPersonalExp.text = LanguageHelper.GetTextContent(6243, curExp.ToString());
            }
            uint residueDegree = uint.Parse(CSVParam.Instance.GetConfData(1032).str_value) - Sys_Family.Instance.familyData.familyPartyInfo.submitTimes;
            txtResidueDegree.text = LanguageHelper.GetTextContent(6229, residueDegree.ToString());

            infinity.CellCount = listFoods.Count > DEFAULT_ITEM_NUM ? listFoods.Count : DEFAULT_ITEM_NUM;
            infinity.ForceRefreshActiveCell();

            bool isSubmitTime = Sys_Family.Instance.CheckIsPartySubmitTime();
            bool canShowbtnSubmit = residueDegree > 0 && selectItem != null && isSubmitTime;
            ImageHelper.SetImageGray(btnSubmit.GetComponent<Image>(), !canShowbtnSubmit, true);
            if (isSubmitTime)
            {
                txtTimeTitle.text = LanguageHelper.GetTextContent(6202);//剩余上缴时间
            }
            else
            {
                txtTimeTitle.text = LanguageHelper.GetTextContent(6261);//距离上缴时间
            }
        }
        private void UpdateRightTimer()
        {
            uint nowtime = Sys_Time.Instance.GetServerTime();
            var fiveTime = Sys_Time.Instance.GetDayZeroTimestamp() + 5 * 3600;//凌晨五点
            var partyStartTime = Sys_Family.Instance.GetPartyStartTimestamp();
            var partySecondStageTime = partyStartTime + uint.Parse(CSVParam.Instance.GetConfData(1036).str_value) * 60;
            if (nowtime < fiveTime)
            {
                countDownTime = fiveTime - nowtime;
            }
            else if (nowtime < partySecondStageTime)
            {
                countDownTime = partySecondStageTime - nowtime;
            }
            else
            {
                countDownTime = fiveTime + 86400 - nowtime;
            }
            timer?.Cancel();
            timer = Timer.Register(countDownTime, OnTimerComplete, OnTimerUpdate, false, false);
        }
        private void UpdateBottomView()
        {
            var curValue = Sys_Family.Instance.familyData.familyPartyInfo.partyValue;
            var maxValue = Sys_Family.Instance.GetPartyMaxValue();
            sliderExp.value = (float)curValue / (float)maxValue;
            curStarBtn.gameObject.SetActive(true);
            maxStarBtn.gameObject.SetActive(true);
            txtCurStar.text = LanguageHelper.GetTextContent(6230, Sys_Family.Instance.GetPartyStarNumByPartyValue(curValue).ToString());
            uint maxStarNum = Sys_Family.Instance.GetPartyStarNumByPartyValue(maxValue);
            txtMaxStar.text = LanguageHelper.GetTextContent(6230, maxStarNum.ToString());
            if (curValue < maxValue)
            {
                txtSliderNum.text = LanguageHelper.GetTextContent(6258, Sys_Family.Instance.GetPartyStarUpNeedValue(curValue).ToString());//距离下个星级还有{0}价值
            }
            else
            {
                txtSliderNum.text = LanguageHelper.GetTextContent(6259);//已达到当前城堡最高星级;
            }
            listStar.Clear();
            var listValue = Sys_Family.Instance.familyData.familyPartyInfo.listValueStage;
            FrameworkTool.CreateChildList(lineParent, (int)maxStarNum);
            for (int i = 0; i < lineParent.childCount; i++)
            {
                Transform child = lineParent.GetChild(i);
                UI_FamilyParty_SubmitSliderStarCell cell = new UI_FamilyParty_SubmitSliderStarCell();
                cell.Init(child, listValue[i + 1]);
                cell.UpdateCellView(curValue);
                listStar.Add(cell);
            }
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        private void OnBtnRecordClick()
        {
            UIManager.OpenUI(EUIID.UI_FamilyParty_Record);
        }
        private void OnBtnSubmitClick()
        {
            uint residueDegree = uint.Parse(CSVParam.Instance.GetConfData(1032).str_value) - Sys_Family.Instance.familyData.familyPartyInfo.submitTimes;
            var selectItem = Sys_Bag.Instance.GetItemDataByUuid(selectFoodUId);
            bool canSubmit = residueDegree > 0;
            if (canSubmit && selectItem != null)
            {
                Sys_Family.Instance.CheckToSubmitPartyFood(selectFoodUId);
            }
            else
            {
                if (canSubmit)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6264));//请选定需要上缴的菜品
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6263));//剩余上缴次数不足
                }
            }
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_FamilyParty_SubmitItemCell itemCell = new UI_FamilyParty_SubmitItemCell();
            itemCell.BindGameObject(go);
            itemCell.AddClickListener(OnItemClick);
            cell.BindUserData(itemCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_FamilyParty_SubmitItemCell mCell = cell.mUserData as UI_FamilyParty_SubmitItemCell;
            if (index < listFoods.Count)
            {
                var item = listFoods[index];
                var isSelected = item.Uuid == selectFoodUId;
                mCell.UpdateCellView(item);
                mCell.SetSelected(isSelected);
            }
            else
            {
                mCell.UpdateCellView(null);
            }
        }
        private void OnItemClick(ItemData itemData)
        {
            if (itemData != null)
            {
                selectFoodUId = itemData.Uuid;
                PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(itemData.Id, 0, false, false, false, false, false, false, true);
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_FamilyParty_Submit, showItemData));
            }
            UpdateRightView();
        }
        private void OnCurStarBtnClick()
        {
            uint curValue = Sys_Family.Instance.familyData.familyPartyInfo.partyValue;
            uint curStarNum = Sys_Family.Instance.GetPartyStarNumByPartyValue(curValue);
            var partyData = Sys_Family.Instance.GetFamilyPartyDataByStarNum(curStarNum);
            string txtTitleValue = LanguageHelper.GetTextContent(6252, curStarNum.ToString());
            string txtDesc = LanguageHelper.GetTextContent(6233, partyData.level1Food.ToString(), partyData.level2Food.ToString(), partyData.level3Food.ToString());
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { TitlelanId = 6245, TitleValue = txtTitleValue, StrContent = txtDesc });
        }
        private void OnMaxStarBtnClick()
        {
            uint maxValue = Sys_Family.Instance.GetPartyMaxValue();
            uint maxStarNum = Sys_Family.Instance.GetPartyStarNumByPartyValue(maxValue);
            var partyData = Sys_Family.Instance.GetFamilyPartyDataByStarNum(maxStarNum);
            string txtTitleValue = LanguageHelper.GetTextContent(6252, maxStarNum.ToString());
            string txtDesc = LanguageHelper.GetTextContent(6233, partyData.level1Food.ToString(), partyData.level2Food.ToString(), partyData.level3Food.ToString());
            Debug.Log(LanguageHelper.GetTextContent(6233));
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { TitlelanId = 6246, TitleValue = txtTitleValue, StrContent = txtDesc });
        }
        private void OnTimerComplete()
        {
            timer?.Cancel();
            UpdateView();
        }
        private void OnTimerUpdate(float time)
        {
            if (countDownTime >= time && txtTime != null)
            {
                txtTime.text = LanguageHelper.TimeToString((uint)(countDownTime - time), LanguageHelper.TimeFormat.Type_1);
            }
        }
        private void OnPartySubmitSucceed()
        {
            selectFoodUId = 0;
        }
        private void OnBtnStartRuleClick()
        {
            UIManager.OpenUI(EUIID.UI_FamilyParty_StarReward);
        }
        #endregion


        #region  ModelShow
        private void OnCreateModel()
        {
            OnDestroyModel();
            _LoadShowScene();
            _LoadShowModel();
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

            rawImageNPC.gameObject.SetActive(true);
            rawImageNPC.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);

            if (modelDisplay == null)
            {
                modelDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                modelDisplay.onLoaded = OnShowModelLoaded;
            }
        }

        private void _LoadShowModel()
        {
            CSVNpc.Data npcData = CSVNpc.Instance.GetConfData(NPCId);
            if (npcData != null)
            {
                string _modelPath = npcData.model_show;
                modelDisplay.eLayerMask = ELayerMask.ModelShow;
                modelDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
                modelDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
                showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x, showSceneControl.mModelPos.transform.localPosition.y, showSceneControl.mModelPos.transform.localPosition.z);
                showSceneControl.mModelPos.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                _uiModelShowManagerEntity?.Dispose();
                _uiModelShowManagerEntity = null;
                uint weaponId = Constants.UMARMEDID;
                CSVNpc.Data npcData = CSVNpc.Instance.GetConfData(NPCId);
                if (npcData != null && npcData.action_show_id > 0)
                {
                    uint charId = npcData.action_show_id;
                    modelDisplay.mAnimation.UpdateHoldingAnimations(charId, weaponId);
                    GameObject mainGo = modelDisplay.GetPart(EPetModelParts.Main).gameObject;
                    mainGo.SetActive(false);
                    _uiModelShowManagerEntity = WS_UIModelShowManagerEntity.Start(500000, null, modelDisplay.mAnimation, mainGo);
                }
            }
        }

        private void OnDestroyModel()
        {
            _uiModelShowManagerEntity?.Dispose();
            _uiModelShowManagerEntity = null;
            rawImageNPC.gameObject.SetActive(false);
            rawImageNPC.texture = null;
            //modelDisplay?.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref modelDisplay);
            showSceneControl?.Dispose();
        }
        #endregion

        public class UI_FamilyParty_SubmitItemCell
        {
            private ItemData itemData;
            private Transform transform;
            private Image imgBg;
            private Image imgIcon;
            private Text txtNum;
            private GameObject goSelect;
            private GameObject goFashionTitle;
            private Button btnItem;
            private Action<ItemData> onClick;

            public void BindGameObject(GameObject go)
            {
                transform = go.transform;

                imgBg = transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
                imgIcon = transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                txtNum = transform.Find("Text_Number").GetComponent<Text>();
                goSelect = transform.Find("Image_Select").gameObject;
                goFashionTitle = transform.Find("Image_Label").gameObject;
                btnItem = transform.Find("Btn_Item").GetComponent<Button>();
                btnItem.onClick.AddListener(OnItemClick);
            }
            public void UpdateCellView(ItemData _itemData)
            {
                itemData = _itemData;
                if (itemData != null)
                {
                    imgIcon.gameObject.SetActive(true);
                    ImageHelper.SetIcon(imgIcon, itemData.cSVItemData.icon_id);
                    ImageHelper.GetQualityColor_Frame(imgBg, (int)itemData.Quality);
                    txtNum.gameObject.SetActive(true);
                    txtNum.text = itemData.Count.ToString();
                    var isSpecial = Sys_Family.Instance.CheckIsFashionFood(itemData.Id);
                    goFashionTitle.SetActive(isSpecial);
                }
                else
                {
                    imgIcon.gameObject.SetActive(false);
                    ImageHelper.GetQualityColor_Frame(imgBg, 6);
                    txtNum.text = "";
                    goSelect.SetActive(false);
                    goFashionTitle.SetActive(false);
                }
            }
            public void SetSelected(bool seleted)
            {
                goSelect.SetActive(seleted);
            }
            public void AddClickListener(Action<ItemData> onclicked = null)
            {
                onClick = onclicked;
            }
            private void OnItemClick()
            {
                onClick?.Invoke(itemData);
            }
        }

        public class UI_FamilyParty_SubmitSliderStarCell
        {
            private Transform transform;
            private uint starLightValue = 0;//星级满足的阈值
            private GameObject goLight;

            public void Init(Transform trans, uint value)
            {
                transform = trans;
                goLight = transform.Find("Light").gameObject;
                starLightValue = value;
            }

            public void UpdateCellView(uint value)
            {
                bool isLight = value >= starLightValue;
                goLight.SetActive(isLight);
            }
        }
    }
}
