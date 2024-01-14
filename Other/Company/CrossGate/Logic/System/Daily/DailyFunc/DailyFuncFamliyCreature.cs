using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;

namespace Logic
{

    public class DailyFuncFamliyCreature : DailyFunc
    {

        public override bool OnJoin()
        {
            if (Sys_Family.Instance.familyData.isInFamily)
            {
                UI_FamilyOpenParam openParam = new UI_FamilyOpenParam()
                {
                    familyMenuEnum = (int)UI_Family.EFamilyMenu.Activity, // 0 - 3 大厅 家族成员 家族建筑， 家族活动
                    activeId = 60 // 家族活动表id 
                };

                Sys_Family.Instance.OpenUI_Family(openParam);

                return base.OnJoin();
            }
            return false;
        }
    }

    public class DailyFuncFamliyMonster: DailyFuncFamliyCreature
    {
        private uint CustomStartTime = 0;
        public override void Init()
        {
            //Data = CSVDailyActivity.Instance.GetConfData(DailyID);

            //UpdataDailyType();

            //ReadNoticDatas();
            //ReadOpenDatas();

            base.Init();

            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnGetFamilyPetInfo,OnCustomOpentime,true);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnSetTrainInfoEnd, OnCustomOpentime, true);


        }


        public override bool InitTimeWork()
        {
            if (isUnLock() == false || IsInOpenDay() == false)
                return false;

            var info = Sys_Family.Instance.GetTrainInfo();

            if (Sys_Family.Instance.familyData.isInFamily)
                Sys_Daily.Instance.GetDailyTimesForSevers(DailyID);

            if (info == null || info.StartTime == 0)
            {
                return base.InitTimeWork();
            }

           // DebugUtil.LogError("Init Time Work  DailyFuncFamliyMonster : " + Data.id.ToString());

            ReadCustomOpenTime();

            bool result = UpdataTimeWorke();

            CustomStartTime = info.StartTime;

            return result;
        }
        private void ReadCustomOpenTime()
        {
            Sys_Ini.Instance.Get<IniElement_Int>(1264, out IniElement_Int durationTime);

            var info = Sys_Family.Instance.GetTrainInfo();

            if (info == null)
                return;

            m_OpenDatas.Clear();

            int noticecount = m_Notice.Count;

            for (int i = 0; i < noticecount; i++)   
            {
                OnNoticeEnd((uint)i);
            }

            m_Notice.Clear();

            

            DailyOpenData opentime = new DailyOpenData()
            {
                ID = DailyID,
                OpenTime = (int)info.StartTime,
                EndTime = (int)info.StartTime + durationTime.value
            };

            m_OpenDatas.Add(opentime);


            if (Data.Playing > 0)
            {
                DailyNotice notice = new DailyNotice { ID = DailyID, StartNoticeTime = (int)info.StartTime + 1, EndNoticeTime = (int)info.StartTime + (int)Data.Playing , Type = 1 };
                m_Notice.Add(notice);
            }

            if (Data.NoticeTime != null && Data.NoticeTime.Count > 0)
            {
                var noticetime = Data.NoticeTime[0][0] * 3600 + Data.NoticeTime[0][1] * 60;
                var opentime0 = Data.OpeningTime[0][0] * 3600 + Data.OpeningTime[0][1] * 60;

                var noticetimeoffset = opentime0 - noticetime;

                if (noticetimeoffset > 0)
                {
                    var starttime = (int)info.StartTime - noticetimeoffset;
                    var endtime = starttime + (int)Data.NoticeLong;

                    DailyNotice notice = new DailyNotice { ID = DailyID, StartNoticeTime = starttime, EndNoticeTime = endtime,Type = 0};

                    m_Notice.Add(notice);
                }
                 
            }
        }


        private bool UpdataTimeWorke()
        {
            Sys_Daily.Instance.RemoveTimeWork(Data.id);

            bool reslut = UpdataOpenTimeWork();
            bool result0 = UpdataNoctionTimeWork();

            return reslut || result0;

        }


        private bool UpdataOpenTimeWork()
        {
            int opendatacount = m_OpenDatas.Count;

            var nowtimetoday = Sys_Time.Instance.GetServerTime() % 86400;

            bool result = false;

            var TodayZeroTimePoint = Sys_Daily.Instance.mTodayZeroTimePoint;

            for (int i = 0; i < opendatacount; i++)
            {
                if (nowtimetoday < m_OpenDatas[i].OpenTime)
                {
                    Sys_Daily.Instance.InsertTimeWork((uint)((uint)TodayZeroTimePoint + m_OpenDatas[i].OpenTime), i, Data.id, Sys_Daily.EDailyTimerWorkType.Open);
                    result = true;
                }

                if (nowtimetoday < m_OpenDatas[i].EndTime)
                {
                    Sys_Daily.Instance.InsertTimeWork((uint)((uint)TodayZeroTimePoint + m_OpenDatas[i].EndTime), i, Data.id, Sys_Daily.EDailyTimerWorkType.Close);
                    result = true;


                    if (Data.id == 101)
                        Sys_Daily.Instance.InsertTimeWork((uint)((uint)TodayZeroTimePoint + (uint)m_OpenDatas[i].EndTime + 2), i, Data.id, Sys_Daily.EDailyTimerWorkType.SendDataToServer);
                }

                if (nowtimetoday >= m_OpenDatas[i].OpenTime && nowtimetoday < m_OpenDatas[i].EndTime)
                    OnOpenTime((uint)i);
            }

            return result;
        }


        private bool UpdataNoctionTimeWork()
        {
            int count = m_Notice.Count;

            var nowtimetoday = Sys_Time.Instance.GetServerTime() % 86400;

            bool result = false;

            var TodayZeroTimePoint = Sys_Daily.Instance.mTodayZeroTimePoint;


            for (int i = 0; i < count; i++)
            {

                if (m_Notice[i].StartNoticeTime + (uint)TodayZeroTimePoint <= Sys_Daily.Instance.ClearNociteTime)
                    continue;

                if (nowtimetoday < m_Notice[i].StartNoticeTime)
                {
                    Sys_Daily.Instance.InsertTimeWork((uint)m_Notice[i].StartNoticeTime + (uint)TodayZeroTimePoint, i, Data.id, Sys_Daily.EDailyTimerWorkType.PreTips);
                    result = true;
                }

                if (nowtimetoday < m_Notice[i].EndNoticeTime)
                {
                    Sys_Daily.Instance.InsertTimeWork((uint)m_Notice[i].EndNoticeTime + (uint)TodayZeroTimePoint, i, Data.id, Sys_Daily.EDailyTimerWorkType.PreTipsClose);
                    result = true;
                }

                if (nowtimetoday >= m_Notice[i].StartNoticeTime && nowtimetoday < m_Notice[i].EndNoticeTime)
                    OnNotice((uint)i);
            }

            return result;
        }
        private void OnCustomOpentime()
        {
            if (Sys_FunctionOpen.Instance.IsOpen(Sys_Daily.Instance.DailyFuncOpenID) == false)
                return;

            if (isUnLock() == false || IsInOpenDay() == false)
                return;

            var info = Sys_Family.Instance.GetTrainInfo();

            if (  info == null || info.StartTime == 0 || CustomStartTime == info.StartTime)
            {
                return;
            }

            ReadCustomOpenTime();

            bool result = UpdataTimeWorke();

            CustomStartTime = info.StartTime;


            //DebugUtil.LogError(" famliy creature custom open time");
         }
    }
}
