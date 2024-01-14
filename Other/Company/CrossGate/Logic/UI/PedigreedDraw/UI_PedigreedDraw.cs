using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Packet;
using System.Text;

namespace Logic
{
    public class UI_PedigreedDraw : UI_OperationalActivityBase
    {
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        private AssetDependencies assetDependencies;
        private RawImage petModel_rawImage;
        private Text m_PetName;

        private GameObject m_PlayerRoot;
        private Text m_FinnalPlayer;
        private Image m_PlayerHead;
        private Image m_PlayerFrame;

        private Transform m_SpecialParent;
        private Transform m_LuckyParent;
        private Image m_SpecialBigIcon;

        private GameObject m_SpecialShowRoot;
        private Image m_SpecialHead;
        private Text m_SpecialName;

        private GameObject m_SpecialModelShowRoot;

        private CSVPedigreedDraw.Data m_CSVPedigreedDrawData;
        private CSVPetNew.Data m_CurrentPetData;

        private Text m_ActivityValidTime;//活动时间
        private Toggle m_PlayAnimToggle;

        private GameObject m_SpecialPropGo;
        private PropItem m_SpecialPropItem;
        private Dictionary<GameObject, PropItem> propItems = new Dictionary<GameObject, PropItem>();

        private Transform m_TaskParent;
        private List<TaskGrid> m_TaskGrids = new List<TaskGrid>();

        private uint luckyTimeStamp;
        private uint finnalTimeStamp;
        private Text m_ShowTime;

        private void InitData()
        {
            m_CSVPedigreedDrawData = Sys_PedigreedDraw.Instance.cSVPedigreedDrawData;

            luckyTimeStamp = uint.Parse(CSVPedigreedParameter.Instance.GetConfData(3).str_value);
            finnalTimeStamp = uint.Parse(CSVPedigreedParameter.Instance.GetConfData(4).str_value);
        }

        private void ParseCp()
        {
            m_PlayerRoot = transform.Find("Show").gameObject;
            assetDependencies = transform.GetComponent<AssetDependencies>();
            m_FinnalPlayer = transform.Find("Show/BG/Name").GetComponent<Text>();
            m_PlayerHead = transform.Find("Show/BG/Head").GetComponent<Image>();
            m_PlayerFrame = transform.Find("Show/BG/Head/Image_Before_Frame").GetComponent<Image>();
            m_ActivityValidTime = transform.Find("Title/Text1").GetComponent<Text>();
            m_ShowTime = transform.Find("Title/Text2").GetComponent<Text>();
            petModel_rawImage = transform.Find("Special/Texture").GetComponent<RawImage>();
            m_PetName = transform.Find("Special/Text1").GetComponent<Text>();
            m_PlayAnimToggle = transform.Find("View_Toggle/Toggle_Read").GetComponent<Toggle>();
            m_SpecialShowRoot = transform.Find("Show").gameObject;
            m_SpecialHead = transform.Find("Show/BG/Head").GetComponent<Image>();
            m_SpecialName = transform.Find("Show/BG/Name").GetComponent<Text>();
            m_SpecialPropGo = transform.Find("Award/Scroll View1/Viewport/PropItem").gameObject;
            m_SpecialBigIcon = transform.Find("Award/Scroll View1/Viewport/PropItem/Btn_Item/Image_Icon").GetComponent<Image>();

            m_SpecialModelShowRoot = transform.Find("Special").gameObject;
            m_SpecialParent = transform.Find("Award/Scroll View1/Viewport");
            m_LuckyParent = transform.Find("Award/Scroll View2/Viewport");
            m_TaskParent = transform.Find("Task/Scroll View/Viewport");

            m_PlayAnimToggle.onValueChanged.AddListener(OnToggleChanged);
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(petModel_rawImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);

            BuildTaskList();
        }

        protected override void InitBeforOnShow()
        {
            InitData();
            ParseCp();
        }

        public override void Show()
        {
            base.Show();
            UpdateInfo();
            LoadModel();
        }

