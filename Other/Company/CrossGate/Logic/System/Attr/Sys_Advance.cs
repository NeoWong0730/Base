using Lib.Core;
using Logic.Core;
using Packet;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public enum EPromoteCareerType
    {
        None = 0,
        Task = 1,
        Title = 2,
        SkillUpdata = 3,
        Level = 4,
        OpenServerTime = 5,
        MercheetLevel = 6,
        FullTeam = 7,
    }

    public class AdvanceTarget
    {
        public EPromoteCareerType type;
        public uint funId;
        public uint skillLimitLv;

        public AdvanceTarget(EPromoteCareerType _type, uint _funId,uint _skillLimitLv)
        {
            type = _type;
            funId = _funId;
            skillLimitLv = _skillLimitLv;
        }
    }

    public class Sys_Advance : SystemModuleBase<Sys_Advance>
    {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents
        {
            OnUpdateCareerUpInfoEvent, //更新当前进阶信息（评分 人数）
            OnTeamPromoteCareerAgreeOrCancel,  //组队进阶同意或拒绝
            OnTeamLeaderCheckPromoteCareer,   //组队检查队员进阶条件是否满足
        }
        public List<AdvanceTarget> targetlist = new List<AdvanceTarget>();
        public CmdRoleCareerUpInfoRes careerUpData;

        public void SetTargets()
        {
            targetlist.Clear();
            uint csvPromoteCareerNextId = SetAdvanceRank() + 1;
            CSVPromoteCareer.Data csvPromoteCareerNext;
            if (CSVPromoteCareer.Instance.ContainsKey(csvPromoteCareerNextId))
            {
                csvPromoteCareerNext = CSVPromoteCareer.Instance.GetConfData(csvPromoteCareerNextId);
            }
            else
            {
                return;
            }
            if (csvPromoteCareerNext.serverConditions != 0)
            {
                AdvanceTarget target = new AdvanceTarget(EPromoteCareerType.OpenServerTime, (uint)csvPromoteCareerNext.serverConditions, 0);
                targetlist.Add(target);
            }
            if (csvPromoteCareerNext.mercheetLevel != 0)
            {
                AdvanceTarget target = new AdvanceTarget(EPromoteCareerType.MercheetLevel, csvPromoteCareerNext.mercheetLevel , 0);
                targetlist.Add(target);
            }
            for (int i = 0; i < csvPromoteCareerNext.conditions.Count; i++)
            {
                AdvanceTarget target = new AdvanceTarget(EPromoteCareerType.Task, csvPromoteCareerNext.conditions[i], 0);
                targetlist.Add(target);
            }
            for (int i = 0; i < csvPromoteCareerNext.titleConditions.Count; i++)
            {
                AdvanceTarget target = new AdvanceTarget(EPromoteCareerType.Title, csvPromoteCareerNext.titleConditions[i], 0);
                targetlist.Add(target);
            }
            if (csvPromoteCareerNext.skillLimit != null)
            {
                AdvanceTarget target = new AdvanceTarget(EPromoteCareerType.SkillUpdata, csvPromoteCareerNext.skillLimit[0], csvPromoteCareerNext.skillLimit[1]);
                targetlist.Add(target);
            }
            if (csvPromoteCareerNext.isFull != 0)
            {
                AdvanceTarget target = new AdvanceTarget(EPromoteCareerType.Level, csvPromoteCareerNext.isFull, 0);
                targetlist.Add(target);
            }
            if (csvPromoteCareerNext.teamCondition != 0)
            {
                AdvanceTarget target = new AdvanceTarget(EPromoteCareerType.FullTeam, (uint)csvPromoteCareerNext.teamCondition, 0);
                targetlist.Add(target);
            }
        }

        public uint SetAdvanceRank()
        {
            uint curPromoteCareerId;
            ECareerType careerType = GameCenter.mainHero.careerComponent.CurCarrerType;
            if (careerType == ECareerType.None)
            {
                return 0;
            }
            uint currentlevel = Sys_Role.Instance.Role.CareerRank;
            curPromoteCareerId = (uint)careerType * 100 + currentlevel;
            return curPromoteCareerId;
        }

        public int NextAdvanceRank()
        {
            SetTargets();
            uint rank = SetAdvanceRank();
            if (rank == 0)
            {
                return -1;
            }
            else
            {
                if (CSVPromoteCareer.Instance.ContainsKey(rank + 1))
                {
                    for (int i = 0; i < targetlist.Count; ++i)
                    {
                         if (targetlist[i].type == EPromoteCareerType.OpenServerTime)
                        {
                            uint canAdvanceTime = Sys_Role.Instance.openServiceGameTime + targetlist[i].funId;
                            uint nowTime = Sys_Time.Instance.GetServerTime();
                            if(canAdvanceTime> nowTime)
                            {
                                return -1;
                            }
                        }
                        else if (targetlist[i].type == EPromoteCareerType.Task)
                        {
                            if (!TaskHelper.HasSubmited(targetlist[i].funId))
                            {
                                return -1;
                            }
                        }
                        else if (targetlist[i].type == EPromoteCareerType.Title)
                        {
                            if (!Sys_Title.Instance.TitleGet(targetlist[i].funId))
                            {
                                return -1;
                            }
                        }
                        else if (targetlist[i].type == EPromoteCareerType.SkillUpdata)
                        {
                            int count = 0;
                            foreach (var skills in Sys_Skill.Instance.bestSkillInfos)
                            {
                                if (skills.Value.Level >= targetlist[i].skillLimitLv)
                                    count++;
                            }
                            if (count < targetlist[i].funId)
                            {
                                return -1;
                            }
                        }
                        else if(targetlist[i].type == EPromoteCareerType.Level)
                        {
                            if (Sys_Role.Instance.Role.Exp + CSVCharacterAttribute.Instance.GetConfData(Sys_Role.Instance.Role.Level - 1).totol_exp < CSVCharacterAttribute.Instance.GetConfData(targetlist[i].funId).totol_exp)
                            {
                                return -1;
                            }
                        }
                    }
                    return (int)rank + 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        public uint GetPromoteIdByRank(uint occupationId,uint rank)
        {
            CSVCareer.Data data = CSVCareer.Instance.GetConfData(occupationId);
            if (data == null)
            {
                return 0;
            }
            uint promoteId =  occupationId * 100 + rank;
            return promoteId;
        }

        /// <summary>
        /// 获取当前限制等级
        /// </summary>
        /// <returns></returns>
        public uint GetCurLimiteLevel()
        {
            uint curPromoteCareerId = SetAdvanceRank();
            if (curPromoteCareerId != 0)
            {
                CSVPromoteCareer.Instance.TryGetValue(curPromoteCareerId, out CSVPromoteCareer.Data csvPromoteCareerNext);
                if (csvPromoteCareerNext != null)
                {
                    return csvPromoteCareerNext.levelLimit;
                }
            }
            return 0;
        }

        public uint IsCanIntoAdvanceFight(out uint level)
        {
            level = 0;
            uint curPromoteCareerId = SetAdvanceRank();
            CSVPromoteCareer.Instance.TryGetValue(curPromoteCareerId + 1, out CSVPromoteCareer.Data csvPromoteCareerNext);
            if (csvPromoteCareerNext == null)
                return 0;
            if (csvPromoteCareerNext.battleLevel == null)
                return 0;
            //Sys_Team.Instance.GetTeamMemMinLv();
            if (!Sys_Team.Instance.HaveTeam)
            {
                if (Sys_Role.Instance.Role.Level < csvPromoteCareerNext.battleLevel[0] || Sys_Role.Instance.Role.Level > csvPromoteCareerNext.battleLevel[1])
                {
                    level = csvPromoteCareerNext.battleLevel[0];
                    return 2005067;
                }
            }
            else
            {
                for (int i = 0; i < Sys_Team.Instance.TeamMemsCount; i++)
                {
                    var teamMem = Sys_Team.Instance.getTeamMem(i);
                    if (teamMem.IsLeave() || teamMem.IsOffLine())
                        continue;
                    if (teamMem.Level < csvPromoteCareerNext.battleLevel[0] || teamMem.Level > csvPromoteCareerNext.battleLevel[1])
                    {
                        level = csvPromoteCareerNext.battleLevel[0];
                        return 2005068;
                    }
                }
            }   

            return 0;
        }

        public bool CanAdvanceInTeam(int count)
        {
            if (Sys_Team.Instance.TeamMemsCount< count)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2005031));
                return false;
            }
            for (int i = 0; i < Sys_Team.Instance.TeamMemsCount; i++)
            {
                TeamMem teamMem = Sys_Team.Instance.getTeamMem(i);
                if ( teamMem.IsOffLine() || teamMem.IsLeave())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000003));
                    return false;
                }
                else if (teamMem.IsRob())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2005081));
                    return false;
                }
            }
            return true;
        }
    }
}
