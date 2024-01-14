using Logic.Core;
using Net;
using Packet;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class NPCAreaCheckSystem : LevelSystemBase
    {
        private Dictionary<ulong, uint> npcTriggerNums = new Dictionary<ulong, uint>();
        private float _areaDistance;
        private float lastTriggerTime = 0;
        private float cd;
        private float lastCheckTime = 0;
        private float cdCheckNpc = 10f;

        public override void OnCreate()
        {
            _areaDistance = CSVExploringSkill.Instance.GetConfData(100).range / 10000f;
            cd = float.Parse(CSVParam.Instance.GetConfData(199).str_value) / 1000f;
        }

        public override void OnUpdate()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
                return;

            if (Time.unscaledTime >= lastCheckTime)
            {
                lastCheckTime = Time.unscaledTime + cdCheckNpc;
                Sys_Npc.Instance.eventEmitter.Trigger(Sys_Npc.EEvents.OnCheckShowNpc);
            }

            if (Time.unscaledTime < lastTriggerTime)
                return;

            lastTriggerTime = Time.unscaledTime + cd;

            for (int i = 0, len = GameCenter.npcsList.Count; i < len; ++i)
            {
                Npc npc = GameCenter.npcsList[i];
                if (!CheckVaild(npc))
                    continue;

                uint npcType = npc.cSVNpcData.type;
                if (npcType == (uint)ENPCType.ActiveMonster || npcType == (uint)ENPCType.EventNPC)
                {
                    ExcuteAreaCheck(npc);
                }
                else if(npc.cSVNpcData.type == (uint)ENPCType.LittleKnow)
                {
                    ExcuteLittleDistanceCheck(npc);
                }
                else if (npc.NPCFunctionComponent.HasActiveFunction(EFunctionType.Inquiry))
                {
                    ExcuteInquiryDistanceCheck(npc);
                }
            }
        }

        private bool CheckVaild(Npc npc)
        {
            return npc.VisualComponent.Visiable;
        }

        private void ExcuteAreaCheck(Npc npc)
        {
            if(npc.Contains(GameCenter.mainHero.transform))
            {
                uint npcTriggerFrequency = npc.cSVNpcData.NpcTriggerFrequency;

                if (npcTriggerFrequency != 0)
                {
                    npcTriggerNums.TryGetValue(npc.uID, out uint triggerNum);
                    if (triggerNum < npcTriggerFrequency)
                    {
                        if (Sys_Team.Instance.HaveTeam)
                        {      
                            if (!Sys_Team.Instance.isCaptain() && !Sys_Team.Instance.isPlayerLeave())
                            {

                            }
                            else
                            {
                                npcTriggerNums[npc.uID] = triggerNum + 1;
                            }
                        }
                        else
                        {
                            npcTriggerNums[npc.uID] = triggerNum + 1;
                        }                     
                    }
                    else
                    {
                        return;
                    }
                }

                Sys_Interactive.Instance.eventEmitter.Trigger<InteractiveEvtData>(EInteractiveType.AreaCheck, new InteractiveEvtData()
                {
                    eInteractiveAimType = EInteractiveAimType.NPCFunction,
                    sceneActor = npc,
                    immediately = true,
                });
            }
        }

        private void ExcuteInquiryDistanceCheck(Npc npc)
        {
            if (Lib.Core.MathUtlilty.SafeDistanceLessEqual(GameCenter.mainHero.transform, npc.transform, _areaDistance))
            {
                Sys_Inquiry.Instance.EnterArea(npc.uID);
            }
            else
            {
                Sys_Inquiry.Instance.ExitArea(npc.uID);
            }
        }

        private void ExcuteLittleDistanceCheck(Npc npc)
        {
            uint knowledgeId = npc.cSVNpcData.subtype;
            if (Sys_Knowledge.Instance.IsKnowledgeActive(knowledgeId))
                return;

            if (!npc.Contains(GameCenter.mainHero.transform))
                return;

            if (!Sys_FunctionOpen.Instance.IsOpen(22000, false))
                return;

            CmdNpcNpcTriggerReq req = new CmdNpcNpcTriggerReq();
            req.UNpcId = npc.uID;

            NetClient.Instance.SendMessage((ushort)CmdNpc.NpcTriggerReq, req);
        }

        public void OnNpcRemove(Npc npc)
        {
            npcTriggerNums.Remove(npc.uID);

            if (npc.NPCFunctionComponent.HasActiveFunction(EFunctionType.Inquiry))
            {
                Sys_Inquiry.Instance.ExitArea(npc.uID);
            }
        }
    }
}