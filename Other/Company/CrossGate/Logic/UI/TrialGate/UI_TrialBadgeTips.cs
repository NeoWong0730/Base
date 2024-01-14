using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using UnityEngine.UI;
using Lib.Core;

namespace Logic
{
    /// <summary>
    ///徽章打开是需要的参数
    /// </summary>
    public class TrialBadgeOpenParam
    {
        public uint type;//1(打开徽章描述) 2(打开Boss列表)
        public uint badgeId;
        public Vector3 Pos = Vector3.zero;
    }
    //试炼徽章详情界面
    public class UI_TrialBadgeTips : UIBase
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
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        protected override void OnOpen(object arg)
        {
            if (arg != null)
                curBadgeParam = arg as TrialBadgeOpenParam;
        }
        #endregion
        #region 组件
        Transform transAni;
        Button btnClose;
        Transform transViewList;
        Text targetTitle, targetNum;
        InfinityGrid infinityGrid;

        Transform transViewMessage;
        Image badgeIcon;
        Text badgeName, badgeDes, badgeSourceDes;
        Button btnBadgeGet;
        #endregion
        #region 数据
        TrialBadgeOpenParam curBadgeParam;
        BadgeData curBadgeData;
        List<BadgeData.BossData> bossDataList = new List<BadgeData.BossData>();
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            transAni = transform.Find("Animator");
            btnClose = transform.Find("Blank").GetComponent<Button>();
            transViewList = transform.Find("Animator/View_List");
            targetTitle = transViewList.Find("Text_Title").GetComponent<Text>();
            targetNum = transViewList.Find("Text").GetComponent<Text>();
            infinityGrid = transViewList.Find("Scroll View").GetComponent<InfinityGrid>();

            transViewMessage = transform.Find("Animator/View_Message");
            badgeIcon = transViewMessage.Find("bg/Top/Image_bg/Icon").GetComponent<Image>();
            badgeName = transViewMessage.Find("bg/Top/Text_Name").GetComponent<Text>();
            badgeDes = transViewMessage.Find("bg/Top/Text2").GetComponent<Text>();
            badgeSourceDes = transViewMessage.Find("bg/Text_Content").GetComponent<Text>();
            btnBadgeGet = transViewMessage.Find("bg/Button").GetComponent<Button>();

