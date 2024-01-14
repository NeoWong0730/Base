using Logic.Core;
using System.Json;
using Lib.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;
using System;

namespace Logic
{
    public class UpliftInfo
    {
        public List<UpliftOpenClose> upliftedInfosList = new List<UpliftOpenClose>();

        public void DeserializeObject(JsonObject jo)
        {
            JsonHeler.DeserializeObject(jo, this);

            if (jo.ContainsKey("upliftedInfosList"))
            {
                Sys_Uplifted.Instance.upliftedClosedDic.Clear();
                upliftedInfosList.Clear();
                JsonArray ja = (JsonArray)jo["upliftedInfosList"];
                foreach (var item in ja)
                {
                    UpliftOpenClose info = new UpliftOpenClose();
                    info.DeserializeObject((JsonObject)item);
                    uint nowtime = Sys_Time.Instance.GetServerTime();
                    uint datatime = info.closeTime;
                    bool isSameDay = Sys_Time.IsServerSameDay(nowtime, datatime);
                    if (isSameDay)
                    {
                        Sys_Uplifted.Instance.upliftedClosedDic[info.upliftId] = info;
                    }

                }
            }
        }
    }

    public class UpliftOpenClose
    {
        public uint upliftId;
        public uint closeTime;
        public bool isClose;

        public void DeserializeObject(JsonObject jo)
        {
            JsonHeler.DeserializeObject(jo, this);
        }
    }

    public class Sys_Uplifted : SystemModuleBase<Sys_Uplifted>
    {
        public UpliftInfo Info = new UpliftInfo();
        public Dictionary<uint, UpliftOpenClose> upliftedClosedDic = new Dictionary<uint, UpliftOpenClose>();
        public List<uint> upliftedList = new List<uint>();
        public int countBefore;
        public bool isFxShow;
        private Timer nextDayRefreshTimer;
        private TimeSpan ts = new TimeSpan();
        private List<uint> upliftedClosedOpenList = new List<uint>();

        private uint breakthroughLv;
        private uint maxLv;
        private DateTime nowDT;
        private DateTime nextDT;

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents
        {
            OnCloseUpliftItem,
            OnRefreshNextDayUpliftItem,
        }

        public int CheckupliftedIsShow()
        {
            var enumerator = CSVUplifted.Instance.GetAll().GetEnumerator();
            int count = 0;
            while (enumerator.MoveNext())
            {
                CSVUplifted.Data data = enumerator.Current;
                if (upliftedClosedDic.TryGetValue(data.id, out UpliftOpenClose openClose) && openClose!=null && openClose.isClose)
                {
                    continue;
                }
                switch (data.Type)
                {
                    case 1:
                        int id;
                        if (Sys_Skill.Instance.ExistedUpgradeRank(out id))
                        {
                            count++;
                        }
                        break;
                    case 2:
                        if (Sys_Attr.Instance.surplusPoint >= data.Parameter)
                        {
                            count++;
                        }
                        break;
                    case 3:
                        for (int i = 0; i < Sys_Pet.Instance.petsList.Count; ++i)
                        {
                            if (Sys_Pet.Instance.petsList[i].baseAttrs != null && Sys_Pet.Instance.petsList[i].baseAttrs.TryGetValue(EBaseAttr.SurplusPoint,out long num) && num >= data.Parameter)
                            {
                                count++;
                            }
                        }
                        break;
                    case 4:
                        if (Sys_Attr.Instance.hpPool <= data.Parameter)
                        {
                            count++;
                        }
                        break;
                    case 5:
                        if (Sys_Attr.Instance.mpPool <= data.Parameter)
                        {
                            count++;
                        }
                        break;
                    case 6:
                        uint vitalityMax = Sys_Vitality.Instance.GetMaxVitality();
                        if (Sys_Bag.Instance.GetItemCount(5) /(float) vitalityMax * 100 >= data.Parameter)
                        {
                            count++;
                        }
                        break;
                    case 7:
                        CSVExperienceLevel.Data experienceLevelData;
                        if (CSVExperienceLevel.Instance.TryGetValue(Sys_Experience.Instance.exPerienceLevel + 1, out experienceLevelData))
                        {
                            List<List<uint>> cost = experienceLevelData.cost;
                            uint costId01 = cost[0][0];
                            uint costCount01 = cost[0][1];
                            uint costId02 = cost[1][0];
                            uint costCount02 = cost[1][1];
                            breakthroughLv = Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildGameInfoNtf.KungFuSkillLv;
                            maxLv = Sys_Experience.Instance.GetMaxLv(breakthroughLv);
                            if (Sys_Experience.Instance.exPerienceLevel < maxLv && Sys_Bag.Instance.GetItemCount(costId01) >= costCount01 && Sys_Bag.Instance.GetItemCount(costId02) >= costCount02)
                            {
                                count++;
                            }
                        }
                        break;
                    case 8:
                        if (Sys_Experience.Instance.experiencePlanDatas.Count>0 && Sys_Experience.Instance.experiencePlanDatas[0].LeftPoint >= data.Parameter)
                        {
                            count++;
                        }
                        break;
                    case 9:
                        int id2;
                        if (Sys_Skill.Instance.ExistedUpgradeLevel(out id2))
                        {
                            count++;
                        }
                        break;
                    case 10:
                        if (Sys_LivingSkill.Instance.HasLivingSkillLevelUp())
                        {
                            count++;
                        }
                        break;
                    default:
                        break;
                }
            }

            return count;
        }

