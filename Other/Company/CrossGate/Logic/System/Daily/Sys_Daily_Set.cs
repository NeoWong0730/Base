using System;
using System.Collections.Generic;
using Logic.Core;
using Net;
using Packet;
using SQLite4Unity3d;

namespace Logic
{

    // 红点设置的保存与读取

    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {
        public class DailyRedSet
        {
            [PrimaryKey]
            public uint ID { get; set; }
           
            public bool State { get; set; }


        }

        private Dictionary<uint, DailyRedSet> mRedSet = new Dictionary<uint, DailyRedSet>(); //数据库原始数据

        private Dictionary<uint, DailyRedSet> mCustomRedSet = new Dictionary<uint, DailyRedSet>();//用户数据

        private DataService mDataService;

      
        private void CreateDB()
        {
            mDataService.CreateTable(typeof(DailyRedSet));
        }

        private  void LoadDB()
        {
            mDataService = new DataService("DailyRedSet.db");

            CreateDB();

            var table = mDataService.GetTableRuntime();

            var datas = table.ToSearch<DailyRedSet>();

            mRedSet.Clear();
            mCustomRedSet.Clear();
            int count = datas.Count;

            for (int i = 0; i < count; i++)
            {
                mRedSet.Add(datas[i].ID, datas[i]);
                mCustomRedSet.Add(datas[i].ID, new DailyRedSet() { ID = datas[i].ID, State = datas[i].State });
            }
        }

        public void LoadRedSet()
        {
            LoadDB();
        }
        public void SaveRedSet()
        {
            foreach (var kvp in mCustomRedSet)
            {
                if (mRedSet.ContainsKey(kvp.Key) && mRedSet[kvp.Key].State != kvp.Value.State)
                    mDataService.Update(kvp.Value, typeof(DailyRedSet));
                else if (mRedSet.ContainsKey(kvp.Key) == false)
                    mDataService.InsertTable(kvp.Value, typeof(DailyRedSet));
            }

           
              
            mDataService.Close();

        }

        public void SetRedset(uint nid, bool nstate)
        {
            if (mCustomRedSet.ContainsKey(nid) == false)
            {
                mCustomRedSet.Add(nid, new DailyRedSet() { ID = nid, State = nstate });
                return;
            }

            mCustomRedSet[nid].State = nstate;

        }

        public DailyRedSet getRedSet(uint id)
        {
            DailyRedSet value = null;

            mCustomRedSet.TryGetValue(id, out value);

            return value;
        }
    }


}
