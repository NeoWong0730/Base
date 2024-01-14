using System;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public partial class Sys_GoddnessTrial:SystemModuleBase<Sys_GoddnessTrial>
    {

        public int ChaperIndex { get; private set; } = 0;
        /// <summary>
        /// 选择的难度,主要用于同步多个界面用
        /// </summary>
        public uint SelectDifficlyID { get; private set; } = 0;

        public uint MaxDifficlyID { get; private set; } = 0;

        /// <summary>
        /// 选择的副本ID
        /// </summary>
        public uint SelectInstance { get; private set; } = 0;


        /// <summary>
        /// 选择的主题ID
        /// </summary>
        public uint SelectID { get; private set; } = 0;

        /// <summary>
        /// 关卡ID
        /// </summary>
        public uint SelectStageID { get; private set; } = 0;


        /// <summary>
        /// 主题类型ID topicID
        /// </summary>
        public uint TopicTypeID { get; private set; } = 0;
        public void SetSelectChaperIndex(uint index)
        {
            ChaperIndex = (int)index;
        }


        public void RefreshSelectChaperInstance()
        {
            var data = GetGoddessTopicData(SelectDifficlyID);

            if (data == null)
                return;

            SelectInstance = data.InstanceId[ChaperIndex];

            SelectStageID = GetSelectStage(SelectID, SelectInstance);
        }
        /// <summary>
        /// 设置难度
        /// </summary>
        /// <param name="difficly"></param>
        public void SetDifficly(uint difficly)
        {
            if (difficly == 0 || (SelectID != 0 && SelectID == difficly))
                return;

            SelectDifficlyID = difficly;
            
            var data = GetGoddessTopicData(difficly);

            SelectID = data.id;

            SendSetDifficulty(SelectID);

            SelectInstance = data.InstanceId[ChaperIndex];

            SelectStageID = GetSelectStage(SelectID, SelectInstance);

            eventEmitter.Trigger(EEvents.TopicDifficult);
        }

        /// <summary>
        /// 设置副本或者是 设置章节
        /// </summary>
        public void SetSelectInstance(uint instance)
        {
            var topicData = CSVGoddessTopic.Instance.GetConfData(SelectID);

            if (topicData == null)
                return;

            int index = topicData.InstanceId.FindIndex(o => o == instance);

            if (index < 0)
                return;

            SelectInstance = instance;

            SelectStageID = GetSelectStage(SelectID, SelectInstance);

        }
        /// <summary>
        /// 关卡ID
        /// </summary>
        /// <param name="id">主题ID</param>
        /// <param name="instance">副本ID</param>
        /// <returns></returns>
        private uint GetSelectStage(uint id, uint instance)
        {
            var value = GTData.Entries.Find<GoddessTopicEntry>(o => o.Id == id);    

            if (value != null)
            {
                var insValue = value.Entries.Find<GoddessInsEntry>(o => o.InsId == instance);

                var voteid = GetLastFinalInstance(id, instance);

                if (voteid != 0)//如果找到了副本的投票记录,就去副本记录的投票关卡
                    return voteid;

                if (insValue != null) //查找到副本记录，直接取第一个关卡，否则取本地配置文件第一个关卡
                {
                    if (insValue.Stages.Count > 0 && insValue.Stages[0].StageId != 0)
                        return insValue.Stages[0].StageId;
                }

            }

            //服务器数据取不到时，采用本地配置文件第一个关卡

            var list = GetInstanceDaily(instance);

            if (list.Count > 0)
                return list[0].id;

            return 0;
        }


        /// <summary>
        /// 获取基础信息后，设置相关的主题信息
        /// </summary>
        private void RefreshDefaultConfig()
        {
            MaxDifficlyID = GetMaxDifficult();

            uint topicID = GTData.SelectTopicDiffId;

            if (topicID == 0 && CurTopicLevel != null)
            {
                SelectDifficlyID = 1;
                topicID = CurTopicLevel.m_DicActity[SelectDifficlyID];

            }

           var data =  CSVGoddessTopic.Instance.GetConfData(topicID);

            if (data == null)
            {
                return;
            }

            SelectDifficlyID = data.topicDifficulty;

            SelectID = data.id;

            var value = GTData.Entries.Find<GoddessTopicEntry>(o => o.Id == SelectID);

            if (value == null || value.PerPassMaxInsId == 0) // 当服务器没有副本记录时，则取默认的副本，即是主题的第一个副本，关卡是副本的第一个关卡
            {
                ChaperIndex = 0;
                SelectInstance = data.InstanceId[0];
                SelectStageID = GetSelectStage(SelectID, SelectInstance);
            }
            else //
            {
                ChaperIndex = data.InstanceId.FindIndex(o => o == value.PerPassMaxInsId);

                ChaperIndex = (ChaperIndex + 1) >= data.InstanceId.Count ? (data.InstanceId.Count - 1) : (ChaperIndex + 1);

                SelectInstance = data.InstanceId[ChaperIndex];

                //var stage = GetLastFinalInstance(SelectID, SelectInstance);

                
                 SelectStageID = GetSelectStage(SelectID, SelectInstance);
             


            }
                
            

        }

        /// <summary>
        /// 获取副本中投票的历史记录
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public uint GetLastFinalInstance(uint topicId,uint instance)
        {
            if (GTData == null)
                return 0;

            var value = GTData.Entries.Find<GoddessTopicEntry>(o => o.Id == topicId);

            if (value == null)
                return 0;

            var data = CSVGoddessTopic.Instance.GetConfData(topicId);

            var index = data.InstanceId.FindIndex(o => o == instance);

            if (index < 1)
                return 0;

            //查询上个副本的最后一个关卡是否有投票结果，有就取上个副本最后的投票结果
            uint realinstance = data.InstanceId[index - 1];
              
            var votedata = value.LastFinalInsStageVote.Find(o => o.InsId == realinstance);

            if (votedata != null)
                return votedata.StageId;


            //因为上个副本某个关卡会有投票，导致最后一个关卡会不同，所以查询上个副本最后的关卡，然后根据配置取出当前副本的第一个关卡id
            var entriesValue = value.Entries.Find(o => o.InsId == realinstance);

            if (entriesValue == null || entriesValue.Stages.Count == 0)
                return 0;

            var csvdata = CSVGoddessSelect.Instance.GetConfData(entriesValue.Stages[entriesValue.Stages.Count - 1].StageId);

            if (csvdata == null)
                return 0;

            var stagedata = CSVInstanceDaily.Instance.GetConfData(csvdata.StageId1);


            if (stagedata == null || stagedata.InstanceId != instance)
                return 0;

            return csvdata.StageId1;

            
        }
        /// <summary>
        /// 获取当前选择副本的关卡列表
        /// </summary>
        /// <returns></returns>
        public List<CSVInstanceDaily.Data>  GetInstanceDaily()
        {
            return GetInstanceDaily(SelectInstance);
        }

        public List<CSVInstanceDaily.Data>  GetInstanceDaily(uint instanceID)
        {
            return Sys_Instance.Instance.getDailyByInstanceID(instanceID);
        }

        public CSVInstanceDaily.Data GetSelectInstanceDailyData()
        {
            return CSVInstanceDaily.Instance.GetConfData(SelectStageID);
        }

        ///// <summary>
        ///// 获得最高的解锁难度
        ///// </summary>
        ///// <returns></returns>
        //public uint GetUnLockDifficult() 
        //{
        //    uint diffic = m_DifficlyList[0];

        //    int count = GTData.Id.Count;

        //    for (int i = 0; i < count; i++)
        //    {
        //        var data = CSVGoddessTopic.Instance.GetConfData(GTData.Id[i]);

        //        if (data != null && data.topicDifficulty > diffic)
        //            diffic = data.topicDifficulty;
        //    }

        //    return diffic;
        //}


        /// <summary>
        /// 根据副本ID获得，在当前主题下副本的顺序序列
        /// </summary>
        /// <param name="instancdID"></param>
        /// <returns></returns>
        public int GetCurSelectTopicChapterIndex(uint instancdID)
        {
            var data = CSVGoddessTopic.Instance.GetConfData(SelectID);

            if (data == null)
                return 0;

            var value = data.InstanceId.FindIndex(o => o == instancdID);

            return value + 1;
        }

        /// <summary>
        /// 结合人物的等级，获得最高的难度
        /// </summary>
        /// <returns></returns>
        public uint GetMaxDifficult()
        {
            if (GTData == null)
                return m_DifficlyList[0];

            int count =  GTData.Id.Count;

            if (count == 0)
                return m_DifficlyList[0];

            uint roleLevel = Sys_Role.Instance.Role.Level;

            for (int i = 0; i < count; i++)
            {
                //根据人物的等级，找到主题表id
               var data = CSVGoddessTopic.Instance.GetConfData(GTData.Id[i]);

                if (data != null && data.copyLevel.Count == 2 && data.copyLevel[0] <= roleLevel && data.copyLevel[1] >= roleLevel)
                {
                    return data.topicDifficulty;
                }
            }

            return m_DifficlyList[0];
        }


        /// <summary>
        /// 根据难度获得主题的首次通过的队伍信息
        /// </summary>
        /// <param name="difficult"></param>
        /// <returns></returns>
        public GoddessTeamEntry GetTeamInfoByDifficult(uint difficult)
        {
            if (FristCrossInfo == null)
                return null;

            var topicdata =   GetGoddessTopicData(difficult);

            if (topicdata == null)
                return null;

            var result = FristCrossInfo.Team.Find<GoddessTeamEntry>(o => o.Id == topicdata.id);

            return result;
        }

        /// <summary>
        /// 根据最后一个关卡id获得结局
        /// </summary>
        /// <param name="endlevleid"></param>
        /// <returns></returns>
        public CSVGoddessEnd.Data GetEndData(uint endlevleid)
        {
            int count = CSVGoddessEnd.Instance.Count;

            for (int i = 0; i < count; i++)
            {
                CSVGoddessEnd.Data data = CSVGoddessEnd.Instance.GetByIndex(i);
                if (data.stageId.FindIndex(o => o == endlevleid) >= 0)
                {
                    return data;
                }
            }

            return null;
        }
    }

    static class RepeatedFieldHelper
    {
        public static T Find<T>(this RepeatedField<T> data, Func<T,bool> action)
        {
            int count = data.Count;
            for (int i = 0; i < count; i++)
            {
                if (action(data[i]))
                    return data[i];
            }

            return default(T);
        }
    }

}
