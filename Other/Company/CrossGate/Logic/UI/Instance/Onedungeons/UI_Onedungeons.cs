using DG.Tweening;
using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{
    /// <summary> 单人副本 </summary>
    public class UI_Onedungeons : UIBase
    {
        #region 数据定义
        /// <summary> 角色动画 </summary>
        public class RoleAnimatorGroup
        {
            /// <summary> 角色方向 </summary>
            public enum RoleDirection
            {
                BottomLeft,  //左下
                TopLeft,     //左上
                BottomRight, //右下
                TopRight,    //右上
            }
            /// <summary> 目标下标 </summary>
            public int targetIndex = 0;
            /// <summary> 移动进度 </summary>
            public float progress = 0;
            /// <summary> 角色 </summary>
            public RectTransform rt_Role;
            /// <summary> 路径点 </summary>
            public Vector3[] arr_PathPos = new Vector3[5];
            /// <summary> 角色动画 </summary>
            public Animator[] arr_RoleAnimator = new Animator[2];
            /// <summary> 阶梯 </summary>
            public Animator[] arr_StepAnimator = new Animator[4];
            /// <summary> 动画序列 </summary>
            public Sequence sequence;
            /// <summary> 角色方向 </summary>
            public Dictionary<bool, RoleDirection[]> roleDirection = new Dictionary<bool, RoleDirection[]>();
            /// <summary> 滚动条 </summary>
            private Scrollbar scrollbar_View;
            /// <summary>
            /// 构建动画
            /// </summary>
            /// <param name="transform"></param>
            public RoleAnimatorGroup(Transform transform)
            {
                rt_Role = transform.Find("Animator/View_Front/Scroll_View/Viewport/Content/View_Dungeons/Role") as RectTransform;

                for (int i = 0, count = arr_RoleAnimator.Length; i < count; i++)
                {
                    arr_RoleAnimator[i] = rt_Role.GetChild(i).GetComponent<Animator>();
                }

                for (int i = 0, count = arr_StepAnimator.Length; i < count; i++)
                {
                    arr_StepAnimator[i] = transform.Find(string.Format("Animator/View_Front/Scroll_View/Viewport/Content/View_Dungeons/View_Steps/Image_Steps{0}", i + 1)).GetComponent<Animator>();
                }
                for (int i = 0, count = arr_PathPos.Length; i < count; i++)
                {
                    RectTransform rect = transform.Find(string.Format("Animator/View_Front/Scroll_View/Viewport/Content/View_Dungeons/Toggle_Pic/toggle{0}/Glass_Go", i + 1)) as RectTransform;
                    arr_PathPos[i] = rt_Role.parent.InverseTransformPoint(rect.transform.position);
                }
                scrollbar_View = transform.Find("Animator/View_Front/Scroll_View/Scrollbar").GetComponent<Scrollbar>();

                roleDirection.Add(true, new RoleDirection[4] { RoleDirection.BottomRight, RoleDirection.BottomLeft, RoleDirection.BottomRight, RoleDirection.BottomLeft });
                roleDirection.Add(false, new RoleDirection[4] { RoleDirection.TopLeft, RoleDirection.TopRight, RoleDirection.TopLeft, RoleDirection.TopRight });
            }
            /// <summary>
            /// 得到相对路径坐标
            /// </summary>
            /// <param name="minIndex"></param>
            /// <param name="maxIndex"></param>
            /// <param name="fdecimal"></param>
            /// <returns></returns>
            public Vector3 GetPathPos(int minIndex, int maxIndex, float fdecimal)
            {
                return Vector3.LerpUnclamped(arr_PathPos[minIndex], arr_PathPos[maxIndex], fdecimal); //实际相对路径中的坐标
            }
            /// <summary>
            /// 同步坐标
            /// </summary>
            public void SynchronizationPosition()
            {
                int minIndex = Mathf.FloorToInt(progress); //向下取整
                int maxIndex = Mathf.CeilToInt(progress);  //向上取整
                float fdecimal = 1f - (maxIndex - progress);      //小数部分
                rt_Role.anchoredPosition = GetPathPos(minIndex, maxIndex, fdecimal);
                scrollbar_View.value = 1f - progress / 4f;
            }
            /// <summary>
            /// 同步角色方向
            /// </summary>
            public void SynchronizationRoleDirection(RoleDirection roleDirection)
            {
                switch (roleDirection)
                {
                    case RoleDirection.BottomLeft://左下
                        {
                            arr_RoleAnimator[0].gameObject.SetActive(true);
                            arr_RoleAnimator[1].gameObject.SetActive(false);
                            rt_Role.transform.localEulerAngles = Vector3.up * 180f;
                        }
                        break;
                    case RoleDirection.TopLeft: //左上
                        {
                            arr_RoleAnimator[0].gameObject.SetActive(false);
                            arr_RoleAnimator[1].gameObject.SetActive(true);
                            rt_Role.transform.localEulerAngles = Vector3.zero;
                        }
                        break;
                    case RoleDirection.BottomRight: //右下
                        {
                            arr_RoleAnimator[0].gameObject.SetActive(true);
                            arr_RoleAnimator[1].gameObject.SetActive(false);
                            rt_Role.transform.localEulerAngles = Vector3.zero;
                        }
                        break;
                    case RoleDirection.TopRight: //右上
                        {
                            arr_RoleAnimator[0].gameObject.SetActive(false);
                            arr_RoleAnimator[1].gameObject.SetActive(true);
                            rt_Role.transform.localEulerAngles = Vector3.up * 180f;
                        }
                        break;
                }
            }
            /// <summary>
            /// 同步角色动画
            /// </summary>
            /// <param name="isRun"></param>
            public void SynchronizationRoleAnimator(bool isRun)
            {
                for (int i = 0, count = arr_RoleAnimator.Length; i < count; i++)
                {
                    if (isRun)
                    {
                        arr_RoleAnimator[i].CrossFade("Run", 0.2f);

                    }
                    else
                    {
                        arr_RoleAnimator[i].CrossFade("Standby", 0.2f);
                    }
                }
            }
            /// <summary>
            /// 同步阶梯动画
            /// </summary>
            /// <param name="targetIndex"> 坐标点下标 </param>
            /// <param name="IsAscendorDescend"> 正序或是倒叙 </param>
            public void SynchronizationStepAnimator(int targetIndex, bool IsAscendorDescend)
            {
                Animator animator = arr_StepAnimator[targetIndex];

                if (IsAscendorDescend)
                {
                    animator.Play("Image_Steps1_Up_Open");
                }
                else
                {
                    animator.Play("Image_Steps1_Down_Open");
                }
            }
            /// <summary>
            /// 得到角色方向
            /// </summary>
            /// <param name="index"></param>
            /// <param name="IsAscendorDescend"></param>
            /// <returns></returns>
            public RoleDirection GetRoleDirection(int index, bool IsAscendorDescend)
            {
                return roleDirection[IsAscendorDescend][index];
            }
            /// <summary>
            /// 得到当前目标路径下标
            /// </summary>
            /// <returns></returns>
            public List<int> GetTargetIndex()
            {
                List<int> list = new List<int>();

                if (targetIndex > progress)
                {
                    int index = Mathf.CeilToInt(progress);
                    if (progress != index)
                        list.Add(index);
                    while (targetIndex > index)
                    {
                        index++;
                        list.Add(index);
                    }
                }
                else if (targetIndex < progress)
                {
                    int index = Mathf.FloorToInt(progress);
                    if (progress != index)
                        list.Add(index);

                    while (targetIndex < index)
                    {
                        index--;
                        list.Add(index);
                    }
                }
                return list;
            }
            /// <summary>
            /// 播放动画
            /// </summary>
            public void Play()
            {
                List<int> list = GetTargetIndex();
                if (list.Count == 0) return;
                bool IsAscendorDescend = list[0] >= progress; //正序或是倒序
                if (null != sequence && sequence.IsPlaying())
                {
                    sequence.Pause();
                    sequence.Kill();
                }
                sequence = DOTween.Sequence();
                for (int i = 0; i < list.Count; i++)
                {
                    int PosIndex = list[i];
                    int StepIndex = IsAscendorDescend ? PosIndex - 1 : PosIndex;

                    float time = list[0] == PosIndex ? 2f * Mathf.Abs(PosIndex - progress) : 2f;
                    Tween tween = DOTween.To(() => progress, x => progress = x, PosIndex, time)
                     .OnStart(() =>
                     {
                         SynchronizationRoleDirection(GetRoleDirection(StepIndex, IsAscendorDescend));
                         SynchronizationRoleAnimator(true);
                         SynchronizationStepAnimator(StepIndex, IsAscendorDescend);
                     })
                    .OnUpdate(() =>
                    {
                        SynchronizationPosition();
                    }).SetEase(Ease.Linear);
                    sequence.Append(tween);
                }
                sequence.OnComplete(() => { SynchronizationRoleAnimator(false); });
                sequence.Play();
            }
        }
        #endregion
        #region 界面组件
        /// <summary> 副本难度下拉菜单 </summary>
        private DropdownEx dropdown_InstanceLevel;
        /// <summary> 中心节点 </summary>
        private RectTransform rt_CenterNode;
        /// <summary> 右边节点 </summary>
        private RectTransform rt_RightNode;
        /// <summary> 关卡背景 </summary>
        private RectTransform rt_StageBg;
        /// <summary> 战斗次数 </summary>
        private Text text_BattlesNumber;
        /// <summary> 历史最佳 </summary>
        private Text text_HistoryBest;
        /// <summary> 今天最佳 </summary>
        private Text text_TodayBest;
        /// <summary> 层级开关组 </summary>
        private ToggleGroup toggleGroup_Layer;
        /// <summary> 层级 </summary>
        private List<Toggle> list_Layer = new List<Toggle>();
        /// <summary> 层级菜单 </summary>
        private List<Toggle> list_LayerTab = new List<Toggle>();
        /// <summary> 层级文字 </summary>
        private List<Text> list_LayerText = new List<Text>();
        /// <summary> 阶梯 </summary>
        private List<RectTransform> list_ladder = new List<RectTransform>();
        /// <summary> 关卡 </summary>
        private List<Toggle> list_Stage = new List<Toggle>();
        /// <summary> 首胜奖励 </summary>
        private Transform tr_FirstRewardItem;
        /// <summary> 首胜奖励列表 </summary>
        private List<PropItem> list_FirstRewardItem = new List<PropItem>();
        /// <summary> 掉落奖励 </summary>
        private Transform tr_DropRewardItem;
        /// <summary> 掉落奖励列表 </summary>
        private List<PropItem> list_DropRewardItem = new List<PropItem>();
        /// <summary> 角色动画类 </summary>
        private RoleAnimatorGroup sp_RoleAnimatorGroup;
        #endregion
        #region 数据
        /// <summary> 单人副本活动ID </summary>
        private uint activityid = 1;
        /// <summary> 动画是否播放 </summary>
        private bool isPlayed = false;
        /// <summary> 是否是第一次打开 </summary>
        private bool isFirstOpen = true;
        /// <summary> 固定层数 </summary>
        private const uint LayerNum = 5;
        /// <summary> 根据选项获取数据 </summary>
        private List<Sys_Instance.InstanceData> list_InstanceDatas = new List<Sys_Instance.InstanceData>();
        private List<uint> list_LayerDatas = new List<uint>();
        private List<Sys_Instance.StageData> list_StageDatas = new List<Sys_Instance.StageData>();
        /// <summary> 下拉菜单下标 </summary>
        private int curDropdownIndex = 0;
        /// <summary> 关卡层菜单下标 </summary>
        private int curLayerIndex = 0;
        /// <summary> 关卡菜单下标 </summary>
        private int curStageIndex = 0;
        /// <summary> 当前副本数据 </summary>
        private Sys_Instance.InstanceData curInstanceData
        {
            get
            {
                return curDropdownIndex < 0 || curDropdownIndex >= list_InstanceDatas.Count ? null : list_InstanceDatas[curDropdownIndex];
            }
        }
        /// <summary> 当前层级数据 </summary>
        private uint curLayerData
        {
            get
            {
                return curLayerIndex < 0 || curLayerIndex >= list_LayerDatas.Count ? 0 : list_LayerDatas[curLayerIndex];
            }
        }
        /// <summary> 当前关卡数据 </summary>
        private Sys_Instance.StageData curStageData
        {
            get
            {
                return curStageIndex < 0 || curStageIndex >= list_StageDatas.Count ? null : list_StageDatas[curStageIndex];
            }
        }
        /// <summary> 服务器数据 </summary>
        private Sys_Instance.ServerInstanceData serverInstanceData { get; set; } = null;
        #endregion
        #region 系统函数        
        protected override void OnLoaded()
        {            
            OnParseComponent();
            ClearItemList();
            CreateItemList();
        }        
        protected override void OnShow()
        {            
            isFirstOpen = true;

            if (Sys_Instance.Instance.IsExpireTime(activityid))
            {
                Sys_Instance.Instance.InstanceDataReq(activityid);
            }
            else
            {
                RefreshView();
            }
        }        
        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent()
        {
            dropdown_InstanceLevel = transform.Find("Animator/View_Front/ContentProto/Dropdown").GetComponent<DropdownEx>();
            rt_CenterNode = transform.Find("Animator/View_Front/Scroll_View/Viewport/Content/View_Dungeons") as RectTransform;
            rt_RightNode = transform.Find("Animator/View_Front/View_Right") as RectTransform;
            rt_StageBg = transform.Find("Animator/View_Front/View_Right/Level_Number/Image_BG4") as RectTransform;
            tr_FirstRewardItem = transform.Find("Animator/View_Front/View_Right/Text_Award/Scroll_View/Viewport/Item");
            tr_DropRewardItem = transform.Find("Animator/View_Front/View_Right/Text_Award (1)/Scroll_View/Viewport/Item");
            tr_FirstRewardItem.gameObject.SetActive(false);
            tr_DropRewardItem.gameObject.SetActive(false);
            text_BattlesNumber = transform.Find("Animator/View_Front/View_Number/Text_Title/Text_Number").GetComponent<Text>();
            text_TodayBest = transform.Find("Animator/View_Front/View_Number/Text_Title1/Text_Number1").GetComponent<Text>();
            text_HistoryBest = transform.Find("Animator/View_Front/View_Number/Text_Title2/Text_Number2").GetComponent<Text>();
            toggleGroup_Layer = transform.Find("Animator/View_Front/Scroll_View/Viewport/Content/View_Dungeons/Toggle_Pic").GetComponent<ToggleGroup>();

            for (int i = 0; i < LayerNum - 1; i++)
            {
                list_ladder.Add(transform.Find(string.Format("Animator/View_Front/Scroll_View/Viewport/Content/View_Dungeons/View_Steps/Image_Steps{0}", i + 1)) as RectTransform);
            }

            for (int i = 0; i < LayerNum; i++)
            {
                list_Layer.Add(transform.Find(string.Format("Animator/View_Front/Scroll_View/Viewport/Content/View_Dungeons/Toggle_Pic/toggle{0}", i + 1)).GetComponent<Toggle>());
                list_LayerTab.Add(transform.Find(string.Format("Animator/View_Front/View_Left/ToggleGroup/Toggle{0}", i + 1)).GetComponent<Toggle>());
                list_LayerText.Add(transform.Find(string.Format("Animator/View_Front/Scroll_View/Viewport/Content/View_Dungeons/Level_State/Image_Level{0}/Text_State", i + 1)).GetComponent<Text>());
                list_Stage.Add(transform.Find(string.Format("Animator/View_Front/View_Right/Level_Number/Level{0}", i + 1)).GetComponent<Toggle>());
            }

            sp_RoleAnimatorGroup = new RoleAnimatorGroup(transform);

            //UI事件
            dropdown_InstanceLevel.onClickItem = OnClick_ResetLayer;

            Lib.Core.EventTrigger.Get(transform.Find("Animator/View_Front/Scroll_View")).AddEventListener(EventTriggerType.PointerClick, OnClick_CancelLayer);

            for (int i = 0, count = list_Layer.Count; i < count; i++)
            {
                Toggle item = list_Layer[i];
                //确保EventTrigger脚本置顶，影响事件执行顺序(点击事件优先，值改变事件滞后)
                Lib.Core.EventTrigger.Get(item.gameObject).onClick = OnClick_ResetStage;

                //Lib.Core.EventTrigger.Get(item.gameObject).AddEventListener(EventTriggerType.PointerClick, OnClick_ResetStage);
                item.onValueChanged.AddListener((bool value) => OnClick_Layer(item, value));
            }

            for (int i = 0, count = list_Stage.Count; i < count; i++)
            {
                var item = list_Stage[i];

                item.onValueChanged.AddListener((bool value) =>
                {
                    if (value) OnClick_Stage(item);
                });
            }

            dropdown_InstanceLevel.onValueChanged.AddListener(delegate { OnClick_InstanceLevel(); });
            transform.Find("Animator/View_Front/View_Title/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/View_Front/Button_Rank").GetComponent<Button>().onClick.AddListener(OnClick_Rank);
            transform.Find("Animator/View_Front/View_Right/Btn_01").GetComponent<Button>().onClick.AddListener(OnClick_Fight);
            transform.Find("Animator/View_Front/View_Right/View_Introduce/Text_Best/Button_Play").GetComponent<Button>().onClick.AddListener(OnClick_CheckVideo1);
            transform.Find("Animator/View_Front/View_Right/View_Introduce/Text_Me/Button_Play").GetComponent<Button>().onClick.AddListener(OnClick_CheckVideo2);
        }
        /// <summary>
        /// 创建模版列表
        /// </summary>
        private void CreateItemList()
        {
            for (int i = 0; i < 5; i++)
            {
                PropItem propItem = new PropItem();
                propItem.BindGameObject(GameObject.Instantiate(tr_FirstRewardItem, tr_FirstRewardItem.parent).gameObject);
                list_FirstRewardItem.Add(propItem);
            }
            for (int i = 0; i < 5; i++)
            {
                PropItem propItem = new PropItem();
                propItem.BindGameObject(GameObject.Instantiate(tr_DropRewardItem, tr_DropRewardItem.parent).gameObject);
                list_DropRewardItem.Add(propItem);
            }
        }
        /// <summary>
        /// 清理模版列表
        /// </summary>
        private void ClearItemList()
        {
            for (int i = 0, count = list_FirstRewardItem.Count; i < count; i++)
            {
                var x = list_FirstRewardItem[i];
                if (x != null && null != x.transform) GameObject.Destroy(x.transform.gameObject);
            }
            list_FirstRewardItem.Clear();
            for (int i = 0, count = list_DropRewardItem.Count; i < count; i++)
            {
                var x = list_DropRewardItem[i];
                if (x != null && null != x.transform) GameObject.Destroy(x.transform.gameObject);
            }
            list_DropRewardItem.Clear();
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.InstanceDataUpdate, OnInstanceDataUpdate, toRegister);
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.DailyInstanceBestInfoRes, OnDailyInstanceBestInfoRes, toRegister);
        }
        /// <summary>
        /// 更新下拉菜单选项
        /// </summary>
        /// <param name="index"></param>
        private void UpdateDropdownOption()
        {
            if (dropdown_InstanceLevel.value != curDropdownIndex)
            {
                dropdown_InstanceLevel.value = curDropdownIndex;
            }
            else
            {
                dropdown_InstanceLevel.onValueChanged.Invoke(curDropdownIndex);
            }
        }
        /// <summary>
        /// 更新关卡层菜单选项
        /// </summary>
        private void UpdateLayerOption()
        {
            Toggle toggle = list_Layer[curLayerIndex];

            if (!toggle.isOn)
            {
                toggle.isOn = true;
                toggle.isOn = false;
            }
            else
            {
                toggle.onValueChanged.Invoke(true);
                toggle.isOn = false;
            }
        }
        /// <summary>
        /// 更新关卡菜单选项
        /// </summary>
        private void UpdateStageOption()
        {
            Toggle toggle = list_Stage[curStageIndex];

            if (!toggle.isOn)
            {
                toggle.isOn = true;
            }
            else
            {
                toggle.onValueChanged.Invoke(true);
            }
        }
        #endregion
        #region 数据设置
        /// <summary>
        /// 重置选项
        /// </summary>
        private void ResetOption()
        {
            curDropdownIndex = DefaultDropdownOption();
            curLayerIndex = DefaultLayer();
            curStageIndex = DefaultStage();
        }
        /// <summary> 
        /// 默认下拉菜单选项
        /// </summary>
        /// <returns></returns>
        private int DefaultDropdownOption()
        {
            Sys_Instance.ServerInstanceData serverInstanceData;
            if (!Sys_Instance.Instance.dict_ServerInstanceData.TryGetValue(activityid, out serverInstanceData))
                return 0;

            List<Packet.InsEntry> list = new List<Packet.InsEntry>(serverInstanceData.instanceCommonData.Entries);

            int index = 0;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Unlock == true)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        /// <summary>
        /// 默认层
        /// </summary>
        /// <returns></returns>
        private int DefaultLayer()
        {
            Packet.InsEntry insEntry = serverInstanceData.GetInsEntry(curInstanceData?.instanceid ?? 0);
            if (null == insEntry)
                return 0;

            CSVInstanceDaily.Data cSVInstanceDailyData = CSVInstanceDaily.Instance.GetConfData(insEntry.PerMaxStageId);
            if (null == cSVInstanceDailyData)
                return 0;

            return (int)cSVInstanceDailyData.LayerStage - 1;
        }
        /// <summary>
        /// 默认关卡
        /// </summary>
        /// <returns></returns>
        private int DefaultStage()
        {
            Packet.InsEntry insEntry = serverInstanceData.GetInsEntry(curInstanceData?.instanceid ?? 0);
            if (null == insEntry)
                return 0;

            CSVInstanceDaily.Data curInstanceDailyData = CSVInstanceDaily.Instance.GetConfData(insEntry.PerMaxStageId);
            if (null == curInstanceDailyData)
                return 0;

            CSVInstanceDaily.Data nextInstanceDailyData = CSVInstanceDaily.Instance.GetConfData(curInstanceDailyData.id + 1);
            return nextInstanceDailyData != null ? (int)nextInstanceDailyData.Layerlevel - 1 : (int)curInstanceDailyData.Layerlevel - 1;
        }
        /// <summary>
        /// 设置下拉菜单数据
        /// </summary>
        /// <param name="index"></param>
        private void SetDropdownData()
        {
            list_InstanceDatas.Clear();
            var datas = Sys_Instance.Instance.GetInstanceData(activityid);
            if (null == datas)
                return;

            for (int i = 0, count = datas.Count; i < count; i++)
            {
                var item = datas[i];
                list_InstanceDatas.Add(item);
            }
        }
        /// <summary>
        /// 设置关卡层数据
        /// </summary>
        private void SetLayerData()
        {
            list_LayerDatas.Clear();
            if (null == curInstanceData) return;

            var dict_Check = curInstanceData.dict_Check;
            List<uint> list_Check = new List<uint>(dict_Check.Keys);

            for (int i = 0, count = list_Check.Count; i < count; i++)
            {
                uint id = list_Check[i];
                list_LayerDatas.Add(id);
            }
        }
        /// <summary>
        /// 设置关卡菜单数据
        /// </summary>
        private void SetStageData()
        {
            list_StageDatas.Clear();
            List<Sys_Instance.StageData> datas = null;
            curInstanceData?.dict_Check.TryGetValue(curLayerData, out datas);
            if (null == datas)
                return;

            for (int i = 0, count = datas.Count; i < count; i++)
            {
                var item = datas[i];
                list_StageDatas.Add(item);
            }
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            if (isFirstOpen)
            {
                isFirstOpen = false;
                isPlayed = false;
                serverInstanceData = Sys_Instance.Instance.GetServerInstanceData(activityid);
                SetDropdownData();
                ResetOption();
            }
            SetDropdownView();
            UpdateDropdownOption();
        }
        /// <summary>
        /// 设置战斗界面
        /// </summary>
        private void SetBattleView()
        {
            if (null == serverInstanceData)
            {
                text_BattlesNumber.text = string.Empty;
                text_HistoryBest.text = string.Empty;
                text_TodayBest.text = string.Empty;
            }
            else
            {
                uint MaxTimes = 10;// Sys_Daily.Instance.GetDailyMaxTimes(activityid);
                uint UsedTimes = (uint)Sys_Instance.Instance.GetDailyTimes(activityid);
                Packet.InsEntry insEntry = serverInstanceData.GetInsEntry(curInstanceData?.instanceid ?? 0);
                CSVInstanceDaily.Data cSVInstanceDailyData1 = CSVInstanceDaily.Instance.GetConfData(insEntry?.PerMaxStageId ?? 0);
                CSVInstanceDaily.Data cSVInstanceDailyData2 = CSVInstanceDaily.Instance.GetConfData(insEntry?.PerMaxStageId ?? 0);
                text_BattlesNumber.text = (MaxTimes - UsedTimes).ToString();
                text_TodayBest.text = string.Format("{0}-{1}", cSVInstanceDailyData1?.LayerStage ?? 1, cSVInstanceDailyData1?.Layerlevel ?? 1);
                text_HistoryBest.text = string.Format("{0}-{1}", cSVInstanceDailyData2?.LayerStage ?? 1, cSVInstanceDailyData2?.Layerlevel ?? 1);
            }
        }
        /// <summary>
        /// 设置下拉界面
        /// </summary>
        private void SetDropdownView()
        {
            SetDropdownMenu();
        }
        /// <summary>
        /// 设置关卡层界面
        /// </summary>
        private void SetLayerView()
        {
            SetLayerData();
            SetLayerMenu();
        }
        /// <summary>
        /// 设置关卡菜单界面
        /// </summary>
        private void SetStageMenuView()
        {
            SetStageData();
            SetStageMenu();
        }
        /// <summary>
        /// 设置下拉菜单
        /// </summary>
        private void SetDropdownMenu()
        {
            List<Sys_Instance.InstanceData> instanceDatas = list_InstanceDatas;
            dropdown_InstanceLevel.ClearOptions();
            List<string> options = new List<string>();

            for (int i = 0, count = instanceDatas.Count; i < count; i++)
            {
                var item = instanceDatas[i];
                options.Add(LanguageHelper.GetTextContent(item.cSVInstanceData.Name));
            }
            dropdown_InstanceLevel.AddOptions(options);
        }
        /// <summary>
        /// 设置关卡层菜单
        /// </summary>
        private void SetLayerMenu()
        {
            Packet.InsEntry insEntry = serverInstanceData?.GetInsEntry(curInstanceData?.instanceid ?? 0);
            bool isUnlockInstance = null != insEntry && insEntry.Unlock;//是否解锁副本
            uint CurPassedStageId = insEntry?.PerMaxStageId ?? 0; //已通关的最高关卡
            CSVInstanceDaily.Data cSVInstanceDailyData = curInstanceData?.GetUnlockStageData(CurPassedStageId)?.cSVInstanceDailyData;        //下一关卡数据
            bool isLastStage = false;//是否通过最后一关

            if (null != curInstanceData)
            {
                int index = curInstanceData.list_StageData.Count - 1;
                isLastStage = curInstanceData.list_StageData[index].stageid == CurPassedStageId;
            }

            List<uint> layerDatas = list_LayerDatas;
            for (int i = 0, count = list_Layer.Count; i < count; i++)
            {
                uint layer = i < layerDatas?.Count ? layerDatas[i] : 0;
                SetLayerItem(list_Layer[i], layer, isUnlockInstance, cSVInstanceDailyData);
                SetLayerTextItem(list_LayerText[i], layer, isLastStage, isUnlockInstance, cSVInstanceDailyData);
            }

            for (int i = 0, count = list_ladder.Count; i < count; i++)
            {
                SetLadderItem(list_ladder[i], i < layerDatas?.Count ? layerDatas[i] : 0, isUnlockInstance, cSVInstanceDailyData);
            }
        }
        /// <summary>
        /// 设置关卡层
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="layerId"></param>
        /// <param name="isUnlockInstance"></param>
        /// <param name="cSVInstanceDailyData"></param>
        private void SetLayerItem(Toggle toggle, uint layerId, bool isUnlockInstance, CSVInstanceDaily.Data cSVInstanceDailyData)
        {
            bool isGray = false;
            if (!isUnlockInstance || null == cSVInstanceDailyData)
            {
                isGray = true;
            }
            else
            {
                isGray = layerId > cSVInstanceDailyData.LayerStage;
            }
            ImageHelper.SetImageGray(toggle, isGray, true);
            toggle.interactable = !isGray;
            toggle.name = string.Format("toggle{0}{1}", layerId.ToString(), isGray ? "_Gray" : string.Empty);
        }
        /// <summary>
        /// 设置层文本
        /// </summary>
        /// <param name="text"></param>
        /// <param name="layerId"></param>
        /// <param name="isUnlockInstance"></param>
        /// <param name="cSVInstanceDailyData"></param>
        private void SetLayerTextItem(Text text, uint layerId, bool lastStage, bool isUnlockInstance, CSVInstanceDaily.Data cSVInstanceDailyData)
        {
            if (!isUnlockInstance || null == cSVInstanceDailyData)
            {
                text.text = LanguageHelper.GetTextContent(1006032);
            }
            else
            {
                if (layerId < cSVInstanceDailyData.LayerStage)
                {
                    text.text = LanguageHelper.GetTextContent(1006033);
                }
                else if (layerId == cSVInstanceDailyData.LayerStage)
                {
                    if (lastStage)
                    {
                        text.text = LanguageHelper.GetTextContent(1006033);
                    }
                    else
                    {
                        text.text = LanguageHelper.GetTextContent(1006034, cSVInstanceDailyData.Layerlevel.ToString());
                    }
                }
                else
                {
                    text.text = LanguageHelper.GetTextContent(1006032);
                }
            }
        }
        /// <summary>
        /// 设置阶梯
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="layerId"></param>
        /// <param name="isUnlockInstance"></param>
        /// <param name="cSVInstanceDailyData"></param>
        private void SetLadderItem(RectTransform rectTransform, uint layerId, bool isUnlockInstance, CSVInstanceDaily.Data cSVInstanceDailyData)
        {
            if (!isUnlockInstance || null == cSVInstanceDailyData)
            {
                ImageHelper.SetImageGray(rectTransform, true, true);
            }
            else
            {
                bool isGray = layerId >= cSVInstanceDailyData.LayerStage;
                ImageHelper.SetImageGray(rectTransform, isGray, true);
            }
        }
        /// <summary>
        /// 设置关卡菜单
        /// </summary>
        private void SetStageMenu()
        {
            List<Sys_Instance.StageData> stageDatas = list_StageDatas;
            for (int i = 0, count = list_Stage.Count; i < count; i++)
            {
                SetStageItem(list_Stage[i].gameObject, i < stageDatas?.Count ? stageDatas[i] : null);
            }
        }
        /// <summary>
        /// 设置关卡
        /// </summary>
        /// <param name="toggle"></param>
        private void SetStageItem(GameObject go, Sys_Instance.StageData data)
        {
            go.SetActive(null != data);
            if (null == data) return;
            go.name = data.cSVInstanceDailyData.id.ToString();
            Text text_Title1 = go.transform.Find("Text").GetComponent<Text>();
            Text text_Title2 = go.transform.Find("Text_Light").GetComponent<Text>();
            Image text_Icon = go.transform.Find("Image").GetComponent<Image>();

            text_Title1.text = string.Format("{0}-{1}", data.cSVInstanceDailyData.LayerStage, data.cSVInstanceDailyData.Layerlevel);
            text_Title2.text = text_Title1.text;
            ImageHelper.SetIcon(text_Icon, data.cSVInstanceDailyData.icon);
        }
        /// <summary>
        /// 设置关卡界面
        /// </summary>
        private void SetStageView()
        {
            RectTransform rt = rt_RightNode;
            var cSVInstanceDailyData = curStageData?.cSVInstanceDailyData;
            Packet.InsEntry insEntry = serverInstanceData?.GetInsEntry(curInstanceData?.instanceid ?? 0);
            Packet.DailyStage dailyStage = serverInstanceData?.GetDailyStage(curInstanceData?.instanceid ?? 0, curStageData?.stageid ?? 0);
            Packet.DailyStageBestInfo dailyStageBestInfo = serverInstanceData?.GetDailyStageBestInfo(curStageData?.stageid ?? 0);
            uint CurPassedStageId = insEntry?.PerMaxStageId ?? 0; //已通关的最高关卡
            CSVInstanceDaily.Data unlockCSVInstanceDailyData = curInstanceData?.GetUnlockStageData(CurPassedStageId)?.cSVInstanceDailyData;

            /// <summary> 关卡层数 </summary>
            Text text_layer = rt.Find("Text_Level").GetComponent<Text>();
            /// <summary> 关卡关数 </summary>
            Text text_stage = rt.Find("Text_Level/Text_Level (1)").GetComponent<Text>();
            /// <summary> 内容 </summary>
            Text text_content = rt.Find("View_Introduce/Text").GetComponent<Text>();
            /// <summary> 最佳玩家 </summary>
            Text text_Name = rt.Find("View_Introduce/Text_Best/Text_Name").GetComponent<Text>();
            /// <summary> 最佳时间 </summary>
            Text text_Time1 = rt.Find("View_Introduce/Text_Best/Text_Time").GetComponent<Text>();
            /// <summary> 播放按钮 </summary>
            GameObject go_Play1 = rt.Find("View_Introduce/Text_Best/Button_Play").gameObject;
            /// <summary> 未通关 </summary>
            Text text_Fail = rt.Find("View_Introduce/Text_Me/Text_Fail").GetComponent<Text>();
            /// <summary> 最佳时间 </summary>
            Text text_Time = rt.Find("View_Introduce/Text_Me/Text_Time").GetComponent<Text>();
            /// <summary> 播放按钮 </summary>
            GameObject go_Play2 = rt.Find("View_Introduce/Text_Me/Button_Play").gameObject;
            /// <summary> 提示 </summary>
            GameObject go_Tips = rt.Find("Text_Tips").gameObject;
            /// <summary>  </summary>
            Button button_Fight = rt.Find("Btn_01").GetComponent<Button>();


            text_layer.text = cSVInstanceDailyData?.LayerStage.ToString() ?? string.Empty;
            text_stage.text = cSVInstanceDailyData == null ? string.Empty : string.Format("-{0}", cSVInstanceDailyData.Layerlevel.ToString());
            text_content.text = cSVInstanceDailyData == null ? string.Empty : LanguageHelper.GetTextContent(cSVInstanceDailyData.Describe);
            text_Name.text = dailyStageBestInfo?.RoleName.ToStringUtf8() ?? string.Empty;
            text_Time1.text = dailyStageBestInfo == null ? string.Empty : LanguageHelper.GetTextContent(1006000, dailyStageBestInfo.Round.ToString());
            go_Play1.SetActive(dailyStageBestInfo == null ? false : dailyStageBestInfo.BattleRecordId != 0);
            text_Fail.gameObject.SetActive(dailyStage == null ? true : dailyStage.BestRound == 0);
            text_Time.gameObject.SetActive(dailyStage == null ? false : dailyStage.BestRound != 0);
            text_Time.text = dailyStage == null ? string.Empty : LanguageHelper.GetTextContent(1006000, dailyStage.BestRound.ToString());
            go_Play2.SetActive(false);
            uint unlockId = null == unlockCSVInstanceDailyData ? 0 : unlockCSVInstanceDailyData.id;
            bool isGray = (null == insEntry || null == cSVInstanceDailyData) ? true : unlockId < cSVInstanceDailyData.id;
            button_Fight.gameObject.SetActive(!isGray);
            go_Tips.SetActive(isGray);

            List<ItemIdCount> list_drop1 = CSVDrop.Instance.GetDropItem(cSVInstanceDailyData?.Award ?? 0);
            List<ItemIdCount> list_drop2 = CSVDrop.Instance.GetDropItem(cSVInstanceDailyData?.RandomAward ?? 0);
            uint x = 0;
            long y = 0;
            for (int i = 0, count = list_FirstRewardItem.Count; i < count; i++)
            {
                x = i < list_drop1.Count ? list_drop1[i].id : 0;
                y = i < list_drop1.Count ? list_drop1[i].count : 0;
                SetRewardItem(list_FirstRewardItem[i], x, y);
            }
            for (int i = 0, count = list_DropRewardItem.Count; i < count; i++)
            {
                x = i < list_drop2.Count ? list_drop2[i].id : 0;
                y = i < list_drop2.Count ? list_drop2[i].count : 0;
                SetRewardItem(list_DropRewardItem[i], x, y);
            }
        }
        /// <summary>
        /// 设置奖励模版
        /// </summary>
        /// <param name="propItem"></param>
        /// <param name="id"></param>
        /// <param name="Num"></param>
        private void SetRewardItem(PropItem propItem, uint id, long Num)
        {
            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(id);
            if (null == cSVItemData)
            {
                propItem.SetActive(false);
                return;
            }
            propItem.SetActive(true);
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(id, Num, true, false, false, false, false, true);
            propItem.SetData(new MessageBoxEvt(EUIID.UI_Onedungeons, itemData));

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(propItem.transform.Find("Btn_Item/Image_BG").gameObject);
            eventListener.triggers.Clear();
            eventListener.AddEventListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, ret => { UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Onedungeons, itemData)); });
        }
        /// <summary>
        /// 界面播放动画
        /// </summary>
        /// <param name="isShow"></param>
        /// <param name="isImmediately"></param>
        private void PlayAnimation_View(bool isShow, bool isImmediately = false)
        {
            if (isShow)
            {
                if (isImmediately)
                {
                    rt_CenterNode.DOAnchorPosX(-206, 0f);
                    rt_RightNode.DOAnchorPosX(0f, 0f);
                }
                else if (isPlayed)
                {
                    rt_RightNode.DOAnchorPosX(200f, 0.3f).SetEase(Ease.InSine)
                    .OnComplete(() =>
                    {
                        rt_RightNode.DOAnchorPosX(0f, 0.3f).SetEase(Ease.OutSine);
                    });
                }
                else
                {
                    rt_CenterNode.DOAnchorPosX(-206, 0.3f);
                    rt_RightNode.DOAnchorPosX(0f, 0.3f);
                }
                isPlayed = true;
            }
            else
            {
                rt_CenterNode.DOAnchorPosX(0f, isImmediately ? 0f : 0.3f);
                rt_RightNode.DOAnchorPosX(600f, isImmediately ? 0f : 0.3f);
                isPlayed = false;
            }
        }
        /// <summary>
        /// 角色播放动画
        /// </summary>
        private void PlayAnimation_Role()
        {
            sp_RoleAnimatorGroup.targetIndex = curLayerIndex;
            sp_RoleAnimatorGroup.Play();
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭界面
        /// </summary>
        public void OnClick_Close()
        {
            CloseSelf();
        }
        /// <summary>
        /// 排行界面
        /// </summary>
        public void OnClick_Rank()
        {
            uint instanceid = curStageData?.instanceid ?? 0;
            if (instanceid == 0) return;
            UIManager.OpenUI(EUIID.UI_Onedungeons_Ranking, false, instanceid);
        }
        /// <summary>
        /// 挑战
        /// </summary>
        public void OnClick_Fight()
        {
            uint instanceid = curStageData?.instanceid ?? 0;
            uint stageid = curStageData?.stageid ?? 0;
            if (instanceid == 0 || stageid == 0) return;
            Sys_Instance.Instance.InstanceEnterReq(instanceid, stageid);
        }
        /// <summary>
        /// 点击重置关卡层
        /// </summary>
        /// <param name="eventData"></param>
        public void OnClick_ResetLayer(BaseEventData eventData)
        {
            curLayerIndex = 0;
            curStageIndex = 0;
        }
        /// <summary>
        /// 点击重置关卡菜单
        /// </summary>
        /// <param name="go"></param>
        public void OnClick_ResetStage(GameObject go)
        {
            bool isGray = go.name.Contains("Gray");
            if (isGray)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1006040));
            }
            else
            {
                curStageIndex = DefaultStage();
            }
        }
        /// <summary>
        /// 点击副本等级
        /// </summary>
        public void OnClick_InstanceLevel()
        {
            curDropdownIndex = dropdown_InstanceLevel.value;
            SetLayerView();
            sp_RoleAnimatorGroup.progress = curLayerIndex;
            sp_RoleAnimatorGroup.SynchronizationPosition();
            UpdateLayerOption();
            SetBattleView();
            if (null != curInstanceData) Sys_Instance.Instance.DailyInstanceBestInfoReq(curInstanceData.instanceid);
        }
        /// <summary>
        /// 点击层数
        /// </summary>
        /// <param name="toggle"></param>
        public void OnClick_Layer(Toggle toggle, bool value)
        {
            if (value)
            {
                curLayerIndex = list_Layer.IndexOf(toggle);
                SetStageMenuView();
                UpdateStageOption();
                list_LayerTab[curLayerIndex].isOn = true;
                PlayAnimation_Role();
            }

            if (value)
            {
                PlayAnimation_View(true, false);
            }
            else if (!toggleGroup_Layer.AnyTogglesOn())
            {
                PlayAnimation_View(false, false);
            }
        }
        /// <summary>
        /// 取消关卡层选项
        /// </summary>
        /// <param name="eventData"></param>
        public void OnClick_CancelLayer(BaseEventData eventData)
        {
            if (!toggleGroup_Layer.AnyTogglesOn()) return;

            for (int i = 0, count = list_Layer.Count; i < count; i++)
            {
                var item = list_Layer[i];
                if (item.isOn)
                    item.isOn = false;
            }
        }
        /// <summary>
        /// 点击关卡
        /// </summary>
        /// <param name="toggle"></param>
        public void OnClick_Stage(Toggle toggle)
        {
            rt_StageBg.SetAsLastSibling();
            toggle.transform.SetAsLastSibling();
            curStageIndex = list_Stage.IndexOf(toggle);
            SetStageView();
        }
        /// <summary>
        /// 查看视频1
        /// </summary>
        public void OnClick_CheckVideo1()
        {

        }
        /// <summary>
        /// 查看视频2
        /// </summary>
        public void OnClick_CheckVideo2()
        {

        }
        /// <summary>
        /// 数据更新事件
        /// </summary>
        public void OnInstanceDataUpdate()
        {
            RefreshView();
        }
        /// <summary>
        /// 更新关卡最佳数据
        /// </summary>
        public void OnDailyInstanceBestInfoRes()
        {
            SetStageView();
        }
        #endregion
        #region 提供功能
        #endregion
    }
}