        public override void Hide()
        {
            base.Hide();
            _UnloadShowContent();

            for (int i = 0; i < m_TaskGrids.Count; i++)
            {
                m_TaskGrids[i].HideFx();
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_PedigreedDraw.Instance.eventEmitter.Handle(Sys_PedigreedDraw.EEvents.OnUpdateTaskAward, UpdateTaskInfo, toRegister);
        }

        private void OnToggleChanged(bool value)
        {
            Sys_PedigreedDraw.Instance.ignorePlayAnim = value;
        }

        private void UpdateInfo()
        {
            DateTime startTime = TimeManager.GetDateTime(Sys_PedigreedDraw.Instance.startTime);
            DateTime endTime = TimeManager.GetDateTime(Sys_PedigreedDraw.Instance.endTime);
            TextHelper.SetText(m_ActivityValidTime, LanguageHelper.GetTextContent(2025301, startTime.Year.ToString(), startTime.Month.ToString(), startTime.Day.ToString()
                , endTime.Year.ToString(), endTime.Month.ToString(), endTime.Day.ToString()));

            uint luckyHour = luckyTimeStamp / 3600;
            uint luckyMinte = (luckyTimeStamp % 3600) / 60;
            string luckyMinteStr = GetTimeFormat((int)luckyMinte);
            //string luckyStr = string.Format("{0}:{1}", luckyHour, luckyMinte);
            uint finnalHour = finnalTimeStamp / 3600;
            uint finnalMinte = (finnalTimeStamp % 3600) / 60;
            string finnalMinteStr = GetTimeFormat((int)finnalMinte);
            //string finnalStr = string.Format("{0}:{1}", finnalHour, finnalMinte);
            TextHelper.SetText(m_ShowTime, LanguageHelper.GetTextContent(2025302, luckyHour.ToString(), luckyMinteStr, finnalHour.ToString(), finnalMinteStr));

            RefreshAward();
            UpdateTaskInfo();
            m_PlayAnimToggle.isOn = Sys_PedigreedDraw.Instance.ignorePlayAnim;
        }

        private string GetTimeFormat(int value)
        {
            string zero = "0";
            string tp = string.Empty;
            StringBuilder sb = StringBuilderPool.GetTemporary();
            if (value < 10)
            {
                sb.Append(zero);
                sb.Append(value.ToString());
                tp = StringBuilderPool.ReleaseTemporaryAndToString(sb);
            }
            else
            {
                sb.Append(value.ToString());
                tp = StringBuilderPool.ReleaseTemporaryAndToString(sb);
            }

            return tp;
        }

        private void RefreshAward()
        {
            //特等奖大图标  掉落一定是宠物
            uint dropId_SpecialBig = m_CSVPedigreedDrawData.Special_Signs;
            List<ItemIdCount> items_Special = CSVDrop.Instance.GetDropItem(dropId_SpecialBig);
            uint itemId_Sp = items_Special[0].id;
            uint itemCount_Sp = (uint)items_Special[0].count;
            ItemData itemData = new ItemData(1, 0, itemId_Sp, itemCount_Sp, 0, false, false, null, null, 0);
            if (m_SpecialPropItem == null)
            {
                m_SpecialPropItem = new PropItem();
            }
            m_SpecialPropItem.BindGameObject(m_SpecialPropGo);
            PropIconLoader.ShowItemData showItem_Sp = new PropIconLoader.ShowItemData
                (_id: itemId_Sp,
                _count: itemData.Count,
                _bUseQuailty: true,
                _bBind: false,
                _bNew: false,
                _bUnLock: false,
                _bSelected: false,
                _bShowCount: true,
                _bShowBagCount: false,
                _bUseClick: true,
                _onClick: OnSpPropItemClicked,
                _bshowBtnNo: false,
                _bUseTips: false);
            m_SpecialPropItem.SetData(new MessageBoxEvt(EUIID.UI_OperationalActivity, showItem_Sp));

            //幸运奖列表
            uint dropId_Lucky = m_CSVPedigreedDrawData.Reward_ID[1];
            List<ItemIdCount> items_Lucky = CSVDrop.Instance.GetDropItem(dropId_Lucky);
            int luckyCount = items_Lucky.Count;
            FrameworkTool.CreateChildList(m_LuckyParent, luckyCount);
            for (int i = 0; i < luckyCount; i++)
            {
                GameObject gameObject = m_LuckyParent.GetChild(i).gameObject;
                if (!propItems.TryGetValue(gameObject, out PropItem propItem))
                {
                    propItem = new PropItem();
                }
                propItem.BindGameObject(gameObject);

                uint itemId = items_Lucky[i].id;
                uint count = (uint)items_Lucky[i].count;
                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                  (_id: itemId,
                  _count: count,
                  _bUseQuailty: true,
                  _bBind: false,
                  _bNew: false,
                  _bUnLock: false,
                  _bSelected: false,
                  _bShowCount: true,
                  _bShowBagCount: false,
                  _bUseClick: true,
                  _onClick: null,
                  _bshowBtnNo: false);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_OperationalActivity, showItem));
            }
        }

