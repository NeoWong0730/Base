using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Framework;
using UnityEngine.EventSystems;
using Packet;

namespace Logic
{
    /// <summary>
    /// 水风火土
    /// </summary>
    public enum EFamilyCreaturesType
    {
        Water = 1,
        Wind = 2,
        Fire = 3,
        Earth = 4
    }

    public class UI_FamilyCreatures_Layout
    {
        private Button closeBtn;
        private Button priviewBtn;
        private Button noticeBtn;
        private GameObject noticeRedPointGo;
        private Transform leftTopTran;
        private Transform rightTran;
        private Transform iconsTran;
        public Transform LeftTopTran { get => leftTopTran;}
        public Transform RightTran { get => rightTran; }
        public Transform IconsTran { get => iconsTran;}

        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/View_Title03/Btn_Close").GetComponent<Button>();
            priviewBtn = transform.Find("Animator/Btn_Form").GetComponent<Button>();
            noticeBtn = transform.Find("Animator/Btn_Notice").GetComponent<Button>();
            leftTopTran = transform.Find("Animator/Image_Title");
            rightTran = transform.Find("Animator/View_Right");
            iconsTran = transform.Find("Animator/Toggle_Group");
            noticeRedPointGo = transform.Find("Animator/Btn_Notice/Image_Prompt").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            priviewBtn.onClick.AddListener(listener.PriviewBtnClicked);
            noticeBtn.onClick.AddListener(listener.NotoceBtnClicked);
        }

