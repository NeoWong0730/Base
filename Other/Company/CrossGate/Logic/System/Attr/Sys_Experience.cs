using Packet;
using Logic.Core;
using System.Collections.Generic;
using Net;
using Lib.Core;
using Table;

namespace Logic
{
    public partial class Sys_Experience : SystemModuleBase<Sys_Experience>
    {
        /// <summary>
        /// 家族历练方案数据///
        /// </summary>
        public class ExperiencePlanData
        {
            /// <summary>
            /// 方案名///
            /// </summary>
            public string Name
            {
                get;
                private set;
            }

            /// <summary>
            /// 已使用点数///
            /// </summary>
            public uint UsePoint
            {
                get;
                set;
            }

            /// <summary>
            /// 历练属性信息///
            /// </summary>
            public Dictionary<uint, ExperienceInfo> infoDic;

            /// <summary>
            /// 剩余点数///
            /// </summary>
            public uint LeftPoint
            {
                get
                {
                    return Instance.totalPoint - UsePoint;
                }
            }

            public ExperiencePlanData(uint index)
            {
                infoDic = new Dictionary<uint, ExperienceInfo>();
                Name = LanguageHelper.GetTextContent(10013502, (index + 1).ToString());
            }

            /// <summary>
            /// 设置方案名///
            /// </summary>
            /// <param name="_name"></param>
            public void SetName(string _name)
            {
                Name = _name;
            }
        }

        /// <summary>
        /// 历练等级///
        /// </summary>
        public uint exPerienceLevel;

        /// <summary>
        /// 总点数///
        /// </summary>
        public uint totalPoint;

        /// <summary>
        /// 当前选择的方案///
        /// </summary>
        public int currentIndex;

        public List<ExperiencePlanData> experiencePlanDatas = new List<ExperiencePlanData>();
        public Dictionary<uint, CmdExperienceAttrAddReq.Types.AttrAddInfo> addinfoDic = new Dictionary<uint, CmdExperienceAttrAddReq.Types.AttrAddInfo>();
        public Dictionary<uint, uint> index2Rank = new Dictionary<uint, uint>();
        public Dictionary<uint, uint> rankAddpoints = new Dictionary<uint, uint>();

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnExperienceUpgrade,    //功勋升级
            OnUpdateLeftPoint,    //更新剩余点数
            OnUpdateExperienceInfo,    //功勋信息更新
            OnAddRankInPreAdd,    //预加点时层级增加

            OnAddNewPlan,
            OnChangePlan,
            OnChangePlanName,
        }


        public override void Init()
        {
            //EventDispatcher.Instance.AddEventListener((ushort)CmdExperience.InfoReq, (ushort)CmdExperience.InfoAck, OnInfoAck, CmdExperienceInfoAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdExperience.InfoReq, (ushort)CmdExperience.InfoNewAck, OnInfoNewAck, CmdExperienceInfoNewAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdExperience.UpgradeReq, (ushort)CmdExperience.UpgradeAck, OnUpgradeAck, CmdExperienceUpgradeAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdExperience.ResetReq, (ushort)CmdExperience.ResetAck, OnResetAck, CmdExperienceResetAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdExperience.AttrAddReq, (ushort)CmdExperience.AttrAddAck, OnAttrAddAck, CmdExperienceAttrAddAck.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdExperience.LevelNtf, OnLevelNtf, CmdExperienceLevelNtf.Parser);

            EventDispatcher.Instance.AddEventListener((ushort)CmdExperience.AddNewPlanReq, (ushort)CmdExperience.AddNewPlanRes, OnAddNewPlanRes, CmdExperienceAddNewPlanRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdExperience.PlanRenameReq, (ushort)CmdExperience.PlanRenameRes, OnPlanRenameRes, CmdExperiencePlanRenameRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdExperience.PlanChangeReq, (ushort)CmdExperience.PlanChangeRes, OnPlanChangeRes, CmdExperiencePlanChangeRes.Parser);

            Sys_Plan.Instance.eventEmitter.Handle<uint, uint>(Sys_Plan.EEvents.ChangePlan, (playType, index) => {
                if (playType == (uint)Sys_Plan.EPlanType.Family)
                {
                    ReqChangePlan(index);
                }
            }, true);
        }

        public override void OnLogin()
        {
            InfoReq();
        }

        public override void OnLogout()
        {
            experiencePlanDatas.Clear();
        }

        #region Call

