using Table;
using System.Collections.Generic;
using Logic.Core;

namespace Logic
{
    //public class SuperHeroSkillComponent : HeroSkillComponent
    //{
    //    protected override void Init()
    //    {
    //        ///默认添加防御,普攻///
    //        holdingActiveSkills.Add(Constants.DEFFENSESKILLID, new SkillData()
    //        {
    //            CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(Constants.DEFFENSEID),
    //        });
    //
    //        holdingActiveSkills.Add(Constants.NORMALATTACKSKILLID, new SkillData()
    //        {
    //            CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(Constants.NEARNORMALATTACKID),
    //        });
    //    }
    //}

    public class HeroSkillComponent : SkillComponent
    {
        WeaponComponent weaponComponent;
        CareerComponent careerComponent;
        public bool IsPlayer = true;
        public bool IsSuperHero = false;

        protected override void OnDispose()
        {
            weaponComponent = null;
            careerComponent = null;
            IsPlayer = true;
            IsSuperHero = false;

            base.OnDispose();
        }

        protected override void Register()
        {
            IWeaponComponent weapon = actor as IWeaponComponent;
            ICareerComponent career = actor as ICareerComponent;

            if (weapon != null)
            {
                weaponComponent = weapon.GetWeaponComponent();
                careerComponent = career.GetCareerComponent();
            }            

            if (weaponComponent != null)
            {
                weaponComponent.ChangeWeaponAction += OnChangeWeapon;
            }

            if (careerComponent != null)
            {
                careerComponent.ChangeCareerAction += OnChangeCareer;
            }
        }

        protected override void UnRegister()
        {
            if (weaponComponent != null)
            {
                weaponComponent.ChangeWeaponAction -= OnChangeWeapon;
            }

            if (careerComponent != null)
            {
                careerComponent.ChangeCareerAction -= OnChangeCareer;
            }

            weaponComponent = null;
            careerComponent = null;
        }

        protected override void Init()
        {
            if (IsSuperHero)
            {
                ///默认添加防御,普攻///
                holdingActiveSkills.Add(Constants.DEFFENSESKILLID, new SkillData()
                {
                    CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(Constants.DEFFENSEID),
                });

                holdingActiveSkills.Add(Constants.NORMALATTACKSKILLID, new SkillData()
                {
                    CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(Constants.NEARNORMALATTACKID),
                });
            }
            else
            {
                ResetHoldingSkills();
            }            
        }

        void ResetHoldingSkills()
        {
            holdingActiveSkills.Clear();

            ///默认添加防御,逃跑技能,收回召唤宠物技能///
            holdingActiveSkills.Add(Constants.DEFFENSESKILLID, new SkillData()
            {
                CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(Constants.DEFFENSEID),
            });
            holdingActiveSkills.Add(Constants.ESCAPESKILLID, new SkillData()
            {
                CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(Constants.ESCAPEID),
            });
            holdingActiveSkills.Add(Constants.PETINSKILLID, new SkillData()
            {
                CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(Constants.PETINID),
            });
            holdingActiveSkills.Add(Constants.PETOUTSKILLID, new SkillData()
            {
                CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(Constants.PETOUTID),
            });

            if (weaponComponent != null)
            {
                CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(weaponComponent.CurWeaponID);
                if (cSVEquipmentData != null)
                {
                    holdingActiveSkills.Add(Constants.NORMALATTACKSKILLID, new SkillData()
                    {
                        CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(cSVEquipmentData.active_skillid),
                    });
                }
            }
        }

        public void InitActiveSkill()
        {
            InitCareerSkill();
            InitEnergySkill();
            InitTalentSkill();
        }

        public void InitTalentSkill()
        {
            if (IsPlayer)
            {
                InitPlayerTalentSkill();
            }
        }

        public void InitEnergySkill()
        {
            if (IsPlayer)
            {
                InitPlayerEnergySkill();
            }
        }

