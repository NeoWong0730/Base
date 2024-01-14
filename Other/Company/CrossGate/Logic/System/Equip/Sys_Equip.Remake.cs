using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public partial class Sys_Equip : SystemModuleBase<Sys_Equip>
    {
        private void OnInitRemakeNtf()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.RebuildEquipReq, (ushort)CmdItem.RebuildEquipRes, OnRebuildEquipRes, CmdItemRebuildEquipRes.Parser);
        }

        public void OnRebuildEquipReq(ulong uId, uint infoId)
        {
            CmdItemRebuildEquipReq req = new CmdItemRebuildEquipReq();
            req.Uuid = uId;
            req.ToinfoId = infoId;
            NetClient.Instance.SendMessage((ushort)CmdItem.RebuildEquipReq, req);
        }

        private void OnRebuildEquipRes(NetMsg msg)
        {
            CmdItemRebuildEquipRes res = NetMsgUtil.Deserialize<CmdItemRebuildEquipRes>(CmdItemRebuildEquipRes.Parser, msg);

            eventEmitter.Trigger(EEvents.OnNtfRebuildEquip);
        }

        public float GetRemakeBasicValue(uint rebuildId, int index, uint curAttrId, uint desEquipId)
        {
            ItemData opItem = Sys_Bag.Instance.GetItemDataByUuid(Sys_Equip.Instance.CurOpEquipUId);
            CSVEquipment.Data opEquip = CSVEquipment.Instance.GetConfData(opItem.Id);
            CSVEquipment.Data desEquip = CSVEquipment.Instance.GetConfData(desEquipId);
            CSVEquipmentRebuild.Data rebuilddata = CSVEquipmentRebuild.Instance.GetConfData(rebuildId);
            
            
            //先计算原装备各个基础属性的评分
            List<float> listSrcScores = new List<float>();
            for (int i = 0; i < opItem.Equip.BaseAttr.Count; ++i)
            {
                for (int j = 0; j < opEquip.attr.Count; ++j)
                {
                    if (opEquip.attr[j][0] == opItem.Equip.BaseAttr[i].Attr2.Id)
                    {
                        float score = 1.0f * opItem.Equip.BaseAttr[i].Attr2.Value * opEquip.attr[j][3] / opEquip.attr[j][2];
                        listSrcScores.Add(score);
                    }
                }
            }
            
            //再计算转换后装备，当前基础属性的评分
            float curScore = 0;
            switch (index)
            {
                case 0:
                    if (rebuilddata.attr_change1 != null)
                    {
                        for (int i = 0; i < rebuilddata.attr_change1.Count; ++i)
                        {
                            if (i < listSrcScores.Count)
                            {
                                curScore += listSrcScores[i] * rebuilddata.attr_change1[i] * 1.0f / 10000f;
                            }
                        }
                    }
                    else
                    {
                        DebugUtil.LogError(rebuildId.ToString() + " attr_change1 = null");
                    }
                    
                    break;
                case 1:
                    if (rebuilddata.attr_change2 != null)
                    {
                        for (int i = 0; i < rebuilddata.attr_change2.Count; ++i)
                        {
                            if (i < listSrcScores.Count)
                            {
                                curScore += listSrcScores[i] * rebuilddata.attr_change2[i] * 1.0f / 10000f;
                            }
                        }
                    }
                    else
                    {
                        DebugUtil.LogError(rebuildId.ToString() + " attr_change2 = null");
                    }
                    break;
                case 2:
                    if (rebuilddata.attr_change3 != null)
                    {
                        for (int i = 0; i < rebuilddata.attr_change3.Count; ++i)
                        {
                            if (i < listSrcScores.Count)
                            {
                                curScore += listSrcScores[i] * rebuilddata.attr_change3[i] * 1.0f / 10000f;
                            }
                        }
                    }
                    else
                    {
                        DebugUtil.LogError(rebuildId.ToString() + " attr_change3 = null");
                    }
                    
                    break;
                case 3:
                    if (rebuilddata.attr_change4 != null)
                    {
                        for (int i = 0; i < rebuilddata.attr_change4.Count; ++i)
                        {
                            if (i < listSrcScores.Count)
                            {
                                curScore += listSrcScores[i] * rebuilddata.attr_change4[i] * 1.0f / 10000f;
                            }
                        }
                    }
                    else
                    {
                        DebugUtil.LogError(rebuildId.ToString() + " attr_change3 = null");
                    }
                    
                    break;
            }
            
            //获取转换装备的属性
            for (int i = 0; i < desEquip.attr.Count; ++i)
            {
                if (desEquip.attr[i][0] == curAttrId)
                {
                    return curScore / desEquip.attr[i][3] * desEquip.attr[i][2];
                }
            }

            return 0;
        }
    }
}

