using System;
using System.Collections.Generic;
using Logic.Core;
using Packet;
using Table;
namespace Logic
{
    /// <summary>
    /// 主题表的ID 主题ID
    /// topicID  主题类型
    /// </summary>
    public partial class Sys_GoddnessTrial : SystemModuleBase<Sys_GoddnessTrial>
    {
        /// <summary>
        /// 相同 topicID (主题类型) 的主题集合,它包含不同等级段和同等级段不同困难度的主题id
        /// </summary>
        public class Topic
        {
            public uint id = 0;

            public uint Name = 0;

            public string icon = string.Empty;

            public Dictionary<uint, TopicLevel> m_DicLevel = new Dictionary<uint, TopicLevel>();

            /// <summary>
            /// 在一个topic 下 查找符合的主题
            /// </summary>
            /// <param name="level"></param>
            /// <returns></returns>
            public TopicLevel Find(uint level)
            {
                foreach (var kvp in m_DicLevel)
                {
                    if (kvp.Value.Min <= level && kvp.Value.Max >= level)
                    {
                        return kvp.Value;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 同一个topicID(主题类型)且等级区间相同的主题集合,它包含同等级不同困难度的主题
        /// </summary>
        public class TopicLevel
        {
            public uint Min = 0;
            public uint Max = 0;

            public Dictionary<uint, uint> m_DicActity = new Dictionary<uint, uint>();

            public List<uint> m_DifficList = new List<uint>();
            /// <summary>
            /// 根据难度查找主题
            /// </summary>
            /// <param name="difficly"></param>
            /// <returns></returns>
            public uint Find(uint difficly)
            {
                uint result = 0;

                m_DicActity.TryGetValue(difficly, out result);

                return result;
            }

        }

        /// <summary>
        /// 不同主题类型,
        /// </summary>
        public Dictionary<uint, Topic> TopicTypeDic { get { return m_DicTopic; } }

        private Dictionary<uint, Topic> m_DicTopic = new Dictionary<uint, Topic>();

        public List<uint> m_DifficlyList = new List<uint>();
        public List<uint> m_DifficlyLangue = new List<uint>();
      

        /// <summary>
        /// 与匹配 m_curTopicLevel 的玩家的等级，用于避免重复计算
        /// </summary>
        private uint m_curRoleLevel = 0;


        /// <summary>
        /// 相同topic 相同等级段的主题集合（包含不同难度）
        /// </summary>
        /// 
        private TopicLevel m_curTopicLevel = null;
        public TopicLevel CurTopicLevel { get { return m_curTopicLevel; } private set {

                if (value == m_curTopicLevel)
                    return;

                m_curTopicLevel = value;

                eventEmitter.Trigger(EEvents.RefrshTopicLevel);
            } }


        private void InitConfig()
        {
            var alltopicData = CSVGoddessTopic.Instance.GetAll();

            foreach (var kvp in alltopicData)
            {
                if (m_DifficlyList.FindIndex(o => o == kvp.topicDifficulty) < 0)
                {
                    m_DifficlyList.Add(kvp.topicDifficulty);
                }

                if (m_DicTopic.ContainsKey(kvp.topicId) == false)
                {
                    m_DicTopic.Add(kvp.topicId, new Topic() { id = kvp.topicId ,Name = kvp.topicLan,icon = kvp.topicIcon});
                }

                var topicItem = m_DicTopic[kvp.topicId];

                uint min = kvp.copyLevel[0];
                uint max = kvp.copyLevel[1];

                uint key = (min << 12) | max;// 人物等级最大不超过 4000 否则是会出错的。前8位留个主题类型ID

                if (topicItem.m_DicLevel.ContainsKey(key) == false)
                {
                    topicItem.m_DicLevel.Add(key, new TopicLevel() { Min = min, Max = max });
                }

                topicItem.m_DicLevel[key].m_DicActity.Add(kvp.topicDifficulty, kvp.id);
                topicItem.m_DicLevel[key].m_DifficList.Add(kvp.topicDifficulty);
            }

            m_DifficlyList.Sort();


            var paramData = CSVParam.Instance.GetConfData(847);
            string[] paramValues = paramData.str_value.Split('|');

            for (int i = 0; i < paramValues.Length; i++)
            {
                uint uvalue = 0;
                uint.TryParse(paramValues[i], out uvalue);

                m_DifficlyLangue.Add(uvalue);
            }

            SelectDifficlyID = m_DifficlyList[0];
            MaxDifficlyID = m_DifficlyList[0];
        }

        /// <summary>
        /// 刷新当前的主题
        /// </summary>
        private void RefreshCurTopicLevel(uint level)
        {
            //首先根据等级从表里去数据判断topic 是否为一个，如果为一个时直接取表数据

            // 如果匹配等级的topic 为多个，则从服务器数据 randomtopic 数组中取得 topic ID

            int datacount = 0;
            TopicLevel topicLevel = null;

            m_curRoleLevel = level;

            uint topicID = 0;
            foreach (var kvp in m_DicTopic)
            {
                var result = kvp.Value.Find(level);

                if (result != null)
                {
                    datacount += 1;
                    topicLevel = result;
                    topicID = kvp.Key;
                }
            }
            if (datacount <= 1)
            {
                CurTopicLevel = topicLevel;

                TopicTypeID = topicID;

                return;
            }

            //根据服务器提供的数据 取主题

            int count = RandomTopicSortList.Count;
          

            for (int i = 0; i < count; i++)
            {
                if ((i > 0 && level < RandomTopicSortList[i].MinLevel && level >= RandomTopicSortList[i].MinLevel) ||
                    (i == 0 && level < RandomTopicSortList[i].MinLevel))
                {
                    topicID = RandomTopicSortList[i].TopicId;
                    break;
                }
            }

            Topic topic;
            bool gresult = m_DicTopic.TryGetValue(topicID, out topic);

            CurTopicLevel = gresult ? topic.Find(level) : null;

            TopicTypeID = gresult ? topicID : 0;
        }


        public CSVGoddessTopic.Data GetGoddessTopicData(uint difficly)
        {
            if (Sys_Role.Instance.Role.Level != m_curRoleLevel)
                RefreshCurTopicLevel(Sys_Role.Instance.Role.Level);

            if (CurTopicLevel == null)
                return null;

            var id = CurTopicLevel.Find(difficly);

            return CSVGoddessTopic.Instance.GetConfData(id);


        }

        /// <summary>
        /// 获得主题数据
        /// </summary>
        /// <param name="topicid">topic id</param>
        /// <param name="level">玩家等级</param>
        /// <param name="difficly">困难度</param>
        /// <returns></returns>
        private CSVGoddessTopic.Data GetGoddessTopicData(uint topicid, uint level, uint difficly)
        {
            Topic topic;

            if (m_DicTopic.TryGetValue(topicid, out topic) == false)
                return null;

            TopicLevel topicLevel = null;

            foreach (var kvp in topic.m_DicLevel)
            {
                if (kvp.Value.Min <= level && kvp.Value.Max >= level)
                {
                    topicLevel = kvp.Value;
                    continue;
                }
            }

            if (topicLevel == null)
                return null;

            uint dataid = 0;
            if (topicLevel.m_DicActity.TryGetValue(difficly, out dataid) == false)
                return null;

            return CSVGoddessTopic.Instance.GetConfData(dataid);
        }

        /// <summary>
        /// 根据副本ID获得关卡列表。
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public List<CSVInstanceDaily.Data>  GetInstanceDailys(uint instance)
        {
            var list = Sys_Instance.Instance.getDailyByInstanceID(instance);

            return list;
        }

        /// <summary>
        /// 获取女神炼狱选择
        /// </summary>
        /// <param name="instanceDailyID">关卡ID</param>
        /// <returns></returns>
        public CSVGoddessSelect.Data GetChapterSelect(uint instanceDailyID)
        {
            return CSVGoddessSelect.Instance.GetConfData(instanceDailyID);
        }

        /// <summary>
        /// 通过副本ID获得主题ID。目前功能待定（应该由服务器直接告知队员主题ID）
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public int GetTopicDataByInstanceID(uint instanceID, out CSVGoddessTopic.Data data)
        {
            var dicdata = CSVGoddessTopic.Instance.GetAll();

            data = null;

            int index = -1;

            foreach (var kvp in dicdata)
            {
                var temp = kvp.InstanceId.FindIndex(o => o == instanceID);

                if (temp >= 0)
                {
                    data = kvp;

                    index = temp;

                    break;
                }
            }

            return index;
        }

        public uint GetTopicTypeNameLangueID(uint topicTypeid)
        {
            Topic value;
            if (m_DicTopic.TryGetValue(topicTypeid, out value) == false)
                return 0;

            return value.Name;
        }

        public bool IsVailtInTeam(CSVTeam.Data data)
        {
            if (GTData == null)
                return false;

            int count = m_DifficlyList.Count;

            uint maxDifficult = GetMaxDifficult();

            for (int i = 0; i < count; i++)
            {
                if (m_DifficlyList[i] <= maxDifficult)
                {
                    var topicdatadiff = GetGoddessTopicData(m_DifficlyList[i]);

                    if (data.id == topicdatadiff.teamTarget)
                        return true;
                }
              
            }
            return false;
        }
    }

    public static class CSVGoddessSelectDataHelper
    {
        public static int StatgeCount(this CSVGoddessSelect.Data data)
        {
            int count = 0;

            if (data.StageId1 > 0)
                count += 1;

            if (data.StageId2 > 0)
                count += 1;

            if (data.StageId3 > 0)
                count += 1;

            if (data.StageId4 > 0)
                count += 1;

            return count;
        }

        public static int GetStatgeIndex(this CSVGoddessSelect.Data data, uint levleID)
        {

            if (data.StageId1 == levleID)
                return 0;

            if (data.StageId2 == levleID)
                return 1;
            if (data.StageId3 == levleID)

                return 2;

            if (data.StageId4 == levleID)
                return 3;

            return 0;
        }

        public static uint GetStatge(this CSVGoddessSelect.Data data,int index)
        {
            uint value = 0;

           switch(index)
            {
                case 0:
                    value = data.StageId1;
                    break;
                case 1:
                    value = data.StageId2;
                    break;
                case 2:
                    value = data.StageId3;
                    break;
                case 3:
                    value = data.StageId4;
                    break;
            }

            return value;
        }

        public static uint GetStatgeLanID(this CSVGoddessSelect.Data data, int index)
        {
            uint value = 0;

            switch (index)
            {
                case 0:
                    value = data.lanid1;
                    break;
                case 1:
                    value = data.lanid2;
                    break;
                case 2:
                    value = data.lanid3;
                    break;
                case 3:
                    value = data.lanid4;
                    break;
            }

            return value;
        }
    }
       
}
