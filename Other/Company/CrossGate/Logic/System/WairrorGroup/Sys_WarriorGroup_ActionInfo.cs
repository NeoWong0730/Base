using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using Net;
using Packet;

namespace Logic
{
    /// <summary>
    /// 勇者团系统_动态///
    /// </summary>
    public partial class Sys_WarriorGroup : SystemModuleBase<Sys_WarriorGroup>
    {
        public enum EActionType
        {
            None,
            GotPet = 1, //获得档位5档以下的极品宠物
            CarrerRankUp = 2,   //职业进阶值>=1
            GotEquip = 3,   //获得橙色品质5级以上装备
            GotSubEquip = 4,    //获得橙色品质5级以上饰品
            LifeSkillup = 5,    //生活技能升级到4级以上
            WakeUpRankUp = 6,   //进阶到觉醒等级大于等于7
            WakeUpNodeCompleted = 7,    //完成编号为999的印记节点升级
            MemberJoined = 8,   //成员进入勇者团
            MemberLeft = 9, //成员离开勇者团
            NewLeader = 10, //新团长上任
            RecruitMeetingStarted = 11, //招募会议开始
            FireMeetingStarted = 12,    //请离会议开始
            SuggestSelfMeetingStarted = 13, //自荐会议开始
            ChangeNameMeetingStarted = 14,  //改名会议开始
            ChangeDeclarationMeetingStarted = 15,   //宣言会议开始
            RecruitMeetingEnd_Passed_Successed = 16, //招募会议结束
            RecruitMeetingEnd_Passed_Failed = 17, //招募会议结束
            RecruitMeetingEnd_UnPassed = 18, //招募会议结束
            RecruitMeetingEnd_OutOfTime = 19, //招募会议结束
            FireMeetingEnd_Passed_Successed = 20,    //请离会议结束
            FireMeetingEnd_Passed_Failed = 21,    //请离会议结束
            FireMeetingEnd_UnPassed = 22,    //请离会议结束
            FireMeetingEnd_OutOfTime = 23,    //请离会议结束
            SuggestSelfMeetingEnd_Passed_Successed = 24, //自荐会议结束
            SuggestSelfMeetingEnd_Passed_Failed = 25, //自荐会议结束
            SuggestSelfMeetingEnd_UnPassed = 26, //自荐会议结束
            SuggestSelfMeetingEnd_OutOfTime = 27, //自荐会议结束
            ChangeNameMeetingEnd_Passed_Successed = 28,  //改名会议结束
            ChangeNameMeetingEnd_Passed_Failed = 29,  //改名会议结束
            ChangeNameMeetingEnd_UnPassed = 30,  //改名会议结束
            ChangeNameMeetingEnd_OutOfTime = 31,  //改名会议结束
            ChangeDeclarationMeetingEnd_Passed_Successed = 32,   //宣言会议结束
            ChangeDeclarationMeetingEnd_Passed_Failed = 33,   //宣言会议结束
            ChangeDeclarationMeetingEnd_UnPassed = 34,   //宣言会议结束
            ChangeDeclarationMeetingEnd_OutOfTime = 35,   //宣言会议结束
        }

        /// <summary>
        /// 动态信息///
        /// </summary>
        public class ActionInfo
        {
            public ActionInfo(uint infoID, uint createTime)
            {
                InfoID = infoID;
                CreateTime = createTime;
            }

            /// <summary>
            /// 动态创建时间///
            /// </summary>
            public uint CreateTime
            {
                get;
                private set;
            }

            /// <summary>
            /// 动态表ID///
            /// </summary>
            public uint InfoID
            {
                get;
                private set;
            }

            public List<string> paramList = new List<string>();

