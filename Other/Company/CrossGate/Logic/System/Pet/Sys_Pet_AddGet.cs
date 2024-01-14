using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System;
using System.Collections.Generic;
using UnityEngine;
using Table;

namespace Logic
{
    public class PetGradeEvt
    {
        public uint costnun;         //已培养次数
        public uint maxnum;       //最大培养次数
        public uint pernum;        //一个道具培养的次数
        public uint itemid;        //道具id
        public uint petuid;       //宠物uid
    }
    public class PetDevelopEvt
    {
        public uint uid;         
        public float gradeUp;       //挡位提升
        public uint gradeUpCount;        //挡位提升次数
        public int growthUp;        //成长系数提升
        public uint growthUpCount;       //成长次数提升次数
    }

    public class PetAddPointRecomandEvt
    {
        public uint petId;
        public uint requestTime;
        public List<uint> nums=new List<uint> ();
    }       

    public class PetDevelopEx
    {
        public uint itemId;
        public ClientPet clientpet;
    }

    public partial class Sys_Pet : SystemModuleBase<Sys_Pet>
    {
        public Dictionary<uint, PetDevelopEvt> petdevelopdic = new Dictionary<uint, PetDevelopEvt>();
        public Dictionary<uint, PetAddPointRecomandEvt> petAddPointRecDic = new Dictionary<uint, PetAddPointRecomandEvt>();
        public Dictionary<uint, uint> expData = new Dictionary<uint, uint>();
        public Dictionary<uint, uint> loyatyData = new Dictionary<uint, uint>();
        public Dictionary<uint, uint> resistanceAddPointDic = new Dictionary<uint, uint>();

        public List<uint> expIdData = new List<uint>();
        public List<uint> loyatyIdData = new List<uint>();
        public List<uint> selectItemItem = new List<uint>();

        public bool isInitDevelopData=false;
        public bool isInitDeviceData = false;
        public bool canAddAutoPoint = false;

        public void OnAllocPointReq(uint uid, uint vit, uint snh, uint inten, uint speed, uint magic, uint index)
        {
            CmdPetAllocPointReq cmdPetAllocPointReq = new CmdPetAllocPointReq();
            cmdPetAllocPointReq.Uid = uid;
            cmdPetAllocPointReq.Vit = vit;
            cmdPetAllocPointReq.Snh = snh;
            cmdPetAllocPointReq.Inten = inten;
            cmdPetAllocPointReq.Speed = speed;
            cmdPetAllocPointReq.Magic = magic;
            cmdPetAllocPointReq.PlanIndex = index;
            NetClient.Instance.SendMessage((ushort)CmdPet.AllocPointReq, cmdPetAllocPointReq);
        }

        public void OnAllocEnhancePointReq(Dictionary<uint,uint>addPointDic,uint uid, uint index)
        {
           // Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event47);

            CmdPetAllocEnhancePointReq cmdPetAllocPointReq = new CmdPetAllocEnhancePointReq();
            cmdPetAllocPointReq.Uid = uid;
            cmdPetAllocPointReq.PlanIndex = index;
            foreach (var info in addPointDic)
            {
                if (info.Value != 0)
                {
                    cmdPetAllocPointReq.AttrId.Add(info.Key);
                    cmdPetAllocPointReq.Points.Add(info.Value);
                }
            }
            NetClient.Instance.SendMessage((ushort)CmdPet.AllocEnhancePointReq, cmdPetAllocPointReq);
        }

        public void OnEnhancePointResetReq( uint uid, uint index)
        {
            CmdPetEnhancePointResetReq cmdPetEnhancePointResetReq = new CmdPetEnhancePointResetReq();
            cmdPetEnhancePointResetReq.Uid = uid;
            cmdPetEnhancePointResetReq.PlanIndex = index;
            NetClient.Instance.SendMessage((ushort)CmdPet.EnhancePointResetReq, cmdPetEnhancePointResetReq);
        }

