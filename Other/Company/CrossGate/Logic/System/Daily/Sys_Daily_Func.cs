using System;
using System.Collections.Generic;
using Logic.Core;
using Net;
using Packet;
using Table;
namespace Logic
{
    public partial class Sys_Daily
    {
        public Dictionary<uint, DailyFunc> m_DailyFuncDic = new Dictionary<uint, DailyFunc>();

        public List<uint> NoticeList { get; private set; } = new List<uint>();

        private void InitFunc(uint dailyId,uint dailytype)
        {
            if (m_DailyFuncDic.ContainsKey(dailyId))
                return;

            var funcObj = CreateFuncObject(dailyId, dailytype);
   
            m_DailyFuncDic.Add(dailyId, funcObj);

            funcObj.Init();
        }

        private bool InitFuncTimeWork(uint dailyid)
        {
            if (m_DailyFuncDic.TryGetValue(dailyid, out DailyFunc dailyFunc) == false)
                return false;

            return dailyFunc.InitTimeWork(); 

        }
        private void InitFuncTimeWork()
        {
            bool result = false;

            foreach (var kvp in m_DailyFuncDic)
            {
                if (kvp.Value.InitTimeWork())
                {
                    result = true;
                }
            }

            if (result)
            {
                eventEmitter.Trigger(EEvents.NewNotice);
            }
        }
        private DailyFunc CreateFuncObject(uint dailyId,uint dailytype)
        {
            DailyFunc dailyFunc = null;

            switch (dailyId)
            {
                case 70u:
                    dailyFunc = new DailyFuncSinglepvp();
                    break;
                case 90u:
                    dailyFunc = new DailyFuncGoddnessTrail();
                    break;
                case 100u:
                    dailyFunc = new DailyFuncHundredPeopleArea();
                    break;
                case 110u:
                    dailyFunc = new DailyFuncBuildFamliy();
                    break;
                case 111u:
                    dailyFunc = new DailyFuncFamliyParty();
                    break;
                case 112u:
                    dailyFunc = new DailyFuncBullDemon();
                    break;
                case 114:
                    dailyFunc = new DailyFuncFamliyCreature();
                    break;
                case 115:
                    dailyFunc = new DailyFuncFamliyMonster();
                    break;
                case 240:
                case 250:
                    dailyFunc = new BossTowerDailyFunc();
                    break;
                default:
                    {
                        if (Sys_WorldBoss.Instance.TryGetActivityIdByDailyId(dailyId, out var actitityId))
                        {
                            dailyFunc = new DailyFuncWorldBoss();
                        }
                        else
                        {
                            dailyFunc = new DailyFunc();
                        }
                      
                    }
                    break;
            }
            dailyFunc.DailyID = dailyId;
            return dailyFunc;
        }

       


        public DailyFunc GetDailyFunc(uint dailyid)
        {
            m_DailyFuncDic.TryGetValue(dailyid, out DailyFunc dailyFunc);

            return dailyFunc;
        }


        public void AddNotice(uint id)
        {
            NoticeList.Add(id);
        }

        public void RemoveNotice(uint id)
        {
            NoticeList.Remove(id);

        }

        public void ClearNotice()
        {
            NoticeList.Clear();

            eventEmitter.Trigger(EEvents.RemoveNotice);
        }

        public DailyNotice GetDailyNotice(uint noticeKey)
        {
            uint dailyid = noticeKey >> 16;
            uint index = noticeKey & (ushort.MaxValue);

            var dailyfunc = GetDailyFunc(dailyid);

            if (dailyfunc == null)
                return null;

            return dailyfunc.GetDailyNotice((int)index);
        }
    }
}