            public void FillParamList(BraveGroupDynamicParamList dynamicParamList)
            {
                if (InfoID == (uint)EActionType.GotPet)
                {
                    if (dynamicParamList.Params != null && dynamicParamList.Params.Count > 0)
                    {
                        if (dynamicParamList.Params[0].IntParam != 0)
                            paramList.Add(Instance.MyWarriorGroup.warriorInfos[dynamicParamList.Params[0].IntParam].RoleName);
                        else
                            paramList.Add(dynamicParamList.Params[0].StrParam.ToStringUtf8());

                        if (dynamicParamList.Params.Count > 1)
                        {
                            if (dynamicParamList.Params[1].IntParam != 0)
                            {
                                ulong rank = dynamicParamList.Params[1].IntParam - 1;
                                if (rank == 0)
                                    paramList.Add(LanguageHelper.GetTextContent(13560));
                                else
                                    paramList.Add(LanguageHelper.GetTextContent(13499) + rank.ToString());
                            }
                            else
                                paramList.Add(dynamicParamList.Params[1].StrParam.ToStringUtf8());
                        }

                        if (dynamicParamList.Params.Count > 2)
                        {
                            if (dynamicParamList.Params[2].IntParam != 0)
                                paramList.Add(LanguageHelper.GetTextContent(CSVPetNew.Instance.GetConfData((uint)dynamicParamList.Params[2].IntParam).name));
                            else
                                paramList.Add(dynamicParamList.Params[2].StrParam.ToStringUtf8());
                        }
                    }
                }
                else if (InfoID == (uint)EActionType.CarrerRankUp)
                {
                    if (dynamicParamList.Params != null && dynamicParamList.Params.Count > 0)
                    {
                        if (dynamicParamList.Params[0].IntParam != 0)
                            paramList.Add(Instance.MyWarriorGroup.warriorInfos[dynamicParamList.Params[0].IntParam].RoleName);
                        else
                            paramList.Add(dynamicParamList.Params[0].StrParam.ToStringUtf8());

                        if (dynamicParamList.Params.Count > 1)
                        {
                            if (dynamicParamList.Params[1].IntParam != 0)
                                paramList.Add(LanguageHelper.GetTextContent(CSVPromoteCareer.Instance.GetConfData((uint)dynamicParamList.Params[1].IntParam).professionLan));
                            else
                                paramList.Add(dynamicParamList.Params[1].StrParam.ToStringUtf8());
                        }
                    }
                }
                else if (InfoID == (uint)EActionType.GotEquip)
                {
                    if (dynamicParamList.Params != null && dynamicParamList.Params.Count > 0)
                    {
                        if (dynamicParamList.Params[0].IntParam != 0)
                            paramList.Add(Instance.MyWarriorGroup.warriorInfos[dynamicParamList.Params[0].IntParam].RoleName);
                        else
                            paramList.Add(dynamicParamList.Params[0].StrParam.ToStringUtf8());

                        if (dynamicParamList.Params.Count > 1)
                        {
                            if (dynamicParamList.Params[1].IntParam != 0)
                                paramList.Add(LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData((uint)dynamicParamList.Params[1].IntParam).name_id));
                            else
                                paramList.Add(dynamicParamList.Params[1].StrParam.ToStringUtf8());
                        }
                    }
                }
                else if (InfoID == (uint)EActionType.GotSubEquip)
                {
                    if (dynamicParamList.Params != null && dynamicParamList.Params.Count > 0)
                    {
                        if (dynamicParamList.Params[0].IntParam != 0)
                            paramList.Add(Instance.MyWarriorGroup.warriorInfos[dynamicParamList.Params[0].IntParam].RoleName);
                        else
                            paramList.Add(dynamicParamList.Params[0].StrParam.ToStringUtf8());

                        if (dynamicParamList.Params.Count > 1)
                        {
                            if (dynamicParamList.Params[1].IntParam != 0)
                                paramList.Add(LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData((uint)dynamicParamList.Params[1].IntParam).name_id));
                            else
                                paramList.Add(dynamicParamList.Params[1].StrParam.ToStringUtf8());
                        }
                    }
                }
                else if (InfoID == (uint)EActionType.LifeSkillup)
                {
                    if (dynamicParamList.Params != null && dynamicParamList.Params.Count > 0)
                    {
                        if (dynamicParamList.Params[0].IntParam != 0)
                            paramList.Add(Instance.MyWarriorGroup.warriorInfos[dynamicParamList.Params[0].IntParam].RoleName);
                        else
                            paramList.Add(dynamicParamList.Params[0].StrParam.ToStringUtf8());

                        if (dynamicParamList.Params.Count > 1)
                        {
                            if (dynamicParamList.Params[1].IntParam != 0)
                                paramList.Add(LanguageHelper.GetTextContent(CSVLifeSkill.Instance.GetConfData((uint)dynamicParamList.Params[1].IntParam).name_id));
                            else
                                paramList.Add(dynamicParamList.Params[1].StrParam.ToStringUtf8());
                        }

                        if (dynamicParamList.Params.Count > 2)
                        {
                            if (dynamicParamList.Params[2].IntParam != 0)
                                paramList.Add(dynamicParamList.Params[2].IntParam.ToString());
                            else
                                paramList.Add(dynamicParamList.Params[2].StrParam.ToStringUtf8());
                        }
                    }
                }
                else if (InfoID == (uint)EActionType.WakeUpRankUp)
                {
                    if (dynamicParamList.Params != null && dynamicParamList.Params.Count > 0)
                    {
                        if (dynamicParamList.Params[0].IntParam != 0)
                            paramList.Add(Instance.MyWarriorGroup.warriorInfos[dynamicParamList.Params[0].IntParam].RoleName);
                        else
                            paramList.Add(dynamicParamList.Params[0].StrParam.ToStringUtf8());

                        if (dynamicParamList.Params.Count > 1)
                        {
                            if (dynamicParamList.Params[1].IntParam != 0)
                                paramList.Add(LanguageHelper.GetTextContent(CSVTravellerAwakening.Instance.GetConfData((uint)dynamicParamList.Params[1].IntParam).NameId));
                            else
                                paramList.Add(dynamicParamList.Params[1].StrParam.ToStringUtf8());
                        }
                    }
                }
                else if (InfoID == (uint)EActionType.WakeUpNodeCompleted)
                {
                    if (dynamicParamList.Params != null && dynamicParamList.Params.Count > 0)
                    {
                        if (dynamicParamList.Params[0].IntParam != 0)
                            paramList.Add(Instance.MyWarriorGroup.warriorInfos[dynamicParamList.Params[0].IntParam].RoleName);
                        else
                            paramList.Add(dynamicParamList.Params[0].StrParam.ToStringUtf8());

                        if (dynamicParamList.Params.Count > 1)
                        {
                            if (dynamicParamList.Params[1].IntParam != 0)
                                paramList.Add(LanguageHelper.GetTextContent(CSVImprintNode.Instance.GetConfData((uint)dynamicParamList.Params[1].IntParam).Imprint_Name));
                            else
                                paramList.Add(dynamicParamList.Params[1].StrParam.ToStringUtf8());
                        }
                    }
                }
                else if (InfoID == (uint)EActionType.MemberJoined || InfoID == (uint)EActionType.MemberLeft ||
                    InfoID == (uint)EActionType.RecruitMeetingStarted || InfoID == (uint)EActionType.RecruitMeetingEnd_Passed_Successed || InfoID == (uint)EActionType.RecruitMeetingEnd_Passed_Failed || InfoID == (uint)EActionType.RecruitMeetingEnd_UnPassed || InfoID == (uint)EActionType.RecruitMeetingEnd_OutOfTime ||
                    InfoID == (uint)EActionType.FireMeetingStarted || InfoID == (uint)EActionType.FireMeetingEnd_Passed_Successed || InfoID == (uint)EActionType.FireMeetingEnd_Passed_Failed || InfoID == (uint)EActionType.FireMeetingEnd_UnPassed || InfoID == (uint)EActionType.FireMeetingEnd_OutOfTime ||
                    InfoID == (uint)EActionType.SuggestSelfMeetingStarted || InfoID == (uint)EActionType.SuggestSelfMeetingEnd_Passed_Successed || InfoID == (uint)EActionType.SuggestSelfMeetingEnd_Passed_Failed || InfoID == (uint)EActionType.SuggestSelfMeetingEnd_UnPassed || InfoID == (uint)EActionType.SuggestSelfMeetingEnd_OutOfTime ||
                    InfoID == (uint)EActionType.ChangeNameMeetingStarted || InfoID == (uint)EActionType.ChangeNameMeetingEnd_Passed_Successed || InfoID == (uint)EActionType.ChangeNameMeetingEnd_Passed_Failed || InfoID == (uint)EActionType.ChangeNameMeetingEnd_UnPassed || InfoID == (uint)EActionType.ChangeNameMeetingEnd_OutOfTime ||
                    InfoID == (uint)EActionType.ChangeDeclarationMeetingStarted || InfoID == (uint)EActionType.ChangeDeclarationMeetingEnd_Passed_Successed || InfoID == (uint)EActionType.ChangeDeclarationMeetingEnd_Passed_Failed || InfoID == (uint)EActionType.ChangeDeclarationMeetingEnd_UnPassed || InfoID == (uint)EActionType.ChangeDeclarationMeetingEnd_OutOfTime)
                {
                    if (dynamicParamList.Params != null && dynamicParamList.Params.Count > 0)
                    {
                        if (dynamicParamList.Params[0].IntParam != 0)
                            paramList.Add(Instance.MyWarriorGroup.warriorInfos[dynamicParamList.Params[0].IntParam].RoleName);
                        else
                            paramList.Add(dynamicParamList.Params[0].StrParam.ToStringUtf8());
                    }
                }
                else if (InfoID == (uint)EActionType.NewLeader)
                {
                    if (dynamicParamList.Params != null && dynamicParamList.Params.Count > 0)
                    {
                        if (dynamicParamList.Params[0].IntParam != 0)
                            paramList.Add(Instance.MyWarriorGroup.warriorInfos[dynamicParamList.Params[0].IntParam].RoleName);
                        else
                            paramList.Add(dynamicParamList.Params[0].StrParam.ToStringUtf8());

                        if (dynamicParamList.Params.Count > 1)
                        {
                            if (dynamicParamList.Params[1].IntParam != 0)
                                paramList.Add(Instance.MyWarriorGroup.warriorInfos[dynamicParamList.Params[1].IntParam].RoleName);
                            else
                                paramList.Add(dynamicParamList.Params[1].StrParam.ToStringUtf8());
                        }
                    }
                }
            }

            public string ToDesc()
            {
                string str = string.Empty;

                var data = CSVBraveTeamNews.Instance.GetConfData(InfoID);
                if (data != null)
                {
                    str = LanguageHelper.GetTextContent(data.Language, paramList.ToArray());
                }

                return str;
            }

            public string ToTime()
            {
                return LanguageHelper.TimeToString(CreateTime, LanguageHelper.TimeFormat.Type_13);         
            }
        }
    }
}