        public void InfoReq()
        {
            CmdExperienceInfoReq req = new CmdExperienceInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdExperience.InfoReq, req);
        }

        public void UpgradeReq()
        {
            CmdExperienceUpgradeReq req = new CmdExperienceUpgradeReq();
            NetClient.Instance.SendMessage((ushort)CmdExperience.UpgradeReq, req);
        }

        public void ResetReq(uint index)
        {
            CmdExperienceResetReq req = new CmdExperienceResetReq();
            req.PlanIndex = index;

            NetClient.Instance.SendMessage((ushort)CmdExperience.ResetReq, req);
        }

        public void AttrAddReq(Dictionary<uint, CmdExperienceAttrAddReq.Types.AttrAddInfo> addinfodic, uint index)
        {
            CmdExperienceAttrAddReq req = new CmdExperienceAttrAddReq();
            foreach (var item in addinfodic)
            {
                req.Info.Add(item.Value);
            }
            req.PlanIndex = index;

            NetClient.Instance.SendMessage((ushort)CmdExperience.AttrAddReq, req);
        }

        public void ReqAddNewPlan()
        {
            CmdExperienceAddNewPlanReq req = new CmdExperienceAddNewPlanReq();

            NetClient.Instance.SendMessage((ushort)CmdExperience.AddNewPlanReq, req);
        }

        public void ReqPlanRename(string name, uint index)
        {
            CmdExperiencePlanRenameReq req = new CmdExperiencePlanRenameReq();

            req.PlanIndex = index;
            req.NewName = FrameworkTool.ConvertToGoogleByteString(name);

            NetClient.Instance.SendMessage((ushort)CmdExperience.PlanRenameReq, req);
        }

        public void ReqChangePlan(uint index)
        {
            CmdExperiencePlanChangeReq req = new CmdExperiencePlanChangeReq();
            req.PlanIndex = index;

            NetClient.Instance.SendMessage((ushort)CmdExperience.PlanChangeReq, req);
        }

        #endregion

        #region Callback

        /// <summary>
        /// 历练数据初始化///
        /// </summary>
        /// <param name="msg"></param>
        void OnInfoNewAck(NetMsg msg)
        {
            CmdExperienceInfoNewAck ack = NetMsgUtil.Deserialize<CmdExperienceInfoNewAck>(CmdExperienceInfoNewAck.Parser, msg);
            if (ack != null)
            {
                exPerienceLevel = ack.Level;
                totalPoint = ack.TotalCount;
                currentIndex = (int)ack.CurrentPlanIndex;
                experiencePlanDatas.Clear();
                for (int index = 0, len = ack.Plans.Count; index < len; index++)
                {
                    ExperiencePlanData experiencePlanData = new ExperiencePlanData((uint)index);
                    experiencePlanData.UsePoint = ack.Plans[index].UseCount;
                    if (!string.IsNullOrEmpty(ack.Plans[index].Name.ToStringUtf8()))
                    {
                        experiencePlanData.SetName(ack.Plans[index].Name.ToStringUtf8());
                    }
                    for (int index2 = 0, len2 = ack.Plans[index].Info.Count; index2 < len2; index2++)
                    {
                        if (ack.Plans[index].Info[index2].IndexId != 0)
                            experiencePlanData.infoDic[ack.Plans[index].Info[index2].IndexId] = ack.Plans[index].Info[index2];
                    }

                    experiencePlanDatas.Add(experiencePlanData);
                }

                eventEmitter.Trigger(EEvents.OnUpdateExperienceInfo);
            }
        }

        /// <summary>
        /// 历练等级同步
        /// </summary>
        /// <param name="msg"></param>
        void OnLevelNtf(NetMsg msg)
        {
            CmdExperienceLevelNtf ntf = NetMsgUtil.Deserialize<CmdExperienceLevelNtf>(CmdExperienceLevelNtf.Parser, msg);
            if (ntf != null)
            {
                exPerienceLevel = ntf.Level;
            }
        }

        /// <summary>
        /// 历练升级返回///
        /// </summary>
        /// <param name="msg"></param>
        void OnUpgradeAck(NetMsg msg)
        {
            CmdExperienceUpgradeAck ack = NetMsgUtil.Deserialize<CmdExperienceUpgradeAck>(CmdExperienceUpgradeAck.Parser, msg);
            if (ack != null)
            {
                exPerienceLevel = ack.Level;
                totalPoint = ack.TotalCount;
                eventEmitter.Trigger(EEvents.OnExperienceUpgrade);
            }
        }

        /// <summary>
        /// 历练重置返回///
        /// </summary>
        /// <param name="msg"></param>
        void OnResetAck(NetMsg msg)
        {
            CmdExperienceResetAck ack = NetMsgUtil.Deserialize<CmdExperienceResetAck>(CmdExperienceResetAck.Parser, msg);
            if (ack != null)
            {
                experiencePlanDatas[(int)ack.PlanIndex].UsePoint = ack.UseCount;

                foreach (var item in experiencePlanDatas[(int)ack.PlanIndex].infoDic)
                {
                    item.Value.AddPoint = 0;
                    item.Value.Level = 0;
                    item.Value.Point = 0;
                }

                eventEmitter.Trigger(EEvents.OnUpdateExperienceInfo);
            }
        }

        void OnAttrAddAck(NetMsg msg)
        {
            CmdExperienceAttrAddAck ack = NetMsgUtil.Deserialize<CmdExperienceAttrAddAck>(CmdExperienceAttrAddAck.Parser, msg);
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Distributional);

            if (ack != null)
            {
                ExperiencePlanData experiencePlanData = new ExperiencePlanData((uint)ack.PlanIndex);
                experiencePlanData.UsePoint = ack.Plan.UseCount;
                if (!string.IsNullOrEmpty(ack.Plan.Name.ToStringUtf8()))
                {
                    experiencePlanData.SetName(ack.Plan.Name.ToStringUtf8());
                }
                for (int index2 = 0, len2 = ack.Plan.Info.Count; index2 < len2; index2++)
                {
                    if (ack.Plan.Info[index2].IndexId != 0)
                        experiencePlanData.infoDic[ack.Plan.Info[index2].IndexId] = ack.Plan.Info[index2];
                }
                experiencePlanDatas[(int)ack.PlanIndex] = experiencePlanData;

                eventEmitter.Trigger(EEvents.OnUpdateExperienceInfo);
            }
        }

        /// <summary>
        /// 添加一个新的历练方案///
        /// </summary>
        /// <param name="msg"></param>
        void OnAddNewPlanRes(NetMsg msg)
        {
            CmdExperienceAddNewPlanRes res = NetMsgUtil.Deserialize<CmdExperienceAddNewPlanRes>(CmdExperienceAddNewPlanRes.Parser, msg);
            if (res != null)
            {
                ExperiencePlanData experiencePlanData = new ExperiencePlanData(res.PlanIndex);
                if (!string.IsNullOrEmpty(res.Plan.Name.ToStringUtf8()))
                {
                    experiencePlanData.SetName(res.Plan.Name.ToStringUtf8());
                }
                experiencePlanData.UsePoint = res.Plan.UseCount;
                for (int index = 0, len = res.Plan.Info.Count; index < len; index++)
                {
                    if (res.Plan.Info[index].IndexId != 0)
                        experiencePlanData.infoDic[res.Plan.Info[index].IndexId] = res.Plan.Info[index];
                }

                experiencePlanDatas.Add(experiencePlanData);
                eventEmitter.Trigger(EEvents.OnAddNewPlan);
                Sys_Plan.Instance.eventEmitter.Trigger<uint, uint>(Sys_Plan.EEvents.AddNewPlan, (uint)Sys_Plan.EPlanType.Family, res.PlanIndex);
            }
        }

        /// <summary>
        ///  改变方案名///
        /// </summary>
        /// <param name="msg"></param>
        void OnPlanRenameRes(NetMsg msg)
        {
            CmdExperiencePlanRenameRes res = NetMsgUtil.Deserialize<CmdExperiencePlanRenameRes>(CmdExperiencePlanRenameRes.Parser, msg);
            if (res != null)
            {
                experiencePlanDatas[(int)res.PlanIndex].SetName(res.NewName.ToStringUtf8());

                eventEmitter.Trigger<int>(EEvents.OnChangePlanName, (int)res.PlanIndex);
                Sys_Plan.Instance.eventEmitter.Trigger<uint, uint, string>(Sys_Plan.EEvents.ChangePlanName, (uint)Sys_Plan.EPlanType.Family, res.PlanIndex, res.NewName.ToStringUtf8());
            }
        }

        /// <summary>
        /// 切换方案///
        /// </summary>
        /// <param name="msg"></param>
        void OnPlanChangeRes(NetMsg msg)
        {
            CmdExperiencePlanChangeRes res = NetMsgUtil.Deserialize<CmdExperiencePlanChangeRes>(CmdExperiencePlanChangeRes.Parser, msg);
            if (res != null)
            {
                currentIndex = (int)res.PlanIndex;

                eventEmitter.Trigger(EEvents.OnChangePlan);
                Sys_Plan.Instance.eventEmitter.Trigger<uint, uint>(Sys_Plan.EEvents.OnChangePlanSuccess, (uint)Sys_Plan.EPlanType.Family, (uint)currentIndex);
            }
        }

        #endregion            

        public uint GetMaxLv(uint breakthroughLv)
        {
            foreach (var data in CSVExperienceLevel.Instance.GetAll())
            {
                if (Sys_Role.Instance.Role.Level < data.need_level || breakthroughLv < data.need_technology)
                {
                    return data.id - 1;
                }
            }
            int index = CSVExperienceLevel.Instance.GetAll().Count - 1;
            return CSVExperienceLevel.Instance.GetByIndex(index).id;
        }   
    }

}

