using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    /// <summary> 新手引导系统 </summary>
    public class Sys_Guide : SystemModuleBase<Sys_Guide>
    {
        #region 数据定义
        /// <summary> 事件枚举 </summary>
        public enum EEvents
        {
            ExecutingGuideGroup,      //执行引导组
            StopGuideGroup,           //终止引导组
            CompletedGuideGroup,      //完成引导组
            TriggerGuide,             //触发引导
            CompletedGuide,           //完成引导
            AddUnForceGuide,          //添加非强制引导
            RemoveUnForceGuide,       //删除非强制引导
            OnClickAutoFightBtn,      //点击自动战斗按钮
            OnFightForceGuide,        //在战斗中触发强制引导

        }
        /// <summary> 引导条件 </summary>
        public enum EGuideCondition
        {
            Job,         //职业
            Level,       //等级
            Map,         //地图
            Task,        //已接取任务
            TaskFinish,  //任务完成
            MonsterGroup,//怪物组
            Round,       //回合
            TaskGoal,    //任务目标
            FunctionOpen,//功能推送
            ViewTabOpen, //页面指定页签打开
        }
        /// <summary> 引导状态 </summary>
        public enum EGuideState
        {
            UnActived,  //未激活
            Waiting,    //等待中
            Executing,  //执行中
            Completed,  //已完成
        }
        /// <summary> 引导模式 </summary>
        public enum EGuideMode
        {
            UnForce = 0,      //非强制
            Force = 1,        //强制
            CheckForce = 2,   //检测(只用于首强制引导判断分支,无操作流程)
        }
        /// <summary> 引导阶段 </summary>
        public enum EGuidePhase
        {
            OpenBackgroundMask = 1,    //开启背景遮罩
            CloseBackgroundMask = 2,   //关闭背景遮罩
            OpenDialog = 3,            //打开对话框
            CloseDialog = 4,           //关闭对话框
            OpenEffect = 5,            //打开特效
            CloseEffect = 6,           //关闭特效
            OpenGuideIcon = 7,         //打开指引图标
            CloseGuideIcon = 8,        //关闭指引图标
            SetCompletedOption = 9,    //设置完成选项
            CannelCompletedOption = 10,//关闭完成选项
            WaitTime = 11,             //等待时间
            OpenView = 12,             //打开界面
        }
        /// <summary> 引导界面功能 </summary>
        public enum EGuideViewFunctionType
        {
            None = 0,       //无
            LiveSkill = 1,  //生活技能
            Pet = 2,        //宠物
        }
        /// <summary> 引导组 </summary>
        public class GuideGroup
        {
            #region 数据定义
            /// <summary> 是否已保存数据 </summary>
            public bool isSaveData { get; set; } = false;
            /// <summary> 编号 </summary>
            public uint Id { get; private set; } = 0;
            /// <summary> 引导组表 </summary>
            public CSVGuideGroup.Data cSVGuideGroupData { get; private set; } = null;
            /// <summary> 引导组当前状态 </summary>
            public EGuideState guideGroupState { get; private set; } = EGuideState.UnActived;
            /// <summary> 触发条件组 </summary>
            public TriggerSlotGroup triggerGroup = new TriggerSlotGroup();
            /// <summary> 引导树 </summary>
            public Dictionary<uint, GuideTree<GuideTask>> dict_GuideTrees = new Dictionary<uint, GuideTree<GuideTask>>();
            /// <summary> 引导树条件 </summary>
            public Dictionary<uint, GuideTreeCondition> dict_GuideTreesCondition = new Dictionary<uint, GuideTreeCondition>();
            /// <summary> 根引导ID </summary>
            public uint rootGuideId { get; private set; } = 0;
            /// <summary> 当前执行引导ID </summary>
            public uint curGuideId { get; private set; } = 0;
            /// <summary> 当前执行引导树节点 </summary>
            public GuideTree<GuideTask> curGuideTrees
            {
                get
                {
                    if (!dict_GuideTrees.ContainsKey(curGuideId))
                        return null;
                    else
                        return dict_GuideTrees[curGuideId];
                }
            }
            /// <summary> 当前执行的引导 </summary>
            public GuideTask curGuideTask
            {
                get
                {
                    if (null == curGuideTrees) return null;
                    else return curGuideTrees.Data;
                }
            }

            #endregion
            #region 初始化
            /// <summary>
            /// 构建函数
            /// </summary>
            /// <param name="id"></param>
            public GuideGroup(uint Id)
            {
                this.Id = Id;
                this.cSVGuideGroupData = CSVGuideGroup.Instance.GetConfData(Id);
                SetGuideTaskList();
                SetTriggerConditionGroup();
            }
            /// <summary>
            /// 设置引导任务列表
            /// </summary>
            private void SetGuideTaskList()
            {
                //构建引导树
                GuideTree<GuideTask> guideTree = null;
                if (null != cSVGuideGroupData.link_id)
                {
                    for (int i = 0, iCount = cSVGuideGroupData.link_id.Count; i < iCount; i++)
                    {
                        var group = cSVGuideGroupData.link_id[i];
                        guideTree = null;

                        for (int j = 0, jCount = group.Count; j < jCount; j++)
                        {
                            uint id = group[j];
                            GuideTree<GuideTask> tree;
                            if (!dict_GuideTrees.TryGetValue(id, out tree))
                            {
                                GuideTask guideTask = new GuideTask(id, cSVGuideGroupData.id);
                                tree = new GuideTree<GuideTask>(guideTask);
                                dict_GuideTrees.Add(id, tree);
                                tree.IsRoot = j == 0;
                                if (tree.IsRoot)
                                    rootGuideId = tree.Data.Id;
                            }

                            if (null != guideTree && guideTree.LeftTree != tree && guideTree.RightTree != tree)//不能有两个相同的节点
                            {
                                if (null == guideTree.LeftTree)
                                {
                                    guideTree.LeftTree = tree;
                                }
                                else if (null == guideTree.RightTree)
                                {
                                    guideTree.RightTree = tree;
                                }
                                else
                                {
                                    DebugUtil.LogError(string.Format("引导树节点构建失败,引导树节点已满。引导组ID:{0},引导ID:{1}", cSVGuideGroupData.id, id.ToString()));
                                }
                            }
                            guideTree = tree;
                        }
                    }
                }
                //设置节点筛选条件
                if (null != cSVGuideGroupData.condition)
                {
                    for (int i = 0, iCount = cSVGuideGroupData.condition.Count; i < iCount; i++)
                    {
                        var group = cSVGuideGroupData.condition[i];

                        if (group.Count == 3)
                        {
                            uint guideId = (uint)group[0];       //存在分支引导ID
                            uint conditionId = (uint)group[1];   //设置分支条件ID
                            int targetGuideId = group[2]; //运行跳转引导ID(条件不满足时)

                            dict_GuideTreesCondition.Add(guideId, new GuideTreeCondition(conditionId, targetGuideId));
                        }
                        else
                        {
                            DebugUtil.LogError(string.Format("引导数节点设置条件参数失败。引导组ID:{0}。", cSVGuideGroupData.id));
                        }
                    }
                }
                //设置初始引导ID
                curGuideId = rootGuideId;
            }
            /// <summary>
            /// 设置触发条件组
            /// </summary>
            private void SetTriggerConditionGroup()
            {
                List<TriggerSlot> triggerConditions = new List<TriggerSlot>();
                bool isActive = System.Convert.ToBoolean(cSVGuideGroupData.mode);
                //主动触发的不设置触发条件,被动触发的设置触发条件
                if (!isActive)
                {
                    var values = System.Enum.GetValues(typeof(EGuideCondition));
                    foreach (EGuideCondition item in values)
                    {
                        switch (item)
                        {
                            case EGuideCondition.Job:
                                {
                                    if (cSVGuideGroupData.career_condition?.Count > 0)
                                    {
                                        triggerConditions.Add(new TriggerCondition_RoleJob(cSVGuideGroupData.career_condition));
                                    }
                                }
                                break;
                            case EGuideCondition.Level:
                                {
                                    if (cSVGuideGroupData.lv?.Count == 2)
                                    {
                                        triggerConditions.Add(new TriggerCondition_PlayerLevel((int)cSVGuideGroupData.lv[0], (int)cSVGuideGroupData.lv[1]));
                                    }
                                }
                                break;
                            case EGuideCondition.Map:
                                {
                                    if (cSVGuideGroupData.instance_limited?.Count > 0)
                                    {
                                        triggerConditions.Add(new TriggerCondition_CurMap(cSVGuideGroupData.instance_limited));
                                    }
                                }
                                break;
                            case EGuideCondition.Task:
                                {
                                    if (cSVGuideGroupData.task_limited != 0)
                                    {
                                        triggerConditions.Add(new TriggerCondition_CurTask(cSVGuideGroupData.task_limited));
                                    }
                                }
                                break;
                            case EGuideCondition.TaskFinish:
                                {
                                    if (cSVGuideGroupData.task_finish != 0)
                                    {
                                        triggerConditions.Add(new TriggerCondition_FinishTasks(new List<uint> { cSVGuideGroupData.task_finish }));
                                    }
                                }
                                break;
                            case EGuideCondition.MonsterGroup:
                                {
                                    if (cSVGuideGroupData.enemy_id != 0)
                                    {
                                        triggerConditions.Add(new TriggerCondition_MonsterGroup(cSVGuideGroupData.enemy_id));
                                    }
                                }
                                break;
                            case EGuideCondition.Round:
                                {
                                    if (cSVGuideGroupData.fight_time?.Count > 0)
                                    {
                                        triggerConditions.Add(new TriggerCondition_FightRound(cSVGuideGroupData.fight_time));
                                    }
                                }
                                break;
                            case EGuideCondition.TaskGoal:
                                {
                                    if (cSVGuideGroupData.tasktarget_finish != 0)
                                    {
                                        triggerConditions.Add(new TriggerCondition_TaskGoal(cSVGuideGroupData.tasktarget_finish));
                                    }
                                }
                                break;
                            case EGuideCondition.FunctionOpen:
                                {
                                    if (cSVGuideGroupData.function_id != 0)
                                    {
                                        triggerConditions.Add(new TriggerCondition_FunctionOpen(cSVGuideGroupData.function_id));
                                    }
                                }
                                break;
                            case EGuideCondition.ViewTabOpen:
                                {
                                    if (cSVGuideGroupData.Tab_path != null)
                                    {
                                        triggerConditions.Add(new TriggerCondition_OpenViewPage(cSVGuideGroupData.UI_id,cSVGuideGroupData.Tab_path));
                                    }
                                }
                                break;
                        }
                    }
                }
                triggerGroup.SetTriggerList(triggerConditions);
            }
            #endregion
            #region 数据处理
            /// <summary>
            /// 触发条件组发生变化(引导组内部条件变化)
            /// </summary>
            /// <param name="count"></param>
            public void OnTriggerGroupChanged(int count)
            {
                //触发条件是否完成
                bool isConditionComplete = triggerGroup.isFinish;
                switch (guideGroupState)
                {
                    case EGuideState.UnActived: //未激活状态
                        {
                            if (isConditionComplete)
                            {
                                ExecutingGuideGroup();
                            }
                        }
                        break;
                    case EGuideState.Executing: //执行状态
                        {
                            if (!isConditionComplete)
                            {
                                foreach (TriggerSlot item in triggerGroup.triggers.Keys)
                                {
                                    if (!item.isFinish && item.GetType() == typeof(TriggerCondition_FightRound))
                                    {
                                        SkipGuideGroup();
                                        return;
                                    }
                                }
                                StopGuideGroup();
                            }
                        }
                        break;
                }
            }
            /// <summary>
            /// 终止引导组
            /// </summary>
            public void StopGuideGroup()
            {
                SetGuideGroupState(EGuideState.UnActived);
                Sys_Guide.Instance.eventEmitter.Trigger<GuideGroup>(EEvents.StopGuideGroup, this);
                StopCurGuide();
            }
            /// <summary>
            /// 停止当前引导
            /// </summary>
            public void StopCurGuide()
            {
                if (null != curGuideTask)
                {
                    SetGuideListen(curGuideTask, false);
                    curGuideTask.StopGuide();
                }
            }
            /// <summary>
            /// 跳过引导组
            /// </summary>
            public void SkipGuideGroup()
            {
                if (null != curGuideTask)
                {
                    curGuideTask.SkipGuide();
                }
            }
            /// <summary>
            /// 执行引导组
            /// </summary>
            public void ExecutingGuideGroup()
            {
                SetGuideGroupState(EGuideState.Executing);
                Sys_Guide.Instance.eventEmitter.Trigger<GuideGroup>(EEvents.ExecutingGuideGroup, this);
                if (null != curGuideTrees)
                {
                    ExecutingCurGuideTree(curGuideTrees, false);
                }
                if (cSVGuideGroupData.enemy_id != 0)
                {
                    Sys_Guide.Instance.eventEmitter.Trigger(EEvents.OnFightForceGuide);
                }
            }
            /// <summary>
            /// 完成引导组
            /// </summary>
            public void CompletedGuideGroup()
            {
                SetGuideGroupState(EGuideState.Completed);
                SetListen(false, false);
                Sys_Guide.Instance.eventEmitter.Trigger<GuideGroup>(EEvents.CompletedGuideGroup, this);
            }
            /// <summary>
            /// 跳至引导链结束
            /// </summary>
            /// <param name="guideTask"></param>
            public void SkipGuide(GuideTask guideTask)
            {
                SetGuideListen(guideTask, false);
                SkipGuideTree(guideTask.Id);
            }
            /// <summary>
            /// 完成引导
            /// </summary>
            /// <param name="guideTask"></param>
            public void CompletedGuide(GuideTask guideTask)
            {
                //当前引导监听关闭
                SetGuideListen(guideTask, false);
                //尝试保存数据
                TrySaveData(guideTask);
                //执行下条引导树
                GuideTree<GuideTask> tree = null;
                if (dict_GuideTrees.TryGetValue(guideTask.Id, out tree))
                {
                    ExecutingNextGuideTree(tree, true);
                }
            }
            /// <summary>
            /// 跳至导树叶节点
            /// </summary>
            /// <param name="Id"></param>
            public void SkipGuideTree(uint Id)
            {
                GuideTree<GuideTask> tree = null;
                if (dict_GuideTrees.TryGetValue(Id, out tree))
                {
                    curGuideId = SkipTreeRoot(tree).Data.Id;
                    CompletedGuide(curGuideTask);
                }
            }
            /// <summary>
            /// 获取引导树叶节点数据
            /// </summary>
            /// <param name="guideTree"></param>
            /// <returns></returns>
            public GuideTree<GuideTask> SkipTreeRoot(GuideTree<GuideTask> guideTree)
            {
                if (guideTree.LeftTree != null)
                {
                    TrySaveData(guideTree.Data);
                    return SkipTreeRoot(guideTree.LeftTree);
                }
                else if (guideTree.RightTree != null)
                {
                    TrySaveData(guideTree.Data);
                    return SkipTreeRoot(guideTree.RightTree);
                }
                return guideTree;
            }
            /// <summary>
            /// 执行当前引导树节点
            /// </summary>
            /// <param name="guideTree"></param>
            /// <param name="isInsert"></param>
            public void ExecutingCurGuideTree(GuideTree<GuideTask> guideTree, bool isInsert)
            {
                //引导开启监听
                SetGuideListen(guideTree.Data, true);
                if (isInsert)
                    guideTree.Data.InsertGuide();
            }
            /// <summary>
            /// 执行下一个引导树节点
            /// </summary>
            /// <param name="guideTree"></param>
            /// <param name="isInsert"></param>
            public void ExecutingNextGuideTree(GuideTree<GuideTask> guideTree, bool isInsert)
            {
                if (guideTree.IsLeaf)
                {
                    CompletedGuideGroup();
                }
                else
                {
                    uint guideId = guideTree.Data.Id;
                    int id = GetNextGuideTree(guideId);
                    if (id == 0)
                    {
                        DebugUtil.LogError(string.Format("执行下一条引导失败。引导组ID:{0},当前引导ID:{1},下一条引导ID:{2}。", cSVGuideGroupData.id, guideId, id));
                    }
                    else if (id < 0)
                    {
                        SkipGuideTree((uint)id);
                    }
                    else
                    {
                        curGuideId = (uint)id;
                        if (null != curGuideTrees)
                        {
                            ExecutingCurGuideTree(curGuideTrees, isInsert);
                        }
                    }
                }
            }
            /// <summary>
            /// 得到引导树下一个引导ID
            /// </summary>
            /// <param name="Id"></param>
            /// <returns></returns>
            public int GetNextGuideTree(uint Id)
            {
                int nextId = -1;
                bool isValue = true;
                //是否存在引导树条件
                GuideTreeCondition guideTreeCondition = null;
                if (dict_GuideTreesCondition.TryGetValue(Id, out guideTreeCondition))
                {
                    isValue = guideTreeCondition.CheckCondition(); //检测引导树条件

                    if (isValue && guideTreeCondition.targetGuideId != 0) //跳转引导ID
                    {
                        nextId = (int)guideTreeCondition.targetGuideId;//跳转引导
                        return nextId;
                    }
                }

                //是否存在引导树
                GuideTree<GuideTask> guideTree = null;
                if (dict_GuideTrees.TryGetValue(Id, out guideTree))
                {
                    if (!guideTree.IsLeaf)
                    {
                        if (isValue)//为真时优先取左树
                        {
                            if (null != guideTree.LeftTree)
                            {
                                nextId = (int)guideTree.LeftTree.Data.Id;
                            }
                            else
                            {
                                nextId = (int)guideTree.RightTree.Data.Id;
                            }
                        }
                        else//为假时优先取右树
                        {
                            if (null != guideTree.RightTree)
                            {
                                nextId = (int)guideTree.RightTree.Data.Id;
                            }
                            else
                            {
                                nextId = (int)guideTree.LeftTree.Data.Id;
                            }
                        }
                    }
                }
                return nextId;
            }
            /// <summary>
            /// 尝试保存数据
            /// </summary>
            public void TrySaveData(GuideTask guideTask)
            {
                //未保存数据
                if (isSaveData) return;

                //当前引导完成后需要上传保存引导组
                if (cSVGuideGroupData.file == guideTask.Id)
                {
                    Sys_Guide.Instance.GuideCompleteReq(Id);
                }
            }
            #endregion
            #region 功能处理
            /// <summary>
            /// 设置引导组状态以及监听
            /// </summary>
            /// <param name="state"></param>
            public void SetGuideGroupStateAndListen(EGuideState state)
            {
                SetGuideGroupState(state);
                switch (guideGroupState)
                {
                    case EGuideState.UnActived:
                        {
                            SetListen(true, true);
                        }
                        break;
                    case EGuideState.Completed:
                        {
                            SetListen(false, false);
                        }
                        break;
                }
            }
            /// <summary>
            /// 设置引导组状态
            /// </summary>
            /// <param name="guideState"></param>
            private void SetGuideGroupState(EGuideState guideGroupState)
            {
                this.guideGroupState = guideGroupState;
            }
            /// <summary>
            /// 设置引导组监听事件
            /// </summary>
            /// <param name="isSet"></param>
            /// <param name="isTrigger"></param>
            private void SetListen(bool isSet, bool isTrigger = false)
            {
                if (isTrigger)
                {
                    triggerGroup.PreCheck();
                    OnTriggerGroupChanged(triggerGroup.trueCount);
                }
                if (isSet)
                {
                    triggerGroup.onTrueCountChanged = OnTriggerGroupChanged;
                    triggerGroup.TryListen(true);
                }
                else
                {
                    triggerGroup.onTrueCountChanged = null;
                    triggerGroup.TryListen(false);
                    StopCurGuide();
                }
            }
            /// <summary>
            /// 设置引导监听
            /// </summary>
            /// <param name="guideTask"></param>
            /// <param name="isSet"></param>
            private void SetGuideListen(GuideTask guideTask, bool isSet)
            {
                if (null == guideTask) return;

                if (isSet)
                {
                    guideTask.SetAciton(() => {
                        SkipGuide(guideTask);
                    },  () => { CompletedGuide(guideTask); });
                    guideTask.SetListen(true, true);
                }
                else
                {
                    guideTask.SetAciton(null, null);
                    guideTask.SetListen(false, false);
                }
            }
            /// <summary>
            /// 重置引导组
            /// </summary>
            public void Reset()
            {
                isSaveData = false;
                ResetState();
            }
            /// <summary>
            /// 重置状态
            /// </summary>
            public void ResetState()
            {
                SetGuideGroupState(EGuideState.UnActived);
                SetListen(false, false);
                foreach (var item in dict_GuideTrees.Values)
                {
                    item.Data?.Reset();
                }
                curGuideId = rootGuideId;
            }
            #endregion
        }
        /// <summary> 引导树 </summary>
        public class GuideTree<T> where T : class
        {
            #region 数据定义
            /// <summary> 数据 </summary>
            private T data;
            public T Data
            {
                get
                {
                    return data;
                }
                set
                {
                    data = value;
                }
            }
            /// <summary> 左节点 </summary>
            private GuideTree<T> leftTree;
            public GuideTree<T> LeftTree
            {
                get
                {
                    return leftTree;
                }
                set
                {
                    leftTree = value;
                }
            }
            /// <summary> 右节点 </summary>
            private GuideTree<T> rightTree;
            public GuideTree<T> RightTree
            {
                get
                {
                    return rightTree;
                }
                set
                {
                    rightTree = value;
                }
            }
            /// <summary> 是否是根目录(首节点) </summary>
            public bool IsRoot { set; get; } = true;
            /// <summary> 是否是叶目录(尾节点) </summary>
            public bool IsLeaf { get { return null == LeftTree && null == RightTree; } }
            #endregion
            #region 初始化
            /// <summary>
            /// 构建函数
            /// </summary>
            /// <param name="data"></param>
            public GuideTree(T data)
            {
                this.data = data;
                this.leftTree = null;
                this.rightTree = null;
            }
            #endregion
            #region 功能处理
            /// <summary>
            /// 返回当前树中所有节点
            /// </summary>
            /// <returns></returns>
            public List<GuideTree<T>> WideOrderTree()
            {
                List<GuideTree<T>> returnList = new List<GuideTree<T>>();
                List<GuideTree<T>> nodeList = new List<GuideTree<T>>();
                nodeList.Add(this);
                GuideTree<T> temp = null;
                while (nodeList.Count > 0)
                {
                    temp = nodeList[0];
                    returnList.Add(temp);
                    nodeList.RemoveAt(0);
                    if (temp.LeftTree != null)
                    {
                        nodeList.Add(temp.LeftTree);
                    }
                    if (temp.RightTree != null)
                    {
                        nodeList.Add(temp.RightTree);
                    }
                }
                return returnList;
            }
            #endregion
        }
        /// <summary> 引导数条件 </summary>
        public class GuideTreeCondition
        {
            #region 数据定义
            /// <summary> 分支条件 </summary>
            public CSVGuideCondition.Data cSVGuideConditionData { get; set; } = null;
            public int targetGuideId { get; set; } = 0;
            #endregion
            #region 初始化
            /// <summary>
            /// 构建函数
            /// </summary>
            /// <param name="id"></param>
            /// <param name="targetGuideId"></param>
            public GuideTreeCondition(uint id, int targetGuideId)
            {
                this.cSVGuideConditionData = CSVGuideCondition.Instance.GetConfData(id);
                this.targetGuideId = targetGuideId;
            }
            #endregion
            #region 功能处理
            /// <summary>
            /// 检测条件
            /// </summary>
            /// <returns></returns>
            public bool CheckCondition()
            {
                if (null == cSVGuideConditionData) return false;
                switch (cSVGuideConditionData.type)
                {
                    case 1://穿戴装备类型
                        {
                            uint WeaponId = Sys_Equip.Instance.GetCurWeapon();
                            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(WeaponId);
                            if (null == cSVEquipmentData) return false;
                            uint equipType = 0;
                            uint.TryParse(cSVGuideConditionData.prarameter, out equipType);
                            return cSVEquipmentData.equipment_type == equipType;
                        }
                    case 2://boss阶段
                        {
                            uint stage = 0;
                            uint.TryParse(cSVGuideConditionData.prarameter, out stage);
                            return Net_Combat.Instance.m_CurClientBattleStage == stage;
                        }
                    case 3://UI目标
                        {
                            string[] str = cSVGuideConditionData.prarameter.Split('|');
                            string check_path = str[0];
                            uint check_state = 0;
                            uint.TryParse(str[1], out check_state);
                            Transform tr = Sys_Guide.Instance.FindGameObject(2, check_path);
                            if (null == tr) return false;
                            return tr.gameObject.activeSelf == Convert.ToBoolean(check_state);
                        }
                    case 4://未穿戴装备类型
                        {
                            uint WeaponId = Sys_Equip.Instance.GetCurWeapon();
                            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(WeaponId);
                            if (null == cSVEquipmentData) return false;
                            uint equipType = 0;
                            uint.TryParse(cSVGuideConditionData.prarameter, out equipType);
                            return cSVEquipmentData.equipment_type != equipType;
                        }
                    case 5://未出战宠物
                        {
                            bool isCarryingPet = !Sys_Pet.Instance.fightPet.HasFightPet();
                            uint value = 0;
                            uint.TryParse(cSVGuideConditionData.prarameter, out value);
                            bool isTrue = System.Convert.ToBoolean(value);
                            return isCarryingPet == isTrue;
                        }
                }
                return true;
            }
            #endregion
        }
        /// <summary> 引导任务 </summary>
        public class GuideTask
        {
            #region 静态数据
            /// <summary> 等待执行的强制引导(执行先后顺序) </summary>
            public static List<GuideTask> list_ForceGuide = new List<GuideTask>();
            /// <summary> 等待执行的非强制引导(执行无序) </summary>
            public static List<GuideTask> list_UnForceGuide = new List<GuideTask>();
            /// <summary>
            /// 插序引导
            /// </summary>
            /// <param name="guideTask"></param>
            public static void InsertGuideList(GuideTask guideTask)
            {
                EGuideMode eGuideMode = (EGuideMode)guideTask.cSVGuideData.force;
                switch (eGuideMode)
                {
                    case EGuideMode.Force:
                    case EGuideMode.CheckForce:
                        {
                            int index = list_ForceGuide.IndexOf(guideTask);
                            if (index < 0)
                            {
                                list_ForceGuide.Insert(0, guideTask);
                            }
                            else if (index > 0)
                            {
                                list_ForceGuide.ForEach(x => x.SetGuideState(EGuideState.Waiting));
                                list_ForceGuide.RemoveAt(index);
                                list_ForceGuide.Insert(0, guideTask);
                            }
                        }
                        break;
                }
            }
            /// <summary>
            /// 添加引导
            /// </summary>
            /// <param name="guideTask"></param>
            public static void AddGuideList(GuideTask guideTask)
            {
                EGuideMode eGuideMode = (EGuideMode)guideTask.cSVGuideData.force;
                switch (eGuideMode)
                {
                    case EGuideMode.UnForce:
                        {
                            if (!list_UnForceGuide.Contains(guideTask))
                            {
                                list_UnForceGuide.Add(guideTask);
                            }
                        }
                        break;
                    case EGuideMode.Force:
                    case EGuideMode.CheckForce:
                        {
                            if (!list_ForceGuide.Contains(guideTask))
                            {
                                list_ForceGuide.Add(guideTask);
                            }
                        }
                        break;
                }
            }
            /// <summary>
            /// 移除引导
            /// </summary>
            /// <param name="guideTask"></param>
            public static void RemoveGuideList(GuideTask guideTask)
            {
                EGuideMode eGuideMode = (EGuideMode)guideTask.cSVGuideData.force;
                switch (eGuideMode)
                {
                    case EGuideMode.UnForce:
                        {
                            if (list_UnForceGuide.Contains(guideTask))
                            {
                                list_UnForceGuide.Remove(guideTask);
                            }
                        }
                        break;
                    case EGuideMode.Force:
                    case EGuideMode.CheckForce:
                        {
                            if (list_ForceGuide.Contains(guideTask))
                            {
                                list_ForceGuide.Remove(guideTask);
                            }
                        }
                        break;
                }
            }
            /// <summary>
            /// 清理引导列表
            /// </summary>
            public static void ClearGuideList()
            {
                list_ForceGuide.Clear();
                list_UnForceGuide.Clear();
            }
            #endregion
            #region 数据定义
            /// <summary> 编号 </summary>
            public uint Id { get; private set; } = 0;
            /// <summary> 所属组编号 </summary>
            public uint GroupId { get; private set; } = 0;
            /// <summary> 是否自动显示跳过 </summary>
            public bool IsAutoSkipBtn
            {
                get
                {
                    if (null == cSVGuideData) return false;

                    return cSVGuideData.auto_time > 0;
                }
            }
            /// <summary> 引导表 </summary>
            public CSVGuide.Data cSVGuideData { get; private set; } = null;
            /// <summary> 引导当前状态 </summary>
            public EGuideState guideState { get; private set; } = EGuideState.UnActived;
            /// <summary> 触发条件组 </summary>
            public TriggerSlotGroup triggerGroup = new TriggerSlotGroup();
            /// <summary> 跳至引导链结束回调 </summary>
            private Action action_Skip;
            /// <summary> 完成回调 </summary>
            private Action action_Completed;
            #endregion
            #region 初始化
            /// <summary>
            /// 构建函数
            /// </summary>
            /// <param name="Id"></param>
            /// <param name="GroupId"></param>
            public GuideTask(uint Id, uint GroupId)
            {
                this.Id = Id;
                this.GroupId = GroupId;
                this.cSVGuideData = CSVGuide.Instance.GetConfData(Id);
                if (null != cSVGuideData)
                    SetTriggerConditionGroup();
                else
                    DebugUtil.LogError("引导表未读取到相关配置,引导ID:" + Id);
            }
            /// <summary>
            /// 设置触发条件组
            /// </summary>
            private void SetTriggerConditionGroup()
            {
                List<TriggerSlot> triggerConditions = new List<TriggerSlot>();
                if (cSVGuideData.UI_id != 0)
                {
                    switch ((EGuideMode)cSVGuideData.force)
                    {
                        case EGuideMode.UnForce:
                            {
                                triggerConditions.Add(new TriggerCondition_ShowView(cSVGuideData.UI_id));
                            }
                            break;
                        case EGuideMode.Force:
                        case EGuideMode.CheckForce:
                            {
                                triggerConditions.Add(new TriggerCondition_OpenView(cSVGuideData.UI_id));
                            }
                            break;
                    }
                }
                triggerGroup.SetTriggerList(triggerConditions);
            }
            /// <summary>
            /// 设置引导状态
            /// </summary>
            /// <param name="guideState"></param>
            public void SetGuideState(EGuideState guideState)
            {
                this.guideState = guideState;
            }
            /// <summary>
            /// 设置监听事件
            /// </summary>
            /// <param name="isSet"></param>
            /// <param name="isTrigger"></param>
            public void SetListen(bool isSet, bool isTrigger)
            {
                if (isTrigger)
                {
                    triggerGroup.PreCheck();
                    OnTrueCountChanged(triggerGroup.trueCount);
                }
                if (isSet)
                {
                    triggerGroup.onTrueCountChanged = OnTrueCountChanged;
                    triggerGroup.TryListen(true);
                }
                else
                {
                    triggerGroup.onTrueCountChanged = null;
                    triggerGroup.TryListen(false);
                    GuideTask.RemoveGuideList(this);
                }
            }
            /// <summary>
            /// 设置回调
            /// </summary>
            /// <param name="action_Skip"></param>
            /// <param name="action_Completed"></param>
            public void SetAciton(Action action_Skip = null, Action action_Completed = null)
            {
                this.action_Skip = action_Skip;
                this.action_Completed = action_Completed;
            }
            #endregion
            #region 数据处理
            /// <summary>
            /// 触发器变化
            /// </summary>
            /// <param name="count"></param>
            public void OnTrueCountChanged(int count)
            {
                if (triggerGroup.isFinish)
                {
                    WaitingGuide();
                }
                else
                {
                    StopGuide();
                }
            }
            /// <summary>
            /// 等待引导
            /// </summary>
            public void WaitingGuide()
            {
                SetGuideState(EGuideState.Waiting);
                GuideTask.AddGuideList(this);
            }
            /// <summary>
            /// 停止引导
            /// </summary>
            public void StopGuide()
            {
                SetGuideState(EGuideState.UnActived);
                GuideTask.RemoveGuideList(this);
            }
            /// <summary>
            /// 插入引导
            /// </summary>
            public void InsertGuide()
            {
                GuideTask.InsertGuideList(this);
            }
            /// <summary>
            /// 尝试运行引导
            /// </summary>
            public void OnTryRunningGuide()
            {
                RunningGuide();
            }
            /// <summary>
            /// 执行引导
            /// </summary>
            public void RunningGuide()
            {
                ExecutingGuide();
            }
            /// <summary>
            /// 运行引导
            /// </summary>
            public void ExecutingGuide()
            {
                SetGuideState(EGuideState.Executing);
                Sys_Guide.Instance.eventEmitter.Trigger<GuideTask>(EEvents.TriggerGuide, this);
                EGuideMode eGuideMode = (EGuideMode)cSVGuideData.force;
                switch (eGuideMode)
                {
                    case EGuideMode.UnForce://非强制引导
                        {
                            Sys_Guide.Instance.eventEmitter.Trigger<GuideTask>(EEvents.AddUnForceGuide, this);
                        }
                        break;
                    case EGuideMode.Force://强制引导
                        {
                            //Action action = () =>
                            //{
                                //Debug.LogError("执行强制引导组=" + this.GroupId + " 分支引导Id =" + this.Id) ;

                                //处理功能
                                UIManager.OpenUI(EUIID.UI_ForceGuide, false, this);
                                //关闭界面
                                if (UIManager.IsOpen(EUIID.UI_Chat))
                                {
                                    UIManager.CloseUI(EUIID.UI_Chat);
                                }
                            //};
                            //action?.Invoke();
                            ////等待界面功能处理后开始处理引导界面
                            //if (!OnViewFunction(action))
                            //{
                            //    action.Invoke(); //界面功能无回调时直接执行引导界面
                            //}
                        }
                        break;
                    case EGuideMode.CheckForce://检测引导
                        {
                            CompletedGuide();
                        }
                        break;
                }
            }
            /// <summary>
            /// 跳至引导链结束
            /// </summary>
            public void SkipGuide()
            {
                Sys_Guide.Instance.HitPointGuide(this.Id, this.cSVGuideData.force, true);
                SetGuideState(EGuideState.Completed);
                GuideTask.RemoveGuideList(this);
                action_Skip?.Invoke();
            }
            /// <summary>
            /// 完成引导
            /// </summary>
            public void CompletedGuide()
            {
                if (this.guideState == EGuideState.Completed)
                    return;

                //Debug.LogError("完成强制引导组=" + this.GroupId + " 分支引导Id =" + this.Id);

                Sys_Guide.Instance.HitPointGuide(this.Id, this.cSVGuideData.force, false);
                SetGuideState(EGuideState.Completed);
                GuideTask.RemoveGuideList(this);
                EGuideMode eGuideMode = (EGuideMode)cSVGuideData.force;
                switch (eGuideMode)
                {
                    case EGuideMode.UnForce: //非强制引导
                        {
                            Sys_Guide.Instance.eventEmitter.Trigger<GuideTask>(EEvents.RemoveUnForceGuide, this);
                        }
                        break;
                    case EGuideMode.Force: //强制引导
                        {
                            UIManager.CloseUI(EUIID.UI_ForceGuide, false, false);
                        }
                        break;
                }
                Sys_Guide.Instance.eventEmitter.Trigger<GuideTask>(EEvents.CompletedGuide, this);
                action_Completed?.Invoke();
            }
            #endregion
            #region 功能处理
            /// <summary>
            /// 界面功能处理
            /// </summary>
            public bool OnViewFunction(Action action)
            {
                /*
                 * 目前存在风险,有可能的问题:
                 * 如果当前action如果不被执行,当前引导将处于等待进行状态。。导致后续引导等待状态。
                 * 看来就是后续引导不被触发,后续还是修改成调用方法，然后直接执行引导。
                 */
                if (null == cSVGuideData.Location) return false;
                uint id = cSVGuideData.Location.Count >= 2 ? cSVGuideData.Location[0] : 0;
                uint parameter = cSVGuideData.Location.Count >= 2 ? cSVGuideData.Location[1] : 0;
                switch ((EGuideViewFunctionType)id)
                {
                    case EGuideViewFunctionType.LiveSkill:
                        {
                            Sys_LivingSkill.Instance.SkipToItem(parameter, action); //临时处理
                        }
                        return true;
                }
                return false;
            }
            /// <summary>
            /// 重置引导
            /// </summary>
            public void Reset()
            {
                SetGuideState(EGuideState.UnActived);
                SetListen(false, false);
                SetAciton(null, null);
                GuideTask.RemoveGuideList(this);
            }

           
            #endregion
        }
        /// <summary> 操作列表 </summary>
        public class OperationsList
        {
            #region 数据定义
            /// <summary> 操作 </summary>
            public class Operation
            {
                public EGuideState eGuideState; //状态
                public EGuidePhase eGuidePhase; //阶段
                public List<string> Parameters = new List<string>(); //参数
                public bool isSkip { get; set; } //是否可跳过
                /// <summary>
                /// 构建函数
                /// </summary>
                /// <param name="parameters"></param>
                public Operation(string[] parameters)
                {
                    this.eGuideState = EGuideState.UnActived;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (i == 0) eGuidePhase = (EGuidePhase)System.Convert.ToUInt32(parameters[i]);
                        else Parameters.Add(parameters[i]);
                    }
                    InitSkip();
                }
                /// <summary>
                /// 构建函数
                /// </summary>
                /// <param name="eGuidePhase"></param>
                /// <param name="Parameters"></param>
                public Operation(EGuidePhase eGuidePhase, List<string> Parameters)
                {
                    this.eGuideState = EGuideState.UnActived;
                    this.eGuidePhase = eGuidePhase;
                    Parameters?.ForEach((x) => { this.Parameters.Add(x); });
                    InitSkip();
                }
                /// <summary>
                /// 初始化跳过设置
                /// </summary>
                private void InitSkip()
                {
                    switch (eGuidePhase)
                    {
                        case EGuidePhase.WaitTime:
                            isSkip = false;
                            break;
                        default:
                            isSkip = true;
                            break;
                    }
                }
            }
            /// <summary> 所属引导ID </summary>
            public uint guideId
            {
                get;
                private set;
            }
            /// <summary> 当前操作下标 </summary>
            public int index
            {
                get;
                private set;
            }
            /// <summary> 是否创建成功 </summary>
            public bool IsCreateSuccess
            {
                get;
                private set;
            }
            /// <summary> 目标列表 </summary>
            public List<Transform> list_Target = new List<Transform>();
            /// <summary> 需要操作列表 </summary>
            public List<Operation> list_Need = new List<Operation>();
            /// <summary> 取消操作列表 </summary>
            public List<Operation> list_Cancel = new List<Operation>();
            /// <summary> 处理操作行为 </summary>
            public Action<Operation> action = null;
            #endregion
            #region 初始化
            /// <summary>
            /// 构建函数
            /// </summary>
            /// <param name="guideTask"></param>
            public OperationsList(Sys_Guide.GuideTask guideTask)
            {
                if (null == guideTask || null == guideTask.cSVGuideData)
                {
                    return;
                }
                //设置引导ID
                this.guideId = guideTask.Id;
                //目标列表
                var list = Sys_Guide.Instance.FindGameObject(guideTask.cSVGuideData.prefab_type, guideTask.cSVGuideData.prefab_path);
                foreach (var item in list)
                {
                    list_Target.Add(item);
                }
                //操作列表
                string[] operations = guideTask.cSVGuideData.effect.Split('|');
                for (int i = 0; i < operations.Length; i++)
                {
                    string[] operation = operations[i].Split('&');
                    if (CheckParameters(operation))
                    {
                        Operation item = new Operation(operation);
                        SetNeedOperation(item);
                        SetCancelOperation(item);
                    }
                }

                if (list_Target.Count <= 0)
                {
                    DebugUtil.Log(ELogType.eGuide, "引导缺少目标导致无法继续，请查找原因。");
                    Clear();
                }
                else if (!CheckMustOperation())
                {
                    DebugUtil.Log(ELogType.eGuide, string.Format("引导设置缺少必要的操作类型：{0}", Sys_Guide.EGuidePhase.SetCompletedOption.ToString()));
                    Clear();
                }
                else
                {
                    IsCreateSuccess = true;
                }
            }
            #endregion
            #region 数据处理
            /// <summary>
            /// 设置操作行为
            /// </summary>
            /// <param name="action"></param>
            public void SetAction(Action<Operation> action)
            {
                this.action = action;
            }
            /// <summary>
            /// 检测参数
            /// </summary>
            /// <param name="parameters"></param>
            private bool CheckParameters(string[] parameters)
            {
                if (parameters.Length == 0)
                {
                    DebugUtil.LogError(string.Format("引导ID:{0}-配置错误!", guideId));
                    return false;
                }
                Sys_Guide.EGuidePhase eGuidePhase = (Sys_Guide.EGuidePhase)System.Convert.ToUInt32(parameters[0]);

                switch (eGuidePhase)
                {
                    case Sys_Guide.EGuidePhase.OpenDialog:
                    case Sys_Guide.EGuidePhase.OpenEffect:
                    case Sys_Guide.EGuidePhase.CloseEffect:
                    case Sys_Guide.EGuidePhase.OpenGuideIcon:
                    case Sys_Guide.EGuidePhase.OpenView:
                        {
                            if (parameters.Length < 2)
                            {
                                DebugUtil.LogError(string.Format("引导ID:{0}-配置错误! 阶段:{1}缺少参数", guideId, eGuidePhase.ToString()));
                                return false;
                            }
                            uint Id = 0;
                            if (!uint.TryParse(parameters[1], out Id))
                            {
                                DebugUtil.LogError(string.Format("引导ID:{0}-配置错误! 阶段:{1}参数类型错误", guideId, eGuidePhase.ToString()));
                                return false;
                            }
                        }
                        break;
                    case Sys_Guide.EGuidePhase.WaitTime:
                        {
                            if (parameters.Length < 2)
                            {
                                DebugUtil.LogError(string.Format("引导ID:{0}-配置错误! 阶段:{1}缺少参数", guideId, eGuidePhase.ToString()));
                                return false;
                            }
                            float Time = 0;
                            if (!float.TryParse(parameters[1], out Time))
                            {
                                DebugUtil.LogError(string.Format("引导ID:{0}-配置错误! 阶段:{1}参数类型错误", guideId, eGuidePhase.ToString()));
                                return false;
                            }
                        }
                        break;
                }
                return true;
            }
            /// <summary>
            /// 设置需要操作
            /// </summary>
            /// <param name="operation"></param>
            private void SetNeedOperation(Operation operation)
            {
                if (operation.eGuidePhase == EGuidePhase.SetCompletedOption) //补充参数
                {
                    CSVGuide.Data cSVGuideData = CSVGuide.Instance.GetConfData(guideId);
                    operation.Parameters.Add(cSVGuideData.prefab_range?.Count == 2 ? cSVGuideData.prefab_range[0].ToString() : "0");
                    operation.Parameters.Add(cSVGuideData.prefab_range?.Count == 2 ? cSVGuideData.prefab_range[1].ToString() : "0");
                }
                list_Need.Add(operation);
            }
            /// <summary>
            /// 设置取消操作
            /// </summary>
            /// <param name="operation"></param>
            private void SetCancelOperation(Operation operation)
            {
                switch (operation.eGuidePhase)
                {
                    case EGuidePhase.OpenBackgroundMask:
                        {
                            if (!list_Cancel.Exists(x => x.eGuidePhase == EGuidePhase.CloseBackgroundMask)) //列表只存在一个
                            {
                                list_Cancel.Add(new Operation(EGuidePhase.CloseBackgroundMask, null));
                            }
                        }
                        break;
                    case EGuidePhase.OpenDialog:
                        {
                            if (!list_Cancel.Exists(x => x.eGuidePhase == EGuidePhase.CloseDialog)) //列表只存在一个
                            {
                                list_Cancel.Add(new Operation(EGuidePhase.CloseDialog, null));
                            }
                        }
                        break;
                    case EGuidePhase.OpenGuideIcon:
                        {
                            if (!list_Cancel.Exists(x => x.eGuidePhase == EGuidePhase.CloseGuideIcon)) //列表只存在一个
                            {
                                list_Cancel.Add(new Operation(EGuidePhase.CloseGuideIcon, null));
                            }
                        }
                        break;
                    case EGuidePhase.OpenEffect:
                        {
                            list_Cancel.Add(new Operation(EGuidePhase.CloseEffect, operation.Parameters)); //列表可能存在多个
                        }
                        break;
                    case EGuidePhase.SetCompletedOption:
                        {
                            list_Cancel.Add(new Operation(EGuidePhase.CannelCompletedOption, operation.Parameters));
                        }
                        break;
                }
            }
            /// <summary>
            /// 执行操作
            /// </summary>
            public void Exectue()
            {
                if (null == action) return;
                list_Need.ForEach(x => { action(x); });
            }
            /// <summary>
            /// 取消所有操作
            /// </summary>
            public void CancelAllOperations()
            {
                if (null == action) return;
                list_Cancel.ForEach(x => { action(x); });
            }
            /// <summary>
            /// 清理
            /// </summary>
            public void Clear()
            {
                guideId = 0;
                index = 0;
                list_Target.Clear();
                list_Need.Clear();
                list_Cancel.Clear();
                action = null;
                IsCreateSuccess = false;
            }
            /// <summary>
            /// 是否存在必要的操作
            /// </summary>
            /// <returns></returns>
            public bool CheckMustOperation()
            {
                return null != list_Need.Find(x => x.eGuidePhase == EGuidePhase.SetCompletedOption);
            }
            #endregion
        }
        /// <summary> 等待动画 </summary>
        public class WaitAnimator
        {
            #region 数据定义
            /// <summary> 等待动画 </summary>
            private Animator animator = null;
            /// <summary>  等待动画剪辑名 </summary>
            private string animatorClipName = string.Empty;
            #endregion
            #region 数据处理
            /// <summary>
            /// 设置目标
            /// </summary>
            /// <param name="cSVGuideData"></param>
            public void SetTarget(CSVGuide.Data cSVGuideData)
            {
                Transform tr_animator = Sys_Guide.Instance.FindGameObject(2, cSVGuideData.Animation_path);
                if (null != tr_animator)
                {
                    this.animatorClipName = cSVGuideData.Motion;
                    this.animator = tr_animator.GetComponent<Animator>();
                }
            }
            /// <summary>
            /// 是否完成动画
            /// </summary>
            /// <returns></returns>
            public bool IsCompleteAnimator()
            {
                if (null == animator || animator.enabled == false) return true;
                if (!animator.gameObject.activeInHierarchy) return false;
                AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (!animatorStateInfo.IsName(animatorClipName)) return true;
                return animatorStateInfo.normalizedTime >= 0.95f;
            }
            #endregion
        }

        /// <summary> 开关 </summary>
        public bool isUseGuide = false;
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        /// <summary> 完整引导组列表 </summary>
        public Dictionary<uint, GuideGroup> dict_GuideGroups = new Dictionary<uint, GuideGroup>();
        /// <summary> 数据加载完成数量 </summary>
        private int SyncFinishedCount = 0;
        /// <summary>  更新计时器 </summary>
        private Timer updateTimer;
        /// <summary>  更新事件 </summary>
        public static Action UpdateAction;
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
            ResetData();
            OpenUnForceGuideView();
        }
        public override void OnLogout()
        {
            ResetData();
            CloseUnForceGuideView();
            CloseForceGuideView();
        }
        public override void OnSyncFinished()
        {
            SyncFinishedCount++;
            if (SyncFinishedCount == 2) //一般数据和任务数据加载完成后开始处理
            {
                QueryAllCompleteGuideReq();
            }
        }
        public void OnUpdateData()
        {
            if (!Sys_Guide.Instance.isUseGuide) return; //系统不允许引导
            if (Sys_FunctionOpen.Instance.isRunning) return;

            for (int i = 0; i < GuideTask.list_UnForceGuide.Count; ++i)
            {
                GuideTask guide = GuideTask.list_UnForceGuide[i];
                if (guide != null && guide.guideState == EGuideState.Waiting)
                    guide.OnTryRunningGuide();
            }

            if (GuideTask.list_ForceGuide.Count > 0)
            {
                GuideTask guide = GuideTask.list_ForceGuide[0];
                if (guide.guideState == EGuideState.Waiting)
                    guide.OnTryRunningGuide();
            }
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            //是否打开新手引导
            CSVParam.Data csv = CSVParam.Instance.GetConfData(410);
            isUseGuide = csv == null ? false : Convert.ToBoolean(Convert.ToInt32(csv.str_value));

            //将引导组配置数据创建引导组列表
            dict_GuideGroups.Clear();
            var data = CSVGuideGroup.Instance.GetAll();

            //非Linq排序
            Dictionary<uint, uint> dict_sort = new Dictionary<uint, uint>();
            foreach (var item in data)
            {
                dict_sort.Add(item.order, item.id);
            }
            //排序ID排序
            List<uint> list = new List<uint>(dict_sort.Keys);
            list.Sort();
            //根据排序顺序创建引导组
            foreach (var key in list)
            {
                uint guideGroupsId = dict_sort[key];
                dict_GuideGroups.Add(guideGroupsId, new GuideGroup(guideGroupsId));
            }
            //updateTimer = Timer.Register(0.01f, null, (float value) =>
            //{
            //    OnUpdateData();
            //    UpdateAction?.Invoke();
            //}, true);

            updateTimer = Timer.Register(0f, ()=> {

                OnUpdateData();
                //UpdateAction?.Invoke();
            }, null, true);
        }
        /// <summary>
        /// 重置数据
        /// </summary>
        public void ResetData()
        {
            SyncFinishedCount = 0;
            GuideTask.ClearGuideList();
            foreach (var guideGroup in dict_GuideGroups.Values)
            {
                guideGroup.Reset();
            }
        }
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="toRegister"></param>
        protected void ProcessEvents(bool toRegister)
        {
            Sys_Task.Instance.eventEmitter.Handle(Sys_Task.EEvents.OnFinishedTasksGot, OnFinishedTasksGot, toRegister);
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnRoundNtf, OnRoundNtf, toRegister);

            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener((ushort)CmdMagicDict.GuideCompleteReq, (ushort)CmdMagicDict.GuideCompleteRes, OnGuideCompleteRes, CmdMagicDictGuideCompleteRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdMagicDict.QueryAllCompleteGuideReq, (ushort)CmdMagicDict.QueryAllCompleteGuideRes, OnQueryAllCompleteGuideRes, CmdMagicDictQueryAllCompleteGuideRes.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdMagicDict.GuideCompleteRes, OnGuideCompleteRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdMagicDict.QueryAllCompleteGuideRes, OnQueryAllCompleteGuideRes);
            }
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
        /// 战斗回合开始
        /// </summary>
        public void OnRoundNtf()
        {
            foreach (var item in dict_GuideGroups.Values)
            {
                if (System.Convert.ToBoolean(item.cSVGuideGroupData.mode) || !System.Convert.ToBoolean(item.cSVGuideGroupData.repeat)) continue;
                if (item.guideGroupState != EGuideState.Completed) continue;
                item.ResetState();
                item.SetGuideGroupStateAndListen(EGuideState.UnActived);
            }
        }
        #endregion
        #region 服务器发送消息
        /// <summary>
        /// 请求引导完成
        /// </summary>
        /// <param name="id"></param>
        public void GuideCompleteReq(uint id)
        {
            CmdMagicDictGuideCompleteReq req = new CmdMagicDictGuideCompleteReq();
            req.GuideGroupID = id;
            NetClient.Instance.SendMessage((ushort)CmdMagicDict.GuideCompleteReq, req);
        }
        /// <summary>
        /// 请求已完成的引导列表
        /// </summary>
        public void QueryAllCompleteGuideReq()
        {
            CmdMagicDictQueryAllCompleteGuideReq req = new CmdMagicDictQueryAllCompleteGuideReq();
            NetClient.Instance.SendMessage((ushort)CmdMagicDict.QueryAllCompleteGuideReq, req);
        }
        #endregion
        #region 服务器接收消息
        /// <summary>
        /// 引导完成
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuideCompleteRes(NetMsg msg)
        {
            CmdMagicDictGuideCompleteRes res = NetMsgUtil.Deserialize<CmdMagicDictGuideCompleteRes>(CmdMagicDictGuideCompleteRes.Parser, msg);
            GuideGroup guideGroup = null;
            if (dict_GuideGroups.TryGetValue(res.GuideGroupID, out guideGroup))
            {
                guideGroup.isSaveData = true;
            }
        }
        /// <summary>
        /// 引导完成列表
        /// </summary>
        /// <param name="msg"></param>
        private void OnQueryAllCompleteGuideRes(NetMsg msg)
        {
            CmdMagicDictQueryAllCompleteGuideRes res = NetMsgUtil.Deserialize<CmdMagicDictQueryAllCompleteGuideRes>(CmdMagicDictQueryAllCompleteGuideRes.Parser, msg);
            foreach (var item in dict_GuideGroups.Values)
            {
                item.isSaveData = res.GuideGroupID.Contains(item.Id);
                if (item.isSaveData || System.Convert.ToBoolean(item.cSVGuideGroupData.mode))
                {
                    item.SetGuideGroupStateAndListen(EGuideState.Completed);
                }
                else
                {
                    item.SetGuideGroupStateAndListen(EGuideState.UnActived);
                }
            }
        }
        /// <summary>
        /// 设置客户端状态标识
        /// </summary>
        /// <param name="Status"></param>
        public void OnSealStatusChange(bool Status)
        {
            isUseGuide = !Status;
        }
        #endregion
        #region 功能函数
        /// <summary>
        /// 主动触发引导组
        /// </summary>
        /// <param name="guideGroupId"></param>
        /// <param name="isRepeat"></param>
        public void TriggerGuideGroup(uint guideGroupId, bool isRepeat = false)
        {
            GuideGroup guideGroup;
            if (!dict_GuideGroups.TryGetValue(guideGroupId, out guideGroup)) return;
            //if (guideGroup.guideGroupState == EGuideState.Executing) return;

            if (isRepeat) //是否可重复触发
            {
                guideGroup.ResetState();
                guideGroup.SetGuideGroupStateAndListen(EGuideState.UnActived);
            }
            else //if (guideGroup.guideGroupState != EGuideState.Waiting && guideGroup.guideGroupState != EGuideState.Executing)
            {
                guideGroup.ResetState();
                guideGroup.SetGuideGroupStateAndListen(EGuideState.UnActived);
            }
        }
        /// <summary>
        /// 常态开启引导界面
        /// </summary>
        public void OpenUnForceGuideView()
        {
            UIManager.OpenUI(EUIID.UI_UnForceGuide, true);
        }
        /// <summary>
        /// 关闭非强制引导界面
        /// </summary>
        public void CloseUnForceGuideView()
        {
            if (UIManager.IsOpen(EUIID.UI_UnForceGuide))
                UIManager.CloseUI(EUIID.UI_UnForceGuide);
        }
        /// <summary>
        /// 关闭引导界面
        /// </summary>
        public void CloseForceGuideView()
        {
            if (UIManager.IsOpen(EUIID.UI_ForceGuide))
                UIManager.CloseUI(EUIID.UI_ForceGuide, false, false);
        }
        /// <summary>
        /// 寻找游戏目标
        /// </summary>
        /// <param name="type"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public List<Transform> FindGameObject(uint type, List<string> paths)
        {
            List<Transform> list = new List<Transform>();
            foreach (var path in paths)
            {
                Transform tr = FindGameObject(type, path);
                if (null != tr)
                    list.Add(tr);
            }
            return list;
        }
        /// <summary>
        /// 寻找游戏目标
        /// </summary>
        /// <param name="type"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public Transform FindGameObject(uint type, string path)
        {
            Transform tr = null;
            switch (type)
            {
                case 1://目标场景物体
                    {
                        GameObject _target = GameObject.Find(path);
                        if (null == _target)
                        {
                            DebugUtil.LogError(string.Format("未找到路径物体:{0}", path));
                            return tr;
                        }
                        Collider collider = _target.GetComponentInChildren<Collider>(true);
                        if (null == collider)
                        {
                            DebugUtil.LogError(string.Format("未找到物体以及子物体上的碰撞盒:{0}", path));
                            return tr;
                        }
                        tr = collider.transform;
                    }
                    break;
                case 2://目标界面组件
                case 3://引导界面上组件
                    {
                        Transform _target = UIManager.mRoot.Find(path);
                        if (null == _target)
                        {
                            DebugUtil.LogError(string.Format("未找到路径物体:{0}", path));
                            return tr;
                        }
                        tr = _target;
                    }
                    break;
            }
            return tr;
        }
        /// <summary>
        /// 得到引导目标坐标
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public Vector3 GetTargetPos(RectTransform rect, Transform target)
        {
            if (null == rect || null == target) return Vector3.zero;

            Vector3 pos;
            RectTransform rt_Target = target as RectTransform;
            Collider c_target = target.GetComponent<Collider>();

            if (null != rt_Target)
            {
                pos = Sys_Guide.Instance.GetTargetPos(rect, rt_Target.position, true);
            }
            else if (null != c_target)
            {
                pos = Sys_Guide.Instance.GetTargetPos(rect, c_target.bounds.center, false);
            }
            else
            {
                pos = Sys_Guide.Instance.GetTargetPos(rect, target.position, false);
            }
            return pos;
        }
        /// <summary>
        /// 得到引导目标坐标
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="targetPos"></param>
        /// <param name="ViewObjectOrScreenObject"></param>
        /// <returns></returns>
        public Vector3 GetTargetPos(RectTransform rect, Vector3 targetPos, bool ViewObjectOrScreenObject)
        {
            Vector3 pos = Vector3.zero;
            Vector2 screenPoint;
            if (ViewObjectOrScreenObject)
                screenPoint = CameraManager.mUICamera.WorldToScreenPoint(targetPos);
            else
                screenPoint = CameraManager.mCamera.WorldToScreenPoint(targetPos);

            RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPoint, CameraManager.mUICamera, out pos);
            return pos;
        }
        #endregion

        #region 引导打点
        HitPointGuide hitPoint = new HitPointGuide();
        /// <summary>
        /// 引导打点
        /// </summary>
        /// <param name="skip"></param>
        public void HitPointGuide(uint Id, uint force, bool skip)
        {
            hitPoint.AppendBaseData();
            hitPoint.guideId = Id;
            hitPoint.if_force = force;
            hitPoint.end = skip ? 0 : 1;

            hitPoint.last_id = 0;//未使用过 设置为默认值

            HitPointManager.HitPoint(Logic.HitPointGuide.Key, hitPoint);
        }
        #endregion
    }
}