        public void InitCareerSkill()
        {
            if (IsPlayer)
            {
                InitPlayerCareerSkill();
            }
            else
            {
                InitParnterCareerSkill();
            }
        }

        bool WeaponCheck(uint skillID, uint weaponId=0)
        {
            if (weaponComponent == null)
                return true;

            CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillID);
            if (cSVActiveSkillInfoData != null)
            {
                if (cSVActiveSkillInfoData.require_weapon_type == null)
                    return true;
                CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(weaponId == 0 ? weaponComponent.CurWeaponID : weaponId);
                if (cSVActiveSkillInfoData.require_weapon_type.Contains(cSVEquipmentData.equipment_type))
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        void InitPlayerTalentSkill() {
            var sch = Sys_Talent.Instance.usingScheme;
            if (sch == null) {
                return;
            }

            foreach (var kvp in sch.talents) {
                var skill = kvp.Value.CurrentActiveSkill;
                if (skill != null) {
                    SkillData skillData = new SkillData();
                    skillData.CSVActiveSkillInfoData = skill;
                    skillData.eActiveSkillType = EActiveSkillType.TalentSkill;
                    uint index = skill.id / 1000;
                    if (holdingActiveSkills.ContainsKey(index))
                    {
                       if(skill.id > holdingActiveSkills[index].CSVActiveSkillInfoData.id)
                        {
                            holdingActiveSkills[index] = skillData;
                        }
                    }
                    else
                    {
                        holdingActiveSkills[index] = skillData;
                    }        
                }
                
            }
        }

        void InitPlayerEnergySkill()
        {
            foreach (var skillInfo in Sys_StoneSkill.Instance.serverDataDic)
            {
                uint skillId = Sys_StoneSkill.Instance.GetPowerStoneActiveSkill(skillInfo.Value.powerStoneUnit.Id, skillInfo.Value.powerStoneUnit.Level);
                if (skillId != 0)
                { 
                    SkillData skillData = new SkillData();
                    skillData.CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                    skillData.eActiveSkillType = EActiveSkillType.EnergySkill;
                    uint index = skillId / 1000;
                    holdingActiveSkills[index] = skillData;                            
                }
            }
        }

        void InitPlayerCareerSkill()
        {
            foreach (Sys_Skill.SkillInfo skillInfo in Sys_Skill.Instance.bestSkillInfos.Values)
            {
                if (skillInfo.Level > 0)
                {
                    SkillData skillData = new SkillData();
                    
                    skillData.Available = WeaponCheck(skillInfo.CurInfoID);
                    skillData.eSkillType = skillInfo.ESkillType;
                    if (skillData.eSkillType == Sys_Skill.ESkillType.Active)
                    {
                        skillData.eActiveSkillType = EActiveSkillType.CarrerSkill;
                        skillData.CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                        holdingActiveSkills[skillInfo.SkillID] = skillData;
                    }
                    else if (skillData.eSkillType == Sys_Skill.ESkillType.Passive)
                    {
                        skillData.CSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                        holdingPassiveSkills[skillInfo.SkillID] = skillData;                        
                    }
                }
            }

            foreach (Sys_Skill.SkillInfo skillInfo in Sys_Skill.Instance.commonSkillInfos.Values)
            {
                if (skillInfo.Level > 0)
                {
                    SkillData skillData = new SkillData();
                    
                    skillData.Available = true;
                    skillData.eSkillType = skillInfo.ESkillType;
                    if (skillData.eSkillType == Sys_Skill.ESkillType.Active)
                    {
                        skillData.eActiveSkillType = EActiveSkillType.CarrerSkill;
                        skillData.CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                        holdingActiveSkills[skillInfo.SkillID] = skillData;
                    }
                    else if (skillData.eSkillType == Sys_Skill.ESkillType.Passive)
                    {
                        skillData.CSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                        holdingPassiveSkills[skillInfo.SkillID] = skillData;                     
                    }
                }
            }
        }
        
        void InitParnterCareerSkill()
        {
            if (careerComponent != null)
            {
                CSVCareer.Data careerData = CSVCareer.Instance.GetConfData((uint)careerComponent.CurCarrerType);
                if (careerData != null)
                {
                    foreach (var skillArray in careerData.battle_skill)
                    {
                        holdingActiveSkills[skillArray[0] / 1000] = new SkillData()
                        {
                            CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillArray[0]),
                        };
                    }
                }
            }
        }

