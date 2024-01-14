using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Framework;
using Lib.Core;

namespace Logic
{
    //试炼主界面
    public class UI_TrialGateMain : UIBase
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
        protected override void OnHide()
        {
            OnDestroyModel();
        }
        protected override void OnClose()
        {
            OnDestroyModel();
        }
        protected override void OnDestroy()
        {
            rewardShowList.Clear();
        }
        protected override void OnLateUpdate(float dt, float usdt)
        {
            LateUpdate();
        }
        #endregion
        #region 组件
        Button btnClose;
        RawImage imgBoss;
        Text textBossName;
        Button btnPeculiarity_1, btnPeculiarity_2;
        Button btnRank;
        Button btnTeamDeploy, btnFastTeam,btnShop;
        Button btnStartFight;
        Button btnSkillDeploy;
        GameObject objRedPoint;
        Text textPeculiarity_1, textPeculiarity_2;
        Transform rewards;
        Text textResetTime;
        InfinityGrid infinityGrid;
        Text checkText;
        #endregion
        #region 数据

        AssetDependencies assetDependencies;
        ShowSceneControl showSceneControl;
        DisplayControl<EHeroModelParts> displayControl;

        List<RewardShowItem> rewardShowList = new List<RewardShowItem>();
        List<SkillColumnDeploy> skillColumnDeployList = new List<SkillColumnDeploy>();
        CSVTrialCharacteristic.Data characteristicData;
        CSVTrialLevelGrade.Data levelGradeData;
        CSVNpc.Data csvNpcData;
        bool resetTimeDirty;
        bool isCanShow;
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            btnClose = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            checkText = transform.Find("Animator/CheckText").GetComponent<Text>();
            Transform transLeft = transform.Find("Animator/View_Left");
            imgBoss = transLeft.Find("Image_Boss").GetComponent<RawImage>();
            imgBoss.gameObject.SetActive(true);
            textBossName = transLeft.Find("Name_Bottom/Text_Name").GetComponent<Text>();
            btnRank = transLeft.Find("Buttons/Button_Rank").GetComponent<Button>();
            btnTeamDeploy = transLeft.Find("Buttons/Button_Team").GetComponent<Button>();
            btnShop = transLeft.Find("Buttons/Button_Shop").GetComponent<Button>();
            btnPeculiarity_1 = transLeft.Find("Specific/Spe01").GetComponent<Button>();
            btnPeculiarity_2 = transLeft.Find("Specific/Spe02").GetComponent<Button>();
            textPeculiarity_1 = btnPeculiarity_1.transform.Find("Text").GetComponent<Text>();
            textPeculiarity_2 = btnPeculiarity_2.transform.Find("Text").GetComponent<Text>();

            Transform transRight = transform.Find("Animator/View_Right");
            rewards = transRight.Find("View_Reward/Grid");
            infinityGrid = transRight.Find("View_Skill/Scroll View").GetComponent<InfinityGrid>();
            textResetTime = transRight.Find("View_Skill/Text_Time").GetComponent<Text>();
            btnSkillDeploy = transRight.Find("View_Skill/Btn_Setting").GetComponent<Button>();
            objRedPoint = btnSkillDeploy.transform.Find("Image_Red").gameObject;
            btnFastTeam = transRight.Find("View_Skill/Btn_Team").GetComponent<Button>();
            btnStartFight = transRight.Find("View_Skill/Btn_Start").GetComponent<Button>();
            assetDependencies = transform.GetComponent<AssetDependencies>();

