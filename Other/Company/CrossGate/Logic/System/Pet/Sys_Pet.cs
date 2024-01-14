using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using static Packet.PetPkAttr.Types;

namespace Logic
{
    public enum EPetUiType
    {
        UI_Message = 1,
        UI_Practice = 2,
        UI_Remake = 3,
        UI_Pet_Tips = 4,
        UI_None = 999,
    }

    public class ClientPet
    {
        public PetUnit petUnit = new PetUnit();
        public CSVPetNew.Data petData;
        public CSVPetMount.Data mountData;
        //放生银币
        public uint abandonCoin
        {
            get
            {
                if (petUnit.SimpleInfo.Bind)
                {
                    return Sys_Pet.Instance.BindPetAbandonCoin;
                }
                else
                {
                    var scoreList = CSVPetRelease.Instance.GetAll();
                    for (int i = scoreList.Count - 1; i >= 0; i--)
                    {
                        if (petUnit.SimpleInfo.Score >= scoreList[i].id)
                        {
                            return scoreList[i].coin;
                        }
                    }
                }
                return 0;
            }
        }

        //契约主宠物
        public uint PartnerUid
        {
            get
            {
                if (null == petUnit || null == petUnit.SimpleInfo)
                    return 0;
                return petUnit.SimpleInfo.ContractPetUid;
            }
            set
            {
                if (null == petUnit || null == petUnit.SimpleInfo)
                    return;
                petUnit.SimpleInfo.ContractPetUid = value;
            }
        }
        public uint ContractLevel
        {
            get
            {
                if (null == petUnit || null == petUnit.SimpleInfo)
                    return 0;
                return petUnit.SimpleInfo.ContractLevel;
            }
            set
            {
                if (null == petUnit || null == petUnit.SimpleInfo)
                    return;
                petUnit.SimpleInfo.ContractLevel = value;
            }
        }

        public Dictionary<EBaseAttr, long> baseAttrs = new Dictionary<EBaseAttr, long>();
        public Dictionary<EBaseAttr, ulong> baseAttrs2AttrAssigh = new Dictionary<EBaseAttr, ulong>();

        public Dictionary<int, long> pkAttrs = new Dictionary<int, long>();
        public Dictionary<uint, uint> addAttrsPreview = new Dictionary<uint, uint>();
        public Dictionary<int, long> baseAttrsafter = new Dictionary<int, long>();
        public Dictionary<uint, uint> grades = new Dictionary<uint, uint>(5);
        public Dictionary<uint, uint> buildGrades = new Dictionary<uint, uint>(5);
        public Dictionary<uint, uint> buildTempGrades = new Dictionary<uint, uint>(5);
        public Dictionary<uint, uint> rebuildTempGrades = new Dictionary<uint, uint>(5);//重置的属性
        public List<PetPointPlansData> addPointPlanList = new List<PetPointPlansData>();
        public List<EnhancePlanInfo> enhancePlanList = new List<EnhancePlanInfo>();
        public Dictionary<uint, PetPkAttr> pointPlanAttrs = new Dictionary<uint, PetPkAttr>();

        public bool isShowLock = true;

        public ClientPet(PetUnit _petUnit, bool showLock = true)
        {
            petUnit.Uid = _petUnit.Uid;
            ResetPetModel(_petUnit);
            this.isShowLock = showLock;
        }

        /// <summary>
        /// 重新设置PetUnit 数据
        /// </summary>
        /// <param name="_petUnit"></param>
        public void ResetPetModel(PetUnit _petUnit)
        {
            if (null == _petUnit)
                return;
            
            petUnit.Islocked = _petUnit.Islocked;
            
            if (null != _petUnit.SimpleInfo)
            {
                petUnit.SimpleInfo = _petUnit.SimpleInfo;
                petData = CSVPetNew.Instance.GetConfData(petUnit.SimpleInfo.PetId);
                mountData = CSVPetMount.Instance.GetConfData(petUnit.SimpleInfo.PetId);
            }

            if (null != _petUnit.GradeInfo)
            {
                ResetPetGradeInfoModel(_petUnit.GradeInfo);
            }

            if (null != _petUnit.PetPointPlanData)
            {
                petUnit.PetPointPlanData = _petUnit.PetPointPlanData;
                ResetPetPointPlanDataModel(_petUnit, (int)_petUnit.PetPointPlanData.CurrentPlanIndex);
            }

            if (null != _petUnit.EnhancePlansData)
            {
                petUnit.EnhancePlansData = _petUnit.EnhancePlansData;
            }

            if (null != _petUnit.PkAttr)
            {
                ResetPetPetPkAttrModel(_petUnit.PkAttr);
            }

            if (null != _petUnit.BaseSkillInfo)
            {
                petUnit.BaseSkillInfo = _petUnit.BaseSkillInfo;
            }

            if (null != _petUnit.BuildInfo)
            {
                ResetPetBuildGradeInfoModel(_petUnit.BuildInfo);
            }
            if (null != _petUnit.EnhancePlansData)
            {
                petUnit.EnhancePlansData = _petUnit.EnhancePlansData;
            }

            if (null != _petUnit.SkillCostInfo)
            {
                ResetRidingSkillCost(_petUnit.SkillCostInfo);
            }

            if (null != _petUnit.PetEquipInfo)
            {
                ResetPetEquip(_petUnit.PetEquipInfo);
            }

            if (null != _petUnit.PetSoulUnit)
            {
                ResetPetSoulUnit(_petUnit.PetSoulUnit);
            }
        }

