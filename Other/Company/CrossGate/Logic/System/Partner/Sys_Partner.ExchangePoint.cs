using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;
using UnityEngine;

namespace Logic {

    public partial class Sys_Partner : SystemModuleBase<Sys_Partner>
    {

        private uint _Point;

        public class PointTempData
        {
            public uint totalPoint;
            public uint costPoint;

            public void Reset()
            {
                totalPoint = 0;
                costPoint = 0;
            }

            public void ResetCost()
            {
                costPoint = 0;
            }
        }
        
        public  PointTempData tempPoint = new PointTempData();
        
        public class SkillResetInfo
        {
            public uint paId;
            public uint index;
            public uint skillId;
        }
        
        public CmdPartnerDistributePointReq.Types.DistributePointInfo[] listDistributes = new CmdPartnerDistributePointReq.Types.DistributePointInfo[8];
        
        private void RegExchangePoint()
        {
            EventDispatcher.Instance.AddEventListener(0,(ushort)CmdPartner.PointUpdateNtf, this.OnPointUpdateNtf, CmdPartnerPointUpdateNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPartner.DistributePointReq, (ushort)CmdPartner.DistributePointRes, this.OnDistributePointRes, CmdPartnerDistributePointRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPartner.ReDistributePointReq, (ushort)CmdPartner.ReDistributePointRes, this.OnDistributePointRes, CmdPartnerReDistributePointRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPartner.ReRandomSkillReq, (ushort)CmdPartner.ReRandomSkillRes, this.OnReRandomSkillRes, CmdPartnerReRandomSkillRes.Parser);
        }

        private void BindExchangePoint(uint point)
        {
            _Point = point;
            tempPoint.Reset();
            tempPoint.totalPoint = _Point;
        }
        
        public void ExchangePointReq(uint addPoint)
        {
            CmdPartnerExchangePointReq req = new CmdPartnerExchangePointReq();
            req.AddPoint = addPoint;
            NetClient.Instance.SendMessage((ushort)CmdPartner.ExchangePointReq, req);
        }

        private void OnPointUpdateNtf(NetMsg msg)
        {
            CmdPartnerPointUpdateNtf ntf = NetMsgUtil.Deserialize<CmdPartnerPointUpdateNtf>(CmdPartnerPointUpdateNtf.Parser, msg);
            _Point = ntf.NowPoint;
            tempPoint.Reset();
            tempPoint.totalPoint = _Point;
            eventEmitter.Trigger(EEvents.OnPartnerPointUpdateNtf);
        }

        //加点
        public void OnDistributePointReq(uint infoId)
        {
            List<CmdPartnerDistributePointReq.Types.DistributePointInfo> temp = new List<CmdPartnerDistributePointReq.Types.DistributePointInfo>();
            for (int i = 0; i < listDistributes.Length; ++i)
            {
                if (listDistributes[i] != null)
                    temp.Add(listDistributes[i]);
            }

            if (temp.Count <= 0)
            {
                Debug.LogError("没有加点");
                return;
            }
            
            CmdPartnerDistributePointReq req = new CmdPartnerDistributePointReq();
            req.InfoId = infoId;
            req.Info.AddRange(temp);
            
            NetClient.Instance.SendMessage((ushort)CmdPartner.DistributePointReq, req);
        }
        
        private void OnDistributePointRes(NetMsg msg)
        {
            CmdPartnerDistributePointRes res = NetMsgUtil.Deserialize<CmdPartnerDistributePointRes>(CmdPartnerDistributePointRes.Parser, msg);

            Partner pa = GetPartnerInfo(res.InfoId);
            pa.ImproveData = res.Data;
            
            eventEmitter.Trigger(EEvents.OnPartnerDistrutePointNtf);
        }
        
        //重置加点
        public void OnReDistributePointReq(uint infoId)
        {
            CmdPartnerReDistributePointReq req = new CmdPartnerReDistributePointReq();
            req.InfoId = infoId;
            
            NetClient.Instance.SendMessage((ushort)CmdPartner.ReDistributePointReq, req);
        }
        
        private void OnReDistributePointRes(NetMsg msg) //会用到上面加点返回
        {
            CmdPartnerReDistributePointRes res = NetMsgUtil.Deserialize<CmdPartnerReDistributePointRes>(CmdPartnerReDistributePointRes.Parser, msg);
            
             Partner pa = GetPartnerInfo(res.InfoId);
             pa.ImproveData = res.Data;
             eventEmitter.Trigger(EEvents.OnPartnerDistrutePointNtf);
        }
        