            btnClose.onClick.AddListener(() => { CloseSelf(); });
            btnBadgeGet.onClick.AddListener(BadgeGetClick);

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {

        }
        private void BadgeGetClick()
        {
            if (!transViewList.gameObject.activeSelf)
            {
                bossDataList = curBadgeData.GetCurLevelStageBossData(Sys_ActivityTrialGate.Instance.GetCurLevelGradeId());
                if (bossDataList != null && bossDataList.Count > 0)
                {
                    transViewList.gameObject.SetActive(true);
                    RefreshBossData();
                }
                else
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000026));
            }
        }
        #endregion
        private void InitView()
        {
            if (curBadgeParam != null)
            {
                //if (!curBadgeParam.Pos.Equals(Vector3.zero))
                //{
                //    Vector2 screenPoint = new Vector2(curBadgeParam.Pos.x, curBadgeParam.Pos.y);
                //    RectTransformUtility.ScreenPointToWorldPointInRectangle(gameObject.GetComponent<RectTransform>(), screenPoint, CameraManager.mUICamera, out Vector3 pos);
                //    transAni.position = pos;
                //}
                curBadgeData = Sys_ActivityTrialGate.Instance.GeBadgeDataByTid(curBadgeParam.badgeId);
                if (curBadgeData != null)
                {
                    if (curBadgeParam.type == 1)
                        SetViewMessage();
                    else
                        SetViewBossList();
                }
            }
        }
        private void SetViewMessage()
        {
            transViewMessage.gameObject.SetActive(true);
            transViewList.gameObject.SetActive(false);
            ImageHelper.SetIcon(badgeIcon, curBadgeData.csv_trialBadge.bigIcon_id);
            badgeName.text = LanguageHelper.GetTextContent(curBadgeData.csv_trialBadge.badge_name);
            badgeDes.text = LanguageHelper.GetTextContent(curBadgeData.csv_trialBadge.badge_description);
            badgeSourceDes.text = LanguageHelper.GetTextContent(curBadgeData.csv_trialBadge.badge_source);
        }
        private void SetViewBossList()
        {
            transViewMessage.gameObject.SetActive(false);
            transViewList.gameObject.SetActive(true);
            RefreshBossData();
        }
        private void RefreshBossData()
        {
            bossDataList = curBadgeData.GetCurLevelStageBossData(Sys_ActivityTrialGate.Instance.GetCurLevelGradeId());
            if (bossDataList != null && bossDataList.Count > 0)
            {
                CSVTrialLevelGrade.Data levelGradeData = Sys_ActivityTrialGate.Instance.GetTrialLevelGrade();
                CSVDailyActivity.Data activityData = CSVDailyActivity.Instance.GetConfData(levelGradeData.dailyActivites);
                if (activityData != null)
                {
                    uint curTimes = Sys_Daily.Instance.getDailyCurTimes(levelGradeData.dailyActivites);//当前次数;
                    uint maxTimes = activityData.limite;
                    targetNum.text = LanguageHelper.GetTextContent(3899000022, (maxTimes - curTimes).ToString());
                }
                else
                    targetNum.text = string.Empty;
                targetTitle.text = LanguageHelper.GetTextContent(3899000021, LanguageHelper.GetTextContent(curBadgeData.csv_trialBadge.badge_name));
                infinityGrid.CellCount = bossDataList.Count;
                infinityGrid.ForceRefreshActiveCell();
            }
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            BossItem entry = new BossItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            BossItem entry = cell.mUserData as BossItem;
            entry.SetData(bossDataList[index], FindNpcOnClick);//索引数据
        }
        private void FindNpcOnClick()
        {
            UIManager.CloseUI(EUIID.UI_TrialGateMain);
            UIManager.CloseUI(EUIID.UI_TrialSkillDeploy);
            CloseSelf();
        }
        #region class
        public class BossItem
        {
            Text textName, textNum;
            Image icon;
            Button btnGo;

            CSVBOSSInformation.Data csv_bossInfo;
            CSVBOSSManual.Data csv_bossManual;
            Action action;
            public void Init(Transform trans)
            {
                textName = trans.Find("Text_Name").GetComponent<Text>();
                textNum = trans.Find("Text_Num").GetComponent<Text>();
                icon = trans.Find("Image_Icon").GetComponent<Image>();
                btnGo = trans.Find("Btn_01_Small").GetComponent<Button>();

                btnGo.onClick.AddListener(FindBossClick);
            }
            public void SetData(BadgeData.BossData data,Action action)
            {
                this.action = action;
                textNum.text = LanguageHelper.GetTextContent(3899000023, data.giveBadgeNum.ToString());
                csv_bossInfo = CSVBOSSInformation.Instance.GetConfData(data.bossId);
                if (csv_bossInfo != null)
                {
                    csv_bossManual = CSVBOSSManual.Instance.GetConfData(csv_bossInfo.bossManual_id);
                    if (csv_bossManual != null)
                    {
                        ImageHelper.SetIcon(icon, csv_bossManual.head_icon);
                        textName.text = LanguageHelper.GetTextContent(csv_bossManual.BOSS_name);
                    }
                    else
                        DebugUtil.LogError("CSVBOSSManual nof found id：" + csv_bossInfo.bossManual_id);
                }
                else
                    DebugUtil.LogError("CSVBOSSInformation nof found id："+ data.bossId);
            }
            private void FindBossClick()
            {
                if (Sys_ActivityTrialGate.Instance.curEnterBattleState == Sys_ActivityTrialGate.EEnterBattleState.Ready)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000035));
                else
                {
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(csv_bossInfo.targetNPC);
                    action?.Invoke();
                }
            }
        }

        #endregion
    }
}