            btnPeculiarity_1.onClick.AddListener(() => { ShowTipsClick(1); });
            btnPeculiarity_2.onClick.AddListener(() => { ShowTipsClick(2); });
            btnRank.onClick.AddListener(() => { UIManager.OpenUI(EUIID.UI_TrialRank); });
            btnTeamDeploy.onClick.AddListener(() => {
                if (!Sys_FunctionOpen.Instance.IsOpen(20218, true))
                    return;
                if (Sys_Team.Instance.HaveTeam)
                    UIManager.OpenUI(EUIID.UI_TrialTeamDeploy);
                else
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101811));
            });
            btnShop.onClick.AddListener(()=> {
                //黑市
                MallPrama mallPrama = new MallPrama
                {
                    mallId = 1600,
                    shopId = 16001,
                };
                UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
            });
            btnFastTeam.onClick.AddListener(FastTeamClick);
            btnStartFight.onClick.AddListener(()=> {
                Sys_ActivityTrialGate.Instance.OnChallengeReadyReq();
            });
            btnSkillDeploy.onClick.AddListener(() => { UIManager.OpenUI(EUIID.UI_TrialSkillDeploy); });

            btnClose.onClick.AddListener(() => { CloseSelf(); });

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(imgBoss);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnPointerClick);
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_ActivityTrialGate.Instance.eventEmitter.Handle<uint>(Sys_ActivityTrialGate.EEvents.OnRefreshSkillColumnDeploy, OnRefreshSkillColumnDeploy, toRegister);
            Sys_ActivityTrialGate.Instance.eventEmitter.Handle(Sys_ActivityTrialGate.EEvents.OnRefreshStage, OnRefreshStage, toRegister);
            Sys_ActivityTrialGate.Instance.eventEmitter.Handle(Sys_ActivityTrialGate.EEvents.OnRefreshRedPoint, OnRefreshRedPoint, toRegister);
            Sys_ActivityTrialGate.Instance.eventEmitter.Handle(Sys_ActivityTrialGate.EEvents.OnRefreshTrialGateReset, OnRefreshTrialGateReset, toRegister);
        }
        private void OnRefreshSkillColumnDeploy(uint type)
        {
            RefreshSkillDeploy();
        }
        /// <summary>
        /// 刷新阶段奖励进度
        /// </summary>
        private void OnRefreshStage()
        {
            RefreshReward();
        }
        private void OnRefreshRedPoint()
        {
            SetRedPoint();
        }
        /// <summary>
        /// 重置刷新
        /// </summary>
        private void OnRefreshTrialGateReset()
        {
            InitView();
        }
        //打开便捷组队并创建对应组队目标
        private void FastTeamClick()
        {
            levelGradeData = Sys_ActivityTrialGate.Instance.GetTrialLevelGrade();
            if (levelGradeData != null)
            {
                if (Sys_Team.Instance.HaveTeam)
                {
                    if (!Sys_Team.Instance.isTeamTarget(levelGradeData.team_id))
                    {
                        CSVTeam.Data teamData = CSVTeam.Instance.GetConfData(levelGradeData.team_id);
                        if (teamData != null)
                            Sys_Team.Instance.ApplyEditTarget(levelGradeData.team_id, teamData.lv_min, teamData.lv_max, true, true);
                        else
                            DebugUtil.LogError("CSVTeam not found id:"+ levelGradeData.team_id);
                    }
                    UIManager.OpenUI(EUIID.UI_Team_Member, false, UI_Team_Member.EType.Team);
                }
                else
                {
                    Sys_Team.Instance.ApplyCreateTeam(Sys_Role.Instance.RoleId, levelGradeData.team_id);
                    UIManager.OpenUI(EUIID.UI_Team_Member, false, UI_Team_Member.EType.Team);
                }
            }
        }
        float defaultSize = 176;
        private void ShowTipsClick(uint type)
        {
            Button btn;
            uint lanId;
            if (type == 1)
            {
                lanId = characteristicData.characteristic1_description;
                btn = btnPeculiarity_1;
            }
            else
            {
                lanId = characteristicData.characteristic2_description;
                btn = btnPeculiarity_2;
            }
            checkText.text = LanguageHelper.GetTextContent(lanId);
            float diffY = checkText.preferredHeight / defaultSize;
            Vector3 btnPos = btn.GetComponent<RectTransform>().position;
            Vector3 worldPos = new Vector3(btnPos.x, btnPos.y + diffY, btnPos.z);
            UIRuleParam param = new UIRuleParam();
            param.StrContent = LanguageHelper.GetTextContent(lanId);
            param.Pos = CameraManager.mUICamera.WorldToScreenPoint(worldPos);
            UIManager.OpenUI(EUIID.UI_Rule, false, param);
        }
        #endregion

        private void InitView()
        {
            resetTimeDirty = true;
            isCanShow = true;
            characteristicData = Sys_ActivityTrialGate.Instance.GetTrialCharacteristic();
            levelGradeData = Sys_ActivityTrialGate.Instance.GetTrialLevelGrade();
            levelGradeData = levelGradeData == null ? CSVTrialLevelGrade.Instance.GetConfData(1): levelGradeData;
            csvNpcData = CSVNpc.Instance.GetConfData(levelGradeData.bossNPC_id);
            if (csvNpcData == null)
                DebugUtil.LogError("CSVNpc not found id :" + levelGradeData.bossNPC_id);

            SetResetTime();
            SetRedPoint();
            SetBossData();
            OnCreateModel();
            SetReward();
            RefreshSkillDeploy();
        }
        private void LateUpdate()
        {
            if (resetTimeDirty)
            {
                SetResetTime();
            }
        }
        private void SetRedPoint()
        {
            objRedPoint.SetActive(Sys_ActivityTrialGate.Instance.CheckRedPoint());
        }
        private void SetResetTime()
        {
            int second = Sys_ActivityTrialGate.Instance.GetActivityRestTimeDiff();
            if (second <= 0)
            {
                second = 0;
                resetTimeDirty = false;
            }
            textResetTime.text = LanguageHelper.GetTextContent(3899000001, LanguageHelper.TimeToString((uint)(second), LanguageHelper.TimeFormat.Type_9));
        }
        private void SetBossData()
        {
            textBossName.text = LanguageHelper.GetNpcTextContent(csvNpcData.name);
            textPeculiarity_1.text= LanguageHelper.GetTextContent(characteristicData.characteristic1_name);
            textPeculiarity_2.text= LanguageHelper.GetTextContent(characteristicData.characteristic2_name);
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
            if (csvNpcData != null)
            {
                displayControl = DisplayControl<EHeroModelParts>.Create((int)EHeroModelParts.Count);
                displayControl.onLoaded = OnShowModelLoaded;
                displayControl.eLayerMask = ELayerMask.ModelShow;

                displayControl.LoadMainModel(EHeroModelParts.Main, csvNpcData.model_show, EHeroModelParts.None, null);
                displayControl.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
                showSceneControl.mModelPos.transform.localPosition = new Vector3(levelGradeData.model_location[0]/10000f, levelGradeData.model_location[1] / 10000f, levelGradeData.model_location[2] / 10000f);
                showSceneControl.mModelPos.transform.localScale = new Vector3(levelGradeData.model_size / 10000f, levelGradeData.model_size / 10000f, levelGradeData.model_size / 10000f);
            }
        }
        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                GameObject objBone = showSceneControl.mModelPos.gameObject.FindChildByName("Bone001/Fx_TSZD_02_wuqi");
                if (objBone != null)
                {
                    ParticleSystem[] particleSystems = objBone.GetComponentsInChildren<ParticleSystem>();
                    if (particleSystems.Length > 0)
                    {
                        for (int i = 0; i < particleSystems.Length; i++)
                        {
                            ParticleSystem.MainModule main = particleSystems[i].main;
                            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                        }
                    }
                }

                GameObject mainGo = displayControl.GetPart(EHeroModelParts.Main).gameObject;
                mainGo?.SetActive(false);
                displayControl.mAnimation.UpdateHoldingAnimations(csvNpcData.action_show_id, Constants.UMARMEDID, null, EStateType.Idle, mainGo);
            }
        }

        private void OnDestroyModel()
        {
            DisplayControl<EHeroModelParts>.Destory(ref displayControl);
            displayControl?.Dispose();
            showSceneControl?.Dispose();
            showSceneControl = null;
        }
        int curIndex = 0;
        private uint GetStateId()
        {
            uint stateId = (uint)EStateType.Idle;
            if (levelGradeData.action != null && levelGradeData.action.Count > 0)
            {
                if (curIndex < levelGradeData.action.Count)
                    stateId = levelGradeData.action[curIndex];
                else
                {
                    curIndex = 0;
                    stateId = levelGradeData.action[curIndex];
                }
                curIndex++;
            }
            return stateId;
        }
        private void OnPointerClick(BaseEventData eventData)
        {
            if (isCanShow)
            {
                isCanShow = false;
                displayControl?.mAnimation.Play(GetStateId(), OnPlayOver);
            }
        }

        private void OnPlayOver()
        {
            displayControl?.mAnimation.Play((uint)EStateType.Idle);
            isCanShow = true;
        }

        public void OnDrag(BaseEventData eventData)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        public void AddEulerAngles(Vector3 angle)
        {
            if (showSceneControl.mModelPos.transform != null)
            {
                Vector3 localAngle = showSceneControl.mModelPos.transform.localEulerAngles;
                localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = localAngle;
            }
        }
        #endregion
        /// <summary>
        /// 刷新宠物技能配置
        /// </summary>
        private void RefreshSkillDeploy()
        {
            skillColumnDeployList = Sys_ActivityTrialGate.Instance.GetSkillColumnDeployList();
            infinityGrid.CellCount = skillColumnDeployList.Count;
            infinityGrid.ForceRefreshActiveCell();
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            SkillShowItem entry = new SkillShowItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            SkillShowItem entry = cell.mUserData as SkillShowItem;
            entry.SetData(skillColumnDeployList[index]);//索引数据
        }
        private void SetReward()
        {
            List<TrialStage> trialStageList = Sys_ActivityTrialGate.Instance.GetTrialStageList();
            if (trialStageList != null && trialStageList.Count > 0)
            {
                for (int i = 0; i < rewardShowList.Count; i++)
                {
                    RewardShowItem cell = rewardShowList[i];
                    PoolManager.Recycle(cell);
                }
                rewardShowList.Clear();
                FrameworkTool.CreateChildList(rewards, trialStageList.Count);
                for (int i = 0; i < trialStageList.Count; i++)
                {
                    Transform trans = rewards.GetChild(i);
                    RewardShowItem cell = PoolManager.Fetch<RewardShowItem>();
                    cell.Init(trans);
                    cell.SetData(trialStageList[i]);
                    rewardShowList.Add(cell);
                }
                FrameworkTool.ForceRebuildLayout(rewards.gameObject);
            }
        }
        /// <summary>
        /// 刷新奖励进度
        /// </summary>
        private void RefreshReward()
        {
            if (rewardShowList != null && rewardShowList.Count > 0)
            {
                List<TrialStage> trialStageList = Sys_ActivityTrialGate.Instance.GetTrialStageList();
                for (int i = 0; i < rewardShowList.Count; i++)
                {
                    rewardShowList[i].SetData(trialStageList[i], true);
                }
            }
        }
        #region item
        //试炼奖励展示
        public class RewardShowItem
        {
            Button btnClick;
            GameObject objSelect;
            Image icon;
            GameObject fx_Ui_PropItem02;
            GameObject receivedObj;
            Text textName;
            TrialStage data;
            public void Init(Transform trans)
            {
                btnClick = trans.Find("Button_Normal").GetComponent<Button>();
                objSelect = trans.Find("Image_Select").gameObject;
                icon = trans.Find("Image_Icon").GetComponent<Image>();
                //fx_Ui_PropItem02 = trans.Find("Fx_Ui_PropItem02").gameObject;
                receivedObj = trans.Find("Image_Blank").gameObject;
                textName = trans.Find("Text_Name").GetComponent<Text>();
                btnClick.onClick.RemoveAllListeners();
                btnClick.onClick.AddListener(OnClick);
            }
            public void SetData(TrialStage data,bool isOnlyState=false)
            {
                this.data = data;
                uint curStage = Sys_ActivityTrialGate.Instance.curStage;
                if (curStage >= data.csv_trialStage.stage_number)//进行到第几阶段
                {
                    receivedObj.SetActive(true);
                }
                else
                {
                    receivedObj.SetActive(false);
                }
                if (isOnlyState)
                    return;
                //ImageHelper.SetIcon(icon,);
                textName.text = LanguageHelper.GetTextContent(data.csv_trialStage.stage_description);
            }
            private void OnClick()
            {
                RewardPanelParam _param = new RewardPanelParam();
                Vector3 _vec = btnClick.gameObject.GetComponent<RectTransform>().position;
                Vector3 newPos = new Vector3(_vec.x - 2.5f, _vec.y - 1.5f, _vec.z);
                Vector3 _screenVec = CameraManager.mUICamera.WorldToScreenPoint(newPos);
                _param.propList = CSVDrop.Instance.GetDropItem(data.csv_trialStage.stage_drop);
                _param.Pos = _screenVec;
                UIManager.OpenUI(EUIID.UI_RewardPanel, false, _param);
            }
        }
        //技能配置展示
        public class SkillShowItem
        {
            Button btnClick;
            Image petIcon, skillIcon;
            Text skillName;
            Image mask;
            Text textMask;
            GameObject objLock;
            GameObject objRecommend;
            public void Init(Transform trans)
            {
                btnClick = trans.GetNeedComponent<Button>();
                petIcon = trans.Find("Image_Pet/Image_Icon").GetComponent<Image>();
                skillIcon = trans.Find("PetSkillItem01/Image_Skill").GetComponent<Image>();
                skillName = trans.Find("Text_Name").GetComponent<Text>();
                mask= trans.Find("Image_Mask").GetComponent<Image>();
                textMask = trans.Find("Image_Mask/Text").GetComponent<Text>();
                objLock= trans.Find("PetSkillItem01/View_Lock").gameObject;
                objRecommend = trans.Find("Image_Tuijian").gameObject;

                btnClick.onClick.AddListener(OnClick);
            }
            public void SetData(SkillColumnDeploy data)
            {
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(data.superSkill.skillId);
                if (skillInfo != null)
                {
                    ImageHelper.SetIcon(skillIcon, skillInfo.icon);
                    skillName.text = LanguageHelper.GetTextContent(skillInfo.name);
                }
                objRecommend.SetActive(data.CheckSuperSkillIsRecommend());
                if (data.petUid != 0)
                {
                    petIcon.gameObject.SetActive(true);
                    ClientPet clientPet = Sys_ActivityTrialGate.Instance.GetClientPet(data.petUid);
                    ImageHelper.SetIcon(petIcon, clientPet.petData.icon_id);
                }
                else
                    petIcon.gameObject.SetActive(false);
                if (data.CheckSuperSkillIsActivate())
                {
                    objLock.SetActive(false);
                    mask.gameObject.SetActive(false);
                }
                else
                {
                    if (data.GetFirstSkillActivateCount() <= 0)
                    {
                        objLock.SetActive(true);
                        mask.gameObject.SetActive(false);
                    }
                    else
                    {
                        objLock.SetActive(false);
                        mask.gameObject.SetActive(true);
                        mask.fillAmount = data.GetFirstSkillActivateRatio();
                        textMask.text = LanguageHelper.GetTextContent(3899000031, data.GetFirstSkillActivateCount().ToString(), data.firstSkillCellList.Count.ToString());
                    }
                }
            }
            private void OnClick()
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(3899000002);
                PromptBoxParameter.Instance.SetConfirm(true, () => {
                    UIManager.OpenUI(EUIID.UI_TrialSkillDeploy);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }
        #endregion
    }
}