using System;
using System.Collections.Generic;
using System.Json;
using Logic.Core;
using Net;
using Packet;
using SQLite4Unity3d;

namespace Logic
{

    // 红点设置的保存与读取

    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {
        public class DailyNewTips
        {
            public uint ID = 0;
            public bool isFuncTips  = false;
            public bool isLimiteTips  = false;
            public bool isNoctice = false;

            public uint time = 0;
        }

        private Dictionary<uint, DailyNewTips> mCustomNewTips = new Dictionary<uint, DailyNewTips>();//用户数据

        private void ParseJson(JsonObject jsonvalue)
        {
            foreach (var value in jsonvalue)
            {
                DailyNewTips item = new DailyNewTips();

                JsonHeler.DeserializeObject((JsonObject)value.Value, item);

                item.isLimiteTips = false;
                item.isNoctice = false;

                mCustomNewTips.Add(item.ID, item);
            }
        }
   
        private  void LoadNewTipsDB()
        {
            string name = "DailyNewTips";

            var jsonValue = FileStore.ReadJson(name);
                
            mCustomNewTips.Clear();

            if (jsonValue == null)
                return;

            ParseJson(jsonValue);
        }

        public void SaveNewTipsDB()
        {
            string name = "DailyNewTips";

            FileStore.WriteJson(name, mCustomNewTips);

        }

        public DailyNewTips GetDailyNewTips(uint id)
        {
            mCustomNewTips.TryGetValue(id, out DailyNewTips tips);

            return tips;
        }

        public DailyNewTips PushDailyNewTips(uint id)
        {
            if (mCustomNewTips.ContainsKey(id) == false)
            {
                mCustomNewTips.Add(id, new DailyNewTips() { ID = id, isLimiteTips = false, time = Sys_Time.Instance.GetServerTime() });
            }

            return mCustomNewTips[id];
        }
        public void SetNewTipsNewLimite(uint nid,bool active)
        {
            var value = PushDailyNewTips(nid);

            if (value.isLimiteTips == active)
                return;

            value.isLimiteTips = active;

            //eventEmitter.Trigger<uint>(EEvents.NewTipsChange, nid);

        }

        public void SetNewTipsNewFunc(uint nid,bool active)
        {
            var value = PushDailyNewTips(nid);

            if (value.isFuncTips == active)
                return;

            //将新的状态赋值给对象
            value.isFuncTips = active;


            eventEmitter.Trigger<uint>(EEvents.NewTipsChange, nid);
        }

        public void SetNewTipsNewNotice(uint nid,bool active)
        {
            var value = PushDailyNewTips(nid);

            if (value.isNoctice == active)
                return;
            //将新的状态赋值给对象
            mCustomNewTips[nid].isNoctice = active;

            eventEmitter.Trigger<uint>(EEvents.NewTipsChange, nid);
        }

        public void CloseNewTips(uint nid)
        {
            if (mCustomNewTips.ContainsKey(nid) == false) 
                return;
            
            mCustomNewTips[nid].isFuncTips = false;
            mCustomNewTips[nid].isLimiteTips = false;
            mCustomNewTips[nid].isNoctice = false;

            eventEmitter.Trigger<uint>(EEvents.NewTipsChange, nid);
        }
        /// <summary>
        /// 用于判断显示 ’新‘标签
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool isNewTips(uint id)
        {
            DailyNewTips value = null;

            mCustomNewTips.TryGetValue(id, out value);

            return value == null ? false : (value.isFuncTips /*|| value.isLimiteTips*/);
        }

        /// <summary>
        /// 用于判断显示 ’新‘标签
        /// </summary>
        /// <returns></returns>
        public bool HaveNewTips()
        {
            bool result = false;

            foreach (var kvp in mCustomNewTips)
            {
                bool isnewtips = ((kvp.Value.isFuncTips) && isTodayDaily(kvp.Value.ID));

                if (isnewtips)
                {
                    result = true;
                    break;
                }

            }
            return result;
        }

        public bool HaveNotice()
        {
            bool result = false;

            foreach (var kvp in mCustomNewTips)
            {
                bool isnewtips = ((kvp.Value.isNoctice) && isTodayDaily(kvp.Value.ID));

                if (isnewtips)
                {
                    result = true;
                    break;
                }

            }
            return result;
        }
    }


}
