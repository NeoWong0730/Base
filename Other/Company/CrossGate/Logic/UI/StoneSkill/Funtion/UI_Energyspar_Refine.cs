using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;
using System;
using UnityEngine;
using Table;

namespace Logic
{

    public class UI_EnergysparRefineBtn
    {
        private Button addItemBtn;
        private Button minItemBtn;
        private Image itemIcomImage;
        private Image itemIcomQualityImage;
        private Text itemNumText;
        private IListener listener;
        private long currentNum = 0;
        CSVItem.Data itemData;
        private uint currentExp;
        private uint maxExp;

        public void Init(Transform transform)
        {
            addItemBtn = transform.GetComponent<Button>();
            addItemBtn.onClick.AddListener(() =>
            {
                OnNumChangeControlBtnClick(1);
            });
            minItemBtn = transform.Find("Btn_Minus").GetComponent<Button>();
            minItemBtn.onClick.AddListener(() =>
            {
                OnNumChangeControlBtnClick(-1);
            });
            itemIcomImage = transform.Find("Image_Icon").GetComponent<Image>();
            itemIcomImage.enabled = true;
            itemIcomQualityImage = transform.Find("Image_Quality").GetComponent<Image>();
            itemIcomQualityImage.enabled = true;
            itemNumText = transform.Find("Text_Number").GetComponent<Text>();
        }

        public void SetData(params object[] arg)
        {
            if (null == itemData)
            {
                itemData = CSVItem.Instance.GetConfData(Convert.ToUInt32(arg[0]));
                if (null != itemData)
                {
                    ImageHelper.SetIcon(itemIcomImage, itemData.icon_id);
                    ImageHelper.GetQualityColor_Frame(itemIcomQualityImage, (int)itemData.quality);
                }
            }
            currentExp = Convert.ToUInt32(arg[1]);
            maxExp = Convert.ToUInt32(arg[2]);
            if (null != itemData)
            {
                long haveNum = Sys_Bag.Instance.GetItemCount(itemData.id);
                TextHelper.SetText(itemNumText, LanguageHelper.GetTextContent(2021030, haveNum.ToString(), currentNum.ToString()));
                minItemBtn.gameObject.SetActive(currentNum != 0);
            }
        }

