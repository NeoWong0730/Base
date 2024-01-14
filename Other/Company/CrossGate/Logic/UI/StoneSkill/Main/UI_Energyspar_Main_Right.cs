using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;
using System;
using UnityEngine;
using Table;
using Lib.Core;
using Packet;

namespace Logic
{
    /// <summary>
    /// 视图用枚举
    /// </summary>
    public enum EStoneViewType
    {
        ECommon,
        EUnlock,
        ECareerExclusive,
    }

    /// <summary>
    /// 标记技能格子状态枚举
    /// </summary>
    public enum EDarkLightState
    {
        Lock,
        CanUnlock,
        Unlock,
    }

    public class UI_Energyspar_Main_RightDescription : UIComponent
    {
        private Text allStarText;
        private Button helpBtn; //说明按钮
        private Slider levelSlider;
        private Text leftLevelText;
        private Text rightLevelText;
        private Text skillDescText;
        private Text leftStarText;
        private Text rightStarText;
        private Text nowStarText;
        private uint currentId;        

        protected override void Loaded()
        {
            allStarText = transform.Find("Star/Value").GetComponent<Text>();
            helpBtn = transform.Find("Button_Message").GetComponent<Button>();
            levelSlider = transform.Find("Slider_Star").GetComponent<Slider>();
            helpBtn.onClick.AddListener(() => { UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2021072) }); });
            leftLevelText = transform.Find("Text_Level_1/Value").GetComponent<Text>();
            rightLevelText = transform.Find("Text_Level_2/Value").GetComponent<Text>();
            leftStarText = transform.Find("Star_1/Value").GetComponent<Text>();
            rightStarText = transform.Find("Star_3/Value").GetComponent<Text>();
            nowStarText = transform.Find("Star_2/Value").GetComponent<Text>();
            skillDescText = transform.Find("Text_Description").GetComponent<Text>();
        }

        public override void SetData(params object[] arg)
        {
            currentId = (uint)arg[0];
            SetView();
            gameObject.SetActive(true);

        }

        public override void Hide()
        {
            currentId = 0;
            base.Hide();
        }

        private void SetView()
        {
            TextHelper.SetText(skillDescText, Sys_StoneSkill.Instance.GetSkillDesc(currentId));
            TextHelper.SetText(allStarText, Sys_StoneSkill.Instance.allStage.ToString());
            StoneSkillData currentData = Sys_StoneSkill.Instance.GetServerDataById(currentId);
            if (null != currentData)
            {
                TextHelper.SetText(leftLevelText, LanguageHelper.GetTextContent(2021031, currentData.powerStoneUnit.Level.ToString()));
                TextHelper.SetText(nowStarText, Sys_StoneSkill.Instance.allStage.ToString());
                CSVStone.Data csvdata = CSVStone.Instance.GetConfData(currentData.powerStoneUnit.Id);
                if (null != csvdata)
                {
                    TextHelper.SetText(rightLevelText,
                        LanguageHelper.GetTextContent(2021031, (currentData.powerStoneUnit.Level < csvdata.max_level ? (currentData.powerStoneUnit.Level + 1) : csvdata.max_level).ToString()));

                }

                CSVStoneLevel.Data currentLevelData = CSVStoneLevel.Instance.GetConfData(currentData.powerStoneUnit.Id * 1000 + currentData.powerStoneUnit.Level);

                if (null != currentLevelData)
                {
                    TextHelper.SetText(leftStarText, currentLevelData.sum_stage.ToString());
                    TextHelper.SetText(nowStarText, Sys_StoneSkill.Instance.allStage.ToString());
                    float tempSliderValue = 0;
                    CSVStoneLevel.Data nextLevelData = CSVStoneLevel.Instance.GetConfData(currentData.powerStoneUnit.Id * 1000 + currentData.powerStoneUnit.Level + 1);

                    if (null != nextLevelData)
                    {
                        TextHelper.SetText(rightStarText, nextLevelData.sum_stage.ToString());
                        tempSliderValue = nextLevelData.sum_stage;
                    }
                    else
                    {
                        TextHelper.SetText(rightStarText, currentLevelData.sum_stage.ToString());
                        tempSliderValue = currentLevelData.sum_stage;
                    }

                    if (tempSliderValue == currentLevelData.sum_stage)
                    {
                        tempSliderValue = 1;
                    }
                    else
                    {
                        tempSliderValue = (Sys_StoneSkill.Instance.allStage - currentLevelData.sum_stage) / (tempSliderValue - currentLevelData.sum_stage);
                    }

                    levelSlider.value = tempSliderValue;
                    //计算星星位置
                    float width = levelSlider.GetComponent<RectTransform>().rect.width;
                    nowStarText.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(width * tempSliderValue - width / 2,
                        nowStarText.transform.parent.GetComponent<RectTransform>().localPosition.y, 0);

                }

            }
        }

        public override void Reset()
        {
            SetView();
        }
    }

    public class UI_Energyspar_Main_RightExchange : UIComponent
    {
        private Transform exchangeitemGrid;
        private Button exchangeBtn;
        private Text descText;
        CSVStone.Data configData;

        private Dictionary<GameObject, PropItem> dic = new Dictionary<GameObject, PropItem>();
        protected override void Loaded()
        {
            exchangeitemGrid = transform.Find("Grid");
            descText = transform.Find("Text_Description").GetComponent<Text>();
            exchangeBtn = transform.Find("Button_Exchange").GetComponent<Button>();
            exchangeBtn.onClick.AddListener(OnExchangeBtnClick);
        }

        private void OnExchangeBtnClick()
        {

            if (null != configData)
            {
                if (null != configData.cost)
                {
                    for (int i = 0; i < configData.cost.Count; i++)
                    {
                        if (null != configData.cost[i] && configData.cost[i].Count >= 2)
                        {
                            long haveNum = Sys_Bag.Instance.GetItemCount(configData.cost[i][0]);
                            if (configData.cost[i][1] > haveNum)
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021060));
                                return;
                            }
                        }
                    }
                }
            }
            Sys_StoneSkill.Instance.OnPowerStoneActivateReq(configData.id);
        }

        public override void SetData(params object[] arg)
        {
            uint energysparId = (uint)arg[0];
            configData = CSVStone.Instance.GetConfData(energysparId);
            SetView();
            gameObject.SetActive(true);
        }

        private void SetView()
        {
            if (null != configData)
            {
                TextHelper.SetText(descText, Sys_StoneSkill.Instance.GetSkillDesc(configData.id));
                bool action = Sys_Role.Instance.Role.Level >= configData.level_limit;
                exchangeBtn.interactable = action;
                exchangeBtn.GetComponent<ButtonScaler>().enabled = action;
                ImageHelper.SetImageGray(exchangeBtn.GetComponent<Image>(), !action, false);
                int count = configData.cost.Count;
                FrameworkTool.CreateChildList(exchangeitemGrid, count);
                for (int i = 0; i < count; i++)
                {
                    if (null != configData.cost[i] && configData.cost[i].Count >= 2)
                    {
                        Transform transform = exchangeitemGrid.GetChild(i);
                        if(dic.ContainsKey(transform.gameObject))
                        {
                            PropItem item = dic[transform.gameObject];
                            item.SetData(new PropIconLoader.ShowItemData(configData.cost[i][0], configData.cost[i][1],
                               true, false, false, false, false, _bShowCount: true, _bShowBagCount: true), EUIID.UI_Energyspar);
                        }
                        else
                        {
                            PropItem item = new PropItem();
                            item.BindGameObject(transform.gameObject);
                            dic.Add(transform.gameObject, item);
                            item.SetData(new PropIconLoader.ShowItemData(configData.cost[i][0], configData.cost[i][1],
                               true, false, false, false, false, _bShowCount: true, _bShowBagCount: true), EUIID.UI_Energyspar);
                        }
                        
                    }
                }
            }
        }

        public override void Reset()
        {
            SetView();
        }

        public override void Hide()
        {
            configData = null;
            base.Hide();
        }
    }

    public class UI_Energyspar_Main_RightRefine : UIComponent
    {
        public class DarkOrLightCeil
        {
            public GameObject gameObject;
            private Button funtionBtn;
            private GameObject lockGo;
            private GameObject addGo;
            private Image iconImage;
            private Image lineImage;
            private Animator showAni;
            private uint index;
            private uint stoneId;
            private uint chaoSkillId;
            private StageSkillUnit stageSkillUnit;
            private EDarkLightState currentState;            
            public void Init(Transform transform)
            {
                gameObject = transform.gameObject;
                funtionBtn = transform.gameObject.GetComponent<Button>();
                funtionBtn.onClick.AddListener(OnFuntionBtnClick);
                lockGo = transform.Find("Image_Lock").gameObject;
                addGo = transform.Find("Image_Add").gameObject;
                iconImage = transform.Find("Image_Icon").GetComponent<Image>();
                showAni = iconImage.GetComponent<Animator>();
                lineImage = transform.Find("Image_Line")?.GetComponent<Image>();
                InitParamData();
            }

            private Color noColor;

            private Color lightColor;

            private Color darkColor;

            private Color GetColor(int state)
            {
                switch (state)
                {
                    case 0:
                        return noColor;
                    case 1:
                        return lightColor;
                    case 2:
                        return darkColor;
                    default:
                        return noColor;
                }
            }

            private void InitParamData()
            {
                string[] _strs1 = CSVParam.Instance.GetConfData(774).str_value.Split('|');
                noColor = new Color(float.Parse(_strs1[0]) / 255f, float.Parse(_strs1[1]) / 255f, float.Parse(_strs1[2]) / 255f, float.Parse(_strs1[3]) / 255f);
                string[] _strs2 = CSVParam.Instance.GetConfData(775).str_value.Split('|');
                lightColor = new Color(float.Parse(_strs2[0]) / 255f, float.Parse(_strs2[1]) / 255f, float.Parse(_strs2[2]) / 255f, float.Parse(_strs2[3]) / 255f);
                string[] _strs3 = CSVParam.Instance.GetConfData(776).str_value.Split('|');
                darkColor = new Color(float.Parse(_strs3[0]) / 255f, float.Parse(_strs3[1]) / 255f, float.Parse(_strs3[2]) / 255f, float.Parse(_strs3[3]) / 255f);
            }

            private void OnFuntionBtnClick()
            {
                CSVStoneStage.Data stageData = CSVStoneStage.Instance.GetConfData(stoneId * 1000 + (index + 1));
                if (currentState == EDarkLightState.Lock)
                {
                    if (null != stageData)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021061, stageData.stone_level.ToString(), stageData.stage.ToString())); ;
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021062));
                    }
                }
                else if (currentState == EDarkLightState.CanUnlock)
                {
                    CSVStone.Data configData = CSVStone.Instance.GetConfData(stoneId);
                    if (null != configData)
                    {
                        HideChild();
                        UIManager.OpenUI(EUIID.UI_Energyspar_Advanced, false, configData);
                    }
                    else
                    {
                        DebugUtil.LogErrorFormat("CSVStone.Data 表 无法获取 Id = {0}", stoneId);
                    }
                }
                else
                {
                    HideChild();
                    CSVPassiveSkillInfo.Data passiveSkillData;
                    if (chaoSkillId == 0)
                    {
                        passiveSkillData = CSVPassiveSkillInfo.Instance.GetConfData(stageSkillUnit.SkillId);
                    }
                    else
                    {
                        passiveSkillData = CSVPassiveSkillInfo.Instance.GetConfData(chaoSkillId);
                    }
                    EnergysparTipsParam param = new EnergysparTipsParam();
                    param.passiveSkillData = passiveSkillData;
                    param.stoneId = stoneId;
                    param.index = index;
                    param.isChao = (chaoSkillId != 0);
                    UIManager.OpenUI(EUIID.UI_Energyspar_Tips, false, param);
                }
            }

            private void HideChild()
            {
                showAni.enabled = false;
                for (int i = 0; i < showAni.transform.childCount; i++)
                {
                    if (showAni.transform.GetChild(i).gameObject.activeSelf)
                    {
                        showAni.transform.GetChild(i).gameObject.SetActive(false);
                    }

                }
            }

            public void PlayAnimator()
            {
                showAni.enabled = true;
                if (chaoSkillId == 0)
                {
                    showAni.Play("BtnSkill_Open", -1, 0);
                }
                else
                {
                    showAni.Play("BtnSkill1_Open", -1, 0);
                }
            }


            public void SetCeilData(uint id, EDarkLightState state, uint index, StageSkillUnit stageSkillUnit = null, uint chaoSkill = 0)
            {
                HideChild();
                this.index = index;
                chaoSkillId = chaoSkill;
                this.stageSkillUnit = stageSkillUnit;
                stoneId = id;
                currentState = state;
                lockGo.gameObject.SetActive(state == EDarkLightState.Lock);
                iconImage.gameObject.SetActive(state == EDarkLightState.Unlock);
                addGo.gameObject.SetActive(state == EDarkLightState.CanUnlock);
                Color temp = GetColor(0);
                if (state == EDarkLightState.Unlock)
                {
                    CSVPassiveSkillInfo.Data passiveSkillData;
                    if (chaoSkill == 0)
                    {
                        temp = GetColor((int)stageSkillUnit.SkillType + 1);
                        passiveSkillData = CSVPassiveSkillInfo.Instance.GetConfData(stageSkillUnit.SkillId);
                    }
                    else
                    {
                        passiveSkillData = CSVPassiveSkillInfo.Instance.GetConfData(chaoSkill);
                    }

                    if (null != passiveSkillData)
                    {
                        ImageHelper.SetIcon(iconImage, passiveSkillData.icon);
                    }
                }
                if (null != lineImage)
                {
                    lineImage.color = temp;
                }
            }

        }

        private Text descText;
        private Button helpBtn;
        private Button refineAndUpBtn;
        private Text refineAndUpBtnText;
        private DarkOrLightCeil midGo;
        private List<DarkOrLightCeil> skillList = new List<DarkOrLightCeil>();
        private DarkOrLightCeil darkLightGo1;
        private DarkOrLightCeil darkLightGo2;
        private DarkOrLightCeil darkLightGo3;


        CSVStone.Data configData;        

        protected override void Loaded()
        {
            descText = transform.Find("Text_Description").GetComponent<Text>();
            helpBtn = transform.Find("Button_Message").GetComponent<Button>();
            helpBtn.onClick.AddListener(() => { UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2021074) }); });
            refineAndUpBtn = transform.Find("Button_Refine").GetComponent<Button>();
            refineAndUpBtnText = transform.Find("Button_Refine/Text_01").GetComponent<Text>();

            refineAndUpBtn.onClick.AddListener(OnRefineAndUpBtnClick);
            midGo = new DarkOrLightCeil();
            midGo.Init(transform.Find("Btn_Skill1"));
            DarkOrLightCeil sub1 = new DarkOrLightCeil();
            sub1.Init(transform.Find("Btn_Skill2"));
            DarkOrLightCeil sub2 = new DarkOrLightCeil();
            sub2.Init(transform.Find("Btn_Skill3"));
            DarkOrLightCeil sub3 = new DarkOrLightCeil();
            sub3.Init(transform.Find("Btn_Skill4"));
            skillList.Add(sub1);
            skillList.Add(sub2);
            skillList.Add(sub3);
        }

        private void OnRefineAndUpBtnClick()
        {
            if (Sys_StoneSkill.Instance.CanAdvance(configData.id))
            {
                UIManager.OpenUI(EUIID.UI_Energyspar_Advanced, false, configData);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_Energyspar_Refine, false, configData);
            }

        }

        public override void SetData(params object[] arg)
        {
            uint energysparId = (uint)arg[0];
            configData = CSVStone.Instance.GetConfData(energysparId);
            SetView();
            gameObject.SetActive(true);
        }

        private void SetView()
        {
            if (null != configData)
            {
                TextHelper.SetText(descText, Sys_StoneSkill.Instance.GetSkillDesc(configData.id));
                TextHelper.SetText(refineAndUpBtnText, Sys_StoneSkill.Instance.CanAdvance(configData.id) ? LanguageHelper.GetTextContent(2021022) : LanguageHelper.GetTextContent(2021028));
                helpBtn.gameObject.SetActive(configData.max_stage != 0);
                midGo.gameObject.SetActive(configData.max_stage != 0);
                for (int i = 0; i < skillList.Count; i++)
                {
                    skillList[i].gameObject.SetActive(configData.max_stage != 0);
                }
                StoneSkillData currentData = Sys_StoneSkill.Instance.GetServerDataById(configData.id);
                if (null != currentData)
                {
                    uint stage = currentData.powerStoneUnit.Stage;
                    refineAndUpBtn.gameObject.SetActive(!(configData.max_level <= currentData.powerStoneUnit.Level && stage >= configData.max_stage));
                    for (int i = 0; i < skillList.Count; i++)
                    {
                        if (stage > i)
                        {
                            skillList[i].SetCeilData(currentData.powerStoneUnit.Id, EDarkLightState.Unlock, (uint)i, currentData.powerStoneUnit.StageSkill[i]);
                        }
                        else
                        {
                            CSVStoneStage.Data stoneStageData = CSVStoneStage.Instance.GetConfData(configData.id * 1000 + ((uint)i + 1));
                            if (stoneStageData.stone_level <= currentData.powerStoneUnit.Level)
                            {
                                skillList[i].SetCeilData(currentData.powerStoneUnit.Id, EDarkLightState.CanUnlock, (uint)i);
                            }
                            else
                            {
                                skillList[i].SetCeilData(currentData.powerStoneUnit.Id, EDarkLightState.Lock, (uint)i);
                            }
                        }
                    }

                    if (currentData.powerStoneUnit.ChaosSkill == 0)
                    {
                        midGo.SetCeilData(currentData.powerStoneUnit.Id, EDarkLightState.Lock, 3);
                    }
                    else
                    {
                        midGo.SetCeilData(currentData.powerStoneUnit.Id, EDarkLightState.Unlock, 3, null, currentData.powerStoneUnit.ChaosSkill);
                    }
                }
            }

        }

        public override void Hide()
        {
            configData = null;
            base.Hide();
        }

        public override void Reset()
        {
            SetView();
        }

        public void ReverseEnd(uint stoneId, uint stage)
        {
            if (null != configData)
            {
                StoneSkillData currentData = Sys_StoneSkill.Instance.GetServerDataById(configData.id);
                if (null != currentData)
                {
                    int index = (int)stage - 1;
                    skillList[index].SetCeilData(currentData.powerStoneUnit.Id, EDarkLightState.Unlock, (uint)index, currentData.powerStoneUnit.StageSkill[index]);
                    skillList[index].PlayAnimator();
                }
            }
        }

        public void ChaoSkillChange(uint stoneId)
        {
            if (null != configData)
            {
                StoneSkillData currentData = Sys_StoneSkill.Instance.GetServerDataById(configData.id);
                if (null != currentData)
                {
                    midGo.SetCeilData(currentData.powerStoneUnit.Id, EDarkLightState.Unlock, 3, null, currentData.powerStoneUnit.ChaosSkill);
                    midGo.PlayAnimator();
                }
            }
        }


        public void AdvancedEnd(uint stoneId)
        {
            if (null != configData)
            {
                StoneSkillData currentData = Sys_StoneSkill.Instance.GetServerDataById(configData.id);
                if (null != currentData)
                {
                    TextHelper.SetText(refineAndUpBtnText, Sys_StoneSkill.Instance.CanAdvance(configData.id) ? LanguageHelper.GetTextContent(2021022) : LanguageHelper.GetTextContent(2021028));
                    uint stage = currentData.powerStoneUnit.Stage;
                    int index = (int)stage - 1;
                    skillList[index].SetCeilData(currentData.powerStoneUnit.Id, EDarkLightState.Unlock, (uint)index, currentData.powerStoneUnit.StageSkill[index]);
                    refineAndUpBtn.gameObject.SetActive(!(configData.max_level <= currentData.powerStoneUnit.Level && stage >= configData.max_stage));
                    if (skillList.Count == stage)
                    {
                        midGo.SetCeilData(currentData.powerStoneUnit.Id, EDarkLightState.Unlock, 3, null, currentData.powerStoneUnit.ChaosSkill);
                        midGo.PlayAnimator();
                    }
                    skillList[index].PlayAnimator();
                }
            }
        }

    }

    public class UI_Energyspar_Main_Right : UIComponent
    {
        private Dictionary<EStoneViewType, UIComponent> rightViews = new Dictionary<EStoneViewType, UIComponent>();
        public Image StoneIconImage;
        public Text stoneNameText;
        public Text stoneTypeText;
        public Text stoneStateText; // 状态用文本
        public GameObject starGo;
        public Transform starGroupGo;
        public Button resolveBtn;

        public UI_Energyspar_Main_RightDescription descriptionView;
        public UI_Energyspar_Main_RightExchange exchangeView;
        public UI_Energyspar_Main_RightRefine refineView;
        public EStoneViewType currentViewType = EStoneViewType.ECareerExclusive;
        private uint currentId;        
        protected override void Loaded()
        {
            descriptionView = AddComponent<UI_Energyspar_Main_RightDescription>(transform.Find("View_Description"));
            exchangeView = AddComponent<UI_Energyspar_Main_RightExchange>(transform.Find("View_Exchange"));
            refineView = AddComponent<UI_Energyspar_Main_RightRefine>(transform.Find("View_Refine"));
            rightViews.Add(EStoneViewType.ECareerExclusive, descriptionView);
            rightViews.Add(EStoneViewType.ECommon, refineView);
            rightViews.Add(EStoneViewType.EUnlock, exchangeView);
            StoneIconImage = transform.Find("Image_SkillIcon").GetComponent<Image>();
            stoneNameText = transform.Find("Text_SkillName").GetComponent<Text>();
            stoneTypeText = transform.Find("Text_SkillType").GetComponent<Text>();
            stoneStateText = transform.Find("Text_State").GetComponent<Text>();
            starGo = transform.Find("Text_SkillName/Star_Dark").gameObject;
            starGroupGo = transform.Find("Text_SkillName/StarGroup");
            resolveBtn = transform.Find("Btn_Resolve").GetComponent<Button>();
            resolveBtn.onClick.AddListener(ResolveBtnClick);
        }

        public override void Hide()
        {
            descriptionView.Hide();
            exchangeView.Hide();
            refineView.Hide();
        }
        public override void SetData(params object[] arg)
        {
            if (null != arg)
            {
                currentId = (uint)arg[0];
                ViewStateSet();
                if (rightViews.ContainsKey(currentViewType))
                    rightViews[currentViewType].SetData(currentId);
            }
        }

        private void ResolveBtnClick()
        {
            UIManager.OpenUI(EUIID.UI_Energyspar_Resolve, false, currentId);
        }

        public void ViewStateSet()
        {
            FrameworkTool.DestroyChildren(starGroupGo.gameObject);
            CSVStone.Data cureentData = CSVStone.Instance.GetConfData(currentId);
            if (null != cureentData)
            {
                ImageHelper.SetIcon(StoneIconImage, cureentData.icon);
                TextHelper.SetText(stoneNameText, cureentData.stone_name);
                TextHelper.SetText(stoneTypeText, cureentData.type == 1 ? LanguageHelper.GetTextContent(2021045) : LanguageHelper.GetTextContent(2021046));
                if (cureentData.exclusive)
                {
                    currentViewType = EStoneViewType.ECareerExclusive;
                    StoneSkillData severData = Sys_StoneSkill.Instance.GetServerDataById(currentId);
                    if (null != severData)
                    {
                        TextHelper.SetText(stoneStateText, LanguageHelper.GetTextContent(2021004, severData.powerStoneUnit.Level.ToString(), cureentData.max_level.ToString()));
                        SetDarkStar(severData.powerStoneUnit.Stage);
                    }
                    else
                    {
                        TextHelper.SetText(stoneStateText, LanguageHelper.GetTextContent(2021005));
                    }

                }
                else
                {
                    StoneSkillData severData = Sys_StoneSkill.Instance.GetServerDataById(currentId);
                    if (null == severData)
                    {
                        currentViewType = EStoneViewType.EUnlock;
                        if (Sys_Role.Instance.Role.Level < cureentData.level_limit)
                        {

                            TextHelper.SetText(stoneStateText, LanguageHelper.GetTextContent(2021006, cureentData.level_limit.ToString()));
                        }
                        else
                        {
                            TextHelper.SetText(stoneStateText, LanguageHelper.GetTextContent(2021005));
                        }

                    }
                    else
                    {
                        currentViewType = EStoneViewType.ECommon;
                        TextHelper.SetText(stoneStateText, LanguageHelper.GetTextContent(2021004, severData.powerStoneUnit.Level.ToString(), cureentData.max_level.ToString()));
                        SetDarkStar(severData.powerStoneUnit.Stage);
                    }
                }

                resolveBtn.gameObject.SetActive(cureentData.can_decompose && currentViewType == EStoneViewType.ECommon);
            }

            List<EStoneViewType> keyList = new List<EStoneViewType>(rightViews.Keys);
            for (int i = 0; i < keyList.Count; i++)
            {
                EStoneViewType key = keyList[i];
                UIComponent value = rightViews[key];
                if (key == currentViewType)
                {
                    value.Show();
                }
                else
                {
                    value.Hide();
                }
            }
        }

        private void SetDarkStar(uint state)
        {
            for (int i = 0; i < state; i++)
            {
                GameObject go = GameObject.Instantiate<GameObject>(starGo, starGroupGo);
                Small_Star small_Star = new Small_Star();
                small_Star.BindGameobject(go.transform);
                small_Star.SetState(true);
            }

            if (Sys_StoneSkill.Instance.CanAdvance(currentId))//即将进阶 显示一个暗
            {
                GameObject go = GameObject.Instantiate<GameObject>(starGo, starGroupGo);
                Small_Star small_Star = new Small_Star();
                small_Star.BindGameobject(go.transform);
                small_Star.SetState(false);
            }
        }

        public void StoneAdvanceEnd(uint stoneId)
        {
            ViewStateSet();
            refineView.AdvancedEnd(stoneId);
        }

        public void ReverseEnd(uint stoneId, uint stage)
        {
            refineView.ReverseEnd(stoneId, stage);
        }

        public void ChaoSkillChange(uint stoneId)
        {
            refineView.ChaoSkillChange(stoneId);
        }

        public void ResetEx(uint stoneId)
        {
            ViewStateSet();
            rightViews[currentViewType].SetData(stoneId);
        }

    }
}
