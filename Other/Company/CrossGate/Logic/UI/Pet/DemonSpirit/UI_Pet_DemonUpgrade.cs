using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Packet;

namespace Logic
{
    public enum EDemonSpiritHitFx
    {
        Hit2 = 2,
        Hit3 = 3,
        Hit5 = 5
    }
    public class UI_Pet_DemonUpgrade_Layout
    {
        private Button closeBtn;
        private Button previewBtn;
        private Button upExpBtn;
        public Transform topTransform;
        public Transform middleTransform;
        public Transform costPropItemParent;
        public GameObject fullGo;
        public GameObject upgradeGo;
        public PropItem costPetPropItem;
        public CP_ToggleRegistry selectToggle;
        public Text addExpText;
        public Text hitText;
        public Text weekTimesText;
        public Text pointTipsText;
        public Animator ani;
        public GameObject expAddFx;
        public Dictionary<EDemonSpiritHitFx, GameObject> hitGos = new Dictionary<EDemonSpiritHitFx, GameObject>(3);
        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/View_TipsBgNew05/Btn_Close").GetComponent<Button>();
            previewBtn = transform.Find("Animator/View_Top/Button_Preview").GetComponent<Button>();
            upExpBtn = transform.Find("Animator/View_Below/Upgrade/Cost/Button_Up").GetComponent<Button>();
            fullGo = transform.Find("Animator/View_Below/Full").gameObject;
            upgradeGo = transform.Find("Animator/View_Below/Upgrade").gameObject;
            topTransform = transform.Find("Animator/View_Top");
            middleTransform = transform.Find("Animator/View_Middle");
            costPropItemParent = transform.Find("Animator/View_Below/Upgrade/Item");
            costPetPropItem = new PropItem();
            costPetPropItem.BindGameObject(transform.Find("Animator/View_Below/Upgrade/Pet/PropItem").gameObject);
            addExpText = transform.Find("Animator/View_Below/Upgrade/Cost/Text1").GetComponent<Text>();
            hitText = transform.Find("Animator/View_Below/Upgrade/Cost/Text2").GetComponent<Text>();
            weekTimesText =  transform.Find("Animator/View_Below/Upgrade/Cost/Text4").GetComponent<Text>();
            pointTipsText = transform.Find("Animator/View_Below/Upgrade/Pet/Text").GetComponent<Text>();
            selectToggle = transform.Find("Animator/View_Below/Upgrade/Toggle").GetComponent<CP_ToggleRegistry>();
            ani = transform.Find("Animator/View_Top/FX").GetComponent<Animator>();
            expAddFx = transform.Find("Animator/View_Top/Slider_Lv").GetChild(0).gameObject;
            hitGos.Add(EDemonSpiritHitFx.Hit2, transform.Find("Animator/View_Top/FX/2").gameObject);
            hitGos.Add(EDemonSpiritHitFx.Hit3, transform.Find("Animator/View_Top/FX/3").gameObject);
            hitGos.Add(EDemonSpiritHitFx.Hit5, transform.Find("Animator/View_Top/FX/5").gameObject);
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            previewBtn.onClick.AddListener(listener.PreViewBtnClicked);
            upExpBtn.onClick.AddListener(listener.UpExpBtnClicked);
            selectToggle.onToggleChange = listener.OnSelectTypeChange;
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void PreViewBtnClicked();
            void UpExpBtnClicked();
            void OnSelectTypeChange(int curToggle, int old);
        }
    }

    public class UI_Pet_DemonUpgradeTop : UIComponent
    {
        private Text nameText;
        private Text levelText;
        private Slider expSlider;
        private RectTransform sliderRect;
        private Text percentText;
        private Image sphereImage;
        private PetSoulBeadInfo sphereTemp;
        protected CSVSoulBead.Data sphereData;
        private float maxSliderWidth;
        protected override void Loaded()
        {
            nameText = transform.Find("Text_Name").GetComponent<Text>();
            levelText = transform.Find("Text_Name/Text").GetComponent<Text>();
            expSlider = transform.Find("Slider_Lv").GetComponent<Slider>();
            percentText =transform.Find("Text_Number").GetComponent<Text>();
            sphereImage = transform.Find("Image1/Image3/Model").gameObject.AddComponent<Image>();
            sliderRect = transform.Find("Slider_Lv/Image1").GetComponent<RectTransform>();
            maxSliderWidth = sliderRect.rect.width;
        }

        //type == 1 道具 2 宠物
        public void SetView(PetSoulBeadInfo sphereTemp, int selectType, uint petuid = 0)
        {
            if (null != sphereTemp)
            {
                this.sphereTemp = sphereTemp;
                uint level = sphereTemp.Level;
                var sphereId = sphereTemp.Type * 10000 + level;
                sphereData = CSVSoulBead.Instance.GetConfData(sphereId);
                if (null != sphereData)
                {
                    ImageHelper.SetIcon(sphereImage, sphereData.icon);
                    TextHelper.SetText(levelText, 680003020, level.ToString());
                    TextHelper.SetText(nameText, 680003010u + sphereData.type - 1);
                    expSlider.value = (sphereTemp.Exp + 0f) / sphereData.exp;
                    if(selectType == 1)
                    {
                        var nextSphereData = CSVSoulBead.Instance.GetConfData(sphereId + 1);
                        bool isMax = null == nextSphereData;
                        if(isMax)
                        {
                            percentText.gameObject.SetActive(false);
                            sliderRect.gameObject.SetActive(false);
                        }
                        else
                        {
                            uint addExp = 0;
                            if (null != sphereData.add_exp && sphereData.add_exp.Count >= 1)
                            {
                                addExp = sphereData.add_exp[0];
                            }
                            float sliderValue = ((sphereTemp.Exp + addExp + 0f) / sphereData.exp);
                            sliderValue = sliderValue > 1 ? 1f : sliderValue;
                            sliderRect.sizeDelta = new Vector2(maxSliderWidth * sliderValue, sliderRect.rect.height);
                            TextHelper.SetText(percentText, 680003044, sphereTemp.Exp.ToString(), addExp.ToString(), sphereData.exp.ToString());
                            sliderRect.gameObject.SetActive(true);
                            percentText.gameObject.SetActive(true);
                        }
                    }
                    else if(selectType == 2)
                    {
                        if(petuid == 0)
                        {
                            TextHelper.SetText(percentText, 680003039, sphereTemp.Exp.ToString(), sphereData.exp.ToString());
                            sliderRect.gameObject.SetActive(false);
                        }
                        else
                        {
                            var costClientPet = Sys_Pet.Instance.GetPetByUId(petuid);
                            if(null != costClientPet)
                            {
                                var score = costClientPet.petUnit.SimpleInfo.Score;
                                var addExpId = Sys_Pet.Instance.GetAddDemonSpiritExpId(score);
                                CSVSoulBeadPetAddexp.Data beadAddExp = CSVSoulBeadPetAddexp.Instance.GetConfData(addExpId);
                                if(null != beadAddExp)
                                {
                                    uint addExp = beadAddExp.base_exp;
                                    TextHelper.SetText(percentText, 680003044, sphereTemp.Exp.ToString(), addExp.ToString(), sphereData.exp.ToString());                                    
                                    float sliderValue = ((sphereTemp.Exp + addExp + 0f) / sphereData.exp);
                                    sliderValue = sliderValue > 1 ? 1f : sliderValue;
                                    sliderRect.sizeDelta = new Vector2(maxSliderWidth * sliderValue, sliderRect.rect.height);
                                    TextHelper.SetText(percentText, 680003044, sphereTemp.Exp.ToString(), addExp.ToString(), sphereData.exp.ToString());
                                    sliderRect.gameObject.SetActive(true);
                                }
                                else
                                {
                                    sliderRect.gameObject.SetActive(false);
                                }
                            }
                        }
                    }
                }
            }

        }
    }

    public class UI_Pet_DemonUpgradeSkillView:UIComponent
    {
        private GameObject leftPresentSkillGo;
        private GameObject leftNextSkillGo;

        private GameObject rightPresentSkillGo;
        private GameObject rightNextSkillGo;

        private GameObject leftMaxGo;
        private GameObject rightMaxGo;

        private Button leftPresentSkillBtn;
        private Button leftNextSkillBtn;
        private Button rightPresentSkillBtn;
        private Button rightNextSkillBtn;
        private PetSoulBeadInfo sphereTemp;
        protected CSVSoulBead.Data sphereData;
        protected override void Loaded()
        {
            leftPresentSkillGo = transform.Find("Left/Skill/Present").gameObject;
            leftPresentSkillBtn = transform.Find("Left/Skill/Present/PetSkillItem01/ImageBG").GetComponent<Button>();
            leftPresentSkillBtn.onClick.AddListener(Skill1BtnClicked);

            leftNextSkillGo = transform.Find("Left/Skill/Next").gameObject;
            leftNextSkillBtn = transform.Find("Left/Skill/Next/PetSkillItem01/ImageBG").GetComponent<Button>();
            leftNextSkillBtn.onClick.AddListener(Skill1PreviewBtnClicked);

            rightPresentSkillGo = transform.Find("Right/Skill/Present").gameObject;
            rightPresentSkillBtn = transform.Find("Right/Skill/Present/PetSkillItem01/ImageBG").GetComponent<Button>();
            rightPresentSkillBtn.onClick.AddListener(Skill2BtnClicked);

            rightNextSkillGo = transform.Find("Right/Skill/Next").gameObject;
            rightNextSkillBtn = transform.Find("Right/Skill/Next/PetSkillItem01/ImageBG").GetComponent<Button>();
            rightNextSkillBtn.onClick.AddListener(Skill2PreviewBtnClicked);

            leftMaxGo = transform.Find("Left/Image_Full").gameObject;
            rightMaxGo = transform.Find("Right/Image_Full").gameObject;
        }

        public void SetView(PetSoulBeadInfo sphereTemp)
        {
            if (null != sphereTemp)
            {
                this.sphereTemp = sphereTemp;
                var sphereId = sphereTemp.Type * 10000 + sphereTemp.Level;
                sphereData = CSVSoulBead.Instance.GetConfData(sphereId);
                if (null != sphereData)
                {
                    uint level = sphereTemp.Level;
                    uint skill1 = sphereTemp.SkillIds[0];
                    uint skill2 = sphereTemp.SkillIds[1];
                    sphereId = sphereTemp.Type * 10000 + level + 1;
                    var nextSphereData = CSVSoulBead.Instance.GetConfData(sphereId);
                    bool isMax = null == nextSphereData;
                    if (!isMax)
                    {
                        if (null != nextSphereData.base_skill_group && nextSphereData.base_skill_group.Count >= 2)
                        {
                            var nextLevel = nextSphereData.base_skill_group[1];
                            uint nextSkillId = skill1 / 1000 * 1000 + nextLevel;
                            if (nextSkillId == skill1)//max
                            {
                                SetPreSentSkill(leftPresentSkillGo, skill1);
                                SetNextSkill(leftNextSkillGo, 0);
                                leftMaxGo.SetActive(true);
                            }
                            else
                            {
                                SetPreSentSkill(leftPresentSkillGo, skill1);
                                SetNextSkill(leftNextSkillGo, nextSkillId);
                                leftMaxGo.SetActive(false);
                            }
                        }

                        if (skill2 != 0)
                        {
                            if (null != nextSphereData.special_skill_group && nextSphereData.special_skill_group.Count >= 2)
                            {
                                var nextLevel = nextSphereData.special_skill_group[1];
                                uint nextSkillId = skill2 / 1000 * 1000 + nextLevel;
                                if (nextSkillId == skill2) //max
                                {
                                    SetPreSentSkill(rightPresentSkillGo, skill2);
                                    SetNextSkill(rightNextSkillGo, 0);
                                    rightMaxGo.SetActive(true);
                                }
                                else
                                {
                                    SetPreSentSkill(rightPresentSkillGo, skill2);
                                    SetNextSkill(rightNextSkillGo, nextSkillId);
                                    rightMaxGo.SetActive(false);
                                }
                            }
                            else
                            {
                                SetPreSentSkill(rightPresentSkillGo, 0);
                                SetNextSkill(rightNextSkillGo, 0);
                                rightMaxGo.SetActive(false);
                            }
                        }
                        else
                        {
                            SetPreSentSkill(rightPresentSkillGo, 0);
                            SetNextSkill(rightNextSkillGo, 0);
                            rightMaxGo.SetActive(false);
                        }
                    }
                    else //max
                    {
                        SetPreSentSkill(leftPresentSkillGo, skill1);
                        SetNextSkill(leftNextSkillGo, 0);
                        SetPreSentSkill(rightPresentSkillGo, skill2);
                        SetNextSkill(rightNextSkillGo, 0);
                        leftMaxGo.SetActive(true);
                        rightMaxGo.SetActive(true);
                    }
                }
            }
        }

        private void SetPreSentSkill(GameObject skillGo, uint skillId)
        {
            bool hasSkill = skillId > 0;
            var levelText = skillGo.transform.Find("Text_Level").GetComponent<Text>();
            if (hasSkill)
            {
                if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
                {
                    CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                    
                    if (skillInfo != null)
                    {
                        TextHelper.SetText(skillGo.transform.Find("PetSkillItem01/Text_Name").GetComponent<Text>(), skillInfo.name);
                        TextHelper.SetText(levelText, 680003020, skillInfo.level.ToString());
                        ImageHelper.SetIcon(skillGo.transform.Find("PetSkillItem01/Image_Skill").GetComponent<Image>(), skillInfo.icon);
                        ImageHelper.GetPetSkillQuality_Frame(skillGo.transform.Find("PetSkillItem01/Image_Quality").GetComponent<Image>(), (int)skillInfo.quality);
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", skillId);
                    }
                }
                else
                {
                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                    if (skillInfo != null)
                    {
                        TextHelper.SetText(skillGo.transform.Find("PetSkillItem01/Text_Name").GetComponent<Text>(), skillInfo.name);
                        TextHelper.SetText(levelText, 680003020, skillInfo.level.ToString());
                        ImageHelper.SetIcon(skillGo.transform.Find("PetSkillItem01/Image_Skill").GetComponent<Image>(), skillInfo.icon);
                        ImageHelper.GetPetSkillQuality_Frame(skillGo.transform.Find("PetSkillItem01/Image_Quality").GetComponent<Image>(), (int)skillInfo.quality);
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                    }
                }
                var unlockGo = skillGo.transform.Find("Lock").gameObject;
                var levelGroundImageGo = skillGo.transform.Find("Image_Level").gameObject;
                levelGroundImageGo?.SetActive(true);
                unlockGo?.SetActive(false);
                levelText.gameObject.SetActive(true);
            }
            else
            {
                var lockData = CSVSoulBead.Instance.GetSkillLockLevelData(sphereData.type);
                if (null != lockData)
                {
                    TextHelper.SetText(skillGo.transform.Find("Lock/Text").GetComponent<Text>(), 680003040, LanguageHelper.GetTextContent(680003010u + sphereData.type - 1), lockData.level.ToString());//解锁等级
                    var unlockGo = skillGo.transform.Find("Lock").gameObject;
                    var levelGroundImageGo = skillGo.transform.Find("Image_Level").gameObject;
                    levelGroundImageGo?.SetActive(false);
                    unlockGo?.SetActive(true);
                    levelText.gameObject.SetActive(false);
                }
            }
            skillGo.transform.Find("PetSkillItem01").gameObject.SetActive(hasSkill);
        }

        private void SetNextSkill(GameObject skillGo, uint skillId)
        {
            bool hasSkill = skillId > 0;
            var levelText = skillGo.transform.Find("Text_Level").GetComponent<Text>();
            if (hasSkill)
            {
                if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
                {
                    CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                    if (skillInfo != null)
                    {
                        TextHelper.SetText(skillGo.transform.Find("PetSkillItem01/Text_Name").GetComponent<Text>(), skillInfo.name);
                        TextHelper.SetText(levelText, 680003020, skillInfo.level.ToString());
                        ImageHelper.SetIcon(skillGo.transform.Find("PetSkillItem01/Image_Skill").GetComponent<Image>(), skillInfo.icon);
                        ImageHelper.GetPetSkillQuality_Frame(skillGo.transform.Find("PetSkillItem01/Image_Quality").GetComponent<Image>(), (int)skillInfo.quality);
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", skillId);
                    }
                }
                else
                {
                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                    if (skillInfo != null)
                    {
                        TextHelper.SetText(skillGo.transform.Find("PetSkillItem01/Text_Name").GetComponent<Text>(), skillInfo.name);
                        TextHelper.SetText(levelText, 680003020, skillInfo.level.ToString());
                        ImageHelper.SetIcon(skillGo.transform.Find("PetSkillItem01/Image_Skill").GetComponent<Image>(), skillInfo.icon);
                        ImageHelper.GetPetSkillQuality_Frame(skillGo.transform.Find("PetSkillItem01/Image_Quality").GetComponent<Image>(), (int)skillInfo.quality);
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                    }
                }
                var unlockGo = skillGo.transform.Find("Lock").gameObject;
                var levelGroundImageGo = skillGo.transform.Find("Image_Level").gameObject;
                levelGroundImageGo?.SetActive(true);
                unlockGo?.SetActive(false);
                skillGo.SetActive(true);
            }
            else
            {
                skillGo.SetActive(false);
            }
        }

        private void Skill1BtnClicked()
        {
            uint skill = sphereTemp.SkillIds[0];
            if (skill > 0)
                UIManager.OpenUI(EUIID.UI_Pet_DemonSkill_Tips, false, new Tuple<uint, uint, uint>(sphereTemp.Type, sphereTemp.Index, 1));
            //UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skill, 0));
        }

        private void Skill2BtnClicked()
        {
            uint skill = sphereTemp.SkillIds[1];
            if (skill > 0)
                UIManager.OpenUI(EUIID.UI_Pet_DemonSkill_Tips, false, new Tuple<uint, uint, uint>(sphereTemp.Type, sphereTemp.Index, 2));
            //UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skill, 0));
        }

        private void Skill1PreviewBtnClicked()
        {
            uint skill = sphereTemp.SkillIds[0] + 1;
            if (sphereTemp.SkillIds[0] > 0)
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skill, 0));
        }

        private void Skill2PreviewBtnClicked()
        {
            uint skill = sphereTemp.SkillIds[1] + 1;
            if (sphereTemp.SkillIds[1] > 0)
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skill, 0));
        }
    }

    public class UI_Pet_DemonUpgrade : UIBase, UI_Pet_DemonUpgrade_Layout.IListener
    {
        private UI_Pet_DemonUpgrade_Layout layout = new UI_Pet_DemonUpgrade_Layout();
        private UI_Pet_DemonUpgradeTop demonUpgradeTop;
        private UI_Pet_DemonUpgradeSkillView demonUpgradeSkillView;
        private PetSoulBeadInfo petSoulBeadInfo;
        private int selectType = 1; // 1道具  2宠物
        private uint petUid = 0; // type = 2 时

        private int scoreTipsLimit = -1;
        public int ScoreTipsLimit
        {
            get
            {
                if(scoreTipsLimit == -1)
                {
                    CSVPetNewParam.Data param = CSVPetNewParam.Instance.GetConfData(86u);
                    if (null != param)
                    {
                        scoreTipsLimit = (int)param.value;
                    }
                }
                return scoreTipsLimit;
            }
        }
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            demonUpgradeTop = AddComponent<UI_Pet_DemonUpgradeTop>(layout.topTransform);
            demonUpgradeSkillView = AddComponent<UI_Pet_DemonUpgradeSkillView>(layout.middleTransform);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnOwnDemonSpiritPetSelect, OnSelectPet, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnDemonSpiritUpgrade, OnDemonSpiritUpgrade, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnSaveOrDelectDemonSpiritSphereSkill, RefreshSphereInfo, toRegister);
        }
        
        protected override void OnOpen(object arg = null)
        {
            petSoulBeadInfo = arg as PetSoulBeadInfo;
        }

        protected override void OnShow()
        {
            timer?.Cancel();
            layout.expAddFx.SetActive(false);
            RefreshView();
        }
        Timer timer;

        private void OnDemonSpiritUpgrade(uint crit)
        {
            uint critP = crit / 10000;
            if (critP > 1)
            {
                foreach (var item in layout.hitGos)
                {
                    item.Value.SetActive(item.Key == (EDemonSpiritHitFx)critP);
                }
                layout.ani.Play("Fx", -1, 0f);
            }
            timer?.Cancel();
            layout.expAddFx.SetActive(true);
            timer = Timer.Register(0.8f, () =>
            {
                if(null != layout && null != layout.expAddFx)
                    layout.expAddFx.SetActive(false);
            });

            petUid = 0;
            RefreshSphereInfo();
        }

        public void RefreshSphereInfo()
        {
            if (null != petSoulBeadInfo)
            {
                petSoulBeadInfo = Sys_Pet.Instance.GetPetDemonSpiritSphereInfo(petSoulBeadInfo.Type, petSoulBeadInfo.Index);
                if (selectType == 2)
                {
                    SetPetItemCostView();
                }
                else if (selectType == 1)
                {
                    SetItemCostView();
                }
                var souldBeadData = CSVSoulBead.Instance.GetConfData(petSoulBeadInfo.Type * 10000 + petSoulBeadInfo.Level + 1);
                bool isMax = souldBeadData == null;
                demonUpgradeTop.SetView(petSoulBeadInfo, selectType, petUid);
                layout.upgradeGo.SetActive(!isMax);
                layout.fullGo.SetActive(isMax);
                demonUpgradeSkillView.SetView(petSoulBeadInfo);
            }
        }

        private void RefreshView()
        {
            if(null != petSoulBeadInfo)
            {
                var souldBeadData = CSVSoulBead.Instance.GetConfData(petSoulBeadInfo.Type * 10000 + petSoulBeadInfo.Level + 1);
                bool isMax = souldBeadData == null;
                if (!isMax)
                {
                    layout.selectToggle.SwitchTo(selectType - 1);
                }
                else
                {
                    demonUpgradeTop.SetView(petSoulBeadInfo, selectType, petUid);
                }
                layout.upgradeGo.SetActive(!isMax);
                layout.fullGo.SetActive(isMax);
                demonUpgradeSkillView.SetView(petSoulBeadInfo);
            }
        }

        private void SetItemCostView()
        {
            if (null != petSoulBeadInfo)
            {
                uint level = petSoulBeadInfo.Level;
                var sphereId = petSoulBeadInfo.Type * 10000 + level;
                var sphereData = CSVSoulBead.Instance.GetConfData(sphereId);
                if(null != sphereData)
                {
                    if(null != sphereData.add_exp && sphereData.add_exp.Count >= 3)
                    {
                        TextHelper.SetText(layout.addExpText, 680003041, sphereData.add_exp[0].ToString());
                        TextHelper.SetText(layout.hitText, 680003042, (sphereData.add_exp[1] / 100f).ToString("0"), (sphereData.add_exp[2] / 10000f).ToString("0"));
                        layout.addExpText.gameObject.SetActive(true);
                        layout.weekTimesText.gameObject.SetActive(false);
                    }

                    if (null != sphereData.item_cost)
                    {
                        var count = sphereData.item_cost.Count;
                        FrameworkTool.CreateChildList(layout.costPropItemParent, count);

                        for (int i = 0; i < count; i++)
                        {
                            var sub = layout.costPropItemParent.GetChild(i);
                            var costPropItem = new PropItem();
                            costPropItem.BindGameObject(sub.gameObject);
                            if (null != sphereData.item_cost[i] && sphereData.item_cost[i].Count >= 2)
                            {
                                var itemId = sphereData.item_cost[i][0];
                                var itemNum = sphereData.item_cost[i][1];

                                PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemId, itemNum, true, false, false, false, false, true, true, true);
                                costPropItem.SetData(itemN, EUIID.UI_Pet_Demon);
                                costPropItem.btnNone.gameObject.SetActive(false);
                                costPropItem.Layout.imgIcon.enabled = true;
                            }
                        }
                    }
                }
            }
            layout.costPropItemParent.gameObject.SetActive(true);
            layout.costPetPropItem.transform.parent.gameObject.SetActive(false);
        }

        private void SetPetItemCostView()
        {
            if (null != petSoulBeadInfo)
            {
                uint level = petSoulBeadInfo.Level;
                var sphereId = petSoulBeadInfo.Type * 10000 + level;
                var sphereData = CSVSoulBead.Instance.GetConfData(sphereId);
                var langId = 680002033u;
                if(sphereData.pet_type == 1)
                {
                    langId = 680002035u;
                }
                else if(sphereData.pet_type == 2)
                {
                    langId = 680002034u;
                }
                TextHelper.SetText(layout.pointTipsText, 680002032, sphereData.pet_score.ToString(), LanguageHelper.GetTextContent(langId));
            }
            if (petUid == 0)
            {
                layout.addExpText.gameObject.SetActive(false);
                PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
                layout.costPetPropItem.SetData(itemN, EUIID.UI_Pet_Demon);
                layout.costPetPropItem.txtNumber.gameObject.SetActive(false);
                layout.costPetPropItem.btnNone.gameObject.SetActive(true);
                layout.costPetPropItem.Layout.imgIcon.enabled = false;
                layout.costPetPropItem.Layout.imgQuality.gameObject.SetActive(false);
            }
            else
            {
                var cp = Sys_Pet.Instance.GetPetByUId(petUid);
                if (null != cp)
                {
                    var addId = Sys_Pet.Instance.GetAddDemonSpiritExpId(cp.petUnit.SimpleInfo.Score);
                    var petCostAddExp = CSVSoulBeadPetAddexp.Instance.GetConfData(addId);
                    if (null != petCostAddExp)
                    {
                        //if(null != petCostAddExp.multiplier[0] && null != petCostAddExp.multiplier[1] && petCostAddExp.multiplier[0].Count >=2 && petCostAddExp.multiplier[1].Count >= 2)
                        TextHelper.SetText(layout.addExpText, 680003041, petCostAddExp.base_exp.ToString());
                        //TextHelper.SetText(layout.hit3AddExpText, 680002031, (petCostAddExp.multiplier[0][0] / 100f).ToString("0"), (petCostAddExp.multiplier[0][1] / 10000f).ToString("0"), (petCostAddExp.multiplier[1][0] / 100f).ToString("0"), (petCostAddExp.multiplier[1][1] / 10000f).ToString("0"));
                    }

                    //宠物id 同道具id
                    PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(cp.petData.id, 0, true, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
                    layout.costPetPropItem.SetData(itemN, EUIID.UI_Pet_Demon);
                    layout.costPetPropItem.btnNone.gameObject.SetActive(false);
                    layout.costPetPropItem.Layout.imgIcon.enabled = true;
                }
                layout.addExpText.gameObject.SetActive(true);
            }
            if (null != Sys_Pet.Instance.petUpgradeBeadInfo)
            {
                /*CSVPetNewParam.Data hit3 = CSVPetNewParam.Instance.GetConfData(90u);
                if (null != hit3)
                {
                    var data = ReadHelper.ReadArray_ReadUInt(hit3.str_value, '|');
                    if (null != data && data.Count >= 1)
                    {
                        TextHelper.SetText(layout.hit3AddExpText, 680002059, (data[0] / 10000f).ToString("0"), Sys_Pet.Instance.petUpgradeBeadInfo.Rate3LeftCount.ToString());
                    }
                }*/
                CSVPetNewParam.Data hit2 = CSVPetNewParam.Instance.GetConfData(91u);
                if (null != hit2)
                {
                    var data = ReadHelper.ReadArray_ReadUInt(hit2.str_value, '|');
                    if (null != data && data.Count >= 1)
                    {
                        TextHelper.SetText(layout.hitText, 680002059, (data[0] / 10000f).ToString("0"), Sys_Pet.Instance.petUpgradeBeadInfo.Rate2LeftCount.ToString());
                    }
                }
                TextHelper.SetText(layout.weekTimesText, 680002061, Sys_Pet.Instance.petUpgradeBeadInfo.LeftCount.ToString());
            }
            layout.weekTimesText.gameObject.SetActive(true);
            layout.costPropItemParent.gameObject.SetActive(false);
            layout.costPetPropItem.transform.parent.gameObject.SetActive(true);
        }

        private void OnSelectPet(uint petUid)
        {
            this.petUid = petUid;
            demonUpgradeTop.SetView(petSoulBeadInfo, selectType, petUid);
            SetPetItemCostView();
        }

        protected override void OnHide()
        {
            timer?.Cancel();
        }

        protected override void OnDestroy()
        {
        }

        private void ItemGridBeClicked(PropItem bulidItem)
        {
            if (null != petSoulBeadInfo)
            {
                UIManager.OpenUI(EUIID.UI_Pet_DemonPet, false,
                    new UI_Pet_DemonParam
                    {
                        type = 3,
                        tuple = petSoulBeadInfo.Type * 10000 + petSoulBeadInfo.Level,
                    });
            }
            
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_DemonUpgrade);
        }

        public void PreViewBtnClicked()
        {
            if (null != petSoulBeadInfo)
            {
                UIManager.OpenUI(EUIID.UI_Pet_Demon_SkillPreview, false, new Tuple<uint, uint>(petSoulBeadInfo.Type, 0));
            }
            
        }

        public void UpExpBtnClicked()
        {
            layout.expAddFx.SetActive(false);
            if (null != petSoulBeadInfo)
            {
                uint level = petSoulBeadInfo.Level;
                var sphereId = petSoulBeadInfo.Type * 10000 + level;
                var sphereData = CSVSoulBead.Instance.GetConfData(sphereId);
                bool isMax = null == CSVSoulBead.Instance.GetConfData(sphereId + 1);
                if(isMax)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002013));
                    return;
                }
                if (selectType == 1)
                {
                    if(null != sphereData && null != sphereData.item_cost)
                    {
                        for (int i = 0; i < sphereData.item_cost.Count; i++)
                        {
                            if(sphereData.item_cost[i].Count >= 2)
                            {
                                ItemIdCount itemIdCount = new ItemIdCount(sphereData.item_cost[i][0], sphereData.item_cost[i][1]);
                                if(!itemIdCount.Enough)
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002014));
                                    return;
                                }
                            }
                           
                        }
                    }
                    Sys_Pet.Instance.PetSoulBeadOperateReq((uint)PetSoulBeadOpType.SoulBeadOpTypeLevelUpItem, petSoulBeadInfo.Index, petSoulBeadInfo.Type, 0);
                }
                else if(selectType == 2)
                {
                    if (petUid == 0)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002017));
                        return;
                    }
                    else if (null != Sys_Pet.Instance.petUpgradeBeadInfo && Sys_Pet.Instance.petUpgradeBeadInfo.LeftCount <= 0)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002056));
                        return;
                    }

                    var cp = Sys_Pet.Instance.GetPetByUId(petUid);
                    if (null != cp)
                    {
                        if (cp.petUnit != null && cp.petUnit.Islocked)
                        {
                            PromptBoxParameter.Instance.Clear();
                            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, cp.GetPetNmae(), cp.GetPetNmae());
                            PromptBoxParameter.Instance.SetConfirm(true, () =>
                            {
                                Sys_Pet.Instance.OnPetLockReq(cp.GetPetUid(), false);
                            });
                            PromptBoxParameter.Instance.SetCancel(true, null);
                            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                            return;
                        }
                        
                        var point = cp.petUnit.SimpleInfo.Score;
                        if(point > ScoreTipsLimit)
                        {
                            PromptBoxParameter.Instance.Clear();
                            PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
                            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680002018, cp.GetPetNmae(), point.ToString());
                            PromptBoxParameter.Instance.SetConfirm(true, () =>
                            {
                                if(null != cp)
                                {
                                    Sys_Pet.Instance.PetSoulBeadOperateReq((uint)PetSoulBeadOpType.SoulBeadOpTypeLevelUpPet, petSoulBeadInfo.Index, petSoulBeadInfo.Type, petUid);
                                }
                            });
                            PromptBoxParameter.Instance.SetCancel(true, null);
                            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                        }
                        else
                        {
                            if (null != cp)
                            {
                                Sys_Pet.Instance.PetSoulBeadOperateReq((uint)PetSoulBeadOpType.SoulBeadOpTypeLevelUpPet, petSoulBeadInfo.Index, petSoulBeadInfo.Type, petUid);
                            }
                        }
                    }
                }
            }
        }

        public void OnSelectTypeChange(int curToggle, int old)
        {
            //if (curToggle == old)
            //    return;

            selectType = curToggle + 1;

            demonUpgradeTop.SetView(petSoulBeadInfo, selectType, petUid);
            if(selectType == 1)
            {
                this.petUid = 0;
                SetItemCostView();
            }
            else if(selectType == 2)
            {
                SetPetItemCostView();
            }
        }
    }
}