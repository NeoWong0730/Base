using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Framework;
using Table;
using Lib.Core;
using System;
using System.Text;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace Logic
{
    public class UI_TitleTips : UIBase
    {
        private Text titleName;
        private Text point;
        private Text condition;
        private Transform attrParent;
        private Image eventBG;
        private Title title;
        private string conditionGet;

        protected override void OnOpen(object arg)
        {
            title = arg as Title;
        }

        protected override void OnLoaded()
        {
            titleName = transform.Find("Animator/View_Content/Text_Name").GetComponent<Text>();
            point = transform.Find("Animator/View_Content/Text_Point").GetComponent<Text>();
            condition = transform.Find("Animator/View_Content/Text_Condition").GetComponent<Text>();
            attrParent = transform.Find("Animator/View_Content/Grid01");
            eventBG = transform.Find("Animator/eventBg").GetComponent<Image>();

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventBG);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (_) => { UIManager.CloseUI(EUIID.UI_TitleTips); });
        }

        protected override void OnShow()
        {
            Refresh();
        }

        private void Refresh()
        {
            TextHelper.SetText(titleName, title.cSVTitleData.titleLan);
            point.text = title.cSVTitleData.titlePoint.ToString();

            string conditionGet = CSVLanguage.Instance.GetConfData(title.cSVTitleData.titleGetLan).words;

            if (title.cSVTitleData.titleGetType == 1)//npc功能组
            {
                string str0 = Sys_Reputation.Instance.GetRankTitleByReputationLevel(title.cSVTitleData.titleGet[0]);
                string str1 = (title.cSVTitleData.titleGet[0] % 100).ToString();
                string str2 = CSVNpcLanguage.Instance.GetConfData(CSVNpc.Instance.GetConfData(title.cSVTitleData.titleGet[1]).name).words;
                TextHelper.SetText(condition, title.active ? 2005901u : 2005902u, string.Format(conditionGet, str0, str1, str2));
            }
            else if (title.cSVTitleData.titleGetType == 2)//成就
            {
                AchievementDataCell data = Sys_Achievement.Instance.GetAchievementByTid(title.cSVTitleData.titleGet[0]);
                TextHelper.SetText(condition, title.active ? 2005901u : 2005902u, string.Format(conditionGet, LanguageHelper.GetAchievementContent(data.csvAchievementData.Achievement_Title)));
            }
            else if (title.cSVTitleData.titleGetType == 3)//任务
            {
                string str = CSVTaskLanguage.Instance.GetConfData(CSVTask.Instance.GetConfData(title.cSVTitleData.titleGet[0]).taskName).words;
                TextHelper.SetText(condition, title.active ? 2005901u : 2005902u, string.Format(conditionGet, str));
            }
            else if (title.cSVTitleData.titleGetType == 4 || title.cSVTitleData.titleGetType == 5)//道具
            {
                string str = CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(title.cSVTitleData.titleGet[0]).name_id).words;
                TextHelper.SetText(condition, title.active ? 2005901u : 2005902u, string.Format(conditionGet, str));
            }
            else if (title.cSVTitleData.titleGetType == 6)  //职业进阶
            {
                uint titleGet = title.cSVTitleData.titleGet[0];
                uint careerid = titleGet / 100;
                uint level = titleGet % 100;
                string str1 = CSVLanguage.Instance.GetConfData(CSVCareer.Instance.GetConfData(careerid).name).words;
                string str2 = level.ToString();
                TextHelper.SetText(condition, title.active ? 2005901u : 2005902u, string.Format(conditionGet, str1, str2));
            }
            else if (title.cSVTitleData.titleGetType == 7)//生活技能段位
            {
                TextHelper.SetText(condition, title.active ? 2005901u : 2005902u, string.Format(conditionGet, title.cSVTitleData.titleGet[0]));
            }
            else if (title.cSVTitleData.titleGetType == 8)   //生活技能等级
            {
                string skillLifeName = CSVLanguage.Instance.GetConfData(CSVLifeSkill.Instance.GetConfData(title.cSVTitleData.titleGet[0]).name_id).words;
                uint skillLifeLevel = title.cSVTitleData.titleGet[1];
                TextHelper.SetText(condition, title.active ? 2005901u : 2005902u, string.Format(conditionGet, skillLifeName, skillLifeLevel));
            }
            else if (title.cSVTitleData.titleGetType == 9) //家族称号
            {
                TextHelper.SetText(condition, title.active ? 2005901u : 2005902u, conditionGet);
            }
            else if (title.cSVTitleData.titleGetType == 10) //导师等级
            {
                CSVTutor.Data data = CSVTutor.Instance.GetConfData(title.cSVTitleData.titleGet[0] + 1);
                string tutorName = CSVLanguage.Instance.GetConfData(data.TutorLevelLan).words;
                TextHelper.SetText(condition, title.active ? 2005901u : 2005902u, string.Format(conditionGet, tutorName));
            }
            else if (title.cSVTitleData.titleGetType == 11)
            {
                uint reachLv = title.cSVTitleData.titleGet[0];
                TextHelper.SetText(condition, title.active ? 2005901u : 2005902u, string.Format(conditionGet, reachLv.ToString()));
            }
            ContructAttr();
        }

        private void ContructAttr()
        {
            FrameworkTool.ForceRebuildLayout(attrParent.gameObject);
            if (title.cSVTitleData.titleProperty != null)
            {
                int attrCount = title.cSVTitleData.titleProperty.Count;
                FrameworkTool.CreateChildList(attrParent, attrCount);
                for (int i = 0; i < attrCount; i++)
                {
                    Transform child = attrParent.GetChild(i);
                    Text attrName = child.GetComponent<Text>();
                    Text num = child.Find("Text").GetComponent<Text>();
                    uint attrid = title.cSVTitleData.titleProperty[i][0];
                    uint attrnum = title.cSVTitleData.titleProperty[i][1];
                    SetAttr(attrid, attrnum, attrName, num);
                    //TextHelper.SetText(attrName, CSVAttr.Instance.GetConfData(attrid).name);
                    //num.text = attrnum.ToString();
                }
            }
        }

        private void SetAttr(uint attr1, uint value1, Text attrName1, Text attrValue1)
        {
            CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(attr1);
            TextHelper.SetText(attrName1, cSVAttrData.name);
            if (cSVAttrData.show_type == 1)
            {
                attrValue1.text = string.Format("+{0}", value1.ToString());
            }
            else
            {
                attrValue1.text = string.Format("+{0}%", (value1 / 100f).ToString());
            }
        }
    }
}

