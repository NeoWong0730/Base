using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System;
using System.Collections.Generic;
using UnityEngine;
using Table;
using static Packet.CmdPetCatchSettingsRes.Types;

namespace Logic
{ 
    public partial class Sys_Pet : SystemModuleBase<Sys_Pet>
    {
        public List<PetSetData> petSetList = new List<PetSetData>();
        private bool hasInit = false;

        public void PetCatchSettingsReq()
        {
            if (!hasInit)
            {
                CmdPetCatchSettingsReq req = new CmdPetCatchSettingsReq();
                NetClient.Instance.SendMessage((ushort)CmdPet.CatchSettingsReq, req);
                hasInit = true;
            }
        }

        private void OnPetCatchSettingsRes(NetMsg msg)
        {
            petSetList.Clear();
            CmdPetCatchSettingsRes res = NetMsgUtil.Deserialize<CmdPetCatchSettingsRes>(CmdPetCatchSettingsRes.Parser, msg);
            for(int i = 0; i < res.CatchSet.Count; ++i)
            {
                PetSetData data = new PetSetData();
                data.PetId = res.CatchSet[i].PetId;
                data.AutoCatch = res.CatchSet[i].AutoCatch;
                petSetList.Add(data);
            }  
            eventEmitter.Trigger(EEvents.OnPetSealSetting);
        }

        public void PetCatchSetReq(uint petId,bool flag)
        {
            CmdPetCatchSetReq req = new CmdPetCatchSetReq();
            req.PetId = petId;
            req.Flag = flag;
            NetClient.Instance.SendMessage((ushort)CmdPet.CatchSetReq, req);
        }

        private void OnPetCatchSetRes(NetMsg msg)
        {
            CmdPetCatchSetRes res = NetMsgUtil.Deserialize<CmdPetCatchSetRes>(CmdPetCatchSetRes.Parser, msg);
            bool hasId = false;
            for (int i = 0; i < petSetList.Count; ++i)
            {
                if (petSetList[i].PetId == res.PetId)
                {
                    petSetList[i].AutoCatch = res.Flag;
                    hasId = true;
                    break;
                }
            }
            if (!hasId)
            {
                PetSetData data = new PetSetData();
                data.PetId = res.PetId;
                data.AutoCatch = res.Flag;
                petSetList.Add(data);
            }
            eventEmitter.Trigger(EEvents.OnPetSealSetting);
        }            
    }
}


