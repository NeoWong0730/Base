using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Framework;
using Packet;

namespace Logic
{
    public class UI_FamilyCreatures_Train_Layout
    {
        private Button closeBtn;
        private Button starRewardBtn;
        private Button pointPersonBtn;
        private Button rankBtn;
        private Transform leftTopTran;
        public Transform LeftTopTran { get => leftTopTran; }

        private Transform rightViewTran;
        public Transform RightViewTran { get => rightViewTran; }

        private Transform trainStar;
        public Transform TrainStar { get => trainStar; }

        public Text myPonitText;
        public void Init(Transform transform)
        {
            leftTopTran = transform.Find("Animator/Image_Title");
            rightViewTran = transform.Find("Animator/View_Right");
            trainStar = transform.Find("Animator/View_TrainStar");
            starRewardBtn = transform.Find("Animator/View_TrainStar/Btn_TreasureBox").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_Title03/Btn_Close").GetComponent<Button>();
            pointPersonBtn = transform.Find("Animator/View_Title03/Btn_Details").GetComponent<Button>();
            myPonitText = transform.Find("Animator/View_Title03/Text_Point_Person/Text_Value").GetComponent<Text>();
            rankBtn = transform.Find("Animator/Btn_Rank").GetComponent<Button>();

        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            starRewardBtn.onClick.AddListener(listener.PreviewStarRewardBtnClicked);
            pointPersonBtn.onClick.AddListener(listener.RuleBtnClick);
            rankBtn.onClick.AddListener(listener.RankBtnClick);
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void PreviewStarRewardBtnClicked();
            void RuleBtnClick();
            void RankBtnClick();
        }
    }

    public class UI_FamilyCreatureTrainStarSlider : UI_FamilyCreatureSlider
    {
        public List<GameObject> starGo = new List<GameObject>(4);
        private Transform starG;
        List<CSVFamilyPetTrainingStarReward.Data>  vs = new List<CSVFamilyPetTrainingStarReward.Data>(5);
        public Text allText;
        public Text nextText;
        private Transform starLiTran;
        public override void InitTransform(Transform transform)
        {
            base.InitTransform(transform);
            starG = transform.Find("Slider/StarG");
            allText = transform.Find("Text_Point/Text_Value").GetComponent<Text>();
            nextText = transform.Find("Text_PointGap").GetComponent<Text>();
            starLiTran = transform.Find("Slider/Image");
        }

        public override void SetOtherInfo()
        {
            GuildPetTraining guildPetTraining = Sys_Family.Instance.GetTrainInfo();
            
            if(null != guildPetTraining)
            {
                vs.Clear();
                for (int i = 1; i < 99; i++)
                {
                    CSVFamilyPetTrainingStarReward.Data cSVFamilyPetTrainingStarRewardData = CSVFamilyPetTrainingStarReward.Instance.GetConfData((uint)(guildPetTraining.TrainingStage * 10 + i));
                    if(null != cSVFamilyPetTrainingStarRewardData)
                    {
                        vs.Add(cSVFamilyPetTrainingStarRewardData);
                    }
                    else
                    {
                        break;
                    }
                }
                uint starL = Sys_Family.Instance.GetRankRewardStar();

                int vsCount = vs.Count;
                FrameworkTool.CreateChildList(starG, vsCount);
                starGo.Clear();
                for (int i = 0; i < vs.Count; i++)
                {
                    Transform starTran = starG.GetChild(i);
                    if (vs[i].trainingStar == 0)
                    {
                        starTran.gameObject.SetActive(false);
                    }
                    else
                    {
                        float b1 = vs[i].trainingIntegralCondition / (vs[vsCount - 1].trainingIntegralCondition + 0f);
                        var allWidth = slider.GetComponent<RectTransform>().rect.width;
                        starTran.localPosition = new Vector3(allWidth * b1 - allWidth / 2.0f, starTran.localPosition.y, starTran.localPosition.z);
                        if (starL == vs[i].trainingStar)
                        {
                            starLiTran.localPosition = new Vector3(allWidth * b1 - allWidth / 2.0f, starLiTran.localPosition.y, starLiTran.localPosition.z);
                        }
                    }
                    starGo.Add(starTran.gameObject);
                }
            }
        }

        private void SetStar()
        {
            for (int i = 0; i < vs.Count; i++)
            {
                starGo[i].transform.Find("Star_Light").gameObject.SetActive(vs[i].trainingIntegralCondition <= Sys_Family.Instance.totalScore);
            }
        }

