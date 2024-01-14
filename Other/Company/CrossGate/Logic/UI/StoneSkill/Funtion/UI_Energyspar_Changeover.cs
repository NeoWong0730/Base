using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;
using System;
using UnityEngine;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class EnergysparChangeoverParam
    {
        public uint id;
        public uint index;
    }

    public class UI_Energyspar_Changeover: UIBase
    {
        private Button closeBtn;
        private Button changeoverBtn;
        private Image currentImage;
        private Image changeoverImage;
        private Image currentBackImage;
        private Image changeoverBackImage;
        private Text currentText;
        private Text changeoverText;
        private Text currentDescText;
        private Text changeoverDescText;
        private Transform itemGrid;
        private bool isEnough;
        private EnergysparChangeoverParam param;

        private Color lightColor;
        private Color darkColor;
        protected override void OnLoaded()
        {
            closeBtn = transform.Find("Animator/View_TipsBg02_Middle/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(() => { CloseSelf(); });
            changeoverBtn = transform.Find("Animator/Button_Changeover").GetComponent<Button>();
            changeoverBtn.onClick.AddListener(ChangeoverBtnClick);

            currentImage = transform.Find("Animator/View_ItemGroup/Item_Light/Image_Skillbg/Image_SkillIcon").GetComponent<Image>();
            changeoverImage = transform.Find("Animator/View_ItemGroup/Item_Dark/Image_Skillbg/Image_SkillIcon").GetComponent<Image>();

            currentBackImage = transform.Find("Animator/View_ItemGroup/Item_Light").GetComponent<Image>();
            changeoverBackImage = transform.Find("Animator/View_ItemGroup/Item_Dark").GetComponent<Image>();

            currentText = transform.Find("Animator/View_ItemGroup/Item_Light/Text_Name").GetComponent<Text>();
            changeoverText = transform.Find("Animator/View_ItemGroup/Item_Dark/Text_Name").GetComponent<Text>();
            currentDescText = transform.Find("Animator/View_ItemGroup/Item_Light/Text_Explain").GetComponent<Text>();
            changeoverDescText = transform.Find("Animator/View_ItemGroup/Item_Dark/Text_Explain").GetComponent<Text>();

            itemGrid = transform.Find("Animator/Grid");

            string[] _strs2 = CSVParam.Instance.GetConfData(812).str_value.Split('|');
            lightColor = new Color(float.Parse(_strs2[0]) / 255f, float.Parse(_strs2[1]) / 255f, float.Parse(_strs2[2]) / 255f, float.Parse(_strs2[3]) / 255f);
            string[] _strs3 = CSVParam.Instance.GetConfData(813).str_value.Split('|');
            darkColor = new Color(float.Parse(_strs3[0]) / 255f, float.Parse(_strs3[1]) / 255f, float.Parse(_strs3[2]) / 255f, float.Parse(_strs3[3]) / 255f);
        }

        private void ChangeoverBtnClick()
        {
            if(isEnough)
            {
                if (null != param)
                {
                    Sys_StoneSkill.Instance.OnPowerStoneReverseReq(param.id, param.index + 1);
                    CloseSelf();
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021064));
            }
        }

        protected override void OnOpen(object arg)
        {
            param = arg as EnergysparChangeoverParam;
        }

        protected override void OnShow()
        {
            SetView();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_StoneSkill.Instance.eventEmitter.Handle<uint>(Sys_StoneSkill.EEvents.EnergysparSpecilEvent, EnergysparSpecilEvent, toRegister);
        }

        private void EnergysparSpecilEvent(uint id)
        {
            CloseSelf();
        }

        private void SetView()
        {
            isEnough = true;
            if (null != param)
            {
                StageSkillUnit stoneStageData = Sys_StoneSkill.Instance.GetStageSkillData(param.id, param.index);
                if (null != stoneStageData)
                {
                    CSVStoneStage.Data data = CSVStoneStage.Instance.GetConfData(param.id * 1000 + param.index + 1);

                    if (null != data)
                    {
                        CSVPassiveSkillInfo.Data skillData = CSVPassiveSkillInfo.Instance.GetConfData(stoneStageData.SkillId);
                        if (null != skillData)
                        {
                            ImageHelper.SetIcon(currentImage, skillData.icon);
                            TextHelper.SetText(currentDescText, LanguageHelper.GetTextContent(skillData.desc));
                        }
                        else
                        {
                            DebugUtil.LogErrorFormat("CSVPassiveSkillInfo 表格 不存在 Id = {0}", stoneStageData.SkillId);
                        }

                        bool isLight = stoneStageData.SkillType == 0;
                        TextHelper.SetText(currentText, LanguageHelper.GetTextContent((isLight ? 2021038u : 2021075u), (param.index + 1).ToString()));
                        TextHelper.SetText(changeoverText, LanguageHelper.GetTextContent((!isLight ? 2021038u : 2021075u), (param.index + 1).ToString()));
                        ImageHelper.SetIcon(changeoverImage, !isLight ? data.icon_light : data.icon_dark);
                        TextHelper.SetText(changeoverDescText, !isLight ? LanguageHelper.GetTextContent(data.desc_light) : LanguageHelper.GetTextContent(data.desc_dark));

                        currentBackImage.color = isLight ? lightColor : darkColor;
                        changeoverBackImage.color = !isLight ? lightColor : darkColor;

                        FrameworkTool.DestroyChildren(itemGrid.gameObject);
                        for (int i = 0; i < data.reverse_cost.Count; i++)
                        {
                            if (null != data.reverse_cost[i] && data.reverse_cost[i].Count >= 2)
                            {
                                if (Sys_Bag.Instance.GetItemCount(data.reverse_cost[i][0]) < data.reverse_cost[i][1] && isEnough)
                                {
                                    isEnough = false;
                                }
                                PropIconLoader.GetAsset(new PropIconLoader.ShowItemData(data.reverse_cost[i][0], data.reverse_cost[i][1],
                                    true, false, false, false, false, _bShowCount: true, _bShowBagCount: true), itemGrid, EUIID.UI_Energyspar_Changeover);
                            }
                        }

                    }
                }

            }
        }

    }
}

