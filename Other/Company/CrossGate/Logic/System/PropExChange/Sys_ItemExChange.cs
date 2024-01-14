using Framework;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Json;
using Table;
using UnityEngine;

namespace Logic
{
    public partial class Sys_ItemExChange : SystemModuleBase<Sys_ItemExChange>, ISystemModuleUpdate
    {
        public string m_StoreFilePath = "ItemExChange";

        public CSVOperationalActivityRuler.Data curCSVActivityRulerData;

        public Dictionary<uint, List<CSVExchangeItem.Data>> m_ClientDatas = new Dictionary<uint, List<CSVExchangeItem.Data>>();

        public List<ExData> exDatas = new List<ExData>();

        private OpenRedInfo m_OpenRedInfo = new OpenRedInfo();

        public uint startTime;

        public uint endTime;

        private bool b_IsActivity;
        public bool isActivity
        {
            get
            {
                return b_IsActivity;
            }
            set
            {
                if (b_IsActivity != value)
                {
                    b_IsActivity = value;
                    if (!b_IsActivity)
                    {
                        UIManager.CloseUI(EUIID.UI_Activity_Exchange);
                    }
                }
            }
        }

        private uint m_ActivityId;

        public uint activityId
        {
            get
            {
                return m_ActivityId;
            }
            set
            {
                if (m_ActivityId != value)
                {
                    if (m_ActivityId != 0)
                    {
                        UIManager.CloseUI(EUIID.UI_Activity_Exchange);
                    }
                    m_ActivityId = value;
                }
            }
        }

        public uint refreshTime;

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            e_RefreshEx,//更新兑换次数
            e_UpdateRedState,//更新红点状态
            e_AcrossDay_Ex,//道具兑换跨天
        }

        public override void Init()
        {
            ProcessEvents(true);
            ParseClientData();
        }

        private void ParseClientData()
        {
            for (int i = 0; i < CSVExchangeItem.Instance.GetAll().Count; i++)
            {
                CSVExchangeItem.Data data = CSVExchangeItem.Instance.GetByIndex(i);
                if (!m_ClientDatas.TryGetValue(data.Activity_Id, out List<CSVExchangeItem.Data> datas))
                {
                    datas = new List<CSVExchangeItem.Data>();
                    m_ClientDatas.Add(data.Activity_Id, datas);
                }
                datas.Add(data);
            }
        }

        public override void Dispose()
        {
            ProcessEvents(false);
        }