        public void OnResetPointReq(uint uid, uint index)
        {
            CmdPetResetPointReq cmdPetResetPointReq = new CmdPetResetPointReq();
            cmdPetResetPointReq.Uid = uid;
            cmdPetResetPointReq.PlanIndex = index;
            NetClient.Instance.SendMessage((ushort)CmdPet.ResetPointReq, cmdPetResetPointReq);
        }

        public void OnGroomUseItemReq(uint uid, uint itemid, uint count,uint type)
        {
            CmdPetGroomUseItemReq cmdPetGroomUseItemReq = new CmdPetGroomUseItemReq();
            cmdPetGroomUseItemReq.Uid = uid;
            cmdPetGroomUseItemReq.ItemCount = count;
            cmdPetGroomUseItemReq.ItemId = itemid;
            cmdPetGroomUseItemReq.Type = type;
            NetClient.Instance.SendMessage((ushort)CmdPet.GroomUseItemReq, cmdPetGroomUseItemReq);
        }

        public void OnSetAutoPointReq(uint selectAddAttrId,bool enable,uint petUid, uint index)
        {
            /*if (enable)
            {
                Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event16);
            }*/

            CmdPetSetAutoPointReq cmdPetSetAutoPointReq = new CmdPetSetAutoPointReq();
            cmdPetSetAutoPointReq.Uid = petUid;
            cmdPetSetAutoPointReq.Autoselect = selectAddAttrId;
            cmdPetSetAutoPointReq.Enabled = enable;
            cmdPetSetAutoPointReq.PlanIndex = index;
            NetClient.Instance.SendMessage((ushort)CmdPet.SetAutoPointReq, cmdPetSetAutoPointReq);
        }

        private void OnSetAutoPointRes(NetMsg msg)
        {
            CmdPetSetAutoPointRes dataRes = NetMsgUtil.Deserialize<CmdPetSetAutoPointRes>(CmdPetSetAutoPointRes.Parser, msg);
            for(int i=0;i< petsList.Count; ++i)
            {
                if (petsList[i].petUnit.Uid == dataRes.Uid)
                {
                    petsList[i].petUnit.PetPointPlanData.Plans[(int)dataRes.PlanIndex].Autoselect = dataRes.Autoselect;
                    petsList[i].petUnit.PetPointPlanData.Plans[(int)dataRes.PlanIndex].Enabled = dataRes.Enabled;
                }
            }
            Sys_Pet.Instance.canAddAutoPoint = true;
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnPetAutoPoint);
        }

        public void AllPlayerAllocInfoReq(uint petId)
        {
            CmdPetAllPlayerAllocInfoReq cmdPetAllPlayerAllocInfoReq = new CmdPetAllPlayerAllocInfoReq();
            cmdPetAllPlayerAllocInfoReq.PetId = petId;
            NetClient.Instance.SendMessage((ushort)CmdPet.AllPlayerAllocInfoReq, cmdPetAllPlayerAllocInfoReq);
        }

        private void AllPlayerAllocInfoRes(NetMsg msg)
        {
            CmdPetAllPlayerAllocInfoRes dataRes = NetMsgUtil.Deserialize<CmdPetAllPlayerAllocInfoRes>(CmdPetAllPlayerAllocInfoRes.Parser, msg);
           if(petAddPointRecDic.TryGetValue(dataRes.PetId,out PetAddPointRecomandEvt evt))
            {
                evt.requestTime = Sys_Time.Instance.GetServerTime();
                evt.nums.Clear();
                for(int i=0;i< dataRes.Nums.Count; ++i)
                {
                    evt.nums.Add(dataRes.Nums[i]);
                }
            }
            else
            {
                PetAddPointRecomandEvt recEvt = new PetAddPointRecomandEvt();
                recEvt.requestTime = Sys_Time.Instance.GetServerTime();
                for (int i = 0; i < dataRes.Nums.Count; ++i)
                {
                    recEvt.nums.Add(dataRes.Nums[i]);
                }
                petAddPointRecDic.Add(dataRes.PetId, recEvt);
            }
            eventEmitter.Trigger(EEvents.OnUpdateAllPlayerAllocPoint);
        }