        //重置技能
        public void OnReRandomSkillReq(uint paId, uint slot)
        {
            CmdPartnerReRandomSkillReq req = new CmdPartnerReRandomSkillReq();
            req.InfoId = paId;
            req.Index = slot;
            NetClient.Instance.SendMessage((ushort)CmdPartner.ReRandomSkillReq, req);
        }
        
        private void OnReRandomSkillRes(NetMsg msg)
        {
            CmdPartnerReRandomSkillRes res = NetMsgUtil.Deserialize<CmdPartnerReRandomSkillRes>(CmdPartnerReRandomSkillRes.Parser, msg);
            
            Partner pa = GetPartnerInfo(res.InfoId);
            pa.ImproveData.ImproveSkills[(int)res.Index].SkillId = res.SkillId;
            
            SkillResetInfo info = new SkillResetInfo();
            info.paId = res.InfoId;
            info.index = res.Index;
            info.skillId = res.SkillId;
            
            eventEmitter.Trigger(EEvents.OnPartnerPointSkillNtf, info);
        }

        public void ClearDistributes()
        {
            for (int i = 0; i < listDistributes.Length; ++i)
                listDistributes[i] = null;
        }

        public CmdPartnerDistributePointReq.Types.DistributePointInfo GenDsitributes(int index, PartnerImproveCount improveCount)
        {
            listDistributes[index] = new CmdPartnerDistributePointReq.Types.DistributePointInfo();
            listDistributes[index].Index = (uint)index;
            listDistributes[index].Count = improveCount.ImproveCount;

            return listDistributes[index];
        }

        public bool IsPlusDistributePoint(uint costPoint)
        {
            uint leftPoint = tempPoint.totalPoint - tempPoint.costPoint;
            if (costPoint <= leftPoint)
            {
                return true;
            }

            return false;
        }

        public void PlusDistributePoint(uint costPoint)
        {
            tempPoint.costPoint += costPoint;
            eventEmitter.Trigger(EEvents.OnPartnerPointChangeNtf);
        }

        public bool IsMinusDistributePoint(uint costPoint)
        {
            if (costPoint <= tempPoint.costPoint)
            {
                return true;
            }

            return false;
        }
        
        public void MinusDistributePoint(uint costPoint)
        {
            tempPoint.costPoint -= costPoint;
            eventEmitter.Trigger(EEvents.OnPartnerPointChangeNtf);
        }

        public uint GetChangeRate(PartnerImproveCount improveCount, int index)
        {
            uint changeRate = 0;
            for (int i = 0; i < listDistributes.Length; ++i)
            {
                if (listDistributes[i] != null)
                {
                    if (listDistributes[i].Index == index)
                    {
                        uint count = listDistributes[i].Count - improveCount.ImproveCount;
                        int start = (int)improveCount.ImproveCount;
                        int end = (int)listDistributes[i].Count;
                        CSVAttrBoost.Data boosInfo = CSVAttrBoost.Instance.GetConfData(improveCount.KeyId);
                        for (int k = start; k < end; ++k)
                        {
                            changeRate += boosInfo.boost_value_cost[k][0];
                        }

                        break;
                    }
                }
            }

            return changeRate;
        }

        public int GetChangeValue(uint paId, uint attrId, int value)
        {
            int result = 0;
            Partner pa = GetPartnerInfo(paId);
            for (int i = 0; i < pa.ImproveData.ImproveAttrs.Count; ++i)
            {
                uint changeRate = 0;
                CSVAttrBoost.Data boosInfo = CSVAttrBoost.Instance.GetConfData(pa.ImproveData.ImproveAttrs[i].KeyId);
                if (boosInfo.attr_id == attrId)
                {
                    for (int k = 0; k < (int) pa.ImproveData.ImproveAttrs[i].ImproveCount; ++k)
                    {
                        changeRate += boosInfo.boost_value_cost[k][0];
                    }

                    float fValue = value * changeRate * 1.0f / 10000;
                    result = Mathf.CeilToInt(fValue);
                    break;
                }
            }

            return result;
        }

        public List<uint> GetPropertySkills(uint paId)
        {
            List<uint> temp = new List<uint>();
            Partner pa = GetPartnerInfo(paId);
            if (pa != null && pa.ImproveData.ImproveSkills != null)
            {
                for ( int i = 0; i < pa.ImproveData.ImproveSkills.Count; ++i)
                    temp.Add(pa.ImproveData.ImproveSkills[i].SkillId);
            }

            return temp;
        }
    }
}