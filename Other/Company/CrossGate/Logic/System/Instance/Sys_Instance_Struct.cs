using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using System;
using Net;
using Packet;
namespace Logic
{
    public partial class Sys_Instance : SystemModuleBase<Sys_Instance>
    {
        #region 数据定义
        /// <summary> 事件枚举 </summary>
        public enum EEvents
        {
            InstanceEnter,               //进入副本
            InstanceExit,                //离开副本
            InstanceData,                //副本数据更新
            InstanceDataUpdate,          //副本数据更新
            InstanceDataAll,          //副本数据更新
            DailyInstanceBestInfoRes,    //最佳数据更新
            DailyInstanceRankInfoRes,    //排行榜数据
            PlayerConfirmNtf,            //多人副本准备投票通知
            TeamInstanceProgress,        //队伍副本进度
            RewardRefresh,//奖励
            StartVote,//开始投票
            PassStage,//通关关卡
            SwitchStage,//切换关卡
            InstanceEnd,//副本结束
            StateNtf,//登录，重连后副本
        }
        /// <summary> 副本开启条件 </summary>
        public enum EInstanceCondition
        {
            PreInstance,  //前置副本
            Level,        //等级
            Item,         //道具
            Count,        //次数
            People,       //人数
            Time,         //时间
        }
        /// <summary> 条件判断基类 </summary>
        public class ConditionBase
        {
            /// <summary> 条件是否完成 </summary>
            protected Dictionary<string, bool> dict_Condition = new Dictionary<string, bool>();
            /// <summary>
            /// 构建函数
            /// </summary>
            /// <param name="type"></param>
            public ConditionBase(Type type)
            {
                var values = System.Enum.GetNames(type);
                foreach (string child in values)
                {
                    dict_Condition.Add(child, false);
                }
            }
            /// <summary>
            /// 是否打开
            /// </summary>
            /// <returns></returns>
            public bool IsOpen()
            {
                foreach (var item in dict_Condition.Values)
                {
                    if (!item) return false;
                }
                return true;
            }
            /// <summary>
            /// 得到错误原因
            /// </summary>
            /// <returns></returns>
            public string GetErrorReason()
            {
                foreach (var item in dict_Condition)
                {
                    if (!item.Value)
                        return GetErrorReason(item.Key);
                }
                return string.Empty;
            }
            /// <summary>
            /// 检测条件
            /// </summary>
            public virtual void Check()
            {

            }
            /// <summary>
            /// 得到错误原因
            /// </summary>
            /// <param name="eActivityCondition"></param>
            /// <returns></returns>
            public virtual string GetErrorReason(string type)
            {
                return string.Empty;
            }
        }
        /// <summary> 副本条件判断 </summary>
        public class Condition_Instance : ConditionBase
        {
            public InstanceData instanceData;
            public Condition_Instance(InstanceData instanceData) : base(typeof(EInstanceCondition))
            {
                this.instanceData = instanceData;
            }
            ~Condition_Instance()
            {
                instanceData = null;
            }
            public override void Check()
            {
                List<string> list_Keys = new List<string>(dict_Condition.Keys);
                for (int i = 0; i < list_Keys.Count; i++)
                {
                    var item = list_Keys[i];
                    switch (Enum.Parse(typeof(EInstanceCondition), item))
                    {
                        case EInstanceCondition.PreInstance:
                            {
                                ServerInstanceData serverInstanceData;
                                if (!Sys_Instance.Instance.dict_ServerInstanceData.TryGetValue(instanceData.activityid, out serverInstanceData))
                                {
                                    dict_Condition[item] = false;
                                    break;
                                }
                                var typeData = serverInstanceData.GetInsEntry(instanceData.instanceid);
                                dict_Condition[item] = typeData.Unlock;
                            }
                            break;
                        case EInstanceCondition.Level:
                            {
                                uint lv = Sys_Role.Instance.Role.Level;
                                var list = instanceData.cSVInstanceData.lv;
                                dict_Condition[item] = list?.Count == 2 ? lv >= list[0] && lv < list[1] : true;
                            }
                            break;
                        case EInstanceCondition.Item:
                            {
                                bool isTrue = true;
                                var list = instanceData.cSVInstanceData.Ticket;
                                foreach (var child in list)
                                {
                                    uint id = child[0];
                                    uint count = child[1];
                                    if (count > 0 && Sys_Bag.Instance.GetItemCount(id) < count)
                                    {
                                        isTrue = false;
                                        break;
                                    }
                                }
                                dict_Condition[item] = isTrue;
                            }
                            break;
                        case EInstanceCondition.Count:
                            {
                                DailyActivity dailyActivity = Sys_Daily.Instance.GetDailyActivityCom(instanceData.activityid);
                                if (null == dailyActivity || null == dailyActivity.ResLimit)
                                {
                                    dict_Condition[item] = false;
                                    break;
                                }
                                dict_Condition[item] = dailyActivity.ResLimit.UsedTimes >= dailyActivity.ResLimit.MaxTimes;
                            }
                            break;
                        case EInstanceCondition.People:
                            {
                                int TeamMemsCount = Sys_Team.Instance.TeamMemsCount == 0 ? 1 : Sys_Team.Instance.TeamMemsCount;
                                dict_Condition[item] = instanceData.cSVInstanceData.limite_number == TeamMemsCount;
                            }
                            break;
                        case EInstanceCondition.Time:
                            {
                                dict_Condition[item] = true;
                            }
                            return;
                    }
                }
            }
            public override string GetErrorReason(string type)
            {
                EInstanceCondition eInstanceCondition = (EInstanceCondition)Enum.Parse(typeof(EInstanceCondition), type);

                switch (eInstanceCondition)
                {
                    case EInstanceCondition.PreInstance:
                        return LanguageHelper.GetTextContent(1006001);
                    case EInstanceCondition.Level:
                        return LanguageHelper.GetTextContent(1006002);
                    case EInstanceCondition.Item:
                        return LanguageHelper.GetTextContent(1006003);
                    case EInstanceCondition.People:
                        return LanguageHelper.GetTextContent(1006004);
                    case EInstanceCondition.Count:
                        return LanguageHelper.GetTextContent(1006005);
                    case EInstanceCondition.Time:
                        return LanguageHelper.GetTextContent(1006006);
                    default:
                        return string.Empty;
                }
            }
        }
        /// <summary> 副本数据 </summary>
        public class InstanceData
        {
            /// <summary> 活动编号 </summary>
            public uint activityid { get; private set; } = 0;
            /// <summary> 副本编号 </summary>
            public uint instanceid { get; private set; } = 0;
            /// <summary> 副本配表数据 </summary>
            public CSVInstance.Data cSVInstanceData { get; private set; } = null;
            /// <summary> 关卡数据 </summary>
            public List<StageData> list_StageData { get; private set; } = new List<StageData>();
            /// <summary> 查询层对应关卡数据 </summary>
            public Dictionary<uint, List<StageData>> dict_Check { get; private set; } = new Dictionary<uint, List<StageData>>();
            /// <summary> 条件判断 </summary>
            public Condition_Instance condition { get; private set; } = null;
            /// <summary>
            /// 构建函数
            /// </summary>
            /// <param name="id"></param>
            public InstanceData(uint id)
            {
                this.cSVInstanceData = CSVInstance.Instance.GetConfData(id);
                this.instanceid = id;
                this.activityid = cSVInstanceData.id;
                this.SetStageList();
                condition = new Condition_Instance(this);
            }
            /// <summary>
            /// 设置关卡数据数据
            /// </summary>
            private void SetStageList()
            {
                var data = CSVInstanceDaily.Instance.GetAll();
                foreach (CSVInstanceDaily.Data child in data)
                {
                    if (child.InstanceId != instanceid)
                        continue;

                    StageData stageData = new StageData(child, (EInstanceType)cSVInstanceData.Type);
                    list_StageData.Add(stageData);

                    if (!dict_Check.ContainsKey(child.LayerStage))
                    {
                        dict_Check.Add(child.LayerStage, new List<StageData>() { stageData });
                    }
                    else
                    {
                        dict_Check[child.LayerStage].Add(stageData);
                    }
                }
            }
            /// <summary>
            /// 寻找关卡数据
            /// </summary>
            /// <param name="Stageid"></param>
            /// <returns></returns>
            public StageData GetCurStageData(uint Stageid)
            {
                return list_StageData.Find(x => x.stageid == Stageid);
            }
            /// <summary>
            /// 根据完成关卡获取下一个解锁关卡数据
            /// </summary>
            /// <param name="Stageid"></param>
            /// <returns></returns>
            public StageData GetUnlockStageData(uint Stageid)
            {
                if (list_StageData.Count > 0)
                {
                    int index = list_StageData.FindIndex(x => x.stageid == Stageid);
                    if (index >= list_StageData.Count - 1)
                        index = list_StageData.Count - 1;
                    else
                        index++;
                    return list_StageData[index];
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary> 关卡数据 </summary>
        public class StageData
        {
            /// <summary> 副本编号 </summary>
            public uint instanceid { get; private set; } = 0;
            /// <summary> 关卡编号 </summary>
            public uint stageid { get; private set; } = 0;
            /// <summary> 副本类型 </summary>
            public EInstanceType instanceType { get; private set; } = 0;
            /// <summary> 副本数据 </summary>
            public CSVInstanceDaily.Data cSVInstanceDailyData { get; private set; } = null;

            /// <summary>
            /// 构建函数
            /// </summary>
            /// <param name="cSVInstanceDailyData"></param>
            /// <param name="instanceType"></param>
            public StageData(CSVInstanceDaily.Data cSVInstanceDailyData, EInstanceType instanceType)
            {
                this.cSVInstanceDailyData = cSVInstanceDailyData;
                this.instanceType = instanceType;
                this.stageid = cSVInstanceDailyData.id;
                this.instanceid = cSVInstanceDailyData.InstanceId;
            }
        }
        /// <summary> 服务器数据 </summary>
        public class ServerInstanceData
        {
            public uint playType;
            public uint insType;
            public InstanceCommonData instanceCommonData;
            public InstancePlayTypeData instancePlayTypeData;
            public Google.Protobuf.Collections.RepeatedField<Packet.DailyStageBestInfo> bestInfos = new Google.Protobuf.Collections.RepeatedField<DailyStageBestInfo>();

            /// <summary>
            /// 获取副本对应类型的具体数据
            /// </summary>
            /// <param name="instanceid"></param>
            /// <returns></returns>
            public object GetData(uint instanceid)
            {
                object ob = null;
                switch ((InsType)instancePlayTypeData.InsType)
                {
                    case InsType.Daily:
                    case InsType.Multi:
                        {
                            var list = instancePlayTypeData.DailyIns.Entries;
                            DailyInsEntry dailyInsEntry = null;
                            for (int i = 0, count = list.Count; i < count; i++)
                            {
                                if (list[i].InstanceId == instanceid)
                                {
                                    dailyInsEntry = list[i];
                                    break;
                                }
                            }
                            ob = dailyInsEntry;
                        }
                        break;
                }
                return ob;
            }
            /// <summary>
            /// 副本通用数据
            /// </summary>
            /// <param name="instanceid"></param>
            /// <returns></returns>
            public InsEntry GetInsEntry(uint instanceid)
            {
                Google.Protobuf.Collections.RepeatedField<Packet.InsEntry> insEntrys = instanceCommonData.Entries;
                InsEntry insEntry = null;
                foreach (Packet.InsEntry item in insEntrys)
                {
                    if (item.InstanceId == instanceid)
                    {
                        insEntry = item;
                        break;
                    }
                }
                return insEntry;
            }
            /// <summary>
            /// 副本单关信息
            /// </summary>
            /// <param name="instanceid"></param>
            /// <param name="stageid"></param>
            /// <returns></returns>
            public DailyStage GetDailyStage(uint instanceid, uint stageid)
            {
                DailyInsEntry dailyInsEntry = null;
                DailyStage dailyStage = null;
                foreach (Packet.DailyInsEntry item in instancePlayTypeData.DailyIns.Entries)
                {
                    if (item.InstanceId == instanceid)
                    {
                        dailyInsEntry = item;
                        break;
                    }
                }
                if (null != dailyInsEntry)
                {
                    foreach (Packet.DailyStage item in dailyInsEntry.Stages)
                    {
                        if (item.StageId == stageid)
                        {
                            dailyStage = item;
                            break;
                        }
                    }
                }
                return dailyStage;
            }
            /// <summary>
            /// 关卡最佳数据
            /// </summary>
            /// <param name="stageid"></param>
            /// <returns></returns>
            public DailyStageBestInfo GetDailyStageBestInfo(uint stageid)
            {
                DailyStageBestInfo dailyStageBestInfo = null;
                foreach (Packet.DailyStageBestInfo item in bestInfos)
                {
                    if (item.StageId == stageid)
                    {
                        dailyStageBestInfo = item;
                        break;
                    }
                }
                return dailyStageBestInfo;
            }
            /// <summary>
            /// 日常副本某玩法数据
            /// </summary>
            /// <param name="instanceid"></param>
            /// <returns></returns>
            public DailyInsEntry GetDailyInsEntry(uint instanceid)
            {
                DailyInsEntry dailyInsEntry = null;
                foreach (Packet.DailyInsEntry item in instancePlayTypeData.DailyIns.Entries)
                {
                    if (item.InstanceId == instanceid)
                    {
                        dailyInsEntry = item;
                        break;
                    }
                }
                return dailyInsEntry;
            }
        }
        /// <summary> 当前副本 </summary>
        public struct CurInstance
        {
            /// <summary> 当前副本ID </summary>
            public uint InstanceId;
            /// <summary> 当前所在关卡ID </summary>
            public uint StageID;
            /// <summary>
            /// 重置
            /// </summary>
            public void Reset()
            {
                InstanceId = 0;
                StageID = 0;
            }
        }
        #endregion
    }


}