        /// <summary>
        /// 换了武器///
        /// 后续还要更新武器限制技能///
        /// </summary>
        /// <param name="oldWeaponID"></param>
        /// <param name="newWeaponID"></param>
        void OnChangeWeapon(uint oldWeaponID, uint newWeaponID)
        {
            ResetHoldingSkills();
            InitCareerSkill();

            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(newWeaponID);
            if (cSVEquipmentData != null)
            {
                holdingActiveSkills[Constants.NORMALATTACKSKILLID] = new SkillData()
                {
                    CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(cSVEquipmentData.active_skillid),
                };
            }
        }

        /// <summary>
        /// 转职, 更新职业技能///
        /// </summary>
        /// <param name="oldCareerType"></param>
        /// <param name="newCareerType"></param>
        void OnChangeCareer(ECareerType oldCareerType, ECareerType newCareerType)
        {
            ResetHoldingSkills();
            InitCareerSkill();
        }

        /// <summary>
        /// 获取逃跑技能///
        /// </summary>
        /// <returns></returns>
        public SkillData GetEscapeSkill()
        {
            return holdingActiveSkills[Constants.ESCAPESKILLID];
        }

        /// <summary>
        /// 获取PetIn技能///
        /// </summary>
        /// <returns></returns>
        public SkillData GetPetInSkill()
        {
            return holdingActiveSkills[Constants.PETINSKILLID];
        }

        /// <summary>
        /// 获取PetOut技能///
        /// </summary>
        /// <returns></returns>
        public SkillData GetPetOutSkill()
        {
            return holdingActiveSkills[Constants.PETOUTSKILLID];
        }
    }

    public class FightPetSkillComponent : SkillComponent
    {
        protected override void Init()
        {
            ///默认添加防御,普攻///
            holdingActiveSkills.Add(Constants.DEFFENSESKILLID, new SkillData()
            {
                CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(Constants.DEFFENSEID),
            });

            holdingActiveSkills.Add(Constants.NORMALATTACKSKILLID, new SkillData()
            {
                CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(Constants.NEARNORMALATTACKID),
            });

            FightPet fightPet = actor as FightPet;
           
            ClientPet clientfightpet = Sys_Pet.Instance.GetFightPetClient((uint)fightPet.battleUnit.PetId);
            if (fightPet.battleUnit.RoleId == Sys_Role.Instance.RoleId)
            {
                if (clientfightpet == null)
                    return;
                List<uint> petSkillList = clientfightpet.GetPetAllSkillList();
                for (int i = 0; i < petSkillList.Count; i++)
                {
                    uint id = petSkillList[i];
                    uint index = id / 1000;
                    if (CSVActiveSkillInfo.Instance.ContainsKey(id))
                    {
                        holdingActiveSkills[index] = new SkillData()
                        {
                            CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(id),
                        };
                    }
                    else
                    {
                        holdingPassiveSkills[index] = new SkillData()
                        {
                            CSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(id),
                        };
                    }
                }
            }
        }
    }

    public class MonsterSkillComponent : SkillComponent
    {
        protected override void Init()
        {
            ///默认添加防御,普攻///
            holdingActiveSkills.Add(Constants.DEFFENSESKILLID, new SkillData()
            {
                CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(Constants.DEFFENSEID),
            });

            holdingActiveSkills.Add(Constants.NORMALATTACKSKILLID, new SkillData()
            {
                CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(Constants.NEARNORMALATTACKID),
            });
        }
    }

