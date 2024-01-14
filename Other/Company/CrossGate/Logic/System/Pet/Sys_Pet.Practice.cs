using Logic.Core;
using Net;
using Packet;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public partial class Sys_Pet : SystemModuleBase<Sys_Pet>
    {
        private int maxBuildCout = 0;
        /// <summary>
        /// 最大改造数量-配置（宠物单独有个最大改造数量）
        /// </summary>
        public int MaxBuildCout
        {
            get
            {
                if (maxBuildCout == 0)
                {
                    maxBuildCout = CSVPetNewReBuild.Instance.Count;
                }
                return maxBuildCout;
            }

            private set { }
        }

        private List<uint> buildSkillNum;
        /// <summary>
        /// 改造次数增加技能数量 n
        /// </summary> 
        public List<uint> BuildSkillNum
        {
            get
            {
                if (buildSkillNum == null)
                {
                    var petNewReBuildDatas = CSVPetNewReBuild.Instance.GetAll();

                    buildSkillNum =  new List<uint>(petNewReBuildDatas.Count);

                    
                    for (int i = 0, len = petNewReBuildDatas.Count; i < len; i++)
                    {
                        if(i > 0)
                        {
                            buildSkillNum.Add(petNewReBuildDatas[i].max_skill - petNewReBuildDatas[i - 1].max_skill);
                        }
                        else
                        {
                            buildSkillNum.Add(petNewReBuildDatas[i].max_skill);
                        }

                    }
                }
                return buildSkillNum;
            }

            private set { }
        }

        private List<uint> buildCount2SkillNum;
        /// <summary>
        /// 改造次数对应技能上限 n+1
        /// </summary>
        public List<uint> BuildCount2SkillNum
        {
            get
            {
                if (buildCount2SkillNum == null)
                {
                    var petNewReBuildDatas = CSVPetNewReBuild.Instance.GetAll();
                    buildCount2SkillNum = new List<uint>(petNewReBuildDatas.Count);
                    for (int i = 0, len = petNewReBuildDatas.Count; i < len; i++)
                    {
                        buildCount2SkillNum.Add(petNewReBuildDatas[i].max_skill);
                    }
                    buildCount2SkillNum.Sort();
                }
                return buildCount2SkillNum;
            }

            private set { }
        }

        private List<uint> buildSkillNumByIndex;
        /// <summary>
        /// 获取下标对应的改造次数
        /// </summary>
        public List<uint> BuildSkillNumByIndex
        {
            get
            {
                if (buildSkillNumByIndex == null)
                {
                    buildSkillNumByIndex = new List<uint>(10);
                    //1 2 2 3 2
                    var skillNums = BuildSkillNum;
                    var count = skillNums.Count;
                    for (int i = 0; i < count; i++)
                    {
                        for (int j = 0; j < skillNums[i]; j++)
                        {
                            buildSkillNumByIndex.Add((uint)i + 1);
                        }
                    }
                }
                return buildSkillNumByIndex;
            }

            private set { }
        }

        private List<uint> gradeStarColorByCount;
        /// <summary>
        /// 改造数量的最低档位代表的颜色 配置值为达到的最低值
        /// </summary>
        public List<uint> GradeStarColorByCount
        {
            get
            {
                if (gradeStarColorByCount == null)
                {
                    gradeStarColorByCount = ReadHelper.ReadArray_ReadUInt(CSVPetNewParam.Instance.GetConfData(52).str_value, '|');
                }
                return gradeStarColorByCount;
            }
            private set { }
        }

        private uint recastNeedBuildCount;

        /// <summary>
        /// 重塑所需的最小改造次数
        /// </summary>
        public uint RecastNeedBuildCount
        {
            get
            {
                if(recastNeedBuildCount == 0)
                {
                    recastNeedBuildCount = CSVPetNewParam.Instance.GetConfData(53).value;
                }
                return recastNeedBuildCount;
            }
        }

        private List<List<uint>> recastCostItemAndIds;
        public List<List<uint>> RecastCostItemAndIds
        {
            get
            {
                if(null == recastCostItemAndIds)
                {
                    recastCostItemAndIds = ReadHelper.ReadArray2_ReadUInt(CSVPetNewParam.Instance.GetConfData(54).str_value, '|', '&');
                }
                return recastCostItemAndIds;
            }
        }


        private int[] buildGradeLimits = null;

        public int[] BuildGradeLimits
        {
            get
            {
                if (null == buildGradeLimits)
                {
                    CSVPetNewParam.Data param = CSVPetNewParam.Instance.GetConfData(30u);
                    string[] strs = param.str_value.Split('|');
                    buildGradeLimits = new int[strs.Length];
                    for (int i = 0; i < strs.Length; i++)
                    {
                        buildGradeLimits[i] = int.Parse(strs[i]);
                    }
                }
                return buildGradeLimits;
            }

            private set { }
        }

        private uint reBuildItemId = 0;
        public uint ReBuildItemId
        {
            get
            {
                if (reBuildItemId == 0)
                {
                    reBuildItemId = CSVPetNewParam.Instance.GetConfData(17).value;
                }
                return reBuildItemId;
            }

            private set { }
        }

        private uint petSkillExpItemId = 0;
        public uint PetSkillExpItemId
        {
            get
            {
                if (petSkillExpItemId == 0)
                {
                    petSkillExpItemId = CSVPetNewParam.Instance.GetConfData(38u).value;
                }
                return petSkillExpItemId;
            }

            private set { }
        }

        private List<uint> dontBeUseUpGradePetSkillExpList;
        public List<uint> DontBeUseUpGradePetSkillExpList
        {
            get
            {
                if (null == dontBeUseUpGradePetSkillExpList)
                {
                    dontBeUseUpGradePetSkillExpList = ReadHelper.ReadArray_ReadUInt(CSVPetNewParam.Instance.GetConfData(43).str_value, '|');
                }
                return dontBeUseUpGradePetSkillExpList;
            }

            private set { }
        }

        public bool isShowRemakeTips = false;
        public bool isShowRemakeSkillTips = false;
        public bool isShowRemakePerfectTips = false;
        public ClientPet remakePet;
        public void OnPetLearnSkillReq(uint uid, uint itemId)
        {
            
            CmdPetLearnSkillReq cmdPetLearnSkillReq = new CmdPetLearnSkillReq();
            cmdPetLearnSkillReq.Uid = uid;
            cmdPetLearnSkillReq.ItemId = itemId;
            NetClient.Instance.SendMessage((ushort)CmdPet.LearnSkillReq, cmdPetLearnSkillReq);
        }

        private void PetLearnSkillRes(NetMsg msg)
        {
            CmdPetLearnSkillRes dataRes = NetMsgUtil.Deserialize<CmdPetLearnSkillRes>(CmdPetLearnSkillRes.Parser, msg);
            /*if (dataRes.SkillType == 1)
            {
                Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event17);
            }
            else if(dataRes.SkillType == 2)
            {
                Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event45);
            }   */         
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnLearnSkill); 
        }

        public void OnPetRemakeReq(uint petUid, uint itemId, uint remakeIndex)
        {
            if (CheckIsLimitPet(petUid))
                return;
            CmdPetRemakeReq cmdPetRemakeReq = new CmdPetRemakeReq();
            cmdPetRemakeReq.Uid = petUid;
            cmdPetRemakeReq.ItemId = itemId;
            cmdPetRemakeReq.Index = remakeIndex;
            NetClient.Instance.SendMessage((ushort)CmdPet.RemakeReq, cmdPetRemakeReq);
        }

        private void PetRemakeRes(NetMsg msg)
        {
            CmdPetRemakeRes dataRes = NetMsgUtil.Deserialize<CmdPetRemakeRes>(CmdPetRemakeRes.Parser, msg);

            for (int i = 0; i < petsList.Count; i++)
            {
                var clientPet = petsList[i];
                if(dataRes.Uid == clientPet.GetPetUid())
                {
                    clientPet.SetBuildEndData(dataRes);
                }
            }

            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10959));
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event44);
            eventEmitter.Trigger(EEvents.OnUpdatePetInfo);
        }

        public void OnPetReRemakeReq(uint petUid)
        {
            if (CheckIsLimitPet(petUid))
                return;
            CmdPetRemakeResetReq req = new CmdPetRemakeResetReq();
            req.Uid = petUid;
            NetClient.Instance.SendMessage((ushort)CmdPet.RemakeResetReq, req);
        }

        public void OnPetReRemakeRes(NetMsg msg)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10994));
            Sys_Hint.Instance.PushEffectInNextFight();  //战斗内提示：当前战斗不生效，将在下一场战斗生效
        }

        /// <summary>
        /// 请求改造选择 
        /// </summary>
        /// <param name="petUid"></param>
        /// <param name="select">0-放弃,1-确定</param>
        public void PetRemakeSelectReq(uint petUid, uint select)
        {
            if (CheckIsLimitPet(petUid))
                return;
            CmdPetRemakeSelectReq req = new CmdPetRemakeSelectReq();
            req.Uid = petUid;
            req.Select = select;
            NetClient.Instance.SendMessage((ushort)CmdPet.RemakeSelectReq, req);
        }

        public void OnPetRemakeSelectRes(NetMsg msg)
        {
            CmdPetRemakeSelectRes res = NetMsgUtil.Deserialize<CmdPetRemakeSelectRes>(CmdPetRemakeSelectRes.Parser, msg);
            for (int i = 0; i < petsList.Count; i++)
            {
                var clientPet = petsList[i];
                if (clientPet.GetPetUid() == res.Uid)
                {
                    if(res.Select == 0)
                    {
                        clientPet.petUnit.BuildInfo.IntenGradeTemp = 0;
                        clientPet.petUnit.BuildInfo.MagicGradeTemp = 0;
                        clientPet.petUnit.BuildInfo.SnhGradeTemp = 0;
                        clientPet.petUnit.BuildInfo.VitGradeTemp = 0;
                        clientPet.petUnit.BuildInfo.SpeedGradeTemp = 0;
                        clientPet.petUnit.BuildInfo.SkillTemp.Clear();

                        clientPet.petUnit.BuildInfo.VitGradeTempTotal = 0;
                        clientPet.petUnit.BuildInfo.SnhGradeTempTotal = 0;
                        clientPet.petUnit.BuildInfo.IntenGradeTempTotal = 0;
                        clientPet.petUnit.BuildInfo.SpeedGradeTempTotal = 0;
                        clientPet.petUnit.BuildInfo.MagicGradeTempTotal = 0;

                        clientPet.petUnit.BuildInfo.Index = 0;
                    }

                    if(followPetUid != 0 && res.Uid == followPetUid)
                    {
                        eventEmitter.Trigger(EEvents.OnPerFectFxChange, 2);
                    }
                    else if(mountPetUid != 0 && res.Uid == mountPetUid)
                    {
                        eventEmitter.Trigger(EEvents.OnPerFectFxChange, 1);
                    }
                    clientPet.petUnit.BuildInfo.BuildState = (uint)EnumPetBuildState.None;
                }
            }
            if (res.Select == 0)
            {
                //选择结束
                eventEmitter.Trigger(EEvents.OnUpdatePetInfo);
            }
        }

        /// <summary>
        /// 改造领悟技能
        /// </summary>
        /// <param name="petUid"></param>
        /// <param name="position"></param>
        /// <param name="learnType"></param>
        /// <param name="itemId"></param>
        public void PetRemakeLearnSkillReq(uint petUid, uint position, uint learnType, uint itemId)
        {
            CmdPetRemakeLearnSkillReq req = new CmdPetRemakeLearnSkillReq();
            req.Uid = petUid;
            req.Position = position;
            req.LearnType = learnType;
            req.SkillBookItemId = itemId;
            NetClient.Instance.SendMessage((ushort)CmdPet.RemakeLearnSkillReq, req);
        }

        public void OnPetRemakeLearnSkillRes(NetMsg msg)
        {
            CmdPetRemakeLearnSkillRes res = NetMsgUtil.Deserialize<CmdPetRemakeLearnSkillRes>(CmdPetRemakeLearnSkillRes.Parser, msg);
            for (int i = 0; i  < petsList.Count; i ++)
            {
                var clientPet = petsList[i];
                if(clientPet.GetPetUid() == res.Uid)
                {
                    clientPet.petUnit.BuildInfo.SkillPosition = res.Position;
                    clientPet.petUnit.BuildInfo.SkillTemp.Clear();
                    clientPet.petUnit.BuildInfo.SkillTemp.AddRange(res.SkillList);
                    // 成功失败
                    bool IsSucc = res.Succ;
                    //状态改变
                    var buildState = (EnumPetBuildState)clientPet.petUnit.BuildInfo.BuildState;
                    if (IsSucc)
                    {
                        if (buildState == EnumPetBuildState.None)
                        {
                            clientPet.petUnit.BuildInfo.BuildState = (uint)EnumPetBuildState.LearnSkill;
                        }
                        /*else if (buildState == EnumPetBuildState.ReGrade)
                        {
                            clientPet.petUnit.BuildInfo.BuildState = (uint)EnumPetBuildState.LearnSkillReGrade;
                        }*/
                    }
                    else
                    {
                        if (buildState == EnumPetBuildState.LearnSkill)
                        {
                            clientPet.petUnit.BuildInfo.BuildState = (uint)EnumPetBuildState.None;
                        }
                        /*else if (buildState == EnumPetBuildState.LearnSkillReGrade)
                        {
                            clientPet.petUnit.BuildInfo.BuildState = (uint)EnumPetBuildState.ReGrade;
                        }*/
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12026));
                    }
                    
                }
            }
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event54);
            eventEmitter.Trigger(EEvents.OnPetRemakeSkillEnd, res.Succ);
        }

        /// <summary>
        /// 领悟后选择学习技能
        /// </summary>
        /// <param name="petUid"></param>
        /// <param name="position"></param>
        /// <param name="learnType"></param>
        /// <param name="itemId"></param>
        public void PetRemakeSelectSkillReq(uint petUid, uint position, uint skillId)
        {
            CmdPetRemakeSelectSkillReq req = new CmdPetRemakeSelectSkillReq();
            req.Uid = petUid;
            req.Position = position;
            req.SelectSkillId = skillId;
            NetClient.Instance.SendMessage((ushort)CmdPet.RemakeSelectSkillReq, req);
        }

        public void OnPetRemakeSelectSkillRes(NetMsg msg)
        {
            CmdPetRemakeSelectSkillRes res = NetMsgUtil.Deserialize<CmdPetRemakeSelectSkillRes>(CmdPetRemakeSelectSkillRes.Parser, msg);
            Sys_Hint.Instance.PushEffectInNextFight();  //战斗内提示：当前战斗不生效，将在下一场战斗生效
            for (int i = 0; i < petsList.Count; i++)
            {
                var clientPet = petsList[i];
                if (clientPet.GetPetUid() == res.Uid)
                {
                    clientPet.petUnit.BuildInfo.SkillPosition = res.Position;
                    if (res.SelectSkillId == 0)
                    {
                        clientPet.petUnit.BuildInfo.SkillTemp.Clear();
                    }
                    else
                    {
                        int index = (int)res.Position;
                        clientPet.petUnit.BuildInfo.BuildSkills.Insert(index, res.SelectSkillId);
                        clientPet.petUnit.BuildInfo.BuildSkills.RemoveAt(index + 1);
                    }
                    //状态改变
                    var buildState = (EnumPetBuildState)clientPet.petUnit.BuildInfo.BuildState;
                    if (buildState == EnumPetBuildState.LearnSkill)
                    {
                        clientPet.petUnit.BuildInfo.BuildState = (uint)EnumPetBuildState.None;
                    }
                    /*else if (buildState == EnumPetBuildState.LearnSkillReGrade)
                    {
                        clientPet.petUnit.BuildInfo.BuildState = (uint)EnumPetBuildState.ReGrade;
                    }*/
                }
            }
            eventEmitter.Trigger(EEvents.OnUpdatePetInfo);
        }

        /// <summary>
        /// 重塑档位
        /// </summary>
        /// <param name="petUid"></param>
        public void PetRemakeReGradeReq(uint petUid)
        {
            CmdPetRemakeReGradeReq req = new CmdPetRemakeReGradeReq();
            req.Uid = petUid;
            NetClient.Instance.SendMessage((ushort)CmdPet.RemakeReGradeReq, req);
        }

        public void OnPetRemakeReGradeRes(NetMsg msg)
        {
            CmdPetRemakeReGradeRes res = NetMsgUtil.Deserialize<CmdPetRemakeReGradeRes>(CmdPetRemakeReGradeRes.Parser, msg);

            for (int i = 0; i < petsList.Count; i++)
            {
                var clientPet = petsList[i];
                if (clientPet.GetPetUid() == res.Uid)
                {
                    clientPet.ResetBuildGradeTemp(res);
                    //状态改变
                    var buildState = (EnumPetBuildState)clientPet.petUnit.BuildInfo.BuildState;
                    if (buildState == EnumPetBuildState.LearnSkill)
                    {
                        //clientPet.petUnit.BuildInfo.BuildState = (uint)EnumPetBuildState.LearnSkillReGrade;
                    }
                    else
                    {
                        clientPet.petUnit.BuildInfo.BuildState = (uint)EnumPetBuildState.ReGrade;
                    }
                }
            }
            eventEmitter.Trigger(EEvents.OnUpdatePetInfo);
        }

        /// <summary>
        /// 选择重塑选择
        /// </summary>
        /// <param name="petUid"></param>
        /// <param name="select">//0-放弃,1-确定</param>
        public void PetRemakeSelectGradeReq(uint petUid, uint select)
        {
            CmdPetRemakeSelectGradeReq req = new CmdPetRemakeSelectGradeReq();
            req.Uid = petUid;
            req.Select = select;
            NetClient.Instance.SendMessage((ushort)CmdPet.RemakeSelectGradeReq, req);
        }

        public void OnPetRemakeSelectGradeRes(NetMsg msg)
        {
            CmdPetRemakeSelectGradeRes res = NetMsgUtil.Deserialize<CmdPetRemakeSelectGradeRes>(CmdPetRemakeSelectGradeRes.Parser, msg);
            Sys_Hint.Instance.PushEffectInNextFight();  //战斗内提示：当前战斗不生效，将在下一场战斗生效
            for (int i = 0; i < petsList.Count; i++)
            {
                var clientPet = petsList[i];
                if (clientPet.GetPetUid() == res.Uid)
                {
                    clientPet.ResetBuildGrade(res);
                    //状态改变
                    var buildState = (EnumPetBuildState)clientPet.petUnit.BuildInfo.BuildState;
                    if (buildState == EnumPetBuildState.ReGrade)
                    {
                        clientPet.petUnit.BuildInfo.BuildState = (uint)EnumPetBuildState.None;
                    }
                    /*else if(buildState == EnumPetBuildState.LearnSkillReGrade)
                    {
                        clientPet.petUnit.BuildInfo.BuildState = (uint)EnumPetBuildState.LearnSkill;
                    }*/
                }
            }
            //选择结束
            eventEmitter.Trigger(EEvents.OnUpdatePetInfo);
        }

        public void PetEatFruitReq(uint petUid, uint itemId)
        {
            CmdPetEatFruitReq req = new CmdPetEatFruitReq();
            req.Uid = petUid;
            req.Itemid = itemId;
            NetClient.Instance.SendMessage((ushort)CmdPet.EatFruitReq, req);
        }

        public void OnPetEatFruitRes(NetMsg msg)
        {
            CmdPetEatFruitRes res = NetMsgUtil.Deserialize<CmdPetEatFruitRes>(CmdPetEatFruitRes.Parser, msg);
            Sys_Hint.Instance.PushEffectInNextFight();  //战斗内提示：当前战斗不生效，将在下一场战斗生效
            for (int i = 0; i < petsList.Count; i++)
            {
                var clientPet = petsList[i];
                if (clientPet.GetPetUid() == res.Uid)
                {
                    clientPet.ResetBuildGrade(res);
                }
            }
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(15106, LanguageHelper.GetTextContent(2011135 + res.MinusGrade)));
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(15107, LanguageHelper.GetTextContent(2011135 + res.AddGrade)));
            List<uint> tempList = new List<uint>();
            tempList.Add(res.MinusGrade);
            if (res.MinusGrade != res.AddGrade)
            {
                tempList.Add(res.AddGrade);
            }
            eventEmitter.Trigger(EEvents.OnEatFruitEnd, tempList);
            eventEmitter.Trigger(EEvents.OnUpdatePetInfo);
        }

        public uint GetLuckValue(uint rebuidId)
        {
            CSVPetNewReBuild.Data rebuildData = CSVPetNewReBuild.Instance.GetConfData(rebuidId);
            if(null != rebuildData)
            {
                return rebuildData.max_luck;
            }
            else
            {
                return 0;
            }
        }

        public uint GetReBuildValue(uint rebuidId)
        {
            CSVPetNewReBuild.Data rebuildData = CSVPetNewReBuild.Instance.GetConfData(rebuidId);
            if (null != rebuildData)
            {
                return rebuildData.remake_value;
            }
            else
            {
                return 1;
            }
        }

        public uint GetNextUnlockRenakeLevel(uint rebuidId)
        {
            CSVPetNewReBuild.Data rebuildData = CSVPetNewReBuild.Instance.GetConfData(rebuidId);
            if (null != rebuildData)
            {
                return rebuildData.need_pet_lv;
            }
            else
            {
                return 0;
            }
        }

        public CSVPetNewReBuild.Data GetNextUnlockRemakeData(uint rebuidId)
        {
            CSVPetNewReBuild.Data rebuildData = CSVPetNewReBuild.Instance.GetConfData(rebuidId);
            if (null != rebuildData)
            {
                return rebuildData;
            }
            else
            {
                return null;
            }
        }

        public List<uint> GetRemakeSkillNums(CSVPetRemake.Data data)
        {
            if (null != data && null != data.skill_weight)
            {
                var start = data.skill_weight.FindIndex(a => a != 0);
                var end = data.skill_weight.FindLastIndex(a => a != 0);

                return new List<uint>(2) { data.add_skill_num[start], data.add_skill_num[end] };
            }

            return new List<uint>(2) { 0, 0 };
        }

        /// <summary>
        /// 通过次数获取改造书数据
        /// </summary>
        /// <param name="currentTimes"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public CSVPetRemake.Data GetRemakeData(uint currentTimes, uint itemId)
        {
            var petRemakeDatas = CSVPetRemake.Instance.GetAll();
            for (int i = 0, len = petRemakeDatas.Count; i < len; i++)
            {
                var data = petRemakeDatas[i];
                if (currentTimes == data.remake_num && itemId == data.item_id)
                {
                    return data;
                }
            }
            return null;
        }

        // 返回改造档位指定的颜色星图
        public uint GetGradeStarImageId(uint grade)
        {
            int index = GetGradeStarIndex(grade);
            switch (index)
            {
                case -1:
                    return 2551;
                case 0:
                    return 2552;
                case 1:
                    return 2553;
                case 2:
                    return 2554;
                case 3:
                    return 2555;
            }
            return 2551;
        }

        /// <summary>
        /// 返回改造档位指定的颜色文字风格id
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        public uint GetGradeStarTextStyleId(uint grade)
        {
            int index = GetGradeStarIndex(grade);
            switch (index)
            {
                case -1:
                    return 119;
                case 0:
                    return 120;
                case 1:
                    return 121;
                case 2:
                    return 122;
                case 3:
                    return 123;
            }
            return 2551;
        }

        /// <summary>
        /// 返回星底板的颜色
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        public Color GetGradeStarBgColor(uint grade)
        {
            int index = GetGradeStarIndex(grade);
            switch (index)
            {
                case -1:
                    return new Color(188 / 255.0f,172 / 255.0f, 144 / 255.0f, 255 / 255.0f);
                case 0:
                    return new Color(93 / 255.0f, 185 / 255.0f, 130 / 255.0f, 255 / 255.0f);
                case 1:
                    return new Color(106 / 255.0f, 136 / 255.0f, 241 / 255.0f, 255 / 255.0f);
                case 2:
                    return new Color(245 / 255.0f, 142 / 255.0f, 239 / 255.0f, 255 / 255.0f);
                case 3:
                    return new Color(255 / 255.0f, 194 / 255.0f, 93 / 255.0f, 255 / 255.0f);
            }
            return new Color(188 / 255.0f, 172 / 255.0f, 144 / 255.0f, 255 / 255.0f);
        }

        /// <summary>
        /// 获取改造档位数量对应星星颜色index
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        public int GetGradeStarIndex(uint grade)
        {
            var lst = GradeStarColorByCount;
            int index = -1;
            for (int i = lst.Count - 1; i >= 0; i--)
            {
                if (grade >= lst[i])
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// 返回改造评级对应印章
        /// </summary>
        /// <param name="ramakeTime"></param>
        /// <param name="reMakePoint"></param>
        /// <returns></returns>
        public uint GetGradeStampImageId(uint ramakeTime, uint reMakePoint)
        {
            int index = GetGradePiontIndex(ramakeTime, reMakePoint);
            switch (index)
            {
                case -1:
                    return 2557;
                case 0:
                    return 2558;
                case 1:
                    return 2559;
                case 2:
                    return 2560;
            }
            return 2557;
        }

        /// <summary>
        /// 返回改造评级对应文本
        /// </summary>
        /// <param name="ramakeTime"></param>
        /// <param name="reMakePoint"></param>
        /// <returns></returns>
        public uint GetGradeStampLangId(uint ramakeTime, uint reMakePoint)
        {
            int index = GetGradePiontIndex(ramakeTime, reMakePoint);
            switch (index)
            {
                case -1:
                    return 12316;
                case 0:
                    return 12315;
                case 1:
                    return 12314;
                case 2:
                    return 12313;
            }
            return 12316;
        }

        /// <summary>
        /// 返回改造评级对应印章的星星
        /// </summary>
        /// <param name="ramakeTime"></param>
        /// <param name="reMakePoint"></param>
        /// <returns></returns>
        public uint GetGradeStampStarImageId(uint ramakeTime, uint reMakePoint)
        {
            int index = GetGradePiontIndex(ramakeTime, reMakePoint);
            switch (index)
            {
                case -1:
                    return 2552;
                case 0:
                    return 2553;
                case 1:
                    return 2554;
                case 2:
                    return 2555;
            }
            return 0;
        }

        /// <summary>
        /// 获取改造次数对应评级图章index
        /// </summary>
        /// <param name="ramakeTime"></param>
        /// <param name="reMakePoint"></param>
        /// <returns></returns>
        public int GetGradePiontIndex(uint ramakeTime, uint reMakePoint)
        {
            CSVPetNewReBuild.Data csvReBuild = CSVPetNewReBuild.Instance.GetConfData(ramakeTime);
            int index = -1;
            if (null != csvReBuild.grade_need)
            {
                for (int i = csvReBuild.grade_need.Count - 1; i >= 0; i--)
                {
                    if (reMakePoint >= csvReBuild.grade_need[i])
                    {
                        index = i;
                        break;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// 获取即将改造 可以用的改造书
        /// </summary>
        /// <param name="willBuildCount"></param>
        /// <returns></returns>
        public List<uint> GetCanUseLowsRemakeItems(int willBuildCount)
        {
            List<uint> configDatas = new List<uint>(5);

            var petRemakeDatas = CSVPetRemake.Instance.GetAll();
            for (int i = 0, len = petRemakeDatas.Count; i < len; i++)                
            {
                var data = petRemakeDatas[i];
                if (data.remake_num == willBuildCount)
                {
                    configDatas.Add(data.item_id);
                }
            }
            if (configDatas.Count > 1)
            {
                configDatas.Sort((a, b) =>
                {
                    return (int)a - (int)b;
                });
            }
            return configDatas;
        }

        /// <summary>
        /// 获取即将改造的 最低改造书
        /// </summary>
        /// <param name="pet"></param>
        /// <returns></returns>
        public uint GetCanUseLowsRemakeItemId(ClientPet pet)
        {
            int willBuildCount = pet.GetPeBuildCount() + 1;
            if(willBuildCount < pet.petData.max_remake_num)
            {
                List<uint> configDatas = GetCanUseLowsRemakeItems(willBuildCount);
                if (configDatas.Count > 0)
                {
                    return configDatas[0];
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 返回未保存改造评级是否 是完美的
        /// </summary>
        /// <param name="ramakeTime"></param>
        /// <param name="reMakePoint"></param>
        /// <returns></returns>
        public bool isPerfectRemakePoint(uint ramakeTime, uint reMakePoint)
        {
            return GetGradePiontIndex(ramakeTime, reMakePoint) >= 2;
        }

        public List<uint> GetRemakeSkillBySortType(EPetRemakeSkillState _sort_type)
        {
            uint sort_type = (uint)_sort_type;
            List<uint> vs = new List<uint>(128);
            var configDatas = CSVPetNewSkillSort.Instance.GetAll();

            for (int i = 0, len = configDatas.Count; i < len; i++)
            {
                var config = configDatas[i];
                if (config.is_show && (sort_type == 3 || sort_type == config.sort_type))
                {
                    vs.Add(config.id);
                }
            }
            if(vs.Count > 1)
            {
                vs.Sort(SortRemakeSKill);
            }
            return vs;
        }

        public int SortRemakeSKill(uint a, uint b)
        {
            CSVPetNewSkillSort.Data configA = CSVPetNewSkillSort.Instance.GetConfData(a);
            CSVPetNewSkillSort.Data configB = CSVPetNewSkillSort.Instance.GetConfData(b);
            return configA.sort.CompareTo(configB.sort);
        }

        private List<List<uint>> petEatFruitGrades = null;
        public List<List<uint>> PetEatFruitGrades
        {
            get
            {
                if(null == petEatFruitGrades)
                {
                    CSVPetNewParam.Data param = CSVPetNewParam.Instance.GetConfData(74);
                    if(null != param)
                    {
                        if(null != param.str_value)
                        {
                            petEatFruitGrades = ReadHelper.ReadArray2_ReadUInt(param.str_value, '|', '&');
                        }
                        else
                        {
                            petEatFruitGrades = new List<List<uint>>(5);
                        }
                    }
                    else
                    {
                        petEatFruitGrades = new List<List<uint>>(5);
                    }
                }
                return petEatFruitGrades;
            }
        }

        public EBaseAttr GetPetRemakeGradeTypeByItemId(uint itemId)
        {
            var fruitGrades = PetEatFruitGrades;
            uint otherType = 0;
            for (int i = 0; i < fruitGrades.Count; i++)
            {
                var _fruitGrade = fruitGrades[i];
                if (null != _fruitGrade && _fruitGrade.Count >= 2)
                {
                    if (itemId == _fruitGrade[0])
                    {
                        otherType = _fruitGrade[1];
                        break;
                    }
                }
            }

            switch(otherType)
            {
                case 0:
                    return EBaseAttr.Vit;
                case 1:
                    return EBaseAttr.Snh;
                case 2:
                    return EBaseAttr.Inten;
                case 3:
                    return EBaseAttr.Speed;
                case 4:
                    return EBaseAttr.Magic;
                default:
                    return EBaseAttr.Vit;
            }
        }
    }
}
