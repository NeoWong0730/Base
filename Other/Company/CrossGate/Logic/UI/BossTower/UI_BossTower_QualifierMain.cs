using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using UnityEngine.UI;
using Lib.Core;
using Packet;
using Framework;
using DG.Tweening;

namespace Logic
{
    //资格挑战主界面
    public class UI_BossTower_QualifierMain : UIBase
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
        protected override void OnLateUpdate(float dt, float usdt)
        {
            LateUpdate();
        }
        protected override void OnClose()
        {
            floorItemList.Clear();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 组件
        Button btnClose;
        Button btnRank;
        //right
        Text lvlTitle, adviceLv, adviceScore, adviceLimit, content;
        Button btnFastTeam, btnStartFight;
        Text curRnak;
        GameObject objTips, objTips2;
        Text promoteBaseNum, promoteExtraNum;
        Button btnAdd;
        Text timeTitle,diffTime;
        InfinityGrid infinityGrid;
        static RectTransform root;
        #endregion
        #region 数据
        List<BossTowerQualifierData> bossTowerQualifierDataList=new List<BossTowerQualifierData>();
        List<FloorItem> floorItemList = new List<FloorItem>();
        static uint curSelectTid;
        static uint nextTid;
        bool resetTimeDirty;
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            root = transform;
            btnClose = transform.Find("Animator/View_FullTitle02_New/Btn_Close").GetComponent<Button>();
            btnRank = transform.Find("Animator/View_Buttons/Button_Rank").GetComponent<Button>();
            infinityGrid = transform.Find("Animator/Scroll View").GetComponent<InfinityGrid>();
            lvlTitle = transform.Find("Animator/View_Right/Text_Name").GetComponent<Text>();
            adviceLv = transform.Find("Animator/View_Right/Text_Lv/Num").GetComponent<Text>();
            adviceScore = transform.Find("Animator/View_Right/Text_Score/Num").GetComponent<Text>();
            adviceLimit = transform.Find("Animator/View_Right/Text_Limit/Num").GetComponent<Text>();
            content = transform.Find("Animator/View_Right/Intro/Text_Content").GetComponent<Text>();
            btnFastTeam = transform.Find("Animator/View_Right/Btn_01").GetComponent<Button>();
            btnStartFight = transform.Find("Animator/View_Right/Btn_02").GetComponent<Button>();
            curRnak = transform.Find("Animator/View_Right/Text_Rank").GetComponent<Text>();
            objTips = transform.Find("Animator/View_Right/Text_Tips").gameObject;
            objTips2 = transform.Find("Animator/View_Right/Text_Tips2").gameObject;
            promoteBaseNum = transform.Find("Animator/Image_Promotion/Text_Number").GetComponent<Text>();
            promoteExtraNum = transform.Find("Animator/Image_Promotion/Text_Add").GetComponent<Text>();
            btnAdd = transform.Find("Animator/Image_Promotion/Button_Add").GetComponent<Button>();
            timeTitle = transform.Find("Animator/Image_Time/Text").GetComponent<Text>();
            diffTime= transform.Find("Animator/Image_Time/Text_Time").GetComponent<Text>();

            btnClose.onClick.AddListener(() => { CloseSelf(); });
            btnRank.onClick.AddListener(() => { UIManager.OpenUI(EUIID.UI_BossTower_QualifierRank); });
            btnFastTeam.onClick.AddListener(()=> { Sys_ActivityBossTower.Instance.FastTeam(false); });
            btnStartFight.onClick.AddListener(()=> {
                Sys_ActivityBossTower.Instance.OnBossTowerChallengeReq(false, curSelectTid);
            });
            btnAdd.onClick.AddListener(()=> { UIManager.OpenUI(EUIID.UI_BossTower_QualifierExplain); });

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_ActivityBossTower.Instance.eventEmitter.Handle(Sys_ActivityBossTower.EEvents.OnRefreshSelfRankData, RefreshSelfRnak, toRegister);
            Sys_ActivityBossTower.Instance.eventEmitter.Handle(Sys_ActivityBossTower.EEvents.OnRefreshBossTowerReset, OnRefreshBossTowerReset, toRegister);
        }
        private void OnRefreshBossTowerReset()
        {
            RefreshFloorItem();
            RefreshDiffTime();
            RefreshSelfRnak();
        }
        #endregion
        private void InitView()
        {
            resetTimeDirty = true;
            Sys_ActivityBossTower.Instance.OnBossTowerSelfRankReq();
            RefreshFloorItem();
            RefreshDiffTime();
            RefreshSelfRnak();
        }
        private void LateUpdate()
        {
            if (resetTimeDirty)
            {
                RefreshDiffTime();
            }
        }
        private void RefreshSelfRnak()
        {
            string str = string.Format("<color=#F1372D>{0}</color>/{1}", Sys_ActivityBossTower.Instance.curQualifierNum, Sys_ActivityBossTower.Instance.baseRankNum);
            if (Sys_ActivityBossTower.Instance.curQualifierNum < Sys_ActivityBossTower.Instance.baseRankNum)
                str = string.Format("<color=#79FF13>{0}</color>/{1}", Sys_ActivityBossTower.Instance.curQualifierNum, Sys_ActivityBossTower.Instance.baseRankNum);
            promoteBaseNum.text = str;
            promoteExtraNum.text = string.Format("+{0}", Sys_ActivityBossTower.Instance.extRankNum);

            curRnak.text = Sys_ActivityBossTower.Instance.GetCurRnakStateDescribe(1);

            bool isHave = Sys_ActivityBossTower.Instance.CheckIsHaveBossFightQualifier();
            objTips.SetActive(!isHave);
            objTips2.SetActive(isHave);
        }
        private void RefreshDiffTime()
        {
            int second = Sys_ActivityBossTower.Instance.GetActivityRestTimeDiff();
            if (second <= 0)
            {
                second = 0;
                resetTimeDirty = false;
            }
            else
                resetTimeDirty = true;
            uint lanId = 1009319;
            if (Sys_ActivityBossTower.Instance.curBossTowerState == BossTowerState.ChallengeOver)
                lanId = 1009320;
            timeTitle.text = LanguageHelper.GetTextContent(lanId);
            diffTime.text = LanguageHelper.TimeToString((uint)(second), LanguageHelper.TimeFormat.Type_1);
        }
        private void RefreshFloorItem()
        {
            nextTid = Sys_ActivityBossTower.Instance.GetBossTowerNextTid(1);
            curSelectTid = nextTid;
            List<BossTowerQualifierData> dataList= Sys_ActivityBossTower.Instance.GetBossTowerQualifierDataList();
            if (dataList != null)
            {
                bossTowerQualifierDataList.Clear();
                bossTowerQualifierDataList.AddRange(dataList);
                bossTowerQualifierDataList.Sort((a, b) => {
                    return (int)(b.tid - a.tid);
                });
            }
            if (bossTowerQualifierDataList != null && bossTowerQualifierDataList.Count>0)
            {
                int curIndex = bossTowerQualifierDataList.FindIndex(o => o.tid == curSelectTid);
                bossTowerQualifierDataList.Add(null);
                infinityGrid.CellCount = bossTowerQualifierDataList.Count;
                infinityGrid.Apply();
                infinityGrid.MoveToIndex(curIndex);
                infinityGrid.ForceRefreshActiveCell();
            }
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            FloorItem entry = new FloorItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
            floorItemList.Add(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            FloorItem entry = cell.mUserData as FloorItem;
            entry.SetData(bossTowerQualifierDataList[index], OnRefreshSelect);//索引数据
        }
        private void OnRefreshSelect(uint tid)
        {
            curSelectTid = tid;
            if (floorItemList != null && floorItemList.Count > 0)
            {
                for (int i = 0; i < floorItemList.Count; i++)
                {
                    if (floorItemList[i].data != null)
                    {
                        if (floorItemList[i].data.tid == tid)
                        {
                            floorItemList[i].Select();
                            RefreshRightShow(floorItemList[i].data);
                        }
                        else
                            floorItemList[i].Release();
                    }
                }
            }
        }
        private void RefreshRightShow(BossTowerQualifierData data)
        {
            lvlTitle.text = LanguageHelper.GetTextContent(data.csvData.name);
            adviceLv.text = string.Format("{0}-{1}", data.csvData.levelGrade_taxt[0], data.csvData.levelGrade_taxt[1]);
            adviceScore.text = data.csvData.recommondPoint.ToString();
            adviceLimit.text = string.Format("{0}-{1}", data.csvData.playerLimit[0], data.csvData.playerLimit[1]);
            content.text = LanguageHelper.GetTextContent(data.csvData.diffcultyDetails);
        }
        #region item
        public class FloorItem
        {
            Transform viewTrans;
            GameObject objOff, objOn, objSelect,objFinish;
            Button btnRewardClose,btnRewardOpen;
            Text flooerNum;
            Button btnTopSelect, btnFloorSelect;
            GameObject objTopBg, objFloorBg, objBottomBg;
            GameObject Fx_UI_BossGame_YellowTa_Floor, Fx_UI_BossGame_YellowTa_Top;
            public BossTowerQualifierData data;
            Action<uint> action;
            public void Init(Transform trans)
            {
                objBottomBg = trans.Find("Image_BottomBG").gameObject;
                viewTrans = trans.Find("View");
                objTopBg = viewTrans.Find("Image_TopBG").gameObject;
                Fx_UI_BossGame_YellowTa_Top = viewTrans.Find("Image_TopBG/Fx_UI_BossGame_YellowTa_Top").gameObject;
                objFloorBg = viewTrans.Find("Image_FloorBG").gameObject;
                Fx_UI_BossGame_YellowTa_Floor = viewTrans.Find("Image_FloorBG/Fx_UI_BossGame_YellowTa_Floor").gameObject;
                btnTopSelect = viewTrans.Find("Image_TopBG").GetComponent<Button>();
                btnFloorSelect = viewTrans.Find("Image_FloorBG").GetComponent<Button>();
                objOff = viewTrans.Find("Image_OFF").gameObject;
                objOn = viewTrans.Find("Image_On").gameObject;
                objSelect = viewTrans.Find("Image_Select").gameObject;
                objFinish = viewTrans.Find("Image_Finish").gameObject;
                btnRewardClose = viewTrans.Find("Image_RewardBG/Button_Icon").GetComponent<Button>();
                btnRewardOpen = viewTrans.Find("Image_RewardBG/Button_IconOpen").GetComponent<Button>();
                flooerNum = viewTrans.Find("Text").GetComponent<Text>();

                btnTopSelect.onClick.AddListener(OnClickSelect);
                btnFloorSelect.onClick.AddListener(OnClickSelect);
                btnRewardClose.onClick.AddListener(()=> { OnClickReward(1); });
                btnRewardOpen.onClick.AddListener(() => { OnClickReward(2); });
            }
            public void SetData(BossTowerQualifierData data, Action<uint> action)
            {
                this.data = data;
                this.action = action;
                if (data == null)
                {
                    objBottomBg.SetActive(true);
                    viewTrans.gameObject.SetActive(false);
                }
                else
                {
                    objBottomBg.SetActive(false);
                    viewTrans.gameObject.SetActive(true);
                    uint lastTid = Sys_ActivityBossTower.Instance.GetBossTowerFirstOrLastDataTid(1, false);
                    if (data.csvData.floor_number == Sys_ActivityBossTower.Instance.GetBossTowerQualifierData(lastTid).csvData.floor_number)
                    {
                        objTopBg.SetActive(true);
                        objFloorBg.SetActive(false);
                    }
                    else
                    {
                        objTopBg.SetActive(false);
                        objFloorBg.SetActive(true);
                    }
                    if (data.tid == nextTid)
                    {
                        StateShow(data.isPass ? 2u : 1u);
                    }
                    else
                    {
                        StateShow(data.isPass ? 2u : 3u);
                    }
                    if (data.tid == curSelectTid)
                    {
                        Select();
                        if (data != null)
                            action?.Invoke(data.tid);
                    }
                    else
                        Release();
                    flooerNum.text = LanguageHelper.GetTextContent(1009321, data.csvData.floor_number.ToString());
                }
            }
            private void StateShow(uint type)
            {
                //当前要挑战的(已通过层数+1)
                if (type == 1)
                {
                    objOff.SetActive(false);
                    objOn.SetActive(true);
                    objFinish.SetActive(false);
                    btnRewardClose.gameObject.SetActive(true);
                    btnRewardOpen.gameObject.SetActive(false);
                    if (objTopBg.activeSelf) ImageHelper.SetImageGray(objTopBg.GetComponent<Image>(), false, true);
                    if (objFloorBg.activeSelf) ImageHelper.SetImageGray(objFloorBg.GetComponent<Image>(), false, true);
                }
                else if (type == 2)//已通关
                {
                    objOff.SetActive(false);
                    objOn.SetActive(true);
                    objFinish.SetActive(true);
                    btnRewardClose.gameObject.SetActive(false);
                    btnRewardOpen.gameObject.SetActive(true);
                    if (objTopBg.activeSelf) ImageHelper.SetImageGray(objTopBg.GetComponent<Image>(), false, true);
                    if (objFloorBg.activeSelf) ImageHelper.SetImageGray(objFloorBg.GetComponent<Image>(), false, true);
                }
                else//未通关
                {
                    objOff.SetActive(true);
                    objOn.SetActive(false);
                    objFinish.SetActive(false);
                    btnRewardClose.gameObject.SetActive(true);
                    btnRewardOpen.gameObject.SetActive(false);
                    if (objTopBg.activeSelf) ImageHelper.SetImageGray(objTopBg.GetComponent<Image>(), true, true);
                    if (objFloorBg.activeSelf) ImageHelper.SetImageGray(objFloorBg.GetComponent<Image>(), true, true);
                }
            }
            public void Select()
            {
                objSelect.gameObject.SetActive(true);
                if(objTopBg.activeSelf) Fx_UI_BossGame_YellowTa_Top.SetActive(true);
                if(objFloorBg.activeSelf) Fx_UI_BossGame_YellowTa_Floor.SetActive(true);
            }
            public void Release()
            {
                objSelect.gameObject.SetActive(false);
                if (objTopBg.activeSelf) Fx_UI_BossGame_YellowTa_Top.SetActive(false);
                if (objFloorBg.activeSelf) Fx_UI_BossGame_YellowTa_Floor.SetActive(false);
            }
            private void OnClickSelect()
            {
                if (data != null)
                    action?.Invoke(data.tid);
            }
            private void OnClickReward(uint type)
            {
                if (data != null && data.csvData != null)
                {
                    Button btn = type == 1 ? btnRewardClose : btnRewardOpen;
                    RectTransform btnRect = btn.gameObject.GetComponent<RectTransform>();
                    //Vector2 screenPos = CameraManager.mUICamera.WorldToScreenPoint(btnRect.position);
                    //RectTransformUtility.ScreenPointToLocalPointInRectangle(root, screenPos, UIManager.mUICamera, out Vector2 localPos);
                    //camera为null，btnRect的position相当于屏幕坐标 无需世界转屏幕
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(root, btnRect.position, null, out Vector2 localPos);
                    float diffY = root.sizeDelta.y / 2 - Sys_ActivityBossTower.Instance.rewardViewHeight / 2;
                    float posY = localPos.y >= diffY ? diffY : localPos.y <= -diffY ? -diffY : localPos.y;
                    Vector3 newPos = new Vector3(localPos.x - Sys_ActivityBossTower.Instance.rewardViewWidth / 2, posY, 0);
                    RewardPanelParam _param = new RewardPanelParam();
                    _param.propList = CSVDrop.Instance.GetDropItem(data.csvData.floor_drop);
                    _param.Pos = newPos;
                    _param.isLocalPos = true;
                    UIManager.OpenUI(EUIID.UI_RewardPanel, false, _param);
                }
            }
        }
        #endregion
    }
}