        public override void OnLogin()
        {
            string path = Application.persistentDataPath + string.Format("/{0}_Uplifts.txt", Sys_Role.Instance.RoleId.ToString());
            string jsonStr = JsonHeler.GetJsonStr(string.Format(path, Sys_Role.Instance.RoleId.ToString()));
            JsonObject json = FileStore.ReadJson("Uplifts.txt");
            if (json != null)
            {
                Sys_Uplifted.Instance.Info.DeserializeObject(json);
            }

            //设置第二天0点刷新已关闭提升
            uint nowtime = Sys_Time.Instance.GetServerTime();
            nowDT = Sys_Time.ConvertToDatetime(nowtime);
            nextDT = nowDT.AddDays(1).Date;
            ts = nextDT - nowDT;
            nextDayRefreshTimer?.Cancel();
            upliftedClosedOpenList.Clear();
            nextDayRefreshTimer = Timer.Register((float)ts.TotalSeconds, () => {
                foreach (var uplift in Sys_Uplifted.Instance.upliftedClosedDic)
                {
                    if (uplift.Value.isClose)
                    {
                        upliftedClosedOpenList.Add(uplift.Key);
                    }
                }
                for(int i=0;i< upliftedClosedOpenList.Count; ++i)
                {
                    if (Sys_Uplifted.Instance.upliftedClosedDic.ContainsKey(upliftedClosedOpenList[i]))
                    {
                        Sys_Uplifted.Instance.upliftedClosedDic.Remove(upliftedClosedOpenList[i]);
                    }
                }
            },null,false,true);
        }

        public override void OnLogout()
        {
            nextDayRefreshTimer?.Cancel();
            upliftedClosedOpenList.Clear();
            upliftedClosedDic.Clear();
            countBefore = 0;
            isFxShow = false;
    }

        public override void Init()
        {
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, true);
        }

        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            //服务器时间调整 重置第二天0点刷新已关闭提升
            nowDT = Sys_Time.ConvertToDatetime(newTime);
            if (nextDT <= nowDT)
            {
                upliftedClosedOpenList.Clear();
                foreach (var uplift in Sys_Uplifted.Instance.upliftedClosedDic)
                {
                    if (uplift.Value.isClose)
                    {
                        upliftedClosedOpenList.Add(uplift.Key);
                    }
                }
                for (int i = 0; i < upliftedClosedOpenList.Count; ++i)
                {
                    if (Sys_Uplifted.Instance.upliftedClosedDic.ContainsKey(upliftedClosedOpenList[i]))
                    {
                        Sys_Uplifted.Instance.upliftedClosedDic.Remove(upliftedClosedOpenList[i]);
                    }
                }
                nextDayRefreshTimer?.Cancel();
            }
            nextDT = nowDT.AddDays(1).Date;
            ts = nextDT - nowDT;
            nextDayRefreshTimer?.Cancel();
            nextDayRefreshTimer = Timer.Register((float)ts.TotalSeconds, () => {
                upliftedClosedOpenList.Clear();
                foreach (var uplift in Sys_Uplifted.Instance.upliftedClosedDic)
                {
                    if (uplift.Value.isClose)
                    {
                        upliftedClosedOpenList.Add(uplift.Key);
                    }
                }
                for (int i = 0; i < upliftedClosedOpenList.Count; ++i)
                {
                    if (Sys_Uplifted.Instance.upliftedClosedDic.ContainsKey(upliftedClosedOpenList[i]))
                    {
                        Sys_Uplifted.Instance.upliftedClosedDic.Remove(upliftedClosedOpenList[i]);
                    }
                }
                uint nowTime = Sys_Time.Instance.GetServerTime();
                nowDT = Sys_Time.ConvertToDatetime(nowTime);
                nextDT = nowDT.AddDays(1).Date;
                Sys_Uplifted.Instance.eventEmitter.Trigger(Sys_Uplifted.EEvents.OnRefreshNextDayUpliftItem);
                nextDayRefreshTimer.Cancel();
            },null,false,true);
            Sys_Uplifted.Instance.eventEmitter.Trigger(Sys_Uplifted.EEvents.OnRefreshNextDayUpliftItem);
        }
    }
}
