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
    public class PopupParam
    {
        public uint type;
        //喂食提示
        public uint itemId;
        public uint mood2Value;
        public uint Health2Value;
        public bool isHunger;
        public uint growth2Value;
    }

    public class UI_FamilyCreatures_Popup_Layout
    {
        private Button closeBtn;
        private GameObject startObj;
        private GameObject endObj;
        private GameObject feedObj;
        private Text startText;
        private Text endText;
        private Text moodOrHealthText;
        //valueShow
        private Text growthValueText;
        private Text moodorHealthValueText;
        private Text expValueText;
        private GameObject feedRatioGo;
        private Text expXText;
        private Text feedTypeText;
        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Image").GetComponent<Button>();
            startObj = transform.Find("Animator/Image_BG").gameObject;
            endObj = transform.Find("Animator/Image_BG1").gameObject;
            feedObj = transform.Find("Animator/Image_BG2").gameObject;
            startText = transform.Find("Animator/Image_BG/Text").GetComponent<Text>();
            endText = transform.Find("Animator/Image_BG1/Text").GetComponent<Text>();

            moodOrHealthText = transform.Find("Animator/Image_BG2/content/MoodValue/Text").GetComponent<Text>();
            growthValueText = transform.Find("Animator/Image_BG2/content/GrowthValue/Value").GetComponent<Text>();
            moodorHealthValueText = transform.Find("Animator/Image_BG2/content/MoodValue/Value").GetComponent<Text>();
            expValueText = transform.Find("Animator/Image_BG2/content/ExpValue/Value/Value").GetComponent<Text>();
            feedRatioGo = transform.Find("Animator/Image_BG2/content/ExpValue/Value/Critical").gameObject;
            feedTypeText = transform.Find("Animator/Image_BG2/Text_Title").GetComponent<Text>();
            transform.Find("Animator/Image_BG2/content/ExpValue/Value/Critical/Text").GetComponent<Text>().text = "x" + Sys_Family.Instance.FeedExpRatio.ToString();

        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseThisUI);
        }

        public void ShowObjByType(PopupParam pop)
        {
            startObj.gameObject.SetActive(pop.type == 1);
            endObj.gameObject.SetActive(pop.type == 2);
            feedObj.gameObject.SetActive(pop.type == 3);
            if(pop.type == 3)
            {
                CSVFamilyPetFood.Data csvFoodData = CSVFamilyPetFood.Instance.GetConfData(pop.itemId);
                bool isDrugs = csvFoodData.isDrugs == 1;
                moodOrHealthText.text = LanguageHelper.GetTextContent(isDrugs ? 2023616u : 2023609u);
                Sys_Family.Instance.SetTextData(growthValueText, pop.growth2Value, csvFoodData.addGrowthValue);
                //健康或者心情
                if(isDrugs)
                {
                    Sys_Family.Instance.SetTextData(moodorHealthValueText, pop.Health2Value, csvFoodData.addHealthValue);
                }
                else
                {
                    Sys_Family.Instance.SetTextData(moodorHealthValueText, pop.mood2Value, csvFoodData.addMoodValue);
                }
                bool isHunger = pop.isHunger;
                feedTypeText.text = LanguageHelper.GetTextContent(isHunger ? 2023831u : 2023833u);
                feedRatioGo.gameObject.SetActive(isHunger);
                var roleLevel = Sys_Role.Instance.Role.Level;
                uint exp = 0;
                for (int i = 0; i < csvFoodData.feedRewardEXP.Count; i++)
                {
                    if (null != csvFoodData.feedRewardEXP[i] && csvFoodData.feedRewardEXP[i].Count >= 2 && roleLevel <= csvFoodData.feedRewardEXP[i][0])
                    {
                        exp = csvFoodData.feedRewardEXP[i][1];
                        break;
                    }
                }
                var expValue = isHunger ? exp * Sys_Family.Instance.FeedExpRatio : exp;
                Sys_Family.Instance.SetTextData(expValueText, (float)expValue, exp);
            }
            else if(pop.type == 1)
            {
                GuildPetTraining trainInfo = Sys_Family.Instance.GetTrainInfo();
                CSVFamilyPet.Data cSVFamilyPetData = CSVFamilyPet.Instance.GetConfData(trainInfo.TrainingStage);
                if(null != cSVFamilyPetData)
                {
                    TextHelper.SetText(startText, 2023790, LanguageHelper.GetTextContent(Sys_Family.Instance.GetTypeName(cSVFamilyPetData.food_Type)));
                }
            }
            else if (pop.type == 2)
            {
                GuildPetTraining trainInfo = Sys_Family.Instance.GetTrainInfo();
                CSVFamilyPet.Data cSVFamilyPetData = CSVFamilyPet.Instance.GetConfData(trainInfo.TrainingStage);
                if (null != cSVFamilyPetData)
                {
                    TextHelper.SetText(endText, 2023791, LanguageHelper.GetTextContent(Sys_Family.Instance.GetTypeName(cSVFamilyPetData.food_Type)));
                }
            }
        }

        public interface IListener
        {
            void CloseThisUI();
        }
    }

    public class UI_FamilyCreatures_Popup : UIBase, UI_FamilyCreatures_Popup_Layout.IListener
    {
        private UI_FamilyCreatures_Popup_Layout layout = new UI_FamilyCreatures_Popup_Layout();
        private Queue<PopupParam> popupParams = new Queue<PopupParam>();
        private Timer closeTimer;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle<PopupParam>(Sys_Family.EEvents.OnFamilyPopAdd, NewMessageAdd, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            if (null != arg)
            {
                popupParams.Enqueue(arg as PopupParam);
            }
        }

        protected override void OnShow()
        {
            if(popupParams.Count > 0)
            {
                ViewShow(popupParams.Dequeue());
            }
            else
            {
                CloseThisUI();
            }
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            closeTimer?.Cancel();
        }

        private void ViewShow(PopupParam waitePop)
        {
            if(waitePop != null)
            {
                layout.ShowObjByType(waitePop);
                closeTimer?.Cancel();
                closeTimer = Timer.Register(5f, () =>
                {
                    CheckData();
                });
            }
            
        }

        private void CheckData()
        {
            if (popupParams.Count > 0)
            {
                ViewShow(popupParams.Dequeue());
            }
            else
            {
                UIManager.CloseUI(EUIID.UI_FamilyCreatures_Popup);
            }
        }

        public void NewMessageAdd(PopupParam pop)
        {
            if (null != pop && popupParams.Count > 0)
            {
                popupParams.Enqueue(pop);
            }
        }

        public void CloseThisUI()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Popup, "CloseThisUI");
            CheckData();
        }
    }
}