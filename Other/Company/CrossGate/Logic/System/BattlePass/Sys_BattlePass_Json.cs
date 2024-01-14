using System;
using System.Collections.Generic;
using System.Json;
using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public partial class Sys_BattlePass : SystemModuleBase<Sys_BattlePass>
    {

        class JsonValue
        {
            public bool IsFristActive = false;

            public uint fristtime = 0;
        }

        readonly string jsonFileName = "BattlePass";

        private uint FristEnterTime = 0;
        public void LoadJson()
        {

            var jsonValue = FileStore.ReadJson(jsonFileName);

            if (jsonValue == null)
                return;


            JsonValue cusjv = new JsonValue();

            JsonHeler.DeserializeObject(jsonValue, cusjv);



            FristEnterTime = cusjv.fristtime;

        }

        public void SaveJson()
        {

            JsonValue jsonValue = new JsonValue();

            jsonValue.IsFristActive = true;

            jsonValue.fristtime = FristEnterTime;

            FileStore.WriteJson(jsonFileName, jsonValue);
        }

        public void SetFristEnterTime()
        {
            FristEnterTime = Sys_Time.Instance.GetServerTime();

            SaveJson();
        }
    }
}