        public override void SetSliderValue(uint currentValue, uint maxValue)
        {
            maxValue = vs[vs.Count - 1].trainingIntegralCondition;
            int index = vs.Count;
            for (int i = 0; i < vs.Count; i++)
            {
                if (currentValue < vs[i].trainingIntegralCondition)
                {
                    index = i;
                    break;
                }
            }
            SetStar();
            bool hasNextStar = index <= vs.Count - 1;
            nextText.gameObject.SetActive(hasNextStar);
            if(hasNextStar)
            {
                nextText.text = LanguageHelper.GetTextContent(2023706, (vs[index].trainingIntegralCondition - Sys_Family.Instance.totalScore).ToString());
            }
            allText.text = currentValue.ToString();
            slider.value = (currentValue + 0f) / maxValue;
        }
    }

    public class UI_FamilyCreatures_TrainRightView : UIComponent
    {
        private Text typeText;
        private Text timeTypeText;
        private Text timeText;
        private Text recommendLevelText;
        private Text descText;
        private UI_FamilyCreatureMoodSlider moodSlider;
        private UI_FamilyCreatureHealthSlider healthSlider;
        private FamilyCreatureEntry familyCreatureEntry;
        private Timer timer;
        private Button teamBtn;
        private Button trainBtn;
        protected override void Loaded()
        {
            typeText = transform.Find("View_Info/bg_Title/Text").GetComponent<Text>();
            timeTypeText = transform.Find("View_Info/Text_RemainingTime").GetComponent<Text>();
            timeText = transform.Find("View_Info/Text_RemainingTime/Text_Value").GetComponent<Text>();
            recommendLevelText = transform.Find("View_Info/Text_Recommend/Text_Value").GetComponent<Text>();
            descText = transform.Find("View_Info/Text_Describe").GetComponent<Text>();

            moodSlider = new UI_FamilyCreatureMoodSlider();
            moodSlider.InitTransform(transform.Find("View_Condition/Mood"));

            healthSlider = new UI_FamilyCreatureHealthSlider();
            healthSlider.InitTransform(transform.Find("View_Condition/Health"));
            teamBtn = transform.Find("Btn_01").GetComponent<Button>();
            teamBtn.onClick.AddListener(TeamBtnClicked);
            trainBtn = transform.Find("Btn_02").GetComponent<Button>();
            trainBtn.onClick.AddListener(TrainClicked);
        }

