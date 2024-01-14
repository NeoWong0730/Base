using Logic.Core;
using System.Collections.Generic;
using Table;

namespace Logic
{ 
    public class VitalityData
    {
        public uint key;   
        public bool isFinish;

        public VitalityData(uint _key, bool _isFinish)
        {
            key = _key;
            isFinish = _isFinish;
        }
    }

    public class Sys_Vitality : SystemModuleBase<Sys_Vitality>
    {
        private List<VitalityData> listGet = new List<VitalityData>();
        private List<VitalityData> listGetFinish = new List<VitalityData>();
        private List<VitalityData> listChange = new List<VitalityData>();
        private List<CSVDailyActivity.Data>  dailyActivityDatas = new List<CSVDailyActivity.Data>();

        public List<VitalityData> SetVitalityGetList()
        {
            listGet.Clear();
            listGetFinish.Clear();
            dailyActivityDatas.Clear();
            var typeDic = CSVDailyActivityShow.Instance.GetAll();
            foreach (var kvp in typeDic)
            {
                var templist = Sys_Daily.Instance.getTodayUsefulDailies(kvp.id);
                if (templist != null)
                    dailyActivityDatas.AddRange(templist);
            }
            foreach (var data in CSVVitality.Instance.GetAll())
            {
                if (data.Way[0] <= 100 && Sys_Role.Instance.Role.Level >= data.Level[0] && Sys_Role.Instance.Role.Level <= data.Level[1])   //1-100获取途径
                {
                    if (data.Way[0] == 1)
                    {
                        CSVDailyActivity.Data cSVDailyActivity = CSVDailyActivity.Instance.GetConfData(data.Way[1]);
                        if (dailyActivityDatas.Contains(cSVDailyActivity))
                        {
                            if (Sys_Daily.Instance.isDailyMaxActivityNum(data.Way[1]))
                            {
                                VitalityData vitalityData = new VitalityData(data.id,  true);
                                if (!listGetFinish.Contains(vitalityData))
                                    listGetFinish.Add(vitalityData);
                            }
                            else
                            {
                                VitalityData vitalityData = new VitalityData( data.id, false);
                                if (!listGet.Contains(vitalityData))
                                    listGet.Add(vitalityData);
                            }
                        }
                    }
                    else if (data.Way[0] == 2)
                    {
                        VitalityData vitalityData = new VitalityData(data.id, false);
                        if (!listGet.Contains(vitalityData))
                            listGet.Add(vitalityData);
                    }
                }
            }
            listGet.AddRange(listGetFinish);
            return listGet;
        }

        public List<VitalityData> SetVitalityChangetList()
        {
            listChange.Clear();
            foreach (var data in CSVVitality.Instance.GetAll())
            {
                if (data.Way[0] > 100 && Sys_Role.Instance.Role.Level >= data.Level[0] && Sys_Role.Instance.Role.Level <= data.Level[1])     //101-200消耗途径
                {
                    VitalityData vitalityData = new VitalityData(data.id, false);
                    listChange.Add(vitalityData);
                }
            }
            return listChange;
        }

        public override void OnLogin()
        {
            base.OnLogin();
        }

        public override void OnLogout()
        {
            listGet.Clear();
            listGetFinish.Clear();
            listChange.Clear();
            dailyActivityDatas.Clear();
        }

        public uint GetMaxVitality()
        {
            uint maxAddByAdventureLv = 0;
            if (CSVAdventureLevel.Instance.TryGetValue(Sys_Adventure.Instance.Level, out CSVAdventureLevel.Data csvData))
            {
                if (csvData.addPrivilege != null)
                {
                    for (int i = 0; i < csvData.addPrivilege.Count; ++i)
                    {
                        if (csvData.addPrivilege[i][0] == 1)
                        {
                            maxAddByAdventureLv = csvData.addPrivilege[i][1];
                            break;
                        }
                    }
                }
            }
            return CSVCharacterAttribute.Instance.GetConfData(Sys_Role.Instance.Role.Level).VitalityLimit + maxAddByAdventureLv;
        }
    }
}
