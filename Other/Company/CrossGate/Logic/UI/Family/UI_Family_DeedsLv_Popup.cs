using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System;

namespace Logic
{
    public class UI_Family_DeedsLv_Popup_Layout
    {
        public Transform transform;
        public Text level;
        public Text cost01;
        public Text cost01Num;
        public Text cost02;
        public Text cost02Num;
        public Text tips;
        public Slider lvSlider;
        public Image cost01Icon;
        public Image cost02Icon;
        public Button upBtn;
        public Button closeBtn;

        public void Init(Transform transform)
        {
            this.transform = transform;
            level = transform.Find("Animator/Text_Lv").GetComponent<Text>();
            lvSlider = transform.Find("Animator/Slider_Exp").GetComponent<Slider>();
            cost01 = transform.Find("Animator/Text_Consume1").GetComponent<Text>();
            cost01Num = transform.Find("Animator/Text_Consume1/Text_Num").GetComponent<Text>();
            cost01Icon = transform.Find("Animator/Text_Consume1/Icon").GetComponent<Image>();
            cost02 = transform.Find("Animator/Text_Consume2").GetComponent<Text>();
            cost02Num = transform.Find("Animator/Text_Consume2/Text_Num").GetComponent<Text>();
            tips = transform.Find("Animator/Text_Tips").GetComponent<Text>();
            cost02Icon = transform.Find("Animator/Text_Consume2/Icon").GetComponent<Image>();
            upBtn = transform.Find("Animator/Btn_01").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnCloseBtnClicked);
            upBtn.onClick.AddListener(listener.OnUpBtnClicked);
        }

        public interface IListener
        {
            void OnCloseBtnClicked();
            void OnUpBtnClicked();
        }
    }

    public class UI_Family_DeedsLv_Popup : UIBase, UI_Family_DeedsLv_Popup_Layout.IListener
    {
        private uint costId01;
        private uint costId02;
        private uint costCount01;
        private uint costCount02;
        private uint curLevel;

        private uint maxLv;
        private uint breakthroughLv;
        private UI_Family_DeedsLv_Popup_Layout layout = new UI_Family_DeedsLv_Popup_Layout();

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }
        protected override void OnShow()
        {
            SetValue();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChanged, toRegister);
            Sys_Experience.Instance.eventEmitter.Handle(Sys_Experience.EEvents.OnExperienceUpgrade, OnExperienceUpgrade, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateFamilyInfo, OnUpdateFamilyInfo, toRegister);
        }       

        private void SetValue()
        {
            curLevel = Sys_Experience.Instance.exPerienceLevel;
            SetCostData(curLevel + 1);
            if (Sys_Family.Instance.familyData.isInFamily)
            {
                // Sys_Family.Instance.SendGuildGetGuildInfoReq();
                OnUpdateFamilyInfo();
            }
            else
            {
                maxLv = Sys_Experience.Instance.GetMaxLv(0);
                SetExploitData(curLevel);
                CSVExperienceLevel.Data nextData = CSVExperienceLevel.Instance.GetConfData(maxLv + 1);
                if (nextData != null)
                {
                    layout.tips.text = LanguageHelper.GetTextContent(2021221, CSVExperienceLevel.Instance.GetConfData(maxLv + 1).need_level.ToString(), CSVExperienceLevel.Instance.GetConfData(maxLv + 1).need_technology.ToString());
                }
                else
                {
                    layout.tips.text = string.Empty;
                }            
            }
        }

        private void SetExploitData(uint level)
        {
            layout.level.text = LanguageHelper.GetTextContent(2021003, level.ToString()) + "/" + LanguageHelper.GetTextContent(2021003, maxLv.ToString());
            if (maxLv == 0 || level >= maxLv)
            {
                layout.lvSlider.value = 1;
            }
            else
            {
                layout.lvSlider.value = (float)(level) / maxLv;
            }
        }

        private void SetCostData(uint level)
        {
            if (!CSVExperienceLevel.Instance.ContainsKey(level))
            {
                //Debug.LogError("CSVExperienceLevel not containskey" + level.ToString());
                return;
            }
            CSVExperienceLevel.Data data = CSVExperienceLevel.Instance.GetConfData(level);
            costId01 = data.cost[0][0];
            costCount01 = data.cost[0][1];
            costId02 = data.cost[1][0];
            costCount02 = data.cost[1][1];
            layout.cost01.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(costId01).name_id);
            layout.cost02.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(costId02).name_id);
            ImageHelper.SetIcon(layout.cost01Icon, CSVItem.Instance.GetConfData(costId01).icon_id);
            ImageHelper.SetIcon(layout.cost02Icon, CSVItem.Instance.GetConfData(costId02).icon_id);
            if (Sys_Bag.Instance.GetItemCount(costId01) < costCount01)
            {
                layout.cost01Num.text = "<color=red>" + Sys_Bag.Instance.GetItemCount(costId01).ToString() + "</color>/" + costCount01.ToString();
            }
            else
            {
                layout.cost01Num.text = "<color=#56422E>" + Sys_Bag.Instance.GetItemCount(costId01).ToString() + "</color>/" + costCount01.ToString();
            }
            if (Sys_Bag.Instance.GetItemCount(costId02) < costCount02)
            {
                layout.cost02Num.text = "<color=red>" + Sys_Bag.Instance.GetItemCount(costId02).ToString() + "</color>/" + costCount02.ToString();
            }
            else
            {
                layout.cost02Num.text = "<color=#56422E>" + Sys_Bag.Instance.GetItemCount(costId02).ToString() + "</color>/" + costCount02.ToString();
            }
        }

        private void OnCurrencyChanged(uint id, long value)
        {
            uint curlevel = Sys_Experience.Instance.exPerienceLevel;
            SetCostData(curlevel);
        }

        private void OnExperienceUpgrade()
        {
            SetValue();
        }

        private void OnUpdateFamilyInfo()
        {
            uint skillId = Sys_Family.Instance.familyData.GetSkillId(Sys_Family.FamilyData.FamilySkillType.BreakthroughTraining);
            if (skillId == 0)
            {
                breakthroughLv = 0;
            }
            else
            {
                breakthroughLv = CSVFamilySkillUp.Instance.GetConfData(skillId).SkillLevel;
            }
            maxLv = Sys_Experience.Instance.GetMaxLv(breakthroughLv);
            SetExploitData(curLevel);
            CSVExperienceLevel.Data nextData = CSVExperienceLevel.Instance.GetConfData(maxLv + 1);
            if (nextData != null)
            {
                layout.tips.text = LanguageHelper.GetTextContent(2021221, CSVExperienceLevel.Instance.GetConfData(maxLv + 1).need_level.ToString(), CSVExperienceLevel.Instance.GetConfData(maxLv + 1).need_technology.ToString());
            }
            else
            {
                layout.tips.text = string.Empty;
            }
        }


        public void OnCloseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_Family_DeedsLv_Popup, "OnCloseBtnClicked");
            UIManager.CloseUI(EUIID.UI_Family_DeedsLv_Popup);
        }

        public void OnUpBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_Family_DeedsLv_Popup, "OnUpBtnClicked");
            if (curLevel < maxLv)
            {
                if (Sys_Bag.Instance.GetItemCount(costId01) >= costCount01)
                {
                    if (Sys_Bag.Instance.GetItemCount(costId02) >= costCount02)
                    {
                        Sys_Experience.Instance.UpgradeReq();
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5606));
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5605));
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5607));
            }
        }
    }
}
