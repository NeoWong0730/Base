using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using UnityEngine.UI;
using Lib.Core;
using Packet;
using Framework;

namespace Logic
{
    //Boss战主界面
    public class UI_BossTower_FightMain : UIBase
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
        protected override void OnHide()
        {
            OnDestroyModel();
        }
        protected override void OnClose()
        {
            floorItemList.Clear();
            OnDestroyModel();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 组件
        Button btnClose;
        Button btnRank, btnStage;
        Text timeTitle, diffTime;
        InfinityGrid infinityGrid;

        Text bossName;
        Button[] btnFeatures = new Button[4];
        Text[] textFeatures = new Text[4];
        Button btnFastTeam, btnStartFight;
        Text checkText;
        #endregion
        #region 数据
        AssetDependencies assetDependencies;
        ShowSceneControl showSceneControl;
        DisplayControl<EHeroModelParts> displayControl;
        CSVBOSSTower.Data featureData;
        List<BossTowerStageData> bossTowerStageDataList = new List<BossTowerStageData>();
        List<FloorItem> floorItemList = new List<FloorItem>();
        static uint nextTid;
        static uint curSelectTid;
        bool resetTimeDirty;
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            assetDependencies = transform.GetComponent<AssetDependencies>();
            checkText = transform.Find("Animator/CheckText").GetComponent<Text>();
            btnClose = transform.Find("Animator/View_FullTitle02_New/Btn_Close").GetComponent<Button>();
            btnRank = transform.Find("Animator/View_Buttons/Button_Rank").GetComponent<Button>();
            btnStage= transform.Find("Animator/View_Buttons/Button_Stage").GetComponent<Button>();
            timeTitle = transform.Find("Animator/Image_Time/Text").GetComponent<Text>();
            diffTime = transform.Find("Animator/Image_Time/Text_Time").GetComponent<Text>();
            infinityGrid = transform.Find("Animator/Scroll View").GetComponent<InfinityGrid>() ;
            bossName = transform.Find("Animator/View_Right/Name_Bottom/Text_Name").GetComponent<Text>();
            Transform specificTrans = transform.Find("Animator/View_Right/Specific");
            for (int i = 0; i < 4; i++)
            {
                btnFeatures[i] = specificTrans.GetChild(i+1).GetComponent<Button>();
                if (i == 0)
                    btnFeatures[i].onClick.AddListener(() => { OnClickFeature(0); });
                else if (i == 1)
                    btnFeatures[i].onClick.AddListener(() => { OnClickFeature(1); });
                else if (i == 2)
                    btnFeatures[i].onClick.AddListener(() => { OnClickFeature(2); });
                else if (i == 3)
                    btnFeatures[i].onClick.AddListener(() => { OnClickFeature(3); });
                textFeatures[i] = specificTrans.GetChild(i + 1).Find("Text").GetComponent<Text>();
            }
            btnFastTeam = transform.Find("Animator/View_Right/Btn_01").GetComponent<Button>();
            btnStartFight = transform.Find("Animator/View_Right/Btn_02").GetComponent<Button>();

            btnClose.onClick.AddListener(() => { CloseSelf(); });
            btnRank.onClick.AddListener(() => { UIManager.OpenUI(EUIID.UI_BossTower_BossFightRank); });
            btnStage.onClick.AddListener(() => { UIManager.OpenUI(EUIID.UI_BossTower_Stages); });

            btnFastTeam.onClick.AddListener(()=> { Sys_ActivityBossTower.Instance.FastTeam(true); });
            btnStartFight.onClick.AddListener(()=> {
                Sys_ActivityBossTower.Instance.OnBossTowerChallengeReq(true);
            });

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_ActivityBossTower.Instance.eventEmitter.Handle(Sys_ActivityBossTower.EEvents.OnRefreshBossTowerReset, OnRefreshBossTowerReset, toRegister);
        }
        private void OnRefreshBossTowerReset()
        {
            SetFeatureData();
            RefreshFloorItem();
            RefreshDiffTime();
            OnCreateModel();
        }
        float defaultSize = 176;
        /// <summary>
        /// 特性展示
        /// </summary>
        /// <param name="type"></param>
        private void OnClickFeature(int index)
        {
            if (featureData != null && featureData.textDetails_id != null)
            {
                Button btn = btnFeatures[index];
                uint lanId = featureData.textDetails_id[index];
                checkText.text = LanguageHelper.GetTextContent(lanId);
                float diffY = checkText.preferredHeight / defaultSize;
                Vector3 btnPos = btn.GetComponent<RectTransform>().position;
                Vector3 worldPos = new Vector3(btnPos.x, btnPos.y + diffY, btnPos.z);
                UIRuleParam param = new UIRuleParam();
                param.StrContent = LanguageHelper.GetTextContent(lanId);
                param.Pos = CameraManager.mUICamera.WorldToScreenPoint(worldPos);
                UIManager.OpenUI(EUIID.UI_Rule, false, param);
            }
        }
        #endregion
        private void InitView()
        {
            resetTimeDirty = true;
            featureData = Sys_ActivityBossTower.Instance.GetBossTowerFeature();
            SetFeatureData();
            RefreshFloorItem();
            RefreshDiffTime();
            OnCreateModel();
        }
        private void LateUpdate()
        {
            if (resetTimeDirty)
            {
                RefreshDiffTime();
            }
        }
        private void SetFeatureData()
        {
            if (featureData != null)
            {
                bossName.text = LanguageHelper.GetTextContent(featureData.bossName);
                SetTextFeatures();
                if (featureData.text_id != null && featureData.text_id.Count > 0)
                {
                    for (int i = 0; i < featureData.text_id.Count; i++)
                    {
                        btnFeatures[i].gameObject.SetActive(true);
                        textFeatures[i].text = LanguageHelper.GetTextContent(featureData.text_id[i]);
                    }
                }
            }
        }
        private void SetTextFeatures()
        {
            for (int i = 0; i < btnFeatures.Length; i++)
            {
                btnFeatures[i].gameObject.SetActive(false);
            }
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
            if (Sys_ActivityBossTower.Instance.curBossTowerState == BossTowerState.BossOver)
                lanId = 1009320;
            timeTitle.text = LanguageHelper.GetTextContent(lanId);
            diffTime.text = LanguageHelper.TimeToString((uint)(second), LanguageHelper.TimeFormat.Type_1);
        }
        private void RefreshFloorItem()
        {
            nextTid = Sys_ActivityBossTower.Instance.GetBossTowerNextTid(2);
            curSelectTid = nextTid;
            bossTowerStageDataList.Clear();
            List<BossTowerStageData> dataList = Sys_ActivityBossTower.Instance.GetBossTowerStageDataList();
            if (dataList != null)
            {
                bossTowerStageDataList.AddRange(dataList);
                bossTowerStageDataList.Sort((a, b) => {
                    return (int)(b.tid - a.tid);
                });
            }
            if (bossTowerStageDataList != null && bossTowerStageDataList.Count > 0)
            {
                int index = bossTowerStageDataList.FindIndex(o => o.tid == curSelectTid);
                bossTowerStageDataList.Add(null);
                infinityGrid.CellCount = bossTowerStageDataList.Count;
                infinityGrid.Apply();
                infinityGrid.MoveToIndex(bossTowerStageDataList.Count - 2);
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
            entry.SetData(bossTowerStageDataList[index], OnRefreshSelect);//索引数据
        }

        private void OnRefreshSelect(uint tid)
        {

        }
        #region  ModelShow
        private void OnCreateModel()
        {
            OnDestroyModel();
            _LoadShowScene();
            _LoadShowModel();
        }
        private void _LoadShowScene()
        {
            if (showSceneControl == null)
                showSceneControl = new ShowSceneControl();

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);
        }
        private void _LoadShowModel()
        {
            featureData = Sys_ActivityBossTower.Instance.GetBossTowerFeature();
            if (featureData != null)
            {
                displayControl = DisplayControl<EHeroModelParts>.Create((int)EHeroModelParts.Count);
                displayControl.onLoaded = OnShowModelLoaded;
                displayControl.eLayerMask = ELayerMask.ModelShow;

                displayControl.LoadMainModel(EHeroModelParts.Main, featureData.model_show, EHeroModelParts.None, null);
                displayControl.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
                showSceneControl.mModelPos.transform.localPosition = new Vector3(featureData.positionx / 10000f, featureData.positiony / 10000f, featureData.positionz / 10000f);
                showSceneControl.mModelPos.transform.localEulerAngles = new Vector3(featureData.rotationx / 10000f, featureData.rotationy / 10000f, featureData.rotationz / 10000f);
                showSceneControl.mModelPos.transform.localScale = new Vector3(featureData.scale / 10000f, featureData.scale / 10000f, featureData.scale / 10000f);
            }
        }
        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                GameObject mainGo = displayControl.GetPart(EHeroModelParts.Main).gameObject;
                mainGo?.SetActive(false);
                displayControl.mAnimation.UpdateHoldingAnimations(featureData.action_show_id, Constants.UMARMEDID, null, EStateType.Idle, mainGo);
            }
        }

        private void OnDestroyModel()
        {
            DisplayControl<EHeroModelParts>.Destory(ref displayControl);
            displayControl?.Dispose();
            showSceneControl?.Dispose();
            showSceneControl = null;
        }
        #endregion
        #region item
        public class FloorItem
        {
            Transform viewTrans;
            GameObject objTopBg, objFloorBg, objBottomBg;
            Text stageNum;
            GameObject objBoxClose, objBoxOpen;
            GameObject objLock,objFinish;
            Action<uint> action;
            public void Init(Transform trans)
            {
                objBottomBg = trans.Find("Image_BottomBG").gameObject;
                viewTrans = trans.Find("View");
                objTopBg = viewTrans.Find("Image_TopBG").gameObject;
                objFloorBg = viewTrans.Find("Image_FloorBG").gameObject;
                objBoxClose = viewTrans.Find("Image_Icon").gameObject;
                objBoxOpen = viewTrans.Find("Image_IconOpen").gameObject;
                stageNum = viewTrans.Find("Text").GetComponent<Text>();
                objLock = viewTrans.Find("Image_Lock").gameObject;
                objFinish = viewTrans.Find("Image_Finish").gameObject;
            }
            public void SetData(BossTowerStageData data,Action<uint> action)
            {
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
                    uint lastTid = Sys_ActivityBossTower.Instance.GetBossTowerFirstOrLastDataTid(2, false);
                    if (data.csvData.stage_number == Sys_ActivityBossTower.Instance.GetBossTowerStageData(lastTid).csvData.stage_number)
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
                    if (data.csvData.stage_number <= 6)
                        stageNum.text = LanguageHelper.GetTextContent(1009322, data.csvData.stage_number.ToString());
                    else//最后一层隐藏宝箱和锁的状态
                    {
                        stageNum.text = LanguageHelper.GetTextContent(1009323);
                        objBoxClose.SetActive(false);
                        objBoxOpen.SetActive(false);
                        objLock.SetActive(false);
                        objFinish.SetActive(false);
                    }
                }
            }
            private void StateShow(uint type)
            {
                if (type == 1)
                {
                    objBoxClose.SetActive(true);
                    objBoxOpen.SetActive(false);
                    objLock.SetActive(false);
                    objFinish.SetActive(false);
                }
                else if (type == 2)
                {
                    objBoxClose.SetActive(false);
                    objBoxOpen.SetActive(true);
                    objLock.SetActive(false);
                    objFinish.SetActive(true);
                }
                else
                {
                    objBoxClose.SetActive(true);
                    objBoxOpen.SetActive(false);
                    objLock.SetActive(true);
                    objFinish.SetActive(false);
                }
            }
        }
        #endregion
    }
}