        private void TeamBtnClicked()
        {
            if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023759));
                return;
            }
            Sys_Team.Instance.OpenFastUI(5001u);
        }

        private void TrainClicked()
        {
            if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023759));
                return;
            }
            else if(Sys_Family.Instance.IsOnReadyTime())//准备时间
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023732));
                return;
            }
            else if (!Sys_Family.Instance.IsInTrainTime())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023415));
                return;
            }

            if(null != familyCreatureEntry)
            {
                Sys_Family.Instance.GuildPetEnterBattleReq();
            }
        }

        public override void Hide()
        {
            timer?.Cancel();
        }

        public void SetInfo(FamilyCreatureEntry familyCreatureEntry, uint trainState = 0)
        {
            timer?.Cancel();
            timer = Timer.Register(1, null, (t) =>
            {
                if(null != timeText)
                {
                    bool isReady = Sys_Family.Instance.IsOnReadyTime();
                    if(isReady)
                    {
                        timeTypeText.text = LanguageHelper.GetTextContent(2023731);
                    }
                    else
                    {
                        timeTypeText.text = LanguageHelper.GetTextContent(2023707);
                    }
                    timeText.text = Sys_Family.Instance.FamilyCreatureTrainTime();
                }
            }, true);
            this.familyCreatureEntry = familyCreatureEntry;
            moodSlider.SetSliderValue(familyCreatureEntry, 0);
            healthSlider.SetSliderValue(familyCreatureEntry, 0);
            if (trainState != 0)
            {
                CSVFamilyPet.Data cSV = CSVFamilyPet.Instance.GetConfData(trainState);
                typeText.text = LanguageHelper.GetTextContent(Sys_Family.Instance.CreatureState(cSV.stage));
                descText.text = LanguageHelper.GetTextContent(cSV.diffcultyDetails);
                if (null != cSV.train_id && cSV.train_id.Count >= 2)
                {
                    recommendLevelText.text = LanguageHelper.GetTextContent(2023730, cSV.train_id[0].ToString(), cSV.train_id[1].ToString());
                }
            }
            else
            {
                typeText.text = LanguageHelper.GetTextContent(Sys_Family.Instance.CreatureState(familyCreatureEntry.cSV.stage));
                descText.text = LanguageHelper.GetTextContent(familyCreatureEntry.cSV.diffcultyDetails);
                if (null != familyCreatureEntry.cSV.train_id && familyCreatureEntry.cSV.train_id.Count >= 2)
                {
                    recommendLevelText.text = LanguageHelper.GetTextContent(2023730, familyCreatureEntry.cSV.train_id[0].ToString(), familyCreatureEntry.cSV.train_id[1].ToString());
                }
            }
                
        }

    }

    public class UI_FamilyCreatures_Train : UIBase, UI_FamilyCreatures_Train_Layout.IListener
    {
        private UI_FamilyCreatures_Train_Layout layout = new UI_FamilyCreatures_Train_Layout();
        private UI_FamilyCreature_LeftView leftTopView;
        private UI_FamilyCreatures_TrainRightView rightView;
        private UI_FamilyCreatureModelLoad modelLoad = new UI_FamilyCreatureModelLoad();
        private FamilyCreatureEntry familyCreatureEntry;
        private UI_FamilyCreatureTrainStarSlider uI_FamilyCreatureTrainStarSlider;
        private GuildPetTraining guildPetTraining;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            leftTopView = AddComponent<UI_FamilyCreature_LeftView>(layout.LeftTopTran);
            rightView = AddComponent<UI_FamilyCreatures_TrainRightView>(layout.RightViewTran);
            uI_FamilyCreatureTrainStarSlider = new UI_FamilyCreatureTrainStarSlider();
            uI_FamilyCreatureTrainStarSlider.InitTransform(layout.TrainStar);
            modelLoad.SetEventImage(transform.Find("Animator/Texture").GetComponent<Image>());
            modelLoad.assetDependencies = transform.GetComponent<AssetDependencies>();
            Sys_Fight.Instance.OnEnterFight += OnEnterFight;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {

            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnFamilyPetTrainScore, RefreshView, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnGetFamilyPetInfo, RefreshView, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            guildPetTraining = Sys_Family.Instance.GetTrainInfo();
            if(null != guildPetTraining)
            {
                familyCreatureEntry = Sys_Family.Instance.GetFamilyCreatureByType(guildPetTraining.TrainingStage / 10);
            }
        }

        private void OnEnterFight(CSVBattleType.Data cSVBattleType)
        {
            CloseSelf();
        }

        protected override void OnShow()
        {
            if(null != familyCreatureEntry)
            {
                Sys_Family.Instance.GuildPetGetTrainingScoreReq();
                Sys_Family.Instance.GuildPetUpdatePetInfoReq(familyCreatureEntry.FmilyCreatureId);
            }
            uI_FamilyCreatureTrainStarSlider.SetOtherInfo();
            RefreshView();
        }

        private void RefreshView()
        {
            if (null != familyCreatureEntry)
            {
                leftTopView.SetFamilyCreatureInfo(familyCreatureEntry, guildPetTraining.TrainingStage);
                rightView.SetInfo(familyCreatureEntry, guildPetTraining.TrainingStage);
                modelLoad.SetValue(CSVFamilyPet.Instance.GetConfData(guildPetTraining.TrainingStage));
                uI_FamilyCreatureTrainStarSlider.SetSliderValue(Sys_Family.Instance.totalScore, 0);
                layout.myPonitText.text = Sys_Family.Instance.score.ToString();
            }
        }

        protected override void OnHide()
        {
            rightView?.Hide();
            modelLoad?.Hide();
        }

        protected override void OnDestroy()
        {
        }

        public void CloseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Train, "CloseBtnClicked");
            UIManager.CloseUI(EUIID.UI_FamilyCreatures_Train);
        }

        public void PreviewStarRewardBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Train, "PreviewStarRewardBtnClicked");
            UIManager.OpenUI(EUIID.UI_FamilyCreatures_Reward);
        }

        public void RuleBtnClick()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Train, "RuleBtnClick");
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2023750) });
        }

        public void RankBtnClick()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Train, "RuleBtnClick");
            UIManager.OpenUI(EUIID.UI_FamilyCreatures_Rank);
        }
    }
}