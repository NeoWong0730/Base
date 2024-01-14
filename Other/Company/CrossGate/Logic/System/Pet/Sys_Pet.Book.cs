using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using static Packet.CmdPetGetHandbookRes.Types;

namespace Logic
{

    public partial class Sys_Pet : SystemModuleBase<Sys_Pet>
    {
        /// <summary> 客户端状态数据 </summary>
        public class ClientStateData
        {
            public uint valueId;     //参数
            public bool isSealState; //服务器状态
        }
        /// <summary> 客户端状态 </summary>
        public bool isSeal
        {
            get
            {
                return clientStateId != Sys_Role.EClientState.None;
            }
        }
        /// <summary> 服务器状态 </summary>
        public bool isSealState
        {
            get
            {
                ClientStateData clientStateData = null;
                if (!dict_ClientStateData.TryGetValue(clientStateId, out clientStateData))
                    return false;
                return clientStateData.isSealState;
            }
        }
        /// <summary> 状态对应的参数 </summary>
        private uint clientStateParameter
        {
            get
            {
                ClientStateData clientStateData = null;
                if (!dict_ClientStateData.TryGetValue(clientStateId, out clientStateData))
                    return 0;
                return clientStateData.valueId;
            }
            set
            {
                ClientStateData clientStateData = null;
                if (!dict_ClientStateData.TryGetValue(clientStateId, out clientStateData))
                    return;
                clientStateData.valueId = value;
            }
        }
        /// <summary> 当前状态Id </summary>
        private Sys_Role.EClientState _clientStateId = Sys_Role.EClientState.None;
        public Sys_Role.EClientState clientStateId
        {
            get
            {
                return _clientStateId;
            }
            set
            {
                if (_clientStateId != value)
                {
                    var old = _clientStateId;
                    _clientStateId = value;
                    Sys_Pet.Instance.eventEmitter.Trigger<int, int>(Sys_Pet.EEvents.OnPatrolStateChange, (int)old, (int)value);
                }
            }
        }
        /// <summary> 当前状态Id对应数据 </summary>
        private Dictionary<Sys_Role.EClientState, ClientStateData> dict_ClientStateData = new Dictionary<Sys_Role.EClientState, ClientStateData>()
        {
            {Sys_Role.EClientState.CatchPet, new ClientStateData()},
            {Sys_Role.EClientState.Hangup, new ClientStateData()},
            {Sys_Role.EClientState.ExecTask, new ClientStateData()},
        };

        private uint maxGradeLo = 0;

        public uint MaxGradeLo
        {
            get
            {
                if(maxGradeLo == 0)
                {
                    CSVPetNewParam.Data param = CSVPetNewParam.Instance.GetConfData(2u);
                    if (null != param)
                    {
                        maxGradeLo = CSVPetNewParam.Instance.GetConfData(2u).value;
                    }
                }
                return maxGradeLo;
            }
        }

        private bool isBookDataInit = false;
        private Timer timerSeal;
        public Dictionary<uint, HandbookData> allBookData = new Dictionary<uint, HandbookData>();
        public List<HBSeverData> bookNameData = new List<HBSeverData>();
        public void OnPetGetHandbookReq()
        {
            if (!isBookDataInit)
            {
                CmdPetGetHandbookReq cmdPetGetHandbookReq = new CmdPetGetHandbookReq();
                NetClient.Instance.SendMessage((ushort)CmdPet.GetHandbookReq, cmdPetGetHandbookReq);
            }
        }

        private void PetGetHandbookRes(NetMsg msg)
        {
            CmdPetGetHandbookRes dataNtf = NetMsgUtil.Deserialize<CmdPetGetHandbookRes>(CmdPetGetHandbookRes.Parser, msg);
            isBookDataInit = true;
            bookNameData.Clear();
            allBookData.Clear();
            for (int i = 0; i < dataNtf.HandbookInfo.Count; i++)
            {
                HandbookData data = dataNtf.HandbookInfo[i];
                if (!allBookData.ContainsKey(data.PetId))
                {
                    allBookData.Add(data.PetId, data);
                }
            }
            for (int i = 0; i < dataNtf.HbServerInfo.Count; i++)
            {
                bookNameData.Add(dataNtf.HbServerInfo[i]);
            }
        }

        public void OnPetActivateReq(uint petId)
        {
            CmdPetActivateReq cmdPetActivateReq = new CmdPetActivateReq();
            cmdPetActivateReq.PetId = petId;
            NetClient.Instance.SendMessage((ushort)CmdPet.ActivateReq, cmdPetActivateReq);
        }