        /// <summary>
        /// 重新设置档位数据
        /// </summary>
        /// <param name="baseGrade"></param>
        private void ResetPetGradeInfoModel(BaseGrade baseGrade)
        {
            petUnit.GradeInfo = baseGrade;
            grades.Clear();
            grades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Vit], petUnit.GradeInfo.VitGrade);
            grades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Snh], petUnit.GradeInfo.SnhGrade);
            grades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Inten], petUnit.GradeInfo.IntenGrade);
            grades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Speed], petUnit.GradeInfo.SpeedGrade);
            grades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Magic], petUnit.GradeInfo.MagicGrade);
        }

        private void ResetPetBuildGradeInfoModel(BuildUnit buildUnit)
        {
            petUnit.BuildInfo = buildUnit;

            buildGrades.Clear();
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Vit], petUnit.BuildInfo.VitGrade);
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Snh], petUnit.BuildInfo.SnhGrade);
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Inten], petUnit.BuildInfo.IntenGrade);
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Speed], petUnit.BuildInfo.SpeedGrade);
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Magic], petUnit.BuildInfo.MagicGrade);

            buildTempGrades.Clear();
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Vit], petUnit.BuildInfo.VitGradeTemp);
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Snh], petUnit.BuildInfo.SnhGradeTemp);
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Inten], petUnit.BuildInfo.IntenGradeTemp);
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Speed], petUnit.BuildInfo.SpeedGradeTemp);
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Magic], petUnit.BuildInfo.MagicGradeTemp);

            rebuildTempGrades.Clear();

            rebuildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Vit], petUnit.BuildInfo.VitGradeTempTotal);
            rebuildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Snh], petUnit.BuildInfo.SnhGradeTempTotal);
            rebuildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Inten], petUnit.BuildInfo.IntenGradeTempTotal);
            rebuildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Speed], petUnit.BuildInfo.SpeedGradeTempTotal);
            rebuildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Magic], petUnit.BuildInfo.MagicGradeTempTotal);
        }

        /// <summary>
        /// 重置未保存改造档位
        /// </summary>
        /// <param name="res"></param>
        public void ResetBuildGradeTemp(CmdPetRemakeReGradeRes res)
        {
            petUnit.BuildInfo.VitGradeTemp = res.VitGrade;
            petUnit.BuildInfo.SnhGradeTemp = res.SnhGrade;
            petUnit.BuildInfo.IntenGradeTemp = res.IntenGrade;
            petUnit.BuildInfo.SpeedGradeTemp = res.SpeedGrade;
            petUnit.BuildInfo.MagicGradeTemp = res.MagicGrade;
            buildTempGrades.Clear();
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Vit], petUnit.BuildInfo.VitGradeTemp);
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Snh], petUnit.BuildInfo.SnhGradeTemp);
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Inten], petUnit.BuildInfo.IntenGradeTemp);
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Speed], petUnit.BuildInfo.SpeedGradeTemp);
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Magic], petUnit.BuildInfo.MagicGradeTemp);
        }

        /// <summary>
        /// 重置改造档位
        /// </summary>
        /// <param name="res"></param>
        public void ResetBuildGrade(CmdPetRemakeSelectGradeRes res)
        {
            petUnit.BuildInfo.VitGrade = res.VitGrade;
            petUnit.BuildInfo.SnhGrade = res.SnhGrade;
            petUnit.BuildInfo.IntenGrade = res.IntenGrade;
            petUnit.BuildInfo.SpeedGrade = res.SpeedGrade;
            petUnit.BuildInfo.MagicGrade = res.MagicGrade;
            buildGrades.Clear();
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Vit], petUnit.BuildInfo.VitGrade);
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Snh], petUnit.BuildInfo.SnhGrade);
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Inten], petUnit.BuildInfo.IntenGrade);
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Speed], petUnit.BuildInfo.SpeedGrade);
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Magic], petUnit.BuildInfo.MagicGrade);
        }

        /// <summary>
        /// 重置改造档位
        /// </summary>
        /// <param name="res"></param>
        public void ResetBuildGrade(CmdPetEatFruitRes res)
        {
            petUnit.BuildInfo.VitGrade = res.VitGrade;
            petUnit.BuildInfo.SnhGrade = res.SnhGrade;
            petUnit.BuildInfo.IntenGrade = res.IntenGrade;
            petUnit.BuildInfo.SpeedGrade = res.SpeedGrade;
            petUnit.BuildInfo.MagicGrade = res.MagicGrade;
            buildGrades.Clear();
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Vit], petUnit.BuildInfo.VitGrade);
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Snh], petUnit.BuildInfo.SnhGrade);
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Inten], petUnit.BuildInfo.IntenGrade);
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Speed], petUnit.BuildInfo.SpeedGrade);
            buildGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Magic], petUnit.BuildInfo.MagicGrade);
        }

        /// <summary>
        /// 重置骑术技能消耗
        /// </summary>
        /// <param name="res"></param>
        public void ResetRidingSkillCost(RidingSkillCost res)
        {
            petUnit.SkillCostInfo = res;
        }

        /// <summary>
        /// 重置骑术技能消耗
        /// </summary>
        /// <param name="res"></param>
        public void ResetPetEquip(PetEquipUnit petequipInfo)
        {
            petUnit.PetEquipInfo = petequipInfo;
        }

        public void ResetPetSoulUnit(PetSoulUnit petSoulUnit)
        {
            petUnit.PetSoulUnit = petSoulUnit;
        }

        /// <summary>
        /// 改造信息返回
        /// </summary>
        /// <param name="res"></param>
        public void SetBuildEndData(CmdPetRemakeRes res)
        {
            if (null == petUnit.BuildInfo)
            {
                petUnit.BuildInfo = new BuildUnit();
            }
            petUnit.BuildInfo.VitGradeTemp = res.VitGrade;
            petUnit.BuildInfo.SnhGradeTemp = res.SnhGrade;
            petUnit.BuildInfo.IntenGradeTemp = res.IntenGrade;
            petUnit.BuildInfo.SpeedGradeTemp = res.SpeedGrade;
            petUnit.BuildInfo.MagicGradeTemp = res.MagicGrade;
            buildTempGrades.Clear();
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Vit], petUnit.BuildInfo.VitGradeTemp);
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Snh], petUnit.BuildInfo.SnhGradeTemp);
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Inten], petUnit.BuildInfo.IntenGradeTemp);
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Speed], petUnit.BuildInfo.SpeedGradeTemp);
            buildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Magic], petUnit.BuildInfo.MagicGradeTemp);

            petUnit.BuildInfo.VitGradeTempTotal = res.VitGradeTempTotal;
            petUnit.BuildInfo.SnhGradeTempTotal = res.SnhGradeTempTotal;
            petUnit.BuildInfo.IntenGradeTempTotal = res.IntenGradeTempTotal;
            petUnit.BuildInfo.SpeedGradeTempTotal = res.SpeedGradeTempTotal;
            petUnit.BuildInfo.MagicGradeTempTotal = res.MagicGradeTempTotal;

            rebuildTempGrades.Clear();
            rebuildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Vit], petUnit.BuildInfo.VitGradeTempTotal);
            rebuildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Snh], petUnit.BuildInfo.SnhGradeTempTotal);
            rebuildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Inten], petUnit.BuildInfo.IntenGradeTempTotal);
            rebuildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Speed], petUnit.BuildInfo.SpeedGradeTempTotal);
            rebuildTempGrades.Add(Sys_Pet.Instance.baseAttrs2Id[EBaseAttr.Magic], petUnit.BuildInfo.MagicGradeTempTotal);
            petUnit.BuildInfo.SkillTemp.Clear();
            petUnit.BuildInfo.SkillTemp.AddRange(res.SkillList);
            petUnit.BuildInfo.GradeScoreTemp = res.GradeScore;
            petUnit.BuildInfo.Index = res.Index;
            petUnit.BuildInfo.BuildState = res.Index > 0 ? (uint)EnumPetBuildState.ReBuilding :(uint)EnumPetBuildState.Building;
        }

        /// <summary>
        /// 重新设置一级属性数据
        /// </summary>
        /// <param name="baseGrade"></param>
        private void ResetPetPointPlanDataModel(PetUnit petUnit, int index)
        {
            baseAttrs2AttrAssigh.Clear();
            uint total = petUnit.PetPointPlanData.TotalPoint;
            uint use = petUnit.PetPointPlanData.Plans[index].UsePoint;
            PetPointPlanInfo info = petUnit.PetPointPlanData.Plans[index];
            baseAttrs[EBaseAttr.SurplusPoint] = total - use;
            baseAttrs[EBaseAttr.VitAssign] = info.VitAssign;
            baseAttrs2AttrAssigh.Add(EBaseAttr.Vit, info.VitAssign);
            baseAttrs[EBaseAttr.SnhAssign] = info.SnhAssign;
            baseAttrs2AttrAssigh.Add(EBaseAttr.Snh, info.SnhAssign);
            baseAttrs[EBaseAttr.IntenAssign] = info.IntenAssign;
            baseAttrs2AttrAssigh.Add(EBaseAttr.Inten, info.IntenAssign);
            baseAttrs[EBaseAttr.SpeedAssign] = info.SpeedAssign;
            baseAttrs2AttrAssigh.Add(EBaseAttr.Speed, info.SpeedAssign);
            baseAttrs[EBaseAttr.MagicAssign] = info.MagicAssign;
            baseAttrs2AttrAssigh.Add(EBaseAttr.Magic, info.MagicAssign);
        }

        /// <summary>
        /// 重新设置二级属性数据
        /// </summary>
        /// <param name="baseGrade"></param>
        private void ResetPetPetPkAttrModel(PetPkAttr petPkAttr)
        {
            petUnit.PkAttr = petPkAttr;
            pkAttrs[(int)EPkAttr.CurHp] = petPkAttr.CurHp;
            pkAttrs[(int)EPkAttr.CurMp] = petPkAttr.CurMp;
            for (int i = 0; i < petPkAttr.Attr.Count; i++)
            {
                uint attrId = petPkAttr.Attr[i].AttrId;
                long attrValue = petPkAttr.Attr[i].AttrValue;
                pkAttrs[(int)attrId] = attrValue;
                CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);
                if (attrData.isShow == 1 && attrData.attr_type == 4)
                {
                    baseAttrsafter[(int)attrId] = attrValue;
                }
            }
            
            if (petUnit.PetPointPlanData!=null && pointPlanAttrs.ContainsKey(petUnit.PetPointPlanData.CurrentPlanIndex))
            {
                pointPlanAttrs[petUnit.PetPointPlanData.CurrentPlanIndex] = petPkAttr;
            }
        }

        public long GetAttrValueByAttrId(int id)
        {
            if (null == pkAttrs)
                return 0;
            pkAttrs.TryGetValue(id, out long value);
            return value;
        }

        /// <summary>
        /// 重新设置改造方案基本属性
        /// </summary>
        public void ResetPetPointPlanAttrModel(CmdPetGetPointPlanAttrRes cmdPetGetPointPlanAttrRes)
        {
            if (petUnit.Uid == cmdPetGetPointPlanAttrRes.PetUid)
            {
                pointPlanAttrs.Clear();
                pointPlanAttrs.Add(petUnit.PetPointPlanData.CurrentPlanIndex, petUnit.PkAttr);
                for (int i=0; i< cmdPetGetPointPlanAttrRes.PlanAttrs.Count; ++i)
                {
                    pointPlanAttrs[cmdPetGetPointPlanAttrRes.PlanAttrs[i].PlanIndex] = cmdPetGetPointPlanAttrRes.PlanAttrs[i].Attr;
                }
            }
        }

        public long GetAttrValueByIndex(int id, int index, float gradePerent)
        {
            long num = GetAttrValueByAttrId(id);
            int curIndex = (int)petUnit.PetPointPlanData.CurrentPlanIndex;
            if(curIndex== index)
            {
                return num;
            }
            long value = 0;
            double addAss = 0;
            double addCurAss = 0;
            float gradeAdd = gradePerent / 100;
            if (id == (int)EBaseAttr.Vit)
            {
                if (gradeAdd != 0)
                {
                    addCurAss =Math.Round(petUnit.PetPointPlanData.Plans[curIndex].VitAssign * gradeAdd);
                    addAss = Math.Round(petUnit.PetPointPlanData.Plans[index].VitAssign * gradeAdd);
                }
                value = num - (petUnit.PetPointPlanData.Plans[curIndex].VitAssign+ (long)addCurAss) + petUnit.PetPointPlanData.Plans[index].VitAssign+ (long)addAss;
            }
            else if (id == (int)EBaseAttr.Inten)
            {
                if (gradeAdd != 0)
                {
                    addCurAss = Math.Round(petUnit.PetPointPlanData.Plans[curIndex].IntenAssign * gradeAdd);
                    addAss = Math.Round(petUnit.PetPointPlanData.Plans[index].IntenAssign * gradeAdd);
                }
                value = num - (petUnit.PetPointPlanData.Plans[curIndex].IntenAssign + (long)addCurAss)  + petUnit.PetPointPlanData.Plans[index].IntenAssign + (long)addAss;
            }
            else if (id == (int)EBaseAttr.Magic)
            {
                if (gradeAdd != 0)
                {
                    addCurAss = Math.Round(petUnit.PetPointPlanData.Plans[curIndex].MagicAssign * gradeAdd);
                    addAss = Math.Round(petUnit.PetPointPlanData.Plans[index].MagicAssign * gradeAdd);
                }
                value = num - (petUnit.PetPointPlanData.Plans[curIndex].MagicAssign + (long)addCurAss) + petUnit.PetPointPlanData.Plans[index].MagicAssign + (long)addAss;
            }
            else if (id == (int)EBaseAttr.Speed)
            {
                if (gradeAdd != 0)
                {
                    addCurAss = Math.Round(petUnit.PetPointPlanData.Plans[curIndex].SpeedAssign * gradeAdd);
                    addAss = Math.Round(petUnit.PetPointPlanData.Plans[index].SpeedAssign * gradeAdd);
                }
                value = num - (petUnit.PetPointPlanData.Plans[curIndex].SpeedAssign + (long)addCurAss) + petUnit.PetPointPlanData.Plans[index].SpeedAssign + (long)addAss;
            }
            else if (id == (int)EBaseAttr.Snh)
            {
                if (gradeAdd != 0)
                {
                    addCurAss = Math.Round(petUnit.PetPointPlanData.Plans[curIndex].SnhAssign * gradeAdd);
                    addAss = Math.Round(petUnit.PetPointPlanData.Plans[index].SnhAssign * gradeAdd);
                }
                value = num - (petUnit.PetPointPlanData.Plans[curIndex].SnhAssign + (long)addCurAss) + petUnit.PetPointPlanData.Plans[index].SnhAssign + (long)addAss;
            }
            return value;
        }

        public List<AttrPair> GetPetEleAttrList()
        {
            List<AttrPair> temp = new List<AttrPair>();
            if (null == petUnit || null != petUnit && null == petUnit.PkAttr)
            {
                return temp;
            }

            for (int i = 0; i < petUnit.PkAttr.Attr.Count; i++)
            {
                AttrPair tempAttrPair = petUnit.PkAttr.Attr[i];
                CSVAttr.Data cSVAttr = CSVAttr.Instance.GetConfData(tempAttrPair.AttrId);
                if (null != cSVAttr && cSVAttr.attr_type == 3 && tempAttrPair.AttrValue != 0)
                {
                    temp.Add(petUnit.PkAttr.Attr[i]);
                }
            }

            if(temp.Count > 1)
            {
                temp.Sort(AttrSort);
            }

            return temp;
        }

        int AttrSort(AttrPair a, AttrPair b)
        {
            return a.AttrId.CompareTo(b.AttrId);
        }

        /// <summary>
        /// 返回是否有详细信息 - 否则就是只有简要信息SimpleInfo
        /// </summary>
        /// <returns></returns>
        public bool HasMinutePetInfo()
        {
            if (null == petUnit)
                return false;
            return null != petUnit.PkAttr;
        }

        public uint GetPetUid()
        {
            if (null == petUnit)
                return 0;
            return petUnit.Uid;
        }

        /// <summary>
        /// 对应属性的总档位（初始+ 改造）
        /// </summary>
        /// <param name="eBaseAttr"></param>
        /// <returns></returns>
        public uint GetPetAllGradeAttr(EBaseAttr eBaseAttr)
        {
            return GetPetGradeAttr(eBaseAttr) + GetPetBuildGradeAttr(eBaseAttr);
        }

        public uint GetPetGradeAttr(EBaseAttr eBaseAttr)
        {
            if (null != petUnit.GradeInfo && grades.TryGetValue(Sys_Pet.Instance.baseAttrs2Id[eBaseAttr], out uint value1))
            {
                return value1;
            }
            return 0;
        }

        /// <summary>
        /// 获取对应改造的档数
        /// </summary>
        /// <param name="eBaseAttr"></param>
        /// <returns></returns>
        public uint GetPetBuildGradeAttr(EBaseAttr eBaseAttr)
        {
            if (null != petUnit.BuildInfo && buildGrades.TryGetValue(Sys_Pet.Instance.baseAttrs2Id[eBaseAttr], out uint value))
            {
                return value;
            }
            return 0;
        }

        /// <summary>
        /// 获取对应未保存档位的数量
        /// </summary>
        /// <param name="eBaseAttr"></param>
        /// <returns></returns>
        public uint GetPetBuildTempGradeAttr(EBaseAttr eBaseAttr)
        {
            if (null != petUnit.BuildInfo && buildTempGrades.TryGetValue(Sys_Pet.Instance.baseAttrs2Id[eBaseAttr], out uint value))
            {
                return value;
            }
            return 0;
        }

        /// <summary>
        /// 获取 重置未保存的档位总数
        /// </summary>
        /// <param name="eBaseAttr"></param>
        /// <returns></returns>
        public uint GetPetBuildTempGradeTotalAttr(EBaseAttr eBaseAttr)
        {
            if (null != petUnit.BuildInfo && rebuildTempGrades.TryGetValue(Sys_Pet.Instance.baseAttrs2Id[eBaseAttr], out uint value))
            {
                return value;
            }
            return 0;
        }


        /// <summary>
        /// 获取宠物的满档数
        /// </summary>
        /// <returns></returns>
        public uint GetPetMaxGradeCount()
        {
            if (null == petData)
                return 0;

            return (uint)(petData.endurance + petData.strength + petData.strong + petData.speed + petData.magic);
        }

        /// <summary>
        /// 获取宠物当前总档数 改造+自带
        /// </summary>
        /// <returns></returns>
        public uint GetPetCurrentGradeCount()
        {
            return GetPetGradeCount() + GetPeBuildGradeCount();
        }

        /// <summary>
        /// 获取宠物改造后的满档数 改造+自带
        /// </summary>
        /// <returns></returns>
        public uint GetPetBuildMaxGradeCount()
        {
            return GetPetMaxGradeCount() + GetPeBuildGradeCount();
        }

        /// <summary>
        /// 宠物生成时的档位-固定不变-服务器生成
        /// </summary>
        /// <returns></returns>
        public uint GetPetGradeCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.GradeInfo))
                return 0;
            return petUnit.GradeInfo.VitGrade
                + petUnit.GradeInfo.SnhGrade
                + petUnit.GradeInfo.IntenGrade
                + petUnit.GradeInfo.SpeedGrade
                + petUnit.GradeInfo.MagicGrade;
        }

        /// <summary>
        /// 获取未保存的档位数
        /// </summary>
        /// <returns></returns>
        public uint GetPetTempGradeCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return 0;
            return petUnit.BuildInfo.VitGradeTemp
                + petUnit.BuildInfo.SnhGradeTemp
                + petUnit.BuildInfo.IntenGradeTemp
                + petUnit.BuildInfo.SpeedGradeTemp
                + petUnit.BuildInfo.MagicGradeTemp;
        }

        /// <summary>
        /// 获取技能格子数
        /// </summary>
        /// <returns></returns>
        public int GetPetSkillGridsCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo))
                return 0;

            return (int)petUnit.BaseSkillInfo.SkillGrids;
        }

        /// <summary>
        /// 获取技能数量
        /// </summary>
        /// <returns></returns>
        public int GetPetSkillCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || (null == petUnit.BaseSkillInfo.UniqueSkills || null == petUnit.BaseSkillInfo.Skills)))
                return 0;

            return petUnit.BaseSkillInfo.Skills.Count + petUnit.BaseSkillInfo.UniqueSkills.Count;
        }

        public List<uint> GetPetSkillList()
        {
            List<uint> skillList = new List<uint>();
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || (null == petUnit.BaseSkillInfo.UniqueSkills || null == petUnit.BaseSkillInfo.Skills)))
                return skillList;
            skillList.AddRange(GetUniqueSkills());
            skillList.AddRange(GetNorolSkills());
            return skillList;
        }

        /// <summary>
        /// 返回原始专属技能
        /// </summary>
        public List<uint> GetBaseUniqueSkills()
        {
            List<uint> skillList = new List<uint>();
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || null == petUnit.BaseSkillInfo.UniqueSkills))
                return skillList;
            skillList.AddRange(petUnit.BaseSkillInfo.UniqueSkills);
            return skillList;
        }

        public uint GetBaseUniqueSkillIdByupGradeSkillId(uint skillId)
        {
            var list = GetBaseUniqueSkills();
            for (int i = 0; i < list.Count; i++)
            {
                var id = skillId / 100;
                var tempId = list[i];
                if(id == tempId / 100)
                {
                    return tempId;
                }
            }
            return 0;
        }

        /// <summary>
        /// 返回现有的专属技能等级的id
        /// </summary>
        /// <returns></returns>
        public List<uint> GetUniqueSkills()
        {
            List<uint> skillList = new List<uint>();
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || null == petUnit.BaseSkillInfo.UniqueSkills))
                return skillList;
            bool hasSuitSkill = CheckHasSuitSkill(out uint skill);
            if (skill > 0)
            {
                CSVPetEquipSuitSkill.Data suitSkillData = CSVPetEquipSuitSkill.Instance.GetConfData(skill);
                if (petUnit.BaseSkillInfo.UniqueSkills.Contains(suitSkillData.base_skill))
                {
                    for (int i = 0; i < petUnit.BaseSkillInfo.UniqueSkills.Count; i++)
                    {
                        if (petUnit.BaseSkillInfo.UniqueSkills[i] == suitSkillData.base_skill)
                        {
                            skillList.Add(suitSkillData.upgrade_skill);
                        }
                        else
                        {
                            skillList.Add(petUnit.BaseSkillInfo.UniqueSkills[i]);
                        }
                    }
                    return skillList;
                }
                else
                {
                    return GetBaseUniqueSkills();
                }
                
            }
            else
            {
                return GetBaseUniqueSkills();
            }
            
        }

        public List<uint> GetNorolSkills()
        {
            List<uint> skillList = new List<uint>();
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || null == petUnit.BaseSkillInfo.Skills))
                return skillList;
            skillList.AddRange(petUnit.BaseSkillInfo.Skills);
            return skillList;
        }

        public List<uint> GetPetAllSkillList()
        {
            List<uint> skillList = new List<uint>();
            skillList.AddRange(GetPetSkillList());
            skillList.AddRange(GetPetBuildSkillList());
            return skillList;
        }

        /// <summary>
        /// 返回基础技能的经验
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public uint GetPetSkillExp(uint skillId)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || null == petUnit.BaseSkillInfo.Skills))
                return 0;
            for (int i = 0; i < petUnit.BaseSkillInfo.Skills.Count; i++)
            {
                uint _skill = (uint)Math.Ceiling(skillId / 1000.0f);
                uint skill_id = (uint)Math.Ceiling(petUnit.BaseSkillInfo.Skills[i] / 1000.0f);
                if (_skill == skill_id && (petUnit.BaseSkillInfo.SkillExp.Count > i))
                {
                    return petUnit.BaseSkillInfo.SkillExp[i];
                }
            }
            return 0;
        }

        /// <summary>
        /// 设置技能的经验
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="skillExp"></param>
        public void SetPetSkillExp(uint skillId, uint skillExp)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || null == petUnit.BaseSkillInfo.Skills))
                return;
            for (int i = 0; i < petUnit.BaseSkillInfo.Skills.Count; i++)
            {
                uint _skill = (uint)Math.Ceiling(skillId / 1000.0f);
                uint skill_id = (uint)Math.Ceiling(petUnit.BaseSkillInfo.Skills[i] / 1000.0f);
                if (_skill == skill_id && (petUnit.BaseSkillInfo.SkillExp.Count > i))
                {
                    petUnit.BaseSkillInfo.SkillExp[i] = skillExp;
                    break;
                }
            }
        }

        /// <summary>
        /// 技能升级替换
        /// </summary>
        /// <param name="skillIds"></param>
        /// <param name="skillExps"></param>
        /// <param name="type"></param>
        public void ReSetSkill(uint oldSkill, uint newSkill, uint skillExp)
        {
            for (int i = 0; i < petUnit.BaseSkillInfo.Skills.Count; i++)
            {
                if (oldSkill == petUnit.BaseSkillInfo.Skills[i])
                {
                    petUnit.BaseSkillInfo.Skills[i] = newSkill;
                    petUnit.BaseSkillInfo.SkillExp[i] = skillExp;
                }
            }
        }


        /// <summary>
        /// 1 专属技能 2 一般技能 3 改造技能
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="type"></param>
        public void RemoveSkill(RepeatedField<uint> skillIds, RepeatedField<uint> skillExps, uint type)
        {
            switch (type)
            {
                case 1:
                    RemoveUniqueSKill(skillIds);
                    break;
                case 2:
                    RemoveNoralSKill(skillIds, skillExps);
                    break;
                case 3:
                    RemoveBuildSKill(skillIds);
                    break;
            }
        }

        private void RemoveNoralSKill(RepeatedField<uint> skillIds, RepeatedField<uint> skillExps)
        {
            petUnit.BaseSkillInfo.Skills.Clear();
            petUnit.BaseSkillInfo.SkillExp.Clear();
            for (int i = 0; i < skillIds.Count; i++)
            {
                petUnit.BaseSkillInfo.Skills.Add(skillIds[i]);
                petUnit.BaseSkillInfo.SkillExp.Add(skillExps[i]);
            }
        }

        private void RemoveUniqueSKill(RepeatedField<uint> skillIds)
        {
            petUnit.BaseSkillInfo.UniqueSkills.Clear();
            for (int i = 0; i < skillIds.Count; i++)
            {
                petUnit.BaseSkillInfo.UniqueSkills.Add(skillIds[i]);
            }
        }

        public void RemoveBuildSKill(RepeatedField<uint> skillIds)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo || null == petUnit.BuildInfo.BuildSkills))
                return;
            petUnit.BuildInfo.BuildSkills.Clear();
            for (int i = 0; i < skillIds.Count; i++)
            {
                petUnit.BuildInfo.BuildSkills.Add(skillIds[i]);
            }
        }

        /// <summary>
        /// 通过旧id 获取可能升级后的技能id
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public uint GetPetSkill(uint skillId)
        {
            for (int i = 0; i < petUnit.BaseSkillInfo.Skills.Count; i++)
            {
                uint _skillId = (uint)Math.Ceiling(skillId / 1000.0f);
                uint skillConfigId = petUnit.BaseSkillInfo.Skills[i];
                uint skill_id = (uint)Math.Ceiling(skillConfigId / 1000.0f);
                if (_skillId == skill_id)
                {
                    return skillConfigId;
                }
            }
            return skillId;
        }

        public bool IsHaveSameSkill(uint skillId)
        {
            List<uint> skillList = new List<uint>();
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo))
                return false;
            for (int i = 0; i < petUnit.BaseSkillInfo.UniqueSkills.Count; i++)
            {
                uint skill_id = (uint)Math.Ceiling(petUnit.BaseSkillInfo.UniqueSkills[i] / 1000.0f);
                if (skillId == skill_id)
                {
                    return true;
                }
            }
            for (int i = 0; i < petUnit.BaseSkillInfo.Skills.Count; i++)
            {
                uint skill_id = (uint)Math.Ceiling(petUnit.BaseSkillInfo.Skills[i] / 1000.0f);
                if (skillId == skill_id)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsSameOrHighBuildSkill(uint skillId)
        {
            List<uint> skillList = new List<uint>();
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return false;
            uint skill_id1 = 0;
            uint skill_level1 = 0;
            uint skill_id2 = 0;
            uint skill_level2 = 0;
            if (!Sys_Skill.Instance.IsActiveSkill(skillId))
            {
                CSVPassiveSkillInfo.Data checkPetSkillData = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                skill_id1 = checkPetSkillData.skill_id;
                skill_level1 = checkPetSkillData.level;
            }
            else
            {
                CSVActiveSkillInfo.Data checkPetSkillData = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (null != checkPetSkillData)
                {
                    skill_id2 = checkPetSkillData.skill_id;
                    skill_level2 = checkPetSkillData.level;
                }
                else
                {
                    DebugUtil.LogError($"Not find id = {skillId} in CSVActiveSkillInfo and CSVPassiveSkillInfo");
                }
            }

            if (skill_id1 == 0)
                return false;
            for (int i = 0; i < petUnit.BuildInfo.BuildSkills.Count; i++)
            {
                uint petSkill = petUnit.BuildInfo.BuildSkills[i];
                if (petSkill != 0)
                {
                    if (!Sys_Skill.Instance.IsActiveSkill(petSkill))
                    {
                        CSVPassiveSkillInfo.Data checkPetSkillData = CSVPassiveSkillInfo.Instance.GetConfData(petSkill);
                        skill_id2 = checkPetSkillData.skill_id;
                        skill_level2 = checkPetSkillData.level;
                    }
                    else
                    {
                        CSVActiveSkillInfo.Data checkPetSkillData = CSVActiveSkillInfo.Instance.GetConfData(petSkill);
                        if (null != checkPetSkillData)
                        {
                            skill_id2 = checkPetSkillData.skill_id;
                            skill_level2 = checkPetSkillData.level;
                        }
                        else
                        {
                            DebugUtil.LogError($"Not find id = {skillId} in CSVActiveSkillInfo and CSVPassiveSkillInfo");
                        }
                    }
                    if (skill_id1 == skill_id2 && skill_level1 <= skill_level2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsHighBuildSkill(uint skillId, ref uint mId)
        {
            List<uint> skillList = new List<uint>();
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return false;
            uint skill_id1 = 0;
            uint skill_level1 = 0;
            uint skill_id2 = 0;
            uint skill_level2 = 0;
            if (!Sys_Skill.Instance.IsActiveSkill(skillId))
            {
                CSVPassiveSkillInfo.Data checkPetSkillData = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                skill_id1 = checkPetSkillData.skill_id;
                skill_level1 = checkPetSkillData.level;
            }
            else
            {
                CSVActiveSkillInfo.Data checkPetSkillData = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (null != checkPetSkillData)
                {
                    skill_id2 = checkPetSkillData.skill_id;
                    skill_level2 = checkPetSkillData.level;
                }
                else
                {
                    DebugUtil.LogError($"Not find id = {skillId} in CSVActiveSkillInfo and CSVPassiveSkillInfo");
                }
            }

            if (skill_id1 == 0)
                return false;
            for (int i = 0; i < petUnit.BuildInfo.BuildSkills.Count; i++)
            {
                uint petSkill = petUnit.BuildInfo.BuildSkills[i];
                if (petSkill != 0)
                {
                    if (!Sys_Skill.Instance.IsActiveSkill(petSkill))
                    {
                        CSVPassiveSkillInfo.Data checkPetSkillData = CSVPassiveSkillInfo.Instance.GetConfData(petSkill);
                        skill_id2 = checkPetSkillData.skill_id;
                        skill_level2 = checkPetSkillData.level;
                    }
                    else
                    {
                        CSVActiveSkillInfo.Data checkPetSkillData = CSVActiveSkillInfo.Instance.GetConfData(petSkill);
                        if (null != checkPetSkillData)
                        {
                            skill_id2 = checkPetSkillData.skill_id;
                            skill_level2 = checkPetSkillData.level;
                        }
                        else
                        {
                            DebugUtil.LogError($"Not find id = {skillId} in CSVActiveSkillInfo and CSVPassiveSkillInfo");
                        }
                    }
                }

                if (skill_id1 == skill_id2 && skill_level1 > skill_level2)
                {
                    mId = petSkill;
                    return true;
                }
            }
            return false;
        }

        public bool IsHasHighBuildSkill(uint skillId)
        {
            List<uint> skillList = new List<uint>();
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return false;
            uint skill_id1 = 0;
            uint skill_level1 = 0;
            uint skill_id2 = 0;
            uint skill_level2 = 0;
            if (!Sys_Skill.Instance.IsActiveSkill(skillId))
            {
                CSVPassiveSkillInfo.Data checkPetSkillData = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                skill_id1 = checkPetSkillData.skill_id;
                skill_level1 = checkPetSkillData.level;
            }
            else
            {
                CSVActiveSkillInfo.Data checkPetSkillData = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (null != checkPetSkillData)
                {
                    skill_id1 = checkPetSkillData.skill_id;
                    skill_level1 = checkPetSkillData.level;
                }
                else
                {
                    DebugUtil.LogError($"Not find id = {skillId} in CSVActiveSkillInfo and CSVPassiveSkillInfo");
                }
            }

            if (skill_id1 == 0)
                return false;
            for (int i = 0; i < petUnit.BuildInfo.BuildSkills.Count; i++)
            {
                uint petSkill = petUnit.BuildInfo.BuildSkills[i];
                if (petSkill != 0)
                {
                    if (!Sys_Skill.Instance.IsActiveSkill(petSkill))
                    {
                        CSVPassiveSkillInfo.Data checkPetSkillData = CSVPassiveSkillInfo.Instance.GetConfData(petSkill);
                        skill_id2 = checkPetSkillData.skill_id;
                        skill_level2 = checkPetSkillData.level;
                    }
                    else
                    {
                        CSVActiveSkillInfo.Data checkPetSkillData = CSVActiveSkillInfo.Instance.GetConfData(petSkill);
                        if (null != checkPetSkillData)
                        {
                            skill_id2 = checkPetSkillData.skill_id;
                            skill_level2 = checkPetSkillData.level;
                        }
                        else
                        {
                            DebugUtil.LogError($"Not find id = {skillId} in CSVActiveSkillInfo and CSVPassiveSkillInfo");
                        }
                    }
                }
                if (skill_id1 == skill_id2 && skill_level1 < skill_level2)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 废弃
        /// </summary>
        /// <returns></returns>
        public uint GetPeBuildLuckValue()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return 0;

            return 0;
        }

        /// <summary>
        /// 宠物已改造次数
        /// </summary>
        /// <returns></returns>
        public int GetPeBuildCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return 0;

            return (int)petUnit.BuildInfo.BuildCount;
        }

        /// <summary>
        /// 获取剩余改造次数
        /// </summary>
        /// <returns></returns>
        public int GetPeCanBuildCount()
        {
            return GetPetLevelCanRemakeTimes() - GetPeBuildCount();
        }

        /// <summary>
        /// 获取宠物可改造次数
        /// </summary>
        /// <returns></returns>
        public int GetPetLevelCanRemakeTimes()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.SimpleInfo))
                return 0 + GetPetDemonSoulRemakeTimes();
            var level = petUnit.SimpleInfo.Level;
            var currentPetMaxCount = petData.max_remake_num;

            var petNewReBuildDatas = CSVPetNewReBuild.Instance.GetAll();
            for (int i = petNewReBuildDatas.Count - 1; i >= 0; i--)
            {
                CSVPetNewReBuild.Data data = petNewReBuildDatas[i];
                if (data.need_pet_lv <= level && currentPetMaxCount >= data.id)
                {
                    return (int)data.id + GetPetDemonSoulRemakeTimes();
                }
            }
            return 0 + GetPetDemonSoulRemakeTimes();
        }

        /// <summary>
        /// 魔魂产生的额外改造次数
        /// </summary>
        /// <returns></returns>
        public int GetPetDemonSoulRemakeTimes()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.PetSoulUnit))
                return 0;

            return (int)petUnit.PetSoulUnit.RemakeCount;
        }

        public List<uint> GetPetBuildSkillList()
        {
            List<uint> skillList = new List<uint>();
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo || null == petUnit.BuildInfo.BuildSkills))
                return skillList;
            skillList.AddRange(petUnit.BuildInfo.BuildSkills);
            return skillList;
        }

        /// <summary>
        /// 获取改造技能格子数量
        /// </summary>
        /// <returns></returns>
        public int GetPeBuildtSkillGridsCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo || null == petUnit.BuildInfo.BuildSkills))
                return 0;

            return (int)petUnit.BuildInfo.BuildSkills.Count;
        }

        /// <summary>
        /// 获取改造技能数量
        /// </summary>
        /// <returns></returns>
        public int GetPeBuildtSkillCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo || null == petUnit.BuildInfo.BuildSkills))
                return 0;
            return petUnit.BuildInfo.BuildSkills.Count;
        }

        /// <summary>
        /// 获取改造技能数量 实际数量 去除 id = 0;
        /// </summary>
        /// <returns></returns>
        public int GetPeBuildtSkillNotZeroCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo || null == petUnit.BuildInfo.BuildSkills))
                return 0;
            var count = 0;
            for (int i = 0; i < petUnit.BuildInfo.BuildSkills.Count; i++)
            {
                if (petUnit.BuildInfo.BuildSkills[i] != 0)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取改造技能数量 实际数量 去除 id = 0,的第一个下标;
        /// </summary>
        /// <returns></returns>
        public int GetPeBuildtSkillNotZeroFirstIndex()
        {
            var count = -1;
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo || null == petUnit.BuildInfo.BuildSkills))
                return count;
            
            for (int i = 0; i < petUnit.BuildInfo.BuildSkills.Count; i++)
            {
                if (petUnit.BuildInfo.BuildSkills[i] != 0)
                {
                    return i;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取预览出现得技能数量
        /// </summary>
        /// <returns></returns>
        public int GetBuildPreviewSkillNotZeroCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return 0;
            var count = 0;
            for (int i = 0; i < petUnit.BuildInfo.SkillTemp.Count; i++)
            {
                if (petUnit.BuildInfo.SkillTemp[i] != 0)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取对应改造次数对应的技能数
        /// </summary>
        /// <returns></returns>
        public int GetPreviewSkillNotZeroCountByRemakeTimes(int times)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return 0;
            var lst = Sys_Pet.Instance.BuildSkillNum;
            int maxNum = (int)lst[times];
            var newRebuild = CSVPetNewReBuild.Instance.GetConfData((uint)times + 1);
            if (null != newRebuild)
            {
                var currentCount = 0;
                for (int i = (int)newRebuild.max_skill - maxNum, len = (int)newRebuild.max_skill; i < len; i++)
                {
                    if (petUnit.BuildInfo.BuildSkills[i] != 0)
                    {
                        currentCount++;
                    }
                }

                return currentCount;
            }
            return 0;
        }


        /// <summary>
        /// 获取重置改造的实有技能差
        /// </summary>
        /// <returns></returns>
        public int GetReRemakdPreviewSkillNotZeroCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return 0;
            var lst = Sys_Pet.Instance.BuildSkillNum;
            int maxNum = (int)lst[(int)petUnit.BuildInfo.Index - 1];
            var count = 0;
            for (int i = 0; i < petUnit.BuildInfo.SkillTemp.Count; i++)
            {
                if (petUnit.BuildInfo.SkillTemp[i] != 0)
                {
                    count++;
                }
            }
            var newRebuild = CSVPetNewReBuild.Instance.GetConfData(petUnit.BuildInfo.Index);
            if(null != newRebuild)
            {
                var currentCount = 0;
                for (int i = (int)newRebuild.max_skill - maxNum, len = (int)newRebuild.max_skill; i < len; i++)
                {
                    if (petUnit.BuildInfo.BuildSkills[i] != 0)
                    {
                        currentCount++;
                    }
                }

                count = currentCount - count;
            }
            
            return count;
        }

        /// <summary>
        /// 获取重置改造时的技能预览
        /// </summary>
        /// <returns></returns>
        public List<uint> GetReRemakePreviewSkill()
        {
            List<uint> skills = new List<uint>(8);
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return skills;
            var lst = Sys_Pet.Instance.BuildSkillNum;
            int maxNum = (int)lst[(int)petUnit.BuildInfo.Index - 1];
            int tempCount = petUnit.BuildInfo.SkillTemp.Count;
            skills.AddRange(petUnit.BuildInfo.BuildSkills);
            var newRebuild = CSVPetNewReBuild.Instance.GetConfData(petUnit.BuildInfo.Index);
            if (null != newRebuild)
            {
                for (int i = (int)newRebuild.max_skill - maxNum, len = (int)newRebuild.max_skill; i < len; i++)
                {
                    int index = i - (int)newRebuild.max_skill + maxNum;
                    if(index >= 0 && index < tempCount)
                    {
                        skills[i] = petUnit.BuildInfo.SkillTemp[index];
                    } 
                }
            }
            return skills;
        }

        /// <summary>
        /// 获取改造次数对应的技能
        /// </summary>
        /// <returns></returns>
        public List<uint> GetPetRemakeSkilByRemakeTimes(int times)
        {
            List<uint> skills = new List<uint>(3);
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return skills;
            var lst = Sys_Pet.Instance.BuildSkillNum;
            int maxNum = (int)lst[times];

            var newRebuild = CSVPetNewReBuild.Instance.GetConfData((uint)times + 1);
            if (null != newRebuild)
            {
                for (int i = (int)newRebuild.max_skill - maxNum, len = (int)newRebuild.max_skill; i < len; i++)
                {
                    skills.Add(petUnit.BuildInfo.BuildSkills[i]);
                }
            }
            return skills;
        }

        /// <summary>
        /// 获取预览得总技能数量
        /// </summary>
        /// <returns></returns>
        public int GetBuildPreviewSkillCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return 0;
            return petUnit.BuildInfo.SkillTemp.Count;
        }

        /// <summary>
        /// 设置 宠物的改造幸运值（废弃）
        /// </summary>
        /// <param name="value"></param>
        public void SetPeBuildtLuckValue(uint value)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return;
        }

        /// <summary>
        /// 获取改造档位总数
        /// </summary>
        /// <returns></returns>
        public uint GetPeBuildGradeCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return 0;

            return petUnit.BuildInfo.VitGrade
                + petUnit.BuildInfo.SnhGrade
                + petUnit.BuildInfo.IntenGrade
                + petUnit.BuildInfo.SpeedGrade
                + petUnit.BuildInfo.MagicGrade;
        }

        /// <summary>
        /// 当前改造次数应该获取的最大档位数量
        /// </summary>
        /// <returns></returns>
        public uint GetPetWillBuildGradeCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return 0;

            return petUnit.BuildInfo.BuildCount * 5;
        }

        /// <summary>
        /// 是否是改造技能
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public bool IsBuildSkill(uint skillId)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo || null == petUnit.BuildInfo.BuildSkills))
                return false;
            return petUnit.BuildInfo.BuildSkills.Contains(skillId);
        }

        /// <summary>
        /// 是否是该宠物的专属技能
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public bool IsUniqueSkill(uint skillId)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || null == petUnit.BaseSkillInfo.UniqueSkills))
                return false;
            return GetUniqueSkills().Contains(skillId);
        }

        /// <summary>
        /// 是否有自动进场技能
        /// </summary>
        /// <returns></returns>
        public bool IsHasAutoBlinkSkill()
        {
            if (null != petUnit && null != petUnit.BaseSkillInfo)
            {
                for (int i = 0; i < petUnit.BaseSkillInfo.Skills.Count; i++)
                {
                    if (Sys_Pet.Instance.PetAutoBlinkSkill.Contains(petUnit.BaseSkillInfo.Skills[i]))
                    {
                        return true;
                    }
                }
                for (int i = 0; i < petUnit.BaseSkillInfo.UniqueSkills.Count; i++)
                {
                    if (Sys_Pet.Instance.PetAutoBlinkSkill.Contains(petUnit.BaseSkillInfo.UniqueSkills[i]))
                    {
                        return true;
                    }
                }
            }

            if (null != petUnit && null != petUnit.BuildInfo)
            {
                for (int i = 0; i < petUnit.BuildInfo.BuildSkills.Count; i++)
                {
                    if (Sys_Pet.Instance.PetAutoBlinkSkill.Contains(petUnit.BuildInfo.BuildSkills[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string GetPetNmae()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.SimpleInfo))
                return "";
            return Sys_Pet.Instance.GetPetNmaeBySeverName(petData.id, petUnit.SimpleInfo.Name);
        }

        public uint GetFollowPetInfo()
        {
            uint flag = 0;
            if (GetPetMaxGradeCount() == GetPetGradeCount())
            {
                flag = 1;
            }
            return petData.id * 10 + flag;
        }

        //通过属性id获取该属性已配分的属性点
        public long GetAssignAttrPointByAttrId(uint id, uint index)
        {
            long num = 0;
            if (null == petUnit)
            {
                return 0;
            }
            switch (id)
            {
                case (int)EBaseAttr.Vit:
                    num = petUnit.PetPointPlanData.Plans[(int)index].VitAssign;
                    break;
                case (int)EBaseAttr.Speed:
                    num = petUnit.PetPointPlanData.Plans[(int)index].SpeedAssign;
                    break;
                case (int)EBaseAttr.Inten:
                    num = petUnit.PetPointPlanData.Plans[(int)index].IntenAssign;
                    break;
                case (int)EBaseAttr.Magic:
                    num = petUnit.PetPointPlanData.Plans[(int)index].MagicAssign;
                    break;
                case (int)EBaseAttr.Snh:
                    num = petUnit.PetPointPlanData.Plans[(int)index].SnhAssign;
                    break;
                default:
                    break;
            }
            return num;
        }

        //获取该属性的自带档位
        private uint GetPetInitGradeByAttrId(uint id)
        {
            uint num = 0;
            if (null == petUnit)
            {
                return 0;
            }
            switch (id)
            {
                case (int)EBaseAttr.Vit:
                    num = petUnit.GradeInfo.VitGrade;
                    break;
                case (int)EBaseAttr.Speed:
                    num = petUnit.GradeInfo.SpeedGrade;
                    break;
                case (int)EBaseAttr.Inten:
                    num = petUnit.GradeInfo.IntenGrade;
                    break;
                case (int)EBaseAttr.Magic:
                    num = petUnit.GradeInfo.MagicGrade;
                    break;
                case (int)EBaseAttr.Snh:
                    num = petUnit.GradeInfo.SnhGrade;
                    break;
                default:
                    break;
            }
            return num;
        }

        //获取该属性的改造档位
        private uint GetPetRemouldGradeByAttrId(uint id)
        {
            uint num = 0;
            if (null == petUnit)
            {
                return 0;
            }
            switch (id)
            {
                case (int)EBaseAttr.Vit:
                    num = petUnit.BuildInfo.VitGrade;
                    break;
                case (int)EBaseAttr.Speed:
                    num = petUnit.BuildInfo.SpeedGrade;
                    break;
                case (int)EBaseAttr.Inten:
                    num = petUnit.BuildInfo.IntenGrade;
                    break;
                case (int)EBaseAttr.Magic:
                    num = petUnit.BuildInfo.MagicGrade;
                    break;
                case (int)EBaseAttr.Snh:
                    num = petUnit.BuildInfo.SnhGrade;
                    break;
                default:
                    break;
            }
            return num;
        }

        // 宠物1级属性=(初始属性+等级属性+分配的1级属性+等级*档位数*档位系数/10000)*(1+满档加成)
        //基础属性=初始属性+等级属性+等级*(总档位数-改造档位数)*档位系数/10000
        //加点点数=分配的1级属性


        //获取该属性的初始属性+等级属性
        public long GetOrigionAttrPointByAttrId(uint id)
        {
            long initNum = 0;
            long lvNum = 0;
            long attrNum = 0;
            uint petLvId = (petData.attr_id - 1) * 100 + petUnit.SimpleInfo.Level;
            CSVPetNewlv.Data csvPetNewlvData = CSVPetNewlv.Instance.GetConfData(petLvId);
            for (int i = 0; i < petData.init_attr.Count; ++i)
            {
                if (petData.init_attr[i][0] == id)
                {
                    initNum = petData.init_attr[i][1];
                    break;
                }
            }
            for (int i = 0; i < csvPetNewlvData.attr.Count; ++i)
            {
                if (csvPetNewlvData.attr[i][0] == id)
                {
                    lvNum = csvPetNewlvData.attr[i][1];
                    break;
                }
            }
            attrNum = initNum + lvNum;
            return attrNum;
        }

        //获取该属性的基础属性点
        public long GetBasicAttrPointByAttrId(uint id)
        {
            long attrNum = 0;
            uint gradeCoe = CSVPetNewParam.Instance.GetConfData(3).value;
            attrNum = GetOrigionAttrPointByAttrId(id) + petUnit.SimpleInfo.Level * GetPetInitGradeByAttrId(id) * gradeCoe / 10000;
            return attrNum;
        }

        //获取该属性的改造额外点数         改造额外点数=等级*改造档位数*档位系数/10000
        public long GetRemouldAddAttrPointByAttrId(uint id)
        {
            long attrNum = 0;
            uint gradeCoe = CSVPetNewParam.Instance.GetConfData(3).value;
            attrNum = petUnit.SimpleInfo.Level * GetPetRemouldGradeByAttrId(id) * gradeCoe / 10000;
            return attrNum;
        }

        //获取该属性的档位加成点数  挡位加成点数=(初始属性+等级属性+分配的1级属性+等级*档位数*档位系数/10000)*满档加成
        public long GetGradeAddAttrPointByAttrId(uint id, float gradePerent,uint index)
        {
            long attrNum = 0;
            uint gradeCoe = CSVPetNewParam.Instance.GetConfData(3).value;
            long num = petUnit.SimpleInfo.Level * GetPetInitGradeByAttrId(id) * gradeCoe / 10000;
            attrNum = (GetOrigionAttrPointByAttrId(id) + num + GetAssignAttrPointByAttrId(id, index)) * (long)gradePerent / 100;
            return attrNum;
        }

        /// <summary>
        /// 通过改造的次数index 获取改造评级分数
        /// </summary>
        /// <param name="index"> 改造次数 的index</param>
        /// <returns></returns>
        public uint GetBuildPointByIndex(int index)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return 0;
            if (index >= 0 && index < petUnit.BuildInfo.GradeScore.Count)
            {
                return petUnit.BuildInfo.GradeScore[index];
            }
            return 0;
        }

        /// <summary>
        ///  获取是否有改造未保存
        /// </summary>
        /// <returns></returns>
        public bool GetBuildNotSave()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return false;
            return petUnit.BuildInfo.BuildState == (uint)EnumPetBuildState.Building || petUnit.BuildInfo.BuildState == (uint)EnumPetBuildState.ReBuilding;
        }

        /// <summary>
        ///  获取是否有改造技能未保存
        /// </summary>
        /// <returns></returns>
        public bool GetBuildSkillNotSave()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return false;
            /// <summary>
            ///true-改造宠物未保存的数据，false-领悟技能或重铸档位未保存的数据
            /// </summary>
            return petUnit.BuildInfo.BuildState == (uint)EnumPetBuildState.LearnSkill;
        }

        /// <summary>
        ///  是否有重塑未保存
        /// </summary>
        /// <returns></returns>
        public bool GetBuildRecastNotSave()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return false;
            /// <summary>
            ///true-改造宠物未保存的数据，false-领悟技能或重铸档位未保存的数据
            /// </summary>
            return petUnit.BuildInfo.BuildState == (uint)EnumPetBuildState.ReGrade;
        }
        /// <summary>
        /// 是否改造最大
        /// </summary>
        /// <returns></returns>
        public bool GetIsBuildMax()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return false;

            return petUnit.BuildInfo.BuildCount >= petData.max_remake_num;
        }

        /// <summary>
        /// 是否处于重新改造状态 index > 0 即 为正在重置某一改
        /// </summary>
        /// <returns></returns>
        public bool IsOnReRemake()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return false;
            return petUnit.BuildInfo.Index > 0;
        }

        /// <summary>
        /// 通过改造次数-获取增加的档位值总量
        /// </summary>
        /// <returns></returns>
        public uint GetAddGradeCountByReamkeTime(int times)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BuildInfo))
                return 0;
            if (times < 0 || times >= petUnit.BuildInfo.EachBuildGrade.Count)
                return 0;
            return petUnit.BuildInfo.EachBuildGrade[times];
        }

        /// <summary>
        /// 是否有专属坐骑技能
        /// </summary>
        /// <returns></returns>
        public bool IsHasMountUnique()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || (null == petUnit.BaseSkillInfo.UniqueRidingSkills)))
                return false;
            return petUnit.BaseSkillInfo.UniqueRidingSkills.Count > 0;
        }

        /// <summary>
        /// 检查技能是否是改宠物的专属技能
        /// </summary>
        /// <returns></returns>
        public bool CheckIsMountUniqueSkillBySkillId(uint skillId)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || (null == petUnit.BaseSkillInfo.UniqueRidingSkills)))
                return false;
            return petUnit.BaseSkillInfo.UniqueRidingSkills.Contains(skillId);
        }

        /// <summary>
        /// 是否契约了其他宠物-自己是主宠
        /// </summary>
        /// <returns></returns>
        public bool HasSubPet()
        {
            if (null == petUnit || null == petUnit.SimpleInfo)
                return false;
            if (null != petUnit.SimpleInfo.ContractPets)
            {
                for (int i = 0; i < petUnit.SimpleInfo.ContractPets.Count; i++)
                {
                    if (petUnit.SimpleInfo.ContractPets[i] > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 是否是其他宠物的契约宠物-自己是副宠
        /// </summary>
        /// <returns></returns>
        public bool HasPartnerPet()
        {
            if (null == petUnit || null == petUnit.SimpleInfo)
                return false;
            return petUnit.SimpleInfo.ContractPetUid > 0;
        }

        /// <summary>
        /// 获取当前宠物的契约宠物uid
        /// </summary>
        /// <returns></returns>
        public List<uint> GetSubsPetUid()
        {
            List<uint> uidList = new List<uint>();
            if (null == petUnit || null == petUnit.SimpleInfo || null == petUnit.SimpleInfo.ContractPets)
                return uidList;
            uidList.AddRange(petUnit.SimpleInfo.ContractPets);
            return uidList;
        }

        /// <summary>
        /// 返回是否是 当前宠物的子契约宠物
        /// </summary>
        /// <returns></returns>
        public bool IsMySubPetUid(uint petId)
        {
            if (null == petUnit || null == petUnit.SimpleInfo || null == petUnit.SimpleInfo.ContractPets)
                return false;
            petUnit.SimpleInfo.ContractPets.Contains(petId);
            return false;
        }

        /// <summary>
        /// 返回是否是 当前宠物的父契约宠物
        /// </summary>
        /// <returns></returns>
        public bool IsMyPartnerPetUid(uint petId)
        {
            if (null == petUnit || null == petUnit.SimpleInfo)
                return false;
            return petId == petUnit.SimpleInfo.ContractPetUid;
        }

        /// <summary>
        /// 通过index 获取对应契约位的宠物id
        /// </summary>
        /// <returns></returns>
        public uint GetSubByIndex(int index)
        {
            if (null == petUnit || null == petUnit.SimpleInfo)
                return 0;
            if (index < 0 || index >= petUnit.SimpleInfo.ContractPets.Count)
                return 0;

            return petUnit.SimpleInfo.ContractPets[index];
        }

        /// <summary>
        /// 通过uid 获取对应契约位的宠物Index
        /// </summary>
        /// <returns></returns>
        public int GetIndexByPetUid(uint uid)
        {
            if (null == petUnit || null == petUnit.SimpleInfo)
                return 0;

            return petUnit.SimpleInfo.ContractPets.IndexOf(uid);
        }

        /// <summary>
        /// 设置契约关系
        /// </summary>
        public void SetContractInfo(uint contractPetUid, RepeatedField<uint> ContractUids)
        {
            if(null != petUnit)
            {
                petUnit.SimpleInfo.ContractPetUid = contractPetUid;
                petUnit.SimpleInfo.ContractPets.Clear();
                petUnit.SimpleInfo.ContractPets.AddRange(ContractUids);
            }
        }

        /// <summary>
        /// 设置契约关系-通过契约位
        /// </summary>
        public void SetContractInfoByPos(uint pos, uint uid)
        {
            if (null != petUnit && null != petUnit.SimpleInfo)
            {
                petUnit.SimpleInfo.ContractPets[(int)pos] = uid;
            }
        }

        /// <summary>
        /// 是否拥有多个宠物技能
        /// </summary>
        /// <returns></returns>
        public bool HasManyMountSkill()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || (null == petUnit.BaseSkillInfo.RidingSkills)))
                return false;
            return petUnit.BaseSkillInfo.RidingSkills.Count >= 1;
        }

        /// <summary>
        /// 是否拥有一样的技能了
        /// </summary>
        /// <returns></returns>
        public bool IsHasSameMountSkill(uint skillId)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || (null == petUnit.BaseSkillInfo.RidingSkills || null == petUnit.BaseSkillInfo.UniqueRidingSkills)))
                return false;

            return petUnit.BaseSkillInfo.RidingSkills.Contains(skillId) || petUnit.BaseSkillInfo.UniqueRidingSkills.Contains(skillId);
        }

        /// <summary>
        /// 获取所有契约技能-
        /// </summary>
        public List<uint> GetMountSkill()
        {
            List<uint> skillList = new List<uint>();
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || (null == petUnit.BaseSkillInfo.RidingSkills || null == petUnit.BaseSkillInfo.UniqueRidingSkills)))
                return skillList;
            skillList.AddRange(petUnit.BaseSkillInfo.UniqueRidingSkills);
            skillList.AddRange(petUnit.BaseSkillInfo.RidingSkills);
            return skillList;
        }

        /// <summary>
        /// 是否是互斥技能
        /// </summary>
        /// <returns></returns>
        public bool IsMutexId(uint mutueId)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || (null == petUnit.BaseSkillInfo.RidingSkills || null == petUnit.BaseSkillInfo.UniqueRidingSkills)))
                return false;
            List<uint> list = new List<uint>(petUnit.BaseSkillInfo.RidingSkills.Count + petUnit.BaseSkillInfo.UniqueRidingSkills.Count);
            list.AddRange(petUnit.BaseSkillInfo.RidingSkills);
            list.AddRange(petUnit.BaseSkillInfo.UniqueRidingSkills);
            for (int i = 0; i < list.Count; i++)
            {
                var passiveSkill = list[i];
                CSVPassiveSkillInfo.Data passiveSkillInfo = CSVPassiveSkillInfo.Instance.GetConfData(passiveSkill);
                if (null != passiveSkillInfo)
                {
                    if (passiveSkillInfo.mutex_id == mutueId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 获取普通骑术技能数量
        /// </summary>
        public int GetRidingSkillCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || (null == petUnit.BaseSkillInfo.RidingSkills)))
                return 0;

            return petUnit.BaseSkillInfo.RidingSkills.Count;
        }

        /// <summary>
        /// 获取骑术专属技能数量
        /// </summary>
        public int GetUniqueRidingSkillsCount()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.BaseSkillInfo || (null == petUnit.BaseSkillInfo.UniqueRidingSkills)))
                return 0;
            return petUnit.BaseSkillInfo.UniqueRidingSkills.Count;
        }

        /// <summary>
        /// 获取骑术技能累计消耗能量
        /// </summary>
        public uint GetMountSkillDailyEnergy(uint skillId)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.SkillCostInfo || (null == petUnit.SkillCostInfo.SkillCostInfo)))
                return 0;
            for (int i = 0; i < petUnit.SkillCostInfo.SkillCostInfo.Count; i++)
            {
                if (skillId == petUnit.SkillCostInfo.SkillCostInfo[i].SkillId)
                {
                    return petUnit.SkillCostInfo.SkillCostInfo[i].TotalEnergy;
                }
            }
            return 0;
        }

        /// <summary>
        /// 是否已驯化
        /// </summary>
        public bool GetPetIsDomestication()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.SimpleInfo))
                return false;
            return petUnit.SimpleInfo.MountDomestication == 1;
        }

        /// <summary>
        /// 宠物是否有装备
        /// </summary>
        /// <returns></returns>
        public bool IsHasEquip()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.PetEquipInfo))
                return false;
            if (null == petUnit.PetEquipInfo.Item)
                return false;
            return petUnit.PetEquipInfo.Item.Count > 0;
        }

        /// <summary>
        /// 获取对应类型宠物是否有装备 有则返回对应装备数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ItemData GetPetEquipByType(uint type)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.PetEquipInfo))
                return null;
            if (null == petUnit.PetEquipInfo.Item)
                return null;

            for (int i = 0; i < petUnit.PetEquipInfo.Item.Count; i++)
            {
                var kv = petUnit.PetEquipInfo.Item[i];
                var petEquipData = CSVPetEquip.Instance.GetConfData(kv.Id);

                if (null != petEquipData && petEquipData.equipment_category == type) 
                {
                    ItemData itemData = new ItemData();
                    itemData.SetData(0, kv.Uuid, kv.Id, kv.Count, kv.Position, kv.ShowNewIcon, kv.Bind, kv.Equipment, kv.Essence, kv.Marketendtime, null, kv.Crystal, kv.Ornament, kv.PetEquip, kv.Islocked);
                    itemData.outTime = kv.OutTime;
                    return itemData;
                }
            }
            return null;
        }

        /// <summary>
        /// 返回宠物激活的套装外观id
        /// </summary>
        /// <returns></returns>
        public uint GetPetSuitFashionId()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.PetEquipInfo))
                return 0;
            if (null == petUnit.PetEquipInfo.Item || petUnit.PetEquipInfo.Item.Count < 3)
                return 0;
            uint fashionId = petUnit.PetEquipInfo.Item[0].PetEquip.SuitAppearance;
            for (int i = 1; i < petUnit.PetEquipInfo.Item.Count; i++)
            {
                var petEquipItem = petUnit.PetEquipInfo.Item[i];
                if(fashionId != petEquipItem.PetEquip.SuitAppearance)
                {
                    fashionId = 0;
                    break;
                }
            }
            if(fashionId > 0)
            {
                CSVPetEquipSuitAppearance.Data petEquipSuitAppearance = CSVPetEquipSuitAppearance.Instance.GetConfData(fashionId);
                if(null != petEquipSuitAppearance)
                {
                    fashionId = (null != petEquipSuitAppearance && petEquipSuitAppearance.pet_id.Contains(petData.id)) ? petEquipSuitAppearance.show_id : 0;
                }
            }
            return fashionId;
        }

        public bool PetEquipIsFitInPet(ulong itemuid)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.PetEquipInfo))
                return false;
            if (null == petUnit.PetEquipInfo.Item)
                return false;
           
            for (int i = 0; i < petUnit.PetEquipInfo.Item.Count; i++)
            {
                if (itemuid == petUnit.PetEquipInfo.Item[i].Uuid)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetPetEquip(uint type, Item item)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.PetEquipInfo))
                return;
            if (null == petUnit.PetEquipInfo.Item)
                return;
            bool hasItem = false;
            for (int i = 0; i < petUnit.PetEquipInfo.Item.Count; i++)
            {
                var petEquipItem = petUnit.PetEquipInfo.Item[i];
                var petEquipData = CSVPetEquip.Instance.GetConfData(petEquipItem.Id);
                if (type == petEquipData.equipment_category)
                {
                    hasItem = true;
                    if (item == null)
                    {
                        petUnit.PetEquipInfo.Item.RemoveAt(i);
                        break;
                    }
                    else
                    {
                        petUnit.PetEquipInfo.Item[i] = item;
                        break;
                    }
                }
            }
            if (null != item && !hasItem)
            {
                petUnit.PetEquipInfo.Item.Add(item);
            }
        }

        /// <summary>
        /// 获取宠物身上有宠物套装技能的装备
        /// </summary>
        /// <param name="type"></param>
        /// <param name="item"></param>
        public List<Item> GetPetSuitSkillEquipItem()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.PetEquipInfo))
                return null;
            if (null == petUnit.PetEquipInfo.Item)
                return null;

            List<Item> petEquips = null;
            for (int i = 0; i < petUnit.PetEquipInfo.Item.Count; i++)
            {
                var petEquipItem = petUnit.PetEquipInfo.Item[i];
                if(petEquipItem.PetEquip.SuitSkill != 0)
                {
                    if(petEquips == null)
                    {
                        petEquips = new List<Item>(3);
                    }
                    petEquips.Add(petEquipItem);
                }
            }
            return petEquips;
        }

        /// <summary>
        /// 获取宠物身上穿的装备
        /// </summary>
        public List<Item> GetPetEquipItems()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.PetEquipInfo))
                return null;
            if (null == petUnit.PetEquipInfo.Item)
                return null;

            List<Item> petEquips = null;
            for (int i = 0; i < petUnit.PetEquipInfo.Item.Count; i++)
            {
                var petEquipItem = petUnit.PetEquipInfo.Item[i];
                if (petEquips == null)
                {
                    petEquips = new List<Item>(3);
                }
                petEquips.Add(petEquipItem);
            }
            return petEquips;
        }

        public bool CheckHasSuitSkill(out uint outSkill)
        {
            outSkill = 0;
            if (null == petUnit || (null != petUnit && null == petUnit.PetEquipInfo))
                return false;
            if (null == petUnit.PetEquipInfo.Item || petUnit.PetEquipInfo.Item.Count < 3)
                return false;
            uint skillId = petUnit.PetEquipInfo.Item[0].PetEquip.SuitSkill;
            CSVPetEquipSuitSkill.Data skillInfo = CSVPetEquipSuitSkill.Instance.GetConfData(petUnit.PetEquipInfo.Item[0].PetEquip.SuitSkill);
            if(null != skillInfo)
            {
                skillId = skillInfo.base_skill;
                var skills = GetBaseUniqueSkills();
                if (skills.Contains(skillInfo.base_skill))
                {
                    for (int i = 1; i < petUnit.PetEquipInfo.Item.Count; i++)
                    {
                        var petEquipItem = petUnit.PetEquipInfo.Item[i];
                        CSVPetEquipSuitSkill.Data tempSkillInfo = CSVPetEquipSuitSkill.Instance.GetConfData(petEquipItem.PetEquip.SuitSkill);
                        if(null != tempSkillInfo)
                        {
                            if (skillId != tempSkillInfo.base_skill)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    outSkill = petUnit.PetEquipInfo.Item[0].PetEquip.SuitSkill;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
        }

        /// <summary>
        /// 宠物当前的进阶次数
        /// </summary>
        /// <returns></returns>
        public uint GetAdvancedNum()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.SimpleInfo))
                return 0;
            return petUnit.SimpleInfo.PetStage;
        }

        //获取当前使用强化方案剩余点数
        public uint GetUseEnhanceFreePoint()
        {
            int index = (int)petUnit.EnhancePlansData.CurrentPlanIndex;
            uint use = petUnit.EnhancePlansData.Plans[index].UsePoint;
            uint total = petUnit.EnhancePlansData.TotalPoint;
            return total - use;
        }

        /// <summary>
        /// 是否装配了魂珠
        /// </summary>
        /// <returns></returns>
        public bool HasEquipDemonSpiritSphere()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.PetSoulUnit))
                return false;
            for (int i = 0; i < petUnit.PetSoulUnit.SoulBeads.Count; i++)
            {
                if(petUnit.PetSoulUnit.SoulBeads[i]!= 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取装配的对应魂珠等级
        /// </summary>
        /// <returns> Level </returns>
        public uint GetEquipSphereLevelByType(uint SphereType)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.PetSoulUnit))
                return 0;
            uint index = petUnit.PetSoulUnit.SoulBeads[(int)SphereType - 1];
            if(index == 0)
            {
                return 0;
            }
            var soulBeadInfos = Sys_Pet.Instance.petSoulBeadInfos;
            var count = soulBeadInfos.Count;
            for (int i = 0; i < count; i++)
            {
                var tempSoul = soulBeadInfos[i];
                if (tempSoul.Type == SphereType && tempSoul.Index == index)
                {
                    return tempSoul.Level;
                }
            }
            return 0;
        }

        /// <summary>
        /// 获取装配的总魂珠等级
        /// </summary>
        /// <returns> Level </returns>
        public uint GetEquipSphereTotalLevel()
        {
            uint count = 0;
            if (null == petUnit || (null != petUnit && null == petUnit.PetSoulUnit))
                return count;
            for (int i = 0; i < petUnit.PetSoulUnit.SoulBeads.Count; i++)
            {
                if(petUnit.PetSoulUnit.SoulBeads[i] != 0)
                {
                    count += GetEquipSphereLevelByType((uint)i + 1);
                }
            }
            return count;
        }

        /// <summary>
        /// 获取宠物身上对应的魂珠的下标（从1开始 0 未没有装配） 通过 类型 - 1 
        /// </summary>
        /// <returns></returns>
        public uint GetSoulSphereByIndex(int index)
        {
            if (null == petUnit || (null != petUnit && null == petUnit.PetSoulUnit))
                return 0;

            return petUnit.PetSoulUnit.SoulBeads[index];
        }

        /// <summary>
        /// 宠物是否激活专属魔魂
        /// </summary>
        /// <returns></returns>
        public bool GetDemonSpiritIsActive()
        {
            if (null == petUnit || (null != petUnit && null == petUnit.PetSoulUnit))
                return false;

            return petUnit.PetSoulUnit.IsActive;
        }

        /// <summary>
        /// 获取魔魂技能
        /// </summary>
        /// <returns></returns>
        public List<uint> GetDemonSpiritSkills()
        {
            List<uint> skillList = new List<uint>();
            if (null == petUnit || (null != petUnit && null == petUnit.PetSoulUnit))
                return skillList;
            if(petUnit.PetSoulUnit.IsActive)
            {
                skillList.Add(petUnit.PetSoulUnit.SkillId);
                var soulBeadInfos = Sys_Pet.Instance.petSoulBeadInfos;
                var count = soulBeadInfos.Count;
                for (int i = 0; i < petUnit.PetSoulUnit.SoulBeads.Count; i++)
                {
                    uint index = petUnit.PetSoulUnit.SoulBeads[i];
                    if (index != 0)
                    {
                        for (int j = 0; j < count; j++)
                        {
                            var tempSoul = soulBeadInfos[j];
                            if (tempSoul.Type == (i + 1) && tempSoul.Index == index)
                            {
                                for (int k = 0; k < tempSoul.SkillIds.Count; k++)
                                {
                                    if (tempSoul.SkillIds[k] != 0)
                                    {
                                        skillList.Add(tempSoul.SkillIds[k]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            return skillList;
        }

        /// <summary>
        /// id 规则  表id *10+index
        /// </summary>
        /// <returns></returns>
        public uint GetAppearanId()
        {
            if (null == petUnit || null == petUnit.SimpleInfo)
                return 0;

            return petUnit.SimpleInfo.FashionId * 10 + petUnit.SimpleInfo.FashionColorIndex;
        }
    }

    public class FightSimplePet
    {
        private uint uid;//宠物uid
        private SimplePet simpleInfo = new SimplePet();
        private long maxHp; // 最大血量
        private long currentHp;// 当前血量
        private long maxMp;// 最大蓝量
        private long currentMp;// 当前蓝量
        
        public uint GetUid()
        {
            return uid;
        }

        public SimplePet GetSimplePet()
        {
            return simpleInfo;
        }

        public float GetHpSliderValue()
        {
            return (float)currentHp / maxHp;
        }

        public float GetMpSliderValue()
        {
            return (float)currentMp / maxMp;
        }

        /// <summary>
        /// 是否出战了宠物
        /// </summary>
        /// <returns></returns>
        public bool HasFightPet()
        {
            return uid != 0;
        }

        public void InitFightPetData(uint uid, SimplePet pet, long maxHp, long currentHp, long maxMp, long currentMp)
        {
            if (uid == 0)
            {
                RemoveFightPetData();
                return;
            }            
                
            this.uid = uid;
            if(null != pet)
            {
                simpleInfo = pet;
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, "SimplePet is null");
            }

            SetAttr_HpAndMp(maxHp, currentHp, maxMp, currentMp);
        }

        private void SimpleClear()
        {
            if(null != simpleInfo)
            {
                simpleInfo.Level = 0;
                simpleInfo.PetId = 0;
                simpleInfo.Loyalty = 0;
            }
        }

        public void RemoveFightPetData()
        {
            uid = 0;
            maxHp = 0;
            currentHp = 0;
            maxMp = 0;
            currentMp = 0;
            Sys_Pet.Instance.currentPetUid = 0;
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnPetAddPonit, null);
        }

        /// <summary>
        /// 返回是不是和出战宠物uid 一致
        /// </summary>
        /// <param name="pet"></param>
        /// <returns></returns>
        public bool IsSamePet(PetUnit pet)
        {
            if (null == pet)
                return false;
            return IsSamePet(pet.Uid);
        }

        /// <summary>
        /// 返回是不是和出战宠物uid 一致
        /// </summary>
        /// <param name="petUid"></param>
        /// <returns></returns>
        public bool IsSamePet(uint petUid)
        {
            return uid == petUid;
        }

        //设置出战宠等级
        public void SetLevel(uint levle)
        {
            if (null != simpleInfo)
                simpleInfo.Level = levle;
        }

        //设置出战血量
        public void SetAttr_HpAndMp(long maxHp, long currentHp, long maxMp, long currentMp)
        {
            if(maxHp > 0)
            {
                this.maxHp = maxHp;
            }

            if(maxMp > 0)
            {
                this.maxMp = maxMp;
            }
            
            this.currentHp = currentHp;
            
            this.currentMp = currentMp;
        }
    }

    public partial class Sys_Pet : SystemModuleBase<Sys_Pet>
    {
        public List<ClientPet> petsList = new List<ClientPet>();
        public List<uint> abandonAutoPetList = new List<uint>();
        public List<uint> autoPetCatchCardQualityList = new List<uint>();

        public uint currentPetUid;

        public FightSimplePet fightPet = new FightSimplePet();

        public Dictionary<EBaseAttr, uint> baseAttrs2Id = new Dictionary<EBaseAttr, uint>();
        public ResLimit dailyTraining;
        public bool hasGetinitPet;
        public uint GoldPetExchangeNum { get; set; }
        private ulong addexp;
        public ulong addexpInBattleEnd;
        private bool showReExchange = false;
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnUnloadModel,    //卸载模型
            OnChangeStatePet,     //改变宠物状态
            OnAddPet,                //添加宠物
            OnAddFightPet,        //添加出战宠物
            OnNumberChangePet, // 放生宠物,融合 改变数量
            OnChoosePetCell,         //选中宠物格子
            OnChangePotency,      //宠物未分配潜能点改变
            OnUpdateAttr,         //属性更新
            OnUpdateExp,           //经验更新
            OnReNamePet,         //宠物重命名
            OnSelectItem,            //选中道具
            OnTriggerGuaranteed, // 触发保底机制
            OnLearnSkill,              //学技能
            OnLockStateChange,      //锁定状态改变
            OnCloseMixSelect, // 关闭选择宠物融合界面
            OnChangePostion,    //改变位置
            OnCancelPostion,     //取消换位
            OnGotoChangelPostion,
            OnUpdateAllPlayerAllocPoint,  //刷新全服玩家分配属性点
            OnChangeItemCount,      //道具数量改变
            OnStarMixEffect,            //开始融合特效
            OnRePlacePetMix,
            OnRemakeEnd,
            OnPetActivate,
            OnPetLoveExpUp,
            OnPetActivateStory,
            OnPetStroyClose,
            OnGetRemakeData,
            OnTemplePetBagChange,
            OnGetPetBankData,
            OnStorageChang,
            OnPetBankUnlock,
            OnPlayerCloseSeal,
            OnPetHpMpUpdate,          //宠物血蓝池更新
            OnChangeAutoPoint,        //宠物自动配分点数改变
            OnUpdateLoyalty,                //忠诚度更新
            OnChangeResPoint,            //抵抗强化点数变化
            OnUpdatePetInfo,           // 宠物信息刷新
            OnSkillToChange,      //通过技能跳转页签
            OnPatrolStateChange,   //巡逻状态切换(目前抓宠和巡逻挂机)
            OnGetAutoBlinkDataEnd,
            OnGuidChangeToggle,      //引导需要
            OnGetPetInfoForUplift,      //提升
            OnPetMountDomestication,    //驯化骑宠
            OnPetAutoPoint,            //自动加点
            OnPetRemakeRecastTipsEntry, // 当提示确定时
            OnPetRemakeSkillEnd, // 当技能重新改造后

            OnEnergyChargeEnd, //坐骑能量充能完毕
            OnContractChange, //契约位发生变化
            OnMountSkillChange, //坐骑技能发生变化
            OnMountScreemChange, //坐骑删选变化
            OnSmeltPetEquipEnd, //宠物装备炼化结束后
            OnSmeltItemShow, //宠物装备展示
            OnPetEquipMakeSuccess, //宠物装备展示
            OnPetSealSetting,        //宠物封印设置
            OnSpecialPetExDraw,    //特殊金宠抽奖

            OnPerFectFxChange,    //宠物完美改造次数变化
            OnEatFruitEnd,           // 宠物吃完果实后
            OnExchangTargetGoldPetSuccess, // 定向兑换宠物完毕
            OnAllocPointPlanRename,      //加点方案改名
            OnAllocPointPlanAdd,        //加点方案新增
            OnCorrectPlanRename,      //属性修正方案改名
            OnCorrectPointPlanAdd,        //属性修正方案新增
            OnSelectAddPointPlan,        //选中加点方案
            OnAllocEnhancePlanUse,  //强化加点方案使用
            OnGetPointPlanAttr,     //方案属性返回

            OnPetFirstChoiceUpdate,//首发宠物更新 			
            OnOwnDemonSpiritPetSelect,// 选中激活魔魂宠物
            OnActiveDemonSpiritSphere,// 激活魂珠
            OnRefreshDemonSpiritSkill,// 刷新魔魂技能
            OnEquipDemonSpiritSphere,// 装备魂珠/卸下
            OnDemonSpiritUpgrade,// 升级魂珠
            OnActiveDemonSpirit,// 激活魔魂
            OnActiveDemonSpiritRemake,// 激活魔魂改造,
            OnEquipDemonSpiritSphereLevelChange,// 穿戴等级发生变化,petuid /是否显示特效
            OnActiveOwnDemonSpiritOrRemakeAniUiAnimatorEnd,// 衔接特效表现
            OnSaveOrDelectDemonSpiritSphereSkill,// 保存或者放弃魔魂技能
            OnMountSkillItemSelect,// 选择契约强化道具书
            OnContractLevelUpRes, // 契约强化升级
            OnPetLockChange, //宠物锁变化
            OnBuyOrActivePetAppearance,//购买外观颜色
            OnPetEquipOrDownPetAppearance,//穿戴或者卸下时装
        }

        private void InitAttr2Id()
        {
            baseAttrs2Id.Add(EBaseAttr.Vit, (uint)EBaseAttr.Vit);
            baseAttrs2Id.Add(EBaseAttr.Snh, (uint)EBaseAttr.Snh);
            baseAttrs2Id.Add(EBaseAttr.Inten, (uint)EBaseAttr.Inten);
            baseAttrs2Id.Add(EBaseAttr.Speed, (uint)EBaseAttr.Speed);
            baseAttrs2Id.Add(EBaseAttr.Magic, (uint)EBaseAttr.Magic);
        }

        public override void Init()
        {
            InitAttr2Id();
            //上线通知的简单信息
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.SysInfoNtf, OnSysInfoNtf, CmdPetSysInfoNtf.Parser);
            //请求的详细信息 或者数据 更新
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.GetPetInfoReq, (ushort)CmdPet.PetInfoNtf, OnGetPetInfoNtf, CmdPetPetInfoNtf.Parser);

            //EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.FightPetNtf, OnFightPetNtf, CmdPetFightPetNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.AddPetNtf, OnAddPetNtf, CmdPetAddPetNtf.Parser);
            //EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.AttrNtf, OnPeAttrUpdate, CmdPetAttrNtf.Parser);
            //EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.DeviceExpNtf, OnDeviceExpNtf, CmdPetDeviceExpNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.AddExpNtf, OnAddExpNtf, CmdPetAddExpNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.AutoActivateStoryNtf, OnPetAutoActivateStoryNtf, CmdPetAutoActivateStoryNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.DiscovererNtf, OnPetDiscovererNtf, CmdPetDiscovererNtf.Parser);

            //宠物临时背包新增消息
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.TempPackNtf, OnPetTempPackNtf, CmdPetTempPackNtf.Parser);

            //宠物信息-出战状态
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.SetCurrentPetReq, (ushort)CmdPet.SetCurrentPetRes, ChangeStateRes, CmdPetSetCurrentPetRes.Parser);

            //宠物培养
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.AbandonPetReq, (ushort)CmdPet.AbandonPetRes, PetAbandonPetRes, CmdPetAbandonPetRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.RenameReq, (ushort)CmdPet.RenameRes, PetRenameRes, CmdPetRenameRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.AllPlayerAllocInfoReq, (ushort)CmdPet.AllPlayerAllocInfoRes, AllPlayerAllocInfoRes, CmdPetAllPlayerAllocInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.ChangePositionReq, (ushort)CmdPet.ChangePositionRes, PetChangePositionRes, CmdPetChangePositionRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.SetAutoPointReq, (ushort)CmdPet.SetAutoPointRes, OnSetAutoPointRes, CmdPetSetAutoPointRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.LoyaltyNtf, OnLoyaltyNtf, CmdPetLoyaltyNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.SkillAddExpReq, (ushort)CmdPet.SkillAddExpRes, PetSkillAddExpRes, CmdPetSkillAddExpRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.RemoveSkillReq, (ushort)CmdPet.RemoveSkillRes, PetRemoveSkillRes, CmdPetRemoveSkillRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.SkillLevelUpReq, (ushort)CmdPet.SkillLevelUpRes, PetSkillLevelUpRes, CmdPetSkillLevelUpRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.EnhanceExpNtf, OnEnhanceExpNtf, CmdPetEnhanceExpNtf.Parser);


            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.RemakeReq, (ushort)CmdPet.RemakeRes, PetRemakeRes, CmdPetRemakeRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.LearnSkillReq, (ushort)CmdPet.LearnSkillRes, PetLearnSkillRes, CmdPetLearnSkillRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.RemakeResetReq, (ushort)CmdPet.RemakeResetRes, OnPetReRemakeRes, CmdPetRemakeResetRes.Parser);

            //宠物图鉴.封印
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.GetHandbookReq, (ushort)CmdPet.GetHandbookRes, PetGetHandbookRes, CmdPetGetHandbookRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.LoveExpUpReq, (ushort)CmdPet.LoveExpUpRes, PetLoveExpUpRes, CmdPetLoveExpUpRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.ActivateReq, (ushort)CmdPet.ActivateRes, PetActivateRes, CmdPetActivateRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.ActivateStoryReq, (ushort)CmdPet.ActivateStoryRes, PetActivateStoryRes, CmdPetActivateStoryRes.Parser);

            //宠物临时背包
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.OutFromPetTempPackReq, (ushort)CmdPet.OutFromPetTempPackRes, PetOutFromPetTempPackRes, CmdPetOutFromPetTempPackRes.Parser);
            //宠物仓库
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.GetBankInfoReq, (ushort)CmdPet.GetBankInfoRes, PetGetBankInfoRes, CmdPetGetBankInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.BankUnlockReq, (ushort)CmdPet.BankUnlockRes, PetBankUnlockRes, CmdPetBankUnlockRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.BankMoveReq, (ushort)CmdPet.BankMoveRes, PetBankMoveRes, CmdPetBankMoveRes.Parser);

            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.ReExchangeGoldPetReq, (ushort)CmdPet.ReExchangeGoldPetRes, OnPetReExchangeGoldPetRes, CmdPetReExchangeGoldPetRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.ExchangeGoldPetReq, (ushort)CmdPet.ExchangeGoldPetRes, OnExchangeGoldPetRes, CmdPetExchangeGoldPetRes.Parser);

            //宠物血蓝池同步信息
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.RecoverNtf, OnRecoverNtf, CmdPetRecoverNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.AutoBlinkInfoReq, (ushort)CmdPet.AutoBlinkInfoRes, PetAutoBlinkInfoRes, CmdPetAutoBlinkInfoRes.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.TriggerNtf, OnTriggerNtf, CmdPetTriggerNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.BagUnlockReq, (ushort)CmdPet.BagUnlockRes, PetBagUnlockRes, CmdPetBagUnlockRes.Parser);

            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.SetCurrentMountReq, (ushort)CmdPet.SetCurrentMountRes, PetSetCurrentMountRes, CmdPetSetCurrentMountRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.MountdomesticationReq, (ushort)CmdPet.MountdomesticationRes, PetMountdomesticationRes, CmdPetMountdomesticationRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.MountExpiredReq, (ushort)CmdPet.MountExpiredRes, PetMountExpiredRes, CmdPetMountExpiredRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.SetFollowPetReq, (ushort)CmdPet.SetFollowPetRes, PetSetFollowPetRes, CmdPetSetFollowPetRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.TempBagBatchAbandonPetReq, (ushort)CmdPet.TempBagBatchAbandonPetRes, OnPetTempBagBatchAbandonPetRes, CmdPetTempBagBatchAbandonPetRes.Parser);
            // 请求改造选择 
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.RemakeSelectReq, (ushort)CmdPet.RemakeSelectRes, OnPetRemakeSelectRes, CmdPetRemakeSelectRes.Parser);
            // 改造领悟技能 
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.RemakeLearnSkillReq, (ushort)CmdPet.RemakeLearnSkillRes, OnPetRemakeLearnSkillRes, CmdPetRemakeLearnSkillRes.Parser);
            // 领悟后选择学习技能 
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.RemakeSelectSkillReq, (ushort)CmdPet.RemakeSelectSkillRes, OnPetRemakeSelectSkillRes, CmdPetRemakeSelectSkillRes.Parser);
            // 重塑档位 
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.RemakeReGradeReq, (ushort)CmdPet.RemakeReGradeRes, OnPetRemakeReGradeRes, CmdPetRemakeReGradeRes.Parser);
            // 选择重塑选择 
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.RemakeSelectGradeReq, (ushort)CmdPet.RemakeSelectGradeRes, OnPetRemakeSelectGradeRes, CmdPetRemakeSelectGradeRes.Parser);
            // 选择重塑选择 
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.EatFruitReq, (ushort)CmdPet.EatFruitRes, OnPetEatFruitRes, CmdPetEatFruitRes.Parser);

            //骑宠契约-设置契约返回
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.ContractSetUpReq, (ushort)CmdPet.ContractSetUpRes, PetContractSetUpRes, CmdPetContractSetUpRes.Parser);
            //骑宠契约-取消契约返回
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.ContractCancleReq, (ushort)CmdPet.ContractCancleRes, PetContractCancleRes, CmdPetContractCancleRes.Parser);
            //骑宠契约-技能学习返回
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.RidingSkillLearnReq, (ushort)CmdPet.RidingSkillLearnRes, PetRidingSkillLearnRes, CmdPetRidingSkillLearnRes.Parser);
            //骑宠契约-技能遗忘返回
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.RidingSkillForgetReq, (ushort)CmdPet.RidingSkillForgetRes, PetRidingSkillForgetRes, CmdPetRidingSkillForgetRes.Parser);
            //骑宠契约-充能返回
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.RidingSkillEnergyChangeNtf, PetRidingSkillEnergyChangeNtf, CmdPetRidingSkillEnergyChangeNtf.Parser);
            //骑宠契约-契约位变化通知
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.ContractInfoUpdateNtf, PetContractInfoUpdateNtf, CmdPetContractInfoUpdateNtf.Parser);

            //宠物唯一宠物替换
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.UniquePetAddNtf, PetUniquePetAddNtf, CmdPetUniquePetAddNtf.Parser);

            //宠物装备炼化完成
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.SmeltPetEquipReq, (ushort)CmdItem.SmeltPetEquipRes, OnItemSmeltPetEquipRes, CmdItemSmeltPetEquipRes.Parser);
            //宠物装备替换 卸下 装备
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.FitPetEquipReq, (ushort)CmdItem.FitPetEquipRes, OnItemFitPetEquipRes, CmdItemFitPetEquipRes.Parser);
            //制作装备完成
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.BuildPetEquipReq, (ushort)CmdItem.BuildPetEquipRes, OnItemBuildPetEquipRes, CmdItemBuildPetEquipRes.Parser);

            //进阶
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.UpStageReq, (ushort)CmdPet.UpStageRes, OnPetUpStageRes, CmdPetUpStageRes.Parser);

            //封印
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.CatchSetReq, (ushort)CmdPet.CatchSetRes, OnPetCatchSetRes, CmdPetCatchSetRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.CatchSettingsReq, (ushort)CmdPet.CatchSettingsRes, OnPetCatchSettingsRes, CmdPetCatchSettingsRes.Parser);

            //宠物兑换保底
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.ExchangeSpecialGoldPetCountRes, OnPetExchangeSpecialGoldPetCountRes, CmdPetExchangeSpecialGoldPetCountRes.Parser);
            //宠物定向兑换
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.ExchangTargetGoldPetReq, (ushort)CmdPet.ExchangTargetGoldPetRes, PetExchangTargetGoldPetRes, CmdPetExchangTargetGoldPetRes.Parser);

            //宠物福袋特殊机制
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.FightGetLuckyBagNtf, PetFightGetLuckyBagNtf, CmdPetFightGetLuckyBagNtf.Parser);

            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.SoulAddRemakeCountReq, (ushort)CmdPet.SoulAddRemakeCountRes, OnPetSoulAddRemakeCountRes, CmdPetSoulAddRemakeCountRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.SoulActiveReq, (ushort)CmdPet.SoulActiveRes, OnPetSoulActiveRes, CmdPetSoulActiveRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.SoulAssembleBeadReq, (ushort)CmdPet.SoulAssembleBeadRes, OnPetSoulAssembleBeadRes, CmdPetSoulAssembleBeadRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.SoulBeadOperateReq, (ushort)CmdPet.SoulBeadOperateRes, OnPetSoulBeadOperateRes, CmdPetSoulBeadOperateRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.SoulBeadInitNtf, PetSoulBeadInitNtf, CmdPetSoulBeadInitNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.SoulBeadUpgradeInfoNtf, PetSoulBeadUpgradeInfoNtf, CmdPetSoulBeadUpgradeInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.LoveExpUpAllReq, (ushort)CmdPet.LoveExpUpAllRes, PetLoveExpUpAllRes, CmdPetLoveExpUpAllRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.ContractLevelUpReq, (ushort)CmdPet.ContractLevelUpRes, PetContractLevelUpRes, CmdPetContractLevelUpRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.ActiveFashionReq, (ushort)CmdPet.ActiveFashionRes, OnPetActiveFashionRes, CmdPetActiveFashionRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.ActiveFashionColorReq, (ushort)CmdPet.ActiveFashionColorRes, OnPetActiveFashionColorRes, CmdPetActiveFashionColorRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.DressOnOffFashionReq, (ushort)CmdPet.DressOnOffFashionRes, OnPetDressOnOffFashionRes, CmdPetDressOnOffFashionRes.Parser);

            //封印巡逻
            // 暂时处理
            {
                Sys_Fight.Instance.OnExitFight += OnExitFight;
                Sys_Fight.Instance.OnEnterFight += OnEnterFight;

                Sys_Task.Instance.eventEmitter.Handle<bool>(Sys_Task.EEvents.OnStartExecTask, OnStartExecTask, true);

                //Sys_FunctionOpen.Instance.eventEmitter.Handle<bool>(Sys_FunctionOpen.EEvents.StopOtherActions, OnFunctionOpen, true);

                // Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.TeamClear, OnTeamClear, true);
                // Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.BeCaptain, OnBeCaptain, true);
                Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.BeMember, OnBeMember, true);

                // 玩家手动操作中断自动任务
                Sys_Input.Instance.onTouchTerrain += OnTouchTerrain;
                Sys_Input.Instance.onLeftJoystick += OnLeftJoystick;
                Sys_Input.Instance.onRightJoystick += OnRightJoystick;
                //断线开始时
                Sys_Net.Instance.eventEmitter.Handle(Sys_Net.EEvents.OnReconnectStart, OnReconnectStart, true);
                //断线重连上时
                Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, true);
            }

            //方案切换
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.AllocPointPlanAddReq, (ushort)CmdPet.AllocPointPlanAddRes, OnAllocPointPlanAddRes, CmdPetAllocPointPlanAddRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.AllocPointPlanRenameReq, (ushort)CmdPet.AllocPointPlanRenameRes, OnAllocPointPlanRenameRes, CmdPetAllocPointPlanRenameRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.AllocPointPlanUseReq, (ushort)CmdPet.AllocPointPlanUseRes, OnAllocPointPlanUseRes, CmdPetAllocPointPlanUseRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.AllocEnhancePlanAddReq, (ushort)CmdPet.AllocEnhancePlanAddRes, OnAllocEnhancePlanAddRes, CmdPetAllocEnhancePlanAddRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.AllocEnhancePlanRenameReq, (ushort)CmdPet.AllocEnhancePlanRenameRes, OnAllocEnhancePlanRenameRes, CmdPetAllocEnhancePlanRenameRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.AllocEnhancePlanUseReq, (ushort)CmdPet.AllocEnhancePlanUseRes, OnAllocEnhancePlanUseRes, CmdPetAllocEnhancePlanUseRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.GetPointPlanAttrReq, (ushort)CmdPet.GetPointPlanAttrRes, OnGetPointPlanAttrRes, CmdPetGetPointPlanAttrRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.SinglePointPlanAttrUpdateNtf, OnSinglePointPlanAttrUpdateNtf, CmdPetSinglePointPlanAttrUpdateNtf.Parser);
            Sys_Plan.Instance.eventEmitter.Handle<uint, uint>(Sys_Plan.EEvents.ChangePlan, OnChangePlan, true);
            //首发出战宠物
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.FightPetSetListReq, (ushort)CmdPet.FightPetSetListRes, OnFightPetSetListRes, CmdPetFightPetSetListRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.FightPetSetReq, (ushort)CmdPet.FightPetSetRes, OnFightPetSetRes, CmdPetFightPetSetRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPet.LockPetReq, (ushort)CmdPet.LockPetRes, OnPetLockRes, CmdPetLockPetRes.Parser);
        }
    
       
        private void OnChangePlan(uint type,uint index)
        {
            if (type == (uint)Sys_Plan.EPlanType.PetAttribute)
            {
                if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
                {
                    if (GameCenter.mainFightPet != null && GameCenter.mainFightPet.battleUnit != null)
                    {
                        AllocPointPlanUseReq((uint)GameCenter.mainFightPet.battleUnit.PetId, index);
                    }
                }
                else
                {
                    AllocPointPlanUseReq(fightPet.GetUid(), index);
                }
            }
            else if (type == (uint)Sys_Plan.EPlanType.PetAttributeCorrect)
            {
                if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
                {
                    if (GameCenter.mainFightPet != null && GameCenter.mainFightPet.battleUnit != null)
                    {
                        AllocEnhancePlanUseReq((uint)GameCenter.mainFightPet.battleUnit.PetId, index);
                    }
                }
                else
                {
                    AllocEnhancePlanUseReq(fightPet.GetUid(), index);
                }
            }
        }

        public override void OnLogin()
        {
            isShowRemakeTips = false;
            isShowRemakeSkillTips = false;
            isShowRemakePerfectTips = false;
            isSmeltSkillTips = false;
            isSmeltFashionTips = false;
            petUiType = EPetUiType.UI_None;
            OnPetGetHandbookReq();
            SetDevelopeItemDataList();
            LoadMountScreeDB();
            uniquePetList.Clear();
        }

        public override void OnLogout()
        {
            isShowRemakeTips = false;
            isShowRemakeSkillTips = false;
            isShowRemakePerfectTips = false;
            isSmeltSkillTips = false;
            isSmeltFashionTips = false;
            showReExchange = false;
            petsList.Clear();
            fightPet.RemoveFightPetData();
            dailyTraining = null;
            devicesCount = 0;
            addexp = 0;
            bookNameData.Clear();
            allBookData.Clear();
            petTempPackUnits.Clear();
            storagePetData.Clear();
            isInitData = false;
            isBookDataInit = false;
            petUiType = EPetUiType.UI_None;
            timerSeal?.Cancel();
            clientStateId = Sys_Role.EClientState.None;
            remakePet = null;
            petAddPointRecDic.Clear();
            uniquePetList.Clear();
            petSetList.Clear();
            hasInit = false;
            PetFirstChoiceDestory();
            base.OnLogout();
        }       

        private void OnSysInfoNtf(NetMsg msg)
        {
            CmdPetSysInfoNtf dataNtf = NetMsgUtil.Deserialize<CmdPetSysInfoNtf>(CmdPetSysInfoNtf.Parser, msg);
            currentPetUid = dataNtf.CurrPetUid;
            bagNum = dataNtf.BagNum;
            costBagNum = dataNtf.CostbagNum;
            hasGetinitPet = dataNtf.HasGetInitPet;
            mountPetUid = dataNtf.CurMountUid;
            followPetUid = dataNtf.CurFollowPetUid;
            RidingEnergy = dataNtf.RidingEnergy;
            SkillCostResetTick = dataNtf.SkillCostResetTick;
            uniquePetList.Clear();
            uniquePetList.AddRange(dataNtf.UniquePetInfoIds);
            //直接覆盖不在处理缓存数据
            petsList.Clear();
            for (int i = 0; i < dataNtf.BagList.Count; i++)
            {
                petsList.Add(new ClientPet(dataNtf.BagList[i]));
            }
            petTempPackUnits.Clear();
            for (int i = 0; i < dataNtf.TempBagList.Count; i++)
            {
                petTempPackUnits.Add(new ClientPet(dataNtf.TempBagList[i]));
            }
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnTemplePetBagChange, petTempPackUnits.Count);
            fightPet.InitFightPetData(currentPetUid, GetSimplePetByUid(currentPetUid), dataNtf.CurrPetMaxHp, dataNtf.CurrPetHp, dataNtf.CurrPetMaxMp, dataNtf.CurrPetMp);
            ChangeFightPetIndex();
            OnGetPetInfoReq();
            //OnGetPetInfoReq(type: 1);
            CheckNextLimitPet();
            InitPetFashionInfos(dataNtf.Fashions);
            if (hasGetinitPet&& UIManager.IsOpen(EUIID.UI_ChoosePet))
            {
                UIManager.CloseUI(EUIID.UI_ChoosePet);
            }
        }

        /// <summary>
        /// 获取出战宠物数据
        /// </summary>
        /// <returns></returns>
        public PetUnit GetFightPet()
        {
            if (currentPetUid == 0)
                return null;
            for (int i = 0; i < petsList.Count; i++)
            {
                PetUnit petUnit = petsList[i].petUnit;
                if (petUnit.Uid == currentPetUid)
                    return petUnit;
            }
            return null;
        }

        /// <summary>
        /// 获取客户端本地宠物
        /// </summary>
        /// <param name="PetUid"></param>
        /// <returns></returns>
        public ClientPet GetFightPetClient(uint PetUid)
        {
            for (int i = 0; i < petsList.Count; i++)
            {
                ClientPet clientPet = petsList[i];
                if (clientPet.petUnit.Uid == PetUid)
                    return clientPet;
            }
            return null;
        }

        public SimplePet GetSimplePetByUid(uint PetUid)
        {
            for (int i = 0; i < petsList.Count; i++)
            {
                ClientPet clientPet = petsList[i];
                if (clientPet.petUnit.Uid == PetUid)
                    return clientPet.petUnit.SimpleInfo;
            }
            return null;
        }

        public int GetPetListIndexByUid(uint PetUid)
        {
            for (int i = 0; i < petsList.Count; i++)
            {
                if (petsList[i].petUnit.Uid == PetUid)
                    return i;
            }
            return -1;
        }

        //新添加宠物,临时存储队列
        private Queue<ClientPet> queuePets = new Queue<ClientPet>();
        private void OnAddPetNtf(NetMsg msg)
        {
            CmdPetAddPetNtf addNtf = NetMsgUtil.Deserialize<CmdPetAddPetNtf>(CmdPetAddPetNtf.Parser, msg);
            ClientPet clientpet = new ClientPet(addNtf.Pet);
            if (addNtf.Type == 1)
            {
                petsList.Add(clientpet);
                /*if (addNtf.Reason == (uint)PetActiveReason.Catch)
                {
                    Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.SealPet);
                }*/

                if (!Sys_Fight.Instance.IsFight())
                {
                    OnDisplayNewPet(clientpet, addNtf.Reason != (uint)LuckyDrawActiveReason.Draw && addNtf.Reason != (uint)LuckyDrawActiveReason.MountDraw, addNtf.Reason);
                }
                else
                {
                    //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009470, LanguageHelper.GetTextContent(clientpet.petData.name)));
                    queuePets.Enqueue(clientpet);
                }
            }
            else
            {
                petTempPackUnits.Add(clientpet);
                Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnTemplePetBagChange, petTempPackUnits.Count);
                if (!Sys_Fight.Instance.IsFight())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000942));
                }
                OnGetPetInfoReq(type: 1);
            }
            ResetLimitPetTimer(clientpet.petUnit.SimpleInfo.ExpiredTick, true);
        }

        private void OnDisplayNewPet(ClientPet pet, bool showUI = true, uint reason = 0)
        {
            eventEmitter.Trigger(EEvents.OnAddPet);
            if(showReExchange)
            {
                showReExchange = false;
                if (!Sys_Fight.Instance.IsFight())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021174));
                }
            }
            UIManager.CloseUI(EUIID.UI_ChoosePet);
            if(showUI)
            {
                EUIID eUIID = EUIID.UI_Pet_Get;
                if (reason == (uint)PetActiveReason.Catch)
                {
                    eUIID = EUIID.UI_Pet_GetCatch;
                }
                else if(reason == (uint)LuckyDrawActiveReason.Draw || reason == (uint)LuckyDrawActiveReason.MountDraw)
                {
                    eUIID = EUIID.UI_Pet_GetMix;
                }
                UIManager.OpenUI(eUIID, false, pet);
            }            
        }

        public void OnCheckGetNewPet()
        {
            if (queuePets.Count > 0)
            {
                OnDisplayNewPet(queuePets.Dequeue(), reason: (uint)PetActiveReason.Catch);
            }
        }


        public void OnInitSelectReq(uint petId)
        {
            CmdPetInitSelectReq cmdPetInitSelectReq = new CmdPetInitSelectReq();
            cmdPetInitSelectReq.PetId = petId;
            NetClient.Instance.SendMessage((ushort)CmdPet.InitSelectReq, cmdPetInitSelectReq);
        }
        /// <summary>
        /// 宠物上战修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        public void OnChangeStateReq(uint id)
        {
            CmdPetSetCurrentPetReq cmdPetChangeStateReq = new CmdPetSetCurrentPetReq();
            cmdPetChangeStateReq.Uid = id;
            NetClient.Instance.SendMessage((ushort)CmdPet.SetCurrentPetReq, cmdPetChangeStateReq);
        }

        private void ChangeStateRes(NetMsg msg)
        {
            CmdPetSetCurrentPetRes dataRes = NetMsgUtil.Deserialize<CmdPetSetCurrentPetRes>(CmdPetSetCurrentPetRes.Parser, msg);
            currentPetUid = dataRes.Currentuid;

            if (currentPetUid == 0)
            {
                fightPet.RemoveFightPetData();
                if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Fight)
                {
                    Sys_Plan.Instance.eventEmitter.Trigger<ulong, uint, uint>(Sys_Plan.EEvents.OnChangeFightPet, 0, 0, 0);
                }
            }
            else
            {
                ClientPet clientPet = GetFightPetClient(currentPetUid);
                PetUnit petUnit = clientPet?.petUnit;
                if (null != petUnit)
                {
                    fightPet.InitFightPetData(petUnit.Uid, petUnit.SimpleInfo, (long)clientPet.GetAttrValueByAttrId((int)EPkAttr.MaxHp), petUnit.PkAttr.CurHp, (long)clientPet.GetAttrValueByAttrId((int)EPkAttr.MaxMp), petUnit.PkAttr.CurMp);
                    if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Fight) //战斗外出战宠物切换
                    {
                        Sys_Plan.Instance.eventEmitter.Trigger<ulong, uint, uint>(Sys_Plan.EEvents.OnChangeFightPet, petUnit.Uid, petUnit.PetPointPlanData.CurrentPlanIndex, petUnit.EnhancePlansData.CurrentPlanIndex);
                    }
                }
                RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnPetAddPonit, null);        
            }
            //ChangeFightPetIndex();
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnChangeStatePet);
       
        }
        /// <summary>
        /// 宠物经验更新
        /// </summary>
        /// <param name="msg"></param>
        private void OnAddExpNtf(NetMsg msg)
        {
            CmdPetAddExpNtf dataNtf = NetMsgUtil.Deserialize<CmdPetAddExpNtf>(CmdPetAddExpNtf.Parser, msg);
            for (int j = 0; j < dataNtf.PetExpInfo.Count; j++)
            {
                CmdPetAddExpNtf.Types.PetAddExpUnit v = dataNtf.PetExpInfo[j];
                for (int i = 0; i < petsList.Count; i++)
                {
                    if (v.Uid == petsList[i].petUnit.Uid)
                    {
                        petsList[i].petUnit.SimpleInfo.Level = v.Level;
                        petsList[i].petUnit.SimpleInfo.Exp = v.Exp;
                        addexp = v.AddExp;
                        if (addexp > 0 && v.Exp >= uint.Parse(CSVParam.Instance.GetConfData(561).str_value))
                        {
                            CSVWorldLevel.Data openServiceDay = CSVWorldLevel.Instance.GetConfData(Sys_Role.Instance.openServiceDay);
                            if (openServiceDay != null)
                            {
                                string content = LanguageHelper.GetTextContent(101513);
                                Sys_Chat.Instance.PushMessage(ChatType.Person, null, content, Sys_Chat.EMessageProcess.None);
                            }
                        }
                        else if(addexp > 0)
                        {
                            string content = LanguageHelper.GetTextContent(10938, addexp.ToString());
                            Sys_Chat.Instance.PushMessage(ChatType.Person, null, content, Sys_Chat.EMessageProcess.None);
                        }
                    }
                    if (fightPet.IsSamePet(v.Uid))
                    {
                        fightPet.SetLevel(v.Level);
                        if (dataNtf.Reason == (uint)LuckyDrawActiveReason.BattleEnd)
                            addexpInBattleEnd = v.AddExp;
                    }
                }
            }
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnUpdateExp);
        }


        /// <summary>
        /// 宠物忠诚度更新
        /// </summary>
        /// <param name="msg"></param>
        private void OnLoyaltyNtf(NetMsg msg)
        {
            CmdPetLoyaltyNtf dataNtf = NetMsgUtil.Deserialize<CmdPetLoyaltyNtf>(CmdPetLoyaltyNtf.Parser, msg);
            for (int i = 0; i < petsList.Count; i++)
            {
                if (petsList[i].petUnit.Uid == dataNtf.Uid)
                {
                    petsList[i].petUnit.SimpleInfo.Loyalty = dataNtf.Loyalty;
                }
            }
            eventEmitter.Trigger(EEvents.OnUpdateLoyalty);
        }

        public void ExchangeGoldPetReq()
        {
            CmdPetExchangeGoldPetReq cmdPetExchangeGoldPetReq = new CmdPetExchangeGoldPetReq();
            NetClient.Instance.SendMessage((ushort)CmdPet.ExchangeGoldPetReq, cmdPetExchangeGoldPetReq);
        }

        public void OnExchangeGoldPetRes(NetMsg netMsg)
        {
           
        }

        public void PetReExchangeGoldPetReq(uint uid)
        {
            if (CheckIsLimitPet(uid))
                return;
            showReExchange = true;
            CmdPetReExchangeGoldPetReq cmdPetReExchangeGoldPetReq = new CmdPetReExchangeGoldPetReq();
            cmdPetReExchangeGoldPetReq.PetUid = uid;
            NetClient.Instance.SendMessage((ushort)CmdPet.ReExchangeGoldPetReq, cmdPetReExchangeGoldPetReq);
        }

        public void OnPetReExchangeGoldPetRes(NetMsg netMsg)
        {
            CmdPetReExchangeGoldPetRes res = NetMsgUtil.Deserialize<CmdPetReExchangeGoldPetRes>(CmdPetReExchangeGoldPetRes.Parser, netMsg);
            GoldPetExchangeNum = res.GoldPetExchangeNum;
        }

        private void OnRecoverNtf(NetMsg msg)
        {
            CmdPetRecoverNtf dataNtf = NetMsgUtil.Deserialize<CmdPetRecoverNtf>(CmdPetRecoverNtf.Parser, msg);
            for (int i = 0; i < dataNtf.PetAttrInfo.Count; i++)
            {
                CmdPetRecoverNtf.Types.PetRecorverAttr datantf = dataNtf.PetAttrInfo[i];
                for (int j = 0; j < petsList.Count; j++)
                {
                    ClientPet data = petsList[j];
                    if (data.petUnit.Uid == datantf.Uid)
                    {
                        data.pkAttrs[(int)EPkAttr.CurHp] = (long)datantf.Hp;
                        data.pkAttrs[(int)EPkAttr.CurMp] = (long)datantf.Mp;
                        if (data.petUnit.PkAttr != null)
                        {
                            data.petUnit.PkAttr.CurHp = (long)datantf.Hp;
                            data.petUnit.PkAttr.CurMp = (long)datantf.Mp;
                        }
                    }
                }
                if (fightPet.IsSamePet(datantf.Uid))
                {
                    fightPet.SetAttr_HpAndMp(0, (long)datantf.Hp, 0, (long)datantf.Mp);
                }
            }
            eventEmitter.Trigger(EEvents.OnPetHpMpUpdate);
        }
        
        public void ExchangeSpecialGoldPetReq()
        {
            CmdPetExchangeSpecialGoldPetReq req=new CmdPetExchangeSpecialGoldPetReq();
            NetClient.Instance.SendMessage((ushort)CmdPet.ExchangeSpecialGoldPetReq, req);
        }

        public uint specialPetExCount;
        
        private void OnPetExchangeSpecialGoldPetCountRes(NetMsg netMsg)
        {
            CmdPetExchangeSpecialGoldPetCountRes res=NetMsgUtil.Deserialize<CmdPetExchangeSpecialGoldPetCountRes>(CmdPetExchangeSpecialGoldPetCountRes.Parser, netMsg);
            specialPetExCount = res.Count;
            eventEmitter.Trigger(EEvents.OnSpecialPetExDraw);
        }

        public void OnPetLockReq(uint uId, bool isLock)
        {
            CmdPetLockPetReq req = new CmdPetLockPetReq();
            req.Uuid = uId;
            req.Islocked = isLock;
            NetClient.Instance.SendMessage((ushort)CmdPet.LockPetReq, req);
        }

        private void OnPetLockRes(NetMsg msg)
        {
            CmdPetLockPetRes res = NetMsgUtil.Deserialize<CmdPetLockPetRes>(CmdPetLockPetRes.Parser, msg);
            ClientPet pet = GetPetByUId(res.Uuid);
            if (pet != null)
            {
                uint lanId = res.Islocked ? 15240u : 15241u;
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(lanId));
                
                pet.petUnit.Islocked = res.Islocked;
                eventEmitter.Trigger(EEvents.OnPetLockChange);
            }
            else
            {
                DebugUtil.LogError("找不到宠物 : " + res.Uuid);
            }
        }
    }
}
