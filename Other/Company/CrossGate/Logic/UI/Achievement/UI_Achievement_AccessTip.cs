using Framework;
using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Achievement_AccessTip : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
            InitView();
        }
        protected override void OnOpen(object arg)
        {
            if (arg != null)
                achData = arg as AchievementDataCell;
        }
        #endregion
        #region 组件
        Button closeBtn;
        Text textName;
        Text textTime;
        Text textContent;
        GameObject sheenObj;
        Text textServerName;
        Text textEverTime;
        #endregion
        #region 数据
        AchievementDataCell achData;
        AchievementIconCell iconCell;
        List<EverGetAchievementItem> everAchList = new List<EverGetAchievementItem>();
        #endregion
        #region 查找组件、注册事件
        private void OnParseComponent()
        {
            iconCell = new AchievementIconCell();
            iconCell.Init(transform.Find("Animator/View_SkillTips/bg/Top/Image_bg/Achievement_Item"));
            closeBtn = transform.Find("Image_Close").GetComponent<Button>();
            textName = transform.Find("Animator/View_SkillTips/bg/Top/Text_Name").GetComponent<Text>();
            textTime = transform.Find("Animator/View_SkillTips/bg/Top/Text_time").GetComponent<Text>();
            textContent = transform.Find("Animator/View_SkillTips/bg/Access/Text_Content").GetComponent<Text>();
            sheenObj = transform.Find("Animator/View_SkillTips/bg/Time").gameObject;
            textServerName = sheenObj.transform.Find("Items/Text_Server").GetComponent<Text>();
            textEverTime = sheenObj.transform.Find("Items/Text_time").GetComponent<Text>();

            closeBtn.onClick.AddListener(() => { CloseSelf(); });
        }
        #endregion
        #region 初始化
        private void InitView()
        {
            if (achData != null)
                SetData();
        }
        #endregion
        #region 界面显示
        private void SetData()
        {
            AchievementDataCell dataCell_1 = Sys_Achievement.Instance.GetAchievementByTid(achData.tid);
            achData.csvAchievementData = dataCell_1.csvAchievementData;
            iconCell.SetData(achData);
            textName.text = LanguageHelper.GetAchievementContent(achData.csvAchievementData.Achievement_Title);
            textContent.text = LanguageHelper.GetAchievementContent(achData.csvAchievementData.Task_Test, 1);
            if (achData.CheckIsMerge())
            {
                sheenObj.SetActive(true);
                for (int i = 0; i < everAchList.Count; i++)
                {
                    EverGetAchievementItem cell = everAchList[i];
                    PoolManager.Recycle(cell);
                }
                everAchList.Clear();
                int needchildCount = achData.achHistoryList.Count;
                int curchildCount = sheenObj.transform.childCount - 1;
                GameObject rTemplateGo = sheenObj.transform.GetChild(1).gameObject;
                int needInstantiateCount = needchildCount - curchildCount;
                if (needInstantiateCount <= 0)
                {
                    for (int i = 0; i < curchildCount; i++)
                    {
                        if (i < needchildCount)
                        {
                            sheenObj.transform.GetChild(i+1).gameObject.SetActive(true);
                        }
                        else
                        {
                            sheenObj.transform.GetChild(i+1).gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= curchildCount; i++)
                    {
                        sheenObj.transform.GetChild(i).gameObject.SetActive(true);
                    }
                    for (int i = 0; i < needInstantiateCount; i++)
                    {
                        FrameworkTool.CreateGameObject(rTemplateGo, sheenObj);
                    }
                }
                for (int i = 0; i < achData.achHistoryList.Count; i++)
                {
                    Transform trans = sheenObj.transform.GetChild(i+1);
                    EverGetAchievementItem cell = PoolManager.Fetch<EverGetAchievementItem>();
                    cell.Init(trans);
                    cell.SetData(achData.achHistoryList[i]);
                    everAchList.Add(cell);
                }
                FrameworkTool.ForceRebuildLayout(sheenObj);
            }
            else
            {
                sheenObj.SetActive(false);
            }
            if (achData.timestamp != 0)
            {
                textTime.gameObject.SetActive(true);
                textTime.text = TimeManager.GetDateTime(achData.timestamp).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
                textTime.gameObject.SetActive(false);
        }
        #endregion

        public class EverGetAchievementItem
        {
            Text serverName;
            Text time;
            public void Init(Transform trans)
            {
                serverName = trans.Find("Text_Server").GetComponent<Text>();
                time = trans.Find("Text_time").GetComponent<Text>();
            }
            public void SetData(AchievementDataCell.RoleAchievementHistory data)
            {
                serverName.text = data.serverName;
                time.text = TimeManager.GetDateTime(data.timestamp).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}