        private void OnNumChangeControlBtnClick(int num)
        {
            long changeEndNum = currentNum + num;
            long endNum = currentExp + Sys_StoneSkill.Instance.expNum * changeEndNum;
            if (endNum > maxExp && endNum >= maxExp + Sys_StoneSkill.Instance.expNum)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021079));
                return;
            }
            long haveNum = Sys_Bag.Instance.GetItemCount(itemData.id);
            if (changeEndNum >= 0 && changeEndNum <= haveNum)
            {
                currentNum = changeEndNum;
                TextHelper.SetText(itemNumText, LanguageHelper.GetTextContent(2021030, haveNum.ToString(), currentNum.ToString()));
                listener?.changeNum(currentNum);
            }
            minItemBtn.gameObject.SetActive(currentNum != 0);
        }

        public void Reset()
        {
            currentNum = 0;
            long haveNum = Sys_Bag.Instance.GetItemCount(itemData.id);
            TextHelper.SetText(itemNumText, LanguageHelper.GetTextContent(2021030, haveNum.ToString(), currentNum.ToString()));
        }

        public void Hide()
        {
            currentNum = 0;
        }


        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void changeNum(long num);
        }

    }

    public class UI_Energyspar_Refine_Layout
    {
        public Transform transform;
        public Button closeButton;
        public Text refineTimeText;
        public Text currentLevelText;
        public Text nextLevelText;
        public Text currentSkillDescText;
        public Text nextSkillDescText;
        public Text refineTipsText;
        public Slider expSlider;
        public Text expSliderText;

        public Text useTipsText;

        public ItemCost onceCoseItem = new ItemCost();
        public ItemCost tenCoseItem = new ItemCost();

        public Button onceBtn;
        public Button tenBtn;
        public Button itemUseBTn;
        public UI_Energyspar_LevelInfo levelInfo;

        public void Init(Transform transform)
        {
            this.transform = transform;
            closeButton = transform.Find("View_TipsBg01_Largest/Btn_Close").GetComponent<Button>();
            refineTimeText = transform.Find("Animator/Text_Time/Value").GetComponent<Text>();
            currentLevelText = transform.Find("Animator/View_Left/Text_Level").GetComponent<Text>();
            nextLevelText = transform.Find("Animator/View_Right/Text_Level").GetComponent<Text>();
            currentSkillDescText = transform.Find("Animator/View_Left/Text_Description").GetComponent<Text>();
            nextSkillDescText = transform.Find("Animator/View_Right/Text_Description").GetComponent<Text>();
            expSlider = transform.Find("Animator/Slider_Exp").GetComponent<Slider>();
            expSliderText = transform.Find("Animator/Slider_Exp/Text_Value").GetComponent<Text>();

            refineTipsText = transform.Find("Animator/Text_RefineTips").GetComponent<Text>();
            useTipsText = transform.Find("Animator/Text_UseTips").GetComponent<Text>();
            onceCoseItem.SetGameObject(transform.Find("Animator/Cost_1").gameObject);
            tenCoseItem.SetGameObject(transform.Find("Animator/Cost_2").gameObject);

            onceBtn = transform.Find("Animator/Btn_Refine1").GetComponent<Button>();
            tenBtn = transform.Find("Animator/Btn_Refine2").GetComponent<Button>();
            itemUseBTn = transform.Find("Animator/Btn_Use").GetComponent<Button>();

            levelInfo = new UI_Energyspar_LevelInfo();
            levelInfo.Init(transform.Find("Animator/View_Skill"));
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OncloseBtnClicked);
            onceBtn.onClick.AddListener(listener.OnOnceBtnClicked);
            tenBtn.onClick.AddListener(listener.OnTenBtnClicked);
            itemUseBTn.onClick.AddListener(listener.OnItemUseClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnOnceBtnClicked();
            void OnTenBtnClicked();
            void OnItemUseClicked();
        }
    }

    public class UI_Energyspar_Refine : UIBase, UI_Energyspar_Refine_Layout.IListener, UI_EnergysparRefineBtn.IListener
    {
        private UI_Energyspar_Refine_Layout layout = new UI_Energyspar_Refine_Layout();
        private CSVStone.Data configData;
        public UI_EnergysparRefineBtn teFuntionBtn;

        private long itemNum;
        private uint alltime;
        private long currentExp;
        private long maxExp;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            teFuntionBtn = new UI_EnergysparRefineBtn();
            teFuntionBtn.Init(transform.Find("Animator/Btn_Item"));
            teFuntionBtn.Register(this);
            CSVParam.Data data = CSVParam.Instance.GetConfData(725);
            if (null != data)
            {
                alltime = Convert.ToUInt32(data.str_value);
            }

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_StoneSkill.Instance.eventEmitter.Handle(Sys_StoneSkill.EEvents.UpgradeResultClose, WhenUpViewClose, toRegister);
            Sys_StoneSkill.Instance.eventEmitter.Handle<uint>(Sys_StoneSkill.EEvents.OnNonaSkillUpGa, WhenUpRes, toRegister);
            Sys_StoneSkill.Instance.eventEmitter.Handle<uint>(Sys_StoneSkill.EEvents.EnergysparSpecilEvent, EnergysparSpecilEvent, toRegister);
            Sys_StoneSkill.Instance.eventEmitter.Handle(Sys_StoneSkill.EEvents.RefineTimeRefresh, RefineTimeRefresh, toRegister);
        }

        private void EnergysparSpecilEvent(uint id)
        {
            CloseSelf();
        }

        protected override void OnOpen(object arg)
        {
            configData = arg as CSVStone.Data;
        }

        protected override void OnShow()
        {
            UpdataInfo();
        }

        protected override void OnHide()
        {
            teFuntionBtn.Hide();
        }

        private void WhenUpRes(uint id)
        {
            UpdataInfo();
        }

        private void WhenUpViewClose()
        {
            if (Sys_StoneSkill.Instance.CanAdvance(configData.id))
            {
                CloseSelf();
                UIManager.OpenUI(EUIID.UI_Energyspar_Advanced, false, configData);
            }
        }

        private void RefineTimeRefresh()
        {
            TextHelper.SetText(layout.refineTimeText, LanguageHelper.GetTextContent(2021030, Sys_StoneSkill.Instance.levelUpCount.UsedTimes.ToString(), alltime.ToString()));
        }

        private void UpdataInfo()
        {
            if (null != configData)
            {
                layout.levelInfo.UpdateInfo(configData);
                if (Sys_StoneSkill.Instance.levelUpCount.ExpireTime < Sys_Time.Instance.GetServerTime())
                {
                    Sys_StoneSkill.Instance.levelUpCount.UsedTimes = 0;
                }
                TextHelper.SetText(layout.refineTimeText, LanguageHelper.GetTextContent(2021030, Sys_StoneSkill.Instance.levelUpCount.UsedTimes.ToString(), alltime.ToString()));
                StoneSkillData severData = Sys_StoneSkill.Instance.GetServerDataById(configData.id);
                if (null != severData)
                {
                    TextHelper.SetText(layout.currentLevelText, LanguageHelper.GetTextContent(2021003, severData.powerStoneUnit.Level.ToString()));
                    TextHelper.SetText(layout.nextLevelText, LanguageHelper.GetTextContent(2021003, (severData.powerStoneUnit.Level + 1).ToString()));
                    CSVStoneLevel.Data currentLevelData = CSVStoneLevel.Instance.GetConfData(configData.id * 1000 + severData.powerStoneUnit.Level);
                    if (null != currentLevelData)
                    {
                        TextHelper.SetText(layout.currentSkillDescText, Sys_StoneSkill.Instance.GetSkillDesc(configData.id));
                        if (null != currentLevelData.cost_money && currentLevelData.cost_money.Count >= 2)
                        {
                            layout.onceCoseItem?.Refresh(new ItemIdCount(currentLevelData.cost_money[0], (long)currentLevelData.cost_money[1]));
                            layout.tenCoseItem?.Refresh(new ItemIdCount(currentLevelData.cost_money[0], (long)currentLevelData.cost_money[1] * 10));
                        }

                        TextHelper.SetText(layout.refineTipsText, LanguageHelper.GetTextContent(2021035, currentLevelData.exp_stone_add.ToString()));
                    }

                    CSVStoneLevel.Data nextLevelData = CSVStoneLevel.Instance.GetConfData(configData.id * 1000 + severData.powerStoneUnit.Level + 1);
                    if (null != nextLevelData)
                    {
                        TextHelper.SetText(layout.nextSkillDescText, Sys_StoneSkill.Instance.GetSkillDesc(configData.id, 1));
                        currentExp = severData.powerStoneUnit.Exp;
                        maxExp = nextLevelData.exp_stone;
                        SetSliderValue(severData.powerStoneUnit.Exp, nextLevelData.exp_stone);
                    }
                }
            }

            TextHelper.SetText(layout.useTipsText, LanguageHelper.GetTextContent(2021033, Sys_StoneSkill.Instance.expNum.ToString()));
            teFuntionBtn.SetData(Sys_StoneSkill.Instance.itemId, currentExp, maxExp);
        }

        public void SetSliderValue(long current, long max)
        {
            layout.expSlider.value = (float)current / max;
            TextHelper.SetText(layout.expSliderText, LanguageHelper.GetTextContent(2021030, current.ToString(), max.ToString()));
        }

        public void changeNum(long num)
        {
            itemNum = num;
            long tempExp = itemNum * Sys_StoneSkill.Instance.expNum + currentExp;
            if (tempExp > maxExp)
            {
                tempExp = maxExp;
            }
            SetSliderValue(tempExp, maxExp);
        }

        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Energyspar_Refine);
        }

        public void OnOnceBtnClicked()
        {
            RefineTime(1);
        }

        public void OnTenBtnClicked()
        {
            RefineTime(10);
        }

        public void OnItemUseClicked()
        {
            if (Sys_StoneSkill.Instance.CanAdvance(configData.id))
                return;
            long haveNum = Sys_Bag.Instance.GetItemCount(Sys_StoneSkill.Instance.itemId);
            if (haveNum == 0)
            {
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(Sys_StoneSkill.Instance.itemId, 0, false, false, false, false, false, false, true);
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Energyspar_Refine, itemData));
                return;
            }
            if (itemNum == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021067));
                return;
            }

            if (null != configData)
            {
                StoneSkillData severData = Sys_StoneSkill.Instance.GetServerDataById(configData.id);
                if (null != severData)
                {
                    CSVStoneLevel.Data currentLevelData = CSVStoneLevel.Instance.GetConfData(configData.id * 1000 + severData.powerStoneUnit.Level);
                    if (null != currentLevelData)
                    {
                        if (Sys_StoneSkill.Instance.expNum * itemNum + severData.powerStoneUnit.Exp > maxExp)
                        {
                            itemNum = Sys_StoneSkill.Instance.stoneUpgradeTimes = (uint)Math.Ceiling((maxExp - severData.powerStoneUnit.Exp) / (float)Sys_StoneSkill.Instance.expNum);
                        }
                        else
                        {
                            Sys_StoneSkill.Instance.stoneUpgradeTimes = (uint)itemNum;
                        }
                    }
                }
            }

            Sys_StoneSkill.Instance.OnPowerStoneLevelUpReq(configData.id, Sys_StoneSkill.Instance.itemId, Convert.ToUInt32(itemNum));
            itemNum = 0;
            teFuntionBtn.Reset();
        }

        private void RefineTime(uint times)
        {
            if (null != configData)
            {
                if (Sys_StoneSkill.Instance.CanAdvance(configData.id))
                    return;
                StoneSkillData severData = Sys_StoneSkill.Instance.GetServerDataById(configData.id);
                if (null != severData)
                {
                    CSVStoneLevel.Data currentLevelData = CSVStoneLevel.Instance.GetConfData(configData.id * 1000 + severData.powerStoneUnit.Level);
                    if (null != currentLevelData)
                    {
                        if (null != currentLevelData.cost_money && currentLevelData.cost_money.Count >= 2)
                        {
                            uint _needCurrencyId = currentLevelData.cost_money[0];
                            long _needCount = (long)currentLevelData.cost_money[1] * times;
                            ItemIdCount nowData = new ItemIdCount(_needCurrencyId, _needCount);
                            if (!nowData.Enough)
                            {
                                Sys_Bag.Instance.TryOpenExchangeCoinUI((ECurrencyType)_needCurrencyId, _needCount);
                            }
                            else
                            {
                                if (times == 1)
                                {
                                    Sys_StoneSkill.Instance.stoneUpgradeTimes = 1;
                                }
                                else if (currentLevelData.exp_stone_add * times + severData.powerStoneUnit.Exp > maxExp)
                                {
                                    times = Sys_StoneSkill.Instance.stoneUpgradeTimes = (uint)Math.Ceiling((maxExp - severData.powerStoneUnit.Exp) / (float)currentLevelData.exp_stone_add);
                                }
                                else
                                {
                                    Sys_StoneSkill.Instance.stoneUpgradeTimes = times;
                                }

                                Sys_StoneSkill.Instance.OnPowerStoneLevelUpReq(configData.id, 0, times);
                                teFuntionBtn.Reset();
                            }
                        }
                    }
                }
            }
        }

    }
}

