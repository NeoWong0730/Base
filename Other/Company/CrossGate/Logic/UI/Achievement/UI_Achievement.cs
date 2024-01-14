using Framework;
using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Achievement : UIBase
    {
        #region 组件
        Button closeBtn;
        UI_CurrencyTitle UI_CurrencyTitle;
        InfinityGrid infinityGridMain;
        Image head;
        Text textLV;
        Text textPoint;
        Text textExp;
        Slider sliderExp;
        Transform achComplete;
        List<Transform> achCompleteList = new List<Transform>();
        Button rewardBtn;
        GameObject rewardRedPoint;
        Button achReachCheckBtn;
        GameObject recentObj;
        InfinityGrid infinityGridRecent;
        Button recentBtn;
        static RectTransform targetParent;
        #endregion
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
        protected override void OnDestroy()
        {
            UI_CurrencyTitle.Dispose();
        }
        #endregion
        #region 数据
        List<AchievementDataCell> curFinishedData = new List<AchievementDataCell>();
        #endregion
        #region 组件查找、时间注册
        private void OnParseComponent()
        {
            targetParent = transform.Find("Animator").GetComponent<RectTransform>();
            closeBtn = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            infinityGridMain = transform.Find("Animator/Content/Scroll View").GetComponent<InfinityGrid>();
            head = transform.Find("Animator/View_Top/Head").GetComponent<Image>();
            textLV = transform.Find("Animator/View_Top/Text_LV/Num").GetComponent<Text>();
            textPoint = transform.Find("Animator/View_Top/Text_Point/Num").GetComponent<Text>();
            textExp = transform.Find("Animator/View_Top/Text_Exp/Num").GetComponent<Text>();
            sliderExp = transform.Find("Animator/View_Top/Text_Exp/Slider_Exp").GetComponent<Slider>();
            achComplete = transform.Find("Animator/View_Top/Text_Complete/List");
            rewardBtn = transform.Find("Animator/View_Top/Btn_01_Small").GetComponent<Button>();
            rewardRedPoint = rewardBtn.transform.Find("Image_Red").gameObject;
            achReachCheckBtn = transform.Find("Animator/View_Top/Button").GetComponent<Button>();

            recentObj = transform.Find("Animator/RecentList").gameObject;
            recentObj.SetActive(false);
            recentBtn = recentObj.transform.Find("Image").GetComponent<Button>();

            infinityGridRecent = recentObj.transform.Find("Scroll View").GetComponent<InfinityGrid>();

            for (int i = 0; i < 5; i++)
            {
                achCompleteList.Add(achComplete.GetChild(i));
            }


            closeBtn.onClick.AddListener(() => { CloseSelf(); });
            //打开奖励面板
            rewardBtn.onClick.AddListener(() => {
                UIManager.OpenUI(EUIID.UI_Achievement_Reward);
            });
            achReachCheckBtn.onClick.AddListener(() => {
                recentObj.SetActive(true);
                RefreshRecentReachCell();
            });

            recentBtn.onClick.AddListener(() => {
                recentObj.SetActive(false);
            });

            infinityGridMain.onCreateCell += OnCreateCellMain;
            infinityGridMain.onCellChange += OnCellChangeMain;
            infinityGridRecent.onCreateCell += OnCreateCellRecentReach;
            infinityGridRecent.onCellChange += OnCellChangeRecentReach;
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Achievement.Instance.eventEmitter.Handle(Sys_Achievement.EEvents.OnRefreshLevelAndExp, OnRefreshLevelAndExp, toRegister);
            Sys_Achievement.Instance.eventEmitter.Handle<bool>(Sys_Achievement.EEvents.OnRefreshAchievementData, OnRefreshAchievementData, toRegister);
            Sys_Achievement.Instance.eventEmitter.Handle(Sys_Achievement.EEvents.OnRefreshReward, OnRefreshRewardRedPoint, toRegister);
        }
        private void OnRefreshLevelAndExp()
        {
            RefreshLevelAndExp();
            CheckRewardRedPoint();
        }
        private void OnRefreshAchievementData(bool isAdd)
        {
            RefreshAchPoint();
            RefreshMainCell();
        }
        private void OnRefreshRewardRedPoint()
        {
            CheckRewardRedPoint();
        }
        #endregion
        #region 初始化
        private void InitView()
        {
            Sys_Achievement.Instance.ReqAllServerAchievement();

            Sys_Head.Instance.SetHeadAndFrameData(head);

            UI_CurrencyTitle.InitUi();
            CheckRewardRedPoint();
            RefreshLevelAndExp();
            RefreshAchPoint();
            RefreshMainCell();
        }
        #endregion
        #region 界面显示
        private void CheckRewardRedPoint()
        {
            bool isShow= Sys_Achievement.Instance.CheckRewardRedPoint();
            rewardRedPoint.SetActive(isShow);
        }
        private void RefreshAchPoint()
        {
            textPoint.text = Sys_Achievement.Instance.GetAchievementStar(0, 0, EAchievementDegreeType.Finished, true, 1, 0, true).ToString();
            for (int i = 0; i < achCompleteList.Count; i++)
            {
                SetAchComplete(i);
            }
        }
        private void RefreshLevelAndExp()
        {
            uint languageId = (uint)Sys_Achievement.Instance.GetAchievementTargetValueByLevel(2);
            languageId = languageId == 0 ? 2821000000 : languageId;
            textLV.text = LanguageHelper.GetAchievementContent(languageId);
            int targetExp = Sys_Achievement.Instance.GetAchievementTargetValueByLevel(0, false);
            textExp.text = string.Format("{0}/{1}", Sys_Achievement.Instance.curAchievementExp, targetExp);
            sliderExp.minValue = 0;
            sliderExp.maxValue = targetExp;
            sliderExp.value = Sys_Achievement.Instance.curAchievementExp;
        }
        private void SetAchComplete(int index)
        {
            string str;
            if (index != 4)
            {
                str = Sys_Achievement.Instance.GetAchievementStar(0, 0, EAchievementDegreeType.Finished, true, 3, 4 - index).ToString();
            }
            else
            {
                str = string.Format("{0}/{1}", Sys_Achievement.Instance.GetAchievementCount(0, 0, EAchievementDegreeType.Finished,true), Sys_Achievement.Instance.GetAchievementCount(0, 0, EAchievementDegreeType.All, true));
            }
            achCompleteList[index].Find("Num").GetComponent<Text>().text = str;
        }
        private void RefreshMainCell()
        {
            infinityGridMain.CellCount = Sys_Achievement.Instance.GetAchievementMainClassData().Count;
            infinityGridMain.ForceRefreshActiveCell();
            //infinityGridMain.MoveToIndex(0);
        }
        private void OnCreateCellMain(InfinityGridCell cell)
        {
            AchievementCell entry = new AchievementCell();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChangeMain(InfinityGridCell cell, int index)
        {
            AchievementCell entry = cell.mUserData as AchievementCell;
            entry.SetData(Sys_Achievement.Instance.GetAchievementMainClassData()[index]);//索引数据
        }
        private void RefreshRecentReachCell()
        {
            List<AchievementDataCell> dataList = Sys_Achievement.Instance.GetAchievementData(0, 0, EAchievementDegreeType.Finished, 1, true, false);
            curFinishedData.Clear();
            int count = dataList.Count >= 10 ? 10 : dataList.Count;
            for (int i = 0; i < count; i++)
            {
                curFinishedData.Add(dataList[i]);
            }
            infinityGridRecent.CellCount = curFinishedData.Count;
            infinityGridRecent.ForceRefreshActiveCell();
            infinityGridRecent.MoveToIndex(0);
        }
        private void OnCreateCellRecentReach(InfinityGridCell cell)
        {
            AchievementReachCell entry = new AchievementReachCell();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChangeRecentReach(InfinityGridCell cell, int index)
        {
            AchievementReachCell entry = cell.mUserData as AchievementReachCell;
            entry.SetData(curFinishedData[index]);//索引数据
        }
        #endregion
        public class AchievementCell
        {
            Image icon;
            Text textName;
            Text textNum;
            Slider slider;
            Button btn;
            GameObject obgNormal;
            GameObject obgGold;
            CSVAchievementType.Data data;
            public void Init(Transform trans)
            {
                icon = trans.Find("Icon").GetComponent<Image>();
                textName = trans.Find("Text").GetComponent<Text>();
                textNum = trans.Find("Text_Num").GetComponent<Text>();
                slider = trans.Find("Slider").GetComponent<Slider>();
                btn = trans.Find("Icon").GetComponent<Button>();
                obgNormal = trans.Find("BG01").gameObject;
                obgGold = trans.Find("BG02").gameObject;
                btn.onClick.AddListener(OnClickOpen);
            }
            public void SetData(AchievementMainClassData data)
            {
                this.data = data.data;
                SetBack();
                ImageHelper.SetIcon(icon, this.data.MainClassIcon);
                textName.text = LanguageHelper.GetAchievementContent(this.data.MainClassTest);
                int finishCount = Sys_Achievement.Instance.GetAchievementCount(this.data.id, 0, EAchievementDegreeType.Finished);
                int allCount = Sys_Achievement.Instance.GetAchievementCount(this.data.id);
                slider.minValue = 0;
                if (data.tid == Sys_Achievement.Instance.GetSheenAchievementMainClassData().tid)
                {
                    textNum.text = 1.ToString("P2");
                    slider.maxValue = 1;
                    slider.value = 1;
                }
                else
                {
                    textNum.text = (finishCount / (float)allCount).ToString("P2");
                    slider.maxValue = allCount;
                    slider.value = finishCount;
                }
            }
            private void SetBack()
            {
                //服务器成就
                if (data.id != Sys_Achievement.Instance.GetServerAchievementMainClassData().tid)
                {
                    obgNormal.SetActive(false);
                    obgGold.SetActive(true);
                }
                else
                {
                    obgNormal.SetActive(true);
                    obgGold.SetActive(false);
                }
            }
            private void OnClickOpen()
            {
                if (data != null)
                {
                    OpenAchievementMenuParam param = new OpenAchievementMenuParam()
                    {
                        mainCalss = data.id,
                        subCalss = Sys_Achievement.Instance.GetSubAchievementTypeDataList(data.id)[0].subClass,
                        tid = 0,
                    };
                    UIManager.OpenUI(EUIID.UI_Achievement_Menu, false, param);
                }
            }
        }
        public class AchievementReachCell
        {
            Text textName;
            Transform starList;
            Text textTime;
            Button openMenuBtn;
            Button shareBtn;
            AchievementDataCell data;
            Transform[] starArray = new Transform[4];
            AchievementIconCell iconCell;
            public void Init(Transform tran)
            {
                iconCell = new AchievementIconCell();
                openMenuBtn = tran.Find("Image_Bottom").GetComponent<Button>();
                iconCell.Init(tran.Find("Achievement_Item"));
                textName = tran.Find("Text_Name").GetComponent<Text>();
                starList = tran.Find("Text_Name/Starlist");
                textTime = tran.Find("Text_Time").GetComponent<Text>();
                shareBtn = tran.Find("Button_Share").GetComponent<Button>();
                for (int i = 0; i < 4; i++)
                {
                    starArray[i] = starList.GetChild(i);
                }
                openMenuBtn.onClick.AddListener(OpenMenuBtn);
                shareBtn.onClick.AddListener(ShareOnClick);
            }
            public void SetData(AchievementDataCell data)
            {
                this.data = data;
                iconCell.SetData(data);
                textName.text = LanguageHelper.GetAchievementContent(data.csvAchievementData.Achievement_Title);
                textTime.text = TimeManager.GetDateTime(data.timestamp).ToString("yyyy-MM-dd HH:mm:ss");
                SetStar();
            }
            private void SetStar()
            {
                for (int i = 0; i < starArray.Length; i++)
                {
                    starArray[i].gameObject.SetActive(false);
                }
                for (int i = 0; i < data.csvAchievementData.Rare; i++)
                {
                    starArray[i].gameObject.SetActive(true);
                }
            }
            private void OpenMenuBtn()
            {
                OpenAchievementMenuParam param = new OpenAchievementMenuParam()
                {
                    tid = data.tid,
                };
                UIManager.OpenUI(EUIID.UI_Achievement_Menu, false, param);
            }
            private void ShareOnClick()
            {
                ClickShowPositionData clickData = new ClickShowPositionData();
                clickData.data = data;
                clickData.clickTarget = shareBtn.GetComponent<RectTransform>();
                clickData.parent = targetParent;
                UIManager.OpenUI(EUIID.UI_Achievement_ShareList, false, clickData);
            }
        }
    }
    public class AchievementIconCell
    {
        Transform iconTrans;
        Image iconGolden;
        Image iconSilver;
        Image iconCoppery;
        Image iconColour;
        Transform bossIconTrans;
        Image iconBoss;
        Image iconBossFrame;
        public void Init(Transform trans)
        {
            iconTrans = trans.Find("Icon");
            iconGolden = iconTrans.Find("Golden").GetComponent<Image>();
            iconSilver = iconTrans.Find("Silver").GetComponent<Image>();
            iconCoppery = iconTrans.Find("Coppery").GetComponent<Image>();
            iconColour = iconTrans.Find("Colour").GetComponent<Image>();
            bossIconTrans = trans.Find("BossIcon");
            iconBoss = bossIconTrans.Find("HeadIcon").GetComponent<Image>();
            iconBossFrame = bossIconTrans.Find("Frame").GetComponent<Image>();
        }
        public void SetData(AchievementDataCell data)
        {
            //Boss头像特殊处理
            if (data.csvAchievementData.Icon_Id >= 8020001)
            {
                bossIconTrans.gameObject.SetActive(true);
                iconTrans.gameObject.SetActive(false);
                var iconData = CSVAchievementIconMix.Instance.GetConfData(data.csvAchievementData.Icon_Id);
                if (iconData != null)
                {
                    ImageHelper.SetIcon(iconBoss, iconData.IconId1);
                    ImageHelper.SetIcon(iconBossFrame, iconData.IconId2);
                }
                else
                    DebugUtil.LogError("CSVAchievementIconMix not found id："+ data.csvAchievementData.Icon_Id);
            }
            else
            {
                bossIconTrans.gameObject.SetActive(false);
                iconTrans.gameObject.SetActive(true);
                if (data.csvAchievementData.Rare == 1)
                {
                    iconGolden.gameObject.SetActive(false);
                    iconSilver.gameObject.SetActive(false);
                    iconCoppery.gameObject.SetActive(true);
                    iconColour.gameObject.SetActive(false);
                    ImageHelper.SetIcon(iconCoppery, data.csvAchievementData.Icon_Id);
                }
                else if (data.csvAchievementData.Rare == 2)
                {
                    iconGolden.gameObject.SetActive(false);
                    iconSilver.gameObject.SetActive(true);
                    iconCoppery.gameObject.SetActive(false);
                    iconColour.gameObject.SetActive(false);
                    ImageHelper.SetIcon(iconSilver, data.csvAchievementData.Icon_Id);
                }
                else if (data.csvAchievementData.Rare == 3)
                {
                    iconGolden.gameObject.SetActive(true);
                    iconSilver.gameObject.SetActive(false);
                    iconCoppery.gameObject.SetActive(false);
                    iconColour.gameObject.SetActive(false);
                    ImageHelper.SetIcon(iconGolden, data.csvAchievementData.Icon_Id);
                }
                else if (data.csvAchievementData.Rare == 4)
                {
                    iconGolden.gameObject.SetActive(false);
                    iconSilver.gameObject.SetActive(false);
                    iconCoppery.gameObject.SetActive(false);
                    iconColour.gameObject.SetActive(true);
                    ImageHelper.SetIcon(iconColour, data.csvAchievementData.Icon_Id);
                }
            }
        }
    }
}