        public void SetNoticeRedPoint()
        {
            noticeRedPointGo.SetActive(Sys_Family.Instance.IsShowNoticeRedPoint());
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void PriviewBtnClicked();
            void NotoceBtnClicked();
        }
    }

    public class UI_FamilyCreatureIconController : UIComponent, UI_FamilyCreatureIconController.UI_FamilyCreatureIconItem.IListener
    {
        public class UI_FamilyCreatureIconItem : UIComponent
        {
            public interface IListener
            {
                void OnFamilyCreatureIconClicked(int index, TrList trList);
            }
            private int index;
            private GameObject lineObj;
            private GameObject selectObj;
            private GameObject lockObj;
            private GameObject fullGo;
            private GameObject trainGo;
            private Text nameText;
            private Action<UI_FamilyCreatureIconItem> action;
            private Image iconImg;
            private Button addBtn;
            public TrList trList;
            private FamilyCreatureEntry entry;
            private IListener listener;
            private bool HasCreature
            {
                get
                {
                    return entry != null;
                }
            }
            protected override void Loaded()
            {
                trList = transform.GetComponent<TrList>();
                lineObj = transform.Find("Image_Line").gameObject;
                selectObj = transform.Find("Image_Select").gameObject;
                lockObj = transform.Find("Image_Lock").gameObject;
                fullGo = transform.Find("Image_Full").gameObject;
                trainGo = transform.Find("Image_Train").gameObject;
                iconImg = transform.Find("Image_Icon").GetComponent<Image>();
                nameText = transform.Find("Text_Name").GetComponent<Text>();
                transform.GetComponent<Button>().onClick.AddListener(OnClicked);
                addBtn = transform.Find("Btn_Plus").GetComponent<Button>();
                addBtn.onClick.AddListener(OnAddBtnClicked);
            }

            public UI_FamilyCreatureIconItem(IListener listener)
            {
                this.listener = listener;
            }

            public void  SetLineState(bool state)
            {
                lineObj.SetActive(state);
            }

            private void OnAddBtnClicked()
            {
                bool hasEditor = Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.FamilyPetEgg);
                UIManager.OpenUI(EUIID.UI_FamilyCreatures_Get, false, new Tuple<uint, object>(0, 0));
            }

            public void SetItemInfo(int index, FamilyCreatureEntry entry)
            {
                this.index = index;
                this.entry = entry;
            }

            public void SetSelectInfo(int index)
            {
                bool showSelect = index == this.index;
                selectObj.SetActive(showSelect);
            }

            public void Reset(bool ShowName)
            {
                bool hasNextCeil = Sys_Family.Instance.GetWillLockFamilyCreatureByIndex(index);
                lockObj.SetActive(!HasCreature && !hasNextCeil);
                addBtn.gameObject.SetActive(!HasCreature && hasNextCeil);
                nameText.gameObject.SetActive(HasCreature && ShowName);
                iconImg.gameObject.SetActive(HasCreature);
                bool isShowTrain = HasCreature && entry.IsTrain && Sys_Family.Instance.ShowTrainInfo();
                fullGo.SetActive(HasCreature && entry.IsFull && !isShowTrain);
                trainGo.SetActive(HasCreature && isShowTrain);
                if (HasCreature)
                {
                    ImageHelper.SetIcon(iconImg, entry.cSV.icon_id);
                    nameText.text = LanguageHelper.GetTextContent(Sys_Family.Instance.GetNameLangIdByType(entry.cSV.food_Type));
                }
            }

            private void OnClicked()
            {
                if(HasCreature)
                    this.listener?.OnFamilyCreatureIconClicked(this.index, trList);
            }
        }

        private List<UI_FamilyCreatureIconItem> iconList = new List<UI_FamilyCreatureIconItem>(4);

        public IListener listener;

        public TrListRegistry registry;

        private List<FamilyCreatureEntry> familyCreatures;

        public UI_FamilyCreatureIconController(IListener listener)
        {
            this.listener = listener;
        }

        protected override void Loaded()
        {
            registry = transform.GetComponent<TrListRegistry>();

            FrameworkTool.CreateChildList(transform, Enum.GetValues(typeof(EFamilyCreaturesType)).Length);
            for (int i = 0; i < transform.childCount; i++)
            {
                UI_FamilyCreatureIconItem creatureIcon = new UI_FamilyCreatureIconItem(this);
                creatureIcon.Init(transform.GetChild(i));
                creatureIcon.SetLineState(i != 0);
                iconList.Add(creatureIcon);
            }
        }

        public void SetListInfo(int index, bool ShowName)
        { 
            familyCreatures = Sys_Family.Instance.familyCreatureEntries;
            if (null != familyCreatures && familyCreatures.Count <= iconList.Count)
            {
                for (int i = 0; i < iconList.Count; i++)
                {
                    UI_FamilyCreatureIconItem creatureIcon = iconList[i];
                    bool noCreature = i > familyCreatures.Count - 1;
                    if(noCreature)
                    {
                        //锁住的格子
                        creatureIcon.SetItemInfo(i, null);
                        creatureIcon.Reset(ShowName);
                    }
                    else
                    {
                        creatureIcon.SetItemInfo(i, familyCreatures[i]);
                        creatureIcon.Reset(ShowName);
                        //有服务器数据
                    }
                    creatureIcon.SetSelectInfo(index);
                }
            }
        }

        public void OnFamilyCreatureIconClicked(int index, TrList trList)
        {
            registry.SwitchTo(trList, false);
            listener?.OnSelectListIndex(index);
        }

        public interface IListener
        {
            void OnSelectListIndex(int index);
        }
    }

    public class UI_FamilyCreatureSlider
    {
        public Transform transform;
        public Slider slider;
        public Text sliderText;
        public virtual void InitTransform(Transform transform)
        {
            this.transform = transform;
            slider = transform.Find("Slider").GetComponent<Slider>();
            sliderText = transform.Find("Slider/Text_Value")?.GetComponent<Text>();
        }

        public virtual void SetOtherInfo()
        {

        }

        public virtual void SetSliderValue(uint currentValue, uint maxValue)
        {
            if(maxValue != 0)
            {
                slider.value = (currentValue + 0f) / maxValue;
                if (null != sliderText)
                {
                    sliderText.text = string.Format("{0}/{1}", currentValue.ToString(), maxValue.ToString());
                }
            }
            else
            {
                slider.value = 1;
                sliderText.text = "";
            }
            
        }

        public virtual void OnDetailBtnClicked()
        {

        }
    }

    public class UI_FamilyCreatureHealthSlider : UI_FamilyCreatureSlider
    {
        public GameObject Line1;
        public Image healthImage;
        public Button healthImageBtn;
        public Button descBtn;

        public Text healthDescText;
        List<CSVFamilyPetHealth.Data>  vs = new List<CSVFamilyPetHealth.Data>(2);
        public override void InitTransform(Transform transform)
        {
            base.InitTransform(transform);
            Line1 = transform.Find("Slider/Image (1)").gameObject;
            healthImage = transform.Find("Slider/Handle").GetComponent<Image>();
            healthImageBtn = healthImage.GetNeedComponent<Button>();
            descBtn = transform.Find("Btn_Details").GetComponent<Button>();
            healthDescText = transform.Find("Text_Tips")?.GetComponent<Text>();

            var dataList = CSVFamilyPetHealth.Instance.GetAll();
            for (int i = 0, len = dataList.Count; i < len; i++)
            {
                CSVFamilyPetHealth.Data cSVFamilyPetHealthData = dataList[i];
                if (cSVFamilyPetHealthData != null && cSVFamilyPetHealthData.Range.Count >= 2)
                {
                    vs.Add(cSVFamilyPetHealthData);
                }
            }
            vs.Sort((a,b)=>
                {
                    return (int)a.Range[1] - (int)b.Range[1];
                });
            if (vs.Count >= 2)
            {
                float b1 = vs[0].Range[1] / (vs[1].Range[1] + 0f);
                var allWidth = slider.GetComponent<RectTransform>().rect.width;
                Line1.transform.localPosition = new Vector3(allWidth * b1 - allWidth / 2.0f, Line1.transform.localPosition.y, Line1.transform.localPosition.z);
            }
            healthImageBtn.onClick.AddListener(HandIconClicke);
            descBtn.onClick.AddListener(OnDetailBtnClicked);
        }

        int index;
        FamilyCreatureEntry entry;
        public void SetSliderValue(FamilyCreatureEntry entry, uint maxValue)
        {
            this.entry = entry;
            uint currentValue = entry.creature.Health;
            if (vs.Count >= 2)
            {
                index = 0;
                for (int i = 0; i < vs.Count; i++)
                {
                    if(currentValue <= vs[i].Range[1])
                    {
                        index = i;
                        break;
                    }
                }
                if (null != healthDescText)
                {
                    TextHelper.SetText(healthDescText, vs[index].trainingTips);
                }
                ImageHelper.SetIcon(healthImage, vs[index].Icon);
                slider.value = (currentValue + 0f) / vs[1].Range[1];
                sliderText.text = string.Format("{0}/{1}", currentValue.ToString(), vs[1].Range[1].ToString());
            }
        }

        private void HandIconClicke()
        {
            if (vs.Count >= 2)
            {
                if(null != entry)
                {
                    UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(vs[index].Des, entry.Name, (vs[index].addGrowthRatio / 100.0f).ToString("#.0")) });
                }
               
            }
        }

        public override void OnDetailBtnClicked()
        {
            if (vs.Count >= 2)
            {
                UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2023376, (vs[0].addGrowthRatio / 100.0f).ToString("#.0"), (vs[1].addGrowthRatio / 100.0f).ToString("#.0"))});
            }
        }
    }

    public class UI_FamilyCreatureMoodSlider : UI_FamilyCreatureSlider
    {
        public GameObject Line1;
        public GameObject Line2;
        public Image moodImage;
        public Button moodImageBtn;
        public Button descBtn;
        public Text moodDescText;
        List<CSVFamilyPetMood.Data>  vs = new List<CSVFamilyPetMood.Data>(3);
        public override void InitTransform(Transform transform)
        {
            base.InitTransform(transform);
            Line1 = transform.Find("Slider/Image (1)").gameObject;
            Line2 = transform.Find("Slider/Image (2)").gameObject;
            moodDescText = transform.Find("Text_Tips")?.GetComponent<Text>();
            moodImage = transform.Find("Slider/Handle").GetComponent<Image>();
            moodImageBtn = moodImage.GetNeedComponent<Button>();
            descBtn = transform.Find("Btn_Details").GetComponent<Button>();

            var dataList = CSVFamilyPetMood.Instance.GetAll();
            for (int i = 0, len = dataList.Count; i < len; i++)
            {
                CSVFamilyPetMood.Data cSVFamilyPetMoodData = dataList[i];
                if (cSVFamilyPetMoodData.Range != null && cSVFamilyPetMoodData.Range.Count >= 2)
                {
                    vs.Add(cSVFamilyPetMoodData);
                }
            }
            vs.Sort((a, b) =>
            {
                return (int)a.Range[1] - (int)b.Range[1];
            });
            if (vs.Count >= 3)
            {
                float b1 = vs[0].Range[1] / (vs[2].Range[1] + 0f);
                float b2 = vs[1].Range[1] / (vs[2].Range[1] + 0f);
                var allWidth = slider.GetComponent<RectTransform>().rect.width;
                Line1.transform.localPosition = new Vector3(allWidth * b1 - allWidth / 2.0f, Line1.transform.localPosition.y, Line1.transform.localPosition.z);
                Line2.transform.localPosition = new Vector3(allWidth * b2 - allWidth / 2.0f, Line2.transform.localPosition.y, Line2.transform.localPosition.z);
            }
            moodImageBtn.onClick.AddListener(HandIconClicke);
            descBtn.onClick.AddListener(OnDetailBtnClicked);
        }

        int index;
        FamilyCreatureEntry entry;

        public void SetSliderValue(FamilyCreatureEntry entry, uint maxValue)
        {
            this.entry = entry;
            uint currentValue = entry.creature.Mood;
            if (vs.Count >= 3)
            {
                index = 0;
                for (int i = 0; i < vs.Count; i++)
                {
                    if (currentValue <= vs[i].Range[1])
                    {
                        index = i;
                        break;
                    }
                }
                if(null != moodDescText)
                {
                    TextHelper.SetText(moodDescText, vs[index].trainingTips);
                }
                ImageHelper.SetIcon(moodImage, vs[index].Icon);
                slider.value = (currentValue + 0f) / vs[2].Range[1];
                sliderText.text = string.Format("{0}/{1}", currentValue.ToString(), vs[2].Range[1].ToString());
            }
        }

        private void HandIconClicke()
        {
            if (vs.Count >= 3)
            {
                if(null != entry)
                {
                    
                }
                UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(vs[index].Des, entry.Name, (vs[index].addGrowthRatio / 100.0f).ToString("#.0")) });
            }
        }

        public override void OnDetailBtnClicked()
        {
            if (vs.Count >= 3)
            {
                UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2023375, (vs[0].addGrowthRatio / 100.0f).ToString("#.0"), (vs[1].addGrowthRatio / 100.0f).ToString("#.0"), (vs[2].addGrowthRatio / 100.0f).ToString("#.0")) });
            }
        }
    }

    public class UI_FamilyCreature_RightView : UIComponent
    {
        private Text nameText;
        private Text stateText;
        private Text oldText;
        private Text typeText;
        private Text trainBtnText;
        private Text trainTipsText;
        private Text trainTimeText;
        private Transform hobbyTransform;
        private Button reNameBtn;
        private Button setTrainBtn;
        private Button goBtn;
        private UI_FamilyCreatureSlider totalSlider;
        private UI_FamilyCreatureMoodSlider moodSlider;
        private UI_FamilyCreatureHealthSlider healthSlider;
        private FamilyCreatureEntry familyCreatureEntry;
        //喂食次数
        private Text residueDegreeText;
        protected override void Loaded()
        {
            nameText = transform.Find("View_Info/Text_Name").GetComponent<Text>();
            stateText = transform.Find("View_Info/Image_Condition/Text_Level").GetComponent<Text>();
            oldText = transform.Find("View_Info/Image_birthday/Text_Level").GetComponent<Text>();
            typeText = transform.Find("View_Total/Text_Name").GetComponent<Text>();

            trainBtnText = transform.Find("Btn_01/Text_01").GetComponent<Text>();
            trainTipsText = transform.Find("Text_Tips").GetComponent<Text>();
            
            reNameBtn = transform.Find("View_Info/Btn_Rename").GetComponent<Button>();
            reNameBtn.onClick.AddListener(OnReNameBtnClicked);

            setTrainBtn = transform.Find("Btn_01").GetComponent<Button>();
            setTrainBtn.onClick.AddListener(OnSetTrainClicked);

            goBtn = transform.Find("Btn_02").GetComponent<Button>();
            goBtn.onClick.AddListener(OnGoBtnClicked);

            hobbyTransform = transform.Find("View_Hobby/Item_Grid");

            residueDegreeText = transform.Find("FeedingTimes/Text_Value").GetComponent<Text>();
            trainTimeText = transform.Find("TrainingTime/Text_Value").GetComponent<Text>();
            moodSlider = new UI_FamilyCreatureMoodSlider();
            moodSlider.InitTransform(transform.Find("View_Condition/Mood"));

            healthSlider = new UI_FamilyCreatureHealthSlider();
            healthSlider.InitTransform(transform.Find("View_Condition/Health"));

            totalSlider = new UI_FamilyCreatureSlider();
            totalSlider.InitTransform(transform.Find("View_Total"));
        }

        public void SetInfo(FamilyCreatureEntry familyCreatureEntry)
        {
            bool hasEditor = Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.FamilyPetName);
            reNameBtn.gameObject.SetActive(hasEditor);
            this.familyCreatureEntry = familyCreatureEntry;
            nameText.text = familyCreatureEntry.Name;
            stateText.text = Sys_Family.Instance.CreatureStateStr();
            oldText.text = familyCreatureEntry.GetCreatureOld();
            typeText.text = LanguageHelper.GetTextContent(Sys_Family.Instance.CreatureState(familyCreatureEntry.cSV.stage));
            moodSlider.SetSliderValue(familyCreatureEntry, 0);
            healthSlider.SetSliderValue(familyCreatureEntry, 0);
            totalSlider.SetSliderValue(familyCreatureEntry.creature.Growth, familyCreatureEntry.cSV.growthValueMax);
            RefreshLikes();

            //开放日
            bool isOpenDate = Sys_Family.Instance.IsFamilyCreaturesOpenDate();
            //开放日自然日判断
            bool isOpenDate2 = Sys_Family.Instance.IsFamilyCreaturesOpenDate(1);
            //是否设置过
            bool isSet = true;
            GuildPetTraining guildPetTraining = Sys_Family.Instance.GetTrainInfo();
            if (null != guildPetTraining)
            {
                isSet = guildPetTraining.BSet;
            }
            //是否训练中或者准备中
            bool isTrainOrReady = Sys_Family.Instance.IsOnReadyTrainTime();
            bool isTrain = Sys_Family.Instance.IsInTrainTime();
            ///持续时间
            Sys_Ini.Instance.Get<IniElement_Int>(1264, out IniElement_Int durationTime);
            ///当前时间
            var currentTime = Sys_Time.Instance.GetServerTime();
            //当日的0点时间戳
            ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            var setTime = zeroTime + guildPetTraining.StartTime;
            bool showTip = !isOpenDate2 || (isOpenDate && ((setTime + (ulong)durationTime.value) < currentTime));

            setTrainBtn.gameObject.SetActive(!showTip);
            trainTipsText.gameObject.SetActive(showTip);
            trainTimeText.transform.parent.gameObject.SetActive(isOpenDate && isTrain);
            if (!showTip)
            {
                if(isOpenDate && isTrainOrReady)
                {
                    trainBtnText.text = LanguageHelper.GetTextContent(2023379);
                }
                else
                {
                    trainBtnText.text = LanguageHelper.GetTextContent((hasEditor && !isSet) ? 2023319u : 2023400u);
                }
                if (isOpenDate && isTrain)
                {
                    timer?.Cancel();
                    timer = Timer.Register(1, null, (t) =>
                    {
                        if (null != trainTimeText)
                        {
                            trainTimeText.text = Sys_Family.Instance.FamilyCreatureTrainTime();
                        }
                    }, true);
                }

            }
            else
            {
                /*Sys_Ini.Instance.Get<IniElement_Int>(1264, out IniElement_Int durationTime);
                ///当前时间
                var currentTime = Sys_Time.Instance.GetServerTime();
                //当日的0点时间戳
                ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
                var setTime = zeroTime + guildPetTraining.StartTime;
                if (isOpenDate && isSet && currentTime < setTime)
                {
                    uint Hour = guildPetTraining.StartTime / 3600;
                    uint Minus = guildPetTraining.StartTime % 3600 / 60;
                    trainTipsText.text = LanguageHelper.GetTextContent(2023368, Hour.ToString("D2"), Minus.ToString("D2"));
                }
                else */
                trainTipsText.text = LanguageHelper.GetTextContent(2023369);
            }
        }
        Timer timer;
       

        private void RefreshLikes()
        {
            FrameworkTool.CreateChildList(hobbyTransform, familyCreatureEntry.creature.LoveFoods.Count);
            for (int i = 0; i < hobbyTransform.childCount; i++)
            {
                PropItem item = new PropItem();
                item.BindGameObject(hobbyTransform.GetChild(i).gameObject);
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(familyCreatureEntry.creature.LoveFoods[i], 0, false, false, false, false, false, false, true);
                item.txtName.text =LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(familyCreatureEntry.creature.LoveFoods[i]).name_id);
                item.SetData(itemData, EUIID.UI_FamilyCreatures);
            }
        }

        public void RefreshFeedCout()
        {
            residueDegreeText.text = Sys_Family.Instance.feedCount.ToString();
        }

        public override void Hide()
        {
            timer?.Cancel();
        }

        private void OnReNameBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures, "OnReNameBtnClicked");
            UIManager.OpenUI(EUIID.UI_FamilyCreatures_Rename, false, familyCreatureEntry);
        }

        private void OnSetTrainClicked()
        {
            //开放日
            bool isOpenDate = Sys_Family.Instance.IsFamilyCreaturesOpenDate();
            //是否训练中
            bool isTrain = Sys_Family.Instance.IsOnReadyTrainTime();
            if (isOpenDate && isTrain)
            {
                Sys_Family.Instance.GotoTrainCreature();
            }
            else if(isOpenDate)
            {
                UIManager.HitButton(EUIID.UI_FamilyCreatures, "OnSetTrainClicked");
                UIManager.OpenUI(EUIID.UI_FamilyCreatures_SetTrain);
            }
        }

        private void OnGoBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures, "OnGoBtnClicked");
            if (null == familyCreatureEntry)
                return;
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(familyCreatureEntry.cSV.id);
            UIManager.CloseUI(EUIID.UI_Family);
            UIManager.CloseUI(EUIID.UI_FamilyCreatures);
        }
    }

    public class UI_FamilyCreature_LeftView : UIComponent
    {
        public Image creatureTypeImg;
        public Image creatureNameImg;
        public Text creatureTypeText;
        public GameObject HighestGo;
        protected override void Loaded()
        {
            creatureTypeImg = transform.Find("Icon").GetComponent<Image>();
            creatureNameImg = transform.Find("Name").GetComponent<Image>();
            creatureTypeText = transform.Find("Text").GetComponent<Text>();
            HighestGo = transform.Find("Highest").gameObject;

        }

        public void SetFamilyCreatureInfo(FamilyCreatureEntry entry, uint trainState = 0)
        {
            if (null != entry)
            {
                if(trainState != 0)
                {
                    CSVFamilyPet.Data cSV = CSVFamilyPet.Instance.GetConfData(trainState);
                    ImageHelper.SetIcon(creatureTypeImg, cSV.food_Type + 975000);
                    ImageHelper.SetIcon(creatureNameImg, cSV.food_Type + 975010);
                    TextHelper.SetText(creatureTypeText, LanguageHelper.GetTextContent(2023366, LanguageHelper.GetTextContent(Sys_Family.Instance.GetTypeName(cSV.food_Type)), LanguageHelper.GetTextContent(Sys_Family.Instance.CreatureState(cSV.stage))), GetTypeWorldStype(cSV.food_Type));
                    HighestGo.SetActive(cSV.familyPet_id == 0);
                }
                else
                {
                    ImageHelper.SetIcon(creatureTypeImg, entry.cSV.food_Type + 975000);
                    ImageHelper.SetIcon(creatureNameImg, entry.cSV.food_Type + 975010);
                    TextHelper.SetText(creatureTypeText, LanguageHelper.GetTextContent(2023366, LanguageHelper.GetTextContent(Sys_Family.Instance.GetTypeName(entry.cSV.food_Type)), LanguageHelper.GetTextContent(Sys_Family.Instance.CreatureState(entry.cSV.stage))), GetTypeWorldStype(entry.cSV.food_Type));
                    HighestGo.SetActive(entry.cSV.familyPet_id == 0);
                }
            }
        }

        private CSVWordStyle.Data GetTypeWorldStype(uint type)
        {
            uint typeID = 0;
            if (type == 1)
            {
                typeID = 110;
            }
            else if (type == 2)
            {
                typeID = 111;
            }
            else if (type == 3)
            {
                typeID = 112;
            }
            else if (type == 4)
            {
                typeID = 113;
            }
            return CSVWordStyle.Instance.GetConfData(typeID);
        }

    }

    public class UI_FamilyCreatureModelLoad 
    {
        //public Image eventImage;
        private CSVFamilyPet.Data cSV;
        //模型3渲2
        public Image eventImage;
        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        
        public void SetEventImage(Image image)
        {
            eventImage = image;
            if(null != eventImage)
            {
                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
                eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
            }
        }

        public void Hide()
        {
            cSV = null;
            UnloadModel();
        }

        void UpdateView()
        {
           
        }

        public void SetValue(CSVFamilyPet.Data familyPetData)
        {
            if (null != familyPetData)
            {
                if (cSV == null)
                {
                    cSV = familyPetData;
                    _LoadShowScene();
                    _LoadShowModel();
                }
                else
                {
                    uint currentId = cSV.id;
                    cSV = familyPetData;
                    if (currentId != cSV.id)
                    {
                        UnloadModel();
                        _LoadShowScene();
                        _LoadShowModel();
                    }
                }
            }
            UpdateView();
        }

        public void UnloadModel()
        {
            _UnloadShowContent();
        }

        private void _UnloadShowContent()
        {
            //petDisplay?.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl?.Dispose();
            modelGo = null;
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            sceneModel.transform.localPosition = new Vector3((int)(EUIID.UI_FamilyCreatures), 0, 0);
            showSceneControl.Parse(sceneModel);

            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }
        }

        private void _LoadShowModel()
        {
            string _modelPath = cSV.model_show;
            //player = GameCenter.modelShowWorld.CreateActor<ModelShowActor>(clientPet.petUnit.PetId);
            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);

            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            showSceneControl.mModelPos.transform.Rotate(new Vector3(cSV.rotationx / 10000.0f, cSV.rotationy / 10000.0f, cSV.rotationz / 10000.0f));
            showSceneControl.mModelPos.transform.localScale = new Vector3(cSV.scale / 10000.0f, cSV.scale / 10000.0f, cSV.scale / 10000.0f);
            showSceneControl.mModelPos.transform.localPosition = new Vector3(
                showSceneControl.mModelPos.transform.localPosition.x + cSV.positionx / 10000.0f,
                showSceneControl.mModelPos.transform.localPosition.y + cSV.positiony / 10000.0f,
                showSceneControl.mModelPos.transform.localPosition.z + cSV.positionz / 10000.0f);

        }

        public GameObject modelGo;
        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                modelGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
                modelGo.SetActive(false);
                petDisplay.mAnimation.UpdateHoldingAnimations(cSV.action_show_id, Constants.UMARMEDID, go: modelGo);
            }
        }

        public void OnDrag(BaseEventData eventData)
        {
            if (null != cSV)
            {
                PointerEventData ped = eventData as PointerEventData;
                Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
                AddEulerAngles(angle);
            }
        }

        public void AddEulerAngles(Vector3 angle)
        {
            Vector3 ilrTemoVector3 = angle;
            if (showSceneControl.mModelPos.transform != null)
            {
                showSceneControl.mModelPos.transform.Rotate(ilrTemoVector3.x, ilrTemoVector3.y, ilrTemoVector3.z);
            }
        }
    }

    public class UI_FamilyCreatures : UIBase, UI_FamilyCreatures_Layout.IListener, UI_FamilyCreatureIconController.IListener
    {
        private UI_FamilyCreatures_Layout layout = new UI_FamilyCreatures_Layout();
        private UI_FamilyCreature_LeftView leftTopView;
        private UI_FamilyCreature_RightView rightView;
        private UI_FamilyCreatureModelLoad modelLoad = new UI_FamilyCreatureModelLoad();
        private UI_FamilyCreatureIconController iconController;
        private int index = -1;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            leftTopView = AddComponent<UI_FamilyCreature_LeftView>(layout.LeftTopTran);
            rightView = AddComponent<UI_FamilyCreature_RightView>(layout.RightTran);
            iconController = new UI_FamilyCreatureIconController(this);
            iconController.Init(layout.IconsTran);
            modelLoad.SetEventImage(transform.Find("Animator/Texture").GetComponent<Image>());
            modelLoad.assetDependencies = transform.GetComponent<AssetDependencies>();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnGetFamilyPetInfo, Refresh, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnFamilyPetNotice, RefreshRedPoint, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnSetTrainInfoEnd, Refresh, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnGetFamilyPetFeedInfo, RefreshFeedCout, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnFamilyPetNoticeVerChange, RefreshRedPoint, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {

        }

        private void Refresh()
        {
            if (index == -1)
                index = 0;
            iconController.SetListInfo(index, false);
            RefreshByIndex();
            layout.SetNoticeRedPoint();
        }

        private void RefreshRedPoint()
        {
            layout.SetNoticeRedPoint();
        }

        protected override void OnShow()
        {
            Refresh();
            Sys_Family.Instance.CheckDifficultyNeedUpdate();
            Sys_Family.Instance.GuildPetGetFeedInfoReq();
            Sys_Family.Instance.GuildPetGetInfoReq();
        }

        protected override void OnHide()
        {
            modelLoad.Hide();
            rightView.Hide();
        }

        protected override void OnDestroy()
        {
        }

        public void CloseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures, "CloseBtnClicked");
            UIManager.CloseUI(EUIID.UI_FamilyCreatures);
        }

        protected override void OnClose()
        {
            index = -1;
        }

        public void OnSelectListIndex(int index)
        {
            this.index = index;
            RefreshByIndex();
        }

        private void RefreshFeedCout()
        {
            rightView.RefreshFeedCout();
        }

        private void RefreshByIndex()
        {
            FamilyCreatureEntry familyCreatureEntry = Sys_Family.Instance.GetFamilyCreatureByIndex(index);
            leftTopView.SetFamilyCreatureInfo(familyCreatureEntry);
            rightView.SetInfo(familyCreatureEntry);
            modelLoad.SetValue(familyCreatureEntry.cSV);
        }

        public void PriviewBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures, "PriviewBtnClicked");
            FamilyCreatureEntry familyCreatureEntry = Sys_Family.Instance.GetFamilyCreatureByIndex(index);
            UIManager.OpenUI(EUIID.UI_FamilyCreatures_Get, false, new Tuple<uint, object>(1, familyCreatureEntry.cSV.food_Type));
        }

        public void NotoceBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures, "NotoceBtnClicked");
            layout.SetNoticeRedPoint();
            UIManager.OpenUI(EUIID.UI_FamilyCreatures_Notice);
        }
    }
}