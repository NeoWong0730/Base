using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using Net;
using Packet;

namespace Logic
{
    public partial class Sys_Knowledge : SystemModuleBase<Sys_Knowledge>
    {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnSelectType,
            OnSelectSubType,
            OnActiveKnowledge,
            OnTakeRewardNtf,
            OnDelNewKnowledgeNtf,
            OnFragmentRedPointUpdate,
            OnAnnalsRedPointUpdate,
        }

        public enum ETypes
        {
            None,
            Gleanings,  //沧海拾遗
            Annals,     //魔力纪年
            Fragment,   //记忆碎片
            Brave,      //勇者传记
            Cooking,    //烹饪
            PetBook,    //宠物图鉴
            Achievement,//成就
        }

        private List<KnowledgeComment> listDatas = new List<KnowledgeComment>(64);

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdKnowledge.ListNtf, OnListNtf, CmdKnowledgeListNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdKnowledge.OneActiveNtf, OnOneActiveNtf, CmdKnowledgeOneActiveNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdKnowledge.TakeAwardReq, (ushort)CmdKnowledge.TakeAwardRes, OnTakeRewardRes, CmdKnowledgeTakeAwardRes.Parser);

            //CalKnowledgeTypes();
        }

        public override void OnLogin()
        {
            //listDatas.Clear();

            //初始化魔力纪年
            InitAnnals();

            //初始化记忆碎片
            InitFragment();

            //沧海拾遗
            InitGleanings();
        }

        private void OnListNtf(NetMsg msg)
        {
            listDatas.Clear();

            CmdKnowledgeListNtf ntf = NetMsgUtil.Deserialize<CmdKnowledgeListNtf>(CmdKnowledgeListNtf.Parser, msg);
            listDatas.AddRange(ntf.Knowledgelist);
        }

        private void OnOneActiveNtf(NetMsg msg)
        {
            CmdKnowledgeOneActiveNtf ntf = NetMsgUtil.Deserialize<CmdKnowledgeOneActiveNtf>(CmdKnowledgeOneActiveNtf.Parser, msg);

            bool isExistType = false;
            for (int i = 0; i < listDatas.Count; ++i)
            {
                if (ntf.Type == listDatas[i].Type)
                {
                    isExistType = true;
                    listDatas[i].ActiveList.Add(ntf.Id);
                    listDatas[i].ShowNewList.Add(ntf.Id);
                }
            }

            if (!isExistType)
            {
                KnowledgeComment activeComment = new KnowledgeComment();
                activeComment.Type = ntf.Type;
                activeComment.ActiveList.Add(ntf.Id);
                activeComment.Awardtake = 0u;
                activeComment.ShowNewList.Add(ntf.Id);

                listDatas.Add(activeComment);
            }

            Sys_CommonTip.Instance.TipKnowledge((ETypes)ntf.Type, ntf.Id);
            eventEmitter.Trigger(EEvents.OnActiveKnowledge);
        }

        /// <summary>
        /// 领取小知识奖励
        /// </summary>
        /// <param name="infoId"></param>
        public void OnTakeAwardReq(ETypes eType)
        {
            CmdKnowledgeTakeAwardReq req = new CmdKnowledgeTakeAwardReq();
            req.Type = (uint)eType;
            NetClient.Instance.SendMessage((ushort)CmdKnowledge.TakeAwardReq, req);
        }

        /// <summary>
        /// 领取小知识奖励response
        /// </summary>
        /// <param name="msg"></param>
        private void OnTakeRewardRes(NetMsg msg)
        {
            CmdKnowledgeTakeAwardRes ntf = NetMsgUtil.Deserialize<CmdKnowledgeTakeAwardRes>(CmdKnowledgeTakeAwardRes.Parser, msg);

            for (int i = 0; i < listDatas.Count; ++i)
            {
                if (listDatas[i].Type == ntf.Type)
                {
                    listDatas[i].Awardtake = ntf.Awardtake;
                    break;
                }
            }

            eventEmitter.Trigger(EEvents.OnTakeRewardNtf);
        }

        /// <summary>
        /// 取消小知识红点
        /// </summary>
        /// <param name="type"></param>
        /// <param name="knowledgeId"></param>
        public void OnClearNewKnoledge(ETypes type, uint knowledgeId)
        {
            CmdKnowledgeClearNewReq req = new CmdKnowledgeClearNewReq();
            req.Type = (uint)type;
            req.Id = knowledgeId;
            NetClient.Instance.SendMessage((ushort)CmdKnowledge.ClearNewReq, req);
        }

        /// <summary>
        /// 判断小知识是否激活
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <returns></returns>
        public bool IsKnowledgeActive(uint knowledgeId)
        {
            bool isActive = false;
            for (int i = 0; i < listDatas.Count; ++i)
            {
                int index = listDatas[i].ActiveList.IndexOf(knowledgeId);
                if (index >= 0)
                {
                    isActive = true;
                    break;
                }
            }

            return isActive;
        }

        /// <summary>
        /// 通过类型获得小知识总数量
        /// </summary>
        /// <param name="etype"></param>
        /// <returns></returns>
        public int GetEventsCount(ETypes etype)
        {
            int count = 0;

            switch (etype)
            {
                case ETypes.Gleanings:
                    foreach (var data in dictGleanings)
                    {
                        foreach (var temp in data.Value)
                        {
                            count += temp.Value.Count;
                        }
                    }
                    break;
                case ETypes.Annals:
                    foreach (var data in dictAnnals)
                    {
                        count += data.Value.Count;
                    }
                    break;
                case ETypes.Fragment:
                    foreach (var data in dictFragments)
                    {
                        count += data.Value.Count;
                    }
                    break;
                case ETypes.Brave:
                    foreach (var data in CSVBrave.Instance.GetAll())
                    {
                        if (data.story_id != null)
                            count += data.story_id.Count;
                    }
                    break;
            }

            return count;
        }

        /// <summary>
        /// 通过类型获得小知识激活数量
        /// </summary>
        /// <param name="etype"></param>
        /// <returns></returns>

        public int GetUnlockEventsCount(ETypes etype)
        {
            int count = 0;

            for (int i = 0; i < listDatas.Count; ++i)
            {
                if (listDatas[i].Type == (uint)etype)
                {
                    count += listDatas[i].ActiveList.Count;
                    break;
                }
            }

            return count;
        }


        /// <summary>
        /// 判断不同类型的领取阶段
        /// </summary>
        /// <param name="eType"></param>
        /// <returns></returns>
        public int GetStageReward(ETypes eType)
        {
            int stage = 0;
            for (int i = 0; i < listDatas.Count; ++i)
            {
                if((uint)eType == listDatas[i].Type)
                {
                    stage = (int)listDatas[i].Awardtake;
                    break;
                }
            }

            return stage;
        }
    }
}