        private void OnEnhanceExpNtf(NetMsg msg)
        {
            CmdPetEnhanceExpNtf dataNtf = NetMsgUtil.Deserialize<CmdPetEnhanceExpNtf>(CmdPetEnhanceExpNtf.Parser, msg);
            for(int i = 0; i < Sys_Pet.Instance.petsList.Count; ++i)
            {
                if (Sys_Pet.Instance.petsList[i].petUnit.Uid == dataNtf.Uid)
                {
                    Sys_Pet.Instance.petsList[i].petUnit.EnhancePlansData.Enhanceexp = dataNtf.Enhanceexp;
                    Sys_Pet.Instance.petsList[i].petUnit.EnhancePlansData.Enhancelvl = dataNtf.Enhancelvl;
                    Sys_Pet.Instance.petsList[i].petUnit.EnhancePlansData.TotalPoint = dataNtf.TotalPoint;
                    Sys_Pet.Instance.petsList[i].petUnit.SimpleInfo.Exp = dataNtf.Exp;
                }
            }
            //宠物抵抗强化信息变化
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnUpdatePetInfo);
        }

        public void AllocPointPlanAddReq(uint petUid)
        {
            CmdPetAllocPointPlanAddReq cmdPetAllocPointPlanAddReq = new CmdPetAllocPointPlanAddReq();
            cmdPetAllocPointPlanAddReq.PetUid = petUid;
            NetClient.Instance.SendMessage((ushort)CmdPet.AllocPointPlanAddReq, cmdPetAllocPointPlanAddReq);
        }

        public void OnAllocPointPlanAddRes(NetMsg msg)
        {
            CmdPetAllocPointPlanAddRes dataRes = NetMsgUtil.Deserialize<CmdPetAllocPointPlanAddRes>(CmdPetAllocPointPlanAddRes.Parser, msg);
            for (int i = 0; i < petsList.Count; ++i)
            {
                if (petsList[i].petUnit.Uid == dataRes.PetUid)
                {
                    petsList[i].petUnit.PetPointPlanData.Plans.Add(dataRes.Info);
                    uint index = (uint)petsList[i].pointPlanAttrs.Count;
                    petsList[i].pointPlanAttrs.Add(index, dataRes.Attr);
                    eventEmitter.Trigger<uint>(EEvents.OnAllocPointPlanAdd, dataRes.PetUid);
                    Sys_Plan.Instance.eventEmitter.Trigger<uint, uint>(Sys_Plan.EEvents.AddNewPlan, (uint)Sys_Plan.EPlanType.PetAttribute, index);
                    return;
                }
            }
        }

        public void AllocPointPlanRenameReq(uint petUid, uint planIndex, string planName)
        {
            CmdPetAllocPointPlanRenameReq cmdPetAllocPointPlanRenameReq = new CmdPetAllocPointPlanRenameReq();
            cmdPetAllocPointPlanRenameReq.PetUid = petUid;
            cmdPetAllocPointPlanRenameReq.PlanIndex = planIndex;
            cmdPetAllocPointPlanRenameReq.PlanName = FrameworkTool.ConvertToGoogleByteString(planName);
            NetClient.Instance.SendMessage((ushort)CmdPet.AllocPointPlanRenameReq, cmdPetAllocPointPlanRenameReq);
        }

        public void OnAllocPointPlanRenameRes(NetMsg msg)
        {
            CmdPetAllocPointPlanRenameRes dataRes = NetMsgUtil.Deserialize<CmdPetAllocPointPlanRenameRes>(CmdPetAllocPointPlanRenameRes.Parser, msg);
            for(int i=0;i< petsList.Count; ++i)
            {
                if (petsList[i].petUnit.Uid == dataRes.PetUid)
                {
                    petsList[i].petUnit.PetPointPlanData.Plans[(int)dataRes.PlanIndex].PlanName = dataRes.PlanName;
                    eventEmitter.Trigger<uint,uint>(EEvents.OnAllocPointPlanRename, dataRes.PetUid, dataRes.PlanIndex);
                    Sys_Plan.Instance.eventEmitter.Trigger<uint, uint, string>(Sys_Plan.EEvents.ChangePlanName, (uint)Sys_Plan.EPlanType.PetAttribute, dataRes.PlanIndex, dataRes.PlanName.ToStringUtf8());
                    return;
                }
            }
        }

        public void AllocPointPlanUseReq(uint petUid, uint planIndex)
        {
            CmdPetAllocPointPlanUseReq cmdPetAllocPointPlanUseReq = new CmdPetAllocPointPlanUseReq();
            cmdPetAllocPointPlanUseReq.PetUid = petUid;
            cmdPetAllocPointPlanUseReq.PlanIndex = planIndex;
            NetClient.Instance.SendMessage((ushort)CmdPet.AllocPointPlanUseReq, cmdPetAllocPointPlanUseReq);
        }

        public void OnAllocPointPlanUseRes(NetMsg msg)
        {
            CmdPetAllocPointPlanUseRes dataRes = NetMsgUtil.Deserialize<CmdPetAllocPointPlanUseRes>(CmdPetAllocPointPlanUseRes.Parser, msg);
            for (int i = 0; i < petsList.Count; ++i)
            {
                if (petsList[i].petUnit.Uid == dataRes.PetUid)
                {
                    petsList[i].petUnit.PetPointPlanData.CurrentPlanIndex= dataRes.PlanIndex;
                    Sys_Plan.Instance.eventEmitter.Trigger<uint, uint>(Sys_Plan.EEvents.OnChangePlanSuccess, (uint)Sys_Plan.EPlanType.PetAttribute, (uint)dataRes.PlanIndex);
                    return;
                }
            }
        }

        public void AllocEnhancePlanAddReq(uint petUid)
        {
            CmdPetAllocEnhancePlanAddReq cmdPetAllocEnhancePlanAddReq = new CmdPetAllocEnhancePlanAddReq();
            cmdPetAllocEnhancePlanAddReq.PetUid = petUid;
            NetClient.Instance.SendMessage((ushort)CmdPet.AllocEnhancePlanAddReq, cmdPetAllocEnhancePlanAddReq);
        }

        public void OnAllocEnhancePlanAddRes(NetMsg msg)
        {
            CmdPetAllocEnhancePlanAddRes dataRes = NetMsgUtil.Deserialize<CmdPetAllocEnhancePlanAddRes>(CmdPetAllocEnhancePlanAddRes.Parser, msg);
            for (int i = 0; i < petsList.Count; ++i)
            {
                if (petsList[i].petUnit.Uid == dataRes.PetUid)
                {
                    petsList[i].petUnit.EnhancePlansData.Plans.Add(dataRes.Info);
                    uint index = (uint)petsList[i].petUnit.EnhancePlansData.Plans.Count - 1;
                    eventEmitter.Trigger<uint>(EEvents.OnCorrectPointPlanAdd, dataRes.PetUid);
                    Sys_Plan.Instance.eventEmitter.Trigger<uint, uint>(Sys_Plan.EEvents.AddNewPlan, (uint)Sys_Plan.EPlanType.PetAttributeCorrect, index);
                    return;
                }
            }
        }

        public void AllocEnhancePlanRenameReq(uint petUid, uint planIndex, string planName)
        {
            CmdPetAllocEnhancePlanRenameReq cmdPetAllocEnhancePlanRenameReq = new CmdPetAllocEnhancePlanRenameReq();
            cmdPetAllocEnhancePlanRenameReq.PetUid = petUid;
            cmdPetAllocEnhancePlanRenameReq.PlanIndex = planIndex;
            cmdPetAllocEnhancePlanRenameReq.PlanName = FrameworkTool.ConvertToGoogleByteString(planName);

            NetClient.Instance.SendMessage((ushort)CmdPet.AllocEnhancePlanRenameReq, cmdPetAllocEnhancePlanRenameReq);
        }

        public void OnAllocEnhancePlanRenameRes(NetMsg msg)
        {
            CmdPetAllocEnhancePlanRenameRes dataRes = NetMsgUtil.Deserialize<CmdPetAllocEnhancePlanRenameRes>(CmdPetAllocEnhancePlanRenameRes.Parser, msg);
            for (int i = 0; i < petsList.Count; ++i)
            {
                if (petsList[i].petUnit.Uid == dataRes.PetUid)
                {
                    petsList[i].petUnit.EnhancePlansData.Plans[(int)dataRes.PlanIndex].PlanName = dataRes.PlanName;
                    eventEmitter.Trigger<uint, uint>(EEvents.OnCorrectPlanRename, dataRes.PetUid, dataRes.PlanIndex);
                    Sys_Plan.Instance.eventEmitter.Trigger<uint, uint, string>(Sys_Plan.EEvents.ChangePlanName, (uint)Sys_Plan.EPlanType.PetAttributeCorrect, dataRes.PlanIndex, dataRes.PlanName.ToStringUtf8());
                }
            }
        }

        public void AllocEnhancePlanUseReq(uint petUid, uint planIndex)
        {
            CmdPetAllocEnhancePlanUseReq cmdPetAllocEnhancePlanUseReq = new CmdPetAllocEnhancePlanUseReq();
            cmdPetAllocEnhancePlanUseReq.PetUid = petUid;
            cmdPetAllocEnhancePlanUseReq.PlanIndex = planIndex;
            NetClient.Instance.SendMessage((ushort)CmdPet.AllocEnhancePlanUseReq, cmdPetAllocEnhancePlanUseReq);
        }

        public void OnAllocEnhancePlanUseRes(NetMsg msg)
        {
            CmdPetAllocEnhancePlanUseRes dataRes = NetMsgUtil.Deserialize<CmdPetAllocEnhancePlanUseRes>(CmdPetAllocEnhancePlanUseRes.Parser, msg);
            for (int i = 0; i < petsList.Count; ++i)
            {
                if (petsList[i].petUnit.Uid == dataRes.PetUid)
                {
                    petsList[i].petUnit.EnhancePlansData.CurrentPlanIndex = dataRes.PlanIndex;
                    break;
                }
            }
            eventEmitter.Trigger<uint>(EEvents.OnAllocEnhancePlanUse, dataRes.PetUid);
            Sys_Plan.Instance.eventEmitter.Trigger<uint, uint>(Sys_Plan.EEvents.OnChangePlanSuccess, (uint)Sys_Plan.EPlanType.PetAttributeCorrect, (uint)dataRes.PlanIndex);

        }

        public void GetPointPlanAttrReq(uint petUid)
        {
            ClientPet clientPet = Sys_Pet.Instance.GetPetByUId(petUid);
            if (clientPet.pointPlanAttrs.Count == 0)
            {
                CmdPetGetPointPlanAttrReq cmdPetGetPointPlanAttrReq = new CmdPetGetPointPlanAttrReq();
                cmdPetGetPointPlanAttrReq.PetUid = petUid;
                NetClient.Instance.SendMessage((ushort)CmdPet.GetPointPlanAttrReq, cmdPetGetPointPlanAttrReq);
            }
        }

        private void OnGetPointPlanAttrRes(NetMsg msg)
        {
            CmdPetGetPointPlanAttrRes dataRes = NetMsgUtil.Deserialize<CmdPetGetPointPlanAttrRes>(CmdPetGetPointPlanAttrRes.Parser, msg);
            for (int i = 0; i < Sys_Pet.Instance.petsList.Count; ++i)
            {
                if (Sys_Pet.Instance.petsList[i].petUnit.Uid == dataRes.PetUid)
                {
                    petsList[i].ResetPetPointPlanAttrModel(dataRes);
                }
            }
            eventEmitter.Trigger(EEvents.OnGetPointPlanAttr);         
        }

        private void OnSinglePointPlanAttrUpdateNtf(NetMsg msg)
        {
            CmdPetSinglePointPlanAttrUpdateNtf dataNtf = NetMsgUtil.Deserialize<CmdPetSinglePointPlanAttrUpdateNtf>(CmdPetSinglePointPlanAttrUpdateNtf.Parser, msg);
            for(int i = 0; i < petsList.Count; ++i)
            {
                if(petsList[i].petUnit.Uid == dataNtf.PetUid&& petsList[i].pointPlanAttrs.ContainsKey(dataNtf.PlanIndex))
                {
                    petsList[i].pointPlanAttrs[dataNtf.PlanIndex] = dataNtf.Attr;
                    Sys_Pet.Instance.eventEmitter.Trigger<int, int>(Sys_Pet.EEvents.OnSelectAddPointPlan, (int)dataNtf.PlanIndex, (int)Sys_Plan.EPlanType.PetAttribute);
                    break;
                }
            }
        }

        public bool CheckAllPlayerAllocInfoIsValid(uint petId)
        {
            if (petAddPointRecDic.TryGetValue(petId, out PetAddPointRecomandEvt evt))
            {
                uint nowTime = Sys_Time.Instance.GetServerTime();
                return nowTime - evt.requestTime < 360;  // 十分钟有效时间 超时请求新数据
            }
            else
            {
                return false;
            }
        }

        public int GetAttrAllocPlayerPer(uint petId, int index)
        {
            uint totalNum = GetAllPalyerNum(petId);
            if (totalNum == 0)
                return 0;
            if (petAddPointRecDic.TryGetValue(petId, out PetAddPointRecomandEvt evt)&& evt.nums.Count>= index)
            {
                return Mathf.FloorToInt( evt.nums[index] / (float)totalNum*100);
            }
            else
            {
                return 0;
            }
        }

        public uint  GetAllPalyerNum(uint petId)
        {
            if (petAddPointRecDic.TryGetValue(petId, out PetAddPointRecomandEvt evt))
            {
                uint num = 0; 
                for(int i=0;i< evt.nums.Count; ++i)
                {
                    num += evt.nums[i];
                }
                return num;
            }
            else
            {
                return 0;
            }
        }

        private void SetDevelopeItemDataList()
        {
            expData.Clear();
            loyatyData.Clear();
            expIdData.Clear();
            loyatyIdData.Clear();
            string[] expStr = CSVPetNewParam.Instance.GetConfData(12).str_value.Split('|');
            for (int i = 0; i < expStr.Length; i++)
            {
                string[] str = expStr[i].Split('&');
               if(!expIdData.Contains(uint.Parse(str[0])))
                {
                    expIdData.Add(uint.Parse(str[0]));
                    expData.Add(uint.Parse(str[0]), uint.Parse(str[1]));
                }
            }

            string[] loyatyStr = CSVPetNewParam.Instance.GetConfData(16).str_value.Split('|');
            for (int i = 0; i < loyatyStr.Length; i++)
            {
                string[] str = loyatyStr[i].Split('&');
                if (!loyatyIdData.Contains(uint.Parse(str[0])))
                {
                    loyatyIdData.Add(uint.Parse(str[0]));
                    loyatyData.Add(uint.Parse(str[0]), uint.Parse(str[1]));
                }
            }
        }

        //宠物加点方案数据
        public PetPointPlansData GetPetAddPointPlanDataByUid(ulong uid)
        {
            ClientPet clientPet = Sys_Pet.Instance.GetPetByUId((uint)uid);
            if (clientPet != null && clientPet.petUnit != null)
            {
                return clientPet.petUnit.PetPointPlanData;
            }
            else
            {
                return null;
            }
        }

        //宠物强化方案数据
        public EnhancePlansData GetPetEnHanPointPlanDataByUid(ulong uid)
        {
            ClientPet clientPet = Sys_Pet.Instance.GetPetByUId((uint)uid);
            if (clientPet != null && clientPet.petUnit != null)
            {
                return clientPet.petUnit.EnhancePlansData;
            }
            else
            {
                return null;
            }
        }
    }
}
