using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System;
using System.Collections.Generic;
using UnityEngine;
using Table;
using Google.Protobuf.Collections;
using static Packet.CmdPetFightPetSetListRes.Types;

namespace Logic
{
    public partial class Sys_Pet : SystemModuleBase<Sys_Pet>
    {
        public Dictionary<uint,int> PetFirstSetField = new Dictionary<uint, int>();//表id/petuid
        /// <summary>
        /// 首发出战宠物列表请求
        /// </summary>
        public void OnFightPetSetListReq()
        {
            CmdPetFightPetSetListReq req = new CmdPetFightPetSetListReq();
            NetClient.Instance.SendMessage((ushort)CmdPet.FightPetSetListReq, req);
        }
        private void OnFightPetSetListRes(NetMsg msg)
        {
            CmdPetFightPetSetListRes res = NetMsgUtil.Deserialize<CmdPetFightPetSetListRes>(CmdPetFightPetSetListRes.Parser, msg);
            InitPetSetList(res.PetSet);
            eventEmitter.Trigger(EEvents.OnPetFirstChoiceUpdate);
        }
        /// <summary>
        /// 首发出战宠物请求，type是表id,set-1出战，2休息
        /// </summary>
        public void OnFightPetSetReq(uint type,uint petUid,uint set)
        {
            CmdPetFightPetSetReq req = new CmdPetFightPetSetReq();
            if (type<=0||petUid<0) return;
            req.FightType = type;
            req.PetUid = petUid;
            req.SetType = set;
            NetClient.Instance.SendMessage((ushort)CmdPet.FightPetSetReq, req);
        }
        private void OnFightPetSetRes(NetMsg msg)
        {
            CmdPetFightPetSetRes res = NetMsgUtil.Deserialize<CmdPetFightPetSetRes>(CmdPetFightPetSetRes.Parser, msg);
            if (res.SetType==2)
            {
                PetFirstSetField[res.FightType]=-1;
            }
            else
            {
                PetFirstSetField[res.FightType]=(int)res.PetUid;
            }
            eventEmitter.Trigger(EEvents.OnPetFirstChoiceUpdate);
        }
        public void PetFirstChoiceDestory()
        {
            PetFirstSetField.Clear();
        }
        private void InitPetSetList(RepeatedField<PetSet> list)
        {
            PetFirstSetField.Clear();
            var data = CSVBattlePet.Instance.GetAll();
            for (int i=0,count= data.Count; i< count;i++)
            {
                PetFirstSetField[data[i].id] = -1;
            }
            for (int j = 0; j < list.Count; j++)
            {
                PetFirstSetField[list[j].FightType]=(int)list[j].PetUid;
            }
        }
        public void PetFirstChoiceOpen()
        {
            UIManager.OpenUI(EUIID.UI_Pet_FirstStart);
        }
    }

}