        private void PetActivateRes(NetMsg msg)
        {
            CmdPetActivateRes dataNtf = NetMsgUtil.Deserialize<CmdPetActivateRes>(CmdPetActivateRes.Parser, msg);
            if (!allBookData.TryGetValue(dataNtf.PetId, out HandbookData handbookData))
            {
                handbookData = new HandbookData();
                handbookData.PetId = dataNtf.PetId;
                handbookData.LoveExp = 0;
                handbookData.LoveBreak = 0;
                handbookData.LoveLevel = 0;
                handbookData.StoryId = 0;
                handbookData.StoryIdIng = 0;
                handbookData.Flag = 1;
                allBookData.Add(dataNtf.PetId, handbookData);
            }
            else
            {
                if (handbookData.Flag == 2)
                {
                    handbookData.Flag = 3;
                }
            }
            CSVPetNew.Data cSVPetNewData = CSVPetNew.Instance.GetConfData(dataNtf.PetId);
            if(null != cSVPetNewData)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12106, LanguageHelper.GetTextContent(cSVPetNewData.name)));
            }
            Sys_CommonTip.Instance.TipKnowledge(Sys_Knowledge.ETypes.PetBook, 0);
            // 发送一条已经激活广播
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnPetActivate);
        }

        public void OnPetLoveExpUpReq(uint petId, uint count, uint chipCount)
        {
            CmdPetLoveExpUpReq cmdPetLoveExpUpReq = new CmdPetLoveExpUpReq();
            cmdPetLoveExpUpReq.PetId = petId;
            cmdPetLoveExpUpReq.Count = count;
            cmdPetLoveExpUpReq.ChipCount = chipCount;
            NetClient.Instance.SendMessage((ushort)CmdPet.LoveExpUpReq, cmdPetLoveExpUpReq);
        }

        private void PetLoveExpUpRes(NetMsg msg)
        {
            CmdPetLoveExpUpRes dataNtf = NetMsgUtil.Deserialize<CmdPetLoveExpUpRes>(CmdPetLoveExpUpRes.Parser, msg);
            if (allBookData.ContainsKey(dataNtf.PetId))
            {
                allBookData[dataNtf.PetId].LoveExp = dataNtf.LoveExp;
                allBookData[dataNtf.PetId].LoveLevel = dataNtf.LoveLevel;
                allBookData[dataNtf.PetId].LoveBreak = dataNtf.LoveBreak;
            }
            else
            {
                bool isAutoActive = GetPetIsAutoActive(dataNtf.PetId);
                HandbookData handbookData = new HandbookData();
                handbookData.PetId = dataNtf.PetId;
                handbookData.LoveExp = dataNtf.LoveExp;
                handbookData.LoveLevel = dataNtf.LoveLevel;
                handbookData.LoveBreak = dataNtf.LoveBreak;
                handbookData.StoryId = 0;
                handbookData.StoryIdIng = 0;
                handbookData.Flag = isAutoActive ? 1u : 0u;
                allBookData.Add(dataNtf.PetId, handbookData);
            }

            // 发送一条已经提升好感度广播
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnPetLoveExpUp);
        }

        public void OnPetActivateStoryReq(uint loveId)
        {
            CmdPetActivateStoryReq cmdPetActivateStoryReq = new CmdPetActivateStoryReq();
            cmdPetActivateStoryReq.LoveId = loveId;
            NetClient.Instance.SendMessage((ushort)CmdPet.ActivateStoryReq, cmdPetActivateStoryReq);
        }

        private void PetActivateStoryRes(NetMsg msg)
        {
            CmdPetActivateStoryRes dataNtf = NetMsgUtil.Deserialize<CmdPetActivateStoryRes>(CmdPetActivateStoryRes.Parser, msg);
            uint petId = dataNtf.LoveId / 1000;
            if (allBookData.ContainsKey(petId))
            {
                allBookData[petId].ClickStory.Add(dataNtf.LoveId - (uint)(dataNtf.LoveId / 1000) * 1000);
                // 发送一条已经解封背景
                Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnPetActivateStory, dataNtf.LoveId);
            }
        }

        private void OnPetAutoActivateStoryNtf(NetMsg msg)
        {
            CmdPetAutoActivateStoryNtf dataNtf = NetMsgUtil.Deserialize<CmdPetAutoActivateStoryNtf>(CmdPetAutoActivateStoryNtf.Parser, msg);
            if (allBookData.ContainsKey(dataNtf.PetId))
            {
                allBookData[dataNtf.PetId].StoryIdIng = dataNtf.StoryIdIng;
                // 发送一条已经背景故事任务已经下发
                Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnPetActivateStory, dataNtf.StoryIdIng);
            }
        }

        private void OnPetDiscovererNtf(NetMsg msg)
        {
            CmdPetDiscovererNtf dataNtf = NetMsgUtil.Deserialize<CmdPetDiscovererNtf>(CmdPetDiscovererNtf.Parser, msg);
            HBSeverData data = new HBSeverData();
            data.PetId = dataNtf.PetId;
            data.DiscovererName = dataNtf.DiscovererName;
            bookNameData.Add(data);

            CSVPetNewSeal.Data cSVPetNewSealData = CSVPetNewSeal.Instance.GetConfData(dataNtf.PetId);
            if (null != cSVPetNewSealData && cSVPetNewSealData.is_msg)
            {
                if(null != cSVPetNewSealData.coordinate && cSVPetNewSealData.coordinate.Count >= 2)
                {
                    string petName = "";
                    CSVPetNew.Data cSVPetNewData = CSVPetNew.Instance.GetConfData(dataNtf.PetId);
                    if(null != cSVPetNewData)
                    {
                        petName = LanguageHelper.GetTextContent(cSVPetNewData.name);
                    }
                    string content = LanguageHelper.GetTextContent(10995, dataNtf.DiscovererName, LanguageHelper.GetTextContent(cSVPetNewSealData.seal_area_spe), Mathf.Abs(cSVPetNewSealData.coordinate[0] / 100f).ToString(), Mathf.Abs(cSVPetNewSealData.coordinate[1] / 100f).ToString(), petName, petName);
                    Sys_Chat.Instance.PushMessage(ChatType.Notice, Sys_Chat.Instance.gSystemChatBaseInfo, content, Sys_Chat.EMessageProcess.None);
                }
                else
                {
                    DebugUtil.LogFormat(ELogType.eNone , $"CSVPetNewSeal.Data id = {dataNtf.PetId} had bad data ");
                }
            }

            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnPetActivateStory);
        }

        private void OnTriggerNtf(NetMsg msg)
        {
            CmdPetTriggerNtf dataNtf = NetMsgUtil.Deserialize<CmdPetTriggerNtf>(CmdPetTriggerNtf.Parser, msg);
            bool isAutoActive = GetPetIsAutoActive(dataNtf.PetId);
            if (!allBookData.TryGetValue(dataNtf.PetId, out HandbookData handbookData))
            {
                handbookData = new HandbookData();
                handbookData.PetId = dataNtf.PetId;
                handbookData.LoveExp = 0;
                handbookData.LoveBreak = 0;
                handbookData.LoveLevel = 1;
                handbookData.StoryId = 0;
                handbookData.StoryIdIng = 0;
                handbookData.Flag = isAutoActive ? 3u : 2u;
                allBookData.Add(dataNtf.PetId, handbookData);
            }
            else
            {
                if (handbookData.Flag == 1)
                {
                    handbookData.Flag = 3;
                }
                else if(handbookData.Flag == 0)
                {
                    handbookData.Flag = isAutoActive ? 3u : 2u;
                }
            }
        }

        public void OnPetLoveExpUpAllReq()
        {
            CmdPetLoveExpUpAllReq req = new CmdPetLoveExpUpAllReq();
            NetClient.Instance.SendMessage((ushort)CmdPet.LoveExpUpAllReq, req);
        }

        private void PetLoveExpUpAllRes(NetMsg msg)
        {
            CmdPetLoveExpUpAllRes res = NetMsgUtil.Deserialize<CmdPetLoveExpUpAllRes>(CmdPetLoveExpUpAllRes.Parser, msg);
            for (int i = 0; i < res.PetList.Count; i++)
            {
                uint petId = res.PetList[i].PetId;
                if (allBookData.ContainsKey(petId))
                {
                    allBookData[petId].LoveExp = res.PetList[i].LoveExp;
                    allBookData[petId].LoveLevel = res.PetList[i].LoveLevel;
                    allBookData[petId].LoveBreak = res.PetList[i].LoveBreak;
                }
                else
                {
                    bool isAutoActive = GetPetIsAutoActive(petId);
                    HandbookData handbookData = new HandbookData();
                    handbookData.PetId = petId;
                    handbookData.LoveExp = res.PetList[i].LoveExp;
                    handbookData.LoveLevel = res.PetList[i].LoveLevel;
                    handbookData.LoveBreak = res.PetList[i].LoveBreak;
                    handbookData.StoryId = 0;
                    handbookData.StoryIdIng = 0;
                    handbookData.Flag = isAutoActive ? 1u : 0u;
                    allBookData.Add(petId, handbookData);
                }
            }

            // 发送一条已经提升好感度广播
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnPetLoveExpUp);
        }

        public void OnSealStatusChange(Sys_Role.EClientState clientStateId, bool Status)
        {
            ClientStateData clientStateData;
            if (dict_ClientStateData.TryGetValue(clientStateId, out clientStateData))
            {
                clientStateData.isSealState = Status;
            }
        }

        public void SetClientStateAboutSeal(Sys_Role.EClientState clientStateId, bool Status)
        {
            if (clientStateId == Sys_Role.EClientState.None) return;
            Sys_Role.Instance.RoleClientStateReq((uint)clientStateId, Status, clientStateParameter);
        }

        public List<CSVPetNewLoveUp.Data>  GetPetLoveUpDataByPetId(uint petId)
        {
            List<CSVPetNewLoveUp.Data>  temp = new List<CSVPetNewLoveUp.Data>();
            for (int i = 0; i < 999; i++)
            {
                uint id = (uint)(petId * 1000 + i + 1);
                CSVPetNewLoveUp.Data data = CSVPetNewLoveUp.Instance.GetConfData(id);
                if (null != data)
                {
                    temp.Add(data);
                }
                else
                {
                    break;
                }
            }
            return temp;
        }

        public List<SkillClientEx> GetPetAllSkill(uint petId)
        {
            List<SkillClientEx> skillClientIces = new List<SkillClientEx>();
            CSVPetNew.Data cSVPetData = CSVPetNew.Instance.GetConfData(petId);
            if (null != cSVPetData)
            {
                if (null != cSVPetData.unique_skills)
                {
                    for (int i = 0; i < cSVPetData.unique_skills.Count; i++)
                    {
                        SkillClientEx skillClientEx = new SkillClientEx(cSVPetData.unique_skills[i][0], ESkillClientType.Unique);
                        skillClientIces.Add(skillClientEx);
                    }
                }

                if (null != cSVPetData.required_skills)
                {
                    for (int i = 0; i < cSVPetData.required_skills.Count; i++)
                    {
                        SkillClientEx skillClientEx = new SkillClientEx(cSVPetData.required_skills[i][0], ESkillClientType.Noral);
                        skillClientIces.Add(skillClientEx);
                    }
                }
                if (cSVPetData.first_remake_num != 0 && Sys_FunctionOpen.Instance.IsOpen(10581, false))
                {
                    if (null != cSVPetData.remake_skills)
                    {
                        for (int i = 0; i < cSVPetData.remake_skills.Count; i++)
                        {
                            SkillClientEx skillClientEx = new SkillClientEx(cSVPetData.remake_skills[i], ESkillClientType.Build);
                            skillClientIces.Add(skillClientEx);
                        }
                    }
                }

                CSVPetMount.Data cSVPetMountData = CSVPetMount.Instance.GetConfData(petId);
                if (null != cSVPetMountData)
                {
                    if (null != cSVPetMountData.mount_skills)
                    {
                        for (int i = 0; i < cSVPetMountData.mount_skills.Count; i++)
                        {
                            SkillClientEx skillClientEx = new SkillClientEx(cSVPetMountData.mount_skills[i][0], ESkillClientType.Mount);
                            skillClientIces.Add(skillClientEx);
                        }
                    }
                }
                if(cSVPetData.soul_skill_id != 0)
                {
                    SkillClientEx skillClientEx = new SkillClientEx(cSVPetData.soul_skill_id + 4, ESkillClientType.DemonSpiritSkill);
                    skillClientIces.Add(skillClientEx);
                }
            }
            
            return skillClientIces;
        }

        public uint GetPetStoryIdOfPetIdAndScrollNum(uint petId, uint scrollnum)
        {
            return petId * 1000 + scrollnum;
        }

        public List<uint> GetPetAllStoryIdOfPetId(uint petId)
        {
            List<uint> thisBackgroundData = new List<uint>();
            for (uint i = 0; i < 99; i++)
            {
                uint tempUd = GetPetStoryIdOfPetIdAndScrollNum(petId, i);
                CSVPetNewLoveUp.Data temp = CSVPetNewLoveUp.Instance.GetConfData(tempUd);
                if (null != temp)
                {
                    if(temp.contests != 0)
                    {
                        thisBackgroundData.Add(tempUd);
                    }
                }
                else
                {
                    break;
                }
            }
            thisBackgroundData.Sort();
            return thisBackgroundData;
        }

        public List<uint> GetAreaBookList(uint way)
        {
            List<uint> currentList = new List<uint>();
            List<uint> willlActives = new List<uint>();
            List<uint> actives = new List<uint>();
            List<uint> noActives = new List<uint>();

            var petNewDatas = CSVPetNew.Instance.GetAll();
            for (int i = 0, len = petNewDatas.Count; i < len; i++)
            {
                CSVPetNew.Data data = petNewDatas[i];
                if (PetBookChannelShow(data) && (way == 0 || data.haunted_area == way))
                {
                    if (GetPetBookCanActive(data.id))
                    {
                        willlActives.Add(data.id);
                    }
                    else if (GetPetIsActive(data.id))
                    {
                        actives.Add(data.id);
                    }
                    else
                    {
                        noActives.Add(data.id);
                    }

                }
            }

            if(way != 0)
            {
                if (willlActives.Count > 0)
                {
                    willlActives.Sort(AreaSort);
                }
                if (actives.Count > 0)
                {
                    actives.Sort(AreaSort);
                }
                if (noActives.Count > 0)
                {
                    noActives.Sort(AreaSort);
                }
            }
            else
            {
                if (willlActives.Count > 0)
                {
                    willlActives.Sort(AllAreaSort);
                }
                if (actives.Count > 0)
                {
                    actives.Sort(AllAreaSort);
                }
                if (noActives.Count > 0)
                {
                    noActives.Sort(AllAreaSort);
                }
            }

            currentList.AddRange(willlActives);
            currentList.AddRange(actives);
            currentList.AddRange(noActives);
            return currentList;
        }

        private int AreaSort(uint ad, uint bd)
        {
            CSVPetNew.Data a = CSVPetNew.Instance.GetConfData(ad);
            CSVPetNew.Data b = CSVPetNew.Instance.GetConfData(bd);
            int re = -a.card_type.CompareTo(b.card_type);
            if (re == 0)
            {
                re = -a.card_lv.CompareTo(b.card_lv);
                if (re == 0)
                {
                    return -a.id.CompareTo(b.id);
                }
            }
            return re;
        }

        private int AllAreaSort(uint ad, uint bd)
        {
            CSVPetNew.Data a = CSVPetNew.Instance.GetConfData(ad);
            CSVPetNew.Data b = CSVPetNew.Instance.GetConfData(bd);

            int re = a.map_show.CompareTo(b.map_show);
            if (re == 0)
            {
                re = -a.card_type.CompareTo(b.card_type);
                if (re == 0)
                {
                    re = -a.card_lv.CompareTo(b.card_lv);
                    if (re == 0)
                    {
                        return -a.id.CompareTo(b.id);
                    }
                }
            }
            return re;
        }

        //种族
        public List<uint> GetBookList(uint race)
        {
            List<uint> currentList = new List<uint>();
            List<uint> willlActives = new List<uint>();
            List<uint> actives = new List<uint>();
            List<uint> noActives = new List<uint>();

            var datas = CSVPetNew.Instance.GetAll();
            for (int i = 0, len = datas.Count; i < len; i++)
            {
                CSVPetNew.Data data = datas[i];
                if(PetBookChannelShow(data) && (race == 0 || data.race == race))
                {
                    if(GetPetBookCanActive(data.id))
                    {
                        willlActives.Add(data.id);
                    }
                    else if(GetPetIsActive(data.id))
                    {
                        actives.Add(data.id);
                    }
                    else
                    {
                        noActives.Add(data.id);
                    }

                }
            }
            if(race != 0)
            {
                if (willlActives.Count > 0)
                {
                    willlActives.Sort(GenusSort);
                }

                if (actives.Count > 0)
                {
                    actives.Sort(GenusSort);
                }

                if (noActives.Count > 0)
                {
                    noActives.Sort(GenusSort);
                }
            }
            else
            {
                if (willlActives.Count > 0)
                {
                    willlActives.Sort(AllGenusSort);
                }

                if (actives.Count > 0)
                {
                    actives.Sort(AllGenusSort);
                }

                if (noActives.Count > 0)
                {
                    noActives.Sort(AllGenusSort);
                }
            }


            currentList.AddRange(willlActives);
            currentList.AddRange(actives);
            currentList.AddRange(noActives);
            return currentList;
        }

        public List<uint> GetCanSealBookList(List<uint> list)
        {
            List<uint> sealList = new List<uint>();
            for(int i=0;i< list.Count; ++i)
            {
               if(CSVPetNew.Instance.TryGetValue(list[i],out CSVPetNew.Data data)&&data.is_seal==1)
                {
                    sealList.Add(list[i]);
                }
            }
        return sealList;
        }

        private int GenusSort(uint ad, uint bd)
        {
            CSVPetNew.Data a = CSVPetNew.Instance.GetConfData(ad);
            CSVPetNew.Data b = CSVPetNew.Instance.GetConfData(bd);
            int re = - a.card_type.CompareTo(b.card_type);
            if(re == 0)
            {
                re = -a.card_lv.CompareTo(b.card_lv);
                if (re == 0)
                {
                    return -a.id.CompareTo(b.id);
                }
            }
            return re;
        }

        private int AllGenusSort(uint ad, uint bd)
        {
            CSVPetNew.Data a = CSVPetNew.Instance.GetConfData(ad);
            CSVPetNew.Data b = CSVPetNew.Instance.GetConfData(bd);
            int re = a.race.CompareTo(b.race);
            if (re == 0)
            {
                re = - a.card_type.CompareTo(b.card_type);
                if (re == 0)
                {
                    re = -a.card_lv.CompareTo(b.card_lv);
                    if (re == 0)
                    {
                        if (re == 0)
                        {
                            return -a.id.CompareTo(b.id);
                        }
                    }
                }
            }
            return re;
        }

        public List<uint> GetRarytyPetList(uint card_type)
        {
            List<uint> currentList = new List<uint>();
            List<uint> willlActives = new List<uint>();
            List<uint> actives = new List<uint>();
            List<uint> noActives = new List<uint>();

            var datas = CSVPetNew.Instance.GetAll();
            for (int i = 0, len = datas.Count; i < len; i++)
            {
                CSVPetNew.Data data = datas[i];
                if (PetBookChannelShow(data) && (card_type == 0 || data.card_type == card_type))
                {
                    if (GetPetBookCanActive(data.id))
                    {
                        willlActives.Add(data.id);
                    }
                    else if (GetPetIsActive(data.id))
                    {
                        actives.Add(data.id);
                    }
                    else
                    {
                        noActives.Add(data.id);
                    }
                }
            }

            if (card_type != 0)
            {
                if (willlActives.Count > 0)
                {
                    willlActives.Sort(RaceSort);
                }
                if (actives.Count > 0)
                {
                    actives.Sort(RaceSort);
                }
                if (noActives.Count > 0)
                {
                    noActives.Sort(RaceSort);
                }
            }
            else
            {
                if (willlActives.Count > 0)
                {
                    willlActives.Sort(AllRaceSort);
                }
                if (actives.Count > 0)
                {
                    actives.Sort(AllRaceSort);
                }
                if (noActives.Count > 0)
                {
                    noActives.Sort(AllRaceSort);
                }
            }

            currentList.AddRange(willlActives);
            currentList.AddRange(actives);
            currentList.AddRange(noActives);

            return currentList;
        }

        private int RaceSort(uint ad, uint bd)
        {
            CSVPetNew.Data a = CSVPetNew.Instance.GetConfData(ad);
            CSVPetNew.Data b = CSVPetNew.Instance.GetConfData(bd);
            int re = -a.card_type.CompareTo(b.card_type);
            if (re == 0)
            {
                re = -a.card_lv.CompareTo(b.card_lv);
                if (re == 0)
                {
                    return -a.id.CompareTo(b.id);
                }
            }
            return re;
        }

        private int AllRaceSort(uint ad, uint bd)
        {
            CSVPetNew.Data a = CSVPetNew.Instance.GetConfData(ad);
            CSVPetNew.Data b = CSVPetNew.Instance.GetConfData(bd);
            if (a.card_type == b.card_type)
            {
                return b.card_lv - a.card_lv;
            }
            else
            {
                return b.card_type - a.card_type;
            }
        }

        //临时
        public List<uint> GetMountPetList(bool isMount, bool isAll)
        {
            List<uint> currentList = new List<uint>();

            List<uint> MountwilllActives = new List<uint>();
            List<uint> Mountactives = new List<uint>();
            List<uint> MountnoActives = new List<uint>();

            List<uint> noMountwilllActives = new List<uint>();
            List<uint> noMounNoActives = new List<uint>();
            List<uint> noMoutNoActives = new List<uint>();

            var datas = CSVPetNew.Instance.GetAll();
            for (int i = 0, len = datas.Count; i < len; i++)
            {
                CSVPetNew.Data data = datas[i];
                if (PetBookChannelShow(data))
                {
                    if (isAll)
                    {
                        if (GetPetBookCanActive(data.id) && data.mount)
                        {
                            MountwilllActives.Add(data.id);
                        }
                        else if (GetPetIsActive(data.id) && data.mount)
                        {
                            Mountactives.Add(data.id);
                        }
                        else if(data.mount)
                        {
                            MountnoActives.Add(data.id);
                        }
                        else if(GetPetBookCanActive(data.id))
                        {
                            noMountwilllActives.Add(data.id);
                        }
                        else if (!GetPetIsActive(data.id))
                        {
                            noMounNoActives.Add(data.id);
                        }
                        else if (!data.mount)
                        {
                            noMoutNoActives.Add(data.id);
                        }
                    }
                    else if (data.mount == isMount)
                    {
                        if (GetPetBookCanActive(data.id))
                        {
                            MountwilllActives.Add(data.id);
                        }
                        else if (GetPetIsActive(data.id))
                        {
                            Mountactives.Add(data.id);
                        }
                        else
                        {
                            MountnoActives.Add(data.id);
                        }
                    }
                }
            }

            if (MountwilllActives.Count > 0)
            {
                MountwilllActives.Sort(MountSort);
            }
            if (Mountactives.Count > 0)
            {
                Mountactives.Sort(MountSort);
            }
            if (MountnoActives.Count > 0)
            {
                MountnoActives.Sort(MountSort);
            }

            if (noMountwilllActives.Count > 0)
            {
                noMountwilllActives.Sort(MountSort);
            }
            if (noMounNoActives.Count > 0)
            {
                noMounNoActives.Sort(MountSort);
            }
            if (noMoutNoActives.Count > 0)
            {
                noMoutNoActives.Sort(MountSort);
            }

            currentList.AddRange(MountwilllActives);
            currentList.AddRange(Mountactives);
            currentList.AddRange(MountnoActives);
            currentList.AddRange(noMountwilllActives);
            currentList.AddRange(noMounNoActives);
            currentList.AddRange(noMoutNoActives);

            return currentList;
        }

        private int MountSort(uint ad, uint bd)
        {
            CSVPetNew.Data a = CSVPetNew.Instance.GetConfData(ad);
            CSVPetNew.Data b = CSVPetNew.Instance.GetConfData(bd);

            int re = -a.card_type.CompareTo(b.card_type);
            if (re == 0)
            {               
                re = -a.card_lv.CompareTo(b.card_lv);
                if (re == 0)
                {
                    re = a.map_show.CompareTo(b.map_show);
                    if (re == 0)
                    {
                        return -a.id.CompareTo(b.id);
                    }
                }                
            }
            return re;
        }


        public List<uint> GetHauntedAreaList()
        {
            List<uint> vs = new List<uint>();
            //List<CSVPetNewShowFilter.Data>  dataList = new List<CSVPetNewShowFilter.Data>();
            //for (int i = 0; i < CSVPetNewShowFilter.Instance.Count; i++)
            //{
            //    dataList.Add(CSVPetNewShowFilter.Instance[i]);
            //}

            List<CSVPetNewShowFilter.Data>  dataList = new List<CSVPetNewShowFilter.Data>(CSVPetNewShowFilter.Instance.GetAll());
            if (dataList.Count > 1)
            {
                dataList.Sort((a, b) => (int)a.map_sequence - (int)b.map_sequence);
            }
            for (int i = 0; i < dataList.Count; i++)
            {
                vs.Add(dataList[i].id);
            }
            return vs;

        }

        public string GetGrowthValue(uint petid, uint remodelCount)
        {
            return "";
        }

        public int GetPetActiveNum(List<uint> petIds)
        {
            int tempnNum = 0;
            for (int i = 0; i < petIds.Count; i++)
            {
                uint id = petIds[i];
                if (GetPetIsActive(id))
                {
                    tempnNum++;
                }
            }
            return tempnNum;
        }

        public bool GetPetIsActive(uint petId)
        {
            bool isAutoActive = GetPetIsAutoActive(petId);
            if (allBookData.TryGetValue(petId, out HandbookData _bookData))
            {
                return _bookData.Flag != 2 || isAutoActive;
            }
            else
            {
                return isAutoActive;
            }
        }

        public bool IsPetLoveHasEffect(CSVPetNewLoveUp.Data data)
        {
            float tempa = data.id / 1000.0f;
            uint petId = (uint)Math.Floor(tempa);
            HandbookData handData = GetPetBookData(petId);
            if(null == handData)
            {
                return false;
            }
            else
            {
                if(GetPetIsActive(petId) && data.level <= handData.LoveLevel)
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanAutoSeal(uint petId)
        {
            bool isAutoActive = GetPetIsAutoActive(petId);
            if (allBookData.TryGetValue(petId, out HandbookData _bookData))
            {
                return _bookData.Flag == 3 || (isAutoActive && _bookData.Flag == 2);
            }
            return false;
        }

        public HandbookData GetPetBookData(uint petId)
        {
            bool isAutoActive = GetPetIsAutoActive(petId);
            if (allBookData.TryGetValue(petId, out HandbookData _bookData))
            {
                return _bookData;
            }
            else
            {
                if(isAutoActive)
                {
                    _bookData = new HandbookData();
                    _bookData.PetId = petId;
                    _bookData.LoveExp = 0;
                    _bookData.LoveBreak = 0;
                    _bookData.LoveLevel = 0;
                    _bookData.StoryId = 0;
                    _bookData.StoryIdIng = 0;
                    _bookData.Flag = 1;
                    allBookData.Add(petId, _bookData);
                }
            }
            return _bookData;
        }

        public HBSeverData GetPetBookNameData(uint petId)
        {
            for (int i = 0; i < bookNameData.Count; i++)
            {
                HBSeverData data = bookNameData[i];
                if (data.PetId == petId)
                {
                    return data;
                }
            }
            return null;
        }

        public List<uint> GetFriendTask(uint petId)
        {
            List<uint> allIds = new List<uint>();
            HandbookData handbookData = Sys_Pet.Instance.GetPetBookData(petId);
            if (null != handbookData)
            {
                uint nextPetLoveUpId = petId * 1000 + handbookData.LoveBreak + 1;
                uint curPetLoveUpId = petId * 1000 + handbookData.LoveBreak;
                CSVPetNewLoveUp.Data nextData = CSVPetNewLoveUp.Instance.GetConfData(nextPetLoveUpId);
                CSVPetNewLoveUp.Data curData = CSVPetNewLoveUp.Instance.GetConfData(curPetLoveUpId);

                if (null != curData && handbookData.LoveLevel == curData.strengthen_lv)
                {
                    if (null != nextData)
                    {
                        for (int i = 0; i < nextData.breakthrough_mission.Count; i++)
                        {
                            allIds.Add(nextData.breakthrough_mission[i]);
                        }
                    }

                }
            }
            return allIds;
        }

        public bool GetPetBookCanActive(uint petId)
        {
            if(!GetPetIsActive(petId))
            {
                CSVPetNew.Data cSVPetNewData = CSVPetNew.Instance.GetConfData(petId);
                if(null!= cSVPetNewData)
                {
                    uint itemId = cSVPetNewData.PetBooks;
                    if(Sys_Bag.Instance.GetItemCount(itemId) > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private uint configLoveExp = 0;
        public uint ConfigLoveExp
        {
            get
            {
                if (configLoveExp == 0)
                {
                    CSVPetNewParam.Data paramData = CSVPetNewParam.Instance.GetConfData(9u);
                    if (null != paramData)
                    {
                        configLoveExp = paramData.value;
                    }
                }
                return configLoveExp;
            }

            private set { }
        }

        public bool GetPetBookLoveCanUp(uint petId)
        {
            if (GetPetIsActive(petId))
            {
                if (allBookData.TryGetValue(petId, out HandbookData _bookData))
                {
                    CSVPetNew.Data cSVPetNewData = CSVPetNew.Instance.GetConfData(petId);
                    if (null != cSVPetNewData)
                    {
                        long itemNum = 0;
                        uint itemId = cSVPetNewData.PetBooks;
                        itemNum = Sys_Bag.Instance.GetItemCount(itemId);
                        uint petLoveId = petId * 1000 + _bookData.LoveLevel;
                        CSVPetNewLoveUp.Data cSVPetNewLoveUpData = CSVPetNewLoveUp.Instance.GetConfData(petLoveId);
                        if (null != cSVPetNewLoveUpData && cSVPetNewLoveUpData.exp != 0)
                        {
                            uint nextExp = cSVPetNewLoveUpData.exp - _bookData.LoveExp;
                            //if((nextExp > 0) && (itemNum * ConfigLoveExp) >= nextExp)
                            if((nextExp > 0) && itemNum > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    CSVPetNew.Data cSVPetNewData = CSVPetNew.Instance.GetConfData(petId);
                    if (null != cSVPetNewData)
                    {
                        long itemNum = 0;
                        uint itemId = cSVPetNewData.PetBooks;
                        itemNum = Sys_Bag.Instance.GetItemCount(itemId);
                        uint petLoveId = petId * 1000 + 0;
                        CSVPetNewLoveUp.Data cSVPetNewLoveUpData = CSVPetNewLoveUp.Instance.GetConfData(petLoveId);
                        if (null != cSVPetNewLoveUpData && cSVPetNewLoveUpData.exp != 0)
                        {
                            uint nextExp = cSVPetNewLoveUpData.exp - 0;
                            //if((nextExp > 0) && (itemNum * ConfigLoveExp) >= nextExp)
                            if ((nextExp > 0) && itemNum > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool BookAllCanActive()
        {
            //List<uint> values = new List<uint>(CSVPetNew.Instance.Count);
            //for (int i = 0; i < CSVPetNew.Instance.Count; i++)
            //{
            //    values.Add(CSVPetNew.Instance[i].id);
            //
            //}
            //return CheckPetBookListCanActive(values);

            var datas = CSVPetNew.Instance.GetAll();
            for (int i = 0, len = datas.Count; i < len; i++)
            {
                CSVPetNew.Data data = datas[i];
                if (CheckPetBookCanActive(data))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckPetBookCanActive(CSVPetNew.Data data)
        {
            uint petId = data.id;
            if (GetPetBookCanActive(petId) || GetPetBookLoveCanUp(petId))
            {
                return true;
            }
            return false;
        }

        private bool CheckPetBookListCanActive(List<uint> pets)
        {
            if (null == pets)
                return false;
            for (int i = 0; i < pets.Count; i++)
            {
                uint petId = pets[i];
                if (GetPetBookCanActive(petId) || GetPetBookLoveCanUp(petId))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 地区和系别红点
        /// </summary>
        /// <param name="race"></param>
        /// <param name="way"></param>
        /// <returns></returns>
        public bool GetBookRedState(uint race, uint way)
        {
            //List<uint> currentList = new List<uint>();
            //List<CSVPetNew.Data>  datalist = new List<CSVPetNew.Data>();
            //for (int i = 0; i < CSVPetNew.Instance.Count; i++)
            //{
            //    CSVPetNew.Data data = CSVPetNew.Instance[i];
            //    if (PetBookChannelShow(data) && (race == 0 || data.race == race) && (way == 0 || data.haunted_area == way))
            //    {
            //        currentList.Add(data.id);
            //    }
            //}
            //return CheckPetBookListCanActive(currentList);

            var datas = CSVPetNew.Instance.GetAll();
            for (int i = 0, len = datas.Count; i < len; i++)
            {
                CSVPetNew.Data data = datas[i];
                if (PetBookChannelShow(data) && (race == 0 || data.race == race) && (way == 0 || data.haunted_area == way))
                {
                    if (CheckPetBookCanActive(data))
                    {
                        return true;
                    }
                }                    
            }
            return false;
        }

        /// <summary>
        /// 金银卡 红点
        /// </summary>
        /// <param name="card_type"></param>
        /// <returns></returns>
        public bool GetRarytyRedState(uint card_type)
        {
            //List<uint> currentList = new List<uint>();
            //for (int i = 0; i < CSVPetNew.Instance.Count; i++)
            //{
            //    CSVPetNew.Data data = CSVPetNew.Instance[i];
            //   
            //    if (PetBookChannelShow(data) && (card_type == 0 || data.card_type == card_type))
            //    {
            //        currentList.Add(data.id);
            //    }
            //}
            //return CheckPetBookListCanActive(currentList);

            var datas = CSVPetNew.Instance.GetAll();
            for (int i = 0, len = datas.Count; i < len; i++)
            {
                CSVPetNew.Data data = datas[i];
                if (PetBookChannelShow(data) && (card_type == 0 || data.card_type == card_type))
                {
                    if (CheckPetBookCanActive(data))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool GetMountRedState(bool isMout)
        {
            //List<uint> currentList = new List<uint>();
            //for (int i = 0; i < CSVPetNew.Instance.Count; i++)
            //{
            //    CSVPetNew.Data data = CSVPetNew.Instance[i];
            //    if (PetBookChannelShow(data) && data.mount == isMout)
            //    {
            //        currentList.Add(data.id);
            //    }
            //}
            //return CheckPetBookListCanActive(currentList);

            var datas = CSVPetNew.Instance.GetAll();
            for (int i = 0, len = datas.Count; i < len; i++)
            {
                CSVPetNew.Data data = datas[i];
                if (PetBookChannelShow(data) && data.mount == isMout)
                {
                    if (CheckPetBookCanActive(data))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void DoSealPositon()
        {
            switch (clientStateId)
            {
                case Sys_Role.EClientState.CatchPet:
                    {
                        DoSealPositon_CatchPet(clientStateParameter);
                    }
                    break;
                case Sys_Role.EClientState.Hangup:
                    {
                        DoSealPosition_HangupFight(clientStateParameter);
                    }
                    break;
                case Sys_Role.EClientState.ExecTask: {
                        clientStateId = Sys_Role.EClientState.ExecTask;
                    }
                    break;
            }
        }
        public void DoSealPositon_CatchPet(uint _petId)
        {
            CSVPetNewSeal.Data cSVPetSealData = CSVPetNewSeal.Instance.GetConfData(_petId);
            if (null != cSVPetSealData)
            {
                if (null != cSVPetSealData.coordinate)
                {
                    clientStateId = Sys_Role.EClientState.CatchPet;
                    clientStateParameter = _petId;
                    Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;
                    Sys_Pet.Instance.SetClientStateAboutSeal(clientStateId, false); //有状态排斥,可能不需要
                    Sys_Pet.Instance.SetClientStateAboutSeal(clientStateId, true);
                    //GameCenter.pathFindContrl.FindMapPoint(cSVPetSealData.map, new Vector2(cSVPetSealData.coordinate[0] / 100f, cSVPetSealData.coordinate[1] / 100f), (pos) =>
                    {
                        Sys_PathFind.Instance.DoPatrolFind(cSVPetSealData.map, cSVPetSealData.enemy_group);
                    }//);
                }
                else
                {
                    if (cSVPetSealData.map == 0)
                    {
                        DebugUtil.LogErrorFormat("CSVPetSeal 表 id = {0} 未配置 map ", _petId);
                    }
                    else
                    {
                        DebugUtil.LogErrorFormat("CSVPetSeal 表 id = {0} 未配置 coordinate ", _petId);
                    }
                }
            }
        }
        public void DoSealPosition_HangupFight(uint _layerId)
        {
            CSVHangupLayerStage.Data cSVHangupLayerStageData = CSVHangupLayerStage.Instance.GetConfData(_layerId);
            if (null != cSVHangupLayerStageData)
            {
                clientStateId = Sys_Role.EClientState.Hangup;
                clientStateParameter = _layerId;
                Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;
                Sys_Pet.Instance.SetClientStateAboutSeal(clientStateId, false);//有状态排斥,可能不需要
                Sys_Pet.Instance.SetClientStateAboutSeal(clientStateId, true);
                Sys_PathFind.Instance.DoPatrolFind(cSVHangupLayerStageData.Mapid, cSVHangupLayerStageData.EnemyGroupid);
                //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event29);
            }
        }

        public void DoFindMapToSealPath()
        {
            switch (clientStateId)
            {
                case Sys_Role.EClientState.CatchPet:
                    {
                        DoFindMapToSealPath_CatchPet();
                    }
                    break;
                case Sys_Role.EClientState.Hangup:
                    {
                        DoFindMapToSealPath_HangupFight();
                    }
                    break;
                case Sys_Role.EClientState.ExecTask:
                    clientStateId = Sys_Role.EClientState.ExecTask;
                    break;
            }
        }
        public void DoFindMapToSealPath_CatchPet()
        {
            CSVPetNewSeal.Data cSVPetSealData = CSVPetNewSeal.Instance.GetConfData(clientStateParameter);
            if (null != cSVPetSealData)
            {
                Sys_PathFind.Instance.DoPatrolFind(cSVPetSealData.map, cSVPetSealData.enemy_group);
            }
        }
        public void DoFindMapToSealPath_HangupFight()
        {
            CSVHangupLayerStage.Data cSVHangupLayerStageData = CSVHangupLayerStage.Instance.GetConfData(clientStateParameter);
            if (null != cSVHangupLayerStageData)
            {
                Sys_PathFind.Instance.DoPatrolFind(cSVHangupLayerStageData.Mapid, cSVHangupLayerStageData.EnemyGroupid);
            }
        }

        private void OnExitFight()
        {
            ReSeal();
        }


        private float sealDurnTime = -1;
        private float SealDurnTime
        {
            get
            {
                if(sealDurnTime == -1)
                {
                    CSVParam.Data paramData = CSVParam.Instance.GetConfData(956u);
                    if (null != paramData)
                    {
                        sealDurnTime = Convert.ToSingle(paramData.str_value) / 1000f;
                    }
                }
                return sealDurnTime;
            }
        }

        private void ReSeal()
        {
            if (isSealState && isSeal)
            {
                timerSeal = Timer.Register(SealDurnTime, () => { DoFindMapToSealPath(); });
            }
            else if (isSeal)
            {
                timerSeal = Timer.Register(SealDurnTime, () => { DoSealPositon(); });
            }
        }

        private void OnEnterFight(CSVBattleType.Data csv)
        {
            timerSeal?.Cancel();
            timerSeal = null;
            Sys_PathFind.Instance.CloseUI();
        }

        private void OnReconnectStart()
        {
            /*if (null != GameMain.Procedure && null != GameMain.Procedure.CurrentProcedure && GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal)
            {
                clientStateId = Sys_Role.EClientState.None;
            }
            ChackStopSeal();*/
            if(isSeal)
            {
                Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;
                GameCenter.mPathFindControlSystem?.Interupt();
            }
            isBookDataInit = false;
        }

        private void OnReconnectResult(bool result)
        {
            if (result)
            {
                timerSeal?.Cancel();
                ReSeal();
                OnPetGetHandbookReq();
            }
        }

        public void TryStopSeal()
        {
            if (isSeal)
            {
                if (null != GameMain.Procedure && null != GameMain.Procedure.CurrentProcedure && GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal)
                {
                    timerSeal?.Cancel();
                    if (isSealState)
                    {
                        Sys_Pet.Instance.SetClientStateAboutSeal(clientStateId, false);
                    }
                    clientStateId = Sys_Role.EClientState.None;

                }
            }
        }

        public void ForceStop() {
            Sys_Pet.Instance.SetClientStateAboutSeal(clientStateId, false);
            clientStateId = Sys_Role.EClientState.None;
        }

        private void OnLeftJoystick(UnityEngine.Vector2 v2, float f)
        {
            // 手动点击寻路，或者手动操作摇杆的时候会停止当前巡逻
            TryStopSeal();
        }
        private void OnRightJoystick(UnityEngine.Vector2 v2, float f)
        {
            // 手动点击寻路，或者手动操作摇杆的时候会停止当前巡逻
            TryStopSeal();
        }

        private void OnTouchTerrain(int layerMask)
        {
            // 手动点击寻路，或者手动操作摇杆的时候会停止当前巡逻
            TryStopSeal();
        }

        //   BeMember, 成为队⚪
        private void OnBeMember()
        {
            // 巡逻时成为队员也要停止
            TryStopSeal();
        }

        private void OnStartExecTask(bool auto)
        {
            //巡逻时点击任务  暂时停下 ，这时会任务巡路
            if (!auto) {
                TryStopSeal();
            }
        }

        private void OnFunctionOpen(bool value)
        {
            //功能开启时 可能会表现问题 暂时停下 有问题
            if (value)
                OnEnterFight(null);
            else
                OnExitFight();
        }

        private bool PetBookChannelShow(CSVPetNew.Data data)
        {
            string channel = SDKManager.GetChannel();
            bool hasChannel = null != data.SubPackageShow;
            return data.show_pet && (!hasChannel || (hasChannel && data.SubPackageShow.Contains(channel)));
        }

        /// <summary>
        /// 宠物是否默认激活图鉴
        /// </summary>
        /// <param name="petId"></param>
        /// <returns></returns>
        private bool GetPetIsAutoActive(uint petId)
        {
            CSVPetNew.Data cSVPetNewData = CSVPetNew.Instance.GetConfData(petId);
            return null != cSVPetNewData && cSVPetNewData.PetBooks_is_act == 1;
        }
    }
}