        private void OnSpPropItemClicked(PropItem propItem)
        {
            ItemData mItemData = new ItemData(0, 0, propItem.ItemData.id, (uint)propItem.ItemData.count, 0, false, false, null, null, 0);

            PropMessageParam propParam = new PropMessageParam();
            propParam.itemData = mItemData;
            propParam.showBtnCheck = true;
            propParam.targetEUIID = (uint)EUIID.UI_Pet_BookReview;
            propParam.sourceUiId = EUIID.UI_OperationalActivity;
            UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
        }

        #region ModelShow
        private void LoadModel()
        {
            if (showSceneControl != null)
            {
                return;
            }
            _LoadShowScene();
            _LoadShowModel();
        }

        private void _LoadShowScene()
        {
            showSceneControl = new ShowSceneControl();

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);

            petModel_rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);

            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }
        }

        private void _LoadShowModel()
        {
            m_CurrentPetData = CSVPetNew.Instance.GetConfData(Sys_PedigreedDraw.Instance.cSVPedigreedDrawData.Show_Item);
            if (m_CurrentPetData != null)
            {
                TextHelper.SetText(m_PetName, m_CurrentPetData.name);
                string modelPath = m_CurrentPetData.model_show;

                petDisplay.eLayerMask = ELayerMask.ModelShow;
                petDisplay.LoadMainModel(EPetModelParts.Main, modelPath, EPetModelParts.None, null);
                petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);

                showSceneControl.mModelPos.transform.Rotate(new Vector3(m_CurrentPetData.angle1, m_CurrentPetData.angle2, m_CurrentPetData.angle3));
                showSceneControl.mModelPos.transform.localScale = new Vector3(m_CurrentPetData.size, m_CurrentPetData.size, m_CurrentPetData.size);
                showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x + m_CurrentPetData.translation, m_CurrentPetData.height,
                    showSceneControl.mModelPos.transform.localPosition.z);
            }
            else
            {
                DebugUtil.LogErrorFormat("not found petId :{0}", Sys_PedigreedDraw.Instance.cSVPedigreedDrawData.Show_Item);
            }
        }


        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                petDisplay.mAnimation.UpdateHoldingAnimations(m_CurrentPetData.action_id_show, m_CurrentPetData.weapon, Constants.PetShowAnimationClipHashSet);
            }
        }

        public void OnDrag(BaseEventData eventData)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        public void AddEulerAngles(Vector3 angle)
        {
            Vector3 ilrTemoVector3 = angle;
            if (showSceneControl.mModelPos.transform != null)
            {
                showSceneControl.mModelPos.transform.Rotate(ilrTemoVector3.x, ilrTemoVector3.y, ilrTemoVector3.z);
            }
        }

        private void _UnloadShowContent()
        {
            if (petModel_rawImage != null)
            {
                petModel_rawImage.texture = null;
            }
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl?.Dispose();
            showSceneControl = null;
        }

        #endregion

        #region TaskInfo

        private void BuildTaskList()
        {
            int count = m_CSVPedigreedDrawData.Task_ID.Count;
            FrameworkTool.CreateChildList(m_TaskParent, count);
            for (int i = 0; i < count; i++)
            {
                GameObject gameObject = m_TaskParent.GetChild(i).gameObject;
                TaskGrid taskGrid = new TaskGrid();
                taskGrid.BindGameObject(gameObject);
                taskGrid.SetData(m_CSVPedigreedDrawData.Task_ID[i]);
                m_TaskGrids.Add(taskGrid);
            }
        }

        private void UpdateTaskInfo()
        {
            for (int i = 0; i < m_TaskGrids.Count; i++)
            {
                TaskGrid taskGrid = m_TaskGrids[i];
                PetLotteryTaskInfo petLotteryTaskInfo = Sys_PedigreedDraw.Instance.GetTaskInfo(taskGrid.taskId);
                PetLotteryCodeState petLotteryCodeState = Sys_PedigreedDraw.Instance.GetCodeState(taskGrid.taskId);
                taskGrid.RefreshState(petLotteryTaskInfo, petLotteryCodeState);
            }

            if (Sys_PedigreedDraw.Instance.finalAwardInfo != null)
            {
                bool reachFinnalTime = Sys_Time.Instance.GetServerTime() - (uint)Sys_Time.Instance.GetDayZeroTimestamp() >= Sys_PedigreedDraw.Instance.finnalTimeStamp;
                if (reachFinnalTime)
                {
                    m_PlayerRoot.SetActive(true);
                    TextHelper.SetText(m_FinnalPlayer, Sys_PedigreedDraw.Instance.finalAwardInfo.RoleName.ToStringUtf8());

                    uint headID = CharacterHelper.getHeadID(Sys_PedigreedDraw.Instance.finalAwardInfo.HeroId, Sys_PedigreedDraw.Instance.finalAwardInfo.HeadId);
                    ImageHelper.SetIcon(m_PlayerHead, headID);

                    uint frameID = CharacterHelper.getHeadFrameID(Sys_PedigreedDraw.Instance.finalAwardInfo.HeadFrameId);
                    if (frameID > 0)
                    {
                        m_PlayerFrame.gameObject.SetActive(true);
                        ImageHelper.SetIcon(m_PlayerFrame, frameID);
                    }
                    else
                    {
                        m_PlayerFrame.gameObject.SetActive(false);
                    }
                }
                else
                {
                    m_PlayerRoot.SetActive(false);
                }
            }
            else
            {
                m_PlayerRoot.SetActive(false);
            }
        }

        #endregion


        public class TaskGrid
        {
            private GameObject m_GameObject;
            private GameObject m_Type1;
            private Text m_NameType1;
            private Text m_ValueType1;
            private Button m_GoToButton;
            private Button m_GetButton;

            private GameObject m_Type2;
            private Text m_NameType2;
            private GameObject m_SpecialTag;
            private GameObject m_LuckyTag;

            public uint taskId;
            private CSVPedigreedTaskGroup.Data m_CSVPedigreedTaskGroupData;

            private bool b_HasRewardState = true;
            private GameObject m_Fx;
            private Timer m_FxPlayTimer;

            public void BindGameObject(GameObject gameObject)
            {
                m_GameObject = gameObject;

                m_Type1 = m_GameObject.transform.Find("Type1").gameObject;
                m_NameType1 = m_GameObject.transform.Find("Type1/Text").GetComponent<Text>();
                m_ValueType1 = m_GameObject.transform.Find("Type1/Num").GetComponent<Text>();
                m_GoToButton = m_GameObject.transform.Find("Type1/Btn_01").GetComponent<Button>();
                m_GetButton = m_GameObject.transform.Find("Type1/Btn_02").GetComponent<Button>();

                m_Type2 = m_GameObject.transform.Find("Type2").gameObject;
                m_NameType2 = m_GameObject.transform.Find("Type2/Text").GetComponent<Text>();
                m_SpecialTag = m_GameObject.transform.Find("Type2/Image_Mark01").gameObject;
                m_LuckyTag = m_GameObject.transform.Find("Type2/Image_Mark02").gameObject;

                m_Fx = m_GameObject.transform.Find("Fx_UI_LuckyPet_04").gameObject;

                m_GoToButton.onClick.AddListener(OnGoToButtonClicked);
                m_GetButton.onClick.AddListener(OnGetButtonClicked);
            }

            public void SetData(uint taskId)
            {
                this.taskId = taskId;
                m_CSVPedigreedTaskGroupData = CSVPedigreedTaskGroup.Instance.GetConfData(this.taskId);
            }

            public void RefreshState(PetLotteryTaskInfo petLotteryTaskInfo, PetLotteryCodeState petLotteryCodeState)
            {
                //有奖券
                if (petLotteryCodeState != null)
                {
                    m_Type1.SetActive(false);
                    m_Type2.SetActive(true);

                    string codeStr = Sys_PedigreedDraw.Instance.HandleLotteryCode(petLotteryCodeState.LotteryCode);
                    TextHelper.SetText(m_NameType2, codeStr);
                    if (petLotteryCodeState.State == 0)    //0:未中奖 1:幸运奖 2:特等奖
                    {
                        m_SpecialTag.SetActive(false);
                        m_LuckyTag.SetActive(false);
                    }
                    else if (petLotteryCodeState.State == 1)
                    {
                        m_SpecialTag.SetActive(false);
                        m_LuckyTag.SetActive(true);
                    }
                    else if (petLotteryCodeState.State == 2)
                    {
                        bool reachFinnalTime = Sys_Time.Instance.GetServerTime() - (uint)Sys_Time.Instance.GetDayZeroTimestamp() >= Sys_PedigreedDraw.Instance.finnalTimeStamp;
                        m_SpecialTag.SetActive(reachFinnalTime);
                        m_LuckyTag.SetActive(false);
                    }
                    if (!b_HasRewardState)
                    {
                        ShowFx();
                        b_HasRewardState = true;
                    }
                }
                else
                {
                    b_HasRewardState = false;
                    if (petLotteryTaskInfo != null)
                    {
                        m_Type1.SetActive(true);
                        m_Type2.SetActive(false);

                        TextHelper.SetText(m_NameType1, m_CSVPedigreedTaskGroupData.Task_Des);
                        uint reachValue = m_CSVPedigreedTaskGroupData.ReachTypeAchievement[m_CSVPedigreedTaskGroupData.ReachTypeAchievement.Count - 1];
                        TextHelper.SetText(m_ValueType1, string.Format("{0}/{1}", petLotteryTaskInfo.Value, reachValue));

                        if (petLotteryTaskInfo.Value == reachValue)
                        {
                            m_GoToButton.gameObject.SetActive(false);
                            m_GetButton.gameObject.SetActive(true);
                        }
                        else
                        {
                            m_GoToButton.gameObject.SetActive(true);
                            m_GetButton.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        DebugUtil.LogFormat(ELogType.eOperationActivity, "{0} 任务进度为0", m_CSVPedigreedTaskGroupData.id);
                        TextHelper.SetText(m_NameType1, m_CSVPedigreedTaskGroupData.Task_Des);
                        uint reachValue = m_CSVPedigreedTaskGroupData.ReachTypeAchievement[m_CSVPedigreedTaskGroupData.ReachTypeAchievement.Count - 1];
                        TextHelper.SetText(m_ValueType1, string.Format("{0}/{1}", 0, reachValue));
                        m_GoToButton.gameObject.SetActive(true);
                        m_GetButton.gameObject.SetActive(false);
                    }
                    HideFx();
                }
            }

            //Change_UI，uint_array1，规则为：跳转类型|界面ID|推送ID， 目前为3种类型，1=跳转界面，3=日常活动界面，=0没有跳转目标。
            //当类型为1时，需要有推送ID，判断功能是否开启，当类型为3时，填写日常活动界面的ID，无需填写推送ID，当类型为0时，按钮无跳转。
            //Tip，uint，前端字段，填写语言表ID，当Change_UI的配置为0|0时读取该字段内的语言表ID，点击前往按钮时飘字提示。
            private void OnGoToButtonClicked()
            {
                uint gotoType = m_CSVPedigreedTaskGroupData.Change_UI[0];
                if (gotoType == 1)
                {
                    uint uiID = m_CSVPedigreedTaskGroupData.Change_UI[1];
                    if (m_CSVPedigreedTaskGroupData.Change_UI.Count > 2)
                    {
                        uint funOpenID = m_CSVPedigreedTaskGroupData.Change_UI[2];
                        if (Sys_FunctionOpen.Instance.IsOpen(funOpenID, true))
                        {
                            UIManager.OpenUI((EUIID)uiID);
                        }
                    }
                    else
                    {
                        UIManager.OpenUI((EUIID)uiID);
                    }
                }
                else if (gotoType == 3)
                {
                    uint parmaID = m_CSVPedigreedTaskGroupData.Change_UI[1];
                    UIManager.OpenUI(EUIID.UI_DailyActivites, false, new UIDailyActivitesParmas() { SkipToID = parmaID });
                }
                else
                {
                    uint tips = m_CSVPedigreedTaskGroupData.Tip;
                    string content = LanguageHelper.GetTextContent(tips);
                    Sys_Hint.Instance.PushContent_Normal(content);
                }
                UIManager.CloseUI(EUIID.UI_OperationalActivity);
            }

            private void OnGetButtonClicked()
            {
                if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(118))
                {
                    string content = LanguageHelper.GetTextContent(2025306);
                    Sys_Hint.Instance.PushContent_Normal(content);
                    return;
                }
                if (!Sys_PedigreedDraw.Instance.InValidGetTime())
                {
                    string content = LanguageHelper.GetTextContent(2025307);
                    Sys_Hint.Instance.PushContent_Normal(content);
                    return;
                }
                Sys_PedigreedDraw.Instance.PetLotteryGetTaskAwardReq(Sys_PedigreedDraw.Instance.curCSVActivityRulerData.id, taskId);
            }

            private void ShowFx()
            {
                m_Fx.SetActive(true);
                m_FxPlayTimer?.Cancel();
                m_FxPlayTimer = Timer.Register(2, HideFx);
            }

            public void HideFx()
            {
                m_FxPlayTimer?.Cancel();
                m_Fx.SetActive(false);
            }
        }
    }
}