        private void ProcessEvents(bool register)
        {
            if (register)
            {
                //EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivityDataReq, (ushort)CmdActivityRuler.CmdActivityDataNtf, ActivityDataNtf, CmdActivityDataNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivityExchangeTakeReq, (ushort)CmdActivityRuler.CmdActivityExchangeTakeRes, ActivityExchangeTakeRes, CmdActivityExchangeTakeRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivityExchangeTake2Req, (ushort)CmdActivityRuler.CmdActivityExchangeTake2Res, ActivityHeFuExchangeTakeRes, CmdActivityExchangeTake2Res.Parser);
                Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, true);
            }
            else
            {
                //EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivityDataNtf, ActivityDataNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivityExchangeTakeRes, ActivityExchangeTakeRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivityExchangeTake2Res, ActivityHeFuExchangeTakeRes);
                Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, false);
            }
            Sys_ActivityOperationRuler.Instance.eventEmitter.Handle(Sys_ActivityOperationRuler.EEvents.OnRefreshActivityInfo, CheckActivityTime, register);
        }

        private void CheckActivityTime()
        {
            ActivityInfo activityInfo = Sys_ActivityOperationRuler.Instance.GetActivityInfo(EActivityRulerType.ItemExChange);
            if (activityInfo != null)
            {
                isActivity = true;
                curCSVActivityRulerData = activityInfo.csvData;
                activityId = curCSVActivityRulerData.id;
                exDatas.Clear();

                if (m_ClientDatas.TryGetValue(curCSVActivityRulerData.id, out List<CSVExchangeItem.Data> _datas))
                {
                    for (int i = 0; i < _datas.Count; i++)
                    {
                        ExData exData = new ExData(_datas[i].id, 0);
                        exDatas.Add(exData);
                    }
                }

                if (curCSVActivityRulerData != null)
                {
                    if (curCSVActivityRulerData.Activity_Type == 3)//开服
                    {
                        startTime = Sys_Role.Instance.openServiceGameTime;
                        endTime = startTime + (curCSVActivityRulerData.Duration_Day) * 86400;
                    }
                    else if (curCSVActivityRulerData.Activity_Type == 2)//限时
                    {
                        startTime = curCSVActivityRulerData.Begining_Date + (uint)TimeManager.TimeZoneOffset;
                        endTime = startTime + (curCSVActivityRulerData.Duration_Day) * 86400;
                    }

                    DebugUtil.LogFormat(ELogType.eOperationActivity, "道具兑换: activityId: {0},curDay:{1}", curCSVActivityRulerData.id, activityInfo.currDay);
                }
            }
            else
            {
                isActivity = false;
                curCSVActivityRulerData = null;
                activityId = 0;
            }

            CheckHeFuActivityTime();
        }

        public class OpenRedData
        {
            public uint id;
            public bool openRed;

            public void DeserializeObject(JsonObject jo)
            {
                JsonHeler.DeserializeObject(jo, this);
            }
        }

        public class OpenRedInfo
        {
            public List<OpenRedData> openRedDatas = new List<OpenRedData>();

            public void DeserializeObject(JsonObject jo)
            {
                JsonHeler.DeserializeObject(jo, this);

                if (jo.ContainsKey("openRedDatas"))
                {
                    openRedDatas.Clear();
                    JsonArray ja = (JsonArray)jo["openRedDatas"];
                    foreach (var item in ja)
                    {
                        OpenRedData data = new OpenRedData();
                        data.DeserializeObject((JsonObject)item);
                        openRedDatas.Add(data);
                    }
                }
            }

            public OpenRedData TryGet(uint id)
            {
                for (int i = 0; i < openRedDatas.Count; i++)
                {
                    if (openRedDatas[i].id == id)
                    {
                        return openRedDatas[i];
                    }
                }
                return null;
            }


        }

        public void LoadedMemory()
        {
            m_OpenRedInfo.openRedDatas.Clear();
            JsonObject json = FileStore.ReadJson(m_StoreFilePath);
            if (json != null)
            {
                m_OpenRedInfo.DeserializeObject(json);
            }
        }

        public void SaveMemory()
        {
            m_OpenRedInfo.openRedDatas.Clear();
            for (int i = 0; i < exDatas.Count; i++)
            {
                OpenRedData openRedData = new OpenRedData();
                openRedData.id = exDatas[i].id;
                openRedData.openRed = exDatas[i].openRed;
                m_OpenRedInfo.openRedDatas.Add(openRedData);
            }
            FileStore.WriteJson(m_StoreFilePath, m_OpenRedInfo);
        }


        public void UpdateExData(ACExchangeData aCExchangeData)
        {
            if (aCExchangeData == null || !b_IsActivity)
            {
                return;
            }
            for (int i = 0; i < aCExchangeData.Ids.Count; i++)
            {
                uint exId = aCExchangeData.Ids[i];

                ExData exData = TryGetExData(exId);

                if (exData != null)
                {
                    exData.exNum = aCExchangeData.ExNums[i];
                }

                DebugUtil.LogFormat(ELogType.eOperationActivity, "UpdateExData ==> id:{0},exNum:{1}", exId, exData.exNum);
            }

            LoadedMemory();

            for (int i = 0; i < exDatas.Count; i++)
            {
                OpenRedData openRedData = m_OpenRedInfo.TryGet(exDatas[i].id);
                if (openRedData != null)
                {
                    exDatas[i].openRed = openRedData.openRed;
                }
            }

            UpdateExItemState();
            eventEmitter.Trigger(EEvents.e_RefreshEx);
        }

        public void ActivityExchangeTakeReq(uint id)
        {
            CmdActivityExchangeTakeReq req = new CmdActivityExchangeTakeReq();
            req.ExId = id;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityExchangeTakeReq, req);
        }

        private void ActivityExchangeTakeRes(NetMsg netMsg)
        {
            CmdActivityExchangeTakeRes res = NetMsgUtil.Deserialize<CmdActivityExchangeTakeRes>(CmdActivityExchangeTakeRes.Parser, netMsg);

            ExData exData = TryGetExData(res.ExId);

            if (exData != null)
            {
                exData.exNum = res.ExNum;
                DebugUtil.LogFormat(ELogType.eOperationActivity, "ActivityExchangeTakeRes ==> id:{0},exNum:{1}", res.ExId, res.ExNum);
            }

            UpdateExItemState();
            eventEmitter.Trigger(EEvents.e_RefreshEx);
        }

        public ExData TryGetExData(uint exId)
        {
            for (int i = 0; i < exDatas.Count; i++)
            {
                if (exDatas[i].id == exId)
                {
                    return exDatas[i];
                }
            }
            DebugUtil.LogErrorFormat($"exId not found {exId}");
            return null;
        }

        private void OnRefreshChangeData(int a,int b)
        {
            UpdateExItemState();
            UpdateHeFuExItemState();
            eventEmitter.Trigger(EEvents.e_RefreshEx);
        }

        public void UpdateExItemState()
        {
            for (int i = 0; i < exDatas.Count; i++)
            {
                exDatas[i].UpdateExRedState();
            }
        }


        public bool hasRed()
        {
            for (int i = 0; i < exDatas.Count; i++)
            {
                if (exDatas[i].canEx)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnUpdate()
        {
            if (isActivity)
            {
                if (Sys_Time.Instance.GetServerTime() > endTime)
                {
                    isActivity = false;
                }
            }
            if (isHeFuActivity)
            {
                if (Sys_Time.Instance.GetServerTime() > endHeFuTime)
                {
                    isHeFuActivity = false;
                }
            }
        }

        public class ExData
        {
            public uint id;

            public CSVExchangeItem.Data csv;

            private uint m_ExNum;

            public uint exNum
            {
                get
                {
                    return m_ExNum;
                }
                set
                {
                    if (m_ExNum != value)
                    {
                        m_ExNum = value;
                        UpdateExRedState();
                        Sys_ItemExChange.Instance.eventEmitter.Trigger(EEvents.e_RefreshEx, id);
                    }
                }
            }

            private bool b_OpenRed;

            public bool openRed
            {
                get
                {
                    return b_OpenRed;
                }
                set
                {
                    if (b_OpenRed != value)
                    {
                        b_OpenRed = value;
                        UpdateExRedState();
                        Sys_ItemExChange.Instance.eventEmitter.Trigger(EEvents.e_RefreshEx, id);
                    }
                }
            }

            private bool b_CanEx;

            public bool canEx
            {
                get
                {
                    return b_CanEx;
                }
                set
                {
                    if (b_CanEx != value)
                    {
                        b_CanEx = value;
                        Sys_ItemExChange.Instance.eventEmitter.Trigger(EEvents.e_UpdateRedState);
                    }
                }
            }

            public bool isCountEnough;

            public void UpdateExRedState()
            {
                isCountEnough = true;

                int needCount = csv.Activity_Item.Count;

                for (int i = 0; i < needCount; i++)
                {
                    uint itemId = csv.Activity_Item[i][0];

                    uint needItemCount = csv.Activity_Item[i][1];

                    uint ownerCount = (uint)Sys_Bag.Instance.GetItemCount(itemId);

                    if (ownerCount < needItemCount)
                    {
                        isCountEnough = false;
                        break;
                    }
                }

                if (csv.Limit_Type == 0)//没有限制
                {
                    canEx = isCountEnough & openRed;
                }
                else
                {
                    bool b_Extra = exNum < csv.Limit_Max;

                    canEx = b_Extra & isCountEnough & openRed;
                }
            }

            public ExData(uint id, uint exNum)
            {
                this.id = id;
                this.exNum = exNum;

                this.csv = CSVExchangeItem.Instance.GetConfData(id);
            }
        }
    }
}


