using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using Net;
using Packet;

namespace Logic
{
    public partial class Sys_Knowledge : SystemModuleBase<Sys_Knowledge>
    {
        public uint CurBraveId = 0;

        /// <summary>
        /// 判断勇者是否激活
        /// </summary>
        /// <param name="braveId"></param>
        /// <returns></returns>
        public bool IsBraveActive(uint braveId)
        {
            bool isActive = false;
            CSVBrave.Data data = CSVBrave.Instance.GetConfData(braveId);
            if (data != null)
            {
                for (int i = 0; i < data.story_id.Count; ++i)
                {
                    isActive = IsKnowledgeActive(data.story_id[i]);
                    if (isActive)
                        break;
                }
            }

            return isActive;
        }

        /// <summary>
        /// 判断勇者是否需要红点显示
        /// </summary>
        /// <param name="braveId"></param>
        /// <returns></returns>
        public bool IsBraveRedPoint(uint braveId)
        {
            CSVBrave.Data data = CSVBrave.Instance.GetConfData(braveId);

            bool isRed = false;
            for (int i = 0; i < listDatas.Count; ++i)
            {
                if (listDatas[i].Type == (uint)ETypes.Brave)
                {
                    if (data != null 
                        && data.story_id != null)
                    {
                        for (int j = 0; j < data.story_id.Count; ++j)
                        {
                           if (listDatas[i].ShowNewList.IndexOf (data.story_id[j]) >= 0)
                           {
                                isRed = true;
                                break;
                           }
                        }
                    }
                }
            }

            return isRed;
        }

        public uint GetBraveId(uint eventId)
        {
            foreach(var data in CSVBrave.Instance.GetAll())
            {
                if (data.story_id != null
                    && data.story_id.IndexOf(eventId) >= 0)
                    return data.id;
            }

            return 0u;
        }
    }
}