    public enum EActiveSkillType
    {
        CarrerSkill,
        EnergySkill,
        TalentSkill,
    }

    public abstract class SkillComponent : Logic.Core.Component
    {
        public class SkillData
        {
            public CSVActiveSkillInfo.Data CSVActiveSkillInfoData;
            public CSVPassiveSkillInfo.Data CSVPassiveSkillInfoData;

            public bool Available
            {
                get;
                set;
            } = true;

            public EActiveSkillType eActiveSkillType
            {
                get;
                set;
            } = EActiveSkillType.CarrerSkill;

            public Sys_Skill.ESkillType eSkillType
            {
                get;
                set;
            }
        }

        protected Dictionary<uint, SkillData> holdingActiveSkills = new Dictionary<uint, SkillData>();
        protected Dictionary<uint, SkillData> holdingPassiveSkills = new Dictionary<uint, SkillData>();

        protected override void OnConstruct()
        {
            base.OnConstruct();
            //Register();
            //Init();
        }

        protected override void OnDispose()
        {
            UnRegister();
            holdingActiveSkills.Clear();
            holdingPassiveSkills.Clear();
            base.OnDispose();
        }

        public void DoInit()
        {
            Register();
            Init();
        }

        protected virtual void Register() { }

        protected virtual void UnRegister() { }

        protected abstract void Init();
       
        /// <summary>
        /// 获取所有装配的主动技能///
        /// </summary>
        /// <returns></returns>
        public Dictionary<uint, SkillData> GetHoldingActiveSkills()
        {
            return holdingActiveSkills;
        }

        /// <summary>
        /// 获取排好序的除了普攻等的主动技能///
        /// </summary>
        /// <returns></returns>
        public List<SkillData> GetHoldingActiveSkillsExt(uint weaponId)
        {
            List<SkillData> skillDatas = new List<SkillData>();

            foreach (var skillData in holdingActiveSkills.Values)
            {
                skillData.Available = WeaponCheck(skillData.CSVActiveSkillInfoData.id, weaponId);
                if (skillData.CSVActiveSkillInfoData.id > 100000 && skillData.eActiveSkillType != EActiveSkillType.EnergySkill)
                {
                    skillDatas.Add(skillData);              
                }
            }

            skillDatas.Sort((dataA, dataB) =>
            {
                if (dataA.CSVActiveSkillInfoData.seqencing > dataB.CSVActiveSkillInfoData.seqencing)
                    return -1;
                else if (dataA.CSVActiveSkillInfoData.seqencing < dataB.CSVActiveSkillInfoData.seqencing)
                    return 1;
                else
                {
                    if (dataA.CSVActiveSkillInfoData.skill_id > dataB.CSVActiveSkillInfoData.skill_id)
                        return -1;
                    else
                        return 1;
                }
            });
       
            return skillDatas;
        }

        bool WeaponCheck(uint skillID, uint weaponId)
        {
            CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillID);
            if (cSVActiveSkillInfoData != null)
            {
                if (cSVActiveSkillInfoData.require_weapon_type == null)
                    return true;             
                CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(weaponId);
                if (cSVEquipmentData!=null && cSVActiveSkillInfoData.require_weapon_type.Contains(cSVEquipmentData.equipment_type))
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public Dictionary<uint, SkillData> GetHoldingPassiveSkills()
        {
            return holdingPassiveSkills;
        }

        /// <summary>
        /// 获取防御技能///
        /// </summary>
        /// <returns></returns>
        public SkillData GetDefenseSkill()
        {
            return holdingActiveSkills[Constants.DEFFENSESKILLID];
        }

        /// <summary>
        /// 获取普攻技能///
        /// </summary>
        /// <returns></returns>
        public SkillData GetNormalAttackSkill()
        {
            return holdingActiveSkills[Constants.NORMALATTACKSKILLID];
        }
    }
}
