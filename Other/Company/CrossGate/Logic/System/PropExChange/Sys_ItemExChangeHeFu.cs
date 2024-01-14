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
    //合服相关逻辑写在这边
    public partial class Sys_ItemExChange : SystemModuleBase<Sys_ItemExChange>, ISystemModuleUpdate
    {
        public string m_HeFuStoreFilePath = "HeFuItemExChange";

        public CSVOperationalActivityRuler.Data curHeFuCSVActivityRulerData;

        public List<ExData> exHeFuDatas = new List<ExData>();

        private OpenRedInfo m_HeFuOpenRedInfo = new OpenRedInfo();

        public uint startHeFuTime;

        public uint endHeFuTime;

        private bool b_IsHeFuActivity;
        public bool isHeFuActivity
        {
            get
            {
                return b_IsHeFuActivity;
            }
            set
            {
                if (b_IsHeFuActivity != value)
                {
                    b_IsHeFuActivity = value;
                    if (!b_IsHeFuActivity)
                    {
                        UIManager.CloseUI(EUIID.UI_Activity_Exchange_HeFu);
                    }
                }
            }
        }

        private uint m_ActivityHeFuId;

        public uint activityHeFuId
        {
            get
            {
                return m_ActivityHeFuId;
            }
            set
            {
                if (m_ActivityHeFuId != value)
                {
                    if (m_ActivityHeFuId != 0)
                    {
                        UIManager.CloseUI(EUIID.UI_Activity_Exchange_HeFu);
                    }
                    m_ActivityHeFuId = value;
                }
            }
        }

        #region 服务器消息
        public void ActivityHeFuExchangeTakeReq(uint id)
        {
            CmdActivityExchangeTake2Req req = new CmdActivityExchangeTake2Req();
            req.ExId = id;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityExchangeTake2Req, req);
        }

        private void ActivityHeFuExchangeTakeRes(NetMsg netMsg)
        {
            CmdActivityExchangeTake2Res res = NetMsgUtil.Deserialize<CmdActivityExchangeTake2Res>(CmdActivityExchangeTake2Res.Parser, netMsg);

            ExData exData = TryGetExHeFuData(res.ExId);

            if (exData != null)
            {
                exData.exNum = res.ExNum;
                DebugUtil.LogFormat(ELogType.eOperationActivity, "ActivityHeFuExchangeTakeRes ==> id:{0},exNum:{1}", res.ExId, res.ExNum);
            }

            UpdateHeFuExItemState();
            eventEmitter.Trigger(EEvents.e_RefreshEx);
        }
        #endregion
        private void CheckHeFuActivityTime()
        {
            ActivityInfo activityInfo = Sys_ActivityOperationRuler.Instance.GetActivityInfo(EActivityRulerType.HeFuItemExChange);
            if (activityInfo != null)
            {
                isHeFuActivity = true;
                curHeFuCSVActivityRulerData = activityInfo.csvData;
                activityHeFuId = curHeFuCSVActivityRulerData.id;
                exHeFuDatas.Clear();

                if (m_ClientDatas.TryGetValue(curHeFuCSVActivityRulerData.id, out List<CSVExchangeItem.Data> _datas))
                {
                    for (int i = 0; i < _datas.Count; i++)
                    {
                        ExData exData = new ExData(_datas[i].id, 0);
                        exHeFuDatas.Add(exData);
                    }
                }

                if (curHeFuCSVActivityRulerData != null)
                {
                    if (curHeFuCSVActivityRulerData.Activity_Type == 3)//开服
                    {
                        startHeFuTime = Sys_Role.Instance.openServiceGameTime;
                        endHeFuTime = startHeFuTime + (curHeFuCSVActivityRulerData.Duration_Day) * 86400;
                    }
                    else if (curHeFuCSVActivityRulerData.Activity_Type == 2)//限时
                    {
                        startHeFuTime = curHeFuCSVActivityRulerData.Begining_Date + (uint)TimeManager.TimeZoneOffset;
                        endHeFuTime = startHeFuTime + (curHeFuCSVActivityRulerData.Duration_Day) * 86400;
                    }

                    DebugUtil.LogFormat(ELogType.eOperationActivity, "合服道具兑换: activityId: {0},curDay:{1}", curHeFuCSVActivityRulerData.id, activityInfo.currDay);
                }
            }
            else
            {
                isHeFuActivity = false;
                curHeFuCSVActivityRulerData = null;
                activityHeFuId = 0;
            }
        }

        public void LoadedHeFuMemory()
        {
            m_HeFuOpenRedInfo.openRedDatas.Clear();
            JsonObject json = FileStore.ReadJson(m_HeFuStoreFilePath);
            if (json != null)
            {
                m_HeFuOpenRedInfo.DeserializeObject(json);
            }
        }

        public void SaveHeFuMemory()
        {
            m_HeFuOpenRedInfo.openRedDatas.Clear();
            for (int i = 0; i < exHeFuDatas.Count; i++)
            {
                OpenRedData openRedData = new OpenRedData();
                openRedData.id = exHeFuDatas[i].id;
                openRedData.openRed = exHeFuDatas[i].openRed;
                m_HeFuOpenRedInfo.openRedDatas.Add(openRedData);
            }
            FileStore.WriteJson(m_HeFuStoreFilePath, m_HeFuOpenRedInfo);
        }
        public void UpdateExHeFuData(ACExchangeData aCExchangeData)
        {
            if (aCExchangeData == null || !b_IsHeFuActivity)
            {
                return;
            }
            for (int i = 0; i < aCExchangeData.Ids.Count; i++)
            {
                uint exId = aCExchangeData.Ids[i];

                ExData exData = TryGetExHeFuData(exId);

                if (exData != null)
                {
                    exData.exNum = aCExchangeData.ExNums[i];
                }

                DebugUtil.LogFormat(ELogType.eOperationActivity, "UpdateExHeFuData ==> id:{0},exNum:{1}", exId, exData.exNum);
            }

            LoadedHeFuMemory();

            for (int i = 0; i < exHeFuDatas.Count; i++)
            {
                OpenRedData openRedData = m_HeFuOpenRedInfo.TryGet(exHeFuDatas[i].id);
                if (openRedData != null)
                {
                    exHeFuDatas[i].openRed = openRedData.openRed;
                }
            }

            UpdateHeFuExItemState();
            eventEmitter.Trigger(EEvents.e_RefreshEx);
        }

        public ExData TryGetExHeFuData(uint exId)
        {
            for (int i = 0; i < exHeFuDatas.Count; i++)
            {
                if (exHeFuDatas[i].id == exId)
                {
                    return exHeFuDatas[i];
                }
            }
            DebugUtil.LogErrorFormat($"exHeFuId not found {exId}");
            return null;
        }

        public void UpdateHeFuExItemState()
        {
            for (int i = 0; i < exHeFuDatas.Count; i++)
            {
                exHeFuDatas[i].UpdateExRedState();
            }
        }

        public bool hasHeFuRed()
        {
            for (int i = 0; i < exHeFuDatas.Count; i++)
            {
                if (exHeFuDatas[i].canEx)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
