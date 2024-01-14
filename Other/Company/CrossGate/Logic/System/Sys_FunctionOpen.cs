using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    /// <summary> 功能开启系统 </summary>
    public class Sys_FunctionOpen : SystemModuleBase<Sys_FunctionOpen>//, ISystemModuleUpdate
    {
        #region 数据定义
        /// <summary> 事件枚举 </summary>
        public enum EEvents
        {
            TriggerFunctionOpen,   //触发功能开启
            StopFunctionOpen,      //中断功能开启
            TriggerFuntion,        //触发具体功能
            CompletedFunctionOpen, //完成功能开启
            StopOtherActions,      //停止其他行为
            InitFinish,//初始完成功能开启
            UnLockAllFuntion,//解锁全部功能
        }
        /// <summary> 功能开放状态 </summary>
        public enum EFunctionOpenState
        {
            UnActived,  //未激活
            Waiting,    //等待中
            Executing,  //执行中
            Actived,    //已激活
        }
        /// <summary> 检测条件事件 </summary>
        public enum ECheckConditionEvents
        {
            View,           //界面
            PlayerLevel,    //玩家等级
            Task,           //任务
            TaskTarget,     //任务目标
            NPCFavorability,//Npc好感解锁
            TreasureUnlock, //宝藏库
            All,            //全部
            AwakenImprint,//觉醒印记
            TownTask,   //城镇任务
            WorldLevel,//世界等级
            BossTower, //boss资格挑战赛
        }

        /// <summary> 检测条件数据 </summary>
        public class CheckConditionData
        {
            #region 数据定义
            /// <summary> 检测条件数据 </summary>
            public CSVCheckseq.Data cSVCheckseqData { get; private set; }
            /// <summary> 包含所有检测条件类型 </summary>
            public HashSet<EConditionType> ConditionTypeList = new HashSet<EConditionType>();
            /// <summary> 所在界面Id </summary>
            public uint uiId;
            /// <summary> 是否忽略条件 </summary>
            public bool isIgnoreCondition = false;
            /// <summary> 缓存条件检查结果 </summary>
            public bool isCheckCondition = false;
            /// <summary> 缓存条件界面 </summary>
            public bool isOnView = false;
            /// <summary> 错误提示 </summary>
            public string strErrorTips = string.Empty;
            #endregion
            #region 功能处理
            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="conditionId"></param>
            /// <param name="uiId"></param>
            public void SetData(uint conditionId, uint uiId)
            {
                cSVCheckseqData = CSVCheckseq.Instance.GetConfData(conditionId);
                this.uiId = uiId;
                ConditionTypeList.Clear();
                if (null == cSVCheckseqData) return;
                var t = typeof(CSVCheckseq.Data);
                for (int i = 0; i < 10; i++)
                {
                    var fieldInfo = t.GetField(string.Concat("CheckCondi", (i + 1).ToString()));
                    if (null == fieldInfo) continue;
                    List<List<int>> conditionList = fieldInfo.GetValue(cSVCheckseqData) as List<List<int>>;
                    if (null == conditionList || conditionList.Count <= 0) continue;

                    for (int j = 0; j < conditionList.Count; j++)
                    {
                        List<int> condition = conditionList[j];
                        if (null == condition || condition.Count <= 0) continue;
                        EConditionType eConditionType = (EConditionType)condition[0];
                        if (!ConditionTypeList.Contains(eConditionType))
                        {
                            ConditionTypeList.Add(eConditionType);
                        }
                    }
                }
            }
            /// <summary>
            /// 检测条件
            /// </summary>
            /// <returns></returns>
            private void IsCheckCondition(ref bool value, ref string errorTips)
            {
                if (isIgnoreCondition)
                {
                    value = true;
                    errorTips = string.Empty;
                    return;
                }
                if (null == cSVCheckseqData)
                {
                    value = false;
                    errorTips = string.Empty;
                    return;
                }

                bool result = cSVCheckseqData.IsValid();
                if (result)
                {
                    value = true;
                    errorTips = string.Empty;
                }
                else
                {
                    value = false;
                    GetErrorTips(ref errorTips);
                }
            }
            /// <summary>
            /// 得到错误提示(只检测第一条列表)
            /// </summary>
            /// <returns></returns>
            public void GetErrorTips(ref string ErrorTips)
            {
                ErrorTips = string.Empty;

                if (null == cSVCheckseqData || null == cSVCheckseqData.CheckCondi1) return;

                for (int i = 0, count = cSVCheckseqData.CheckCondi1.Count; i < count; i++)
                {
                    List<int> CheckCondi = cSVCheckseqData.CheckCondi1[i];

                    bool isValue = ConditionManager.IsValid(CheckCondi);

                    if (!isValue)
                    {
                        EConditionType eConditionType = (EConditionType)CheckCondi[0];
                        switch (eConditionType)
                        {
                            case EConditionType.GreaterThanLv:
                                {
                                    int iParameter1 = CheckCondi.Count >= 2 ? CheckCondi[1] + 1 : 0;
                                    string strParameter1 = iParameter1.ToString();
                                    ErrorTips = LanguageHelper.GetTextContent(1004030, strParameter1);
                                }
                                break;
                            case EConditionType.TaskSubmitted:
                                {
                                    int iParameter1 = CheckCondi.Count >= 2 ? CheckCondi[1] : 0;
                                    CSVTask.Data cSVTaskData = CSVTask.Instance.GetConfData((uint)iParameter1);
                                    string strParameter1 = null == cSVTaskData ? string.Empty : LanguageHelper.GetTaskTextContent(cSVTaskData.taskName);
                                    ErrorTips = LanguageHelper.GetTextContent(1004031, strParameter1);
                                }
                                break;
                            default:
                                ErrorTips = LanguageHelper.GetTextContent(1004029);
                                break;
                        }
                        return;
                    }
                }
            }
            /// <summary>
            /// 检测界面
            /// </summary>
            /// <returns></returns>
            private bool IsOnView()
            {
                return uiId == 0 || UIManager.IsOpenState((EUIID)uiId);
            }
            /// <summary>
            /// 更新检测条件事件
            /// </summary>
            /// <param name="eCheckConditionEvents"></param>
            private void UpdateCheckCondition(ECheckConditionEvents eCheckConditionEvents)
            {
                switch (eCheckConditionEvents)
                {
                    case ECheckConditionEvents.View:
                        {
                            isOnView = IsOnView();
                        }
                        break;
                    case ECheckConditionEvents.PlayerLevel:
                        {
                            if (ConditionTypeList.Contains(EConditionType.GreaterThanLv) ||
                                ConditionTypeList.Contains(EConditionType.LessThanLv) ||
                                ConditionTypeList.Contains(EConditionType.GreaterOrEqualLv) ||
                                ConditionTypeList.Contains(EConditionType.EqualLv))
                            {
                                IsCheckCondition(ref isCheckCondition, ref strErrorTips);
                            }
                        }
                        break;
                    case ECheckConditionEvents.Task:
                        {
                            if (ConditionTypeList.Contains(EConditionType.HaveTask) ||
                                ConditionTypeList.Contains(EConditionType.TaskUnReceived) ||
                                ConditionTypeList.Contains(EConditionType.TaskUnCompleted) ||
                                ConditionTypeList.Contains(EConditionType.TaskCompleted) ||
                                ConditionTypeList.Contains(EConditionType.TaskSubmitted))
                            {
                                IsCheckCondition(ref isCheckCondition, ref strErrorTips);
                            }
                        }
                        break;
                    case ECheckConditionEvents.TaskTarget:
                        {
                            if (ConditionTypeList.Contains(EConditionType.TaskTargetUnCompleted) ||
                                ConditionTypeList.Contains(EConditionType.TaskTargetCompleted))
                            {
                                IsCheckCondition(ref isCheckCondition, ref strErrorTips);
                            }
                        }
                        break;
                    case ECheckConditionEvents.NPCFavorability:
                        {
                            if (ConditionTypeList.Contains(EConditionType.HasInquiryedFavorabilityNpc))
                            {
                                IsCheckCondition(ref isCheckCondition, ref strErrorTips);
                            }
                        }
                        break;
                    case ECheckConditionEvents.TownTask:
                        {
                            if (ConditionTypeList.Contains(EConditionType.TownTaskAvaiable))
                            {
                                IsCheckCondition(ref isCheckCondition, ref strErrorTips);
                            }
                        }
                        break;
                    case ECheckConditionEvents.TreasureUnlock:
                        if (ConditionTypeList.Contains(EConditionType.HaveTreasure))
                        {
                            IsCheckCondition(ref isCheckCondition, ref strErrorTips);
                        }
                        break;
                    case ECheckConditionEvents.All:
                        {
                            IsCheckCondition(ref isCheckCondition, ref strErrorTips);
                            isOnView = IsOnView();
                        }
                        break;
                    case ECheckConditionEvents.AwakenImprint:
                        if (ConditionTypeList.Contains(EConditionType.AwakenImprint))
                        {
                            IsCheckCondition(ref isCheckCondition, ref strErrorTips);
                        }
                        break;
                    case ECheckConditionEvents.WorldLevel:
                        if (ConditionTypeList.Contains(EConditionType.WorldLevel))
                        {
                            IsCheckCondition(ref isCheckCondition, ref strErrorTips);
                        }
                        break;
                    case ECheckConditionEvents.BossTower:
                        if (ConditionTypeList.Contains(EConditionType.CheckBossTowerState))
                        {
                            IsCheckCondition(ref isCheckCondition, ref strErrorTips);
                        }
                        break;
                }
            }
            /// <summary>
            /// 条件是否完成
            /// </summary>
            /// <param name="isIgnoreView"></param>
            /// <returns></returns>
            private bool IsCompletedCondition(bool isIgnoreView = false)
            {
                if (isIgnoreView)
                    return isCheckCondition;
                else
                    return isCheckCondition && isOnView;
            }
            /// <summary>
            /// 是否检测条件
            /// </summary>
            /// <param name="eCheckConditionEvents"></param>
            /// <param name="isIgnoreView"></param>
            /// <returns></returns>
            public bool IsCheckCondition(ECheckConditionEvents eCheckConditionEvents, bool isIgnoreView = false)
            {
                UpdateCheckCondition(eCheckConditionEvents);
                return IsCompletedCondition(isIgnoreView);
            }
            #endregion
        }
        /// <summary> 功能开启数据 </summary>
        public class FunctionOpenData
        {
            #region 静态数据
            /// <summary> 未激活列表(未激活,等待中,执行中) </summary>
            public static List<FunctionOpenData> list_UnActived = new List<FunctionOpenData>();
            /// <summary> 已激活列表(已激活) </summary>
            public static List<FunctionOpenData> list_Actived = new List<FunctionOpenData>();
            /// <summary> 等待激活中列表 </summary>
            public static List<FunctionOpenData> list_WaitActiving = new List<FunctionOpenData>();
            /// <summary> 当前正在执行功能开启 </summary>
            private static FunctionOpenData _curFunctionOpenData;
            public static FunctionOpenData curFunctionOpenData
            {
                get
                {
                    return _curFunctionOpenData;
                }
                set
                {
                    _curFunctionOpenData = value;
                    Sys_FunctionOpen.Instance.isRunning = _curFunctionOpenData != null;
                }
            }
            #endregion
            #region 数据定义
            /// <summary> 编号 </summary>
            public uint id { get; private set; } = 0;
            /// <summary> 功能开放表 </summary>
            public CSVFunctionOpen.Data cSVFunctionOpenData { get; private set; } = null;
            /// <summary> 触发条件 </summary>
            public CheckConditionData checkConditionData { get; private set; } = null;
            /// <summary> 功能开放状态 </summary>
            public EFunctionOpenState eFunctionOpenState { get; private set; } = EFunctionOpenState.UnActived;
            #endregion
            #region 功能处理
            /// <summary>
            /// 构建函数
            /// </summary>
            /// <param name="id"></param>
            public FunctionOpenData(uint id)
            {
                this.id = id;
                this.cSVFunctionOpenData = CSVFunctionOpen.Instance.GetConfData(id);
                this.checkConditionData = new CheckConditionData();
                this.checkConditionData.SetData(cSVFunctionOpenData.Condition_id, cSVFunctionOpenData.Active_UI);
            }
            /// <summary>
            /// 是否已打开功能
            /// </summary>
            /// <returns></returns>
            public bool IsOpen()
            {
                return eFunctionOpenState == EFunctionOpenState.Actived;
            }
            /// <summary>
            /// 激活状态中
            /// </summary>
            /// <returns></returns>
            public bool IsActiving()
            {
                return eFunctionOpenState != EFunctionOpenState.UnActived;
            }
            /// <summary>
            /// 设置功能开启状态并更新静态列表
            /// </summary>
            /// <param name="eFunctionOpenState"></param>
            public void SetFunctionOpenStateAndUpdateStaticList(EFunctionOpenState eFunctionOpenState)
            {
                SetFunctionOpenState(eFunctionOpenState);
                UpdateStaticList();
            }
            /// <summary>
            /// 设置功能开启状态
            /// </summary>
            /// <param name="eFunctionOpenState"></param>
            public void SetFunctionOpenState(EFunctionOpenState eFunctionOpenState)
            {
                this.eFunctionOpenState = eFunctionOpenState;
            }
            /// <summary>
            /// 更新置对应状态下的静态列表
            /// </summary>
            public void UpdateStaticList()
            {
                //设置是否激活列表
                switch (eFunctionOpenState)
                {
                    case EFunctionOpenState.UnActived:
                    case EFunctionOpenState.Waiting:
                    case EFunctionOpenState.Executing:
                        {
                            if (!list_UnActived.Contains(this))
                            {
                                list_UnActived.Add(this);
                            }
                            else if (list_Actived.Contains(this))
                            {
                                list_Actived.Remove(this);
                            }
                        }
                        break;
                    case EFunctionOpenState.Actived:
                        {
                            if (!list_Actived.Contains(this))
                            {
                                list_Actived.Add(this);
                            }
                            else if (list_UnActived.Contains(this))
                            {
                                list_UnActived.Remove(this);
                            }
                        }
                        break;
                }
                //设置等待激活列表
                switch (eFunctionOpenState)
                {
                    case EFunctionOpenState.UnActived:
                    case EFunctionOpenState.Actived:
                        {
                            if (list_WaitActiving.Contains(this))
                            {
                                list_WaitActiving.Remove(this);
                            }
                        }
                        break;
                    case EFunctionOpenState.Waiting:
                    case EFunctionOpenState.Executing:
                        {
                            if (!list_WaitActiving.Contains(this))
                            {
                                list_WaitActiving.Add(this);
                            }
                        }
                        break;
                }
                //检测当前需要执行的功能开启
                CheckExecuteFunctionOpen();
            }
            /// <summary>
            /// 检测检测条件
            /// </summary>
            public void CheckCondition(ECheckConditionEvents eCheckConditionEvents)
            {
                switch (eFunctionOpenState)
                {
                    case EFunctionOpenState.UnActived://未激活
                        {
                            if (checkConditionData.IsCheckCondition(eCheckConditionEvents))
                            {
                                TriggerFunctionOpen(); //触发功能开启
                            }
                        }
                        break;
                    case EFunctionOpenState.Waiting:  //等待中
                    case EFunctionOpenState.Executing://执行中
                        {
                            if (!checkConditionData.IsCheckCondition(eCheckConditionEvents))
                            {
                                StopFunctionOpen();  //中断功能开启
                            }
                        }
                        break;
                    case EFunctionOpenState.Actived: //已激活
                        { }
                        break;
                }
            }
            /// <summary>
            /// 触发功能开启
            /// </summary>
            /// <param name="sendMessage"></param>
            public void TriggerFunctionOpen(bool sendMessage = true)
            {
                bool isOpenView = cSVFunctionOpenData.Active_UI != 0;
                if (isOpenView)
                {
                    ExecuteFunctionOpen(sendMessage);
                }
                else
                {
                    CompletedFunctionOpen(sendMessage);
                }
            }
            /// <summary>
            /// 执行功能开启
            /// </summary>
            /// <param name="sendMessage"></param>
            public void ExecuteFunctionOpen(bool sendMessage = true)
            {
                //设置状态
                SetFunctionOpenStateAndUpdateStaticList(EFunctionOpenState.Waiting);
                //发送事件
                if (sendMessage)
                    Sys_FunctionOpen.Instance.eventEmitter.Trigger<FunctionOpenData>(Sys_FunctionOpen.EEvents.TriggerFunctionOpen, this);
            }
            /// <summary>
            /// 中断功能开启
            /// </summary>
            public void StopFunctionOpen(bool sendMessage = true)
            {
                //设置状态
                SetFunctionOpenStateAndUpdateStaticList(EFunctionOpenState.UnActived);
                //发送事件
                if (sendMessage)
                    Sys_FunctionOpen.Instance.eventEmitter.Trigger<FunctionOpenData>(Sys_FunctionOpen.EEvents.StopFunctionOpen, this);
            }
            /// <summary>
            /// 检测执行功能开启
            /// </summary>
            public void CheckExecuteFunctionOpen()
            {
                if (null != curFunctionOpenData)
                {
                    if (curFunctionOpenData.eFunctionOpenState != EFunctionOpenState.Executing) //功能开启状态非执行
                    {
                        curFunctionOpenData = null;
                    }
                    else
                    {
                        return; //功能开启正在触发中且未完成
                    }
                }

                if (list_WaitActiving.Count > 0)
                {
                    curFunctionOpenData = list_WaitActiving[0]; //获取功能开启
                    curFunctionOpenData.ExecuteFunction();
                }
            }
            /// <summary>
            /// 执行功能
            /// </summary>
            public void ExecuteFunction(bool sendMessage = true)
            {
                //设置状态
                SetFunctionOpenStateAndUpdateStaticList(EFunctionOpenState.Executing);
                //堆栈中如果存在功能推送界面，刷新界面状态
                UIBase uIBase = UIManager.GetUI((int)EUIID.UI_Newfunction);
                if (null != uIBase)
                {
                    UIManager.UpdateState();
                }
                //处理功能
                UIManager.OpenUI(EUIID.UI_Newfunction, false, this);
                //关闭界面
                if (UIManager.IsOpen(EUIID.UI_Chat))
                {
                    UIManager.CloseUI(EUIID.UI_Chat);
                }
                //发送事件
                if (sendMessage)
                    Sys_FunctionOpen.Instance.eventEmitter.Trigger<FunctionOpenData>(Sys_FunctionOpen.EEvents.TriggerFuntion, this);
            }
            /// <summary>
            /// 完成功能开启
            /// </summary>
            public void CompletedFunctionOpen(bool sendMessage = true)
            {
                //设置状态
                SetFunctionOpenStateAndUpdateStaticList(EFunctionOpenState.Actived);
                //处理功能
                if (UIManager.IsVisibleAndOpen((EUIID)cSVFunctionOpenData.Icon_UI))
                {
                    if (!Sys_FamilyResBattle.Instance.InFamilyBattle) {
                        Sys_FunctionOpen.Instance.SetTextureState(this);
                    }
                }
                //发送事件
                if (sendMessage)
                    Sys_FunctionOpen.Instance.eventEmitter.Trigger<FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, this);
            }
            /// <summary>
            /// 设置忽略条件
            /// </summary>
            /// <param name="value"></param>
            public void SetIgnoreCondition(bool value)
            {
                checkConditionData.isIgnoreCondition = value;
            }
            /// <summary>
            /// 清理数据
            /// </summary>
            public static void Clear()
            {
                list_UnActived.Clear();
                list_Actived.Clear();
                list_WaitActiving.Clear();
                curFunctionOpenData = null;
            }
            #endregion
        }
        /// <summary> 数据加载完成数量 </summary>
        private int SyncFinishedCount = 0;
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        /// <summary> 功能开放字典 </summary>
        public Dictionary<uint, FunctionOpenData> dict_AllFunctionOpen = new Dictionary<uint, FunctionOpenData>();
        /// <summary> 查找各个界面包含功能开放 </summary>
        private Dictionary<uint, List<uint>> dict_View = new Dictionary<uint, List<uint>>();
        /// <summary> 是否正在运行 </summary>
        private bool _isRunning = false;
        public bool isRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    Sys_FunctionOpen.Instance.eventEmitter.Trigger(Sys_FunctionOpen.EEvents.StopOtherActions, value);
                }
            }
        }
        #endregion
        #region 系统函数
        public override void Init()
        {
            InitData();
            ProcessEvents(true);
        }
        public override void Dispose()
        {
            ProcessEvents(false);
        }
        public override void OnLogin()
        {
            SyncFinishedCount = 0;
            ClearData();
        }
        public override void OnLogout()
        {
            ClearData();
            CloseNewfunctionView();
        }
        public override void OnSyncFinished()
        {
            SyncFinishedCount++;
            if (SyncFinishedCount == 2) //一般数据和任务数据加载完成后开始处理
            {
                OnCheckUnActivedFunctionOpen();

                eventEmitter.Trigger(EEvents.InitFinish);
            }
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            dict_AllFunctionOpen.Clear();
            dict_View.Clear();
            FunctionOpenData.Clear();

            var data = CSVFunctionOpen.Instance.GetAll();
            foreach (var item in data)
            {
                dict_AllFunctionOpen.Add(item.id, new FunctionOpenData(item.id));
            }
            foreach (var item in dict_AllFunctionOpen)
            {
                uint uiID = item.Value.cSVFunctionOpenData.Icon_UI;

                if (!dict_View.ContainsKey(uiID))
                {
                    dict_View.Add(uiID, new List<uint>() { item.Key });
                }
                else
                {
                    dict_View[uiID].Add(item.Key);
                }
            }
        }
        /// <summary>
        /// 清理数据
        /// </summary>
        public void ClearData()
        {
            isRunning = false;
            FunctionOpenData.Clear();
            foreach (var item in dict_AllFunctionOpen.Values)
            {
                item.SetFunctionOpenState(EFunctionOpenState.UnActived);
            }
        }
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="toRegister"></param>
        protected void ProcessEvents(bool toRegister)
        {
            Sys_Task.Instance.eventEmitter.Handle(Sys_Task.EEvents.OnFinishedTasksGot, OnFinishedTasksGot, toRegister);
            //各个相关事件触发条件变化
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.HasEvent, OnView, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginEnter, OnEnterView, toRegister);

            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnUpdatePlayerLevel, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<TaskEntry, ETaskState, ETaskState>(Sys_Task.EEvents.OnTaskStatusChanged, OnTaskStatusChanged, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<uint, uint, bool, bool>(Sys_Task.EEvents.OnTaskGoalStatusChanged, OnTaskGoalStatusChanged, toRegister);
            Sys_NPCFavorability.Instance.eventEmitter.Handle<uint, uint>(Sys_NPCFavorability.EEvents.OnNPCUnlock, OnNPCUnlock, toRegister);
            Sys_Treasure.Instance.eventEmitter.Handle(Sys_Treasure.EEvents.OnUnlockNotify, OnTreasureUnlock, toRegister);

            Sys_NPCFavorability.Instance.eventEmitter.Handle<uint, uint, uint>(Sys_NPCFavorability.EEvents.OnFavorabilityStageChanged, OnFavorabilityStageChanged, toRegister);
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnUpdateOpenServiceDay, OnUpdateOpenServiceDay, toRegister);            Sys_ActivityBossTower.Instance.eventEmitter.Handle(Sys_ActivityBossTower.EEvents.OnBossTowerStateChange, OnBossTowerStateChange, toRegister);
        }
        #endregion
        #region 数据处理
        /// <summary>
        /// 等待任务数据全部完成
        /// </summary>
        public void OnFinishedTasksGot()
        {
            OnSyncFinished();
        }
        /// <summary>
        /// 检测未激活的功能开放
        /// </summary>
        public void OnCheckUnActivedFunctionOpen()
        {
            FunctionOpenData.Clear();
            foreach (var item in dict_AllFunctionOpen.Values)
            {
                if (item.checkConditionData.IsCheckCondition(ECheckConditionEvents.All, true))
                {
                    item.SetFunctionOpenStateAndUpdateStaticList(EFunctionOpenState.Actived);
                }
                else
                {
                    item.SetFunctionOpenStateAndUpdateStaticList(EFunctionOpenState.UnActived);
                }
            }
        }
        /// <summary>
        /// 解锁一个功能开放(GM测试)
        /// </summary>
        /// <param name="id"></param>
        public void OnUnlockOneFunctionOpen(uint id)
        {
            FunctionOpenData functionOpenData = null;
            if (!dict_AllFunctionOpen.TryGetValue(id, out functionOpenData))
                return;
            //忽略条件
            functionOpenData.SetIgnoreCondition(true);
            //执行触发功能开启
            functionOpenData.TriggerFunctionOpen();
        }
        /// <summary>
        /// 解锁全部功能开放(GM测试)
        /// </summary>
        public void OnUnlockAllFunctionOpen(bool isPlayAnimation)
        {
            FunctionOpenData.Clear();
            var list = new List<uint>(dict_AllFunctionOpen.Keys);
            for (int i = 0; i < list.Count; i++)
            {
                var item = dict_AllFunctionOpen[list[i]];
                item.SetIgnoreCondition(true);
                //执行触发功能开启
                bool isSendMessage = list.Count - 1 == i;

                if (isPlayAnimation)
                {
                    item.TriggerFunctionOpen(isSendMessage);
                }
                else
                {
                    item.CompletedFunctionOpen(isSendMessage);
                }
            }

            eventEmitter.Trigger(EEvents.UnLockAllFuntion);
        }
        /// <summary>
        /// 界面操作
        /// </summary>
        /// <param name="stackID"></param>
        /// <param name="nEventFlage"></param>
        public void OnView(uint stackID, int nEventFlage)
        {
            if (0 != (nEventFlage & ((int)UIStack.EUIStackEvent.EndExit | (int)UIStack.EUIStackEvent.BeginEnter)))
            {
                OnUpdateCondition(ECheckConditionEvents.View);
            }
        }
        /// <summary>
        /// 进入界面
        /// </summary>
        /// <param name="Id"></param>
        public void OnEnterView(uint stack, int Id)
        {
            if (!Sys_FamilyResBattle.Instance.InFamilyBattle) {
                SetAllTextureOnView(Id);
            }
        }
        /// <summary>
        /// 更新玩家等级
        /// </summary>
        private void OnUpdatePlayerLevel()
        {
            OnUpdateCondition(ECheckConditionEvents.PlayerLevel);
            OnUpdateCondition(ECheckConditionEvents.AwakenImprint);//觉醒印记开启条件玩家等级30级
        }
        /// <summary>
        /// 更新任务
        /// </summary>
        private void OnTaskStatusChanged(TaskEntry taskEntry, ETaskState oldStatus, ETaskState newStatus)
        {
            OnUpdateCondition(ECheckConditionEvents.Task);
        }
        /// <summary>
        /// 更新任务目标
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskGoalId"></param>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        private void OnTaskGoalStatusChanged(uint taskId, uint taskGoalId, bool oldState, bool newState)
        {
            OnUpdateCondition(ECheckConditionEvents.TaskTarget);
        }
        /// <summary>
        /// 解锁Npc好感度
        /// </summary>
        private void OnNPCUnlock(uint zoneId, uint npcId)
        {
            OnUpdateCondition(ECheckConditionEvents.NPCFavorability);
        }

        /// <summary>
        /// NPC好感度阶段改变///
        /// </summary>
        /// <param name="infoID"></param>
        /// <param name="old"></param>
        /// <param name=""></param>
        void OnFavorabilityStageChanged(uint infoID, uint oldValue, uint newValue)
        {
            OnUpdateCondition(ECheckConditionEvents.TownTask);
        }
        /// <summary>
        /// 世界等级
        /// </summary>
        private void OnUpdateOpenServiceDay()
        {
            OnUpdateCondition(ECheckConditionEvents.WorldLevel);
        }
        /// <summary>
        /// 解锁宝藏库
        /// </summary>
        private void OnTreasureUnlock()
        {
            OnUpdateCondition(ECheckConditionEvents.TreasureUnlock);
        }

        /// <summary>
        /// boss战阶段改变
        /// </summary>
        private void OnBossTowerStateChange()
        {
            OnUpdateCondition(ECheckConditionEvents.BossTower);
            //已激活列表
            for (int i = FunctionOpenData.list_Actived.Count - 1; i >= 0; i--)
            {
                FunctionOpenData functionOpenData = FunctionOpenData.list_Actived[i];
                if (functionOpenData.checkConditionData.ConditionTypeList.Contains(EConditionType.CheckBossTowerState))
                {
                    if (!functionOpenData.checkConditionData.IsCheckCondition(ECheckConditionEvents.BossTower))
                    {
                        functionOpenData.StopFunctionOpen();
                    }
                }
            }
        }
        /// <summary>
        /// 更新条件
        /// </summary>
        public void OnUpdateCondition(ECheckConditionEvents eCheckConditionEvents)
        {
            //未激活列表
            for (int i = 0, count = FunctionOpenData.list_UnActived.Count; i < count; i++)
            {
                FunctionOpenData functionOpenData = FunctionOpenData.list_UnActived[i];
                functionOpenData.CheckCondition(eCheckConditionEvents);
            }
        }
        #endregion
        #region 功能函数
        /// <summary>
        /// 是否能打开
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="isTips"></param>
        /// <param name="containActivingState"></param>
        /// <returns></returns>
        public bool IsOpen(uint openID, bool isTips = false, bool containActivingState = false)
        {
            FunctionOpenData functionOpenData = null;
            if (!dict_AllFunctionOpen.TryGetValue(openID, out functionOpenData))
                return false;

            bool result = containActivingState ? functionOpenData.IsActiving() : functionOpenData.IsOpen();
            if (!result && isTips)
            {
                if (openID==51402)
                {
                    Sys_Hint.Instance.PushContent_Normal(Sys_TravellerAwakening.Instance.AwakenImprintErrorText());
                    return result;
                }
                Sys_Hint.Instance.PushContent_Normal(functionOpenData.checkConditionData.strErrorTips);
            }
            return result;
        }
        /// <summary>
        /// 当前界面所有功能推送设置图标
        /// </summary>
        /// <param name="Id"></param>
        public void SetAllTextureOnView(int Id)
        {
            List<uint> functionOpenDatas = null;
            dict_View.TryGetValue((uint)Id, out functionOpenDatas);
            if (null == functionOpenDatas) return;

            foreach (uint id in functionOpenDatas)
            {
                SetTextureState(dict_AllFunctionOpen[id]);
            }
        }
        /// <summary>
        /// 设置图片状态
        /// </summary>
        /// <param name="data"></param>
        public void SetTextureState(FunctionOpenData functionOpenData)
        {
            if (string.IsNullOrEmpty(functionOpenData.cSVFunctionOpenData.Icon_Path)) return;
            Transform tr = UIManager.mRoot.Find(functionOpenData.cSVFunctionOpenData.Icon_Path);
            if (null == tr)
            {
                DebugUtil.LogError(string.Format("功能推送ID:{0}未找到界面路径:{1}", functionOpenData.id, functionOpenData.cSVFunctionOpenData.Icon_Path));
                return;
            }
            bool isOpen = functionOpenData.eFunctionOpenState == EFunctionOpenState.Actived;
            switch (functionOpenData.cSVFunctionOpenData.functionClose)
            {
                case 1://显&隐
                    {
                        tr.gameObject.SetActive(isOpen);
                    }
                    break;
                case 2://灰态&高亮
                    {
                        ImageHelper.SetImageGray(tr, !isOpen, true);
                    }
                    break;
            }
        }
        /// <summary>
        /// 关闭新功能界面
        /// </summary>
        public void CloseNewfunctionView()
        {
            if (UIManager.IsOpen(EUIID.UI_Newfunction))
                UIManager.CloseUI(EUIID.UI_Newfunction);
        }
        #endregion
    }
}